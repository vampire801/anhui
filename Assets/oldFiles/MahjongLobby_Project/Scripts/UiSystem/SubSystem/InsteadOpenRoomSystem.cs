using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using System;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class InsteadOpenRoomSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_BTNCLOSE, HandleCloseBtn);
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADRULECLOSE, HandleInsteadRuleCloseBtn);
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTAEDOPENRECORD, HandleInsteadOpenRecordBtn);
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADOPEN, HandleInsteadOpenBtn);
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADRULE, HandleInsteadRuleBtn);
                    Messenger_anhui.AddListener(MainViewInsteadOpenRoomPanel.MESSAGE_OPENCREATROOM, HandleOpenRoomBtn);
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
                    Messenger_anhui.RemoveListener(MainViewInsteadOpenRoomPanel.MESSAGE_BTNCLOSE, HandleCloseBtn);
                    Messenger_anhui.RemoveListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTAEDOPENRECORD, HandleInsteadOpenRecordBtn);
                    Messenger_anhui.RemoveListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADOPEN, HandleInsteadOpenBtn);
                    Messenger_anhui.RemoveListener(MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADRULE, HandleInsteadRuleBtn);
                    Messenger_anhui.RemoveListener(MainViewInsteadOpenRoomPanel.MESSAGE_OPENCREATROOM, HandleOpenRoomBtn);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void InsteadOpenRoomSystemEventHandle();
        public InsteadOpenRoomSystemEventHandle InsteadOpenRoomPanelUpdate;


        /// <summary>
        /// 面板的更新通知
        /// </summary>
        public void UpdateShow()
        {
            if(InsteadOpenRoomPanelUpdate!=null)
            {
                InsteadOpenRoomPanelUpdate();
            }
        }

        /// <summary>
        /// 处理代开面板的关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            iorpd.PanelShow = false;
            iorpd.isClickInsteadOpenRoom = false;
            iorpd.isFirstSpwanInsteadRecord = true;          
            UpdateShow();
            iorpd.iSpwanPrefabNum = 0;
        }

        /// <summary>
        /// 处理代开规则面板的关闭按钮
        /// </summary>
        void HandleInsteadRuleCloseBtn()
        {
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            iorpd.InsteadRulePanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理代开记录按钮
        /// </summary>
        void HandleInsteadOpenRecordBtn()
        {
            Debug.LogError("===================================1");
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            
            //获取代理的历史开房记录
            string str = LobbyContants.MAJONG_PORT_URL + iorpd.url_suffix;
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                str = LobbyContants.MAJONG_PORT_URL_T + iorpd.url_suffix;
            }
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
            value.Add("mode", "2");
            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(str, value, iorpd.GetMessageData, iorpd.json_title,0);

            //根据代开记录，显示玩家的信息
            UIMainView.Instance.InsteadOpenRoomPanel.InsteadOpenRecord.SetActive(true);
            UIMainView.Instance.InsteadOpenRoomPanel.InstaedOPenRoomStatus.SetActive(false);
            iorpd.iBtnStatus = 2;
            SystemMgr.Instance.InsteadOpenRoomSystem.UpdateShow();
            UIMainView.Instance.InsteadOpenRoomPanel.Delay();            
        }

        /// <summary>
        /// 处理代开房间按钮
        /// </summary>
        void HandleInsteadOpenBtn()
        {           
            //删除之前的代开房间信息
            UIMainView.Instance.InsteadOpenRoomPanel.Deleteprefab();
            UIMainView.Instance.InsteadOpenRoomPanel.InsteadOpenRecord.SetActive(false);
            UIMainView.Instance.InsteadOpenRoomPanel.InstaedOPenRoomStatus.SetActive(true);
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;

            float height = UIMainView.Instance.InsteadOpenRoomPanel.ContentSize+ (iorpd.OpenRoomInfo_Started.Count%5)*225f;

            UIMainView.Instance.InsteadOpenRoomPanel.Content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            if (iorpd.iSpwanPrefabNum == 0)
            {
                Debug.LogError("count:" + iorpd.OpenRoomInfo_Started.Count);              
                iorpd.iSpwanPrefabNum = 1;
            }

            //处理已经开启的房间，但是没有进入游戏
            for (int i = 0; i < iorpd.OpenRoomInfo_UnStart.Count; i++)
            {
                UIMainView.Instance.InsteadOpenRoomPanel.SpwanAllRoomStatus(iorpd.OpenRoomInfo_UnStart[i]);
            }

      
            //处理没有开启的房间
            for (int i = 0; i < 5 - iorpd.OpenRoomInfo_UnStart.Count; i++)
            {
                UIMainView.Instance.InsteadOpenRoomPanel.SpwanAllRoomStatus(null);
            }

            iorpd.iBtnStatus = 1;
            SystemMgr.Instance.InsteadOpenRoomSystem.UpdateShow();            
        }
      

        /// <summary>
        /// 点击代开规则按钮
        /// </summary>
        void HandleInsteadRuleBtn()
        {
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            iorpd.InsteadRulePanelShow = true;
            UpdateShow();
        }

        /// <summary>
        /// 点击创建房间按钮
        /// </summary>
        void HandleOpenRoomBtn()
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            //对应县区选择玩法
            anhui.MahjongCommonMethod.Instance.lsMethodId = new List<int>();            
            string[] id = anhui.MahjongCommonMethod.Instance._dicDisConfig[sapd.iCountyId].METHOD.Split('_');
            for (int k = 0; k < id.Length; k++)
            {
                int ID = Convert.ToInt16(id[k]);
                if (ID != 0)
                {
                    Debug.LogWarning("点击创建房间按钮");
                    anhui.MahjongCommonMethod.Instance.lsMethodId.Add(ID);                    
                }
            }
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData srmpd = gd.CreatRoomMessagePanelData;
            //打开创建房间面板
            srmpd.CreatRoomType = 2;
            srmpd.PanelShow = true;
            SystemMgr.Instance.CreatRoomMessageSystem.UpdateShow();            
        }
    }
}
