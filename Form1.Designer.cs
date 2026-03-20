namespace DiscordAutoChat
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveConfig(); // 關閉前自動存檔
            base.OnFormClosing(e);
        }
        private void InitializeComponent()
        {
            txtTokens = new TextBox();
            btnConnectWS = new Button();
            txtChannels = new TextBox();
            txtMessages = new TextBox();
            btnStart = new Button();
            btnStop = new Button();
            txtLog = new TextBox();
            numMinDelay = new NumericUpDown();
            numMaxDelay = new NumericUpDown();
            chkEnableTimeLimit = new CheckBox();
            dtpStartTime = new DateTimePicker();
            dtpEndTime = new DateTimePicker();
            lstMessages = new ListBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            lblStatus = new Label();
            label7 = new Label();
            btnClearLogs = new Button();
            label6 = new Label();
            grpBotSettings = new GroupBox();
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
            ((System.ComponentModel.ISupportInitialize)numMinDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).BeginInit();
            grpBotSettings.SuspendLayout();
            SuspendLayout();
            // 
            // txtTokens
            // 
            txtTokens.Location = new Point(18, 48);
            txtTokens.Margin = new Padding(4, 5, 4, 5);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.ScrollBars = ScrollBars.Vertical;
            txtTokens.Size = new Size(277, 93);
            txtTokens.TabIndex = 0;
            // 
            // btnConnectWS
            // 
            btnConnectWS.Location = new Point(303, 48);
            btnConnectWS.Margin = new Padding(4, 5, 4, 5);
            btnConnectWS.Name = "btnConnectWS";
            btnConnectWS.Size = new Size(98, 95);
            btnConnectWS.TabIndex = 1;
            btnConnectWS.Text = "登入\r\n監聽";
            btnConnectWS.UseVisualStyleBackColor = true;
            btnConnectWS.Click += btnConnectWS_Click;
            // 
            // txtChannels
            // 
            txtChannels.Location = new Point(18, 182);
            txtChannels.Margin = new Padding(4, 5, 4, 5);
            txtChannels.Multiline = true;
            txtChannels.Name = "txtChannels";
            txtChannels.ScrollBars = ScrollBars.Vertical;
            txtChannels.Size = new Size(382, 93);
            txtChannels.TabIndex = 2;
            // 
            // txtMessages
            // 
            txtMessages.Location = new Point(18, 317);
            txtMessages.Margin = new Padding(4, 5, 4, 5);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.Size = new Size(382, 93);
            txtMessages.TabIndex = 3;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(18, 554);
            btnStart.Margin = new Padding(4, 5, 4, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(188, 54);
            btnStart.TabIndex = 4;
            btnStart.Text = "開始自動發文";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(213, 554);
            btnStop.Margin = new Padding(4, 5, 4, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(188, 54);
            btnStop.TabIndex = 5;
            btnStop.Text = "停止任務";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.Black;
            txtLog.ForeColor = Color.Lime;
            txtLog.Location = new Point(18, 641);
            txtLog.Margin = new Padding(4, 5, 4, 5);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1250, 216);
            txtLog.TabIndex = 6;
            // 
            // numMinDelay
            // 
            numMinDelay.Location = new Point(125, 434);
            numMinDelay.Margin = new Padding(4, 5, 4, 5);
            numMinDelay.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            numMinDelay.Name = "numMinDelay";
            numMinDelay.Size = new Size(82, 27);
            numMinDelay.TabIndex = 7;
            // 
            // numMaxDelay
            // 
            numMaxDelay.Location = new Point(240, 434);
            numMaxDelay.Margin = new Padding(4, 5, 4, 5);
            numMaxDelay.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            numMaxDelay.Name = "numMaxDelay";
            numMaxDelay.Size = new Size(82, 27);
            numMaxDelay.TabIndex = 8;
            // 
            // chkEnableTimeLimit
            // 
            chkEnableTimeLimit.AutoSize = true;
            chkEnableTimeLimit.Location = new Point(18, 499);
            chkEnableTimeLimit.Margin = new Padding(4, 5, 4, 5);
            chkEnableTimeLimit.Name = "chkEnableTimeLimit";
            chkEnableTimeLimit.Size = new Size(121, 23);
            chkEnableTimeLimit.TabIndex = 9;
            chkEnableTimeLimit.Text = "限制執行時段";
            chkEnableTimeLimit.UseVisualStyleBackColor = true;
            // 
            // dtpStartTime
            // 
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.Location = new Point(138, 498);
            dtpStartTime.Margin = new Padding(4, 5, 4, 5);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Size = new Size(121, 27);
            dtpStartTime.TabIndex = 10;
            // 
            // dtpEndTime
            // 
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.Location = new Point(282, 498);
            dtpEndTime.Margin = new Padding(4, 5, 4, 5);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(124, 27);
            dtpEndTime.TabIndex = 11;
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(414, 48);
            lstMessages.Margin = new Padding(4, 5, 4, 5);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(427, 517);
            lstMessages.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 19);
            label1.Name = "label1";
            label1.Size = new Size(117, 19);
            label1.TabIndex = 33;
            label1.Text = "Tokens (每行一)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 158);
            label2.Name = "label2";
            label2.Size = new Size(132, 19);
            label2.TabIndex = 32;
            label2.Text = "Channels (每行一)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 293);
            label3.Name = "label3";
            label3.Size = new Size(69, 19);
            label3.TabIndex = 31;
            label3.Text = "發文內容";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 437);
            label4.Name = "label4";
            label4.Size = new Size(101, 19);
            label4.TabIndex = 30;
            label4.Text = "延遲區間(秒): ";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(414, 9);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(84, 19);
            lblStatus.TabIndex = 29;
            lblStatus.Text = "狀態：離線";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(285, 11);
            label7.Name = "label7";
            label7.Size = new Size(102, 19);
            label7.TabIndex = 34;
            label7.Text = "目前對話視窗:";
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(1141, 607);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(127, 29);
            btnClearLogs.TabIndex = 25;
            btnClearLogs.Text = "清除訊息";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // label6
            // 
            label6.Location = new Point(213, 438);
            label6.Name = "label6";
            label6.Size = new Size(20, 23);
            label6.TabIndex = 28;
            label6.Text = "~";
            // 
            // grpBotSettings
            // 
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
            grpBotSettings.TabIndex = 27;
            grpBotSettings.TabStop = false;
            grpBotSettings.Text = "機器人轉發與關鍵字 Tag 管理";
            // 
            // labelTagId
            // 
            labelTagId.AutoSize = true;
            labelTagId.Location = new Point(125, 485);
            labelTagId.Name = "labelTagId";
            labelTagId.Size = new Size(95, 19);
            labelTagId.TabIndex = 0;
            labelTagId.Text = "要 Tag 的 ID:";
            // 
            // labelKey
            // 
            labelKey.AutoSize = true;
            labelKey.Location = new Point(15, 485);
            labelKey.Name = "labelKey";
            labelKey.Size = new Size(57, 19);
            labelKey.TabIndex = 1;
            labelKey.Text = "關鍵字:";
            // 
            // btnDeleteRule
            // 
            btnDeleteRule.Location = new Point(345, 508);
            btnDeleteRule.Name = "btnDeleteRule";
            btnDeleteRule.Size = new Size(45, 30);
            btnDeleteRule.TabIndex = 2;
            btnDeleteRule.Text = "刪除";
            btnDeleteRule.Click += btnDeleteRule_Click;
            // 
            // btnAddRule
            // 
            btnAddRule.Location = new Point(295, 508);
            btnAddRule.Name = "btnAddRule";
            btnAddRule.Size = new Size(45, 30);
            btnAddRule.TabIndex = 3;
            btnAddRule.Text = "新增";
            btnAddRule.Click += btnAddRule_Click;
            // 
            // txtTagUserID
            // 
            txtTagUserID.Location = new Point(125, 510);
            txtTagUserID.Name = "txtTagUserID";
            txtTagUserID.Size = new Size(160, 27);
            txtTagUserID.TabIndex = 4;
            // 
            // txtKeyword
            // 
            txtKeyword.Location = new Point(15, 510);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(100, 27);
            txtKeyword.TabIndex = 5;
            // 
            // labelRuleList
            // 
            labelRuleList.AutoSize = true;
            labelRuleList.Location = new Point(15, 170);
            labelRuleList.Name = "labelRuleList";
            labelRuleList.Size = new Size(151, 19);
            labelRuleList.TabIndex = 6;
            labelRuleList.Text = "關鍵字 Tag 規則清單:";
            // 
            // lstTagRules
            // 
            lstTagRules.FormattingEnabled = true;
            lstTagRules.Location = new Point(15, 195);
            lstTagRules.Name = "lstTagRules";
            lstTagRules.Size = new Size(378, 270);
            lstTagRules.TabIndex = 7;
            // 
            // txtTargetChannelId
            // 
            txtTargetChannelId.Location = new Point(15, 120);
            txtTargetChannelId.Name = "txtTargetChannelId";
            txtTargetChannelId.Size = new Size(378, 27);
            txtTargetChannelId.TabIndex = 8;
            txtTargetChannelId.Text = "1484034366685450240";
            // 
            // labelTargetId
            // 
            labelTargetId.AutoSize = true;
            labelTargetId.Location = new Point(15, 95);
            labelTargetId.Name = "labelTargetId";
            labelTargetId.Size = new Size(180, 19);
            labelTargetId.TabIndex = 9;
            labelTargetId.Text = "轉發目標頻道 ID (管理用):";
            // 
            // txtBotToken
            // 
            txtBotToken.Location = new Point(15, 55);
            txtBotToken.Name = "txtBotToken";
            txtBotToken.Size = new Size(378, 27);
            txtBotToken.TabIndex = 10;
            txtBotToken.Text = "MTQ4Mzc0MzcxNDY3MjExNTcxMg.G3V6QC.PvoRARBnAXlc1H9RWtCUlXVo05Z5J-7nj_S2HM";
            // 
            // labelBotToken
            // 
            labelBotToken.AutoSize = true;
            labelBotToken.Location = new Point(15, 30);
            labelBotToken.Name = "labelBotToken";
            labelBotToken.Size = new Size(81, 19);
            labelBotToken.TabIndex = 11;
            labelBotToken.Text = "Bot Token:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 871);
            Controls.Add(grpBotSettings);
            Controls.Add(label6);
            Controls.Add(lblStatus);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lstMessages);
            Controls.Add(dtpEndTime);
            Controls.Add(dtpStartTime);
            Controls.Add(chkEnableTimeLimit);
            Controls.Add(numMaxDelay);
            Controls.Add(numMinDelay);
            Controls.Add(txtLog);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(txtMessages);
            Controls.Add(txtChannels);
            Controls.Add(btnConnectWS);
            Controls.Add(txtTokens);
            Controls.Add(btnClearLogs);
            Controls.Add(label7);
            Name = "Form1";
            Text = "Chihlin - Discord 整合控制台 V2";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numMinDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).EndInit();
            grpBotSettings.ResumeLayout(false);
            grpBotSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // 原本的組件
        private System.Windows.Forms.TextBox txtTokens;
        private System.Windows.Forms.Button btnConnectWS;
        private System.Windows.Forms.TextBox txtChannels;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.NumericUpDown numMinDelay;
        private System.Windows.Forms.NumericUpDown numMaxDelay;
        private System.Windows.Forms.CheckBox chkEnableTimeLimit;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblStatus;
        private Label label7;
        private System.Windows.Forms.Button btnClearLogs;
        private Label label6;

        // 新增組件
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
    }
}