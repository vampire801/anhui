using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XLua;

namespace MahjongLobby_AH.Network.Message
{
    [Hotfix]
    [LuaCallCSharp]
    public class NetMsg
    {
        #region 消息编号

        /// <summary>
        /// 全局的消息
        /// </summary>

        public const sbyte MESSAGE_VERSION = 0x4D; //消息版本        
        public const byte CLIENT_CONNECT = 0x0001; //客户端连接消息
        public const byte CLIENT_DISCONNECT = 0x0002; //客户端断开连接消息
        public const byte SERVER_CONNECT = 0x0003; //服务器连接消
        public const byte SERVER_DISCONNECT = 0x0004; // 服务器断开连接消息
        public const byte SERVER_RECONNECT = 0x0005;  // 服务器重连成功消息
        public const byte KEEP_ALIVE = 0x0006; //保持连接
        public const byte KICK_USER_NOTICE = 0x0007;  //玩家被踢下线
        public const byte BULLETIN_REQ = 0x0013;  // [厅服/游服]->[告服]公告请求消息
        public const byte BULLETIN_NOTICE = 0x0014;  //[厅服/游服]->[厅客/游客]或[厅服/游服]<->[告服]公告通知消息

        public const ushort CLIENT_SERVER_REQ = 0x0100; //[厅客/游客]->[厅网/游网]大厅/游戏客户端请求大厅/游戏网关服务器的请求消息
        public const ushort CLIENT_SERVER_RES = 0x0101; //[厅网/游网]->[厅客/游客]大厅/游戏客户端请求大厅/游戏网关服务器的回应消息
        public const ushort CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public const ushort CLIENT_AUTHEN_RES = 0x0211;// [厅服/游服]->[厅客/游客]认证回应消息


        public const ushort CLIENT_CHANGE_USER_INFO_REQ = 0x0220;   // [厅客]->[厅服]或[厅服]->[计服]修改用户信息请求消息
        public const ushort CLIENT_CHANGE_USER_INFO_RES = 0x0221;	// [厅服]->[厅客]或[计服]->[厅服]修改用户信息回应消息
        public const ushort CLIENT_FULL_NAME_REQ = 0x0222;  // [厅客]->[厅服]实名认证请求消息
        public const ushort CLIENT_FULL_NAME_RES = 0x0223;	// [厅客]->[厅服]实名认证回应消息
        public const ushort CLIENT_CITY_COUNTY_REQ = 0x0224;	// [厅客]->[厅服]选择市县请求消息
        public const ushort CLIENT_CITY_COUNTY_RES = 0x0225;    // [厅客]->[厅服]选择市县回应消息

        public const ushort CLIENT_GETEXCHANGE_COIN_REQ = 0x0228;// [厅客]->[厅服]读取充值金币配置请求消息
        public const ushort CLIENT_GETEXCHANGE_COIN_RES = 0x0229;//	[厅服]->[厅客]读取充值金币配置回应消息

        public const ushort CLIENT_SPREAD_CODE_REQ = 0x0230;	// [厅客]->[厅服]使用推广码请求消息
        public const ushort CLIENT_SPREAD_CODE_RES = 0x0231;    // [厅客]->[厅服]使用推广码回应消息       

        public const ushort CLIENT_SPREAD_GIFT_REQ = 0x0232;	// [厅客]->[厅服]推广礼包请求消息
        public const ushort CLIENT_SPREAD_GIFT_RES = 0x0233;    // [厅服]->[厅客]推广礼包回应消息

        //public const ushort CLIENT_BIND_PROXY_REQ = 0x0010;	// [厅客]->[厅服]绑定代理请求消息
        //public const ushort CLIENT_BIND_PROXY_RES = 0x0241;	// [厅服]->[厅客]绑定代理回应消息
        //public const ushort CLIENT_ASK_UNBIND_PROXY_REQ = 0x0242;// [厅客]->[厅服]申请解绑代理请求消息
        //public const ushort CLIENT_ASK_UNBIND_PROXY_RES = 0x0243;	// [厅服]->[厅客]申请解绑代理回应消息
        //public const ushort CLIENT_PROXY_INFO_REQ = 0x0244;	// [厅客]->[厅服]或[厅服]->[计服]代理信息请求消息
        //public const ushort CLIENT_PROXY_INFO_RES = 0x0245;	// [厅服]->[厅客]或[计服]->[厅服]代理信息回应消息

        public const ushort CLIENT_MESSAGE_NOTICE = 0x0250;// [厅服]->[厅客]信息通知
        public const ushort CLIENT_MESSAGE_OPERATE_REQ = 0x0251;	// [厅服]->[厅客]信息操作请求消息
        public const ushort CLIENT_MESSAGE_OPERATE_RES = 0x0252;    // [厅服]->[厅客]信息操作回应消息

        public const ushort CLIENT_OPEN_ROOM_REQ = 0x0260;	// [厅客]->[厅服]创建房间请求消息
        public const ushort CLIENT_OPEN_ROOM_RES = 0x0261;	// [厅服]->[厅客]创建房间回应消息
        public const ushort CLIENT_OPEN_ROOM_INFO_REQ = 0x0262;	// [厅客]->[厅服]开房信息请求消息
        public const ushort CLIENT_OPEN_ROOM_INFO_RES = 0x0263;	// [厅服]->[厅客]开房信息回应消息
        public const ushort CLIENT_OPEN_ROOM_INFO_NOTICE = 0x0264;	// [厅客]->[厅服]开房信息通知消息
        public const ushort CLIENT_MY_ROOM_INFO_REQ = 0x0265;	// [厅客]->[厅服]我的开房信息请求消息
        public const ushort CLIENT_MY_ROOM_INFO_RES = 0x0266;	// [厅服]->[厅客]我的开房信息回应消息

        public const ushort CLIENT_GAME_SERVER_INFO_REQ = 0x0270;	// [厅客]->[厅服]游戏服务器信息请求消息
        public const ushort CLIENT_GAME_SERVER_INFO_RES = 0x0271;	// [厅服]->[厅客]游戏服务器信息回应消息

        public const ushort CLIENT_COMPLIMENT_REQ = 0x0280;	// [厅客]->[厅服]点赞请求消息
        public const ushort CLIENT_COMPLIMENT_RES = 0x0281;	// [厅服]->[厅客]点赞回应消息

        public const ushort CLIENT_HOLIDAY_REQ = 0x0290;	// [厅客]->[厅服]节日信息请求消息
        public const ushort CLIENT_HOLIDAY_RES = 0x0291;// [厅服]->[厅客]节日信息回应消息
        public const ushort CLIENT_HOLIDAY_GIFT_REQ = 0x0292;   // [厅客]->[厅服]领取节日活动奖励请求消息
        public const ushort CLIENT_HOLIDAY_GIFT_RES = 0x0293;   // [厅服]->[厅客]领取节日活动奖励回应消息
        public const ushort CLIENT_FREE_TIME_REQ = 0x0294;// [厅客]->[厅服]免费时间信息请求消息
        public const ushort CLIENT_FREE_TIME_RES = 0x0295;// [厅服]->[厅客]免费时间信息回应消息
        public const ushort CLIENT_SHARE_REQ = 0x0296;//[厅客]->[厅服]分享活动信息请求消息
        public const ushort CLIENT_SHARE_RES = 0x0297;	// [厅服]->[厅客]分享活动信息回应消息
        public const ushort CLIENT_SHARE_USER_REQ = 0x0298;	// [厅客]->[厅服]分享活动用户信息请求消息（大厅客户端无需请求
        public const ushort CLIENT_SHARE_USER_RES = 0x0299;// [厅服]->[厅客]分享活动用户信息回应消息
        public const ushort CLIENT_SHARE_SUCCESS_REQ = 0x029A;	// [厅客]->[厅服]分享成功请求消息
        public const ushort CLIENT_SHARE_SUCCESS_RES = 0x029B;	// [厅服]->[厅客]分享成功回应消息
        public const ushort CLIENT_USE_ACTIVE_CODE_REQ = 0x029C;    // [厅客]->[厅服]使用激活码请求消息

        public const ushort CLIENT_CREATE_ORDER_REQ = 0x02A0;	// [厅客]->[厅服]创建订单请求消息
        public const ushort CLIENT_CREATE_ORDER_RES = 0x02A1;// [厅服]->[厅客]创建订单回应消息
        public const ushort CLIENT_CHARGE_REQ = 0x02A2;// [厅客]->[厅服]充值请求消息
        public const ushort CLIENT_CHARGE_RES = 0x02A3;// [厅服]->[厅客]充值请求的回应消息
        public const ushort CLIENT_USER_CHARGE_REQ = 0x02B0;// [厅客]->[厅服]用户充值信息请求消息
        public const ushort CLIENT_USER_CHARGE_RES = 0x02B1;// [厅服]->[厅客]用户充值信息回应消息

        public const ushort CLIENT_USE_ACTIVE_CODE_RES = 0x029D;// [厅服]->[厅客]使用激活码回应消息

        public const ushort CLIENT_CARD_PAY_NOTICE = 0x0267;	// [游服]->[桌服]/[桌服]->[厅服]代开房支付房卡通知消息
        public const ushort CLIENT_ROOM_NUM_SERVER_TABLE_REQ = 0x0268;	// [厅客]->[厅服]房间号的服务器和桌信息请求消息
        public const ushort CLIENT_ROOM_NUM_SERVER_TABLE_RES = 0x0269;	// [厅服]->[厅客]房间号的服务器和桌信息回应消息
        public const ushort CLIENT_SPREADER_INFO_REQ = 0x0234;// [厅客]->[厅服]推广员信息请求消息
        public const ushort CLIENT_SPREADER_INFO_RES = 0x0235;//[厅服]->[厅客]推广员信息回应消息


        public const ushort CLIENT_LOTTERY_COUNT_REQ = 0x02C0;	//[厅客]->[厅服]增加抽奖次数请求
        public const ushort CLIENT_LOTTERY_COUNT_RES = 0x02C1;	//[厅服]->[厅客]增加抽奖次数回应
        public const ushort CLIENT_LOTTERY_REQ = 0x02C2;	//[厅客]->[厅服]抽奖请求消息
        public const ushort CLIENT_LOTTERY_RES = 0x02C3;	//[厅服]->[厅客]抽奖回应消息
        public const ushort CLIENT_USER_LOTTERY_REQ = 0x02C4;   // [厅客]->[厅服]用户抽奖活动信息请求消息
        public const ushort CLIENT_USER_LOTTERY_RES = 0x02C5;   // [厅服]->[厅客]用户抽奖活动信息回应消息


        public const ushort CLIENT_BESPEAK_REQ = 0x1112;    //[游客]->[游服] 用户占座请求消息
        public const ushort CLIENT_BESPEAK_RES = 0x1113;    //[游服]->[游客] 用户占座回应消息

        public const ushort CLIENT_GET_TABLE_USERID_REQ = 0x0257;	// [厅客]->[厅服]获取桌上用户信息请求消息
        public const ushort CLIENT_GET_TABLE_USERID_RES = 0x0258; // [厅服]->[厅客]获取桌上用户信息回应消息

        public const ushort CLIENT_GET_TABLE_USER_INFO_REQ = 0x025B;	// [厅客]->[厅服]获取用户信息请求消息
        public const ushort CLIENT_GET_TABLE_USER_INFO_RES = 0x025C; // [厅服]->[厅客]获取用户信息回应消息

        public const ushort CLIENT_RED_NUM_REQ = 0x02E0;	// [厅客]->[厅服]所有红包数量请求消息
        public const ushort CLIENT_RED_NUM_RES = 0x02E1;	// [厅服]->[厅客]所有红包数量回应消息
        public const ushort CLIENT_OPEN_RECEIVE_RED_REQ = 0x02E2;	// [厅客]->[厅服]领取红包请求消息
        public const ushort CLIENT_OPEN_RECEIVE_RED_RES = 0x02E3;	// [厅服]->[厅客]领取红包回应消息

