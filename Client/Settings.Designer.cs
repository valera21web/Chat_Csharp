namespace Client
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.textLogin = new System.Windows.Forms.TextBox();
            this.textServer = new System.Windows.Forms.TextBox();
            this.butttonConnect = new System.Windows.Forms.Button();
            this.textPort = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbMulti = new System.Windows.Forms.RadioButton();
            this.rbOne = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(114, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = ":";
            // 
            // textLogin
            // 
            this.textLogin.Location = new System.Drawing.Point(166, 12);
            this.textLogin.Name = "textLogin";
            this.textLogin.Size = new System.Drawing.Size(66, 20);
            this.textLogin.TabIndex = 14;
            this.textLogin.Text = "myLogin";
            this.textLogin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textLogin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textLogin_KeyUp);
            // 
            // textServer
            // 
            this.textServer.Location = new System.Drawing.Point(12, 12);
            this.textServer.Name = "textServer";
            this.textServer.Size = new System.Drawing.Size(102, 20);
            this.textServer.TabIndex = 12;
            this.textServer.Text = "localhost";
            this.textServer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textLogin_KeyUp);
            // 
            // butttonConnect
            // 
            this.butttonConnect.Location = new System.Drawing.Point(155, 111);
            this.butttonConnect.Name = "butttonConnect";
            this.butttonConnect.Size = new System.Drawing.Size(75, 23);
            this.butttonConnect.TabIndex = 15;
            this.butttonConnect.Text = "Save";
            this.butttonConnect.UseVisualStyleBackColor = true;
            this.butttonConnect.Click += new System.EventHandler(this.butttonConnect_Click);
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(124, 12);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(36, 20);
            this.textPort.TabIndex = 13;
            this.textPort.Text = "8001";
            this.textPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textLogin_KeyUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMulti);
            this.groupBox1.Controls.Add(this.rbOne);
            this.groupBox1.Location = new System.Drawing.Point(14, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 47);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View";
            // 
            // rbMulti
            // 
            this.rbMulti.AutoSize = true;
            this.rbMulti.Enabled = false;
            this.rbMulti.Location = new System.Drawing.Point(110, 19);
            this.rbMulti.Name = "rbMulti";
            this.rbMulti.Size = new System.Drawing.Size(91, 17);
            this.rbMulti.TabIndex = 1;
            this.rbMulti.Text = "MultiWindows";
            this.rbMulti.UseVisualStyleBackColor = true;
            // 
            // rbOne
            // 
            this.rbOne.AutoSize = true;
            this.rbOne.Checked = true;
            this.rbOne.Location = new System.Drawing.Point(15, 19);
            this.rbOne.Name = "rbOne";
            this.rbOne.Size = new System.Drawing.Size(84, 17);
            this.rbOne.TabIndex = 0;
            this.rbOne.TabStop = true;
            this.rbOne.Text = "OneWindow";
            this.rbOne.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 146);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textLogin);
            this.Controls.Add(this.textServer);
            this.Controls.Add(this.butttonConnect);
            this.Controls.Add(this.textPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(258, 185);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(258, 185);
            this.Name = "Settings";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textLogin;
        private System.Windows.Forms.TextBox textServer;
        private System.Windows.Forms.Button butttonConnect;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMulti;
        private System.Windows.Forms.RadioButton rbOne;
    }
}