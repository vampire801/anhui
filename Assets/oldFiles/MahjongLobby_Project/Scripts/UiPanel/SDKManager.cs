using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using System.Text;
using System.Collections.Generic;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.Runtime.InteropServices;
using YunvaIM;
using System.IO;
using zlib;
using UnityEngine.UI;
using JPush;
using UnityEngine.Networking;
using ExceptionDefind;
using Common;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    public class HandleCallBack
    {
        public delegate void handleCallBack(string str);
        public static handleCallBack loginSuccess;
    }
    [Hotfix]
    [LuaCallCSharp]
    public class SDKManager : MonoBehaviour
    {
        public static AndroidJavaClass sxJc;
        #region 实例化
        private TimerManager _timeManager = new TimerManager();
        static SDKManager instance;
        public static SDKManager Instance { get { return instance; } }
        public Text _t;
        void Awake()
        {
            instance = this;
            _timeManager.Tick(Time.deltaTime);
            Instance.gameObject.GetComponent<Consolation.TestConsole>()._isShowWind.gameObject.SetActive(true);
            gameObject.name = "SDKMgr";
            if (Application.platform == RuntimePlatform.Android)
            {
                return;
            }

            Screen.orientation = ScreenOrientation.LandscapeLeft ;
        }

        public void InitBuglySDK(string id)
        {
            BuglyAgent.ConfigDebugMode(true);
            BuglyAgent.ConfigDefault("channel:" + LobbyContants.iChannelVersion.ToString(), LobbyContants.version_typr + LobbyContants.version_v, id, 0);
            BuglyAgent.ConfigAutoReportLogLevel(LobbyContants.LogLevel);
            BuglyAgent.ConfigAutoQuitApplication(false);
            BuglyAgent.RegisterLogCallback(null);
            BuglyAgent.EnableExceptionHandler();
        }

        void Start()
        {
            InItAll();
        }
        private void InItAll()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                sxJc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            }
            InitBuglySDK(PlayerPrefs.HasKey("userId") ? "" + PlayerPrefs.GetInt("userId") : "nil");
            JPushBinding.Init(gameObject.name);
            JPushBinding.SetDebug(true);
            //JPushBinding.SetLatestNotificationNumber(4);
            registID = GetRegistID();
            if (Application.platform == RuntimePlatform.Android)
            {
                return;
            }
         
            //if (!string.IsNullOrEmpty(LobbyContants.version_typr))
            //{
            //    _t.text = LobbyContants.version_typr + LobbyContants.version_v;
            //}
            //else
            //{
            //    _t.text = "";
            //}
        }
        void Update()
        {
//#if IOS
//            if (Screen.orientation != ScreenOrientation.AutoRotation||
//               !Screen.autorotateToLandscapeLeft||!Screen.autorotateToLandscapeRight||
//               Screen.autorotateToPortrait|| Screen.autorotateToPortraitUpsideDown
//                )
//            {
//                Screen.autorotateToLandscapeLeft = true;
//                Screen.autorotateToLandscapeRight = true;
//                Screen.autorotateToPortrait = false;
//                Screen.autorotateToPortraitUpsideDown = false;
//                Screen.orientation = ScreenOrientation.AutoRotation;

//                }
//#endif

        }
        #endregion 实例化

        #region 消息定义
        public const string MESSAGE_SDKGETCODE = "SKDManager.MESSAGE_SDKGETCODE";

        #endregion 消息定义

        //[HideInInspector]
        public int CheckStatus = 0;  //1Android表示审核版本
        public int IOSCheckStaus = 0;//Ios审核
        public int IsConnectNetWork = 1;//1表示通过ip连接大厅服务器，2表示通过域名连接大厅服务器
                                        //局域网不可以使用域名连接（局域网没有域名）

        public int iShowGuestLogin=1; //0不显示游客登录  1显示游客登陆

        public bool isGetAccessTokenOver;
        public bool isGetRefreshTokenOver;
        /// <summary>
        /// 0-女 1-男 2女
        /// </summary>
        public int isex;
        public bool IsDisConnect;  //是否断开网络的标志位，主要是用于服务器重连问题

        public int isChangeUserInfo;//是否改变微信信息 2 更新 1不更新
        public const string szShareRoomIdUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxdf24ef79f5e6c4e4&redirect_uri=https%3A%2F%2Fsx.ibluejoy.com%2Fm%2FwxSpread.shtml&response_type=code&scope=snsapi_base&state={0}#wechat_redirect";
        //邀请地址
        public const string WXInviteUrl = "https://sx.ibluejoy.com/m/down.html?mj=";
        public const string szShareSongSheng = "https://sxver.ibluejoy.com/m/down3.html";

        public int ParlorId; //保存拉起app传入的麻将馆id
        //保存当前回调的位置信息
        public string sPosMessage_Curr;
        //玩家的最后一个显示的玩家面板
        public GameObject userInfoLast;
        #region 注册表字符串
        public string iUserId_iAuthType_ServerType = "iUserId_iAuthType_" + LobbyContants.SeverType;
        // public string szaccess_token = "access_token";
        public string szrefresh_token = "refresh_token";
        // public string szOpenid = "szOpenid";
        // public string szNickname = "szNickname";
        //public string szHeadimgurl = "szHeadimgurl";
        public string szLastday = "szLastDay";

        #endregion 注册表字符串

        public class HeadData
        {
            public List<WXWebElement.EleAccessToken> Listaccesstoken = new List<WXWebElement.EleAccessToken>();
            public List<WXWebElement.EleRefreshToken> Listrefreshtoken = new List<WXWebElement.EleRefreshToken>();
            public List<WXWebElement.EleUserInfo> Listuserinfo = new List<WXWebElement.EleUserInfo>();
        }

        #region 微信认证回调
        /// <summary>
        /// WxAndroid登录回调
        /// </summary>
        /// <param name="str"></param>

        public void OnGetmCodeCB(string str)
        {
            WXLoginPanelData ld = MahjongLobby_AH.GameData.Instance.WXLoginPanelData;
            ld.AuthonState = 0;
            if (str.CompareTo("cancel") == 0)//点击登录之后返回必执行
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("登录失败请重试");
                Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                GameData.Instance.WXLoginPanelData.isBtnOk = false;
                return;
            }

            Debug.LogWarning("回调unity方法成功");
            if (Application.platform == RuntimePlatform.Android)
            {
                if (string.IsNullOrEmpty(str))
                {
                    Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    MahjongCommonMethod.Instance.ShowRemindFrame("微信登录失败，请重试");
                    GameData.Instance.WXLoginPanelData.isBtnOk = false;
                }
                else
                {
                    SendAuthReqTyp1(str);
                }
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (string.IsNullOrEmpty(str))
                {
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    //MahjongCommonMethod.Instance.ShowRemindFrame("微信登录失败，请重新登录。"); 
                }
                else
                {
                    SendAuthReqTyp1(str);
                }
            }
            
            //GetAccessToken(strs[0]);
            //Messenger<string>.Broadcast(MESSAGE_SDKGETCODE,strs[0]);
        }

        public void OnAndroidDug(string str)
        {
            Debug.LogWarning(str);
        }

        public void OnAndroidShowNote(string str)
        {
            MahjongCommonMethod.Instance.ShowRemindFrame(str);
        }
        public void OnGetBatteryInfo(string num)
        {
            MahjongGame_AH.UIMainView.Instance.PlayerPlayingPanel._imageBattery.fillAmount = int.Parse(num) * 0.01f;
        }

        /// <summary>
        ///  红包分享通用方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="friendOr1"> 1朋友圈或者 0朋友 </param>
        /// <param name="RPType"> 是表中的那个类型 </param>
        /// <param name="ActivityType"></param>
        public void BtnShare(int friendOr1, int RPType, string imgUrl)
        {
            StartCoroutine(BtnShareForGetNetTimer(friendOr1, RPType, imgUrl));
        }

        /// <summary>
        /// 获取完网络事件之后再去分享
        /// </summary>
        /// <param name="url"></param>
        /// <param name="friendOr1"> 1朋友圈或者 0朋友 </param>
        /// <param name="RPType"> 是表中的那个类型 </param>
        /// <param name="ActivityType"></param>
        IEnumerator BtnShareForGetNetTimer(int friendOr1, int RPType, string imgUrl)
        {
            int count = 0;
            WWW www = new WWW(LobbyContants.WebTime);
            yield return www;
            int timer = 0;
            while (count <= 3)
            {
                if (www.text.Length >= 15)
                {
                    timer = Convert.ToInt32(www.text.Substring(2, 10));
                    break;
                }
                else
                {
                    count++;
                }
            }
            if (timer != 0)
            {
                string url = "", title = "", content = "";
                int RPID = 0;
                if (RPType == 4)
                {
                    RPID = MahjongCommonMethod.Instance.iUserid;
                }
                if (RPType == 3)
                {
                    RPID = UIMgr.GetInstance().ShowRedPagePanel.m_iRpid;
                }
                url = Instance.CheckStatus == 1 ? url = szShareSongSheng : string.Format(LobbyContants.ShareJsp, MahjongCommonMethod.Instance.iUserid, RPType, RPID, timer);
                Debug.LogWarning("————分享地址：" + url + ",timer = " + timer);
                CommonConfig.Json_ShareConfig js = new CommonConfig.Json_ShareConfig();
                string postUrl = IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;

                int shareNum = 1;
                switch (friendOr1)
                {
                    case 0: shareNum = 2; break;
                    case 1: shareNum = 1; break;
                }

                MahjongCommonMethod.Instance.GetUrlJson(string.Format(MahjongCommonMethod.shareParam, postUrl, RPType, shareNum, RPID), js,
                    (a, b) =>
                    {
                        if (a.status != 1)
                        {
                            MahjongCommonMethod.Instance.ShowRemindFrame("分享参数错误");
                            return;
                        }
                        js = a;
                        string county_Name;
                        if (MahjongCommonMethod.Instance._dicDisConfig.ContainsKey(GameData.Instance.SelectAreaPanelData.iCountyId))
                        {
                            county_Name = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
                        }
                        else
                        {
                            county_Name = "长治市";
                        }

                        string name = GameData.Instance.PlayerNodeDef.szNickname;

                        if (!string.IsNullOrEmpty(js.value))
                        {
                            content = js.value.Replace("XXS", county_Name).Replace("XXN", name);
                        }
                        if (!string.IsNullOrEmpty(js.value2))
                        {
                            title = js.value2.Replace("XXS", county_Name).Replace("XXN", name);
                        }
                        title = friendOr1 == 1 ? content : title;

                        HandleShareWX(url, title, content, friendOr1, RPType, 0, imgUrl);
                    });
            }
        }

        /// <summary>
        /// 分享状态--解决双微信问题
        /// </summary>
        public int shareState;
        /// <summary>
        /// 处理分享到微信消息的按钮事件
        /// </summary>
        /// <param name="url">分享地址</param>
        /// <param name="title">分享表头</param>
        /// <param name="discription">分享描述</param>
        /// <param name="friendOr1">type=0 分享好友 1->朋友圈 2->收藏</param>
        /// <param name="RPType">type= 红包ID 10-分享活动 13</param>
        /// <param name="ActivityType">活动ID</param>
        /// <param name="imgUrl">在RPType==4时表示图片url</param>
        public void HandleShareWX(string url, string title, string discription, int friendOr1, int RPType, int ActivityType, string imgUrl)
        {
            if (shareState == 0)
            {
                shareState = 1;
            }
            imgUrl = LobbyContants.ActivitePic + "fenxiang/sx_share_logo1.jpg";
            Debug.LogWarning("分享标题：" + title + "分享内容：" + discription);
            if (CheckStatus == 1)//如果是审核
            {
                url = "https://sxver.ibluejoy.com/m/down3.html";
            }
            if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 1)//如果是游客
            {
                if (GameData.Instance)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", false);
                }
                else
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", true);
                }
                return;
            }

            GameData.Instance.m_iFestivalActivity = RPType;
            iWitchShare = RPType;
            activityType = ActivityType;
