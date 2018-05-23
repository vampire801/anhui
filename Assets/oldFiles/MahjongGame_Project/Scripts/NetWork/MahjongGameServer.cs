using UnityEngine;
using System.Collections.Generic;
using MahjongGame_AH.Network;
using System.Text;
using MahjongGame_AH.Data;
using System.Net;
using MahjongGame_AH.Network.Message;
using MahjongGame_AH.GameSystem.SubSystem;
using JPush;
using XLua;
using anhui;
namespace MahjongGame_AH.Network
{

    [Hotfix]
    [LuaCallCSharp]
    public class MahjongGameServer
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
        public int ServerPort;
        /// <summary>
        /// 取得公告和大奖信息成功
        /// </summary>
        public bool GetAwardBullFinished;

        public List<SavedMsgItem> m_SavedMsgList = new List<SavedMsgItem>();

        #endregion 公共成员变量

        #region 私有成员变量

        List<MsgData> sendMsgQueue = new List<MsgData>(); // 发送消息队列，由于是单线程，所以没有加互斥锁
        int proxyIndex; //使用代理的索引，从1开始，应该是1~5
        public int tryCount; //尝试连接次数
        const int messageCount = 10;  //每帧发消息的个数

        #endregion 私有成员变量

        #region 公共成员方法
        /// <summary>
        /// 更新，在NetworkMgr的Update中被调用
        /// </summary>
        public void Update()
        {
            if (Connected)
            {
                //发送消息
                for (int i = 0; i < GameConstants.messageCount; i++)
                {
                    if (sendMsgQueue.Count > 0)
                    {
                        MsgData msgData = sendMsgQueue[0];
                        NetClient.SendCliMsg(msgData.obj, msgData.cType, msgData.bEncrypt);
                        sendMsgQueue.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
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
        /// 游戏服务器的IP和端口是由报名服务器发给的，不读配置文件
        /// </summary>
        public void Init(string ip, int port)
        {
            tryCount = 0;
            ServerIP = ip;
            ServerPort = port;
        }


        /// <summary> 
        /// 连接游戏服务器
        /// </summary>
        public void Connect()
        {
            NetClient = new NetClient();
            tryCount++;
            //设置回调函数
            NetClient.SetCallBack(OnNetMessage, OnDisConnect, OnConnectResult);

            if (NetworkMgr.Instance.ProxyEnable == 1)
            {
                if (proxyIndex == 0)
                {
                    proxyIndex = 1 + UnityEngine.Random.Range(0, NetworkMgr.Instance.ProxyInfoList.Count);
                }

                DEBUG.Networking("开始通过" + proxyIndex + "号代理连接游戏服务器[" + ServerIP + ":" + ServerPort + "]");

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

                Debug.LogWarning("开始连接游戏服务器[" + ServerIP + ":" + ServerPort + "]");
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
            Debug.LogWarning("[LobbyNetMgr] " + "断开游戏服务器[" + ServerIP + ":" + ServerPort + "]");
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

        /// <summary>
        /// 发送附带结构体的消息
        /// </summary>
        /// <param name="item">消息数组</param>
        /// <param name="type">消息类型</param>
        /// <param name="encrypt">是否加密</param>
        public void SendClientMsg_(byte[] item, ushort type, bool encrypt)
        {

            MsgData msgData;
            try
            {
                msgData = new MsgData();
                msgData.cType = type;
                msgData.bEncrypt = encrypt;
                msgData.obj = item;
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
                case NetMsg.KEEP_ALIVE:
                    {
                        NetMsg.KeepAlive msg = (NetMsg.KeepAlive)obj;
                        msg.MsgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadInfo.MsgType = type;
                        //Debug.LogError("发送保持连接消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_AUTHEN_REQ:
                    {
                        NetMsg.ClientAuthenReq msg = (NetMsg.ClientAuthenReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("发送认证请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SIT_REQ:
                    {
                        NetMsg.ClientSitReq msg = (NetMsg.ClientSitReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("入座请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_ESCAPE_REQ:
                    {
                        NetMsg.ClientEscapeReq msg = (NetMsg.ClientEscapeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("离座请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_READY_REQ:
                    {
                        NetMsg.ClientReadyReq msg = (NetMsg.ClientReadyReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("准备请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_DISCARD_TILE_REQ:
                    {
                        NetMsg.ClientDiscardTileReq msg = (NetMsg.ClientDiscardTileReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("出牌请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_DISMISS_ROOM_REQ:
                    {
                        NetMsg.ClientDismissRoomReq msg = (NetMsg.ClientDismissRoomReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("解散房间请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SPECIAL_TILE_REQ:
                    {

                        NetMsg.ClientSpecialTileReq msg = (NetMsg.ClientSpecialTileReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("吃碰杠胡抢请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_THIRTEEN_ORPHANS_REQ:
                    {
                        NetMsg.ClientThirteenOrphansReq msg = (NetMsg.ClientThirteenOrphansReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("长治花三门的十三幺请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_VOICE_REQ:
                    {
                        NetMsg.ClientVoiceReq msg = (NetMsg.ClientVoiceReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("玩家语音请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_PLAYING_METHOD_CONF_REQ:
                    {
                        NetMsg.ClientPlayingMethodConfReq msg = (NetMsg.ClientPlayingMethodConfReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("玩法配置请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CHAT_REQ:
                    {
                        NetMsg.ClientChatReq msg = (NetMsg.ClientChatReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("聊天请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CAN_DOWN_RU_REQ:
                    {
                        NetMsg.ClientCanDownRunReq msg = (NetMsg.ClientCanDownRunReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("长治潞城麻将的能跑能下请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_READHAND_REQ:
                    {
                        NetMsg.ClientReadHandReq msg = (NetMsg.ClientReadHandReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("报听请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_BESPEAK_REQ:
                    {
                        NetMsg.ClientUserBespeakReqDef msg = (NetMsg.ClientUserBespeakReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("用户占座请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CANCLE_BESPEAK_REQ:
                    {
                        NetMsg.ClientUserCancleBespeakReqDef msg = (NetMsg.ClientUserCancleBespeakReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("用户取消占座请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_OPEN_RP17_REQ:
                    {
                        NetMsg.ClientOpenRp17Req msg = (NetMsg.ClientOpenRp17Req)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("抢红包请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CANCLE_AUTO_STATUS_REQ:
                    {
                        NetMsg.ClientCancleAutoStatusReq msg = (NetMsg.ClientCancleAutoStatusReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("用户取消托管请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SHARE_SUCCESS_REQ:
                    {
                        NetMsg.ClientShareSuccessReq msg = (NetMsg.ClientShareSuccessReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("分享成功请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_OPEN_RECEIVE_RED_REQ:
                    {
                        NetMsg.ClientOpenReceiveRedReqDef msg = (NetMsg.ClientOpenReceiveRedReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("领取红包请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_READY_TIME_REQ:
                    {
                        NetMsg.ClientReadyTimeReqDef msg = (NetMsg.ClientReadyTimeReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取准备剩余时间请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_APPLIQUE_REQ:
                    {
                        NetMsg.ClientAppliqueReq msg = (NetMsg.ClientAppliqueReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("补花请求消息");
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
            DEBUG.LogTracer("[LobbyNetMgr] " + "第" + tryCount + "次连接游戏服务器，失败!");
            bool bTryAgain = true; //是否再尝试连接            

            MahjongCommonMethod.isConnectGameSeverFailed = true;
            if (NetworkMgr.Instance.ProxyEnable == 0)
            { //当不用代理，重试3次
                if (tryCount >= 3)
                {
                    MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    bTryAgain = false;
                    UIMainView.Instance.disConnect.UpdateShow(1, @"亲，游戏服务器连接不上，
请稍后重试或联系客服寻求帮助！");
                }
                else
                {
                    if (bTryAgain)
                    {
                        MahjongCommonMethod.Instance.RetryConnectSever(3);
                    }
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
                    UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_SEVER_FAILED, ReturnLobby);
                }
            }
        }

        /// <summary>
        /// 当连接服务器成功
        /// </summary>
        void OnConnectServerOK()
        {
            DEBUG.LogTracer("[LobbyNetMgr] " + "第" + tryCount + "次连接游戏服务器，成功!");
            Connected = true;
            //连接成功，发送玩家认证请求            
            NetMsg.ClientAuthenReq msg = new NetMsg.ClientAuthenReq();
            msg.wVer = MahjongLobby_AH.LobbyContants.SeverVersion;
            msg.iAuthenType = 3;
            msg.iUserId = MahjongCommonMethod.Instance.iUserid;  // MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId;
                                                                 //Debug.LogError("szAccessToken:" +PlayerPrefs.GetString("szaccess_token"));

            msg.szToken = MahjongCommonMethod.Instance.accessToken;
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            msg.szIp = MahjongCommonMethod.PlayerIp;
            //    MahjongLobby_AH.SDKManager.Instance.GetIP(()=> {  });
            //    Debug.LogWarning("1设备IP：" + msg.szIp);
            MahjongLobby_AH.SDKManager.Instance.GetIP(() => { msg.szIp = MahjongCommonMethod.PlayerIp; });
            if (MahjongCommonMethod.Instance.isMoNiQi)
            {
                msg.fLongitude = 0;
                msg.fLatitude = 0;
                msg.szAddress = "";
            }
            else
            {
                msg.fLatitude = MahjongCommonMethod.Instance.fLatitude;
                msg.fLongitude = MahjongCommonMethod.Instance.fLongitude;
                msg.szAddress = MahjongCommonMethod.Instance.sPlayerAddress;
            }

            msg.iRegistSource = MahjongLobby_AH.LobbyContants.iChannelVersion;
            msg.szRegistMac = MahjongCommonMethod.Instance.MacId;

            NetworkMgr.Instance.GameServer.SendAuthenReq(msg);
            //发送保持连接消息
            NetworkMgr.Instance.KeepGameServerAlive();
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
                MahjongCommonMethod.Instance.StopRetryConnectSever();
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
            Connected = false;
            Debug.LogWarning("游戏网络断开连接====================0iCloseByServer:" + iCloseByServer);
            NetClient.Disconnect();

            //开始自动连接
            if (!MahjongCommonMethod.isIntivateDisConnct)
            {
                tryCount = 0;
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在重连游戏服务器，请稍候!");
                Connect();
            }



            //if (iCloseByServer == 1)
            //{
            //    NetClient.Disconnect();
            //}

        }

        void CheckNetwork(int status)
        {
            if (!UIMainView.Instance.disConnect)
            {
                return;
            }

            UIMainView.Instance.disConnect.UpdateShow(1, "亲，您的网络不给力哦，请检查网络后重试");

            //if (status == 0)
            //{
            //    UIMainView.Instance.disConnect.UpdateShow(1, "亲，您的网络不给力哦，请检查网络后重试");
            //}
            //else
            //{
            //    if (MahjongCommonMethod.NetWorkStatus_int == 0)
            //    {
            //        UIMainView.Instance.disConnect.UpdateShow(2, "服务器正在维护中，请稍后登录…");
            //    }
            //    else
            //    {

            //    }
            //}
        }

        /// <summary>
        /// 当收到网络消息
        /// </summary>
        /// <param name='cMsgType'>消息类型</param>
        /// <param name='pMsg'>消息数据</param>
        /// <param name='len'>数据长度</param>
        void OnNetMessage(ushort cMsgType, byte[] pMsg, int len)
        {
            DEBUG.NetworkServer(cMsgType, pMsg);

            switch (cMsgType)
            {
                case NetMsg.CLIENT_AUTHEN_RES:
                    Debug.Log("玩家认证请求回应消息");
                    HandleAuthenRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_AGAIN_LOGIN_RES:
                    Debug.Log("断线重入回应消息");
                    HandleAgainLoginRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OTHER_AGAIN_LOGIN_NOTICE:
                    Debug.Log("他人断线重入通知消息");
                    HandleOtherAgainLoginNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_USER_JOIN_TABLE:
                    Debug.Log("用户入座消息002");
                    HandleUserJoinTable(pMsg, len);
                    break;
                case NetMsg.CLIENT_USER_LEAVE_TABLE:
                    Debug.Log("用户离座消息003");
                    HandleUserLevelTable(pMsg, len);
                    break;
                case NetMsg.CLIENT_SIT_RES:
                    Debug.Log("用户坐下回应消息007");
                    HandleSitRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_ESCAPE_RES:
                    Debug.Log("离开回应消息009");
                    HandleEscapeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_READY_RES:
                    Debug.Log("准备回应消息008");
                    HandleReadyRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_DISMISS_ROOM_RES:
                    Debug.Log("解散房间回应消息011");
                    HandleDismissRoomRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_DISMISS_ROOM_NOTICE:
                    Debug.Log("解散房间通知消息012");
                    HandleDismissRoomNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_DEAL_TILE_NOTICE:
                    Debug.Log("发牌通知消息100");
                    HandleDealTileNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_DISCARD_TILE_NOTICE:
                    Debug.Log("出牌通知消息101::");
                    HandleDiscardTileNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_DISCARD_TILE_RES:
                    Debug.Log("出牌回应消息103:" + Time.realtimeSinceStartup);
                    HandleDiscardTileRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SPECIAL_TILE_NOTICE:
                    Debug.Log("吃碰杠胡抢通知消息104");
                    HandleSpecialTileNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_SPECIAL_TILE_RES:
                    Debug.Log("吃碰杠胡抢回应消息0x1106");
                    HandleSpecialTileRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_REALTIME_POINT_NOTICE:
                    //Debug.Log("及时计分通知消息0x1107");
                    HandleRealtimePointNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_THIRTEEN_ORPHANS_RES:
                    Debug.Log("长治花三门的十三幺回应消息108");
                    HandleThirteenOrphansRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_VOICE_RES:
                    Debug.Log("语音回应消息");
                    HandleClientVoiceRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GAME_RESULT_NOTICE:
                    Debug.Log("一局游戏结果通知消息");
                    HandleGameReultNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_ROOM_RESULT_NOTICE:
                    Debug.Log("房间游戏结果通知消息");
                    HandleRoomResultNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_PLAYING_METHOD_CONF_RES:
                    Debug.Log("玩法配置回应消息");
                    HandlePlayingMethodConfRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CHAT_RES:
                    Debug.Log("聊天回应消息");
                    HandleChatRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CAN_DOWN_RU_NOTICE:
                    Debug.Log("长治潞城麻将的能跑能下通知消息");
                    HandleClientCanDownRuNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_CAN_DOWN_RU_RES:
                    //Debug.Log("长治潞城麻将的能跑能下回应消息");
                    HandleClientCanDownRuRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_READHAND_RES:
                    Debug.Log("报听回应消息");
                    HandleClientReadHandRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_LAIZI_NOTICE:
                    Debug.Log("癞子回应消息");
                    HandleClientltLaiZiNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_BESPEAK_RES:
                    Debug.Log("用户占座回应消息");
                    HandleClientUserBespeakRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CANCLE_BESPEAK_RES:
                    Debug.Log("用户取消占座回应消息");
                    HandleClientUserCancleBespeakRes(pMsg, len);
                    break;

                case NetMsg.SERVER_TO_CLIENT_WAIT_READY_NOTICE:
                    Debug.Log("四人入座等待用户准备/取消准备通知");
                    HandleClientWaitReadyNotice(pMsg, len);
                    break;
                case NetMsg.KICK_OUT_PLAYER_WITHOUT_READY:
                    Debug.Log("踢出为准备用户通知");
                    HandlePlayerWithoutReady(pMsg, len);
                    break;
                case NetMsg.CLIENT_RP17_START_NOTICE:
                    Debug.Log("开始抢红包通知");
                    HandleClientRp17StartNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_RP17_RES:
                    Debug.Log("抢红包回应");
                    HandleClientOpenRp17Res(pMsg, len);
                    break;
                case NetMsg.SERVER_TO_CLIENT_AUTO_STATUS:
                    Debug.Log("发送用户托管状态给客户端通知消息");
                    HandleClientAutoStatus(pMsg, len);
                    break;
                case NetMsg.CLIENT_CANCLE_AUTO_STATUS_RES:
                    Debug.Log("用户取消托管回应消息");
                    HandleClientCanelAutoStatus(pMsg, len);
                    break;
                case NetMsg.CLIENT_BESPEAK_USERINFO_NOTICE:
                    Debug.Log("桌上游戏外占座玩家信息");
                    HandleClientBespeakUserInfoDef(pMsg, len);
                    break;
                case NetMsg.CLIENT_SHARE_SUCCESS_RES:
                    Debug.Log("分享成功回应消息");
                    HandleSharedSuccessRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_RECEIVE_RED_RES:
                    Debug.Log("领取红包回应消息");
                    HandleClientReceiveRedRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OBTAIN_RED_NOTICE:
                    Debug.Log("通知用户获得一个红包消息");
                    HandleClientObtainRedNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_OBTAIN_RECEIVE_RED_NOTICE:
                    Debug.Log("通知用户获得一个待领状态红包消息");
                    HandleClientObtainReceiveRedNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_READY_TIME_RES:
                    Debug.Log("获取准备剩余时间回应");
                    HandleClientReadyTimeResDef(pMsg, len);
                    break;
                case NetMsg.BESPEAK_TIME_OUT_NOTICE:
                    Debug.Log("预约时间结束通知");
                    HandleBespeakTimeOutNoticeDef(pMsg, len);
                    break;
                case NetMsg.CLIENT_APPLIQUE_NOTICE:
                    Debug.Log("补花通知消息");
                    HandleClientAppliqueNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_APPLIQUE_RES:
                    Debug.Log("补花回应消息");
                    HandleClientAppliqueRes(pMsg, len);
                    break;
                default:
                    //Debug.LogWarning("未处理的消息类型：" + cMsgType);
                    break;
            }

            NetworkMgr.Instance.OnNetMessage(cMsgType);
        }

        #endregion 事件处理

        #region 发送网络消息

        //KEEP_ALIVE_MSG = 0xFF; //保持连接
        public void SendKeepAlive()
        {
            NetMsg.KeepAlive msg = new NetMsg.KeepAlive();
            SendClientMsg(msg, NetMsg.KEEP_ALIVE, false);
        }

        //CLIENT_SIT_REQ		0x1006	// [游客]->[游服]坐下请求消息
        public void SendSitReq(NetMsg.ClientSitReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SIT_REQ, false);
        }

        //CLIENT_ESCAPE_REQ		0x1008	// [游客]->[游服]离开请求消息
        public void SendEscapeReq(NetMsg.ClientEscapeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_ESCAPE_REQ, false);
        }

        //CLIENT_READY_REQ		0x100A	// [游客]->[游服]准备请求消息
        public void SendReadyReq(NetMsg.ClientReadyReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_READY_REQ, false);
        }

        //CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public void SendAuthenReq(NetMsg.ClientAuthenReq msg)
        {
            msg.szRegistImei = "NOIMEI";
            MahjongCommonMethod.Instance.isFinshSceneLoad = true;
            SendClientMsg(msg, NetMsg.CLIENT_AUTHEN_REQ, false);
        }
        /// <summary>
        /// DISMISS_ROOM_REQ =0x1010	// [游客]->[游服]解散房间请求消息
        /// </summary>
        public void SendDisMissRoomReq(NetMsg.ClientDismissRoomReq msg)
        {
            GameData.Instance.PlayerPlayingPanelData.isCanHandCard = false;
            GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame = 0;
            SendClientMsg(msg, NetMsg.CLIENT_DISMISS_ROOM_REQ, false);
        }
        /// <summary>
        /// CLIENT_DISCARD_TILE_REQ=0x1102	// [游客]->[游服]出牌请求消息
        /// </summary>
        public void SendDiscardTileReq(NetMsg.ClientDiscardTileReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_DISCARD_TILE_REQ, false);
        }
        /// <summary>
        /// CLIENT_SPECIAL_TILE_REQ	=0x1105	// [游客]->[游服]吃碰杠胡抢请求消息
        /// </summary>
        public void SendSpecialTileReq(NetMsg.ClientSpecialTileReq msg)
        {
            Debug.LogWarning("发送的牌：" + msg.byaTiles[0] + "," + msg.byaTiles[1]);
            SendClientMsg(msg, NetMsg.CLIENT_SPECIAL_TILE_REQ, false);
        }
        /// <summary>
        /// CLIENT_THIRTEEN_ORPHANS_REQ		=		0x1107	// [游客]->[游服]长治花三门的十三幺请求消息
        /// </summary>
        public void SendThirteenOrphansRes(NetMsg.ClientThirteenOrphansReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_THIRTEEN_ORPHANS_REQ, false);
        }

        //发送玩家语音请求消息
        public void SendVoiceReq(NetMsg.ClientVoiceReq msg)
        {
            Debug.Log("发送语音请求消息");
            SendClientMsg(msg, NetMsg.CLIENT_VOICE_REQ, false);
        }

        //发送玩法配置请求消息
        public void SendPlayingMethodConfReq(NetMsg.ClientPlayingMethodConfReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_PLAYING_METHOD_CONF_REQ, false);
        }
        //发送玩家聊天请求
        public void SendChatReq(NetMsg.ClientChatReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CHAT_REQ, false);
        }
        //发送长治潞城麻将的能跑能下请求消息
        public void SendClientCanDownRuReq(NetMsg.ClientCanDownRunReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CAN_DOWN_RU_REQ, false);
        }

        //发送报听请求消息
        public void SendClientReadHandReq(NetMsg.ClientReadHandReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_READHAND_REQ, false);
        }


        //用户占座请求消息
        public void SendClientUserBespeakReq(NetMsg.ClientUserBespeakReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_BESPEAK_REQ, false);
        }
        //用户取消占座请求消息
        public void SendClientUserCancleBespeakReq(NetMsg.ClientUserCancleBespeakReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CANCLE_BESPEAK_REQ, false);
        }

        //抢红包请求
        public void SendClientOpenRp17Req(NetMsg.ClientOpenRp17Req msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_OPEN_RP17_REQ, false);
        }

        //用户取消托管请求消息
        public void SendClientCancleAutoStatusReq(NetMsg.ClientCancleAutoStatusReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CANCLE_AUTO_STATUS_REQ, false);
        }

        //CLIENT_SHARE_SUCCESS_REQ = 0x029A;	// [厅客]->[厅服]分享成功请求消息
        public void SendShareSuccessReq(NetMsg.ClientShareSuccessReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SHARE_SUCCESS_REQ, false);
        }

        //领取红包请求消息
        public void SendClientReceiveRedReq(NetMsg.ClientOpenReceiveRedReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_OPEN_RECEIVE_RED_REQ, false);
        }
        //获取准备剩余时间请求
        public void SendClientReadyTimeReqDef(NetMsg.ClientReadyTimeReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_READY_TIME_REQ, false);
        }

        //补花请求消息
        public void SendClientAppliqueReq(NetMsg.ClientAppliqueReq msg)
        {
            //Debug.LogError("花牌张数:" + msg.byFlowerTileNum + ",花牌数：" + msg.byaFlowerTile[0] + "," + msg.byaFlowerTile[1] + "," + msg.byaFlowerTile[2]);
            SendClientMsg(msg, NetMsg.CLIENT_APPLIQUE_REQ, false);
        }
        #endregion 发送网络消息

        #region 收到网络消息

        /// <summary>
        /// 玩家认证回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleAuthenRes(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            NetMsg.ClientAuthenRes msg = new NetMsg.ClientAuthenRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("玩家认证回应消息长度不一致");
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("玩家认证回应消息错误编号" + msg.iError + ":" + MahjongLobby_AH.Err_ClientAuthenRes.Err(msg.iError));
                //玩家重复登陆
                if (msg.iError == 11 || msg.iError == 22)
                {
                    if (MahjongCommonMethod.Instance.isGameDisConnect)
                    {
                        //延迟三秒之后，重新发送认证请求
                        MahjongCommonMethod.Instance.isGameDisConnect = false;
                        NewPlayerGuide.Instance.SendPlayerAutnenReq(3f);
                    }
                    else
                    {
                        UIMgr.GetInstance().GetUIMessageView().Show("您的账号在其他地方请求登录，账号异常请重新登录！", LoginAccount);
                        MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    }
                }
                else
                {
                    MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    switch (msg.iError)
                    {
                        case 23: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号已经在其他设备登录，重入失败！", LoginAccount); break;
                        case 4: UIMgr.GetInstance().GetUIMessageView().Show("当前服务器人数已满，请稍候登录！", LoginAccount); break;
                        case 3:
                        case 5:
                        case 6:
                        case 7:
                        case 12:
                        case 13:
                        case 14:
                        case 17:
                        case 18:
                        case 16: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号登录异常，请重新登录！", LoginAccount); break;
                        case 8: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号被禁用，请联系客服！", LoginAccount); break;
                        case 15: UIMgr.GetInstance().GetUIMessageView().Show("您的版本号不匹配，请下载最新版本！", LoginAccount); break;
                        case 2:
                        case 20: UIMgr.GetInstance().GetUIMessageView().Show("游戏服务器不可用，请联系客服！", LoginAccount); break;
                        default: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号登录异常，请重新登录！", LoginAccount); break;
                    }
                }
                return;
            }
            MahjongCommonMethod.Instance.isStartInit_Game = false;
            MahjongCommonMethod.isAuthenSuccessInLobby = false;
            PlayerNodeDef pnd = new PlayerNodeDef(msg.clientUser.iUserId, msg.clientUser.szNickname, msg.clientUser.szHeadimgurl);
            GameData.Instance.PlayerNodeDef = pnd;
            MahjongLobby_AH.SDKManager.Instance.isex = msg.clientUser.bySex;
            pppd.iUserId = msg.clientUser.iUserId;
            MahjongCommonMethod.isGameToLobby = true;

            //删除显示更新界面的预置体
            if (CheckUpdateManager.Instance)
            {
                CheckUpdateManager.Instance.Destroy_Self();
            }

            //延迟发送
            //MahjongCommonMethod.Instance.SendSitRes();

            //玩家认证回应消息成功之后，发送玩家入座消息
            NetMsg.ClientSitReq sit = new NetMsg.ClientSitReq();
            sit.iRoomNum = System.Convert.ToInt32(MahjongCommonMethod.Instance.RoomId);
            sit.sTableNum = (ushort)MahjongCommonMethod.Instance.iTableNum;
            sit.iUserId = MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId;
            sit.iParlorId = MahjongCommonMethod.Instance.iParlorId;
            NetworkMgr.Instance.GameServer.SendSitReq(sit);

            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
            {
                //预加载大厅场景
                SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_LOBBY_GENERAL);

                return;
            }
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Debug.LogWarning("--------语音进入初始化");
                //  Loom.RunAsync(() =>
                //{
                VoiceManegerMethord.Instance.VoiceInit();
                //});
            }

            //预加载大厅场景
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_LOBBY_GENERAL);
            sw.Stop();
            Debug.Log("预加载大厅场景时间：====================" + sw.ElapsedMilliseconds);

        }
        int userSeatNum;
        /// <summary>
        /// 处理玩家断线重入回应消息 0x1000
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleAgainLoginRes(byte[] pMsg, int len)
        {
            //发送玩法配置请求
            NetMsg.ClientPlayingMethodConfReq msgreq = new NetMsg.ClientPlayingMethodConfReq();
            msgreq.iUserId = MahjongCommonMethod.Instance.iUserid;
            NetworkMgr.Instance.GameServer.SendPlayingMethodConfReq(msgreq);
            PoolManager.Clear();
            Debug.LogWarning("#######断线重入回应消息");
            NetMsg.ClientAgainLoginRes msg = new NetMsg.ClientAgainLoginRes();
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            PlayerPrefs.SetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo, "2222");
            MahjongCommonMethod.Instance.iTableNum = msg.sTableNum;
            pppd.playerPos = msg.cSeatNum;
            pppd.bySeatNum = msg.cSeatNum;

            JPushBinding.StopPush();
            UIMainView.Instance.PlayerPlayingPanel.m_gameStart = true;

            Debug.Log("msg.cUserNum:" + msg.cUserNum);

            for (int i = 0; i < msg.cUserNum; i++)
            {
                NetMsg.UserInfoDef msgg = new NetMsg.UserInfoDef();
                ioffset = msgg.parseBytes(pMsg, ioffset);
                Debug.Log("UserInfoDef长度 ：" + ioffset + "用户座位号：" + msgg.cSeatNum + ",位置信息:" + msgg.szAddress);
                //添加用户节点信息
                //显示头像               

                if (msgg.cSeatNum == msg.cSeatNum)
                {
                    pppd.iUserId = msgg.iUserId;
                    #region 用户节点数据保存
                    PlayerNodeDef pnd = new PlayerNodeDef(msgg.iUserId, msgg.szNickname, msgg.szHeadimgurl);
                    GameData.Instance.PlayerNodeDef = pnd;
                    MahjongLobby_AH.SDKManager.Instance.isex = msgg.bySex;
                    #endregion  用户节点数据保存
                }
                //添加其他用户的信息
                if (!pppd.usersInfo.ContainsKey(msgg.cSeatNum))
                {
                    pppd.usersInfo.Add(msgg.cSeatNum, msgg);
                }
                else
                {
                    pppd.usersInfo[msgg.cSeatNum] = msgg;
                }


                if (msgg.byDisconnectType != 0)
                {
                    UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(msgg.cSeatNum) - 1, true);
                }
                SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow((msgg.cSeatNum));
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[i].SetActive(false);
            //}


            UIMainView.Instance.PlayerPlayingPanel.InitPanel();

            NetMsg.AgainLoginGameData msg2 = new NetMsg.AgainLoginGameData();
            ioffset = msg2.parseBytes(pMsg, ioffset);

            GameData.Instance.PlayerNodeDef.byLaiziSuit = (byte)(msg2.byLaiziTIle >> 4);
            GameData.Instance.PlayerNodeDef.byLaiziValue = (byte)(msg2.byLaiziTIle & 0x0f);

            //Debug.LogWarning("++++++"+msg2.byLaiziTIle.ToString("X2"));
            //  Debug.LogWarning("++++++" + GameData.Instance.PlayerNodeDef.byLaiziSuit+"  "+ GameData.Instance.PlayerNodeDef.byLaiziValue);

            for (int i = 0; i < 4; i++)
            {
                pppd.sScoreShow[i] = msg2.saTotalPoint[i];
            }

            pppd.iOpenRoomUserId = msg2.iOpenRoomId;

            //更新房主头像信息
            if (pppd.usersInfo.ContainsKey(1))
            {
                if (pppd.iOpenRoomUserId == pppd.usersInfo[1].iUserId)
                {
                    UIMainView.Instance.PlayerPlayingPanel.UpdataFangZhu(1);
                }
            }

            //更新补花数量
            for (int i = 0; i < 4; i++)
            {
                pppd.PlayerFlowerCount[i] = msg2.byFlowerTileNum[i];
            }

            MahjongCommonMethod.isGameToLobby = true;

            //关闭玩家的准备手势
            UIMainView.Instance.PlayerPlayingPanel.CloseReadyImage();

            pppd.LeftCardCount = msg2.byRemainTiles;

            pppd.FinshedGameNum = msg2.byInnings;

            Debug.LogWarning("基础局数:" + msg2.byInnings + ",基础圈数：" + msg2.byRounds);

            if (msg2.byRounds == 0)
            {
                pppd.FinshedQuanNum = 1;
            }
            else
            {
                pppd.FinshedQuanNum = msg2.byRounds;
            }

            pppd.byDealerSeat = msg2.byDealerSeat;
            GameData.Instance.AntiCheatingPanelData.iReadyShowAnti++;
            Debug.Log("庄家位置 处理玩家断线重入回应消息：" + pppd.GetOtherPlayerShowPos(msg2.byDealerSeat));
            UIMainView.Instance.PlayerPlayingPanel.SetDealer(pppd.GetOtherPlayerPos(msgreq.iUserId), pppd.GetOtherPlayerShowPos(msg2.byDealerSeat), true);//处理庄家显示
            UIMainView.Instance.PlayerPlayingPanel.ShowWitchPlayerPlay(msg2.byDiscardSeat - 1);//处理该谁出牌           
            for (int i = 0; i < 4; i++)
            {
                int index = pppd.GetOtherPlayerShowPos(i + 1) - 1;
                if (msg2.bybyAutoPlayState[i] == 1)
                {
                    UIMainView.Instance.PlayerPlayingPanel.ShowHosting(index, true);
                }
                else
                {
                    UIMainView.Instance.PlayerPlayingPanel.ShowHosting(index, false);
                }
            }

            if (msg2.bybyAutoPlayState[msg.cSeatNum - 1] == 1)
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(true);
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(false);
            }

            pppd.iPlayerHostStatus = msg2.bybyAutoPlayState[msg.cSeatNum - 1];
            pppd.iAllPlayerHostStatus = msg2.bybyAutoPlayState;
            pppd.isAllAutoStatus = true;
            for (int i = 0; i < 4; i++)
            {
                if (msg2.bybyAutoPlayState[i] == 0)
                {
                    pppd.isAllAutoStatus = false;
                    break;
                }
            }

            SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow(pppd.bySeatNum);
            int index_1 = pppd.GetOtherPlayerShowPos(pppd.bySeatNum);


            //预加载大厅场景
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_LOBBY_GENERAL);
            sw.Stop();
            Debug.Log("预加载大厅场景时间：====================" + sw.ElapsedMilliseconds + ",msg2.byDealStatus:" + msg2.byDealStatus
                + ",==:" + pppd.usersInfo.ContainsKey(pppd.bySeatNum) + ",准备状态:" + msg2.byDealStatus);


            //处理玩家是否处于准备状态
            if ((msg2.byDealStatus == 0 || msg2.byDealStatus == 2) && pppd.usersInfo.ContainsKey(pppd.bySeatNum))
            {
                pppd.usersInfo[pppd.bySeatNum].cReady = 0;
                UIMainView.Instance.PlayerPlayingPanel.ReadyImage[index_1].gameObject.SetActive(false);
                if (msg2.byDealStatus == 0)
                {
                    pppd.isPanelShow_Wating = true;
                    pppd.isPanelShow_Playing = false;
                    SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
                }
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                return;
            }
            else if (msg2.byDealStatus == 1 && pppd.usersInfo.ContainsKey(pppd.bySeatNum))
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                //显示玩家的准备手势
                pppd.usersInfo[pppd.bySeatNum].cReady = 1;
                UIMainView.Instance.PlayerPlayingPanel.ReadyImage[index_1].gameObject.SetActive(true);
                return;
            }

            pppd.isPanelShow_Wating = false;
            pppd.isPanelShow_Playing = true;

            pppd.againLoginData = msg2;
            pppd.bLastCard = msg2.byTableTile;
            //数据恢复
            UIMainView.Instance.PlayerPlayingPanel.MoveHeadImage();
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();

            UIMainView.Instance.PlayerPlayingPanel.CloseReadyImage();

            for (int i = 0; i < 4; i++)
            {
                if (msg2.resultType[i].byTripletNum > 0)
                {
                    pppd.iGangNum += msg2.resultType[i].byTripletNum;
                    //break;
                }
            }

            //显示过胡提示
            if (msg2.byOverWater > 0)
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowSkipWin(true);
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowSkipWin(false);
            }

            if (msg2.resultType[0].byThirteenOrphansNum > 0)
            {
                pppd.iTirState = 2;
            }
            else if (msg2.resultType[0].byTripletNum > 0)
            {
                pppd.iTirState = 1;
            }
            else
            {
                pppd.iTirState = 0;
            }
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 0)
            {
                if (Application.platform != RuntimePlatform.WindowsEditor)
                {
                    Debug.Log("执行语音初始化");
                    //  Loom.RunAsync(() =>
                    // {
                    VoiceManegerMethord.Instance.VoiceInit();
                    // });

                }
            }

            pppd.isChoiceTing = msg2.byReadHandStauts[msg.cSeatNum - 1];
            pppd.isChoiceTing_ALL = msg2.byReadHandStauts;
            //   userSeatNum = msg2.byDrawSeat;
            pppd.TransCardsData(msg2.byDrawSeat);

            for (int i = 0; i < 4; i++)
            {
                if (msg2.byReadHandStauts[i] > 0)
                {
                    //Debug.LogError("msg2.byReadHandStauts[i]：" + msg2.byReadHandStauts[i] + ",i:" + i);
                    int index = pppd.GetOtherPlayerShowPos(i + 1) - 1;
                    UIMainView.Instance.PlayerPlayingPanel.UpdatePlayerTingStatus(index);
                }
            }


            GameData.Instance.PlayerPlayingPanelData.isChoicTir = msg2.byThirteenOrphans;

            pppd.isFirstDealCard = false;
            pppd.isBeginGame = true;
            pppd.iBeginRoundGame = 0;

            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");


            //恢复玩家如果正在发起解散房间状态
            if (msg2.ttDismiss > 0)
            {
                //显示界面
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(true);
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localPosition = Vector3.zero;
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localScale = Vector3.one;

                ulong nowTime = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().timer -= nowTime - (ulong)msg2.ttDismiss;
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().iseatNum = msg2.byDismissSeat;
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().ShowAllPlayerMessage();

                //更新界面
                for (int i = 0; i < msg2.byaDismiss.Length; i++)
                {
                    if (i != msg2.byDismissSeat - 1)
                    {
                        if (msg2.byaDismiss[i] == 1)
                        {
                            UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().UpdateShow(i + 1, 1);
                        }

                    }
                }
            }

            //处理玩家重入补花
            for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
            {
                if (pppd.usersCardsInfo[0].listCurrentCards[i].cardNum > 100)
                {
                    //在这里处理玩家时候手中有花牌
                    byte[] temp = pppd.GetPlayerFlowerCard();

                    //如果玩家手中有花牌请求花牌信息
                    if (temp.Length > 0)
                    {
                        NetMsg.ClientAppliqueReq msg_que = new NetMsg.ClientAppliqueReq();
                        msg_que.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                        msg_que.byFlowerTileNum = (byte)temp.Length;
                        for (int j = 0; j < 8; j++)
                        {
                            if (j < temp.Length)
                            {
                                msg_que.byaFlowerTile[j] = temp[j];
                            }
                            else
                            {
                                msg_que.byaFlowerTile[j] = 0;
                            }
                        }
                        NetworkMgr.Instance.GameServer.SendClientAppliqueReq(msg_que);
                    }
                    break;
                }
            }
            //恢复玩家的防作弊信息
            //打开防作弊面板
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus != 1)
            {
                acpd.iReadyShowAnti++;
            }
            else
            {
                return;
            }

#if UNITY_ANDROID
            for (int i = 0; i < 4; i++)
            {
                if (i == pppd.bySeatNum - 1)
                {
                    //ip赋值
                    acpd.Ip[i] = MahjongCommonMethod.PlayerIp;
                    acpd.PlayerPos[i, 0] = MahjongCommonMethod.Instance.fLongitude;
                    acpd.PlayerPos[i, 1] = MahjongCommonMethod.Instance.fLatitude;
                }
                else
                {
                    if (pppd.usersInfo.ContainsKey(i + 1))
                    {
                        //ip赋值
                        acpd.Ip[i] = pppd.usersInfo[i + 1].szIp;
                        //经纬度赋值
                        acpd.PlayerPos[i, 0] = pppd.usersInfo[i + 1].fLongitude;
                        acpd.PlayerPos[i, 1] = pppd.usersInfo[i + 1].fLatitude;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 他人断线重入通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleOtherAgainLoginNotice(byte[] pMsg, int len)
        {
            NetMsg.ClientOtherAgainLoginNotice msg = new NetMsg.ClientOtherAgainLoginNotice();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //保存玩家的离线信息
            if (pppd.GetOtherPlayerPos(msg.iUserId) > 0 && pppd.GetOtherPlayerPos(msg.iUserId) < 5)
            {
                pppd.DisConnectStatus[pppd.GetOtherPlayerPos(msg.iUserId) - 1] = 0;
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.join_match, false, false);
                UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId)) - 1, false);
            }
        }

        /// <summary>
        /// 用户入座消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleUserJoinTable(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientUserJoinTable msg = new NetMsg.ClientUserJoinTable();
            NetMsg.UserInfoDef msgg = new NetMsg.UserInfoDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            ioffset = msgg.parseBytes(pMsg, ioffset);

            Debug.Log("用户入座消息");
            if (!pppd.usersInfo.ContainsKey(msgg.cSeatNum))
            {
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.join_match, false, false);
                pppd.usersInfo.Add(msgg.cSeatNum, msgg);
                SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow(msgg.cSeatNum);
            }
            else
            {
                pppd.usersInfo[msgg.cSeatNum] = msgg;
            }

            if (pppd.usersInfo.ContainsKey(msgg.cSeatNum))
            {
                int pos = pppd.GetOtherPlayerShowPos(pppd.usersInfo[msgg.cSeatNum].cSeatNum) - 1;
                UIMainView.Instance.PlayerPlayingPanel.PlayerOutRoom[pos].gameObject.SetActive(false);
            }

            UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(false, 0);

        }

        /// <summary>
        /// 用户离座消息0x1003
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleUserLevelTable(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientUserLeaveTable msg = new NetMsg.ClientUserLeaveTable();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("用户离座消息长度错误");
                return;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            Debug.LogWarning("哪种掉线类型？" + msg.cLeaveType);
            int pos = msg.cSeatNum;
            if (pos != pppd.playerPos)
            {
                if (SystemMgr.Instance)
                {
                    SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.leave_game, false, false);
                }
            }

            switch (msg.cLeaveType)
            {
                case 1://退出程序
                    if (pppd.usersInfo.ContainsKey(pos))
                    {
                        pppd.usersInfo.Remove(pos);
                        if (SystemMgr.Instance)
                            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                    }
                    break;
                case 2:
                    //删除玩家节点信息，更新界面
                    if (pppd.usersInfo.ContainsKey(pos))
                    {
                        pppd.usersInfo.Remove(pos);
                        if (SystemMgr.Instance)
                            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                    }
                    break;
                case 3://游戏状态下掉线
                    if (UIMainView.Instance)
                        UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(pos) - 1, true);
                    //保存玩家的离线信息
                    pppd.DisConnectStatus[pos - 1] = 1;
                    break;
                case 4://游戏状态下离开                    
                    break;
                case 5://预约房，预约用户已占座离开游戏
                    {
                        UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(3, pppd.GetOtherPlayerShowPos(pos) - 1);
                        int index = pppd.GetOtherPlayerShowPos(pos) - 1;
                        UIMainView.Instance.PlayerPlayingPanel._txUsersScor[index].text = "占座中";
                        pppd.DisConnectStatus[pos - 1] = 1;
                    }
                    break;
                case 6://非馆主开房离做
                    {
                        if (pppd.usersInfo.ContainsKey(pos))
                        {
                            pppd.DisConnectStatus[pos - 1] = 1;
                            pppd.usersInfo.Remove(pos);
                            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 坐下回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSitRes(byte[] pMsg, int len)
        {
            //修改玩家的坐下的标志位
            MahjongCommonMethod.Instance.isSitSuccess = true;
            JPushBinding.StopPush();//停止极光推送
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientSitRes msg = new NetMsg.ClientSitRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (msg.iError != 0)
            {
                Debug.LogWarning("坐下回应消息错误编号:" + msg.iError);
                switch (msg.iError)
                {
                    case 1: UIMgr.GetInstance().GetUIMessageView().Show("入座失败，请返回大厅！", ReturnLobby); break;
                    case 2: UIMgr.GetInstance().GetUIMessageView().Show("房间已满，无法加入房间！", ReturnLobby); break;
                    case 3: UIMgr.GetInstance().GetUIMessageView().Show("该房间已解散，请返回大厅", ReturnLobby); break;
                    case 4: UIMgr.GetInstance().GetUIMessageView().Show("该房间已解散，请返回大厅", ReturnLobby); break;
                    case 5: UIMgr.GetInstance().GetUIMessageView().Show("该房间已解散，请返回大厅", ReturnLobby); break;
                    case 6: UIMgr.GetInstance().GetUIMessageView().Show("该座位已被占，无法加入房间！", ReturnLobby); break;
                    case 7: UIMgr.GetInstance().GetUIMessageView().Show("您的赞数量不足，不满足该房间的条件", ReturnLobby); break;
                    case 8: UIMgr.GetInstance().GetUIMessageView().Show("您的掉线率过高，不满足该房间的条件", ReturnLobby); break;
                    case 9: UIMgr.GetInstance().GetUIMessageView().Show("您的出牌速过慢，不满足该房间的条件", ReturnLobby); return;
                    case 10: UIMgr.GetInstance().GetUIMessageView().Show("您已经在牌桌，无法加入其他房间！", ReturnLobby); break;
                    case 11: UIMgr.GetInstance().GetUIMessageView().Show("已在其它地方占座，无法加入其他房间！", ReturnLobby); break;
                    case 12: UIMgr.GetInstance().GetUIMessageView().Show("您还不是该馆成员，无法进入房间", ReturnLobby); break;
                    case 13: UIMgr.GetInstance().GetUIMessageView().Show("您的金币数量不足，无法进入房间", ReturnLobby); break;
                    case 14: UIMgr.GetInstance().GetUIMessageView().Show("房间已满，无法加入房间！", ReturnLobby); break;
                    default: UIMgr.GetInstance().GetUIMessageView().Show("入座失败，请返回大厅！", ReturnLobby); break;
                }
                return;
            }


            pppd.playerPos = msg.cSeatNum;
            pppd.iUserId = msg.iUserId;
            MahjongCommonMethod.Instance.mySeatid = msg.cSeatNum;
            pppd.bySeatNum = msg.cSeatNum;
            pppd.iOpenRoomUserId = msg.iOpenRoomUserId;
            pppd.isPanelShow_Wating = true;
            pppd.SeatID = msg.SeatId;
            UIMainView.Instance.PlayerPlayingPanel.CreatRoomTime = msg.CreatRoomTime;//创建房间的时间
            //是自己创建的房间 默认占座
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            pppd.byOpenRoomMode = msg.byOpenRoomMode;

            Debug.Log("同桌玩家数量:" + msg.cUserNum);

            //保存其他玩家信息
            for (int i = 0; i < msg.cUserNum; i++)
            {
                NetMsg.UserInfoDef msgg = new NetMsg.UserInfoDef();
                Debug.Log("玩家id:" + msgg.iUserId + ",昵称:" + msgg.szNickname);
                ioffset = msgg.parseBytes(pMsg, ioffset);
                if (!pppd.usersInfo.ContainsKey(msgg.cSeatNum))
                {
                    pppd.usersInfo.Add(msgg.cSeatNum, msgg);
                    SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow((msgg.cSeatNum));
                }
                else
                {
                    pppd.usersInfo[msgg.cSeatNum] = msgg;
                }

                if (pppd.usersInfo.ContainsKey(msgg.cSeatNum))
                {
                    if (msgg.cSeatNum == msg.cSeatNum && msg.SeatId[msgg.cSeatNum - 1] == 3)
                    {

                    }
                    else
                    {
                        UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(msg.SeatId[msgg.cSeatNum - 1], pppd.GetOtherPlayerShowPos(pppd.usersInfo[msgg.cSeatNum].cSeatNum) - 1);
                    }
                }
            }

            if (ioffset != len)
            {
                Debug.LogError("坐下回应消息的长度不一致,ioffset:" + ioffset + ", len:" + len);
                return;
            }

            if (pppd.usersInfo.ContainsKey(msg.cSeatNum))
            {
                UIMainView.Instance.PlayerPlayingPanel._txUsersScor[pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.cSeatNum].cSeatNum) - 1].text = "0";//[pppd.GetOtherPlayerShowPosForOtherPlayerID(msg.iUserId)].text = "0";
            }

            UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(false, 0);

            UIMainView.Instance.PlayerPlayingPanel.SetDealer(pppd.GetOtherPlayerPos(msg.iUserId), pppd.GetOtherPlayerShowPos(1));


            ///发送玩法配置请求
            NetMsg.ClientPlayingMethodConfReq msgreq = new NetMsg.ClientPlayingMethodConfReq();
            msgreq.iUserId = MahjongCommonMethod.Instance.iUserid;
            NetworkMgr.Instance.GameServer.SendPlayingMethodConfReq(msgreq);

            //保存第一次创建房间的时间
            if (PlayerPrefs.GetFloat("SitRes") == 0)
            {
                //如果是房主开房
                if (msg.cSeatNum == 1 && PlayerPrefs.GetInt(NewPlayerGuide.Guide.ShareTowX.ToString()) < 3)
                {
                    NewPlayerGuide.Instance.OpenIndexGuide(NewPlayerGuide.Guide.ShareTowX);
                }

                float timer = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
                PlayerPrefs.SetFloat("SitRes", timer);
            }

            if (msg.cUserNum > 1)
            {
                NetMsg.ClientEscapeReq escape = new NetMsg.ClientEscapeReq();
                escape.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                escape.byEscape = 0;
                NetworkMgr.Instance.GameServer.SendEscapeReq(escape);
            }

            //判断玩家是否有麻将馆红包
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            mcm.GetNowTimer(mcm.iParlorId, UIMainView.Instance.ParlorRedBagMessage.GetParlorRedBagStatus);
        }

        /// <summary>
        /// 离开回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleEscapeRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientEscapeRes msg = new NetMsg.ClientEscapeRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("离开回应消息长度不一致");
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("离开回应消息错误编号：" + msg.iError);
                return;
            }

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                RuningToBackGround.Instance.isFocus = false;
                RuningToBackGround.Instance.isPause = false;
                //Debug.LogError("玩家自己不处理");
                return;
            }

            int seatNum = pppd.GetOtherPlayerPos(msg.iUserId);

            //更新玩家离线显示信息
            if (msg.byEscape == 1)   //玩家离开
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(seatNum) - 1, true);
            }
            else if (msg.byEscape == 0)  //玩家回来
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(seatNum) - 1, false);
            }
        }

        /// <summary>
        /// 准备回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleReadyRes(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientReadyRes msg = new NetMsg.ClientReadyRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("准备回应消息长度不一致");
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("准备回应消息错误编号:" + msg.iError);
                return;
            }

            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(false, 0, false);
                if (pppd.isAllAutoStatus && GameData.Instance.GameResultPanelData.iHandClick != 1)
                {
                    pppd.isAllAutoStatus = false;
                    GameData.Instance.GameResultPanelData.iHandClick = 2;
                    //关闭结算界面，发送准备请求
                    GameData.Instance.GameResultPanelData.isPanelShow = false;
                    GameData.Instance.GameResultPanelData.isShowRoundGameResult = false;
                    GameData.Instance.GameResultPanelData.isShowRoomGameResult = false;
                    SystemMgr.Instance.GameResultSystem.UpdateShow();
                    //初始化界面信息
                    UIMainView.Instance.PlayerPlayingPanel.InitPanel();
                    //发送准备请求
                    NetMsg.ClientReadyReq ready = new NetMsg.ClientReadyReq();
                    ready.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    Network.NetworkMgr.Instance.GameServer.SendReadyReq(ready);
                }
            }


            MahjongLobby_AH.Data.CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;
            if (msg.iUserId == pppd.iUserId)
            {
                RuningToBackGround.Instance.isFocus = false;
                RuningToBackGround.Instance.isPause = false;
                //关闭结算面板
                GameData.Instance.GameResultPanelData.isPanelShow = false;
                SystemMgr.Instance.GameResultSystem.UpdateShow();
            }
            else
            {
                SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            }

            if (pppd.usersInfo.ContainsKey(pppd.GetOtherPlayerPos(msg.iUserId)))
            {
                pppd.usersInfo[pppd.GetOtherPlayerPos(msg.iUserId)].cReady = 1;
            }

            SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow(pppd.GetOtherPlayerPos(msg.iUserId));
        }
        /// <summary>
        /// 发牌通知消息1100
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDealTileNotice(byte[] pMsg, int len)
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Card_Dealing, false, false);
            int ioffset = 0;
            NetMsg.ClientDealTileNotice msg = new NetMsg.ClientDealTileNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("发牌通知消息长度不一致");
            }

            //关闭接收消息机制
            Lock();

            UIMainView.Instance.PlayerPlayingPanel.OnCLickSeatSuccess((MahjongCommonMethod.Instance.mySeatid - 1), false, false, false);
            GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame = 0;

            //处理每局的对局开始动画
            UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(4, 11);
            //关闭玩家的准备手势
            UIMainView.Instance.PlayerPlayingPanel.CloseReadyImage();
            PlayerPlayingPanelData pd = GameData.Instance.PlayerPlayingPanelData;
            pd.isAllAutoStatus = false;
            //如果不是本局不是庄家。则第一张手牌的标志位置为false
            if (msg.byDealerSeat != pd.bySeatNum)
            {
                pd.isFirstDealCard = false;
            }

            pd.szGameNum = msg.szGameNum;

            if (pd.FinshedQuanNum <= 1)
            {
                //占座中消息
                UIMainView.Instance.PlayerPlayingPanel.OnCloseZhanZuo();
            }

            //判断如果庄家座位号如果上次和这次之和是5就说明完成一圈            
            if (pd.byDealerSeat - msg.byDealerSeat == 3)
            {
                pd.FinshedQuanNum++;
            }

            pd.byDealerSeat = msg.byDealerSeat;

            //关闭玩家等待界面
            pd.isPanelShow_Wating = false;
            pd.isPanelShow_Playing = true;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            //移动头像位置
            UIMainView.Instance.PlayerPlayingPanel.MoveHeadImage();
            //处理庄家显示
            Debug.LogWarning("庄家显示 发牌通知消息1100 :" + pd.GetOtherPlayerShowPos(msg.byDealerSeat));
            UIMainView.Instance.PlayerPlayingPanel.SetDealer(pd.bySeatNum, pd.GetOtherPlayerShowPos(msg.byDealerSeat), true);
            byte[] mahjongValue = new byte[13];

            //处理玩家自己的牌墙
            mahjongValue = msg.caCard;
            UIMainView.Instance.PlayerPlayingPanel.PlayerDeal_Ie(mahjongValue);

            //如果玩家的玩法配置为0
            if (pd.playingMethodConf.byBillingMode == 0)
            {
                pd.isFirstZero = true;
            }
            else
            {
                //处理桌面的剩余牌的数量            
                //if (pd.playingMethodConf.byReserveMode == 1 || pd.playingMethodConf.byReserveMode == 3 || pd.playingMethodConf.byReserveMode == 4)
                //{
                //    pd.LeftCardCount -= 12;  //减去固定的留12张牌
                //}
                //else if (pd.playingMethodConf.byReserveMode == 2)
                //{
                //    pd.LeftCardCount -= 14;  //减去固定的留14张牌
                //}
                //else if(pd.playingMethodConf.byReserveMode==5)
                //{
                //    pd.LeftCardCount -= 6;
                //}
            }

            pd.FinshedGameNum++;
            pd.FinshedGameNum = pd.FinshedGameNum > pd.AllGameNum ? pd.AllGameNum : pd.FinshedGameNum;

            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            pd.isBeginGame = true;

            if (PlayerPrefs.GetInt(NewPlayerGuide.Guide.ShareTowX.ToString()) < 3)
            {
                int status = PlayerPrefs.GetInt(NewPlayerGuide.Guide.ShareTowX.ToString()) + 1;
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.ShareTowX, status);
            }

            //打开防作弊面板
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (acpd.iReadyShowAnti == 0)
            {
                //再打开防作弊面板之前，关闭消息接口
                Lock();
                acpd.iReadyShowAnti++;
            }
            else
            {
                return;
            }
            acpd.isPanelShow = true;
            SystemMgr.Instance.AntiCheatingSystem.UpdateShow();

#if UNITY_ANDROID||UNITY_IOS
            for (int i = 0; i < 4; i++)
            {
                if (i == pppd.bySeatNum - 1)
                {
                    //ip赋值
                    acpd.Ip[i] = MahjongCommonMethod.PlayerIp;
                    acpd.PlayerPos[i, 0] = MahjongCommonMethod.Instance.fLongitude;
                    acpd.PlayerPos[i, 1] = MahjongCommonMethod.Instance.fLatitude;
                }
                else
                {
                    if (pppd.usersInfo.ContainsKey(i + 1))
                    {
                        //ip赋值
                        acpd.Ip[i] = pppd.usersInfo[i + 1].szIp;
                        //经纬度赋值
                        acpd.PlayerPos[i, 0] = pppd.usersInfo[i + 1].fLongitude;
                        acpd.PlayerPos[i, 1] = pppd.usersInfo[i + 1].fLatitude;
                    }
                }

                ///Debug.LogError("玩家的经纬度信息:" + acpd.PlayerPos[i, 0] + "," + acpd.PlayerPos[i, 1]);
            }

#elif UNITY_EDITOR
            for (int i = 0; i < 4; i++)
            {
                if (i == pppd.bySeatNum - 1)
                {
                    //ip赋值
                    acpd.Ip[i] = MahjongCommonMethod.sIp;
                }
                else
                {
                        //ip赋值
                    acpd.Ip[i] = pppd.usersInfo[i + 1].szIp;
                }
                //经纬度赋值
                acpd.PlayerPos[i, 0] = 0;
                acpd.PlayerPos[i, 1] = 0;
            }
#endif
        }


        /// <summary>
        /// 解散房间回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDismissRoomRes(byte[] pMsg, int len)
        {

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int seatNum = 0;
            int ioffset = 0;
            NetMsg.ClientDismissRoomRes msg = new NetMsg.ClientDismissRoomRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("解散房间回应长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("解散房间回应错误编号：" + msg.iError);
                return;
            }
            //Debug.LogError("解散房间1：" + pppd.iDissolveFlag + "," + msg.cType);
            //如果有玩家处于托管状态，且有人同意，不发送准备请求
            if (pppd.iDissolveFlag != 0)
            {
                if (msg.cType == 2)
                {
                    pppd.iDissolveStatus = 1;
                }
                //有人拒绝直接
                else if (msg.cType == 3)
                {
                    pppd.DismissRoomRes = msg;
                }
                return;
            }

            pppd.iDissolveFlag = msg.byFlag;
            //如果是托管发起的解散，特殊处理

            if (msg.byFlag == 1)
            {
                pppd.DismissRoomRes = msg;
                return;
            }

            seatNum = pppd.GetOtherPlayerPos(msg.iUserId);


            //显示玩家发起解散房间
            if (msg.cType == 1)
            {
                if (pppd.isBeginGame || pppd.iMethodId == 11)
                {
                    //显示界面
                    UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(true);
                    UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localPosition = Vector3.zero;
                    UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localScale = Vector3.one;
                    if (pppd.isBeginGame || pppd.isWatingPlayerDownOrUp)
                    {
                        //更新界面
                        UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().iseatNum = seatNum;
                        UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().ShowAllPlayerMessage();
                    }
                }
                return;
            }
            if (msg.cType == 2)
            {
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().UpdateShow(seatNum, 1);
                if (seatNum == pppd.bySeatNum)
                {
                    UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().HideBtn();
                }
                return;
            }
            if (msg.cType == 3)
            {
                //关闭界面
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(false);
                StringBuilder str = new StringBuilder();
                str.Append("由于玩家【");
                str.Append(pppd.usersInfo[seatNum].szNickname);
                str.Append("】拒绝，房间解散失败，继续游戏");
                UIMgr.GetInstance().GetUIMessageView().Show("温馨提示", str.ToString());
            }
        }

        /// <summary>
        /// 解散房间通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDismissRoomNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            NetMsg.ClientDismissRoomNotice msg = new NetMsg.ClientDismissRoomNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (!GameData.Instance.PlayerPlayingPanelData.isBeginGame)
            {
                if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId && pppd.usersInfo.Count <= 1)
                {
                    ReturnLobby();
                }
                else
                {
                    MahjongCommonMethod.Instance.GameLaterDo_ = ShowTuiCHuTishi;
                    ReturnLobby();
                    //显示解散房间通知界面
                    //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.DISSOLVE_GAME, ReturnLobby);
                }
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(false);

                if (GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame < 2)
                {
                    //关闭一局结算面板，同时打开一局结算界面
                    GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
                    grpd.isPanelShow = true;
                    grpd.isShowRoundGameResult = false;
                    //显示总的结算数据
                    grpd.isShowRoomGameResult = true;
                    UIMainView.Instance.GameResultPanel.SpwanAllGameResult();
                    SystemMgr.Instance.GameResultSystem.UpdateShow();
                }
            }
            GameData.Instance.PlayerPlayingPanelData.isAllAutoStatus = false;
        }

        void ShowTuiCHuTishi()
        {
            MahjongCommonMethod.Instance.ShowRemindFrame(MahjongGame_AH.TextConstant.DISSOLVE_GAME);
        }

        void LoginAccount()
        {
            MahjongCommonMethod.isLoginOut = true;
            MahjongCommonMethod.isAuthenSuccessInLobby = false;
            //断开游戏/大厅服务器连接
            if (MahjongGame_AH.Network.NetworkMgr.Instance)
            {
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Disconnect();
                //清空gc垃圾
                System.GC.Collect();
                SceneManager_anhui.Instance.OpenPointScene(ESCENE.MAHJONG_LOBBY_GENERAL);
                //MahjongGame_AH.Scene.SceneManager.Instance.LoadScene(MahjongGame_AH.Scene.ESCENE.MAHJONG_LOBBY_GENERAL);
            }
        }

        //返回大厅
        void ReturnLobby()
        {
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                //隐藏加载界面
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                MahjongCommonMethod.Instance.ShowRemindFrame("亲，您的网络不给力哦，请检查网络后重试");
            }
            else
            {
                Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN);
                return;
            }
            test();
        }

        void test()
        {
            UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_SERVER_DISCONNECT, ReturnLobby);
        }

        /// <summary>
        /// 出牌通知消息1101 + 发牌
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDiscardTileNotice(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientDiscardTileNotice msg = new NetMsg.ClientDiscardTileNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("出牌通知消息长度错误,ioffset:" + ioffset + ",len:" + len);
            }
            pppd.LeftCardCount = msg.byResidueTileNum;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            //如果手牌碰杠之后，不会立即产生新摸得牌
            if (pppd.isGangToPeng_Later || pppd.isGangToPeng)
            {
                pppd.DiscardTileNotice = msg;
                return;
            }
            UIMainView.Instance.PlayerPlayingPanel.ShowWitchPlayerPlay(msg.bySeatNum - 1);
            //处理重新开始倒计时时间
            GameData.Instance.PlayerPlayingPanelData.DownTime = 0f;

            //处理自己摸得牌
            if (msg.bySeatNum == pppd.bySeatNum)
            {
                //把自己的手牌置为空
                MahjongManger.Instance.PlayerDealHandCrad = null;
                MahjongManger.Instance.iniAnimationStatus = 0;
                //表示自己可出牌
                Debug.Log("出牌通知消息,座位号：" + msg.bySeatNum + ",摸得牌：" + msg.byDrawTile + ",标志位：" + pppd.isFirstDealCard
                    + "===:" + Time.realtimeSinceStartup);

                pppd.bLastCard = msg.byDrawTile;

                //关闭过胡信息
                UIMainView.Instance.PlayerPlayingPanel.ShowSkipWin(false);

                if (msg.byDrawTile != 0)//发牌
                {
                    Lock();
                    //  Debug.LogError("isLastPongOrKong:" + pppd.isLastPongOrKong + "FirstDealCard" + pppd.FirstDealCard);
                    if (!pppd.isFirstDealCard)
                    {
                        pppd.isLastPongOrKong = false;
                    }
                    else
                    {
                        pppd.FirstDealCard = msg.byDrawTile;
                    }

                    UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(msg.byDrawTile, 0.1f);
                }
                else
                {
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(0, 0.1f);
                    #region 吃碰
                    if (!pppd.isRecordScore_Break)
                    {
                        //玩家出牌之后，把最后一张牌分出间隙
                        UIMainView.Instance.PlayerPlayingPanel.MoveLastCard();
                    }

                    pppd.isCanHandCard = true;
                    if (!pppd.isGangToPeng)
                    {
                        //处理玩家摸到手牌之后，判断玩家的杠
                        if (MahjongHelper.Instance.JudgeGang(pppd.GetPlayerAllHandCard(2), pppd.usersCardsInfo[0].listSpecialCards) > 0)
                        {
                            pppd.isSendPlayerPass = false;
                            MahjongHelper.Instance.specialValue_[2] = 1;
                            MahjongHelper.Instance.specialValue_[7] = 1;
                            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                        }
                    }
                    else
                    {
                        pppd.isGangToPeng = false;
                        pppd.isLastPongOrKong = false;
                    }
                    //在碰之后也要检测是否可以听牌                                        
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongHelper.Instance.mahjongTing = MahjongHelper.Instance.GetEnableTingCard(2);
                    if (MahjongHelper.Instance.mahjongTing.Count > 0)
                    {
                        //显示所有可以听牌的花色值
                        UIMainView.Instance.PlayerPlayingPanel.UpdateTingCard(MahjongHelper.Instance.Ting.ToArray());
                    }
                    #endregion
                }
                return;
            }
            else ///以上处理自己摸得牌
            {
                if (msg.byDrawTile != 0)
                {
                    int index = pppd.GetOtherPlayerShowPos(msg.bySeatNum);
                    //产生别的玩家的手牌
                    UIMainView.Instance.PlayerPlayingPanel.SpwanOtherPlayerCrad(1, index);
                }
            }

            //关闭玩家的吃碰杠胡的界面显示 
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
        }
        /// <summary>
        /// 出牌回应0x1103
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDiscardTileRes(byte[] pMsg, int len)
        {
            GameData.Instance.PlayerPlayingPanelData.DownTime = 0;
            GameData gd = GameData.Instance;
            PlayerPlayingPanelData pppd = gd.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientDiscardTileRes msg = new NetMsg.ClientDiscardTileRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("出牌回应长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("出牌回应错误编号:" + msg.iError);
                UIMainView.Instance.disConnect.RecoverGameScene(2);
                return;
            }

            if (PlayerPrefs.HasKey("TingOneCard"))
            {
                PlayerPrefs.SetInt("TingOneCard", 0);
            }

            pppd.bLastCard = msg.cTitle;
            int seatNum = pppd.GetOtherPlayerPos(msg.iUserId);

            //停止接收消息
            //NetworkMgr.Instance.GameServer.Lock();          

            //处理玩家出牌之后的回应消息
            if (pppd.GetOtherPlayerPos(msg.iUserId) == pppd.bySeatNum)
            {
                if (pppd.isChoiceTing == 1)
                {
                    pppd.isChoiceTing = 2;
                }

                if (MahjongManger.Instance.isEndPutAnimation)
                {
                    //Debug.LogError("开始接受消息====================0");
                    //NetworkMgr.Instance.GameServer.Unlock();
                }

                byte mahjongValue = msg.cTitle;
                if (msg.cTitle == 255)
                {
                    mahjongValue = MahjongManger.Instance.TingFirstValue;
                }
                // Debug.LogError("听牌数量： "+MahjongHelper.Instance.mahjongTing.Count);
                //移除玩家其他听牌的信息
                if (MahjongHelper.Instance.mahjongTing.ContainsKey(mahjongValue))//msg.cTitle为255的时候是晋城高平玩法报听了
                {
                    //Debug.LogError("====================出牌回应1");
                    MahjongHelper.Instance.TingCount = new List<MahjongHelper.TingMessage>();
                    for (int i = 0; i < MahjongHelper.Instance.mahjongTing[mahjongValue].Length; i++)
                    {
                        MahjongHelper.Instance.TingCount.Add(MahjongHelper.Instance.mahjongTing[mahjongValue][i]);
                    }
                    //判断玩家是否继续显示停牌按钮
                    // Debug.LogError("-------------"+SystemMgr.Instance.PlayerPlayingSystem.isShowTingBtn(mahjongValue));
                    UIMainView.Instance.PlayerPlayingPanel.ShowBtnTing(SystemMgr.Instance.PlayerPlayingSystem.isShowTingBtn(mahjongValue));
                }
                else
                {
                    // if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinLimitReadHand == 1)
                    // {
                    //  if (GameData.Instance.PlayerPlayingPanelData.isChoiceTing == 0)
                    //   {
                    UIMainView.Instance.PlayerPlayingPanel.ShowBtnTing(false);
                    // }
                    //}
                }

                //Debug.LogError("====================出牌回应3");
                MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                // Debug.LogWarning("播放自己声：" + MahjongLobby.SDKManager.Instance.isex);
                SystemMgr.Instance.AudioSystem.PlayAuto(MahjongCommonMethod.Instance.GetAudioSourceIndex(msg.cTitle), MahjongLobby_AH.SDKManager.Instance.isex);

                //如果是玩家成功选择了听牌，则直接把所有的手牌改为不可打出状态
                UIMainView.Instance.PlayerPlayingPanel.ChangeHandCardStatus(1);


                //在这里找到要自动打出的手牌，打出
                if (msg.cTitle != 0 && pppd.iPlayerHostStatus > 0)
                {
                    //关闭所有的听牌提示
                    MahjongManger.Instance.HideTingLogo();
                    Mahjong mah = MahjongManger.Instance.GetDealCard();
                    if (mah != null && mah.bMahjongValue == msg.cTitle)
                    {
                        //Debug.LogError("====================出牌回应6");
                        mah.PutCard(1);
                    }
                    else
                    {
                        //Debug.LogError("====================出牌回应7");
                        MahjongManger.Instance.GetPointCard(msg.cTitle).PutCard(1);
                    }
                }
            }
            else
            {
                Lock();
                //关闭离线显示
                int temp_index = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId)) - 1;
                if (temp_index >= 0 && pppd.iAllPlayerHostStatus[temp_index] == 0)
                {
                    UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(temp_index, false);
                }

                //删除别人的手牌
                UIMainView.Instance.PlayerPlayingPanel.DelHandCrad(seatNum);
                //产生其他玩家出的牌
                UIMainView.Instance.PlayerPlayingPanel.ShowBigCardInTable(seatNum, msg.cTitle);
                SystemMgr.Instance.AudioSystem.PlayAuto(MahjongCommonMethod.Instance.GetAudioSourceIndex(msg.cTitle), pppd.usersInfo[seatNum].bySex);
            }


        }

        /// <summary>
        /// 吃碰杠胡抢通知消息1104
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSpecialTileNotice(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            // PlayerWatingPanelData pwpd = GameData.Instance.PlayerWatingPanelData;
            int ioffset = 0;
            NetMsg.ClientSpecialTileNotice msg = new NetMsg.ClientSpecialTileNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("吃碰杠胡抢通知消息,ioffset：" + ioffset + ",len:" + len);
            }

            if (pppd.iPlayerHostStatus > 0 && msg.cSeatNum == pppd.bySeatNum)
            {
                return;
            }

            // int seatNum = pwpd.GetOtherPlayerPos(msg.cTableNum );
            List<int> status = new List<int>();
            //通知玩家吃碰杠消息，给出提示
            if (msg.cSeatNum == pppd.bySeatNum)
            {
                //吃
                if (msg.byChow == 1)
                {
                    status.Add(1);
                }
                else
                {
                    status.Add(0);
                }

                //碰
                if (msg.byPong == 1)
                {
                    pppd.isGangAndPengEnable = true;
                    status.Add(1);
                }
                else
                {
                    pppd.isGangAndPengEnable = false;
                    status.Add(0);
                }

                //杠
                if (msg.byKong == 1)
                {
                    pppd.bkongValue_.Clear();
                    pppd.isGangAndPengEnable = true;
                    status.Add(1);
                }
                else
                {
                    pppd.isGangAndPengEnable = false;
                    status.Add(0);
                }

                //胡
                if (msg.byWin == 1)
                {
                    status.Add(1);
                }
                else
                {
                    status.Add(0);
                }

                //吃
                if (msg.byThirteenOrphansChow == 1)
                {
                    status.Add(1);
                }
                else
                {
                    status.Add(0);
                }

                //抢
                if (msg.byThirteenOrphansRob == 1)
                {
                    status.Add(1);
                }
                else
                {
                    status.Add(0);
                }

                //听
                status.Add(0);

                //过
                status.Add(1);

                pppd.isNeedSendPassMessage = true;
            }
            else
            {

            }
            // UIMainView.Instance.PlayerPlayingPanel.ShowWitchPlayerPlay(seatNum % 4, seatNum - 1 % 4);        
            //更新界面            
            SystemMgr.Instance.PlayerPlayingSystem.SpecialTileNoticeShow(status.ToArray());


        }
        /// <summary>
        /// 吃碰杠胡抢回应消息1106
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSpecialTileRes(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientSpecialTileRes msg = new NetMsg.ClientSpecialTileRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            //Debug.LogError("iError:" + msg.iError);
            //Debug.LogError("iUserId:" + msg.iUserId);
            //Debug.LogError("请求的牌:" + msg.byaTiles[0] + "," + msg.byaTiles[1] + "," + msg.byaTiles[2]);
            //Debug.LogError("0弃1吃2碰3杠4胡5吃:" + msg.bySpecial);
            //Debug.LogError("是否是碰杠:" + msg.byPongKong);
            //Debug.LogError("出牌的座位号:" + msg.byTileSeat);

            if (ioffset != len)
            {
                Debug.LogError("吃碰杠胡抢回应消息长度不一致，ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("操作错误" + msg.iError + "请重试！", () => { UIMainView.Instance.disConnect.RecoverGameScene(2); });
                Debug.LogError("吃碰杠胡抢回应消息的错误编号:" + msg.iError);
                return;
            }


            //如果玩家处于自动托管状态，且是自己直接关闭界面
            if (pppd.iPlayerHostStatus > 0 && (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId))
            {
                MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                SystemMgr.Instance.PlayerPlayingSystem.SpecialTileNoticeShow(MahjongHelper.Instance.specialValue_);
                return;
            }

            pppd.isCanHandCard = false;
            int seatNum = pppd.GetOtherPlayerPos(msg.iUserId);
            //处理自己的操作
            if (seatNum == pppd.bySeatNum)
            {
                //如果是进行了碰之后，不可以进行杠
                if (msg.bySpecial != 3)
                {
                    pppd.isLastPongOrKong = true;
                }

                if (msg.bySpecial == 2)
                {
                    pppd.isGangToPeng = true;
                }
            }

            //        Debug.LogError("玩家吃碰杠胡回应消息，玩家座位号:" + seatNum + "类型：" + msg.bySpecial + "是否是碰杠：" + msg.byPongKong
            //+ "回应牌的花色:" + msg.byaTiles[0] + ",出牌座位号:" + msg.byTileSeat);


            #region 吃碰杠胡抢语音
            AudioSystem.AudioType type = AudioSystem.AudioType.AUDIO_NONE;
            if (msg.bySpecial == 1 || msg.bySpecial == 5)
            {
                type = AudioSystem.AudioType.Chi;
            }

            if (msg.bySpecial == 2)
            {
                type = AudioSystem.AudioType.Peng;
            }

            if (msg.bySpecial == 3)
            {
                type = AudioSystem.AudioType.Gang;
                if (msg.byPongKong == 1 && msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
                {
                    MahjongManger.Instance.bPongGang = true;
                }
            }

            if (msg.bySpecial == 6)
            {
                type = AudioSystem.AudioType.Qiang;
            }

            #endregion 吃碰杠胡抢语音

            //播放声音之后，产生特殊牌的提示
            int index_ = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId)) - 1;
            string content = "";

            if (msg.bySpecial != 4)
            {
                if (msg.byPongKong == 1)
                {
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(index_, 18);
                }
                else
                {
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(index_, msg.bySpecial);
                }
            }

            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                SystemMgr.Instance.PlayerPlayingSystem.SpecialTileNoticeShow(MahjongHelper.Instance.specialValue_);
                SystemMgr.Instance.AudioSystem.PlayAuto(type, MahjongLobby_AH.SDKManager.Instance.isex);
            }
            else
            {
                if (pppd.usersInfo.ContainsKey(seatNum))
                {
                    SystemMgr.Instance.AudioSystem.PlayAuto(type, pppd.usersInfo[seatNum].bySex);
                }
            }
            int pangStatus = 0;
            if (msg.byaTiles[0] != 0)
            {
                if (msg.bySpecial == 3)
                {
                    if (msg.iUserId == pppd.iUserId)
                    {
                        pppd.bkongValue = msg.byaTiles[0];
                    }
                    if (msg.byTileSeat != 0)
                    {
                        pangStatus = 1;
                    }
                    else
                    {
                        pangStatus = 2;
                    }
                    if (msg.byPongKong == 1)
                    {
                        pangStatus = 3;
                    }
                }


                //    Debug.LogError("pangStatus:" + pangStatus);
                UIMainView.Instance.PlayerPlayingPanel.SpwanSpecialCard(msg.byaTiles, seatNum, msg.bySpecial, false, msg.byTileSeat, pangStatus);
                // Debug.LogError(pppd.playingMethodConf.byReserveMode);
                //减去玩家的桌面上的牌的数量
                if (msg.bySpecial == 3)
                {
                    pppd.iGangNum++;

                    //if (pangStatus == 2 && pppd.playingMethodConf.byReserveMode == 3)  //暗杠多减两张
                    //{
                    //    pppd.LeftCardCount -= 2;
                    //}


                    //if (pppd.playingMethodConf.byReserveMode == 4) //4留牌数量:(0杠12,1杠14,2杠16,3杠18,4杠12,5杠14...)
                    //{
                    //    if (pppd.iGangNum % 4 == 0)
                    //    {
                    //        pppd.LeftCardCount += 6;
                    //    }
                    //    else
                    //    {
                    //        pppd.LeftCardCount -= 2;
                    //    }
                    //    Debug.Log("剩余排数 2222：" + pppd.LeftCardCount + "," + pppd.iGangNum);
                    //}
                    //else if (pppd.playingMethodConf.byReserveMode != 0&& pppd.playingMethodConf.byReserveMode != 5)
                    //{
                    //    pppd.LeftCardCount -= 2;
                    //}

                    SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
                }

                Debug.LogWarning("++++吃碰杠胡强回应：" + msg.byTileSeat);

                //删除自己桌面上对应的牌
                if (msg.byTileSeat != 0 && msg.byPongKong == 0)
                {
                    UIMainView.Instance.PlayerPlayingPanel.DeleteTableCard(pppd.GetOtherPlayerShowPos(msg.byTileSeat) - 1);
                }
            }
        }
        /// <summary>
        /// 即时计分通知0x1107
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleRealtimePointNotice(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            int ioffset = 0;
            NetMsg.ClientRealtimePointNotice msg = new NetMsg.ClientRealtimePointNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("即时计分通知消息长度不一致,ioffset" + ioffset + ",len:" + len);
                return;
            }

            pppd.byType = msg.byType;

            //产生前台后和的特效
            if (msg.byType == 1)
            {
                UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(4, 13);
            }
            if (msg.byType == 4)
            {
                UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(4, 14);
            }

            //即时更新玩家总分数
            for (int i = 0; i < msg.caPoints.Length; i++)
            {
                pppd.sScoreShow[i] += (sbyte)msg.caPoints[i];
                //将玩家座位号转换成数组下标
                int index = pppd.GetOtherPlayerShowPos(i + 1) - 1;
                if ((sbyte)msg.caPoints[i] != 0)
                {
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclScoreRemind(index, (sbyte)msg.caPoints[i]);
                }
            }
        }
        /// <summary>
        /// 长治花三门的十三幺回应消息0x1108
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleThirteenOrphansRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientThirteenOrphansRes msg = new NetMsg.ClientThirteenOrphansRes();
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("长治花三门十三幺回应长度不一致ioffset" + ioffset + "len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("十三幺错误代码：" + msg.iError + "  " + MahjongLobby_AH.Err_ClientThirteenOrphansRes.Err(msg.iError));
                return;
            }

            if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
            {
                GameData.Instance.PlayerPlayingPanelData.isChoicTir = msg.byThirteenOrphans;
                UIMainView.Instance.PlayerPlayingPanel.DealThirteenState(msg.byThirteenOrphans);
            }

            //GameData.Instance.PlayerPlayingPanelData .iTirState=
        }

        /// <summary>
        /// 处理语音回应消息 0x1021
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientVoiceRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientVoiceRes msg = new NetMsg.ClientVoiceRes(81);
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("收到语音消息回应长度不一致  ");
            }
            if (msg.iError != 0)
            {
                Debug.LogError("收到语音消息回应" + msg.iError);
            }
            MainViewShortTalkPanel stp = UIMainView.Instance.ShortTalkPanel;
            PlayerNodeDef pnd = GameData.Instance.PlayerNodeDef;

            if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
            {
                stp.OnReceiveVoice(msg.szUrl, msg.iDuration / 1000, false, true, pnd.szHeadimgurl, msg.id);
            }
            else
            {
                PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
                if (pppd.usersInfo.ContainsKey(pppd.GetOtherPlayerPos(msg.iUserId)))
                {
                    stp.OnReceiveVoice(msg.szUrl, msg.iDuration / 1000, false, false, pppd.usersInfo[pppd.GetOtherPlayerPos(msg.iUserId)].szHeadimgurl, msg.id);
                }
            }
            // Debug.LogError("说话人" + msg.iUserId);
            Debug.Log(MahjongCommonMethod.Instance.isOpenVoicePlay);
            if (MahjongCommonMethod.Instance.isOpenVoicePlay)
            {
                //if (msg.iUserId != MahjongCommonMethod.Instance.iUserid)
                ///{
                // Debug.LogError("声音添加地址;"+msg.szUrl );
                VoiceManegerMethord.Instance.voices.Add(new VoiceManegerMethord.UsersVoiceInfo(msg.iUserId, msg.szUrl, null, msg.id, msg.iDuration / 1000));
                //}
            }
        }
        const string PlayLGameCount = "PlayLGameCount";
        /// <summary>
        /// 处理一局结束的通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGameReultNotice(byte[] pMsg, int len)
        {
            AudioSystem.AudioType type = AudioSystem.AudioType.AUDIO_NONE;
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Card_Down_Win, false, false);
            int sex = 1;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            bool isHuangZhuang = true; //是否荒庄的标志
            bool isDraw = false; //是否自摸的标志位
            List<int> winSeat = new List<int>();  //赢家的座位号
            int ioffset = 0;
            NetMsg.ClientGameResultNotice msg = new NetMsg.ClientGameResultNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("收到处理一局结束的通知消息不一致,ioffset:" + ioffset + ",len:" + len);
            }
            if (MahjongCommonMethod.Instance.isCreateRoom)
            {
                MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CreatRoomSucc);
            }
            else
            {
                MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.JoinRoomSucc);
            }
            GameData.Instance.PlayerPlayingPanelData.lcDealerMultiple = 0;
            //关闭吃碰杠胡界面
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            SystemMgr.Instance.PlayerPlayingSystem.SpecialTileNoticeShow(MahjongHelper.Instance.specialValue_);

            //关闭过胡信息
            UIMainView.Instance.PlayerPlayingPanel.ShowSkipWin(false);
            pppd.iBeginRoundGame++;

            //处理重新开始倒计时时间
            GameData.Instance.PlayerPlayingPanelData.DownTime = 0f;

            //初始化数据
            grpd.InitData();

            //保存四个玩家的值
            grpd.bHandleTiles = msg.bya2HandTiles;

            if (pppd.iMethodId == 14)
            {
                for (int i = 0; i < 4; i++)
                {
                    grpd.bResultPoint[i] = msg.caResultPoint[i] * 100;
                }
            }
            else
            {
                grpd.bResultPoint = msg.caResultPoint;
            }

            grpd.resultType = msg.aResultType;
            grpd.byShootSeat = msg.byShootSeat;
            grpd.byShootSeatReadHand = msg.byShootSeatReadHand;
            //grpd.byIfNormalOrDragon = msg.byIfNormalOrDragon;
            grpd.byaWinSrat = msg.byaWinSeat;
            for (int i = 0; i < NetMsg.MAX_USER_PER_TABLE; i++)
            {
                for (int j = 0; j < NetMsg.F_TOTAL_NUM; j++)
                {
                    grpd.caFanResult[i, j] = msg.caFanResult[i, j];
                }
            }
            grpd.caFanResult = msg.caFanResult;

            for (int i = 0; i < 4; i++)
            {
                if (pppd.iMethodId == 14)
                {
                    pppd.sScoreShow[i] += msg.caResultPoint[i] * 100;
                }
                else
                {
                    pppd.sScoreShow[i] += msg.caResultPoint[i];
                }
            }

            //保存玩家是否是解散解散的数据
            grpd.HandDissolve = msg.byDismiss;

            for (int i = 0; i < 4; i++)
            {
                if (msg.byaWinSeat[i] > 0)
                {
                    isHuangZhuang = false;
                }
            }

            UIMainView.Instance.PlayerPlayingPanel.CloseWaringMusic();

            if (msg.byDismiss == 1)
            {
                //更新面板的显示数据
                grpd.isPanelShow = true;
                grpd.isShowRoundGameResult = true;
                SystemMgr.Instance.GameResultSystem.UpdateShow();
                UIMainView.Instance.GameResultPanel.SpwanGameReult_Round();
            }
            else
            {
                //荒庄
                if (isHuangZhuang && msg.byDismiss == 0)
                {
                    //前台后和
                    //if (pppd.playingMethodConf.byRaiseMode < 2)
                    //{
                    //    UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(4, 9, true);
                    //}

                    //延迟3s显示更新面板的显示数据
                    UIMainView.Instance.PlayerPlayingPanel.DelayShowResule_(4f);
                }

                for (int i = 0; i < 4; i++)
                {
                    if (msg.byaWinSeat[i] > 0)
                    {
                        winSeat.Add(i + 1);
                    }
                }


                //自摸
                if (winSeat.Count > 0 && msg.byShootSeat <= 0)
                {
                    for (int i = 0; i < winSeat.Count; i++)
                    {
                        //显示玩家输赢信息
                        UIMainView.Instance.PlayerPlayingPanel.ShowPlayerCardMessage(winSeat[i], 2);
                        UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(pppd.GetOtherPlayerShowPos(winSeat[i]) - 1, 10, false);
                        if ((winSeat[i]) == MahjongCommonMethod.Instance.mySeatid)//性别赋值
                        {
                            sex = MahjongLobby_AH.SDKManager.Instance.isex;
                        }
                        else
                        {
                            sex = GameData.Instance.PlayerPlayingPanelData.usersInfo[winSeat[i]].bySex;
                        }
                        type = AudioSystem.AudioType.Hu_Zimo;
                        isDraw = true;
                    }
                }


                //放炮
                if (msg.byShootSeat > 0 && !isDraw)
                {
                    type = AudioSystem.AudioType.Hu;
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(pppd.GetOtherPlayerShowPos(msg.byShootSeat) - 1, 15);
                    //在玩家头像上方添加放炮图片
                    UIMainView.Instance.PlayerPlayingPanel.ShowPlayerCardMessage(msg.byShootSeat, 0);

                    //获取胡牌人的位置,
                    for (int i = 0; i < 4; i++)
                    {
                        if (msg.byaWinSeat[i] > 0)
                        {
                            UIMainView.Instance.PlayerPlayingPanel.ShowPlayerCardMessage(i + 1, 1);
                            //如果是点炮胡
                            if (msg.byaWinSeat[i] == 1)
                            {
                                UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(pppd.GetOtherPlayerShowPos(i + 1) - 1, 4);
                                //  Debug.LogWarning("点炮胡");
                            }

                            //如果是抢杠胡
                            if (msg.byaWinSeat[i] == 2)
                            {
                                UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(pppd.GetOtherPlayerShowPos(i + 1) - 1, 17);
                            }
                            sex = GameData.Instance.PlayerPlayingPanelData.usersInfo[i + 1].bySex;
                        }
                    }
                }
                //显示翻码牌
                if (UIMainView.Instance.PlayerPlayingPanel.DealHongZhongZhongma(grpd.resultType[0].byFanTiles))
                {
                    //播放对局结束动画
                    // Debug.LogError("延迟五秒");
                    UIMainView.Instance.PlayerPlayingPanel.DelaySpwanSpeaiclTypeRemind(5f, 4, 12);
                }
                else
                {
                    //  Debug.LogError("延迟2秒动画");
                    //播放对局结束动画
                    UIMainView.Instance.PlayerPlayingPanel.DelaySpwanSpeaiclTypeRemind(2f, 4, 12);
                }
                for (int i = 0; i < 4; i++)
                {
                    if (msg.byaWinSeat[i] > 0)
                    {
                        UIMainView.Instance.PlayerPlayingPanel.WinAction(i);
                    }
                }
                SystemMgr.Instance.AudioSystem.PlayAuto(type, sex);
            }

        }

        /// <summary>
        /// 房间游戏结果通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleRoomResultNotice(byte[] pMsg, int len)
        {
            if (PlayerPrefs.HasKey(PlayLGameCount))
            {
                int a = PlayerPrefs.GetInt(PlayLGameCount) + 1;
                if (a == 2)
                {
                    MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeletTwo);
                }
                else if (a == 3)
                {
                    MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeletThr);
                }
                else if (a == 5)
                {
                    MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeletFiv);
                }
                else if (a == 10)
                {
                    MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeletTen);
                }
                PlayerPrefs.SetInt(PlayLGameCount, a);
            }
            else
            {
                PlayerPrefs.SetInt(PlayLGameCount, 1);
                MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeletOne);
            }
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            int ioffset = 0;
            NetMsg.ClientRoomResultNotice msg = new NetMsg.ClientRoomResultNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("房间游戏结果通知消息不一致,ioffset:" + ioffset + ",len:" + len);
            }

            GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame++;

            acpd.iReadyShowAnti = 0;
            //为游戏结果赋值
            grpd.roomResultNotice = msg;
            GameData.Instance.PlayerPlayingPanelData.isGameEnd = true;
            grpd.isEndGame = true;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //处理如果玩家在最后一局发起解散，托管状态也不能显示最终结算结果
            if (pppd.iPlayerHostStatus > 0 && GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame == 2
                && msg.iRoomEndType == 0)
            {
                GameData.Instance.PlayerPlayingPanelData.iBeginRoundGame = 0;
                //关闭玩家的单局结算界面
                grpd.isPanelShow = true;
                grpd.isShowRoundGameResult = false;
                //显示总的结算数据
                grpd.isShowRoomGameResult = true;
                UIMainView.Instance.GameResultPanel.SpwanAllGameResult();
                SystemMgr.Instance.GameResultSystem.UpdateShow();
            }

            //向网页发送手机型号信息
            //MahjongCommonMethod.Instance.SendMobileToWeb(1);
        }

