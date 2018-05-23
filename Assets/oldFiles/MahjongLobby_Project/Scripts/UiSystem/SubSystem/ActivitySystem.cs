
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ActivitySystem:GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewActivityPanel.MESSAGE_BTNCLOSE, HandleBtnClose);
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
                    
                    Messenger_anhui.RemoveListener(MainViewActivityPanel.MESSAGE_BTNCLOSE, HandleBtnClose);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void OnActivitySystemUpdateShowEventHandler();
        public OnActivitySystemUpdateShowEventHandler OnActivitySystemUpdate;

        public void UpdateShow()
        {
            if(OnActivitySystemUpdate!=null)
            {
                OnActivitySystemUpdate();
            }
        }

        /// <summary>
        /// 处理点击关闭按钮
        /// </summary>
        void HandleBtnClose()
        {
            GameData gd = GameData.Instance;
            ActivityPanelData apd = gd.ActivityPanelData;
            apd.PanelShow = false;
            UpdateShow();
        }
    }

}
