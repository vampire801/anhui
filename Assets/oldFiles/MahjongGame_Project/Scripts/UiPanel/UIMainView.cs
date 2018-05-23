using UnityEngine;
using System.Collections;
using Common;
using MahjongGame_AH.UISystem;
using System;
using anhui;
using XLua;

namespace MahjongGame_AH
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

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        #endregion 实例

        protected override void Start()
        {
            Canvas[] canvas = transform.GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvas.Length; i++)
            {
                canvas[i].worldCamera = Camera.main;
            }
        }

        /// <summary>
        /// 玩家游戏面板
        /// </summary>
        public MainViewPlayerPlayingPanel PlayerPlayingPanel;

        /// <summary>
        /// 玩家的结算结果面板
        /// </summary>
        public MainViewGameResultPanel GameResultPanel;

        /// <summary>
        /// 防作弊检测面板
        /// </summary>
        public MainViewAntiCheatingPanel AntiCheatingPanel;

        /// <summary>
        /// 聊天面板
        /// </summary>
        public MainViewShortTalkPanel ShortTalkPanel;

        /// <summary>
        /// 断网面板
        /// </summary>
        public DisConnect disConnect;

        /// <summary>
        /// 游戏规则面板
        /// </summary>
        public MainViewGameRulePanel GameRulePanel;
        /// <summary>
        /// 查看游戏玩法面板
        /// </summary>
        public GameObject GamePlayingMethodGameobject;
        MahjongLobby_AH.MainViewGamePlayingMethodPanel gameplayingmethodpanel;
        [HideInInspector]
        public MahjongLobby_AH.MainViewGamePlayingMethodPanel GamePlayingMethodPanel
        {
            get
            {
                if (gameplayingmethodpanel == null)
                {
                    GameObject instance = Instantiate(GamePlayingMethodGameobject) as GameObject;
                    instance.transform.SetParent(transform);
                    instance.transform.localScale = new Vector3(1, 1, 1);
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localPosition = Vector3.zero;
                    instance.name = "GamePlayingMethodPanel";
                    instance.GetComponent<Canvas>().overrideSorting = true;
                    gameplayingmethodpanel = instance.GetComponent<MahjongLobby_AH.MainViewGamePlayingMethodPanel>();
                }
                return gameplayingmethodpanel;
            }
        }

        /// <summary>
        /// 预约房间点击预约时显示弹框
        /// </summary>
        public MainViweImportantMessagePanel ImportantMessagePanel;

        /// <summary>
        /// 红包面板
        /// </summary>
        public ParlorRedBagMessage ParlorRedBagMessage;

        /// <summary>
        /// 游戏内的新的规则面板
        /// </summary>
        public MainViewPlayingRule playeringRule;

        #region 父类方法
        /// <summary>
        /// 获取UI面板的编号
        /// </summary>
        /// <returns></returns>
        public override ushort GetID()
        {
            return UIConstant.UIID_MAHJONG_GAME_MAIN_PANEL;
        }


        /// <summary>
        /// 当试图已经显示，注册事件
        /// </summary>
        protected override void ViewDidAppear()
        {

            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingUpdate += OnPlayerPlayingPanelUpdateShow;//收到游戏界面更新
            SystemMgr.Instance.ShortTalkSystem.OnShortTalkPanelUpdate += OnShortTalkPanelUpdateShow;//聊天面板更新

            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate += OnPlayerPlayingPanelHeadUpdateShow;//游戏界面头像更新
            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingCardsUpdate += OnPlayerPlayingPanelCardsUpdateShow;//游戏界面头像更新
            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingSpecialTileNoticeShow += OnplayerPlayingPanelShowSpecialTileNoticeShow; //收到显示吃碰杠胡的通知

            SystemMgr.Instance.GameResultSystem.OnGameResultUpdate += OnGameResultPanelUpdateShow;  //游戏结果结算的更新通知
            SystemMgr.Instance.AntiCheatingSystem.OnAntiCheatingUpdate += OnAntiCheatingPanelUpdateShow; //防作弊面板的更新通知


        }

        void OnShortTalkPanelUpdateShow()
        {
            if (ShortTalkPanel != null)
                ShortTalkPanel.UpdateShow();

        }

        /// <summary>
        /// 当视图即将隐藏，注销事件
        /// </summary>
        protected override void ViewWillDisappear()
        {

            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingUpdate -= OnPlayerPlayingPanelUpdateShow;//收到游戏界面更新
            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate -= OnPlayerPlayingPanelHeadUpdateShow;//游戏界面头像更新
            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingCardsUpdate -= OnPlayerPlayingPanelCardsUpdateShow;//游戏界面头像更新
            SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingSpecialTileNoticeShow -= OnplayerPlayingPanelShowSpecialTileNoticeShow; //收到显示吃碰杠胡的通知
            SystemMgr.Instance.ShortTalkSystem.OnShortTalkPanelUpdate -= OnShortTalkPanelUpdateShow;//聊天面板更新
            SystemMgr.Instance.GameResultSystem.OnGameResultUpdate -= OnGameResultPanelUpdateShow;  //游戏结果结算的更新通知
            SystemMgr.Instance.AntiCheatingSystem.OnAntiCheatingUpdate -= OnAntiCheatingPanelUpdateShow; //防作弊面板的更新通知
        }
        #endregion 父类方法

        #region 面板更新

        /// <summary>
        /// 游戏界面更新
        /// </summary>
        void OnPlayerPlayingPanelUpdateShow()
        {
            if (PlayerPlayingPanel != null)
            {
                PlayerPlayingPanel.UpdateShow();
            }
        }
        /// <summary>
        /// 玩家游戏界面问题
        /// </summary>
        /// <param name="status"></param>
        void OnplayerPlayingPanelShowSpecialTileNoticeShow(int[] status)
        {
            if (PlayerPlayingPanel != null)
            {
                PlayerPlayingPanel.ShowSpecialTileNoticeRemind(status);
            }
        }


        /// <summary>
        /// 游戏面板玩家头像更新
        /// </summary>
        /// <param name="pos"></param>
        void OnPlayerPlayingPanelHeadUpdateShow(int seatNum, byte num = 2)
        {
            if (PlayerPlayingPanel != null)
            {
                PlayerPlayingPanel.HeadUpdateShow(seatNum ,num);
            }

        }

        /// <summary>
        /// 游戏面板牌的花色刷新
        /// </summary>
        void OnPlayerPlayingPanelCardsUpdateShow()
        {
            if (PlayerPlayingPanel != null)
                PlayerPlayingPanel.CardsUpdateShow();
        }

        /// <summary>
        /// 游戏结算面板的更新
        /// </summary>
        void OnGameResultPanelUpdateShow()
        {
            if (GameResultPanel != null)
            {
                GameResultPanel.UpdateShow();
            }
        }

        /// <summary>
        /// 防作弊检测面板
        /// </summary>
        void OnAntiCheatingPanelUpdateShow()
        {
            if (AntiCheatingPanel != null)
            {
                AntiCheatingPanel.UpdateShow();
            }
        }

        #endregion 面板更新

    }

}
