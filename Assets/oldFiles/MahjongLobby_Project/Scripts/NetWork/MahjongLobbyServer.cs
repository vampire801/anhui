using UnityEngine;
using System.Collections.Generic;
using System.Net;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using System;

using XLua;
using anhui;
namespace MahjongLobby_AH.Network
{
    [Hotfix]
    [LuaCallCSharp]
    public class MahjongLobbyServer
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

        public List<SavedMsgItem> m_SavedMsgList = new List<SavedMsgItem>();

        #endregion 公共成员变量

        #region 私有成员变量

        List<MsgData> sendMsgQueue = new List<MsgData>(); // 发送消息队列，由于是单线程，所以没有加互斥锁
        int proxyIndex; //使用代理的索引，从1开始，应该是1~5
        int tryCount; //尝试连接次数

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
                if (sendMsgQueue.Count > 0)
                {
                    MsgData msgData = sendMsgQueue[0];
                    NetClient.SendCliMsg(msgData.obj, msgData.cType, msgData.bEncrypt);
                    sendMsgQueue.RemoveAt(0);
                }

                if (DisconnectFlag)
                {
                    Debug.LogWarning("===============================0");
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
        /// 连接大厅服务器
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
                Debug.LogWarning("开始连接大厅服务器[" + ServerIP + ":" + ServerPort + "]");

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
            DisconnectFlag = true;
            isSuccessLogin = false;
            tryCount = 0;
            NetworkMgr.Instance.LobbyGateway.DisconnectFlag = true;
        }



        bool isSuccessLogin; //玩家是登录成功

