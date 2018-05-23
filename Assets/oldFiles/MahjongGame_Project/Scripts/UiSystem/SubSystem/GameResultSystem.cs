using UnityEngine;
using System.Collections;
using MahjongGame_AH.Data;
using MahjongGame_AH.Network.Message;
using System;
using MahjongLobby_AH;
using UnityEngine.UI;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameResultSystem : GameSystemBase
    {

        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui<int>.AddListener(MainViewGameResultPanel.MESSAGER_NEXTROUND_ROUND, HandleBtnNextRound);
                    Messenger_anhui< GameObject[]>.AddListener(MainViewGameResultPanel.MESSAGE_SHARE_ROUND, HandleBtnShareWx);
                    Messenger_anhui.AddListener(MainViewGameResultPanel.MESSAGER_NEXTROUND_ROOM, HandleBtnNextRoom);
                    Messenger_anhui< GameObject[]>.AddListener(MainViewGameResultPanel.MESSAGE_SHARE_ROOM, HandleBtnShareRoom);
                    break;
                default:
                    break;
            }
        }
        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui<int>.RemoveListener(MainViewGameResultPanel.MESSAGER_NEXTROUND_ROUND, HandleBtnNextRound);
                    Messenger_anhui< GameObject[]>.RemoveListener(MainViewGameResultPanel.MESSAGE_SHARE_ROUND, HandleBtnShareWx);
                    Messenger_anhui.RemoveListener(MainViewGameResultPanel.MESSAGER_NEXTROUND_ROOM, HandleBtnNextRoom);
                    Messenger_anhui< GameObject[]>.RemoveListener(MainViewGameResultPanel.MESSAGE_SHARE_ROOM, HandleBtnShareRoom);
                    break;
                default:
                    break;
            }
        }


        #region 事件
        //处理面板更新
        public delegate void GameResultUpdateEventHandler();
        public GameResultUpdateEventHandler OnGameResultUpdate;
        #endregion

        public void UpdateShow()
        {
            if (OnGameResultUpdate != null)
            {
                OnGameResultUpdate();
            }
        }

        /// <summary>
        /// 点击继续按钮
        /// </summary>
        void HandleBtnNextRound(int status)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;

            //如果是手动点击继续，自动解除托管
            if (status == 1)
            {
                if (pppd.iPlayerHostStatus > 0)
                {
                    GameData.Instance.GameResultPanelData.iHandClick = 1;
                    NetMsg.ClientCancleAutoStatusReq msg = new NetMsg.ClientCancleAutoStatusReq();
                    msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    msg.iSeatNum = (byte)pppd.bySeatNum;
                    Network.NetworkMgr.Instance.GameServer.SendClientCancleAutoStatusReq(msg);
                }
            }
            else
            {
                GameData.Instance.GameResultPanelData.iHandClick = 2;
            }

            //如果玩家是托管，这个时候检测解散信息
            if (pppd.iDissolveFlag == 1)
            {
                pppd.iDissolveFlag = 0;
                NetMsg.ClientDismissRoomRes msg = pppd.DismissRoomRes;
                //显示解散界面
                int seatNum = pppd.GetOtherPlayerPos(msg.iUserId);

                //显示玩家发起解散房间
                if (msg.cType == 1)
                {
                    if (pppd.isBeginGame || pppd.iMethodId == 11)
                    {
                        //显示界面
                        UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(true);
                        UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localPosition = Vector3.zero;
                        UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.transform.localScale = Vector3.one;
                        if (pppd.isBeginGame || pppd.isWatingPlayerDownOrUp)
                        {
                            //更新界面
                            UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().iseatNum = seatNum;
                            UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.GetComponent<DissvloeNoticePanel>().ShowAllPlayerMessage();
                        }
                    }
                }
                else if (msg.cType == 3)
                {
                    UIMainView.Instance.PlayerPlayingPanel.DissolvePanel.SetActive(false);
                    System.Text.StringBuilder str = new System.Text.StringBuilder();
                    str.Append("由于玩家【");
                    str.Append(pppd.usersInfo[seatNum].szNickname);
                    str.Append("】拒绝，房间解散失败，继续游戏");
                    UIMgr.GetInstance().GetUIMessageView().Show(str.ToString(), Ok);
                }
            }

            //判断玩家是解散结束还是正常结束
            if (grpd.HandDissolve == 0 && !grpd.isEndGame)
            {
                if (!pppd.isAllAutoStatus && pppd.iDissolveStatus != 1)
                {
                    //发送准备请求
                    NetMsg.ClientReadyReq msg = new NetMsg.ClientReadyReq();
                    msg.iUserId = GameData.Instance.PlayerPlayingPanelData.iUserId;
                    Debug.Log ("点击继续按钮，发送准备请求");
                    Network.NetworkMgr.Instance.GameServer.SendReadyReq(msg);
                }
                GameData.Instance.GameResultPanelData.isPanelShow = false;
                if (pppd.isBeginGame)
                {
                    UIMainView.Instance.PlayerPlayingPanel.InitPanel();
                }
            }
            //如果玩家是解散结束的
            else
            {
                //关闭玩家的单局结算界面
                grpd.isShowRoundGameResult = false;
                //显示总的结算数据
                grpd.isShowRoomGameResult = true;
                UIMainView.Instance.GameResultPanel.SpwanAllGameResult();
                UpdateShow();
            }

            SystemMgr.Instance.GameResultSystem.UpdateShow();
        }


        void Ok()
        {
            //发送准备请求
            NetMsg.ClientReadyReq msg = new NetMsg.ClientReadyReq();
            msg.iUserId = GameData.Instance.PlayerPlayingPanelData.iUserId;
            Network.NetworkMgr.Instance.GameServer.SendReadyReq(msg);
        }

        /// <summary>
        /// 点击分享按钮
        /// </summary>
        void HandleBtnShareWx(  GameObject[] _obj)
        {
            SDKManager.Instance.HandleShareImage( LobbyContants.DownLoadQRcode, 0, 1, _obj);
        }


        //处理游戏结束的确定按钮
        void HandleBtnNextRoom()
        {
            anhui.MahjongCommonMethod.isIntivateDisConnct = true;
            MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Disconnect();

            Messenger_anhui.Broadcast(MainViewPlayerPlayingPanel.MESSAGE_RETURN);//
        }

        /// <summary>
        /// 游戏结束的分享按钮
        /// </summary>
        void HandleBtnShareRoom(  GameObject[] obj)
        {
            //分享图片
            SDKManager.Instance.HandleShareImage( LobbyContants.DownLoadQRcode, 0, 1, obj);
        }
    }
}