#if UNITY_EDITOR
            MahjongCommonMethod.Instance.ShowRemindFrame("分享给朋友", false);
#elif UNITY_ANDROID 
            AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
            // swpd.szInNeedSendNumber = "110";//赋值网址传参
            Debug.LogWarning("imgUrl:"+imgUrl);
            jo.Call("SendToWXSceneSession",url, title, discription, friendOr1,imgUrl);//type=0 分享朋友 1->朋友圈 2->收藏
#endif
#if UNITY_IOS
            _WXOpenShareReq(url, title, discription, friendOr1);
#endif
          
        }

        /// <summary>
        /// 分享图片接口
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="url">二维码地址</param>
        /// <param name="type">type=0 分享朋友 1->朋友圈 2->收藏</param>
        /// <param name="mode">0-二进制，1图片</param>
        public void HandleShareImage(string url, int type, int mode, GameObject[] obj = null)
        {
            // Debug.LogError("HandleShareImage:" + type);
            StartCoroutine(LoadQRcodeAndDisplay(url, type, mode, obj));
        }

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="url">本地图片地址</param>
        /// <param name="type">type=0 分享朋友 1->朋友圈 2->收藏</param>
        /// <param name="mode">0-二进制，1图片</param>
        public void ShareImage(byte[] data, string url, int type, int mode, int avtivity = 0)
        {
            if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 1)//如果是游客
            {
                if (GameData.Instance)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", false);
                }
                else
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", true);
                }

            }
            else
            {
                GameData.Instance.m_iFestivalActivity = avtivity;
                iWitchShare = avtivity;
#if UNITY_EDITOR
                MahjongCommonMethod.Instance.ShowRemindFrame("分享给朋友", false);
#elif UNITY_ANDROID 
            AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
           // swpd.szInNeedSendNumber = "110";//赋值网址传参
            jo.Call("SendToWXShareImage",data, url, type, mode);//type=0 分享朋友 1->朋友圈 2->收藏
#endif
#if UNITY_IOS
                Debug.LogWarning ("传参到xcode ");
				_WXOpenShareImageReq(data, url, type, mode);
#endif
            }
        }

        public int iWitchShare;
        int activityType;
        [DllImport("__Internal")]
        private static extern void _WXOpenShareReq(string url, string title, string discription, int type);

        [DllImport("__Internal")]
        private static extern void _WXOpenShareImageReq(byte[] data, string url, int type, int mode);
        /// <summary>
        /// 分享成功回调
        /// </summary>
        /// <param name="str"></param>
        public void SharedSuccess(string str)
        {
            shareState = 0;
            if (str.CompareTo("UnShared") == 0)
            {
                Debug.Log("取消分享" + str);
                MahjongCommonMethod.Instance.ShowRemindFrame("取消分享");
                return;
            }
            MahjongCommonMethod.Instance.ShowRemindFrame("分享成功");
            Debug.LogWarning("分享成功回调" + iWitchShare + "," + GameData.Instance.m_iFestivalActivity + "," + str);
            iWitchShare = 0;
            // Debug.LogError("分享成功：" + str);
            int m_ifrom = 0;
            if (PlayerPrefs.HasKey("GameActivie" + GameData.Instance.PlayerNodeDef.iUserId))
            {
                m_ifrom = PlayerPrefs.GetInt("GameActivie" + GameData.Instance.PlayerNodeDef.iUserId);
            }
            Debug.LogWarning("m_ifrom = " + m_ifrom);
            if (m_ifrom == 0)
            {
                Debug.LogWarning("大厅分享");
                NetMsg.ClientShareSuccessReq msg = new NetMsg.ClientShareSuccessReq();
                msg.iUserID = MahjongCommonMethod.Instance.iUserid;
                msg.byShareType = (byte)GameData.Instance.m_iFestivalActivity;
                msg.iShareId = activityType;
                Debug.LogWarning("byShareType:" + GameData.Instance.m_iFestivalActivity);
                Debug.LogWarning("iShareId:" + msg.iShareId + "," + activityType);
                NetworkMgr.Instance.LobbyServer.SendShareSuccessReq(msg);
            }
            else
            {
                if (m_ifrom == 13 || m_ifrom == 14)
                {
                    MahjongGame_AH.UIMainView.Instance.GameResultPanel.RPButton.SetActive(false);
                    Debug.LogWarning("分享完成后关闭大赢家和炮王红包按钮" + m_ifrom);
                }
                Debug.LogWarning("游戏分享");
                MahjongGame_AH.Network.Message.NetMsg.ClientShareSuccessReq msg = new MahjongGame_AH.Network.Message.NetMsg.ClientShareSuccessReq();
                msg.iUserID = MahjongCommonMethod.Instance.iUserid;
                msg.byShareType = (byte)m_ifrom;
                Debug.LogWarning("byShareType:" + msg.byShareType);
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.SendShareSuccessReq(msg);
            }
            //隐藏分享按钮的引导
            if (NewPlayerGuide.Instance)
            {
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.ShareToWx_2);
            }
            if (GameData.Instance.m_iFestivalActivity == 20)
            {
                Debug.LogWarning("分享文字到朋友圈  文字");
                UIMainView.Instance.FestivalActivity.OnFenXiangLaterSend();
                return;
            }
            else
            {

            }

        }
        /// <summary>
        /// 邀请回调
        /// </summary>
        /// <param name="str"></param>
        public void OnGetParamCB(string str)
        {
            Debug.LogWarning("str:" + str);
            if (String.Equals(str, "0"))
            {
                return;
            }
            szCommonParam = str;
            string[] temp_value = str.Split('_');

            //处理麻将馆拉起回调
            if (temp_value.Length == 2)
            {
                ParlorId = Convert.ToInt32(temp_value[1]);

                if (GameData.Instance && GameData.Instance.PlayerNodeDef.iUserId > 0)
                {
                    GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 3);
                }
            }
            //处理其他拉起回调
            else
            {
                if (str.Length == 7)
                {
                    HandleCallBack.loginSuccess += instance.OpenReview;
                }
                else
                {
                    MahjongCommonMethod.Instance.RoomId = str;
                    HandleCallBack.loginSuccess += instance.JoinInToRoom;
                }
                if (GameData.Instance)
                {
                    LoginSuccess();
                }
            }
        }

        public string szCommonParam;//拉起APP回传参数
        void JoinInToRoom(string dk)
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            //保存进入的房间id
            MahjongCommonMethod.Instance.RoomId = dk;
            jrpd.PanelShow = false;
            SystemMgr.Instance.JoinRoomShowSystem.HandleOkBtn(Convert.ToInt32(dk));
            if (HandleCallBack.loginSuccess != null)
                HandleCallBack.loginSuccess -= instance.JoinInToRoom;

        }
        void OpenReview(string str)
        {
            UIMainView.Instance.HistroyGradePanel.BtnConfirmPlayBackCode(str);
            if (HandleCallBack.loginSuccess != null)
            {
                HandleCallBack.loginSuccess -= instance.OpenReview;
            }
        }
        #endregion 微信回调

        #region 极光注册ID回调



        public string GetRegistID()
        {
            string registID = "000";
            //Debug.LogWarning("获取注册ID");
            if (PlayerPrefs.HasKey(JIRegistionID) && !string.IsNullOrEmpty(PlayerPrefs.GetString(JIRegistionID)))
            {
                registID = PlayerPrefs.GetString(JIRegistionID);
                //Debug.LogError("保存起来的激光注册ID:" + registID);
                return PlayerPrefs.GetString(JIRegistionID);
            }
            else
            {
                registID = JPushBinding.GetRegistrationId();
                //Debug.LogError("SDK获取到的激光注册ID:" + registID);
                if (!string.IsNullOrEmpty(registID))
                {
                    PlayerPrefs.SetString(JIRegistionID, registID);
                }
                return registID;
            }
        }
        public string registID = "blue";

        string JIRegistionID = "JIRegistionID";
        //public void OnGotRegistID(string regID)
        //{
        //    if (!string.IsNullOrEmpty(regID))
        //    {
        //        registID = regID;
        //        PlayerPrefs.SetString(JIRegistionID, registID);
        //        Debug.LogError("极光注册ID：" + registID);
        //    }
        //}
        #endregion 极光注册ID回调

        #region 苹果充值回调
        /// <summary>
        /// 苹果充值回调
        /// </summary>
        /// <param name="result"></param>
        void OnPurchaseFinish(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                UIMainView.Instance.LobbyPanel.isOnPay = false;
                return;
            }
            Debug.Log("IOS回调unity方法成功");
            // Debug.Log("OnPurchaseFinish: " + result);
            //t = result;
            //f = true;
           // byte[] bytes = strToToHexByte(result);
            //if (bytes.Length > 8000)
            //{
           // Debug.LogWarning("压缩前的数组长度：" + bytes.Length);
            //}
           // byte[] ts = compressBytes(bytes);
            // byte[] ss = deCompressBytes(ts);
           // Debug.LogWarning("压缩后的数组长度：" + ts.Length);
            //string str = "";
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    str += bytes[i].ToString("X2");
            //}
            if (UIMainView.Instance.LobbyPanel.stackprice.Count == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("充值失败，请联系客服处理。");
                return;
            }
            NetMsg.ClientCreateOrderRes req1 = UIMainView.Instance.LobbyPanel.stackprice.Pop() as NetMsg.ClientCreateOrderRes;
           // NetMsg.ClientChargeReq msg = new NetMsg.ClientChargeReq(ts.Length, ts);
            NetMsg.ClientChargeReq msg = new NetMsg.ClientChargeReq();
            msg.iUserId = MahjongCommonMethod.Instance.iUserid;
            msg.iChargeId = req1.iChargeId;
            msg.iChargeMode = 1;
            //  Debug.LogWarning("msg.iChargeId:" + req1.iChargeId+"____"+MahjongCommonMethod.Instance._dicCharge.Count );
            msg.iChargeNumber = req1.iChargeNumber;
            msg.szOrderId = req1.szOrderId;
            msg.iBoss = req1.iBoss;
            msg.iUserVoucherId = req1.iUserVoucherId;
            msg.iVoucherAmount = req1.iVoucherAmount;
            Dictionary<string, string> dir = new Dictionary<string, string>();
            string url = IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL + "replyAppleOrder.x" : LobbyContants.MAJONG_PORT_URL_T + "replyAppleOrder.x";
            dir.Add("order_no", msg.szOrderId);
            dir.Add("receipt_data", result);
            MahjongCommonMethod.Instance.isPush = true;
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, dir, onReplayOrderRes, "", 5);
            Debug.LogWarning(" ichargeNumber " + msg.iChargeNumber + " iboss " + msg.iBoss + " iUserVoucherId " + msg.iUserVoucherId);
            //NetworkMgr.Instance.LobbyServer.SendChargeReq(msg);
        }
        void onReplayOrderRes(string json, int status)
        {
            PayResJoson ps = new PayResJoson();
            ps = JsonUtility.FromJson<PayResJoson>(json);
            if (ps.status == 1)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("充值成功!!");
                NetMsg.ClientRefreshUserReq req = new NetMsg.ClientRefreshUserReq();
                req.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                Network.NetworkMgr.Instance.LobbyServer.SendClientRefreshUserReq(req);
                UIMainView.Instance.LobbyPanel.BtnCloseBuyRoomCard();
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }
            else
            {
                Debug.Log(status);
                MahjongCommonMethod.Instance.ShowRemindFrame("充值失败 "+status );
            }
        }
        #endregion 苹果充值回调

        #region 支付宝充值回调
        //支付宝充值回调
        void OnAliPayResult(string result)
        {
            if (!string.IsNullOrEmpty (result ))
            {
                if (result.Length<=2)
                {
                    if (int.Parse(result) == 1)
                    {
                        if (UpdateCoinShow != null)
                        {
                            StopCoroutine(UpdateCoinShow);
                        }
                    }
                    MahjongCommonMethod.Instance.ShowRemindFrame("取消充值!");
                }
            }
            return;
            Debug.LogWarning("支付宝充值毁掉成功:" + result);
            string orderId = "";
            byte[] ts = new byte[1];
            string[] strS = result.Split('&');
            for (int i = 0; i < strS.Length; i++)
            {
                string[] tS = strS[i].Split('=');
                if (tS[0] == "out_trade_no")
                {
                    orderId = tS[1].Trim('"');
                    ts = strToByteArr(orderId);
                }
            }
            NetMsg.ClientChargeReq msg = new NetMsg.ClientChargeReq(ts.Length, ts);
            Debug.LogWarning("支付宝充值回调：" + UIMainView.Instance.LobbyPanel.stackprice.Count);
            if (UIMainView.Instance.LobbyPanel.stackprice.Count > 0)
            {
                NetMsg.ClientCreateOrderRes req1 = UIMainView.Instance.LobbyPanel.stackprice.Pop() as NetMsg.ClientCreateOrderRes;
                msg.iUserId = req1.iUserId;
                msg.iChargeId = req1.iChargeId;
                msg.iChargeMode = req1.iChargeMode;
                msg.iChargeNumber = req1.iChargeNumber;
                msg.iUserVoucherId = req1.iUserVoucherId;
                msg.iVoucherAmount = req1.iVoucherAmount;
                msg.iBoss = req1.iBoss;
                msg.szOrderId = orderId;
                // msg.iReceiptLength = ts.Length;
                msg.szReceipt = ts;
                Debug.LogWarning("支付宝充值回调SendChargeReq：" + msg.szOrderId);
                NetworkMgr.Instance.LobbyServer.SendChargeReq(msg);
            }
            else
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("充值失败，请联系客服处理。");
            }
        }
        byte[] GetBytesFromStr(string resource)
        {
            byte[] ts = new byte[1];
            string[] strS = resource.Split('&');
            for (int i = 0; i < strS.Length; i++)
            {
                string[] tS = strS[i].Split('=');
                if (tS[0] == "out_trade_no")
                {
                    Debug.LogWarning(strS[i]);
                    string result = tS[1].Trim('"');
                    Debug.LogWarning(result);
                    ts = strToByteArr(result);
                }
            }
            return ts;
        }
        #endregion 支付宝充值回调

        #region 微信支付回调
        void OnWXPayResult(string result)
        {
            Debug.LogWarning("微信充值回调：" + result);
            if (!string.IsNullOrEmpty (result))
            {
                if (int.Parse(result )!=0)
                {
                    if (UpdateCoinShow !=null)
                    {
                        StopCoroutine(UpdateCoinShow);
                    }
                }
                MahjongCommonMethod.Instance.ShowRemindFrame("取消充值!");
            }
            return;
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogWarning("微信充值1");
                if (int.Parse(result) == 0)
                {
                    Debug.LogWarning("微信充值1" + UIMainView.Instance.LobbyPanel.stackprice.Count);
                    if (UIMainView.Instance.LobbyPanel.stackprice.Count > 0)
                    {
                        NetMsg.ClientCreateOrderRes req1 = UIMainView.Instance.LobbyPanel.stackprice.Pop() as NetMsg.ClientCreateOrderRes;
                        byte[] st = strToByteArr(req1.szOrderId);
                        Debug.LogWarning(st.Length + "  req1.szOrderId:  " + req1.szOrderId);
                        NetMsg.ClientChargeReq msg = new NetMsg.ClientChargeReq(st.Length, st);
                        msg.iUserId = req1.iUserId;
                        msg.iChargeId = req1.iChargeId;
                        msg.iChargeMode = req1.iChargeMode;
                        msg.iChargeNumber = req1.iChargeNumber;
                        msg.iUserVoucherId = req1.iUserVoucherId;
                        msg.iVoucherAmount = req1.iVoucherAmount;
                        msg.szOrderId = req1.szOrderId;
                        msg.iBoss = req1.iBoss;
                        //msg.iReceiptLength = st.Length;
                        // msg.szReceipt = st;
                        Debug.LogWarning("微信充值SendChargeReq" + msg.szOrderId);
                        NetworkMgr.Instance.LobbyServer.SendChargeReq(msg);
                    }
                    else
                    {
                        MahjongCommonMethod.Instance.ShowRemindFrame("充值失败，请联系客服处理。");
                    }
                }
            }
        }
        #endregion 微信支付回调
        /// <summary>
        /// 通用网页充值接口
        /// </summary>
        /// <param name="msg"></param>
        public void CreatChargeOderReq(NetMsg.ClientCreateOrderReq msg)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("uid", msg.iUserId.ToString());
            dir.Add("money", msg.iChargeNumber.ToString());
            dir.Add("payType", msg.iChargeMode.ToString());
            dir.Add("boss", msg.iBoss.ToString());
            dir.Add("voucherId", msg.iUserVoucherId.ToString());
            dir.Add("chargeId", msg.iChargeId.ToString());
            dir.Add("sandbox", "0");
            string url = IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL + "createOrder.x" : LobbyContants.MAJONG_PORT_URL_T + "createOrder.x";
            string title = "";
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, dir, OnGetValue, title, 5);
        }
        void OnGetValue(string json, int status)
        {
            PayResJoson ps = new PayResJoson();
            ps = JsonUtility.FromJson<PayResJoson>(json);
            if (ps.status == 1)
            {
                if (UIMainView.Instance.LobbyPanel.stackprice.Count < 1)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("充值堆栈被清空..");
                    return;
                }
                NetMsg.ClientCreateOrderRes msg = UIMainView.Instance.LobbyPanel.stackprice.Peek() as NetMsg.ClientCreateOrderRes;
                msg.szOrderId = ps.order_no;
                msg.szOrderInfo = ps.payInfo;
                //苹果充值
                if (msg.iChargeMode == 1)
                {
#if UNITY_IOS && !UNITY_EDITOR
                InAppPurchasePluginiOS.UniIAPCharge(msg.iChargeId);
                return;
#endif
                }
                else if (msg.iChargeMode == 3)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.shuangxi.wxapi.WXEntryActivity");
                    AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                    jo.Call("AliPay", msg.szOrderInfo);

                }
                else if (msg.iChargeMode == 2)
                {
                   // Debug.LogError(msg.szOrderInfo);
                    AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.shuangxi.wxapi.WXEntryActivity");
                    AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                    jo.Call("WeChatPay", msg.szOrderInfo);
                }
                StartUpdateCoinChange();
            }
            else
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("网络连接异常,请检查您的网络环境!");
            }

        }
        [Serializable]
        public class PayResJoson
        {
            public int status;
            public string order_no;
            public string payInfo;
        }

        [Serializable]
        public class CreateOrderx
        {
            public string uid;//充值人编号
            public string money;///充值金额-
            public string payType;///支付方式 1苹果充值、2客户端微信、3客户端支付宝
            public string boss;/// 是否老板充值 0不是1是
            public string voucherId;///代金券ID
            public string chargeId;///充值项编号
            public string sandbox;///否苹果沙盒测试 0不是 1是,为空值则默认转为0
            public CreateOrderx() { }
            public CreateOrderx(string _uid, string _money, string _payType, string _boss, string _voucherId, string _chargeId, string _sandbox)
            {
                uid = _uid;
                money = _money;
                payType = _payType;
                boss = _boss;
                voucherId = _voucherId;
                chargeId = _chargeId;
                sandbox = _sandbox;
            }

        }
        public IEnumerator UpdateCoinShow = null;
        //开始刷新金币信息
        void StartUpdateCoinChange()
        {
            UpdateCoinShow = UpdateCoinChange();
            StartCoroutine(UpdateCoinShow);
        }
        IEnumerator UpdateCoinChange()
        {
            isChargeSuccess = false;
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(isChangeCoin());
            if (isChargeSuccess)
                yield break;
            int num = 0;
            while (true)
            {
                yield return StartCoroutine(isChangeCoin());
                if (isChargeSuccess)
                    yield break;
                num++;
                if (RuntimePlatform.IPhonePlayer == Application.platform)
                {
                    yield return new WaitForSeconds(10);
                }
                else
                {
                    yield return new WaitForSeconds(3);
                }
                if (num > 2)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("充值失败,请联系客服.");
                    yield break;
                }
            }
        }
        bool isChargeSuccess = false;
        public bool isUpdateCompelet = false;
        IEnumerator isChangeCoin()
        {
            isUpdateCompelet = false;
            int[] allcoin = GameData.Instance.PlayerNodeDef.userDef.iCoin;
            int lastCoin = 0;
            int currentCoin = 0;
            for (int i = 0; i < allcoin.Length; i++)
            {
                lastCoin += allcoin[i];
            }
            NetMsg.ClientRefreshUserReq req = new NetMsg.ClientRefreshUserReq();
            req.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            Network.NetworkMgr.Instance.LobbyServer.SendClientRefreshUserReq(req);
            yield return new WaitUntil(() =>
                isUpdateCompelet
            );
            for (int i = 0; i < allcoin.Length; i++)
            {
                currentCoin += GameData.Instance.PlayerNodeDef.userDef.iCoin[i];
            }
            if (currentCoin == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("充值确认中,请稍等...");
            }
            else if (currentCoin != lastCoin)
            {
                isChargeSuccess = true;
                MahjongCommonMethod.Instance.ShowRemindFrame("充值成功!!");
                UIMainView.Instance.LobbyPanel.BtnCloseBuyRoomCard();
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }
            else
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("充值确认中,请稍等...");
            }        

        }
        /// <summary>
        /// 刷新refreshtoken
        /// </summary>
        /// <param name="APP_ID"></param>
        /// <param name="REFTRSH_TOKEN"></param>
        /// 是否是首次登陆
        public void GetRefreshToken(string REFTRSH_TOKEN)
        {
            StartCoroutine(GetReTokenReq(REFTRSH_TOKEN));
        }
        //认证回应消息数据1:1,id2:10000105,type3:2,openid4:osde0wsHEHQMSy-zWb5VzqMrED1s
        public void GetUserInfo(string REFRESH_TOKEN, string OPENID)
        {
            StartCoroutine(IEnumeratorMag(REFRESH_TOKEN, OPENID));
        }

        IEnumerator IEnumeratorMag(string REFRESH_TOKEN, string OPENID)
        {
            yield return StartCoroutine(GetReTokenReq(REFRESH_TOKEN));
            if (isRefreshSuccess)
            {
                StartCoroutine(GetUserInfoReq(OPENID));
                isRefreshSuccess = false;
            }
        }
        bool isRefreshSuccess;
        /// <summary>
        /// 刷新ReToken
        /// </summary>
        /// <param name="REFRESH_TOKEN"></param>
        /// <returns></returns>
        IEnumerator GetReTokenReq(string REFRESH_TOKEN)
        {
            //  string retoken =Regex.Replace ( REFRESH_TOKEN,"\0","");
            string url;
            Debug.Log("开启线程2:" + REFRESH_TOKEN.Length);
            WXWebElement.EleRefreshToken elac = new WXWebElement.EleRefreshToken();
            url = string.Format(LobbyContants.WeChatRefreshUrl, LobbyContants.APP_ID, REFRESH_TOKEN);
            // url = "https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=wxa463ca68cf4717bd&grant_type=refresh_token&refresh_token=5_VsgcXqu3seGCpVFmg1HJ9KYOf_2rtEWN4Hse-biRiReMfnO-39hIq5SwGfqkvp8Hea1g960PdduOyr3b7n3HLJ8ArFKKvrfw3ssq6cj8lek";
            Debug.Log("刷新地址:" + url);
            // Debug.LogWarning ("================获取的elac.refresh_token" + elac.refresh_token);
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                elac = JsonUtility.FromJson<WXWebElement.EleRefreshToken>(www.downloadHandler.text);
                try
                {
                    if (www.isNetworkError)
                    {
                        Debug.LogError("www刷新token失败" + www.error + url);
                        throw (new ApplicationException("GetReTokenError:" + www.error + url));
                    }
                    if (elac.errcode != 0)
                    {
                        Debug.LogError("www刷新token失败" + www.downloadHandler.text + url);
                        throw (new ApplicationException("GetReTokenError:" + www.downloadHandler.text + url));
                    }
                }
                catch
                {
                    WXOpenAuthReq();
                    yield break;
                }
            }
            isRefreshSuccess = true;
            //保存accessToken
            PlayerPrefs.SetString(szrefresh_token + GameData.Instance.PlayerNodeDef.iUserId, elac.refresh_token);
            // PlayerPrefs.SetString(szaccess_token, headData.Listrefreshtoken[0].access_token);
            // PlayerPrefs.SetString(szOpenid, headData.Listrefreshtoken[0].openid);
            GameData.Instance.PlayerNodeDef.userDef.szOpenid = elac.openid;
            GameData.Instance.PlayerNodeDef.userDef.szAccessToken = elac.access_token;
            GameData.Instance.PlayerNodeDef.userDef.szRefreshToken = elac.refresh_token;
            //    Debug.LogError("acc:" + headData.Listrefreshtoken[0].access_token);
            //    Debug.LogError("ref:" + headData.Listrefreshtoken[0].refresh_token);

            isGetRefreshTokenOver = true;
            if (GameData.Instance.PlayerNodeDef.isSencondTime == 2)
            {
                SendAuthReqTyp2();//刷新完认证2登录
            }
            if (GameData.Instance.PlayerNodeDef.isAuthSuccess)
            {
                instance.LoginSuccess();
            }
        }
        IEnumerator GetUserInfoReq(string OPENID)
        {
            Debug.Log("请求用户信息网址" + string.Format(LobbyContants.WcChatUserInfUrl, GameData.Instance.PlayerNodeDef.userDef.szAccessToken, OPENID));
            string url = string.Format(LobbyContants.WcChatUserInfUrl, GameData.Instance.PlayerNodeDef.userDef.szAccessToken, OPENID);
            WXWebElement.EleUserInfo elac = new WXWebElement.EleUserInfo();
            using (UnityWebRequest wwf = UnityWebRequest.Get(url))
            {
                yield return wwf.SendWebRequest();
                try
                {
                    if (wwf.isNetworkError)
                    {
                        Debug.LogError("获取用户信息错误：" + wwf.error);
                        throw (new ApplicationException("GetUserInfoError0：" + wwf.error + url));
                    }
                    elac = JsonUtility.FromJson<WXWebElement.EleUserInfo>(wwf.downloadHandler.text);
                    if (elac.errcode != 0)
                    {
                        Debug.LogError("www刷新token失败" + wwf.downloadHandler.text);
                        throw (new ApplicationException("GetUserInfoError0：" + wwf.downloadHandler.text + url));
                    }
                }
                catch
                {
                    WXOpenAuthReq();
                    yield break;
                }
            }
            //  GameData.Instance.PlayerNodeDef.wxElement.usinfo = elac;//保存到内存
            //规范化用户信息字符
            string strLocalHead = GameData.Instance.PlayerNodeDef.szHeadimgurl;
            string strLocalName = GameData.Instance.PlayerNodeDef.szNickname;
            int iLocalsex = GameData.Instance.PlayerNodeDef.bySex;
            string strloadHead = elac.headimgurl.Trim();
            string strLoadName = elac.nickname.Trim();
            if (elac.sex == 1)
            {
                isex = 1;
            }
            else
            {
                isex = 0;
            }
            if (string.Compare(strLocalHead.ToLowerInvariant(), strloadHead.ToLowerInvariant()) != 0 || string.Compare(strLocalName.ToLowerInvariant(), strLoadName.ToLowerInvariant()) != 0
                || iLocalsex != isex)
            {
                //  Debug.LogError("头像变了");
                GameData.Instance.PlayerNodeDef.szNickname = strLoadName;
                GameData.Instance.PlayerNodeDef.szHeadimgurl = strloadHead;
                GameData.Instance.PlayerNodeDef.bySex = (byte)isex;
                //发送修改用户信息请求
                NetMsg.ClientChangeUserInfoReq msg = new NetMsg.ClientChangeUserInfoReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.szNickname = GameData.Instance.PlayerNodeDef.szNickname;
                msg.cSex = (sbyte)GameData.Instance.PlayerNodeDef.bySex;
                msg.szHeadimgurl = GameData.Instance.PlayerNodeDef.szHeadimgurl;
                Regex reg = new Regex(@"http\b");
                if (!reg.IsMatch(msg.szHeadimgurl))
                {
                    msg.szHeadimgurl = "no_head_img_url";
                    Debug.LogError("未能正常获取头像");
                }
                NetworkMgr.Instance.LobbyServer.SendChangeUserInfo(msg);
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }
            //}  
        }





        public void GetIP(Action action)
        {
            string url = LobbyContants.GetIPWeb;
            StartCoroutine(GetIp_Web(url, action));
        }



        //获取玩家的IP地址
        IEnumerator GetIp_Web(string url, Action action)
        {
            //#if UNITY_IOS
            //            //使用webclient 请求网页数据
            //            System.Net.WebClient MyWebClient = new System.Net.WebClient();
            //            yield return MyWebClient;

            //            MyWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。        
            //            byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据               
            //            PlayerIp = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句         
            //            MahjongCommonMethod.sIp = PlayerIp;
            //#elif UNITY_ANDROID || UNITY_EDITOR         
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log("GetIp_Web数据错误:" + www.error);
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        AndroidJavaObject jo = sxJc.CallStatic<AndroidJavaObject>("GetActivity");
                        MahjongCommonMethod.PlayerIp = jo.Call<string>("getLocalIpAddress");
                    }
                }
                else
                {
                    MahjongCommonMethod.PlayerIp = www.downloadHandler.text;
                    action();
                }
            }
            //  www = new WWW(url);
            // WWW www;

            //Debug.LogError("PlayerIp:" + PlayerIp);
            //#endif
        }


        //拉起登录认证
        public void WXOpenAuthReq()
        {
            WXLoginPanelData ld = GameData.Instance.WXLoginPanelData;
            ld.AuthonState = 1;
            if (PlayerPrefs.HasKey(instance.szLastday + GameData.Instance.PlayerNodeDef.iUserId + MahjongCommonMethod.EVERYDAY.HEADIMG_UPDATE))
            {
                MahjongCommonMethod.Instance.DeletPlayerPrefs(MahjongCommonMethod.EVERYDAY.HEADIMG_UPDATE);
            }
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("编辑器不能拉起认证登录");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                Debug.LogWarning("拉起微信认证");
                AndroidJavaObject jo = sxJc.CallStatic<AndroidJavaObject>("GetActivity");
                jo.Call("Auth_wx");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _WXOpenAuthReq();
            }
        }

        [DllImport("__Internal")]
        private static extern void _WXOpenAuthReq();

        //登录成功进入游戏
        public void LoginSuccess()
        {
            GameData gd = GameData.Instance;
            gd.WXLoginPanelData.isPanelShow = false;
            SystemMgr.Instance.WXLoginSystem.UpdateShow();
            if (HandleCallBack.loginSuccess != null)
            {
                HandleCallBack.loginSuccess(instance.szCommonParam); return;
            }
            Debug.LogWarning("信息,城市:" + gd.PlayerNodeDef.iCityId + ",县：" + gd.PlayerNodeDef.iCountyId);
            gd.LobbyMainPanelData.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }
        public int _iUserid = 0;
        /// <summary>
        /// 2方式认证
        /// </summary>
        public void SendAuthReqTyp2()
        {
            NetMsg.ClientAuthenReq msg = new NetMsg.ClientAuthenReq();
            msg.wVer = LobbyContants.SeverVersion;
            msg.iAuthenType = 2;// Convert.ToInt32 (strs[1]);
            MahjongCommonMethod.iAutnehType = 2;
            msg.szToken = PlayerPrefs.GetString(Instance.szrefresh_token + GameData.Instance.PlayerNodeDef.iUserId);   //
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            msg.szIp = MahjongCommonMethod.PlayerIp;
            //   Instance.GetIP(() => {  });
            //  Debug.LogWarning("6设备IP：" + msg.szIp);

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                msg.iUserId = _iUserid;
            }
            else
            {
                string[] strs = PlayerPrefs.GetString(Instance.iUserId_iAuthType_ServerType + GameData.Instance.PlayerNodeDef.iUserId).Split('_');
                msg.iUserId = Convert.ToInt32(strs[0]);

            }
            Debug.LogWarning("认证2准备发送认证消息");
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
            msg.REGISTRATION_ID = instance.GetRegistID();
            NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);
        }
        //模式1认证登录
        public void SendAuthReqTyp1(string sztoken)
        {
            NetMsg.ClientAuthenReq msg = new NetMsg.ClientAuthenReq();
            msg.wVer = LobbyContants.SeverVersion;
            msg.iAuthenType = 1;
            MahjongCommonMethod.iAutnehType = 1;
            msg.szToken = sztoken;//发送的code
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            msg.szIp = MahjongCommonMethod.PlayerIp;
            // Instance.GetIP(() => {  });
            //  Debug.LogWarning("6设备IP：" + msg.szIp);

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
            Debug.LogWarning("认证1准备发送消息");
            msg.iRegistSource = LobbyContants.iChannelVersion;
            msg.szRegistMac = MahjongCommonMethod.Instance.MacId;
            msg.REGISTRATION_ID = GetRegistID();
            NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);

        }

        //base64字符串转换成字节数组
        private byte[] strToToHexByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            returnBytes = Convert.FromBase64String(hexString);
            return returnBytes;
        }
        private byte[] strToHexByte(string hexString)
        {
            //   byte[] returnBytes = new byte[hexString.Length / 2];
            // byte[] returnBytes = Encoding.ASCII.GetBytes(hexString);
            byte[] returnBytes = Encoding.Default.GetBytes(hexString);
            return returnBytes;
        }
        //字符串转换成字节数组
        private byte[] strToByteArr(string rStr)
        {
            byte[] result = new byte[rStr.Length];
            char[] cS = rStr.ToCharArray();
            for (int i = 0; i < cS.Length; i++)
            {
                result[i] = Convert.ToByte(cS[i]);
            }
            return result;
        }
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="sourceByte">需要被压缩的字节数组</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] compressBytes(byte[] sourceByte)
        {
            MemoryStream inputStream = new MemoryStream(sourceByte);
            Stream outStream = compressStream(inputStream);
            byte[] outPutByteArray = new byte[outStream.Length];
            outStream.Position = 0;
            outStream.Read(outPutByteArray, 0, outPutByteArray.Length);
            outStream.Close();
            inputStream.Close();
            return outPutByteArray;
        }
        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="sourceStream">需要被压缩的流</param>
        /// <returns>压缩后的流</returns>
        public static Stream compressStream(Stream sourceStream)
        {
            MemoryStream streamOut = new MemoryStream();
            ZOutputStream streamZOut = new ZOutputStream(streamOut, zlibConst.Z_DEFAULT_COMPRESSION);
            CopyStream(sourceStream, streamZOut);
            streamZOut.finish();
            return streamOut;
        }
        /// <summary>
        /// 解压缩流
        /// </summary>
        /// <param name="sourceStream">需要被解压缩的流</param>
        /// <returns>解压后的流</returns>
        public static Stream deCompressStream(Stream sourceStream)
        {
            MemoryStream outStream = new MemoryStream();
            ZOutputStream outZStream = new ZOutputStream(outStream);
            CopyStream(sourceStream, outZStream);
            outZStream.finish();
            return outStream;
        }
        /// <summary>
        /// 解压缩流
        /// </summary>
        /// <param name="sourceByte">需要被压缩的字节数组</param>
        /// <returns>压缩后的字节数组</returns>
        public static string DecompressBytes(byte[] sourceByte)
        {
            MemoryStream inputStream = new MemoryStream(sourceByte);
            Stream outStream = deCompressStream(inputStream);
            byte[] outPutByteArray = new byte[outStream.Length];
            outStream.Position = 0;
            outStream.Read(outPutByteArray, 0, outPutByteArray.Length);
            outStream.Close();
            inputStream.Close();
            return Encoding.Default.GetString(outPutByteArray);
        }
        /// <summary>
        /// 复制流
        /// </summary>
        /// <param name="input">原始流</param>
        /// <param name="output">目标流</param>
        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len = 0;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
        Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {

            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }
        //   bool hasDownLoadQRcode;
        //下载分享二维码并截图分享二维码
        public IEnumerator LoadQRcodeAndDisplay(string url, int type, int mode, GameObject[] obj)
        {
            int xx = 0;
            Texture2D texture2D = null;
            while (xx++ < 4)
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    yield return www.SendWebRequest();
                    if (www.isNetworkError)
                    {
                        yield return new WaitForSeconds(1);
                        continue;
                    }
                    // int width = (int)MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.uvRect.width;
                    //  int height = (int)MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.uvRect.height;
                    // texture2D = ScaleTexture(DownloadHandlerTexture.GetContent(www), width, height);
                    texture2D = DownloadHandlerTexture.GetContent(www);

                    Debug.Log("复制图片");
                    //  MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.material.mainTexture = texture2D;
                    //  MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.enabled = true;
                    if (texture2D != null)
                    {
                        //     hasDownLoadQRcode = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            //MahjongCommonMethod.Instance.ShowRemindFrame("网络不稳定哦");
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(false);
            }
            StartCoroutine(GenerateShareImage(texture2D, type, mode, obj));

        }
        // public Image _imagePic;
        IEnumerator GenerateShareImage(Texture2D _qRcode, int type, int mode, GameObject[] obj)
        {
            yield return new WaitForEndOfFrame();
            string filename = Application.persistentDataPath + "/NewScreenshot.jpg";
            string newPath = filename;

#if !UNITY_EDITOR
            newPath = filename.Replace(Application.persistentDataPath + "/", "");
#endif
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            Rect rect = Camera.main.pixelRect;
            Texture2D tex0 = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            tex0.ReadPixels(rect, 0, 0);
            tex0.Apply();
            //取消显示二维码
            //二维码重设长宽
            int code_width = (int)MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.rectTransform.sizeDelta.x;
            int code_height = (int)MahjongGame_AH.UIMainView.Instance.GameResultPanel._rawImage.rectTransform.sizeDelta.y;
            Debug.LogWarningFormat("重设后的长:{0}高：{1}", code_width, code_height);
            int totalPixels = tex0.width * tex0.height;
            int x_limit = tex0.width - code_width + 1;
            try
            {
                for (int y = 0; y < tex0.height; y++)
                {
                    for (int x = 0; x < tex0.width; x++)
                    {
                        if (y <= code_height && x >= x_limit)
                        {
                            tex0.SetPixel(x, y, MahjongCommonMethod.AlphaBlend(tex0.GetPixel(x, y), _qRcode.GetPixelBilinear((float)(x - x_limit) / code_width, (float)(y - code_height) / code_height)));
                        }
                    }
                }
            }
            catch
            {
                throw (new ApplicationException("像素下标:" + totalPixels));
            }
            byte[] bytes = tex0.EncodeToPNG();
            FileStream file = File.Open(filename, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Flush();
            file.Close();
            DestroyImmediate(tex0);
            MahjongGame_AH.UIMainView.Instance.GameResultPanel.ShowContent[2].gameObject.SetActive(false);
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(true);
            }
            float time = Time.time;
            bool b = false;
            yield return new WaitUntil(() =>
            {
                b = System.IO.File.Exists(filename);
                return b || ((Time.time - time) > 1f);
            });
            if (b == false)
            {
                Debug.LogError("截屏出错！");
                yield break;
            }

            //这里分享到微信0
            ShareImage(bytes, filename, type, mode);
        }

        byte[] CaptureCamera(RawImage _qRcode, Rect rect, Camera MainCamera, Camera camera2)
        {

            Debug.LogWarning("Canvas_____Enable");
            // 创建一个RenderTexture对象  
            _qRcode.enabled = true;
            ScreenCapture.CaptureScreenshot("NewScreenshot.jpg");
            //   byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/NewScreenshot.jpg");
            RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
            MainCamera.targetTexture = rt;
            MainCamera.Render();
            //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
            camera2.targetTexture = rt;
            camera2.Render();
            //ps: -------------------------------------------------------------------

            // 激活这个rt, 并从中中读取像素。  
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
            screenShot.Apply();
            _qRcode.enabled = false;
            // 重置相关参数，以使用camera继续在屏幕上显示  
            MainCamera.targetTexture = null;
            camera2.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors  
            GameObject.Destroy(rt);
            Debug.LogWarning("截图完成,返回Texture");
            return null;
        }


        /// <summary>
        /// 获取玩家的经纬度
        /// </summary>
        /// <param name="content"></param>
        public void GetLonglatitude(string content)
        {
            string[] longlat = content.Split('_');
            MahjongCommonMethod.Instance.fLongitude = (float)Convert.ToDouble(longlat[0]);
            MahjongCommonMethod.Instance.fLatitude = (float)Convert.ToDouble(longlat[1]);
            Debug.LogWarning("content:" + content);
        }

        /// <summary>
        /// 玩家是否是模拟器
        /// </summary>
        /// <param name="status"></param>
        public void GetPlayerIsMoniQi(string status)
        {
            if (Convert.ToInt16(status) == 1)
            {
                MahjongCommonMethod.Instance.isMoNiQi = true;
                Debug.LogWarning("玩家是模拟器");
            }
            else
            {
                MahjongCommonMethod.Instance.isMoNiQi = false;
                Debug.LogWarning("玩家不是模拟器");
            }
        }


        /// <summary>
        /// 获取玩家的位置信息
        /// </summary>
        /// <param name="posMessage"></param>
        public void GetPlayerPosMessage(string posMessage)
        {
            Debug.LogWarning("玩家位置信息:" + posMessage);
            MahjongCommonMethod.Instance.sPlayerAddress = posMessage;
            //sPosMessage_Curr = posMessage;
            //userInfoLast.GetComponent<UserInfo>().sPosMessage = posMessage;
            //userInfoLast.GetComponent<UserInfo>().ShowPlayPosMessage();
        }
        /// <summary>
        /// 取得数据存放目录
        /// </summary>
        public static string DataPath
        {
            get
            {
                string game = LobbyContants.AppName.ToLower();
                if (Application.isMobilePlatform)
                {
                    return Application.persistentDataPath + "/" + game + "/";
                }
                if (LobbyContants.DebugMode)
                {
                    return Application.dataPath + "/" + LobbyContants.AssetDir + "/";
                }
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    int i = Application.dataPath.LastIndexOf('/');
                    return Application.dataPath.Substring(0, i + 1) + game + "/";
                }
                return "c:/" + game + "/";
            }
        }

        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppContentPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.dataPath + "/" + LobbyContants.AssetDir + "/";
                    break;
            }
            return path;
        }
       
    }

}

