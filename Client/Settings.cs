using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Settings : Form
    {
        ClientForm parent;

        public Settings(ClientForm _parent)
        {
            InitializeComponent();
            parent = _parent;
            if(parent.isConnect)
            {
                textServer.Enabled = false;
                textPort.Enabled = false;
                textLogin.Enabled = false;
            }
        }

        private void butttonConnect_Click(object sender, EventArgs e)
        {
            save();
            Close();
        }

        private void textLogin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();
            }
        }

        private void save()
        {
            if (textServer.Text.Length > 0)
                this.parent.ConnectServer = textServer.Text;

            int t;
            if (textPort.Text.Length > 0 && int.TryParse(textPort.Text, out t))
                this.parent.ConnectPort = textPort.Text;

            if (textLogin.Text.Length > 0)
                this.parent.ConnectLogin = textLogin.Text;

            this.parent.Text = "Client - " + this.parent.ConnectLogin;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                Close();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.S))
            {
                save();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
