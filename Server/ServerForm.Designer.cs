namespace Server
{
    partial class ServerForm
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
            this.textPort = new System.Windows.Forms.TextBox();
            this.butttonConnect = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(12, 12);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(100, 20);
            this.textPort.TabIndex = 1;
            this.textPort.Text = "8001";
            this.textPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textPort_KeyUp);
            // 
            // butttonConnect
            // 
            this.butttonConnect.Location = new System.Drawing.Point(118, 10);
            this.butttonConnect.Name = "butttonConnect";
            this.butttonConnect.Size = new System.Drawing.Size(75, 23);
            this.butttonConnect.TabIndex = 1;
            this.butttonConnect.Text = "Connect";
            this.butttonConnect.UseVisualStyleBackColor = true;
            this.butttonConnect.Click += new System.EventHandler(this.butttonConnect_Click);
            // 
            // logBox
            // 
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.Location = new System.Drawing.Point(13, 39);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(259, 210);
            this.logBox.TabIndex = 2;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.butttonConnect);
            this.Controls.Add(this.textPort);
            this.Name = "ServerForm";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Button butttonConnect;
        private System.Windows.Forms.TextBox logBox;
    }
}

