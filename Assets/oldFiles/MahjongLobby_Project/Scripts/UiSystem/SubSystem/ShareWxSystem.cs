using UnityEngine;
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ShareWxSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewShareWxPanel.MESSAGE_CLOSEBTN, HandleColseBtn);
                    Messenger_anhui<string >.AddListener(MainViewShareWxPanel.MESSAGE_SHAREWX, HandleShareWX);
                    Messenger_anhui.AddListener(MainViewShareWxPanel.MESSAGE_GETMONY, HandleGetInvitationCodeMoney);
                    Messenger_anhui.AddListener(MainViewShareWxPanel.MESSAGE_NEWPLAYERGOLD, HandleGetNewPlayerGold);
                    Messenger_anhui.AddListener(MainViewShareWxPanel.MESSAGE_PROMOTIONGOLD, HandleGetPromotionGold);
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
                    Messenger_anhui.RemoveListener(MainViewShareWxPanel.MESSAGE_CLOSEBTN, HandleColseBtn);
                    Messenger_anhui<string >.RemoveListener(MainViewShareWxPanel.MESSAGE_SHAREWX, HandleShareWX);
                    Messenger_anhui.RemoveListener(MainViewShareWxPanel.MESSAGE_GETMONY, HandleGetInvitationCodeMoney);
                    Messenger_anhui.RemoveListener(MainViewShareWxPanel.MESSAGE_NEWPLAYERGOLD, HandleGetNewPlayerGold);
                    Messenger_anhui.RemoveListener(MainViewShareWxPanel.MESSAGE_PROMOTIONGOLD, HandleGetPromotionGold);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        //通知界面更新事件
        public delegate void ShareWxUpdateShowEventHandler();
        public ShareWxUpdateShowEventHandler OnShareWxUpdate;

        /// <summary>
        /// 处理更新
        /// </summary>
        public void UpdateShow()
        {
            if (OnShareWxUpdate != null)
            {
                OnShareWxUpdate();
            }
        }
        /// <summary>
        /// 处理点击关闭按钮事件
        /// </summary>
        void HandleColseBtn()
        {
            GameData.Instance.ShareWxPanelData.PanelShow = false;
            UpdateShow();
        }
        /// <summary>
        /// 处理分享到微信消息的按钮事件
        /// </summary>
        /// </summary>
        /// <param name="type"> type=0 分享朋友 1->朋友圈 2->收藏</param>
        void HandleShareWX(string  type)
        {
            // string url = ComposeShareUrlWithpara("0");
            SDKManager.Instance.iWitchShare = 1;
            // Debug.LogWarning("分享按钮点击");
            string url = "";
            if (SDKManager.Instance.CheckStatus ==1)
            {
                url = SDKManager.szShareSongSheng ;
            }
            else
            {
                url = string.Format(SDKManager.szShareRoomIdUrl, anhui.MahjongCommonMethod.Instance.iUserid);
            }
            // Debug.LogWarning("URL:" + url);
            if (anhui.MahjongCommonMethod.Instance.iCityId==0)
            {
                anhui.MahjongCommonMethod.Instance.iCityId = 355;
                anhui.MahjongCommonMethod.Instance.iCountyId = 140400;
            }
            //string title = MahjongCommonMethod.Instance._dicCityConfig[MahjongCommonMethod.Instance  .iCityId].SHARING;
            //string discritption = MahjongCommonMethod.Instance._dicCityConfig[MahjongCommonMethod.Instance.iCityId].SHARING_INVITATION.Insert(1, GameData.Instance.PlayerNodeDef.szNickname);
            //SDKManager.Instance.HandleShareWX(url, title, discritption, type,11);
            string[] str = type.Split(',');
            Debug.LogError("红包类型：" + str[1]);
           // SDKManager.Instance.BtnShare(int.Parse ( str[0]), int.Parse(str[1]),"");
            string surl = LobbyContants.ActivitePic + "fenxiang/sx_share_logo1.jpg";
           //  string surl = LobbyContants.ActivitePic + "fenxiang/sx_share_logo.png";
            SDKManager.Instance.BtnShare(int.Parse(str[0]), int.Parse(str[1]), surl);
        }

        /// <summary>
        /// 填写邀请码领金币
        /// </summary>
        public void HandleGetInvitationCodeMoney()
        {
            anhui.MahjongCommonMethod.Instance.HasClicked((int)anhui.MahjongCommonMethod.StateType.ClickShareGift);
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_GETGIFTBAG);
            UIMainView.Instance.ShareWxPanel.BtnClose();
            //UIMgr.GetInstance().GetUIMessageView().Show("填写邀请码领金币");
        }
        /// <summary>
        /// 新手金币
        /// </summary>
        public void HandleGetNewPlayerGold()
        {
            UIMainView.Instance.ShareWxPanel.NewPlayerGetGoldPanel.SetActive(true);
            UIMainView.Instance.ShareWxPanel.QRCode.SetActive(false);
            UIMainView.Instance.ShareWxPanel.InvitationCode.text = GameData.Instance.VerifyCode(GameData.Instance.PlayerNodeDef.iUserId).ToString();
            UIMainView.Instance.ShareWxPanel.BtnClose();
            //UIMgr.GetInstance().GetUIMessageView().Show("新手金币");
        }
        /// <summary>
        /// 推广金币
        /// </summary>
        public void HandleGetPromotionGold()
        {
            UIMgr.GetInstance().GetUIMessageView().Show("推广金币");
        }

        string ComposeShareUrlWithpara(string para)
        {
            string str = GameData.Instance.ShareWxPanelData.szShareUrl + para;
            return str;
        }
    }
}


