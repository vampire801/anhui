using UnityEngine;
using MahjongLobby_AH.LobbySystem;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class GetGiftBagSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewGetGiftBagPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<string>.AddListener(MainViewGetGiftBagPanel.MESSAGE_ACTIVITYBTN, HandleActivityBtn);
                    Messenger_anhui.AddListener(MainViewGetGiftBagPanel.MESSAGE_BTNWHATCODE, HandleBtnWhatCode);
                    Messenger_anhui.AddListener(MainViewGetGiftBagPanel.MESSAGE_BTNCLOSECODE, HandleCloseCodeBtn);
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
                    Messenger_anhui.RemoveListener(MainViewGetGiftBagPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<string>.RemoveListener(MainViewGetGiftBagPanel.MESSAGE_ACTIVITYBTN, HandleActivityBtn);
                    Messenger_anhui.RemoveListener(MainViewGetGiftBagPanel.MESSAGE_BTNWHATCODE, HandleBtnWhatCode);
                    Messenger_anhui.RemoveListener(MainViewGetGiftBagPanel.MESSAGE_BTNCLOSECODE, HandleCloseCodeBtn);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        //通知更新的事件
        public delegate void GetGiftBagUpdateShowEventHandler();
        public GetGiftBagUpdateShowEventHandler OnGetGiftBagUpdate;

        //通知更新
        public void UpdateShow()
        {
            if(OnGetGiftBagUpdate!=null)
            {
                OnGetGiftBagUpdate();
            }
        }


        /// <summary>
        /// 处理关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            GameData gd = GameData.Instance;
            GetGiftSpreadBagPanelData ggbpd = gd.GetGiftSpreadBagPanelData;
            ggbpd.PanelShow = false;
            UpdateShow();
        }


        /// <summary>
        /// 处理点击使用推广码按钮
        /// </summary>
        void HandleActivityBtn(string text)
        {       
            //如果已经绑定但是没有领取礼包。发送领取礼包消息     
            if(GameData.Instance.PlayerNodeDef.iSpreaderId>0&&GameData.Instance.PlayerNodeDef.iSpreadGiftTime==0)
            {
                UIMainView.Instance.GetGiftBagPanel.ActivityKey.interactable = false;
                NetMsg.ClientSpreadGiftReq msg = new NetMsg.ClientSpreadGiftReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                NetworkMgr.Instance.LobbyServer.SendSpreadGift(msg);
            }   
            else if(GameData.Instance.PlayerNodeDef.iSpreaderId == 0 && GameData.Instance.PlayerNodeDef.iSpreadGiftTime == 0)
            {
                UIMainView.Instance.GetGiftBagPanel.ActivityKey.interactable = true;
                //发送使用推广码网络消息
                NetMsg.ClientSpreadCodeReq msg = new NetMsg.ClientSpreadCodeReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.szSpreadCode = text;                
                NetworkMgr.Instance.LobbyServer.SendSpreadCode(msg);
            }            
        }

        /// <summary>
        /// 处理点击什么是推广码按钮
        /// </summary>
        void HandleBtnWhatCode()
        {
            GetGiftSpreadBagPanelData ggbpd = GameData.Instance.GetGiftSpreadBagPanelData;
            ggbpd.isShowCode = true;
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
        }

        /// <summary>
        /// 处理点击关闭推广码按钮
        /// </summary>
        void HandleCloseCodeBtn()
        {
            GetGiftSpreadBagPanelData ggbpd = GameData.Instance.GetGiftSpreadBagPanelData;
            ggbpd.isShowCode = false;
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
        }
    }

}