        /// <summary>
        /// 发送消息，把消息加入发送队列
        /// </summary>
        /// <param name='obj'>消息对象</param>
        /// <param name='type'>消息类型</param>
        /// <param name='encrypt'>是否加密</param>
        public void SendClientMsg(object obj, ushort type, bool encrypt)
        {
            //如果玩家未登录，不发送请求消息
            if (type != NetMsg.CLIENT_AUTHEN_REQ && !isSuccessLogin)
            {
                return;
            }

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

        #region 添加发送消息
        byte[] ToByteArray(object obj, ushort type)
        {
            switch (type)
            {
                case NetMsg.KEEP_ALIVE:
                    {
                        NetMsg.KeepAlive msg = (NetMsg.KeepAlive)obj;
                        msg.MsgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadInfo.MsgType = type;
                        //Debug.LogWarning("发送保持连接消息");
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
                case NetMsg.CLIENT_CHANGE_USER_INFO_REQ:
                    {
                        NetMsg.ClientChangeUserInfoReq msg = (NetMsg.ClientChangeUserInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("发送修改用户信息请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_FULL_NAME_REQ:
                    {
                        NetMsg.ClientFullNameReq msg = (NetMsg.ClientFullNameReq)obj;
                        msg.MsgHeadDef.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadDef.MsgType = type;
                        Debug.Log("发送实名认证请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SPREAD_CODE_REQ:
                    {
                        NetMsg.ClientSpreadCodeReq msg = (NetMsg.ClientSpreadCodeReq)obj;
                        msg.MsgHeadDef.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadDef.MsgType = type;
                        Debug.Log("发送激活推广码请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_GETEXCHANGE_COIN_REQ:
                    {
                        NetMsg.ClientGetExchangeCoinReq msg = (NetMsg.ClientGetExchangeCoinReq)obj;
                        msg.MsgHeadDef.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadDef.MsgType = type;
                        Debug.Log("发送充值兑换配置请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SPREAD_GIFT_REQ:
                    {
                        NetMsg.ClientSpreadGiftReq msg = (NetMsg.ClientSpreadGiftReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("推广礼包请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SPREADER_INFO_REQ:
                    {
                        NetMsg.ClientSpreaderInfoReq msg = (NetMsg.ClientSpreaderInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("推广员信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.BULLETIN_REQ:
                    {
                        NetMsg.BulletinReq msg = (NetMsg.BulletinReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("发送公告请求消息");
                        return msg.toBytes();
                    }
                //case NetMsg.CLIENT_BIND_PROXY_REQ:
                //    {
                //        NetMsg.ClientBindProxyReq msg = (NetMsg.ClientBindProxyReq)obj;
                //        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                //        msg.msgHeadInfo.MsgType = type;
                //        Debug.LogError("发送绑定代理请求消息");
                //        return msg.toBytes();
                //    }
                //case NetMsg.CLIENT_ASK_UNBIND_PROXY_REQ:
                //    {
                //        NetMsg.ClientAskUnbindProxyReq msg = (NetMsg.ClientAskUnbindProxyReq)obj;
                //        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                //        msg.msgHeadInfo.MsgType = type;
                //        Debug.LogError("发送申请解除绑定代理请求消息");
                //        return msg.toBytes();
                //    }
                case NetMsg.CLIENT_MESSAGE_OPERATE_REQ:
                    {
                        NetMsg.ClientMessageOperateReq msg = (NetMsg.ClientMessageOperateReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("发送信息操作请求消息");
                        return msg.toBytes();
                    }
                //case NetMsg.CLIENT_PROXY_INFO_REQ:
                //    {
                //        NetMsg.ClientProxyInfoReq msg = (NetMsg.ClientProxyInfoReq)obj;
                //        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                //        msg.msgHeadInfo.MsgType = type;
                //        //Debug.LogError("发送代理信息请求消息");
                //        return msg.toBytes();
                //    }
                case NetMsg.CLIENT_CITY_COUNTY_REQ:
                    {
                        NetMsg.ClientCityCountyReq msg = (NetMsg.ClientCityCountyReq)obj;
                        msg.MsgHeadDef.Version = NetMsg.MESSAGE_VERSION;
                        msg.MsgHeadDef.MsgType = type;
                        //Debug.LogError("发送选择市县请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_OPEN_ROOM_REQ:
                    {
                        NetMsg.ClientOpenRoomReq msg = (NetMsg.ClientOpenRoomReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("发送开启房间的请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_OPEN_ROOM_INFO_REQ:
                    {
                        NetMsg.ClientOpenRoomInfoReq msg = (NetMsg.ClientOpenRoomInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("发送开房信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_MY_ROOM_INFO_REQ:
                    {
                        NetMsg.ClientMyRoomInfoReq msg = (NetMsg.ClientMyRoomInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("发送我的开房信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_GAME_SERVER_INFO_REQ:
                    {
                        NetMsg.ClientGameServerInfoReq msg = (NetMsg.ClientGameServerInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("发送游戏服务器信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_COMPLIMENT_REQ:
                    {
                        NetMsg.ClientComplimentReq msg = (NetMsg.ClientComplimentReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("发送点赞请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_HOLIDAY_REQ:
                    {
                        NetMsg.ClientHolidayReq msg = (NetMsg.ClientHolidayReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("节日信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_HOLIDAY_GIFT_REQ:
                    {
                        NetMsg.ClientHolidayGiftReq msg = (NetMsg.ClientHolidayGiftReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("领取节日活动奖励请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_FREE_TIME_REQ:
                    {
                        NetMsg.ClientFreeTimeReq msg = (NetMsg.ClientFreeTimeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        //Debug.LogError("免费时间信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SHARE_REQ:
                    {
                        NetMsg.ClientShareReq msg = (NetMsg.ClientShareReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("分享活动信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_SHARE_USER_REQ:
                    {
                        NetMsg.ClientShareUserReq msg = (NetMsg.ClientShareUserReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("分享活动用户信息请求消息（大厅客户端无需请求）");
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
                case NetMsg.CLIENT_USE_ACTIVE_CODE_REQ:
                    {
                        NetMsg.ClientUseActiveCodeReq msg = (NetMsg.ClientUseActiveCodeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("使用激活码请求消息");
                        return msg.toBytes();
                    }


                case NetMsg.CLIENT_CREATE_ORDER_REQ:
                    {
                        NetMsg.ClientCreateOrderReq msg = (NetMsg.ClientCreateOrderReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("创建订单请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CHARGE_REQ:
                    {
                        NetMsg.ClientChargeReq msg = (NetMsg.ClientChargeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("充值请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_USER_CHARGE_REQ:
                    {
                        NetMsg.ClientUserChargeReq msg = (NetMsg.ClientUserChargeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("充值信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_ROOM_NUM_SERVER_TABLE_REQ:
                    {
                        NetMsg.ClientRoomNumServerTableReq msg = (NetMsg.ClientRoomNumServerTableReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("房间号的服务器和桌信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_LOTTERY_COUNT_REQ:
                    {
                        NetMsg.ClientLotteryCountReqDef msg = (NetMsg.ClientLotteryCountReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("增加抽奖次数请求");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_LOTTERY_REQ:
                    {
                        NetMsg.ClientLotteryReqDef msg = (NetMsg.ClientLotteryReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("抽奖请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_USER_LOTTERY_REQ:
                    {
                        NetMsg.ClientUserLotteryReqDef msg = (NetMsg.ClientUserLotteryReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("抽奖请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CREATE_PARLOR_REQ:
                    {
                        NetMsg.ClientCreateParlorReq msg = (NetMsg.ClientCreateParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("创建麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_DISMISS_PARLOR_REQ:
                    {
                        NetMsg.ClientDismissParlorReq msg = (NetMsg.ClientDismissParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("解散麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CHANGE_PARLOR_BULLETIN_INFO_REQ:
                    {
                        NetMsg.ClientChangeParlorBulletinInfoReq msg = (NetMsg.ClientChangeParlorBulletinInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("修改麻将馆公告信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CHANGE_PARLOR_CONTACT_INFO_REQ:
                    {
                        NetMsg.ClientChangeParlorContactInfoReq msg = (NetMsg.ClientChangeParlorContactInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("修改麻将馆联系信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_JOIN_PARLOR_REQ:
                    {
                        NetMsg.ClientJoinParlorReq msg = (NetMsg.ClientJoinParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("申请 加入麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_LEAVE_PARLOR_REQ:
                    {
                        NetMsg.ClientLeaveParlorReq msg = (NetMsg.ClientLeaveParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("申请 退出麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_INVITE_PARLOR_REQ:
                    {
                        NetMsg.ClientInvitParlorReq msg = (NetMsg.ClientInvitParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("邀请用户进入麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_KICK_PARLOR_REQ:
                    {
                        NetMsg.ClientKickParlorReq msg = (NetMsg.ClientKickParlorReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("踢用户出麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_GETPARLORINFO_REQ:
                    {
                        NetMsg.ClientGetParlorInfoReq msg = (NetMsg.ClientGetParlorInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取麻将馆信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_PARLOR_CERT_REQ:
                    {
                        NetMsg.ClientParlorCertReq msg = (NetMsg.ClientParlorCertReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取是否有创建麻将馆资格的请求消息");
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
                case NetMsg.CLIENT_GETUSERINFO_REQ:
                    {
                        NetMsg.ClientGetUserInfoReq msg = (NetMsg.ClientGetUserInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取用户信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_GET_TABLE_USERID_REQ:
                    {
                        NetMsg.ClientGetTableUserIDReq msg = (NetMsg.ClientGetTableUserIDReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取桌上玩家信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_GET_TABLE_USER_INFO_REQ:

                    {
                        NetMsg.ClientGetTableUserInfoReq msg = (NetMsg.ClientGetTableUserInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取桌上玩家信息请求消息");
                        return msg.toBytes();
                    }

                case NetMsg.CLIENT_RED_NUM_REQ:
                    {
                        NetMsg.ClientRedNumReqDef msg = (NetMsg.ClientRedNumReqDef)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("所有红包数量请求消息");
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

                case NetMsg.CLIENT_BOSSSCORE_TO_COIN_REQ:
                    {
                        NetMsg.ClientScoreToCoinReq msg = (NetMsg.ClientScoreToCoinReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("三种兑换金币请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_REQ:
                    {
                        NetMsg.ClientCAncelApplyOrJudgeApplyTooReq msg = (NetMsg.ClientCAncelApplyOrJudgeApplyTooReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_REFRESH_USER_REQ:
                    {
                        NetMsg.ClientRefreshUserReq msg = (NetMsg.ClientRefreshUserReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("刷新用户信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_GET_PARLOR_TABLEINFO_REQ:
                    {
                        NetMsg.ClientGetParlorTableInfoReq msg = (NetMsg.ClientGetParlorTableInfoReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取麻将馆内某一页的桌信息请求消息");
                        return msg.toBytes();
                    }
                case NetMsg.CLIENT_RP17_TYPE_REQ:
                    {
                        NetMsg.ClientRp17TypeReq msg = (NetMsg.ClientRp17TypeReq)obj;
                        msg.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
                        msg.msgHeadInfo.MsgType = type;
                        Debug.Log("获取红包状态请求消息");
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
            }
            byte[] ret = new byte[1];
            return ret;
        }



        #endregion 添加发送消息

        #region 私有成员方法

        /// <summary>
        /// 当连接服务器失败
        /// </summary>
        void OnConnectServerFailed()
        {
            //重连服务器
            DEBUG.LogTracer("[LobbyNetMgr] " + "第" + tryCount + "次连接大厅服务器，失败!");
            bool bTryAgain = true; //是否再尝试连接
            if (NetworkMgr.Instance.ProxyEnable == 0)
            {
                //当不用代理，重试3次
                if (tryCount >= 3)
                {
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    bTryAgain = false;
                    if (UIMainView.Instance)
                    {
                        UIMainView.Instance.disConnect.UpdateShow(2, @"亲，大厅服务器连接不上，
请稍后重试或联系客服寻求帮助！");
                    }
                    else
                    {
                        UIMgr.GetInstance().GetUIMessageView().Show(@"亲，大厅服务器连接不上，
请稍后重试或联系客服寻求帮助！", () => { MahjongCommonMethod.Instance.InitScene(); });
                    }
                }
                else
                {
                    if (bTryAgain)
                    {
                        MahjongCommonMethod.Instance.RetryConnectSever(2);
                    }
                }
            }
        }

        ///重连回调
        void ConnectAgain()
        {

            if (NetworkMgr.Instance.ProxyEnable == 1)
            {

                proxyIndex++;
                if (proxyIndex > NetworkMgr.Instance.ProxyInfoList.Count)
                {
                    proxyIndex = 1;
                }

            }
            Connect();
        }

        /// <summary>
        /// 当连接服务器成功
        /// </summary>
        void OnConnectServerOK()
        {
            Connected = true;
            DisconnectFlag = false;
            //发送保持连接
            MahjongLobby_AH.Network.NetworkMgr.Instance.KeepGameServerAlive();

            //如果玩家从游戏断线到大厅，第一次认证请求失败，再次发送认证请求
            if (MahjongCommonMethod.Instance.isLobbySendAuthenReq && MahjongCommonMethod.Instance.iUserid != 0)
            {
                GameData gd = GameData.Instance;
                MahjongCommonMethod.Instance.isLobbySendAuthenReq = false;
                gd.WXLoginPanelData.isPanelShow = false;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();
                gd.LobbyMainPanelData.isPanelShow = true;
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                //发送第三种方式认证请求
                Network.Message.NetMsg.ClientAuthenReq msg = new Network.Message.NetMsg.ClientAuthenReq();
                msg.wVer = LobbyContants.SeverVersion;
                msg.iAuthenType = 3;
                MahjongCommonMethod.iAutnehType = 3;
                msg.szToken = MahjongCommonMethod.Instance.accessToken;
                msg.szDui = SystemInfo.deviceUniqueIdentifier;
                msg.szIp = MahjongCommonMethod.PlayerIp;
                msg.iUserId = MahjongCommonMethod.Instance.iUserid;
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
                msg.iRegistSource = LobbyContants.iChannelVersion;
                msg.szRegistMac = MahjongCommonMethod.Instance.MacId;
                msg.REGISTRATION_ID = SDKManager.Instance.GetRegistID();
                NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);
                return;
            }
            else
            {
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }


            //如果玩家是大厅掉线，则直接重新发送认证请求
            if (MahjongCommonMethod.isAuthenSuccessInLobby)
            {
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");
                MahjongCommonMethod.Instance.AgainLogin(0);
                return;
            }

            if (!MahjongCommonMethod.isLoginOut)
            {
                if (MahjongCommonMethod.Instance && !MahjongCommonMethod.isGameToLobby)
                {
                    string url = LobbyContants.MAJONG_PORT_URL + "LoginBulletin.x";
                    if (SDKManager.Instance.IOSCheckStaus == 1)
                    {
                        url = LobbyContants.MAJONG_PORT_URL_T + "LoginBulletin.x";
                    }
                    Dictionary<string, string> value = new Dictionary<string, string>();
                    value.Add("cityId", PlayerPrefs.GetInt("CityId" + GameData.Instance.PlayerNodeDef.iUserId).ToString());
                    value.Add("countyId", PlayerPrefs.GetInt("CountyId" + GameData.Instance.PlayerNodeDef.iUserId).ToString());

                    //连接成功之后，请求登录公告内容,断线重入或者从游戏内返回，不请求登录公告
                    if (MahjongCommonMethod.Instance && LobbyBulletin.Instance && LobbyContants.SeverType != "182")
                    {
                        LobbyBulletin.Instance.Status = 1;
                        MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, LobbyBulletin.Instance.GetBulletinData, "LoginBulletinData");
                    }
                    else
                    {
                        GameData gd = GameData.Instance;
                        gd.WXLoginPanelData.isPanelShow = true;
                        SystemMgr.Instance.WXLoginSystem.UpdateShow();
                    }
                }
            }
            else
            {
                MahjongCommonMethod.isLoginOut = false;
                GameData gd = GameData.Instance;
                gd.WXLoginPanelData.isPanelShow = true;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();
            }
        }
        #endregion 私有成员方法

        #region 事件处理

        /// <summary>
        /// 当收到连接结果
        /// </summary>
        /// <param name='result'>结果</param>
        void OnConnectResult(int result)
        {
            Debug.LogWarning("大厅服务器链接结果:" + result);
            if (result == 0)
            {
                OnConnectServerOK();
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
            Debug.Log("大厅服务器主动断开:" + iCloseByServer);
            Connected = false;
            MahjongCommonMethod.Instance.DelayCheckNeteork(CheckMetWOrk);
            if (iCloseByServer == 1)
            {
                NetClient.Disconnect();
            }
            else
            {
                MahjongCommonMethod.Instance.isLobbySendAuthenReq = true;
            }

            //如果是主动断开连接，自动重连
            tryCount = 0;
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在重连大厅服务器，请稍候!");
            Connect();
        }

        void CheckMetWOrk(int status)
        {



            //UIMainView.Instance.disConnect.UpdateShow(2, TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED);
            //if (status > 0)
            //{
            //    UIMainView.Instance.disConnect.UpdateShow(1, TextConstant.NET_MSG_SERVER_UNOPEN);
            //}
            //else
            //{
            //    UIMainView.Instance.disConnect.UpdateShow(2, TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED);
            //}
        }

        void InitLogin()
        {
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("亲，您的网络不给力哦，请检查网络后重试");
                return;
            }

            MahjongCommonMethod.isLoginOut = true;
            Messenger_anhui.Broadcast(MainViewUserInfoPanel.MESSAGE_BACKTOLOGINPANEL);
        }


        void Ok()
        {
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("亲，您的网络不给力哦，请检查网络后重试");
                return;
            }
            Debug.LogWarning("切换后天台重连=================================4");
            MahjongCommonMethod.Instance.InitScene();
        }

        /// <summary>
        /// 点击弹框的确定按钮
        /// </summary>
        void BtnOn()
        {
            Debug.LogWarning("处理断开连接");
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
                case NetMsg.KICK_USER_NOTICE:
                    Debug.Log("踢人通知消息");
                    HandleKickUserNotice(pMsg, len);
                    break;
                case NetMsg.SERVER_DISCONNECT:
                    Debug.Log("服务器断开连接的通知消息");
                    HandleServerDisconnect(pMsg, len);
                    break;
                case NetMsg.BULLETIN_NOTICE:
                    //Debug.Log("大厅公告通知消息");
                    HandleLobbyBulletinNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_AUTHEN_RES:
                    Debug.Log("收到认证回应");
                    HandleAuthenRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CHANGE_USER_INFO_RES:
                    Debug.Log("收到修改用户信息回应");
                    HandleChangeUserInfo(pMsg, len);
                    break;
                case NetMsg.CLIENT_FULL_NAME_RES:
                    Debug.Log("收到实名认证的回应消息");
                    HandleFullNameRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SPREAD_CODE_RES:
                    Debug.Log("收到推广码激活的号回应消息");
                    HandleSpreadCodeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GETEXCHANGE_COIN_RES:
                    Debug.Log("读取兑换金币配置回应回应消息");
                    HandleGetExchangeCoinRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SPREAD_GIFT_RES:
                    Debug.Log("收到推广码礼包的回应消息");
                    HandleSpreadGiftBag(pMsg, len);
                    break;
                //case NetMsg.CLIENT_ASK_UNBIND_PROXY_RES:
                //    Debug.Log("申请解绑代理回应消息");
                //    HandleAskUnbindProxyRes(pMsg, len);
                //    break;
                //case NetMsg.CLIENT_BIND_PROXY_RES:
                //    Debug.Log("绑定代理回应消息");
                //    HandleBindProxy(pMsg, len);
                //    break;
                case NetMsg.CLIENT_MESSAGE_OPERATE_RES:
                    Debug.Log("信息操作回应消息");
                    HandleMessageOperate(pMsg, len);
                    break;
                case NetMsg.CLIENT_MESSAGE_NOTICE:
                    Debug.Log("信息通知消息");
                    HandleMessageNotice(pMsg, len);
                    break;
                //case NetMsg.CLIENT_PROXY_INFO_RES:
                //    Debug.Log("代理信息回应消息");
                //    HandleProxyInfo(pMsg, len);
                //    break;
                case NetMsg.CLIENT_CITY_COUNTY_RES:
                    Debug.Log("选择市县回应消息");
                    HandleCityCountyRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_ROOM_RES:
                    Debug.Log("开房回应消息");
                    HandleOpenRoomRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_ROOM_INFO_RES:
                    Debug.Log("开房信息回应消息");
                    HandleOpenRoomInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_ROOM_INFO_NOTICE:
                    Debug.Log("开房信息通知消息");
                    HandleOpenRoomInfoNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_MY_ROOM_INFO_RES:
                    Debug.Log("我的开房信息回应消息");
                    HandleMyRoomInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GAME_SERVER_INFO_RES:
                    Debug.Log("游戏服务器信息回应消息");
                    HandleGameServerInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_COMPLIMENT_RES:
                    Debug.Log("点赞回应消息");
                    HandleComplimentRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_HOLIDAY_RES:
                    //Debug.Log("节日信息回应消息");
                    HandleHolidayRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_HOLIDAY_GIFT_RES:
                    //Debug.Log("领取节日活动奖励回应消息");
                    HandleHolidayGiftRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_FREE_TIME_RES:
                    //Debug.Log("免费时间信息回应消息");
                    HandleFreeTimeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SHARE_RES:
                    //Debug.Log("分享活动信息回应消息");
                    HandleSharedRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SHARE_USER_RES:
                    //Debug.Log("分享活动用户信息回应消息");
                    HandleSharedUserRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SHARE_SUCCESS_RES:
                    //Debug.Log("分享成功回应消息");
                    HandleSharedSuccessRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_USE_ACTIVE_CODE_RES:
                    //Debug.Log("使用激活码回应消息");
                    HandleUseActiveCodeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_SPREADER_INFO_RES:
                    //Debug.Log("推广员信息回应消息");
                    HandleSpreaderInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CREATE_ORDER_RES:
                    Debug.Log("创建订单回应消息");
                    HandleCreateOrderRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CHARGE_RES:
                    Debug.Log("充值请求的回应消息");
                    HandleChargeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_USER_CHARGE_RES:
                    Debug.Log("充值信息回应消息");
                    HandleUserChareRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_ROOM_NUM_SERVER_TABLE_RES:
                    Debug.Log("房间号的服务器和桌信息回应消息");
                    HandleRoomNumSeverTableRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CARD_PAY_NOTICE:
                    Debug.Log("代开房支付房卡通知消息");
                    HandleCardPayNotice(pMsg, len);
                    break;

                case NetMsg.CLIENT_LOTTERY_COUNT_RES:
                    Debug.Log("增加抽奖次数回应");
                    HandleLotteryCountRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_LOTTERY_RES:
                    Debug.Log("抽奖回应消息");
                    HandleLotteryRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_USER_LOTTERY_RES:
                    Debug.Log("用户抽奖活动信息回应消息");
                    HandleUserLotteryRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CREATE_PARLOR_RES:
                    Debug.Log("创建馆的回应消息");
                    HandleCreatParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_DISMISS_PARLOR_RES:
                    Debug.Log("解散馆的回应消息");
                    HandleDismissParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CHANGE_PARLOR_BULLETIN_INFO_RES:
                    Debug.Log("修改麻将馆公告信息回应消息");
                    HandleChangeParlorBullrtinInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CHANGE_PARLOR_CONTACT_INFO_RES:
                    Debug.Log("修改麻将馆联系信息回应消息");
                    HandleChangeParlorContactInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_JOIN_PARLOR_RES:
                    Debug.Log("申请 加入麻将馆回应消息");
                    HandleJoinParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_LEAVE_PARLOR_RES:
                    Debug.Log("申请 退出麻将馆回应消息");
                    HandleLevelParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_INVITE_PARLOR_RES:
                    Debug.Log("邀请用户进入麻将馆回应消息");
                    HandleInvitParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_KICK_PARLOR_RES:
                    Debug.Log("踢用户出麻将馆回应消息");
                    HandleKickParlorRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GETPARLORINFO_RES:
                    Debug.Log("获取麻将馆信息回应消息");
                    HandleGetParlorInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_PARLOR_CERT_RES:
                    Debug.Log("玩家是否有开办麻将馆资格的回应消息");
                    HandleCreatParlorCertRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_BESPEAK_RES:
                    Debug.Log("用户占座回应消息");
                    HandleClientUserBespeakRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GETUSERINFO_RES:
                    Debug.Log("获取用户信息回应消息");
                    HandleGetUserInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GET_TABLE_USERID_RES:
                    Debug.Log("桌上玩家信息回应");
                    HandleGetTableUserIdRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GET_TABLE_USER_INFO_RES:
                    Debug.Log("获取用户信息回应消息");
                    HandleGetTableUserInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_RED_NUM_RES:
                    //Debug.Log("所有红包数量回应消息");
                    HandleClientRedNumRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OPEN_RECEIVE_RED_RES:
                    Debug.Log("领取红包回应消息");
                    HandleClientReceiveRedRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OBTAIN_RED_NOTICE:
                    Debug.Log("通知用户获得一个红包消息");
                    HandleClientObtainRedNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_BOSSSCORE_TO_COIN_RES:
                    Debug.Log("业绩兑换金币回应消息");
                    HandleClientBossScoreToCoinRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_OBTAIN_RECEIVE_RED_NOTICE:
                    Debug.Log("通知用户获得一个待领状态红包消息");
                    HandleClientObtainReceiveRedNotice(pMsg, len);
                    break;
                case NetMsg.CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_RES:
                    Debug.Log("取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息");
                    HandleCAncelApplyOrJudgeApplyTooRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_REFRESH_USER_RES:
                    Debug.Log("刷新用户信息回应消息");
                    HandleClientRefreshUserRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_GET_PARLOR_TABLEINFO_RES:
                    Debug.Log("获取麻将馆内某一页的桌信息回应");
                    HandleClientGetParlorTableInfoRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_RP17_TYPE_RES:
                    Debug.Log("红包状态回应消息");
                    HandleClientRp17TypeRes(pMsg, len);
                    break;
                case NetMsg.CLIENT_CANCLE_BESPEAK_RES:
                    Debug.Log("用户取消占座回应消息");
                    HandleClientBespeakRes(pMsg, len);
                    break;
                default:
                    Debug.LogWarning("未处理的消息名称：" + cMsgType);
                    break;
            }
        }

        #endregion 事件处理

        #region 回应调用方法

        /// <summary>
        /// 踢人通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleKickUserNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.KickUserNotice msg = new NetMsg.KickUserNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("踢人通知消息长度不一致，ioffset:" + ioffset + ",len:" + len);
            }

            if (msg.iReason == 1)
            {
                //如果玩家被后面的人重复登陆踢掉
                if (MahjongLobby_AH.Network.NetworkMgr.Instance)
                {
                    //Debug.LogWarning("===============================1");
                    Network.NetworkMgr.Instance.LobbyServer.Disconnect();
                }

                UIMgr.GetInstance().GetUIMessageView().Show("您的账号在其他地方请求登录，账号异常请重新登录", InitLogin);
            }
            else if (msg.iReason == 2)
            {
                UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_MSG_SERVER_UNOPEN, InitLogin);
            }
            else
            {
                UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED, InitLogin);
            }

        }



        public void HandleServerDisconnect(byte[] pMsg, int len)
        {
            DEBUG.NetworkServer(NetMsg.SERVER_DISCONNECT, pMsg);
            int iOffset = 0;
            NetMsg.ServerDisconnect msg = new NetMsg.ServerDisconnect();
            iOffset = msg.parseBytes(pMsg, iOffset);
            if (iOffset != len)
            {
                Debug.LogError("消息长度不一致");
                return;
            }
            UIMainView.Instance.disConnect.UpdateShow(1);
            //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED, Ok);
        }

        /// <summary>
        /// 处理大厅公告通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleLobbyBulletinNotice(byte[] pMsg, int len)
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            NetMsg.BulletinNotice msg = new NetMsg.BulletinNotice();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("公告通告长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (!lmpd.isPanelShow)
            {
                return;
            }
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }
            //如果公告不做限制
            if (msg.iBulletinRange == 1 || msg.iBulletinRange == 4 || msg.iBulletinRange == 0)
            {
                //对所有城市都可见
                if (msg.iCityId == 0 || msg.iCityId == sapd.iCityId)
                {
                    //玩家选择的县城可见
                    if (msg.iCountyId == 0 || msg.iCountyId == sapd.iCountyId)
                    {
                        lmpd.lsBulletinNotice.Add(msg.szBulletinContent);
                        //更新显示跑马灯内容            
                        UIMainView.Instance.LobbyPanel.MoveBulltein(lmpd.lsBulletinNotice[0]);
                    }
                }
            }
        }

        /// <summary>
        /// 处理玩家认证回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleAuthenRes(byte[] pMsg, int len)
        {
            //修改玩家的返回大厅认证成功的标志位            
            NetMsg.ClientAuthenRes msg = new NetMsg.ClientAuthenRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("认证登录回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("认证回应错误编号：" + msg.iError + " :" + Err_ClientAuthenRes.Err(msg.iError));

                //重复登陆,延迟3s之后重新发送认证请求
                if (msg.iError == 11)
                {
                    //说明玩家是大厅掉线之后，重复登录
                    if (MahjongCommonMethod.isAuthenSuccessInLobby)
                    {
                        //延迟三秒，重新认证请求
                        MahjongCommonMethod.Instance.AgainLogin(3f);
                    }
                    else
                    {
                        MahjongCommonMethod.Instance.AgainLogin(0f);
                    }
                }
                else
                {
                    //如果是令牌错误，重新拉起认证
                    if (msg.iError == 21 || msg.iError == 12)
                    {
                        if (PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType +
                            GameData.Instance.PlayerNodeDef.iUserId))
                        {
                            PlayerPrefs.DeleteKey(SDKManager.Instance.iUserId_iAuthType_ServerType);
                            Messenger_anhui.Broadcast(MainViewWXLoginPanel.MESSAGE_WXLOGINAUTHBTN);
                        }
                    }
                    else
                    {
                        switch (msg.iError)
                        {
                            case 23: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号已经在其他设备登录，重入失败！", LoginAccount); break;
                            case 2:
                            case 3:
                            case 5:
                            case 6:
                            case 7:
                            case 13:
                            case 14:
                            case 17:
                            case 18:
                            case 16: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号登录异常，请重新登录！", LoginAccount); break;
                            case 8: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号被禁用，请联系客服！", LoginAccount); break;
                            case 15: UIMgr.GetInstance().GetUIMessageView().Show("您的版本号不匹配，请下载最新版本！", LoginAccount); break;
                            case 20: UIMgr.GetInstance().GetUIMessageView().Show("游戏服务器不可用，请联系客服！", LoginAccount); break;
                            default: UIMgr.GetInstance().GetUIMessageView().Show("您的帐号登录异常，请重新登录！", LoginAccount); break;
                        }
                    }
                }
                return;
            }

            isSuccessLogin = true;

            MahjongCommonMethod.Instance.isStartInit_Lobby = false;

            PlayerPrefs.SetInt("userId", msg.clientUser.iUserId);

            RuningToBackGround.Instance.isFocus = false;
            RuningToBackGround.Instance.isPause = false;
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            GameData.Instance.PlayerNodeDef.byNewUser = msg.byNewUser;
            if (GameData.Instance.PlayerNodeDef.byNewUser == 1)
            {
                UIMainView.Instance.LobbyPanel.isDengluOnce = true;
            }

            GameData.Instance.WXLoginPanelData.isClickLogin = false;
            #region 用户节点数据保存

            //Debug.LogError("玩家积分数据:" + msg.clientUser.fParlorScore);
            PlayerNodeDef pnd = new PlayerNodeDef(msg.clientUser);
            pnd.iAuthenType = msg.iAuthenType;

            GameData.Instance.PlayerNodeDef = pnd;

            pnd.userDef = msg.clientUser;//复制构造器构造
            GameData.Instance.PlayerNodeDef.isAuthSuccess = true;
            MahjongCommonMethod.Instance.accessToken = msg.clientUser.szAccessToken;
            MahjongCommonMethod.Instance.iUserid = msg.clientUser.iUserId;
            //SDKManager.Instance.InitBuglySDK(MahjongCommonMethod.Instance.iUserid.ToString());
            //NetMsg.ClientUserChargeReq chargMsg = new NetMsg.ClientUserChargeReq();//用户充值信息请求
            //chargMsg.iUserId = msg.clientUser.iUserId;
            //NetworkMgr.Instance.LobbyServer.SendUserChargeReq(chargMsg);
            if (!MahjongCommonMethod.isGameToLobby)
            {
                NetMsg.ClientGetExchangeCoinReq msg_getExchange = new NetMsg.ClientGetExchangeCoinReq();
                msg_getExchange.iUserId = msg.clientUser.iUserId;
                NetworkMgr.Instance.LobbyServer.SendGetExchangeCoinReq(msg_getExchange);
            }
            //  UIMainView.Instance.LobbyPanel.GetVoucherData();
            #endregion  用户节点数据保存                              

            if (GameData.Instance.PlayerNodeDef.userDef.szAccessToken == null)
            {
                Debug.LogError("accessToken为空 ");
                return;
            }


            //Debug.LogError("msg.clientUser.byUserSource：" + msg.clientUser.byUserSource + ",msg.iAuthenType：" + msg.iAuthenType);
            if (msg.clientUser.byUserSource == 1)
            {
                pnd.szNickname = "游客" + msg.clientUser.iUserId;
                GameData.Instance.WXLoginPanelData.isPanelShow = false;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();

            }
            else
            {
                if (msg.iAuthenType == 3)
                {
                    SDKManager.Instance.LoginSuccess();
                }
                else
                {
                    PlayerPrefs.SetString(SDKManager.Instance.iUserId_iAuthType_ServerType + GameData.Instance.PlayerNodeDef.iUserId, msg.clientUser.iUserId + "_" + msg.iAuthenType + "_" + LobbyContants.SeverType);
                    PlayerPrefs.SetString(SDKManager.Instance.szrefresh_token + GameData.Instance.PlayerNodeDef.iUserId, pnd.userDef.szRefreshToken);
                    Debug.LogWarning(pnd.userDef.szRefreshToken);
                    SDKManager.Instance.GetUserInfo(pnd.userDef.szRefreshToken, pnd.szOpenid);
                }
            }

            GameData gd = GameData.Instance;
            int num = GameData.Instance.PlayerNodeDef.userDef.iLastCoin3Tim + 172800 - (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.LoginSuccess);

            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            //如果玩家是新玩家
            if (msg.byNewUser == 1 && SDKManager.Instance.CheckStatus == 0)
            {
                PlayerPrefs.DeleteAll();
                GameData.Instance.SelectAreaPanelData.iOpenStatus = 1;
                GameData.Instance.SelectAreaPanelData.isPanelShow = true;
                SystemMgr.Instance.SelectAreaSystem.UpdateShow();
                gd.LobbyMainPanelData.isPanelShow = false;
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                //如果是新玩家会添加一条固定消息
                PlayerPrefs.SetFloat("RegistTime" + GameData.Instance.PlayerNodeDef.iUserId, MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now));
                //如果是新玩家，实名注册，分享礼包的红点
                PlayerPrefs.SetFloat(GameData.RedPoint.RealName.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 1);
                PlayerPrefs.SetFloat(GameData.RedPoint.ShareBag.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 1);
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                return;
            }
            else
            {
                sapd.iCityId = msg.clientUser.iCityId;
                sapd.iCountyId = msg.clientUser.iCountyId;
                GameData.Instance.LobbyMainPanelData.isPanelShow = true;
                if (msg.clientUser.iCityId == 0 || msg.clientUser.iCountyId == 0)
                {
                    NetMsg.ClientCityCountyReq ccreq = new NetMsg.ClientCityCountyReq();
                    ccreq.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    ccreq.iCityId = 550;
                    ccreq.iCountyId = 341125;
                    NetworkMgr.Instance.LobbyServer.SendCityCountyReq(ccreq);
                }
            }


            //在这里请求，玩家的推广员的数量
            if (PlayerPrefs.GetFloat(NewPlayerGuide.Guide.Promote.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 0)
            {
                string url = LobbyContants.MAJONG_PORT_URL + "userInfo.x";
                if (SDKManager.Instance.IOSCheckStaus == 1)
                {
                    url = LobbyContants.MAJONG_PORT_URL_T + "userInfo.x";
                }
                Dictionary<string, string> value = new Dictionary<string, string>();
                value.Add("uid", msg.clientUser.iUserId.ToString());
                MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, NewPlayerGuide.Instance.GetProductCount, "Product");
            }
            //在这里向服务器请求，玩家当前是否有房间，如果有显示返回房间
            NetMsg.ClientMyRoomInfoReq myroominfo = new NetMsg.ClientMyRoomInfoReq();
            myroominfo.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            NetworkMgr.Instance.LobbyServer.SendMyRoomInfoReq(myroominfo);

            //获取战绩数据          
            HistroyGradePanelData hgpd = gd.HistroyGradePanelData;
            hgpd.HistroyReq_Web(0);

            //获取消息数据
            SystemMgr.Instance.LobbyMainSystem.MessageReq(0);

            //向网站请求麻将馆红包的信息
            GameData.Instance.ParlorShowPanelData.FromWebGetParlorRedBagData();

            //获取上次查询的时间            
            hgpd.timer = PlayerPrefs.GetInt("time_h" + GameData.Instance.PlayerNodeDef.iUserId);
            gd.PlayerMessagePanelData.timer = PlayerPrefs.GetInt("time_m" + GameData.Instance.PlayerNodeDef.iUserId);

            //读取配置文件
            MahjongCommonMethod.Instance.ReadJson();

            //删除显示更新界面的预置体
            if (CheckUpdateManager.Instance)
            {
                CheckUpdateManager.Instance.Destroy_Self();
            }

            //获取红包数量
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_REDPAGE);

            SystemMgr.Instance.LobbyMainSystem.UpdateShow();

            //如果玩家是从麻将馆进入房间，退出到大厅之后，直接进入麻将馆            

            if (MahjongCommonMethod.Instance.iFromParlorInGame > 0)
            {
                UIMainView.Instance.LobbyPanel.BtnOpenParlor();
            }
            else
            {
                //获取玩家的申请馆的id
                GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 3);
            }

            if (MahjongCommonMethod.Instance.GameLaterDo_ != null)
            {
                MahjongCommonMethod.Instance.GameLaterDo_();
                MahjongCommonMethod.Instance.GameLaterDo_ = null;
            }
            //UIMainView.Instance.LobbyPanel.CloseHealthyPrompt();

            ////如果是玩家正常登录，向网页发送手机型号信息
            //if (msg.iAuthenType <= 2)
            //{
            //    MahjongCommonMethod.Instance.SendMobileToWeb(0);
            //}          

            if (MahjongCommonMethod.isGameToLobby)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_GAME_GENERAL);
                sw.Stop();
                Debug.Log("预加载游戏场景时间：====================" + sw.ElapsedMilliseconds);
            }
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

        /// <summary>
        /// 收到修改用户信息回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>

        void HandleChangeUserInfo(byte[] pMsg, int len)
        {
            NetMsg.ClientChangeUserInfoRes msg = new Message.NetMsg.ClientChangeUserInfoRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("修改用户信息回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("修改用户信息回应" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            UIMainView.Instance.LobbyPanel.UpdateShow();
        }
        /// <summary>
        /// 实名认证回应消息处理
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleFullNameRes(byte[] pMsg, int len)
        {
            NetMsg.ClientFullNameRes msg = new NetMsg.ClientFullNameRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("实名认证的回应消息长度不一致" + ioffset + ",len:" + len);
            }

            if (msg.iError != 0)
            {
                Debug.LogError("实名认证回应错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
            }

            //处理关闭实名认证面板
            GameData.Instance.RealNameApprovePanelData.PanelShow = false;
            SystemMgr.Instance.RealNameApproveSystem.UpdateShow();
            MahjongCommonMethod.Instance.ShowRemindFrame("实名注册成功");

            //更新大厅面板
            GameData.Instance.PlayerNodeDef.byNameAuthen = 1;
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();

            //掉金币的这个消息去除
            //if (msg.iFreeCard > 0)
            //{
            //    GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
            //    UIMainView.Instance.LobbyPanel.BtnNewPlayerBag(msg.iFreeCard);
            //}
        }

        /// <summary>
        /// 处理使用推广码的回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSpreadCodeRes(byte[] pMsg, int len)
        {
            NetMsg.ClientSpreadCodeRes msg = new NetMsg.ClientSpreadCodeRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("使用推广码的回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("使用推广码的回应的错误编号：" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("请输入正确的推广码");
                return;
            }
            //GameData.Instance.PlayerNodeDef.iFreeCard += msg.iFreeCard;
            //处理玩家绑定成功之后，获取的礼包效果
            if (msg.iFreeCard > 0)
            {
                GameData.Instance.GetGiftSpreadBagPanelData.PanelShow = false;
                SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();

                GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
                UIMainView.Instance.LobbyPanel.PlayGold_Particle(msg.iFreeCard);
                GameData.Instance.PlayerNodeDef.iSpreadGiftTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
                //MahjongCommonMethod.Instance.ShowRemindFrame("使用推广码成功");
            }
        }
        /// <summary>
        /// 兑换金币配置回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGetExchangeCoinRes(byte[] pMsg, int len)
        {
            NetMsg.ClientGetExchangeCoinRes msg = new NetMsg.ClientGetExchangeCoinRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (msg.iError != 0)
            {
                Debug.LogError("读取兑换金币配置回应的错误编号：" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                //  return;
            }
            //Debug.LogError("结构体数量：" + msg.iChargeNum + ",iExchangeNum:" + msg.iExchangeNum);
            MahjongCommonMethod.Instance._configcharge = msg;
            for (int i = 0; i < msg.iChargeNum; i++)
            {
                NetMsg.Charge msg1 = new NetMsg.Charge();
                ioffset = msg1.parseBytes(pMsg, ioffset);
                //Debug.LogWarning("读取充值配置ChargeId" + msg1.ChargeId);
                if (!MahjongCommonMethod.Instance._dicCharge.ContainsKey(msg1.ChargeId))
                    MahjongCommonMethod.Instance._dicCharge.Add(msg1.ChargeId, msg1);
            }
            //添加业绩兑换业绩数值
            MahjongCommonMethod.Instance.ExChange = new List<NetMsg.Exchange>();
            for (int i = 0; i < msg.iExchangeNum; i++)
            {
                NetMsg.Exchange msg2 = new NetMsg.Exchange();
                ioffset = msg2.parseBytes(pMsg, ioffset);
                //Debug.LogError("金币：" + msg2.iCoin + ",赠币:" + msg2.iBindCoin + "，消耗数量:" + msg2.iAsset);
                if (msg2.iType == 1)
                {
                    MahjongCommonMethod.Instance.ExChange.Add(msg2);
                }
                else if (msg2.iType == 2)
                {
                    if (!MahjongCommonMethod.Instance._dicExchage2.ContainsKey(msg2.iExchangeId))
                        MahjongCommonMethod.Instance._dicExchage2.Add(msg2.iExchangeId, msg2);
                }
                else if (msg2.iType == 3)
                {
                    if (!MahjongCommonMethod.Instance._dicExchage3.ContainsKey(msg2.iExchangeId))
                        MahjongCommonMethod.Instance._dicExchage3.Add(msg2.iExchangeId, msg2);
                }
            }
            if (ioffset != len)
            {
                Debug.LogError("读取兑换金币配置回应消息长度" + ioffset + ",len:" + len);
                return;
            }
        }
        /// <summary>
        /// 推广礼包回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSpreadGiftBag(byte[] pMsg, int len)
        {
            NetMsg.ClientSpreadGiftRes msg = new NetMsg.ClientSpreadGiftRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("获取推广码礼包的回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("获取推广码礼包的回应的错误编号：" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //显示玩家信息的红点,存入注册表
            PlayerPrefs.SetInt("userInfoRed" + GameData.Instance.PlayerNodeDef.iUserId, 1);   //玩家信息红点标志为1，点击之后为2     

            //UIMgr.GetInstance().GetUIMessageView().Show("恭喜您获得推广大礼包", CloseGift);
            GameData.Instance.GetGiftSpreadBagPanelData.PanelShow = false;
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
            GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
            UIMainView.Instance.LobbyPanel.PlayGold_Particle(msg.iFreeCard);
            GameData.Instance.PlayerNodeDef.iSpreadGiftTime = msg.iSpreadGiftTime;
            //GameData.Instance.PlayerNodeDef.iFreeCard += msg.iFreeCard;
            //SystemMgr.Instance.LobbyMainSystem.UpdateShow();  

        }

        void CloseGift()
        {
            GameData.Instance.GetGiftSpreadBagPanelData.PanelShow = false;
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
        }

        /// <summary>
        /// 处理玩家解绑代理回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleAskUnbindProxyRes(byte[] pMsg, int len)
        {
            NetMsg.ClientAskUnbindProxyRes msg = new NetMsg.ClientAskUnbindProxyRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("玩家解绑代理回应消息产度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("玩家解绑代理回应的错误编号:" + msg.iError);
                if (msg.iError == 5)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("您的申请已经提交，请等待您的代理的回复");
                }

                if (msg.iError == 6)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("你申请的玩家已经不是代理");
                }

                return;
            }
            GameData.Instance.ProductAgencyPanelData.iProxyId = 0;
            GameData.Instance.ProductAgencyPanelData.PanelShow = false;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
            MahjongCommonMethod.Instance.ShowRemindFrame("您的申请提交成功，请等待您的代理的回复");
            UIMainView.Instance.LobbyPanel.RedPoint[1].gameObject.SetActive(true);
        }

        /// <summary>
        /// 处理信息通知
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleMessageNotice(byte[] pMsg, int len)
        {
            NetMsg.ClientMessageNotice msg = new NetMsg.ClientMessageNotice();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("信息通知消息长度不一致" + ioffset + ",len:" + len);
            }

            PlayerNodeDef pnf = GameData.Instance.PlayerNodeDef;
            //如果玩家的申请被馆主同意，修改用户节点信息
            if (msg.byType == 2)
            {
                for (int i = 0; i < pnf.iaJoinParlorId.Length; i++)
                {
                    if (pnf.iaJoinParlorId[i] == 0)
                    {
                        pnf.iaJoinParlorId[i] = 1;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 处理玩家请求代理信息回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleProxyInfo(byte[] pMsg, int len)
        {
            NetMsg.ClientProxyInfoRes msg = new NetMsg.ClientProxyInfoRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("代理信息回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("代理信息回应消息错误编号:" + msg.iError);
                return;
            }

            Debug.LogError("代理微信号：" + msg.szProxyWx);
            GameData.Instance.ProductAgencyPanelData.szNickname = msg.szNickname;
            GameData.Instance.ProductAgencyPanelData.szHeadimgurl = msg.szHeadimgurl;
            GameData.Instance.ProductAgencyPanelData.szProxyComment = msg.szProxyComment;
            GameData.Instance.ProductAgencyPanelData.wx = msg.szProxyWx;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
        }

        /// <summary>
        /// 选择市县回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCityCountyRes(byte[] pMsg, int len)
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            // sapd.isPanelShow = false;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
            GameData.Instance.LobbyMainPanelData.isPanelShow = true;
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            NetMsg.ClientCityCountyRes msg = new NetMsg.ClientCityCountyRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("选择市县回应消息长度不一致：" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError == 1) return;
            if (msg.iError == 2)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("绑定区域失败，请稍候重试！");
                sapd.iCityId = 550;
                sapd.iCountyId = 341125;
                MahjongCommonMethod.Instance.ReadJson();
                return;
            }

         //   Debug.LogError("选择市县回应消息：" + msg.iCityId + "," + msg.iCountyId);

            //为区域id赋值
            sapd.iCityId = msg.iCityId;
            sapd.iCountyId = msg.iCountyId;
            MahjongCommonMethod.Instance.ReadJson();
            //MahjongCommonMethod.Instance.ShowRemindFrame("恭喜您成功绑定所在区域");
            //处理玩家如果是新玩家，会获取房卡礼包
            if (UIMainView.Instance.LobbyPanel.isDengluOnce)
            {
                UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(4, 1, 2, "新手红包", RedPageShowPanel.NowState.Lobby);
                UIMainView.Instance.LobbyPanel.isDengluOnce = false;
            }
            if (GameData.Instance.PlayerNodeDef.byNewUser == 1)
            {
                //Debug.LogError("处理玩家如果是新玩家，会获取房卡礼包");
                //UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(4, 1, 2, "新手红包", RedPageShowPanel.NowState.Lobby);
            }
            else
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("恭喜您选择区域成功");
            }
        }
        /// <summary>
        /// 信息操作回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleMessageOperate(byte[] pMsg, int len)
        {
            NetMsg.ClientMessageOperateRes msg = new NetMsg.ClientMessageOperateRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("信息操作回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("信息操作回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            //如果是馆主同意玩家申请
            if (msg.cMessageType == 3)
            {
                //为馆主的成员数量+1
                GameData.Instance.ParlorShowPanelData.parlorInfoDef[0].iMemberNum += 1;
                UIMainView.Instance.ParlorShowPanel.UpdateMemberCount(GameData.Instance.ParlorShowPanelData.parlorInfoDef[0].iMemberNum);
                SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(GameData.Instance.ParlorShowPanelData.parlorInfoDef[0]);
            }
        }


        /// <summary>
        /// 绑定代理回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleBindProxy(byte[] pMsg, int len)
        {
            NetMsg.ClientBindProxyRes msg = new NetMsg.ClientBindProxyRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("绑定代理回应消息消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("绑定代理回应消息错误编号:" + msg.iError);

                //已经有代理
                if (msg.iError == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您已经有了上级代理");
                }
                //绑定的代理不是代理
                else if (msg.iError == 4)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您绑定的玩家还不是代理，无法绑定");
                }
                else if (msg.iError == 2 || msg.iError == 3 || msg.iError == 5)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您输入的ID不存在！");
                }

                return;
            }

            //绑定代理之后处理界面显示问题
            PlayerNodeDef pdd = GameData.Instance.PlayerNodeDef;
            //pdd.iProxyId = msg.iProxyId;

            Debug.LogError("代理微信号：" + msg.szProxyWx);
            GameData.Instance.ProductAgencyPanelData.PanelShow = false;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
            //弹出公用弹框
            UIMgr.GetInstance().GetUIMessageView().Show("您和\"" + msg.szProxyNickname + "\"(代理)成功绑定，建立服务关系，您可以联系代理购买金币！");

            //成功绑定代理
            if (PlayerPrefs.GetFloat(GameData.RedPoint.SuccessBindDaili.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 0)
            {
                PlayerPrefs.SetFloat(GameData.RedPoint.SuccessBindDaili.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 1);
            }
            UIMainView.Instance.LobbyPanel.RedPoint[1].gameObject.SetActive(true);
        }

        /// <summary>
        /// 开房回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleOpenRoomRes(byte[] pMsg, int len)
        {
            NetMsg.ClientOpenRoomRes msg = new NetMsg.ClientOpenRoomRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("ioffset:" + ioffset + ",len:" + len);
                Debug.LogError("开房回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("开房回应消息错误编号:" + msg.iError);
                switch (msg.iError)
                {
                    //开房达到上限
                    case 11:
                    case 12: UIMgr.GetInstance().GetUIMessageView().Show("开房失败，请稍后重试！"); break;
                    case 15: UIMgr.GetInstance().GetUIMessageView().Show("您的开房次数达到上限，无法创建房间"); break;
                    case 13: UIMgr.GetInstance().GetUIMessageView().Show("您的金币数量不足，无法创建房间", OpenPay, () => { }, 4, 0); break;
                    case 14: UIMgr.GetInstance().GetUIMessageView().Show("您的赞数量不足，无法创建房间"); break;
                    case 16: UIMgr.GetInstance().GetUIMessageView().Show("您的掉线率过高，无法创建房间"); break;
                    case 17: UIMgr.GetInstance().GetUIMessageView().Show("您的出牌速过慢，无法创建房间"); break;
                    case 23: UIMgr.GetInstance().GetUIMessageView().Show("您已在其他房间占座，无法创建房间"); break;
                    default: UIMgr.GetInstance().GetUIMessageView().Show("开房失败，请稍后重试！"); break;
                }
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("uloading");
                return;
            }
            GameData.Instance.CreatRoomMessagePanelData.LastCreatTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
            //保存玩家的开放模式
            MahjongCommonMethod.Instance.cOpenRoomMode = msg.cOpenRoomMode;
            MahjongCommonMethod.Instance.RoomId = String.Format("{0:000000}", msg.iRoomNum);

            //Debug.Log("房间信息:" + msg.iTableNum + ",房间号：" + MahjongCommonMethod.Instance.RoomId + ",服务器编号：" + msg.iServerId
            //    + "，卓编号:" + msg.iTableNum + ",开房类型:" + msg.cOpenRoomMode + "，保存的麻将馆id:" + GameData.Instance.ParlorShowPanelData.iParlorId);

            //普通开放或者是馆内成员开房
            if (msg.cOpenRoomMode == 1 || msg.cOpenRoomMode == 3)
            {
                MahjongCommonMethod.Instance.iTableNum = msg.iTableNum;
                MahjongCommonMethod.Instance.iSeverId = msg.iServerId;
                MahjongCommonMethod.Instance.iPlayingMethod = msg.iPlayingMethod;
                //请求游戏服务器信息
                NetMsg.ClientGameServerInfoReq msgg = new NetMsg.ClientGameServerInfoReq();
                msgg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msgg.iServerId = MahjongCommonMethod.Instance.iSeverId;
                NetworkMgr.Instance.LobbyServer.SendGameSeverInfoReq(msgg);
            }
            //麻将馆馆主开房
            else
            {
                NetMsg.OpenRoomInfo openRoomInfo = new NetMsg.OpenRoomInfo();
                openRoomInfo.iPlayingMethod = msg.iPlayingMethod;
                openRoomInfo.PavilionID = GameData.Instance.ParlorShowPanelData.GetNowMahjongPavilionID();
                openRoomInfo.iServerId = msg.iServerId;
                openRoomInfo.iTableNum = msg.iTableNum;
                openRoomInfo.cOpenRoomStatus = 0;
                openRoomInfo.byColorFlag = msg.byColorFlag;
                openRoomInfo.iOpenRoomTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
                //Debug.Log("openRoomInfo.iOpenRoomTime" + openRoomInfo.iOpenRoomTime + "," + ((int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now)));
                openRoomInfo.iCompliment = msg.iCompliment;
                openRoomInfo.iDisconnectRate = msg.iDisconnectRate;
                openRoomInfo.iDiscardTime = msg.iDiscardTime;
                openRoomInfo.iRoomNum = msg.iRoomNum;
                openRoomInfo.iaUserId = new int[] { 0, 0, 0, 0 };
                openRoomInfo.caOpenRoomParam = msg.caParam;
                openRoomInfo.iBespeakUserId = new int[] { 0, 0, 0, 0 };
                UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo = openRoomInfo;
                UIMainView.Instance.ParlorShowPanel.BtnSendClickTable();
            }
            MahjongCommonMethod.Instance.isCreateRoom = true;

            if (msg.cOpenRoomMode > 1)
            {
                MahjongCommonMethod.Instance.iFromParlorInGame = GameData.Instance.ParlorShowPanelData.iParlorId;
            }
            else
            {
                MahjongCommonMethod.Instance.iFromParlorInGame = 0;
            }

        }

        //购买房卡按钮
        void OpenPay()
        {
            UIMainView.Instance.LobbyPanel.OpenChargePanel();
        }

        /// <summary>
        /// 开房信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleOpenRoomInfoRes(byte[] pMsg, int len)
        {
            GameData gd = GameData.Instance;
            InsteadOpenRoomPanelData iorpd = gd.InsteadOpenRoomPanelData;
            NetMsg.ClientOpenRoomInfoRes msg = new NetMsg.ClientOpenRoomInfoRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            //清空list
            iorpd.OpenRoomInfo_Started = new List<InsteadOpenRoomPanelData.RoomInfo>();
            iorpd.OpenRoomInfo_UnStart = new List<InsteadOpenRoomPanelData.RoomInfo>();

            for (int i = 0; i < msg.iNum; i++)
            {
                NetMsg.OpenRoomInfo msgg = new NetMsg.OpenRoomInfo();
                ioffset = msgg.parseBytes(pMsg, ioffset);
                iorpd.Value(msgg);
                Debug.LogError("status:" + msgg.cOpenRoomStatus);
            }

            if (ioffset != len)
            {
                Debug.LogError("开房信息回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("开房信息回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            Debug.LogWarning("玩家的代开房间个数：" + msg.iNum);

            if (iorpd.isClickInsteadOpenRoom)
            {
                //如果玩家是第一次打开代开房间，则显示代开规则
                if (PlayerPrefs.GetFloat(GameData.RedPoint.FirstOpenInsteadPanel.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 0)
                {
                    iorpd.InsteadRulePanelShow = true;
                    PlayerPrefs.SetFloat(GameData.RedPoint.FirstOpenInsteadPanel.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 1);
                }

                iorpd.PanelShow = true;
                iorpd.isFirstSpwanInsteadRecord = true;
                SystemMgr.Instance.InsteadOpenRoomSystem.UpdateShow();
                Messenger_anhui.Broadcast(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADOPEN);
            }


        }

        /// <summary>
        /// 开房信息通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleOpenRoomInfoNotice(byte[] pMsg, int len)
        {
            GameData gd = GameData.Instance;
            ParlorShowPanelData pspd = gd.ParlorShowPanelData;
            NetMsg.ClientOpenRoomInfoNotice msg = new NetMsg.ClientOpenRoomInfoNotice();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("开房信息回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            ParlorShowPanelData.OpenRoomInfo info = new ParlorShowPanelData.OpenRoomInfo();
            info.iaUserId = msg.iaUserId;
            info.iaBespeakUserId = msg.iaBespeakUserid;
            info.iRoomNum = msg.iRoomNum;
            info.iParlorId = msg.iParlorId;
            info.byOpenRoomStatus = msg.cOpenRoomStatus;
            info.iDissolveByte = msg.iDissolveType;
            pspd.UpdateTabelMessage(info);
        }

        /// <summary>
        /// 我的开房信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleMyRoomInfoRes(byte[] pMsg, int len)
        {
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            NetMsg.ClientMyRoomInfoRes msg = new NetMsg.ClientMyRoomInfoRes();
            int ioffset = 0;
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("我的开房信息回应消息长度不一致" + ioffset + ",len:" + len);
                return;
            }

            Debug.Log("玩家id：" + msg.iUserId + ",房间号：" + msg.iRoomNum + "，错误编号：" + msg.iError + ",预约时间:" + msg.iTimeLeft + ",馆号:" + msg.iParlorId);

            if (msg.iError == 1 || msg.iRoomNum == 0)
            {
                mcm.PlayerRoomStatus = 0;
                mcm.iTableNum = 0;
                mcm.iSeverId = 0;
                mcm.RoomId = "0";
                lmpd.isJoinedRoom = false;
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = false;
                SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            }
            else
            {
                mcm.PlayerRoomStatus = 1;
                mcm.iTableNum = msg.sTableNum;
                mcm.iSeverId = msg.iServerId;

                mcm.RoomId = String.Format("{0:000000}", msg.iRoomNum);

                //如果玩家已经创建过房间或者已经加入过其他房间，不让点击加入房间
                lmpd.isJoinedRoom = true;
                //如果玩家已经有开放信息且是普通房间，直接让玩家返回房间中
                if (msg.iTimeLeft == 0 && (!MahjongCommonMethod.isGameToLobby && !MahjongCommonMethod.isConnectGameSeverFailed)
                    && GameData.Instance.ParlorShowPanelData.iUpdateOrderTimer == 0)
                {
                    Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_RETURNROOM);
                }
                else
                {
                    PlayerPrefs.SetInt("TingOneCard" + GameData.Instance.PlayerNodeDef.iUserId, 0);
                }

                GameData.Instance.ParlorShowPanelData.iUpdateOrderTimer = 0;

                if (msg.iTimeLeft > 0)
                {
                    //时间赋值
                    UIMainView.Instance.LobbyPanel.m_bInGame = true;
                    Debug.LogWarning("显示玩家预约房间倒计时");
                    GameData.Instance.ParlorShowPanelData.isShowPanel = true;
                    GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = true;
                    SystemMgr.Instance.ParlorShowSystem.UpdateShow();
                    UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.StopIeNum();
                    UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.UpdateShow(msg.iTimeLeft);
                }
                else
                {
                    UIMainView.Instance.LobbyPanel.m_bInGame = false;
                    GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = false;
                    SystemMgr.Instance.ParlorShowSystem.UpdateShow();
                }

                MahjongCommonMethod.Instance.iParlorId = msg.iParlorId;
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();

                if (msg.iParlorId > 0)
                {
                    MahjongCommonMethod.Instance.iFromParlorInGame = msg.iParlorId;
                }
                else
                {
                    MahjongCommonMethod.Instance.iFromParlorInGame = 0;
                }
            }
        }

        /// <summary>
        /// 游戏服务器信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGameServerInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientGameServerInfoRes msg = new NetMsg.ClientGameServerInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("游戏服务器信息回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("游戏服务器信息回应消息错误编号:" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("没找到对应的房间");
                //清空加入房间数据
                JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
                jrpd.RoomId = "";
                jrpd.lsRoomId.Clear();
                SystemMgr.Instance.JoinRoomShowSystem.UpdateShow();
                return;
            }

            MahjongCommonMethod.isGameToLobby = false;
            GameData.Instance.JoinRoomShowPanelData.PanelShow = false;
            SystemMgr.Instance.JoinRoomShowSystem.UpdateShow();

            MahjongCommonMethod.Instance.SetParlorUserID();//保存麻将馆

            MahjongCommonMethod.Instance.SeverIp = msg.gameServer.szIp;
            MahjongCommonMethod.Instance.SeverPort = (ushort)msg.gameServer.usPort;

            //断开大厅服务器
            if (NetworkMgr.Instance && Connected)
            {
                NetworkMgr.Instance.LobbyServer.Disconnect();
            }

            //弹出加载示意图
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载房间资源，请稍候...");

            MahjongCommonMethod.Instance.iCityId = GameData.Instance.SelectAreaPanelData.iCityId;
            MahjongCommonMethod.Instance.iCountyId = GameData.Instance.SelectAreaPanelData.iCountyId;

            //清空gc垃圾
            System.GC.Collect();

            SceneManager_anhui.Instance.OpenPointScene(ESCENE.MAHJONG_GAME_GENERAL);

            //跳转场景
            //Scene.SceneManager.Instance.LoadScene(Scene.ESCENE.MAHJONG_GAME_GENERAL);
            MahjongCommonMethod.Instance.isFinshSceneLoad = false;
            MahjongCommonMethod.Instance.serverID = msg.gameServer.iGameServerId.ToString();

            Debug.Log("游戏服务器信息,服务器编号:" + msg.gameServer.iGameServerId + ",ip地址:" + MahjongCommonMethod.Instance.SeverIp + ",端口:" + MahjongCommonMethod.Instance.SeverPort);
            //清空对象池中的对象
            PoolManager.Clear();
        }

        /// <summary>
        /// 点赞回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleComplimentRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientComplimentRes msg = new NetMsg.ClientComplimentRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("点赞回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("点赞回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            //更新赞的数据，同时写入数据中
            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;

            int seatNum = -1;


            for (int i = 0; i < hgpd.GradeMessage.Count; i++)
            {
                if (string.Equals(hgpd.GradeMessage[i].openRoomId, msg.szOpenRoomId))
                {
                    Debug.LogWarning("msg.szOpenRoomId：" + msg.szOpenRoomId);
                    for (int j = 0; j < 4; j++)
                    {
                        if (Convert.ToInt64(hgpd.GradeMessage[i].userid[j]) == GameData.Instance.PlayerNodeDef.iUserId)
                        {

                            seatNum = j;
                        }
                    }
                    hgpd.GradeMessage[i].playerComplient[seatNum] = msg.byComplimentSeat.ToString();
                }
            }
        }
        /// <summary>
        ///  CLIENT_HOLIDAY_RES = 0x0291;// [厅服]->[厅客]节日信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleHolidayRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientHolidayRes msg = new NetMsg.ClientHolidayRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("点赞回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("点赞回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //Debug.LogWarning("节日编号：" + msg.holiday.iHolidayId + "是否有效;" + msg.holiday.byValid + "开始时间" + msg.holiday.iBeginTim + "结束时间;" + msg.holiday.iEndTim + "会员赠卡：" + msg.holiday.byMemberFreeCard + "代理赠卡：" + msg.holiday.byProxyFreeCard);
            //HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            //hapd.HolidayDef = msg.holiday;
            //NetMsg.ClientUserDef ud = GameData.Instance.PlayerNodeDef.userDef;
            //if (msg.holiday.byValid == 1)//节日活动开启，
            //{

            //    if (ud.iLastHolidayId == msg.holiday.iHolidayId)
            //    {
            //        if (ud.iLastHolidayGiftTim >= msg.holiday.iBeginTim)
            //        {
            //            hapd.isGiftCanSave[1] = false;
            //        }
            //    }
            //    else
            //    {
            //        string timeStr = null;
            //        System.Net.WebClient client = new System.Net.WebClient();
            //        client.Encoding = Encoding.Default;
            //        try
            //        {
            //            string response = client.DownloadString("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1");
            //            timeStr = response.Substring(2);
            //        }
            //        catch (Exception)
            //        {

            //            throw;
            //        }
            //        System.DateTime time = System.DateTime.MinValue;
            //        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //        time = startTime.AddMilliseconds(Convert.ToDouble(timeStr));
            //        DateTime dt = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(hapd.HolidayDef.iBeginTim, 0);
            //        //  Debug.LogError(dt.Day + "   " + time.Day);
            //        if (time.Day == dt.Day)
            //        {
            //            hapd.isGiftCanSave[1] = true;
            //            hapd.SetIsRed(0, true);
            //        }
            //        else
            //        {
            //            hapd.isGiftCanSave[1] = false;
            //        }

            //    }
            //}
            //else
            //{
            //    hapd.isGiftCanSave[1] = false;

            //}
            //SystemMgr.Instance.HolidayActivitySystem.UpdateActivityCreate(0, false);
        }
        /// <summary>
        /// CLIENT_HOLIDAY_GIFT_RES = 0x0293;   // [厅服]->[厅客]领取节日活动奖励回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleHolidayGiftRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientHolidayGiftRes msg = new NetMsg.ClientHolidayGiftRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("点赞回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("领取节日活动奖励回应消息:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            Debug.LogWarning("领取节日回应，用户:" + msg.iUserId + "赠卡数量：" + msg.iFreeCard + "节日编号：" + msg.iHolidayId + "获取时间" + msg.iTim);
            //更新界面按钮
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            //if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
            //{
            //    //时间重置
            //    if (msg.byType == 1)
            //    {
            //        GameData.Instance.PlayerNodeDef.userDef.iLastCoin3Tim = msg.iTim;
            //        SystemMgr.Instance.HolidayActivitySystem.UpdateActivityCreate(0, false);
            //    }
            //    if (msg.byType == 2)
            //    {
            //        Debug.LogWarning("节日ID  " + msg.iHolidayId);
            //        hapd.iHolidayId = msg.iHolidayId;
            //        GameData.Instance.PlayerNodeDef.userDef.iLastHolidayGiftTim = msg.iTim;
            //        GameData.Instance.PlayerNodeDef.userDef.iLastHolidayId = msg.iHolidayId;
            //    }
            //    //按钮变灰
            //    hapd.isGiftCanSave[msg.byType - 1] = false;
            //    //hapd._isRedList[0] = false;//消失节日红点
            //    SystemMgr.Instance.HolidayActivitySystem.UpdateActivityCreate(0, true);

            //    hapd.isPanelShow = false;
            //    SystemMgr.Instance.HolidayActivitySystem.UpdateShow();
            //    //道具增加
            //    UIMainView.Instance.HolidayActivityPanel.gameObject.SetActive(false);
            //    //添加获得道具特效
            //    GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
            //    UIMainView.Instance.LobbyPanel.PlayGold_Particle(msg.iFreeCard);
            //}
        }
        /// <summary>
        /// CLIENT_FREE_TIME_RES = 0x0295;// [厅服]->[厅客]限免活动信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleFreeTimeRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientFreeTimeRes msg = new NetMsg.ClientFreeTimeRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("点赞回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("点赞回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //Debug.LogError("免费时间开放回应开始时间：" + msg.freeTime.iBeginTim + "结束时间：" + msg.freeTime.iEndTim + "是否有效：" + msg.freeTime.byValid);
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;

            if (SDKManager.Instance.CheckStatus == 1)
            {
                return;
            }


            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }
        /// <summary>
        /// 分享活动回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSharedRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientShareRes msg = new NetMsg.ClientShareRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("分享活动信息回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("分享活动信息回应消息错误编号:" + msg.iError);
                return;
            }


            //如果玩家分享次数为零，打开新手引导
            if (msg.share.iShareCount == 0)
            {
                NewPlayerGuide.Instance.OpenIndexGuide(NewPlayerGuide.Guide.ShareToWx_2);
            }

        }

        /// <summary>
        /// 分享活动用户信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSharedUserRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientShareUserRes msg = new NetMsg.ClientShareUserRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("分享活动用户信息回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("分享活动用户信息回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
        }
        /// <summary>
        /// 使用激活码回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleUseActiveCodeRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientUseActiveCodeRes msg = new NetMsg.ClientUseActiveCodeRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("分享活动用户信息回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("分享活动用户信息回应消息错误编号:" + msg.iError + Err_ClientUseActiveCodeRes.Err(msg.iError));
                UIMgr.GetInstance().GetUIMessageView().Show(Err_ClientUseActiveCodeRes.Err(msg.iError));
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //  HolidayActivityPanelData  agpd = GameData.Instance.HolidayActivityPanelData;
            Debug.LogWarning("批次：" + msg.szBatch + "激活码" + msg.szCode + "或卡数量" + msg.iCardCount);
            if (msg.iUserId == GameData.Instance.PlayerNodeDef.iUserId)
            {
                //    agpd.istipsShow = true;
                if (msg.iError == 0)
                {
                    UIMainView.Instance.HolidayActivityPanel.gameObject.SetActive(false);
                    //添加获得道具特效
                    GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
                    UIMainView.Instance.LobbyPanel.BtnNewPlayerBag(msg.iCardCount);
                }
                else if (msg.iError >= 3 && msg.iError <= 5)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("输入激活码错误");
                    // UIMainView.Instance.ActiveGiftPanel.UpdateShow();
                }
            }

        }
        /// <summary>
        /// 推广员信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleSpreaderInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientSpreaderInfoRes msg = new NetMsg.ClientSpreaderInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("推广员信息回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("推广员信息回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //Debug.LogError("推广员用户编号：" + msg.iSpreaderId + "推广员微信昵称" + msg.szSpreaderNickname);
            GetGiftSpreadBagPanelData gspd = GameData.Instance.GetGiftSpreadBagPanelData;
            if (msg.iSpreaderId == GameData.Instance.PlayerNodeDef.iSpreaderId)
            {
                gspd.szNickName = msg.szSpreaderNickname;
                gspd.szUrlHead = msg.szSpreaderHeadimgurl;
            }
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
        }

        /// <summary>
        /// 创建订单回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCreateOrderRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCreateOrderRes msg = new NetMsg.ClientCreateOrderRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("创建订单回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {

                Debug.LogError("创建订单回应消息编号:" + msg.iError + Err_ClientCreateOrderRes.Err(msg.iError));
                MahjongCommonMethod.Instance.ShowRemindFrame("创建订单错误ID：" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            Debug.LogWarning("创建订单回应成功 ichargeId；" + msg.iChargeId + "ichargeMode:" + msg.iChargeMode + "num：" + msg.iChargeNumber + "szorderid:" + msg.szOrderId + "szOrderInfo:" + msg.szOrderInfo);
            Debug.LogWarning(msg.szOrderInfo);
           // GameData.Instance.LobbyMainPanelData.szOrderId = msg.szOrderId;
            Debug.LogWarning("创建订单回应成功 iBoss；" + msg.iBoss);
            UIMainView.Instance.LobbyPanel.stackprice.Clear();
            UIMainView.Instance.LobbyPanel.stackprice.Push(msg);

            //苹果充值
            if (msg.iChargeMode == 1)
            {
#if UNITY_IOS && !UNITY_EDITOR
                InAppPurchasePluginiOS.UniIAPCharge(msg.iChargeId);
#endif
            }
            else if (msg.iChargeMode == 3)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                jo.Call("AliPay", msg.szOrderInfo);

            }
            else if (msg.iChargeMode == 2)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                jo.Call("WeChatPay", msg.szOrderInfo);
            }
        }
        /// <summary>
        /// 充值请求的回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleChargeRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientChargeRes msg = new NetMsg.ClientChargeRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("充值请求的回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("充值请求的回应消息错误编号:" + msg.iError);
                Debug.LogError(Err_ClientChargeRes.Err(msg.iError));
                MahjongCommonMethod.Instance.ShowRemindFrame("充值失败，请重入或联系客服");
                return;
            }
            Debug.LogWarning("充值成功 ichargeId；" + msg.iChargeId + "ichargeMode:" + msg.iChargeMode + "num：" + msg.iChargeNumber + "szorderid:" +
              msg.szOrderId + " 获得房卡数量:" + msg.iaCoin);
            GameData.Instance.LobbyMainPanelData.isDown = false;
            PlayerNodeDef pnd = GameData.Instance.PlayerNodeDef;
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            {
                pnd.userDef.byBossFirstChargeAward = 1;
            }
            else
            {
                pnd.userDef.byMemberFirstChargeAward = 1;
            }
            int coinNum = 0;
            for (int ms = 0; ms < msg.iaCoin.Length; ms++)
            {
                pnd.iCoin += msg.iaCoin[ms];
                coinNum += msg.iaCoin[ms];
            }
            UIMainView.Instance.LobbyPanel.isOnPay = false;

            MahjongCommonMethod.Instance.ShowRemindFrame("恭喜您成功购买  " + coinNum + "  个金币!");
            //NetMsg.ClientUserChargeReq chargMsg = new NetMsg.ClientUserChargeReq();//用户充值信息请求
            //chargMsg.iUserId = msg.iUserId;
            //Debug.LogError("用户充值信息请求"+msg.iUserId);
            //NetworkMgr.Instance.LobbyServer.SendUserChargeReq(chargMsg);

            UIMainView.Instance.LobbyPanel.BtnCloseBuyRoomCard();
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();

        }
        /// <summary>
        /// 充值信息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleUserChareRes(byte[] pMsg, int len, bool isOn = false)
        {
            if (!isOn)
            {
                int ioffset = 0;
                NetMsg.ClientUserChargeRes msg = new Message.NetMsg.ClientUserChargeRes();
                ioffset = msg.parseBytes(pMsg, ioffset);
                if (msg.iError != 0)
                {
                    Debug.LogError("充值信息请求错误编号：" + msg.iError);
                    return;
                }
                if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
                {
                    //Debug.LogError("充值个数：" + msg.iNumber);
                    for (int i = 0; i < msg.iNumber; i++)
                    {
                        NetMsg.UserCharge msgg = new NetMsg.UserCharge();
                        ioffset = msgg.parseBytes(pMsg, ioffset);
                    }
                }
                if (ioffset != len)
                {
                    Debug.LogError("充值信息请求长度不一致：iofffest:" + ioffset + ",len" + len);
                    return;
                }
            }


        }

        /// <summary>
        /// 房间号的服务器和桌信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleRoomNumSeverTableRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientRoomNumServerTableRes msg = new NetMsg.ClientRoomNumServerTableRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("房间号的服务器和桌信息回应消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            int count = 0;  //表示一入座的玩家数量，当等于四时，表示房间满员，给出提示
            for (int i = 1; i < 4; i++)
            {
                if (msg.iaUserId[i] > 0)
                {
                    Debug.LogWarning("玩家id：" + msg.iaUserId[i]);
                    count++;
                }
            }

            if (count > 3)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您选择的房间人数已满！");
                Messenger_anhui.Broadcast(MahjongLobby_AH.MainViewJoinRoomShowPanel.MESSAGE_CLEARBTN);
                return;
            }

            if (msg.iError != 0)
            {
                //Debug.LogError("房间号的服务器和桌信息回应消息:" + msg.iError);
                if (msg.iError == 3)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您还没有加入该麻将馆，不可进入此房间");
                }
                else
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您输入的房间号不存在，请重新输入");
                }
                Messenger_anhui.Broadcast(MahjongLobby_AH.MainViewJoinRoomShowPanel.MESSAGE_CLEARBTN);
                return;
            }

            MahjongCommonMethod.Instance.iSeverId = msg.iServerId;
            MahjongCommonMethod.Instance.iTableNum = msg.wTableNum;

            Debug.LogWarning("msg.wTableNum：" + msg.wTableNum + ",msg.iServerId：" + msg.iServerId);

            MahjongCommonMethod.Instance.RoomId = String.Format("{0:000000}", msg.iRoomNum);

            ////获取要连接的服务器
            Network.Message.NetMsg.ClientGameServerInfoReq msg_ = new Network.Message.NetMsg.ClientGameServerInfoReq();
            msg_.iUserId = msg.iUserId;
            msg_.iServerId = msg.iServerId;
            Network.NetworkMgr.Instance.LobbyServer.SendGameSeverInfoReq(msg_);
            MahjongCommonMethod.Instance.isCreateRoom = false;
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
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            Debug.LogWarning("分享成功回应：shareID" + msg.iShareId + "次数" + msg.iShareCount + "最后分享时间" + msg.iShareTim + "领取奖励时间;" + msg.iAwardTim + "领取赠卡书量" + msg.iFreeCard);
            ShareWxPanelData swpd = GameData.Instance.ShareWxPanelData;
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CompeleGetShareGift);
            swpd.shareSuccess = msg;
            if (msg.iUserId == MahjongCommonMethod.Instance.iUserid)
            {
                if (msg.iShareId != 0)
                {
                    GameData.Instance.ShareWxPanelData.PanelShow = false;
                    SystemMgr.Instance.ShareWxSystem.UpdateShow();
                    GameData.Instance.PlayerNodeDef.iBindCoin += msg.iFreeCard;
                    GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
                    //UIMainView.Instance.LobbyPanel.PlayGold_Particle(msg.iFreeCard);
                }

                Debug.LogWarning("分享成功 发送服务器成功" + GameData.Instance.m_iFestivalActivity);
                switch (GameData.Instance.m_iFestivalActivity)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 7:
                    case 11:
                        {
                            UIMgr.GetInstance().ShowRedPagePanel.OnSetState();
                            break;
                        }
                    case 12:
                        PlayerPrefs.SetInt("activePanel.imageRP" + msg.iShareId + GameData.Instance.PlayerNodeDef.iUserId, 0);
                        UIMainView.Instance.HolidayActivityPanel.activePanel._tShare.text = "分享";
                        UIMainView.Instance.HolidayActivityPanel.activePanel.imageRP.gameObject.SetActive(false);
                        break;
                }

                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }
        }

        /// <summary>
        /// 代开房支付房卡通知消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCardPayNotice(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCardPayNotice msg = new NetMsg.ClientCardPayNotice();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("代开房支付房卡通知消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            ////处理代理的房卡消耗
            //if (GameData.Instance.PlayerNodeDef.iIsProxy == 1)
            //{
            //    GameData.Instance.PlayerNodeDef.iRoomCard -= msg.iRoomCard;
            //    GameData.Instance.PlayerNodeDef.iFreeCard -= msg.iFreeCard;
            //    SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            //}
            //else
            //{
            //    Debug.LogError("玩家不是代理，但是发送了消耗放卡的消息");
            //}
        }

        /// <summary>
        /// 增加抽奖次数
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleLotteryCountRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientLotteryCountResDef msg = new NetMsg.ClientLotteryCountResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (msg.iError != 0 && (msg.iUserId != MahjongCommonMethod.Instance.iUserid))
            {
                Debug.LogError("error = " + msg.iError + "," + msg.iUserId + "=" + MahjongCommonMethod.Instance.iUserid);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            Debug.LogWarning("---110b 增加的次数：" + msg.byAddLotteryCount);

            //增加抽奖次数
            UIMainView.Instance.FestivalActivity.OnShowLotteryNumber(msg.byAddLotteryCount);

        }
        /// <summary>
        /// 抽奖消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleLotteryRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientLotteryResDef msg = new NetMsg.ClientLotteryResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);

            //if (ioffset != len)
            //{
            //    Debug.LogError("抽奖消息,ioffset:" + ioffset + ",len:" + len);
            //    return;
            //}
            if (msg.iError != 0 && (msg.iUserId != MahjongCommonMethod.Instance.iUserid))
            {
                Debug.LogError("error = " + msg.iError + "," + msg.iUserId + "=" + MahjongCommonMethod.Instance.iUserid);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            Debug.LogError("---110D 奖品编号：" + msg.byAwardCount);
            UIMainView.Instance.FestivalActivity.m_iWin = msg.byAwardCount - 1;
            SystemMgr.Instance.FestivalActivitySystem.OnActivityUpdateShow();
        }

        /// <summary>
        /// 用户抽奖活动信息消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleUserLotteryRes(byte[] pMsg, int len)
        {
            //string str = "";
            //for (int i = 0; i < pMsg.Length; i++)
            //{
            //    str += " " + pMsg[i];
            //}
            //Debug.LogError("----" + pMsg.Length + "," + str);
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }
            int ioffset = 0;
            NetMsg.ClientUserLotteryResDef msg = new NetMsg.ClientUserLotteryResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (msg.iError != 0 && (msg.iUserId != MahjongCommonMethod.Instance.iUserid))
            {
                Debug.LogError("error = " + msg.iError + "," + msg.iUserId + "=" + MahjongCommonMethod.Instance.iUserid);
                return;
            }

            UIMainView.Instance.LobbyPanel.OnTestTimeForScend(msg.iBeginTime, msg.iEndTime);

            if (GameData.Instance.PlayerNodeDef.iAuthenType != 3)
            {
                if ((msg.userLotteryDef.byLotteryCount == 0 && msg.userLotteryDef.iLotteryShareTim != 0)
                || GameData.Instance.PlayerNodeDef.userDef.byUserSource != 2)
                {
                    UIMainView.Instance.FestivalActivity.gameObject.SetActive(false);
                }
                else
                {
                    UIMainView.Instance.FestivalActivity.gameObject.SetActive(true);
                }

                if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 2)
                {
                    //如果是活动期间  开启活动按钮
                    UIMainView.Instance.LobbyPanel.DoFestivalActivityShowOrClose(true);
                }
            }
            else
            {
                UIMainView.Instance.LobbyPanel.DoFestivalActivityShowOrClose(true);
            }

            if (msg.userLotteryDef.iLotteryShareTim != 0)
            {
                UIMainView.Instance.FestivalActivity.OnShowFenXiangBtn(false);
            }

            Debug.LogError("---1111 登陆之后获取可转动转盘次数：" + msg.userLotteryDef.byLotteryCount + "，时间:" + msg.userLotteryDef.iLotteryShareTim);
            //增加抽奖次数
            UIMainView.Instance.FestivalActivity.OnShowLotteryNumber(msg.userLotteryDef.byLotteryCount);
        }

        /// <summary>
        /// 创建馆的回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCreatParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCreateParlorRes msg = new NetMsg.ClientCreateParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset=" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("创建馆的回应消息错误编号：" + msg.iError);
                if (msg.iError == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您还没有开馆资格");
                }

                if (msg.iError == 2)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您已经有了自己的麻将馆");
                }

                if (msg.iError == 3)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆名字包含敏感词");
                }

                if (msg.iError == 20)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆名字已被占用");
                }

                if (msg.iError == 23)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆名字长度不足");
                }

                if (msg.iError == 24)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您最近两小时退出或者解散过馆，不可创建！");
                }
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            //关闭馆的创建界面，同时显示编辑馆的公告和联系方式
            GameData gd = GameData.Instance;
            ParlorShowPanelData pspd = gd.ParlorShowPanelData;
            pspd.iOldParlorNum += 1;
            //保存馆的信息            
            NetMsg.ParlorInfoDef info = new NetMsg.ParlorInfoDef();
            pspd.parlorInfoDef = new NetMsg.ParlorInfoDef[4];
            info.iBossId = GameData.Instance.PlayerNodeDef.iUserId;
            info.szBossHeadimgurl = GameData.Instance.PlayerNodeDef.szHeadimgurl;
            info.szBossNickname = GameData.Instance.PlayerNodeDef.szNickname;
            info.szParlorName = msg.cParlorName;
            info.iParlorId = msg.iParlorId;
            info.iMemberNum = 0;
            info.iVitality = 0;
            info.iMonthVitality = 0;
            info.iCreateTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
            info.iCityId = msg.iCityId;
            info.iCountyId = msg.iCountyId;
            info.szBulletin = " ";
            info.szContact = " ";
            pspd.parlorInfoDef[0] = info;

            gd.PlayerNodeDef.iMyParlorId = msg.iParlorId;
            pspd.iParlorId = msg.iParlorId;
            pspd.isChangeBulletin = true;
            pspd.isChangeContact = true;
            pspd.isShowChangeParlorMessage = true; //打开边界公告界面            
            pspd.isShowWriteParlorName = false;  //关闭创建麻将馆的界面            
            pspd.ParlorBulletin = " ";
            pspd.ParlorContact = " ";
            UIMainView.Instance.ParlorShowPanel.InitData();

            SystemMgr.Instance.ParlorShowSystem.UpdateShow();

            pspd.PointParlorTabelMessage_UnStart = new List<NetMsg.TableInfoDef>();
            pspd.PointParlorTabelMessage_Started = new List<ParlorShowPanelData.GetPointParlorTabelMessage>();
            NetMsg.TableInfoDef message = new NetMsg.TableInfoDef();
            message.iSeverId = 0;
            message.iRoomNum = 0;
            pspd.PointParlorTabelMessage_UnStart.Add(message);
            UIMainView.Instance.ParlorShowPanel.RemoveAllListener();
            UIMainView.Instance.ParlorShowPanel.InitAllBtn_Parlor();
            UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.TabelGame);
            UIMainView.Instance.ParlorShowPanel.SpwanPorlorTabelMessage(2);

            UIMainView.Instance.ParlorShowPanel.Boss_Mem[0].SetActive(true);
            UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].SetActive(false);

            //更新界面信息
            SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(info);

            //如果玩家有该馆的预约信息
            if (MahjongCommonMethod.Instance.iParlorId == msg.iParlorId && Convert.ToInt32(MahjongCommonMethod.Instance.RoomId) > 0)
            {
                //在这里向服务器请求，玩家当前是否有房间，如果有显示返回房间
                NetMsg.ClientMyRoomInfoReq myroominfo = new NetMsg.ClientMyRoomInfoReq();
                myroominfo.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                NetworkMgr.Instance.LobbyServer.SendMyRoomInfoReq(myroominfo);
            }
        }

        /// <summary>
        /// 解散麻将馆的回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleDismissParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientDismissParlorRes msg = new NetMsg.ClientDismissParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("解散麻将馆的回应消息错误编号：" + msg.iError);

                if (msg.iError == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您还没有麻将馆");
                }

                if (msg.iError == 2)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的业绩积分大于100");
                }
                return;
            }

            //添加玩家的解散馆的次数
            GameData.Instance.PlayerNodeDef.iDismissParlorAcc++;
            GameData.Instance.PlayerNodeDef.userDef.iLeaveParlorTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            pspd.parlorInfoDef = new NetMsg.ParlorInfoDef[4];  //清空麻将馆的数据
            pspd.isShowMyParlorMessage = false;
            pspd.isShowChangeParlorMessage = false;
            pspd.isShowSecondDismissPanel = false;
            GameData.Instance.PlayerNodeDef.iMyParlorId = 0;  //将用户节点中的麻将馆的id置为0
            pspd.isGetAllData_PointCountyParlor = new bool[] { false, false, false };
            for (int i = 0; i < 2; i++)
            {
                pspd.iCountyId[i] = sapd.iCountyId;
                pspd.iCityId[i] = sapd.iCityId;
            }
            pspd.isShowParlorRoundPanel = true;
            pspd.iOldParlorNum -= 10;

            //重新进入麻将馆信息
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_OPENPARLOR);

            if (msg.iCoinNum > 0)
            {
                GameData.Instance.PlayerNodeDef.iCoin += msg.iCoinNum;
            }
            //关闭预约房间倒计时面板
            pspd.isShowOrderTimePanel = false;
            UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.gameObject.SetActive(false);
            UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.ParlorGod);
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }


        /// <summary>
        /// 修改麻将馆公告信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleChangeParlorBullrtinInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientChangeParlorBulletinInfoRes msg = new NetMsg.ClientChangeParlorBulletinInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("修改麻将馆公告信息回应消息错误编号：" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("修改麻将馆的信息失败");
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.parlorInfoDef[0].szBulletin = msg.szBulletinContent;
            //打开自己的公告面板
            pspd.isShowMyParlorMessage = true;
            pspd.isShowChangeParlorMessage = false;
            SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(pspd.parlorInfoDef[0]);
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();

            //Debug.LogError("公告信息:" + msg.szBulletinContent);

            //if (!pspd.isChangeBulletin || !pspd.isChangeContact)
            //{
            //    //打开自己的公告面板
            //    pspd.isShowMyParlorMessage = true;
            //    pspd.isShowWriteParlorName = false;
            //    SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(pspd.parlorInfoDef[0]);
            //    SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            //}

        }

        /// <summary>
        /// 修改麻将馆联系方式回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleChangeParlorContactInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientChangeParlorContactInfoRes msg = new NetMsg.ClientChangeParlorContactInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("修改麻将馆联系信息回应消息错误编号：" + msg.iError);
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.parlorInfoDef[0].szContact = msg.szContact;
            //打开自己的公告面板
            pspd.isShowMyParlorMessage = true;
            pspd.isShowChangeParlorMessage = false;
            SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(pspd.parlorInfoDef[0]);
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
        }

        /// <summary>
        /// 申请 加入麻将馆回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleJoinParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientJoinParlorRes msg = new NetMsg.ClientJoinParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("申请 加入麻将馆回应消息：" + msg.iError);
                if (msg.iError == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的申请数量已达到上限！");
                }

                if (msg.iError == 18)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您已经申请过该馆，请耐心等待馆主审核！");
                }
                return;
            }

            UIMgr.GetInstance().GetUIMessageView().Show("已经为您提交审核，请耐心等待馆主审核");
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < pspd.ApplyParlorId_All.Length; i++)
            {
                if (pspd.ApplyParlorId_All[i] == 0)
                {
                    pspd.ApplyParlorId_All[i] = msg.iParlorId;
                    break;
                }
            }
            PlayerPrefs.SetInt(ParlorShowPanelData.SaveLaseParlorId + GameData.Instance.PlayerNodeDef.iUserId, msg.iParlorId);
            //重新请求自己的麻将馆信息
            GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
        }

        /// <summary>
        /// 申请 退出麻将馆回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleLevelParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientLeaveParlorRes msg = new NetMsg.ClientLeaveParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("申请 退出麻将馆回应消息：" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("退出馆失败");
                return;
            }

            UIMgr.GetInstance().GetUIMessageView().Show("您已经成功退出该馆", Ok_QuitParlor);

            GameData.Instance.PlayerNodeDef.userDef.iLeaveParlorTime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);

            //清空该馆的对应的信息
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.iOldParlorNum -= 10;
            for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
            {
                //如果馆id一样，清除该馆信息
                if (pspd.parlorInfoDef[i].iParlorId == msg.iParlorId)
                {
                    pspd.parlorInfoDef[i] = new NetMsg.ParlorInfoDef();

                    //////发送取消占座消息
                    //NetMsg.ClientUserCancleBespeakReqDef ClientUserCancleBespeakReq = new NetMsg.ClientUserCancleBespeakReqDef();
                    //ClientUserCancleBespeakReq.byFromType = 1;//消息来源，1来自大厅，2来自游戏客户端
                    //ClientUserCancleBespeakReq.iUserId = GameData.Instance.PlayerNodeDef.iUserId; // 用户编号
                    //ClientUserCancleBespeakReq.iRoomNum = Convert.ToInt32(MahjongCommonMethod.Instance.RoomId);//房间号
                    //Debug.LogError("----主动退出馆---" + ClientUserCancleBespeakReq.iRoomNum + "," + MahjongCommonMethod.Instance.RoomId);
                    //SendClientUserCancleBespeakReq(ClientUserCancleBespeakReq);

                    break;
                }
            }

            PlayerNodeDef pnf = GameData.Instance.PlayerNodeDef;
            int count = 0;
            for (int i = 0; i < pnf.iaJoinParlorId.Length; i++)
            {
                if (msg.iParlorId == pnf.iaJoinParlorId[i])
                {
                    pnf.iaJoinParlorId[i] = 0;
                }

                if (pnf.iaJoinParlorId[i] > 0)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                //清掉自己的馆的信息
                UIMainView.Instance.ParlorShowPanel.ClearPointParlorPrefab(msg.iParlorId);
            }
            //如果退出该馆后，没有自己的麻将馆则直接回到，雀神广场
            else
            {
                UIMainView.Instance.ParlorShowPanel.BtnParlorRound();
                SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            }

            //添加玩家的退馆次数
            GameData.Instance.PlayerNodeDef.userDef.iLeaveParlorAcc++;

            //如果玩家有该馆的预约信息
            if (MahjongCommonMethod.Instance.iParlorId == msg.iParlorId && Convert.ToInt32(MahjongCommonMethod.Instance.RoomId) > 0)
            {
                //在这里向服务器请求，玩家当前是否有房间，如果有显示返回房间
                NetMsg.ClientMyRoomInfoReq myroominfo = new NetMsg.ClientMyRoomInfoReq();
                myroominfo.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                NetworkMgr.Instance.LobbyServer.SendMyRoomInfoReq(myroominfo);
            }
        }


        void Ok_QuitParlor()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowMyParlorMessage = false;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
        }

        /// <summary>
        /// 邀请用户进入麻将馆回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleInvitParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientInvitParlorRes msg = new NetMsg.ClientInvitParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                if (msg.iError == 18)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您已经邀请过\"" + msg.iInviteUserId + "\"了");
                }

                if (msg.iError == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您邀请的玩家加入的麻将馆已经达到上限!");
                }

                if (msg.iError == 2 || msg.iError == 5)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("他已经是麻将馆老板，不可以加入其他的麻将馆");
                }

                if (msg.iError == 19)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆已被封馆，不可以邀请用户加入");
                }
                return;
            }
            UIMgr.GetInstance().GetUIMessageView().Show("邀请已成功发送 请等待玩家同意");
            //关闭邀请面板            
            UIMainView.Instance.ParlorShowPanel.RemingMessage.SetActive(true);
            UIMainView.Instance.ParlorShowPanel.BtnInviteAndLevel.SetActive(false);
            UIMainView.Instance.ParlorShowPanel.MemberMessage.SetActive(false);
        }

        /// <summary>
        /// 踢用户出麻将馆回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleKickParlorRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientKickParlorRes msg = new NetMsg.ClientKickParlorRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("踢用户出麻将馆回应消息：" + msg.iError);
                return;
            }

            //将成员从列表删除
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.parlorInfoDef[0].iMemberNum -= 1;
            UIMainView.Instance.ParlorShowPanel.UpdateMemberCount(GameData.Instance.ParlorShowPanelData.parlorInfoDef[0].iMemberNum);
            UIMgr.GetInstance().GetUIMessageView().Show("您已经将玩家\"" + msg.iKickUserId + "\"从您的麻将馆踢出");

            //刷新成员数据
            if (UIMainView.Instance.ParlorShowPanel.ParlorTableGame.KickType == 3)
            {
                pspd.FromWebGetParlorMember();
            }
        }

        /// <summary>
        /// 获取麻将馆信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGetParlorInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientGetParlorInfoRes msg = new NetMsg.ClientGetParlorInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Debug.LogWarning("ParlorInfoDef麻将馆数量：" + msg.byParlorNum);
            for (int i = 0; i < 4; i++)
            {
                if (i < msg.byParlorNum)
                {
                    NetMsg.ParlorInfoDef msgg = new NetMsg.ParlorInfoDef();
                    ioffset = msgg.parseBytes(pMsg, ioffset);
                    msgg.iMemberNum += 1;
                    pspd.parlorInfoDef[i] = msgg;
                }
                else
                {
                    NetMsg.ParlorInfoDef msgg = new NetMsg.ParlorInfoDef();
                    pspd.parlorInfoDef[i] = msgg;
                }
                ParlorShowPanelData.ParlorRedBag_ info = new ParlorShowPanelData.ParlorRedBag_();
                if (pspd.parlorRedBagInfo[i] == null)
                {
                    pspd.parlorRedBagInfo[i] = info;
                }
            }

            if (ioffset != len)
            {
                Debug.LogError("消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("获取麻将馆信息回应消息：" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("获取麻将馆信息失败，请联系客服反馈！");
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            GameData.Instance.PlayerNodeDef.userDef.byFirstInParlor = 1;

            SDKManager.Instance.GetComponent<CameControler>().PostMsg("uloading");

            //为麻将馆的县和城市id赋值
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            pspd.iCountyId[1] = sapd.iCountyId;
            pspd.iCityId[1] = sapd.iCityId;

            //表示定时请求刷新麻将馆界面，下面不执行
            if (pspd.GetParlorMessageType > 1)
            {
                if (pspd.GetParlorMessageType == 3)
                {
                    if (SDKManager.Instance.ParlorId > 0)
                    {
                        pspd.HandleLqAppToParlorId(SDKManager.Instance.ParlorId);
                    }
                    return;
                }
                //刷新麻将馆界面
                if (pspd.iParlorId > 0)
                {
                    NetMsg.ParlorInfoDef info = new NetMsg.ParlorInfoDef();
                    for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                    {
                        if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                        {
                            info = pspd.parlorInfoDef[i];
                            break;
                        }
                    }

                    if (info != null && info.iParlorId > 0)
                    {
                        UIMainView.Instance.ParlorShowPanel.UpdateMyParlorMessage_Title(info);
                    }

                    //定时请求麻将馆信息
                    if (pspd.GetParlorMessageType != 3)
                    {
                        UIMainView.Instance.ParlorShowPanel.TimerDoSomeThing(600, pspd.FromWebGetApplyParlorId);
                    }
                }
                return;
            }

            //获取显示地区图片的初始位置
            UIMainView.Instance.ParlorShowPanel.GetInitParlorImagePos();

            //如果老板进入麻将馆，直接显示馆的信息
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            {
                Debug.Log("---------------------");
                //如果自己的麻将馆被封，则显示界面
                if (pspd.parlorInfoDef[0].iStatus == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆已被封馆,请尽快联系官方客服解封");
                    return;
                }
                UIMainView.Instance.ParlorShowPanel.Boss_Mem[0].SetActive(true);
                UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].SetActive(false);
                SystemMgr.Instance.LobbyMainSystem.MessageReq(0);
                pspd.isShowMyParlorMessage = true;
                pspd.iParlorId = GameData.Instance.PlayerNodeDef.iMyParlorId;
                //显示自己创建的麻将界面
                SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(pspd.parlorInfoDef[0]);

                pspd.iCityId[2] = pspd.parlorInfoDef[0].iCityId;
                pspd.iCountyId[2] = pspd.parlorInfoDef[0].iCountyId;

                UIMainView.Instance.ParlorShowPanel.RemoveAllListener();
                UIMainView.Instance.ParlorShowPanel.InitAllBtn_Parlor();
                //请求该馆的开房信息
                UIMainView.Instance.ParlorShowPanel.BtnParlorTabelGame(true, 0);
            }
            //如果会员进入，显示其他信息
            else
            {
                ShowParlorMessage[] temp = UIMainView.Instance.ParlorShowPanel.DeleteMyParlorMessage();
                UIMainView.Instance.ParlorShowPanel.Boss_Mem[0].SetActive(false);
                UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].SetActive(true);
                pspd.iCountyId[0] = sapd.iCountyId;
                pspd.iCityId[0] = sapd.iCityId;
                pspd.isShowMyParlorMessage = true;

                int count = temp.Length > (msg.byParlorNum + pspd.ApplyParlorMessage.Count) ? temp.Length : (msg.byParlorNum + pspd.ApplyParlorMessage.Count);

                int alredySpwanCount = 0;

                //Debug.LogError("count:" + count + ",temp.Length:" + temp.Length);

                //产生麻将馆的信息
                for (int i = 0; i < msg.byParlorNum; i++)
                {
                    if (alredySpwanCount < temp.Length)
                    {
                        temp[alredySpwanCount].gameObject.SetActive(true);
                        temp[alredySpwanCount].UpdateParlorBtn(pspd.parlorInfoDef[i]);
                        alredySpwanCount++;
                    }
                    else
                    {
                        UIMainView.Instance.ParlorShowPanel.ShowMyParlorMessage(pspd.parlorInfoDef[i], 1, UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].transform);
                    }
                }

                //已经申请的
                for (int i = 0; i < pspd.ApplyParlorMessage.Count; i++)
                {
                    if (alredySpwanCount < temp.Length)
                    {
                        temp[alredySpwanCount].gameObject.SetActive(true);
                        temp[alredySpwanCount].UpdateParlorBtn(pspd.ApplyParlorMessage[i]);
                        alredySpwanCount++;
                    }
                    else
                    {
                        UIMainView.Instance.ParlorShowPanel.ShowMyParlorMessage(pspd.ApplyParlorMessage[i], 1, UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].transform, true);
                    }
                }

                //如果玩家不足四个馆，额外产生一个加入麻将馆的信息
                if (msg.byParlorNum + pspd.ApplyParlorMessage.Count < 4)
                {
                    if (alredySpwanCount < temp.Length)
                    {
                        temp[alredySpwanCount].gameObject.SetActive(true);
                        temp[alredySpwanCount].UpdateParlorBtn(null);
                        alredySpwanCount++;
                    }
                    else
                    {
                        UIMainView.Instance.ParlorShowPanel.ShowMyParlorMessage(null, 2, UIMainView.Instance.ParlorShowPanel.Boss_Mem[1].transform);
                    }
                }

                if (alredySpwanCount < temp.Length)
                {
                    for (int i = alredySpwanCount; i < temp.Length; i++)
                    {
                        temp[i].gameObject.SetActive(false);
                    }
                }

                //如果是馆内成员，进入当前馆内
                if (MahjongCommonMethod.Instance.iFromParlorInGame > 0)
                {
                    UIMainView.Instance.ParlorShowPanel.PointShowParlor(MahjongCommonMethod.Instance.iFromParlorInGame);
                    MahjongCommonMethod.Instance.iFromParlorInGame = 0;
                }
                else
                {
                    UIMainView.Instance.ParlorShowPanel.PointShowParlor(PlayerPrefs.GetInt(ParlorShowPanelData.SaveLaseParlorId));
                }

            }
            pspd.isShowPanel = true;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();

            //定时请求麻将馆信息
            if (pspd.GetParlorMessageType != 3)
            {
                UIMainView.Instance.ParlorShowPanel.TimerDoSomeThing(600, pspd.FromWebGetApplyParlorId);
            }

            //请求刷新用户节点信息
            if (pspd.iOldParlorNum != msg.byParlorNum && pspd.iOldParlorNum != 0)
            {
                NetMsg.ClientRefreshUserReq req = new NetMsg.ClientRefreshUserReq();
                req.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                Network.NetworkMgr.Instance.LobbyServer.SendClientRefreshUserReq(req);
            }
            pspd.iOldParlorNum = msg.byParlorNum + 2;
        }

        /// <summary>
        /// 获取玩家是否有开馆资格的回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCreatParlorCertRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientParlorCertRes msg = new NetMsg.ClientParlorCertRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("玩家是否有开馆资格的回应消息长度不一致,ioffset:" + ioffset + ",len：" + len);
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            pspd.isShowSecondSure = false;
            //如果玩家有开馆资格
            pspd.isShowApplyCreatParlorPanel = true;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            if (msg.iError != 0)
            {
                Debug.LogError("玩家是否有开馆资格的回应消息错误编号：" + msg.iError);
                pspd.isShowApplyCreatParlorPanel = true;
                UIMainView.Instance.ParlorShowPanel.ApplyCreatParlorCertl.UpdateShow(0);
                return;
            }

            if (msg.Type == 1)
            {
                UIMainView.Instance.ParlorShowPanel.ApplyCreatParlorCertl.BtnCreatParlor();
            }
            else
            {
                pspd.isShowApplyCreatParlorPanel = true;
                UIMainView.Instance.ParlorShowPanel.ApplyCreatParlorCertl.UpdateShow(0);
            }

            SystemMgr.Instance.ParlorShowSystem.UpdateShow();

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
                Debug.LogError("用户占座回应消息 错误" + msg.iError);
                switch (msg.iError)
                {
                    case 16: UIMgr.GetInstance().GetUIMessageView().Show("您已经有了预约房间，不可以继续占座"); break;
                    case 17: UIMgr.GetInstance().GetUIMessageView().Show("此位置已被占", () => { UIMainView.Instance.ReservationSeatPanel.gameObject.SetActive(false); }); break;
                    case 18: UIMgr.GetInstance().GetUIMessageView().Show("您的金币不足"); break;
                    case 19: UIMgr.GetInstance().GetUIMessageView().Show("您已在其他地方开房"); break;
                }
                return;
            }
            //修改玩家的桌的列表信息
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            for (int i = 0; i < pspd.PointParlorTabelMessage_UnStart.Count; i++)
            {
                //找到这个桌的信息，修改占座需求信息
                if (pspd.PointParlorTabelMessage_UnStart[i] != null &&
                    pspd.PointParlorTabelMessage_UnStart[i].iRoomNum == msg.iRoomNum)
                {
                    pspd.PointParlorTabelMessage_UnStart[i].iaBespeakUserId[msg.iSeatNum - 1] = msg.iUserId;
                    MahjongCommonMethod.Instance.iSeverId = pspd.PointParlorTabelMessage_UnStart[i].iSeverId;
                    iBespeakTime = Convert.ToInt32(pspd.PointParlorTabelMessage_UnStart[i].iBespeakTime) * 60 + Convert.ToInt32(pspd.PointParlorTabelMessage_UnStart[i].iOpenRoomTime);
                    break;
                }
            }
            MahjongCommonMethod.Instance.iTableNum = msg.wTableNum;
            MahjongCommonMethod.Instance.RoomId = msg.iRoomNum.ToString();
            MahjongCommonMethod.Instance.GetNowTimer(0, DelayGetNowTimer);
            //重新请求卓信息
            pspd.FromWebPointParlorTabelMessage(5);

            UIMainView.Instance.ReservationSeatPanel.DoZhanZhuo(msg.iSeatNum - 1);
            GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = true;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
        }

        int iBespeakTime = 0;  //预约房间开始时间
        /// <summary>
        /// 延迟获得当前时间时间戳
        /// </summary>
        void DelayGetNowTimer(int id, int timer)
        {
            Debug.LogWarning("时间:" + iBespeakTime + ",timer:" + timer);
            int Interval = iBespeakTime - timer;  //剩余时间
            if (Interval > 0 && iBespeakTime > 0)
            {
                UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.StopIeNum();
                UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.UpdateShow(Interval);
                if (Interval != 0)
                {
                    UIMainView.Instance.LobbyPanel.m_bInGame = true;
                }
                else
                {
                    UIMainView.Instance.LobbyPanel.m_bInGame = false;
                }
            }
        }

        /// <summary>
        /// 获取用户信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGetUserInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientGetUserInfoRes msg = new NetMsg.ClientGetUserInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("获取用户信息回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("获取用户信息回应消息错误：" + msg.iError);
                UIMgr.GetInstance().GetUIMessageView().Show("您查找的成员不存在");
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            //如果玩家不在馆内
            if (msg.userInfo.byParlorType == 1 && msg.userInfo.iUserId != GameData.Instance.PlayerNodeDef.iUserId)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("此ID非本馆成员");
                return;
            }

            Debug.LogWarning("玩家id:" + msg.iUserId + ",是否是馆成员:" + msg.userInfo.byParlorType);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //显示玩家信息
            UIMainView.Instance.ParlorShowPanel.ShowSearchMemberMessage(msg.userInfo.byParlorType, msg.userInfo);
        }

        /// <summary>
        /// 桌上玩家信息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGetTableUserIdRes(byte[] pMsg, int len)
        {
            //先清理一下
            for (int index = 0; index < 4; index++)
            {
                UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[index] = null;
            }
            int ioffset = 0;
            NetMsg.ClientGetTableUserIDRes msg = new NetMsg.ClientGetTableUserIDRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (ioffset != len)
            {
                Debug.LogError("获取用户信息回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("获取用户信息回应消息错误：" + msg.iError);
                return;
            }

            bool isSendMessage = false;

            List<int> userId = new List<int>();

            if (pspd.ComeInParlorType == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg.iUserId[i] != 0)
                    {
                        if (pspd.userInfo_Tabel.ContainsKey(msg.iUserId[i]))
                        {
                            UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[i] = pspd.userInfo_Tabel[msg.iUserId[i]];
                        }
                        else
                        {
                            //请求当前玩家信息
                            isSendMessage = true;
                            userId.Add(msg.iUserId[i]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg.iUserId[i] == 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        if (pspd.userInfo_Tabel.ContainsKey(UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[j]))
                        {
                            UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[j] =
                                pspd.userInfo_Tabel[UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[j]];
                        }
                        else
                        {
                            //请求消息
                            isSendMessage = true;
                            userId.Add(msg.iUserId[i]);
                        }
                    }
                }
            }

            if (!isSendMessage)
            {
                UIMainView.Instance.ReservationSeatPanel.OnOpen();//打开界面
            }
            else
            {
                int[] id = new int[4];
                for (int i = 0; i < userId.Count; i++)
                {
                    id[i] = userId[i];
                }
                //发送信息
                NetMsg.ClientGetTableUserInfoReq info = new NetMsg.ClientGetTableUserInfoReq();
                info.iUserId = id;
                //NetworkMgr.Instance.LobbyServer.SendClientGetUseInfoReq(info);
            }
        }

        /// <summary>
        /// CLIENT_GET_TABLE_USER_INFO_RES	0x025C // [厅服]->[厅客]获取用户信息回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleGetTableUserInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientGetTableUserInfoRes msg = new NetMsg.ClientGetTableUserInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);

            //Debug.LogError("msg.UserNum:" + msg.UserNum);
            if (GameData.Instance.ParlorShowPanelData.ComeInParlorType == 3)
            {
                UIMainView.Instance.ReservationSeatPanel.m_lUserInfo = new NetMsg.TableUserInfoDef[4];
            }


            for (int i = 0; i < msg.UserNum; i++)
            {
                NetMsg.TableUserInfoDef info = new NetMsg.TableUserInfoDef();
                ioffset = info.parseBytes(pMsg, ioffset);

                if (GameData.Instance.ParlorShowPanelData.ComeInParlorType == 3)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[i] = info;
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[j] == info.iUserId)
                        {
                            UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[j] = info;
                            break;
                        }
                    }
                }

                //Debug.LogError("============保存玩家信息");
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                if (pspd.userInfo_Tabel.ContainsKey(info.iUserId))
                {
                    pspd.userInfo_Tabel[info.iUserId] = info;
                }
                else
                {
                    pspd.userInfo_Tabel.Add(info.iUserId, info);
                }
            }
            if (ioffset != len)
            {
                Debug.LogError("获取用户信息回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("获取用户信息回应消息错误：" + msg.iError);
                return;
            }

            UIMainView.Instance.ReservationSeatPanel.OnOpen();//打开界面

        }
        /// <summary>
        /// 所有红包数量回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientRedNumRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientRedNumResDef msg = new NetMsg.ClientRedNumResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);

            if (ioffset != len)
            {
                Debug.LogError("所有红包数量回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("所有红包数量回应消息：" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            int RPNum = 0;
            for (int i = 0; i < 16; i++)
            {
                RPContent rp1 = new RPContent();
                rp1.Image = UIMainView.Instance.RedPagePanel.RedPage[i].Image;
                rp1.Name = UIMainView.Instance.RedPagePanel.RedPage[i].Name;
                rp1.ShareNum = msg.iaRedPag[i, 1];
                rp1.CanUseNum = msg.iaRedPag[i, 0];//为可以红包数
                rp1.GetMoneyNum = UIMainView.Instance.RedPagePanel.RedPage[i].GetMoneyNum;
                rp1.GetMoneyType = UIMainView.Instance.RedPagePanel.RedPage[i].GetMoneyType;
                rp1.isShare = UIMainView.Instance.RedPagePanel.RedPage[i].isShare;
                rp1.RpNumber = UIMainView.Instance.RedPagePanel.RedPage[i].RpNumber;
                rp1.Describe = UIMainView.Instance.RedPagePanel.RedPage[i].Describe;
                UIMainView.Instance.RedPagePanel.RedPage[i] = rp1;
                RPNum += rp1.CanUseNum + rp1.ShareNum;
                //Debug.LogError(rp1.Name + " 可用:" + rp1.CanUseNum + " 需分享：" + rp1.ShareNum);
            }

            if (UIMainView.Instance.LobbyPanel.isDengluOnce)
            {
                if (UIMainView.Instance.RedPagePanel.RedPage[4].CanUseNum > 0)
                {
                    UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(4, UIMainView.Instance.RedPagePanel.RedPage[4].CanUseNum, 2, UIMainView.Instance.RedPagePanel.RedPage[4].Name, RedPageShowPanel.NowState.Lobby);
                }
            }

            //打开红包界面
            //UIMainView.Instance.RedPagePanel.gameObject.SetActive(true);
            SystemMgr.Instance.RedPageShowSystem.UpdateShow();
            if (RPNum > 0 && SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                UIMainView.Instance.LobbyPanel.RedPage.SetActive(true);

            UIMainView.Instance.LobbyPanel.isDengluOnce = false;
            //UIMainView.Instance.RedPagePanel.OnInitRedPage();
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
                //关闭这个界面
                //UIMgr.GetInstance().ShowRedPagePanel.OnSetClose();
                return;
            }

            if (msg.byRedPagType == 7)
            {
                if (GameData.Instance.ParlorShowPanelData.isShowMyParlorMessage)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (GameData.Instance.ParlorShowPanelData.parlorInfoDef[j].iParlorId == GameData.Instance.ParlorShowPanelData.iParlorId)
                        {
                            GameData.Instance.ParlorShowPanelData.parlorInfoDef[j].iRp7Type = 0;
                            break;
                        }
                    }
                }

            }

            if (msg.iRp3ID != 0)
            {
                Debug.LogWarning("是大红包" + msg.iRp3ID);
                UIMgr.GetInstance().ShowRedPagePanel.OnSetShareRP(2, true, msg.iRp3ID);
                //UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(2, 1, 1, UIMainView.Instance.RedPagePanel.RedPage[2].Name, RedPageShowPanel.NowState.Lobby, msg.iRp3ID);
                return;
            }

            for (int i = 0; i < 16; i++)
            {
                RPContent rp1 = new RPContent();
                rp1.Image = UIMainView.Instance.RedPagePanel.RedPage[i].Image;
                rp1.Name = UIMainView.Instance.RedPagePanel.RedPage[i].Name;
                rp1.CanUseNum = msg.ia2RedPacket[i, 0];//可以使用的数量
                rp1.ShareNum = msg.ia2RedPacket[i, 1];//需要分享的数量
                rp1.GetMoneyNum = UIMainView.Instance.RedPagePanel.RedPage[i].GetMoneyNum;
                rp1.GetMoneyType = UIMainView.Instance.RedPagePanel.RedPage[i].GetMoneyType;
                rp1.isShare = UIMainView.Instance.RedPagePanel.RedPage[i].isShare;
                rp1.RpNumber = UIMainView.Instance.RedPagePanel.RedPage[i].RpNumber;
                rp1.Describe = UIMainView.Instance.RedPagePanel.RedPage[i].Describe;
                UIMainView.Instance.RedPagePanel.RedPage[i] = rp1;
                //Debug.LogError(rp1.Name + " 可用:" + rp1.CanUseNum + " 需分享：" + rp1.ShareNum);
            }
            //Debug.LogError("多少钱" + msg.dResourceNum);
            int RpType = -1;
            //获得资源类型:1现金，2话费，3流量，4储值卡，（5代金券，6赠币没有对应字段）
            string str = "", str1 = "", str2 = "";
            str2 = (msg.dResourceNum / 100.0f).ToString();
            switch (msg.byResourceType)
            {
                case 1:
                    {
                        str = "现金";
                        str1 = "元";
                        RpType = 3;
                        GameData.Instance.PlayerNodeDef.userDef.da2Asset[0] += msg.dResourceNum;
                    }
                    break;
                case 2:
                    {
                        str = "话费";
                        str1 = "元";
                        RpType = 4;
                        GameData.Instance.PlayerNodeDef.userDef.da2Asset[1] += msg.dResourceNum;
                    }
                    break;
                case 3:
                    {
                        str = "流量";
                        str1 = "M";
                        RpType = 1;
                        GameData.Instance.PlayerNodeDef.userDef.da2Asset[2] += msg.dResourceNum;
                    }
                    break;
                case 4:
                    {
                        str = "储值卡";
                        str1 = "元";
                        RpType = 2;
                        GameData.Instance.PlayerNodeDef.userDef.da2Asset[3] += msg.dResourceNum;
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
                        GameData.Instance.PlayerNodeDef.iBindCoin += msg.dResourceNum;
                    }
                    break;
            }
            int index = msg.byRedPagType - 1;
            RPContent rp2 = new RPContent();
            rp2.Image = UIMainView.Instance.RedPagePanel.RedPage[index].Image;
            rp2.Name = UIMainView.Instance.RedPagePanel.RedPage[index].Name;
            rp2.ShareNum = UIMainView.Instance.RedPagePanel.RedPage[index].ShareNum;
            rp2.CanUseNum = UIMainView.Instance.RedPagePanel.RedPage[index].CanUseNum;
            rp2.GetMoneyNum = str2;
            rp2.GetMoneyType = str;
            rp2.isShare = UIMainView.Instance.RedPagePanel.RedPage[index].isShare;
            rp2.RpNumber = UIMainView.Instance.RedPagePanel.RedPage[index].RpNumber;
            rp2.Describe = UIMainView.Instance.RedPagePanel.RedPage[index].Describe;
            UIMainView.Instance.RedPagePanel.RedPage[index] = rp2;

            UIMgr.GetInstance().ShowRedPagePanel.DirectOpenRedPagePanel(str2, RpType, rp2.Describe);// str, str1, rp2.Name);

            Debug.LogWarning("领取的红包：" + msg.dResourceNum.ToString() + str1 + str + "," + msg.byRedPagType);
            Debug.LogWarning("获得资源类型:1现金，2话费，3流量，4储值卡，（5代金券，6赠币没有对应字段）" + msg.byResourceType);

            SystemMgr.Instance.RedPageShowSystem.UpdateShow();
            //UIMainView.Instance.RedPagePanel.OnInitRedPage();
        }


        /// <summary>
        /// 通知用户获得一个红包消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientObtainRedNotice(byte[] pMsg, int len)
        {
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }
            if (SDKManager.Instance.IOSCheckStaus == 1) return;//送审

            int ioffset = 0;
            NetMsg.ClientObtainRedNoticeDef msg = new NetMsg.ClientObtainRedNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("通知用户获得一个红包消息长度不一致,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            //Debug.Log("通知用户获得一个红包消息" + msg.byRedPagType + "," + UIMainView.Instance.RedPagePanel.RedPage[msg.byRedPagType - 1].Name);
            if (msg.byRedPagType == 3)
            {
                UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 1, UIMainView.Instance.RedPagePanel.RedPage[msg.byRedPagType - 1].Name, RedPageShowPanel.NowState.Lobby, msg.iRp3ID);
            }
            else
            {
                //谈一个框出来显示
                UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedPagType - 1, 1, 2, UIMainView.Instance.RedPagePanel.RedPage[msg.byRedPagType - 1].Name, RedPageShowPanel.NowState.Lobby);
            }
        }

        /// <summary>
        /// 业绩兑换金币回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientBossScoreToCoinRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientScoreToCoinRes msg = new NetMsg.ClientScoreToCoinRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("三种兑换金币回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            if (msg.iError != 0)
            {
                Debug.LogError("三种兑换金币回应错误编号:" + msg.iError);
                if (msg.iError == 2)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("业绩不足，无法兑换");
                }
                else if (msg.iError == 3)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("现金不足，无法兑换");
                }
                else if (msg.iError == 4)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("储值卡不足，无法兑换");
                }
                return;
            }
            Debug.LogWarning("用户编号:" + msg.iUserId + ",兑换的金币ID：" + msg.iExchangeId);

            int coin = 0;
            int score = 0;
            if (msg.byExchange == 1)
            {
                for (int i = 0; i < MahjongCommonMethod.Instance.ExChange.Count; i++)
                {
                    if (MahjongCommonMethod.Instance.ExChange[i].iExchangeId == msg.iExchangeId)
                    {
                        coin = MahjongCommonMethod.Instance.ExChange[i].iCoin + MahjongCommonMethod.Instance.ExChange[i].iBindCoin;
                        score = MahjongCommonMethod.Instance.ExChange[i].iAsset;
                        break;
                    }
                }

            }
            else if (msg.byExchange == 2)
            {
                coin = MahjongCommonMethod.Instance._dicExchage2[msg.iExchangeId].iCoin + MahjongCommonMethod.Instance._dicExchage2[msg.iExchangeId].iBindCoin;
                score = MahjongCommonMethod.Instance._dicExchage2[msg.iExchangeId].iAsset;
                UIMainView.Instance.LobbyPanel.panel_wallet.panelCashToCoin._gPanelCashToCoin.SetActive(false);
                UIMainView.Instance.LobbyPanel.panel_wallet._gPanelWallet.SetActive(false);
                GameData.Instance.PlayerNodeDef.userDef.da2Asset[0] -= score * 100;
            }
            else if (msg.byExchange == 3)
            {

                coin = MahjongCommonMethod.Instance._dicExchage3[msg.iExchangeId].iCoin + MahjongCommonMethod.Instance._dicExchage3[msg.iExchangeId].iBindCoin;
                score = MahjongCommonMethod.Instance._dicExchage3[msg.iExchangeId].iAsset;
                UIMainView.Instance.LobbyPanel.panel_wallet.panelCashToCoin._gPanelCashToCoin.SetActive(false);
                UIMainView.Instance.LobbyPanel.panel_wallet._gPanelWallet.SetActive(false);
                GameData.Instance.PlayerNodeDef.userDef.da2Asset[3] -= score * 100;
            }

            GameData.Instance.PlayerNodeDef.iCoin += coin;

            //业绩积分扣除业绩积分
            if (msg.byExchange == 1)
            {
                GameData.Instance.PlayerNodeDef.userDef.fParlorScore -= score * 100;
            }


            UIMgr.GetInstance().GetUIMessageView().Show("恭喜您，成功兑换" + coin + "金币！");
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            if (msg.byExchange == 1)
            {
                //更新界面积分
                UIMainView.Instance.ParlorShowPanel.ExChangePanel.UpdateShow_Score();
                UIMainView.Instance.ParlorShowPanel.BossScore.text = (GameData.Instance.PlayerNodeDef.userDef.fParlorScore / 100f).ToString("0.00");
            }
        }

        /// <summary>
        /// 通知用户获得一个待领状态红包消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientObtainReceiveRedNotice(byte[] pMsg, int len)
        {
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                return;
            }
            int ioffset = 0;
            NetMsg.ClientObtainReceiveRedNoticeDef msg = new NetMsg.ClientObtainReceiveRedNoticeDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("业绩兑换金币回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }
            Debug.LogWarning("通知用户获得一个待领状态红包消息");
            UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(msg.byRedType - 1, 1, 1, UIMainView.Instance.RedPagePanel.RedPage[msg.byRedType - 1].Name, RedPageShowPanel.NowState.Lobby);
        }


        /// <summary>
        /// 取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleCAncelApplyOrJudgeApplyTooRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientCAncelApplyOrJudgeApplyTooRes msg = new NetMsg.ClientCAncelApplyOrJudgeApplyTooRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息:" + msg.iError);
                if (msg.iType == 2)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("取消加入馆的申请失败");
                }
                return;
            }

            //已经申请过
            if (msg.iType == 1)
            {
                //修改按钮状态
                UIMainView.Instance.ParlorShowPanel.SpwanPointParlorPanel(GameData.Instance.ParlorShowPanelData.InfoDef_PointParlor, msg.iStaus + 1);
            }


            //取消申请回应
            if (msg.iType == 2)
            {
                //如果雀神界面点击申请
                if (GameData.Instance.ParlorShowPanelData.isShowParlorRoundPanel)
                {
                    ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                    for (int i = 0; i < pspd.ApplyParlorId_All.Length; i++)
                    {
                        if (msg.iParlorId == pspd.ApplyParlorId_All[i])
                        {
                            pspd.ApplyParlorId_All[i] = 0;
                            break;
                        }
                    }

                    for (int i = 0; i < pspd.ApplyParlorMessage.Count; i++)
                    {
                        if (msg.iParlorId == pspd.ApplyParlorMessage[i].iParlorId)
                        {
                            pspd.ApplyParlorMessage.RemoveAt(i);
                            break;
                        }
                    }

                    if (pspd.isShowParlorRoundPanel)
                    {
                        //更新馆的加入申请界面信息
                        UIMainView.Instance.ParlorShowPanel.UpdateParlorStatus(msg.iParlorId, 0);
                    }
                }

                UIMgr.GetInstance().GetUIMessageView().Show("您已成功取消加入馆的申请");
                //获取玩家的申请馆的id
                GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("loading", "正在获取您的麻将馆信息");
            }
        }

        /// <summary>
        /// 玩家信息刷新按钮
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientRefreshUserRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientRefreshUserRes msg = new NetMsg.ClientRefreshUserRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("刷新用户信息回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            if (msg.iError != 0)
            {
                Debug.LogError("刷新用户信息回应消息错误编号:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }

            //Debug.LogError("用户的麻将馆信息:" + msg.clientUser.iaJoinParlorId[0] + "," + msg.clientUser.iaJoinParlorId[1] + ","
            //    + msg.clientUser.iaJoinParlorId[2] + "," + msg.clientUser.iaJoinParlorId[3] + ",老板积分:" + msg.clientUser.fParlorScore
            //    + ",是否进入过麻将馆:" + msg.clientUser.byFirstInParlor);

            //刷新玩家信息
            PlayerNodeDef pnd = new PlayerNodeDef(msg.clientUser);
            GameData.Instance.PlayerNodeDef = pnd;
            GameData.Instance.PlayerNodeDef.userDef = msg.clientUser;
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            SDKManager.Instance.isUpdateCompelet = true;
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");

        }

        /// <summary>
        /// 获取麻将馆内某一页的桌信息回应
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientGetParlorTableInfoRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientGetParlorTableInfoRes msg = new NetMsg.ClientGetParlorTableInfoRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            //添加创建房间的按钮
            if (pspd.iPageId_TabelGame == 0)
            {
                pspd.PointParlorTabelMessage_UnStart = new List<NetMsg.TableInfoDef>();
                NetMsg.TableInfoDef message = new NetMsg.TableInfoDef();
                message.iSeverId = 0;
                message.iRoomNum = 0;
                pspd.PointParlorTabelMessage_UnStart.Insert(0, message);
            }

            for (int i = 0; i < msg.iTableNum; i++)
            {
                NetMsg.TableInfoDef msgg = new NetMsg.TableInfoDef();
                ioffset = msgg.parseBytes(pMsg, ioffset);

                //Debug.LogError("服务器id:" + msgg.iSeverId + ",房间号:" + msgg.iRoomNum + ",开放人id:" + msgg.iOpenRoomId + "入座座位号:" + msgg.iaUserId[0]
                //    + "入座座位号:" + msgg.iaUserId[1] + "入座座位号:" + msgg.iaUserId[2] + "入座座位号:" + msgg.iaUserId[3]
                //    + "预约座位号:" + msgg.iaBespeakUserId[0] + "预约座位号:" + msgg.iaBespeakUserId[1] + "预约座位号:" + msgg.iaBespeakUserId[2] + "预约座位号:" + msgg.iaBespeakUserId[3]
                //    + "开放参数：" + msgg.param[0] + "," + msgg.param[1] + "," + msgg.param[2] + "," + msgg.param[3] + "," + msgg.param[4] + ","
                //    + msgg.param[5] + "," + msgg.param[6] + "," + msgg.param[7] + "," + msgg.param[8] + "," + msgg.param[9] + "," + msgg.param[10] + ","
                //    + msgg.param[11] + "," + msgg.param[12] + "," + msgg.param[13] + "," + msgg.param[14] + "," + msgg.param[15]);

                //处理一个麻将馆的桌的信息                
                pspd.PointParlorTabelMessage_UnStart.Add(msgg);
            }

            if (msg.iError != 0)
            {
                Debug.LogError("获取麻将馆内某一页的桌信息回应:" + msg.iError);
                MahjongCommonMethod.Instance.SendAuthenReq(msg.iError);
                return;
            }
            if (ioffset != len)
            {
                Debug.LogError("获取麻将馆内某一页的桌信息回应,ioffset:" + ioffset + ",len:" + len);
                return;
            }


            //当时第一页时，产生对应的未开始游戏的房间预置体
            if (pspd.iPageId_TabelGame == 0)
            {
                UIMainView.Instance.ParlorShowPanel.SpwanPorlorTabelMessage(2);
            }

            //如果这次大于等于24说明还有分页，添加页码
            if (msg.iTableNum >= 24)
            {
                pspd.iPageId_TabelGame++;
                pspd.isGetUnStartDataEnd = false;
            }
            else
            {
                //说明所有数据获取完毕
                pspd.isGetUnStartDataEnd = true;
            }
        }


        /// <summary>
        /// 红包状态回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientRp17TypeRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientRp17TypeRes msg = new NetMsg.ClientRp17TypeRes();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("红包状态回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (msg.iError != 0)
            {
                Debug.LogError("红包状态回应消息:" + msg.iError);
                UIMainView.Instance.ParlorShowPanel.UpdateParlorRedBagMessage_Res(pspd.iParlorId);
                return;
            }

            int parlorId = 0;
            for (int i = 0; i < pspd.parlorRedBagInfo.Length; i++)
            {
                if (pspd.parlorRedBagInfo[i].rpId == msg.iRp17Id)
                {
                    parlorId = pspd.parlorRedBagInfo[i].parlorId;
                    pspd.parlorRedBagInfo[i].state = msg.byRp17Type;
                    Debug.LogWarning("msg.byRp17Type：" + msg.byRp17Type);
                    break;
                }
            }
            UIMainView.Instance.ParlorShowPanel.UpdateParlorRedBagMessage_Res(parlorId);
        }

        /// <summary>
        /// 用户取消占座回应消息
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="len"></param>
        void HandleClientBespeakRes(byte[] pMsg, int len)
        {
            int ioffset = 0;
            NetMsg.ClientUserCancleBespeakResDef msg = new NetMsg.ClientUserCancleBespeakResDef();
            ioffset = msg.parseBytes(pMsg, ioffset);
            if (ioffset != len)
            {
                Debug.LogError("用户取消占座回应消息,ioffset:" + ioffset + ",len:" + len);
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (msg.iError != 0)
            {
                Debug.LogError("用户取消占座回应消息:" + msg.iError);
                return;
            }

            pspd.isShowOrderTimePanel = false;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
        }

        #endregion 回应调用方法

        #region  发送网络消息

        //KEEP_ALIVE_MSG = 0xFF; //保持连接
        public void SendKeepAlive()
        {
            NetMsg.KeepAlive msg = new NetMsg.KeepAlive();
            SendClientMsg(msg, NetMsg.KEEP_ALIVE, false);
        }

        //CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public void SendAuthenReq(NetMsg.ClientAuthenReq msg)
        {
            msg.szRegistImei = "NOIMEI";
            if (!NetworkMgr.Instance.LobbyServer.Connected)
            {
                return;
            }
            MahjongCommonMethod.Instance.isFinshSceneLoad = true;
            Debug.LogWarning("经纬度:" + msg.fLongitude + "," + msg.fLatitude + ",ip:" + msg.szIp + ",玩家id:" + msg.iUserId);
            SendClientMsg(msg, NetMsg.CLIENT_AUTHEN_REQ, false);
        }

        //CLIENT_CHANGE_USER_INFO_RES =0x0220 //[厅服]->[厅客]或[计服]->[厅服]修改用户信息请求消息
        public void SendChangeUserInfo(NetMsg.ClientChangeUserInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CHANGE_USER_INFO_REQ, false);
        }

        //CLIENT_FULL_NAME_REQ					0x0222	// [厅客]->[厅服]实名认证请求消息
        public void SendFullNameReq(NetMsg.ClientFullNameReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_FULL_NAME_REQ, false);
        }
        //CLIENT_FULL_NAME_REQ					0x0222	// [厅客]->[厅服]实名认证请求消息
        public void SendGetExchangeCoinReq(NetMsg.ClientGetExchangeCoinReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GETEXCHANGE_COIN_REQ, false);
        }

        //CLIENT_SPREAD_CODE_REQ					0x0230	// [厅客]->[厅服]使用推广码请求消息
        public void SendSpreadCode(NetMsg.ClientSpreadCodeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SPREAD_CODE_REQ, false);
        }

        //CLIENT_SPREAD_GIFT_REQ = 0x0232;	// [厅客]->[厅服]推广礼包请求消息
        public void SendSpreadGift(NetMsg.ClientSpreadGiftReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SPREAD_GIFT_REQ, false);
        }

        //BULLETIN_REQ		0x0013[厅服/游服]->[告服]公告请求消息
        public void SendBulletinReq(NetMsg.BulletinReq msg)
        {
            SendClientMsg(msg, NetMsg.BULLETIN_REQ, false);
        }

        ////CLIENT_BIND_PROXY_REQ					0x0240	// [厅客]->[厅服]绑定代理请求消息
        //public void SendBindProxyReq(NetMsg.ClientBindProxyReq msg)
        //{
        //    SendClientMsg(msg, NetMsg.CLIENT_BIND_PROXY_REQ, false);
        //}

        ////CLIENT_ASK_UNBIND_PROXY_REQ				0x0242	// [厅客]->[厅服]申请解绑代理请求消息
        //public void SendUnbindProxy(NetMsg.ClientAskUnbindProxyReq msg)
        //{
        //    SendClientMsg(msg, NetMsg.CLIENT_ASK_UNBIND_PROXY_REQ, false);
        //}

        //CLIENT_MESSAGE_OPERATE_REQ					0x023D	// [厅服]->[厅客]信息操作请求消息
        public void SendMessageOperateReq(NetMsg.ClientMessageOperateReq msg)
        {
            Debug.LogWarning("玩家信息：" + msg.iUserId + ",iMessageId:" + msg.iMessageId + ",cMessageType:" + msg.cMessageType
                + ",cOperate:" + msg.cOperate);

            SendClientMsg(msg, NetMsg.CLIENT_MESSAGE_OPERATE_REQ, false);
        }

        ////CLIENT_PROXY_INFO_REQ					0x0244	// [厅客]->[厅服]或[厅服]->[计服]代理信息请求消息
        //public void SendProxyInfoReq(NetMsg.ClientProxyInfoReq msg)
        //{
        //    SendClientMsg(msg, NetMsg.CLIENT_PROXY_INFO_REQ, false);
        //}

        //CLIENT_CITY_COUNTY_REQ 0x0224;	// [厅客]->[厅服]选择市县请求消息
        public void SendCityCountyReq(NetMsg.ClientCityCountyReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CITY_COUNTY_REQ, false);
        }

        //CLIENT_OPEN_ROOM_REQ		0x0260	// [厅客]->[厅服]开房请求消息
        public void SendOpenRoomReq(NetMsg.ClientOpenRoomReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_OPEN_ROOM_REQ, false);
        }

        //CLIENT_OPEN_ROOM_INFO_REQ		0x0262	// [厅客]->[厅服]开房信息请求消息
        public void SendOpenRoomInfoReq(NetMsg.ClientOpenRoomInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_OPEN_ROOM_INFO_REQ, false);
        }

        //CLIENT_MY_ROOM_INFO_REQ   0x0265	// [厅客]->[厅服]我的开房信息请求消息
        public void SendMyRoomInfoReq(NetMsg.ClientMyRoomInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_MY_ROOM_INFO_REQ, false);
        }

        //CLIENT_GAME_SERVER_INFO_REQ		0x0270	// [厅客]->[厅服]游戏服务器信息请求消息
        public void SendGameSeverInfoReq(NetMsg.ClientGameServerInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GAME_SERVER_INFO_REQ, false);
        }

        //CLIENT_COMPLIMENT_REQ		0x0280	// [厅客]->[厅服]点赞请求消息
        public void SendComplimentReq(NetMsg.ClientComplimentReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_COMPLIMENT_REQ, false);
        }
        /// <summary>
        ///  CLIENT_HOLIDAY_REQ = 0x0290;	// [厅客]->[厅服]节日信息请求消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendHolidayReq(NetMsg.ClientHolidayReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_HOLIDAY_REQ, false);
        }
        /// <summary>
        ///  CLIENT_HOLIDAY_GIFT_REQ = 0x0292;   // [厅客]->[厅服]领取节日活动奖励请求消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendHolidayGiftReq(NetMsg.ClientHolidayGiftReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_HOLIDAY_GIFT_REQ, false);
        }
        /// <summary>
        ///  CLIENT_FREE_TIME_REQ = 0x0294;// [厅客]->[厅服]免费时间信息请求消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendFreeTimeReq(NetMsg.ClientFreeTimeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_FREE_TIME_REQ, false);
        }
        //CLIENT_SHARE_REQ = 0x0296;//[厅客]->[厅服]分享活动信息请求消息
        public void SendShareReq(NetMsg.ClientShareReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SHARE_REQ, false);
        }
        //CLIENT_SHARE_USER_REQ  = 0x0298 [厅客]->[厅服]分享活动用户信息请求消息（大厅客户端无需请求
        public void SendShareUserReq(NetMsg.ClientShareUserReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SHARE_USER_REQ, false);
        }
        public void SendClientSpreaderInfoReq(NetMsg.ClientSpreaderInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SPREADER_INFO_REQ, false);
        }
        //CLIENT_SHARE_SUCCESS_REQ = 0x029A;	// [厅客]->[厅服]分享成功请求消息
        public void SendShareSuccessReq(NetMsg.ClientShareSuccessReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_SHARE_SUCCESS_REQ, false);
        }
        // CLIENT_USE_ACTIVE_CODE_REQ = 0x029C;	// [厅客]->[厅服]使用激活码请求消息
        public void SendUseActiveCodeReq(NetMsg.ClientUseActiveCodeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_USE_ACTIVE_CODE_REQ, false);
        }
        //创建订单请求
        public void SendCreateOrderReq(NetMsg.ClientCreateOrderReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CREATE_ORDER_REQ, false);
        }
        //充值请求
        public void SendChargeReq(NetMsg.ClientChargeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CHARGE_REQ, false);
        }
        public void SendUserChargeReq(NetMsg.ClientUserChargeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_USER_CHARGE_REQ, false);
        }
        //CLIENT_ROOM_NUM_SERVER_TABLE_REQ		0x0268	// [厅客]->[厅服]房间号的服务器和桌信息请求消息
        public void SendRoomNumSeverTableReq(NetMsg.ClientRoomNumServerTableReq msg)
        {
           // Debug.LogError(msg.iRoomNum);
            SendClientMsg(msg, NetMsg.CLIENT_ROOM_NUM_SERVER_TABLE_REQ, false);
        }

        //#define CLIENT_LOTTERY_COUNT_REQ					0x110A	//[厅客]->[厅服]增加抽奖次数请求
        public void SendLotteryCountReq(NetMsg.ClientLotteryCountReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_LOTTERY_COUNT_REQ, false);
        }
        //#define CLIENT_LOTTERY_REQ							0x110C	//[厅客]->[厅服]抽奖请求消息
        public void SendLotteryReq(NetMsg.ClientLotteryReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_LOTTERY_REQ, false);
        }
        //#define CLIENT_USER_LOTTERY_REQ				0x1110	// [厅客]->[厅服]用户抽奖活动信息请求消息
        public void SendUserLotteryReq(NetMsg.ClientUserLotteryReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_USER_LOTTERY_REQ, false);
        }

        //#define CLIENT_CREATE_DISMISS_PARLOR_REQ			0x0240	// [厅客/游客]->[厅服/游服]创建麻将馆请求消息
        public void SendCreateParlorReq(NetMsg.ClientCreateParlorReq msg)
        {
            Debug.LogWarning("名字:" + msg.cParlorName);
            SendClientMsg(msg, NetMsg.CLIENT_CREATE_PARLOR_REQ, false);
        }

        //#define CLIENT_DISMISS_PARLOR_REQ				0x0242	// [厅客/游客]->[厅服/游服]解散麻将馆请求消息
        public void SendDismissParlorReq(NetMsg.ClientDismissParlorReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_DISMISS_PARLOR_REQ, false);
        }

        //CLIENT_CHANGE_PARLOR_BULLETIN_INFO_REQ			0x0244	// [厅客/游客]->[厅服/游服]修改麻将馆公告信息请求消息
        public void SendChangeParlorBulletinInfoReq(NetMsg.ClientChangeParlorBulletinInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CHANGE_PARLOR_BULLETIN_INFO_REQ, false);
        }

        //CLIENT_CHANGE_PARLOR_CONTACT_INFO_REQ			0x0246	// [厅客/游客]->[厅服/游服]修改麻将馆联系信息请求消息
        public void SendChangeParlorContactInfoReq(NetMsg.ClientChangeParlorContactInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CHANGE_PARLOR_CONTACT_INFO_REQ, false);
        }

        //CLIENT_JOIN_PARLOR_REQ					0x0248	// [厅客]->[厅服]申请 加入麻将馆请求消息
        public void SendJoinParlorReq(NetMsg.ClientJoinParlorReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_JOIN_PARLOR_REQ, false);
        }

        // CLIENT_LEAVE_PARLOR_REQ					0x024A	// [厅客]->[厅服]申请 退出麻将馆请求消息
        public void SendLevelParlorReq(NetMsg.ClientLeaveParlorReq msg)
        {
            Debug.LogWarning("用户id：" + msg.iUserId + ",馆id:" + msg.iParlorId);
            SendClientMsg(msg, NetMsg.CLIENT_LEAVE_PARLOR_REQ, false);
        }

        // CLIENT_INVITE_PARLOR_REQ				0x024C	// [厅客]->[厅服]邀请用户进入麻将馆请求消息
        public void SendInvifeParlorReq(NetMsg.ClientInvitParlorReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_INVITE_PARLOR_REQ, false);
        }


        // CLIENT_KICK_PARLOR_REQ					0x024E	// [厅客]->[厅服]踢用户出麻将馆请求消息
        public void SendKickParlorReq(NetMsg.ClientKickParlorReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_KICK_PARLOR_REQ, false);
        }

