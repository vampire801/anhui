using UnityEngine;
using System.Collections;
using MahjongGame_AH;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH
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
        /// 是否加载完系统
        /// </summary>
        public bool Inited = false;

        #region  父类的方法
        /// <summary>
        /// 唤醒
        /// </summary>
        void Start()
        {
            instance = this;

            //初始化各个子系统            
            ServerSystem.Init();
           

            InitLobbyMainSystems();
            Inited = true;
        }

        void OnDestroy()
        {
            ServerSystem.Destroy();
            

            UnInitLobbyMainSystems();
        }

        //初始化各个子系统
        void InitLobbyMainSystems()
        {
            PlayerPlayingSystem.Init();
            GameResultSystem.Init();
            ShortTalkSystem.Init();

            RuleSystem.Init();
        }

        //删除各自子系统的处理
        void UnInitLobbyMainSystems()
        {
            PlayerPlayingSystem.Destroy();
            GameResultSystem.Destroy();
            ShortTalkSystem.Destroy ();
            RuleSystem.Destroy();
        }

        //子系统的更新
        void Update()
        {
            ServerSystem.Update();
            
            UpdateLobbyMainSystems();
        }


        private void UpdateLobbyMainSystems()
        {

        }
        #endregion 父类的方法

        #region 子系统
        //需要时再次返回所需子系统的实例
        ServerSystem serverSystem;
        /// <summary>
        /// 服务器系统
        /// </summary>
        public ServerSystem ServerSystem
        {
            get
            {
                if (serverSystem == null)
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
        /// 玩家游戏面板系统
        /// </summary>
        PlayerPlayingSystem playerPlayingSystem;
        public PlayerPlayingSystem PlayerPlayingSystem
        {
            get
            {
                if (playerPlayingSystem == null)
                    playerPlayingSystem = new PlayerPlayingSystem();
                return playerPlayingSystem;
            }
        }

        /// <summary>
        /// 游戏结果结算的系统
        /// </summary>
        GameResultSystem gameResultSystem;
        public GameResultSystem GameResultSystem
        {
            get
            {
                if (gameResultSystem == null)
                {
                    gameResultSystem = new GameResultSystem();
                }
                return gameResultSystem;
            }
        }


        /// <summary>
        /// 防作弊面板的系统
        /// </summary>
        AntiCheatingSystem antiCheatingSystem;
        public AntiCheatingSystem AntiCheatingSystem
        {
            get
            {
                if(antiCheatingSystem==null)
                {
                    antiCheatingSystem = new AntiCheatingSystem();
                }
                return antiCheatingSystem;
            }
        }
        ShortTalkSystem shortSystem;
        public ShortTalkSystem ShortTalkSystem
        {
            get
            {
                if (shortSystem == null)
                {
                    shortSystem = new ShortTalkSystem();
                }
                return shortSystem;
            } }

        GameRuleSystem ruleSystem;
        public GameRuleSystem RuleSystem
        {
            get
            {
                if (ruleSystem == null)
                {
                    ruleSystem = new GameRuleSystem();
                }
                return ruleSystem;
            }
        }
        #endregion 子系统

    }
}