        public const ushort CLIENT_OBTAIN_RED_NOTICE = 0x02E4;	// [厅服]->[厅客]通知用户获得一个红包消息
        public const ushort CLIENT_OBTAIN_RECEIVE_RED_NOTICE = 0x02E5;	// [厅服 ]->[厅客]通知用户获得一个待领状态红包消息        
        public const ushort CLIENT_REFRESH_USER_REQ = 0x0226;	// [厅客]->[厅服]刷新用户信息请求消息
        public const ushort CLIENT_REFRESH_USER_RES = 0x0227;	// [厅客]->[厅服][厅客]->[厅服]刷新用户信息回应消息
        #region 麻将馆相关消息
        public const ushort CLIENT_CREATE_PARLOR_REQ = 0x0240;	// [厅客/游客]->[厅服/游服]创建麻将馆请求消息
        public const ushort CLIENT_CREATE_PARLOR_RES = 0x0241;	// [厅服]->[厅客]创建麻将馆回应消息
        public const ushort CLIENT_DISMISS_PARLOR_REQ = 0x0242;	// [厅客/游客]->[厅服/游服]解散麻将馆请求消息
        public const ushort CLIENT_DISMISS_PARLOR_RES = 0x0243;	// [厅客/游客]->[厅服/游服]解散麻将馆回应消息
        public const ushort CLIENT_CHANGE_PARLOR_BULLETIN_INFO_REQ = 0x0244;	// [厅客/游客]->[厅服/游服]修改麻将馆公告信息请求消息
        public const ushort CLIENT_CHANGE_PARLOR_BULLETIN_INFO_RES = 0x0245;	// [厅服]->[厅客]修改麻将馆公告信息回应消息
        public const ushort CLIENT_CHANGE_PARLOR_CONTACT_INFO_REQ = 0x0246;	// [厅客/游客]->[厅服/游服]修改麻将馆联系信息请求消息
        public const ushort CLIENT_CHANGE_PARLOR_CONTACT_INFO_RES = 0x0247;	// [厅服]->[厅客]修改麻将馆联系信息回应消息
        public const ushort CLIENT_JOIN_PARLOR_REQ = 0x0248;	// [厅客]->[厅服]申请 加入麻将馆请求消息
        public const ushort CLIENT_JOIN_PARLOR_RES = 0x0249;	// [厅服]->[厅客]申请 加入麻将馆回应消息
        public const ushort CLIENT_LEAVE_PARLOR_REQ = 0x024A;	// [厅客]->[厅服]申请 退出麻将馆请求消息
        public const ushort CLIENT_LEAVE_PARLOR_RES = 0x024B;	// [厅客]->[厅服]申请 退出麻将馆回应消息
        public const ushort CLIENT_INVITE_PARLOR_REQ = 0x024C;	// [厅客]->[厅服]邀请用户进入麻将馆请求消息
        public const ushort CLIENT_INVITE_PARLOR_RES = 0x024D;	// [厅客]->[厅服]邀请用户进入麻将馆回应消息
        public const ushort CLIENT_KICK_PARLOR_REQ = 0x024E;	// [厅客]->[厅服]踢用户出麻将馆请求消息
        public const ushort CLIENT_KICK_PARLOR_RES = 0x024F;    // [厅服]->[厅客]踢用户出麻将馆回应消息   

        public const ushort CLIENT_GETPARLORINFO_REQ = 0x0255;	// [厅客]->[厅服]获取麻将馆信息请求消息
        public const ushort CLIENT_GETPARLORINFO_RES = 0x0256;	// [厅服]->[厅客]获取麻将馆信息回应消息
        public const ushort CLIENT_PARLOR_CERT_REQ = 0x02D0;	// [厅客]->[厅服]是否有开麻将馆权限请求消息
        public const ushort CLIENT_PARLOR_CERT_RES = 0x02D1;	// [厅客]->[厅服]是否有开麻将馆权限回应消息
        public const ushort CLIENT_GETUSERINFO_REQ = 0x0253;	// [厅客]->[厅服]获取用户信息请求消息
        public const ushort CLIENT_GETUSERINFO_RES = 0x0254;    // [厅服]->[厅客]获取用户信息回应消息        

        public const ushort CLIENT_BOSSSCORE_TO_COIN_REQ = 0X02D2;	//[厅客]->[厅服]业绩兑换金币请求消息
        public const ushort CLIENT_BOSSSCORE_TO_COIN_RES = 0X02D3;	//[厅服]->[厅客]业绩兑换金币回应消息
        public const ushort CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_REQ = 0X02D4;	//[厅客]->[厅服 计服]取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆请求消息
        public const ushort CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_RES = 0X02D5;    //[厅服]->[厅客]取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息

        public const ushort CLIENT_GET_PARLOR_TABLEINFO_REQ = 0x0259;//	[厅客]->[厅服]获取麻将馆内某一页的桌信息
        public const ushort CLIENT_GET_PARLOR_TABLEINFO_RES = 0X025A;	//	[厅服]->[厅客]获取麻将馆内某一页的桌信息回应
        public const ushort CLIENT_RP17_TYPE_REQ = 0x1127;	//[厅客]->[厅服] 获取红包状态请求消息
        public const ushort CLIENT_RP17_TYPE_RES = 0x1128;	//[厅服]->[厅客] 红包状态回应消息
        public const ushort CLIENT_CANCLE_BESPEAK_RES = 0x1115; //[游服]->[游客] 用户取消占座回应消息
        public const ushort CLIENT_CANCLE_BESPEAK_REQ = 0x1114;	// [游客]->[游服]用户取消占座请求消息
        #endregion

        #endregion 消息编号


        //消息头
        //[StructLayout(LayoutKind.Sequential)]
        public class MsgHeadDef : Protocol
        {
            public sbyte Version; //版本
            public sbyte MsgFromType; //消息来源类型
            public ushort MsgType; //消息类型
            public ushort SocketIndex;	//用于发送的SocketIndex                     
            public ushort Flag; //预留，对于RADIUS和LOGIN_SERVER之间有保存用户和LOGIN_SERVER的SocketIndex         

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                stream.writeByte((byte)Version);
                stream.writeByte((byte)MsgFromType);
                stream.writeShort((short)MsgType);
                stream.writeShort((short)SocketIndex);
                stream.writeShort((short)Flag);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;

                FieldStream stream = new FieldStream(bytes, begin);

                Version = (sbyte)stream.readByte();
                MsgFromType = (sbyte)stream.readByte();
                MsgType = (ushort)stream.readShort();
                SocketIndex = (ushort)stream.readShort();
                Flag = (ushort)stream.readShort();
                return stream.currentPos();
            }

            public void toBytes(FieldStream stream)
            {
                stream.writeByte((byte)Version);
                stream.writeByte((byte)MsgFromType);
                stream.writeShort((short)MsgType);
                stream.writeShort((short)SocketIndex);
                stream.writeShort((short)Flag);
            }

            public void parseBytes(FieldStream stream)
            {
                Version = (sbyte)stream.readByte();
                MsgFromType = (sbyte)stream.readByte();
                MsgType = (ushort)stream.readShort();
                SocketIndex = (ushort)stream.readShort();
                Flag = (ushort)stream.readShort();
            }
        }

