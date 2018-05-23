using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using System;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ProductAgencySystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewProductAgencyPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<string>.AddListener(MainViewProductAgencyPanel.MESSAGE_AGENCYBTN, HandleAgencyBtn);
                    Messenger_anhui.AddListener(MainViewProductAgencyPanel.MESSAGE_CHANGEAGENCYBTN, HandleCompanyMessageBtn);
                    Messenger_anhui.AddListener(MainViewProductAgencyPanel.MESSAGE_APPLYUNBIND, HandleApplyUnBind);
                    Messenger_anhui.AddListener(MainViewProductAgencyPanel.MESSAGE_BTNCLOSECHANGEAGENC, HandleCloseCompanyMessage);
                    Messenger_anhui<Text>.AddListener(MainViewProductAgencyPanel.MESSAGE_BTNCOPYWXNUMBER, HandleCopyWxNumber);
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
                    Messenger_anhui.RemoveListener(MainViewProductAgencyPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui<string>.RemoveListener(MainViewProductAgencyPanel.MESSAGE_AGENCYBTN, HandleAgencyBtn);
                    Messenger_anhui.RemoveListener(MainViewProductAgencyPanel.MESSAGE_CHANGEAGENCYBTN, HandleCompanyMessageBtn);
                    Messenger_anhui.RemoveListener(MainViewProductAgencyPanel.MESSAGE_APPLYUNBIND, HandleApplyUnBind);
                    Messenger_anhui.RemoveListener(MainViewProductAgencyPanel.MESSAGE_BTNCLOSECHANGEAGENC, HandleCloseCompanyMessage);
                    Messenger_anhui<Text>.RemoveListener(MainViewProductAgencyPanel.MESSAGE_BTNCOPYWXNUMBER, HandleCopyWxNumber);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void ProductAgencyUpdateEventerHandler();
        public ProductAgencyUpdateEventerHandler OnProductAgencyUpdate;

        public void UpdateShow()
        {
            if (OnProductAgencyUpdate != null)
            {
                OnProductAgencyUpdate();
            }
        }

        /// <summary>
        /// 处理点击关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            GameData gd = GameData.Instance;
            ProductAgencyPanelData papd = gd.ProductAgencyPanelData;
            papd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击绑定代理按钮
        /// </summary>
        void HandleAgencyBtn(string id)
        {
            int Id = 0;
            try
            {
                Id = Convert.ToInt32(id);
            }
            catch
            {
                Debug.LogError("字符串转为int有异常：");
            }

            if (Id <= 0)
            {
                return;
            }
            NetMsg.ClientBindProxyReq msg = new NetMsg.ClientBindProxyReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iProxyId = Id;
            Debug.LogError("发送玩家申请代理的消息，userid:" + msg.iUserId + ",代理id：" + msg.iProxyId);
            //NetworkMgr.Instance.LobbyServer.SendBindProxyReq(msg);
        }

        /// <summary>
        /// 处理点击成为代理按钮
        /// </summary>
        void HandleCompanyMessageBtn()
        {
            GameData gd = GameData.Instance;
            ProductAgencyPanelData papd = gd.ProductAgencyPanelData;
            papd.IsShowCompany = true;
            UpdateShow();
        }


        /// <summary>
        /// 点击解绑按钮
        /// </summary>
        void HandleApplyUnBind()
        {
            NetMsg.ClientAskUnbindProxyReq msg = new NetMsg.ClientAskUnbindProxyReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            //NetworkMgr.Instance.LobbyServer.SendUnbindProxy(msg);
            GameData.Instance.ProductAgencyPanelData.isShowSecondUnbind = false;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
        }

        /// <summary>
        /// 关闭如何成为代理弹框
        /// </summary>
        void HandleCloseCompanyMessage()
        {
            GameData gd = GameData.Instance;
            ProductAgencyPanelData papd = gd.ProductAgencyPanelData;
            papd.IsShowCompany = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击复制微信号按钮
        /// </summary>
        /// <param name="text"></param>
        void HandleCopyWxNumber(Text text)
        {
            anhui.MahjongCommonMethod.Instance.CopyString(text.text);
            //处理玩家复制成功之后提示文字
            anhui.MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }
    }

}
