using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Text.RegularExpressions;
using Common;
using MahjongLobby_AH.Network.Message;

namespace NetComm
{
    //[StructLayout(LayoutKind.Sequential)]
    public class MsgHeadDef
    {
        public byte cVersion;
        public byte cMsgType;
        public int iMsgBodyLen;
    };

    public class CompatibilityIP
    {
        [DllImport("__Internal")]
        private static extern string getIPv6(string host, string port);
        private static string GetIPv6(string host, string port)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
                                return getIPv6(host, port);
#endif
            return host + "&&ipv4";
        }
        /// <summary>
        /// 新增ipv6客户端方法
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="newServerIp"></param>
        /// <param name="newServerAddressFamily"></param>
        public static void GetIpType(string serverIp, string serverPort, out string newServerIp, out AddressFamily newServerAddressFamily)
        {
            newServerAddressFamily = AddressFamily.InterNetwork;
            newServerIp = serverIp;
            try
            {
                string mIPv6 = GetIPv6(serverIp, serverPort);
                if (!string.IsNullOrEmpty(mIPv6))
                {
                    string[] strTemp = Regex.Split(mIPv6, "&&");
                    if (strTemp.Length >= 2)
                    {
                        string type = strTemp[1];
                        if (type == "ipv6")
                        {
                            newServerIp = strTemp[0];
                            newServerAddressFamily = AddressFamily.InterNetworkV6;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public class NetProxy
    {
        ProxyType m_lProxyType;
        int m_lProxyState = 0;
        string m_szAuthUser = null;
        string m_szAuthPwd = null;
        string m_szDestHost = null;
        byte[] m_szProxyBuff = new byte[2048];
        int m_lBuffPos = 0;
        int m_nDestPort = 0;

        public enum ProxyType : int
        {
            PROXY_SOCKS4,
            PROXY_SOCKS5,
            PROXY_HTTP
        }

        public void Initialize(ProxyType type, string strDestHost, int nDestPort, string strAuthUser, string strAuthPwd)
        {
            m_lProxyType = type;
            m_szDestHost = strDestHost;
            m_nDestPort = nDestPort;
            m_szAuthUser = strAuthUser;
            m_szAuthPwd = strAuthPwd;
            m_lProxyState = 0;
            m_lBuffPos = 0;
        }

        // > 0, packet size
        // 0, proxy ok
        // -1, packet error
        // -2, lpszReturnBuf == null | 0 >= lLength | lLength >= sizeof(m_szProxyBuff) - m_lBuffPos;
        // -3, wait for next recv
        // -4, user name or password error
        // -5, connect dest host failed
        // -6, (HTTP) dest host is null
        // -7, proxytype invalid
        public int MakePacket(byte[] lpszReturnBuf, int lLength)
        {
            int lResult = 0;
            byte[] host, port, user, post;
            string str;

            if (ProxyType.PROXY_SOCKS5 == m_lProxyType)
            {
                if (0 == m_lProxyState)
                { //verify version
                    m_nDestPort = (ushort)IPAddress.HostToNetworkOrder((short)m_nDestPort);
                    bool bAuthenticate = (null != m_szAuthUser);
                    m_szProxyBuff[0] = 0x05;
                    m_szProxyBuff[1] = (byte)(bAuthenticate ? 0x02 : 0x01); //2, need to authenticate
                    m_szProxyBuff[2] = (byte)(bAuthenticate ? 0x02 : 0x00); //2=user/pass, 0=no logon
                    m_szProxyBuff[3] = 0x00;
                    lResult = bAuthenticate ? 0x04 : 0x03; //length of request
                    m_lProxyState++;
                }
                else
                {
                    if ((null == lpszReturnBuf) || (0 >= lLength))
                    {
                        lResult = -2;
                    }
                    else if (1 == m_lProxyState)
                    { //respond version
                        Array.Copy(lpszReturnBuf, 0, m_szProxyBuff, m_lBuffPos, 2 - m_lBuffPos);
                        //memcpy(&m_szProxyBuff[m_lBuffPos], lpszReturnBuf, 2 - m_lBuffPos);
                        m_lBuffPos += lLength;

                        if (2 > m_lBuffPos)
                        {
                            lResult = -3;
                        }
                        else if ((0xFF == m_szProxyBuff[1]) || (0x05 != m_szProxyBuff[0]))
                        {
                            lResult = -1;
                        }
                        else if (0x00 == m_szProxyBuff[1])
                        { //need not to authenticate
                            m_szProxyBuff[0] = 0x05;
                            m_szProxyBuff[1] = 0x01; //connect
                            m_szProxyBuff[2] = 0x00;
                            m_szProxyBuff[3] = 0x03; //remote resolve host name;

                            lResult = 4;

                            host = Encoding.ASCII.GetBytes(m_szDestHost);
                            m_szProxyBuff[lResult++] = (byte)host.Length;
                            Array.Copy(host, 0, m_szProxyBuff, lResult, host.Length);
                            //memcpy(&m_szProxyBuff[lResult], m_szDestHost, m_lDestHostLen);
                            lResult += host.Length;

                            //memcpy(&m_szProxyBuff[lResult], &m_nDestPort, 2);
                            port = BitConverter.GetBytes(m_nDestPort);
                            Array.Copy(port, 0, m_szProxyBuff, lResult, port.Length);
                            lResult += 2;
                            m_lBuffPos = 0;
                            m_lProxyState += 2;
                        }
                        else if (0x02 == m_szProxyBuff[1])
                        { //need to authenticate
                            str = m_szAuthUser + " " + m_szAuthPwd;
                            user = Encoding.ASCII.GetBytes(str);
                            //sprintf(m_szProxyBuff,"  %s %s", m_szAuthUser, m_szAuthPwd);
                            Array.Copy(user, 0, m_szProxyBuff, 2, user.Length);
                            m_szProxyBuff[0] = 0x05;
                            m_szProxyBuff[1] = (byte)m_szAuthUser.Length;
                            m_szProxyBuff[2 + m_szAuthUser.Length] = (byte)m_szAuthPwd.Length;
                            lResult = 3 + m_szAuthUser.Length + m_szAuthPwd.Length;
                            m_lBuffPos = 0;
                            m_lProxyState++;
                        }
                        else
                        { //data error
                            lResult = -1;
                        }
                    }
                    else if (2 == m_lProxyState)
                    { //respond authenticate
                        Array.Copy(lpszReturnBuf, 0, m_szProxyBuff, m_lBuffPos, 2 - m_lBuffPos);
                        //memcpy(&m_szProxyBuff[m_lBuffPos], lpszReturnBuf, 2 - m_lBuffPos);
                        m_lBuffPos += lLength;

                        if (2 > m_lBuffPos)
                        {
                            lResult = -3;
                        }
                        else if ((0xFF == m_szProxyBuff[1]) || (0x05 != m_szProxyBuff[0]))
                        {
                            lResult = -1;
                        }
                        else
                        {
                            if (0x00 != m_szProxyBuff[1])
                            { //verify error
                                lResult = -4;
                            }
                            else
                            {
                                m_szProxyBuff[0] = 0x05;
                                m_szProxyBuff[1] = 0x01; //connect
                                m_szProxyBuff[2] = 0x00;
                                m_szProxyBuff[3] = 0x03; //remote resolve host name;

                                lResult = 4;

                                host = Encoding.ASCII.GetBytes(m_szDestHost);
                                m_szProxyBuff[lResult++] = (byte)host.Length;
                                Array.Copy(host, 0, m_szProxyBuff, lResult, host.Length);
                                //memcpy(&m_szProxyBuff[lResult], m_szDestHost, m_lDestHostLen);
                                lResult += host.Length;

                                //memcpy(&m_szProxyBuff[lResult], &m_nDestPort, 2);
                                port = BitConverter.GetBytes(m_nDestPort);
                                Array.Copy(port, 0, m_szProxyBuff, lResult, port.Length);
                                lResult += 2;
                                m_lBuffPos = 0;
                                m_lProxyState++;
                            }
                        }
                    }
                    else if (3 == m_lProxyState)
                    { //respond connect
                        Array.Copy(lpszReturnBuf, 0, m_szProxyBuff, m_lBuffPos, 10 - m_lBuffPos);
                        //memcpy(&m_szProxyBuff[m_lBuffPos], lpszReturnBuf, 10 - m_lBuffPos);
                        m_lBuffPos += lLength;

                        if (10 > m_lBuffPos)
                        {
                            lResult = -3;
                        }
                        else if ((0xFF == m_szProxyBuff[1]) || (0x05 != m_szProxyBuff[0]))
                        {
                            lResult = -1;
                        }
                        else
                        {
                            if (0x00 != m_szProxyBuff[1])
                            { //connect failed
                                lResult = -5;
                            }
                            else
                            {
                                //if (!m_bProxyConnect) //listen
                                //{
                                //unsigned int ip;
                                //unsigned short port;
                                //memcpy(&ip, &m_szProxyBuff[4], 4);
                                //memcpy(&port, &m_szProxyBuff[8], 2);
                                // }
                                m_lBuffPos = 0;
                            }
                        }
                    }
                }
            }
            else if (ProxyType.PROXY_SOCKS4 == m_lProxyType)
            {
                if (0 == m_lProxyState)
                { //request connect
                    m_nDestPort = (ushort)IPAddress.HostToNetworkOrder((short)m_nDestPort);
                    lResult = 9;
                    m_szProxyBuff[0] = 0x04; //version
                    m_szProxyBuff[1] = 1; //CONNECT or BIND request
                    port = BitConverter.GetBytes(m_nDestPort);
                    Array.Copy(port, 0, m_szProxyBuff, 2, port.Length);
                    //memcpy(&m_szProxyBuff[2], &m_nDestPort, 2); //Copy target address

                    //Set the IP to 0.0.0.x (x is nonzero)
                    m_szProxyBuff[4] = 0x00;
                    m_szProxyBuff[5] = 0x00;
                    m_szProxyBuff[6] = 0x00;
                    m_szProxyBuff[7] = 0x01;
                    //Add host as URL
                    host = Encoding.ASCII.GetBytes(m_szDestHost);
                    Array.Copy(host, 0, m_szProxyBuff, 9, host.Length);
                    //memcpy(&m_szProxyBuff[9], m_szDestHost, m_lDestHostLen + 1);
                    lResult += host.Length + 1;

                    m_szProxyBuff[8] = 0x00;
                    m_lProxyState++;
                }
                else
                {
                    if ((null == lpszReturnBuf) || (0 >= lLength))
                    {
                        lResult = -2;
                    }
                    else if (1 == m_lProxyState)
                    { //respond connect
                        Array.Copy(lpszReturnBuf, 0, m_szProxyBuff, m_lBuffPos, 8 - m_lBuffPos);
                        //memcpy(&m_szProxyBuff[m_lBuffPos], lpszReturnBuf, 8 - m_lBuffPos);
                        m_lBuffPos += lLength;

                        if (8 > m_lBuffPos)
                        {
                            lResult = -3;
                        }
                        else if ((0x5A != m_szProxyBuff[1]) || (0x00 != m_szProxyBuff[0]))
                        {
                            lResult = (0x5C == m_szProxyBuff[1]) ? -5 : -1;
                        }
                        else
                        {
                            //if (!m_bProxyConnect) //listen
                            {
                                //unsigned int ip;
                                //unsigned short port;
                                //memcpy(&ip, &m_szProxyBuff[4], 4);
                                //memcpy(&port, &m_szProxyBuff[2], 2);
                            }
                            m_lBuffPos = 0;
                        }
                    }
                }
            }
            else if (ProxyType.PROXY_HTTP == m_lProxyType)
            {
                if (0 == m_lProxyState)
                {
                    if (null == m_szDestHost)
                    {
                        lResult = -6;
                    }
                    else
                    {
                        if (null == m_szAuthUser)
                        {
                            //sprintf(m_szProxyBuff, "CONNECT %s:%d HTTP/1.1\r\nHost: %s:%d\r\n\r\n", m_szDestHost, m_nDestPort, m_szDestHost, m_nDestPort);
                            str = "CONNECT " + m_szDestHost + ":" + m_nDestPort.ToString() + " HTTP/1.1\r\nHost: " + m_szDestHost + ":" + m_nDestPort.ToString() + "\r\n\r\n";
                            post = Encoding.ASCII.GetBytes(str);
                            Array.Copy(post, 0, m_szProxyBuff, 0, post.Length);
                            lResult = post.Length;
                        }
                        else
                        {
                            //sprintf(m_szProxyBuff, "CONNECT %s:%d HTTP/1.1\r\nHost: %s:%d\r\n", m_szDestHost, m_nDestPort, m_szDestHost, m_nDestPort);
                            str = "CONNECT " + m_szDestHost + ":" + m_nDestPort.ToString() + " HTTP/1.1\r\nHost: " + m_szDestHost + ":" + m_nDestPort.ToString() + "\r\n";
                            post = Encoding.ASCII.GetBytes(str);
                            Array.Copy(post, 0, m_szProxyBuff, 0, post.Length);
                            lResult = post.Length;

                            //char szAuthBuf[513];
                            //sprintf(szAuthBuf, "%s:%s", m_szAuthUser, m_szAuthPwd);
                            str = m_szAuthUser + ":" + m_szAuthPwd;

                            //char szEncodeBuf[1024];
                            //Base64Encode(szAuthBuf, m_lAuthUserLen + m_lAuthPwdLen + 1, szEncodeBuf, sizeof(szEncodeBuf));
                            string strEncode = Convert.ToBase64String(Encoding.ASCII.GetBytes(str));

                            str = "Authorization: Basic " + strEncode + "\r\nProxy-Authorization: Basic " + strEncode + "\r\n\r\n";
                            post = Encoding.ASCII.GetBytes(str);
                            Array.Copy(post, 0, m_szProxyBuff, lResult, post.Length);
                            lResult += post.Length;

                            //strcat(m_szProxyBuff, "Authorization: Basic ");
                            //strcat(m_szProxyBuff, szEncodeBuf);
                            //strcat(m_szProxyBuff, "\r\n");
                            //strcat(m_szProxyBuff, "Proxy-Authorization: Basic ");
                            //strcat(m_szProxyBuff, szEncodeBuf);
                            //strcat(m_szProxyBuff, "\r\n\r\n");
                        }
                        //lResult = strlen(m_szProxyBuff);
                        m_lProxyState++;
                    }
                }
                else
                {
                    if ((null == lpszReturnBuf) || (0 >= lLength) || (lLength >= 2048 - m_lBuffPos))
                    {
                        lResult = -2;
                    }
                    else if (1 == m_lProxyState)
                    {
                        Array.Copy(lpszReturnBuf, 0, m_szProxyBuff, m_lBuffPos, lLength);
                        //memcpy(&m_szProxyBuff[m_lBuffPos], lpszReturnBuf, lLength);
                        m_lBuffPos += lLength;
                        m_szProxyBuff[m_lBuffPos] = 0;

                        str = Encoding.ASCII.GetString(m_szProxyBuff);

                        //if (null == strstr(m_szProxyBuff, "\r\n\r\n"))
                        if (-1 == str.IndexOf("\r\n\r\n"))
                        {
                            lResult = -3;
                        }
                        else
                        {
                            //char *p = strstr(m_szProxyBuff, " ");
                            //if (null == p)
                            int iPos = str.IndexOf(" ");
                            if (-1 == iPos)
                            {
                                lResult = -1;
                            }
                            else
                            {
                                //if (0 == memcmp(p + 1, "200", 3)) //success
                                if ("200" == str.Substring(iPos + 1, 3))
                                {

                                }
                                else if ("407" == str.Substring(iPos + 1, 3))
                                {//(0 == memcmp(p + 1, "407", 3)) //需要验
                                    lResult = -4;
                                }
                                else
                                {
                                    lResult = -1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                lResult = -7;
            }

            return lResult;
        }

        public byte[] GetPacketBuff()
        {
            return m_szProxyBuff;
        }
    }

    public class NetSocket
    {
        private AutoResetEvent ev = new AutoResetEvent(true);
        private Socket s;
        private byte[] buf = new byte[4096];
        private EndPoint addr;
        private NetComm.NetProxy proxy = new NetComm.NetProxy();
        private bool bProxy = false;
        private int iProxyState = 1;

        public enum NetType
        {
            TCP = ProtocolType.Tcp,
            UDP = ProtocolType.Udp
        }

        ~NetSocket()
        {

            //close();
        }

        public bool IsProxy
        {
            get
            {
                return bProxy;
            }
        }

        public int ProxyState
        {
            get
            {
                return iProxyState;
            }
        }

        public bool InitializeProxy(NetProxy.ProxyType type, string strDestHost, int nDestPort, string strAuthUser, string strAuthPwd)
        {
            bool br = false;

            bProxy = true;

            proxy.Initialize(type, strDestHost, nDestPort, strAuthUser, strAuthPwd);

            if (null != s)
            {
                int len = proxy.MakePacket(null, 0);
                byte[] data = new byte[len];
                Array.Copy(proxy.GetPacketBuff(), data, len);
                br = (0 < s.Send(data));
            }

            return br;
        }

        public bool connect(string strHost, int Port)
        {
            //Debug.LogError("connect============" + strHost);
            return create(strHost, Port, NetType.TCP);
        }

        public bool create(string strHost, int Port, NetType type)
        {
            //Debug.LogError("create1");
            bProxy = false;
            bool br = false;

            if (null == s)
            {
                //Debug.LogError("create2");
                try
                {
                    IPAddress ip = (null == strHost) ? new IPAddress(0) : IPAddress.Parse(Hostname2ip(strHost));
                    if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
                    {
                        //Debug.LogError("create3   " + Hostname2ip(strHost));
                        // IPAddress ip = (null == strHost) ? new IPAddress(0) : IPAddress.Parse(Hostname2ip(strHost));
                        //Debug.LogError("create4    " + ip.ToString());
                        addr = new IPEndPoint(ip, Port);

                        Socket ipv6 = new Socket(AddressFamily.InterNetworkV6, (NetType.TCP == type) ? SocketType.Stream : SocketType.Dgram, (ProtocolType)type);
                        Socket ipv4 = new Socket(AddressFamily.InterNetwork, (NetType.TCP == type) ? SocketType.Stream : SocketType.Dgram, (ProtocolType)type);

                        if (s == null)
                        {
                            try
                            {
                                string serverIp;
                                AddressFamily ipType;
                                CompatibilityIP.GetIpType(((null == strHost) ? new IPAddress(0) : IPAddress.Parse(Hostname2ip(strHost))).ToString(), Port.ToString(), out serverIp, out ipType);
                                //  Debug.LogError("ipType   " + ipType.ToString());
                                ipv6 = new Socket(ipType, SocketType.Stream, ProtocolType.Tcp);

                                // Debug.LogError("create5   " + serverIp);
                                IPAddress address = IPAddress.Parse(serverIp);
                                //  Debug.LogError("create6   " + address.ToString());
                                //EndPoint port = new IPEndPoint(address, Port);
                                s = ipv6;// new Socket(ipType, SocketType.Stream, ProtocolType.Tcp) ;
                                s.Connect(address, Port);

                                Debug.Log("ipv6 ");
                            }
                            catch
                            {
                                s = ipv4; //new Socket(AddressFamily.InterNetwork, (NetType.TCP == type) ? SocketType.Stream : SocketType.Dgram, (ProtocolType)type); ;
                                s.Connect(Hostname2ip(strHost), Port);
                                Debug.Log("ipv4 ");
                            }
                        }
                        else if (NetType.TCP == type)
                        {
                            s.Connect(Hostname2ip(strHost), Port);
                            Debug.Log("正常111 ");
                        }
                        else
                        {
                            s.Bind(addr);
                            Debug.Log("正常2222 ");
                        }
                    }
                    else
                    {
                        //#else
                        //Debug.LogError("create4");
                        s = new Socket(AddressFamily.InterNetwork, (NetType.TCP == type) ? SocketType.Stream : SocketType.Dgram, (ProtocolType)type);
                        // IPAddress ip = (null == strHost) ? new IPAddress(0) : IPAddress.Parse(Hostname2ip(strHost));
                        addr = new IPEndPoint(ip, Port);
                        //Debug.Log("ipv4 ");
                        //Debug.LogError("类型============" + type);
                        if (NetType.TCP == type)
                        {
                            //Debug.LogError("域名解析ip" + Hostname2ip(strHost));
                            s.Connect(Hostname2ip(strHost), Port);

                        }
                        else
                        {
                            s.Bind(addr);
                        }
                        //s.BeginReceiveFrom(buf, 0, buf.Length, 0, ref addr, RecvCallback, null);
                        //br = true;

                        //先开始ipv6的支持，ipv6连不上就使用ipv4
                        //Debug.Log("正常 ");

                        //#endif

                    }
                    s.BeginReceiveFrom(buf, 0, buf.Length, 0, ref addr, RecvCallback, null);
                    br = true;

                }
                catch
                {
                    if (null != s)
                    {
                        s.Close();
                        s = null;
                    }
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetSocket.create, exp: ", LogType.Exception);
                }
            }
            return br;
        }

        /// <summary>
        /// 域名转换为IP地址
        /// </summary>
        /// <param name="hostname">域名或IP地址</param>
        /// <returns>IP地址</returns>
        public static string Hostname2ip(string hostname)
        {
            try
            {
                IPAddress ip;
                if (IPAddress.TryParse(hostname, out ip))
                    return ip.ToString();
                else
                    return Dns.GetHostEntry(hostname).AddressList[0].ToString();
            }
            catch (Exception)
            {
                throw new Exception("IP Address Error");
            }
        }

        public void close()
        {
            if (ev != null)
            {
                ev.WaitOne();
            }
            if (null != s)
            {
                try
                {
                    s.Close();
                }
                catch (Exception e)
                {
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetSocket.close, exp: " + e.Message, LogType.Exception);
                }
                s = null;
            }

            if (ev != null)
            {
                ev.Set();
            }

        }

        public int send(byte[] data)
        {
            int ir = -1;
            if ((null != s) && (null != data))
            {
                try
                {
                    //Debug.Log("发送消息：" + data[5].ToString("X2"));
                    //Debug.LogError("发送消息");
                    //FishLobby.GameData.Instance.Printbuff(data);
                    ir = s.Send(data);
                }
                catch (Exception e)
                {
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetSocket.send, exp: " + e.Message, LogType.Exception);
                }
            }

            return ir;
        }

        public int send(byte[] data, ref EndPoint addr)
        {
            int ir = -1;

            if ((null != s) && (null != data))
            {
                try
                {
                    ir = s.SendTo(data, addr);
                }
                catch (Exception e)
                {
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetSocket.send, exp: " + e.Message, LogType.Exception);
                }
            }

            return ir;
        }

        private void RecvCallback(IAsyncResult ar)
        {
            try
            {
                int len = s.EndReceiveFrom(ar, ref addr);
                if (0 == len)
                { //close
                    Debug.LogWarning("断开服务器=================:" + (s == null ? "0" : "1"));
                    ev.WaitOne();
                    OnClose(null != s);
                    ev.Set();
                }
                else
                {
                    if (true == bProxy)
                    {
                        int length = proxy.MakePacket(buf, len);
                        if (0 == length)
                        { //proxy ok
                            bProxy = false;
                            OnProxyState(0);
                        }
                        else if (0 < length)
                        {
                            byte[] data = new byte[length];
                            Array.Copy(proxy.GetPacketBuff(), data, length);
                            s.Send(data);
                        }
                        else if (-3 != length)
                        {
                            bProxy = false;
                            OnProxyState(length);
                        }
                    }
                    else
                    {
                        OnRead(buf, len, ref addr);
                    }
                    s.BeginReceiveFrom(buf, 0, buf.Length, 0, ref addr, RecvCallback, null);
                }
            }
            catch (Exception e)
            {
                ev.WaitOne();
                OnClose(null != s);
                ev.Set();
            }
        }

        virtual protected void OnProxyState(int state)
        {
            iProxyState = state;
        }

        virtual protected void OnClose(bool bClosed) //true: server closed;
        {

        }

        virtual protected void OnRead(byte[] data, int len, ref EndPoint addr)
        {

        }
    }



    public class Packet
    {
        public byte[] data;
        public bool bClosed; //true: server closed
        public EndPoint addr;
    }

    public class NetUdp : NetSocket
    {
        private ArrayList list = new ArrayList();
        private AutoResetEvent ev = new AutoResetEvent(true);

        public Packet ReadData()
        {
            Packet msg = null;

            ev.WaitOne(1);
            if (list.Count > 0)
            {
                msg = (Packet)list[0];
                list.RemoveAt(0);
            }
            ev.Set();

            return msg;
        }

        public int send(byte[] data, string strHost, int Port)
        {
            EndPoint addr = new IPEndPoint(new IPAddress(Encoding.ASCII.GetBytes(strHost)), Port);
            return send(data, ref addr);
        }

        public new int send(byte[] data, ref EndPoint addr)
        {
            int ir = -1;

            if (null != data)
            {
                try
                {
                    ir = base.send(data, ref addr);
                }
                catch (Exception e)
                {
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetUdp.send, exp: " + e.Message, LogType.Exception);
                }
            }

            return ir;
        }

        override protected void OnRead(byte[] data, int len, ref EndPoint addr)
        {
            Packet pkt = new Packet();
            pkt.data = new byte[len];
            Array.Copy(data, pkt.data, len);
            IPEndPoint ep = addr as IPEndPoint;
            pkt.addr = new IPEndPoint(ep.Address, ep.Port);
            ev.WaitOne();
            list.Add(pkt);
            ev.Set();
        }
    }

    public class NetConnection : NetSocket
    {
        private ArrayList list = new ArrayList();
        private AutoResetEvent ev = new AutoResetEvent(true);
        private byte[] buffer = new byte[BUFFER_SIZE];
        private int length;

        public Packet ReadData()
        {
            Packet msgPk = null;

            ev.WaitOne(1);

            if (list.Count > 0)
            {
                msgPk = (Packet)list[0];
                list.RemoveAt(0);
            }
            ev.Set();

            return msgPk;
        }

        public int send(byte[] data, bool bEncrypt)
        {
            int ir = -3;

            if (null != data)
            {
                try
                {
                    string s = MsgID.ToString(data[1]);
                    s += ("  长度: " + data.Length);
                    s += "  数据:";

                    for (int i = 0; i < data.Length; ++i)
                    {
                        s += (" " + data[i].ToString("X2"));
                    }

                    ////// TODO 查看发送十六进制数据(底层发送十六进制消息）
                    if (MahjongLobby_AH.LobbyContants.isOpenDebugMessage_Send)
                    {
                        Debug.Log("+++++++++++++++++++++" + s);
                    }

                    //DEBUG.NetworkClient(data[1], data);
                    byte[] pMsg = (bEncrypt) ? AESCrypt.Encrypt(data, AESCrypt.KEY) : data;
                    if (null != pMsg)
                    {
                        short iMsgLen = (short)(pMsg.Length + 4);
                        byte[] msg = new byte[iMsgLen];
                        msg[0] = PACKAGE_IDENTIFY;
                        msg[1] = (byte)((bEncrypt) ? 0 : 1);
                        iMsgLen = IPAddress.HostToNetworkOrder(iMsgLen);
                        byte[] len = BitConverter.GetBytes(iMsgLen);
                        Array.Copy(len, 0, msg, 2, len.Length);
                        Array.Copy(pMsg, 0, msg, 4, pMsg.Length);

                        ir = send(msg);
                    }
                }
                catch (Exception e)
                {
                    DEBUG.Networking(DEBUG.TRACER_LOG + "NetConnection.send, exp: " + e.Message, LogType.Exception);
                }
            }

            return ir;
        }

        override protected void OnClose(bool bClosed) //true: server closed
        {
            try
            {
                length = 0;
                ev.WaitOne();
                Packet msgPk = new Packet();
                msgPk.data = null;
                msgPk.bClosed = bClosed;
                list.Add(msgPk);
                ev.Set();
            }
            catch (Exception e)
            {
                DEBUG.Networking(DEBUG.TRACER_LOG + "NetConnection.OnClose, exp: " + e.Message, LogType.Exception);
            }
        }

        override protected void OnRead(byte[] data, int len, ref EndPoint addr)
        {
            byte[] msg = null;
            Packet msgPk = null;
            int pos = 0, size = 0, msglen;

            try
            {
                if (0 < length)
                {
                    size = length + len;

                    if (PACKAGE_HEADER_SIZE > size)
                    {
                        if (BUFFER_SIZE >= size)
                        {
                            Array.Copy(data, 0, buffer, length, len);
                            length = size;
                        }
                        else
                        {
                            Debug.LogError("Close===========2");
                            close();
                        }
                        return;
                    }

                    if (PACKAGE_HEADER_SIZE > length)
                    {
                        pos = PACKAGE_HEADER_SIZE - length;
                        Array.Copy(data, 0, buffer, length, pos);
                        length += pos;
                        len -= pos;
                    }

                    //message version
                    if (PACKAGE_IDENTIFY == buffer[0])
                    {
                        //msg length
                        msglen = BitConverter.ToInt16(buffer, 2);
                        msglen = IPAddress.NetworkToHostOrder((short)msglen);

                        if (length + len >= msglen)
                        {
                            size = msglen - length;
                            msg = new byte[msglen - PACKAGE_HEADER_SIZE];
                            Array.Copy(buffer, PACKAGE_HEADER_SIZE, msg, 0, length - PACKAGE_HEADER_SIZE);
                            Array.Copy(data, pos, msg, length - PACKAGE_HEADER_SIZE, size);
                            pos += size;
                            len -= size;
                            msgPk = new Packet();
                            msgPk.data = (0 == buffer[1]) ? AESCrypt.Decrypt(msg, AESCrypt.KEY) : msg;
                            DEBUG.NetworkServer(data[1], data);
                            if (null != msgPk.data)
                            {
                                ev.WaitOne();
                                list.Add(msgPk);
                                ev.Set();
                            }
                            else
                            {
                                Debug.LogError("Close========3");
                                close();
                                return;
                            }
                        }
                        else
                        {
                            size = length + len;
                            if (BUFFER_SIZE >= size)
                            {
                                Array.Copy(data, pos, buffer, length, len);
                                length = size;
                            }
                            else
                            {
                                Debug.LogError("Close========4");
                                close();
                            }
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError("Close=========5");
                        close();
                        return;
                    }
                }

                while (PACKAGE_HEADER_SIZE <= len)
                {
                    //message version
                    if (PACKAGE_IDENTIFY == data[pos])
                    {
                        //msg length
                        msglen = BitConverter.ToInt16(data, pos + 2);
                        msglen = IPAddress.NetworkToHostOrder((short)msglen);

                        if (BUFFER_SIZE < msglen)
                        {
                            Debug.LogError("Close=========6");
                            close();
                            return;
                        }
                        else if (len >= msglen)
                        {
                            msg = new byte[msglen - PACKAGE_HEADER_SIZE];
                            Array.Copy(data, pos + PACKAGE_HEADER_SIZE, msg, 0, msg.Length);
                            msgPk = new Packet();
                            msgPk.data = (0 == data[pos + 1]) ? AESCrypt.Decrypt(msg, AESCrypt.KEY) : msg;
                            pos += msglen;
                            len -= msglen;

                            if (null != msgPk.data)
                            {
                                ev.WaitOne();
                                list.Add(msgPk);
                                ev.Set();
                            }
                            else
                            {
                                Debug.LogError("Close=========7");
                                close();
                                return;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError("Close=========8");
                        close();
                        return;
                    }
                }

                Array.Copy(data, pos, buffer, 0, len);
                length = len;
            }
            catch (Exception e)
            {
                DEBUG.Networking(DEBUG.TRACER_LOG + "NetConnection.OnRead, exp: " + e.Message, LogType.Exception);
            }
        }

        private const int PACKAGE_HEADER_SIZE = 0x04;
        private const int PACKAGE_IDENTIFY = 0x4C;
        private const int BUFFER_SIZE = 40 * 1024;
    }
}

public class NetClient
{
    private NetComm.NetConnection m_client = new NetComm.NetConnection();
    [HideInInspector]
    public string
        m_strServer = "";
    [HideInInspector]
    public int
        m_iPort = 0;

    [HideInInspector]
    bool
        bLockDelayMsg = false;

    //网络消息的回
    public delegate void ONNETMSG(ushort cMsgType, byte[] pMsg, int len);
    //断线的回
    public delegate void ONDISCONNECT(int closeflag);

    //proxy callback
    public delegate void ONNETSTATE(int state);

    bool bProxy = false;
    ONNETSTATE OnNetState = null;
    ONNETMSG OnNetMsg = null;
    ONDISCONNECT OnDisconnect = null;

    public static int[] CROSSDOMAIN_PORTLIST = new int[]
    {
        843, 11843, 12843
    };

    public void Disconnect()
    {
        //DEBUG.Networking(DEBUG.TRACER_LOG + "Disconnect()");
        //m_client.list = new ArrayList();
        m_client.close();
    }

    public static byte[] ConvertToByte(object o)
    {
        return XConvert.ToByte(o);
    }

    public void ConnectServer(NetComm.NetProxy.ProxyType type, string strProxyHost, ushort nProxyPort, string strDestHost, int nDestPort, string strAuthUser, string strAuthPwd)
    {
        if (strProxyHost == null || strProxyHost == "")
        {
            ConnectServer(strDestHost, nDestPort);
        }
        else
        {
            m_strServer = strDestHost;
            m_iPort = nDestPort;
            if (Connect(strProxyHost, nProxyPort))
            {
                if (!m_client.InitializeProxy(type, strDestHost, nDestPort, strAuthUser, strAuthPwd))
                {
                    if (null != OnNetState)
                    {
                        OnNetState(-8);
                    }
                }
                else
                {
                    bProxy = true;
                }
            }
        }
    }

    public bool ConnectServer(string ip, int port)
    {
        bProxy = false;
        m_strServer = ip;
        m_iPort = port;
        //Debug.LogError("ConnectServer============" + ip);
        bool br = Connect(ip, port);

        if (null != OnNetState)
        {
            OnNetState(br ? 0 : -8);
        }

        return br;
    }

    // do network message
    virtual protected void OnGameNetMessage(ushort cMsgType, byte[] pMsg, int len)
    {
        if (cMsgType == 0)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(len);
            }
        }
        else
        {
            if (OnNetMsg != null)
            {
                OnNetMsg(cMsgType, pMsg, len);
            }
        }
    }

    public void SetCallBack(ONNETMSG callback, ONDISCONNECT disconnect, ONNETSTATE state)
    {
        OnNetMsg = callback;
        OnDisconnect = disconnect;
        OnNetState = state;
    }
    // send network message
    public int SendCliMsg(object obj, ushort type)
    {
        return SendCliMsg(obj, type, true);
    }

    public int SendCliMsg(object obj, ushort type, bool bEncrypt)
    {
        int iRet = -2;

        if ((null != m_client) && (null != obj))
        {
            try
            {
                iRet = m_client.send((byte[])obj, bEncrypt);
                //byte[] item = NetClient.ConvertToByte(obj);
                //int offset = 0;
                //NetComm.MsgHeadDef header = XConvert.ToObject<NetComm.MsgHeadDef>(item, ref offset);
                //header.cVersion = 3;
                //header.cMsgType = type;
                //byte[] headItem = NetClient.ConvertToByte(header);
                //Array.Copy(headItem, item, headItem.Length);
                ////DEBUG.NetworkClient(type, item);
                //iRet = m_client.send(item, bEncrypt);
            }
            catch (Exception e)
            {
                DEBUG.Networking(DEBUG.TRACER_LOG + "GameClient.SendCliMsg, exp: " + e.Message, LogType.Exception);
            }
        }

        return iRet;
    }

    public void LockDelayMsg(bool locking)
    {
        bLockDelayMsg = locking;
        //Debug.LogError("========================bLockDelayMsg:" + bLockDelayMsg);        
    }

    public bool IsLocked()
    {
        return bLockDelayMsg;
    }

    public int SendCliMsg(byte[] data)
    {
        return SendCliMsg(data, true);
    }

    public int SendCliMsg(byte[] data, bool bEncrypt)
    {
        int iRet = -2;

        if ((null != m_client) && (null != data))
        {
            iRet = m_client.send(data, bEncrypt);
        }

        return iRet;
    }

    // connect to server
    public bool Connect(string strHost, int Port)
    {
        //m_strServer = strHost;
        //m_iPort = Port;

        //Debug.LogError("IP" + strHost + "Port" + Port);
        bool br = false;
        int start_index = UnityEngine.Random.Range(0, CROSSDOMAIN_PORTLIST.Length);
        try
        {
            //if (Application.webSecurityEnabled)
            //{

            //    int i = 0;
            //    for (; i < CROSSDOMAIN_PORTLIST.Length; i++)
            //    {
            //        Debug.LogError(Network.Connect(strHost, CROSSDOMAIN_PORTLIST[(start_index + i) % CROSSDOMAIN_PORTLIST.Length]));
            //        if (Network.Connect(strHost, CROSSDOMAIN_PORTLIST[(start_index + i) % CROSSDOMAIN_PORTLIST.Length]) != 0)
            //        {
            //            break;
            //        }
            //    }
            //    if (i == CROSSDOMAIN_PORTLIST.Length)
            //    {
            //        DEBUG.Networking(DEBUG.TRACER_LOG + "cross_domain failed, ip=" + strHost, LogType.Error);
            //        return br;
            //    }
            //}
            // Debug.LogError("connect000============" + strHost);
            br = m_client.connect(strHost, Port);
        }
        catch
        {

        }

        return br;
    }

    // Update is called once per frame
    public void Update()
    {
        if (null != m_client)
        {
            if (bProxy)
            {
                if (!m_client.IsProxy)
                {
                    if (null != OnNetState)
                    {
                        OnNetState(m_client.ProxyState);
                    }
                    bProxy = !bProxy;
                }

                return;
            }

            if (bLockDelayMsg)
            {
                return;
            }


            NetComm.Packet msgPk = m_client.ReadData();

            if (null != msgPk)
            {
                if (null == msgPk.data)
                {
                    Debug.LogWarning("服务器的数据为空，断开连接");
                    OnGameNetMessage(0, null, msgPk.bClosed ? 1 : 0);
                }
                else
                {
                    int offset = 0;
                    NetMsg.MsgHeadDef header = XConvert.ToObject<MahjongLobby_AH.Network.Message.NetMsg.MsgHeadDef>(msgPk.data, ref offset);
                    // TODO 查看接收十六进制数据(底层发送十六进制消息）                       

                    if (MahjongLobby_AH.LobbyContants.isOpenDebugMessage_Onreceive)
                    {
                        //DEBUG.Error(header.MsgType.ToString());
                        anhui.MahjongCommonMethod.Instance.Printbuff(msgPk.data);
                    }

                    OnGameNetMessage(header.MsgType, msgPk.data, msgPk.data.Length);
                }
            }

        }
    }

    // Application Quit
    void OnApplicationQuit()
    {
        //if (null != m_client && Application.platform == RuntimePlatform.WindowsWebPlayer)
        //{
        //    Debug.LogError("Close=========10");
        //    m_client.close();
        //    m_client = null;
        //}
    }




}
