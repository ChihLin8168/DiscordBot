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
        private System.Windows.Forms.Timer _wsCheckTimer;
        private int _currentTokenIndex = 0;
        private int _failureCount = 0;       // 目前連續失敗次數
        private const int _maxFailures = 3;  // 最大容許失敗次數 (超過就停止檢查)
        private int? _lastSequence = null;
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
            _failureCount = 0;
            btnConnectWS.Enabled = false;

            // 呼叫連線邏輯
            bool success = await StartWSService(true);

            if (success)
            {
                // 連線成功：讓中斷按鈕可以點擊
                btnDisconnectWS.Enabled = true;
                btnConnectWS.Enabled = false;
            }
            else
            {
                // 登入失敗：恢復連線按鈕，關閉中斷按鈕
                btnConnectWS.Enabled = true;
                btnDisconnectWS.Enabled = false;
            }
        }
        private DateTime _lastForwardTime = DateTime.MinValue;
         
        private async Task ReceiveLoop(string currentToken, CancellationToken ct)
        {
            var buffer = new byte[1024 * 16]; // 16KB 緩衝區
            AppendLog("[系統] 開始監聽 Discord 事件...");

            while (_ws != null && _ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                try
                {
                    WebSocketReceiveResult result;
                    string rawJson = "";

                    // 1. 確保完整接收大封包 (如 READY 可能超過 1MB)
                    using (var ms = new System.IO.MemoryStream())
                    {
                        do
                        {
                            result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                AppendLog("[警告] Discord 伺服器已主動關閉 WebSocket 連線。");
                                goto ExitLoop;
                            }

                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        rawJson = Encoding.UTF8.GetString(ms.ToArray());
                    }

                    if (string.IsNullOrWhiteSpace(rawJson)) continue;

                    // 2. 解析 JSON 封包
                    var data = JObject.Parse(rawJson);

                    // 更新序列號 (s)，這對維持連線與心跳至關重要
                    if (data["s"] != null && data["s"].Type != JTokenType.Null)
                    {
                        _lastSequence = data["s"].Value<int>();
                    }

                    string op = data["op"]?.ToString();
                    string t = data["t"]?.ToString();

                    // --- A. 處理 HELLO (OP 10) ---
                    if (op == "10")
                    {
                        var d = data["d"];
                        if (d != null && d["heartbeat_interval"] != null)
                        {
                            int heartbeatInterval = d["heartbeat_interval"].Value<int>();
                            // 啟動一個背景任務定時發送心跳
                            _ = Task.Run(() => StartHeartbeat(heartbeatInterval, _cts.Token));
                            AppendLog($"[系統] 收到 HELLO，心跳機制已啟動 ({heartbeatInterval}ms)");
                        }
                    }

                    // --- B. 登入成功事件 (READY) ---
                    if (!string.IsNullOrEmpty(t) && t.Equals("READY", StringComparison.OrdinalIgnoreCase))
                    {
                        _myUserId = data["d"]["user"]["id"].ToString();
                        AppendLog($"[系統] 登入成功！您的 ID 是: {_myUserId}");
                        this.Invoke((MethodInvoker)(() => lblStatus.Text = "狀態：已登入"));
                    }

                    // --- C. 接收訊息事件 (MESSAGE_CREATE) ---
                    if (!string.IsNullOrEmpty(t) && t.Equals("MESSAGE_CREATE", StringComparison.OrdinalIgnoreCase))
                    {
                        var d = data["d"];
                        string authorId = d["author"]?["id"]?.ToString();
                        string authorName = d["author"]?["username"]?.ToString();
                        string content = d["content"]?.ToString();
                        string channelId = d["channel_id"]?.ToString();

                        // 排除自己發送的訊息且確保欄位不為空
                        if (authorId != null && authorId != _myUserId)
                        {
                            bool isNewArrival = false;
                            string currentTime = DateTime.Now.ToString("HH:mm");

                            // --- 第一步：UI 判斷與顯示 (Invoke) ---
                            this.Invoke((MethodInvoker)delegate
                            {
                                bool foundInLog = false;
                                // 檢查中間對話框 lstMessages 是否已有此人名字 (保留原本邏輯)
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
                                    AppendLog($"[忽略] {authorName} 頻繁觸發，5 分鐘內不重複轉發標記。");
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    // 收到異常時記錄但不立刻斷開連線，除非連線狀態真的壞了
                    if (_ws != null && _ws.State == WebSocketState.Open)
                    {
                        AppendLog($"[監聽異常]: {ex.Message}");
                        await Task.Delay(1000, ct); // 緩衝一下避免無限報錯
                    }
                    else
                    {
                        break;
                    }
                }
            }

        ExitLoop:
            // --- 退出迴圈後的清理 ---
            StopInternal();
            this.Invoke((MethodInvoker)(() => lblStatus.Text = "狀態：已斷線"));
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

            // 只有當 WebSocket 物件存在，且狀態為 Open (已連線) 時，才繼續執行
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                // 選擇性記錄：如果你想知道為什麼沒轉發，可以取消註解下一行
                AppendLog("[系統] 連線已中斷，停止發送訊息。");
                return;
            }
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
                            AppendLog($"[發送推文] 頻道: {channelId} 成功。");

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

            // 只有當 WebSocket 物件存在，且狀態為 Open (已連線) 時，才繼續執行
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                // 選擇性記錄：如果你想知道為什麼沒轉發，可以取消註解下一行
                // AppendLog("[轉發跳過] 監聽連線已中斷，停止發送轉發訊息。");
                return;
            }
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

        // 在 Form1_Load 或 btnConnectWS_Click 成功後初始化
        private void InitWsMonitor()
        {
            if (_wsCheckTimer == null)
            {
                _wsCheckTimer = new System.Windows.Forms.Timer();
                _wsCheckTimer.Interval = 5 * 60 * 1000; // 5 分鐘 (毫秒)
                _wsCheckTimer.Tick += WsCheckTimer_Tick;
            }
            _wsCheckTimer.Start();
            AppendLog("[系統] 已啟動連線自動監控 (每 5 分鐘檢查一次)");
        }

        private async void WsCheckTimer_Tick(object sender, EventArgs e)
        {
            // 如果連線狀態不正常，且失敗次數還在容許範圍內
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                if (_failureCount < _maxFailures)
                {
                    await StartWSService(false); // 這是自動重連，不是初始登入
                }
                else
                {
                    _wsCheckTimer.Stop();
                    AppendLog("[監控停止] 連續重連失敗，已停止自動檢查。");
                }
            }
            else
            {
                // 連線正常，更新狀態標籤 (選配)
                lblStatus.Text = $"狀態：連線中 ({DateTime.Now:HH:mm:ss} 檢查正常)";
            }
        }

        public async Task<bool> StartWSService(bool isManualClick = false)
        {
            // 1. 取得 Token
            string[] tokenList = txtTokens.Lines.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
            if (tokenList.Length == 0) return false;
            string currentToken = tokenList[0].Trim();

            // 2. 清理舊資源
            StopInternal();

            _ws = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            // 使用 v10 版本，並確保編碼為 json
            Uri uri = new Uri("wss://gateway.discord.gg/?v=10&encoding=json");

            try
            {
                AppendLog("[連線] 正在連接 Discord ...");

                // 設定連線逾時
                var connectTask = _ws.ConnectAsync(uri, _cts.Token);
                if (await Task.WhenAny(connectTask, Task.Delay(8000)) != connectTask) throw new Exception("網路連線逾時");

                // 3. 準備 Identity (修正格式以符合個人 Token)
                // 注意：這裡使用了 Dictionary 確保產出的 JSON Key 包含 $ 符號，且移除了 intents
                var identity = new
                {
                    op = 2,
                    d = new
                    {
                        token = currentToken,
                        properties = new Dictionary<string, string> {
                    { "$os", "windows" },
                    { "$browser", "chrome" },
                    { "$device", "pc" }
                }
                        // 個人 Token 不要加 intents = 32767，否則會被秒踢
                    }
                };

                string json = JsonConvert.SerializeObject(identity);
                await _ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, _cts.Token);

                // 4. 重要：等待驗證結果
                // Discord 在收到 Identity 後，如果 Token 錯誤會立刻斷開連線
                await Task.Delay(2000);

                if (_ws.State != WebSocketState.Open)
                {
                    // 如果走到這裡 State 變成了 Aborted 或 Closed，代表驗證失敗
                    throw new Exception("Token 驗證失敗 (Discord 已主動拒絕連線)");
                }

                // --- 確定成功連線後，才啟動後續機制 ---
                AppendLog("[成功] 驗證通過，啟動監聽。");
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "狀態：連線中"));

                _failureCount = 0;

                // 5. 正式開啟接收迴圈
                _ = Task.Run(() => ReceiveLoop(currentToken, _cts.Token));

                // 6. 啟動 5 分鐘自動巡檢
                InitWsMonitor();

                return true;
            }
            catch (Exception ex)
            {
                // 核心邏輯：只要 catch 到任何錯誤，就徹底關閉，不進監聽
                AppendLog($"[登入失敗]: {ex.Message}");
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "狀態：登入錯誤"));

                if (_wsCheckTimer != null) _wsCheckTimer.Stop();
                StopInternal();

                AppendLog("[停止] 初始登入失敗，已取消自動監聽機制。");
                return false;
            }
        }

        // 輔助方法：統一清理連線資源
        private void StopInternal()
        {
            if (_cts != null) { _cts.Cancel(); _cts.Dispose(); _cts = null; }
            if (_ws != null) { _ws.Dispose(); _ws = null; }
        }
        // 心跳發送方法
        private async Task StartHeartbeat(int interval, CancellationToken ct)
        {
          
            AppendLog($"[系統] 系統會持續檢查連線。");

            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(interval, ct);

                // 這裡最容易噴 NullReferenceException！
                if (_ws == null || _ws.State != WebSocketState.Open) break;

                try
                {
                    var heartbeat = new { op = 1, d = _lastSequence }; // 確保 _lastSequence 有初始化
                    string json = JsonConvert.SerializeObject(heartbeat);
                    await _ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                                       WebSocketMessageType.Text, true, ct);
                }
                catch { break; }
                finally
                {
                    AppendLog("[系統] 檢查Token連線正常。");
                }
            }
           
        }

        private async void btnDisconnectWS_Click(object sender, EventArgs e)
        {
            btnDisconnectWS.Enabled = false;
            _failureCount = 0;

            AppendLog("[系統] 使用者點擊中斷連線...");

            // 1. 停止計時器與清理資源
            if (_wsCheckTimer != null) _wsCheckTimer.Stop();
            StopInternal();

            lblStatus.Text = "狀態：已手動斷開";

            // 2. 恢復連線按鈕狀態
            btnConnectWS.Enabled = true;
            AppendLog("[系統] 監聽已停止。");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "修改")
            {
                txtTargetChannelId.Enabled = true;
                txtBotToken.Enabled = true;
                button1.Text = "完成";
            }
            else
            {
                txtTargetChannelId.Enabled = false;
                txtBotToken.Enabled = false;
                button1.Text = "修改";
            }

        }
    }
}