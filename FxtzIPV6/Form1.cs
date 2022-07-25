using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace FxtzIPV6
{
    public partial class Form1 : Form
    {
        int GamePort()
        {
            try
            {
                var portmes = ConfigurationSettings.AppSettings["gameport"];
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
            try
            {
                var blackarr = ConfigurationSettings.AppSettings["black"].Split('=');
                foreach (var item in blackarr)
                {
                    var arr=item.Split('_'); 
                    if(arr.Length==2)
                        User.blackhistorys.Add(new HistoryUser(arr[0],int.Parse(arr[1])));
                }
            }
            catch
            {
            }
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

        bool pingok = false;
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
                pingok = false;
               
                short ping =0;
                var pingsocket = Util.GetUdpSocket(true);

                var thread = new Thread(() =>
                {
                    var pingport = port + 1;
                    try
                    {
                        pingsocket.BeginSendTo(new byte[] {1,2,3,4}, 0,4, SocketFlags.None, new IPEndPoint(ip, pingport), (r) => { }, null);
                        Thread.Sleep(100);
                        for (int i = 0; i < 12; i++)
                        {
                            var time = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
                            pingsocket.BeginSendTo(time, 0, time.Length, SocketFlags.None, new IPEndPoint(ip, pingport), (r) => { }, null);
                        }
                        EndPoint remote = new IPEndPoint(ip, 0);
                        long pingtime = 0;
                        var rcount = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            var buf = new byte[8];
                            var n = pingsocket.ReceiveFrom(buf, ref remote);
                            if (n == 8)
                            {
                                var rtime = BitConverter.ToInt64(buf, 0);
                                pingtime += (DateTime.UtcNow.Ticks - rtime);
                                rcount++;
                            }
                        }
                        if(pingtime>0) pingtime /= (10000*rcount);
                        if (pingtime < 1) pingtime = 1;
                        if (pingtime > 1000) pingtime = 1000;
                        ping = (short)pingtime;
                        for (int i = 0; i < 3; i++)
                        {
                            pingsocket.BeginSendTo(BitConverter.GetBytes(ping), 0, 2, SocketFlags.None, new IPEndPoint(ip, pingport), (r) => { }, null);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            var buf = new byte[8];
                            var n = pingsocket.ReceiveFrom(buf, ref remote);
                            if (n == 2)
                            {
                                pingok = true;
                            }
                        }
                    }
                    catch { }
                });
                thread.Start();
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(200);
                    if (pingok) break;
                }
                pingsocket.Close();
                thread.Abort();

                if (pingok)
                {
                    User.historys.Remove(new HistoryUser(ip.ToString(), ping));
                    User.historys.Add(new HistoryUser(ip.ToString(),ping));
                    var gameport = Util.GetRandomPort();
                    userRoom = new User(gameport, new IPEndPoint(ip, port));
                    userRoom.Start();
                    downText.Text = $"127.0.0.1:{gameport}";
                    UpdateUI(false, true);
                }
                else
                {
                    MessageBox.Show("无法连接到该房间");
                    return;
                }
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

        void UpdateHistroy(HistoryUser history,bool black)
        {
            if (black)
            {
                User.blackhistorys.Add(history);
                User.historys.Remove(history);
            }
            else
            {
                User.blackhistorys.Remove(history);
                User.historys.Add(history);
            }
            Util.UpdateAppSettings("black",string.Join("=",User.blackhistorys.Select(x=>x.host+"_"+x.delay).ToArray()));
        }
        private void settingBtn_Click(object sender, EventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            var histroys=new List<MenuItem>();
            foreach (var item in User.historys)
            {
                histroys.Add(new MenuItem(item.host, new[] {new MenuItem("加入黑名单", delegate { UpdateHistroy(item, true); }),new MenuItem("延迟:"+item.delay+"ms")}));
            }
            var history = new MenuItem("历史连接IP", histroys.ToArray());
            menu.MenuItems.Add(history);
            var blackhistroys = new List<MenuItem>();
            foreach (var item in User.blackhistorys)
            {
                blackhistroys.Add(new MenuItem(item.host, new[] { new MenuItem("移出黑名单", delegate { UpdateHistroy(item, false); }) }));
            }
            var blackhistory = new MenuItem("黑名单", blackhistroys.ToArray());
            menu.MenuItems.Add(blackhistory);
            menu.MenuItems.Add("打开github源码");
            
            menu.MenuItems[2].Click +=delegate
            {
                var github = "https://github.com/lilipbb/FxtzIPV6";
                Clipboard.SetText(github);
                MessageBox.Show(github + " 网址已复制，请粘贴到网站浏览");
            };
            if (e is MouseEventArgs m)
            {
                menu.Show((Button)sender, new Point(m.X, m.Y));
            }
        }
    }
}
