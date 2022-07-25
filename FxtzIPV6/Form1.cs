using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FxtzIPV6
{
    public partial class Form1 : Form
    {
        int GamePort()
        {
            try
            {
                var portmes = System.Configuration.ConfigurationSettings.AppSettings["gameport"];
                return int.Parse(portmes); 
            }
            catch
            {
                return 10800;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }
        static User userRoom;
        void UpdateUI(bool isroom,bool isconn)
        {
            copyRoomBtn.Enabled = isroom;
            copyGameBtn.Enabled = isconn;

            createRoomBtn.Text = isroom? "退出房间":"创建房间";
            connectRoomBtn.Enabled = !isroom;
            connectRoomBtn.Text = isconn ? "退出房间" : "连接房间";
            createRoomBtn.Enabled = !isconn;
        }
        private void createRoomBtn_Click(object sender, EventArgs e)
        {
            if (userRoom != null)
            {
                userRoom.Dispose();
                userRoom = null;
                upText.Text = "";
                UpdateUI(false,false);
                return;
            }
            var ipv6host = Util.GetLocalIPV6();
            if (string.IsNullOrEmpty(ipv6host))
            {
                MessageBox.Show("您的主机目前在内网，没有公网ipv6地址");
            }
            else
            {
                userRoom = new User(GamePort());
                upText.Text = $"[{Util.GetLocalIPV6()}]:{userRoom.Ipv6Prot}";
                UpdateUI(true,false);
                userRoom.Start();
            }
        }

        private void copyRoomBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(upText.Text);
            MessageBox.Show(upText.Text+" 已复制");
        }

        private void connectRoomBtn_Click(object sender, EventArgs e)
        {
            if (userRoom != null)
            {
                userRoom.Dispose();
                userRoom = null;
                downText.Text = "";
                UpdateUI(false, false);
                return;
            }
            var mes = downText.Text;
            var match = Regex.Match(mes, @"^\[(.*?)\]:(\d+)$");
            if(match.Success && IPAddress.TryParse(match.Groups[1].Value, out IPAddress ip) && int.TryParse(match.Groups[2].Value, out int port))
            {
                var gameport = Util.GetRandomPort();
                userRoom = new User(gameport, new IPEndPoint(ip, port));
                userRoom.Start();
                downText.Text = $"127.0.0.1:{gameport}";
                UpdateUI(false, true);
            }
            else
            {
                MessageBox.Show("请输入正确的ipv6地址");
            }
        }

        private void copyGameBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(downText.Text);
            MessageBox.Show(downText.Text + " 已复制");
        }
    }
}
