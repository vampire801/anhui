using UnityEngine;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using System;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class CreatRoomMessageSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_OKBTN, HandleBtnOk);
                    Messenger_anhui.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_CANELBTN, HandleBtnCanel);
                    Messenger_anhui.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_SWITCHCITY, HandleSwitchCity);
                    //Messenger<GameObject>.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_GAMENUM, HandleBtnGameNum);
                    //Messenger<GameObject>.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_GAMEMUL, HandleBtnGameMul);
                    Messenger_anhui<GameObject>.AddListener(MianViewCreatRoomMessagePanel.MESSAGE_PLAYMETHOD, HandleBtnGamePlayMethod);
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
                    Messenger_anhui.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_OKBTN, HandleBtnOk);
                    Messenger_anhui.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_CANELBTN, HandleBtnCanel);
                    Messenger_anhui.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_SWITCHCITY, HandleSwitchCity);
                    //Messenger<GameObject>.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_GAMENUM, HandleBtnGameNum);
                    //Messenger<GameObject>.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_GAMEMUL, HandleBtnGameMul);
                    Messenger_anhui<GameObject>.RemoveListener(MianViewCreatRoomMessagePanel.MESSAGE_PLAYMETHOD, HandleBtnGamePlayMethod);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void HandleCreatRoomMessageUpdate();
        public HandleCreatRoomMessageUpdate OnCreatRoomMessageUpdateShow;

        public delegate void HandleCreatRoomMethodData(int index);
        public HandleCreatRoomMethodData OnCreatRoomMethodDataUpdateShow;

        public void UpdateShow()
        {
            if (OnCreatRoomMessageUpdateShow != null)
            {
                OnCreatRoomMessageUpdateShow();
            }
        }

        public void UpdateMethodShow(int index)
        {
            if (OnCreatRoomMethodDataUpdateShow != null)
            {
                OnCreatRoomMethodDataUpdateShow(index);
            }
        }

        /// <summary>
        /// 处理点击确定按钮
        /// </summary>
        void HandleBtnOk()
        {
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            InsteadOpenRoomPanelData iorpd = gd.InsteadOpenRoomPanelData;
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            crmpd.PanelShow = false;
            NetMsg.ClientOpenRoomReq msg = new NetMsg.ClientOpenRoomReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.cGameId = (byte)anhui.MahjongCommonMethod.Instance._dicMethodConfig[crmpd.MethodId].gameid;
            msg.iPlayingMethod = crmpd.MethodId;
            msg.iCompliment = crmpd.iCompliment;
            msg.iDisconnectRate = crmpd.iDisconnectRate;
            msg.iDiscardTime = crmpd.iDiscardTime;
            msg.iRoomCard = crmpd.iRoomCard;
            // Debug.LogError("房卡;"+crmpd.iRoomCard);
            msg.cOpenRoomMode = (byte)crmpd.CreatRoomType;
            msg.byColorFlag = (byte)crmpd.iColorFlag;
            msg.caParam = crmpd.roomMessage_;
            msg.iCountyId = sapd.iCountyId;
            if (msg.cOpenRoomMode > 1)
            {
                msg.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
            }
            else
            {
                anhui.MahjongCommonMethod.Instance.iParlorId = 0;
            }
           // Debug.Log("出牌参数0：" + msg.caParam[0].ToString("X8") + "_" + msg.caParam[2].ToString("X8") + "/n" + "出牌参数1：" + msg.caParam[1].ToString("X8") + "_" + msg.caParam[3].ToString("X8"));
            //  Debug.Log("玩法id：" + msg.iPlayingMethod + ",游戏编号:" + msg.cGameId
            //     + ",开放模式:" + msg.cOpenRoomMode + ",馆主id:" + msg.iParlorId + "县ID:" + msg.iCountyId);
            //保存玩家的本次创建房间的详细信息
            // string willSaveRoomRule = "";
            // for (int i = 0; i < msg.caParam.Length; i++)
            // {
            //     if (i == 0)
            //     {
            //       willSaveRoomRule += msg.caParam[i];
            //     }
            //     else
            //    {
            //        willSaveRoomRule += ("," + msg.caParam[i]);
            //    }
            //  }
            // PlayerPrefs.DeleteKey("OpenRoomMsgg");
            PlayerPrefs.SetInt("iPlayingMethod", msg.iPlayingMethod);//  
            PlayerPrefs.SetInt("iCountyId", msg.iCountyId);// 开房所属县id
                                                           // PlayerPrefs.SetString("OpenRoomMsgg", willSaveRoomRule);
                                                           //如果玩家赞的数量或者断线率或者出牌速度小于房间设置，不让创建房间            
            if (gd.PlayerNodeDef.iPlayCardAcc > 0 && (gd.PlayerNodeDef.iPlayCardTimeAcc / gd.PlayerNodeDef.iPlayCardAcc) > msg.iDiscardTime && msg.iDiscardTime > 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的出牌速度不满足，自己设置的开房条件");
                return;
            }
            if (gd.PlayerNodeDef.iGameNumAcc > 0 && ((gd.PlayerNodeDef.iDisconnectAcc / (float)gd.PlayerNodeDef.iGameNumAcc) * 100f) > msg.iDisconnectRate && msg.iDisconnectRate > 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的断线率不满足，自己设置的开房条件");
                return;
            }
            if (gd.PlayerNodeDef.iCompliment < msg.iCompliment && msg.iCompliment > 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的赞的数量不满足，自己设置的开房条件");
                return;
            }
            //处理玩家选择的房间信息是否符合要求
            NetworkMgr.Instance.LobbyServer.SendOpenRoomReq(msg);
            //iorpd.PanelShow = false;
            iorpd.isClickInsteadOpenRoom = false;
            iorpd.isFirstSpwanInsteadRecord = true;
            SystemMgr.Instance.InsteadOpenRoomSystem.UpdateShow();
            UpdateShow();
        }

        /// <summary>
        /// 处理点击取消按钮
        /// </summary>
        void HandleBtnCanel()
        {
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            crmpd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击切换城市按钮
        /// </summary>
        void HandleSwitchCity()
        {
            //打开选择城市面板
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            sapd.iOpenStatus = 2;
            sapd.pos_index = 2;
            sapd.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
        }

        /// <summary>
        /// 处理玩家选择房间局数的按钮
        /// </summary>
        /// <param name="go"></param>
        void HandleBtnGameNum(GameObject go)
        {
            string name = go.name;
            int index = Convert.ToInt32(name.Split('_')[1]);
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            crmpd.GameNum = index * 4;
            crmpd.ChoiceIndex[0] = index;
            UpdateShow();
        }




        /// <summary>
        /// 处理玩家选择房间番数的按钮
        /// </summary>
        /// <param name="go"></param>
        void HandleBtnGameMul(GameObject go)
        {
            string name = go.name;
            int index = Convert.ToInt32(name.Split('_')[1]);
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            crmpd.MulLimit = index * 2;
            crmpd.ChoiceIndex[1] = index;
            UpdateShow();
        }

        /// <summary>
        /// 处理玩家选择玩法的按钮
        /// </summary>
        /// <param name="go"></param>
        void HandleBtnGamePlayMethod(GameObject go)
        {
            string name = go.name;
            int index = Convert.ToInt32(name.Split('_')[1]);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            bool isContainMethor = false;
            //在这里初始化创建房间的参数
            for (int i = 0; i < crmpd.allRoomMethor.Count; i++)
            {
               // Debug.LogError("--"+crmpd.allRoomMethor[i].methord + "_" + crmpd.roomMessage_[0].ToString("X8") + "_" + crmpd.roomMessage_[1].ToString("X8") + "_" + crmpd.roomMessage_[2].ToString("X8") + "_" + crmpd.roomMessage_[3].ToString("X8") + "_" + crmpd.roomMessage_[4].ToString("X8") + "_" + crmpd.roomMessage_[5].ToString("X8"));

                if (crmpd.allRoomMethor[i].methord == index)
                {
                    isContainMethor = true;
                    crmpd.roomMessage_ = crmpd.allRoomMethor[i].param;
                  //  Debug.LogError("读取：" + index + "_" + crmpd.roomMessage_[0].ToString("X8") + "_" + crmpd.roomMessage_[1].ToString("X8") + "_" + crmpd.roomMessage_[2].ToString("X8") + "_" + crmpd.roomMessage_[3].ToString("X8") + "_" + crmpd.roomMessage_[4].ToString("X8") + "_" + crmpd.roomMessage_[5].ToString("X8"));
                }
            }
            if (!isContainMethor)
            {
                for (int i = 0; i < crmpd.roomMessage_.Length; i++)
                {
                    crmpd.roomMessage_[i] = 0;
                }
              //  Debug.LogError("读取不存在：" );
            }
            crmpd.MethodId = index;
            UpdateMethodShow(index);
        }
    }
}

