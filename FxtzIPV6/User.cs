using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class User
{
    public static bool gameClose = false;
    static bool debug = false;
    static void Log(string mes)
    {
        if (debug) File.AppendAllText("./log.txt",mes);
    }
    class OuterUser
    {
        public Socket socket;
        public EndPoint endpoint;
        public int remotePort = -1;
        byte[] buf =new byte[2048];
        EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        public void Receive(Action<byte[],int,EndPoint> send)
        {
            try
            {
                while (socket != null)
                {
                    var rnum = socket.ReceiveFrom(buf, SocketFlags.None,ref ep);
                    // Log("收到游戏数据长度:" + "  " + r.ReceivedBytes.ToString()+" "+buf[0]);
                    var portbuf = BitConverter.GetBytes(remotePort == -1 ? (ep as IPEndPoint).Port : remotePort);
                    if (rnum == 1 && buf[0] == 11) gameClose = true;
                    for (int i = 0; i < 4; i++)
                    {
                        buf[rnum + i] = portbuf[i];
                    }
                    send(buf, rnum + 4, endpoint);
                }
            }
            catch(Exception ex)
            {
                Log("error1:"+ex.Message);
            }
        }
        public void Dispose()
        {
            if (socket == null) return;
            socket.Close();
            socket = null;
        }
    }
    int ipv6port;
    Socket ipv6Socket;
    EndPoint gamePoint;
    EndPoint remoteUser;
    bool isdispose = false;
    Dictionary<string, OuterUser> outerIpv6s = new Dictionary<string, OuterUser>();
    OuterUser clientUser;
    public int Ipv6Prot => ipv6port;
    public User(int gameport, EndPoint remoteUser=null)
    {
        gamePoint = new IPEndPoint(IPAddress.Loopback, gameport);
        
        ipv6port = Util.GetRandomPort();
        ipv6Socket = Util.GetUdpSocket(true);
        ipv6Socket.Bind(new IPEndPoint(IPAddress.IPv6Any,ipv6port));
        this.remoteUser = remoteUser;
    }
    public void Start()
    {
        ReceiveOutterStart();
        if (remoteUser != null)//开始监听游戏的数据
        {
            var s = new OuterUser();
            s.socket = Util.GetUdpSocket();
            s.endpoint = remoteUser;
            s.socket.Bind(gamePoint);
            var t = new Thread(() => { s.Receive((buf,num, p) => ipv6Socket.SendTo(buf,num, SocketFlags.None, p)); });
            t.IsBackground = true;
            t.Start();
            clientUser = s;
        }
    }
    byte[] ipv6recbuf =new byte[2048];
    HashSet<string> clears=new HashSet<string>();
    int clearindex = 0;
    void ReceiveOutterStart()
    {
        var t = new Thread(ReceiveOutter);
        t.IsBackground = true;
        t.Start();
    }
    EndPoint ep = new IPEndPoint(IPAddress.IPv6Any, 0);
    void ReceiveOutter()
    {
        while (!isdispose)
        {
            try
            {
                var rnum = ipv6Socket.ReceiveFrom(ipv6recbuf,  ref ep);
                var udpport = BitConverter.ToInt32(ipv6recbuf, rnum - 4);
                if (clears.Count > 0 && clears.Contains(ep.ToString() + udpport)) { }
                else
                {
                    if (clientUser != null)//接收到主机消息转发给游戏
                    {
                        //Log("收到主机数据:" + r.RemoteEndPoint.ToString() + "  " + udpport);
                        clientUser.socket.BeginSendTo(ipv6recbuf,0, rnum - 4, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, udpport), (r) => { },null);
                    }
                    else
                    {
                        //Log("收到辅机数据:" + r.RemoteEndPoint.ToString() + "  " + udpport);
                        if (outerIpv6s.TryGetValue(ep.ToString() + udpport, out OuterUser s))
                        {
                        }
                        else
                        {
                            s = new OuterUser();
                            s.socket = Util.GetUdpSocket();
                            s.endpoint = ep;
                            s.remotePort = udpport;
                            s.socket.Bind(new IPEndPoint(IPAddress.Loopback, Util.GetRandomPort()));
                            var t = new Thread(() => { s.Receive((buf, num, p) => ipv6Socket.BeginSendTo(buf,0, num, SocketFlags.None, p, (r) => { },null)); });
                            t.IsBackground = true;
                            t.Start();
                            outerIpv6s[ep.ToString() + udpport] = s;
                        }
                        //对外接收到的数据需要发送给游戏
                        s.socket.BeginSendTo(ipv6recbuf, 0,rnum - 4, SocketFlags.None, gamePoint, (r) => {},null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("error2:" + ex.Message);
            }
            try
            {
                if (clientUser == null && gameClose)
                {
                    Log("清理资源");
                    gameClose = false;
                    foreach (var item in outerIpv6s)
                    {
                        clears.Add(item.Value.endpoint.ToString() + item.Value.remotePort);
                        item.Value.Dispose();
                    }
                    outerIpv6s.Clear();
                    var i = ++clearindex;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        Thread.Sleep(20000);
                        if (i == clearindex)
                            clears.Clear();
                    });
                }
            }
            catch (Exception ex)
            {
                Log("error3:" + ex.Message);
            }
        }
    }
   
    public void Dispose()
    {
        if (isdispose) return;
        isdispose = true;
        ipv6Socket?.Close();
        ipv6Socket = null;
        clientUser?.Dispose();
        clientUser = null;
        foreach (var item in outerIpv6s)
        {
            item.Value.Dispose();
        }
        outerIpv6s = null;
    }
}