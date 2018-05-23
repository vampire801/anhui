using UnityEngine;
using System.Collections.Generic;
using System.Net;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH.Network
{
    [Hotfix]
    [LuaCallCSharp]
    public class MahjongGameGateway
    {
        public class SavedMsgItem
        {
            public object obj;
            public byte type;
            public bool encrypt;
        }

        #region 公共成员变量

        /// <summary>
        /// 客户端网络连接
        /// </summary>
        public NetClient NetClient = new NetClient();
        /// <summary>
        /// 断线标志
        /// </summary>
        public bool DisconnectFlag;
        /// <summary>
        /// 网络是否连接
        /// </summary>
        public bool Connected;
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort ServerPort;

        public List<SavedMsgItem> m_SavedMsgList = new List<SavedMsgItem>();
        #endregion 公共成员变量

        #region 私有成员变量

        List<MsgData> sendMsgQueue = new List<MsgData>(); // 发送消息队列，由于是单线程，所以没有加互斥锁
        int proxyIndex; //使用代理的索引，从1开始，应该是1~5
        int tryCount; //尝试连接次数

        #endregion 私有成员变量

        #region 公共方法
        /// <summary>
        /// 更新，在NetworkMgr的Update中被调用
        /// </summary>
        public void Update()
        {
            if (Connected)
            {
                //发送消息
                if (sendMsgQueue.Count > 0)
                {
                    MsgData msgData = sendMsgQueue[0];
                    NetClient.SendCliMsg(msgData.obj, msgData.cType, msgData.bEncrypt);
                    sendMsgQueue.RemoveAt(0);
                }

                if (DisconnectFlag)
                {
                    Disconnect();
                }
            }

            if (!DisconnectFlag)
            {
                NetClient.Update();
            }
        }





        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            tryCount = 0;
            ServerIP = anhui.MahjongCommonMethod.Instance.SeverIp;
            ServerPort = anhui.MahjongCommonMethod.Instance.SeverPort;

            Debug.Log("Connet to serverIP: " + ServerIP + "，ServerPort:" + ServerPort);
        }

        // <summary>
        /// 连接报名服务器
        /// </summary>
        public void Connect()
        {
            tryCount++;
            //设置回调函数
            NetClient.SetCallBack(OnNetMessage, OnDisConnect, OnConnectResult);
            if (NetworkMgr.Instance.ProxyEnable == 1)
            {
                if (proxyIndex == 0)
                {
                    proxyIndex = 1 + Random.Range(0, NetworkMgr.Instance.ProxyInfoList.Count);
                }
                Debug.LogWarning("开始连接服务器" + "开始通过代理[" + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].ip + ":"
                    + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].port + "]连接游戏报名服务器["
                    + ServerIP + ":" + ServerPort + "]");

                DEBUG.Networking(DEBUG.TRACER_LOG + "开始通过代理[" + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].ip + ":"
                    + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].port + "]连接游戏报名服务器["
                    + ServerIP + ":" + ServerPort + "]");

                //把代理网址转为IP地址
                IPHostEntry ipHost = Dns.GetHostEntry(NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].ip);
                IPAddress[] ipAddr = ipHost.AddressList;
                NetClient.ConnectServer(NetComm.NetProxy.ProxyType.PROXY_HTTP,
                                        ipAddr[0].ToString(), NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].port,
                                        ServerIP, ServerPort, "", "");
            }
            else
            {
                proxyIndex = 0;                

                Debug.LogError("[LobbyNetMgr] " + "开始连接游戏报名服务器[" + ServerIP + ":" + ServerPort + "]");

                NetClient.ConnectServer(ServerIP, ServerPort);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            NetClient.Disconnect();
            Connected = false;
            DEBUG.LogTracer("[LobbyNetMgr] " + "断开游戏报名服务器[" + ServerIP + ":" + ServerPort + "]");
        }


        /// <summary>
        /// 发送消息，把消息加入发送队列
        /// </summary>
        /// <param name='obj'>消息对象</param>
        /// <param name='type'>消息类型</param>
        /// <param name='encrypt'>是否加密</param>
        public void SendClientMsg(object obj, ushort type, bool encrypt)
        {

            MsgData msgData;
            try
            {
                msgData = new MsgData();
                msgData.cType = type;
                msgData.bEncrypt = encrypt;
                //msgData.obj = obj;
                byte[] item = ToByteArray(obj, type);
                msgData.obj = (object)item;
                sendMsgQueue.Add(msgData);
            }
            catch
            {
                DEBUG.Networking(DEBUG.TRACER_LOG + "Send message except.", LogType.Exception);
            }

        }

        byte[] ToByteArray(object obj, ushort type)
        {
            switch (type)
            {
                case NetMsg.CLIENT_SERVER_REQ:
                    {
                        NetMsg.ClentGateSeverReqDef msg = (NetMsg.ClentGateSeverReqDef)obj;
                        msg.MsgHeadInfo.Version = MahjongGame_AH.Network.Message.NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadInfo.MsgType = type;
                        return msg.toBytes();
                    }
            }
            byte[] ret = new byte[1];
            return ret;
        }

        /// <summary>
        /// 加锁，暂时不接收消息
        /// </summary>
        public void Lock()
        {
            NetClient.LockDelayMsg(true);
        }

        /// <summary>
        /// 解锁，重新开始接收消息
        /// </summary>
        public void Unlock()
        {
            NetClient.LockDelayMsg(false);
        }
        #endregion 公共成员方法

        #region 私有成员方法
        /// <summary>
        /// 当连接服务器失败
        /// </summary>
        void OnConnectServerFailed()
        {            
            Debug.LogWarning("[LobbyNetMgr] " + "第" + tryCount + "次连接游戏服务器，失败!");
            bool bTryAgain = true; //是否再尝试连接

            if (NetworkMgr.Instance.ProxyEnable == 0)
            { //当不用代理，重试3次
                if (tryCount >= 3)
                {
                    bTryAgain = false;
                    //当重连三次之后还是连不上服务器的时候，直接弹出提示框，让玩家检查网络重新登录                       
                }
            }
            else
            { 	//当使用代理，把所有代理都尝试1次
                if (tryCount < NetworkMgr.Instance.ProxyInfoList.Count)
                {
                    proxyIndex++;
                    if (proxyIndex > NetworkMgr.Instance.ProxyInfoList.Count)
                    {
                        proxyIndex = 1;
                    }
                }
                else
                {
                    bTryAgain = false;
                    //TODO
                    // UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED, TitlePanelSystem.QuitGame);
                }
            }

            if (bTryAgain)
            {
                Connect();
            }
        }

        /// <summary>
        /// 当连接服务器成功
        /// </summary>
        void OnConnectServerOK()
        {
            //TODO
            Debug.LogWarning("[LobbyNetMgr] " + "第" + tryCount + "次连接游戏服务器，成功！");
            Connected = true;            
            //SendSeverIPReq();
        }
        #endregion 私有成员方法

        #region 事件处理
        /// <summary>
        /// 当收到连接结果
        /// </summary>
        /// <param name='result'>结果</param>
        void OnConnectResult(int result)
        {
            Debug.LogError("result:" + result);
            if (result == 0)
            {
                OnConnectServerOK();
            }
            else
            {
                OnConnectServerFailed();
            }
        }

        /// <summary>
        /// 当断开连接
        /// </summary>
        /// <param name='iCloseByServer'>是否是服务器主动断开</param>
        void OnDisConnect(int iCloseByServer)
        {
            // Debug.LogWarning(TextConstant.NET_MSG_LOBBY_SERVER_DISCONNECT);

            Connected = false;
            if (iCloseByServer == 1)
            {
                NetClient.Disconnect();

                //				UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_MSG_LOBBY_SERVER_DISCONNECT, TitlePanelSystem.QuitGame);
            }
        }

        /// <summary>
        /// 当收到网络消息
        /// </summary>
        /// <param name='cMsgType'>消息类型</param>
        /// <param name='pMsg'>消息数据</param>
        /// <param name='len'>数据长度</param>
        void OnNetMessage(ushort cMsgType, byte[] pMsg, int len)
        {            
            switch (cMsgType)
            {               
                case NetMsg.CLIENT_SERVER_RES:
                    DEBUG.Networking(DEBUG.RECV_MSG_LOG + MsgID.ToString(cMsgType));
                    HanldeGateSeverRes(pMsg, len);
                    break;
                default:
                    DEBUG.Networking(DEBUG.TRACER_LOG + "未处理的接收消息类型：" + MsgID.ToString(cMsgType), LogType.Warning);
                    break;
            }
            NetworkMgr.Instance.OnNetMessage(cMsgType);
        }
        #endregion 事件处理

        #region 发送网络消息
        public void SendSeverIPReq()
        {            
            NetMsg.ClentGateSeverReqDef msg = new NetMsg.ClentGateSeverReqDef();

            SendClientMsg(msg, NetMsg.CLIENT_SERVER_REQ, false);
        }
        #endregion 发送网络消息

        #region 处理网络消息
        public const string MSG_LOGIN_SUCCESS = "LobbyNetMgr:MsgLoginSucess";
        public const string MSG_LOGIN_NEED_CREATE_PLAYER = "LobbyNetMgr:MsgLoginNeedCreatePlayer";
        public const string MSG_LOGIN_FAILED = "LobbyNetMgr:MSG_LOGIN_FAILED";
        public const string MSG_LOGIN_CLOSE_FAILED_DIALOG = "LobbyNetMgr:MSG_LOGIN_CLOSE_FAILED_DIALOG";

        void HanldeGateSeverRes(byte[] pMsg, int len)
        {            
            int iOffset = 0;
            NetMsg.ClientServerResDef msg = new NetMsg.ClientServerResDef();
            iOffset = msg.parseBytes(pMsg, iOffset);

           
            //连接游戏服务器
            switch (MahjongLobby_AH.SDKManager.Instance .IsConnectNetWork)
            {
                case 1:
                    NetworkMgr.Instance.GameServer.Init(msg.szIP, msg.iPort);
                    break;
                case 2:                    
                    NetworkMgr.Instance.GameServer.Init(NetComm.NetSocket.Hostname2ip(msg.domain), msg.iPort);
                    break;
                default:
                    break;
            }
            Disconnect();
            Debug.LogWarning("===========================0");
            NetworkMgr.Instance.GameServer.Connect();
        }

        #endregion 处理网络消息
    }
}

