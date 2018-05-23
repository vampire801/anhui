using UnityEngine;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using Common;
using System;
using XLua;

namespace MahjongGame_AH.Network.Message
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
        public const byte KEEP_ALIVE = 0x0006; //保持连接
        public const byte CLIENT_CONNECT = 0x0001; //客户端连接消息
        public const byte CLIENT_DISCONNECT = 0x0002; //客户端断开连接消息
        public const byte SERVER_CONNECT = 0x0003; //服务器连接消
        public const byte SERVER_DISCONNECT = 0x0004; // 服务器断开连接消息
        public const byte SERVER_RECONNECT = 0x0005;  // 服务器重连成功消息
        public const byte KICK_USER_NOTICE = 0x0007;  //玩家被踢下线
        public const ushort CLIENT_SERVER_REQ = 0x0100; //[厅客/游客]->[厅网/游网]大厅/游戏客户端请求大厅/游戏服务器的请求消息(连接成功后发送）
        public const ushort CLIENT_SERVER_RES = 0x0101; //[厅网/游网]->[厅客/游客]大厅/游戏客户端请求大厅/游戏服务器的回应消息

        public const ushort CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public const ushort CLIENT_AUTHEN_RES = 0x0211;// [厅服/游服]->[厅客/游客]认证回应消息

        public const ushort CLIENT_AGAIN_LOGIN_RES = 0x1000;  // [游服]->[游客]断线重入回应消息
        public const ushort CLIENT_OTHER_AGAIN_LOGIN_NOTICE = 0x1001;	// [游服]->[游客]他人断线重入通知消息
        public const ushort CLIENT_USER_JOIN_TABLE = 0x1002;	// [游服]->[游客]用户入座消息
        public const ushort CLIENT_USER_LEAVE_TABLE = 0x1003;	// [游服]->[游客]用户离座消息
        public const ushort CLIENT_SIT_REQ = 0x1006;	// [游客]->[游服]坐下请求消息
        public const ushort CLIENT_SIT_RES = 0x1007;	// [游服]->[游客]坐下回应消息
        public const ushort CLIENT_ESCAPE_REQ = 0x1008;	// [游客]->[游服]离开请求消息
        public const ushort CLIENT_ESCAPE_RES = 0x1009;	// [游服]->[游客]离开回应消息
        public const ushort CLIENT_READY_REQ = 0x100A;	// [游客]->[游服]准备请求消息
        public const ushort CLIENT_READY_RES = 0x100B;	// [游服]->[游客]准备回应消息
        public const ushort CLIENT_DISMISS_ROOM_REQ = 0x1010;	// [游客]->[游服]解散房间请求消息
        public const ushort CLIENT_DISMISS_ROOM_RES = 0x1011;	// [游服]->[游客]解散房间回应消息
        public const ushort CLIENT_DISMISS_ROOM_NOTICE = 0x1012;	// [游服]->[游客]解散房间通知消息

        public const ushort CLIENT_VOICE_REQ = 0x1020;	// [游客]->[游服]语音请求消息
        public const ushort CLIENT_VOICE_RES = 0x1021;	// [游服]->[游客]语音回应消息

        //麻将相关消息
        public const ushort CLIENT_DEAL_TILE_NOTICE = 0x1100;   // [游服]->[游客]发牌通知消息
        public const ushort CLIENT_DISCARD_TILE_NOTICE = 0x1101;	// [游服]->[游客]出牌通知消息
        public const ushort CLIENT_DISCARD_TILE_REQ = 0x1102;	// [游客]->[游服]出牌请求消息
        public const ushort CLIENT_DISCARD_TILE_RES = 0x1103;	// [游服]->[游客]出牌回应消息
        public const ushort CLIENT_SPECIAL_TILE_NOTICE = 0x1104;    // [游服]->[游客]吃碰杠胡抢通知消息
        public const ushort CLIENT_SPECIAL_TILE_REQ = 0x1105;	// [游客]->[游服]吃碰杠胡抢请求消息
        public const ushort CLIENT_SPECIAL_TILE_RES = 0x1106;//[游服]->[游客]吃碰杠胡抢回应消息
        public const ushort CLIENT_REALTIME_POINT_NOTICE = 0x1107;// [游服]->[游客]即时计分通知消息
        public const ushort CLIENT_THIRTEEN_ORPHANS_REQ = 0x1108;//[游客]->[游服]长治花三门的十三幺请求消息
        public const ushort CLIENT_THIRTEEN_ORPHANS_RES = 0x1109;//[游服]->[游客]长治花三门的十三幺回应消息

        public const ushort CLIENT_GAME_RESULT_NOTICE = 0x1200;	// [游服]->[游客]一局游戏结果通知消息
        public const ushort CLIENT_ROOM_RESULT_NOTICE = 0x1201;	// [游服]->[游客]房间游戏结果通知消息

        public const ushort CLIENT_PLAYING_METHOD_CONF_REQ = 0x1202;	// [游客]->[游服]玩法配置请求消息
        public const ushort CLIENT_PLAYING_METHOD_CONF_RES = 0x1203;    // [游服]->[游客]玩法配置回应消息

        public const ushort CLIENT_CHAT_REQ = 0x1022;	// [游客]->[游服]聊天请求消息
        public const ushort CLIENT_CHAT_RES = 0x1023;   // [游服]->[游客]聊天回应消息
                                                        //新增消息
        public const ushort CLIENT_CAN_DOWN_RU_NOTICE = 0x1204; // [游服]->[游客]长治潞城麻将的能跑能下通知消息
        public const ushort CLIENT_CAN_DOWN_RU_REQ = 0x1205;// [游客]->[游服]长治潞城麻将的能跑能下请求消息
        public const ushort CLIENT_CAN_DOWN_RU_RES = 0x1206;	// [游服]->[游客]长治潞城麻将的能跑能下回应消息

        public const ushort CLIENT_READHAND_REQ = 0x1207;	//[游客]->[游服]报听请求消息
        public const ushort CLIENT_READHAND_RES = 0x1208;   //[游客]->[游服]报听回应消息

        public const ushort CLIENT_LAIZI_NOTICE = 0x1300;   //[游服]->[游客]癞子牌通知消息

        public const ushort CLIENT_BESPEAK_REQ = 0x1112;    //[游客]->[游服] 用户占座请求消息
        public const ushort CLIENT_BESPEAK_RES = 0x1113;    //[游服]->[游客] 用户占座回应消息
        public const ushort CLIENT_CANCLE_BESPEAK_REQ = 0x1114; // [游客]->[游服]用户取消占座请求消息
        public const ushort CLIENT_CANCLE_BESPEAK_RES = 0x1115; //[游服]->[游客] 用户取消占座回应消息
        public const ushort CLIENT_BESPEAK_USERINFO_NOTICE = 0x1123;	//[游服]->[游客] 发送桌上游戏外占座玩家信息

        public const ushort SERVER_TO_CLIENT_WAIT_READY_NOTICE = 0x1119;	//[游服]->[游客]四人入座等待用户准备/取消准备通知
        public const ushort KICK_OUT_PLAYER_WITHOUT_READY = 0x111A; //[游服]->[游客]踢出为准备用户通知      



        //麻将馆红包相关消息
        public const ushort CLIENT_RP17_START_NOTICE = 0x1120;	//	[游服]->[游客]开始抢红包通知
        public const ushort CLIENT_OPEN_RP17_REQ = 0x1121;	//	[游客]->[游服]抢红包请求
        public const ushort CLIENT_OPEN_RP17_RES = 0x1122;  //	[游服]->[游客]抢红包回应

        public const ushort CLIENT_SHARE_SUCCESS_REQ = 0x029A;	// [厅客]->[厅服]分享成功请求消息
        public const ushort CLIENT_SHARE_SUCCESS_RES = 0x029B;	// [厅服]->[厅客]分享成功回应消息

        public const ushort CLIENT_OBTAIN_RED_NOTICE = 0x02E4;	// [厅服]->[厅客]通知用户获得一个红包消息
        public const ushort CLIENT_OBTAIN_RECEIVE_RED_NOTICE = 0x02E5;  // [厅服 ]->[厅客]通知用户获得一个待领状态红包消息  

        public const ushort CLIENT_OPEN_RECEIVE_RED_REQ = 0x02E2;	// [厅客]->[厅服]领取红包请求消息
        public const ushort CLIENT_OPEN_RECEIVE_RED_RES = 0x02E3;	// [厅服]->[厅客]领取红包回应消息

        //托管相关
        public const ushort SERVER_TO_CLIENT_AUTO_STATUS = 0x1116;	//[游服]->[游客] 发送用户托管状态给客户端
        public const ushort CLIENT_CANCLE_AUTO_STATUS_REQ = 0x1117;	//[游客]->[游服]用户取消托管请求消息
        public const ushort CLIENT_CANCLE_AUTO_STATUS_RES = 0x1118; //[游客]->[游服]用户取消托管回应消息

        public const ushort CLIENT_READY_TIME_REQ = 0x1124;	//[游客]->[游服]获取准备剩余时间请求
        public const ushort CLIENT_READY_TIME_RES = 0x1125;	//[游服]->[游客]获取准备剩余时间回应
        public const ushort BESPEAK_TIME_OUT_NOTICE = 0x1126;   //[游服]->[游客]预约时间结束通知

        //public const ushort CLIENT_NEXT_POINT_NOTICE = 0x1211;// [游服]->[游客]下分通知消息
        //public const ushort CLIENT_NEXT_POINT_REQ = 0x1212;// [游客]->[游服]下分请求消息
        //public const ushort CLIENT_NEXT_POINT_RES = 0x1213;	// [游服]->[游客]下分回应消息
        public const ushort CLIENT_APPLIQUE_NOTICE = 0x1209;	//[游客]->[游服]补花通知消息
        public const ushort CLIENT_APPLIQUE_REQ = 0x1210;	//[游客]->[游服]补花请求消息
        public const ushort CLIENT_APPLIQUE_RES = 0x1211;   //[游客]->[游服]补花回应消息

        // public const ushort CLIENT_GET_TABLE_USERID_REQ = 0x0257;	// [厅客]->[厅服]获取桌上用户ID请求消息
        // public const ushort CLIENT_GET_TABLE_USERID_RES = 0x0258; // [厅服]->[厅客]获取桌上用户ID回应消息



        #endregion 消息编号


        #region 消息定义
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
            public int iPort; // 可连接的服务器的端口
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
                // szIP = StringUtil.NormalizeMarshaledString(szIP.Trim());
                domain = stream.readFixedLenString(40);
                //   domain = StringUtil.NormalizeMarshaledString(domain);
                iPort = stream.readInt();

                return stream.currentPos();
            }
            //public void NormalizeMarshaledString()
            //{
            //    szIP = StringUtil.NormalizeMarshaledString(szIP);
            //    domain = StringUtil.NormalizeMarshaledString(domain);
            //}
        }


        //CLIENT_AUTHEN_REQ = 0x0210;// [厅客/游客]->[厅服/游服]认证请求消息
        public class ClientAuthenReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public short wVer; // 版本号
            public sbyte iAuthenType; // 认证类型：
                                      // 1.微信access_token登录模式，填写szToken
                                      // 2.微信refresh_token登录模式，填写szToken
                                      // 3.用户编号登录，填写iUserId，szToken
            public int iUserId; // 用户编号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szToken; // 根据认证类型不同：
                                   // iAuthenType=1：微信code
                                   // iAuthenType=2：微信refresh_token
                                   // iAuthenType=3：微信access_token
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szDui; // 设备唯一标识符(deviceUniqueIdentifier)【所有认证类型都填写】
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szIp; // IP【所有认证类型都填写】
            public float fLongitude; // 经度【所有认证类型都填写】
            public float fLatitude; // 纬度【所有认证类型都填写】
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string szAddress; // IP【所有认证类型都填写】
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
                stream.writeShort((short)wVer);
                stream.writeByte((byte)iAuthenType);
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
            public byte byMemberFirstChargeAward; // 是否得过会员首充双倍
            public byte byBossFirstChargeAward; // 是否得过老板首充双倍
            public byte byFirstInParlor;//用户是否进入过麻将馆
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
                                                            //  szNickname = StringUtil.NormalizeMarshaledString(szNickname);
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址
                                                               //  szHeadimgurl = StringUtil.NormalizeMarshaledString(szHeadimgurl);
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


        /// <summary>
        /// 游戏内玩家信息
        /// </summary>
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
            public byte cSeatNum; // 座位号，从1开始
            public byte cReady; // 在桌上是否准备好了，0没准备好，1准备好了
            public byte byDisconnectType; // 是否离开
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
                // szNickname = StringUtil.NormalizeMarshaledString(szNickname.Trim());
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址    
                                                               //  szHeadimgurl = StringUtil.NormalizeMarshaledString(szHeadimgurl.Trim());
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
                cSeatNum = stream.readByte(); // 座位号，从1开始
                cReady = stream.readByte(); // 在桌上是否准备好了，0没准备好，1准备好了
                byDisconnectType = stream.readByte(); // 是否离开            
                szDui = stream.readFixedLenString(48); // 设备唯一标识符(deviceUniqueIdentifier)            
                szIp = stream.readFixedLenString(40); // IP      
                                                      //  szIp = StringUtil.NormalizeMarshaledString(szIp).Trim();
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
                //   szNickname = StringUtil.NormalizeMarshaledString(szNickname);
                szHeadimgurl = stream.readFixedLenString(160); // 微信头像网址                   
                                                               //  szHeadimgurl = StringUtil.NormalizeMarshaledString(szHeadimgurl);
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
                cSeatNum = stream.readByte(); // 座位号，从1开始
                cReady = stream.readByte(); // 在桌上是否准备好了，0没准备好，1准备好了
                byDisconnectType = stream.readByte(); // 是否离开            
                szDui = stream.readFixedLenString(48); // 设备唯一标识符(deviceUniqueIdentifier)            
                szIp = stream.readFixedLenString(40); // IP   
                                                      //      szIp = StringUtil.NormalizeMarshaledString(szIp).Trim();
                iLeaveParlorAcc = stream.readInt(); // 用户退出麻将馆次数累积
                iKickParlorAcc = stream.readInt(); // 用户被踢出麻将馆次数累积
                byUserSource = stream.readByte(); // 用户来源：1游客，2微信
                byParlorType = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
            }
        }

        //顺子信息
        public class SequenceTypeDef : Protocol
        {
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌
            public byte byFirstValue; // 顺子的第一个值            
            public byte byChowValue;  // 吃来的那张牌的值

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byFirstValue = stream.readByte();
                byChowValue = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byFirstValue = stream.readByte();
                byChowValue = stream.readByte();
            }
        }

        //刻子信息
        public class TripletTypeDef : Protocol
        {
            /// <summary>
            ///  牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            /// </summary>
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            /// <summary>
            /// 牌值
            /// </summary>
            public byte byValue; // 牌值
            /// <summary>
            /// 是否是碰/杠别人的牌
            /// </summary>
            public byte bySeatNum; // 碰/杠牌的座位号，碰杠记录的是碰的座位号，暗杠记录的是自己的座位号
            /// <summary>
            ///  碰杠类型：1碰，2明杠，3暗杠,4补杠
            /// </summary>
            public byte byPongKongType; // 碰杠类型：1碰，2明杠，3暗杠

            public byte byPongKongChoose;//是否可杠时选择了碰 0不是，1是,byPongKongType为1时生效
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byValue = stream.readByte();
                bySeatNum = stream.readByte();
                byPongKongType = stream.readByte();
                byPongKongChoose = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byValue = stream.readByte();
                bySeatNum = stream.readByte();
                byPongKongType = stream.readByte();
                byPongKongChoose = stream.readByte();
            }
        }

        //十三幺吃抢信息（长治花三门）
        public class ThirteenOrphansTypeDef : Protocol
        {
            /// <summary>
            /// 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            /// </summary>
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            public byte byValue; // 牌值
            /// <summary>
            ///  类型：1吃、2抢 
            /// </summary>
            public byte byType;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byType = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byType = stream.readByte();
            }
        }
        //最后到手的牌信息
        public class LastTileDef : Protocol
        {
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌，6花牌
            public byte byValue; // 牌值
            public byte byOther; // 是别人的牌，0为自摸，1别人放炮
            public byte byKong; // 是杠来的牌
            public byte byRobbingKong; // 是抢杠来的牌
            public byte bySeaBottom; // 是和别人打的最后张牌
            public byte bySeaBottomDraw; // 是和海底最后张摸牌
            //public byte pos;  //补位
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byOther = stream.readByte();
                byKong = stream.readByte();
                byRobbingKong = stream.readByte();
                bySeaBottom = stream.readByte();
                bySeaBottomDraw = stream.readByte();
                //pos = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byOther = stream.readByte();
                byKong = stream.readByte();
                byRobbingKong = stream.readByte();
                bySeaBottom = stream.readByte();
                bySeaBottomDraw = stream.readByte();
                //pos = stream.readByte();
            }
        }

        //欠分信息
        public class OwePointDef : Protocol
        {
            public byte bySeatNum; // 欠/被欠哪个座位的分
            public sbyte cOwePoint; // 欠分数量：正数是别人欠自己的，负数是自己欠别人的
            public byte byOweType; // 欠分类型：1前抬，2明杠，3暗杠，4后和
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySeatNum = stream.readByte();
                cOwePoint = (sbyte)stream.readByte();
                byOweType = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySeatNum = stream.readByte();
                cOwePoint = (sbyte)stream.readByte();
                byOweType = stream.readByte();
            }
        }

        //将牌信息
        public class EyesTypeDef : Protocol
        {
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            public byte byValue; // 牌值
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byValue = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byValue = stream.readByte();
            }
        }

        //* 癞子牌信息
        public class LaiziTypeDef : Protocol
        {
            public byte bySuit;// 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌
            public byte byValue;// 牌值
            public byte byChangeSuit;// 癞子变化后的牌类型
            public byte byChangeValue;// 癞子变化后的牌值
            public byte byChangeType; // 变化后的类型：0手牌，1刻子，2顺子，3将牌
            public byte byChangeIndex; // 变化后的类型的下标

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byChangeSuit = stream.readByte();
                byChangeValue = stream.readByte();
                byChangeType = stream.readByte();
                byChangeIndex = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                bySuit = stream.readByte();
                byValue = stream.readByte();
                byChangeSuit = stream.readByte();
                byChangeValue = stream.readByte();
                byChangeType = stream.readByte();
                byChangeIndex = stream.readByte();
            }
        }

        //结果信息
        public class ResultTypeDef : Protocol
        {

            public sbyte cTripletToKongPoint; //补杠得的分数和（补杠用户加，其他用户减）
            public sbyte cRobbingKongPoint;//抢杠胡得的分数和（庄家加，闲家减 抢杠用户减/加）
            public byte bySequenceNum; // 顺子的数量
            public byte byTripletNum; //刻子的数量
            public byte bylaizinum;//癞子的数量 
            public byte byThirteenOrphansNum; // 十三幺吃抢的数量    
            public int cExposedKongPoint; // 明杠得的番数和（明杠用户加，被杠/其他用户减）
            public int cConcealedKongPoint; // 暗杠得的番数和（暗杠用户加，其他用户减）  
            public byte[] byFanTiles = new byte[8]; // 翻码牌             

            public EyesTypeDef eyesType = new EyesTypeDef(); // 将牌
            public SequenceTypeDef[] sequenceType = new SequenceTypeDef[4]; // 顺子牌
            public TripletTypeDef[] tripletType = new TripletTypeDef[4]; // 刻子牌
            public ThirteenOrphansTypeDef[] thirteenOrphansType = new ThirteenOrphansTypeDef[14]; // 十三幺吃抢信息（长治花三门）
            public LaiziTypeDef[] LaiziType = new LaiziTypeDef[4]; // 癞子牌
            public LastTileDef lastTile = new LastTileDef(); // 最后到手的牌

            public byte pos_0;  //补位
                                // public byte pos_1;  //补位

            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public ResultTypeDef()
            {
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i] = new SequenceTypeDef();
                    tripletType[i] = new TripletTypeDef();
                    LaiziType[i] = new LaiziTypeDef();
                }
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i] = new ThirteenOrphansTypeDef();
                }
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                cTripletToKongPoint = (sbyte)stream.readByte();
                cRobbingKongPoint = (sbyte)stream.readByte();

                bySequenceNum = stream.readByte();
                byTripletNum = stream.readByte();
                Debug.LogError("壳子数量" + byTripletNum + "位置" + stream.currentPos());

                bylaizinum = stream.readByte();
                Debug.LogError("癞子数量" + byTripletNum);
                byThirteenOrphansNum = stream.readByte();
                cExposedKongPoint = stream.readInt();
                cConcealedKongPoint = stream.readInt();
                for (int i = 0; i < byFanTiles.Length; i++)
                {
                    byFanTiles[i] = stream.readByte();
                }
                eyesType.parseBytes(stream);
                //11+5+2↑
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i].parseBytes(stream);
                }
                //11+5+2+12↑
                for (int i = 0; i < 4; i++)
                {
                    tripletType[i].parseBytes(stream);
                }
                //11+5+2+12+16↑
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i].parseBytes(stream);
                }
                for (int i = 0; i < 4; i++)
                {
                    LaiziType[i].parseBytes(stream);
                }
                //11+5+2+12+16+42↑
                lastTile.parseBytes(stream);
                pos_0 = stream.readByte();
                // pos_1 = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {

                cTripletToKongPoint = (sbyte)stream.readByte();
                cRobbingKongPoint = (sbyte)stream.readByte();

                bySequenceNum = stream.readByte();
                byTripletNum = stream.readByte();
                //  Debug.LogError("壳子数量" + byTripletNum + "位置" + stream.currentPos());
                bylaizinum = stream.readByte();
                //  Debug.LogError("癞子数量" + byTripletNum);
                byThirteenOrphansNum = stream.readByte();
                cExposedKongPoint = stream.readInt();
                cConcealedKongPoint = stream.readInt();
                for (int i = 0; i < byFanTiles.Length; i++)
                {
                    byFanTiles[i] = stream.readByte();
                }
                eyesType.parseBytes(stream);
                //11+5+2↑
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i].parseBytes(stream);
                }
                //11+5+2+12↑
                for (int i = 0; i < 4; i++)
                {
                    tripletType[i].parseBytes(stream);
                }
                //11+5+2+12+16↑
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i].parseBytes(stream);
                }
                for (int i = 0; i < 4; i++)
                {
                    LaiziType[i].parseBytes(stream);
                }
                //11+5+2+12+16+42↑
                lastTile.parseBytes(stream);
                pos_0 = stream.readByte();
                // pos_1 = stream.readByte();
            }
        }

        //房间结果信息
        public class RoomResultTypeDef : Protocol
        {
            public ushort wSelfDrawn; // 自摸次数
            public ushort wExposedKong; // 明杠次数
            public ushort wConcealedKong; // 暗杠次数
            public ushort wShoot; // 放炮次数
            public ushort wTakeShoot; // 接炮次数            
            public short sTotalPoint; // 总分数
            public byte byBigWinnerCount;//大赢家次数
            public byte byIfGetBigWinRp;//0没有，1有
            public byte byBestGunnerCount;//最佳炮手次数
            public byte byIfGetBestGunnerRp;//0没有，1有
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                wSelfDrawn = (ushort)stream.readShort();
                wExposedKong = (ushort)stream.readShort();
                wConcealedKong = (ushort)stream.readShort();
                wShoot = (ushort)stream.readShort();
                wTakeShoot = (ushort)stream.readShort();
                sTotalPoint = stream.readShort();
                byBigWinnerCount = stream.readByte();
                byIfGetBigWinRp = stream.readByte();
                byBestGunnerCount = stream.readByte();
                byIfGetBestGunnerRp = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {
                wSelfDrawn = (ushort)stream.readShort();
                wExposedKong = (ushort)stream.readShort();
                wConcealedKong = (ushort)stream.readShort();
                wShoot = (ushort)stream.readShort();
                wTakeShoot = (ushort)stream.readShort();
                sTotalPoint = stream.readShort();
                byBigWinnerCount = stream.readByte();
                byIfGetBigWinRp = stream.readByte();
                byBestGunnerCount = stream.readByte();
                byIfGetBestGunnerRp = stream.readByte();
            }

        }
        public static int MAX_USER_PER_TABLE = 4;
        public static int F_TOTAL_NUM = 80; //番种类总数

        ////重入游戏数据，跟在ClientAgainLoginResDef的最后（在它前面还有桌上所有用户信息UserInfoDef）
        public class AgainLoginGameData : Protocol
        {
            public int ttDismiss;//申请解散的时间，大于0需要同意或拒绝（如果别人申请解散，自己同意后掉线，重入后这是0）
            public int iParlorId; // 房间所属馆id
            public int iOpenRoomId; //开房人id
            public byte byDismissSeat; // 申请解散的座位号
            public byte[] byaDismiss = new byte[4]; // 桌上用户同意解散状况
            public byte byRounds; // 当前第几圈
            public byte byInnings; // 当前第几局
            public byte byDealerSeat; // 庄家座位号
            public byte byDrawSeat; // 摸牌用户座位号
            public byte byDiscardSeat; // 出牌用户座位号
            public byte byTableTile; // 当前出的牌，桌上的牌
            public byte byTableTileSeat; // 当前出桌上牌用户的座位号
            public byte byThirteenOrphans;//自己是否做牌型(长治花三门的十三幺)            
            public short[] saTotalPoint = new short[4]; // 分数
            /// <summary>
            /// 每个用户手上牌的数量
            /// </summary>
            public byte[] byaHandTileNum = new byte[4]; // 每个用户手上牌的数量
            public byte[,] bya2DiscardTiles = new byte[4, 24];//[MAX_USER_PER_TABLE][24]; // 每个用户打出去的牌
            public byte[] byaDiscardTileNum = new byte[4];//[MAX_USER_PER_TABLE]; // 每个用户打出去的牌的数量
            public byte[] byaHandTiles = new byte[14]; // 自己手上拿的牌（吃碰杠的牌都在结果resultType当中）
            public byte byRemainTiles; // 剩余可摸的牌数量
            public byte byDealStatus; //桌状态 0 人没齐 1准备阶段 2 开始游戏到打牌之前 3打牌以后到游戏结束
            public byte byLaiziTIle; // 癞子牌值21168
            public byte byOverWater; // 过胡
            public byte[] byReadHandStauts = new byte[4];  //报听状态
            public byte[] bybyAutoPlayState = new byte[4];//托管状态，0为否，1为托管
            public byte[] byFlowerTileNum = new byte[4]; //花牌张数

            public ResultTypeDef[] resultType = new ResultTypeDef[MAX_USER_PER_TABLE];//[MAX_USER_PER_TABLE]; // 结果类型（别人的暗杠需要隐藏牌的花色和值）  

            // public byte pos_1; // 
            //public byte pos_2; // 
            //public byte pos_3; // 
            public AgainLoginGameData()
            {
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    resultType[i] = new ResultTypeDef();
                }
            }
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                switch (anhui.MahjongCommonMethod.Instance.SeverPort / 1000)
                {
                    case 20: parseBytes_20168(stream); break;
                    case 21: parseBytes_21168(stream); break;
                    default: parseBytes_20168(stream); break;
                }
                return stream.currentPos();
            }
            public void parseBytes_20168(FieldStream stream)
            {
                ttDismiss = stream.readInt();
                iParlorId = stream.readInt();
                iOpenRoomId = stream.readInt();
                byDismissSeat = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    byaDismiss[i] = stream.readByte();
                }
                //17↑
                byRounds = stream.readByte();
                byInnings = stream.readByte();
                byDealerSeat = stream.readByte();
                byDrawSeat = stream.readByte();
                byDiscardSeat = stream.readByte();
                byTableTile = stream.readByte();
                byTableTileSeat = stream.readByte();
                byThirteenOrphans = stream.readByte();
                //17+10↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    saTotalPoint[i] = stream.readShort();
                }
                //17+10+8↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    byaHandTileNum[i] = stream.readByte();
                }
                //17+10+8+4↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bya2DiscardTiles[i, j] = stream.readByte();
                    }
                }
                //17+10+8+4+96↑
                for (int i = 0; i < 4; i++)
                {
                    byaDiscardTileNum[i] = stream.readByte();
                }
                //17+10+8+4+96+4↑
                for (int i = 0; i < 14; i++)
                {
                    byaHandTiles[i] = stream.readByte();
                }
                //17+10+8+4+96+4+14↑
                byRemainTiles = stream.readByte();
                byDealStatus = stream.readByte();
                byLaiziTIle = stream.readByte();
                byOverWater = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    byReadHandStauts[i] = stream.readByte();
                }

                //17+10+8+4+96+4+14+6+380↑
                for (int i = 0; i < 4; i++)
                {
                    bybyAutoPlayState[i] = stream.readByte();
                }
                for (int i = 0; i < 4; i++)
                {
                    byFlowerTileNum[i] = stream.readByte();
                }
                //17+10+8+4+96+4+14+6↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    resultType[i].parseBytes(stream);
                }
                //17+10+8+4+96+4+14+6+380+4↑ == 211   543

                //pos_1 = stream.readByte();
                //pos_2 = stream.readByte();
            }

            public void parseBytes_21168(FieldStream stream)
            {
                ttDismiss = stream.readInt();
                iParlorId = stream.readInt();
                iOpenRoomId = stream.readInt();
                byDismissSeat = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    byaDismiss[i] = stream.readByte();
                }
                //17↑
                byRounds = stream.readByte();
                byInnings = stream.readByte();
                byDealerSeat = stream.readByte();
                byDrawSeat = stream.readByte();
                byDiscardSeat = stream.readByte();
                byTableTile = stream.readByte();
                byTableTileSeat = stream.readByte();
                byThirteenOrphans = stream.readByte();
                //17+8↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    saTotalPoint[i] = stream.readShort();
                }
                //17+8+8↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    byaHandTileNum[i] = stream.readByte();
                }
                //17+8+8+4↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bya2DiscardTiles[i, j] = stream.readByte();
                    }
                }
                //17+8+8+4+96↑
                for (int i = 0; i < 4; i++)
                {
                    byaDiscardTileNum[i] = stream.readByte();
                }
                //17+8+8+4+96+4↑
                for (int i = 0; i < 14; i++)
                {
                    byaHandTiles[i] = stream.readByte();
                }
                //17+8+8+4+96+4+14↑
                byRemainTiles = stream.readByte();
                byDealStatus = stream.readByte();
                byLaiziTIle = stream.readByte();
                byOverWater = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    byReadHandStauts[i] = stream.readByte();
                }

                //17+8+8+4+96+4+14+6+472↑
                for (int i = 0; i < 4; i++)
                {
                    bybyAutoPlayState[i] = stream.readByte();
                }
                for (int i = 0; i < 4; i++)
                {
                    byFlowerTileNum[i] = stream.readByte();
                }

                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    resultType[i].parseBytes(stream);
                }
                //17+8+8+4+96+4+14+6+472+4+1↑==634
                // pos_1 = stream.readByte();
                //pos_2 = stream.readByte();
            }
        }

        //CLIENT_AGAIN_LOGIN_RES  0x1000	// [游服]->[游客]断线重入回应消息
        public class ClientAgainLoginRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public short sTableNum; // 桌号            
            public sbyte cSeatNum; // 座位号            
            public sbyte cUserNum; // 同桌用户数，结构体后跟iUserNum个用户信息UserInfoDef
            public int iGameDataLen; // 游戏数据长度，用户信息后面再跟的游戏数据长度，具体游戏各自解释

            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                sTableNum = stream.readShort();
                cSeatNum = (sbyte)stream.readByte();
                cUserNum = (sbyte)stream.readByte();
                iGameDataLen = stream.readInt();

                return stream.currentPos();
            }
        }

        //CLIENT_OTHER_AGAIN_LOGIN_NOTICE			0x1001	// [游服]->[游客]他人断线重入通知消息
        public class ClientOtherAgainLoginNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                return stream.currentPos();
            }
        }

        // CLIENT_USER_JOIN_TABLE					0x1002	// [游服]->[游客]用户入座消息
        public class ClientUserJoinTable : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef(); // 消息结构体后跟了入座用户的用户信息结构体UserInfoDef
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);


                return stream.currentPos();
            }
        }

        //CLIENT_USER_LEAVE_TABLE					0x1003	// [游服]->[游客]用户离座消息
        public class ClientUserLeaveTable : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 离桌的用户编号            
            public sbyte cSeatNum;//离座的座位号
            public sbyte cLeaveType; // 离桌的原因：
            public sbyte pos;
            public sbyte pos1;
            // 1 退出程序，客户端发起，收到后关闭客户端
            // 2 等待状态下离开桌子，为了换桌，客户端发起，收到后换桌
            // 3 游戏状态下掉线
            // 4 游戏状态下离开，客户端发起，收到后关闭客户端
            // 5 用户预约房占座   //目前服务器没用，自己写在这留着的

            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iUserId = stream.readInt();
                cSeatNum = (sbyte)stream.readByte();
                cLeaveType = (sbyte)stream.readByte();
                pos = (sbyte)stream.readByte();
                pos1 = (sbyte)stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_SIT_REQ							0x1006	// [游客]->[游服]坐下请求消息
        public class ClientSitReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iRoomNum; // 房间号
            public int iParlorId;  //馆的id只有馆内开房才会赋值
            public ushort sTableNum; // 桌号，0为不指定桌号            
            public sbyte cSeatNum; // 座位号，0为不指定座位号  
            public sbyte pos;  //补位          
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iRoomNum);
                stream.writeInt(iParlorId);
                stream.writeShort((short)sTableNum);
                stream.writeByte((byte)cSeatNum);
                stream.writeByte((byte)pos);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_SIT_RES							0x1007	// [游服]->[游客]坐下回应消息
        public class ClientSitRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iOpenRoomUserId; // 开房用户编号
            public short sTableNum; // 桌号，从1开始            
            public byte cSeatNum; // 座位号，从1开始            
            public byte[] SeatId = new byte[4];// 0没人 1有人 2有人并且占座 3有人占座不在房间内
            public uint CreatRoomTime; // 创建房间的时间
            public byte byOpenRoomMode;//开房模式：1普通 2馆主 3馆内成员
            public byte cUserNum; // 同桌其他用户数量，后面跟iUserNum个用户信息结构体UserInfoDef
            public byte pos_0; // 
            public byte pos_1; // 


            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iOpenRoomUserId = stream.readInt();
                sTableNum = stream.readShort();
                cSeatNum = stream.readByte();
                for (int i = 0; i < 4; i++)
                {
                    SeatId[i] = stream.readByte();
                }
                CreatRoomTime = (uint)stream.readInt();
                byOpenRoomMode = stream.readByte();
                cUserNum = stream.readByte();
                // Debug.LogWarning("--###########" + iUserId + "  " + byOpenRoomMode + "  " + cUserNum + ","+CreatRoomTime);
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_ESCAPE_REQ	 0x1008	// [游客]->[游服]离开请求消息
        public class ClientEscapeReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号            
            public byte byEscape; // 离开状态：0回来，1离开
            public byte pos_0;  //补位
            public byte pos_1;
            public byte pos_2;

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byEscape);
                stream.writeByte(pos_0);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_ESCAPE_RES		0x1009	// [游服]->[游客]离开回应消息
        public class ClientEscapeRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号            
            public byte byEscape; // 离开状态：0回来，1离开
            public byte pos_0;  //补位
            public byte pos_1;
            public byte pos_2;
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                byEscape = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_READY_REQ		0x100A	// [游客]->[游服]准备请求消息
        public class ClientReadyReq : Protocol
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
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_READY_RES		0x100B	// [游服]->[游客]准备回应消息
        public class ClientReadyRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();

                return stream.currentPos();
            }
        }


        //CLIENT_DISMISS_ROOM_REQ		0x1010	// [游客]->[游服]解散房间请求消息
        public class ClientDismissRoomReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号            
            public byte cType;  //类型：1发起，2同意，3拒绝
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(cType);
                stream.writeByte(0);
                stream.writeByte(0);
                stream.writeByte(0);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_DISMISS_ROOM_RES	0x1011	// [游服]->[游客]解散房间回应消息
        public class ClientDismissRoomRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号           
            public byte cType;  // 类型：1发起，2同意，3拒绝
            public byte byFlag; //0表示正常发起解散 1表示取消托管发起解散
            public byte pos_1; //补位
            public byte pos_2; //补位
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                cType = stream.readByte();
                byFlag = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_DISMISS_ROOM_NOTICE	0x1012	// [游服]->[游客]解散房间通知消息
        public class ClientDismissRoomNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;  //错误编号
            public short sTableNum; // 桌号                  

            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                sTableNum = stream.readShort();
                return stream.currentPos();
            }
        }

        //CLIENT_VOICE_REQ		0x1020	// [游客]->[游服]语音请求消息
        public class ClientVoiceReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iDuration; // 持续时间
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
            public string szUrl; // 下载语音的URL
            public byte[] btId;
            public byte[] bt = new byte[3];
            public ClientVoiceReq()
            {

            }
            public ClientVoiceReq(byte[] btID)
            {
                btId = btID;
            }
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();

                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeInt(iDuration);
                stream.writeFixedLenString(szUrl, 60);
                Debug.Log(btId.Length);
                stream.writeBuff(btId, btId.Length);
                stream.writeBuff(bt, 3);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_VOICE_RES		0x1021	// [游服]->[游客]语音回应消息
        public class ClientVoiceRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iDuration; // 持续时间
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
            public string szUrl; // 下载语音的URL
            public byte[] id;
            public byte[] bt = new byte[3];
            public ClientVoiceRes()
            {

            }
            public ClientVoiceRes(int a)
            {
                id = new byte[a];
            }
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                iError = stream.readInt();
                iUserId = stream.readInt();
                iDuration = stream.readInt();
                szUrl = stream.readFixedLenString(60);

                stream.readBuff(ref id, id.Length);
                stream.readBuff(ref bt, bt.Length);
                return stream.currentPos();
            }
        }

        //CLIENT_DEAL_TILE_NOTICE   0x1100	// [游服]->[游客]发牌通知消息
        public class ClientDealTileNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szGameNum; // 一局游戏的编号
            public byte byDealerSeat; // 庄家的座位号
            public byte bySeatNum; // 牌的座位号            
            public byte[] caCard = new byte[13]; // 发给这个用户的牌
            public byte pos;

            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                msgHeadInfo.parseBytes(stream);
                szGameNum = stream.readFixedLenString(16);
                byDealerSeat = stream.readByte();
                bySeatNum = stream.readByte();

                for (int i = 0; i < 13; i++)
                {
                    caCard[i] = stream.readByte();
                }
                pos = stream.readByte();
                return stream.currentPos();
            }
        }
        //CLIENT_DISCARD_TILE_NOTICE				0x1101	// [游服]->[游客]出牌通知消息
        public class ClientDiscardTileNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte bySeatNum; // 座位号
            public byte byDrawTile; // 摸的牌，如果时吃碰后没摸牌为0；如果是别人为0xFF
            public byte byDrawFromEnd; // 是否从牌墙最后摸的牌
            public byte byDrawSeaBottom; // 是否是摸海底
            public byte byResidueTileNum;// 剩余的牌数量
            public byte bys;
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
                bySeatNum = stream.readByte();
                byDrawTile = stream.readByte();
                byDrawFromEnd = stream.readByte();
                byDrawSeaBottom = stream.readByte();
                byResidueTileNum = stream.readByte();
                //   for (int i = 0; i < bytes.Length ; i++)
                //  {
                bys = stream.readByte();
                //   }
                return stream.currentPos();
            }
        }
        //CLIENT_DISCARD_TILE_REQ	0x1102	// [游客]->[游服]出牌请求消息
        public class ClientDiscardTileReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号                   
            public byte byDrawSeaBottom; // 出的牌，当牌为0时是胡牌（自摸）

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byDrawSeaBottom);
                stream.writeByte(0);
                stream.writeByte(0);
                stream.writeByte(0);
                return stream.toBytes();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new System.NotImplementedException();
            }
        }

        //CLIENT_DISCARD_TILE_RES		0x1103	// [游服]->[游客]出牌回应消息
        public class ClientDiscardTileRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                      
            public byte cTitle; // 出的牌，0表示胡牌
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;
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
                cTitle = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_SPECIAL_TILE_NOTICE	0x1104	// [游服]->[游客]吃碰杠胡抢通知消息
        public class ClientSpecialTileNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte cSeatNum; // 座位号            
            public byte byChow; // 是否能吃
            public byte byPong; // 是否能碰
            public byte byKong; // 是否能杠
            public byte byWin; // 是否能胡
            public byte byPassWin; //是否可以过胡
            public byte byThirteenOrphansChow; // 是否能吃(长治花三门的十三幺)
            public byte byThirteenOrphansRob; // 是否能抢(长治花三门的十三幺)            
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                cSeatNum = stream.readByte();
                byChow = stream.readByte();
                byPong = stream.readByte();
                byKong = stream.readByte();
                byWin = stream.readByte();
                byPassWin = stream.readByte();
                byThirteenOrphansChow = stream.readByte();
                byThirteenOrphansRob = stream.readByte();
                return stream.currentPos();
            }
        }
        // CLIENT_SPECIAL_TILE_REQ					0x1105	// [游客]->[游服]吃碰杠胡抢请求消息
        public class ClientSpecialTileReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte[] byaTiles = new byte[2]; // 请求的牌：碰杠的牌放索引0位置
            public byte bySpecial; // 0弃1吃2碰3杠4胡5吃(长治花三门的十三幺)6抢(长治花三门的十三幺)
            public byte byPongKong; // 是否是碰杠

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                for (int i = 0; i < 2; i++)
                {
                    stream.writeByte(byaTiles[i]);
                }
                stream.writeByte(bySpecial);
                stream.writeByte(byPongKong);
                return stream.toBytes();

            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        // CLIENT_SPECIAL_TILE_RES					0x1106	// [游服]->[游客]吃碰杠胡抢回应消息
        public class ClientSpecialTileRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号，0正确
            public int iUserId; // 用户编号
            public byte[] byaTiles = new byte[3]; // 请求的牌
            /// <summary>
            /// 0弃1吃2碰3杠4胡5吃(长治花三门的十三幺)6抢(长治花三门的十三幺)
            /// </summary>
            public byte bySpecial; // 0弃1吃2碰3杠4胡5吃(长治花三门的十三幺)6抢(长治花三门的十三幺)
            public byte byPongKong; // 是否是碰杠
            public byte byTileSeat; // 出牌的座位号，自摸为0，杠自己手上的牌为0
            public byte pos_0;
            public byte pos_1;
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
                for (int i = 0; i < 3; i++)
                {
                    byaTiles[i] = stream.readByte();
                }
                bySpecial = stream.readByte();
                byPongKong = stream.readByte();
                byTileSeat = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                return stream.currentPos();
            }
        }
        //CLIENT_REALTIME_POINT_NOTICE			0x1107	// [游服]->[游客]即时计分通知消息
        public class ClientRealtimePointNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte byType; // 类型：1前抬，2明杠，3暗杠，4后和
            public byte[] caPoints = new byte[MAX_USER_PER_TABLE]; // 每个人的分数
            public byte pos;  //补位
            public override byte[] toBytes()
            {
                throw new System.NotImplementedException();
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null)
                    return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                msgHeadInfo.parseBytes(stream);
                byType = stream.readByte();
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    caPoints[i] = stream.readByte();
                }
                pos = stream.readByte();
                return stream.currentPos();
            }
        }
        //CLIENT_THIRTEEN_ORPHANS_REQ				0x1108	// [游客]->[游服]长治花三门的十三幺请求消息
        public class ClientThirteenOrphansReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 座位号
            /// <summary>
            /// 0不做1做
            /// </summary>
            public byte byThirteenOrphans;
            public byte pos1;
            public byte pos2;
            public byte pos3;

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byThirteenOrphans);
                stream.writeByte(pos1);
                stream.writeByte(pos2);
                stream.writeByte(pos3);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();

            }
        }
        //CLIENT_THIRTEEN_ORPHANS_RES				0x1109	// [游服]->[游客]长治花三门的十三幺回应消息
        public class ClientThirteenOrphansRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号，0正确
            public int iUserId; // 用户编号           
            public byte byThirteenOrphans; // 0不做1做
            public byte pos1;
            public byte pos2;
            public byte pos3;
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
                byThirteenOrphans = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                pos3 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_GAME_RESULT_NOTICE	0x1200	// [游服]->[游客]一局游戏结果通知消息
        public class ClientGameResultNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte[] byaWinSeat = new byte[MAX_USER_PER_TABLE]; // 所有用户胡牌标志
            public byte byShootSeat; // 放炮用户座位号
            public byte byShootSeatReadHand;//放炮用户是否报听  0没操作 1没报听  2报听点炮            
            public byte byDismiss; // 是否解散
            public int[] caResultPoint = new int[MAX_USER_PER_TABLE]; // 所有用户得分
            public byte[,] bya2HandTiles = new byte[MAX_USER_PER_TABLE, 14]; // 所有用户手上的牌            
            public sbyte[,] caFanResult = new sbyte[MAX_USER_PER_TABLE, F_TOTAL_NUM]; // 胡牌的人番数结果
            public ResultTypeDef[] aResultType = new ResultTypeDef[MAX_USER_PER_TABLE]; // 所有用户结果   

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
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    byaWinSeat[i] = stream.readByte();
                }
                //12↑
                byShootSeat = stream.readByte();
                byShootSeatReadHand = stream.readByte();
                //byIfNormalOrDragon = stream.readByte();
                byDismiss = stream.readByte();
                //12+3↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    caResultPoint[i] = stream.readInt();
                }
                //12+3+16↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        bya2HandTiles[i, j] = stream.readByte();
                    }
                }
                //12+3+16+56↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < F_TOTAL_NUM; j++)
                    {
                        caFanResult[i, j] = (sbyte)stream.readByte();
                    }
                }
                //12+3+16+56+72↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    aResultType[i] = new ResultTypeDef();
                    aResultType[i].parseBytes(stream);
                }

                return stream.currentPos();
            }
        }

        //CLIENT_ROOM_RESULT_NOTICE	 0x1201	// [游服]->[游客]房间游戏结果通知消息
        public class ClientRoomResultNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iRoomEndType;  //1为全部游戏结束解散解散，0不是
            public RoomResultTypeDef[] aRoomResultType = new RoomResultTypeDef[MAX_USER_PER_TABLE]; // 所有用户房间游戏结果
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
                iRoomEndType = stream.readInt();
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    aRoomResultType[i] = new RoomResultTypeDef();
                    aRoomResultType[i].parseBytes(stream);
                }
                return stream.currentPos();
            }
        }

        //CLIENT_PLAYING_METHOD_CONF_REQ     0x1202	// [游客]->[游服]玩法配置请求消息
        public class ClientPlayingMethodConfReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 玩家ID       

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
        //CLIENT_CHAT_REQ				0x1022 // [游客]->[游服]聊天请求消息

        public class ClientChatReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int[] iUserId = new int[2]; // 位子[0]代表发起聊天用户编号，位子[1]代表接收道具用户编号（聊天类型为3才有效，其他类型都为0）
            public byte byChatType; //聊天类型，1表情符，2文字3 道具
            public byte byContentId;//聊天内容编号
            public byte b1;//聊天内容编号
            public byte b2;//聊天内容编号
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId[0]);
                stream.writeInt(iUserId[1]);
                stream.writeByte(byChatType);
                stream.writeByte(byContentId);
                stream.writeByte(b1);
                stream.writeByte(b2);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }
        //麻将玩法配置结构体 20168
        public class PlayingMethodConfDef : Protocol
        {
            // 付费 [billing]
            public byte byBillingMode; // 付费模式：1按圈付费，2按局付费，3按分付费
            public byte byBillingNumber; // 付费量：多少圈、局、分付费
            public byte byBillingPrice; // 付费价格：消耗多少张金币
            public byte byBillingBespeak; //预约订金
            public byte byBillingAwardRate; //预约守约奖励
            public byte byBillingPayMode; // 房费支付模式:0房主支付,1AA支付
                                          // 胡牌结算 [win_point]
            public byte byWinPointMode; // 计分模式：
                                        // 1 底分*庄闲倍数 + 奖码*翻码（红中赖子）
                                        //  2 底分*自摸倍数*庄闲倍数 + 底分*牌型分数（其他玩法）
                                        //  3连庄次数*2+牌型分数
                                        /// <summary>
                                        /// // 计番模板:0原版,1新版(区别:底分,自摸倍数,杠开倍数,杠分)
                                        /// </summary>
            public byte byWinPointBasePointMode;
            public byte byWinPointBasePoint; // 底分
            public byte byWinPointFanNum; // 扎0 1 2 4 6码
            public byte byWinPointFanPoint; // 翻码分数
            public byte byWinPointDealerMultiple; // 庄家倍数
            public byte byWinPointPlayerMultiple;//闲家倍数
            public byte byWinPointSelfDrawnMultiple; // 自摸倍数
            public byte byWinPointMultiShoot; // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
            public byte byWinPointMultiPay; // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
            public byte byWinPointMultiPayExtraMode; // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
            public byte byWinPointMultiPayExtra; // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
            public byte byWinPointRedDargonPoint; // 红中算码：0（不算），1（算）
            public byte byWinPointRemainDelear; //连庄时额外获得/付出分数
                                                //创牌模式 [create_mode]
            public byte byCreateModeHaveWind; // 是否有风牌：0没有，1有
            public byte byCreateModeHaveDragon; // 是否有箭牌：0没有，1有，2只有红中
            public byte byCreateModeHaveFlower; // 是否有花牌：0没有，1有
                                                // 出牌规则 [discard]
            public byte byDiscardChow; // 是否允许吃：0不允许，1允许
            public byte byDiscardLaizi; // 是否有癞子：0没有癞子，1原配，2升配，3降配,4搬配子
            public byte byDiscardLaiziTile; // 是否固定癞子牌:0随机,1红中,2白板(有癞子才算)
            public byte byWaitAutoDisCardTim; //等待进入托管状态时间,sec
            public byte byDiscardConcealedKongVisidle; // 暗杠是否可见
                                                       /**
                                                        * 留墩	[reserve]
                                                        * 留墩模式：0不留墩，1留牌数量=6 ，2掷骰子得到留墩数(1~6)墩 1~12张
                                                        **/
            public byte byReserveMode;
            public byte byReserveNum; // 留墩数量（掷骰子获得（1～ 6））（模式2有效）
            public byte byUseMoveTlie;//是否有搬子牌，0没有，1有
                                      // 荒庄 [draw]
            public byte byDrawFourKong; // 四杠荒庄：0不可以，1可以
            public byte byDrawFourNoKong;// 荒庄荒杠:0 荒庄不荒杠 ,1 荒庄荒杠
                                         // 换庄 [dealer]
            public byte byDealerDraw; // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
            public byte byDealerDealer; // 庄家胡牌后换庄模式：1轮庄，2连庄
            public byte byDealerPlayer; // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
                                        // 胡牌限制 [win_limit]
            public byte byWinLimitLack; // 缺一门：0无要求，1必须缺一门
            public byte byWinLimitDependEight; // 靠八张：0无要求，1必须靠八张
            public byte byWinLimitSelfDrawn; // 自摸：0可接炮，1必须自摸
            public byte byWinLimitRobbingKong; // 抢杠胡：0不可抢杠胡，1可抢杠胡,2有癞子不可抢杠胡
            public byte byWinLimitBeginFan; // 起胡番数
            public byte byWinLimitMaxFan;//最大番数
            public byte byWinLimitReadHand; //是否需要报听0不许要，1需要

            public byte byWinAndGongPriority; //是否优先下家，0不优先，1优先

            public byte byWinLimitEscapeWinPong; //是否漏碰漏胡,0，不漏碰不漏胡，1漏胡漏碰,2漏胡不漏碰，3漏碰不漏胡
            public byte byWinWhiteBoardChange;//白板配子，0不是，1是 2白皮配子
            public byte byWinFirstTileLimit;//是否计算天胡地胡,0不是，1计算天胡，2计算地胡，3计算天胡与地胡

            public byte byWinOverWater;//过水不胡: 0不开启 1开启
                                       // 特殊牌型 [win_special]

            public byte byWinHonorFlower; //字牌是否为花: 0 不是 ,1 是 
            public byte byWinSpecialFourLaiziWin; // 四癞子直接胡牌:0不胡,1胡
            public byte byWinSpecialThirteenIndepend; // 十三不靠：0不可胡，1可胡
            public byte byWinSpecialThirteenOrphans; // 十三幺：0不可胡，1可胡
                                                     /** 十三幺吃抢： 0 不可吃抢，
                                                      *  1 可吃抢（当十三幺为1可胡时才有效，只能吃抢各一次）
                                                      *  2 可吃抢(当十三幺为1可胡时才有效，能吃n次，只能抢一次)
                                                      *  3 可吃不可抢(当十三幺为1可胡时才有效，能吃n次)
                                                      *  4 可枪不可吃(只能抢2次)
                                                      **/
            public byte byWinSpecialThirteenOrphansCr;
            public byte byWinSpecialThirteenOrphansGrab; // 十三幺-抢后可吃：0不可吃，1可吃
            public byte byWinSpecialThirteenOrphansNum; //十三幺-吃抢牌数相关倍数：0不开启，1开启
            public byte byWinSpecialSevenPairs; // 七对：0不可胡，1可胡
            public byte byWinSpecialSevenPairsFlag; // 七对标志：0普通，1不能有同色三顺对
            public byte byWinSpecialLuxurySevenPairs; // 豪华七对：0不可胡，1可胡
            public byte byWinKong; // 坎,0不计分，1计分
            public byte byWinLack; // 缺门,0不计分，1计分
            public byte byWinOneNineJong; // 幺九将,0不计分，1计分
            public byte byWinOrphas; // 断独幺,0不计分，1计分
            public byte byWinContinuousSix; // 六连,0不计分，1计分
            public byte byWinDoubleContinuousSix;//双六连,0不计分，1计分
            public byte byWinMeet;//见面,0不计分，1计分
            public byte byWinPoint;//点数,0不计分，1计分
            public byte byWinKe;//刻，0不计分，1计分
            public byte byWinFlower;//计算花牌数0,不计算，1计算
            public byte byWinKongFlagNum; //是否计算下地坎数 0不计算，1计算
                                          // 计番 [fan]
            public byte byFanCommonHand; // 平胡
            public byte byFanThirteenIndepend; // 十三不靠
            public byte byFanThirteenOrphans; // 十三幺
            public byte byFanSevenPairs; // 七对
            public byte byFanLuxurySevenPairs; // 豪华七对
            public byte byFanAllConcealedHand; //门清
            public byte byFanSisterPave; //姐妹
            public byte byFanLackSuit; //缺一门
            public byte byFanConnectedSequence; //一条龙
            public byte byFanCouplesHand; //对对胡
            public byte byFanSameColor; //清一色
            public byte byFanSameCouples; //清对对
            public byte byFanMixedOneSuit; //混一色
            public byte byFanGangKai; // 杠上开花番数
            public byte byFanFourLaizi; // 四癞子胡牌番数
            public byte byFanCommonHandFourLaizi; // 天胡/地胡四癞子倍数
            public byte byFanTianHand; // 天胡
            public byte byFanDiHu;//地胡
            public byte byFanFourinHand; //四遇子
            public byte byFanSoftLack; //软缺
            public byte byFanForcedLack; //硬缺
            public byte byFanBrokenOrphas; //断幺
            public byte byOnlyOrphas;//独幺
            public byte byFanSistersPave; //双姊妹铺
            public byte byFanValueableWind; //值钱风
            public byte byFanRobbingKongWin; //抢杠胡
            public byte byFanSeaBottomDrawWin; //海底捞月
            public byte byFanPeninWind; //圈风
            public byte byFanWindJong; //风圈将
            public byte byFanWholeBeg; //全求人
            public byte byFanAllOneNine; //幺幺胡(全一九)
            public byte byFanBigThree; //大三元
            public byte byFanThreeWind; //三季风
            public byte byFanHonorTiles; //字一色
            public byte byFanWhiteAsJong;//配子吃
            public byte byIfUseOnlyTileWin;//是否开启独一,0不开启,1开启
            public byte byFanOnlyOneTileWin;//独一
            public byte byFlower;//花牌
            public byte byContinuousSix;//六连
            public byte byDoubleContinuousSix;//双六连
            public byte byMeet;//见面
            public byte byOneNineJong;//幺九将
            public byte byFanKe;//刻
            public byte byFanOneselfDoorWind;//本门风
            public byte byFanDealerSeatWind;//本圈风
            public byte byFanNoOneNine;//断一九
            public byte byFanEighteenFourKong;//十八罗汉/四杠
            public byte byFanFourWindKong;//大四喜
            public byte byFanWinKong; //坎 额外计番 [fan_extra]
            public byte byFanSingleHoist; //单吊（最后拿到的牌与将牌相同）
            public byte byFanReadHand; //天听 (庄家起牌听)
            public byte byFanDiReadHand; //地听
            public byte byFanRegulatingBars; //将明杠 (本门风杠 )
            public byte byFanTheDarkBars; //将暗杠 (本门风杠 )
            public byte byFanDoNotLeave; //不下架(不下地)
            public byte byFanDongFengDong; //东风东-南风南-西风西-北风北(东风圈时手上有三个东风 就叫为东风东)
            public byte byFanFireForkOne;  //火叉1
            public byte byFanFireForkTwo;  //火叉2
            public byte byFanEdgeTension;  //边张
            public byte byFanCarDan;           //卡当
            public byte byFanAbsoluteSheet;    //绝张
            public byte byFanDarker;           //暗绝
            public byte byFanDarkCard;     //暗卡绝
            public byte byFanHuRight;      //胡对对
            public byte byFanNoFlowers;        //无花
            public byte byFanStartToListen;    //起手报听
            public byte byFanFourHi;	//四喜 ]
            public byte byFanExtraMode;    //结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
            public byte byFanExtraExposedMode; // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
            public byte byFanExtraTripletToKongMode; // 补杠支付模式：0不支付，1被碰者支付，2三家都支付
            public byte byFanExtraExrobbingKongMode; // 抢杠支付模式：0不支付，1被抢杠者支付,2一家包三家
            public byte byFanExtraExposedBase; // 明杠基础番数
            public byte byFanExtraExposedExtra; // 明杠额外番数(被杠/碰支付)
            public byte byFanExtraExposedHonor;//明杠字牌番数
            public byte byFanExtraConcealedMode; // 暗杠支付模式：0不支付,1三家都支付 2点炮者支付，自摸三家支付
            public byte byFanExtraConcealedKong; // 暗杠
            public byte byFanExtraConcealedKongHonor;//暗杠字牌番数
            public byte byProcessTripletToKong;//可杠选择碰时再补杠时不计分是否开启，0不开启，1开启
            public byte byFanExtraTripletToKong; //  补杠
            public byte byFanExtraExrobbingKongBase; // 抢杠胡
            public byte byFanExtraThreeBanzi;//三搬子在手
            public byte byFanExtraTripletKongExtra;	//补杠额外番数
            public byte byFanExtraFourPeizi;//四配子在手
            //public byte[] pos_0 = new byte[1];
            //public byte pos_1;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                // 付费 [billing]
                byBillingMode = stream.readByte(); // 付费模式：1按圈付费，2按局付费，3按分付费
                byBillingNumber = stream.readByte(); // 付费量：多少圈、局、分付费
                byBillingPrice = stream.readByte(); // 付费价格：消耗多少张金币
                byBillingBespeak = stream.readByte(); //预约订金
                byBillingAwardRate = stream.readByte(); //预约守约奖励
                byBillingPayMode = stream.readByte(); // 房费支付模式:0房主支付,1AA支付
                                                      // 胡牌结算 [win_point]
                byWinPointMode = stream.readByte(); // 计分模式：
                                                    // 1 底分*庄闲倍数 + 奖码*翻码（红中赖子）
                                                    //  2 底分*自摸倍数*庄闲倍数 + 底分*牌型分数（其他玩法）
                                                    //  3连庄次数*2+牌型分数
                byWinPointBasePointMode = stream.readByte(); // 计番模板:0原版,1新版(区别:底分,自摸倍数,杠开倍数,杠分)
                byWinPointBasePoint = stream.readByte(); // 底分
                byWinPointFanNum = stream.readByte(); // 翻码数量
                byWinPointFanPoint = stream.readByte(); // 翻码分数
                byWinPointDealerMultiple = stream.readByte(); // 庄家倍数
                byWinPointPlayerMultiple = stream.readByte();//闲家倍数
                byWinPointSelfDrawnMultiple = stream.readByte(); // 自摸倍数
                byWinPointMultiShoot = stream.readByte(); // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
                byWinPointMultiPay = stream.readByte(); // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
                byWinPointMultiPayExtraMode = stream.readByte(); // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
                byWinPointMultiPayExtra = stream.readByte(); // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
                byWinPointRedDargonPoint = stream.readByte(); // 红中算码：0（不算），1（算）
                byWinPointRemainDelear = stream.readByte(); //连庄时额外获得/付出分数
                                                            //创牌模式 [create_mode]
                byCreateModeHaveWind = stream.readByte(); // 是否有风牌：0没有，1有
                byCreateModeHaveDragon = stream.readByte(); // 是否有箭牌：0没有，1有，2只有红中
                byCreateModeHaveFlower = stream.readByte(); // 是否有花牌：0没有，1有
                                                            // 出牌规则 [discard]
                byDiscardChow = stream.readByte(); // 是否允许吃：0不允许，1允许
                byDiscardLaizi = stream.readByte(); // 是否有癞子：0没有癞子，1原配，2升配，3降配,4搬配子
                byDiscardLaiziTile = stream.readByte(); // 是否固定癞子牌:0随机,1红中,2白板(有癞子才算)
                byWaitAutoDisCardTim = stream.readByte(); //等待进入托管状态时间,sec
                byDiscardConcealedKongVisidle = stream.readByte(); // 暗杠是否可见
                                                                   /**
                                                                    * 留墩	[reserve]
                                                                    * 留墩模式：0不留墩，1留牌数量=6 ，2掷骰子得到留墩数(1~6)墩 1~12张
                                                                    **/
                byReserveMode = stream.readByte();
                byReserveNum = stream.readByte(); // 留墩数量（掷骰子获得（1～ 6））（模式2有效）
                byUseMoveTlie = stream.readByte();//是否有搬子牌，0没有，1有
                                                  // 荒庄 [draw]
                byDrawFourKong = stream.readByte(); // 四杠荒庄：0不可以，1可以
                byDrawFourNoKong = stream.readByte();// // 荒庄荒杠:0 荒庄不荒杠 ,1 荒庄荒杠

                // 换庄 [dealer]
                byDealerDraw = stream.readByte(); // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
                byDealerDealer = stream.readByte(); // 庄家胡牌后换庄模式：1轮庄，2连庄
                byDealerPlayer = stream.readByte(); // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
                                                    // 胡牌限制 [win_limit]
                byWinLimitLack = stream.readByte(); // 缺一门：0无要求，1必须缺一门
                byWinLimitDependEight = stream.readByte(); // 靠八张：0无要求，1必须靠八张
                byWinLimitSelfDrawn = stream.readByte(); // 自摸：0可接炮，1必须自摸
                byWinLimitRobbingKong = stream.readByte(); // 抢杠胡：0不可抢杠胡，1可抢杠胡,2有癞子不可抢杠胡
                byWinLimitBeginFan = stream.readByte(); // 起胡番数
                byWinLimitMaxFan = stream.readByte();//最大番数
                byWinLimitReadHand = stream.readByte(); //是否需要报听0不许要，1需要
                byWinAndGongPriority = stream.readByte();
                byWinLimitEscapeWinPong = stream.readByte(); //是否漏碰漏胡,0，不漏碰不漏胡，1漏胡漏碰,2漏胡不漏碰，3漏碰不漏胡
                byWinWhiteBoardChange = stream.readByte();//白板配子，0不是，1是
                byWinFirstTileLimit = stream.readByte();//是否计算天胡地胡,0不是，1计算天胡，2计算地胡，3计算天胡与地胡
                byWinOverWater = stream.readByte();//过水不胡: 0不开启 1开启
                byWinHonorFlower = stream.readByte();
                // 特殊牌型 [win_special]
                byWinSpecialFourLaiziWin = stream.readByte(); // 四癞子直接胡牌:0不胡,1胡
                byWinSpecialThirteenIndepend = stream.readByte(); // 十三不靠：0不可胡，1可胡
                byWinSpecialThirteenOrphans = stream.readByte(); // 十三幺：0不可胡，1可胡
                                                                 /**  * 十三幺吃抢：  *  0 不可吃抢，
                                                                  *  1 可吃抢（当十三幺为1可胡时才有效，只能吃抢各一次）
                                                                  *  2 可吃抢(当十三幺为1可胡时才有效，能吃n次，只能抢一次)
                                                                  *  3 可吃不可抢(当十三幺为1可胡时才有效，能吃n次)
                                                                  *  4 可枪不可吃(只能抢2次)
                                                                  **/
                byWinSpecialThirteenOrphansCr = stream.readByte();
                byWinSpecialThirteenOrphansGrab = stream.readByte(); // 十三幺-抢后可吃：0不可吃，1可吃
                byWinSpecialThirteenOrphansNum = stream.readByte(); //十三幺-吃抢牌数相关倍数：0不开启，1开启
                byWinSpecialSevenPairs = stream.readByte(); // 七对：0不可胡，1可胡
                byWinSpecialSevenPairsFlag = stream.readByte(); // 七对标志：0普通，1不能有同色三顺对
                byWinSpecialLuxurySevenPairs = stream.readByte(); // 豪华七对：0不可胡，1可胡
                byWinKong = stream.readByte(); // 坎,0不计分，1计分
                byWinLack = stream.readByte(); // 缺门,0不计分，1计分
                byWinOneNineJong = stream.readByte(); // 幺九将,0不计分，1计分
                byWinOrphas = stream.readByte(); // 断独幺,0不计分，1计分
                byWinContinuousSix = stream.readByte(); // 六连,0不计分，1计分
                byWinDoubleContinuousSix = stream.readByte();//双六连,0不计分，1计分
                byWinMeet = stream.readByte();//见面,0不计分，1计分
                byWinPoint = stream.readByte();//点数,0不计分，1计分
                byWinKe = stream.readByte();//刻，0不计分，1计分
                byWinFlower = stream.readByte();//计算花牌数0,不计算，1计算
                byWinKongFlagNum = stream.readByte();
                // 计番 [fan]
                byFanCommonHand = stream.readByte(); // 平胡
                byFanThirteenIndepend = stream.readByte(); // 十三不靠
                byFanThirteenOrphans = stream.readByte(); // 十三幺
                byFanSevenPairs = stream.readByte(); // 七对
                byFanLuxurySevenPairs = stream.readByte(); // 豪华七对
                byFanAllConcealedHand = stream.readByte(); //门清
                byFanSisterPave = stream.readByte(); //姐妹
                byFanLackSuit = stream.readByte(); //缺一门
                byFanConnectedSequence = stream.readByte(); //一条龙
                byFanCouplesHand = stream.readByte(); //对对胡
                byFanSameColor = stream.readByte(); //清一色
                byFanSameCouples = stream.readByte(); //清对对
                byFanMixedOneSuit = stream.readByte(); //混一色
                byFanGangKai = stream.readByte(); // 杠上开花番数
                byFanFourLaizi = stream.readByte(); // 四癞子胡牌番数
                byFanCommonHandFourLaizi = stream.readByte(); // 天胡/地胡四癞子倍数
                byFanTianHand = stream.readByte(); // 天胡
                byFanDiHu = stream.readByte();//地胡
                byFanFourinHand = stream.readByte(); //四遇子
                byFanSoftLack = stream.readByte(); //软缺
                byFanForcedLack = stream.readByte(); //硬缺
                byFanBrokenOrphas = stream.readByte(); //断幺
                byOnlyOrphas = stream.readByte();//独幺
                byFanSistersPave = stream.readByte(); //双姊妹铺
                byFanValueableWind = stream.readByte(); //值钱风
                byFanRobbingKongWin = stream.readByte(); //抢杠胡
                byFanSeaBottomDrawWin = stream.readByte(); //海底捞月
                byFanPeninWind = stream.readByte(); //圈风
                byFanWindJong = stream.readByte(); //风圈将
                byFanWholeBeg = stream.readByte(); //全求人
                byFanAllOneNine = stream.readByte(); //幺幺胡(全一九)
                byFanBigThree = stream.readByte(); //大三元
                byFanThreeWind = stream.readByte(); //三季风
                byFanHonorTiles = stream.readByte(); //字一色
                byFanWhiteAsJong = stream.readByte();//配子吃
                byIfUseOnlyTileWin = stream.readByte();//是否开启独一,0不开启,1开启
                byFanOnlyOneTileWin = stream.readByte();//独一
                byFlower = stream.readByte();//花牌
                byContinuousSix = stream.readByte();//六连
                byDoubleContinuousSix = stream.readByte();//双六连
                byMeet = stream.readByte();//见面
                byOneNineJong = stream.readByte();//幺九将
                byFanKe = stream.readByte();//刻
                byFanOneselfDoorWind = stream.readByte();//本门风
                byFanDealerSeatWind = stream.readByte();//本圈风
                byFanNoOneNine = stream.readByte();//断一九
                byFanEighteenFourKong = stream.readByte();//十八罗汉/四杠
                byFanFourWindKong = stream.readByte();//大四喜
                byFanWinKong = stream.readByte(); // 额外计番 [fan_extra]
                byFanSingleHoist = stream.readByte(); //单吊（最后拿到的牌与将牌相同）
                byFanReadHand = stream.readByte();
                byFanDiReadHand = stream.readByte();
                byFanRegulatingBars = stream.readByte();
                byFanTheDarkBars = stream.readByte();
                byFanDoNotLeave = stream.readByte(); //不下架(不下地)
                byFanDongFengDong = stream.readByte(); //东风东-南风南-西风西-北风北(东风圈时手上有三个东风 就叫为东风东)
                byFanFireForkOne = stream.readByte();  //火叉1
                byFanFireForkTwo = stream.readByte();  //火叉2
                byFanEdgeTension = stream.readByte();  //边张
                byFanCarDan = stream.readByte();           //卡当
                byFanAbsoluteSheet = stream.readByte();    //绝张
                byFanDarker = stream.readByte();           //暗绝
                byFanDarkCard = stream.readByte();     //暗卡绝
                byFanHuRight = stream.readByte();      //胡对对
                byFanNoFlowers = stream.readByte();        //无花
                byFanStartToListen = stream.readByte();    //起手报听
                byFanFourHi = stream.readByte();	//四喜 ]
                byFanExtraMode = stream.readByte();    //结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
                byFanExtraExposedMode = stream.readByte(); // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
                byFanExtraTripletToKongMode = stream.readByte(); // 补杠支付模式：0不支付，1被碰者支付，2三家都支付
                byFanExtraExrobbingKongMode = stream.readByte(); // 抢杠支付模式：0不支付，1被抢杠者支付,2一家包三家
                byFanExtraExposedBase = stream.readByte(); // 明杠基础番数
                byFanExtraExposedExtra = stream.readByte(); // 明杠额外番数(被杠/碰支付)
                byFanExtraExposedHonor = stream.readByte();
                byFanExtraConcealedMode = stream.readByte(); // 暗杠支付模式：0不支付,1三家都支付 2点炮者支付，自摸三家支付
                byFanExtraConcealedKong = stream.readByte(); // 暗杠
                byFanExtraConcealedKongHonor = stream.readByte();
                byProcessTripletToKong = stream.readByte();//可杠选择碰时再补杠时不计分是否开启，0不开启，1开启
                byFanExtraTripletToKong = stream.readByte(); //  补杠
                byFanExtraExrobbingKongBase = stream.readByte(); // 抢杠胡
                byFanExtraThreeBanzi = stream.readByte();//三搬子在手
                byFanExtraTripletKongExtra = stream.readByte();
                byFanExtraFourPeizi = stream.readByte();//四配子在手
                                                        // for (int i = 0; i < pos_0.Length; i++)
                                                        //  {
                                                        //     pos_0[i] = stream.readByte();
                                                        //  }

                //pos_1 = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                // Debug.LogError("结构体开始"+stream.currentPos());
                // 付费 [billing]
                byBillingMode = stream.readByte(); // 付费模式：1按圈付费，2按局付费，3按分付费
                byBillingNumber = stream.readByte(); // 付费量：多少圈、局、分付费
                byBillingPrice = stream.readByte(); // 付费价格：消耗多少张金币
                byBillingBespeak = stream.readByte(); //预约订金
                byBillingAwardRate = stream.readByte(); //预约守约奖励
                byBillingPayMode = stream.readByte(); // 房费支付模式:0房主支付,1AA支付
                                                      // 胡牌结算 [win_point]
                byWinPointMode = stream.readByte(); // 计分模式：
                                                    // 1 底分*庄闲倍数 + 奖码*翻码（红中赖子）
                                                    //  2 底分*自摸倍数*庄闲倍数 + 底分*牌型分数（其他玩法）
                                                    //  3连庄次数*2+牌型分数
                byWinPointBasePointMode = stream.readByte(); // 计番模板:0原版,1新版(区别:底分,自摸倍数,杠开倍数,杠分)
                byWinPointBasePoint = stream.readByte(); // 底分
                byWinPointFanNum = stream.readByte(); // 翻码数量
                byWinPointFanPoint = stream.readByte(); // 翻码分数
                byWinPointDealerMultiple = stream.readByte(); // 庄家倍数
                byWinPointPlayerMultiple = stream.readByte();//闲家倍数
                byWinPointSelfDrawnMultiple = stream.readByte(); // 自摸倍数
                byWinPointMultiShoot = stream.readByte(); // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
                byWinPointMultiPay = stream.readByte(); // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
                byWinPointMultiPayExtraMode = stream.readByte(); // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
                byWinPointMultiPayExtra = stream.readByte(); // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
                byWinPointRedDargonPoint = stream.readByte(); // 红中算码：0（不算），1（算）
                byWinPointRemainDelear = stream.readByte(); //连庄时额外获得/付出分数
                                                            //创牌模式 [create_mode]
                byCreateModeHaveWind = stream.readByte(); // 是否有风牌：0没有，1有
                byCreateModeHaveDragon = stream.readByte(); // 是否有箭牌：0没有，1有，2只有红中
                byCreateModeHaveFlower = stream.readByte(); // 是否有花牌：0没有，1有
                                                            // 出牌规则 [discard]
                byDiscardChow = stream.readByte(); // 是否允许吃：0不允许，1允许
                byDiscardLaizi = stream.readByte(); // 是否有癞子：0没有癞子，1原配，2升配，3降配,4搬配子
                byDiscardLaiziTile = stream.readByte(); // 是否固定癞子牌:0随机,1红中,2白板(有癞子才算)
                byWaitAutoDisCardTim = stream.readByte(); //等待进入托管状态时间,sec
                byDiscardConcealedKongVisidle = stream.readByte(); // 暗杠是否可见
                                                                   /**
                                                                    * 留墩	[reserve]
                                                                    * 留墩模式：0不留墩，1留牌数量=6 ，2掷骰子得到留墩数(1~6)墩 1~12张
                                                                    **/
                byReserveMode = stream.readByte();
                byReserveNum = stream.readByte(); // 留墩数量（掷骰子获得（1～ 6））（模式2有效）
                byUseMoveTlie = stream.readByte();//是否有搬子牌，0没有，1有
                                                  // 荒庄 [draw]
                byDrawFourKong = stream.readByte();
                byDrawFourNoKong = stream.readByte();// 四杠荒庄：0不可以，1可以
                // 换庄 [dealer]
                byDealerDraw = stream.readByte(); // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
                byDealerDealer = stream.readByte(); // 庄家胡牌后换庄模式：1轮庄，2连庄
                byDealerPlayer = stream.readByte(); // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
                                                    // 胡牌限制 [win_limit]
                byWinLimitLack = stream.readByte(); // 缺一门：0无要求，1必须缺一门
                byWinLimitDependEight = stream.readByte(); // 靠八张：0无要求，1必须靠八张
                byWinLimitSelfDrawn = stream.readByte(); // 自摸：0可接炮，1必须自摸
                byWinLimitRobbingKong = stream.readByte(); // 抢杠胡：0不可抢杠胡，1可抢杠胡,2有癞子不可抢杠胡
                byWinLimitBeginFan = stream.readByte(); // 起胡番数
                byWinLimitMaxFan = stream.readByte();//最大番数
                byWinLimitReadHand = stream.readByte(); //是否需要报听0不许要，1需要
                byWinAndGongPriority = stream.readByte();
                byWinLimitEscapeWinPong = stream.readByte(); //是否漏碰漏胡,0，不漏碰不漏胡，1漏胡漏碰,2漏胡不漏碰，3漏碰不漏胡
                byWinWhiteBoardChange = stream.readByte();//白板配子，0不是，1是
                byWinFirstTileLimit = stream.readByte();//是否计算天胡地胡,0不是，1计算天胡，2计算地胡，3计算天胡与地胡
                byWinOverWater = stream.readByte();
                byWinHonorFlower = stream.readByte();
                // 特殊牌型 [win_special]
                byWinSpecialFourLaiziWin = stream.readByte(); // 四癞子直接胡牌:0不胡,1胡
                byWinSpecialThirteenIndepend = stream.readByte(); // 十三不靠：0不可胡，1可胡
                byWinSpecialThirteenOrphans = stream.readByte(); // 十三幺：0不可胡，1可胡
                                                                 /**
                                                                  * 十三幺吃抢：
                                                                  *  0 不可吃抢，
                                                                  *  1 可吃抢（当十三幺为1可胡时才有效，只能吃抢各一次）
                                                                  *  2 可吃抢(当十三幺为1可胡时才有效，能吃n次，只能抢一次)
                                                                  *  3 可吃不可抢(当十三幺为1可胡时才有效，能吃n次)
                                                                  *  4 可枪不可吃(只能抢2次)
                                                                  **/
                byWinSpecialThirteenOrphansCr = stream.readByte();
                byWinSpecialThirteenOrphansGrab = stream.readByte(); // 十三幺-抢后可吃：0不可吃，1可吃
                byWinSpecialThirteenOrphansNum = stream.readByte(); //十三幺-吃抢牌数相关倍数：0不开启，1开启
                byWinSpecialSevenPairs = stream.readByte(); // 七对：0不可胡，1可胡
                                                            // Debug.LogError("七对:" + byWinSpecialSevenPairs+"  "+stream.currentPos ());
                byWinSpecialSevenPairsFlag = stream.readByte(); // 七对标志：0普通，1不能有同色三顺对
                byWinSpecialLuxurySevenPairs = stream.readByte(); // 豪华七对：0不可胡，1可胡
                byWinKong = stream.readByte(); // 坎,0不计分，1计分
                byWinLack = stream.readByte(); // 缺门,0不计分，1计分
                byWinOneNineJong = stream.readByte(); // 幺九将,0不计分，1计分
                byWinOrphas = stream.readByte(); // 断独幺,0不计分，1计分
                byWinContinuousSix = stream.readByte(); // 六连,0不计分，1计分
                byWinDoubleContinuousSix = stream.readByte();//双六连,0不计分，1计分
                byWinMeet = stream.readByte();//见面,0不计分，1计分
                byWinPoint = stream.readByte();//点数,0不计分，1计分
                byWinKe = stream.readByte();//刻，0不计分，1计分
                byWinFlower = stream.readByte();//计算花牌数0,不计算，1计算
                byWinKongFlagNum = stream.readByte();
                // 计番 [fan]
                byFanCommonHand = stream.readByte(); // 平胡
                byFanThirteenIndepend = stream.readByte(); // 十三不靠
                byFanThirteenOrphans = stream.readByte(); // 十三幺
                byFanSevenPairs = stream.readByte(); // 七对
                byFanLuxurySevenPairs = stream.readByte(); // 豪华七对
                byFanAllConcealedHand = stream.readByte(); //门清
                byFanSisterPave = stream.readByte(); //姐妹
                byFanLackSuit = stream.readByte(); //缺一门
                byFanConnectedSequence = stream.readByte(); //一条龙
                byFanCouplesHand = stream.readByte(); //对对胡
                byFanSameColor = stream.readByte(); //清一色
                byFanSameCouples = stream.readByte(); //清对对
                byFanMixedOneSuit = stream.readByte(); //混一色
                byFanGangKai = stream.readByte(); // 杠上开花番数
                byFanFourLaizi = stream.readByte(); // 四癞子胡牌番数
                byFanCommonHandFourLaizi = stream.readByte(); // 天胡/地胡四癞子倍数
                byFanTianHand = stream.readByte(); // 天胡
                byFanDiHu = stream.readByte();//地胡
                byFanFourinHand = stream.readByte(); //四遇子
                byFanSoftLack = stream.readByte(); //软缺
                byFanForcedLack = stream.readByte(); //硬缺
                byFanBrokenOrphas = stream.readByte(); //断幺
                byOnlyOrphas = stream.readByte();//独幺
                byFanSistersPave = stream.readByte(); //双姊妹铺
                byFanValueableWind = stream.readByte(); //值钱风
                byFanRobbingKongWin = stream.readByte(); //抢杠胡
                byFanSeaBottomDrawWin = stream.readByte(); //海底捞月
                byFanPeninWind = stream.readByte(); //圈风
                byFanWindJong = stream.readByte(); //风圈将
                byFanWholeBeg = stream.readByte(); //全求人
                byFanAllOneNine = stream.readByte(); //幺幺胡(全一九)
                byFanBigThree = stream.readByte(); //大三元
                byFanThreeWind = stream.readByte(); //三季风
                byFanHonorTiles = stream.readByte(); //字一色
                byFanWhiteAsJong = stream.readByte();//配子吃
                byIfUseOnlyTileWin = stream.readByte();//是否开启独一,0不开启,1开启
                byFanOnlyOneTileWin = stream.readByte();//独一
                byFlower = stream.readByte();//花牌
                byContinuousSix = stream.readByte();//六连
                byDoubleContinuousSix = stream.readByte();//双六连
                byMeet = stream.readByte();//见面
                byOneNineJong = stream.readByte();//幺九将
                byFanKe = stream.readByte();//刻
                byFanOneselfDoorWind = stream.readByte();//本门风
                byFanDealerSeatWind = stream.readByte();//本圈风
                byFanNoOneNine = stream.readByte();//断一九
                byFanEighteenFourKong = stream.readByte();//十八罗汉/四杠
                byFanFourWindKong = stream.readByte();//大四喜
                byFanWinKong = stream.readByte();    //坎 额外计番 [fan_extra]
                byFanSingleHoist = stream.readByte(); //单吊（最后拿到的牌与将牌相同）
                byFanReadHand = stream.readByte();
                byFanDiReadHand = stream.readByte();
                byFanRegulatingBars = stream.readByte();
                byFanTheDarkBars = stream.readByte();
                byFanDoNotLeave = stream.readByte(); //不下架(不下地)
                byFanDongFengDong = stream.readByte(); //东风东-南风南-西风西-北风北(东风圈时手上有三个东风 就叫为东风东)
                byFanFireForkOne = stream.readByte();  //火叉1
                byFanFireForkTwo = stream.readByte();  //火叉2
                byFanEdgeTension = stream.readByte();  //边张
                byFanCarDan = stream.readByte();           //卡当
                byFanAbsoluteSheet = stream.readByte();    //绝张
                byFanDarker = stream.readByte();           //暗绝
                byFanDarkCard = stream.readByte();     //暗卡绝
                byFanHuRight = stream.readByte();      //胡对对
                byFanNoFlowers = stream.readByte();        //无花
                byFanStartToListen = stream.readByte();    //起手报听
                byFanFourHi = stream.readByte();	//四喜 ]
                // 额外计番 [fan_extra]
                byFanExtraMode = stream.readByte();    //结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
                byFanExtraExposedMode = stream.readByte(); // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
                byFanExtraTripletToKongMode = stream.readByte(); // 补杠支付模式：0不支付，1被碰者支付，2三家都支付
                byFanExtraExrobbingKongMode = stream.readByte(); // 抢杠支付模式：0不支付，1被抢杠者支付,2一家包三家
                byFanExtraExposedBase = stream.readByte(); // 明杠基础番数
                byFanExtraExposedExtra = stream.readByte(); // 明杠额外番数(被杠/碰支付)
                byFanExtraExposedHonor = stream.readByte();//明杠字牌番数
                byFanExtraConcealedMode = stream.readByte(); // 暗杠支付模式：0不支付,1三家都支付 2点炮者支付，自摸三家支付
                byFanExtraConcealedKong = stream.readByte(); // 暗杠
                byFanExtraConcealedKongHonor = stream.readByte();// 暗杠字牌番数
                byProcessTripletToKong = stream.readByte();//可杠选择碰时再补杠时不计分是否开启，0不开启，1开启
                byFanExtraTripletToKong = stream.readByte(); //  补杠
                byFanExtraExrobbingKongBase = stream.readByte(); // 抢杠胡
                byFanExtraThreeBanzi = stream.readByte();//三搬子在手
                byFanExtraTripletKongExtra = stream.readByte();
                byFanExtraFourPeizi = stream.readByte();//四配子在手
                                                        //     for (int i = 0; i < pos_0.Length; i++)
                                                        //   {
                                                        //        pos_0[i] = stream.readByte();
                                                        //    };
            }

        }
        public const int OpenRoomParma = 6;
        //CLIENT_PLAYING_METHOD_CONF_RES	0x1203	// [游服]->[游客]玩法配置回应消息
        public class ClientPlayingMethodConfRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iPlayingMethod; // 20001 20015 20016
            public int iRoomType;//是否是预约房 0不是 1是
            public uint[] roomMessage_ = new uint[OpenRoomParma];//玩家玩法选择配置   
            public PlayingMethodConfDef playingMethodConf = new PlayingMethodConfDef(); // 玩法配置    
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
                iPlayingMethod = stream.readInt();
                iRoomType = stream.readInt();
                for (int i = 0; i < roomMessage_.Length; i++)
                {
                    roomMessage_[i] = stream.readUint();
                }
                Debug.LogWarning("玩法配置回应消息 结构体之前长度" + stream.currentPos());
                playingMethodConf.parseBytes(stream);
                // Debug.LogWarning("结构体之后长度" + stream.currentPos());
                return stream.currentPos();
            }
        }

        //CLIENT_CHAT_RES		0x1023	// [游服]->[游客]聊天回应消息
        public class ClientChatRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int[] iUserId = new int[2]; // // 位子[0]代表发起聊天用户编号，位子[1]代表接收道具用户编号（聊天类型为3才有效，其他类型都为0）
            /// <summary>
            /// 1表情，2文字
            /// </summary>
            public byte byChatType; //聊天类型，1表情符，2文字3记录
            public byte byContentId;//聊天内容编号
            byte b1; //聊天类型，1表情符，2文字
            byte b2;//聊天内容编号


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
                iUserId[0] = stream.readInt();
                iUserId[1] = stream.readInt();
                byChatType = stream.readByte();
                byContentId = stream.readByte();
                b1 = stream.readByte();
                b2 = stream.readByte();
                return stream.currentPos();
            }
        }
        //CLIENT_CAN_DOWN_RU_NOTICE				0x111A// [游服]->[游客]长治潞城麻将的能跑能下通知消息
        public class ClientCanDownRunNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iRemainingTime; //剩余时间
            public byte byCanDownRu; //能下/能跑：1能下 2能跑（如果自己是庄为能下/则能跑）
            public byte pos_0; //补位   
            public byte pos_1; //补位   
            public byte pos_2; //补位   

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
                iRemainingTime = stream.readInt();
                byCanDownRu = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //CLIENT_CAN_DOWN_RU_REQ				0x111B// [游客]->[游服]长治潞城麻将的能跑能下请求消息
        public class ClientCanDownRunReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 玩家ID                
            public byte byCanDownRu; //是否能下/能跑：0不开启，1能下 2能跑（如果自己是庄为能下/则能跑）   
            public byte pos_0;  //补位
            public byte pos_1;  //补位
            public byte pos_2;  //补位

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byCanDownRu);
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

        //CLIENT_CAN_DOWN_RU_RES		0x111C	// [游服]->[游客]长治潞城麻将的能跑能下回应消息
        public class ClientCanDownRunRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public byte byCanDownRu; //开启能下/能跑：0不开启，1能下 2能跑（如果自己是庄为能下/则能跑）
            public byte pos_0;  //补位           
            public byte pos_1;  //补位
            public byte pos_2;  //补位

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
                byCanDownRu = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();

                return stream.currentPos();
            }
        }


        //CLIENT_READHAND_REQ   	0x1207	//[游客]->[游服]报听请求消息
        public class ClientReadHandReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 玩家ID                           

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

        //CLIENT_READHAND_RES   					0x1208	//[游服]->[游客]报听回应消息
        public class ClientReadHandRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号         
            public int iError; // 错误代码

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
                iError = stream.readInt();

                return stream.currentPos();
            }
        }
        /// <summary>
        /// 0x1209	// [游服]->[游客]癞子牌通知消息
        /// </summary>
        public class ClientLaiZiNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte byLaiziSuit; //  癞子的花色
            public byte byLaiziValue; // 癞子的牌值

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
                byLaiziSuit = stream.readByte();
                byLaiziValue = stream.readByte();

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
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;

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
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
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

        //#define CLIENT_BESPEAK_USERINFO_NOTICE						0X1123	//[游服]->[游客] 发送桌上游戏外占座玩家信息
        public class ClientBespeakUserInfoDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public int iUserId;//查询玩家ID
            public byte byUserNum;//用户数量，后面跟byUserNum个用户信息结构体UserInfoDef
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
                byUserNum = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define SERVER_TO_CLIENT_WAIT_READY_NOTICE			0x1119	//[游服]->[游客]四人入座等待用户准备/取消准备通知
        public class Server2ClientWaitReadyNoticeDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public ushort wTableNum;//桌号
            public byte byMode;//准备或是取消准备标志，1为准备，2为取消准备
            public int iWaitTime;//byMode为1时为等待时间，单位为s，byMode为2时为0
            public int[] UserID = new int[4];//桌上用户ID
            public int[] OutUserID = new int[4];//桌上离座用户ID
            public byte byStartFrequency;//第几次准备
            public byte pos_0;//
            public byte pos_1;//
            public byte pos_2;//
            //public byte pos0;//补位
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
                wTableNum = (ushort)stream.readShort();
                byMode = stream.readByte();
                iWaitTime = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    UserID[i] = stream.readInt();
                }
                for (int i = 0; i < 4; i++)
                {
                    OutUserID[i] = stream.readInt();
                }
                byStartFrequency = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                //pos0 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define KICK_OUT_PLAYER_WITHOUT_READY				0x111A //[游服]->[游客]踢出为准备用户通知
        public class KickOutPlayerWithoutReadyDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int[] iUserId = new int[4];//用户id
            public ushort wTableNum;//桌号
            public sbyte byKickType;//提出原因
            public byte pos_0;//
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
                for (int i = 0; i < 4; i++)
                {
                    iUserId[i] = stream.readInt();
                }
                wTableNum = (ushort)stream.readShort();
                byKickType = (sbyte)stream.readByte();
                pos_0 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_RP17_START_NOTICE		0x1120	//	[游服]->[游客]开始抢红包通知
        public class ClientRp17StartNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iRap17Id; //大红包编号
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
                iRap17Id = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_OPEN_RP17_REQ								0x1121	//	[游客]->[游服]抢红包请求
        public class ClientOpenRp17Req : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;
            public int iRp17Id; // 麻将馆大红包id

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

        //麻将馆的小红包信息
        public class ClientSrp17Info : Protocol
        {
            public int bySrp17Id; // 小红包id
            public int iRp17Id; // 大红包id
            public int iUserId; // 获取用户id                        
            public int iGetTime; // 获得时间
            public double iAssetNum; // 获取的资源数量            
            public byte byAssetType; // 获取的资源类型:1现金,2话费,3流量,4储值卡,5代金券,6赠币
            public byte pos0;
            public byte pos1;
            public byte pos2;
            public byte pos3;
            public byte pos4;
            public byte pos5;
            public byte pos6;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                bySrp17Id = stream.readByte();
                iRp17Id = stream.readInt();
                iUserId = stream.readInt();
                iGetTime = stream.readInt();
                iAssetNum = stream.readDouble();
                byAssetType = stream.readByte();
                pos0 = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                pos3 = stream.readByte();
                pos4 = stream.readByte();
                pos5 = stream.readByte();
                pos6 = stream.readByte();
                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                bySrp17Id = stream.readByte();
                iRp17Id = stream.readInt();
                iUserId = stream.readInt();
                iGetTime = stream.readInt();
                iAssetNum = stream.readDouble();
                byAssetType = stream.readByte();
                pos0 = stream.readByte();
                pos1 = stream.readByte();
                pos2 = stream.readByte();
                pos3 = stream.readByte();
                pos4 = stream.readByte();
                pos5 = stream.readByte();
                pos6 = stream.readByte();
            }
        }


        //#define CLIENT_OPEN_RP17_RES								0x1122	//	[游服]->[游客]抢红包回应
        public class ClientOpenRp17Res : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public int iUserId;
            public ClientSrp17Info srp17Info = new ClientSrp17Info(); // 小红包信息

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
                srp17Info.parseBytes(stream);
                return stream.currentPos();
            }
        }

        //#define SERVER_TO_CLIENT_AUTO_STATUS				0x1116	//[游服]->[游客] 发送用户托管状态给客户端
        public class SendUserAutoStatus : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte bySeatNum; //座位号
            public byte byStatus; //托管状态,0为未托管，1为托管
            public byte pos_0; //补位
            public byte pos_1; //补位
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
                bySeatNum = stream.readByte();
                byStatus = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_CANCLE_AUTO_STATUS_REQ				0x1117	//[游客]->[游服]用户取消托管请求消息
        public class ClientCancleAutoStatusReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte iSeatNum;//座位号         
            public byte pos_1;  //补位     
            public byte pos_2;  //补位     
            public byte pos_3;  //补位     
            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(iSeatNum);
                stream.writeByte(pos_1);
                stream.writeByte(pos_2);
                stream.writeByte(pos_3);
                return stream.toBytes();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                throw new NotImplementedException();
            }
        }

        //#define CLIENT_CANCLE_AUTO_STATUS_RES				0x1118	//[游服]->[游客]用户取消托管回应消息
        public class ClientCancleAutoStatusRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号

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
                return stream.currentPos();
            }
        }

        //#define CLIENT_READY_TIME_REQ							0x1124	//[游客]->[游服]获取准备剩余时间请求
        public class ClientReadyTimeReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId;//查询玩家ID
            public byte wTableNum; //桌号
            public byte pos_0; //
            public byte pos_1; //
            public byte pos_2; //

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(wTableNum);
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

        //#define CLIENT_READY_TIME_RES							0x1125	//[游服]->[游客]获取准备剩余时间回应
        public class ClientReadyTimeResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError;
            public int iUserId;//查询玩家ID
            public int iLeftSecs;//剩余秒数
            //public int[] iaUserReadyStatus = new int[4];//桌上用户准备状态，玩家的座位号 准备的是他的座位号 没准备的是0 没有人是-1

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
                iLeftSecs = stream.readInt();
                //for (int i = 0; i < 4; i++)
                //{
                //    iaUserReadyStatus[i] = stream.readInt();
                //}
                return stream.currentPos();
            }
        }

        //#define BESPEAK_TIME_OUT_NOTICE							0x1126	//[游服]->[游客]预约时间结束通知
        public class BespeakTimeOutNoticeDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int UserRoomID;//房主ID
            public byte[] OutSeatNum = new byte[4];//不在座位上人的座位号

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
                UserRoomID = stream.readInt();
                for (int i = 0; i < 4; i++)
                {
                    OutSeatNum[i] = stream.readByte();
                }
                return stream.currentPos();
            }
        }

        //#define CLIENT_NEXT_POINT_NOTICE				0x1211// [游服]->[游客]下分通知消息
        public class ClientNextPointNoticeDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public int iRemainingTime; //剩余时间
                                       //	BYTE byNextPoint; //下分：0不开启，1下一分 2下二分

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
                iRemainingTime = stream.readInt();
                return stream.currentPos();
            }
        }

        //#define CLIENT_NEXT_POINT_REQ				0x1212// [游客]->[游服]下分请求消息
        public class ClientNextPointReqDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte byNextPoint; //下分：0不开启，1下一分 2下二分
            public byte pos_0; //
            public byte pos_1; //
            public byte pos_2; //

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                stream.writeByte(byNextPoint);
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

        //#define CLIENT_NEXT_POINT_RES		0x1213	// [游服]->[游客]下分回应消息
        public class ClientNextPointResDef : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误编号，0正确
            public int iUserId; // 用户编号
            public byte byNextPoint; //下分：0不开启，1下一分 2下二分
            public byte pos_0; //
            public byte pos_1; //
            public byte pos_2; //

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
                byNextPoint = stream.readByte();
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
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
            public byte byShareType; ////0大厅 1活动 2邀请 3单局 4总局 5大赢家红包 6最佳炮手红包 7推广红包 8充值红包 9提现红包 10首次提现红包 11大厅分享按钮分享 12战绩分享
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
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;
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
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
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

        //#define CLIENT_APPLIQUE_NOTICE  0x1209	//[游客]->[游服]补花通知消息
        public class ClientAppliqueNotice : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号                                   
            public byte bySeatNum; // 座位号
            public byte byFlowerTileNum; // 花牌张数          
            public byte[] byaFlowerTile = new byte[8]; // 花牌
            public byte pos_0;
            public byte pos_1;

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
                bySeatNum = stream.readByte();
                byFlowerTileNum = stream.readByte();
                for (int i = 0; i < 8; i++)
                {
                    byaFlowerTile[i] = stream.readByte();
                }
                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                return stream.currentPos();
            }
        }

        //#define CLIENT_APPLIQUE_REQ   0x1210	//[游客]->[游服]补花请求消息
        public class ClientAppliqueReq : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iUserId; // 用户编号
            public byte[] byaFlowerTile = new byte[8]; // 花牌
            public byte byFlowerTileNum; // 花牌张数
            public byte pos_0;
            public byte pos_1;
            public byte pos_2;

            public override byte[] toBytes()
            {
                FieldStream stream = new FieldStream();
                msgHeadInfo.toBytes(stream);
                stream.writeInt(iUserId);
                for (int i = 0; i < 8; i++)
                {
                    stream.writeByte(byaFlowerTile[i]);
                }
                stream.writeByte(byFlowerTileNum);
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


        //#define CLIENT_APPLIQUE_RES   	0x1211	//[游服]->[游客]补花回应消息
        public class ClientAppliqueRes : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号                                               
            public byte byTileNum; //补牌张数
            public byte[] byaFlowerTile = new byte[8]; // 花牌
            public byte[] byaTile = new byte[8]; //牌

            public byte pos_0;
            public byte pos_1;
            public byte pos_2;

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
                byTileNum = stream.readByte();
                for (int i = 0; i < 8; i++)
                {
                    byaFlowerTile[i] = stream.readByte();
                }
                for (int i = 0; i < 8; i++)
                {
                    byaTile[i] = stream.readByte();
                }

                pos_0 = stream.readByte();
                pos_1 = stream.readByte();
                pos_2 = stream.readByte();
                return stream.currentPos();
            }
        }
        #endregion 消息定义

        #region 上版本的回放数据消息

        #region 麻将玩法配置结构体
        //CLIENT_PLAYING_METHOD_CONF_RES	0x1203	// [游服]->[游客]玩法配置回应消息
        public class ClientPlayingMethodConfRes_2 : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public int iError; // 错误代码
            public int iUserId; // 用户编号
            public int iPlayingMethod; // 玩法：1长治（缺门待炮），2长治（花三门），3长治打锅（花三门）
            public PlayingMethodConfDef_2 playingMethodConf = new PlayingMethodConfDef_2(); // 玩法配置


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
                iPlayingMethod = stream.readInt();
                playingMethodConf.parseBytes(stream);

                return stream.currentPos();
            }
        }

        //麻将玩法配置结构体
        public class PlayingMethodConfDef_2 : Protocol
        {
            // 6 付费 [billing]
            public byte byBillingMode; // 付费模式：1按圈付费，2按局付费，3按分付费
            public byte byBillingNumber; // 付费量：多少圈、局、分付费
            public byte byBillingPrice; // 付费价格：消耗多少张房卡
            public byte byBillingBespeak;//预约订金
            public byte byBillingAwardRate;//预约守约奖励
            public byte byBillingPayMode; // 房费支付模式:0房主支付,1AA支付
                                          // 3 前抬后和结算 [raise]
            public byte byRaiseMode; // 前抬后和模式：0都不用，1前抬，2后和,3前抬与后和同时开启
            public byte byRaiseMultiple; // 前抬后和支付：支付n倍底分
            public byte byRaiseNum; //前抬一次:0无次数限制，1只能一次
                                    // 1 胡牌结算 [win_point]
            public byte byWinPointMode; // 计分模式：
                                        // 1 底分*计番之和*庄闲倍数*自摸倍数*胡牌倍数 + 底分*额外计番之和
                                        // 2 底分*相同序牌数量*庄闲倍数*自摸倍数*胡牌倍数 + 底分*额外计番之和
                                        // 12
            public byte byWinPointBasePoint; // 底分
            public byte byWinPointFanPoint; // 每番分数(暂时没用)
            public byte byWinPointDealerMultiple; // 庄家倍数
            public byte byWinPointSelfDrawnMultiple; // 自摸倍数
            public byte byWinPointMultiShoot; // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
            public byte byWinPointMultiPay; // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
            public byte byWinPointMultiPayExtraMode; // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
            public byte byWinPointMultiPayExtra; // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
            public byte byWinPointWinSeveralGainSeveral; //放几出几：0不开启，1开启(开启放几出几，必须设置【胡 1 2 3 】为必须自摸)
            public byte byWinPointCanRunCanDown; //能跑能下：0不开启，1开启
            public byte byWinPointWinSevenToNineKokusai; // 胡 7 - 9 与字牌 倍数:0不开启，则为倍数
            public byte byWinPointMultiPayTrisection; //点炮三分:0不开启，1开启
            public byte byWinPointNextPoint; //下分 ：0不开启，1开启
                                             // 3 创牌模式 [create_mode]
            public byte byCreateHaveWind; // 是否有风牌：0没有，1有
            public byte byCreateHaveDragon; // 是否有箭牌：0没有，1有
            public byte byCreateHaveFlower; // 是否有花牌：0没有，1有

            // 2 出牌规则 [discard]
            public byte byDiscardChow; // 是否允许吃：0不允许，1允许
            public byte byWaitAutoDisCardTim;//等待进入托管状态时间,sec
            public byte byDiscardCanRunCanDownAndNextPointTime; //等待选择能跑能下或者下分时间:0不等待,1则为等待时间(必须能跑能下或者下分开启才有效)
                                                                /*留墩	[reserve]
                                                                留墩模式：0不留墩，
                                                                        1留牌数量=12+杠次数*2+杠次数%2，
                                                                        2留牌数量=14+杠次数*2+杠次数%2，
                                                                        3留牌数量=12+明杠次数*2+暗杠次数*4+杠次数%2
                                                                        **/
                                                                // 1
            public byte byReserveMode;
            // 1 荒庄 [draw]
            public byte byDrawFourKong; // 四杠荒庄：0不可以，1可以
            public byte byDrawFourNoKong;// 荒庄荒杠:0 荒庄不荒杠 ,1 荒庄荒杠
                                         // 6 换庄 [dealer]
            public byte byDealerMode; //换庄模式，1(庄闲胡牌有效)，2 (放炮自摸有效)
            public byte byDealerDraw; // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
            public byte byDealerDealer; // 庄家胡牌后换庄模式：1轮庄，2连庄
            public byte byDealerPlayer; // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
            public byte byDealerShoot;  //放炮后换庄模式：1轮庄，2抢庄
            public byte byDealerSelfDrawn; //放炮者自摸后换庄模式：1轮庄，2连庄
                                           // 10  胡牌限制 [win_limit]
            public byte byWinLimitLack; // 缺一门：0无要求，1必须缺一门
            public byte byWinLimitDependEight; // 靠八张：0无要求，1必须靠八张
            public byte byWinLimitSelfDrawn; // 自摸：0可接炮，1必须自摸
            public byte byWinLimitRobbingKong; // 抢杠胡：0不可抢杠胡，1可抢杠胡
            public byte byWinLimitBeginFan; // 起胡番数
            public byte byWinLimitWinOneTwoThree;//胡1 2 3:0无要求，1必须自摸(七对，十三幺除外)
            public byte byWinLimintReadHand;//是否需要报听0不许要，1需要
            public byte byUnderThreeLimit;//是否限制3以下序数牌成牌,0不限值，1限制
            public byte byWinAndGongPriority;//是否优先下家，0不优先，1优先
            public byte byEscapeWinAndPong;//是否漏碰漏胡,0，不漏碰漏胡，1漏胡漏碰
                                           // 2 特殊牌型 [win_special]
            public byte byWinSpecialThirteenIndepend; // 十三不靠：0不可胡，1可胡
            public byte byWinSpecialThirteenOrphans; // 十三幺：0不可胡，1可胡
                                                     /* 十三幺吃抢：
                                                      *  0 不可吃抢，
                                                      *  1 可吃抢（当十三幺为1可胡时才有效，只能吃抢各一次）
                                                      *  2 可吃抢(当十三幺为1可胡时才有效，能吃n次，只能抢一次)
                                                      *  3 可吃不可抢(当十三幺为1可胡时才有效，能吃n次)
                                                      *  4 可枪不可吃
                                                      **/
                                                     // 6
            public byte byWinSpecialThirteenOrphansCr;
            public byte byWinSpecialThirteenOrphansGrab; // 十三幺-抢后可吃：0不可吃，1可吃
            public byte byWinSpecialThirteenOrphansNum;//十三幺-吃抢牌数相关倍数：0不开启，1开启
            public byte byWinSpecialSevenPairs; // 七对：0不可胡，1可胡
            public byte byWinSpecialSevenPairsFlag; // 七对标志：0普通，1不能有同色三顺对
            public byte byWinSpecialLuxurySevenPairs; // 豪华七对：0不可胡，1可胡
                                                      // 19 计番 [fan]
            public byte byFanCommonHand; // 平胡
            public byte byDragonHand;//龙胡
            public byte byFanThirteenIndepend; // 十三不靠
            public byte byFanThirteenOrphans; // 十三幺
            public byte byFanSevenPairs; // 七对
            public byte byFanLuxurySevenPairs; // 豪华七对
            public byte byFanGangKai; // 杠上开花番数，华昌
            public byte byAllConcealedHand;//门清
            public byte bySisterPave;//姐妹
            public byte byOldYoung;//老少
            public byte byBrokenOrphas;//断幺
            public byte brLackSuit;//缺一门
            public byte byConnectedSequence;//一条龙
            public byte byCouplesHand;//对对胡
            public byte bySameColor;//清一色
            public byte byMixedOneSuit;//混一色
            public byte byUseFanDragonWind;//是否使用令牌风牌番数
            public byte byFanDragonTille;//令牌番数（硬三百）
            public byte byFanWindTile;//风牌番数（硬三百）
                                      // 8 额外计番 [fan_extra]
            public byte byFanExtraMode;//结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
            public byte byFanExtraExposedMode; // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
            public byte byFanExtraExposedBase; // 明杠基础番数(三家都支付)
            public byte byFanExtraExposedExtra; // 明杠额外番数(被杠/碰支付)
            public byte byFanExtraConcealedKong; // 暗杠
            public byte byFanRobbingKongMode; // 抢杠胡支付模式:0不支付，1三家都支付
            public byte byFanRobbingposedBase; // 抢杠胡基础番数(三家都支付)
            public byte byFanRobbingExposedExtra; // 抢杠胡额外番数(抢杠支付)      
                                                  // 6 黎城翻番倍数
            public byte byLiFanPlusNum;//2 3 4 5 翻几倍 
            public byte[] byLiFanPlus = new byte[5];

            //public byte pos_0;
            //public byte pos_1;
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }

            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);
                byBillingMode = stream.readByte(); // 付费模式：1按圈付费，2按局付费，3按分付费
                byBillingNumber = stream.readByte(); // 付费量：多少圈、局、分付费
                byBillingPrice = stream.readByte(); // 付费价格：消耗多少张房卡   
                byBillingBespeak = stream.readByte();//预约订金
                byBillingAwardRate = stream.readByte();//预约守约奖励       
                byBillingPayMode = stream.readByte();
                byRaiseMode = stream.readByte(); // 前抬后和模式：0都不用，1前抬，2后和,3前抬与后和同时开启
                byRaiseMultiple = stream.readByte(); // 前抬后和支付：支付n倍底分
                byRaiseNum = stream.readByte(); //前抬一次:0无次数限制，1只能一次
                byWinPointMode = stream.readByte(); // 计分模式：                                    
                byWinPointBasePoint = stream.readByte(); // 底分
                byWinPointFanPoint = stream.readByte(); // 每番分数(暂时没用)
                byWinPointDealerMultiple = stream.readByte(); // 庄家倍数
                byWinPointSelfDrawnMultiple = stream.readByte(); // 自摸倍数
                byWinPointMultiShoot = stream.readByte(); // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
                byWinPointMultiPay = stream.readByte(); // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
                byWinPointMultiPayExtraMode = stream.readByte(); // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
                byWinPointMultiPayExtra = stream.readByte(); // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
                byWinPointWinSeveralGainSeveral = stream.readByte(); //放几出几：0不开启，1开启(开启放几出几，必须设置【胡 1 2 3 】为必须自摸)
                byWinPointCanRunCanDown = stream.readByte(); //能跑能下：0不开启，1开启
                byWinPointWinSevenToNineKokusai = stream.readByte(); // 胡 7 - 9 与字牌 倍数:0不开启，则为倍数  
                byWinPointMultiPayTrisection = stream.readByte();
                byWinPointNextPoint = stream.readByte();
                byCreateHaveWind = stream.readByte();
                byCreateHaveDragon = stream.readByte();
                byCreateHaveFlower = stream.readByte();


                byDiscardChow = stream.readByte(); // 是否允许吃：0不允许，1允许      
                byWaitAutoDisCardTim = stream.readByte();
                byDiscardCanRunCanDownAndNextPointTime = stream.readByte();
                byReserveMode = stream.readByte();
                byDrawFourKong = stream.readByte(); // 四杠荒庄：0不可以，1可以     
                byDrawFourNoKong = stream.readByte();// 荒庄荒杠:0 荒庄不荒杠 ,1 荒庄荒杠                              
                byDealerMode = stream.readByte(); //换庄模式，1(庄闲胡牌有效)，2 (放炮自摸有效)
                byDealerDraw = stream.readByte(); // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
                byDealerDealer = stream.readByte(); // 庄家胡牌后换庄模式：1轮庄，2连庄
                byDealerPlayer = stream.readByte(); // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
                byDealerShoot = stream.readByte();  //放炮后换庄模式：1轮庄，2抢庄
                byDealerSelfDrawn = stream.readByte(); //放炮者自摸后换庄模式：1轮庄，2连庄                                        
                byWinLimitLack = stream.readByte(); // 缺一门：0无要求，1必须缺一门
                byWinLimitDependEight = stream.readByte(); // 靠八张：0无要求，1必须靠八张
                byWinLimitSelfDrawn = stream.readByte(); // 自摸：0可接炮，1必须自摸
                byWinLimitRobbingKong = stream.readByte(); // 抢杠胡：0不可抢杠胡，1可抢杠胡
                byWinLimitBeginFan = stream.readByte(); // 起胡番数
                byWinLimitWinOneTwoThree = stream.readByte();//胡1 2 3:0无要求，1必须自摸(七对，十三幺除外)
                byWinLimintReadHand = stream.readByte();//是否需要报听0不许要，1需要
                byUnderThreeLimit = stream.readByte();//是否限制3以下序数牌成牌,0不限值，1限制    
                byWinAndGongPriority = stream.readByte();
                byEscapeWinAndPong = stream.readByte();
                byWinSpecialThirteenIndepend = stream.readByte(); // 十三不靠：0不可胡，1可胡
                byWinSpecialThirteenOrphans = stream.readByte(); // 十三幺：0不可胡，1可胡                              
                byWinSpecialThirteenOrphansCr = stream.readByte();
                byWinSpecialThirteenOrphansGrab = stream.readByte(); // 十三幺-抢后可吃：0不可吃，1可吃
                byWinSpecialThirteenOrphansNum = stream.readByte();//十三幺-吃抢牌数相关倍数：0不开启，1开启
                byWinSpecialSevenPairs = stream.readByte(); // 七对：0不可胡，1可胡
                byWinSpecialSevenPairsFlag = stream.readByte(); // 七对标志：0普通，1不能有同色三顺对
                byWinSpecialLuxurySevenPairs = stream.readByte(); // 豪华七对：0不可胡，1可胡
                byFanCommonHand = stream.readByte(); // 平胡
                byDragonHand = stream.readByte();//龙胡
                byFanThirteenIndepend = stream.readByte(); // 十三不靠
                byFanThirteenOrphans = stream.readByte(); // 十三幺
                byFanSevenPairs = stream.readByte(); // 七对
                byFanLuxurySevenPairs = stream.readByte(); // 豪华七对
                byFanGangKai = stream.readByte(); // 杠上开花番数
                byAllConcealedHand = stream.readByte();//门清
                bySisterPave = stream.readByte();//姐妹
                byOldYoung = stream.readByte();//老少
                byBrokenOrphas = stream.readByte();//断幺
                brLackSuit = stream.readByte();//缺一门
                byConnectedSequence = stream.readByte();//一条龙
                byCouplesHand = stream.readByte();//对对胡
                bySameColor = stream.readByte();//清一色
                byMixedOneSuit = stream.readByte();//混一色
                byUseFanDragonWind = stream.readByte();//是否使用令牌风牌番数
                byFanDragonTille = stream.readByte();//令牌番数（硬三百）
                byFanWindTile = stream.readByte();//风牌番数（硬三百）
                byFanExtraMode = stream.readByte();//结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
                byFanExtraExposedMode = stream.readByte(); // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
                byFanExtraExposedBase = stream.readByte(); // 明杠基础番数(三家都支付)
                byFanExtraExposedExtra = stream.readByte(); // 明杠额外番数(被杠/碰支付)
                byFanExtraConcealedKong = stream.readByte(); // 暗杠
                byFanRobbingKongMode = stream.readByte(); // 抢杠胡支付模式:0不支付，1三家都支付
                byFanRobbingposedBase = stream.readByte(); // 抢杠胡基础番数(三家都支付)
                byFanRobbingExposedExtra = stream.readByte(); // 抢杠胡额外番数(抢杠支付)             
                byLiFanPlusNum = stream.readByte();
                for (int i = 0; i < 5; i++)
                {
                    byLiFanPlus[i] = stream.readByte();
                }

                //pos_0 = stream.readByte();
                //pos_1 = stream.readByte();

                return stream.currentPos();
            }
            public void parseBytes(FieldStream stream)
            {
                byBillingMode = stream.readByte(); // 付费模式：1按圈付费，2按局付费，3按分付费
                byBillingNumber = stream.readByte(); // 付费量：多少圈、局、分付费
                byBillingPrice = stream.readByte(); // 付费价格：消耗多少张房卡                   
                byBillingBespeak = stream.readByte(); ;//预约订金
                byBillingAwardRate = stream.readByte(); ;//预约守约奖励   
                byBillingPayMode = stream.readByte();
                byRaiseMode = stream.readByte(); // 前抬后和模式：0都不用，1前抬，2后和,3前抬与后和同时开启
                byRaiseMultiple = stream.readByte(); // 前抬后和支付：支付n倍底分
                byRaiseNum = stream.readByte(); //前抬一次:0无次数限制，1只能一次
                byWinPointMode = stream.readByte(); // 计分模式：                                    
                byWinPointBasePoint = stream.readByte(); // 底分
                byWinPointFanPoint = stream.readByte(); // 每番分数(暂时没用)
                byWinPointDealerMultiple = stream.readByte(); // 庄家倍数
                byWinPointSelfDrawnMultiple = stream.readByte(); // 自摸倍数
                byWinPointMultiShoot = stream.readByte(); // 一炮多响：0一炮单响，1一炮多响（自摸设置必须是可接炮才有效）
                byWinPointMultiPay = stream.readByte(); // 接炮收三家/坐庄：0只收放炮的，1收三家（自摸设置必须是可接炮才有效）
                byWinPointMultiPayExtraMode = stream.readByte(); // 放炮额外支付模式：0不额外支付，1加n倍底分（接炮收三家设置必须是1收三家才有效）
                byWinPointMultiPayExtra = stream.readByte(); // 放炮额外支付底分倍数（放炮额外支付模式必须是1加n倍底分才有效）
                byWinPointWinSeveralGainSeveral = stream.readByte(); //放几出几：0不开启，1开启(开启放几出几，必须设置【胡 1 2 3 】为必须自摸)
                byWinPointCanRunCanDown = stream.readByte(); //能跑能下：0不开启，1开启
                byWinPointWinSevenToNineKokusai = stream.readByte(); // 胡 7 - 9 与字牌 倍数:0不开启，则为倍数   
                byWinPointMultiPayTrisection = stream.readByte();
                byWinPointNextPoint = stream.readByte();
                byCreateHaveWind = stream.readByte();
                byCreateHaveDragon = stream.readByte();
                byCreateHaveFlower = stream.readByte();
                byDiscardChow = stream.readByte(); // 是否允许吃：0不允许，1允许    
                byWaitAutoDisCardTim = stream.readByte();
                byDiscardCanRunCanDownAndNextPointTime = stream.readByte();
                byReserveMode = stream.readByte();
                byDrawFourKong = stream.readByte(); // 四杠荒庄：0不可以，1可以     
                byDrawFourNoKong = stream.readByte();// 荒庄荒杠:0 荒庄不荒杠 ,1 荒庄荒杠                              
                byDealerMode = stream.readByte(); //换庄模式，1(庄闲胡牌有效)，2 (放炮自摸有效)
                byDealerDraw = stream.readByte(); // 荒庄后换庄模式：1轮庄，2连庄，3有杠轮庄无杠连庄
                byDealerDealer = stream.readByte(); // 庄家胡牌后换庄模式：1轮庄，2连庄
                byDealerPlayer = stream.readByte(); // 闲家胡牌后换庄模式：1轮庄，2抢庄(不能是按圈付费)
                byDealerShoot = stream.readByte();  //放炮后换庄模式：1轮庄，2抢庄
                byDealerSelfDrawn = stream.readByte(); //放炮者自摸后换庄模式：1轮庄，2连庄                                        
                byWinLimitLack = stream.readByte(); // 缺一门：0无要求，1必须缺一门
                byWinLimitDependEight = stream.readByte(); // 靠八张：0无要求，1必须靠八张
                byWinLimitSelfDrawn = stream.readByte(); // 自摸：0可接炮，1必须自摸
                byWinLimitRobbingKong = stream.readByte(); // 抢杠胡：0不可抢杠胡，1可抢杠胡
                byWinLimitBeginFan = stream.readByte(); // 起胡番数
                byWinLimitWinOneTwoThree = stream.readByte();//胡1 2 3:0无要求，1必须自摸(七对，十三幺除外)
                byWinLimintReadHand = stream.readByte();//是否需要报听0不许要，1需要
                byUnderThreeLimit = stream.readByte();//是否限制3以下序数牌成牌,0不限值，1限制  
                byWinAndGongPriority = stream.readByte();
                byEscapeWinAndPong = stream.readByte();
                byWinSpecialThirteenIndepend = stream.readByte(); // 十三不靠：0不可胡，1可胡
                byWinSpecialThirteenOrphans = stream.readByte(); // 十三幺：0不可胡，1可胡                              
                byWinSpecialThirteenOrphansCr = stream.readByte();
                byWinSpecialThirteenOrphansGrab = stream.readByte(); // 十三幺-抢后可吃：0不可吃，1可吃
                byWinSpecialThirteenOrphansNum = stream.readByte();//十三幺-吃抢牌数相关倍数：0不开启，1开启
                byWinSpecialSevenPairs = stream.readByte(); // 七对：0不可胡，1可胡
                byWinSpecialSevenPairsFlag = stream.readByte(); // 七对标志：0普通，1不能有同色三顺对
                byWinSpecialLuxurySevenPairs = stream.readByte(); // 豪华七对：0不可胡，1可胡
                byFanCommonHand = stream.readByte(); // 平胡
                byDragonHand = stream.readByte();//龙胡
                byFanThirteenIndepend = stream.readByte(); // 十三不靠
                byFanThirteenOrphans = stream.readByte(); // 十三幺
                byFanSevenPairs = stream.readByte(); // 七对
                byFanLuxurySevenPairs = stream.readByte(); // 豪华七对
                byFanGangKai = stream.readByte(); // 杠上开花番数
                byAllConcealedHand = stream.readByte();//门清
                bySisterPave = stream.readByte();//姐妹
                byOldYoung = stream.readByte();//老少
                byBrokenOrphas = stream.readByte();//断幺
                brLackSuit = stream.readByte();//缺一门
                byConnectedSequence = stream.readByte();//一条龙
                byCouplesHand = stream.readByte();//对对胡
                bySameColor = stream.readByte();//清一色
                byMixedOneSuit = stream.readByte();//混一色
                byUseFanDragonWind = stream.readByte();//是否使用令牌风牌番数
                byFanDragonTille = stream.readByte();//令牌番数（硬三百）
                byFanWindTile = stream.readByte();//风牌番数（硬三百）
                byFanExtraMode = stream.readByte();//结算模式 :1即时结算，2结算胡牌人额外番数，3结算所有人额外番数
                byFanExtraExposedMode = stream.readByte(); // 明杠支付模式：0不支付，1被杠者支付，2三家都支付
                byFanExtraExposedBase = stream.readByte(); // 明杠基础番数(三家都支付)
                byFanExtraExposedExtra = stream.readByte(); // 明杠额外番数(被杠/碰支付)
                byFanExtraConcealedKong = stream.readByte(); // 暗杠
                byFanRobbingKongMode = stream.readByte(); // 抢杠胡支付模式:0不支付，1三家都支付
                byFanRobbingposedBase = stream.readByte(); // 抢杠胡基础番数(三家都支付)
                byFanRobbingExposedExtra = stream.readByte(); // 抢杠胡额外番数(抢杠支付)                                   
                byLiFanPlusNum = stream.readByte();
                for (int i = 0; i < 5; i++)
                {
                    byLiFanPlus[i] = stream.readByte();
                }
                //pos_0 = stream.readByte();
                //pos_1 = stream.readByte();
            }

        }


        #endregion

        #region 一局结算的预置体
        //CLIENT_GAME_RESULT_NOTICE	0x1200	// [游服]->[游客]一局游戏结果通知消息
        public class ClientGameResultNotice_2 : Protocol
        {
            public MsgHeadDef msgHeadInfo = new MsgHeadDef();
            public byte[] byaWinSeat = new byte[MAX_USER_PER_TABLE]; // 所有用户胡牌标志
            public byte byShootSeat; // 放炮用户座位号
            public byte byShootSeatReadHand;//放炮用户是否报听  0没操作 1没报听  2报听点炮            
            public byte byDismiss; // 是否解散
            public int[] caResultPoint = new int[MAX_USER_PER_TABLE]; // 所有用户得分
            public byte[,] bya2HandTiles = new byte[MAX_USER_PER_TABLE, 14]; // 所有用户手上的牌            
            public sbyte[,] caFanResult = new sbyte[MAX_USER_PER_TABLE, F_TOTAL_NUM]; // 胡牌的人番数结果
            public ResultTypeDef_2[] aResultType = new ResultTypeDef_2[MAX_USER_PER_TABLE]; // 所有用户结果   
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
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    byaWinSeat[i] = stream.readByte();
                }
                //12↑
                byShootSeat = stream.readByte();
                byShootSeatReadHand = stream.readByte();
                //byIfNormalOrDragon = stream.readByte();
                byDismiss = stream.readByte();
                //12+3↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    caResultPoint[i] = stream.readInt();
                }
                //12+3+16↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        bya2HandTiles[i, j] = stream.readByte();
                    }
                }
                //12+3+16+56↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    for (int j = 0; j < F_TOTAL_NUM; j++)
                    {
                        caFanResult[i, j] = (sbyte)stream.readByte();
                    }
                }
                //12+3+16+56+72↑
                for (int i = 0; i < MAX_USER_PER_TABLE; i++)
                {
                    aResultType[i] = new ResultTypeDef_2();
                    aResultType[i].parseBytes(stream);
                }

                return stream.currentPos();
            }
        }



        //结果信息
        public class ResultTypeDef_2 : Protocol
        {
            public sbyte cTripletToKongPoint; //补杠得的分数和（补杠用户加，其他用户减）
            public sbyte cRobbingKongPoint;//抢杠胡得的分数和（庄家加，闲家减 抢杠用户减/加）
            public byte bySequenceNum; // 顺子的数量
            public byte byTripletNum; //刻子的数量
            public byte bylaizinum;//癞子的数量 
            public byte byThirteenOrphansNum; // 十三幺吃抢的数量    
            public int cExposedKongPoint; // 明杠得的番数和（明杠用户加，被杠/其他用户减）
            public int cConcealedKongPoint; // 暗杠得的番数和（暗杠用户加，其他用户减）  
            public byte[] byFanTiles = new byte[8]; // 翻码牌             

            public EyesTypeDef eyesType = new EyesTypeDef(); // 将牌
            public SequenceTypeDef[] sequenceType = new SequenceTypeDef[4]; // 顺子牌
            public TripletTypeDef[] tripletType = new TripletTypeDef[4]; // 刻子牌
            public ThirteenOrphansTypeDef[] thirteenOrphansType = new ThirteenOrphansTypeDef[14]; // 十三幺吃抢信息（长治花三门）
            public LaiziTypeDef[] LaiziType = new LaiziTypeDef[4]; // 癞子牌
            public LastTileDef lastTile = new LastTileDef(); // 最后到手的牌

            public byte pos_0;  //补位
            public override byte[] toBytes()
            {
                throw new NotImplementedException();
            }
            public ResultTypeDef_2()
            {
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i] = new SequenceTypeDef();
                    tripletType[i] = new TripletTypeDef();

                }
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i] = new ThirteenOrphansTypeDef();
                }
            }
            public override int parseBytes(byte[] bytes, int begin)
            {
                if (bytes == null) return 0;
                FieldStream stream = new FieldStream(bytes, begin);

                cTripletToKongPoint = (sbyte)stream.readByte();
                cRobbingKongPoint = (sbyte)stream.readByte();

                bySequenceNum = stream.readByte();
                byTripletNum = stream.readByte();
                Debug.LogError("壳子数量" + byTripletNum + "位置" + stream.currentPos());

                bylaizinum = stream.readByte();
                Debug.LogError("癞子数量" + byTripletNum);
                byThirteenOrphansNum = stream.readByte();
                cExposedKongPoint = stream.readInt();
                cConcealedKongPoint = stream.readInt();
                for (int i = 0; i < byFanTiles.Length; i++)
                {
                    byFanTiles[i] = stream.readByte();
                }
                eyesType.parseBytes(stream);
                //11+5+2↑
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i].parseBytes(stream);
                }
                //11+5+2+12↑
                for (int i = 0; i < 4; i++)
                {
                    tripletType[i].parseBytes(stream);
                }
                //11+5+2+12+16↑
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i].parseBytes(stream);
                }
                for (int i = 0; i < 4; i++)
                {
                    LaiziType[i].parseBytes(stream);
                }
                //11+5+2+12+16+42↑
                lastTile.parseBytes(stream);
                pos_0 = stream.readByte();
                // pos_1 = stream.readByte();
                return stream.currentPos();
            }

            public void parseBytes(FieldStream stream)
            {

                cTripletToKongPoint = (sbyte)stream.readByte();
                cRobbingKongPoint = (sbyte)stream.readByte();

                bySequenceNum = stream.readByte();
                byTripletNum = stream.readByte();
                //  Debug.LogError("壳子数量" + byTripletNum + "位置" + stream.currentPos());
                bylaizinum = stream.readByte();
                //  Debug.LogError("癞子数量" + byTripletNum);
                byThirteenOrphansNum = stream.readByte();
                cExposedKongPoint = stream.readInt();
                cConcealedKongPoint = stream.readInt();
                for (int i = 0; i < byFanTiles.Length; i++)
                {
                    byFanTiles[i] = stream.readByte();
                }
                eyesType.parseBytes(stream);
                //11+5+2↑
                for (int i = 0; i < 4; i++)
                {
                    sequenceType[i].parseBytes(stream);
                }
                //11+5+2+12↑
                for (int i = 0; i < 4; i++)
                {
                    tripletType[i].parseBytes(stream);
                }
                //11+5+2+12+16↑
                for (int i = 0; i < 14; i++)
                {
                    thirteenOrphansType[i].parseBytes(stream);
                }
                for (int i = 0; i < 4; i++)
                {
                    LaiziType[i].parseBytes(stream);
                }
                //11+5+2+12+16+42↑
                lastTile.parseBytes(stream);
                pos_0 = stream.readByte();
                //pos_1 = stream.readByte();
            }
        }
        #endregion

        #endregion

    }
}

