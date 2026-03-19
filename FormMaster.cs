using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordAutoChat
{
    public partial class FormMaster : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly Random random = new Random();
        private CancellationTokenSource _cts; // 用於控制停止

        public FormMaster()
        {
            InitializeComponent();
            // 初始化一些預設值 (選配)
            numMinDelay.Value = 10;
            numMaxDelay.Value = 30;
        }

        // 寫入 Log 的輔助方法
        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(message)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret(); // 自動捲動到底部
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            // 讀取介面輸入並轉換為清單
            var channels = txtChannels.Lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var tokens = txtTokens.Lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var messages = txtMessages.Text;

            if (channels.Count == 0 || tokens.Count == 0)
            {
                MessageBox.Show("請確保 Channel、Token 與 文字內容皆已輸入！");
                return;
            }

            _cts = new CancellationTokenSource();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            AppendLog(">>> 任務開始...");

            try
            {
                await RunChatTask(channels, tokens, messages, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                AppendLog("<<< 任務已手動停止。");
            }
            catch (Exception ex)
            {
                AppendLog($"發生錯誤: {ex.Message}");
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private async Task RunChatTask(List<string> channels, List<string> tokens, string messages, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                // --- 新增：時間區間判斷 ---
                 
                if (chkEnableTimeLimit.Checked)
                {
                    // 直接讀取畫面上控制項的「目前數值」
                    TimeSpan start = dtpStartTime.Value.TimeOfDay;
                    TimeSpan end = dtpEndTime.Value.TimeOfDay;
                    TimeSpan now = DateTime.Now.TimeOfDay;

                    bool inRange = (start <= end)
                        ? (now >= start && now <= end)           // 一般時段 (如 09:00 ~ 18:00)
                        : (now >= start || now <= end);          // 跨夜時段 (如 22:00 ~ 06:00)

                    if (!inRange)
                    {
                        AppendLog($"[暫停] 非執行時段 ({start:hh\\:mm} ~ {end:hh\\:mm})，等候中...");
                        await Task.Delay(30000, ct); // 每 30 秒檢查一次
                        continue;
                    }
                }
                // -------------------------

                foreach (var auth in tokens)
                {
                    foreach (var channelId in channels)
                    {
                        if (ct.IsCancellationRequested) return;

                        string contentText = messages;
                        await SendDiscordMessage(channelId, auth, contentText);

                        // 帳號/頻道間的小間隔 (1~3秒)
                        await Task.Delay(random.Next(1000, 3000), ct);
                    }
                }

                // 一輪結束後的隨機等待時間
                int waitSec = random.Next((int)numMinDelay.Value, (int)numMaxDelay.Value + 1);
                AppendLog($"一輪發送完畢，等待 {waitSec} 秒...");
                await Task.Delay(waitSec * 1000, ct);
            }
        }

        private async Task SendDiscordMessage(string channelId, string auth, string text)
        {
            var url = $"https://discord.com/api/v9/channels/{channelId}/messages";
            var payload = new
            {
                content = text,
                nonce = $"82329451214{random.Next(0, 1000)}33232234",
                tts = false
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.TryAddWithoutValidation("Authorization", auth);
                request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                request.Content = content;

                try
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                        AppendLog($"成功 [頻道:{channelId}]: {text}");
                    else
                        AppendLog($"失敗 [代碼:{(int)response.StatusCode}] 頻道:{channelId}");
                }
                catch (Exception ex)
                {
                    AppendLog($"網路異常: {ex.Message}");
                }
            }
        }
    }
}