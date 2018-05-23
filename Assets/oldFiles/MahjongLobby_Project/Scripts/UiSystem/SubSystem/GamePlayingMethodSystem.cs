using UnityEngine;
using MahjongLobby_AH.LobbySystem;
using MahjongLobby_AH.Data;
using MahjongLobby_AH;
using XLua;

//using MahjongGame_AH.GameSystem;
//using MahjongGame_AH.Scene;
//using MahjongGame_AH.Data;
//using MahjongGame_AH;

//namespace MahjongLobby_AH.LobbySystem.SubSystem
//{
[Hotfix]
[LuaCallCSharp]
public class GamePlayingMethodSystem : GameSystemBase
    {
        #region 事件处理
        /// <summary>
        /// 处理进入场景
        /// </summary>
        /// <param name="sender"></param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewGamePlayingMethodPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<int>.AddListener(MainViewGamePlayingMethodPanel.MESSAGE_MAHJONGMETHOD, HandleReadGamePlayingMethod);
                    break;
                default:
                    break;
            }
        }

        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.RemoveListener(MainViewGamePlayingMethodPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<int>.RemoveListener(MainViewGamePlayingMethodPanel.MESSAGE_MAHJONGMETHOD, HandleReadGamePlayingMethod);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        //更新事件
        public delegate void GamePlayingMethodUpdateEeventHandler(int status);
        public GamePlayingMethodUpdateEeventHandler OnGamePlayingMethodUpdate;

        public void UpdateShow(int status = 0)
        {
            if (OnGamePlayingMethodUpdate != null)
            {
                OnGamePlayingMethodUpdate(status);
            }
        }

        /// <summary>
        /// 处理关闭面板按钮
        /// </summary>
        void HandleCloseBtn()
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            gpmpd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理查看某种玩法的按钮
        /// </summary>
        /// <param name="index">按钮对应的下标</param>
        void HandleReadGamePlayingMethod(int index)
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            gpmpd.GameIndex = index;
            //默认打开基本玩法的基本规则
            UIMainView.Instance.GamePlayingMethodPanel.PointIndexBtn(false, 0);
        }
    }

//}
