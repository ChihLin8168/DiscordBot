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

                    if (!string.IsNullOrEmpty(t) && t.Equals("MESSAGE_CREATE", StringComparison.OrdinalIgnoreCase))
                    {
                        var d = data["d"];
                        string authorId = d["author"]["id"].ToString();
                        string authorName = d["author"]["username"].ToString();
                        string content = d["content"].ToString();
                        string channelId = d["channel_id"].ToString();

                        // 1. 排除自己發出的訊息
                        if (authorId != _myUserId)
                        {
                            bool isNewArrival = false;
                            string currentTime = DateTime.Now.ToString("HH:mm"); // 取得目前時間

                            // --- 核心判斷：檢查 lstMessages (中間對話框) ---
                            this.Invoke((MethodInvoker)delegate {
                                bool foundInLog = false;

                                // 遍歷目前的列表，尋找是否有該使用者的名字標記
                                foreach (var item in lstMessages.Items)
                                {
                                    // 比對格式範例： "[18:30] [Username]: ..." 
                                    // 只要包含 "[Username]" 就代表這人在清單中
                                    if (item.ToString().Contains($"[{authorName}]"))
                                    {
                                        foundInLog = true;
                                        break;
                                    }
                                }

                                // 組合顯示文字： [時間] [名字]: 內容
                                string displayLine = $"[{currentTime}] [{authorName}]: {content}";

                                if (!foundInLog)
                                {
                                    isNewArrival = true;
                                    // 發現新 ID，加入列表
                                    lstMessages.Items.Add(displayLine);

                                    // 同步更新左側清單與字典
                                    if (!lstDMs.Items.Contains(authorName)) lstDMs.Items.Add(authorName);
                                    if (!_userChannels.ContainsKey(authorName)) _userChannels[authorName] = channelId;
                                }
                                else
                                {
                                    // 已存在的 ID，直接增加對話行，不觸發警報
                                    lstMessages.Items.Add(displayLine);
                                }

                                // 如果目前正選中此人，同步更新右側歷史框
                                if (_selectedChannelId == channelId)
                                {
                                    txtChatHistory.AppendText($"{displayLine}\r\n");
                                }
                            });

                            // --- 2. 如果是「新出現的 ID」，執行轉發提醒 ---
                            if (isNewArrival)
                            {
                                string forwardId = txtForwardChannelId.Text.Trim();
                                if (!string.IsNullOrEmpty(forwardId))
                                {
                                    // 執行安全延遲與轉發
                                    await Task.Delay(random.Next(1000, 2000));

                                    string alertMsg = $"【🔔 新未回覆提醒】";
                                    await SendDiscordMessage(forwardId, currentToken, alertMsg);

                                    AppendLog($"[警報] 已將新 ID [{authorName}] 轉發至提醒頻道。");
                                }
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
            // 1. 基本檢查
            if (string.IsNullOrEmpty(_selectedChannelId))
            {
                MessageBox.Show("請先從清單中選擇回覆對象！");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtReply.Text)) return;

            string token = txtTokens.Lines.FirstOrDefault()?.Trim();
            string messageContent = txtReply.Text;

            // 2. UI 鎖定與狀態提示
            btnSendReply.Enabled = false;           // 鎖定按鈕防止重複點擊
            string originalText = btnSendReply.Text;
            btnSendReply.Text = "傳送中...";         // 視覺回饋
            txtReply.ReadOnly = true;               // 暫時鎖定輸入框

            try
            {
                // 3. 執行發送 (包含我們之前的打字偽裝)
                await SendTypingIndicator(_selectedChannelId, token);
                await Task.Delay(random.Next(800, 1500)); // 模擬人類打字時間

                await SendDiscordMessage(_selectedChannelId, token, messageContent);

                // 4. 成功後的處理
                AppendLog($"[成功] 已回覆訊息至 ID: {_selectedChannelId}");
                txtReply.Clear(); // 清空輸入框
            }
            catch (Exception ex)
            {
                AppendLog($"[失敗] 回覆失敗: {ex.Message}");
                MessageBox.Show($"傳送失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 5. 無論成功或失敗，最後都恢復 UI 狀態
                btnSendReply.Enabled = true;
                btnSendReply.Text = originalText;
                txtReply.ReadOnly = false;
                txtReply.Focus(); // 重新聚焦，方便下次輸入
            }
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
        private async void lstDMs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDMs.SelectedItem == null) return;

            string name = lstDMs.SelectedItem.ToString();
            if (_userChannels.TryGetValue(name, out string cid))
            {
                _selectedChannelId = cid;
                lblReplyTarget.Text = $"目標：{name}";

                // 觸發讀取對話內容
                await FetchChatHistory(cid);
            }
        }


        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            // 核心：清空這個列表，偵測機制就會重置
            lstMessages.Items.Clear();

            // 選用：清空左側名字列表與歷史框
            lstDMs.Items.Clear();
            txtChatHistory.Clear();
            _userChannels.Clear();

            AppendLog("[系統] 已清除所有緩存，新訊息將重新觸發提醒。");
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


        // 新增讀取歷史訊息的方法
        private async Task FetchChatHistory(string channelId)
        {
            string token = txtTokens.Lines.FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(channelId)) return;

            // UI 初始狀態回饋
            txtChatHistory.Text = "正在讀取對話紀錄...";

            // Discord API: 獲取頻道訊息 (limit=25 則通常足以查看上下文)
            string url = $"https://discord.com/api/v9/channels/{channelId}/messages?limit=25";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", token);

                try
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var messages = JArray.Parse(json);

                        if (messages.Count > 0)
                        {
                            // --- 1. 標記已讀 (消除 Discord 驚嘆號) ---
                            // 取最新的訊息 ID (通常是第一筆) 發送 Ack
                            string latestMsgId = messages[0]["id"].ToString();
                            await MarkAsRead(channelId, latestMsgId, token);

                            // --- 2. 格式化訊息內容 ---
                            StringBuilder sb = new StringBuilder();

                            // API 回傳是從新到舊 (Index 0 是最新)，我們反轉為從舊到新顯示
                            foreach (var msg in messages.Reverse())
                            {
                                string author = msg["author"]["username"].ToString();
                                string content = msg["content"].ToString();

                                // 處理時間：從 UTC 轉為在地時間 (如 GMT+8)
                                DateTime timestamp = DateTime.Parse(msg["timestamp"].ToString()).ToLocalTime();
                                string timeStr = timestamp.ToString("MM/dd HH:mm");

                                // 區分「我」與「對方」的視覺感 (選用)
                                if (msg["author"]["id"].ToString() == _myUserId)
                                    sb.AppendLine($"[{timeStr}] <我>: {content}");
                                else
                                    sb.AppendLine($"[{timeStr}] {author}: {content}");
                            }

                            // --- 3. 更新 UI ---
                            txtChatHistory.Text = sb.ToString();

                            // 自動捲動到最底端 (Focus 在最新訊息)
                            txtChatHistory.SelectionStart = txtChatHistory.Text.Length;
                            txtChatHistory.ScrollToCaret();
                        }
                        else
                        {
                            txtChatHistory.Text = "此頻道尚無訊息。";
                        }
                    }
                    else
                    {
                        txtChatHistory.Text = $"無法讀取訊息。錯誤碼: {response.StatusCode}";
                        AppendLog($"[錯誤] FetchChatHistory 失敗: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    txtChatHistory.Text = $"讀取發生異常: {ex.Message}";
                    AppendLog($"[異常] FetchChatHistory 崩潰: {ex.Message}");
                }
            }
        }


        private async Task MarkAsRead(string channelId, string messageId, string token)
        {
            // Discord API 標記已讀的端點
            string url = $"https://discord.com/api/v9/channels/{channelId}/messages/{messageId}/ack";

            // Ack 請求通常需要一個空 JSON 或 token 驗證
            var payload = new { token = (string)null };

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", token);
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        // AppendLog($"[系統] 頻道 {channelId} 已標記為已讀。");
                    }
                }
                catch { /* 忽略失敗 */ }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}