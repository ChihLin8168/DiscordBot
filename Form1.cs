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

                var auth = new { op = 2, d = new { token = token, properties = new { os = "Windows", browser = "Chrome", device = "" }, intents = 32767 } };
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

        private async Task ReceiveLoop(string currentToken)
        {
            var buffer = new byte[1024 * 64];
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result;
                    var ms = new System.IO.MemoryStream();

                    // 循環接收直到拿到完整的 EndOfMessage
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    string rawJson = Encoding.UTF8.GetString(ms.ToArray());
                    var data = JObject.Parse(rawJson);
                    string t = data["t"]?.ToString();
                    // 不分大小寫分析 READY 
                    if (!string.IsNullOrEmpty(t) && t.Equals("READY", StringComparison.OrdinalIgnoreCase))
                    {
                        _myUserId = data["d"]["user"]["id"].ToString();
                        AppendLog($"[系統] 識別成功！我的 ID 為: {_myUserId}");
                    }

                    if (!string.IsNullOrEmpty(t) && t.Equals("MESSAGE_CREATE", StringComparison.OrdinalIgnoreCase))
                    {
                        var d = data["d"];
                        string authorId = d["author"]["id"].ToString();
                        string authorName = d["author"]["username"].ToString();
                        string content = d["content"].ToString();
                        string channelId = d["channel_id"].ToString();

                        // 判斷是否為私訊 (無 guild_id)
                        if (d["guild_id"] == null)
                        {
                            string forwardText = "";

                            // --- 關鍵判斷邏輯 ---
                            if (authorId == _myUserId)
                            {
                                // 嘗試從緩存中找出這個 channelId 對應的使用者名稱
                                string targetName = _userChannels.FirstOrDefault(x => x.Value == channelId).Key ?? "未知對象";

                                // 格式化轉發文字
                                forwardText = $"【回覆給 {targetName}】: {content}";

                                this.Invoke((MethodInvoker)delegate {
                                    // 在介面清單顯示回覆了誰
                                    lstMessages.Items.Add($"> [回覆 {targetName}]: {content}");
                                });
                            }
                            else
                            {
                                // 這是對方發來的訊息
                                forwardText = $"【收到來自 {authorName}】: {content}";
                                this.Invoke((MethodInvoker)delegate {
                                    if (!_userChannels.ContainsKey(authorName)) _userChannels.Add(authorName, channelId);
                                    lstMessages.Items.Add($"[{authorName}]: {content}");
                                });
                            }

                            // 執行轉發到指定的頻道
                            string forwardId = txtForwardChannelId.Text.Trim();
                            if (!string.IsNullOrEmpty(forwardId))
                                await SendDiscordMessage(forwardId, currentToken, forwardText);
                        }
                    }
                }
                catch { }
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
            while (!ct.IsCancellationRequested)
            {
                if (chkEnableTimeLimit.Checked)
                {
                    TimeSpan start = dtpStartTime.Value.TimeOfDay;
                    TimeSpan end = dtpEndTime.Value.TimeOfDay;
                    TimeSpan now = DateTime.Now.TimeOfDay;
                    bool inRange = (start <= end) ? (now >= start && now <= end) : (now >= start || now <= end);
                    if (!inRange) { await Task.Delay(30000, ct); continue; }
                }

                foreach (var auth in tokens)
                {
                    foreach (var channelId in channels)
                    {
                        if (ct.IsCancellationRequested) return;
                        await SendDiscordMessage(channelId, auth, msg);
                        await Task.Delay(random.Next(2000, 5000), ct);
                    }
                }
                await Task.Delay((int)numMinDelay.Value * 1000, ct);
            }
        }

        private async Task SendDiscordMessage(string channelId, string auth, string text)
        {
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

        private void btnStop_Click(object sender, EventArgs e) => _cts?.Cancel();

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


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}