using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client
{
    public partial class ClientForm : Form
    {

        public delegate void CallAppendText(String str, int index);
        public delegate void CallDisconnect();
        public delegate void CallAddTab(String _name, User _user);

        private UTF32Encoding en = new UTF32Encoding();
        private TcpClient cln;
        private Thread thr;
        private bool Run;

        private Tabs tabs = new Tabs();
        private BindingList<User> listUsers = new BindingList<User>();

        public String ConnectServer = "localhost";
        public String ConnectPort = "8001";
        public String ConnectLogin = "MyLogin";
        public Boolean isConnect = false;


        public ClientForm()
        {
            InitializeComponent();
            listBoxUsers.DataSource = listUsers;
            textValue.ReadOnly = true;
            buttonSend.Enabled = false;
            this.Text = "Client - " + ConnectLogin;
            this.addTab("Public");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                fConnect();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.F))
            {
                new Settings(this).ShowDialog();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.N))
            {
                if (listUsers.Count > 0)
                    newTabUser(listBoxUsers.SelectedIndex);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Tab))
            {
                if (this.tabControl1.TabCount - 1 > this.tabControl1.SelectedIndex)
                    this.tabControl1.SelectedIndex = 
                        this.tabControl1.SelectedIndex + 1;
                else
                    this.tabControl1.SelectedIndex = 0;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Enter))
            {
                fSend(this.tabControl1.SelectedIndex);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.H))
            {
                showInfoHotKeys();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.I))
            {
                showInfo();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fConnect();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sendToServer(DataInfo.TYPE_DECONNECT, "");
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            fSend(this.tabControl1.SelectedIndex);
        }

        private void textValue_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (!textValue.ReadOnly && e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                fSend(this.tabControl1.SelectedIndex);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            String title = tabs.getTab(tabControl1.SelectedIndex).tabPage.Text;
            int index;
            if ((index = title.IndexOf('(')) != -1 &&
                title.Substring(title.Length - 1, 1) == ")")
            {
                tabs.getTab(tabControl1.SelectedIndex).tabPage.Text = title.Substring(0, index);
            }
        }

        private void buttonNewTab_Click(object sender, EventArgs e)
        {
            if (listUsers.Count > 0)
                newTabUser(listBoxUsers.SelectedIndex);
        }

        private void listBoxUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int _index = this.listBoxUsers.IndexFromPoint(e.Location);
            if (_index != System.Windows.Forms.ListBox.NoMatches)
            {
                newTabUser(_index);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Settings(this).ShowDialog();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInfo();
        }

        private void hotKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInfoHotKeys();
        }

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // check if the right mouse button was pressed
            if (e.Button == MouseButtons.Right)
            {
                // iterate through all the tab pages
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    // get their rectangle area and check if it contains the mouse cursor
                    Rectangle r = tabControl1.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        // show the context menu here
                        MessageBox.Show("TabPressed: " + i);
                    }
                }
            }
        }

        private void work()
        {
            
            byte[] bufer = new byte[1024];
            while (Run)
            {
                if (cln.Client.Poll(5, SelectMode.SelectRead))
                {
                    if (cln.Client.Available <= 0) break;
                    DataInfo dataInfo = DataInfo.ToObject(en.GetString(bufer, 0, cln.Client.Receive(bufer)));
                    switch (dataInfo.getType())
                    {

                        case DataInfo.TYPE_DECONNECT:
                            disconnect();
                            listUsers.Clear();
                            appendText("Disconnect with server;");
                            break;

                        case DataInfo.TYPE_MESSAGE:
                            appendText(dataInfo.getMessage(), 0);
                            break;

                        case DataInfo.TYPE_PRIVATE_MESSAGE:
                            String[] _str = dataInfo.getMessage().Split('|');
                            if (_str.Length >= 2)
                            {
                                String message = _str[1].Trim();
                                User _user = new User("", "");
                                foreach(User _use in listUsers)
                                {
                                    if (_use.IP.Equals(_str[0].Trim()))
                                    {
                                        _user = _use;
                                        break;
                                    }   
                                }
                                if (!_user.IP.Equals(""))
                                {
                                    int index = tabs.hasTab(_user.IP);
                                    if (index > 0)
                                    {
                                        String title = tabs.getTab(index).tabPage.Text;
                                        if(index != this.tabControl1.SelectedIndex)
                                        {
                                            int count = 1;
                                            int indexOf;
                                            if ((indexOf = title.IndexOf('(')) != -1 && 
                                                title.Substring(title.Length - 1, 1) == ")")
                                            {
                                                String coun_str = title.Substring(indexOf + 1, title.Length - indexOf - 2);
                                                if (coun_str != "10+")
                                                {
                                                    int.TryParse(coun_str, out count);
                                                    ++count;
                                                }
                                                else
                                                    count = 11;
                                            }
                                            tabs.getTab(index).tabPage.Text = 
                                                _user.Name + " (" + (count > 10 ? "10+" : count.ToString()) + ")";
                                            }
                                        else
                                        {
                                            int c = title.IndexOf('(');
                                            c = c == -1 ? title.Length : c;
                                            tabs.getTab(tabControl1.SelectedIndex).tabPage.Text = title.Substring(0, c);
                                        }
                                        
                                    }
                                    else
                                    {
                                        this.addTab(_user.Name, _user);
                                        index = tabs.hasTab(_user.IP);
                                        tabs.getTab(index).tabPage.Text = _user.Name + " (1)";
                                    }
                                    appendText(message, index);
                                }
                            }
                            break;

                        case DataInfo.TYPE_LIST_USERS:
                            String _users_info = dataInfo.getMessage();
                            String[] _users = _users_info.Split(';');
                            if(_users != null && _users.Length > 0)
                            {
                                foreach(String _user_str in _users)
                                {
                                    String[] _user = _user_str.Split('|');
                                    if (_user != null && _user.Length >= 2)
                                    {
                                        listUsers.Add(new User(_user[0].Trim(), _user[1].Trim()));
                                    }
                                }
                            }
                            
                            break;

                        case DataInfo.TYPE_ADD_USER:
                            String[] _user1 = dataInfo.getMessage().Split('|');
                            if (_user1 != null && _user1.Length >= 2)
                            {
                                User use = new User(_user1[0].Trim(), _user1[1].Trim());
                                listUsers.Add(use);
                                appendText(use.Name + " [" + use.IP + "] is connect");
                            }
                                
                            
                            break;

                        case DataInfo.TYPE_REMOVE_USERS:
                            String[] _user2 = dataInfo.getMessage().Split('|');
                            if (_user2 != null && _user2.Length >= 2)
                            {
                                User us = new User(_user2[0].Trim(), _user2[1].Trim());
                                listUsers.Remove(us);
                                appendText(us.Name + " [" + us.IP + "] is disconnect");
                            }
                            break;

                        default:
                            appendText(dataInfo.ToString());
                            break;
                    }
                }
            }
            try
            {
                cln.Client.Disconnect(true);
            }
            catch (Exception ex) { }
        }
        
        private void sendToServer(int type, String msg)
        {
            if (!Run)
                return;
            String send = new DataInfo(type, msg).ToString();
            byte[] bufer = new byte[1024];
            try
            {
                cln.Client.Send(en.GetBytes(send));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error send message to server!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void appendText(String str)
        {
            this.appendText(str, 0);
        }

        private void appendText(String str, int index)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new CallAppendText(appendText), new Object[] { str, index });
                else
                {
                    if (index < 0 && index >= tabs.Count())
                        index = 0;
                    TextBox t = tabs.getTextBox(index);
                    t.AppendText((t.TextLength > 0 ? Environment.NewLine : "") + str);
                }
                    
            }
            catch (ObjectDisposedException ex) { }
        }

        private void disconnect()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new CallDisconnect(disconnect), new Object[] { });
                }
                else
                {
                    textValue.ReadOnly = true;
                    buttonSend.Enabled = false;
                    isConnect = false;
                    Run = false;
                }
            }
            catch (ObjectDisposedException ex) { }
        }

        private void fConnect()
        {
            if (isConnect)
            {
                textValue.ReadOnly = true;
                buttonSend.Enabled = false;
                isConnect = false;
                connectToolStripMenuItem1.Text = "Connect";
                sendToServer(DataInfo.TYPE_DECONNECT, "");
            }
            else
            {
                if (ConnectServer.Length > 0 && ConnectPort.Length > 0 && ConnectLogin.Length > 0)
                {
                    try
                    {
                        cln = new TcpClient(ConnectServer, int.Parse(ConnectPort));
                        thr = new Thread(work);
                        thr.Start();

                        textValue.ReadOnly = false;
                        buttonSend.Enabled = true;
                        isConnect = true;
                        Run = true;
                        connectToolStripMenuItem1.Text = "Disconnect";

                        sendToServer(DataInfo.TYPE_LOGIN, ConnectLogin);
                        appendText("Connected with server");
                    }
                    catch (Exception ex)
                    {
                        disconnect();
                        MessageBox.Show(
                            ex.ToString(),
                            "Error connect to server!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                else 
                {
                    MessageBox.Show(
                        "Not valid values for connect to server",
                        "Not valid",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                
            } 
        }

        private void fSend(int index)
        {
            if (isConnect && textValue.Text.Length > 0)
            {
                if (index == 0)
                {
                    sendToServer(DataInfo.TYPE_MESSAGE, textValue.Text);
                } 
                else
                {
                    sendToServer(
                        DataInfo.TYPE_PRIVATE_MESSAGE,
                        tabs.getTab(index).user.IP + "|" + textValue.Text
                    );
                }
                appendText("ja: " + textValue.Text, index);
                textValue.Text = "";
            }
        }

        private void addTab(String _name, User _user)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new CallAddTab(addTab), new Object[] { _name, _user });
                else
                {
                    lock (tabs)
                    {
                        TextBox _textBox = new TextBox();
                        _textBox.Size = new System.Drawing.Size(190, 90);
                        _textBox.Anchor = ((System.Windows.Forms.AnchorStyles)
                            (((
                            (System.Windows.Forms.AnchorStyles.Top |
                                System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                        _textBox.Location = new System.Drawing.Point(6, 6);
                        _textBox.Multiline = true;
                        _textBox.ScrollBars = ScrollBars.Vertical;
                        _textBox.TabStop = true;
                        _textBox.Cursor = Cursors.Arrow;
                        _textBox.Name = "textBox_" + _name;
                        _textBox.ReadOnly = true;

                        TabPage _tabPage = new TabPage();
                        _tabPage.SuspendLayout();
                        _tabPage.ResumeLayout(false);
                        _tabPage.PerformLayout();
                        _tabPage.Name = _name;
                        _tabPage.Text = _name;
                        _tabPage.TabIndex = 20 + tabs.Count();
                        _tabPage.Controls.Add(_textBox);
                        _tabPage.Location = new System.Drawing.Point(4, 22);
                        _tabPage.Padding = new System.Windows.Forms.Padding(3);
                        _tabPage.Size = new System.Drawing.Size(291, 168);
                        _tabPage.UseVisualStyleBackColor = true;

                        tabs.Add(new Tab(_tabPage, _textBox, _user));
                        this.tabControl1.Controls.Add(_tabPage);
                    }
                }

            }
            catch (ObjectDisposedException ex) { }
        }

        private void addTab(String _name)
        {
            addTab(_name, new User("", ""));
        }

        private void newTabUser(int _index)
        {
            User _user = listUsers.ElementAt(_index);

            int ind = tabs.hasTab(_user.IP);
            if (ind == -1)
            {
                this.addTab(_user.Name, _user);
                ind = tabs.hasTab(_user.IP);
            }
            textValue.Focus();
            textValue.Select();
            this.tabControl1.SelectedIndex = ind;
        }

        public class DataInfo
        {
            public const int TYPE_CONNECT = 1;
            public const int TYPE_DECONNECT = 2;
            public const int TYPE_LOGIN = 3;
            public const int TYPE_MESSAGE = 4;
            public const int TYPE_LIST_USERS = 5;
            public const int TYPE_ADD_USER = 6;
            public const int TYPE_REMOVE_USERS = 7;
            public const int TYPE_PRIVATE_MESSAGE = 8;

            private int type;
            private String msg;

            public DataInfo(int _type, String _msg)
            {
                this.type = _type;
                this.msg = _msg;
            }

            public int getType()
            {
                return this.type;
            }

            public String getMessage()
            {
                return this.msg;
            }

            public override string ToString()
            {
                return "[" + this.type + "|" + this.msg + "]";
            }

            public static DataInfo ToObject(String deserializ)
            {
                int split;
                if (deserializ.Substring(0, 1) == "["
                    && deserializ.Substring(deserializ.Length - 1, 1) == "]"
                    && (split = deserializ.IndexOf("|")) != -1)
                {
                    String type = deserializ.Substring(1, split - 1);
                    String msg = deserializ.Substring(split + 1, (deserializ.Length - 1) - (split + 1));
                    return new DataInfo(int.Parse(type), msg);
                }
                return new DataInfo(-1, "");
            }
        }

        public class User
        {
            public String Name = "";
            public String IP = "";

            public User(String _IP, String _Name)
            {
                Name = _Name.Trim();
                IP = _IP.Trim();
            }

            public override string ToString()
            {
                return Name + " [" + IP + "]";
            }

            public override bool Equals(object obj)
            {
                return this.IP == ((User)obj).IP;
            }

            public override int GetHashCode()
            {
                return IP.GetHashCode();
            }
        }

        private class Tabs
        {
            private List<Tab> tabPages = new List<Tab>();

            public void Add(Tab _tab)
            {
                tabPages.Add(_tab);
            }

            public int hasTab(String IP)
            {
                if (tabPages.Count > 0)
                {
                    foreach (Tab _tab in tabPages)
                        if (IP.Equals(_tab.user.IP))
                            return _tab.tabPage.TabIndex - 20;
                }
                return -1;
            }


            public Tab getTab(int index)
            {
                if (index >= 0 && tabPages.Count > index)
                    return tabPages.ElementAt(index);
                return new Tab(new TabPage(), new TextBox());
            }

            public Tab getTabByIp(String IP)
            {
                if (tabPages.Count > 0)
                {
                    foreach (Tab _tab in tabPages)
                        if (IP.Equals(_tab.user.IP))
                            return _tab;
                }
                return new Tab(new TabPage(), new TextBox());
            }

            public TextBox getTextBox(int index)
            {
                if (tabPages.Count > index)
                    return tabPages.ElementAt(index).textBox;
                return new TextBox();
            }

            public TextBox getTextBoxByIp(String IP)
            {
                if (tabPages.Count > 0)
                {
                    foreach (Tab _tab in tabPages)
                        if (IP.Equals(_tab.user.IP))
                            return _tab.textBox;
                }
                return new TextBox();
            }

            public TabPage getTabPage(int index)
            {
                if (tabPages.Count > index)
                    return tabPages.ElementAt(index).tabPage;
                return new TabPage();
            }

            public TabPage getTabPageByIp(String IP)
            {
                if (tabPages.Count > 0)
                {
                    foreach (Tab _tab in tabPages)
                        if (IP.Equals(_tab.user.IP))
                            return _tab.tabPage;
                }
                return new TabPage();
            }
            public int Count()
            {
                return tabPages.Count;
            }
        }

        private class Tab
        {
            public User user;
            public TextBox textBox;
            public TabPage tabPage;

            public Tab(TabPage _tabPage, TextBox _textBox)
            {
                user = new User("" , "");
                textBox = _textBox;
                tabPage = _tabPage;
            }

            public Tab(TabPage _tabPage, TextBox _textBox, User _user)
            {
                user = _user;
                textBox = _textBox;
                tabPage = _tabPage;
            }
        }

        private void showInfo()
        {
            MessageBox.Show(
                "Developer Valerii Shtuvbeinyi\n2015",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


        private void showInfoHotKeys()
        {
            MessageBox.Show(
                "CTRL+D - Connect/Disconnect\n" +
                "CTRL+F - Open settings\n" +
                "CTRL+N - New tab with select user\n" +
                "CTRL+H - Open HotKeys\n" +
                "CTRL+I - Open Information\n" +
                "",
                "HotKeys",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
