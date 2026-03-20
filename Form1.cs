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
        private string myBotToken = "MTQ4Mzc0MzcxNDY3MjExNTcxMg.G3V6QC.PvoRARBnAXlc1H9RWtCUlXVo05Z5J-7nj_S2HM";
        private Dictionary<string, string> _tagRules = new Dictionary<string, string>();
        private string _configPath = "config.json";
        // 記錄標記歷史：Key = "AuthorID_Tag", Value = 觸發時間
        private Dictionary<string, DateTime> _lastMentionTimes = new Dictionary<string, DateTime>();
        private readonly TimeSpan _mentionCooldown = TimeSpan.FromMinutes(5);
        public Form1()
        {
            InitializeComponent();
            numMinDelay.Value = 10;
            numMaxDelay.Value = 30;
            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
        // 讀取設定檔並套用到 UI
        private void LoadConfig()
        {
            try
            {
                if (System.IO.File.Exists(_configPath))
                {
                    string json = System.IO.File.ReadAllText(_configPath);
                    var config = JsonConvert.DeserializeObject<AppConfig>(json);

                    if (config != null)
                    {
                        txtTokens.Text = config.Tokens;
                        txtChannels.Text = config.Channels;
                        txtMessages.Text = config.Messages; // 載入發文內容
                        numMinDelay.Value = config.MinDelay;
                        numMaxDelay.Value = config.MaxDelay;
                        txtBotToken.Text = config.BotToken;
                        txtTargetChannelId.Text = config.TargetChannelId;

                        // 載入關鍵字規則
                        _tagRules = config.TagRules ?? new Dictionary<string, string>();
                        lstTagRules.Items.Clear();
                        foreach (var rule in _tagRules)
                        {
                            lstTagRules.Items.Add($"{rule.Key} -> {rule.Value}");
                        }
                        AppendLog("[系統] 設定檔已成功載入。");
                    }
                }
            }
            catch (Exception ex) { AppendLog($"[系統錯誤] 讀取設定失敗: {ex.Message}"); }
        }

        // 抓取 UI 目前狀態並存檔
        private void SaveConfig()
        {
            try
            {
                var config = new AppConfig
                {
                    Tokens = txtTokens.Text,
                    Channels = txtChannels.Text,
                    Messages = txtMessages.Text, // 儲存發文內容
                    MinDelay = numMinDelay.Value,
                    MaxDelay = numMaxDelay.Value,
                    BotToken = txtBotToken.Text,
                    TargetChannelId = txtTargetChannelId.Text,
                    TagRules = _tagRules
                };

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                System.IO.File.WriteAllText(_configPath, json);

                CleanOldMentions();
            }
            catch (Exception ex) { AppendLog($"[系統錯誤] 儲存設定失敗: {ex.Message}"); }
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

                    // 3. 接收訊息事件 (MESSAGE_CREATE)
                    if (!string.IsNullOrEmpty(t) && t.Equals("MESSAGE_CREATE", StringComparison.OrdinalIgnoreCase))
                    {
                        var d = data["d"];
                        string authorId = d["author"]["id"].ToString();
                        string authorName = d["author"]["username"].ToString();
                        string content = d["content"].ToString();
                        string channelId = d["channel_id"].ToString();
                        string messageId = d["id"].ToString();
                        bool isPrivate = (d["guild_id"] == null); // 是否為私訊

                        // 排除自己發送的訊息
                        if (authorId != _myUserId)
                        {
                            bool isNewArrival = false;
                            string currentTime = DateTime.Now.ToString("HH:mm");

                            // --- 第一步：UI 判斷與顯示 (Invoke) ---
                            this.Invoke((MethodInvoker)delegate
                            {
                                bool foundInLog = false;
                                // 檢查中間對話框 lstMessages 是否已有此人名字
                                foreach (var item in lstMessages.Items)
                                {
                                    if (item.ToString().Contains($"[{authorName}]"))
                                    {
                                        foundInLog = true;
                                        break;
                                    }
                                }

                                string displayLine = $"[{currentTime}] [{authorName}]: {content}";

                                if (!foundInLog)
                                {
                                    isNewArrival = true;
                                    lstMessages.Items.Add(displayLine);

                                    // 更新左側列表與字典 
                                    if (!_userChannels.ContainsKey(authorName)) _userChannels[authorName] = channelId;
                                }
                                else
                                {
                                    // 已存在的 ID，僅增加行
                                    lstMessages.Items.Add(displayLine);
                                }

                                // 自動捲動到底部
                                lstMessages.TopIndex = lstMessages.Items.Count - 1;


                            });

                            // --- 第二步：標記與轉發邏輯處理 (含 5 分鐘冷卻機制) ---
                            string currentBotToken = txtBotToken.Text.Trim();
                            string currentAdminChannel = txtTargetChannelId.Text.Trim();

                            if (!string.IsNullOrEmpty(currentBotToken) && !string.IsNullOrEmpty(currentAdminChannel))
                            {
                                List<string> tagsToSend = new List<string>();
                                bool shouldForward = false;
                                DateTime now = DateTime.Now;

                                // A. 判斷是否為「新 ID」：標記 @everyone
                                if (isNewArrival)
                                {
                                    string tagKey = $"{authorId}_everyone";
                                    if (!_lastMentionTimes.ContainsKey(tagKey) || (now - _lastMentionTimes[tagKey]) > _mentionCooldown)
                                    {
                                        tagsToSend.Add("@everyone");
                                        _lastMentionTimes[tagKey] = now;
                                        shouldForward = true;
                                    }
                                }

                                // B. 判斷「關鍵字」：標記當事人
                                HashSet<string> matchedIds = new HashSet<string>();
                                foreach (var rule in _tagRules)
                                {
                                    if (content.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string tagKey = $"{authorId}_{rule.Value}";
                                        // 檢查該使用者針對此標記是否過期
                                        if (!_lastMentionTimes.ContainsKey(tagKey) || (now - _lastMentionTimes[tagKey]) > _mentionCooldown)
                                        {
                                            matchedIds.Add(rule.Value);
                                            _lastMentionTimes[tagKey] = now;
                                            shouldForward = true;
                                        }
                                    }
                                }

                                foreach (var id in matchedIds)
                                {
                                    tagsToSend.Add($"<@{id}>");
                                }

                                // C. 執行轉發：只有在有「新的有效標記」時才發送
                                if (shouldForward && tagsToSend.Count > 0)
                                {
                                    string mentionString = string.Join(" ", tagsToSend.Distinct());
                                    await ForwardToAdminChannel(currentAdminChannel, currentBotToken, authorName, content, mentionString);

                                    AppendLog($"[轉發] 已標記 {mentionString}。 (下次標記此人需等 5 分鐘)");
                                }
                                else if (shouldForward == false && isNewArrival)
                                {
                                    // 雖然是新 ID，但如果在冷卻中，我們可以選擇只顯示在 UI 不轉發
                                    AppendLog($"[忽略] {authorName} 頻繁觸發，5 分鐘內不重複轉發標記。");
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
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
                this.Invoke((MethodInvoker)delegate
                {
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
            await Task.Delay(random.Next(1500, 3000));

            var url = $"https://discord.com/api/v10/channels/{channelId}/messages";
            var payload = new { content = text, tts = false };
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", auth);
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                try { await client.SendAsync(request); } catch (Exception ex) { AppendLog($"發送文案失敗錯誤原因 : {ex}"); }
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





        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            // 核心：清空這個列表，偵測機制就會重置
            lstMessages.Items.Clear();
            _userChannels.Clear();
            AppendLog("[系統] 已清除所有緩存。");
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
            LoadConfig(); // 啟動時讀取
            AppendLog("系統啟動成功。");

        }
        public async Task<string> GetOrCreateDMChannel(string userId, string botToken)
        {
            string url = "https://discord.com/api/v9/users/@me/channels";
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                // Bot Token 的格式是 "Bot [TOKEN]"
                request.Headers.TryAddWithoutValidation("Authorization", $"Bot {botToken}");

                var payload = new { recipient_id = userId };
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json)["id"].ToString(); // 這就是私訊頻道的 ID
                }
            }
            return null;
        }


        // --- 機器人轉發方法 (使用漂亮的 Embed 格式) ---
        public async Task ForwardToAdminChannel(string senderName, string messageContent)
        {
            string botToken = txtBotToken.Text.Trim();
            string targetChannelId = txtTargetChannelId.Text.Trim();

            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(targetChannelId)) return;

            // 關鍵字匹配判斷
            string mentionTag = "";
            foreach (var rule in _tagRules)
            {
                if (messageContent.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                {
                    mentionTag = $"<@{rule.Value}>"; // 格式化為 Discord Tag 語法
                    break;
                }
            }

            string url = $"https://discord.com/api/v10/channels/{targetChannelId}/messages";
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bot {botToken}");

                    var payload = new
                    {
                        content = $"{mentionTag} 📩 **收到新私訊**",
                        embeds = new[] {
                    new {
                        title = "訊息詳情",
                        color = 3447003,
                        fields = new[] {
                            new { name = "發送者", value = senderName, inline = true },
                            new { name = "內容", value = messageContent, inline = false }
                        },
                        footer = new { text = "自動監控系統" }
                    }
                }
                    };

                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                        AppendLog($"[Bot] 已成功轉發來自 {senderName} 的訊息。");
                    else
                        AppendLog($"[Bot 錯誤] 轉發失敗: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[Bot 異常] {ex.Message}");
            }
        }
        public async Task ForwardToAdminChannel(string channelId, string botToken, string senderName, string messageContent, string mentionTag)
        {
            string url = $"https://discord.com/api/v10/channels/{channelId}/messages";

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bot {botToken.Trim()}");
                    request.Headers.TryAddWithoutValidation("User-Agent", "DiscordBot (https://github.com/discord-net/Discord.Net, v3.13.0)");

                    // 組合內容：Tag 放在 Embed 外面才會跳通知
                    var payload = new
                    {
                        content = $"{(string.IsNullOrEmpty(mentionTag) ? "" : mentionTag + " ")}🔔 **【新私訊轉發】**",
                        embeds = new[] {
                                new {
                                    title = "詳細訊息內容",
                                    color = 3447003, // 藍色
                                    fields = new[] {
                                        new { name = "發送者", value = senderName, inline = true },
                                        new { name = "內容", value = messageContent, inline = false }
                                    },
                                    footer = new { text = $"接收時間: {DateTime.Now:HH:mm:ss}" }
                                }
                            },
                        allowed_mentions = new
                        {
                            parse = new[] { "everyone", "users", "roles" }
                        }
                    };

                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        AppendLog($"[轉發錯誤] 狀態碼: {response.StatusCode}, 原因: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[連線異常] 機器人發送失敗: {ex.Message}");
            }
        }
        public async Task DebugCheckChannels(string botToken)
        {
            string url = "https://discord.com/api/v10/users/@me/guilds";
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"Bot {botToken.Trim()}");
                var response = await client.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();

                // 這會列出機器人所在的伺服器，如果連這都噴 403，就是 Token 錯了。
                AppendLog($"[權限偵測] 機器人所在的伺服器資料: {result}");
            }
        }

        private void btnAddRule_Click(object sender, EventArgs e)
        {
            string key = txtKeyword.Text.Trim();
            string id = txtTagUserID.Text.Trim();

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(id))
            {
                if (!_tagRules.ContainsKey(key))
                {
                    _tagRules.Add(key, id);
                    lstTagRules.Items.Add($"{key} -> {id}");
                    txtKeyword.Clear();
                    txtTagUserID.Clear();
                    AppendLog($"[系統] 已新增規則: 當訊息包含 '{key}' 時 Tag {id}");
                }
                else
                {
                    MessageBox.Show("此關鍵字已存在。");
                }
            }
        }

        private void btnDeleteRule_Click(object sender, EventArgs e)
        {
            if (lstTagRules.SelectedIndex != -1)
            {
                string selectedItem = lstTagRules.SelectedItem.ToString();
                string key = selectedItem.Split(new string[] { " -> " }, StringSplitOptions.None)[0];

                _tagRules.Remove(key);
                lstTagRules.Items.RemoveAt(lstTagRules.SelectedIndex);
                AppendLog($"[系統] 已刪除關鍵字規則: {key}");
            }
        }

        public class AppConfig
        {
            public string Tokens { get; set; } = "";
            public string Channels { get; set; } = "";
            public string Messages { get; set; } = ""; // <--- 新增發文內容
            public decimal MinDelay { get; set; } = 5;
            public decimal MaxDelay { get; set; } = 10;
            public string BotToken { get; set; } = "";
            public string TargetChannelId { get; set; } = "";
            // 儲存關鍵字規則：Key 是關鍵字, Value 是 ID
            public Dictionary<string, string> TagRules { get; set; } = new Dictionary<string, string>();
        }

        private void CleanOldMentions()
        {
            var now = DateTime.Now;
            var keysToRemove = _lastMentionTimes.Where(kv => (now - kv.Value) > _mentionCooldown)
                                              .Select(kv => kv.Key).ToList();
            foreach (var key in keysToRemove) _lastMentionTimes.Remove(key);
        }
    }
}