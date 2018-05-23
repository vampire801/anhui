//#define SEVER_TYPE_214    //正式
//#define SEVER_TYPE_182  //刘迪
//#define SEVER_TYPE_21   //局域
#define SEVER_TYPE_FORMAL //外网测试

using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class LobbyContants
    {
        internal const string  AppName="shangxiAnhui";
        internal const bool DebugMode = false;
        public const string AssetDir = "StreamingAssets";           //素材目录 
        public static string version_v = "v1.2.9";
        public const bool isContorledByBack = false;//是否受后台控制(ios送审专用)
        public const int iShowGuestLogin = 1;  //是否显示游客登陆
        public const int iChannelVersion = 1001;  //包体的渠道编号  1000苹果，1001表示官方，1002表示应用宝，1003今日头条，1004广点通
        public static short SeverVersion = 1001;  //消息版本号，1001服务器本机版本号  1004服务器外部版本号  
        public static int LOBBY_GATEWAY_PORT = 1031;   //大厅网关端口1033 
        public const bool isOpenDebugMessage_Send = true;   //是否打开打印发送服务器日志的消息
        public const bool isOpenDebugMessage_Onreceive = true;  //是否发开打印接受的服务器的日志的消息   
        public const string SetSeatIDAgo = "SetSeatIDAgo";//是不是新的玩家进入这个桌
        public const string IOSDownLoadURL = "https://itunes.apple.com/cn/app/%E5%8F%8C%E5%96%9C%E9%BA%BB%E5%B0%86-%E5%AE%89%E5%BE%BD/id1276148462?mt=8";//安徽
        public const string WebTime = "http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1";//香港天文
        public const string GetIPWeb = "https://sxinterf.ibluejoy.com/getIp.x";//玩家IP请求地址
        public const string WeChatRefreshUrl = "https://api.weixin.qq.com/sns/oauth2/refresh_token?APPID={0}&grant_type=refresh_token&refresh_token={1}";
        public const string WcChatUserInfUrl = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}";
        public static LogSeverity LogLevel = LogSeverity.LogException;
        //  public static LogSeverity LogLevel = LogSeverity.Log;
#if SEVER_TYPE_182
        public const int Version = 10001;  //版本号
        public const string UpdateDownLoadUrl = "http://192.168.0.11/update/sxah/"; //热更下载地址
        public static string LOBBY_GATEWAY_IP = "192.168.1.182"; //199   "192.168.0.16"  "192.168.1.199"
        public const string MAJONG_PORT_URL = "http://192.168.0.22/sxah_interf/";  //局域网的接口地址
        public const string MAJONG_PORT_URL_T = "http://192.168.0.22/sxah_interf/";  //局域网的接口地址
        public const string URL_MEMBERCENTER = "http://192.168.0.22/sxah_mobile/clientLogin.shtml?";  //局域网的会员中心的地址,添加参数uid玩家id,token玩家的accesstoken,t 1表示登录个人中心，2表示登录到代理
        public const string SeverType = "182";
        public const string RankUrl = "http://192.168.0.22/sxah_interf/rank.x?type={0}&uid={1}";//排行
        public const string JoinPostUrl = "http://192.168.0.22/sxah_gm/upload/qb1.jpg?r={0}";//加盟图片
        public const string QunQRcode = "http://192.168.0.22/sxah_gm/upload/qr/{0}1.jpg?r={1}";//JoinQRcode
        public const string OfficePostUrl = "http://192.168.0.22/sxah_gm/upload/qb0.jpg?r={0}";//官方活动公告
        public const string DownLoadQRcode = "http://192.168.0.22/sxah_gm/upload/qr/qr_down.png"; //截图二维码
        public const string ActivityUrl = "http://192.168.0.22:8080/sxah_interf/ActivityState.x";//活动接口