        //KEEP_ALIVE_MSG = 0xFF; //保持连接
        //[StructLayout(LayoutKind.Sequential)]
        public class KeepAlive : Protocol
        {
            public MsgHeadDef MsgHeadInfo = new MsgHeadDef();
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                MsgHeadInfo.toBytes(stream);

                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }

        }

        //[用服]->[厅服/游服]或[厅服/游服]->[厅客/游客]踢用户下线的通知消息
        public class KickUserNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iReason; // 被踢原因，1重复登录，2GM踢人，3每日输赢过大，4不满足进入游戏服务器条件

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                iReason = stream.readInt();
                return stream.currentPos();
            }
        }

        //SERVER_DISCONNECT		0x0004 服务器断开连接消息
        public class ServerDisconnect : Protocol
        {
            MsgHeadDef msgHeadInfo = new MsgHeadDef();

            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);

                return stream.currentPos();
            }
        }

        //CLIENT_SERVER_REQ 0x80 玩家报名请求当前游戏服务器IP
        //[StructLayout(LayoutKind.Sequential)]
        public class ClentGateSeverReqDef : Protocol
        {
            public MsgHeadDef MsgHeadInfo = new MsgHeadDef();
            public int LockServerID; //大于0的话，标识被卡在这个服务器里了，要求强行进入

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                MsgHeadInfo.toBytes(stream);
                stream.writeInt(LockServerID);

                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }


        //CLIENT_SERVER_RES 0x81 回应服务器IP和端口信息，玩家连上游戏服务器后，如果登录正常，则就可以断开和报名服务器的TCP链接了，不正常，重新请求
        //[StructLayout(LayoutKind.Sequential)]
        public class ClientServerResDef : Protocol
        {
            public MsgHeadDef MsgHeadInfo = new MsgHeadDef();
            public int iError; //错误编号，0是没有错误
            public int iServerId; // 可连接的服务器编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szIP;// 可连接的服务器的IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string domain;// 可连接的服务器的域名
            public ushort usPort; // 可连接的服务器的端口
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;

                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iServerId = stream.readInt();
                szIP = stream.readFixedLenString(40);
                domain = stream.readFixedLenString(40);
                usPort = (ushort)stream.readShort();

                return stream.currentPos();
            }
            public void NormalizeMarshaledString()
            {
                //szIP = StringUtil.NormalizeMarshaledString(szIP);
                //domain = StringUtil.NormalizeMarshaledString(domain);
            }
        }


        //BULLETIN_REQ		0x0013[厅服/游服]->[告服]公告请求消息
        public class BulletinReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iBulletinRange; // 公告范围，0大厅和所有游戏，1仅大厅，2大厅和指定游戏，3指定游戏，4所有游戏
            public int iCityId; // 城市编号，0全部城市可见，>0指定城市可见
            public int iCountyId; // 县编号，0全部县可见，>0指定县可见
            public int iGameId; // 游戏编号，当公告范围为2和3时有效
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 320)]
            public string szBulletinContent; // 公告内容，最多100个汉字
            public int iBeginTime; // 开始时间
            public int iEndTime; // 结束时间
            public int iSendInterval; // 发送时间间隔，0代表不需要重复发送
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iBulletinRange);
                stream.writeInt(iCityId);
                stream.writeInt(iCountyId);
                stream.writeInt(iGameId);
                stream.writeFixedLenString(szBulletinContent, 320);
                stream.writeInt(iBeginTime);
                stream.writeInt(iEndTime);
                stream.writeInt(iSendInterval);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //BULLETIN_NOTICE	0x0014[厅服/游服]->[厅客/游客]或[厅服/游服]<->[告服]公告通知消息
        public class BulletinNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iBulletinRange; // 公告范围，0大厅和所有游戏，1仅大厅，2大厅和指定游戏，3指定游戏，4所有游戏
            public int iCityId; // 城市编号，0全部城市可见，>0指定城市可见
            public int iCountyId; // 县编号，0全部县可见，>0指定县可见
            public int iGameId; // 游戏编号，当公告范围为2和3时有效
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 304)]
            public string szBulletinContent; // 公告内容，最多100个汉字
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iBulletinRange = stream.readInt();
                iCityId = stream.readInt();
                iCountyId = stream.readInt();
                iGameId = stream.readInt();
                szBulletinContent = stream.readFixedLenString(304);
                return stream.currentPos();
            }
        }


        //CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public class ClientAuthenReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public short wVer; //版本号
            public byte iAuthenType; // 认证类型：
                                     // 1.微信access_token登录模式，填写szToken
                                     // 2.微信refresh_token登录模式，填写szToken
                                     // 3.用户编号登录，填写iUserId，szToken
                                     // 4.游客登录
            public int iUserId; // 用户编号
                                // iAuthenType=1：不用填写
                                // iAuthenType=2：不用填写
                                // iAuthenType=3：用户编号
                                // iAuthenType=4：不用填写
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szToken; // 根据认证类型不同：
                                   // iAuthenType=1：微信code
                                   // iAuthenType=2：微信refresh_token
                                   // iAuthenType=3：微信access_token
                                   // iAuthenType=4：不用填写
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szDui; // 设备唯一标识符(deviceUniqueIdentifier)【所有认证类型都填写】
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szIp; // IP【所有认证类型都填写】
            public float fLongitude; // 经度【所有认证类型都填写】
            public float fLatitude; // 纬度【所有认证类型都填写】
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szAddress; // 经纬度对应的地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string szRegistMac;  //注册Mac地址【所有认证类型填写】
            public int iRegistSource; // 用户注册市场来源【所有认证类型都填写】1000ios,1001官方 1002应用宝 1003今日头条,1004
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szRegistImei; // 用户注册市场IMEI(IOS系统存储idfa)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string REGISTRATION_ID;
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeShort(wVer);
                stream.writeByte(iAuthenType);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szToken, 128);
                stream.writeFixedLenString(szDui, 48);
                stream.writeFixedLenString(szIp, 40);
                stream.writeFloat(fLongitude);
                stream.writeFloat(fLatitude);
                stream.writeFixedLenString(szAddress, 48);
                stream.writeFixedLenString(szRegistMac, 20);
                stream.writeInt(iRegistSource);
                stream.writeFixedLenString(szRegistImei, 40);
                stream.writeFixedLenString(REGISTRATION_ID, 20);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //CLIENT_AUTHEN_RES = 0x0211;// [厅服/游服]->[厅客/游客]认证回应消息
        public class ClientAuthenRes : Protocol
        {
            public MsgHeadDef MsgHeadInfo = new MsgHeadDef();
            public int iError;
            public sbyte iAuthenType; // 认证类型：
                                      // 1.微信access_token登录模式
                                      // 2.微信refresh_token登录模式
                                      // 3.用户编号登录，填写iUserId
                                      // 4.游客登录
            public byte byNewUser; // 新用户标志
            public ClientUserDef clientUser = new ClientUserDef(); // 客户端用户信息节点

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iAuthenType = (sbyte)stream.readByte();
                byNewUser = stream.readByte();
                clientUser.parseBytes(stream);
                return stream.currentPos();
            }
        }
        public class ClientUserDef : Protocol
        {
            public int iUserId; // 用户编号
            public int iCityId; // 选择的市编号
            public int iCountyId; // 选择的县编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szOpenid; // 微信openid
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUnionid; // 微信unionid
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname; // 微信昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl; // 微信头像网址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szAccessToken; // 微信access_token
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szRefreshToken; // 微信refresh_token
            public int[] iCoin = new int[3];  // 币数量：1安卓币、2苹果币、3赠币
                                              //public int iBindCoin; // 绑定金币
                                              //public int iScore; // 当前积分

            public int iCashBag;//现金红包个数
            public int iCompliment; // 赞的数量
            public int iSpreaderId; // 上级推广员的用户编号
            public int iSpreadGiftTime; // 获取推广礼包时间
            public int iMyParlorId; // 自己开的麻将馆编号
            public int[] iaJoinParlorId = new int[4]; // 加入的麻将馆编号
            public int iJoinParlorTime; // 最后加入麻将馆时间
            public int iLeaveParlorTime; // 最后退出或被踢出麻将馆时间
            public int iKickParlorTime; // 最后被踢出麻将馆时间
            public int iGameNumAcc; // 游戏局数累计
            public int iDisconnectAcc; // 掉线次数累计
            public int iPlayCardAcc; // 出牌次数累计
            public int iPlayCardTimeAcc; // 出牌时间累计
            public int iLastCoin3Tim; // 最后一次领取绑定金币时间
            public int iLastHolidayId; // 最后一次节日编号
            public int iLastHolidayGiftTim; // 最后一次领取节日礼包时间 
            public int iLeaveParlorAcc; // 退出麻将馆次数累计
            public int iKickParlorAcc; // 被踢出麻将馆次数累计
            public int iDismissParlorAcc; // 解散麻将管次数累计     
            public int[,] da2Score = new int[3, 2]; //// 当前积分积分：1安卓币积分、2苹果币积分、3赠币积分
            public int fParlorScore;//麻将馆业绩积分
            /// <summary>
            /// 资源:
            /// 第1个数字：0现金，1话费，2流量，3储值卡，（5代金券，6赠币没有对应字段）
            /// </summary>
            public int[] da2Asset = new int[4];
            public int iSpreadAcc; // 自己推广人数累计
            public int AllRpNum;//所有红包数量
            public byte bySex; // 性别
            public byte byCreateParlorCert; // 是否有开麻将馆资格
            public byte byNameAuthen; // 实名认证标志
            public byte byUserSource; // 用户来源：1游客，2微信
            public byte byMemberFirstChargeAward; // 0未冲过
            public byte byBossFirstChargeAward; //1冲过
            public byte byFirstInParlor; //用户是否进入过麻将馆
            public byte pos_0;
            //public byte pos_1;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iUserId = stream.readInt(); // 用户编号
                iCityId = stream.readInt(); // 选择的市编号
                iCountyId = stream.readInt(); // 选择的县编号
                szOpenid = stream.readFixedLenString(32); // 微信openid
                szUnionid = stream.readFixedLenString(32); // 微信unionid
                szNickname = stream.readFixedLenString(52); // 微信昵称
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址

                szAccessToken = stream.readFixedLenString(128); // 微信access_token
                szRefreshToken = stream.readFixedLenString(128); // 微信refresh_token
                for (int i = 0; i < iCoin.Length; i++)
                {
                    iCoin[i] = stream.readInt(); // 金币
                }
                //iBindCoin = stream.readInt(); // 绑定金币              

                iCashBag = stream.readInt();
                iCompliment = stream.readInt(); // 赞的数量
                iSpreaderId = stream.readInt(); // 上级推广员的用户编号
                iSpreadGiftTime = stream.readInt(); // 获取推广礼包时间
                iMyParlorId = stream.readInt(); // 自己开的麻将馆编号
                for (int i = 0; i < 4; i++)
                {
                    iaJoinParlorId[i] = stream.readInt(); // 加入的麻将馆编号
                }

                iJoinParlorTime = stream.readInt(); // 最后加入麻将馆时间
                iLeaveParlorTime = stream.readInt(); // 最后退出或被踢出麻将馆时间
                iKickParlorTime = stream.readInt(); // 最后被踢出麻将馆时间
                iGameNumAcc = stream.readInt(); // 游戏局数累计
                iDisconnectAcc = stream.readInt(); // 掉线次数累计
                iPlayCardAcc = stream.readInt(); // 出牌次数累计
                iPlayCardTimeAcc = stream.readInt(); // 出牌时间累计
                iLastCoin3Tim = stream.readInt(); // 最后一次领取绑定金币时间
                iLastHolidayId = stream.readInt(); // 最后一次节日编号
                iLastHolidayGiftTim = stream.readInt(); // 最后一次领取节日礼包时间
                iLeaveParlorAcc = stream.readInt();
                iKickParlorAcc = stream.readInt();
                iDismissParlorAcc = stream.readInt();

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        da2Score[i, j] = stream.readInt(); // 当前积分
                    }
                }
                fParlorScore = stream.readInt();   //麻将馆业绩积分
                for (int i = 0; i < 4; i++)
                {
                    da2Asset[i] = stream.readInt();
                }
                iSpreadAcc = stream.readInt();
                AllRpNum = stream.readInt();
                bySex = stream.readByte(); // 性别
                byCreateParlorCert = stream.readByte();  // 是否有开麻将馆资格
                byNameAuthen = stream.readByte();  // 实名认证标志
                byUserSource = stream.readByte(); // 用户来源：1游客，2微信
                byMemberFirstChargeAward = stream.readByte();
                byBossFirstChargeAward = stream.readByte();
                byFirstInParlor = stream.readByte();
                pos_0 = stream.readByte();
                //pos_1 = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iUserId = stream.readInt(); // 用户编号
                iCityId = stream.readInt(); // 选择的市编号
                iCountyId = stream.readInt(); // 选择的县编号
                szOpenid = stream.readFixedLenString(32); // 微信openid
                szUnionid = stream.readFixedLenString(32); // 微信unionid
                szNickname = stream.readFixedLenString(52); // 微信昵称
                                                            // szNickname = StringUtil.NormalizeMarshaledString(szNickname.Trim());
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址
                                                               //  szHeadimgurl = StringUtil.NormalizeMarshaledString(szHeadimgurl.Trim());
                szAccessToken = stream.readFixedLenString(128); // 微信access_token
                szRefreshToken = stream.readFixedLenString(128); // 微信refresh_token
                for (int i = 0; i < iCoin.Length; i++)
                {
                    iCoin[i] = stream.readInt(); // 金币
                }
                //iBindCoin = stream.readInt(); // 绑定金币              

                iCashBag = stream.readInt();
                iCompliment = stream.readInt(); // 赞的数量
                iSpreaderId = stream.readInt(); // 上级推广员的用户编号
                iSpreadGiftTime = stream.readInt(); // 获取推广礼包时间
                iMyParlorId = stream.readInt(); // 自己开的麻将馆编号
                for (int i = 0; i < 4; i++)
                {
                    iaJoinParlorId[i] = stream.readInt(); // 加入的麻将馆编号
                }

                iJoinParlorTime = stream.readInt(); // 最后加入麻将馆时间
                iLeaveParlorTime = stream.readInt(); // 最后退出或被踢出麻将馆时间
                iKickParlorTime = stream.readInt(); // 最后被踢出麻将馆时间
                iGameNumAcc = stream.readInt(); // 游戏局数累计
                iDisconnectAcc = stream.readInt(); // 掉线次数累计
                iPlayCardAcc = stream.readInt(); // 出牌次数累计
                iPlayCardTimeAcc = stream.readInt(); // 出牌时间累计
                iLastCoin3Tim = stream.readInt(); // 最后一次领取绑定金币时间
                iLastHolidayId = stream.readInt(); // 最后一次节日编号
                iLastHolidayGiftTim = stream.readInt(); // 最后一次领取节日礼包时间
                iLeaveParlorAcc = stream.readInt();
                iKickParlorAcc = stream.readInt();
                iDismissParlorAcc = stream.readInt();

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        da2Score[i, j] = stream.readInt(); // 当前积分
                    }
                }
                fParlorScore = stream.readInt();   //麻将馆业绩积分
                for (int i = 0; i < 4; i++)
                {
                    da2Asset[i] = stream.readInt();
                }
                iSpreadAcc = stream.readInt();
                AllRpNum = stream.readInt();
                bySex = stream.readByte(); // 性别
                byCreateParlorCert = stream.readByte();  // 是否有开麻将馆资格
                byNameAuthen = stream.readByte();  // 实名认证标志
                byUserSource = stream.readByte(); // 用户来源：1游客，2微信
                byMemberFirstChargeAward = stream.readByte();
                byBossFirstChargeAward = stream.readByte();
                byFirstInParlor = stream.readByte();
                pos_0 = stream.readByte();
                //pos_1 = stream.readByte();
            }
        }
        //CLIENT_CHANGE_USER_INFO_REQ				0x0220	// [厅客]->[厅服]或[厅服]->[计服]修改用户信息请求消息
        public class ClientChangeUserInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname; // 微信昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl; // 微信头像网址            
            public sbyte cSex; // 性别
            public short sCoverPosition_0;  //补位
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szNickname, 52);
                stream.writeFixedLenString(szHeadimgurl, 160);
                stream.writeByte((byte)cSex); // 性别
                stream.writeShort(sCoverPosition_0);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //CLIENT_CHANGE_USER_INFO_RES				0x0221	// [厅服]->[厅客]或[计服]->[厅服]修改用户信息回应消息
        public class ClientChangeUserInfoRes : Protocol
        {
            public MsgHeadDef MsgHeadInfo = new MsgHeadDef();
            public int iError;         // 错误编号
            public int iUserID;        // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname;  //微信昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl;//微信头像网址
            public sbyte iSex; // 性别
            public short sCoverposition_0;//补位 
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) { return 0; }
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserID = stream.readInt();
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160);
                iSex = (sbyte)stream.readByte();
                sCoverposition_0 = stream.readShort();
                return stream.currentPos();
            }
        }


        //CLIENT_FULL_NAME_REQ					0x0222	// [厅客]->[厅服]实名认证请求消息
        public class ClientFullNameReq : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFullName; // 真实姓名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string szIdentityCard; // 身份证号码

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                MsgHeadDef.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szFullName, 32);
                stream.writeFixedLenString(szIdentityCard, 20);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }


        //CLIENT_FULL_NAME_RES		0x0223	// [厅客]->[厅服]实名认证回应消息
        public class ClientFullNameRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iFreeCard; // 获得赠卡数量 
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_CITY_COUNTY_REQ 0x0224;	// [厅客]->[厅服]选择市县请求消息
        public class ClientCityCountyReq : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iCityId; // 市
            public int iCountyId; // 县
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                MsgHeadDef.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iCityId);
                stream.writeInt(iCountyId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_CITY_COUNTY_RES					0x0225	// [厅客]->[厅服]选择市县回应消息
        public class ClientCityCountyRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iCityId; // 市
            public int iCountyId; // 县
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iCityId = stream.readInt();
                iCountyId = stream.readInt();
                return stream.currentPos();
            }
        }


        //CLIENT_SPREAD_CODE_REQ					0x0234	// [厅客]->[厅服]使用推广码请求消息
        public class ClientSpreadCodeReq : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string szSpreadCode; // 推广码
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                MsgHeadDef.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szSpreadCode, 12);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //CLIENT_GETEXCHANGE_COIN_REQ				0x0228	// [厅客]->[厅服]读取兑换金币配置请求消息
        public class ClientGetExchangeCoinReq : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                MsgHeadDef.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //CLIENT_GETEXCHANGE_COIN_RES				0x0229	//	[厅服]->[厅客]读取充值金币配置回应消息
        public class ClientGetExchangeCoinRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号、
            public int iChargeNum;// 充值数据数量
            public int iExchangeNum;// 兑换数据数量:1001:业绩,2001:现金,3001:储值卡
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iChargeNum = stream.readInt();
                iExchangeNum = stream.readInt();
                return stream.currentPos();
            }
        }
        /// <summary>
        /// 充值配置结构体
        /// </summary>
        public class Charge : Protocol
        {
            /// <summary>
            /// 1001:用户充值,2001:老板充值,3001:业绩兑换,4001:现金兑换,5001:储值卡兑换
            /// </summary>
            public int ChargeId;
            public int Coin;//金币数量
            public int Price;//价格
            public int Present;// 老板赠币数量
            public int Boss; //是否是给老板充值的

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                {
                    return 0;
                };
                FieldStream stream = new FieldStream(bytes, begin);
                ChargeId = stream.readInt();
                Coin = stream.readInt();
                Price = stream.readInt();
                Present = stream.readInt();
                Boss = stream.readInt();
                return stream.currentPos();
            }

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_SPREAD_CODE_RES					0x0235	// [厅客]->[厅服]使用推广码回应消息
        public class ClientSpreadCodeRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string szSpreadCode; // 推广码
            public int iSpreaderId; // 推广员编号
            public int iSpreadTime; // 推广绑定时间，获取推广礼包时间
            public int iFreeCard; // 获得的赠卡
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                szSpreadCode = stream.readFixedLenString(12);
                iSpreaderId = stream.readInt();
                iSpreadTime = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
        }


        //CLIENT_SPREAD_GIFT_REQ	0x0232	// [厅客]->[厅服]推广礼包请求消息
        public class ClientSpreadGiftReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();

            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_SPREAD_GIFT_RES		0x0233	// [厅服]->[厅客]推广礼包回应消息
        public class ClientSpreadGiftRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iSpreadGiftTime; // 获取推广礼包时间
            public int iFreeCard; // 获得的赠卡
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iSpreadGiftTime = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_SPREADER_INFO_REQ	0x0234	// [厅客]->[厅服]推广员信息请求消息
        public class ClientSpreaderInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iSpreaderId;//推广员用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iSpreaderId);
                return stream.toBytes();

            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //CLIENT_SPREADER_INFO_RES				0x0235	// [厅服]->[厅客]推广员信息回应消息
        public class ClientSpreaderInfoRes : Protocol
        {
            public MsgHeadDef MsgHeadDef = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iSpreaderId; // 推广员用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szSpreaderNickname;//  推广员微信昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szSpreaderHeadimgurl;//  推广员微信昵称
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                MsgHeadDef.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iSpreaderId = stream.readInt();
                szSpreaderNickname = stream.readFixedLenString(52);
                szSpreaderHeadimgurl = stream.readFixedLenString(160);
                return stream.currentPos();
            }
        }

        //CLIENT_OPEN_ROOM_REQ			0x0260	// [厅客]->[厅服]开房请求消息
        public class ClientOpenRoomReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 创建房间的用户编号
            public byte cGameId; // 游戏编号                        
            public byte cOpenRoomMode; // 开房模式：1普通开房，2馆内馆主开房 2馆内成员开房
            public byte byColorFlag; // 颜色标志       
            public int iPlayingMethod; // 玩法：1长治（缺门待炮），2长治（花三门），3长治打锅（花三门）  
            public int iCountyId; // 开房所属县id
            /// <summary>
            /// 要求攒的数量
            /// </summary>
            public int iCompliment;
            public int iDisconnectRate;//掉线率要求
            public int iDiscardTime;//出牌速度要求              
            public int iRoomCard; // 消耗房卡数量                    
            public uint [] caParam = new uint [OpenRoomParamRow ]; // 创建房间的参数。每个游戏自己解释
            public int iParlorId;  //创建的房间所属的麻将馆的编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte((byte)cGameId);
                stream.writeByte((byte)cOpenRoomMode);
                stream.writeByte((byte)byColorFlag);
                stream.writeInt(iPlayingMethod);
                stream.writeInt(iCountyId);
                stream.writeInt(iCompliment);
                stream.writeInt(iDisconnectRate);
                stream.writeInt(iDiscardTime);
                stream.writeInt(iRoomCard);
                for (int i = 0; i < caParam.Length ; i++)
                {
                    stream.writeUint (caParam[i]);
                }
                stream.writeInt(iParlorId);
                return stream.toBytes();

            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_OPEN_ROOM_RES	  0x0261	// [厅服]->[厅客]开房回应消息
        public class ClientOpenRoomRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 创建房间的用户编号            
            public byte cGameId; // 游戏编号            
            public byte cOpenRoomMode; // 开房模式：1普通开房，2表示馆内馆主开房 3表示馆内成员开房
            public byte byColorFlag; // 颜色标志
            public int iPlayingMethod; // 玩法：1长治（缺门待炮），2长治（花三门），3长治打锅（花三门）
            public int iCountyId; // 开房所属县id

            public int iCompliment;//要求攒的数量
            public int iDisconnectRate;//掉线率要求
            public int iDiscardTime;//出牌速度要求   
            public int iRoomCard; // 消耗房卡数量
            public uint [] caParam = new uint [OpenRoomParamRow ]; // 创建房间的参数。每个游戏自己解释
            public int iRoomNum; // 房间号
            public int iServerId; // 服务器编号            
            public short iTableNum; // 桌编号
            public short scoverPosition;  //补位
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                cGameId = stream.readByte();
                cOpenRoomMode = stream.readByte();
                byColorFlag = stream.readByte();
                iPlayingMethod = stream.readInt();
                iCountyId = stream.readInt();
                iCompliment = stream.readInt();
                iDisconnectRate = stream.readInt();
                iDiscardTime = stream.readInt();
                iRoomCard = stream.readInt();
                for (int i = 0; i < caParam.Length ; i++)
                {
                    caParam[i] = stream.readUint  ();
                }
                iRoomNum = stream.readInt();
                iServerId = stream.readInt();
                iTableNum = stream.readShort();
                scoverPosition = stream.readShort();
                return stream.currentPos();
            }
        }

        //#define Err_ClientOpenRoomRes_NoServer							1	// 没可用的服务器
        //#define Err_ClientOpenRoomRes_TableNumMax						2	// 支持的最大人数错误
        //#define Err_ClientOpenRoomRes_NoTable							3	// 没可用的桌
        //#define Err_ClientOpenRoomRes_GameServerInfo					4	// 获取游戏服务器信息失败
        //#define Err_ClientOpenRoomRes_GameId							5	// 错误的游戏编号
        //#define Err_ClientOpenRoomRes_GameServerId					6	// 错误的游戏服务器编号
        //#define Err_ClientOpenRoomRes_TableNum							7	// 错误的桌号
        //#define Err_ClientOpenRoomRes_TableNode						8	// 空的桌节点
        //#define Err_ClientOpenRoomRes_OpenRoomStatus					9	// 开房状态不为0
        //#define Err_ClientOpenRoomRes_OpenRoomMode					10	// 开房模式错误
        //#define Err_ClientOpenRoomRes_OpenRoomMax						11	// 开房达到上限
        //#define Err_ClientOpenRoomRes_SetOpenRoomParam				12	// 设置开房参数错误
        //#define Err_ClientOpenRoomRes_Card								13	// 卡不够
        //#define Err_ClientOpenRoomRes_Compliment						14	// 开房人不满足开房要求的赞数量
        //#define Err_ClientOpenRoomRes_CountMax							15	// 今日开房次数达到上限
        //#define Err_ClientOpenRoomRes_iDisconnectRate				16	// 开房人不满足开房要求的掉线率
        //#define Err_ClientOpenRoomRes_DiscardTime						17	// 开房人不满足开房要求的出牌时间
        //#define Err_ClientOpenRoomRes_ServerInvalid					18	// 游戏服务器不可用
        //#define Err_ClientOpenRoomRes_BossParlor						19 // 馆主编号与开房所属麻将馆id不一致
        //#define Err_ClientOpenRoomRes_UserParlor						20 // 馆内成员所属麻将馆与开房所属麻将馆id不一致
        //#define Err_ClientOpenRoomRes_Database							21 // 数据库数据查询失败
        //#define Err_ClientOpenRoomRes_ParlorStatus					22	// 麻将馆状态错误

        //CLIENT_OPEN_ROOM_INFO_REQ			0x0262	// [厅客]->[厅服]开房信息请求消息
        public class ClientOpenRoomInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 创建房间的用户编号                 
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 开房参数二维2行  0-3位表示玩法 4预约 5 托管
        /// </summary>
        public const byte OpenRoomParamRow = 6;
        //开房信息的结构体
        public class OpenRoomInfo : Protocol
        {
            public int iPlayingMethod; // 游戏玩法
            public int PavilionID;//是不是馆主创建的房间  如果不是这个值是0 
            public int iServerId; // 游戏服务器编号
            public short iTableNum; // 桌编号            
            public byte cOpenRoomStatus; // 桌的开房状态，0没使用，1已经预订，2等待开始游戏，3已开始游戏
            public byte byColorFlag; // 颜色标志
            public int iOpenRoomTime; // 开房时间
            public int iCompliment; // 要求赞的数量
            public int iDisconnectRate; // 掉线率要求
            public int iDiscardTime; // 出牌速度要求            
            public uint[] caOpenRoomParam = new uint[OpenRoomParamRow]; // 开房的参数 64位长度
            public int iRoomNum; // 房间号
            public int[] iaUserId = new int[4]; // 桌上用户
            public int[] iBespeakUserId = new int[4];//桌上预约用户ID
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iPlayingMethod = stream.readInt();
                iServerId = stream.readInt();
                iTableNum = stream.readShort();
                cOpenRoomStatus = stream.readByte();
                byColorFlag = stream.readByte();
                iOpenRoomTime = stream.readInt();
                iCompliment = stream.readInt();
                iDisconnectRate = stream.readInt();
                iDiscardTime = stream.readInt();
                for (int i = 0; i < caOpenRoomParam.Length; i++)
                {
                    caOpenRoomParam[i] = stream.readUint ();
                }
                iRoomNum = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    iBespeakUserId[i] = stream.readInt();
                }
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iPlayingMethod = stream.readInt();
                iServerId = stream.readInt();
                iTableNum = stream.readShort();
                cOpenRoomStatus = stream.readByte();
                byColorFlag = stream.readByte();
                iOpenRoomTime = stream.readInt();
                iCompliment = stream.readInt();
                iDisconnectRate = stream.readInt();
                iDiscardTime = stream.readInt();
                for (int i = 0; i < caOpenRoomParam.Length; i++)
                {
                    caOpenRoomParam[i] = stream.readUint();
                }
                iRoomNum = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    iBespeakUserId[i] = stream.readInt();
                }
            }
        }


        //CLIENT_OPEN_ROOM_INFO_RES				0x0263	// [厅服]->[厅客]开房信息回应消息
        public class ClientOpenRoomInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 创建房间的用户编号            
            public int iNum; // 数量，后面跟随iNum个OpenRoomInfoDef的数据
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iNum = stream.readInt();
                return stream.currentPos();
            }
        }

        // 当代开房的开房状态变为0或3时，通知代理
        //CLIENT_OPEN_ROOM_INFO_NOTICE			0x0264	// [厅客]->[厅服]开房信息通知消息
        public class ClientOpenRoomInfoNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 创建房间的用户编号            
            public int iServerId; // 游戏服务器编号
            public short sTableNum; // 桌编号            
            public byte cOpenRoomStatus; // 桌的开房状态，0没使用，1已经预订，2等待开始游戏，3已开始游戏
            public int[] iaUserId = new int[4]; // 桌上用户
            public int[] iaBespeakUserid = new int[4]; // 桌上预约用户
            public int iRoomNum;//房间编号
            public int iParlorId; // 房间所属麻将馆编号
            public int iDissolveType; //解散类型
                                      // 1未开始游戏房主主动解散
                                      // 2未开始游戏时间到了自动解散
                                      // 3一局游戏内解散，没计费
                                      // 4一局游戏后解散，有计费
                                      // 5全部游戏结束解散
                                      // 6未到预约时间房主解散
                                      // 7后台解散

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                iServerId = stream.readInt();
                sTableNum = stream.readShort();
                cOpenRoomStatus = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    iaBespeakUserid[i] = stream.readInt();
                }
                iRoomNum = stream.readInt();
                iParlorId = stream.readInt();
                iDissolveType = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_BIND_PROXY_REQ					0x0240	// [厅客]->[厅服]绑定代理请求消息
        public class ClientBindProxyReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iProxyId; // 代理的用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iProxyId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_BIND_PROXY_RES					0x0241	// [厅服]->[厅客]绑定代理回应消息
        public class ClientBindProxyRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iProxyId; // 代理的用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szProxyNickname; // 代理的昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string szProxyWx; // 代理微信
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 92)]
            public string szProxyComment; // 代理留言
            public int iProxyTime; // 绑定代理的时间
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iProxyId = stream.readInt();
                szProxyNickname = stream.readFixedLenString(52);
                // szProxyNickname = StringUtil.NormalizeMarshaledString(szProxyNickname);
                szProxyWx = stream.readFixedLenString(24);
                // szProxyWx = StringUtil.NormalizeMarshaledString(szProxyWx);
                szProxyComment = stream.readFixedLenString(92);
                //szProxyComment = StringUtil.NormalizeMarshaledString(szProxyComment);
                iProxyTime = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_ASK_UNBIND_PROXY_REQ				0x0242	// [厅客]->[厅服]申请解绑代理请求消息
        public class ClientAskUnbindProxyReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_ASK_UNBIND_PROXY_RES				0x0243	// [厅服]->[厅客]申请解绑代理回应消息
        public class ClientAskUnbindProxyRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iUnbindProxyTime; // 申请解绑时间
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iUnbindProxyTime = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_MESSAGE_NOTICE					0x0250	// [厅服]->[厅客]信息通知
        public class ClientMessageNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iOperatedUserId;//被操作的用户编号
            public byte byType; // 消息类型：1 馆主邀请用户成为馆内成员同意通知 2 用户申请加入麻将馆同意通知
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                iOperatedUserId = stream.readInt();
                byType = stream.readByte();
                return stream.currentPos();
            }

        }

        //CLIENT_MESSAGE_OPERATE_REQ 0x0251	// [厅服]->[厅客]信息操作请求消息
        public class ClientMessageOperateReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iMessageId; // 信息编号            
            public sbyte cMessageType; //消息类型：1普通信息，2馆主邀请成员，3成员申请加入馆
            public sbyte cOperate; // 操作：1同意，2拒绝
            public byte pos0;  //补位
            public byte pos1;  //补位
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iMessageId);
                stream.writeByte((byte)cMessageType);
                stream.writeByte((byte)cOperate);
                stream.writeByte(pos0);
                stream.writeByte(pos1);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_MESSAGE_OPERATE_RES					0x0252	// [厅服]->[厅客]信息操作回应消息
        public class ClientMessageOperateRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号，0正确
            public int iUserId; // 用户编号
            public int iMessageId; // 信息编号            
            public sbyte cMessageType; // 消息类型：1普通信息，2馆主邀请成员，3成员申请加入馆 
            public sbyte cOperate; // 操作
            public byte pos0;  //补位
            public byte pos1;  //补位
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iMessageId = stream.readInt();
                cMessageType = (sbyte)stream.readByte();
                cOperate = (sbyte)stream.readByte();
                pos0 = stream.readByte();
                pos1 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_PROXY_INFO_REQ					0x0244	// [厅客]->[厅服]或[厅服]->[计服]代理信息请求消息a
        public class ClientProxyInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iProxyId; // 代理用户编号         
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iProxyId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_PROXY_INFO_RES					0x0245	// [厅服]->[厅客]或[计服]->[厅服]代理信息回应消息
        public class ClientProxyInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号，0正确
            public int iUserId; // 用户编号
            public int iProxyId; // 代理用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname; //代理昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl; // 微信头像网址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string szProxyWx; // 代理微信
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 92)]
            public string szProxyComment; // 代理留言
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iProxyId = stream.readInt();
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160);
                szProxyWx = stream.readFixedLenString(24);
                szProxyComment = stream.readFixedLenString(92);
                return stream.currentPos();
            }
        }


        // 游戏服务器信息结构体
        // 与TBL_GAME_SERVER相比，少了VALID字段（这里获取的都是VALID为1的数据）、
        // ONLINE_NUMBER字段（由游戏服务器发给游戏网关）和CUR_GAME_NUM
        // sizeof(GameServerDef)=140，8096/140=57.8，一次取超过57个服务器信息就会有问题
        public class GameServerDef : Protocol
        {
            public int iGameServerId; // 游戏服务器编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szGameServerName; // 游戏服务器名称，最多10个汉字
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szIp; // IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szDomain; // 域名
            public ushort usPort;  //端口
            public ushort wVer;  //版本号
            public short iOnlineMax; // 支持的最大在线人数            
            public sbyte iGameId; // 游戏编号            
            public sbyte iBeginHour; // 开始小时数，0~23小时            
            public sbyte iContinueHour; // 持续小时数，1~24小时            
            public sbyte iUserRange; // 开放的用户范围，1内部用户，2全部用户  
            public ushort pos;

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iGameServerId = stream.readInt();
                szGameServerName = stream.readFixedLenString(32);
                szIp = stream.readFixedLenString(40);
                szDomain = stream.readFixedLenString(40);
                usPort = (ushort)stream.readShort();
                wVer = (ushort)stream.readShort();
                iOnlineMax = stream.readShort();
                iGameId = (sbyte)stream.readByte();
                iBeginHour = (sbyte)stream.readByte();
                iContinueHour = (sbyte)stream.readByte();
                iUserRange = (sbyte)stream.readByte();
                pos = (ushort)stream.readShort();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iGameServerId = stream.readInt();
                szGameServerName = stream.readFixedLenString(32);
                szIp = stream.readFixedLenString(40);
                szDomain = stream.readFixedLenString(40);
                usPort = (ushort)stream.readShort();
                wVer = (ushort)stream.readShort();
                iOnlineMax = stream.readShort();
                iGameId = (sbyte)stream.readByte();
                iBeginHour = (sbyte)stream.readByte();
                iContinueHour = (sbyte)stream.readByte();
                iUserRange = (sbyte)stream.readByte();
                pos = (ushort)stream.readShort();
            }

            //public void NormalizeMarshaledString()
            //{
            //    szGameServerName = StringUtil.NormalizeMarshaledString(szGameServerName);
            //    szIp = StringUtil.NormalizeMarshaledString(szIp);
            //    szDomain = StringUtil.NormalizeMarshaledString(szDomain);
            //}
        }

        //CLIENT_MY_ROOM_INFO_REQ	0x0265	// [厅客]->[厅服]我的开房信息请求消息
        public class ClientMyRoomInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_MY_ROOM_INFO_RES	0x0266	// [厅服]->[厅客]我的开房信息回应消息
        public class ClientMyRoomInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iRoomNum; // 房间号
            public int iServerId; // 游戏服务器编号
            public int iParlorId; // 房间所属麻将馆编号
            public short sTableNum; // 桌号
            public short iTimeLeft; //还有多长时间开始
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iRoomNum = stream.readInt();
                iServerId = stream.readInt();
                iParlorId = stream.readInt();
                sTableNum = stream.readShort();
                iTimeLeft = stream.readShort();
                return stream.currentPos();
            }
        }

        //CLIENT_GAME_SERVER_INFO_REQ	 0x0270	// [厅客]->[厅服]游戏服务器信息请求消息
        public class ClientGameServerInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iServerId; // 游戏服务器编号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iServerId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }


        //CLIENT_GAME_SERVER_INFO_RES	0x0271	// [厅服]->[厅客]游戏服务器信息回应消息
        public class ClientGameServerInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public GameServerDef gameServer = new GameServerDef(); // 游戏服务器信息

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                gameServer.parseBytes(stream);
                return stream.currentPos();

            }
        }

        //CLIENT_COMPLIMENT_REQ		0x0280	// [厅客]->[厅服]点赞请求消息
        public class ClientComplimentReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szOpenRoomId; // 开房序号，格式：服务器编号-序号
            public byte byComplimentSeat; // 点赞的座位号
            public byte pos_0;  //补位
            public byte pos_1;  //补位
            public byte pos_2;  //补位           

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szOpenRoomId, 16);
                stream.writeByte(byComplimentSeat);
                stream.writeByte(pos_0);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }


        //CLIENT_COMPLIMENT_RES		0x0281	// [厅服]->[厅客]点赞回应消息
        public class ClientComplimentRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szOpenRoomId; // 开房序号，格式：服务器编号-序号
            public byte byComplimentSeat; // 点赞的座位号
            public byte pos_0;  //补位
            public byte pos_1;  //补位
            public byte pos_2;  //补位     

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                szOpenRoomId = stream.readFixedLenString(16);
                byComplimentSeat = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();

            }
        }
        public class HolidayDef : Protocol
        {
            public int iHolidayId; // 节日编号  
            public int iBeginTim; // 开始时间
            public int iEndTim; // 结束时间
            public byte byValid; // 是否有效
            public byte byMemberFreeCard; //  会员奖励赠卡数量
            public byte byProxyFreeCard; // 代理奖励赠卡数量
            public byte pos;

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iHolidayId = stream.readInt();
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                byMemberFreeCard = stream.readByte();
                byProxyFreeCard = stream.readByte();
                pos = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iHolidayId = stream.readInt();
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                byMemberFreeCard = stream.readByte();
                byProxyFreeCard = stream.readByte();
                pos = stream.readByte();
            }
        }

        /// <summary>
        /// CLIENT_HOLIDAY_REQ	0x0290	// [厅客]->[厅服]节日信息请求消息
        /// </summary>
        public class ClientHolidayReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                return stream.toBytes();
            }
        }

        /// <summary>
        ///  CLIENT_HOLIDAY_RES	0x0291	// [厅服]->[厅客]节日信息回应消息
        /// </summary>
        public class ClientHolidayRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public HolidayDef holiday = new HolidayDef();
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                holiday.parseBytes(stream);
                return stream.currentPos();
            }
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// CLIENT_HOLIDAY_GIFT_REQ	0x0292	// [厅客]->[厅服]领取节日活动奖励请求消息
        /// </summary>
        public class ClientHolidayGiftReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte byType; // 类型：1每日赠卡，2节日礼包
            public byte pos1;
            public byte pos2;
            public byte pos3;
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byType);
                stream.writeBytes(new byte[] { pos1, pos2, pos3 });
                return stream.toBytes();
            }
        }

        /// <summary>
        /// CLIENT_HOLIDAY_GIFT_RES					0x0293	// [厅服]->[厅客]领取节日活动奖励回应消息
        /// </summary>
        public class ClientHolidayGiftRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iFreeCard; // 获得赠卡数量
            public int iHolidayId; // 节日编号（类型2才有效）
            public int iTim; // 获取时间
            /// <summary>
            /// 类型：1每日赠卡，2节日礼包
            /// </summary>
            public byte byType;
            public string p3;

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                {
                    return 0;
                }
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iFreeCard = stream.readInt();
                iHolidayId = stream.readInt();
                iTim = stream.readInt();
                byType = stream.readByte();
                p3 = stream.readFixedLenString(3);
                return stream.currentPos();
            }
        }
        /// <summary>
        /// 免费时间结构体
        /// </summary>
        public class FreeTimeDef : Protocol
        {
            public int iBeginTim; // 开始时间
            public int iEndTim; // 结束时间
            public byte byValid; // 是否有效
            public byte pos1;
            public byte pos2;
            public byte pos3;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                pos3 = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                pos3 = stream.readByte();
            }
        }
        /// <summary>
        /// CLIENT_FREE_TIME_REQ	0x0294	// [厅客]->[厅服]免费时间信息请求消息
        /// </summary>
        public class ClientFreeTimeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                return stream.toBytes();
            }
        }
        /// <summary>
        ///  CLIENT_FREE_TIME_RES	0x0295	// [厅服]->[厅客]免费时间信息回应消息
        /// </summary>
        public class ClientFreeTimeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public FreeTimeDef freeTime = new FreeTimeDef();
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                freeTime.parseBytes(stream);
                return stream.currentPos();
            }
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 分享活动结构体
        /// </summary>
        public class Share : Protocol
        {
            public int iShareId; // 分享活动编号
            public int iBeginTim; // 开始时间
            public int iEndTim; // 结束时间
            public byte byValid; // 是否有效
            public int iShareCount; // 分享次数
            public int iFreeCard; // 获取赠卡次数

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iShareId = stream.readInt();
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                iShareCount = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iShareId = stream.readInt();
                iBeginTim = stream.readInt();
                iEndTim = stream.readInt();
                byValid = stream.readByte();
                iShareCount = stream.readInt();
                iFreeCard = stream.readInt();
            }
        }
        /// <summary>
        /// CLIENT_SHARE_REQ = 0x0296	// [厅客]->[厅服]分享活动信息请求消息
        /// </summary>
        public class ClientShareReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                return stream.toBytes();
            }
        }
        /// <summary>
        /// CLIENT_SHARE_USER_RES =	0x0297	// [厅服]->[厅客]分享活动信息回应消息
        /// </summary>
        public class ClientShareRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public Share share = new Share();
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                share.parseBytes(stream);
                return stream.currentPos();
            }
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// CLIENT_SHARE_USER_REQ=0x0298	// [厅客]->[厅服]分享活动用户信息请求消息（大厅客户端无需请求）
        /// </summary>
        public class ClientShareUserReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;//用户编号
            public int iShareId;//分享活动编号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iShareId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// CLIENT_SHARE_USER_RES	0x0299	// [厅服]->[厅客]分享活动用户信息回应消息
        /// </summary>
        public class ClientShareUserRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iShareId; // 分享活动编号
            public int iShareCount; // 用户已经分享次数
            public int iShareTim; // 最后分享时间
            public int iAwardTim; // 领取奖励的时间

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                {
                    return 0;
                }
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iShareId = stream.readInt();
                iShareCount = stream.readInt();
                iShareTim = stream.readInt();
                iAwardTim = stream.readInt();
                return stream.currentPos();
            }
        }

        /// <summary>
        /// CLIENT_SHARE_SUCCESS_REQ=0x029A	// [厅客]->[厅服]分享成功请求消息
        /// </summary>
        public class ClientShareSuccessReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserID;//用户编号
            public int iShareId;//活动编号
            public byte byShareType; ////0大厅 1活动 2邀请 3单局 4总局 5大赢家红包 6最佳炮手红包 7推广红包 8充值红包 9提现红包 10首次提现红包 11大厅分享按钮分享
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserID);
                stream.writeInt(iShareId);
                stream.writeByte(byShareType);
                stream.writeByte(pos_0);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                return stream.toBytes();
            }
        }
        /// <summary>
        /// CLIENT_SHARE_SUCCESS_RES =	0x029B	// [厅服]->[厅客]分享成功回应消息
        /// </summary>
        public class ClientShareSuccessRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iShareId; // 分享活动编号
            public int iShareCount; // 用户已经分享次数
            public int iShareTim; // 最后分享时间
            public int iAwardTim; // 领取奖励的时间
            public int iFreeCard; // 领取赠卡数量 


            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iShareId = stream.readInt();
                iShareCount = stream.readInt();
                iShareTim = stream.readInt();
                iAwardTim = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
        }
        /// <summary>
        /// CLIENT_USE_ACTIVE_CODE_REQ	0x029C	// [厅客]->[厅服]使用激活码请求消息
        /// </summary>
        public class ClientUseActiveCodeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string szBatch; // 批次
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string szCode; // 激活码
            public byte[] pos = new byte[2];

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szBatch, 4);
                stream.writeFixedLenString(szCode, 10);
                stream.writeBytes(pos);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// CLIENT_USE_ACTIVE_CODE_RES	0x029D	// [厅服]->[厅客]使用激活码回应消息
        /// </summary>
        public class ClientUseActiveCodeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public string szBatch; // 批次
            public string szCode; // 激活码
            public int iCardCount; // 获得卡数量
            public int iTim; // 使用时间
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                szBatch = stream.readFixedLenString(4);
                //   szBatch = StringUtil.NormalizeMarshaledString(szBatch);
                szCode = stream.readFixedLenString(10);
                // szCode = StringUtil.NormalizeMarshaledString(szCode);
                iCardCount = stream.readInt();
                iTim = stream.readInt();
                return stream.currentPos();
            }
        }
        //-------------------IOS充值相关------------------

        /// <summary>
        ///  CLIENT_CREATE_ORDER_REQ		0x02A0	// [厅客]->[厅服]创建订单请求消息
        /// </summary>
        public class ClientCreateOrderReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iChargeId; // 充值编号
            public int iChargeMode; // 充值模式：1苹果充值，2微信充值，3支付宝充值
            public int iChargeNumber; //打折和使用代金券后的实际充值金额，单位人民币分
            public int iUserVoucherId; // 使用代金券编号
            public int iVoucherAmount; // 使用代金券金额，单位人民币分
            public int iBoss; // 是否老板充值
