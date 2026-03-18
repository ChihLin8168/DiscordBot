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

        private void InitializeComponent()
        {
            txtChannels = new TextBox();
            txtTokens = new TextBox();
            txtMessages = new TextBox();
            btnStart = new Button();
            btnStop = new Button();
            txtLog = new RichTextBox();
            numMinDelay = new NumericUpDown();
            numMaxDelay = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            chkEnableTimeLimit = new CheckBox();
            dtpStartTime = new DateTimePicker();
            lblTimeRange = new Label();
            dtpEndTime = new DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)numMinDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).BeginInit();
            SuspendLayout();
            // 
            // txtChannels
            // 
            txtChannels.Location = new Point(19, 38);
            txtChannels.Margin = new Padding(4);
            txtChannels.Multiline = true;
            txtChannels.Name = "txtChannels";
            txtChannels.ScrollBars = ScrollBars.Vertical;
            txtChannels.Size = new Size(265, 40);
            txtChannels.TabIndex = 0;
            // 
            // txtTokens
            // 
            txtTokens.Location = new Point(296, 38);
            txtTokens.Margin = new Padding(4);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.ScrollBars = ScrollBars.Vertical;
            txtTokens.Size = new Size(433, 40);
            txtTokens.TabIndex = 1;
            // 
            // txtMessages
            // 
            txtMessages.Location = new Point(21, 123);
            txtMessages.Margin = new Padding(4);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(725, 171);
            txtMessages.TabIndex = 2;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.PaleGreen;
            btnStart.Location = new Point(527, 323);
            btnStart.Margin = new Padding(4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(109, 44);
            btnStart.TabIndex = 3;
            btnStart.Text = "開始執行";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(643, 323);
            btnStop.Margin = new Padding(4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(103, 44);
            btnStop.TabIndex = 4;
            btnStop.Text = "停止";
            btnStop.Click += btnStop_Click;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.Black;
            txtLog.ForeColor = Color.Lime;
            txtLog.Location = new Point(19, 386);
            txtLog.Margin = new Padding(4);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(725, 227);
            txtLog.TabIndex = 5;
            txtLog.Text = "";
            // 
            // numMinDelay
            // 
            numMinDelay.Location = new Point(132, 350);
            numMinDelay.Margin = new Padding(4);
            numMinDelay.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numMinDelay.Name = "numMinDelay";
            numMinDelay.Size = new Size(88, 27);
            numMinDelay.TabIndex = 6;
            numMinDelay.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // numMaxDelay
            // 
            numMaxDelay.Location = new Point(254, 350);
            numMaxDelay.Margin = new Padding(4);
            numMaxDelay.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numMaxDelay.Name = "numMaxDelay";
            numMaxDelay.Size = new Size(77, 27);
            numMaxDelay.TabIndex = 7;
            numMaxDelay.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // label1
            // 
            label1.Location = new Point(15, 15);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(203, 29);
            label1.TabIndex = 8;
            label1.Text = "頻道 ID (一行一個)";
            // 
            // label2
            // 
            label2.Location = new Point(292, 15);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(129, 29);
            label2.TabIndex = 9;
            label2.Text = "Authorization Tokens (一行一個)";
            // 
            // label3
            // 
            label3.Location = new Point(19, 91);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(432, 28);
            label3.TabIndex = 10;
            label3.Text = "自定義發送訊息 (隨機抽取)";
            // 
            // label4
            // 
            label4.Location = new Point(21, 353);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(103, 29);
            label4.TabIndex = 1;
            label4.Text = "間隔時間(秒):";
            // 
            // label5
            // 
            label5.Location = new Point(228, 353);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(18, 24);
            label5.TabIndex = 0;
            label5.Text = "~";
            // 
            // chkEnableTimeLimit
            // 
            chkEnableTimeLimit.AutoSize = true;
            chkEnableTimeLimit.Location = new Point(21, 320);
            chkEnableTimeLimit.Name = "chkEnableTimeLimit";
            chkEnableTimeLimit.Size = new Size(121, 23);
            chkEnableTimeLimit.TabIndex = 11;
            chkEnableTimeLimit.Text = "啟用時段限制";
            // 
            // dtpStartTime
            // 
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.Location = new Point(143, 316);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Size = new Size(100, 27);
            dtpStartTime.TabIndex = 12;
            // 
            // lblTimeRange
            // 
            lblTimeRange.AutoSize = true;
            lblTimeRange.Location = new Point(249, 318);
            lblTimeRange.Name = "lblTimeRange";
            lblTimeRange.Size = new Size(24, 19);
            lblTimeRange.TabIndex = 13;
            lblTimeRange.Text = "至";
            // 
            // dtpEndTime
            // 
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.Location = new Point(274, 315);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(100, 27);
            dtpEndTime.TabIndex = 14;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(771, 633);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(numMaxDelay);
            Controls.Add(numMinDelay);
            Controls.Add(txtLog);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(txtMessages);
            Controls.Add(txtTokens);
            Controls.Add(txtChannels);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(chkEnableTimeLimit);
            Controls.Add(dtpStartTime);
            Controls.Add(lblTimeRange);
            Controls.Add(dtpEndTime);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            Name = "Form1";
            Text = "ChihLin Discord 推播系統";
            ((System.ComponentModel.ISupportInitialize)numMinDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxDelay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtChannels;
        private System.Windows.Forms.TextBox txtTokens;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.NumericUpDown numMinDelay;
        private System.Windows.Forms.NumericUpDown numMaxDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Label lblTimeRange;
        private System.Windows.Forms.CheckBox chkEnableTimeLimit;
    }
}