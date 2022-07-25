using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

class Util
{
    public static string GetLocalIPV6()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            return IpEntry.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetworkV6 && !x.ToString().StartsWith("fe")).LastOrDefault()?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public static Socket GetUdpSocket(bool isipv6 = false)
    {
        var socket = new Socket(isipv6?AddressFamily.InterNetworkV6:AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        return socket;
    }

    static List<int> PortIsUsed()
    {
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();       
        IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
        var allPorts = new List<int>();
        foreach (IPEndPoint ep in ipsUDP)
        {
            allPorts.Add(ep.Port);
        }
        return allPorts;

    }
    public static int GetRandomPort(int firstselectPort=-1)
    {
        var HasUsedPort = PortIsUsed();
        Random random = new Random((int)DateTime.Now.Ticks);
        var port = firstselectPort;
        while (true)
        {

            if (port<0||HasUsedPort.Contains(port))
            {
                port = random.Next(10801, 65535);
                continue;
            }
            return port;
        }
    }
}