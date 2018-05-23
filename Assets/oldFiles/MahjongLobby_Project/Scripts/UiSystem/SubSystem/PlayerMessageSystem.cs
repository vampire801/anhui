using UnityEngine;
using MahjongLobby_AH.LobbySystem;
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerMessageSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewPlayerMessagePanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui.AddListener(MainViewPlayerMessagePanel.MESSAGE_JOINAGENCY, HandleJoinAgencyBtn);
                    Messenger_anhui.AddListener(MainViewPlayerMessagePanel.MESSAGE_DISJOINAGENCY, HandleDisJoinAgencyBtn);
                    Messenger_anhui.AddListener(MainViewPlayerMessagePanel.MESSAGE_AGREECANELRELATION, HandleAgreeCanelRelation);
                    Messenger_anhui.AddListener(MainViewPlayerMessagePanel.MESSAGE_DISAGREECANELRELATION, HandleDisAgreeCanelRelation);
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
                    Messenger_anhui.RemoveListener(MainViewPlayerMessagePanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui.RemoveListener(MainViewPlayerMessagePanel.MESSAGE_JOINAGENCY, HandleJoinAgencyBtn);
                    Messenger_anhui.RemoveListener(MainViewPlayerMessagePanel.MESSAGE_DISJOINAGENCY, HandleDisJoinAgencyBtn);
                    Messenger_anhui.RemoveListener(MainViewPlayerMessagePanel.MESSAGE_AGREECANELRELATION, HandleAgreeCanelRelation);
                    Messenger_anhui.RemoveListener(MainViewPlayerMessagePanel.MESSAGE_DISAGREECANELRELATION, HandleDisAgreeCanelRelation);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理


        public delegate void OnPlayerMessageUpdateShowEvnetHandler();
        public OnPlayerMessageUpdateShowEvnetHandler OnPlayerMessageUpdate;

        /// <summary>
        /// 面板更新
        /// </summary>
        public void UpdateShow()
        {
            if(OnPlayerMessageUpdate!=null)
            {
                OnPlayerMessageUpdate();
            }
        }

        /// <summary>
        /// 点击面板关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            pmpd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 点击玩家同意加入代理关系
        /// </summary>
        void HandleJoinAgencyBtn()
        {

        }

        /// <summary>
        /// 玩家点击拒绝加入代理关系
        /// </summary>
        void HandleDisJoinAgencyBtn()
        {

        }

        /// <summary>
        /// 玩家点击同意解除代理关系
        /// </summary>
        void HandleAgreeCanelRelation()
        {

        }

        /// <summary>
        /// 玩家点击拒绝解除代理关系
        /// </summary>
        void HandleDisAgreeCanelRelation()
        {

        }
       
    }

}
