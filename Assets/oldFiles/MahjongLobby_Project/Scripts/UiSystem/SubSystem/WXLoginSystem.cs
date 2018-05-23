using UnityEngine;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class WXLoginSystem : GameSystemBase
    {
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            //base.HandleOnEnterScene(sender);
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewWXLoginPanel.MESSAGE_WXLOGINAUTHBTN, HandleWXLoginBtn);
                    Messenger_anhui.AddListener(MainViewWXLoginPanel.MESSAGE_BTNOK, HandleBtnOk);
                    break;
                default:
                    break;
            }

        }
        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            //base.HandleOnLeaveScene(sender);
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    
                    Messenger_anhui.RemoveListener(MainViewWXLoginPanel.MESSAGE_WXLOGINAUTHBTN, HandleWXLoginBtn);
                    Messenger_anhui.RemoveListener(MainViewWXLoginPanel.MESSAGE_BTNOK, HandleBtnOk);
                    break;
                default:
                    break;
            }
        }

        public delegate void GetWXLoginUpdateShowEventHandler();
        public GetWXLoginUpdateShowEventHandler OnGetWXLoginUpdate;
        public void UpdateShow()
        {
            if (OnGetWXLoginUpdate != null)
            {
                OnGetWXLoginUpdate();
            }
        }

        void HandleWXLoginBtn()
        {
            //SDKManager.Instance.iUserId_iAuthType_ServerType = MahjongCommonMethod.Instance.iUserid + LobbyContants.SeverType;//赋值
            // Debug.LogError("按钮OK" + PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType));
            if (PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType))
            {
                GameData.Instance.PlayerNodeDef.isSencondTime = 2;
                //判断是否过期
                Debug.LogError("判断是否过期");
                Debug.LogError("玩家认证类型:===================4");
                SDKManager.Instance.GetRefreshToken(PlayerPrefs.GetString(SDKManager.Instance.szrefresh_token));
            }
            else
            {

                Debug.LogWarning ("注册表没有iUserID_iAuthType");
                SDKManager.Instance.WXOpenAuthReq();
            }
        }

        /// <summary>
        /// 处理点击确定按钮
        /// </summary>
        void HandleBtnOk()
        {
            //检查网络情况，如果网络正常则关闭，并重连服务器，都则不会关闭该面板
            if (anhui.MahjongCommonMethod.Instance.NetWorkStatus() > 0)
            {
                //关闭面板
                GameData.Instance.WXLoginPanelData.isCloseDisConnectPanel = false;
                UpdateShow();
                Debug.LogError("重连===================================3");
                anhui.MahjongCommonMethod.Instance.SendConnectSever();
            }
        }


    }
}
