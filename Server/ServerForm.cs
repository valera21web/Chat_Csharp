using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace Server
{
    public partial class ServerForm : Form
    {
        public delegate void CallAppendText(String str);
        public delegate void CallStop();
        public delegate void CallDisconnect();

        private TcpListener srv;
        private Thread thr;
        private bool Run;
        private List<Socket> listSocet = new List<Socket>();
        private Users users = new Users();
        private UTF32Encoding en = new UTF32Encoding();
        private byte[] bufer = new byte[1024];

        public ServerForm()
        {
            InitializeComponent();
            fConnect();
        }

        private void butttonConnect_Click(object sender, EventArgs e)
        {
            fConnect();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            disconnect();
            while (Run);
        }

        private void textPort_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fConnect();
            }
        }

        private void work() 
        {
            srv.Start();
            while(Run) 
            {
                if(srv.Pending()) 
                {
                    new Thread(client).Start(srv.AcceptSocket());
                }
            }
            srv.Stop();
            Stop();
        }

        private void client(Object param)
        {
            Socket sck = param as Socket;
            
            try
            {
                lock(thr)
                {
                    listSocet.Add(sck);
                }
                String ip = sck.RemoteEndPoint.ToString();
                User UserNow = new User(ip, "", sck);
                if (!users.hasInList(UserNow.IP))
                {
                    sentToClient(UserNow.socket, new DataInfo(DataInfo.TYPE_LIST_USERS, users.ToString()).ToString());
                    users.add(UserNow);
                }
                    
                appendText("Cliend is connect: " + ip);

                while(Run && sck.Connected) 
                {
                    if(sck.Poll(5, SelectMode.SelectRead)) 
                    {
                        if (sck.Available <= 0) break;
                        DataInfo dataInfo = DataInfo.ToObject(en.GetString(bufer, 0, sck.Receive(bufer)));
                        String msg;
                        switch(dataInfo.getType())
                        {
                            case DataInfo.TYPE_LOGIN:
                                users.editName(ip, dataInfo.getMessage());
                                UserNow.Name = dataInfo.getMessage();
                                sentToClients(UserNow.socket, 
                                    new DataInfo(
                                        DataInfo.TYPE_ADD_USER,
                                        UserNow.ToString()
                                    ).ToString()
                                );
                                appendText(UserNow.IP + " => " + UserNow.Name);
                                break;

                            case DataInfo.TYPE_DECONNECT:
                                appendText("Cliend is disconnect: " + UserNow.IP);
                                sentToClient(sck, new DataInfo(DataInfo.TYPE_DECONNECT, "").ToString());
                                users.remove(UserNow);
                                sentToClients(sck,
                                    new DataInfo(
                                        DataInfo.TYPE_REMOVE_USERS,
                                        UserNow.ToString()
                                    ).ToString()
                                );
                                break;

                            case DataInfo.TYPE_MESSAGE:
                                msg = UserNow.Name + ": " + dataInfo.getMessage();
                                appendText(msg);
                                sentToClients(UserNow.socket, new DataInfo(DataInfo.TYPE_MESSAGE, msg).ToString());
                                break;

                            case DataInfo.TYPE_PRIVATE_MESSAGE:
                                String[] _str = dataInfo.getMessage().Split('|');
                                if(_str.Length >= 2) 
                                {
                                    msg = UserNow.IP + "|" + UserNow.Name + ": " + _str[1];
                                    appendText(
                                        UserNow.Name + " to " + users.getName(_str[0]) 
                                        + ": " + _str[1]
                                    );
                                    sentToClient(users.getSocket(_str[0]), new DataInfo(DataInfo.TYPE_PRIVATE_MESSAGE, msg).ToString());
                                }
                                break;
                        }
                    }
                }
            } finally {
                try
                {
                    listSocet.Remove(sck);
                }
                catch (Exception e) { }
            }
        }

        private void sentToClients(Socket soc, String message)
        {
            lock (listSocet)
            {
                try
                {
                    if (listSocet != null && listSocet.Count > 0)
                        foreach (Socket s in listSocet)
                            if (s != soc) 
                                sentToClient(s, message);
                }
                catch (Exception ex) { }
            }
        }

        private void sentToClients(String message)
        {
            lock (listSocet)
            {
                try
                {
                    if (listSocet != null && listSocet.Count > 0)
                        foreach (Socket s in listSocet)
                            sentToClient(s, message);
                }
                catch (Exception ex) { }
            }
        }

        private void sentToClient(Socket soc, String message)
        {
            try
            {
                soc.Send(en.GetBytes(message));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error send message to client!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void appendText(String str)
        {
            if (InvokeRequired) 
            {
                Invoke(new CallAppendText(appendText), new Object[] {str});
            } else
                logBox.AppendText(str + Environment.NewLine);
        }

        private void disconnect()
        {
            if (InvokeRequired)
            {
                Invoke(new CallDisconnect(disconnect), new Object[] { });
            }
            else
            {
                textPort.ReadOnly = false;
                butttonConnect.Text = "Connect";
                Run = false;
            }
        }

        private void Stop()
        {
            if (InvokeRequired)
            {
                Invoke(new CallStop(Stop), new Object[] { });
            }
            else
            {
                lock (listSocet)
                {
                    sentToClients(new DataInfo(DataInfo.TYPE_DECONNECT, "").ToString());
                    if (textPort.ReadOnly)
                        appendText("Server is disconnect");
                    users.Clear();
                }
                disconnect();
            }
        }

        private void fConnect()
        {
            if (textPort.ReadOnly)
            {
                Stop();
            }
            else
            {
                int port;
                if (int.TryParse(textPort.Text, out port))
                {
                    textPort.ReadOnly = true;
                    butttonConnect.Text = "Disconnect";
                    Run = true;
                    try
                    {
                        srv = new TcpListener(IPAddress.Any, port);
                        thr = new Thread(work);
                        thr.Start();
                    }
                    catch (Exception ex)
                    {
                        disconnect();
                        MessageBox.Show(
                            ex.Message,
                            "Error init server",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }   
        }

        private class Users
        {
            private List<User> list = new List<User>();

            public String getName(String IP)
            {
                if (list != null && list.Count > 0)
                    foreach(User user in list)
                        if (IP.Equals(user.IP))
                            return user.Name;
                return "";
            }

            public Socket getSocket(String IP)
            {
                if (list != null && list.Count > 0)
                    foreach (User user in list)
                        if (IP.Equals(user.IP))
                            return user.socket;
                return null;
            }

            public void add(String IP, String Name, Socket socket)
            {
                this.add(new User(IP, Name, socket));
            }

            public void add(User _user)
            {
                if (!list.Contains(_user))
                {
                    list.Add(_user);
                }
            }

            public void editName(String IP, String Name)
            {
                if (list != null && list.Count > 0)
                    foreach (User user in list)
                        if (IP.Equals(user.IP))
                            user.Name = Name;
            }

            public bool hasInList(String IP)
            {
                if (list != null && list.Count > 0)
                    foreach (User user in list)
                        if (IP.Equals(user.IP)) 
                            return true;

                return false;
            }

            public void remove(String IP)
            {
                if (list != null && list.Count > 0)
                {
                    try
                    {
                        foreach (User user in list)
                            if (IP.Equals(user.IP))
                                list.Remove(user);
                    }
                    catch (InvalidOperationException e) { }
                }
            }

            public void remove(User _user)
            {
                if (list != null && list.Count > 0)
                {
                    try
                    {
                        list.Remove(_user);
                    }
                    catch (InvalidOperationException e) { }
                }
            }

            public override string ToString()
            {
                String str = "";
                foreach (User user in list)
                    str += user.ToString() + "; ";
                return str;
            }

            public void Clear()
            {
                list.Clear();
            }
        }

        private class User
        {
            public String Name = "";
            public String IP = "";
            public Socket socket;

            public User(String _IP, String _Name, Socket _socket)
            {
                Name = _Name;
                IP = _IP;
                socket = _socket;
            }

            public override string ToString()
            {
                return IP + "|" + Name;
            }
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
    }
}
