using UnityEngine;
using System;
using MahjongLobby_AH;
using MahjongLobby_AH.Data;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameData : MonoBehaviour
    {
        [HideInInspector]
        public GameObject UiCamera;   //保存ui相机       
        [HideInInspector]
        public bool isShowQuitPanel = true; //是否显示关闭面板      

        #region 实例
        static GameData instance;
        public static GameData Instance { get { return instance; } }
        HistroyGradePanelData hgpd;

        /// <summary>
        /// 玩家的分享出自什么按钮 目前只有20  20分享活动
        /// </summary>
        public int m_iFestivalActivity = 0;

        /// <summary>
        /// 分享红包的索引值
        /// </summary>
        public int m_iRedPageIndex = 0;

        /// <summary>
        /// 分享红包的索引值
        /// </summary>
        public bool m_isShare = false;

        void Awake()
        {
            instance = this;
            hgpd = GameData.instance.HistroyGradePanelData;
        }
        #endregion 实例

        #region 红点枚举

        //存储注册表的键 float
        public enum RedPoint
        {
            FirstOpenInsteadPanel,  //第一次打开代开房间            
            SuccessBindDaili,  //成功绑定代理之后，显示小红点
            RealName,  //实名注册显示小红点
            ShareBag,  //分享礼包的显示小红点
        }
        #endregion



        void Start()
        {
            UiCamera = Camera.main.gameObject;
            //DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isShowQuitPanel)
            {
                if (UIMgr.GetInstance())
                {
                    //弹出退出框
                    UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.PLAYER_QUITAPPLATION, Ok, null, 1, 1);
                }
                else
                {
                    Debug.LogError("0==================GameLauncher:Update()");
                }
            }

        }

        void Ok()
        {
            Application.Quit();
        }

        #region 用户节点数据

        public PlayerNodeDef PlayerNodeDef = new PlayerNodeDef();

        WXLoginPanelData wxLoginPanelData;
        public WXLoginPanelData WXLoginPanelData { get { if (wxLoginPanelData == null) wxLoginPanelData = new WXLoginPanelData(); return wxLoginPanelData; } }

        #endregion 用户节点数据     

        #region 面板数据
        LobbyMainPanelData lobbyMainPanelData;
        public LobbyMainPanelData LobbyMainPanelData
        {
            get
            {
                if (lobbyMainPanelData == null)
                {
                    lobbyMainPanelData = new LobbyMainPanelData();
                }
                return lobbyMainPanelData;
            }
        }

        SetPanelData setPanelData;
        /// <summary>
        /// 设置面板数据
        /// </summary>
        public SetPanelData SetPanelData
        {
            get
            {
                if (setPanelData == null)
                {
                    setPanelData = new SetPanelData();
                }
                return setPanelData;
            }
        }

        SysButtonPanelData sysButtonPanelData;
        /// <summary>
        /// 系统按钮面板数据
        /// </summary>
        public SysButtonPanelData SysButtonPanelData
        {
            get
            {
                if (sysButtonPanelData == null)
                {
                    sysButtonPanelData = new SysButtonPanelData();
                }
                return sysButtonPanelData;
            }
        }
        /// <summary>
        /// 用户信息面板数据
        /// </summary>
        UserInfoPanelData userInfoPanelData;
        public UserInfoPanelData UserInfoPanelData { get { if (userInfoPanelData == null) userInfoPanelData = new UserInfoPanelData(); return userInfoPanelData; } }
        /// <summary>
        /// 实名认证的面板数据
        /// </summary>
        RealNameApprovePanelData realNameApprovePanelData;
        public RealNameApprovePanelData RealNameApprovePanelData
        {
            get
            {
                if (realNameApprovePanelData == null)
                {
                    realNameApprovePanelData = new RealNameApprovePanelData();
                }
                return realNameApprovePanelData;
            }
        }


        /// <summary>
        /// 分享面板的数据
        /// </summary>
        ShareWxPanelData shareWxPanelData;
        public ShareWxPanelData ShareWxPanelData
        {
            get
            {
                if (shareWxPanelData == null)
                {
                    shareWxPanelData = new ShareWxPanelData();
                }

                return shareWxPanelData;
            }
        }

        /// <summary>
        /// 获取推广礼包的面板数据
        /// </summary>
        GetGiftSpreadBagPanelData getGiftBagPanelData;
        public GetGiftSpreadBagPanelData GetGiftSpreadBagPanelData
        {
            get
            {
                if (getGiftBagPanelData == null)
                {
                    getGiftBagPanelData = new GetGiftSpreadBagPanelData();
                }
                return getGiftBagPanelData;
            }
        }

        /// <summary>
        /// 获取激活礼包的面板数据
        /// </summary>
        ActiveGiftPanelData activeGiftPanelData;
        public ActiveGiftPanelData ActiveGiftPanelData
        {
            get
            {
                if (activeGiftPanelData == null)
                {
                    activeGiftPanelData = new ActiveGiftPanelData();
                }
                return activeGiftPanelData;
            }
        }

        /// <summary>
        /// 游戏玩法介绍的面板
        /// </summary>
        GamePlayingMethodPanelData gamePlayingMethodPanelData;
        public GamePlayingMethodPanelData GamePlayingMethodPanelData
        {
            get
            {
                if (gamePlayingMethodPanelData == null)
                {
                    gamePlayingMethodPanelData = new GamePlayingMethodPanelData();
                }
                return gamePlayingMethodPanelData;
            }
        }

        /// <summary>
        /// 推广面板的介绍
        /// </summary>
        ProductAgencyPanelData productAgencyPanelData;
        public ProductAgencyPanelData ProductAgencyPanelData
        {
            get
            {
                if (productAgencyPanelData == null)
                {
                    productAgencyPanelData = new ProductAgencyPanelData();
                }
                return productAgencyPanelData;
            }
        }

        /// <summary>
        /// bug反馈的面板数据
        /// </summary>
        CustomPanelData customPanelData;
        public CustomPanelData CustomPanelData
        {
            get
            {
                if (customPanelData == null)
                {
                    customPanelData = new CustomPanelData();
                }
                return customPanelData;
            }
        }

        /// <summary>
        /// 玩家消息的面板数据
        /// </summary>
        PlayerMessagePanelData playerMessagePanelData;
        public PlayerMessagePanelData PlayerMessagePanelData
        {
            get
            {
                if (playerMessagePanelData == null)
                {
                    playerMessagePanelData = new PlayerMessagePanelData();
                }
                return playerMessagePanelData;
            }
        }

        /// <summary>
        /// 活动面板的数据
        /// </summary>
        ActivityPanelData activityPanelData;
        public ActivityPanelData ActivityPanelData
        {
            get
            {
                if (activityPanelData == null)
                {
                    activityPanelData = new ActivityPanelData();
                }
                return activityPanelData;
            }
        }

        /// <summary>
        /// 创建房间面板数据
        /// </summary>
        CreatRoomMessagePanelData creatRoomMessagePanelData;
        public CreatRoomMessagePanelData CreatRoomMessagePanelData
        {
            get
            {
                if (creatRoomMessagePanelData == null)
                {
                    creatRoomMessagePanelData = new CreatRoomMessagePanelData();
                }
                return creatRoomMessagePanelData;
            }
        }

        HolidayActivityPanelData holidayActivityPanelData;
        public HolidayActivityPanelData HolidayActivityPanelData
        {
            get
            {
                if (holidayActivityPanelData == null)
                {
                    holidayActivityPanelData = new HolidayActivityPanelData();
                }
                return holidayActivityPanelData;
            }
        }
        /// <summary>
        /// 加入房间的面板数据
        /// </summary>
        JoinRoomShowPanelData joinRoomShowPanelData;
        public JoinRoomShowPanelData JoinRoomShowPanelData
        {
            get
            {
                if (joinRoomShowPanelData == null)
                {
                    joinRoomShowPanelData = new JoinRoomShowPanelData();
                }
                return joinRoomShowPanelData;
            }
        }

        /// <summary>
        /// 代开房间的面板数据
        /// </summary>
        InsteadOpenRoomPanelData insteadOpenRoomPanelData;
        public InsteadOpenRoomPanelData InsteadOpenRoomPanelData
        {
            get
            {
                if (insteadOpenRoomPanelData == null)
                {
                    insteadOpenRoomPanelData = new InsteadOpenRoomPanelData();
                }
                return insteadOpenRoomPanelData;
            }
        }

        /// <summary>
        /// 选择区域面板的数据
        /// </summary>
        SelectAreaPanelData selectAreaPanelData;
        public SelectAreaPanelData SelectAreaPanelData
        {
            get
            {
                if (selectAreaPanelData == null)
                {
                    selectAreaPanelData = new SelectAreaPanelData();
                }
                return selectAreaPanelData;
            }
        }

        //玩家历史战绩面板数据
        HistroyGradePanelData histroyGradePanelData;
        public HistroyGradePanelData HistroyGradePanelData
        {
            get
            {
                if (histroyGradePanelData == null)
                {
                    histroyGradePanelData = new HistroyGradePanelData();
                }
                return histroyGradePanelData;
            }
        }

        /// <summary>
        /// 麻将馆的面板数据
        /// </summary>
        ParlorShowPanelData parlorShowPanelData;
        public ParlorShowPanelData ParlorShowPanelData
        {
            get
            {
                if (parlorShowPanelData == null)
                {
                    parlorShowPanelData = new ParlorShowPanelData();
                }
                return parlorShowPanelData;
            }
        }
        #endregion 面板数据

        #region 回放的数据

        PlayBack_1.PlayBackData playBackData;
        /// <summary>
        /// 系统按钮面板数据
        /// </summary>
        public PlayBack_1.PlayBackData PlayBackData
        {
            get
            {
                if (playBackData == null)
                {
                    playBackData = new PlayBack_1.PlayBackData();
                }
                return playBackData;
            }
        }
        #endregion


        #region 大厅中的通用方法



        /// <summary>
        /// 获取到显示的内容
        /// </summary>
        /// <param name="index"></param>
        public void GetPlayingMethondContent(string name)
        {
            StartCoroutine(gamePlayingMethodPanelData.ReadPlayingMethond_(name));
        }




        #endregion 大厅中的通用方法

        #region 十进制转换为三十六进制       

        private const string X36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //10进制转换成36进制
        string ConvertTo36(int userid)
        {
            string result = "";
            while (userid >= 36)
            {
                result = X36[userid % 36] + result;
                userid /= 36;
            }
            if (userid >= 0) result = X36[userid] + result;
            return result;
        }

        /// <summary>
        /// 产生推广码
        /// 对推广码进行运算，获取值放在第一位
        /// </summary>
        public string VerifyCode(int userid)
        {
            string codeKey = ConvertTo36(userid);
            int index = 0;
            char[] ch = new char[codeKey.Length];

            for (int i = 0; i < codeKey.Length; i++)
            {
                for (int j = 0; j < X36.Length; j++)
                {
                    if (codeKey[i] == X36[j])
                    {
                        index += (j);
                        break;
                    }
                }
            }
            int num = index % 36;
            codeKey = X36[num] + codeKey;
            return codeKey;
        }
        #endregion 十进制转换为三十六进制
        
    }

}
