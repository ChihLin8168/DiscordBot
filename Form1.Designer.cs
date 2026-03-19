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
            txtReply = new TextBox();
            btnSendReply = new Button();
            lblReplyTarget = new Label();
            txtForwardChannelId = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            lblStatus = new Label();
            label7 = new Label();
            btnFetchDMs = new Button();
            lstDMs = new ListBox();
            btnClearLogs = new Button();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)numMinDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).BeginInit();
            SuspendLayout();
            // 
            // txtTokens
            // 
            txtTokens.Location = new Point(18, 48);
            txtTokens.Margin = new Padding(4, 5, 4, 5);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.ScrollBars = ScrollBars.Vertical;
            txtTokens.Size = new Size(276, 93);
            txtTokens.TabIndex = 0;
            // 
            // btnConnectWS
            // 
            btnConnectWS.Location = new Point(304, 48);
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
            btnStart.Size = new Size(188, 55);
            btnStart.TabIndex = 4;
            btnStart.Text = "開始自動發文";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(214, 554);
            btnStop.Margin = new Padding(4, 5, 4, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(188, 55);
            btnStop.TabIndex = 5;
            btnStop.Text = "停止任務";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.Black;
            txtLog.ForeColor = Color.Lime;
            txtLog.Location = new Point(18, 633);
            txtLog.Margin = new Padding(4, 5, 4, 5);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1138, 216);
            txtLog.TabIndex = 6;
            // 
            // numMinDelay
            // 
            numMinDelay.Location = new Point(125, 435);
            numMinDelay.Margin = new Padding(4, 5, 4, 5);
            numMinDelay.Name = "numMinDelay";
            numMinDelay.Size = new Size(82, 27);
            numMinDelay.TabIndex = 7;
            // 
            // numMaxDelay
            // 
            numMaxDelay.Location = new Point(241, 435);
            numMaxDelay.Margin = new Padding(4, 5, 4, 5);
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
            dtpEndTime.Location = new Point(281, 498);
            dtpEndTime.Margin = new Padding(4, 5, 4, 5);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(124, 27);
            dtpEndTime.TabIndex = 11;
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(428, 48);
            lstMessages.Margin = new Padding(4, 5, 4, 5);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(461, 403);
            lstMessages.TabIndex = 12;
            lstMessages.SelectedIndexChanged += lstMessages_SelectedIndexChanged;
            // 
            // txtReply
            // 
            txtReply.Location = new Point(428, 562);
            txtReply.Margin = new Padding(4, 5, 4, 5);
            txtReply.Name = "txtReply";
            txtReply.Size = new Size(607, 27);
            txtReply.TabIndex = 13;
            // 
            // btnSendReply
            // 
            btnSendReply.Location = new Point(1044, 554);
            btnSendReply.Margin = new Padding(4, 5, 4, 5);
            btnSendReply.Name = "btnSendReply";
            btnSendReply.Size = new Size(112, 40);
            btnSendReply.TabIndex = 14;
            btnSendReply.Text = "傳送回覆";
            btnSendReply.UseVisualStyleBackColor = true;
            btnSendReply.Click += btnSendReply_Click;
            // 
            // lblReplyTarget
            // 
            lblReplyTarget.AutoSize = true;
            lblReplyTarget.Font = new Font("新細明體", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            lblReplyTarget.Location = new Point(428, 542);
            lblReplyTarget.Margin = new Padding(4, 0, 4, 0);
            lblReplyTarget.Name = "lblReplyTarget";
            lblReplyTarget.Size = new Size(151, 15);
            lblReplyTarget.TabIndex = 15;
            lblReplyTarget.Text = "回覆對象：尚未選擇";
            // 
            // txtForwardChannelId
            // 
            txtForwardChannelId.Location = new Point(553, 495);
            txtForwardChannelId.Margin = new Padding(4, 5, 4, 5);
            txtForwardChannelId.Name = "txtForwardChannelId";
            txtForwardChannelId.Size = new Size(202, 27);
            txtForwardChannelId.TabIndex = 16;
            txtForwardChannelId.Text = "1484034366685450240";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 19);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(117, 19);
            label1.TabIndex = 17;
            label1.Text = "Tokens (每行一)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 158);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(132, 19);
            label2.TabIndex = 18;
            label2.Text = "Channels (每行一)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 293);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(69, 19);
            label3.TabIndex = 19;
            label3.Text = "發文內容";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 437);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(101, 19);
            label4.TabIndex = 20;
            label4.Text = "延遲區間(秒): ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(424, 498);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(121, 19);
            label5.TabIndex = 21;
            label5.Text = "私訊轉發頻道 ID:";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(424, 19);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(84, 19);
            lblStatus.TabIndex = 22;
            lblStatus.Text = "狀態：離線";
            // 
            // label7
            // 
            label7.Location = new Point(259, 500);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(17, 24);
            label7.TabIndex = 24;
            label7.Text = "~";
            // 
            // btnFetchDMs
            // 
            btnFetchDMs.Location = new Point(907, 36);
            btnFetchDMs.Name = "btnFetchDMs";
            btnFetchDMs.Size = new Size(180, 31);
            btnFetchDMs.TabIndex = 0;
            btnFetchDMs.Text = "讀取現有私訊列表";
            btnFetchDMs.Click += btnFetchDMs_Click;
            // 
            // lstDMs
            // 
            lstDMs.Location = new Point(907, 67);
            lstDMs.Name = "lstDMs";
            lstDMs.Size = new Size(180, 384);
            lstDMs.TabIndex = 1;
            lstDMs.SelectedIndexChanged += lstDMs_SelectedIndexChanged;
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(762, 451);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(127, 29);
            btnClearLogs.TabIndex = 25;
            btnClearLogs.Text = "清除訊息";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // label6
            // 
            label6.Location = new Point(214, 438);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(17, 24);
            label6.TabIndex = 26;
            label6.Text = "~";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1176, 871);
            Controls.Add(label6);
            Controls.Add(btnFetchDMs);
            Controls.Add(lstDMs);
            Controls.Add(label7);
            Controls.Add(lblStatus);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtForwardChannelId);
            Controls.Add(lblReplyTarget);
            Controls.Add(btnSendReply);
            Controls.Add(txtReply);
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
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Chihlin - Discord 整合控制台 V2";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numMinDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.TextBox txtReply;
        private System.Windows.Forms.Button btnSendReply;
        private System.Windows.Forms.Label lblReplyTarget;
        private System.Windows.Forms.TextBox txtForwardChannelId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblStatus;
        private Label label7;
        private System.Windows.Forms.Button btnFetchDMs;
        private System.Windows.Forms.ListBox lstDMs;
        private System.Windows.Forms.Button btnClearLogs;
        private Label label6;
    }
}