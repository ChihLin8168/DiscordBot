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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            lblStatus = new Label();
            label7 = new Label();
            btnFetchDMs = new Button();
            lstDMs = new ListBox();
            btnClearLogs = new Button();
            label6 = new Label();
            txtChatHistory = new TextBox();
            txtForwardChannelId = new TextBox();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)numMinDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).BeginInit();
            SuspendLayout();
            // 
            // txtTokens
            // 
            txtTokens.Location = new Point(14, 38);
            txtTokens.Margin = new Padding(3, 4, 3, 4);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.ScrollBars = ScrollBars.Vertical;
            txtTokens.Size = new Size(216, 74);
            txtTokens.TabIndex = 0;
            // 
            // btnConnectWS
            // 
            btnConnectWS.Location = new Point(236, 38);
            btnConnectWS.Margin = new Padding(3, 4, 3, 4);
            btnConnectWS.Name = "btnConnectWS";
            btnConnectWS.Size = new Size(76, 75);
            btnConnectWS.TabIndex = 1;
            btnConnectWS.Text = "登入\r\n監聽";
            btnConnectWS.UseVisualStyleBackColor = true;
            btnConnectWS.Click += btnConnectWS_Click;
            // 
            // txtChannels
            // 
            txtChannels.Location = new Point(14, 144);
            txtChannels.Margin = new Padding(3, 4, 3, 4);
            txtChannels.Multiline = true;
            txtChannels.Name = "txtChannels";
            txtChannels.ScrollBars = ScrollBars.Vertical;
            txtChannels.Size = new Size(298, 74);
            txtChannels.TabIndex = 2;
            // 
            // txtMessages
            // 
            txtMessages.Location = new Point(14, 250);
            txtMessages.Margin = new Padding(3, 4, 3, 4);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.Size = new Size(298, 74);
            txtMessages.TabIndex = 3;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(14, 437);
            btnStart.Margin = new Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(146, 43);
            btnStart.TabIndex = 4;
            btnStart.Text = "開始自動發文";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(166, 437);
            btnStop.Margin = new Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(146, 43);
            btnStop.TabIndex = 5;
            btnStop.Text = "停止任務";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.Black;
            txtLog.ForeColor = Color.Lime;
            txtLog.Location = new Point(14, 506);
            txtLog.Margin = new Padding(3, 4, 3, 4);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1051, 171);
            txtLog.TabIndex = 6;
            // 
            // numMinDelay
            // 
            numMinDelay.Location = new Point(97, 343);
            numMinDelay.Margin = new Padding(3, 4, 3, 4);
            numMinDelay.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            numMinDelay.Name = "numMinDelay";
            numMinDelay.Size = new Size(64, 23);
            numMinDelay.TabIndex = 7;
            // 
            // numMaxDelay
            // 
            numMaxDelay.Location = new Point(187, 343);
            numMaxDelay.Margin = new Padding(3, 4, 3, 4);
            numMaxDelay.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            numMaxDelay.Name = "numMaxDelay";
            numMaxDelay.Size = new Size(64, 23);
            numMaxDelay.TabIndex = 8;
            // 
            // chkEnableTimeLimit
            // 
            chkEnableTimeLimit.AutoSize = true;
            chkEnableTimeLimit.Location = new Point(14, 394);
            chkEnableTimeLimit.Margin = new Padding(3, 4, 3, 4);
            chkEnableTimeLimit.Name = "chkEnableTimeLimit";
            chkEnableTimeLimit.Size = new Size(98, 19);
            chkEnableTimeLimit.TabIndex = 9;
            chkEnableTimeLimit.Text = "限制執行時段";
            chkEnableTimeLimit.UseVisualStyleBackColor = true;
            // 
            // dtpStartTime
            // 
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.Location = new Point(107, 393);
            dtpStartTime.Margin = new Padding(3, 4, 3, 4);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Size = new Size(95, 23);
            dtpStartTime.TabIndex = 10;
            // 
            // dtpEndTime
            // 
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.Location = new Point(219, 393);
            dtpEndTime.Margin = new Padding(3, 4, 3, 4);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(97, 23);
            dtpEndTime.TabIndex = 11;
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(732, 38);
            lstMessages.Margin = new Padding(3, 4, 3, 4);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(333, 409);
            lstMessages.TabIndex = 12;
            lstMessages.SelectedIndexChanged += lstMessages_SelectedIndexChanged;
            // 
            // txtReply
            // 
            txtReply.Location = new Point(331, 421);
            txtReply.Margin = new Padding(3, 4, 3, 4);
            txtReply.Name = "txtReply";
            txtReply.Size = new Size(385, 23);
            txtReply.TabIndex = 13;
            // 
            // btnSendReply
            // 
            btnSendReply.Location = new Point(628, 458);
            btnSendReply.Margin = new Padding(3, 4, 3, 4);
            btnSendReply.Name = "btnSendReply";
            btnSendReply.Size = new Size(87, 32);
            btnSendReply.TabIndex = 14;
            btnSendReply.Text = "傳送回覆";
            btnSendReply.UseVisualStyleBackColor = true;
            btnSendReply.Click += btnSendReply_Click;
            // 
            // lblReplyTarget
            // 
            lblReplyTarget.AutoSize = true;
            lblReplyTarget.Font = new Font("新細明體", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            lblReplyTarget.Location = new Point(331, 405);
            lblReplyTarget.Name = "lblReplyTarget";
            lblReplyTarget.Size = new Size(122, 12);
            lblReplyTarget.TabIndex = 15;
            lblReplyTarget.Text = "回覆對象：尚未選擇";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 15);
            label1.Name = "label1";
            label1.Size = new Size(94, 15);
            label1.TabIndex = 17;
            label1.Text = "Tokens (每行一)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 125);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 18;
            label2.Text = "Channels (每行一)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 231);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 19;
            label3.Text = "發文內容";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(14, 345);
            label4.Name = "label4";
            label4.Size = new Size(81, 15);
            label4.TabIndex = 20;
            label4.Text = "延遲區間(秒): ";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(732, 7);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(67, 15);
            lblStatus.TabIndex = 22;
            lblStatus.Text = "狀態：離線";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(222, 9);
            label7.Name = "label7";
            label7.Size = new Size(82, 15);
            label7.TabIndex = 24;
            label7.Text = "目前對話視窗:";
            // 
            // btnFetchDMs
            // 
            btnFetchDMs.Location = new Point(331, 11);
            btnFetchDMs.Margin = new Padding(2);
            btnFetchDMs.Name = "btnFetchDMs";
            btnFetchDMs.Size = new Size(141, 24);
            btnFetchDMs.TabIndex = 0;
            btnFetchDMs.Text = "讀取現有私訊列表";
            btnFetchDMs.Click += btnFetchDMs_Click;
            // 
            // lstDMs
            // 
            lstDMs.Location = new Point(331, 36);
            lstDMs.Margin = new Padding(2);
            lstDMs.Name = "lstDMs";
            lstDMs.Size = new Size(141, 364);
            lstDMs.TabIndex = 1;
            lstDMs.SelectedIndexChanged += lstDMs_SelectedIndexChanged;
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(965, 452);
            btnClearLogs.Margin = new Padding(2);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(99, 23);
            btnClearLogs.TabIndex = 25;
            btnClearLogs.Text = "清除訊息";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // label6
            // 
            label6.Location = new Point(166, 346);
            label6.Name = "label6";
            label6.Size = new Size(13, 19);
            label6.TabIndex = 26;
            label6.Text = "~";
            // 
            // txtChatHistory
            // 
            txtChatHistory.BackColor = Color.White;
            txtChatHistory.Location = new Point(476, 13);
            txtChatHistory.Margin = new Padding(2);
            txtChatHistory.Multiline = true;
            txtChatHistory.Name = "txtChatHistory";
            txtChatHistory.ReadOnly = true;
            txtChatHistory.ScrollBars = ScrollBars.Vertical;
            txtChatHistory.Size = new Size(241, 387);
            txtChatHistory.TabIndex = 27;
            // 
            // txtForwardChannelId
            // 
            txtForwardChannelId.Location = new Point(331, 468);
            txtForwardChannelId.Margin = new Padding(2);
            txtForwardChannelId.Name = "txtForwardChannelId";
            txtForwardChannelId.Size = new Size(205, 23);
            txtForwardChannelId.TabIndex = 28;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("新細明體", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label5.Location = new Point(331, 450);
            label5.Name = "label5";
            label5.Size = new Size(57, 12);
            label5.TabIndex = 29;
            label5.Text = "發送頻道";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1074, 688);
            Controls.Add(label5);
            Controls.Add(txtForwardChannelId);
            Controls.Add(label6);
            Controls.Add(btnFetchDMs);
            Controls.Add(lstDMs);
            Controls.Add(lblStatus);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
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
            Controls.Add(txtChatHistory);
            Controls.Add(label7);
            Margin = new Padding(3, 4, 3, 4);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblStatus;
        private Label label7;
        private System.Windows.Forms.Button btnFetchDMs;
        private System.Windows.Forms.ListBox lstDMs;
        private System.Windows.Forms.Button btnClearLogs;
        private Label label6;
        private System.Windows.Forms.TextBox txtChatHistory;
        private TextBox txtForwardChannelId;
        private Label label5;
    }
}