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
    public class HolidayActivitySystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewHolidayActivityPanel.MESSAGE_CLOSE ,BtnClosePanel );
                    Messenger_anhui<string>.AddListener(MainViewHolidayActivityPanel.MESSAGE_BTNGETGIFT, HandleBtnGetGife);
                    Messenger_anhui<int >.AddListener(MainViewHolidayActivityPanel.MESSAGE_RECEIVE ,BtnReceiveGift );
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
                    Messenger_anhui.RemoveListener(MainViewHolidayActivityPanel.MESSAGE_CLOSE, BtnClosePanel);
                    Messenger_anhui<string>.RemoveListener(MainViewHolidayActivityPanel.MESSAGE_BTNGETGIFT, HandleBtnGetGife);
                    Messenger_anhui<int >.RemoveListener(MainViewHolidayActivityPanel.MESSAGE_RECEIVE, BtnReceiveGift);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理
        public delegate void HandleHolidayActivityUpdate();
        public HandleHolidayActivityUpdate OnHolidayActivityUpdateShow;

        public delegate void HandleHolidayActivityCreate(int index,bool isupdateBtnOnly);
        public HandleHolidayActivityCreate OnTheHolidayActivityUpdate;

        public void UpdateShow()
        {
            if (OnHolidayActivityUpdateShow != null)
            {
                OnHolidayActivityUpdateShow();
            }
        }
        /// <summary>
        /// 更新对应活动面板
        /// </summary>
        /// <param name="index">面板序号</param>
        /// <param name="isUpdateBtenOnly">是否更新按钮</param>
        public void UpdateActivityCreate(int index,bool isUpdateBtenOnly)
        {
            if (OnTheHolidayActivityUpdate !=null)
            {
                OnTheHolidayActivityUpdate(index , isUpdateBtenOnly);
            }
        }

        #region 按钮事件
        /// <summary>
        /// 活动面板关闭
        /// </summary>
        void BtnClosePanel()
        {
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            hapd.isPanelShow = false;
            UpdateShow();
            anhui.MahjongCommonMethod.Instance.HasClicked((int)anhui.MahjongCommonMethod.StateType.CloseActivity);
        }
        /// <summary>
        /// 领取对应活动礼包
        /// </summary>
        /// <param name="id">礼包参数ID</param>
        void BtnReceiveGift(int id)
        {
            NetMsg.ClientHolidayGiftReq msg = new NetMsg.ClientHolidayGiftReq();
            msg.byType =(byte ) id ;
            Debug.LogWarning("请求参数ID" + msg.byType);
            msg.iUserId = anhui.MahjongCommonMethod.Instance.iUserid;
            NetworkMgr.Instance.LobbyServer.SendHolidayGiftReq (msg );
        }
   
        /// <summary>
        /// 获取激活礼包
        /// </summary>
        void HandleBtnGetGife(string cardNum)
        {
            char[] nums = cardNum.ToCharArray();
            NetMsg.ClientUseActiveCodeReq msg = new NetMsg.ClientUseActiveCodeReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.szBatch = nums[0].ToString() + nums[1].ToString();
            string code = "";
            for (int i = 2; i < nums.Length; i++)
            {
                code += nums[i].ToString();
            }
            msg.szCode = code;

            //把所有小写字母转换成大写字母
            msg.szBatch = msg.szBatch.ToUpper();
            msg.szCode = msg.szCode.ToUpper();

            Debug.LogError("msg.szBatch:" + msg.szBatch + "，msg.szCode：" + msg.szCode);
            NetworkMgr.Instance.LobbyServer.SendUseActiveCodeReq(msg);
        }
        #endregion 按钮事件
    }
}
