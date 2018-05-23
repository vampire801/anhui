using UnityEngine;
using System;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorShowSystem : GameSystemBase
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
                    Messenger_anhui<int>.AddListener(MainViewParlorShowPanel.MESSAGE_RETURNLOBBY, HandleBtnReturnLobby);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_WHATRULE, HandleBtnRuleContent);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_PARLORGROUND, HandleBtnParlorRound);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_CREATPARLOR, HandleBtnCreatParlor);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_APPLYCREATPARLOR, HandleBtnApplyCreatParlor);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_SECONDSUREBTN, HandleBtnSecondSureCreatParlor);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_CREATPARLORBTN, HandleBtnSureCreatParlor);
                    Messenger_anhui<int>.AddListener(MainViewParlorShowPanel.MESSAGE_CHANGEAREASETTING, HandleBtnChangeAreaSetting);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_DISMISSPARLOR, HandleBtnDismissParlor);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_CHANGEPARLORCONTACTANDBULLTIEN, HandleBtnChangeParlorMessage);
                    Messenger_anhui<int>.AddListener(MainViewParlorShowPanel.MESSAGE_PARLORORDERTYPE, HandleBtnParlorOrderType);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_LEVELPARLOR, HandleLevelParlor);
                    Messenger_anhui<string>.AddListener(MainViewParlorShowPanel.MESSAGE_SEARCHMEMBER, HandleParlorerSearchMember);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_INVITEMEMBER, HandleInviteMember);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_KICKMEMBER, HandleKickMember);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_MEMBERMANAGER, HandleParlorMemberManger);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_CHECKLIST, HandleParlorCheckList);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_GAMERECORD, HandleParlorGameRecord);
                    Messenger_anhui.AddListener(MainViewParlorShowPanel.MESSAGE_PARLORREDBAG, HandleParlorGameRecord);
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
                    Messenger_anhui<int>.RemoveListener(MainViewParlorShowPanel.MESSAGE_RETURNLOBBY, HandleBtnReturnLobby);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_WHATRULE, HandleBtnRuleContent);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_PARLORGROUND, HandleBtnParlorRound);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_CREATPARLOR, HandleBtnCreatParlor);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_APPLYCREATPARLOR, HandleBtnApplyCreatParlor);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_SECONDSUREBTN, HandleBtnSecondSureCreatParlor);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_CREATPARLORBTN, HandleBtnSureCreatParlor);
                    Messenger_anhui<int>.RemoveListener(MainViewParlorShowPanel.MESSAGE_CHANGEAREASETTING, HandleBtnChangeAreaSetting);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_DISMISSPARLOR, HandleBtnDismissParlor);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_CHANGEPARLORCONTACTANDBULLTIEN, HandleBtnChangeParlorMessage);
                    Messenger_anhui<int>.RemoveListener(MainViewParlorShowPanel.MESSAGE_PARLORORDERTYPE, HandleBtnParlorOrderType);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_LEVELPARLOR, HandleLevelParlor);
                    Messenger_anhui<string>.RemoveListener(MainViewParlorShowPanel.MESSAGE_SEARCHMEMBER, HandleParlorerSearchMember);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_INVITEMEMBER, HandleInviteMember);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_KICKMEMBER, HandleKickMember);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_MEMBERMANAGER, HandleParlorMemberManger);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_CHECKLIST, HandleParlorCheckList);
                    Messenger_anhui.RemoveListener(MainViewParlorShowPanel.MESSAGE_GAMERECORD, HandleParlorGameRecord);
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
        public delegate void ParlorShowUpdateEventHandler();
        public ParlorShowUpdateEventHandler OnParlorShowUpdate;

        public delegate void MyParlorPanelUpdateEventHandler(Network.Message.NetMsg.ParlorInfoDef msg);
        public MyParlorPanelUpdateEventHandler OnMyParlorPanelUpdate;

        public delegate void ParlorRedBagUpdateEventHandle(int iParlorId, int timer);
        public ParlorRedBagUpdateEventHandle OnParlorRedBagUpdate;

        public void UpdateShow()
        {
            if (OnParlorShowUpdate != null)
            {
                OnParlorShowUpdate();
            }
        }

        //更新玩家的麻将馆的信息
        public void UpdateMyParlor(Network.Message.NetMsg.ParlorInfoDef msg)
        {
            if (OnMyParlorPanelUpdate != null)
            {
                OnMyParlorPanelUpdate(msg);
            }
        }

        //更新玩家的麻将馆红包的信息
        public void UpdateParlorRedBagMessage(int iParlorId, int timer)
        {
            if (OnParlorRedBagUpdate != null)
            {
                OnParlorRedBagUpdate(iParlorId, timer);
            }
        }

        /// <summary>
        /// 处理点击规则说明
        /// </summary>
        void HandleBtnRuleContent()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowRulePanel = true;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击返回大厅
        /// </summary>
        void HandleBtnReturnLobby(int type)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            anhui.MahjongCommonMethod.Instance.iFromParlorInGame = 0;
            if (type == 1)
            {
                pspd.isShowMyParlorMessage = false;
                pspd.isShowApplyCreatParlorPanel = false;
                pspd.isShowSecondSure = false;  //显示创
                pspd.isShowWriteParlorName = false;  //            
                pspd.isShowChangeParlorMessage = false;
                pspd.isShowSecondDismissPanel = false;
                pspd.isSpwanStartedRoom = false;
                pspd.isSpwanUnStartRoom = false;                
                UIMainView.Instance.ParlorShowPanel.InitData();
            }
            else
            {
                //根据当前界面，决定显示那个界面
                pspd.isShowMyParlorMessage = false;
                UIMainView.Instance.ParlorShowPanel.RemoveAllListener();
            }
            pspd.RefreshCount = 0;
            //GameData.Instance.CreatRoomMessagePanelData.CreatRoomType = 1;
            //先隐藏麻将馆红包
            UIMainView.Instance.ParlorShowPanel.ParlorBulletin_RedBag.gameObject.SetActive(false);
            UIMainView.Instance.ParlorShowPanel.ParlorRedBagBtn.gameObject.SetActive(false);

            UpdateShow();
        }

        /// <summary>
        /// 处理点击雀神广场
        /// </summary>
        void HandleBtnParlorRound()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //进入雀神广场，获取该区域所有的麻将馆的信息
            pspd.isShowPanel = true;
            UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.ParlorGod);
            UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.NoParlorMessage(1);
            //获取现在选择县的所有麻将馆
            //if (!pspd.isGetAllData_PointCountyParlor[0])
            //{
                //pspd.FromWebGetParlorMessage(1, 5);
            //}

        }

        /// <summary>
        /// 处理点击开麻将馆
        /// </summary>
        void HandleBtnCreatParlor()
        {
            //如果自己已经开过麻将馆
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            {
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("每人只可以开办一个麻将馆！");
                return;
            }

            ////判断加馆冷却时间
            //if ((int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - GameData.Instance.PlayerNodeDef.userDef.iLeaveParlorTime < GameData.Instance.ParlorShowPanelData.ColdTimer
            //    || (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - GameData.Instance.PlayerNodeDef.userDef.iKickParlorTime < GameData.Instance.ParlorShowPanelData.ColdTimer)
            //{
            //    MahjongCommonMethod.Instance.ShowRemindFrame("您最近两小时退出或者解散过馆，不可加入！");
            //    return;
            //}

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            NetMsg.ClientParlorCertReq msg = new NetMsg.ClientParlorCertReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            Network.NetworkMgr.Instance.LobbyServer.SendCreatParlorCertReq(msg);
        }

        /// <summary>
        /// 处理点击申请开馆
        /// </summary>
        void HandleBtnApplyCreatParlor()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //打开浏览器,进入馆主资格申请界面
            Application.OpenURL(pspd.ApplyBossCert_Url + GameData.Instance.PlayerNodeDef.iUserId);
            pspd.isShowApplyCreatParlorPanel = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击二次确认申请开馆
        /// </summary>
        void HandleBtnSecondSureCreatParlor()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            pspd.isShowSecondSure = false;
            pspd.isShowWriteParlorName = true;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击确认创建馆
        /// </summary>
        void HandleBtnSureCreatParlor()
        {
            GameData gd = GameData.Instance;

            //如果玩家不满足开馆条件，直接拦截
            if ((gd.PlayerNodeDef.iCoin) <= 0 && gd.PlayerNodeDef.byCreateParlorCert == 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的金币数量不足，无法开馆");
                return;
            }

            //发送创建麻将馆之前，初始化面板数据
            if (UIMainView.Instance.ParlorShowPanel.ParlorName.text.Length < 2)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的麻将馆名字长度不足");
                return;
            }

            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            Network.Message.NetMsg.ClientCreateParlorReq msg = new Network.Message.NetMsg.ClientCreateParlorReq();
            msg.iUserId = gd.PlayerNodeDef.iUserId;
            msg.iCityId = sapd.iCityId;
            msg.iCountyId = sapd.iCountyId;
            msg.cParlorName = UIMainView.Instance.ParlorShowPanel.ParlorName.text;
            Network.NetworkMgr.Instance.LobbyServer.SendCreateParlorReq(msg);
        }


        /// <summary>
        /// 修改玩家改变选择区域
        /// </summary>
        void HandleBtnChangeAreaSetting(int type)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            sapd.iOpenStatus = type;
            sapd.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
        }

        /// <summary>
        /// 点击解散麻将馆
        /// </summary>
        void HandleBtnDismissParlor()
        {
            //如果业绩积分大于100时，不让解散麻将馆
            if (GameData.Instance.PlayerNodeDef.fParlorScore > 100)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您的业绩还未处理，无法解散，请前往兑换业绩");
                return;
            }

            //如果业绩积分小于100时
            if (GameData.Instance.PlayerNodeDef.fParlorScore < 100 && GameData.Instance.PlayerNodeDef.fParlorScore > 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您还有业绩未处理，若解散麻将馆，系统将自动转换为金币", Ok, () => { });
                return;
            }

            if (GameData.Instance.PlayerNodeDef.fParlorScore <= 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您确定解散麻将馆吗？", Ok, () => { });
                return;
            }

        }

        void Ok()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowSecondDismissPanel = true;
            UpdateShow();
        }

        /// <summary>
        /// 修改麻将馆的公告和联系方式
        /// </summary>
        void HandleBtnChangeParlorMessage()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowChangeParlorMessage = true;
            UpdateShow();
        }

        /// <summary>
        /// 处理点击麻将馆的排序方式，默认时间排序
        /// </summary>
        /// <param name="type"></param>
        void HandleBtnParlorOrderType(int type)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.iNowCheckType = type;
            pspd.UnNeedSpecialSort = true;
            if (!pspd.isGetAllData_PointCountyParlor[type - 1])
            {
                pspd.FromWebGetParlorMessage(type, 5);
            }
            else
            {
                UIMainView.Instance.ParlorShowPanel.ShowPointCountyParlor(1, type);
            }
        }

        /// <summary>
        /// 离开麻将馆
        /// </summary>
        void HandleLevelParlor()
        {
            if (UIMainView.Instance.ParlorShowPanel.OrderRoomTimePanel.Timer <= 0.9f)
            {
                //弹出提示框
                UIMgr.GetInstance().GetUIMessageView().Show("亲，退出后再加入要重新申请哦\n而且会记录退馆次数的\n您确定现在就退出本馆吗？", LevelParlorOk,
                    () => { });
            }
            else
            {
                UIMgr.GetInstance().GetUIMessageView().Show("亲，您还有预约房未处理？");
            }
        }

        void LevelParlorOk()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            NetMsg.ClientLeaveParlorReq msg = new NetMsg.ClientLeaveParlorReq();
            msg.iParlorId = pspd.iParlorId;
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            Network.NetworkMgr.Instance.LobbyServer.SendLevelParlorReq(msg);
        }
        /// <summary>
        /// 处理馆主搜索会员按钮
        /// </summary>
        void HandleParlorerSearchMember(string text)
        {
            NetMsg.ClientGetUserInfoReq msg = new NetMsg.ClientGetUserInfoReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iFindUserId = Convert.ToInt32(text);
            msg.iParlorId = GameData.Instance.PlayerNodeDef.iMyParlorId;
            Network.NetworkMgr.Instance.LobbyServer.SendClientGetUseInfoReq(msg);
        }

        /// <summary>
        /// 邀请成员加入馆
        /// </summary>
        void HandleInviteMember()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            NetMsg.ClientInvitParlorReq msg = new NetMsg.ClientInvitParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = GameData.Instance.PlayerNodeDef.iMyParlorId;
            msg.iInviteUserId = pspd.iUserid_Seach;
            Debug.LogError("麻将馆的id:" + msg.iParlorId + ",玩家id：" + msg.iInviteUserId);
            Network.NetworkMgr.Instance.LobbyServer.SendInvifeParlorReq(msg);
        }

        /// <summary>
        /// 馆主踢出成员
        /// </summary>
        void HandleKickMember()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            NetMsg.ClientKickParlorReq msg = new NetMsg.ClientKickParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = pspd.iParlorId;
            msg.iKickUserId = pspd.iUserid_Seach;
            Network.NetworkMgr.Instance.LobbyServer.SendKickParlorReq(msg);
        }

        /// <summary>
        /// 成员管理
        /// </summary>
        void HandleParlorMemberManger()
        {
            //获取信息
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isGetAllData_ParlorMember = false;
            pspd.iPageID_Member = 1;
            pspd.ParlorAllMemberMessage = new System.Collections.Generic.List<ParlorShowPanelData.ParlorMemberMessage>();
            pspd.FromWebGetParlorMember();
        }

        /// <summary>
        /// 馆主的审核列表
        /// </summary>
        void HandleParlorCheckList()
        {
            //请求当前消息记录，提取出审核信息
            SystemMgr.Instance.LobbyMainSystem.MessageReq(2);
        }


        /// <summary>
        /// 馆主的查看牌局记录
        /// </summary>
        void HandleParlorGameRecord()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.ParlorGameRecordPageId = 1;
            pspd.isGetAllData_ParlorGameRecord = false;
            pspd.PointParlorGameRecordData = new System.Collections.Generic.List<ParlorShowPanelData.GetPointParlorGameRecordData>();
            GameData.Instance.ParlorShowPanelData.FromWebPointParlorThreeDayGameNum(5);
        }

    }

}
