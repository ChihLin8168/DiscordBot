namespace DiscordAutoChat
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveConfig(); // 關閉前自動存檔
            base.OnFormClosing(e);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtTokens = new TextBox();
            btnConnectWS = new Button();
            btnDisconnectWS = new Button();
            lblStatus = new Label();
            label1 = new Label();
            grpAutoPost = new GroupBox();
            label2 = new Label();
            txtChannels = new TextBox();
            label3 = new Label();
            txtMessages = new TextBox();
            label4 = new Label();
            numMinDelay = new NumericUpDown();
            label6 = new Label();
            numMaxDelay = new NumericUpDown();
            chkEnableTimeLimit = new CheckBox();
            dtpStartTime = new DateTimePicker();
            label5 = new Label();
            dtpEndTime = new DateTimePicker();
            btnStart = new Button();
            btnStop = new Button();
            lstMessages = new ListBox();
            txtLog = new TextBox();
            label7 = new Label();
            btnClearLogs = new Button();
            grpBotSettings = new GroupBox();
            button1 = new Button();
            labelTagId = new Label();
            labelKey = new Label();
            btnDeleteRule = new Button();
            btnAddRule = new Button();
            txtTagUserID = new TextBox();
            txtKeyword = new TextBox();
            labelRuleList = new Label();
            lstTagRules = new ListBox();
            txtTargetChannelId = new TextBox();
            labelTargetId = new Label();
            txtBotToken = new TextBox();
            labelBotToken = new Label();
            grpAutoPost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMinDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).BeginInit();
            grpBotSettings.SuspendLayout();
            SuspendLayout();
            // 
            // txtTokens
            // 
            txtTokens.Location = new Point(18, 48);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.Size = new Size(277, 114);
            txtTokens.TabIndex = 0;
            // 
            // btnConnectWS
            // 
            btnConnectWS.Location = new Point(303, 48);
            btnConnectWS.Name = "btnConnectWS";
            btnConnectWS.Size = new Size(97, 53);
            btnConnectWS.TabIndex = 6;
            btnConnectWS.Text = "登入\r\n監聽";
            btnConnectWS.Click += btnConnectWS_Click;
            // 
            // btnDisconnectWS
            // 
            btnDisconnectWS.Enabled = false;
            btnDisconnectWS.Location = new Point(303, 109);
            btnDisconnectWS.Name = "btnDisconnectWS";
            btnDisconnectWS.Size = new Size(97, 53);
            btnDisconnectWS.TabIndex = 7;
            btnDisconnectWS.Text = "中斷\r\n連線";
            btnDisconnectWS.Click += btnDisconnectWS_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.DarkBlue;
            lblStatus.Location = new Point(158, 19);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(84, 19);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "狀態：離線";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 19);
            label1.Name = "label1";
            label1.Size = new Size(90, 19);
            label1.TabIndex = 3;
            label1.Text = "User Token:";
            // 
            // grpAutoPost
            // 
            grpAutoPost.Controls.Add(label2);
            grpAutoPost.Controls.Add(txtChannels);
            grpAutoPost.Controls.Add(label3);
            grpAutoPost.Controls.Add(txtMessages);
            grpAutoPost.Controls.Add(label4);
            grpAutoPost.Controls.Add(numMinDelay);
            grpAutoPost.Controls.Add(label6);
            grpAutoPost.Controls.Add(numMaxDelay);
            grpAutoPost.Controls.Add(chkEnableTimeLimit);
            grpAutoPost.Controls.Add(dtpStartTime);
            grpAutoPost.Controls.Add(label5);
            grpAutoPost.Controls.Add(dtpEndTime);
            grpAutoPost.Controls.Add(btnStart);
            grpAutoPost.Controls.Add(btnStop);
            grpAutoPost.Location = new Point(12, 175);
            grpAutoPost.Name = "grpAutoPost";
            grpAutoPost.Size = new Size(395, 685);
            grpAutoPost.TabIndex = 0;
            grpAutoPost.TabStop = false;
            grpAutoPost.Text = "自動發文任務設定";
            // 
            // label2
            // 
            label2.Location = new Point(10, 25);
            label2.Name = "label2";
            label2.Size = new Size(177, 23);
            label2.TabIndex = 0;
            label2.Text = "目標頻道 ID (每行一個):";
            // 
            // txtChannels
            // 
            txtChannels.Location = new Point(10, 50);
            txtChannels.Multiline = true;
            txtChannels.Name = "txtChannels";
            txtChannels.Size = new Size(375, 51);
            txtChannels.TabIndex = 1;
            // 
            // label3
            // 
            label3.Location = new Point(10, 110);
            label3.Name = "label3";
            label3.Size = new Size(100, 23);
            label3.TabIndex = 2;
            label3.Text = "發文內容:";
            // 
            // txtMessages
            // 
            txtMessages.Location = new Point(10, 135);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(375, 365);
            txtMessages.TabIndex = 3;
            // 
            // label4
            // 
            label4.Location = new Point(12, 520);
            label4.Name = "label4";
            label4.Size = new Size(100, 23);
            label4.TabIndex = 4;
            label4.Text = "隨機延遲(秒):";
            // 
            // numMinDelay
            // 
            numMinDelay.Location = new Point(117, 518);
            numMinDelay.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            numMinDelay.Name = "numMinDelay";
            numMinDelay.Size = new Size(70, 27);
            numMinDelay.TabIndex = 5;
            // 
            // label6
            // 
            label6.Location = new Point(192, 520);
            label6.Name = "label6";
            label6.Size = new Size(20, 23);
            label6.TabIndex = 6;
            label6.Text = "~";
            // 
            // numMaxDelay
            // 
            numMaxDelay.Location = new Point(218, 518);
            numMaxDelay.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            numMaxDelay.Name = "numMaxDelay";
            numMaxDelay.Size = new Size(70, 27);
            numMaxDelay.TabIndex = 7;
            // 
            // chkEnableTimeLimit
            // 
            chkEnableTimeLimit.Location = new Point(12, 559);
            chkEnableTimeLimit.Name = "chkEnableTimeLimit";
            chkEnableTimeLimit.Size = new Size(91, 24);
            chkEnableTimeLimit.TabIndex = 8;
            chkEnableTimeLimit.Text = "限制時段";
            // 
            // dtpStartTime
            // 
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.Location = new Point(109, 555);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Size = new Size(124, 27);
            dtpStartTime.TabIndex = 9;
            // 
            // label5
            // 
            label5.Location = new Point(239, 559);
            label5.Name = "label5";
            label5.Size = new Size(13, 23);
            label5.TabIndex = 10;
            label5.Text = "~";
            // 
            // dtpEndTime
            // 
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.Location = new Point(258, 555);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(120, 27);
            dtpEndTime.TabIndex = 11;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.LightCyan;
            btnStart.Location = new Point(10, 600);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(180, 60);
            btnStart.TabIndex = 12;
            btnStart.Text = "開始自動發文";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(205, 600);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(180, 60);
            btnStop.TabIndex = 13;
            btnStop.Text = "停止發文";
            btnStop.Click += btnStop_Click;
            // 
            // lstMessages
            // 
            lstMessages.Location = new Point(414, 48);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(427, 517);
            lstMessages.TabIndex = 4;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.Black;
            txtLog.ForeColor = Color.Lime;
            txtLog.Location = new Point(414, 641);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(854, 216);
            txtLog.TabIndex = 5;
            // 
            // label7
            // 
            label7.Location = new Point(414, 24);
            label7.Name = "label7";
            label7.Size = new Size(137, 23);
            label7.TabIndex = 8;
            label7.Text = "監聽訊息流 (即時):";
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(751, 579);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(90, 54);
            btnClearLogs.TabIndex = 9;
            btnClearLogs.Text = "清除訊息";
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // grpBotSettings
            // 
            grpBotSettings.Controls.Add(button1);
            grpBotSettings.Controls.Add(labelTagId);
            grpBotSettings.Controls.Add(labelKey);
            grpBotSettings.Controls.Add(btnDeleteRule);
            grpBotSettings.Controls.Add(btnAddRule);
            grpBotSettings.Controls.Add(txtTagUserID);
            grpBotSettings.Controls.Add(txtKeyword);
            grpBotSettings.Controls.Add(labelRuleList);
            grpBotSettings.Controls.Add(lstTagRules);
            grpBotSettings.Controls.Add(txtTargetChannelId);
            grpBotSettings.Controls.Add(labelTargetId);
            grpBotSettings.Controls.Add(txtBotToken);
            grpBotSettings.Controls.Add(labelBotToken);
            grpBotSettings.Location = new Point(860, 48);
            grpBotSettings.Name = "grpBotSettings";
            grpBotSettings.Size = new Size(408, 560);
            grpBotSettings.TabIndex = 1;
            grpBotSettings.TabStop = false;
            grpBotSettings.Text = "機器人轉發與關鍵字 Tag 管理";
            // 
            // button1
            // 
            button1.Location = new Point(318, 152);
            button1.Name = "button1";
            button1.Size = new Size(72, 30);
            button1.TabIndex = 3;
            button1.Text = "修改";
            button1.Click += button1_Click;
            // 
            // labelTagId
            // 
            labelTagId.Location = new Point(125, 485);
            labelTagId.Name = "labelTagId";
            labelTagId.Size = new Size(100, 23);
            labelTagId.TabIndex = 1;
            labelTagId.Text = "要 Tag 的 ID:";
            // 
            // labelKey
            // 
            labelKey.Location = new Point(15, 485);
            labelKey.Name = "labelKey";
            labelKey.Size = new Size(100, 23);
            labelKey.TabIndex = 2;
            labelKey.Text = "關鍵字:";
            // 
            // btnDeleteRule
            // 
            btnDeleteRule.Location = new Point(345, 508);
            btnDeleteRule.Name = "btnDeleteRule";
            btnDeleteRule.Size = new Size(45, 30);
            btnDeleteRule.TabIndex = 3;
            btnDeleteRule.Text = "刪";
            btnDeleteRule.Click += btnDeleteRule_Click;
            // 
            // btnAddRule
            // 
            btnAddRule.Location = new Point(295, 508);
            btnAddRule.Name = "btnAddRule";
            btnAddRule.Size = new Size(45, 30);
            btnAddRule.TabIndex = 4;
            btnAddRule.Text = "新";
            btnAddRule.Click += btnAddRule_Click;
            // 
            // txtTagUserID
            // 
            txtTagUserID.Location = new Point(125, 510);
            txtTagUserID.Name = "txtTagUserID";
            txtTagUserID.Size = new Size(160, 27);
            txtTagUserID.TabIndex = 5;
            // 
            // txtKeyword
            // 
            txtKeyword.Location = new Point(15, 510);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(100, 27);
            txtKeyword.TabIndex = 6;
            // 
            // labelRuleList
            // 
            labelRuleList.Location = new Point(15, 170);
            labelRuleList.Name = "labelRuleList";
            labelRuleList.Size = new Size(100, 23);
            labelRuleList.TabIndex = 7;
            labelRuleList.Text = "關鍵字規則:";
            // 
            // lstTagRules
            // 
            lstTagRules.Location = new Point(15, 195);
            lstTagRules.Name = "lstTagRules";
            lstTagRules.Size = new Size(378, 270);
            lstTagRules.TabIndex = 8;
            // 
            // txtTargetChannelId
            // 
            txtTargetChannelId.Enabled = false;
            txtTargetChannelId.Location = new Point(15, 120);
            txtTargetChannelId.Name = "txtTargetChannelId";
            txtTargetChannelId.Size = new Size(378, 27);
            txtTargetChannelId.TabIndex = 9;
            // 
            // labelTargetId
            // 
            labelTargetId.Location = new Point(15, 95);
            labelTargetId.Name = "labelTargetId";
            labelTargetId.Size = new Size(100, 23);
            labelTargetId.TabIndex = 10;
            labelTargetId.Text = "轉發目標頻道 ID:";
            // 
            // txtBotToken
            // 
            txtBotToken.Enabled = false;
            txtBotToken.Location = new Point(15, 55);
            txtBotToken.Name = "txtBotToken";
            txtBotToken.Size = new Size(378, 27);
            txtBotToken.TabIndex = 11;
            // 
            // labelBotToken
            // 
            labelBotToken.Location = new Point(15, 30);
            labelBotToken.Name = "labelBotToken";
            labelBotToken.Size = new Size(100, 23);
            labelBotToken.TabIndex = 12;
            labelBotToken.Text = "Bot Token:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 871);
            Controls.Add(grpAutoPost);
            Controls.Add(grpBotSettings);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Controls.Add(lstMessages);
            Controls.Add(txtLog);
            Controls.Add(txtTokens);
            Controls.Add(btnConnectWS);
            Controls.Add(btnDisconnectWS);
            Controls.Add(label7);
            Controls.Add(btnClearLogs);
            Name = "Form1";
            Text = "Chihlin - Discord 整合控制台 V2.1";
            Load += Form1_Load;
            grpAutoPost.ResumeLayout(false);
            grpAutoPost.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMinDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).EndInit();
            grpBotSettings.ResumeLayout(false);
            grpBotSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // 宣告控制項
        private TextBox txtTokens;
        private Button btnConnectWS;
        private Button btnDisconnectWS;
        private Label lblStatus;
        private Label label1;

        // 自動發文群組內容
        private GroupBox grpAutoPost;
        private TextBox txtChannels;
        private TextBox txtMessages;
        private Button btnStart;
        private Button btnStop;
        private NumericUpDown numMinDelay;
        private NumericUpDown numMaxDelay;
        private CheckBox chkEnableTimeLimit;
        private DateTimePicker dtpStartTime;
        private DateTimePicker dtpEndTime;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;

        // 監聽與日誌
        private ListBox lstMessages;
        private TextBox txtLog;
        private Label label7;
        private Button btnClearLogs;

        // 機器人設定群組
        private GroupBox grpBotSettings;
        private Label labelBotToken;
        private TextBox txtBotToken;
        private Label labelTargetId;
        private TextBox txtTargetChannelId;
        private ListBox lstTagRules;
        private Label labelRuleList;
        private TextBox txtKeyword;
        private TextBox txtTagUserID;
        private Button btnAddRule;
        private Button btnDeleteRule;
        private Label labelKey;
        private Label labelTagId;
        private Button button1;
    }
}