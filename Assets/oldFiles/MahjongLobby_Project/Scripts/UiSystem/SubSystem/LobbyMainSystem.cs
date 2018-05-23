using UnityEngine;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using DG.Tweening;
using System.Collections.Generic;
using System.Text;
using MahjongLobby_AH.Network;
using System;
using XLua;
using anhui;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class LobbyMainSystem : GameSystemBase
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
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_REALNAME, HandleRealNameSure);

                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_SHARE, HandleShareWX);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_GETGIFTBAG, HandleGetSpreadGiftBag);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_PALYINGMETHOD, HandlePlayingMehod);
                    Messenger_anhui<int>.AddListener(MainViewLobbyPanel.MESSAGE_PRODUCTAGENCY, HandleProductAgency);
                    Messenger_anhui<int>.AddListener(MainViewLobbyPanel.MESSAGE_PRODUCTGENE, HandleProductGene);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_PLAYERMESSAGE, HandlePlayerMessageBtn);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_PLAYERFestivalActivity, HandleFestivalActivity);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_HolidayACTIVITYBTN, HandleActivityBtn);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_HISTROYGRADE, HandleHistroyGrade);
                    Messenger_anhui<int>.AddListener(MainViewLobbyPanel.MESSAGE_CREATROOM, HandleCreatRoom);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_RETURNROOM, HandleReturnRoom);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_JOINROOM, HandleJoinRoom);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_INSTEADCREATROOM, HandleInsteadCreatRoom);
                    //Messenger<int>.AddListener(MainViewLobbyPanel.MESSAGE_MOREBTN, HandleMoreBtn);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_HEADTITLE, HandleHeadTitle);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_CUSTOMSEVERBTN, HandleCustomSeverBtn);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_BUYROOMCARD, HandleBtnBuyRoomCrad);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_CLOSEBUYROOMCARD, HandleBtnCloseBuyRoomCardPanel);
                    Messenger_anhui<int>.AddListener(MainViewLobbyPanel.MESSAGE_BUYCARD, HandleBtnSendOrderReq);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_OPENPARLOR, HandleBtnOpenParlor);
                    Messenger_anhui.AddListener(MainViewLobbyPanel.MESSAGE_REDPAGE, HandleRedPage);
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
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_REALNAME, HandleRealNameSure);

                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_SHARE, HandleShareWX);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_GETGIFTBAG, HandleGetSpreadGiftBag);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_PALYINGMETHOD, HandlePlayingMehod);
                    Messenger_anhui<int>.RemoveListener(MainViewLobbyPanel.MESSAGE_PRODUCTAGENCY, HandleProductAgency);
                    Messenger_anhui<int>.RemoveListener(MainViewLobbyPanel.MESSAGE_PRODUCTGENE, HandleProductGene);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_PLAYERMESSAGE, HandlePlayerMessageBtn);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_PLAYERFestivalActivity, HandleFestivalActivity);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_HolidayACTIVITYBTN, HandleActivityBtn);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_HISTROYGRADE, HandleHistroyGrade);
                    Messenger_anhui<int>.RemoveListener(MainViewLobbyPanel.MESSAGE_CREATROOM, HandleCreatRoom);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_RETURNROOM, HandleReturnRoom);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_JOINROOM, HandleJoinRoom);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_INSTEADCREATROOM, HandleInsteadCreatRoom);
                    //Messenger<int>.RemoveListener(MainViewLobbyPanel.MESSAGE_MOREBTN, HandleMoreBtn);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_HEADTITLE, HandleHeadTitle);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_CUSTOMSEVERBTN, HandleCustomSeverBtn);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_BUYROOMCARD, HandleBtnBuyRoomCrad);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_CLOSEBUYROOMCARD, HandleBtnCloseBuyRoomCardPanel);
                    Messenger_anhui<int>.RemoveListener(MainViewLobbyPanel.MESSAGE_BUYCARD, HandleBtnSendOrderReq);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_OPENPARLOR, HandleBtnOpenParlor);
                    Messenger_anhui.RemoveListener(MainViewLobbyPanel.MESSAGE_REDPAGE, HandleRedPage);
                    break;
                default:
                    break;
            }
        }
        public override void Init()
        {
            base.Init();
        }


        #endregion 事件处理

        //通知面板更新事件
        public delegate void LobbyMainUpdateEventHandler();
        public LobbyMainUpdateEventHandler OnLobbyMainUpdate;


        public void UpdateShow()
        {
            if (OnLobbyMainUpdate != null)
            {
                OnLobbyMainUpdate();
            }
        }

        /// <summary>
        /// 处理玩家点击实名认证按钮
        /// </summary>
        void HandleRealNameSure()
        {
            GameData gd = GameData.Instance;
            RealNameApprovePanelData rnapd = gd.RealNameApprovePanelData;
            rnapd.PanelShow = true;
            SystemMgr.Instance.RealNameApproveSystem.UpdateShow();
            PlayerPrefs.SetFloat(GameData.RedPoint.RealName.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 2);
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }

        /// <summary>
        /// 处理玩家点击分享按钮
        /// </summary>
        void HandleShareWX()
        {
            GameData gd = GameData.Instance;
            ShareWxPanelData swpd = gd.ShareWxPanelData;
            swpd.PanelShow = true;
            SystemMgr.Instance.ShareWxSystem.UpdateShow();
            PlayerPrefs.SetFloat(GameData.RedPoint.ShareBag.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 2);
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }

        /// <summary>
        /// 处理推广领取礼包按钮
        /// </summary>
        void HandleGetSpreadGiftBag()
        {
            GameData gd = GameData.Instance;
            GetGiftSpreadBagPanelData ggbpd = gd.GetGiftSpreadBagPanelData;

            if (gd.PlayerNodeDef.iSpreaderId > 0 && gd.PlayerNodeDef.iSpreadGiftTime == 0)
            {
                ggbpd.GiftSpreadCode = GameData.Instance.VerifyCode(gd.PlayerNodeDef.iSpreaderId);
            }
            Debug.LogWarning ("id:" + GameData.Instance.PlayerNodeDef.iSpreaderId + ",time:" + GameData.Instance.PlayerNodeDef.iSpreadGiftTime);
            if (GameData.Instance.PlayerNodeDef.iSpreaderId > 0 && GameData.Instance.PlayerNodeDef.iSpreadGiftTime == 0)//已经被推广
            {
                GameData.Instance.GetGiftSpreadBagPanelData.isShared = true;
                NetMsg.ClientSpreaderInfoReq msg = new NetMsg.ClientSpreaderInfoReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iSpreaderId = GameData.Instance.PlayerNodeDef.iSpreaderId;
                NetworkMgr.Instance.LobbyServer.SendClientSpreaderInfoReq(msg);
            }

            if (GameData.Instance.PlayerNodeDef.iSpreaderId == 0 && GameData.Instance.PlayerNodeDef.iSpreadGiftTime == 0)//未被推广
            {
                GameData.Instance.GetGiftSpreadBagPanelData.isShared = false;
            }


            ggbpd.PanelShow = true;
            SystemMgr.Instance.GetGiftSpreadBagSystem.UpdateShow();
        }

        void HandleRedPage()
        {
            if (GameData.Instance.PlayerNodeDef.iUserId <= 0 || SDKManager.Instance.IOSCheckStaus == 1)
                return;
            NetMsg.ClientRedNumReqDef msg = new NetMsg.ClientRedNumReqDef();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            NetworkMgr.Instance.LobbyServer.SendClientRedNumReq(msg);
        }


        /// <summary>
        /// 处理点击玩法按钮
        /// </summary>
        void HandlePlayingMehod()
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            SelectAreaPanelData sapd = gd.SelectAreaPanelData;
            ParlorShowPanelData pspd = gd.ParlorShowPanelData;
            if (crmpd.CreatRoomType == 1)
            {
                gpmpd.CountyId = sapd.iCountyId;
                string[] id = MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].METHOD.Split('_');
                MahjongCommonMethod.Instance.lsMethodId = new List<int>();
                for (int i = 0; i < id.Length; i++)
                {
                   // Debug.LogWarning("处理点击玩法按钮1");
                    MahjongCommonMethod.Instance.lsMethodId.Add(System.Convert.ToInt32(id[i]));
                }
            }
            else
            {
                gpmpd.CountyId = pspd.iCountyId[2];
            }

            if (gpmpd.Status != 1)
            {
                GameData.Instance.CreatRoomMessagePanelData.MethodId = System.Convert.ToInt32(MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].METHOD.Split('_')[0]);
                string[] id = MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].METHOD.Split('_');
                MahjongCommonMethod.Instance.lsMethodId = new List<int>();
                for (int i = 0; i < id.Length; i++)
                {
                   // Debug.LogWarning("处理点击玩法按钮2");
                    MahjongCommonMethod.Instance.lsMethodId.Add(System.Convert.ToInt32(id[i]));
                }
            }
            else
            {
                gpmpd.Status = 0;
            }

            //如果玩家的没有可玩的玩法
            if (MahjongCommonMethod.Instance.lsMethodId.Count <= 0)
            {
                if (SDKManager.Instance.IOSCheckStaus == 1)
                {
                    //获取所有城市的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_city
                        , null, gpmpd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_county
                        , null, gpmpd.GetCountyMessage, "CountyData");
                }
                else
                {
                    //获取所有城市的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_city
                        , null, gpmpd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_county
                        , null, gpmpd.GetCountyMessage, "CountyData");
                }

            }
            else
            {
                gpmpd.PanelShow = true;
                //gpmpd.GameIndex = 1; //默认打开第一个            
                SystemMgr.Instance.GamePlayingMethodSystem.UpdateShow(0);
            }
        }

        /// <summary>
        /// 处理点击代理按钮
        /// </summary>
        void HandleProductAgency(int index)
        {
            //处理如果玩家是一级代理或者是授权代理，直接跳转会员中心
            PlayerPrefs.SetFloat(GameData.RedPoint.SuccessBindDaili.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 2);
            if (index == 2)
            {
                Messenger_anhui<int>.Broadcast(MainViewLobbyPanel.MESSAGE_PRODUCTGENE, 2);
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                return;
            }
            GameData gd = GameData.Instance;
            ProductAgencyPanelData papd = gd.ProductAgencyPanelData;
            papd.PanelShow = true;
            papd.index = index;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
        }

        /// <summary>
        /// 处理点击会员中心按钮
        /// </summary>
        void HandleProductGene(int status)
        {
            GameData gd = GameData.Instance;

            //游客登录,不要使用认证方式，进入游戏之后，会改为3
            if (gd.PlayerNodeDef.byUserSource == 1)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("请使用微信登录后，点击进入会员中心");
                return;
            }

            if (status == 1)
            {
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.Promote);
            }
            StringBuilder str = new StringBuilder();
            str.Append(LobbyContants.URL_MEMBERCENTER);
            str.Append("uid=");
            str.Append(gd.PlayerNodeDef.iUserId);
            str.Append("&t=");
            str.Append(status);
            str.Append("&token=");
            Debug.LogWarning("gd.PlayerNodeDef.szAccessToken：" + gd.PlayerNodeDef.szAccessToken);
            str.Append(gd.PlayerNodeDef.userDef .szAccessToken);
            Application.OpenURL(str.ToString());
        }

        /// <summary>
        /// 点击反馈按钮
        /// </summary>
        void HandleCustomSeverBtn()
        {

            GameData gd = GameData.Instance;
            CustomPanelData bspd = gd.CustomPanelData;
            bspd.PanelShow = true;

            SystemMgr.Instance.CustomSystem.UpdateShow();
        }

        /// <summary>
        /// 处理大厅的玩家消息的按钮
        /// </summary>
        void HandlePlayerMessageBtn()
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            MessageReq(1);
            pmpd.timer = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
            //把时间写入请求时间
            PlayerPrefs.SetInt("time_m" + GameData.Instance.PlayerNodeDef.iUserId, (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now));
            UIMainView.Instance.LobbyPanel.RedPoint[1].gameObject.SetActive(false);
        }

        /// <summary>
        /// 活动按钮
        /// </summary>
        void HandleFestivalActivity()
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            //pmpd.timer = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
            //把时间写入请求时间