#if UNITY_IOS
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string szOrderInfo; // 产生订单信息所需数据
#else
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string szOrderInfo; // 产生订单信息所需数据
#endif
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iChargeId);
                stream.writeInt(iChargeMode);
                stream.writeInt(iChargeNumber);
                stream.writeInt(iUserVoucherId);
                stream.writeInt(iVoucherAmount);
                stream.writeInt(iBoss);
#if UNITY_IOS
                stream.writeFixedLenString(szOrderInfo, 1024);

#else
                stream.writeFixedLenString(szOrderInfo, 1024);

#endif 
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        ///  CLIENT_CREATE_ORDER_RES	0x02A1	// [厅服]->[厅客]创建订单回应消息
        /// </summary>
        public class ClientCreateOrderRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iChargeId; // 充值编号
            public int iChargeMode; // 充值模式：1苹果充值
            public int iChargeNumber; // 充值金额，单位人民币分
            public int iUserVoucherId; // 使用代金券编号
            public int iVoucherAmount; // 使用代金券金额，单位人民币分
            public int iBoss; // 是否老板充值
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szOrderId; // 订单编号
#if UNITY_IOS
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string szOrderInfo; // 产生订单信息所需数据
#else
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string szOrderInfo; // 产生订单信息所需数据
#endif 
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iChargeId = stream.readInt();
                iChargeMode = stream.readInt();
                iChargeNumber = stream.readInt();
                iUserVoucherId = stream.readInt();
                iVoucherAmount = stream.readInt();
                iBoss = stream.readInt();
                szOrderId = stream.readFixedLenString(32);
