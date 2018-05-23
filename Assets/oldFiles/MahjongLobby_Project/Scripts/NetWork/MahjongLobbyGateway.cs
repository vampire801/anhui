using UnityEngine;
using System.Collections.Generic;
using System.Net;
using MahjongLobby_AH.Network.Message;
using XLua;
using anhui;

namespace MahjongLobby_AH.Network
{
    [Hotfix]
    [LuaCallCSharp]
    public class MahjongLobbyGateway
    {
        /// <summary>
        /// 网关
        /// </summary>
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
        public int tryCount; //尝试连接次数

        #endregion 私有成员变量

        #region 公共成员方法

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
            ServerIP = LobbyContants.LOBBY_GATEWAY_IP; 	// CCDW_C 测试 TODO 修改大厅IP，加密后写入 gameconfig.txt , 并修改 GameConstant 里面的网址， PlayerProfile代理设置
            ServerPort = (ushort)LobbyContants.LOBBY_GATEWAY_PORT;
            Debug.Log("Connet to server. IP: " + ServerIP + ":" + ServerPort + "，当前时间：" + Time.realtimeSinceStartup);
        }

        /// <summary>
        /// 连接报名服务器
        /// </summary>
        public void Connect()
        {
            Debug.Log("开始连接:" + Time.realtimeSinceStartup);
            tryCount++;
            //设置回调函数
            NetClient.SetCallBack(OnNetMessage, OnDisConnect, OnConnectResult);

            if (NetworkMgr.Instance.ProxyEnable == 1)
            {
                if (proxyIndex == 0)
                {
                    proxyIndex = 1 + Random.Range(0, NetworkMgr.Instance.ProxyInfoList.Count);
                }

                DEBUG.Networking(DEBUG.TRACER_LOG + "开始通过代理[" + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].ip + ":"
                    + NetworkMgr.Instance.ProxyInfoList[proxyIndex - 1].port + "]连接报名服务器["
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
                Debug.LogWarning("[LobbyNetMgr] " + "开始连接大厅报名服务器[" + ServerIP + ":" + ServerPort + "]");
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
            Debug.LogWarning("[LobbyNetMgr] " + "断开大厅报名服务器[" + ServerIP + ":" + ServerPort + "]");
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
                        msg.MsgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadInfo.MsgType = type;
                        //GameData.Instance.Printbuff(msg.toBytes());
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
            SDKManager.Instance.IsDisConnect = true;
            DEBUG.LogTracer("[LobbyNetMgr] " + "第" + tryCount + "次连接大厅网关，失败!");
            bool bTryAgain = true; //是否再尝试连接

            if (NetworkMgr.Instance.ProxyEnable == 0)
            { //当不用代理，重试3次
                if (tryCount >= 3)
                {
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    bTryAgain = false;
                    //检测连不上网关服务器，但是网站可连接，弹出网站公告
                    Debug.LogError("发送公告登录请求=======================0");
                    string url = LobbyContants.MAJONG_PORT_URL + "LoginBulletin.x";
                    if (SDKManager.Instance.IOSCheckStaus == 1)
                    {
                        url = LobbyContants.MAJONG_PORT_URL_T + "LoginBulletin.x";
                    }
                    Dictionary<string, string> value = new Dictionary<string, string>();
                    value.Add("cityId", PlayerPrefs.GetInt("CityId").ToString());
                    value.Add("countyId", PlayerPrefs.GetInt("CountyId").ToString());

                    LobbyBulletin.Instance.Status = 2;
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, LobbyBulletin.Instance.GetBulletinData, "LoginBulletinData");

                    UIMainView.Instance.disConnect.UpdateShow(2, @"亲，大厅服务器连接不上，
请稍后重试或联系客服寻求帮助！");
                }
                else
                {
                    if (bTryAgain)
                    {
                        MahjongCommonMethod.Instance.RetryConnectSever(1);
                    }
                }
            }
            else
            {
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                //当使用代理，把所有代理都尝试1次
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
                }
            }

            //if (bTryAgain)
            //{
            //    Debug.LogError("重连===================================0");
            //    Connect();
            //}
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        void BtnOk()
        {
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("亲，您的网络不给力哦，请检查网络后重试");
                return;
            }
            Debug.LogError("切换后天台重连=================================2");
            MahjongCommonMethod.Instance.InitScene();
        }

        /// <summary>
        /// 当连接服务器成功
        /// </summary>
        void OnConnectServerOK()
        {
            SDKManager.Instance.IsDisConnect = false;
            DEBUG.LogTracer("[LobbyNetMgr] " + "第" + tryCount + "次连接大厅报名服务器，成功！:" + Time.realtimeSinceStartup);
            Connected = true;
            // SendNetMessage(SignupMsg.CLIENT_GATE_SERVER_REQ);
            Debug.LogWarning("请求当前大厅服务器IP");
            SendSeverIPReq();
        }

        #endregion 私有成员方法

        #region 事件处理

        /// <summary>
        /// 当收到连接结果
        /// </summary>
        /// <param name='result'>结果</param>
        void OnConnectResult(int result)
        {
            if (result == 0)
            {
                //关闭重连界面
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                OnConnectServerOK();
                MahjongCommonMethod.isIntivateDisConnct = false;
                MahjongCommonMethod.Instance.StopRetryConnectSever();
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
            Debug.LogWarning("网关服务器主动断开连接:" + iCloseByServer);
            Connected = false;
            if (iCloseByServer == 1)
            {
                NetClient.Disconnect();
                //如果刚从后台切回来，直接自动链接不显示界面
                if (RuningToBackGround.Instance.isChangeFromTable)
                {
                    UIMainView.Instance.disConnect.Ok();
                    RuningToBackGround.Instance.isChangeFromTable = false;
                }
                else
                {
                    UIMainView.Instance.disConnect.UpdateShow(1, TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED);
                }
            }
        }

        void Ok()
        {
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您的网络不可用，请检查您的网络设置");
                return;
            }
            Debug.LogError("切换后天台重连=================================3");
            MahjongCommonMethod.Instance.InitScene();
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

        //发送验证消息,服务器收到该消息会自动发送所有服务器信息和AuthenRes
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

        /// <summary>
        /// 网关回应消息处理
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HanldeGateSeverRes(byte[] pMsg, int len)
        {
            int iOffset = 0;
            NetMsg.ClientServerResDef msg = new NetMsg.ClientServerResDef();
            iOffset = msg.parseBytes(pMsg, iOffset);
            msg.NormalizeMarshaledString();
            if (msg.iError != 0)
            {
                Debug.LogError("请求大厅服务器失败，错误ID：" + msg.iError);
                return;
            }

            //连接游戏服务器
            switch (SDKManager.Instance.IsConnectNetWork)
            {
                case 1:
                    NetworkMgr.Instance.LobbyServer.Init(msg.szIP, msg.usPort);
                    break;
                case 2:
                    NetworkMgr.Instance.LobbyServer.Init(NetComm.NetSocket.Hostname2ip(msg.domain), msg.usPort);
                    break;
                case 3:
                    NetworkMgr.Instance.LobbyServer.Init(LobbyContants.LOBBY_GATEWAY_IP, msg.usPort);
                    break;
                default:
                    break;
            }

            Disconnect();

            //Debug.LogError("请求大厅服务器回应消息处理========================，ip:" + msg.szIP+ ",usPort:" + msg.usPort);
            NetworkMgr.Instance.LobbyServer.Connect();
        }

        #endregion 处理网络消息
    }
}

