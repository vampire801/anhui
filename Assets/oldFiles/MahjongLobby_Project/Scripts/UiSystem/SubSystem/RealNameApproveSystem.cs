using UnityEngine;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class RealNameApproveSystem : GameSystemBase
    {
        #region 事件处理
       
        /// <summary>
        /// 当进入场景处理
        /// </summary>
        /// <param name="sender"></param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:         
                    Messenger_anhui.AddListener(MainViewRealNameApprovePanel.MESSAGE_CLOSE, HandleClosePanel);
                    Messenger_anhui<string,string>.AddListener(MainViewRealNameApprovePanel.MESSAGE_OK, HandleClickOk);
                    break;
                default:
                    break;
            }
        }

        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch(sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_GENERAL:
                    Messenger_anhui.RemoveListener(MainViewRealNameApprovePanel.MESSAGE_CLOSE, HandleClosePanel);
                    Messenger_anhui<string, string>.RemoveListener(MainViewRealNameApprovePanel.MESSAGE_OK, HandleClickOk);
                    break;
                default:
                    break;
            }
        }

        //通知界面更新
        public delegate void RealNameApproveUpdateShowEventHandler();
        public RealNameApproveUpdateShowEventHandler OnRealNameApprovePanelUpdate;

        /// <summary>
        /// 处理更新
        /// </summary>
        public void UpdateShow()
        {
            if(OnRealNameApprovePanelUpdate!=null)
            {
                OnRealNameApprovePanelUpdate();
            }
        }

        /// <summary>
        /// 处理关闭面板
        /// </summary>
        public void HandleClosePanel()
        {
            GameData gd = GameData.Instance;
            RealNameApprovePanelData rnapd = gd.RealNameApprovePanelData;
            rnapd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击确认按钮
        /// </summary>
        public void HandleClickOk(string name,string IdCard)
        {
            GameData gd = GameData.Instance;
            RealNameApprovePanelData rnapd = gd.RealNameApprovePanelData;
            rnapd.Name = name;
            rnapd.IdCard = IdCard;
            //发送实名认证请求消息
            NetMsg.ClientFullNameReq msg = new NetMsg.ClientFullNameReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.szFullName = rnapd.Name;
            msg.szIdentityCard = rnapd.IdCard;
            NetworkMgr.Instance.LobbyServer.SendFullNameReq(msg);
            Debug.LogError("name:" + rnapd.Name + ",IdCard:" + rnapd.IdCard+"userid:" + GameData.Instance.PlayerNodeDef.iUserId);                                    
            rnapd.PanelShow = false;
            UpdateShow();
        }

        #endregion 事件处理

    }

}
