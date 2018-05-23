using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem;
using System;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class JoinRoomShowSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewJoinRoomShowPanel.MESSAGE_BTNCANEL, HandleCloseBtn);
                    Messenger_anhui<int>.AddListener(MainViewJoinRoomShowPanel.MESSAGE_NUMBTN, HandleNumBtn);
                    Messenger_anhui.AddListener(MainViewJoinRoomShowPanel.MESSAGE_CLEARBTN, HandleClearNumBtn);
                    Messenger_anhui.AddListener(MainViewJoinRoomShowPanel.MESSAGE_DELNUM, HandleDelNumBtn);
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
                    Messenger_anhui.RemoveListener(MainViewJoinRoomShowPanel.MESSAGE_BTNCANEL, HandleCloseBtn);
                    Messenger_anhui<int>.RemoveListener(MainViewJoinRoomShowPanel.MESSAGE_NUMBTN, HandleNumBtn);
                    Messenger_anhui.RemoveListener(MainViewJoinRoomShowPanel.MESSAGE_CLEARBTN, HandleClearNumBtn);
                    Messenger_anhui.RemoveListener(MainViewJoinRoomShowPanel.MESSAGE_DELNUM, HandleDelNumBtn);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        //更新通知事件
        public delegate void JoinRoomShowEventHandler();
        public JoinRoomShowEventHandler OnJoinRoomShowUpdate;

        /// <summary>
        /// 通知更新面板显示
        /// </summary>
        public void UpdateShow()
        {
            if (OnJoinRoomShowUpdate != null)
            {
                OnJoinRoomShowUpdate();
            }
        }

        /// <summary>
        /// 处理关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            jrpd.PanelShow = false;
            SystemMgr.Instance.JoinRoomShowSystem.UpdateShow();
        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        public void HandleOkBtn(int roomId)
        {
            GameData gd = GameData.Instance;
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            anhui.MahjongCommonMethod.Instance.iParlorId = 0;

            ////如果玩家是代理，会检测是不是进入的是代开的房间，如果是直接返回
            //if (gd.PlayerNodeDef.iIsProxy == 1)
            //{
            //    for(int i=0;i<iorpd.OpenRoomInfo_Started.Count;i++)
            //    {
            //        if(iorpd.OpenRoomInfo_Started[i].roomNum==roomId)
            //        {
            //            UIMgr.GetInstance().GetUIMessageView().Show("您无法进入自己的代开房间！");
            //            HandleClearNumBtn();
            //            return;
            //        }
            //    }

            //    for (int i = 0; i < iorpd.OpenRoomInfo_UnStart.Count; i++)
            //    {
            //        if (iorpd.OpenRoomInfo_UnStart[i].roomNum == roomId)
            //        {
            //            UIMgr.GetInstance().GetUIMessageView().Show("您无法进入自己的代开房间！");
            //            HandleClearNumBtn();
            //            return;
            //        }
            //    }
            //}


            //保存进入的房间id
            anhui.MahjongCommonMethod.Instance.RoomId = string.Format(roomId.ToString(), 6d);

            //获取服务器编号
            Network.Message.NetMsg.ClientRoomNumServerTableReq msg = new Network.Message.NetMsg.ClientRoomNumServerTableReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iRoomNum = roomId;
            msg.iParlorId = 0;
            Network.NetworkMgr.Instance.LobbyServer.SendRoomNumSeverTableReq(msg);
        }

        /// <summary>
        /// 处理点击数字按钮
        /// </summary>
        /// <param name="index"></param>
        void HandleNumBtn(int index)
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;

            if (jrpd.lsRoomId.Count < 6)
            {
                jrpd.lsRoomId.Add(index.ToString().ToCharArray()[0]);
                if (jrpd.lsRoomId.Count >= 6)
                {
                    string sRoomId = "";
                    for (int i = 0; i < 6; i++)
                    {
                        sRoomId += jrpd.lsRoomId[i];
                    }
                    int roomId = Convert.ToInt32(sRoomId);
                    HandleOkBtn(roomId);
                }
            }

            UpdateShow();
        }

        /// <summary>
        /// 处理数字清空按钮
        /// </summary>
        void HandleClearNumBtn()
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            //jrpd.RoomId = "";
            jrpd.lsRoomId.Clear();
            UpdateShow();
        }


        /// <summary>
        /// 处理删除按钮
        /// </summary>
        void HandleDelNumBtn()
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            if (jrpd.lsRoomId.Count > 0)
            {
                jrpd.lsRoomId.RemoveAt(jrpd.lsRoomId.Count - 1);
            }
            UpdateShow();
        }
    }

}
