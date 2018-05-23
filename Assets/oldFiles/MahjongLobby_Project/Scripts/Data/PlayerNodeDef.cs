using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Network.Message;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerNodeDef
    {

        public WXWebElement wxElement = new WXWebElement();

        public uint isSencondTime; //1第一次，2第二次
        public int iAuthenType;        //认证类型
        public bool isAuthSuccess = false;//认证成功标志位

        public int iUserId; // 用户编号
        public int iCityId; // 选择的市编号
        public int iCountyId; // 选择的县编号
        public string szOpenid; // 微信openid
        public string szUnionid; // 微信unionid
        public string szNickname; // 微信昵称
        public string szHeadimgurl; // 微信头像网址
        public string szAccessToken; // 微信access_token
        public string szRefreshToken; // 微信refresh_token
        public int iCoin; //保存玩家的金币数量
        /// <summary>
        /// 
        /// </summary>
        public int[] iCoin_3 = new int[3]; // 金币 
        public int iBindCoin; // 绑定金币
        public int[,] iScore = new int[3, 2]; // 当前积分
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
        public int iLastBindCoinTim; // 最后一次领取绑定金币时间
        public int iLastHolidayId; // 最后一次节日编号
        public int iLastHolidayGiftTim; // 最后一次领取节日礼包时间
        public double fParlorScore;//麻将馆业绩积分
        public byte bySex; // 性别
        public byte byCreateParlorCert; // 是否有开麻将馆资格
        public byte byNameAuthen; // 实名认证标志
        public byte byUserSource; // 用户来源：1游客，2微信
        public int iLeaveParlorAcc; // 退出麻将馆次数累计
        public int iKickParlorAcc; // 被踢出麻将馆次数累计
        public int iDismissParlorAcc; // 解散麻将管次数累计  
        public int iCreatParlorAcc; //创建馆的次数累计
        public byte byNewUser; // 新用户标志

        public NetMsg.ClientUserDef userDef = new NetMsg.ClientUserDef();
        public PlayerNodeDef()
        {

        }
        public PlayerNodeDef(NetMsg.ClientUserDef userinfo)
        {
            iUserId = userinfo.iUserId; bySex = userinfo.bySex; byCreateParlorCert = userinfo.byCreateParlorCert;
            iCityId = userinfo.iCityId; iCountyId = userinfo.iCountyId; szOpenid = userinfo.szOpenid; szUnionid = userinfo.szUnionid; szNickname = userinfo.szNickname; szHeadimgurl = userinfo.szHeadimgurl; szAccessToken = userinfo.szAccessToken;
            szRefreshToken = userinfo.szRefreshToken; iCoin_3 = userinfo.iCoin; iScore = userinfo.da2Score; iCompliment = userinfo.iCompliment;
            iSpreaderId = userinfo.iSpreaderId; iSpreadGiftTime = userinfo.iSpreadGiftTime; iMyParlorId = userinfo.iMyParlorId;
            iaJoinParlorId = userinfo.iaJoinParlorId; iJoinParlorTime = userinfo.iJoinParlorTime; iLeaveParlorTime = userinfo.iLeaveParlorTime; iKickParlorTime = userinfo.iKickParlorTime;
            iGameNumAcc = userinfo.iGameNumAcc; iDisconnectAcc = userinfo.iDisconnectAcc;
            iPlayCardAcc = userinfo.iPlayCardAcc; iPlayCardTimeAcc = userinfo.iPlayCardTimeAcc;
            byNameAuthen = userinfo.byNameAuthen; byUserSource = userinfo.byUserSource; iLastBindCoinTim = userinfo.iLastCoin3Tim; iLastHolidayId = userinfo.iLastHolidayId; iLastHolidayGiftTim = userinfo.iLastHolidayGiftTim;
            fParlorScore = (float)userinfo.fParlorScore / 100f; iCoin = userinfo.iCoin[0] + userinfo.iCoin[1] + userinfo.iCoin[2]; iLeaveParlorAcc = userinfo.iLeaveParlorAcc;
            iKickParlorAcc = userinfo.iKickParlorAcc; iDismissParlorAcc = userinfo.iDismissParlorAcc; iCreatParlorAcc = userinfo.iDismissParlorAcc;
        }

    }
}