        /// <summary>
        /// 聊天回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleChatRes(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientChatRes msg = new NetMsg.ClientChatRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("聊天回应消息回应消息不一致,ioffset:" + ioffset + ",len:" + len);
            }

            if (msg.iError != 0)
            {
                Debug.LogError("聊天回应消息回应消息错误编号:" + msg.iError);
            }

            // Debug.Log("聊天回应消息:" + msg.byChatType + " msg.byContentID:" + msg.byContentId);
            int seat = pppd.GetOtherPlayerPos(msg.iUserId[0]);
            int index = pppd.GetOtherPlayerShowPos(seat) - 1;
            if (msg.byChatType == 1)//播放表情
            {
                UIMainView.Instance.PlayerPlayingPanel.PlayEmotion(index, msg.byContentId);
            }
            else if (msg.byChatType == 2)//播放话术
            {

                if (msg.iUserId[0] == GameData.Instance.PlayerNodeDef.iUserId)
                {
                    SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.DajiaHao + msg.byContentId, MahjongLobby_AH.SDKManager.Instance.isex);

                }
                else
                {
                    SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.DajiaHao + msg.byContentId, pppd.usersInfo[seat].bySex);
                }
                UIMainView.Instance.PlayerPlayingPanel.PlayShortTalk(index, msg.byContentId);
            }
            else if (msg.byChatType == 3)
            {
                int fromindex = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId[1])) - 1;
                UIMainView.Instance.PlayerPlayingPanel.PlayGameTools(index, fromindex, msg.byContentId);
            }
        }
        /// <summary>
        /// 玩法配置回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandlePlayingMethodConfRes(byte[] pMsg, int len)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int ioffset = 0;
            NetMsg.ClientPlayingMethodConfRes msg = new NetMsg.ClientPlayingMethodConfRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("玩法配置回应消息不一致,ioffset:" + ioffset + ",len:" + len);
            }

            if (msg.iError != 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("参数错误！请重联", () => { UIMainView.Instance.disConnect.RecoverGameScene(2); });
                // UIMainView.Instance.disConnect.RecoverGameScene(2);
                Debug.LogError("玩法配置回应消息错误编号:" + msg.iError);
            }
            //保存玩家的玩法id
            // Debug.LogError("玩法配置回应" + msg.iPlayingMethod);
            MahjongCommonMethod.Instance.iPlayingMethod = msg.iPlayingMethod;
            pppd.CurrentCradSort(1);//排序

            #region 处理最后一张牌
            for (int i = 0; i < NetMsg.MAX_USER_PER_TABLE; i++)
            {
                int index = pppd.GetOtherPlayerShowPos(i + 1) - 1;
                int sumNum = pppd.againLoginData.resultType[i].byTripletNum * 3 + pppd.againLoginData.resultType[i].bySequenceNum * 3 + pppd.againLoginData.resultType[i].byThirteenOrphansNum + pppd.againLoginData.byaHandTileNum[i];

                if (sumNum == 14 && index == 0)
                {
                    pppd.byLastCard = pppd.againLoginData.byaHandTiles[pppd.againLoginData.byaHandTileNum[i] - 1];
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(pppd.byLastCard, 0);
                    //pppd.LeftCardCount_DisConnect += 1;
                }
            }
            if (GameData.Instance.PlayerPlayingPanelData.isChoiceTing == 2)
            {
                UIMainView.Instance.PlayerPlayingPanel.ChangeHandCardStatus(0);
            }

            //pppd.LeftCardCount = pppd.LeftCardCount_DisConnect;

            #endregion 处理最后一张牌


            if (msg.playingMethodConf.byCreateModeHaveFlower > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    UIMainView.Instance.PlayerPlayingPanel.UpdateShowFlowerCount(2, i + 1, pppd.PlayerFlowerCount[i], 1);
                }
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.UpdateShowFlowerCount(0, 0, 0);
            }

            if (!pppd.isRecordScore_Break)
            {
                if (msg.playingMethodConf.byCreateModeHaveFlower > 0)
                {
                    pppd.LeftCardCount = 144;
                }
                else
                {
                    pppd.LeftCardCount = 136;
                }
            }

            if (msg.iPlayingMethod == 14)
            {
                for (int i = 0; i < pppd.sScoreShow.Length; i++)
                {
                    pppd.sScoreShow[i] *= 100;
                }
            }

            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();

            MahjongHelper.Instance.GetSpecialTypeMultiple(msg.playingMethodConf);
            pppd.iMethodId = msg.iPlayingMethod;
            pppd.playingMethodConf = msg.playingMethodConf;
            MahjongLobby_AH.Data.CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;
            cd.MethodId = msg.iPlayingMethod;
            cd.roomMessage_ = msg.roomMessage_;
            //string str1 = "回应配置：";
           // for (int i = 0; i < msg.roomMessage_.Length; i++)
           // {
              //  Debug.LogWarning(msg.roomMessage_[i].ToString("X8"));
           // }
            
            if (msg.roomMessage_[4] > 0 && msg.iRoomType == 1)
            {
                //Debug.LogError("玩家的座位号是不是" + MahjongCommonMethod.Instance.mySeatid);
                UIMainView.Instance.PlayerPlayingPanel.OnAppointmentRoom(msg.roomMessage_[4]);
                UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[0].SetActive(true);
                //UIMainView.Instance.PlayerPlayingPanel._txUsersScor[0].text = "占座中";
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.OnSetClose();
            }


            //更新玩家的分数和局数圈数的显示情况
            if (msg.playingMethodConf.byBillingMode == 1)
            {
                //一圈四个庄家
                pppd.AllGameNum = 4;
                pppd.FinshedGameNum = 1;
            }

            if (msg.playingMethodConf.byBillingMode == 2)
            {
                pppd.AllGameNum = msg.playingMethodConf.byBillingNumber;
            }


            //if (!pppd.isRecordScore_Break)
            //{
            //    //处理玩家的手中的所有风牌
            //    if (pppd.playingMethodConf.byCreateModeHaveWind == 0)
            //    {
            //        pppd.LeftCardCount -= 16;
            //    }

            //    //处理玩家手中的箭牌
            //    if (pppd.playingMethodConf.byCreateModeHaveDragon == 0)
            //    {
            //        pppd.LeftCardCount -= 12;
            //    }
            //}

            //if (pppd.isPanelShow_Playing && !pppd.isRecordScore_Break)
            //{
            //    //处理桌面的剩余牌的数量            
            //    if (pppd.playingMethodConf.byReserveMode == 1 || pppd.playingMethodConf.byReserveMode == 3 || pppd.playingMethodConf.byReserveMode == 4)
            //    {
            //        pppd.LeftCardCount -= 12;  //减去固定的留12张牌
            //    }
            //    else if (pppd.playingMethodConf.byReserveMode == 2)
            //    {
            //        pppd.LeftCardCount -= 14;  //减去固定的留14张牌
            //    }
            //    else if (pppd.playingMethodConf.byReserveMode ==5)
            //    {
            //        pppd.LeftCardCount -= 6;
            //    }
            //}

            if (msg.playingMethodConf.byBillingMode == 3 && !pppd.isRecordScore_Break)
            {
                int num = msg.playingMethodConf.byBillingNumber;
                pppd.sScoreShow = new int[] { num, num, num, num };
                pppd.isBreakConnect = false;
                pppd.isRecordScore_Break = false;
            }
            else
            {
                pppd.isRecordScore_Break = false;
            }

            pppd.isPanelShow_Playing = true;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();

            if (pppd.isCanHandCard)
            {
                //处理玩家摸到手牌之后，判断玩家的杠
                if (MahjongHelper.Instance.JudgeGang(pppd.GetPlayerAllHandCard(2), pppd.usersCardsInfo[0].listSpecialCards) > 0)
                {
                    MahjongHelper.Instance.specialValue_[2] = 1;
                    MahjongHelper.Instance.specialValue_[7] = 1;
                    pppd.isSendPlayerPass = false;
                    UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                }
                else
                {
                    MahjongHelper.Instance.specialValue_[2] = 0;
                    MahjongHelper.Instance.specialValue_[7] = 0;
                    pppd.isSendPlayerPass = false;
                    UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                }

                //判断自摸
                if (MahjongHelper.Instance.JudgeWin(pppd.GetPlayerAllHandCard(1)) > 0)
                {
                    if (pppd.playingMethodConf.byWinLimitReadHand == 0 || pppd.usersCardsInfo[0].listShowCards.Count == 0 || pppd.isChoiceTing == 2)
                    {
                        pppd.isSendPlayerPass = false;
                        MahjongHelper.Instance.specialValue_[3] = 1;
                        MahjongHelper.Instance.specialValue_[7] = 1;
                        UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);

                        //关闭听牌提示
                        Mahjong[] mah_ = MahjongManger.Instance.GetSelfCard();

                        for (int i = 0; i < mah_.Length; i++)
                        {
                            mah_[i].transform.Find("Ting").gameObject.SetActive(false);
                        }
                    }
                }

                if (pppd.isChoiceTing == 0)
                {
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongHelper.Instance.mahjongTing = MahjongHelper.Instance.GetEnableTingCard(2);
                    if (MahjongHelper.Instance.mahjongTing.Count > 0)
                    {
                        //显示所有可以听牌的花色值
                        UIMainView.Instance.PlayerPlayingPanel.UpdateTingCard(MahjongHelper.Instance.Ting.ToArray());
                    }
                    else
                    {
                        MahjongManger.Instance.HideTingLogo();
                    }
                }

                //处理玩家听牌之后，没有出牌之后重入
                if (pppd.isChoiceTing == 1 && !pppd.isAlreadyChangeStatus)
                {
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongHelper.Instance.mahjongTing = MahjongHelper.Instance.GetEnableTingCard(1);
                    if (MahjongHelper.Instance.mahjongTing.Count > 0)
                    {
                        //显示所有可以听牌的花色值
                        UIMainView.Instance.PlayerPlayingPanel.UpdateTingCard(MahjongHelper.Instance.Ting.ToArray());
                    }
                    UIMainView.Instance.PlayerPlayingPanel.ChangeHandCardStatus(1);
                }
            }
            else
            {
                //  Debug.LogWarning("判断报亭牌");
                //玩家处于不可出牌状态，如果重入之前是听牌状态，恢复该状态
                MahjongHelper.Instance.TingCount = new List<MahjongHelper.TingMessage>();
                MahjongHelper.TingMessage[] message = MahjongHelper.Instance.GetTingRemind_Add(pppd.GetPlayerAllHandCard(1));

                if (message.Length > 0)
                {
                    for (int i = 0; i < message.Length; i++)
                    {
                        MahjongHelper.Instance.TingCount.Add(message[i]);
                    }
                    //显示听牌按钮
                    UIMainView.Instance.PlayerPlayingPanel.ShowBtnTing(true);
                }
                else
                {
                    //关闭听牌按钮
                    UIMainView.Instance.PlayerPlayingPanel.ShowBtnTing(false);
                }
            }


            #region 房间需不需要再次弹出规则信息
            int[] SeatIDAgo = new int[4] { 0, 0, 0, 0 };
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo)) == false)
            {
                string splayerSeatId = PlayerPrefs.GetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo);
                SeatIDAgo[0] = int.Parse(splayerSeatId.Substring(0, 1));
                SeatIDAgo[1] = int.Parse(splayerSeatId.Substring(1, 1));
                SeatIDAgo[2] = int.Parse(splayerSeatId.Substring(2, 1));
                SeatIDAgo[3] = int.Parse(splayerSeatId.Substring(3, 1));
            }
            //如果是预约房
            if (msg.roomMessage_[4] > 0)
            {
                if (pppd.iOpenRoomUserId == msg.iUserId && SeatIDAgo[pppd.bySeatNum - 1] == 0)
                {
                    UIMainView.Instance.PlayerPlayingPanel.OnOpenGameRulePanel();
                }
                else if ((pppd.iOpenRoomUserId != msg.iUserId) && (pppd.SeatID[pppd.bySeatNum - 1] == 0 || pppd.SeatID[pppd.bySeatNum - 1] == 1))
                {
                    UIMainView.Instance.PlayerPlayingPanel.OnOpenGameRulePanel();
                }
            }
            else//如果不是预约房
            {
                if ((pppd.iOpenRoomUserId != msg.iUserId) || (pppd.iOpenRoomUserId == msg.iUserId && SeatIDAgo[pppd.bySeatNum - 1] == 0))
                {
                    UIMainView.Instance.PlayerPlayingPanel.OnOpenGameRulePanel();
                }
            }
            //保存上一次开放信息
            //保存上一次房间内人员的信息 0没人 1有人 2有人并且占座 3有人占座不在房间内
            string saveSeatID = pppd.SeatID[0].ToString() + pppd.SeatID[1].ToString() + pppd.SeatID[2].ToString() + pppd.SeatID[3].ToString();
            PlayerPrefs.SetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo, saveSeatID);
            #endregion

            #region 是否显示占座按钮的状态
            if (msg.roomMessage_[4] > 0)
            {
                //Debug.LogError("我座位上的信息：" + pppd.SeatID[pppd.bySeatNum - 1]);
                //Debug.LogError("我是不是馆主：" + MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId);
                //Debug.LogError("是不是我创建房间的：" + (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId));
                if (/*pppd.SeatID[pppd.bySeatNum - 1] == 1 || */pppd.SeatID[pppd.bySeatNum - 1] == 2 || pppd.SeatID[pppd.bySeatNum - 1] == 3)
                {
                    bool isOutRoom = false;
                    if (pppd.SeatID[pppd.bySeatNum - 1] == 2)
                        isOutRoom = true;
                    if (pppd.SeatID[pppd.bySeatNum - 1] == 3)
                        isOutRoom = false;
                    UIMainView.Instance.PlayerPlayingPanel.OnCLickSeatSuccess(pppd.bySeatNum - 1, isOutRoom);
                }
                //是老板 并且是老板创建的房间 并且没有占座信息
                else if (MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId <= 0 && (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId))
                {
                    Debug.Log("------占座消息发送------" + pppd.bySeatNum);
                    Messenger_anhui<int>.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_INSEAT, pppd.bySeatNum);
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[0].transform.GetChild(0).gameObject.SetActive(false);
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[0].transform.GetChild(1).gameObject.SetActive(false);
                }

                //Debug.LogError("------：" + "," + pppd.usersInfo.Count + "," + pppd.usersInfo[0].iUserId + "," + pppd.iOpenRoomUserId);
                //if (pppd.usersInfo.Count >= 1)
                //{
                //    for (int i = 0; i < pppd.usersInfo.Count; i++)
                //    {
                //        //创建房间的ID
                //        if (pppd.usersInfo[i].iUserId == pppd.iOpenRoomUserId)
                //        {
                //            Debug.LogError("--" + pppd.usersInfo[i].iUserId + "," + pppd.iOpenRoomUserId + "--");
                //            UIMainView.Instance.PlayerPlayingPanel._txUsersScor[pppd.GetOtherPlayerPos(pppd.usersInfo[i].iUserId) - 1].text = "占座中";
                //        }
                //    }
                //}
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.OnCLickSeatSuccess(0, false, false, true);
            }
            #endregion

        }


        /// <summary>
        /// 长治潞城麻将的能跑能下通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientCanDownRuNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCanDownRunNotice msg = new NetMsg.ClientCanDownRunNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("长治潞城麻将的能跑能下通知消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                //显示界面
                UIMainView.Instance.PlayerPlayingPanel.ShowCanDownRu(msg.byCanDownRu, 1);
            }

            //更新显示庄的位置，只有当是下的时候处理
            if (msg.byCanDownRu == 1)
            {
                int seatNum = GameData.Instance.PlayerPlayingPanelData.bySeatNum;
                UIMainView.Instance.PlayerPlayingPanel.SetDealer(seatNum, seatNum);
                GameData.Instance.PlayerPlayingPanelData.lcDealerMultiple = 1;
            }
            else
            {
                GameData.Instance.PlayerPlayingPanelData.lcDealerMultiple = 0;
            }

            //关闭玩家等待界面
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            pppd.isWatingPlayerDownOrUp = true;
            pppd.isPanelShow_Wating = false;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
        }

        /// <summary>
        /// 长治潞城麻将的能跑能下回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientCanDownRuRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCanDownRunRes msg = new NetMsg.ClientCanDownRunRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("长治潞城麻将的能跑能下回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("长治潞城麻将的能跑能下回应消息错误编号:" + msg.iError);
                return;
            }

            GameData.Instance.PlayerPlayingPanelData.byCanDownRu = msg.byCanDownRu;
        }


        /// <summary>
        /// 报听回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientReadHandRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientReadHandRes msg = new NetMsg.ClientReadHandRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("报听回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("报听回应消息错误编号:" + msg.iError);
                return;
            }

            ////提示停牌前重新排个序
            //GameData.Instance.PlayerPlayingPanelData.CurrentCradSort(2);

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                pppd.isChoiceTing = 1;
                pppd.isAlreadyChangeStatus = false;
                //改变牌的状态
                UIMainView.Instance.PlayerPlayingPanel.ChangeHandCardStatus(0);
            }

            int index = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId)) - 1;

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.Ting, MahjongLobby_AH.SDKManager.Instance.isex);

            //播放听牌动画
            UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(index, 7, false);

            //显示玩家听牌状态
            UIMainView.Instance.PlayerPlayingPanel.UpdatePlayerTingStatus(index);

            PlayerPrefs.SetInt("TingOneCard", 1);
        }
        /// <summary>
        /// 癞子牌回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientltLaiZiNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientLaiZiNotice msg = new NetMsg.ClientLaiZiNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("癞子牌回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            Debug.LogWarning("HandleClientltLaiZiNotice" + msg.byLaiziSuit);
            GameData.Instance.PlayerNodeDef.byLaiziSuit = msg.byLaiziSuit;
            GameData.Instance.PlayerNodeDef.byLaiziValue = msg.byLaiziValue;

        }

        /// <summary>
        /// 用户占座回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientUserBespeakRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientUserBespeakResDef msg = new NetMsg.ClientUserBespeakResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("用户占座回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                switch (msg.iError)
                {
                    case 16: UIMgr.GetInstance().GetUIMessageView().Show("您已经有了预约房间，不可以继续占座"); break;
                    case 17: UIMgr.GetInstance().GetUIMessageView().Show("此位置已被占", () => { Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN); }); break;
                    case 18: UIMgr.GetInstance().GetUIMessageView().Show("您的金币不足"); break;
                    case 19: UIMgr.GetInstance().GetUIMessageView().Show("您已在其他地方开房"); break;
                }
                Debug.LogError("用户占座回应消息 错误" + msg.iError);
                return;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (msg.iSeatNum == pppd.bySeatNum)
            {
                Debug.Log("用户占座回应消息 是自己");
                UIMainView.Instance.PlayerPlayingPanel.OnCLickSeatSuccess(0, true);
                if (pppd.usersInfo.ContainsKey(msg.iSeatNum))
                    UIMainView.Instance.PlayerPlayingPanel._txUsersScor[pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.iSeatNum].cSeatNum) - 1].text = "占座中";//[pppd.GetOtherPlayerShowPosForOtherPlayerID(pppd.iUserId)].text = "0";
                if (!(MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId <= 0 && (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId)))
                {
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[pppd.bySeatNum - 1].transform.GetChild(0).gameObject.SetActive(false);
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[pppd.bySeatNum - 1].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else
            {
                //Debug.LogError("用户占座回应消息 其他玩家" + (pppd.GetOtherPlayerPos(GameData.Instance.PlayerNodeDef.iUserId) - 1) + "," + (pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.iSeatNum].cSeatNum) - 1)
                //    + "," + msg.iUserId + "," + (pppd.GetOtherPlayerShowPosForOtherPlayerID(msg.iUserId)));
                Debug.Log("用户占座回应消息，其他玩家");
                if (pppd.usersInfo.ContainsKey(msg.iSeatNum))
                    UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(2, pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.iSeatNum].cSeatNum) - 1);// pppd.GetOtherPlayerShowPosForOtherPlayerID(msg.iUserId));
            }
        }

        /// <summary>
        /// 用户取消占座回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientUserCancleBespeakRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientUserCancleBespeakResDef msg = new NetMsg.ClientUserCancleBespeakResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("用户取消占座回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("用户取消占座回应消息 错误" + msg.iError);
                return;
            }
            Debug.Log("用户取消占座回应消息" + msg.iSeatNum);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (msg.iSeatNum == pppd.bySeatNum)
            {
                Debug.Log("用户取消占座回应消息 是自己");
                UIMainView.Instance.PlayerPlayingPanel.OnCLickSeatSuccess(0, false);
                UIMainView.Instance.PlayerPlayingPanel.isSeat = true;
                if (pppd.usersInfo.ContainsKey(pppd.usersInfo[msg.iSeatNum].cSeatNum))
                    UIMainView.Instance.PlayerPlayingPanel._txUsersScor[pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.iSeatNum].cSeatNum) - 1].text = "0";//[pppd.GetOtherPlayerShowPosForOtherPlayerID(pppd.iUserId)].text = "0";
                if (!(MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId <= 0 && (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId)))
                {
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[pppd.bySeatNum - 1].transform.GetChild(0).gameObject.SetActive(true);
                    UIMainView.Instance.PlayerPlayingPanel.PlayerSeat[pppd.bySeatNum - 1].transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("用户取消占座回应消息 其他玩家" + msg.iUserId);
                if (pppd.usersInfo.ContainsKey(pppd.usersInfo[msg.iSeatNum].cSeatNum))
                    UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(1, pppd.GetOtherPlayerShowPos(pppd.usersInfo[msg.iSeatNum].cSeatNum) - 1);// pppd.GetOtherPlayerShowPosForOtherPlayerID(msg.iUserId));
            }

        }

        /// <summary>
        /// 桌上游戏外占座玩家信息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientBespeakUserInfoDef(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientBespeakUserInfoDef msg = new NetMsg.ClientBespeakUserInfoDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            //if (ioffset != len)
            {
                Debug.LogError("桌上游戏外占座玩家信息,ioffset:" + ioffset + ",len:" + len);
                //   return;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            for (int i = 0; i < msg.byUserNum; i++)
            {
                NetMsg.UserInfoDef userinfo = new NetMsg.UserInfoDef();
                ioffset = userinfo.parseBytes(pMsg, ioffset);
                if (!pppd.usersInfo.ContainsKey(userinfo.cSeatNum))
                {
                    pppd.usersInfo.Add(userinfo.cSeatNum, userinfo);
                    SystemMgr.Instance.PlayerPlayingSystem.HeadUpdateShow((userinfo.cSeatNum));
                }
                else
                {
                    pppd.usersInfo[userinfo.cSeatNum] = userinfo;
                }

                //调用其他玩家是否在线
                int pos = pppd.GetOtherPlayerShowPos(pppd.usersInfo[userinfo.cSeatNum].cSeatNum) - 1;
                UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(3, pos);
                UIMainView.Instance.PlayerPlayingPanel._txUsersScor[pos].text = "占座中";
            }
        }

        /// <summary>
        /// 四人入座等待用户准备/取消准备通知
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientWaitReadyNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.Server2ClientWaitReadyNoticeDef msg = new NetMsg.Server2ClientWaitReadyNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            //if (ioffset != len)
            //{
            //    Debug.LogError("四人入座等待用户准备,ioffset:" + ioffset + ",len:" + len);
            //    return;
            //}
            //UIMgr.GetInstance().GetUIMessageView().Show("由于" + "退出，离开房间，牌局人数不足，请耐心等待！", () =>
            //{
            //});
            Debug.Log("准备或是取消准备标志：" + msg.byMode + "," + msg.iWaitTime);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //取消准备
            if (msg.byMode == 2)
            {
                string userName = "";

                for (int i = 1; i < pppd.usersInfo.Count + 1; i++)
                {
                    for (int j = 0; j < msg.OutUserID.Length; j++)
                    {
                        if (pppd.usersInfo.ContainsKey(i) && pppd.usersInfo[i].iUserId == msg.OutUserID[j])
                        {
                            userName += pppd.usersInfo[i].szNickname + ",";
                        }
                    }
                }

                if (UIMainView.Instance.PlayerPlayingPanel.LoatTime <= 0)
                {
                    Debug.Log("被踢玩家的名称" + userName);
                    //关闭玩家的准备手势
                    UIMainView.Instance.PlayerPlayingPanel.CloseReadyImage();
                    UIMgr.GetInstance().GetUIMessageView().Show(userName + "离开房间，牌局人数不足，请耐心等待！");
                }
                else//说明是预约房时间到了
                {
                    if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId)
                    {
                        UIMgr.GetInstance().GetUIMessageView().Show("由于" + userName + "退出，离开房间，牌局人数不足，请耐心等待！", () =>
                        {
                            //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER,
                            //    () =>
                            //    {
                            //        UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(false, 0, false);
                            //    },
                            //    () =>
                            //    {
                            //        Messenger.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_DISSOLVEROOM);
                            //    }
                            //);
                        });
                    }
                    else
                    {
                        UIMgr.GetInstance().GetUIMessageView().Show("由于" + userName + "退出，离开房间，牌局人数不足，请耐心等待！", () =>
                        {
                            //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER,
                            //    () =>
                            //    {
                            //        UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(false, 0, false);
                            //    },
                            //    () =>
                            //    {
                            //        Messenger.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN);
                            //    }
                            //);
                        });

                    }
                }

                UIMainView.Instance.PlayerPlayingPanel.m_isLost60Scend = false;

            }

            if (msg.iWaitTime > 0)
            {
                UIMainView.Instance.PlayerPlayingPanel.m_isLost60Scend = true;
            }
            if (msg.byStartFrequency == 2)
            {
                if (UIMainView.Instance.PlayerPlayingPanel.LoatTime <= 0)
                    UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueRoomLostOneMin(msg.iWaitTime);
            }
            else
            {
                //倒计时开始
                UIMainView.Instance.PlayerPlayingPanel.OnPlayerHereReady(msg.byMode == 1 ? true : false, msg.iWaitTime);

                NetMsg.ClientReadyTimeReqDef msg_readtime = new NetMsg.ClientReadyTimeReqDef();
                msg_readtime.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg_readtime.wTableNum = (byte)MahjongCommonMethod.Instance.iTableNum;
                NetworkMgr.Instance.GameServer.SendClientReadyTimeReqDef(msg_readtime);
            }
            pppd.TablePlayerUserID = msg.UserID;
        }

        /// <summary>
        /// 踢出为准备用户通知
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandlePlayerWithoutReady(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.KickOutPlayerWithoutReadyDef msg = new NetMsg.KickOutPlayerWithoutReadyDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("踢出为准备用户通知,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            Debug.Log("--" + GameData.Instance.PlayerNodeDef.iUserId + "," + msg.iUserId[0] + "," + msg.iUserId[1] + "," + msg.iUserId[2] + "," + msg.iUserId[3] + "--");
            //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOM_NOTREADY, () => { Messenger.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN); }, 5);
            bool isPlayer = false;
            for (int i = 0; i < 4; i++)
            {
                if (msg.iUserId[i] == GameData.Instance.PlayerNodeDef.iUserId)
                {
                    isPlayer = true;
                    break;
                }
            }
            if (isPlayer)
            {
                Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN);
                MahjongCommonMethod.Instance.GameLaterDo_ = showbackLobbyLog;
                //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOM_NOTREADY, () => { Messenger.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN); }, 5);
            }

            UIMainView.Instance.PlayerPlayingPanel.CloseReadyImage();
        }

        void showbackLobbyLog()
        {
            MahjongLobby_AH.UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOM_NOTREADY, () => { }, 5);
        }

        /// <summary>
        /// 处理游戏内可以开始抢红包的通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientRp17StartNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientRp17StartNotice msg = new NetMsg.ClientRp17StartNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("开始抢红包通知消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            //红包开启失败，弹出提示
            if (msg.iError != 0)
            {
                Debug.LogError("开始抢红包通知错误编号:" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("当前在线人数未满足条件，无法领取红包");
                //隐藏红包信息
                UIMainView.Instance.ParlorRedBagMessage.UpdateRob(0);
                return;
            }
            else
            {
                UIMainView.Instance.ParlorRedBagMessage.UpdateRob(1);
            }
        }

        /// <summary>
        /// 处理玩家抢红包回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientOpenRp17Res(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientOpenRp17Res msg = new NetMsg.ClientOpenRp17Res();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("玩家抢红包回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            //红包开启失败，弹出提示
            if (msg.iError != 0)
            {
                if (msg.iError == 6)
                {
                    //关闭红包界面
                    UIMgr.GetInstance().GetUIMessageView().Show("在线人数不足，红包开启失败");
                    UIMainView.Instance.ParlorRedBagMessage.UpdateRobStatus(0);
                    return;
                }

                if (msg.iError == 5)
                {
                    //关闭红包界面
                    UIMgr.GetInstance().GetUIMessageView().Show("您已经领取过该红包了！");
                    UIMainView.Instance.ParlorRedBagMessage.UpdateRobStatus(0);
                    return;
                }


                UIMainView.Instance.ParlorRedBagMessage.GetAwardShow(null);
                //隐藏红包信息
                return;
            }

            UIMainView.Instance.ParlorRedBagMessage.GetAwardShow(msg.srp17Info);

        }

        /// <summary>
        /// 处理玩家托管通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientAutoStatus(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.SendUserAutoStatus msg = new NetMsg.SendUserAutoStatus();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("发送用户托管状态给客户端,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            Debug.Log("玩家托管通知消息,玩家id:" + msg.iUserId + ",托管类型:" + msg.byStatus + ",座位号:" + msg.bySeatNum);

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            pppd.iAllPlayerHostStatus[msg.bySeatNum - 1] = msg.byStatus;
            //如果托管人是自己，显示托管按钮
            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                if (msg.byStatus == 1)
                {
                    pppd.iPlayerHostStatus = msg.byStatus;
                    UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(true);
                    UIMainView.Instance.PlayerPlayingPanel.ShowHosting(0, true);
                }
                else
                {
                    UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(false);
                }
            }
            else
            {
                Debug.Log("msg.byStatus：" + msg.byStatus + ",msg.bySeatNum：" + msg.bySeatNum);

                if (msg.byStatus == 1)
                {
                    //显示玩家托管状态
                    int index = pppd.GetOtherPlayerShowPos(msg.bySeatNum) - 1;
                    Debug.Log("index:" + index);
                    UIMainView.Instance.PlayerPlayingPanel.ShowHosting(index, true);
                }
                else
                {
                    //显示玩家托管状态
                    int index = pppd.GetOtherPlayerShowPos(msg.bySeatNum) - 1;
                    UIMainView.Instance.PlayerPlayingPanel.ShowHosting(index, false);
                }
            }
        }

        /// <summary>
        /// 处理用户取消托管回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientCanelAutoStatus(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCancleAutoStatusRes msg = new NetMsg.ClientCancleAutoStatusRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("用户取消托管回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("用户取消托管回应消息错误编号:" + msg.iError);
                return;
            }


            //处理玩家托管回应中，如果有四个玩家托管
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (pppd.isAllAutoStatus)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("有四个玩家进行托管，是否解散",
                () =>
                {
                    //发送解散请求
                    NetMsg.ClientDismissRoomReq diss = new NetMsg.ClientDismissRoomReq();
                    diss.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    diss.cType = 1;
                    Network.NetworkMgr.Instance.GameServer.SendDisMissRoomReq(diss);
                },
                () =>
                {
                    Debug.Log("点击取消，发送准备请求");
                    //发送准备请求
                    NetMsg.ClientReadyReq ready = new NetMsg.ClientReadyReq();
                    ready.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    NetworkMgr.Instance.GameServer.SendReadyReq(ready);
                }
                );
            }

            pppd.iAllPlayerHostStatus[pppd.GetOtherPlayerPos(msg.iUserId) - 1] = 0;
            if (GameData.Instance.PlayerNodeDef.iUserId == msg.iUserId)
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(false);
                GameData.Instance.PlayerPlayingPanelData.iPlayerHostStatus = 0;
                //关闭头像上的托管提示
                UIMainView.Instance.PlayerPlayingPanel.ShowHosting(0, false);
            }
        }

        /// <summary>
        /// 分享成功回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSharedSuccessRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientShareSuccessRes msg = new NetMsg.ClientShareSuccessRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("分享成功回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("分享成功回应消息错误编号:" + msg.iError);
                return;
            }
            Debug.LogWarning("分享成功回应：shareID" + msg.iShareId + "次数" + msg.iShareCount + "最后分享时间" + msg.iShareTim + "领取奖励时间;" + msg.iAwardTim + "领取赠卡书量" + msg.iFreeCard);
            if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
            {
                Debug.Log("分享成功 发送服务器成功" + GameData.Instance.m_active);
                switch (GameData.Instance.m_active)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 7:
                    case 11:
                    case 13:
                    case 14:
                        {
                            //UIMgr.GetInstance().ShowRedPagePanel.OnSetState();
                            //UIMainView.Instance.GameResultPanel.RPButton.SetActive(false);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 领取红包回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientReceiveRedRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientReceiveRedResDef msg = new NetMsg.ClientReceiveRedResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("领取红包回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                switch (msg.iError)
                {
                    case -1: UIMgr.GetInstance().GetUIMessageView().Show("领取红包数量上限"); break;
                    case -18: UIMgr.GetInstance().GetUIMessageView().Show("打开红包失败"); break;
                }
                Debug.LogError("领取红包回应消息：" + msg.iError);
                return;
            }

            int RpType = -1;
            string str = "", str1 = "", str2 = "";
            str2 = (msg.dResourceNum / 100.0f).ToString();
            switch (msg.byResourceType)
            {
                case 1:
                    {
                        str = "现金";
                        str1 = "元";
                        RpType = 3;
                        MahjongLobby_AH.GameData.Instance.PlayerNodeDef.userDef.da2Asset[0] += msg.dResourceNum;
                    }
                    break;
                case 2:
                    {
                        str = "话费";
                        str1 = "元";
                        RpType = 4;
                        MahjongLobby_AH.GameData.Instance.PlayerNodeDef.userDef.da2Asset[1] += msg.dResourceNum;
                    }
                    break;
                case 3:
                    {
                        str = "流量";
                        str1 = "M";
                        RpType = 1;
                        MahjongLobby_AH.GameData.Instance.PlayerNodeDef.userDef.da2Asset[2] += msg.dResourceNum;
                    }
                    break;
                case 4:
                    {
                        str = "储值卡";
                        str1 = "元";
                        RpType = 2;
                        MahjongLobby_AH.GameData.Instance.PlayerNodeDef.userDef.da2Asset[3] += msg.dResourceNum;
                    }
                    break;
                case 5:
                    {
                        str = "代金券";
                        str1 = "元";
                        RpType = 5;
                    }
                    break;
                case 6:
                    {
                        str = "赠币";
                        str1 = "个";
                        RpType = 0;
                        MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iBindCoin += msg.dResourceNum;
                    }
                    break;
            }

            string strDesc = "";
            switch (msg.byRedPagType)
            {
                case 1: strDesc = "成功创建房间有机会获得一个红包，惊喜连连！"; break;
                case 6: strDesc = "体验一局游戏就拿到一个红包，真是惊喜连连！"; break;
                case 13: strDesc = "打牌也有红包拿！"; break;
                case 14: strDesc = "打牌也有红包拿！"; break;
            }

            UIMgr.GetInstance().ShowRedPagePanel.DirectOpenRedPagePanel(str2, RpType, strDesc);

            Debug.Log("领取的红包：" + msg.dResourceNum.ToString() + str1 + str);
            Debug.Log("获得资源类型:1现金，2话费，3流量，4储值卡，（5代金券，6赠币没有对应字段）" + msg.byResourceType);
        }

        /// <summary>
        /// 通知用户获得一个红包消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientObtainRedNotice(byte[] pMsg, int len)
        {
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }

            int ioffset = 0;
            NetMsg.ClientObtainRedNoticeDef msg = new NetMsg.ClientObtainRedNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("通知用户获得一个红包消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            Debug.LogError("通知用户获得一个红包消息" + msg.byRedPagType);

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            Debug.Log("-==-" + msg.byRedPagType);

            switch (msg.byRedPagType)
            {
                case 1:
                    {
                        //GameData.Instance.m_active = 0;
                        //PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                        MahjongCommonMethod.Instance.GameLaterDo_ = () =>
                        {
                            MahjongLobby_AH.UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 2, "建房红包", RedPageShowPanel.NowState.Lobby);
                        };
                    }
                    break;
                case 6:
                    {
                        //GameData.Instance.m_active = 0;
                        //PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                        MahjongCommonMethod.Instance.GameLaterDo_ = () =>
                        {
                            MahjongLobby_AH.UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 2, "首次参与红包", RedPageShowPanel.NowState.Lobby);
                        };
                    }
                    break;
                case 13:
                    {
                        GameData.Instance.m_active = 13;
                        PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                        UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 2, "大赢家红包", RedPageShowPanel.NowState.Game);
                    }
                    break;
                case 14:
                    {
                        GameData.Instance.m_active = 14;
                        PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                        UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 2, "最佳炮手红包", RedPageShowPanel.NowState.Game);
                    }
                    break;
            }
        }



        /// <summary>
        /// 通知用户获得一个待领状态红包消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientObtainReceiveRedNotice(byte[] pMsg, int len)
        {
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }
            int ioffset = 0;
            NetMsg.ClientObtainReceiveRedNoticeDef msg = new NetMsg.ClientObtainReceiveRedNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("通知用户获得一个待领状态红包消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            Debug.Log("通知用户获得一个待领状态红包消息");

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            // Debug.Log("----" + msg.byRedType);

            switch (msg.byRedType)
            {
                case 1:
                    {
                        //GameData.Instance.m_active = 0;
                        //PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                    }
                    break;
                case 6:
                    {
                        //GameData.Instance.m_active = 0;
                        //PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                    }
                    break;
                case 13:
                    {
                        GameData.Instance.m_active = 13;
                        PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                    }
                    break;
                case 14:
                    {
                        GameData.Instance.m_active = 14;
                        PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                    }
                    break;
            }

            if (msg.byRedType == 13 || msg.byRedType == 14)
            {
                Debug.Log("==" + GameData.Instance.m_active);
                //UIMainView.Instance.GameResultPanel.RPButton.SetActive(true);
                if (msg.byRedType == 13)
                {
                    UIMainView.Instance.GameResultPanel.RPButton.transform.GetChild(0).gameObject.SetActive(true);
                    UIMainView.Instance.GameResultPanel.RPButton.transform.GetChild(1).gameObject.SetActive(false);
                }
                else if (msg.byRedType == 14)
                {
                    UIMainView.Instance.GameResultPanel.RPButton.transform.GetChild(0).gameObject.SetActive(false);
                    UIMainView.Instance.GameResultPanel.RPButton.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 获取准备剩余时间回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientReadyTimeResDef(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientReadyTimeResDef msg = new NetMsg.ClientReadyTimeResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("获取准备剩余时间回应,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            Debug.Log("剩余预约时间" + msg.iLeftSecs);
            UIMainView.Instance.PlayerPlayingPanel.StartOnPlayerHereReady(msg.iLeftSecs);

            //PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //for (int i = 0; i < msg.iaUserReadyStatus.Length; i++)
            //{
            //    if (msg.iaUserReadyStatus[i] > 0)
            //    {
            //        UIMainView.Instance.PlayerPlayingPanel.ReadyImage[msg.iaUserReadyStatus[i] - 1].gameObject.SetActive(true);
            //    }
            //}

            //UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueRoomLostOneMin(msg.iLeftSecs);
        }


        /// <summary>
        /// 预约时间结束通知
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleBespeakTimeOutNoticeDef(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.BespeakTimeOutNoticeDef msg = new NetMsg.BespeakTimeOutNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("预约时间结束通知,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            int[] userID = new int[4] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                if (pppd.usersInfo.ContainsKey((msg.OutSeatNum[i])))
                    userID[i] = pppd.usersInfo[i].iUserId;
                else
                    userID[i] = 0;
            }

            //如果桌上少人
            if (userID[0] != 0 && userID[1] != 0 && userID[2] != 0 && userID[3] != 0)
            {
                UIMainView.Instance.PlayerPlayingPanel.PlayerReady[0].SetActive(false);
                UIMainView.Instance.PlayerPlayingPanel.PlayerReady[1].SetActive(true);
                UIMainView.Instance.PlayerPlayingPanel.PlayerReadyTimeLost.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                UIMainView.Instance.PlayerPlayingPanel.PlayerReady[0].SetActive(true);
                UIMainView.Instance.PlayerPlayingPanel.PlayerReady[1].SetActive(false);
                UIMainView.Instance.PlayerPlayingPanel.PlayerReadyTimeLost.transform.GetChild(2).gameObject.SetActive(false);
            }

            if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER,
                    () =>
                    {

                    },
                    () => { Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_DISSOLVEROOM); });
            }
            else
            {
                UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER,
                    () =>
                    {

                    },
                    () => { Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN); });
            }

            for (int i = 0; i < 4; i++)
            {
                if (msg.OutSeatNum[i] != 0)
                {
                    if (pppd.usersInfo.ContainsKey((msg.OutSeatNum[i])))
                    {
                        pppd.usersInfo.Remove((msg.OutSeatNum[i]));
                        SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate((msg.OutSeatNum[i]));
                    }
                }
            }
        }

        //补花通知消息
        void HandleClientAppliqueNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientAppliqueNotice msg = new NetMsg.ClientAppliqueNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("补花通知消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }


            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            pppd.isNeedSort = 1;
            //在这里处理玩家时候手中有花牌
            byte[] temp = pppd.GetPlayerFlowerCard();

            //如果玩家手中有花牌请求花牌信息
            if (temp.Length > 0)
            {
                NetMsg.ClientAppliqueReq msg_que = new NetMsg.ClientAppliqueReq();
                msg_que.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg_que.byFlowerTileNum = (byte)temp.Length;
                for (int i = 0; i < 8; i++)
                {
                    if (i < temp.Length)
                    {
                        msg_que.byaFlowerTile[i] = temp[i];
                    }
                    else
                    {
                        msg_que.byaFlowerTile[i] = 0;
                    }
                }
                NetworkMgr.Instance.GameServer.SendClientAppliqueReq(msg_que);
            }
        }

        //补花回应消息
        void HandleClientAppliqueRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientAppliqueRes msg = new NetMsg.ClientAppliqueRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (msg.iError != 0)
            {
                Debug.LogError("补花回应消息错误编号:" + msg.iError);
                return;
            }
            if (ioffset != len)
            {
                Debug.LogError("补花回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            //播放声音之后，产生补花动画          
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.BuHua, MahjongLobby_AH.SDKManager.Instance.isex);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int index_ = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(msg.iUserId)) - 1;
            UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(index_, 16);
            Lock();
            if (msg.iUserId == pppd.iUserId)
            {
                //处理玩家的花牌信息
                List<Mahjong> mah = new List<Mahjong>();
                Mahjong[] go = null;
                go = MahjongManger.Instance.transform.GetComponentsInChildren<Mahjong>();
                for (int i = 0; i < go.Length; i++)
                {
                    if (go[i].enabled && go[i].name.Contains("current"))
                    {
                        mah.Add(go[i]);
                    }
                }
                for (int i = 0; i < msg.byaFlowerTile.Length; i++)
                {
                    //处理手牌信息
                    if (msg.byaFlowerTile[i] > 0)
                    {
                        //删除牌的值
                        for (int j = 0; j < pppd.usersCardsInfo[0].listCurrentCards.Count; j++)
                        {
                            if (msg.byaFlowerTile[i] == pppd.usersCardsInfo[0].listCurrentCards[j].cardNum)
                            {
                                pppd.usersCardsInfo[0].listCurrentCards.RemoveAt(j);
                            }
                        }

                        for (int j = 0; j < mah.Count; j++)
                        {
                            if (msg.byaFlowerTile[i] == mah[j].bMahjongValue)
                            {
                                PoolManager.Unspawn(mah[j].gameObject);
                                pppd.iSpwanCardNum--;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                List<byte> flowerCard = new List<byte>();
                for (int i = 0; i < msg.byaTile.Length; i++)
                {
                    if (msg.byaTile[i] > 96)
                    {
                        flowerCard.Add(msg.byaTile[i]);
                    }

                    pppd.bLastCard = msg.byaTile[i];
                    if (msg.byaTile[i] == 0)
                    {
                        break;
                    }
                }
                //产生花牌
                byte[] mahjongValue = new byte[msg.byTileNum];
                for (int i = 0; i < msg.byaTile.Length; i++)
                {
                    if (msg.byaTile[i] > 0)
                    {
                        mahjongValue[i] = msg.byaTile[i];
                        //Debug.LogError("产生的手牌信息:" + msg.byaTile[i]);
                    }
                    else
                    {
                        break;
                    }
                }

                UIMainView.Instance.PlayerPlayingPanel.SpwanMahjong(mahjongValue, 0);
                if (pppd.isNeedSort >0)
                {
                    pppd.CurrentCradSort(2);
                    pppd.isNeedSort = 0;
                }
                Unlock();

                //如果花牌数量不为0，继续补花
                if (flowerCard.Count > 0)
                {
                    NetMsg.ClientAppliqueReq msg_que = new NetMsg.ClientAppliqueReq();
                    msg_que.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    msg_que.byFlowerTileNum = (byte)flowerCard.Count;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < flowerCard.Count)
                        {
                            msg_que.byaFlowerTile[i] = flowerCard[i];
                        }
                        else
                        {
                            msg_que.byaFlowerTile[i] = 0;
                        }
                    }
                    NetworkMgr.Instance.GameServer.SendClientAppliqueReq(msg_que);
                }
                byte[] bts = { msg.byaTile[msg.byTileNum - 1] };

                //  UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(msg.byaTile[msg.byTileNum - 1], 0.1f);
                // UIMainView.Instance.PlayerPlayingPanel.MoveLastCard();
                //  pppd.CurrentCradSort(2);
                //   pppd.isRecordScore_Break = false;
                //PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;


            }
            else
            {
                Unlock();
            }

            //如果是其他玩家补花，减去手牌数量
            //if (pppd.iUserId != msg.iUserId)
            //{
            //    pppd.LeftCardCount -= msg.byTileNum;
            //    //更新剩余牌数量
            //    SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
            //}

            //更新玩家花牌数量
            UIMainView.Instance.PlayerPlayingPanel.UpdateShowFlowerCount(2, pppd.GetOtherPlayerPos(msg.iUserId), msg.byTileNum);
        }
        #endregion 收到网络消息     

    }

}
