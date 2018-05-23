using UnityEngine;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using System;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class FestivalActivitySystem : GameSystemBase
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
                    Messenger_anhui.AddListener(FestivalActivityPanelData.MESSAGE_HISTORY, OnFestivalActivityUpdateShow);
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
                    Messenger_anhui.RemoveListener(FestivalActivityPanelData.MESSAGE_HISTORY, OnFestivalActivityUpdateShow);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理
        public delegate void HandleHolidayActivityUpdate();
        public HandleHolidayActivityUpdate OnActivityUpdateShow;

        public delegate void HandleHolidayActivityCreate(int index);
        public HandleHolidayActivityCreate OnTheActivityUpdate;

        public void OnFestivalActivityUpdateShow()
        {
            if (OnActivityUpdateShow != null)
                OnActivityUpdateShow();
        }

    }
}