#if UNITY_IOS
                szOrderInfo = stream.readFixedLenString(1024);

#else
                szOrderInfo = stream.readFixedLenString(1024);

#endif
                return stream.currentPos();
            }
        }

        const int MAX_RECEIPT_LEN = 7600;   // 收据最大长度
                                            /// <summary>
                                            /// CLIENT_CHARGE_REQ						0x02A2	// [厅客]->[厅服]充值请求消息
                                            /// </summary>
        public class ClientChargeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iChargeId; // 充值编号
            public int iChargeMode; // 充值模式：1苹果充值
            public int iChargeNumber; // 充值金额，单位人民币分
            public int iUserVoucherId; // 使用代金券编号
            public int iVoucherAmount; // 使用代金券金额，单位人民币分
            public int iBoss; // 是否老板充值
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szOrderId; // 订单编号
            public int iReceiptLength; // 收据数据长度
            public byte[] szReceipt; // 收据数据：
            internal ClientChargeReq()
            {

            }
            internal ClientChargeReq(int a, byte[] b)
            {
                iReceiptLength = a;
                szReceipt = b;
            }
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iChargeId);
                stream.writeInt(iChargeMode);
                stream.writeInt(iChargeNumber);
                stream.writeInt(iUserVoucherId);
                stream.writeInt(iVoucherAmount);
                stream.writeInt(iBoss);
                stream.writeFixedLenString(szOrderId, 32);
                stream.writeInt(iReceiptLength);
                for (int i = 0; i < iReceiptLength; i++)
                {
                    stream.writeByte(szReceipt[i]);
                }
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// CLIENT_CHARGE_RES						0x02A3	// [厅服]->[厅客]充值请求的回应消息
        /// </summary>
        public class ClientChargeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 用户编号
            public int iChargeId; // 充值编号
            public int iChargeMode; // 充值模式：1苹果充值

            public int iChargeNumber; // 充值金额，单位人民币分
            public int iUserVoucherId; // 使用代金券编号
            public int iVoucherAmount; // 使用代金券金额，单位人民币分
            public int iBoss; // 是否老板充值
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szOrderId; // 订单编号
            public int iReceiptLength; // 收据数据长度
                                       // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_RECEIPT_LEN)]
            public int[] iaCoin = new int[3]; // 获得3种金币数量
            public Queue szReceipt = new Queue(); // 收据数据：

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iChargeId = stream.readInt();
                iChargeMode = stream.readInt();
                iChargeNumber = stream.readInt();
                iUserVoucherId = stream.readInt();
                iVoucherAmount = stream.readInt();
                iBoss = stream.readInt();
                szOrderId = stream.readFixedLenString(32);
                iReceiptLength = stream.readInt();
                for (int i = 0; i < 3; i++)
                {
                    iaCoin[i] = stream.readInt();
                }
                for (int i = 0; i < iReceiptLength; i++)
                {
                    szReceipt.Enqueue(stream.readByte());
                }
                return stream.currentPos();
            }

        }

        public class UserCharge : Protocol
        {
            public int iChargeId; // 充值编号
            public int iChargeCount; // 这个编号的充值次数
            public int iLastChargeTime; // 这个充值编号的最后一次充值时间(time_t)

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iChargeId = stream.readInt();
                iChargeCount = stream.readInt();
                iLastChargeTime = stream.readInt();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iChargeId = stream.readInt();
                iChargeCount = stream.readInt();
                iLastChargeTime = stream.readInt();
            }
        }

        public class Exchange : Protocol
        {
            public int iExchangeId; // 兑换编号
            public int iCoin; // 兑换获得的金币数量
            public int iBindCoin; // 兑换获得的绑定金币数量
            public int iAsset; // 价格，单位人民币“元”
            public int iType; // 兑换类型:1业绩,2现金,3储值卡
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iExchangeId = stream.readInt();
                iCoin = stream.readInt();
                iBindCoin = stream.readInt();
                iAsset = stream.readInt();
                iType = stream.readInt();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                iExchangeId = stream.readInt();
                iCoin = stream.readInt();
                iAsset = stream.readInt();
                iType = stream.readInt();
            }
        }


        //CLIENT_USER_CHARGE_REQ        0x02B0	// [厅客]->[厅服]用户充值信息请求消息
        public class ClientUserChargeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo;
            public int iUserId; // 用户编号
            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                return stream.currentPos();
            }
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 充值信息回应 CLIENT_USER_CHARGE_RES = 0x02B1;// [厅服]->[厅客]用户充值信息回应消息
        /// </summary>
        public class ClientUserChargeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iNumber; // 充值信息数量，后面跟iNumber个UserChargeDef
                                // public Dictionary<int, UserCharge> usercharge = new Dictionary<int, UserCharge> ();
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iNumber = stream.readInt();
                //for (int i = 0; i < iNumber ; i++)
                //{
                //    usercharge.Add()
                //}
                return stream.currentPos();
            }
        }

        //CLIENT_CARD_PAY_NOTICE		0x0267	// [游服]->[桌服]/[桌服]->[厅服]代开房支付房卡通知消息
        public class ClientCardPayNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 代理的用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szOpenRoomId; // 开房序号，格式：服务器编号-序号
            public int iRoomCard; // 支付房卡的数量
            public int iFreeCard; // 支付赠卡的数量

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                szOpenRoomId = stream.readFixedLenString(16);
                iRoomCard = stream.readInt();
                iFreeCard = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_ROOM_NUM_SERVER_TABLE_REQ 0x0268	[厅客]->[厅服]房间号的服务器和桌信息请求消息
        public class ClientRoomNumServerTableReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iRoomNum; // 房间号
            public int iParlorId; //房间所属麻将馆的编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iRoomNum);
                stream.writeInt(iParlorId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_ROOM_NUM_SERVER_TABLE_RES		0x0269	// [厅服]->[厅客]房间号的服务器和桌信息回应消息
        public class ClientRoomNumServerTableRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号
            public int iUserId; // 代理的用户编号
            public int iRoomNum; // 房间号
            public int iServerId; // 服务器编号
            public short wTableNum; // 桌号
            public int[] iaUserId = new int[4]; // 用户编号           

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iRoomNum = stream.readInt();
                iServerId = stream.readInt();
                wTableNum = stream.readShort();

                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }

                return stream.currentPos();
            }
        }


        //#define CLIENT_LOTTERY_COUNT_REQ					0x02C0	//[厅客]->[厅服]增加抽奖次数请求
        public class ClientLotteryCountReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;                // 用户编号
            public byte byAddLotteryCount; // 要增加的抽奖次数
            public byte pos_0; // 
            public byte pos_1; // 
            public byte pos_2; //   


            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byAddLotteryCount);
                stream.writeByte(pos_0);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_LOTTERY_COUNT_RES					0x02C1	//[厅服]->[厅客]增加抽奖次数回应
        public class ClientLotteryCountResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;         // 错误编号
            public int iUserId;            // 用户编号
            public sbyte byAddLotteryCount; // 增加的抽奖次数

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                byAddLotteryCount = (sbyte)stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_LOTTERY_REQ							0x02C2	//[厅客]->[厅服]抽奖请求消息
        public class ClientLotteryReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;                // 用户编号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_LOTTERY_RES							0x02C3	//[厅服]->[厅客]抽奖回应消息
        public class ClientLotteryResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;         // 错误编号
            public int iUserId;            // 用户编号
            public byte byAwardCount;  // 奖品编号:1~10代表对应奖品,0代表没有奖品

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                byAwardCount = stream.readByte();
                return stream.currentPos();
            }
        }
        //#define ERR_CLIENTLOTTERYRES_NONE	1	// 没有了该奖项

        //#define CLIENT_USER_LOTTERY_REQ				0x02C4	// [厅客]->[厅服]用户抽奖活动信息请求消息
        public class ClientUserLotteryReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        // 用户活动信息
        public class UserLotteryDef : Protocol
        {
            public int iLotteryLoginTim; // 登录时间
            public int iLotteryLoginAwarTim; // 登录领奖时间
            public int iLotteryShareTim; // 分享时间
            public byte byLotteryCount; // 剩余抽奖次数

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);

                iLotteryLoginTim = stream.readInt();
                iLotteryLoginAwarTim = stream.readInt();
                iLotteryShareTim = stream.readInt();
                byLotteryCount = stream.readByte();
                return stream.currentPos();
            }
            public int parseBytes(FieldStream stream)
            {
                iLotteryLoginTim = stream.readInt();
                iLotteryLoginAwarTim = stream.readInt();
                iLotteryShareTim = stream.readInt();
                byLotteryCount = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_USER_LOTTERY_RES				0x02C5	// [厅服]->[厅客]用户抽奖活动信息回应消息
        public class ClientUserLotteryResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iBeginTime; // 抽奖活动起始时间
            public int iEndTime; // 抽奖活动结束时间
            public UserLotteryDef userLotteryDef = new UserLotteryDef(); //用户活动信息

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iBeginTime = stream.readInt();
                iEndTime = stream.readInt();
                userLotteryDef.parseBytes(stream);
                return stream.currentPos();
            }

        }

        //#define CLIENT_BESPEAK_REQ				0x1112	//[游客]->[游服] 用户占座请求消息
        public class ClientUserBespeakReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte byFromType;//消息来源，1来自大厅，2来自游戏客户端
            public int iUserId; // 用户编号
            public int iTotlaCoin;//用户金币数，当byFromType=1时由大厅服务器赋值
            public byte iSeatNum;//座位号
            public int iRoomNum;//房间号
            public int iParlorId;//麻将馆号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeByte(byFromType);
                stream.writeInt(iUserId);
                stream.writeInt(iTotlaCoin);
                stream.writeByte(iSeatNum);
                stream.writeInt(iRoomNum);
                stream.writeInt(iParlorId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_BESPEAK_RES				0x1113	//[游服]->[游客] 用户占座回应消息
        public class ClientUserBespeakResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public byte iSeatNum;//座位号
            public short wTableNum; //桌号
            public int iRoomNum;//房间号
            public byte[] szOpenRoomNum = new byte[16]; // 开房序号
            public byte byCoinCost;//抵押金币
            public byte m_pos0;//
            public byte m_pos1;//
            public byte m_pos2;//

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iSeatNum = stream.readByte();
                wTableNum = stream.readShort();
                iRoomNum = stream.readInt();
                for (int i = 0; i < 16; i++)
                {
                    szOpenRoomNum[i] = stream.readByte();
                }
                byCoinCost = stream.readByte();
                m_pos0 = stream.readByte();
                m_pos1 = stream.readByte();
                m_pos2 = stream.readByte();
                return stream.currentPos();
            }
        }
        //#define CLIENT_CANCLE_BESPEAK_REQ				0x1114	// [游客]->[游服]用户取消占座请求消息
        public class ClientUserCancleBespeakReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte byFromType;//消息来源，1来自大厅，2来自游戏客户端
            public int iUserId; // 用户编号
            public byte iSeatNum;//座位号
            public int iRoomNum;//房间号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeByte(byFromType);
                stream.writeInt(iUserId);
                stream.writeByte(iSeatNum);
                stream.writeInt(iRoomNum);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_RED_NUM_REQ				0x02E0	// [厅客]->[厅服]所有红包数量请求消息
        public class ClientRedNumReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_RED_NUM_RES				0x02E1	// [厅服]->[厅客]所有红包数量回应消息
        public class ClientRedNumResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
                                //public int[] iaReceive = new int[4]; //4种待领红包数量 0: 待领推广红包 1:待领提现红包 2:待领加入麻将馆红包 3:待领首次提现红包红包
            public int[,] iaRedPag = new int[16, 2]; //下标对应红包数量

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                //for (int i = 0; i < 4; i++)
                //{
                //    iaReceive[i] = stream.readInt();
                //}
                for (int i = 0; i < 16; i++)
                {
                    iaRedPag[i, 0] = stream.readInt();
                    iaRedPag[i, 1] = stream.readInt();
                }
                return stream.currentPos();
            }
        }

        //#define CLIENT_OPEN_RECEIVE_RED_REQ				0x02E2	// [厅客]->[厅服]领取红包请求消息
        public class ClientOpenReceiveRedReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iParlorId;//麻将馆Id
            public byte byRedPagType; //红包类型
            public byte pos_0; //
            public byte pos_1; //
            public byte pos_2; //

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);
                stream.writeByte(byRedPagType);
                stream.writeByte(pos_0);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_OPEN_RECEIVE_RED_RES				0x02E3	// [厅服]->[厅客]领取红包回应消息
        public class ClientReceiveRedResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
                                //public int[] iaRedPag = new int[4]; //4种待领红包数量 0: 待领推广红包 1:待领提现红包 2:待领加入麻将馆红包 3:待领首次提现红包红包
            public int[,] ia2RedPacket = new int[16, 2]; //下标对应红包数量
            public int iCreateVouchersttNow; //创建代金券时间(有效)
            public int iEffectiveTime; //代金券有效时间
            public int iRp3ID; //红包大红包ID (充值红包有效)
            public int dResourceNum; //资源数量
            public byte byRedPagType; // byAssetType 领取红包类型
            public byte byResourceType; //  byRedPacket 获得资源类型:1现金，2话费，3流量，4储值卡，（5代金券，6赠币没有对应字段）
            public byte pos_0; //
            public byte pos_1; //

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                for (int i = 0; i < 16; i++)
                {
                    ia2RedPacket[i, 0] = stream.readInt();
                    ia2RedPacket[i, 1] = stream.readInt();
                }
                iCreateVouchersttNow = stream.readInt();
                iEffectiveTime = stream.readInt();
                iRp3ID = stream.readInt();
                dResourceNum = stream.readInt();
                byRedPagType = stream.readByte();
                byResourceType = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                //byRedPagType = stream.readDouble();
                return stream.currentPos();
            }
        }


        //#define CLIENT_OBTAIN_RED_NOTICE			0x02E4	// [厅服]->[厅客]通知用户获得一个红包消息
        public class ClientObtainRedNoticeDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iRp3ID; // 红包大红包ID
            public byte byRedPagType; //红包下标类型 1创建房间红包、2推荐好友红包、3充值红包、4提现红包、5新手红包，6首次参与游戏红包，7加入麻将馆红包,8关注公众号红包，9实名认证，10大厅主推分享红包，11首次提现红包，12活动分享红包，13大赢家红包、14最佳炮手红包、15玩法红包、16提交bug红包、17麻将馆抢红包
            public byte byasset_type; //红包状态 1带领 2可用
            public byte pos_0; //
            public byte pos_1; //

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                iRp3ID = stream.readInt();
                byRedPagType = stream.readByte();
                byasset_type = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_OBTAIN_RECEIVE_RED_NOTICE		0x02E5	// [厅服 ]->[厅客]通知用户获得一个待领状态红包消息
        public class ClientObtainReceiveRedNoticeDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte byRedType; //红包类型:1创建房间红包、2推荐好友红包、3充值红包、4提现红包、5新手红包，6首次参与游戏红包，7加入麻将馆红包,8关注公众号红包，9实名认证，10大厅主推分享红包，11首次提现红包，12活动分享红包，13大赢家红包、14最佳炮手红包、15玩法红包、16提交bug红包、17麻将馆抢红包
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                byRedType = stream.readByte();
                return stream.currentPos();
            }
        }



        #region 麻将馆相关消息
        //#define CLIENT_CREATE_PARLOR_REQ				0x0240	// [厅客/游客]->[厅服/游服]创建麻将馆请求消息
        public class ClientCreateParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号
            public int iCityId;        // 市
            public int iCountyId;      // 县	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
            public string cParlorName;  //麻将馆昵称               

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iCityId);
                stream.writeInt(iCountyId);
                stream.writeFixedLenString(cParlorName, 28);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_CREATE_PARLOR_RES			0x0241	// [厅服]->[厅客]创建麻将馆回应消息
        public class ClientCreateParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iParlorId;  // 馆编号
            public int iCityId;    // 市
            public int iCountyId;  // 县 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
            public string cParlorName; //麻将馆昵称            

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                iCityId = stream.readInt();
                iCountyId = stream.readInt();
                cParlorName = stream.readFixedLenString(28);
                return stream.currentPos();
            }
        }


        //#define CLIENT_DISMISS_PARLOR_REQ				0x0242	// [厅客/游客]->[厅服/游服]解散麻将馆请求消息
        public class ClientDismissParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_DISMISS_PARLOR_RES				0x0243	// [厅服]->[厅客]解散麻将馆回应消息
        public class ClientDismissParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iCoinNum; //获得金币数量

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iCoinNum = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_CHANGE_PARLOR_BULLETIN_INFO_REQ			0x0244	// [厅客/游客]->[厅服/游服]修改麻将馆公告信息请求消息
        public class ClientChangeParlorBulletinInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 360)]
            public string szBulletinContent;  //公告内容，最多120个汉字  

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szBulletinContent, 360);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_CHANGE_PARLOR_BULLETIN_INFO_RES			0x0245	// [厅服]->[厅客]修改麻将馆公告信息回应消息
        public class ClientChangeParlorBulletinInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号           
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 360)]
            public string szBulletinContent; // 公告内容，最多120个汉字 

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                szBulletinContent = stream.readFixedLenString(360);
                return stream.currentPos();
            }
        }

        //#define CLIENT_CHANGE_PARLOR_CONTACT_INFO_REQ			0x0246	// [厅客/游客]->[厅服/游服]修改麻将馆联系信息请求消息
        public class ClientChangeParlorContactInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string szContact;  //麻将馆联系方式 

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeFixedLenString(szContact, 24);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_CHANGE_PARLOR_CONTACT_INFO_RES			0x0247	// [厅服]->[厅客]修改麻将馆联系信息回应消息
        public class ClientChangeParlorContactInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号           
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string szContact; // 公告内容，最多120个汉字 

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                szContact = stream.readFixedLenString(24);
                return stream.currentPos();
            }
        }


        //CLIENT_JOIN_PARLOR_REQ					0x0248	// [厅客]->[厅服]申请 加入麻将馆请求消息
        public class ClientJoinParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            public int iParlorId;      // 馆编号 

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);

                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_JOIN_PARLOR_RES					0x0249	// [厅服]->[厅客]申请 加入麻将馆回应消息
        public class ClientJoinParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号           
            public int iParlorId;      // 馆编号       

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define  CLIENT_LEAVE_PARLOR_REQ					0x024A	// [厅客]->[厅服]申请 退出麻将馆请求消息
        public class ClientLeaveParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            public int iParlorId;      // 馆编号 

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);

                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_LEAVE_PARLOR_RES					0x024B	// [厅服]->[厅客]申请 退出麻将馆回应消息
        public class ClientLeaveParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                           
            public int iParlorId; //馆的id                           

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_INVITE_PARLOR_REQ				0x024C	// [厅客]->[厅服]邀请用户进入麻将馆请求消息
        public class ClientInvitParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            public int iParlorId;      // 馆编号 
            public int iInviteUserId;   // 邀请用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);
                stream.writeInt(iInviteUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_INVITE_PARLOR_RES				0x024D	// [厅服]->[厅客]邀请用户进入麻将馆回应消息
        public class ClientInvitParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                           
            public int iParlorId;      // 馆编号
            public int iInviteUserId;   // 邀请用户编号
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                iInviteUserId = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_KICK_PARLOR_REQ					0x024E	// [厅客]->[厅服]踢用户出麻将馆请求消息
        public class ClientKickParlorReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            public int iParlorId;      // 馆编号 
            public int iKickUserId;     // 踢出用户编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);
                stream.writeInt(iKickUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_KICK_PARLOR_RES					0x024F	// [厅服]->[厅客]踢用户出麻将馆回应消息
        public class ClientKickParlorRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                           
            public int iParlorId;      // 馆编号
            public int iKickUserId; // 踢出用户编号
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                iKickUserId = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_GETPARLORINFO_REQ				0x0255	// [厅客]->[厅服]获取麻将馆信息请求消息
        public class ClientGetParlorInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号                        
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //麻将馆的信息
        public class ParlorInfoDef : Protocol
        {
            /// <summary>
            ///  麻将馆id
            /// </summary>
            public int iParlorId; //
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
            public string szParlorName;
            /// <summary>
            /// 馆主ID
            /// </summary>
            public int iBossId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szBossNickname; // 昵称
            /// <summary>
            /// 微信头像网址
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szBossHeadimgurl;
            /// <summary>
            ///  成员人数
            /// </summary>
            public int iMemberNum;
            /// <summary>
            /// 总活跃度
            /// </summary>
            public int iVitality;
            /// <summary>
            ///  月活跃度
            /// </summary>
            public int iMonthVitality;
            /// <summary>
            /// 创建时间
            /// </summary>
            public int iCreateTime;
            /// <summary>
            /// 市id
            /// </summary>
            public int iCityId;
            /// <summary>
            /// 县id
            /// </summary>
            public int iCountyId;
            /// <summary>
            /// 麻将馆状态 0正常  1封馆-  5表示申请状态（自己设置，非服务器提供，用于显示申请界面）
            /// </summary>
            public int iStatus;
            /// <summary>
            /// // 公告
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 360)]
            public string szBulletin;
            /// <summary>
            /// // 麻将馆联系方式
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string szContact;
            /// <summary>
            /// // 加入麻将馆红包状态:0没有红包,1有可用红包,2表示失败
            /// </summary>
            public int iRp7Type;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iParlorId = stream.readInt(); // 麻将馆id
                szParlorName = stream.readFixedLenString(28);
                //  szParlorName = StringUtil.NormalizeMarshaledString(szParlorName).Trim();
                iBossId = stream.readInt(); // 馆主ID
                szBossNickname = stream.readFixedLenString(52);
                //  szBossNickname = StringUtil.NormalizeMarshaledString(szBossNickname).Trim();
                szBossHeadimgurl = stream.readFixedLenString(160);
                //   szBossHeadimgurl = StringUtil.NormalizeMarshaledString(szBossHeadimgurl).Trim();
                iMemberNum = stream.readInt(); // 成员人数
                iVitality = stream.readInt(); // 总活跃度
                iMonthVitality = stream.readInt(); // 月活跃度
                iCreateTime = stream.readInt(); // 创建时间
                iCityId = stream.readInt();
                iCountyId = stream.readInt();
                iStatus = stream.readInt();
                szBulletin = stream.readFixedLenString(360); // 公告         
                                                             // szBulletin = StringUtil.NormalizeMarshaledString(szBulletin).Trim();
                szContact = stream.readFixedLenString(24); // 麻将馆联系方式   
                                                           //  szContact = StringUtil.NormalizeMarshaledString(szContact).Trim();
                iRp7Type = stream.readInt();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iParlorId = stream.readInt(); // 麻将馆id
                szParlorName = stream.readFixedLenString(28);
                // szParlorName = StringUtil.NormalizeMarshaledString(szParlorName.Trim());
                iBossId = stream.readInt(); // 馆主ID
                szBossNickname = stream.readFixedLenString(52);
                //  szBossNickname = StringUtil.NormalizeMarshaledString(szBossNickname.Trim());
                szBossHeadimgurl = stream.readFixedLenString(160);
                // szBossHeadimgurl = StringUtil.NormalizeMarshaledString(szBossHeadimgurl.Trim());
                iMemberNum = stream.readInt(); // 成员人数
                iVitality = stream.readInt(); // 总活跃度
                iMonthVitality = stream.readInt(); // 月活跃度
                iCreateTime = stream.readInt(); // 创建时间
                iCityId = stream.readInt();
                iCountyId = stream.readInt();
                iStatus = stream.readInt();
                szBulletin = stream.readFixedLenString(360); // 公告
                                                             //  szBulletin = StringUtil.NormalizeMarshaledString(szBulletin).Trim();
                Debug.LogError("+" + szBulletin + "+");
                szContact = stream.readFixedLenString(24); // 麻将馆联系方式
                                                           //  szContact = StringUtil.NormalizeMarshaledString(szContact.Trim());
                iRp7Type = stream.readInt();
            }
        }


        //#define CLIENT_GETPARLORINFO_RES				0x0256	// [厅服]->[厅客]获取麻将馆信息回应消息
        public class ClientGetParlorInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                           
            public int byParlorNum; //用户加入麻将馆的数量,后面跟byPaolorNum个ParlorInfoDef            

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                byParlorNum = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_PARLOR_CERT_REQ					0x02D0	// [厅客]->[厅服是否有开麻将馆权限请求消息
        public class ClientParlorCertReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号               
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_PARLOR_CERT_RES					0x02D1	// [厅服]->[厅客]是否有开麻将馆权限回应消息
        public class ClientParlorCertRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                           
            public byte Type;// 状态:0没有 1有
            public byte pos_1;
            public byte pos_2;
            public byte pos_3;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                Type = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                pos_3 = stream.readByte();
                return stream.currentPos();
            }
        }

        //玩家节点信息
        public class UserInfoDef : Protocol
        {
            public int iUserId; // 用户编号     
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname; // 昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl; // 微信头像网址            
            public int[] iaCoin = new int[3]; // 当前金币数量币数量：1安卓币、2苹果币、3赠币
            public int iCompliment; // 赞的数量
            public int iGameNumAcc; // 游戏局数累计
            public int iDisconnectAcc; // 掉线次数累计
            public int iPlayCardAcc; // 出牌次数累计
            public int iPlayCardTimeAcc; // 出牌时间累计
            public float fLongitude; // 经度
            public float fLatitude; // 纬度
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szAddress; // 经纬度对应的地址
            public byte bySex; // 性别
            public byte bySeatNum; // 座位号，从1开始
            public byte byReady; // 在桌上是否准备好了，0没准备好，1准备好了
            public byte byEscape; // 是否离开
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szDui; // 设备唯一标识符(deviceUniqueIdentifier)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szIp; // IP
            public int iLeaveParlorAcc; // 用户退出麻将馆次数累积
            public int iKickParlorAcc; // 用户被踢出麻将馆次数累积
            public byte byUserSource; // 用户来源：1游客，2微信
            public byte byParlorType; // 用户麻将馆状态:0普通用户查看普通用户,1不在馆内,2申请中,3在馆内
            public byte pos1;
            public byte pos2;

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iUserId = stream.readInt(); // 用户编号
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址    
                for (int i = 0; i < 3; i++)
                {
                    iaCoin[i] = stream.readInt(); // 当前金币数量
                }
                iCompliment = stream.readInt(); // 赞的数量
                iGameNumAcc = stream.readInt(); // 游戏局数累计
                iDisconnectAcc = stream.readInt(); // 掉线次数累计
                iPlayCardAcc = stream.readInt(); // 出牌次数累计
                iPlayCardTimeAcc = stream.readInt(); // 出牌时间累计
                fLongitude = stream.readFloat(); // 经度
                fLatitude = stream.readFloat(); // 纬度            
                szAddress = stream.readFixedLenString(48); // 经纬度对应的地址
                bySex = stream.readByte(); // 性别
                bySeatNum = stream.readByte(); // 座位号，从1开始
                byReady = stream.readByte(); // 在桌上是否准备好了，0没准备好，1准备好了
                byEscape = stream.readByte(); // 是否离开            
                szDui = stream.readFixedLenString(48); // 设备唯一标识符(deviceUniqueIdentifier)            
                szIp = stream.readFixedLenString(40); // IP                
                iLeaveParlorAcc = stream.readInt(); // 用户退出麻将馆次数累积
                iKickParlorAcc = stream.readInt(); // 用户被踢出麻将馆次数累积
                byUserSource = stream.readByte(); // 用户来源：1游客，2微信
                byParlorType = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iUserId = stream.readInt(); // 用户编号
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址    
                for (int i = 0; i < 3; i++)
                {
                    iaCoin[i] = stream.readInt(); // 当前金币数量
                }
                iCompliment = stream.readInt(); // 赞的数量
                iGameNumAcc = stream.readInt(); // 游戏局数累计
                iDisconnectAcc = stream.readInt(); // 掉线次数累计
                iPlayCardAcc = stream.readInt(); // 出牌次数累计
                iPlayCardTimeAcc = stream.readInt(); // 出牌时间累计
                fLongitude = stream.readFloat(); // 经度
                fLatitude = stream.readFloat(); // 纬度            
                szAddress = stream.readFixedLenString(48); // 经纬度对应的地址
                bySex = stream.readByte(); // 性别
                bySeatNum = stream.readByte(); // 座位号，从1开始
                byReady = stream.readByte(); // 在桌上是否准备好了，0没准备好，1准备好了
                byEscape = stream.readByte(); // 是否离开            
                szDui = stream.readFixedLenString(48); // 设备唯一标识符(deviceUniqueIdentifier)            
                szIp = stream.readFixedLenString(40); // IP                
                iLeaveParlorAcc = stream.readInt(); // 用户退出麻将馆次数累积
                iKickParlorAcc = stream.readInt(); // 用户被踢出麻将馆次数累积
                byUserSource = stream.readByte(); // 用户来源：1游客，2微信
                byParlorType = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
            }
        }


        //#define CLIENT_GETUSERINFO_REQ				0x0253	// [厅客]->[厅服]获取用户信息请求消息
        public class ClientGetUserInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号     
            public int iFindUserId; //要查找的用户编号    
            public int iParlorId; //麻将馆的id      
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iFindUserId);
                stream.writeInt(iParlorId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_GETUSERINFO_RES				0x0254	// [厅服]->[厅客]获取用户信息回应消息
        public class ClientGetUserInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                                       
            public UserInfoDef userInfo = new UserInfoDef();
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                userInfo.parseBytes(stream);
                return stream.currentPos();
            }
        }

        //        public const ushort CLIENT_GET_TABLE_USERINFO_REQ = 0x0257;	// [厅客]->[厅服]获取桌上用户信息请求消息
        public class ClientGetTableUserIDReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int RoomNum;        // 房间编号      
            public int[] iBespeakUserId = new int[4];//桌上预约用户ID
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(RoomNum);
                for (int i = 0; i < 4; i++)
                {
                    stream.writeInt(iBespeakUserId[i]);
                }
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        public class ClientGetTableUserIDRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int[] iUserId = new int[4]; // 四个玩家id

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    iUserId[i] = stream.readInt();
                }

                return stream.currentPos();
            }
        }

        // CLIENT_GET_TABLE_USER_INFO_REQ				0x025B	// [厅客]->[厅服]获取用户信息请求消息
        public class ClientGetTableUserInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int userid;
            public int[] iUserId = new int[4];//用户ID
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(userid);
                for (int i = 0; i < 4; i++)
                {
                    stream.writeInt(iUserId[i]);
                }

                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //public const ushort CLIENT_GET_TABLE_USERINFO_RES = 0x0258; // [厅服]->[厅客]获取桌上用户信息回应消息
        public class ClientGetTableUserInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int UserNum; //玩家数量

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                UserNum = stream.readInt();

                return stream.currentPos();
            }

        }
        public class TableUserInfoDef : Protocol
        {
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 52)]
            public string szNickname; // 昵称
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public string szHeadimgurl; // 微信头像网址
            public int iCompliment; // 赞的数量
            public int iDisconnectAcc; // 掉线次数累计
            public int iPlayCardAcc; // 出牌次数累计
            public int iPlayCardTimeAcc; // 出牌时间累计
            public int iGameNumAcc; // 游戏局数累计
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                iUserId = stream.readInt();
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160);
                iCompliment = stream.readInt();
                iDisconnectAcc = stream.readInt();
                iPlayCardAcc = stream.readInt();
                iPlayCardTimeAcc = stream.readInt();
                iGameNumAcc = stream.readInt();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iUserId = stream.readInt();
                szNickname = stream.readFixedLenString(52);
                szHeadimgurl = stream.readFixedLenString(160);
                iCompliment = stream.readInt();
                iDisconnectAcc = stream.readInt();
                iPlayCardAcc = stream.readInt();
                iPlayCardTimeAcc = stream.readInt();
                iGameNumAcc = stream.readInt();
            }
        }



        //#define CLIENT_BOSSSCORE_TO_COIN_REQ			0X02D2	//[厅客]->[厅服]兑换金币请求消息
        public class ClientScoreToCoinReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;          // 用户编号
            public int iExchangeId;		// 兑换的编号
            public byte byExchange;	// 兑换类型:1业绩,2现金,3储值卡
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iExchangeId);
                stream.writeInt(byExchange);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_BOSSSCORE_TO_COIN_RES			0X02D3	//[厅服]->[厅客]兑换金币回应消息
        public class ClientScoreToCoinRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId;        // 用户编号
            public int iExchangeId;		// 兑换的编号
            public byte byExchange;	// 兑换类型:1业绩,2现金,3储值卡
            public byte[] b = new byte[3];
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iExchangeId = stream.readInt();
                byExchange = stream.readByte();
                for (int i = 0; i < 3; i++)
                {
                    b[i] = stream.readByte();
                }
                return stream.currentPos();
            }
        }

        //#define CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_REQ			0X02D4	//[厅客]->[厅服 计服]取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆请求消息
        public class ClientCAncelApplyOrJudgeApplyTooReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号
            public int iParlorId;  // 馆编号
            public int iType;   // 消息类型:1请求状态 2 取消状态
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);
                stream.writeInt(iType);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_CANCEL_APPLY_OR_JUDGE_APPLY_TOO_RES			0X02D5	//[厅服]->[厅客]取消申请加入麻将馆 或者 判断是否申请过加入这个麻将馆回应消息
        public class ClientCAncelApplyOrJudgeApplyTooRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;         //错误编号
            public int iUserId;        // 用户编号
            public int iParlorId;  // 馆编号
            public int iType;  // 消息类型:1请求状态 2 取消状态
            public int iStaus;//0 没有申请过 1 已经申请过


            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                iType = stream.readInt();
                iStaus = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_REFRESH_USER_REQ					0x0226	// [厅客]->[厅服]刷新用户信息请求消息
        public class ClientRefreshUserReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号           
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_REFRESH_USER_RES		0x0227	// [厅客]->[厅服]刷新用户信息回应消息
        public class ClientRefreshUserRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;         //错误编号
            public int iUserId;        // 用户编号
            public ClientUserDef clientUser = new ClientUserDef();  //客户端用户信息节点

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                clientUser.parseBytes(stream);
                return stream.currentPos();
            }
        }

        //#define CLIENT_GET_PARLOR_TABLEINFO_REQ		0x0259	//	[厅客]->[厅服]获取麻将馆内某一页的桌信息请求消息
        public class ClientGetParlorTableInfoReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iParlorId; // 麻将馆编号
            public int iPageNum; // 获取第几页的信息,从0开始
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iParlorId);
                stream.writeInt(iPageNum);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }


        //麻将馆的信息
        public class TableInfoDef : Protocol
        {
            public int iParlorId; // 房间所属馆id
            public int iSeverId; // 房间所属服务器编号
            public int iRoomNum; //房间号
            public int iOpenRoomId; //开房人ID
            public int iOpenRoomTime; //开房时间
            public int iBespeakTime; //预约时间 (单位:分)      
            public int iPlayMethod; //玩法编号     
            public int[] iaUserId = new int[4]; //座位上的用户ID 
            public int[] iaBespeakUserId = new int[4]; //桌上的预约用户ID
            public uint[] param = new uint[OpenRoomParamRow]; //开房参数          
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                iParlorId = stream.readInt();
                iSeverId = stream.readInt(); // 麻将馆id                
                iRoomNum = stream.readInt(); // 馆主ID
                iOpenRoomId = stream.readInt();
                iOpenRoomTime = stream.readInt();
                iBespeakTime = stream.readInt();
                iPlayMethod = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    iaBespeakUserId[i] = stream.readInt();
                }
                for (int i = 0; i < param.Length; i++)
                {
                    param[i] = stream.readUint ();
                }
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                iParlorId = stream.readInt();
                iSeverId = stream.readInt(); // 麻将馆id                
                iRoomNum = stream.readInt(); // 馆主ID
                iOpenRoomId = stream.readInt();
                iOpenRoomTime = stream.readInt();
                iBespeakTime = stream.readInt();
                iPlayMethod = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    iaUserId[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    iaBespeakUserId[i] = stream.readInt();
                }
                for (int i = 0; i < param.Length; i++)
                {
                    param[i] = stream.readUint ();
                }
            }
        }


        //#define CLIENT_GET_PARLOR_TABLEINFO_RES		0X025A	//	[厅服]->[厅客]获取麻将馆内某一页的桌信息回应
        public class ClientGetParlorTableInfoRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; //错误编号
            public int iUserId; // 用户编号
            public int iParlorId; // 麻将馆编号
            public int iPageNum; // 获取第几页的信息,从0开始
            public int iTableNum; // 获取到的桌的数量

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iParlorId = stream.readInt();
                iPageNum = stream.readInt();
                iTableNum = stream.readInt();
                return stream.currentPos();
            }
        }

        //CLIENT_RP17_TYPE_REQ0x1127	//[厅客]->[厅服] 获取红包状态请求消息
        public class ClientRp17TypeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;        // 用户编号  
            public int iRp17Id; // 红包编号 
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iRp17Id);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //CLIENT_RP17_TYPE_RES									0x1128	//[厅服]->[厅客] 红包状态回应消息
        public class ClientRp17TypeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;         //错误编号
            public int iUserId;        // 用户编号
            public int iRp17Id;        // 红包状态
            public byte byRp17Type;
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iRp17Id = stream.readInt();
                byRp17Type = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_CANCLE_BESPEAK_RES				0x1115	//[游服]->[游客] 用户取消占座回应消息
        public class ClientUserCancleBespeakResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public byte iSeatNum;//座位号
            public byte pos_0;//
            public byte pos_1;//
            public byte pos_2;//

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iSeatNum = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        #endregion
    }
}