#endif
#if SEVER_TYPE_21
        public const string version_typr = "局域网";
        public const int ClientVersion = 10001;  //客户端版本号  10001是168的包   10005是169的包     
        public const string UpdateDownLoadUrl = "http://192.168.0.11/update/sxah/"; //热更下载地址
        public static string LOBBY_GATEWAY_IP = "192.168.0.21"; //局域网   "192.168.0.16"  "192.168.1.199"
        public const string MAJONG_PORT_URL = "http://192.168.0.22/sxah_interf/";  //局域网的接口地址
        public const string MAJONG_PORT_URL_T = "http://192.168.0.22/sxah_interf/";  //局域网的接口地址
        public const string URL_MEMBERCENTER = "http://192.168.0.22/sxah_mobile/clientLogin.shtml?";  //局域网的会员中心的地址
        public const string SeverType = "21";
        public const string ActivitePic = "http://192.168.0.22/sxah_gm/upload/";//图片接口
        public const string ShareJsp = "http://192.168.0.22/sxah_mobile/share.jsp?uid={0}&type={1}&id={2}&time={3}";//红包分享接口
        public const string OfficePostUrl = "http://192.168.0.22/sxah_gm/upload/qb0.jpg?r={0}";//官方活动公告
        public const string QunQRcode = "http://192.168.0.22/sxah_gm/upload/qr/{0}1.jpg?r={1}";//JoinQRcode
        public const string DownLoadQRcode = "http://192.168.0.22/sxah_gm/upload/qr/qr_down.png";//截图二维码
        public const string FestivalActivityUrl = "http://192.168.0.22/sxah_interf/";//节日活动接口
        public const string FestivalShareActivityUrl = "http://192.168.0.22/sxah_gm/upload/sx_share.jpg";//节日活动分享图片
        public const string RankUrl = "http://192.168.0.22/sxah_interf/rank.x?type={0}&uid={1}";//排行
        public const string JoinPostUrl = "http://192.168.0.22/sxah_gm/upload/qb1.jpg?r={0}";//加盟图片
        public const string ActivityUrl = "http://192.168.0.22:8080/sxah_interf/ActivityState.x";//活动接口

#endif
#if SEVER_TYPE_FORMAL
        public const int ClientVersion = 10002;  //版本号 1001是外网正式  1002是外网送审   
        public const string version_typr = "外网测试 ";//_外网测试          
        public const string UpdateDownLoadUrl = "https://update.ibluejoy.com/sxah/"; //正式服热更下载地址  
        public static string LOBBY_GATEWAY_IP = "47.94.223.95"; //外网测试  
        public const string MAJONG_PORT_URL = "https://sxahinterft.ibluejoy.com/";  //外网测试的接口地址
        public const string MAJONG_PORT_URL_T = "https://sxahinterft.ibluejoy.com/";  //外网测试的接口地址
        public const string URL_MEMBERCENTER = "https://sxaht.ibluejoy.com/m/clientLogin.shtml?";  //外网测试的会员中心的地址
        public const string SeverType = "07";
        public const string ActivitePic = "https://sxahgmt.ibluejoy.com/upload/";//图片接口
        public const string ShareJsp = "https://sxaht.ibluejoy.com/m/share.jsp?uid={0}&type={1}&id={2}&time={3}";//红包分享接口
        public const string QunQRcode = "https://sxahgm.ibluejoy.com/upload/qr/{0}1.jpg?r={1}";//JoinQRcode
        public const string OfficePostUrl = "http://sxahgm.ibluejoy.com/upload/qb0.jpg?r={0}";//官方活动公告
        public const string DownLoadQRcode = "http://sxahgm.ibluejoy.com/upload/qr/qr_down.png";//截图二维码
        public const string FestivalActivityUrl = "https://sxahinterft.ibluejoy.com/";//节日活动接口
        public const string FestivalShareActivityUrl = "https://sxahgmt.ibluejoy.com/upload/sx_share.jpg";//节日活动分享图片
#endif
        #region WX登录
        public const string APP_ID = "wxf860bb512a154117";
#endregion WX

        #region YvWa登录
        public const uint YvWaApp_Id = 1001269;
        #endregion YvWa
        #region GVoice
        public const string GvoiceAPP_ID = "1183674726";
        public const string GvoiceAPP_Key = "dba21dd96f251e35ea9ade6d27832843";
        #endregion
    }

}
