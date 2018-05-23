using UnityEngine;
using System.Collections.Generic;
using Common;
using MahjongLobby_AH.UISystem;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class UIMainView : BaseUI
    {
        #region 实例

        static UIMainView instance;
        public static UIMainView Instance
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// 微信登录面板
        /// </summary>
        public MainViewWXLoginPanel WXLoginPanel;

        /// <summary>
        /// 实名认证的面板
        /// </summary>
        public MainViewRealNameApprovePanel RealNameApprovePanel;

        /// <summary>
        /// 大厅主面板
        /// </summary>
        public MainViewLobbyPanel LobbyPanel;

        /// <summary>
        /// 分享面板
        /// </summary>
        public MainViewShareWxPanel ShareWxPanel;

        /// <summary>
        /// 领取礼包面板
        /// </summary>
        public MainViewGetGiftBagPanel GetGiftBagPanel;

        /// <summary>
        /// 玩家头像信息面板
        /// </summary>
        public MainViewUserInfoPanel UserInfoPanel;

        /// <summary>
        /// 查看游戏玩法面板
        /// </summary>
        public MainViewGamePlayingMethodPanel GamePlayingMethodPanel;
        //MainViewGamePlayingMethodPanel gameplayingmethodpanel;
        //[HideInInspector]
        //public MainViewGamePlayingMethodPanel GamePlayingMethodPanel
        //{
        //    get
        //    {
        //        if (gameplayingmethodpanel == null)
        //        {
        //            GameObject instance = Instantiate(GamePlayingMethodGameobject) as GameObject;
        //            instance.transform.SetParent(this.transform);
        //            instance.transform.localScale = new Vector3(1, 1, 1);
        //            instance.transform.localRotation = Quaternion.identity;
        //            instance.transform.localPosition = Vector3.zero;
        //            instance.name = "GamePlayingMethodPanel";
        //            gameplayingmethodpanel = instance.GetComponent<MainViewGamePlayingMethodPanel>();
        //        }
        //        return gameplayingmethodpanel;
        //    }
        //}

        /// <summary>
        /// 推广界面
        /// </summary>
        public MainViewProductAgencyPanel ProductAgencyPanel;

        /// <summary>
        /// 反馈面板
        /// </summary>
        public MainViewCustomSeverPanel CustomSeverPanel;

        /// <summary>
        /// 玩家消息面板
        /// </summary>
        public MainViewPlayerMessagePanel PlayerMessagePanel;

        /// <summary>
        /// 活动面板
        /// </summary>
      //  public MainViewActivityPanel ActivityPanel;
        /// <summary>
        /// 激活码面板
        /// </summary>
        public MainViewAcitveGiftPanel ActiveGiftPanel;

        /// <summary>
        /// 创建房间的信息面板
        /// </summary>
        public MianViewCreatRoomMessagePanel CreatRoomMessagePanel;
        /// <summary>
        /// 活动面板
        /// </summary>
        public MainViewHolidayActivityPanel HolidayActivityPanel;
        /// <summary>
        /// 加入房间的显示面板
        /// </summary>
        public MainViewJoinRoomShowPanel JoinRoomShowPanel;

        /// <summary>
        /// 代开房间的显示面板
        /// </summary>
        public MainViewInsteadOpenRoomPanel InsteadOpenRoomPanel;

        /// <summary>
        /// 选择区域的显示面板
        /// </summary>
        public MainViewSelectAreaPanel SelectAreaPanel;

        /// <summary>
        /// 历史战绩的显示面板
        /// </summary>
        public MainViewHistroyGradePanel HistroyGradePanel;

        /// <summary>
        /// 活动 的显示 面板
        /// </summary>
        public FestivalActivityPanelData FestivalActivity;

        /// <summary>
        /// 麻将馆的面板
        /// </summary>
        public MainViewParlorShowPanel ParlorShowPanel;
        /// <summary>
        /// 断开连接的面板
        /// </summary>
        public DisConnect disConnect;

        /// <summary>
        /// 查看预约桌界面
        /// </summary>
        public MainViewReservationSeatPanel ReservationSeatPanel;

        /// <summary>
        /// 红包界面
        /// </summary>
        public MainViewRedPagePanel RedPagePanel;

        protected override void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

                transform.GetComponent<Canvas>().worldCamera = Camera.main;
            

            GameData gd = GameData.Instance;
            //决定开启微信认证登录还是，直接打开大厅面板进行认证请求
            //Debug.LogError("isGameToLobby:" + MahjongCommonMethod.isGameToLobby);
            if (MahjongCommonMethod.isGameToLobby)
            {
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
              //  SDKManager.Instance.GetIP(() => { });
              //  Debug.LogWarning("5设备IP：" + msg.szIp);

                msg.REGISTRATION_ID = SDKManager.Instance.GetRegistID();
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
                if (!Network.NetworkMgr.Instance.LobbyServer.Connected)
                {
                    MahjongCommonMethod.Instance.isLobbySendAuthenReq = true;
                }
                msg.iRegistSource = LobbyContants.iChannelVersion;
                msg.szRegistMac = MahjongCommonMethod.Instance.MacId;
                Network.NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);
            }
        }
        #endregion 实例

        #region 父类方法

        #endregion 父类方法
        /// <summary>
        /// 获取UI面板的编号
        /// </summary>
        /// <returns></returns>
        public override ushort GetID()
        {
            return UIConstant.UIID_MAHJONG_LOBBY_MAIN_PANEL;
        }
        /// <summary>
        /// 当试图已经显示，注册事件
        /// </summary>
        protected override void ViewDidAppear()
        {
            SystemMgr.Instance.WXLoginSystem.OnGetWXLoginUpdate += OnWxLoginPanelUpdateShow;//收到微信登录面板更新显示
            SystemMgr.Instance.RealNameApproveSystem.OnRealNameApprovePanelUpdate += OnRealNameApprovePanelUpdateShow;//收到实名认证面板更新显示
            SystemMgr.Instance.LobbyMainSystem.OnLobbyMainUpdate += OnLobbyPanelUpdateShow;  //收到大厅主面板的更新显示
            SystemMgr.Instance.ShareWxSystem.OnShareWxUpdate += OnshareWxPanelUpdateShow;  //收到分享面板的更新显示
            SystemMgr.Instance.GetGiftSpreadBagSystem.OnGetGiftBagUpdate += OnGetGiftBagPanelUpdateShow;  //收到领取礼包面板的更新显示
            SystemMgr.Instance.UserInfoSystem.OnUserInfoUpdate += OnUserInfoPanelUpdateShow;//收到用户信息更新显示
            SystemMgr.Instance.GamePlayingMethodSystem.OnGamePlayingMethodUpdate += OnamePlayingMethodPanelUpdateShow;   //收到查看玩法的面板的更新显示
            SystemMgr.Instance.ProductAgencySystem.OnProductAgencyUpdate += OnProductAgencyPanelUpdateShow;  //收到推广界面的更新通知
            SystemMgr.Instance.CustomSystem.OnBugSubmitUpdate += OnBugSubmitPanelUpdateShow;  //收到bug反馈界面的更新通知
            SystemMgr.Instance.PlayerMessageSystem.OnPlayerMessageUpdate += OnPlayerMessagePanelUpdateShow;  //收到玩家消息面板的通知
            SystemMgr.Instance.PlayerMessageSystem.OnPlayerMessageUpdate += OnPlayerMessagePanelUpdateShow;  //收到玩家消息面板的通知
            // SystemMgr.Instance.ActivitySystem.OnActivitySystemUpdate += OnActivityPanelUpdateShow; //收到活动面板更新的通知    
            SystemMgr.Instance.FestivalActivitySystem.OnActivityUpdateShow += OnFestivalActivityUpdateShow;  //收到玩家消息面板的通知
            SystemMgr.Instance.ActiveGiftSystem.OnActiveGiftSystemUpdate += OnActiveGiftPanelUpdateShow; //收到激活吗面板更新的通知    
            SystemMgr.Instance.CreatRoomMessageSystem.OnCreatRoomMessageUpdateShow += OnCreatRoomMessagePanelUpdateShow;  //收到创建房间面板的通知
            SystemMgr.Instance.CreatRoomMessageSystem.OnCreatRoomMethodDataUpdateShow += OnCreatRoomMessagePanelDataUpdateShow;  //收到更新创建房间面板玩法的通知
            SystemMgr.Instance.JoinRoomShowSystem.OnJoinRoomShowUpdate += OnJoinRoomShowPanelUpdateShow;   //收到加入房间面板更新的通知
            SystemMgr.Instance.InsteadOpenRoomSystem.InsteadOpenRoomPanelUpdate += OnInsteadOpenRoomPanelUpdateShow;  //收到代开面板更新的通知
            SystemMgr.Instance.SelectAreaSystem.SelectAreaSystemUpdate += OnSelectAreaPanelUpdateShow;  //收到选择面板更新
            SystemMgr.Instance.HistroyGradeSystem.HistroyGradeSystemUpdate += OnHistroyGradePanelUpdateShow;  //收到玩家历史玩家战绩的面板更新 
            SystemMgr.Instance.HolidayActivitySystem.OnHolidayActivityUpdateShow += OnHolidayActivityPanelUpdateShow;//收到活动面板更新
            SystemMgr.Instance.HolidayActivitySystem.OnTheHolidayActivityUpdate += OnTheHolidayActivityUpdateShow;//收到活动面板更新
            SystemMgr.Instance.ParlorShowSystem.OnParlorShowUpdate += OnParlorShowUpdateShow;  //麻将馆的更新通知
            SystemMgr.Instance.ParlorShowSystem.OnMyParlorPanelUpdate += OnMyParlorUpdate;  //我的麻将馆的更新通知
            SystemMgr.Instance.ParlorShowSystem.OnParlorRedBagUpdate += OnParlorRedBagUpdate; //麻将馆红包信息更新
            SystemMgr.Instance.RedPageShowSystem.OnRedPageUpdate += OnRedPageUpdateShow;   //收到红包面板更新
        }

        /// <summary>
        /// 当视图即将隐藏，注销事件
        /// </summary>
        protected override void ViewWillDisappear()
        {
            SystemMgr.Instance.WXLoginSystem.OnGetWXLoginUpdate -= OnWxLoginPanelUpdateShow;//收到微信登录面板更新显示
            SystemMgr.Instance.RealNameApproveSystem.OnRealNameApprovePanelUpdate -= OnRealNameApprovePanelUpdateShow;//收到实名认证面板更新显示
            SystemMgr.Instance.LobbyMainSystem.OnLobbyMainUpdate -= OnLobbyPanelUpdateShow;  //收到大厅主面板的更新显示
            SystemMgr.Instance.ShareWxSystem.OnShareWxUpdate -= OnshareWxPanelUpdateShow;  //收到分享面板的更新显示
            SystemMgr.Instance.GetGiftSpreadBagSystem.OnGetGiftBagUpdate -= OnGetGiftBagPanelUpdateShow;  //收到领取礼包面板的更新显示
            SystemMgr.Instance.UserInfoSystem.OnUserInfoUpdate -= OnUserInfoPanelUpdateShow;//用户信息面板
            SystemMgr.Instance.GamePlayingMethodSystem.OnGamePlayingMethodUpdate -= OnamePlayingMethodPanelUpdateShow;   //收到查看玩法的面板的更新显示
            SystemMgr.Instance.ProductAgencySystem.OnProductAgencyUpdate -= OnProductAgencyPanelUpdateShow;  //收到推广界面的更新通知
            SystemMgr.Instance.CustomSystem.OnBugSubmitUpdate -= OnBugSubmitPanelUpdateShow;  //收到bug反馈界面的更新通知
            SystemMgr.Instance.PlayerMessageSystem.OnPlayerMessageUpdate -= OnPlayerMessagePanelUpdateShow;  //收到玩家消息面板的通知
                                                                                                             // SystemMgr.Instance.ActivitySystem.OnActivitySystemUpdate -= OnActivityPanelUpdateShow; //收到活动面板更新的通知
            SystemMgr.Instance.CreatRoomMessageSystem.OnCreatRoomMessageUpdateShow -= OnCreatRoomMessagePanelUpdateShow;  //收到创建房间面板的通知
            SystemMgr.Instance.CreatRoomMessageSystem.OnCreatRoomMethodDataUpdateShow -= OnCreatRoomMessagePanelDataUpdateShow;  //收到更新创建房间面板玩法的通知
            SystemMgr.Instance.JoinRoomShowSystem.OnJoinRoomShowUpdate -= OnJoinRoomShowPanelUpdateShow;   //收到加入房间面板更新的通知
            SystemMgr.Instance.InsteadOpenRoomSystem.InsteadOpenRoomPanelUpdate -= OnInsteadOpenRoomPanelUpdateShow;  //收到代开面板更新的通知
            SystemMgr.Instance.SelectAreaSystem.SelectAreaSystemUpdate -= OnSelectAreaPanelUpdateShow;  //收到选择面板更新
            SystemMgr.Instance.HistroyGradeSystem.HistroyGradeSystemUpdate -= OnHistroyGradePanelUpdateShow;  //收到玩家历史玩家战绩的面板更新 
            SystemMgr.Instance.HolidayActivitySystem.OnHolidayActivityUpdateShow -= OnHolidayActivityPanelUpdateShow;//活动面板更新
            SystemMgr.Instance.HolidayActivitySystem.OnTheHolidayActivityUpdate += OnTheHolidayActivityUpdateShow;//一个活动面板更新
            SystemMgr.Instance.ParlorShowSystem.OnParlorShowUpdate -= OnParlorShowUpdateShow;  //麻将馆的更新通知
            SystemMgr.Instance.ParlorShowSystem.OnMyParlorPanelUpdate -= OnMyParlorUpdate;  //我的麻将馆的更新通知
            SystemMgr.Instance.ParlorShowSystem.OnParlorRedBagUpdate -= OnParlorRedBagUpdate; //麻将馆红包信息更新
            SystemMgr.Instance.RedPageShowSystem.OnRedPageUpdate -= OnRedPageUpdateShow;   //收到红包面板更新
        }

        #region 更新

        /// <summary>
        /// 微信登录面板更新
        /// </summary>
        void OnWxLoginPanelUpdateShow()
        {
            if (WXLoginPanel != null)
            {
                WXLoginPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 实名认证面板的更新
        /// </summary>
        void OnRealNameApprovePanelUpdateShow()
        {
            if (RealNameApprovePanel != null)
            {
                RealNameApprovePanel.UpdateShow();
            }
        }

        /// <summary>
        /// 大厅面板的更新
        /// </summary>
        void OnLobbyPanelUpdateShow()
        {
            if (LobbyPanel != null)
            {
                LobbyPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 红包面板更新
        /// </summary>
        void OnRedPageUpdateShow()
        {
            if (RedPagePanel != null)
            {
                RedPagePanel.OnInitRedPage();
            }
        }

        /// <summary>
        /// 分享面板的更新
        /// </summary>
        void OnshareWxPanelUpdateShow()
        {
            if (ShareWxPanel != null)
            {
                ShareWxPanel.UpdateShow();
            }
        }

        void OnGetGiftBagPanelUpdateShow()
        {
            if (GetGiftBagPanel != null)
            {
                GetGiftBagPanel.UpdateShow();
            }
        }
        /// <summary>
        /// 玩家信息面板更新
        /// </summary>
        void OnUserInfoPanelUpdateShow()
        {
            if (UserInfoPanel != null)
            {
                UserInfoPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 查看游戏玩法面板更新
        /// </summary>
        void OnamePlayingMethodPanelUpdateShow(int status)
        {
            if (GamePlayingMethodPanel != null)
            {
                GamePlayingMethodPanel.UpdateShow(status);
            }
        }

        /// <summary>
        /// 推广界面
        /// </summary>
        void OnProductAgencyPanelUpdateShow()
        {
            if (ProductAgencyPanel != null)
            {
                ProductAgencyPanel.UpdateShow();
            }
        }


        /// <summary>
        /// bug反馈界面
        /// </summary>
        void OnBugSubmitPanelUpdateShow()
        {
            if (CustomSeverPanel != null)
            {
                CustomSeverPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 玩家消息界面更新
        /// </summary>
        void OnPlayerMessagePanelUpdateShow()
        {
            if (PlayerMessagePanel != null)
            {
                PlayerMessagePanel.UpdateShow();
            }
        }

        /// <summary>
        /// 活动面板更新
        /// </summary>
        //void OnActivityPanelUpdateShow()
        //{
        //    if(ActivityPanel!=null)
        //    {
        //        ActivityPanel.UpdateShow();
        //    }
        //}
        /// <summary>
        /// 激活码面板更新
        /// </summary>
        void OnActiveGiftPanelUpdateShow()
        {
            if (ActiveGiftPanel != null)
            {
                ActiveGiftPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 创建房间面板更新
        /// </summary>
        void OnCreatRoomMessagePanelUpdateShow()
        {
            if (CreatRoomMessagePanel != null)
            {
                CreatRoomMessagePanel.UpdateShow();
            }
        }

        void OnCreatRoomMessagePanelDataUpdateShow(int index)
        {
            if (CreatRoomMessagePanel != null)
            {
                CreatRoomMessagePanel.UpdateShowMethod(index);
            }
        }

        /// <summary>
        /// 加入房间面板更新
        /// </summary>
        void OnJoinRoomShowPanelUpdateShow()
        {
            if (JoinRoomShowPanel != null)
            {
                JoinRoomShowPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 代价房间的面板更新
        /// </summary>
        void OnInsteadOpenRoomPanelUpdateShow()
        {
            if (InsteadOpenRoomPanel != null)
            {
                InsteadOpenRoomPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 选择区域的面板更新
        /// </summary>
        void OnSelectAreaPanelUpdateShow()
        {
            if (SelectAreaPanel != null)
            {
                SelectAreaPanel.UpdateShow();
            }
        }
        /// <summary>
        /// 活动面板 历史纪录根性
        /// </summary>
        void OnFestivalActivityUpdateShow()
        {
            if (FestivalActivity != null)
            {
                FestivalActivity.UpdataHistoryShow();
            }
        }

        /// <summary>
        /// 历史战绩的面板更新
        /// </summary>
        void OnHistroyGradePanelUpdateShow()
        {
            if (HistroyGradePanel != null)
            {
                HistroyGradePanel.UpdateShow();
            }
        }

        void OnHolidayActivityPanelUpdateShow()
        {
            if (HolidayActivityPanel != null)
            {
                HolidayActivityPanel.UpdateShow();
            }
        }

        void OnTheHolidayActivityUpdateShow(int index, bool isUpdateBtnOnly)
        {
            if (HolidayActivityPanel != null)
            {
                HolidayActivityPanel.UpdateShowById(index, isUpdateBtnOnly);
            }
        }

        //麻将馆的面板更新
        void OnParlorShowUpdateShow()
        {
            if (ParlorShowPanel != null)
            {
                ParlorShowPanel.UpdateShow();
            }
        }

        //更新玩家拥有过的麻将馆的信息
        void OnMyParlorUpdate(Network.Message.NetMsg.ParlorInfoDef msg)
        {
            if (ParlorShowPanel != null)
            {
                ParlorShowPanel.UpdateMyParlorPanel(msg);
            }
        }

        public delegate void updateRed(int id, int timer);
        public updateRed updateRed_;
        //更新麻将馆的红包信息
        void OnParlorRedBagUpdate(int iParlorId, int timer)
        {
            if (ParlorShowPanel != null)
            {
                ParlorShowPanel.UpdateParlorRedBagMessage(iParlorId, timer);
            }
        }

        #endregion 更新

        public void PlayClickVoice()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(LobbySystem.SubSystem.AudioSystem.AudioType.VIEW_CLOSE);
        }
    }

}
