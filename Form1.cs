using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordAutoChat
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly Random random = new Random();
        private CancellationTokenSource _cts;
        private ClientWebSocket _ws;

        private Dictionary<string, string> _userChannels = new Dictionary<string, string>();
        private string _selectedChannelId = "";
        private string _myUserId = "";

        public Form1()
        {
            InitializeComponent();
            numMinDelay.Value = 10;
            numMaxDelay.Value = 30;
            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => AppendLog(message))); return; }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        // --- WebSocket 監聽登入 ---
        private async void btnConnectWS_Click(object sender, EventArgs e)
        {
            string token = txtTokens.Lines.FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(token)) { MessageBox.Show("請輸入 Token！"); return; }

            _ws = new ClientWebSocket();
            try
            {
                AppendLog("連接中...");
                await _ws.ConnectAsync(new Uri("wss://gateway.discord.gg/?v=9&encoding=json"), CancellationToken.None);

                var auth = new
                {
                    op = 2,
                    d = new
                    {
                        token = token,
                        properties = new
                        {
                            os = "Windows",
                            browser = "Chrome", // 偽裝成 Chrome
                            device = "",
                            system_locale = "zh-TW",
                            browser_user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                            browser_version = "120.0.0.0"
                        },
                        intents = 32767
                    }
                };
                await SendJsonAsync(JsonConvert.SerializeObject(auth));

                _ = Task.Run(() => ReceiveLoop(token));
                _ = Task.Run(() => HeartbeatLoop());

                btnConnectWS.Enabled = false;
                lblStatus.Text = "狀態：已連線監聽";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                AppendLog("私訊監聽啟動成功。");
            }
            catch (Exception ex) { AppendLog($"連線失敗: {ex.Message}"); }
        }
        private DateTime _lastForwardTime = DateTime.MinValue;
        private async Task ReceiveLoop(string currentToken)
        {

            var buffer = new byte[1024 * 16]; // 16KB 緩衝區
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result;
                    var ms = new System.IO.MemoryStream();

                    // 1. 確保完整接收大封包 (如 READY)
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    string rawJson = Encoding.UTF8.GetString(ms.ToArray());
                    var data = JObject.Parse(rawJson);
                    string t = data["t"]?.ToString();

                    // 2. 登入成功事件 (獲取自己的 ID)
                    if (!string.IsNullOrEmpty(t) && t.Equals("READY", StringComparison.OrdinalIgnoreCase))
                    {
                        _myUserId = data["d"]["user"]["id"].ToString();
                        AppendLog($"[系統] 登入成功！您的 ID 是: {_myUserId}");
                    }

                    // 3. 接收訊息事件
                    if (!string.IsNullOrEmpty(t) && t.Equals("MESSAGE_CREATE", StringComparison.OrdinalIgnoreCase))
                    {
                        var d = data["d"];
                        string authorId = d["author"]["id"].ToString();
                        string authorName = d["author"]["username"].ToString();
                        string content = d["content"].ToString();
                        string channelId = d["channel_id"].ToString();

                        // 判斷是否為私訊 (guild_id 為空)
                        if (d["guild_id"] == null)
                        {
                            string forwardText = "";

                            if (authorId == _myUserId)
                            {
                                // A. 如果是我發出的 (回覆別人)
                                // 從字典反查這個頻道 ID 是屬於哪個使用者的
                                string targetName = _userChannels.FirstOrDefault(x => x.Value == channelId).Key ?? "未知對象";
                                forwardText = $"【回覆給 {targetName}】: {content}";

                                this.Invoke((MethodInvoker)delegate {
                                    lstMessages.Items.Add($"> [回覆 {targetName}]: {content}");
                                });
                            }
                            else
                            {
                                // B. 如果是對方發來的
                                forwardText = $"【收到來自 {authorName}】: {content}";

                                this.Invoke((MethodInvoker)delegate {
                                    // 自動記憶此對話，方便之後主動回覆
                                    if (!_userChannels.ContainsKey(authorName)) _userChannels[authorName] = channelId;
                                    lstMessages.Items.Add($"[{authorName}]: {content}");
                                });
                            }

                            // 4. 執行安全轉發邏輯
                            string forwardId = txtForwardChannelId.Text.Trim();
                            if (!string.IsNullOrEmpty(forwardId))
                            {
                                // --- 防封鎖：強制延遲保護 ---
                                double elapsed = (DateTime.Now - _lastForwardTime).TotalMilliseconds;
                                int minDelay = random.Next(10000, 15000); // 隨機延遲 10 ~ 15 秒

                                if (elapsed < minDelay)
                                {
                                    int waitTime = minDelay - (int)elapsed;
                                    // AppendLog($"[安全] 轉發冷卻中，等待 {waitTime}ms...");
                                    await Task.Delay(waitTime);
                                }

                                // 模擬打字效果 (更像真人)
                                await SendTypingIndicator(forwardId, currentToken);
                                await Task.Delay(random.Next(2000, 5000));

                                // 正式轉發
                                await SendDiscordMessage(forwardId, currentToken, forwardText);
                                _lastForwardTime = DateTime.Now; // 更新最後轉發時間
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 避免因為單個封包解析失敗導致整個 Loop 停止
                    if (_ws.State == WebSocketState.Open)
                        AppendLog($"[監聽錯誤]: {ex.Message}");
                }
            }
        }

        // --- 批量發文任務 ---
        private async void btnStart_Click(object sender, EventArgs e)
        {
            var tokens = txtTokens.Lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var channels = txtChannels.Lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (tokens.Count == 0 || channels.Count == 0) return;

            _cts = new CancellationTokenSource();
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            try
            {
                await RunChatTask(channels, tokens, txtMessages.Text, _cts.Token);
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private async Task RunChatTask(List<string> channels, List<string> tokens, string msg, CancellationToken ct)
        {
            try
            {
                AppendLog("[系統] 自動發文任務已啟動。");

                while (!ct.IsCancellationRequested)
                {
                    // 檢查時段限制
                    if (chkEnableTimeLimit.Checked)
                    {
                        TimeSpan start = dtpStartTime.Value.TimeOfDay;
                        TimeSpan end = dtpEndTime.Value.TimeOfDay;
                        TimeSpan now = DateTime.Now.TimeOfDay;
                        bool inRange = (start <= end) ? (now >= start && now <= end) : (now >= start || now <= end);

                        if (!inRange)
                        {
                            // 沒在時段內，等待 30 秒再檢查，傳入 ct 確保能隨時停止
                            await Task.Delay(30000, ct);
                            continue;
                        }
                    }

                    foreach (var auth in tokens)
                    {
                        foreach (var channelId in channels)
                        {
                            // 每次發送前都檢查一次是否已取消
                            if (ct.IsCancellationRequested) break;

                            await SendDiscordMessage(channelId, auth, msg);
                            AppendLog($"[發送] 頻道: {channelId} 成功。");

                            // 隨機延遲，傳入 ct 確保按下停止時能「立刻」中斷，而不是等完這幾秒
                            int nextDelay = random.Next(2000, 5000);
                            await Task.Delay(nextDelay, ct);
                        }
                        if (ct.IsCancellationRequested) break;
                    }

                    // 大循環延遲 (秒)
                    int loopDelay = (int)numMinDelay.Value * 1000;
                    await Task.Delay(loopDelay, ct);
                }
            }
            catch (OperationCanceledException)
            {
                // 這是正常停止，不需當作錯誤處理
                AppendLog("[系統] 任務已手動停止。");
            }
            catch (Exception ex)
            {
                // 捕捉其他非預期的錯誤
                AppendLog($"[異常] 任務中斷: {ex.Message}");
            }
            finally
            {
                // 確保按鈕狀態恢復
                this.Invoke((MethodInvoker)delegate {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                });
            }
        }

        private async Task SendDiscordMessage(string channelId, string auth, string text)
        {
            // 1. 先模擬打字
            await SendTypingIndicator(channelId, auth);
            // 2. 模擬打字需要時間 (隨機 1.5 ~ 3 秒)
            await Task.Delay(random.Next(5000, 10000));

            var url = $"https://discord.com/api/v9/channels/{channelId}/messages";
            var payload = new { content = text, tts = false };
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", auth);
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                try { await client.SendAsync(request); } catch (Exception ex){ AppendLog($"發送文案失敗錯誤原因 : {ex}"); }
            }
        }

        private async Task HeartbeatLoop()
        {
            while (_ws.State == WebSocketState.Open)
            {
                await Task.Delay(40000);
                await SendJsonAsync(JsonConvert.SerializeObject(new { op = 1, d = (int?)null }));
            }
        }

        private async Task SendJsonAsync(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            if (_ws.State == WebSocketState.Open)
                await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                AppendLog("[系統] 正在停止任務...");
                _cts.Cancel();
                // 這裡不需要 Dispose，讓 Task 內部的 finally 處理 UI 狀態
            }
        }

        private void lstMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMessages.SelectedItem == null) return;
            string line = lstMessages.SelectedItem.ToString();
            if (line.Contains("[") && line.Contains("]:"))
            {
                string name = line.Substring(1, line.IndexOf("]:") - 1);
                if (_userChannels.TryGetValue(name, out string cid))
                {
                    _selectedChannelId = cid;
                    lblReplyTarget.Text = $"回覆對象：{name}";
                }
            }
        }

        private async void btnSendReply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedChannelId)) return;
            string token = txtTokens.Lines.FirstOrDefault();
            await SendDiscordMessage(_selectedChannelId, token, txtReply.Text);
            txtReply.Clear();
        }

        // 1. 點擊按鈕後，向 Discord 請求所有的私訊頻道
        private async void btnFetchDMs_Click(object sender, EventArgs e)
        {
            string token = txtTokens.Lines.FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            AppendLog("正在讀取私訊列表...");
            string url = "https://discord.com/api/v9/users/@me/channels";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", token);
                try
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var channels = JArray.Parse(json);

                        lstDMs.Items.Clear();
                        foreach (var channel in channels)
                        {
                            // 取得對方名稱 (如果是私訊，會有 recipients 陣列)
                            var recipients = channel["recipients"];
                            if (recipients != null && recipients.HasValues)
                            {
                                string username = recipients[0]["username"].ToString();
                                string channelId = channel["id"].ToString();

                                // 存入字典以便點擊使用
                                if (!_userChannels.ContainsKey(username))
                                    _userChannels.Add(username, channelId);

                                lstDMs.Items.Add(username);
                            }
                        }
                        AppendLog($"成功讀取 {lstDMs.Items.Count} 個對話對象。");
                    }
                    else
                    {
                        AppendLog($"讀取失敗: {response.StatusCode}");
                    }
                }
                catch (Exception ex) { AppendLog($"讀取發生異常: {ex.Message}"); }
            }
        }

        // 2. 當點擊右側列表中的名字時，切換目前要發送訊息的對象
        private void lstDMs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDMs.SelectedItem == null) return;
            string name = lstDMs.SelectedItem.ToString();

            if (_userChannels.TryGetValue(name, out string cid))
            {
                _selectedChannelId = cid;
                lblReplyTarget.Text = $"主動發訊對象：{name}";
                AppendLog($"已切換目標為: {name} (ID: {cid})");
            }
        }


        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            // 清除中間的 ListBox (對話紀錄)
            lstMessages.Items.Clear();
            // 清除底部的 TextBox (系統運行 Log)
            txtLog.Clear();
            // 選用：重設目前選取的對話目標 (避免誤發)
            _selectedChannelId = "";
            lblReplyTarget.Text = "回覆對象：尚未選擇";
            AppendLog("[系統] 已清除所有畫面訊息。");
        }

        private async Task SendTypingIndicator(string channelId, string token)
        {
            var url = $"https://discord.com/api/v9/channels/{channelId}/typing";
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", token);
                try { await client.SendAsync(request); } catch { }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}