#if UNITY_EDITOR
            UIMainView.Instance.FestivalActivity.gameObject.SetActive(true);
#else
            if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 2)//如果是微信
            {
                UIMainView.Instance.FestivalActivity.gameObject.SetActive(true);
            }
            else
            {
                UIMgr.GetInstance().GetUIMessageView().Show("请使用微信登陆参与活动!");
            }
#endif
        }

        /// <summary>
        /// 请求玩家消息内容
        /// </summary>
        /// <param name="status">1表示每隔五分钟请求</param>
        public void MessageReq(int status)
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            string url = LobbyContants.MAJONG_PORT_URL + pmpd.url_suffix;
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + pmpd.url_suffix;
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("uid", gd.PlayerNodeDef.iUserId.ToString());
            //value.Add("uid", "10000940");
            //点击玩家消息按钮，获取消息的对应数据
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, pmpd.GetMessageData, pmpd.json_title, status);
        }

        /// <summary>
        /// 处理大厅的活动按钮
        /// </summary>
        void HandleActivityBtn()
        {
            //  if (MahjongCommonMethod.isGameToLobby)
            UIMainView.Instance.HolidayActivityPanel.GetActivityJson();
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickActivity);
            GameData gd = GameData.Instance;
            //开活动面板
            HolidayActivityPanelData hapd = gd.HolidayActivityPanelData;
            hapd.isPanelShow = true;
            SystemMgr.Instance.HolidayActivitySystem.UpdateShow();
        }

        /// <summary>
        /// 处理点击战绩按钮
        /// </summary>
        void HandleHistroyGrade()
        {
            //显示战绩面板，同时产生玩家战绩
            GameData gd = GameData.Instance;
            HistroyGradePanelData hgpd = gd.HistroyGradePanelData;

            hgpd.isPanelShow = true;
            hgpd.isShowGrade_Room = true;
            hgpd.isShowGrade_Round = false;
            hgpd.BtnCloseStatus = 2;
            hgpd.timer = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
            //把时间写入请求时间
            PlayerPrefs.SetInt("time_h" + GameData.Instance.PlayerNodeDef.iUserId, (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now));

            //Debug.LogError("time_h:" + PlayerPrefs.GetInt("time_h"));

            UIMainView.Instance.LobbyPanel.RedPoint[2].gameObject.SetActive(false);
            SystemMgr.Instance.HistroyGradeSystem.UpdateShow();
        }

        /// <summary>
        /// 点击创建房间按钮
        /// </summary>
        void HandleCreatRoom(int createroom)
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            //对应县区选择玩法
            MahjongCommonMethod.Instance.lsMethodId = new List<int>();
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            int countId = 0;
            if (createroom == 1)
            {
                countId = sapd.iCountyId;
            }
            else
            {
                countId = GameData.Instance.ParlorShowPanelData.iCountyId[2];
            }

            string[] id = MahjongCommonMethod.Instance._dicDisConfig[countId].METHOD.Split('_');
            for (int k = 0; k < id.Length; k++)
            {
                int ID = System.Convert.ToInt16(id[k]);
                if (ID != 0)
                {

                    MahjongCommonMethod.Instance.lsMethodId.Add(ID);
                }
            }
            crmpd.CreatRoomType = createroom;
            crmpd.PanelShow = true;
            UIMainView.Instance.CreatRoomMessagePanel.m_CreateRoomSource = createroom;
            //if (PlayerPrefs.HasKey("OpenRoomMsgg"))
            //{
            //    //获取玩家上次创建房间的默认参数
            //    string message = PlayerPrefs.GetString("OpenRoomMsgg");
            //    if (PlayerPrefs.HasKey("iPlayingMethod"))
            //    {
            //        crmpd.iParamMethid = PlayerPrefs.GetInt("iPlayingMethod");
            //    }
            //    string[] msgs = message.Split(',');
            //    if (crmpd.iParamMethid > 0)
            //    {
            //        for (int i = 0; i < msgs.Length; i++)
            //        {
            //            crmpd.roomMessage_[i] = Convert.ToUInt32(msgs[i]);
            //        }
            //    }
            //    Debug.LogWarningFormat("{0}:{1}\n{2}:{3}\n{4}:{5}", crmpd.roomMessage_[0].ToString("X8"),
            //        crmpd.roomMessage_[1].ToString("X8"), crmpd.roomMessage_[2].ToString("X8"), crmpd.roomMessage_[3].ToString("X8"),
            //        crmpd.roomMessage_[4].ToString("X8"), crmpd.roomMessage_[5].ToString("X8")
            //        );
            //}
            //////////////////////////////////////////////////////////////////////
            if (UIMainView.Instance.CreatRoomMessagePanel.m_showNew[0] != null)
            {
                if (!PlayerPrefs.HasKey("CreatRoomShowNew" + GameData.Instance.PlayerNodeDef.iUserId))
                {
                    PlayerPrefs.SetInt("CreatRoomShowNew" + GameData.Instance.PlayerNodeDef.iUserId, 0);
                }
                int CreatRoomShowNew_num = PlayerPrefs.GetInt("CreatRoomShowNew" + GameData.Instance.PlayerNodeDef.iUserId);
                if (CreatRoomShowNew_num <= 3)
                {
                    UIMainView.Instance.CreatRoomMessagePanel.m_showNew[0].SetActive(true);
                    CreatRoomShowNew_num++;
                    PlayerPrefs.SetInt("CreatRoomShowNew" + GameData.Instance.PlayerNodeDef.iUserId, CreatRoomShowNew_num);
                }
                else
                {
                    MonoBehaviour.Destroy(UIMainView.Instance.CreatRoomMessagePanel.m_showNew[0]);
                    PlayerPrefs.DeleteKey("CreatRoomShowNew" + GameData.Instance.PlayerNodeDef.iUserId);
                }
            }
            //////////////////////////////////////////////////////////////////////
            SystemMgr.Instance.CreatRoomMessageSystem.UpdateShow();
            UIMainView.Instance.CreatRoomMessagePanel.UpdateShowMethod(GameData.Instance.CreatRoomMessagePanelData.MethodId);

        }


        /// <summary>
        /// 对List<CAttributeFeature> 进行排序时作为参数使用
        /// </summary>
        /// <param name = "AF1" ></ param >
        /// < param name="AF2"></param>
        /// <returns></returns>
        internal int SortCompare(LobbyMainPanelData.IVoucherData AF1, LobbyMainPanelData.IVoucherData AF2)
        {
            //是否使用过 0 未使用 1使用
            if (AF1.isFirst > AF2.isFirst)
                return -1;
            else if (AF1.isFirst < AF2.isFirst)
                return 1;

            if (AF1.state < AF2.state)
                return -1;
            else if (AF1.state > AF2.state)
                return 1;

            if (AF1.iCanUse > AF2.iCanUse)
                return -1;
            else if (AF1.iCanUse < AF2.iCanUse)
                return 1;
            //如果状态相同比较金额
            if (AF1.amount > AF2.amount)
                return -1;
            else if (AF1.amount < AF2.amount)
                return 1;
            if (AF1.exTime > AF2.exTime)
                return -1;
            else
                return 1;
        }
        internal void FindFirst(int amout)
        {
            int i = -1;
            Debug.LogWarning("FindFirst:amout" + amout);
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            for (int k = 0; k < lmpd._dicVoucher.Count; k++)
            {
                if (amout > lmpd._dicVoucher[k].limit && MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) < (ulong)lmpd._dicVoucher[k].exTime && lmpd._dicVoucher[k].state == 0)
                {
                    if (i == -1)
                    {
                        i = k;
                    }
                    else if (lmpd._dicVoucher[k].amount > lmpd._dicVoucher[i].amount)
                    {
                        i = k;
                    }
                    else if (lmpd._dicVoucher[k].amount == lmpd._dicVoucher[i].amount)
                    {
                        if (lmpd._dicVoucher[k].exTime < lmpd._dicVoucher[i].exTime)
                        {
                            i = k;
                        }
                    }
                }
            }
            if (amout == 0)
            {
                amout = 9999999;
            }
            ulong ext = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(MahjongCommonMethod.Instance._DateTime);
            for (int k = 0; k < lmpd._dicVoucher.Count; k++)
            {
                if (k == i)
                {
                    lmpd._dicVoucher[k].isFirst = 1;
                }
                else
                {
                    lmpd._dicVoucher[k].isFirst = 0;
                }

                // Debug.LogError(lmpd._dicVoucher[k].limit + "  " + amout + "===" + lmpd._dicVoucher[k].exTime + "  " + ext);
                if (lmpd._dicVoucher[k].limit < amout && (ulong)lmpd._dicVoucher[k].exTime > ext)
                {
                    lmpd._dicVoucher[k].iCanUse = 1;
                }
                else
                {
                    lmpd._dicVoucher[k].iCanUse = 0;
                }
            }
        }
        /// <summary>
        /// 点击返回房间按钮
        /// </summary>
        void HandleReturnRoom()
        {
            //向服务器请求游戏服务器信息
            //请求游戏服务器信息
            NetMsg.ClientGameServerInfoReq msgg = new NetMsg.ClientGameServerInfoReq();
            msgg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msgg.iServerId = MahjongCommonMethod.Instance.iSeverId;
            Network.NetworkMgr.Instance.LobbyServer.SendGameSeverInfoReq(msgg);
        }

        /// <summary>
        /// 点击加入房间按钮
        /// </summary>
        void HandleJoinRoom()
        {
            JoinRoomShowPanelData jrpd = GameData.Instance.JoinRoomShowPanelData;
            jrpd.PanelShow = true;
            SystemMgr.Instance.JoinRoomShowSystem.UpdateShow();
        }

        /// <summary>
        /// 点击代开房间按钮
        /// </summary>
        void HandleInsteadCreatRoom()
        {
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            PlayerNodeDef pnd = gd.PlayerNodeDef;
            InsteadOpenRoomPanelData iorpd = gd.InsteadOpenRoomPanelData;
            iorpd.OpenRoomInfo_UnStart.Clear();

            PlayerPrefs.SetFloat(GameData.RedPoint.FirstOpenInsteadPanel.ToString() + GameData.Instance.PlayerNodeDef.iUserId, 2);


            ////如果不是代理，让玩家查看代理规则
            //if(pnd.iIsProxy != 1)
            //{
            //    UIMgr.GetInstance().GetUIMessageView().Show("温馨提示", TextConstant.LOOKDAILI, DailiRule,null,0,false,-1,2,0);
            //    return;
            //}


            ////判断代理如果房卡数量小于0个，不让开房
            //if ((pnd.iRoomCard + pnd.iFreeCard) < 0)
            //{                
            //    UIMgr.GetInstance().GetUIMessageView().Show("您的金币数量不足，暂时不能使用代开房间功能哦！", ok);
            //    return;
            //}

            ////判断玩家是否是代理
            //if (pnd.iIsProxy == 1)
            //{
            //    //发送玩家的开房信息请求
            //    NetMsg.ClientOpenRoomInfoReq msg = new NetMsg.ClientOpenRoomInfoReq();
            //    msg.iUserId = pnd.iUserId;
            //    Network.NetworkMgr.Instance.LobbyServer.SendOpenRoomInfoReq(msg);
            //    iorpd.isClickInsteadOpenRoom = true;
            //}           
        }

        internal void OnPointerUp(GameObject go)
        {
            LobbyMainPanelData ld = GameData.Instance.LobbyMainPanelData;
            MainViewLobbyPanel lp = UIMainView.Instance.LobbyPanel;
            int indexCurrent = 1;
            int num = 2;//滑动总个数
            float upPos = lp.panels_joinUs._g[1].transform.localPosition.x;
            float lenth = lp.panels_joinUs._g[1].GetComponent<RectTransform>().sizeDelta.x;
            indexCurrent = upPos - ld.downPos >= 0 ? indexCurrent - 1 : indexCurrent + 1;
            indexCurrent = indexCurrent > num ? num : indexCurrent;
            indexCurrent = indexCurrent < 1 ? 1 : indexCurrent;

            switch (indexCurrent)
            {
                case 1:
                    lp.panels_joinUs._toglePoint[0].isOn = true;
                    lp.panels_joinUs._g[1].transform.DOLocalMoveX(ld.oriPos, 0.3f);
                    break;
                case 2:
                    lp.panels_joinUs._toglePoint[1].isOn = true;
                    lp.panels_joinUs._g[1].transform.DOLocalMoveX(ld.oriPos - lenth * 0.5f, 0.3f);
                    break;
                default:
                    break;
            }
        }

        //跳到我想成为代理的三级窗口
        void DailiRule()
        {
            //打开代理面板
            GameData.Instance.ProductAgencyPanelData.PanelShow = true;
            //打开成为代理面板
            GameData.Instance.ProductAgencyPanelData.IsShowCompany = true;
            SystemMgr.Instance.ProductAgencySystem.UpdateShow();
        }

        internal void OnPointerDown(GameObject go)
        {
            GameData.Instance.LobbyMainPanelData.downPos = UIMainView.Instance.LobbyPanel.panels_joinUs._g[1].transform.localPosition.x;
        }



        //跳到购买房卡
        void ok()
        {
            UIMainView.Instance.LobbyPanel.OpenChargePanel();
        }

        /// <summary>
        /// 点击打开购买房卡面板
        /// </summary>
        void HandleBtnBuyRoomCrad()
        {
            GameData gd = GameData.Instance;
            LobbyMainPanelData lmpd = gd.LobbyMainPanelData;
            lmpd.isShowBuyRoomCard = true;
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }


        /// <summary>
        /// 关闭购买房卡的面板
        /// </summary>
        void HandleBtnCloseBuyRoomCardPanel()
        {
            GameData gd = GameData.Instance;
            LobbyMainPanelData lmpd = gd.LobbyMainPanelData;
            lmpd.isShowBuyRoomCard = false;
           // Debug.LogError("关闭购买房卡的面板");
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }

        ///// <summary>
        ///// 处理更多按钮
        ///// </summary>
        //void HandleMoreBtn(int status)
        //{
        //    float pos_up = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.localPosition.y + 
        //        55f*MahjongCommonMethod.Instance.FindChildNum(UIMainView.Instance.LobbyPanel.MorePopPanel)+UIMainView.Instance.LobbyPanel.ClickBgHieight;
        //    float pos_down = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.localPosition.y-
        //        55f * MahjongCommonMethod.Instance.FindChildNum(UIMainView.Instance.LobbyPanel.MorePopPanel)- UIMainView.Instance.LobbyPanel.ClickBgHieight;
        //    if (status==1)
        //    {                
        //        Tweener tweener = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.DOLocalMoveY(pos_up, 0.5f);
        //        //Tweener tweener = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.DOLocalMoveY(80f-316f, 0.5f);
        //        tweener.SetEase(Ease.InSine);
        //        tweener.OnComplete(CallBack_1);
        //    }
        //    else if(status==2)
        //    {
        //        Debug.LogError("pos_down:" + pos_down);
        //        //Tweener tweener = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.DOLocalMoveY(pos_down, 0.5f);
        //        Tweener tweener = UIMainView.Instance.LobbyPanel.MorePopPanel.transform.DOLocalMoveY(pos_down , 0.5f);
        //        tweener.SetEase(Ease.InSine);
        //        tweener.OnComplete(CallBack_2);
        //    }
        //}

        ///// <summary>
        ///// 运动完之后的回调方法
        ///// </summary>
        //void CallBack_1()
        //{
        //    GameData gd = GameData.Instance;
        //    LobbyMainPanelData lmpd = gd.LobbyMainPanelData;
        //    lmpd.MoreStatus = 2;
        //    lmpd.MoveEnd = true;
        //}

        ///// <summary>
        ///// 运动完之后的回调方法
        ///// </summary>
        //void CallBack_2()
        //{
        //    GameData gd = GameData.Instance;
        //    LobbyMainPanelData lmpd = gd.LobbyMainPanelData;
        //    lmpd.MoreStatus = 1;
        //    lmpd.MoveEnd = true;
        //}

        /// <summary>
        /// 处理点击头像按钮的事件
        /// </summary>
        void HandleHeadTitle()
        {
            GameData gd = GameData.Instance;
            UserInfoPanelData uipd = gd.UserInfoPanelData;
            uipd.isPanelShow = true;
            SystemMgr.Instance.UserInfoSystem.UpdateShow();
            PlayerPrefs.SetInt("userInfoRed" + GameData.Instance.PlayerNodeDef.iUserId, 2);
        }
        /// <summary>
        /// 处理充值点击
        /// </summary>
        /// <param name="iChargeId">10018,10030,10060</param>
        void HandleBtnSendOrderReq(int iChargeId)
        {
            Debug.LogWarning("点击充值ichargeId:" + iChargeId);
            NetMsg.ClientCreateOrderReq msg = new NetMsg.ClientCreateOrderReq();
            NetMsg.ClientCreateOrderRes msgres = new NetMsg.ClientCreateOrderRes();
            msg.iUserId = MahjongCommonMethod.Instance.iUserid;
            msgres.iUserId = msg.iUserId;
            msg.iChargeMode = 1;
            msgres.iChargeMode = 1;
            msg.iChargeId = iChargeId;
            msgres.iChargeId = iChargeId;
            msg.iBoss = GameData.Instance.PlayerNodeDef.iMyParlorId > 0 ? 1 : 0;
            msgres.iBoss = msg.iBoss;
            Debug.LogWarning("msg.iBoss " + msg.iBoss);
            msg.iChargeNumber = MahjongCommonMethod.Instance._dicCharge[iChargeId].Price;
            msgres.iChargeNumber = msg.iChargeNumber;
            UIMainView.Instance.LobbyPanel.stackprice.Push(msgres);
            SDKManager.Instance.CreatChargeOderReq(msg);
           // NetworkMgr.Instance.LobbyServer.SendCreateOrderReq(msg);
        }

        /// <summary>
        /// 处理点击打开麻将馆
        /// </summary>
        void HandleBtnOpenParlor()
        {
            //如果是第一次进入麻将馆，显示自动弹出地图        
            if (GameData.Instance.PlayerNodeDef.userDef.byFirstInParlor == 0)
            {
                GameData.Instance.SelectAreaPanelData.iOpenStatus = 8;
                GameData.Instance.SelectAreaPanelData.isPanelShow = true;
                SystemMgr.Instance.SelectAreaSystem.UpdateShow();
            }
            else
            {
                //获取玩家的申请馆的id
                GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("loading", "正在获取您的麻将馆信息");
            }
        }

    }
}

