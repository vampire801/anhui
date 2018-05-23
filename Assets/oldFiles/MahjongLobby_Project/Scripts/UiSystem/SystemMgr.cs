using UnityEngine;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class SystemMgr : MonoBehaviour
    {
        #region 实例
        static SystemMgr instance;
        
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns>实例</returns>
        public static SystemMgr Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion 实例

        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        public bool Inited = false;

        #region  父类的方法

        void Awake()
        {
            instance = this;
        }
        /// <summary>
        /// 唤醒
        /// </summary>
        void Start()
        {
            UnInitLobbyMainSystems();
            //初始化各个子系统
            ServerSystem.Init();
            InitLobbyMainSystems();
            Inited = true;            
        }
        void Update()
        {
            ServerSystem.Update();

            UpdateLobbyMainSystems();
        }

        void OnDestroy()
        {
            ServerSystem.Destroy();            
            UnInitLobbyMainSystems();
        }

        /// <summary>
        /// 初始化各个子面板
        /// </summary>
        public void InitLobbyMainSystems()
        {
            WXLoginSystem.Init();
            RealNameApproveSystem.Init();
            LobbyMainSystem.Init();
            ShareWxSystem.Init();
            GetGiftSpreadBagSystem.Init();
            HolidayActivitySystem.Init();
            UserInfoSystem.Init();
			GamePlayingMethodSystem.Init();
            ProductAgencySystem.Init();
            CustomSystem.Init();
            PlayerMessageSystem.Init();
            ActivitySystem.Init();
            ActiveGiftSystem.Init(); 
            CreatRoomMessageSystem.Init();
            JoinRoomShowSystem.Init();
            InsteadOpenRoomSystem.Init();
            SelectAreaSystem.Init();
            HistroyGradeSystem.Init();
            ParlorShowSystem.Init();
            RedPageShowSystem.Init();
        }

        /// <summary>
        /// 清除各个子面板
        /// </summary>
        public void UnInitLobbyMainSystems()
        {
            WXLoginSystem.Destroy();
            RealNameApproveSystem.Destroy();
            LobbyMainSystem.Destroy();
            ShareWxSystem.Destroy();
            HolidayActivitySystem.Destroy();
            GetGiftSpreadBagSystem.Destroy();
            UserInfoSystem.Destroy();
			GamePlayingMethodSystem.Destroy();
            ProductAgencySystem.Destroy();
            CustomSystem.Destroy();
            PlayerMessageSystem.Destroy();
            ActivitySystem.Destroy();
            ActiveGiftSystem.Destroy();
            CreatRoomMessageSystem.Destroy();
            JoinRoomShowSystem.Destroy();
            InsteadOpenRoomSystem.Destroy();
            SelectAreaSystem.Destroy();
            HistroyGradeSystem.Destroy();
            ParlorShowSystem.Destroy();
            RedPageShowSystem.Destroy();
        }


        private void UpdateLobbyMainSystems()
        {
            //realNameApproveSystem.Update();
        }

        #endregion 父类的方法

        #region 子系统

        /// <summary>
        /// 服务器的子系统
        /// </summary>
        ServerSystem serverSystem;
        public ServerSystem ServerSystem
        {
            get
            {
                if(serverSystem==null)
                {
                    serverSystem = new ServerSystem();
                }
                return serverSystem;
            }
        }


        BgAudioManageSystem bgmSystem;
        /// <summary>
        /// 背景音乐系统
        /// </summary>
        public BgAudioManageSystem BgmSystem
        {
            get
            {
                if (bgmSystem == null)
                {
                    bgmSystem = new BgAudioManageSystem();
                }
                return bgmSystem;
            }
        }

        AudioSystem audioSystem;
        /// <summary>
        /// 音效系统
        /// </summary>
        public AudioSystem AudioSystem
        {
            get
            {
                if (audioSystem == null)
                {
                    audioSystem = new AudioSystem();
                }
                return audioSystem;
            }
        }
        /// <summary>
        /// 微信登录系统
        /// </summary>
        WXLoginSystem wxLoginSystem;
        public WXLoginSystem WXLoginSystem
        {
            get
            {
                if (wxLoginSystem==null )
                {
                    wxLoginSystem = new WXLoginSystem();
                }
                return wxLoginSystem;
            }
        }

        /// <summary>
        /// 实名认证系统
        /// </summary>
        RealNameApproveSystem realNameApproveSystem;
        public RealNameApproveSystem RealNameApproveSystem
        {
            get
            {
                if(realNameApproveSystem==null)
                {
                    realNameApproveSystem = new RealNameApproveSystem();
                }
                return realNameApproveSystem;
            }
        }

        /// <summary>
        /// 大厅主面板系统
        /// </summary>
        LobbyMainSystem lobbyMainSystem;
        public LobbyMainSystem LobbyMainSystem
        {
            get
            {
                if(lobbyMainSystem==null)
                {
                    lobbyMainSystem = new LobbyMainSystem();
                }
                return lobbyMainSystem;
            }
        }

        //分享面板系统
        ShareWxSystem shareWxSystem;
        public ShareWxSystem ShareWxSystem
        {
            get
            {
                if(shareWxSystem==null)
                {
                    shareWxSystem = new ShareWxSystem();
                }
                return shareWxSystem;
            }
        }


        //领取激活礼包按钮
        GetGiftBagSystem getGiftBagSystem;
        public GetGiftBagSystem GetGiftSpreadBagSystem
        {
            get
            {
                if(getGiftBagSystem==null)
                {
                    getGiftBagSystem = new GetGiftBagSystem();
                }
                return getGiftBagSystem;
            }
        }
        //玩家信息面板
        UserInfoSystem userInfoSystem;
        public UserInfoSystem UserInfoSystem
        {
            get
            {
                if (userInfoSystem ==null)
                {
                    userInfoSystem = new UserInfoSystem();
                }
                return userInfoSystem;
            }
        }
		
        //查看游戏玩法的系统
		GamePlayingMethodSystem gamePlayingMethodSystem;
        public GamePlayingMethodSystem GamePlayingMethodSystem
        {
            get
            {
                if(gamePlayingMethodSystem==null)
                {
                    gamePlayingMethodSystem = new GamePlayingMethodSystem();
                }
                return gamePlayingMethodSystem;
            }
        }

        //代理面板系统
        ProductAgencySystem productAgencySystem;
        public ProductAgencySystem ProductAgencySystem
        {
            get
            {
                if(productAgencySystem==null)
                {
                    productAgencySystem = new ProductAgencySystem();
                }
                return productAgencySystem; 
            }
        }
        
        /// <summary>
        /// bug反馈面板系统
        /// </summary>
        CustomSystem customSystem;
        public CustomSystem CustomSystem
        {
            get
            {
                if(customSystem==null)
                {
                    customSystem = new CustomSystem();
                }
                return customSystem;
            }
        }

        /// <summary>
        /// 玩家消息面板的子系统
        /// </summary>
        PlayerMessageSystem playerMessageSystem;
        public PlayerMessageSystem PlayerMessageSystem
        {
            get
            {
                if(playerMessageSystem==null)
                {
                    playerMessageSystem = new PlayerMessageSystem();
                }
                return playerMessageSystem;
            }
        }

        /// <summary>
        /// 活动面板子系统
        /// </summary>
        ActivitySystem activitySystem;
        public ActivitySystem ActivitySystem
        {
            get
            {
                if(activitySystem==null)
                {
                    activitySystem = new ActivitySystem();
                }
                return activitySystem;
            }
        }
        /// <summary>
        /// 激活码面板
        /// </summary>
        ActiveGiftSystem activeGiftSystem;
        public ActiveGiftSystem ActiveGiftSystem
        {
            get
            {
                if (activeGiftSystem == null)
                {
                    activeGiftSystem = new ActiveGiftSystem();
                }
                return activeGiftSystem;
            }
        }

        /// <summary>
        /// 创建房间面板的子系统
        /// </summary>
        CreatRoomMessageSystem creatRoomMessageSystem;
        public CreatRoomMessageSystem CreatRoomMessageSystem
        {
            get
            {
                if(creatRoomMessageSystem==null)
                {
                    creatRoomMessageSystem = new CreatRoomMessageSystem();
                }
                return creatRoomMessageSystem;
            }
        }

        /// <summary>
        /// 加入房间的子系统
        /// </summary>
        JoinRoomShowSystem joinRoomShowSystem;
        public JoinRoomShowSystem JoinRoomShowSystem
        {
            get
            {
                if(joinRoomShowSystem==null)
                {
                    joinRoomShowSystem = new JoinRoomShowSystem();
                }
                return joinRoomShowSystem;
            }
        }

        /// <summary>
        /// 代开房间的子系统
        /// </summary>
        InsteadOpenRoomSystem insteadOpenRoomSystem;
        public InsteadOpenRoomSystem InsteadOpenRoomSystem
        {
            get
            {
                if(insteadOpenRoomSystem==null)
                {
                    insteadOpenRoomSystem = new InsteadOpenRoomSystem();
                }
                return insteadOpenRoomSystem;
            }
        }

        /// <summary>
        /// 选择区域的子系统
        /// </summary>
        SelectAreaSystem selectAreaSystem;
        public SelectAreaSystem SelectAreaSystem
        {
            get
            {
                if(selectAreaSystem==null)
                {
                    selectAreaSystem = new SelectAreaSystem();
                }
                return selectAreaSystem;
            }
        }

        //历史战绩的子系统
        HistroyGradeSystem histroyGradeSystem;
        public HistroyGradeSystem HistroyGradeSystem
        {
            get
            {
                if(histroyGradeSystem==null)
                {
                    histroyGradeSystem = new HistroyGradeSystem();
                }
                return histroyGradeSystem;
            }
        }/// <summary>
        /// 节日系统
        /// </summary>
        HolidayActivitySystem holidayActivitySystem;
        public HolidayActivitySystem HolidayActivitySystem
        {
            get
            {
                if (holidayActivitySystem ==null )
                {
                    holidayActivitySystem = new HolidayActivitySystem();
                }
                return holidayActivitySystem;
            }
        }

        /// <summary>
        /// 节日活动系统
        /// </summary>
        FestivalActivitySystem festivalActivitySystem;
        public FestivalActivitySystem FestivalActivitySystem
        {
            get
            {
                if (festivalActivitySystem == null)
                {
                    festivalActivitySystem = new FestivalActivitySystem();
                }
                return festivalActivitySystem;
            }
        }

        /// <summary>
        /// 麻将馆的系统
        /// </summary>
        ParlorShowSystem parlorShowSystem;
        public ParlorShowSystem ParlorShowSystem
        {
            get
            {
                if (parlorShowSystem == null)
                {
                    parlorShowSystem = new ParlorShowSystem();
                }
                return parlorShowSystem;
            }
        }

        RedPageSystem redPageShowSystem;
        public RedPageSystem RedPageShowSystem
        {
            get
            {
                if (redPageShowSystem == null)
                {
                    redPageShowSystem = new RedPageSystem();
                }
                return redPageShowSystem;
            }
        }

        #endregion 子系统
    }

}