        // CLIENT_GETPARLORINFO_REQ				0x0255	// [厅客]->[厅服]获取麻将馆信息请求消息
        public void SendGetParlorInfoReq(NetMsg.ClientGetParlorInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GETPARLORINFO_REQ, false);
        }

        // CLIENT_PARLOR_CERT_REQ					0x02D0	// [厅客]->[厅服]是否有开麻将馆权限请求消息
        public void SendCreatParlorCertReq(NetMsg.ClientParlorCertReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_PARLOR_CERT_REQ, false);
        }

        //用户占座请求消息
        public void SendClientUserBespeakReq(NetMsg.ClientUserBespeakReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_BESPEAK_REQ, false);
        }

        //获取用户信息请求消息
        public void SendClientGetUseInfoReq(NetMsg.ClientGetUserInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GETUSERINFO_REQ, false);
        }
        //获取用户信息请求消息
        public void SendClientGetUseIdReq(NetMsg.ClientGetTableUserIDReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GET_TABLE_USERID_REQ, false);
        }

        //获取桌上玩家信息请求消息
        public void SendClientGetTableUseInfoReq(NetMsg.ClientGetTableUserInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GET_TABLE_USER_INFO_REQ, false);
        }
        //所有红包数量请求消息
        public void SendClientRedNumReq(NetMsg.ClientRedNumReqDef msg)
        {
            if (SDKManager.Instance.IOSCheckStaus == 0 && Network.NetworkMgr.Instance.LobbyServer.Connected)
            {
                SendClientMsg(msg, NetMsg.CLIENT_RED_NUM_REQ, false);
            }
            //#if UNITY_EDITOR
            //            SendClientMsg(msg, NetMsg.CLIENT_RED_NUM_REQ, false);
            //#else
            //            //游客不给红包
            //            if (SDKManager.Instance.IOSCheckStaus == 0 && Network.NetworkMgr.Instance.LobbyServer.Connected && GameData.Instance.PlayerNodeDef.byUserSource == 2)
            //            {
            //                SendClientMsg(msg, NetMsg.CLIENT_RED_NUM_REQ, false);
            //            }
            //#endif
        }

        //领取红包请求消息
        public void SendClientReceiveRedReq(NetMsg.ClientOpenReceiveRedReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_OPEN_RECEIVE_RED_REQ, false);
        }


        //业绩兑换金币请求消息
        public void SendClientScoreToCoinReq(NetMsg.ClientScoreToCoinReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_BOSSSCORE_TO_COIN_REQ, false);
        }

        //取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆请求消息
        public void SendClientCAncelApplyOrJudgeApplyTooReq(NetMsg.ClientCAncelApplyOrJudgeApplyTooReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_REQ, false);
        }

        //CLIENT_REFRESH_USER_REQ					0x0226	// [厅客]->[厅服]刷新用户信息请求消息
        public void SendClientRefreshUserReq(NetMsg.ClientRefreshUserReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_REFRESH_USER_REQ, false);
        }

        //CLIENT_GET_PARLOR_TABLEINFO_REQ		0x0259	//	[厅客]->[厅服]获取麻将馆内某一页的桌信息
        public void SendClientGetParlorTableInfoReq(NetMsg.ClientGetParlorTableInfoReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_GET_PARLOR_TABLEINFO_REQ, false);
        }

        //CLIENT_RP17_TYPE_REQ   0x1127	 //[厅客]->[厅服] 获取红包状态请求消息
        public void SendClientRp17TypeReq(NetMsg.ClientRp17TypeReq msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_RP17_TYPE_REQ, false);
        }

        //CLIENT_CANCLE_BESPEAK_REQ				0x1114	// [游客]->[游服]用户取消占座请求消息
        public void SendClientUserCancleBespeakReq(NetMsg.ClientUserCancleBespeakReqDef msg)
        {
            SendClientMsg(msg, NetMsg.CLIENT_CANCLE_BESPEAK_REQ, false);
        }
        #endregion 发送网络消息

    }
}

