using UnityEngine;
using System.Collections;
using System;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameRuleSystem : GameSystemBase
    {
        #region 事件处理
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui<int>.AddListener(MainViewGameRulePanel.MESSAGE_OPEN, Open);
                    Messenger_anhui.AddListener(MainViewGameRulePanel.MESSAGE_CLOSEBTN, Close);
                    break;
                default:
                    break;
            }
        }
        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui<int>.RemoveListener(MainViewGameRulePanel.MESSAGE_OPEN, Open);
                    Messenger_anhui.RemoveListener(MainViewGameRulePanel.MESSAGE_CLOSEBTN, Close);
                    break;
                default:
                    break;
            }
        }


        #endregion 事件处理

        //处理面板更新
        public delegate void GameRuleUpdateEventHandler();
        public GameRuleUpdateEventHandler OnGameRuleUpdate;


        void Open(int methonID)
        {
            //UIMainView.Instance.GameRulePanel.gameObject.SetActive(true);
            //UIMainView.Instance.GameRulePanel.GetIndexMethodPanel(methonID);

            //默认打开基本玩法的基本规则
            MahjongLobby_AH.GameData.Instance.GamePlayingMethodPanelData.GameIndex = methonID;
            GameData gd = GameData.Instance;
            MahjongLobby_AH.Data.GamePlayingMethodPanelData gpmpd = MahjongLobby_AH.GameData.Instance.GamePlayingMethodPanelData;
            gpmpd.GameIndex = methonID;
            
            UIMainView.Instance.GamePlayingMethodPanel.PointIndexBtn(true, 0, anhui.MahjongCommonMethod.Instance._dicMethodConfig[methonID].METHOD_NAME);
        }

        void Close()
        {
            UIMainView.Instance.GameRulePanel.gameObject.SetActive(false);
        }
    }
}