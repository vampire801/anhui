using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class RedPageSystem : GameSystemBase
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
                    //Messenger.AddListener(MainViewLobbyPanel.MESSAGE_REALNAME, HandleRealNameSure);
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
                    //Messenger.RemoveListener(MainViewLobbyPanel.MESSAGE_REALNAME, HandleRealNameSure);
                    break;
                default:
                    break;
            }
        }
        public override void Init()
        {
            base.Init();
        }
        #endregion 事件处理

        //通知面板更新事件
        public delegate void RedPageUpdateEventHandler();
        public RedPageUpdateEventHandler OnRedPageUpdate;

        public void UpdateShow()
        {
            if (OnRedPageUpdate != null)
            {
                OnRedPageUpdate();
            }
        }


    }
}
