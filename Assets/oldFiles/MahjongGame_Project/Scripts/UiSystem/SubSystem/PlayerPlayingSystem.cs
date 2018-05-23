using UnityEngine;
using System.Collections;
using System.Text;
using MahjongGame_AH.Network.Message;
using MahjongGame_AH.Data;
using MahjongGame_AH.Network;
using System.Collections.Generic;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using System;
using XLua;
using anhui;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerPlayingSystem : GameSystemBase
    {
        #region 事件处理
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Debug.LogWarning ("进入场景，事件处理");
                    //Messenger<int>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNPLAYERAVATOR, HandleBtnPlayerAvator);
                    Messenger_anhui<int>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNSPECIALCARD, HandleBtnSpecialCard);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_COPYROOMID, HandleBtnCopyRoomId);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_SHAREWX, HandleBtnShareWx);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_ROOMRULE, HandleBtnRoomRule);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_RETURN, HandleBtnReturnLobby);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_DISSOLVEROOM, HandleBtnDissolveRoom);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_CHOICETHIRTEEN, HandleBtnChoiceThirteen);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_CANCALTHIRTEEN, HandleBtnCancalThirteen);
                    Messenger_anhui<float>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_MUSICVALUE, HandleChangeMusic);
                    Messenger_anhui<float>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_EFFECVALUE, HandleChangeEffect);
                    Messenger_anhui<float>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_VOICEVALUE, HandleChangeVoice);

                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_MUSICCLICK, HandleSetMusic);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_EFFECCLICK, HandleSetMusicEffect);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_VOICECLICK, HandleSetVoice);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNANTICHEATING, HandleAntiCheating);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNTING_DOWN, HandleLookTingCard_Down);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNTING_UP, HandleLookTingCard_Up);
                    //Messenger.AddListener(MainViewPlayerPlayingPanel.MESSAGE_BTNVOICE, HandleBtnVoice);
                    Messenger_anhui<int>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_INSEAT, HandleBtnInSeat);
                    Messenger_anhui<int>.AddListener(MainViewPlayerPlayingPanel.MESSAGE_OUTSEAT, HandleBtnOutSeat);
                    Messenger_anhui.AddListener(MainViewPlayerPlayingPanel.MESSAGE_PLAYRREADY, HandleBtnPlayReady);
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
                    //Messenger<int>.RemoveListener(MainViewPlayerPlayingPanel, HandleBtnPlayerAvator);
                    Messenger_anhui<int>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_BTNSPECIALCARD, HandleBtnSpecialCard);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_COPYROOMID, HandleBtnCopyRoomId);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_SHAREWX, HandleBtnShareWx);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_ROOMRULE, HandleBtnRoomRule);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_RETURN, HandleBtnReturnLobby);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_DISSOLVEROOM, HandleBtnDissolveRoom);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_CHOICETHIRTEEN, HandleBtnChoiceThirteen);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_CANCALTHIRTEEN, HandleBtnCancalThirteen);
                    Messenger_anhui<float>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_MUSICVALUE, HandleChangeMusic);
                    Messenger_anhui<float>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_EFFECVALUE, HandleChangeEffect);
                    Messenger_anhui<float>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_VOICEVALUE, HandleChangeVoice);

                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_MUSICCLICK, HandleSetMusic);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_EFFECCLICK, HandleSetMusicEffect);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_VOICECLICK, HandleSetVoice);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_BTNANTICHEATING, HandleAntiCheating);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_BTNTING_DOWN, HandleLookTingCard_Down);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_BTNTING_UP, HandleLookTingCard_Up);
                    //Messenger.RemoveListener(MainViewPlayerPlayingPanel, HandleBtnVoice);
                    Messenger_anhui<int>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_OUTSEAT, HandleBtnOutSeat);
                    Messenger_anhui<int>.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_INSEAT, HandleBtnInSeat);
                    Messenger_anhui.RemoveListener(MainViewPlayerPlayingPanel.MESSAGE_PLAYRREADY, HandleBtnPlayReady);
                    break;
                default:
                    break;
            }
        }


        #endregion 事件处理
        //处理面板更新
        public delegate void PlayerPlayingUpdateEventHandler();
        public PlayerPlayingUpdateEventHandler OnPlayerPlayingUpdate;

        //游戏界面的头像更新
        public delegate void PlayerPlayingHeadUpdateEventHandler(int seatNum, byte num = 2);
        public PlayerPlayingHeadUpdateEventHandler OnPlayerPlayingHeadUpdate;

        //牌面更新
        public delegate void PlayerPlayingCardsUpdateEventHandler();
        public PlayerPlayingCardsUpdateEventHandler OnPlayerPlayingCardsUpdate;

        //更新显示吃碰杠胡
        public delegate void PlayerPlayingSpecialTileNoticeShowHandler(int[] status);
        public PlayerPlayingSpecialTileNoticeShowHandler OnPlayerPlayingSpecialTileNoticeShow;

        /// <summary>
        /// 玩家等待界面的更新通知事件
        /// </summary>
        public void UpdateShow()
        {
            if (OnPlayerPlayingUpdate != null)
            {
                OnPlayerPlayingUpdate();
            }
        }

        /// <summary>
        /// 更新玩家头像面板
        /// </summary>
        public void HeadUpdateShow(int seatNum)
        {
            if (OnPlayerPlayingHeadUpdate != null)
            {
                OnPlayerPlayingHeadUpdate(seatNum);
            }
        }
        /// <summary>
        /// 牌面有关更新
        /// </summary>
        public void UpdateCards()
        {

        }

        public void HandleBtnInSeat(int index)
        {
            //发送占座消息
            NetMsg.ClientUserBespeakReqDef msg = new NetMsg.ClientUserBespeakReqDef();
            msg.byFromType = 2;//消息来源，1来自大厅，2来自游戏客户端
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId; // 用户编号
            msg.iSeatNum = (byte)(index);//座位号
            Debug.LogError("发送占座消息：" + msg.iSeatNum);
            msg.iRoomNum = Convert.ToInt32(MahjongCommonMethod.Instance.RoomId);//房间号
            msg.iParlorId = MahjongLobby_AH.GameData.Instance.ParlorShowPanelData.iParlorId;
            NetworkMgr.Instance.GameServer.SendClientUserBespeakReq(msg);
        }

        public void HandleBtnOutSeat(int index)
        {
            //发送取消占座消息
            NetMsg.ClientUserCancleBespeakReqDef msg = new NetMsg.ClientUserCancleBespeakReqDef();
            msg.byFromType = 2;//消息来源，1来自大厅，2来自游戏客户端
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId; // 用户编号
            msg.iSeatNum = (byte)(index);//座位号
            Debug.LogError("发送取消占座消息：" + msg.iSeatNum);
            msg.iRoomNum = Convert.ToInt32(MahjongCommonMethod.Instance.RoomId); ;//房间号
            NetworkMgr.Instance.GameServer.SendClientUserCancleBespeakReq(msg);
        }

        public void HandleBtnPlayReady()
        {
            //发送玩家准备按钮
            NetMsg.ClientReadyReq msg = new NetMsg.ClientReadyReq();
            msg.iUserId = GameData.Instance.PlayerPlayingPanelData.iUserId;
            Debug.Log ("发送玩家准备按钮");
            NetworkMgr.Instance.GameServer.SendReadyReq(msg);
        }

        /// <summary>
        /// 处理点击复制房间号码按钮
        /// </summary>
        void HandleBtnCopyRoomId()
        {
            MahjongCommonMethod.Instance.CopyString(MahjongCommonMethod.Instance.RoomId.ToString());
            //处理玩家复制成功之后提示文字
            MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", true);
        }
        /// <summary>
        /// 处理点击邀请按钮
        /// </summary>
        void HandleBtnShareWx()
        {
            string url = MahjongLobby_AH.SDKManager.WXInviteUrl + MahjongCommonMethod.Instance.RoomId;
            // Debug.LogError("分享链接" + url);
            string city;
            try
            {
                city = MahjongLobby_AH.GameData.Instance.SelectAreaPanelData.listCityMessage[MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iCityId].cityName;
            }
            catch (System.Exception)
            {
                city = "双喜";
            }
            StringBuilder title = new StringBuilder();
            // Debug.LogError("玩法Num" + MahjongCommonMethod.Instance.iPlayingMethod);
            if (MahjongLobby_AH.SDKManager.Instance.CheckStatus == 1)
            {
                title.AppendFormat("{0}麻将->{1}<-房号：{2} 点击进入房间", city, "推倒胡", MahjongCommonMethod.Instance.RoomId);
            }
            else
            {
                title.AppendFormat("{0}麻将->{1}<-房号：{2} 点击进入房间", city, MahjongCommonMethod.Instance._dicMethodConfig[MahjongCommonMethod.Instance.iPlayingMethod].METHOD_NAME, MahjongCommonMethod.Instance.RoomId);
            }
            #region 配置
            StringBuilder discription = new StringBuilder();
            Debug.LogWarning("老板标志位" + GameData.Instance.PlayerPlayingPanelData.byOpenRoomMode);
            int opmd = GameData.Instance.PlayerPlayingPanelData.byOpenRoomMode;
            if (opmd == 2)
            {
                discription.Append("【老板开房】");
            }
            //&& MahjongLobby_AH.GameData.Instance.ParlorShowPanelData.GetNowMahjongPavilionID()  == MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId)

            discription.Append("玩法：");
            CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;
          //  Debug.LogWarning("AA支付" + cd.roomMessage_[13]);
            //if (cd.roomMessage_[13] == 1)
           // {
              //  discription.Append("AA支付");
           // }
            MahjongCommonMethod.Instance.ShowParamOfOpenRoom(ref  discription, cd.roomMessage_ ,0 ,MahjongCommonMethod.Instance.iPlayingMethod );
           // MahjongCommonMethod.Instance.GetWeiteInfoForMethodID(discription, cd.MethodId, cd.roomMessage_);

            #endregion 配置
           // Debug.LogError("===" + discription);
            if (cd.roomMessage_[4] > 0)
            {
                discription.Append("预约 " + cd.roomMessage_[4] + " 分钟 ");
            }

            if (cd.roomMessage_[5] > 0)
            {
                discription.Append("托管 " + cd.roomMessage_[5] + " 分钟");
            }
            MahjongLobby_AH.SDKManager.Instance.HandleShareWX(url, title.ToString(), discription.ToString(), 0, 0, 22, "");
        }
        /// <summary>
        /// 处理点击返回大厅按钮
        /// </summary>
        void HandleBtnReturnLobby()
        {
            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载大厅资源，请稍候...");
#if UNITY_EDITOR
#else
             YunvaIM.YunVaImSDK.instance.YunVaLogOut();
#endif        

            //断开游戏服务器
            if (NetworkMgr.Instance && NetworkMgr.Instance.GameServer.Connected)
            {
                MahjongCommonMethod.isIntivateDisConnct = true;
                NetworkMgr.Instance.GameServer.Disconnect();
            }

            //清空gc垃圾
            System.GC.Collect();

            SceneManager_anhui.Instance.OpenPointScene(ESCENE.MAHJONG_LOBBY_GENERAL);

            //处理返回大厅            
            //SceneManager.Instance.LoadScene(ESCENE.MAHJONG_LOBBY_GENERAL);
            MahjongCommonMethod.Instance.isFinshSceneLoad = false;
        }


        /// <summary>
        /// 处理点击解散房间按钮
        /// </summary>
        void HandleBtnDissolveRoom()
        {
            //发送玩家申请解散房间请求
            NetMsg.ClientDismissRoomReq msg = new NetMsg.ClientDismissRoomReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.cType = 1;
            NetworkMgr.Instance.GameServer.SendDisMissRoomReq(msg);
        }
        //更新显示吃碰杠胡
        public void SpecialTileNoticeShow(int[] status)
        {
            if (OnPlayerPlayingSpecialTileNoticeShow != null)
            {
                OnPlayerPlayingSpecialTileNoticeShow(status);
            }
        }
        /// <summary>
        /// 选择成十三幺
        /// </summary>
        void HandleBtnChoiceThirteen()
        {
            NetMsg.ClientThirteenOrphansReq msg = new NetMsg.ClientThirteenOrphansReq();
            msg.iUserId = MahjongCommonMethod.Instance.iUserid;
            msg.byThirteenOrphans = 1;
            NetworkMgr.Instance.GameServer.SendThirteenOrphansRes(msg);
        }
        /// <summary>
        /// 取消选择十三幺
        /// </summary>
        void HandleBtnCancalThirteen()
        {
            NetMsg.ClientThirteenOrphansReq msg = new NetMsg.ClientThirteenOrphansReq();
            msg.iUserId = MahjongCommonMethod.Instance.iUserid;
            msg.byThirteenOrphans = 0;
            NetworkMgr.Instance.GameServer.SendThirteenOrphansRes(msg);
        }
        /// <summary>
        /// 修改游戏音量
        /// </summary>
        void HandleChangeMusic(float volume)
        {
            MahjongCommonMethod.Instance.MusicVolume = (int)(volume * 100f);
            SystemMgr.Instance.BgmSystem.UpdateVolume();
        }
        /// <summary>
        /// 改变音效的大小
        /// </summary>
        /// <param name="volume"></param>
        void HandleChangeEffect(float volume)
        {
            MahjongCommonMethod.Instance.EffectValume = (int)(volume * 100f);
            SystemMgr.Instance.AudioSystem.UpdateVolume();

        }
        /// <summary>
        /// 改变语音音量
        /// </summary>
        /// <param name="volume"></param>
        void HandleChangeVoice(float volume)
        {
            MahjongCommonMethod.Instance.LastVoiceValume = (int)(volume * 100f);
        }

        /// <summary>
        /// 处理设置音乐
        /// </summary>
        void HandleSetMusic()
        {
            MahjongCommonMethod com = MahjongCommonMethod.Instance;
            if (com.isMusicShut)
            {
                com.isMusicShut = false;
                // AudioListener.pause=false ;
            }
            else
            {
                com.isMusicShut = true;
                // AudioListener.pause  = true ;
            }
            SystemMgr.Instance.BgmSystem.UpdateVolume();
        }

        /// <summary>
        /// 处理设置音效
        /// </summary>
        void HandleSetMusicEffect()
        {
            MahjongCommonMethod uipd = MahjongCommonMethod.Instance;
            if (uipd.isEfectShut)
            {
                uipd.isEfectShut = false;
            }
            else
            {
                uipd.isEfectShut = true;
            }
            SystemMgr.Instance.AudioSystem.UpdateVolume();
        }
        /// <summary>
        /// 设置语音
        /// </summary>
        void HandleSetVoice()
        {
            MahjongCommonMethod uipd = MahjongCommonMethod.Instance;
            if (uipd.isOpenVoicePlay)
            {
                uipd.isOpenVoicePlay = false;
            }
            else
            {
                uipd.isOpenVoicePlay = true;
            }
        }
        /// <summary>
        /// 处理点击房间规则按钮
        /// </summary>
        void HandleBtnRoomRule()
        {
            //显示房间的规则
            //UIMainView.Instance.PlayerPlayingPanel.GameRulePanel.SetActive(true);
            UIMainView.Instance.playeringRule.UpdateShowMethod();
        }


        /// <summary>
        /// 处理点击防作弊按钮
        /// </summary>
        void HandleAntiCheating()
        {
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            acpd.isPanelShow = true;
            acpd.iReadyShowAnti++;
            SystemMgr.Instance.AntiCheatingSystem.UpdateShow();
            UIMainView.Instance.AntiCheatingPanel.InitPanel();
        }


        /// <summary>
        /// 处理点击吃碰杠胡的按钮
        /// </summary>
        /// <param name="index"></param>
        void HandleBtnSpecialCard(int index)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;


            //Debug.LogError("下标:" + index);

            //处理听牌的消息
            if (index == 7)
            {
                NetMsg.ClientReadHandReq msgg = new NetMsg.ClientReadHandReq();
                msgg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                NetworkMgr.Instance.GameServer.SendClientReadHandReq(msgg);
                MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                return;
            }

            if (index == 0)
            {
                //如果玩家听牌之后点击过，直接把所有听牌的显示取消
                if (MahjongHelper.Instance.specialValue_[6] == 1)
                {
                    Debug.LogError("如果玩家听牌之后点击过，直接把所有听牌的显示取消");
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongManger.Instance.HideTingLogo();
                }

                //如果点击了过胡
                if (index == 0 && MahjongHelper.Instance.specialValue_[3] == 1 && pppd.isNeedSendPassMessage)
                {
                    //显示过胡信息
                    UIMainView.Instance.PlayerPlayingPanel.ShowSkipWin(true);
                }

                if (!pppd.isSendPlayerPass)
                {
                    MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);

                    pppd.isSendPlayerPass = true;

                    if (!pppd.isNeedSendPassMessage)
                    {
                        return;
                    }
                }
            }

            if(index==1)
            {
                pppd.isGangToPeng_Sort = true;
            }

            if (index == 2)
            {
                if (pppd.isGangAndPengEnable)
                {
                    pppd.isGangToPeng_Sort = true;
                    pppd.isGangToPeng_Put = true;
                }
            }

            if (index == 2 || index == 3)
            {
                pppd.iTirState = 1;
            }
            if (index == 1 || index == 6)
            {
                pppd.iTirState = 2;
            }
            byte[] mah = new byte[2];
            //处理杠
            if (index == 3)
            {
                mah = new byte[] { pppd.bkongValue, pppd.bkongValue };
            }
            else
            {
                mah = new byte[] { pppd.bLastCard, pppd.bLastCard };
            }

            if (!pppd.isSendPlayerPass)
            {
                pppd.isSendPlayerPass = true;
            }

            NetMsg.ClientSpecialTileReq msg = new NetMsg.ClientSpecialTileReq();
            msg.byPongKong = (byte)pppd.IsPongKong();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.byaTiles = mah;
            msg.bySpecial = (byte)index;

            NetworkMgr.Instance.GameServer.SendSpecialTileReq(msg);
            //处理玩家点击吃碰杠胡之后，直接删除按钮图标
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
        }


        /// <summary>
        /// 是否要显示查看听牌的按钮
        /// </summary>
        /// <param name="bySuit">打出的牌值,如果是0，直接显示按钮</param>
        /// <returns></returns>
        public bool isShowTingBtn(byte bySuit = 0)
        {           
            //如果玩法需报听可胡 ，如果玩家不报听 ，不显示查看停牌的按钮
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinLimitReadHand >0)
            {
                if (GameData.Instance.PlayerPlayingPanelData.isChoiceTing == 0)
                {
                    return false;
                }               
            }

            //如果玩家没有可听的牌，直接不显示
            if (MahjongHelper.Instance.mahjongTing.Count == 0)
            {
                return false;
            }

            if (bySuit == 0)
            {
                return true;
            }


            for (int i = 0; i < MahjongHelper.Instance.Ting.Count; i++)
            {
                if (bySuit == MahjongHelper.Instance.Ting[i])
                {
                    //Debug.LogError("-bySuit-" + bySuit+","+ MahjongHelper.Instance.Ting[i]);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 处理点击查看听牌按钮
        /// </summary>
        void HandleLookTingCard_Down()
        {
           // MahjongHelper.Instance.TingCount.Sort(MahjongHelper.Instance.SortTingMessage);
            //显示玩家的听牌具体信息    
            UIMainView.Instance.PlayerPlayingPanel.SpwanTingShow(MahjongHelper.Instance.TingCount.ToArray());
        }

        /// <summary>
        /// 处理点击查看听牌按钮
        /// </summary>
        void HandleLookTingCard_Up()
        {
            //显示玩家的听牌具体信息    
            UIMainView.Instance.PlayerPlayingPanel.TingShow.SetActive(false);
        }
    }
}

