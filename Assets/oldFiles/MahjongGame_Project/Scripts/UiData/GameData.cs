using UnityEngine;
using MahjongGame_AH.Data;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameData : MonoBehaviour
    {
        [HideInInspector]
        public GameObject UiCamera;

        #region  单例
        static GameData instance;
        public static GameData Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            UiCamera = Camera.main.gameObject;
        }
        #region 面板数据

        //
        public int m_active = 0;

        public PlayerNodeDef PlayerNodeDef = new MahjongGame_AH.PlayerNodeDef();


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

        /// <summary>
        /// 游戏面板数据
        /// </summary>
        PlayerPlayingPanelData playerPlayingPanelData;
        public PlayerPlayingPanelData PlayerPlayingPanelData
        {
            get
            {
                if (playerPlayingPanelData == null)
                    playerPlayingPanelData = new PlayerPlayingPanelData();
                return playerPlayingPanelData;
            }
        }

        /// <summary>
        /// 游戏结算结果的数据信息
        /// </summary>
        GameResultPanelData gameResultPanelData;
        public GameResultPanelData GameResultPanelData
        {
            get
            {
                if (gameResultPanelData == null)
                {
                    gameResultPanelData = new GameResultPanelData();
                }
                return gameResultPanelData;
            }
        }

        //防作弊面板数据
        AntiCheatingPanelData antiCheatingPanelData;



        public AntiCheatingPanelData AntiCheatingPanelData
        {
            get
            {
                if (antiCheatingPanelData == null)
                {
                    antiCheatingPanelData = new AntiCheatingPanelData();
                }
                return antiCheatingPanelData;
            }
        }
        //聊天面板数据
        ShortTalkData shortTalkData;
        public ShortTalkData ShortTalkData
        {
            get
            {
                if (shortTalkData == null)
                {
                    shortTalkData = new ShortTalkData();
                }
                return shortTalkData;
            }
        }

        #endregion 面板数据       
    }

}
