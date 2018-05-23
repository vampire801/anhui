using UnityEngine;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.LobbySystem.SubSystem;
using DG.Tweening;
using System;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewParlorShowPanel : MonoBehaviour
    {
        [Serializable]
        public class Parlor_Panel
        {
            /// <summary>
            /// 麻将馆名字
            /// </summary>
            public Text _tNameParlor;
        }
        /// <summary>
        /// 麻将馆总面板
        /// </summary>
        public Parlor_Panel _panel;
        public JoinParlorPanel joinParlorPanel; //输入馆号面板
        public GameObject RuleContent;  //麻将馆的规则说明面板
        public parlorApplyCreatPanel ApplyCreatParlorCertl;  //申请开办麻将馆资格的面板
        public GameObject SecondSureCreatParlorPanel;  //二次确认是否开办麻将馆的面板
        public GameObject WriteParlorName;  //填写麻将馆名字的信息
        public GameObject ParlorMessagePanel; //馆的信息的面板        
        public ParlorGodGround ParlorGodPanel;  //雀神广场的面板
        public GameObject Refresh_God; //刷新界面
        public GameObject ParlorGodNoviceGuidance;//麻将馆新手引导
        public GameObject EditBulletinAndContact;  //编辑公告和微信号的界面
        public GameObject SecondDismissPanel;  //二次确认解散面板
        public GameObject SearchMemberListPanel;  //搜索成员的面板
        public GameObject ParlorBossScore;  //老板业绩面板
        public ParlorTabelGameMessage ParlorTableGame; //牌桌信息       
        public GameObject ParlorCheckList;  //麻将的审核列表面板
        public GameObject ParlorGameRecord; //麻将馆的游戏记录列表面板
        public GameObject ParlorSmallReaBagMessage;  //麻将馆的小红包详情面板
        public ScoreExChange ExChangePanel; //积分兑换面板
        public GameObject ParlorRedBagBtn;// 麻将馆的红包按钮   
        public OrderRoomTime OrderRoomTimePanel; //预约倒计时面板
        public GameObject ParlorBulletin_RedBag;  //麻将馆的公告信息
        public Sprite[] RedBagImage; //红包图片  0表示未开始抢的状态  1表示已经开始  2表示已经结束展示期间
        public Toggle[] ParlorBtnALL; //麻将馆左侧所有的列表
        public GameObject[] ParlorAllPanel; //麻将馆面板所有的面板
        public ScoreStatistics ScoreStatisticsPanel; //老板积分统计的面板
        public Text[] ChoiceArea;  //1表示申请麻将馆时  2表示雀神广场显示        
        public InputField ParlorName;  //麻将馆的名字              
        public Text BossScore; //老板业绩积分显示
        public GameObject[] MoreBtn; //更多按钮
        public GameObject[] Boss_Mem; //老板和成员的按钮 0表示老板 1表示会员
        #region 成员信息面板
        public GameObject MemberMessage; //成员信息
        public GameObject BtnInviteAndLevel; //邀请和踢出按钮
        public GameObject RemingMessage; //搜索成员提示信息
        #endregion
        #region 常量
        public const string MESSAGE_RETURNLOBBY = "MainViewParlorShowPanel.MESSAGE_RETURNLOBBY";  //点击返回按钮
        public const string MESSAGE_WHATRULE = "MainViewParlorShowPanel.MESSAGE_WHATRULE"; //点击规则说明
        public const string MESSAGE_PARLORGROUND = "MainViewParlorShowPanel.MESSAGE_PARLORGROUND"; //点击雀神广场
        public const string MESSAGE_CREATPARLOR = "MainViewParlorShowPanel.MESSAGE_CREATPARLOR"; //点击开麻将馆
        public const string MESSAGE_APPLYCREATPARLOR = "MainViewParlorShowPanel.MESSAGE_APPLYCREATPARLOR";  //点击申请开馆
        public const string MESSAGE_SECONDSUREBTN = "MainViewParlorShowPanel.MESSAGE_SECONDSUREBTN";  //二次确认开馆的按钮
        public const string MESSAGE_CREATPARLORBTN = "MainViewParlorShowPanel.MESSAGE_CREATPARLORBTN";  //点击确认开馆按钮
        public const string MESSAGE_CHANGEAREASETTING = "MainViewParlorShowPanel.MESSAGE_CHANGEAREASETTING"; //点击修改选择区域
        public const string MESSAGE_DISMISSPARLOR = "MainViewParlorShowPanel.MESSAGE_DISMISSPARLOR"; //点击解散我的麻将馆
        public const string MESSAGE_CHANGEPARLORCONTACTANDBULLTIEN = "MainViewParlorShowPanel.MESSAGE_CHANGEPARLORCONTACTANDBULLTIEN"; //点击修改我的麻将馆的公告信息联系方式
        public const string MESSAGE_PARLORORDERTYPE = "MainViewParlorShowPanel.MESSAGE_PARLORORDERTYPE"; //点击麻将馆的排序方式
        public const string MESSAGE_LEVELPARLOR = "MainViewParlorShowPanel.MESSAGE_LEVELPARLOR";  //退出该馆的点击按钮
        public const string MESSAGE_SEARCHMEMBER = "MainViewParlorShowPanel.MESSAGE_SEARCHMEMBER";  //馆主搜索成员按钮
        public const string MESSAGE_INVITEMEMBER = "MainViewParlorShowPanel.MESSAGE_INVITEMEMBER";  //邀请加入按钮
        public const string MESSAGE_KICKMEMBER = "MainViewParlorShowPanel.MESSAGE_KICKMEMBER";  //踢出成员按钮
        public const string MESSAGE_MEMBERMANAGER = "MainViewParlorShowPanel.MESSAGE_MEMBERMANAGER"; //成员管理按钮
        public const string MESSAGE_CHECKLIST = "MainViewParlorShowPanel.MESSAGE_CHECKLIST";  //审核列表按钮
        public const string MESSAGE_GAMERECORD = "MainViewParlorShowPanel.MESSAGE_GAMERECORD";  //牌局记录按钮
        public const string MESSAGE_PARLORREDBAG = "MainViewParlorShowPanel.MESSAGE_PARLORREDBAG"; //点击麻将馆内红包的按钮        
        #endregion        

        #region 界面更新

        /// <summary>
        /// 初始化数据信息
        /// </summary>
        public void InitData()
        {

           // Debug.Log ("========================0");
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.iPageID_Member = 1;
            pspd.ParlorAllMemberMessage = new List<ParlorShowPanelData.ParlorMemberMessage>();
            pspd.ParlorCheckList = new List<PlayerMessagePanelData.Message>();
            pspd.IdOrNameGetParlorMessageList = new List<ParlorShowPanelData.PointIdOrNameGetParlorMessage>();
            pspd.pageId = new int[] { 1, 1, 1 };
            pspd.TimeOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            pspd.MemberCountOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            pspd.ActivityOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            pspd.ParlorGameRecordPageId = 1;
            pspd.PointParlorGameRecordData = new List<ParlorShowPanelData.GetPointParlorGameRecordData>();
            pspd.PointParlorTabelMessage_UnStart = new List<NetMsg.TableInfoDef>();
            pspd.PointParlorTabelMessage_Started = new List<ParlorShowPanelData.GetPointParlorTabelMessage>();
            pspd.isGetAllData_PointCountyParlor = new bool[3] { false, false, false };
            pspd.isGetAllData_ParlorGameRecord = false;
            pspd.isGetAllData_ParlorMember = false;
            pspd.isGetBossScoreFinish[0] = false;
            pspd.isGetBossScoreFinish[1] = false;
            pspd.PageId_BossScore = new int[] { 1, 1 };
        }

        void Start()
        {
            InintPos_RdBulletin = ParlorBulletin_RedBag.transform.localPosition;
        }


        public void GetInitParlorImagePos()
        {
            if (InitParlorImagePos.x == 0)
            {
                InitParlorImagePos = ParlorMessagePanel.transform.GetChild(0).GetChild(2).transform.localPosition;
            }
        }

        void Update()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (pspd.isShowRedDownTimer)
            {
                if (pspd.ParlorRedStatus == 1)
                {
                    if (pspd.ParlorRedDownTimer > 0)
                    {
                        pspd.ParlorRedDownTimer -= Time.deltaTime;
                        System.TimeSpan span = new System.TimeSpan(0, 0, (int)pspd.ParlorRedDownTimer);
                        string str = (span.Hours).ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
                        ParlorRedBagBtn.transform.GetChild(1).GetComponent<Text>().text = str;
                    }
                    else
                    {
                        pspd.ParlorRedStatus = 2;
                        pspd.ParlorRedDownTimer = 0;
                        pspd.isShowRedDownTimer = false;
                        iRedBagStatus = 2;
                        ParlorRedBagBtn.transform.GetChild(1).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(0).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(2).gameObject.SetActive(true);
                        ParlorRedBagBtn.transform.GetChild(3).gameObject.SetActive(false);
                        //请求该红包的状态，决定是否隐藏
                        MahjongCommonMethod.Instance.GetNowTimer(pspd.iParlorId, UpdateParlorRedBagMessage);
                    }
                }
            }
        }

        public void UpdateShow()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            if (pspd.isShowPanel)
            {
                gameObject.SetActive(true);
            }
            else
            {
                //Debug.LogError("关闭麻将馆的主面板");
                gameObject.SetActive(false);
                return;
            }


            //显示不具备开办麻将馆的资格的面板
            if (pspd.isShowApplyCreatParlorPanel)
            {
                ApplyCreatParlorCertl.gameObject.SetActive(true);
            }
            else
            {
                ApplyCreatParlorCertl.gameObject.SetActive(false);
            }

            //显示二次确认面板
            if (pspd.isShowSecondSure)
            {
                SecondSureCreatParlorPanel.SetActive(true);
            }
            else
            {
                SecondSureCreatParlorPanel.SetActive(false);
            }

            //显示填写麻将馆名字的面板
            if (pspd.isShowWriteParlorName)
            {
                //拼接选中的地区名字                
                string name = MahjongCommonMethod.Instance._dicCityConfig[sapd.iCityId].CITY_NAME + "_" +
                    MahjongCommonMethod.Instance._dicDisConfig[sapd.iCountyId].COUNTY_NAME;
                ChoiceArea[0].text = name;
                WriteParlorName.SetActive(true);
            }
            else
            {
                WriteParlorName.SetActive(false);
            }

            //显示馆的信息面板
            if (pspd.isShowMyParlorMessage)
            {
                ParlorMessagePanel.SetActive(true);
            }
            else
            {
                ParlorMessagePanel.SetActive(false);
            }

            //显示规则信息面板
            if (pspd.isShowRulePanel)
            {
                RuleContent.SetActive(true);
            }
            else
            {
                RuleContent.SetActive(false);
            }

            //显示雀神广场
            if (pspd.isShowParlorRoundPanel)
            {
                ChoiceArea[1].text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
                ParlorGodPanel.gameObject.SetActive(true);
            }
            else
            {
                ParlorGodPanel.gameObject.SetActive(false);
                if (ParlorGodNoviceGuidance != null) ParlorGodNoviceGuidance.SetActive(false);
            }


            //显示编辑公告和联系方式的信息
            if (pspd.isShowChangeParlorMessage)
            {
                EditBulletinAndContact.transform.GetChild(4).GetChild(0).GetComponent<InputField>().text = pspd.parlorInfoDef[0].szBulletin;
                EditBulletinAndContact.transform.GetChild(5).GetChild(0).GetComponent<InputField>().text = pspd.parlorInfoDef[0].szContact;
                EditBulletinAndContact.SetActive(true);
            }
            else
            {
                EditBulletinAndContact.SetActive(false);
                if (GameData.Instance.ParlorShowPanelData.MoreBtnStatus == 2)
                {
                    GameData.Instance.ParlorShowPanelData.MoreBtnStatus = 1;
                    MoreBtn[1].SetActive(false);
                }
            }

            //显示二次确认解散面板
            if (pspd.isShowSecondDismissPanel)
            {
                SecondDismissPanel.SetActive(true);
            }
            else
            {
                SecondDismissPanel.SetActive(false);
            }

            //显示面板
            if (pspd.isShowOrderTimePanel)
            {
                OrderRoomTimePanel.gameObject.SetActive(true);
            }
            else
            {
                OrderRoomTimePanel.gameObject.SetActive(false);
            }
        }


        public void CloseSecondDissolve()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowSecondDismissPanel = false;
            UpdateShow();
        }

        /// <summary>
        /// 获取四个玩家的userID
        /// </summary>
        /// <returns></returns>
        int[] GetTableUser()
        {
            int[] tableuser = new int[4];

            for (int i = 0; i < 4; i++)
            {
                if ((UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[i] + UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iBespeakUserId[i]) == 0)
                {
                    tableuser[i] = 0;
                }
                else
                {
                    if (UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[i] != 0)
                        tableuser[i] = UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[i];
                    else
                        tableuser[i] = UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iBespeakUserId[i];
                }
            }
            return tableuser;
        }

        public void BtnSendClickTable()
        {
            //先清理一下
            for (int index = 0; index < 4; index++)
            {
                UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[index] = null;
            }
            NetMsg.ClientGetTableUserIDReq msg = new NetMsg.ClientGetTableUserIDReq();
            msg.RoomNum = UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iTableNum;//桌编号
            //Debug.LogError("桌号:" + msg.RoomNum + "," + GetTableUser()[0] + "," + GetTableUser()[1] +
            //    "," + GetTableUser()[2] + "," + GetTableUser()[3]);
            msg.iBespeakUserId = GetTableUser();//四个玩家的ID号         
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //在这里直接处理玩家信息
            bool isSendMessage = false;

            List<int> userId = new List<int>();


            //foreach (var t in pspd.userInfo_Tabel)
            //{
            //    Debug.LogError("key：" + t.Key + ",value：" + t.Value);
            //}

            if (pspd.ComeInParlorType == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg.iBespeakUserId[i] != 0)
                    {
                        if (pspd.userInfo_Tabel.ContainsKey(msg.iBespeakUserId[i]))
                        {
                            UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[i]
                                = pspd.userInfo_Tabel[msg.iBespeakUserId[i]];
                        }
                        else
                        {
                            //Debug.LogError("==========================0");
                            //请求当前玩家信息
                            isSendMessage = true;
                            userId.Add(msg.iBespeakUserId[i]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg.iBespeakUserId[i] != 0)
                    {
                        if (pspd.userInfo_Tabel.ContainsKey(UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[i]))
                        {
                            UIMainView.Instance.ReservationSeatPanel.m_lUserInfo[i] =
                                pspd.userInfo_Tabel[UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iaUserId[i]];
                        }
                        else
                        {
                            //Debug.LogError("==========================1");
                            //请求消息
                            isSendMessage = true;
                            userId.Add(msg.iBespeakUserId[i]);
                        }
                    }
                }
            }

            if (!isSendMessage)
            {
                UIMainView.Instance.ReservationSeatPanel.OnOpen( );//打开界面
            }
            else
            {
                int[] id = new int[4];
                for (int i = 0; i < userId.Count; i++)
                {
                    id[i] = userId[i];
                }
                //发送信息
                NetMsg.ClientGetTableUserInfoReq info = new NetMsg.ClientGetTableUserInfoReq();
                info.iUserId = id;

                NetworkMgr.Instance.LobbyServer.SendClientGetTableUseInfoReq(info);
            }

            //NetworkMgr.Instance.LobbyServer.SendClientGetTableUseInfoReq(msg);
        }
        [HideInInspector]
        public int iRedBagStatus = 0;  //0表示没有红包  1表示有红包且未开启 2表示有红包已经开启且还没有结束  3有红包但是已经结束 保留消息24小时
        Vector3 InintPos_RdBulletin = Vector3.zero;  //红包公告的初始位置
        /// <summary>
        /// 更新玩家所在的馆的界面信息
        /// </summary>
        /// <param name="infoDef"></param>
        public void UpdateMyParlorPanel(NetMsg.ParlorInfoDef infoDef)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //确定是申请状态，还是已经是自己的麻将馆
            if (infoDef.iStatus == 5)
            {
                ParlorTableGame.ApplyPanel.SetActive(true);
                ParlorTableGame.TabelPanel.SetActive(false);
                //打开申请按钮
                ParlorTableGame.Setting.gameObject.SetActive(false);
                ParlorTableGame.QuitParlor.gameObject.SetActive(false);
                ParlorTableGame.CanelApplyParlor.gameObject.SetActive(true);
            }
            //已经加入的馆或者自己的馆
            else
            {
                ParlorTableGame.ApplyPanel.SetActive(false);
                ParlorTableGame.TabelPanel.SetActive(true);
                //如果是玩家自己的馆
                if (GameData.Instance.PlayerNodeDef.iUserId == infoDef.iBossId)
                {
                    //隐藏老板的操作按钮
                    int count = UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).childCount;
                    for (int i = 1; i < count - 1; i++)
                    {
                        UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
                    }
                    //开启普通成员的操作按钮
                    UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).GetChild(count - 1).gameObject.SetActive(false);
                    //开启老板特殊按钮
                    ParlorTableGame.Setting.gameObject.SetActive(true);
                    ParlorTableGame.QuitParlor.gameObject.SetActive(false);
                    ParlorTableGame.CanelApplyParlor.gameObject.SetActive(false);
                }
                else
                {
                    //关闭老板特殊按钮
                    ParlorTableGame.Setting.gameObject.SetActive(false);
                    ParlorTableGame.QuitParlor.gameObject.SetActive(true);
                    ParlorTableGame.CanelApplyParlor.gameObject.SetActive(false);
                }

                //显示房间数量和已开始数量
                if (pspd.PointParlorTabelMessage_UnStart.Count > 0)
                {
                    ParlorTableGame.UnStartCount.text = (pspd.PointParlorTabelMessage_UnStart.Count - 1).ToString();
                }
                else
                {
                    ParlorTableGame.UnStartCount.text = "0";
                }

                int timer_red = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(MahjongCommonMethod.FirstSecondTime(System.DateTime.Now));
                //检查该馆有没有红包（进馆红包）
                if (timer_red > PlayerPrefs.GetInt(ParlorShowPanelData.IsShowParlorRed) && infoDef.iRp7Type == 1 && !MahjongCommonMethod.isGameToLobby)
                {
                    PlayerPrefs.SetInt(ParlorShowPanelData.IsShowParlorRed, (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now));
                    UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(6, 1, 1, UIMainView.Instance.RedPagePanel.RedPage[6].Name, RedPageShowPanel.NowState.Lobby);
                }
                MahjongCommonMethod.Instance.GetNowTimer(infoDef.iParlorId, UpdateParlorRedBagMessage);
            }

            MahjongCommonMethod.Instance.iParlorId = infoDef.iParlorId;
            //将馆的id保存进注册表
            PlayerPrefs.SetInt(ParlorShowPanelData.SaveLaseParlorId, infoDef.iParlorId);

            //更新馆主和馆的公用信息
            ParlorTableGame.StartedCount.text = (pspd.PointParlorTabelMessage_Started.Count).ToString();
            //显示馆主的头像
            MahjongCommonMethod.Instance.GetPlayerAvatar(ParlorTableGame.BossImage, infoDef.szBossHeadimgurl);
            UpdateMyParlorMessage_Title(infoDef);
            GameData.Instance.ParlorShowPanelData.iCountyId[2] = infoDef.iCountyId;
            GameData.Instance.ParlorShowPanelData.iCityId[2] = infoDef.iCityId;
        }

        //麻将馆的地区的显示初始位置
        public Vector3 InitParlorImagePos;

        /// <summary>
        /// 更新麻将馆的相关信息
        /// </summary>
        /// <param name="infoDef"></param>
        /// <param name="type">1表示正常更新  2表示玩家广场的标题界面</param>
        public void UpdateMyParlorMessage_Title(NetMsg.ParlorInfoDef infoDef, int type = 1)
        {
            Debug.LogWarning("UpdateMyParlorMessage_Title:" + type);
            if (type == 1)
            {
                //显示玩家昵称
                ParlorTableGame.BossNick.text = infoDef.szBossNickname;
                //显示馆主的id
                ParlorTableGame.ParlorId.text = infoDef.iParlorId.ToString();
                //显示馆主的联系方式
                ParlorTableGame.BossContact.text = " :" + infoDef.szContact.ToString();
                //显示馆的成员数量
                ParlorTableGame.ParlorMemberCount.text = infoDef.iMemberNum.ToString();
                //显示馆的月活跃度
                ParlorTableGame.ParlorMonthActi.text = infoDef.iMonthVitality.ToString();
                //显示馆的总活跃度
                ParlorTableGame.ParlorAllActi.text = infoDef.iVitality.ToString();
                //显示馆的公告信息
                ParlorTableGame.ParlorBulletin.text = infoDef.szBulletin.ToString();
                //更新馆名字
                _panel._tNameParlor.text = infoDef.szParlorName;
                ParlorMessagePanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                ParlorMessagePanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance._dicDisConfig[infoDef.iCountyId].COUNTY_NAME;
            }
            else if (type == 2)
            {
                //更新馆名字
                _panel._tNameParlor.text = "麻将馆";
                //更新地区
                SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
                //ParlorMessagePanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance._dicDisConfig[sapd.iCountyId].COUNTY_NAME;
                ParlorMessagePanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                ChoiceArea[1].text = MahjongCommonMethod.Instance._dicDisConfig[sapd.iCountyId].COUNTY_NAME;
            }
            //动态改变地区的位置            
            ParlorMessagePanel.transform.GetChild(0).GetChild(2).transform.localPosition = InitParlorImagePos + new Vector3(ParlorMessagePanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().preferredWidth + 10, 0, 0);
        }

        //主要是更新馆的成员数量
        public void UpdateMemberCount(int count)
        {
            ParlorTableGame.ParlorMemberCount.text = count.ToString();
        }

        //更新馆的桌的数量
        public void UpdateMemberTabelCount()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (pspd.PointParlorTabelMessage_UnStart.Count > 0)
            {
                int count = 0;
                if (pspd.isGetUnStartDataEnd)
                {
                    count = pspd.PointParlorTabelMessage_UnStart.Count - 1;
                }
                else
                {
                    count = pspd.iAllTabelNumUnStart - 1;
                }
                ParlorTableGame.UnStartCount.text = "0";
                if (count >= 0)
                {
                    ParlorTableGame.UnStartCount.text = (count).ToString();
                }
            }
            else
            {
                ParlorTableGame.UnStartCount.text = "0";
            }

            ParlorTableGame.StartedCount.text = (pspd.PointParlorTabelMessage_Started.Count).ToString();
            //隐藏红点
            if (pspd.PointParlorTabelMessage_Started.Count == 0)
            {
                ShowStartedRedPoint(0);
            }
        }


        public int nowTimer_ParlorRp = 0; //麻将馆红包的时间
        /// <summary>
        /// 请求指定麻将馆的红包状态
        /// </summary>
        /// <param name="iParlorId"></param>
        /// <param name="timerNow"></param>
        public void UpdateParlorRedBagMessage(int iParlorId, int timerNow)
        {
            nowTimer_ParlorRp = timerNow;
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < 4; i++)
            {
                if (iParlorId == pspd.parlorRedBagInfo[i].parlorId)
                {
                    if (pspd.parlorRedBagInfo[i].rpId != 0)
                    {
                        NetMsg.ClientRp17TypeReq msg = new NetMsg.ClientRp17TypeReq();
                        msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                        msg.iRp17Id = pspd.parlorRedBagInfo[i].rpId;
                        NetworkMgr.Instance.LobbyServer.SendClientRp17TypeReq(msg);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// 更新红包信息
        /// </summary>
        /// <param name="iParlorId"></param>
        /// <param name="timerNow"></param>
        public void UpdateParlorRedBagMessage_Res(int iParlorId)
        {
            //判断该馆有没有红包信息，如果有显示红包的值     
            iRedBagStatus = 0;
            int nowTimer = nowTimer_ParlorRp;
            string content = " "; //公告内容
            float timer_int = 0;  //公告间隔时间
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < 4; i++)
            {
                if (iParlorId == pspd.parlorRedBagInfo[i].parlorId)
                {
                    int interval = nowTimer - pspd.parlorRedBagInfo[i].beginTim;
                    //显示红包倒计时
                    pspd.ParlorRedDownTimer = pspd.parlorRedBagInfo[i].beginTim - nowTimer;
                    //表示有红包且未开启
                    if (pspd.parlorRedBagInfo[i].state == 0 && nowTimer < pspd.parlorRedBagInfo[i].beginTim)
                    {
                        iRedBagStatus = 1;
                        pspd.ParlorRedStatus = 1;
                        content = pspd.parlorRedBagInfo[i].context;
                        timer_int = pspd.parlorRedBagInfo[i].interval;
                        pspd.isShowRedDownTimer = true;
                    }

                    //表示有红包已经开启且还没有结束
                    if (pspd.parlorRedBagInfo[i].state == 0 && nowTimer < pspd.parlorRedBagInfo[i].endTim && nowTimer > pspd.parlorRedBagInfo[i].beginTim)
                    {
                        iRedBagStatus = 2;
                        pspd.ParlorRedStatus = 1;
                        content = pspd.parlorRedBagInfo[i].context;
                        timer_int = pspd.parlorRedBagInfo[i].interval;
                    }

                    //表示有红包且已经结束，保留消息24小时                    
                    if ((pspd.parlorRedBagInfo[i].state == 1 || pspd.parlorRedBagInfo[i].state == 2) && interval < 86400)
                    {
                        iRedBagStatus = 3;
                    }
                }
            }

            //显示红包信息
            if (iRedBagStatus > 0)
            {
                //显示红包按钮
                ParlorRedBagBtn.SetActive(true);
                if (iRedBagStatus == 1)
                {
                    ParlorRedBagBtn.transform.GetChild(1).gameObject.SetActive(true);
                    ParlorRedBagBtn.transform.GetChild(0).gameObject.SetActive(true);
                    ParlorRedBagBtn.transform.GetChild(2).gameObject.SetActive(false);
                    ParlorRedBagBtn.transform.GetChild(3).gameObject.SetActive(false);
                }
                else
                {
                    if (iRedBagStatus == 2)
                    {
                        ParlorRedBagBtn.transform.GetChild(1).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(0).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(2).gameObject.SetActive(true);
                        ParlorRedBagBtn.transform.GetChild(3).gameObject.SetActive(false);
                    }
                    else
                    {
                        ParlorRedBagBtn.transform.GetChild(1).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(0).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(2).gameObject.SetActive(false);
                        ParlorRedBagBtn.transform.GetChild(3).gameObject.SetActive(true);
                    }

                }

                if (iRedBagStatus < 3)
                {
                    //显示公告信息                
                    ParlorBulletin_RedBag.SetActive(true);
                    ParlorBulletin_RedBag.transform.GetChild(0).GetComponent<Text>().text = content;
                    float width = ParlorBulletin_RedBag.transform.GetChild(0).GetComponent<Text>().preferredWidth + 140;  //字体宽度
                    float x = ParlorBulletin_RedBag.transform.localPosition.x - width - Screen.width;
                    ParlorBulletin_RedBag.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    ParlorBulletin_RedBag.transform.localPosition = InintPos_RdBulletin;
                    MoveBulletin(x, timer_int);
                }
            }
            else
            {
                ParlorBulletin_RedBag.gameObject.SetActive(false);
                ParlorRedBagBtn.gameObject.SetActive(false);
            }

            //30分钟之后，重新获取麻将馆红包信息
            if (gameObject.activeSelf)
            {
                StartCoroutine(ThirSecondsDelay());
            }
        }





        IEnumerator ThirSecondsDelay()
        {
            yield return new WaitForSeconds(1800f);
            GameData.Instance.ParlorShowPanelData.FromWebGetParlorRedBagData();
        }

        //移动公告
        void MoveBulletin(float x, float downTimer)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < 4; i++)
            {
                if (pspd.iParlorId == pspd.parlorRedBagInfo[i].parlorId)
                {
                    if ((int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now) > pspd.parlorRedBagInfo[i].endTim - pspd.parlorRedBagInfo[i].interval)
                    {
                        return;
                    }
                }
            }
            float speed = 150f;
            float timer = (ParlorBulletin_RedBag.transform.localPosition.x - x) / speed;
            Tweener tweener_0 = ParlorBulletin_RedBag.transform.DOLocalMoveX(x, timer);
            tweener_0.SetEase(Ease.Linear);
            tweener_0.OnComplete(() => StartCoroutine(DelayShowBulletin(x, downTimer)));
        }

        IEnumerator DelayShowBulletin(float x, float timer)
        {
            if (timer < 1)
            {
                timer = 1;
            }
            ParlorBulletin_RedBag.transform.localPosition = InintPos_RdBulletin + new Vector3(ParlorBulletin_RedBag.transform.GetChild(0).GetComponent<Text>().preferredWidth, 0, 0);
            yield return new WaitForSeconds(timer);
            MoveBulletin(x, timer);
        }

        #endregion


        #region 点击事件       

        /// <summary>
        /// 关闭编写麻将馆的名字红包
        /// </summary>

        public void CloseWriteNamePanel(int type)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (type == 1)
            {
                pspd.isShowWriteParlorName = false;
            }
            else if (type == 2)
            {
                pspd.isShowChangeParlorMessage = false;
            }

            UpdateShow();
        }


        /// <summary>
        /// 点击直接返回房间
        /// </summary>
        public void BtnReturnRoom()
        {
            NetMsg.ClientGameServerInfoReq msgg = new NetMsg.ClientGameServerInfoReq();
            msgg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msgg.iServerId = MahjongCommonMethod.Instance.iSeverId;
            Debug.LogError("msgg.iUserId:" + msgg.iUserId + ",msgg.iServerId:" + msgg.iServerId);
            Network.NetworkMgr.Instance.LobbyServer.SendGameSeverInfoReq(msgg);
        }
        /// <summary>
        /// 点击业绩兑换安卓币
        /// </summary>
        public void BtnExChangeCoin()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //产生兑换预置体数据
            ExChangePanel.UpdateShow();
        }

        /// <summary>
        /// 点击关闭再次确认面板
        /// </summary>
        public void BtnCloseApplyCreatParlorPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowApplyCreatParlorPanel = false;
            UpdateShow();
        }

        /// <summary>
        /// 点击麻将馆的红包按钮
        /// </summary>
        /// <param name="status"></param>
        public void BtnParlorRedBag()
        {
            int status = iRedBagStatus;
            Debug.LogError("红包状态:" + iRedBagStatus);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (status == 1)
            {
                StringBuilder str = new StringBuilder();
                str.Append("红包还未开启，请于");
                string benginTim = " ";
                for (int i = 0; i < 4; i++)
                {
                    if (pspd.iParlorId == pspd.parlorRedBagInfo[i].parlorId)
                    {
                        benginTim = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(pspd.parlorRedBagInfo[i].beginTim, 0).ToString("HH:mm");
                        continue;
                    }
                }
                str.Append(benginTim);
                str.Append("点前前往牌桌领取");
                MahjongCommonMethod.Instance.ShowRemindFrame(str.ToString());
            }

            //提示进入牌桌抢红包
            if (status == 2)
            {
                StringBuilder str = new StringBuilder();
                str.Append("红包已开启，请进入牌桌领取");
                MahjongCommonMethod.Instance.ShowRemindFrame(str.ToString());
            }

            //获取领取的红包记录信息
            if (status == 3)
            {
                //获取红包信息
                pspd.FromWebGetRedBag17();
            }
        }

        /// <summary>
        /// 点击规则说明
        /// </summary>
        public void BtnParlorRule()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_WHATRULE);
        }

        /// <summary>
        /// 点击雀神广场
        /// </summary>
        public void BtnParlorRound(bool isOn = true)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (isOn)
            {
                Messenger_anhui.Broadcast(MESSAGE_PARLORGROUND);
            }
        }

        /// <summary>
        /// 点击更多按钮
        /// </summary>
        public void BtnMore()
        {
            ParlorShowPanelData pppd = GameData.Instance.ParlorShowPanelData;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            //旋转按钮
            MoreBtn[0].transform.GetChild(1).transform.localEulerAngles += new Vector3(0, 0, 180);
            //打开操作按钮
            if (pppd.MoreBtnStatus == 1)
            {
                pppd.MoreBtnStatus = 2;
                MoreBtn[1].SetActive(true);
            }
            else
            {
                pppd.MoreBtnStatus = 1;
                MoreBtn[1].SetActive(false);
            }
        }

        /// <summary>
        /// 点击转让按钮
        /// </summary>
        public void BtnZhuangRang()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            UIMgr.GetInstance().GetUIMessageView().Show("移交麻将馆权限，请联系客服！");
        }


        /// <summary>
        /// 一键邀请
        /// </summary>
        public void BtnOneKeyIntivate()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //邀请麻将馆
            string url = ""; //分享地址
            url = SDKManager.WXInviteUrl + "3_" + pspd.parlorInfoDef[0].iParlorId;
            string title = ""; //标题             
            title = "【" + pspd.parlorInfoDef[0].szParlorName + "】麻将馆邀你打麻将啦！";

           // Debug.Log ("title:" + title);

            string discription = ""; //描述           
            discription = "当前已开" + ((pspd.iAllTabelNumUnStart - 1) > 0 ? (pspd.iAllTabelNumUnStart - 1) : 0) + "桌，三缺一啦，快来凑一桌！";
            MahjongLobby_AH.SDKManager.Instance.HandleShareWX(url, title, discription, 0, 0, 0, "");
        }

        /// <summary>
        /// 点击开麻将馆按钮
        /// </summary>
        public void BtnCreatParlorMessage()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CREATPARLOR);
        }

        /// <summary>
        /// 点击返回大厅
        /// </summary>
        /// <param name="type">1表示显示会员麻将馆面板  1表示某个麻将馆的详细信息的面板</param>
        public void BtnReturnLobby(int type)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui<int>.Broadcast(MESSAGE_RETURNLOBBY, type);
        }

        /// <summary>
        /// 点击申请开馆按钮
        /// </summary>
        public void BtnApplyCreatParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_APPLYCREATPARLOR);
        }

        /// <summary>
        /// 二次确认开馆的按钮
        /// </summary>
        public void BtnSecondSure()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_SECONDSUREBTN);
        }


        /// <summary>
        /// 点击创建开馆的按钮
        /// </summary>
        public void BtnCreatParlor()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            if (pspd.isIncludeForbidFont)
            {
                StringBuilder str = new StringBuilder();
                str.Append("您的麻将馆名字中包含敏感字\"");
                str.Append(pspd.ForbidContent);
                str.Append("\"");
                str.Append(",请重新填写");
                UIMgr.GetInstance().GetUIMessageView().Show(str.ToString());
                return;
            }
            Messenger_anhui.Broadcast(MESSAGE_CREATPARLORBTN);
        }

        /// <summary>
        /// 点击二次确认点击解散按钮
        /// </summary>
        public void BtnSecondDismiss()
        {
            //如果玩家没有麻将馆，不可以发解散消息
            if (GameData.Instance.PlayerNodeDef.iMyParlorId <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您还没有麻将馆，无法解散");
                return;
            }

            Network.Message.NetMsg.ClientDismissParlorReq msg = new Network.Message.NetMsg.ClientDismissParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            Network.NetworkMgr.Instance.LobbyServer.SendDismissParlorReq(msg);
        }


        /// <summary>
        /// 点击解散我的麻将馆
        /// </summary>
        public void BtnDismissParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_DISMISSPARLOR);
        }

        /// <summary>
        /// 修改创建麻将馆的地区选择
        /// </summary>
        public void BtnChangeAreaSetting(int type)
        {
            if (type == 4)
            {
                GameData.Instance.SelectAreaPanelData.pos_index = type;
            }
            if (type == 3)
            {
                GameData.Instance.SelectAreaPanelData.pos_index = type;
            }
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui<int>.Broadcast(MESSAGE_CHANGEAREASETTING, type);
        }

        /// <summary>
        /// 修改麻将馆的公告和联系方式
        /// </summary>
        public void BtnChangeParlorBulletinAndContact()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CHANGEPARLORCONTACTANDBULLTIEN);
        }

        /// <summary>
        /// 麻将馆名字屏蔽字检查
        /// </summary>
        /// <param name="input"></param>
        public void EditParlorName(InputField input)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            string forbid = CheckForbidFont(input);
            //如果名字有屏蔽字库            
            if (forbid != null)
            {
                pspd.isIncludeForbidFont = true;
                pspd.ForbidContent = forbid;
            }
            else
            {
                pspd.isIncludeForbidFont = false;
            }
        }

        /// <summary>
        /// 编辑公告结束通知
        /// </summary>
        /// <param name="input"></param>
        public void EditBulletin(InputField input)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Debug.LogError("编辑公告结束：" + input.text + ",保存信息:" + pspd.ParlorBulletin);
            //分析公告是否改变
            if (!string.Equals(input.text, pspd.parlorInfoDef[0].szBulletin))
            {
                pspd.isChangeBulletin = true;
                //分析屏蔽字库                
                string forbid = CheckForbidFont(input);
                //如果名字有屏蔽字库            
                if (forbid != null)
                {
                    pspd.isIncludeForbidFont = true;
                    pspd.ForbidContent = forbid;
                }
                else
                {
                    pspd.isIncludeForbidFont = false;
                }
            }
            pspd.ParlorBulletin = input.text;
        }


        /// <summary>
        /// 编辑微信号结束通知    
        /// </summary>
        /// <param name="input"></param>
        public void EditContact(InputField input)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Debug.LogError("编辑微信号结束：" + input.text + ",保存信息:" + pspd.ParlorContact);
            //分析微信号是否改变
            if (!string.Equals(input.text, pspd.parlorInfoDef[0].szContact))
            {
                pspd.isChangeContact = true;
            }
            pspd.ParlorContact = input.text;
        }

        /// <summary>
        /// 编辑完麻将馆的信息之后的确定按钮
        /// </summary>
        public void BtnEditMessageOk()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            int count = 0; //临时数据
            if (pspd.isChangeBulletin)
            {
                if (pspd.isIncludeForbidFont)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append("您的公告中包含敏感字\"");
                    str.Append("\"");
                    str.Append(pspd.ForbidContent);
                    str.Append(",请重新填写");
                    UIMgr.GetInstance().GetUIMessageView().Show(str.ToString());
                    return;
                }

                count++;
                NetMsg.ClientChangeParlorBulletinInfoReq msg = new NetMsg.ClientChangeParlorBulletinInfoReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.szBulletinContent = pspd.ParlorBulletin;
                NetworkMgr.Instance.LobbyServer.SendChangeParlorBulletinInfoReq(msg);
            }

            if (pspd.isChangeContact)
            {
                count++;
                NetMsg.ClientChangeParlorContactInfoReq msg = new NetMsg.ClientChangeParlorContactInfoReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.szContact = pspd.ParlorContact;
                NetworkMgr.Instance.LobbyServer.SendChangeParlorContactInfoReq(msg);
            }

            if (count == 0)
            {
                pspd.isShowChangeParlorMessage = false;
                SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            }
        }

        /// <summary>
        /// 点击麻将馆的排序方式
        /// </summary>
        /// <param name="type">1创建时间排序 2成员数量排序 3月活跃度排序</param>
        public void BtnParlorSortType(Toggle go)
        {
            if (go.isOn)
            {
                int type = System.Convert.ToInt32(go.name.Split('_')[1]);
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_PARLORORDERTYPE, type);
            }
        }


        /// <summary>
        /// 点击退出麻将馆
        /// </summary>
        public void BtnLevelParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_LEVELPARLOR);
        }

        /// <summary>
        /// 点击馆主邀请会员按钮
        /// </summary>        
        public void BtnInviteMember(bool isOn, int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (isOn)
            {
                ShowPointPanel(ParlorPanel.IntiveList);
                //打开提示信息
                RemingMessage.SetActive(true);
                BtnInviteAndLevel.SetActive(false);
                MemberMessage.SetActive(false);
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
            }
            else
            {
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }
        }

        /// <summary>
        /// 点击搜索成员按钮
        /// </summary>
        public void BtnSearchMember(InputField input)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (input.text.Length < 8)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您输入的成员ID错误，请重新输入");
                return;
            }
            GameData.Instance.ParlorShowPanelData.iUserid_Seach = System.Convert.ToInt32(input.text);
            Messenger_anhui<string>.Broadcast(MESSAGE_SEARCHMEMBER, input.text);
        }

        /// <summary>
        /// 点击邀请成员按钮
        /// </summary>
        public void BtnInvite()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.MyParlorMessageStastu = 2;
            Messenger_anhui.Broadcast(MESSAGE_INVITEMEMBER);
        }

        /// <summary>
        /// 提现按钮
        /// </summary>
        public void GetMoney()
        {
            UIMgr.GetInstance().GetUIMessageView().Show("请前往公众号<color=#bc35d0>【双喜麻将】</color>提现");
        }

        /// <summary>
        /// 点击踢出成员
        /// </summary>
        public void BtnKickMember()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_KICKMEMBER);
        }

        /// <summary>
        /// 成员管理按钮
        /// </summary>
        public void BtnMemberManger(Toggle tog)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (tog.isOn)
            {
                //获取当前进入的麻将馆的信息
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                {
                    if (pspd.parlorInfoDef[i] == null)
                    {
                        continue;
                    }
                    if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                    {
                        if (pspd.parlorInfoDef[i].iStatus == 1)
                        {
                            UIMgr.GetInstance().GetUIMessageView().Show("麻将馆已被封，无法进行任何操作，详情请联系官方客服！");
                            return;
                        }
                    }
                }
                //ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
                pspd.MyParlorMessageStastu = 2;
                Messenger_anhui.Broadcast(MESSAGE_MEMBERMANAGER);
            }
            else
            {
                //ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }
        }

        /// <summary>
        /// 审核列表按钮
        /// </summary>
        public void BtnCheckList(bool isOn, int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (isOn)
            {
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.MyParlorMessageStastu = 2;
                Messenger_anhui.Broadcast(MESSAGE_CHECKLIST);
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
            }
            else
            {
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }
        }

        /// <summary>
        /// 牌局记录按钮
        /// </summary>
        public void BtnGameRecord(bool isOn, int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (isOn)
            {
                ShowPointRedPoint(1, ParlorBtn.Record);
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.MyParlorMessageStastu = 2;
                Messenger_anhui.Broadcast(MESSAGE_GAMERECORD);
                Debug.Log ("牌局记录按钮");
                ShowPointPanel(ParlorPanel.Record);
                ParlorGameRecord.transform.SetAsLastSibling();
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
            }
            else
            {
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }

        }


        /// <summary>
        /// 业绩收入按钮
        /// </summary>
        public void BtnParlorBossScore(bool isOn, int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (isOn)
            {
                ShowPointRedPoint(1, ParlorBtn.BossScore);
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.Status_BossScore = 1;
                pspd.MyParlorMessageStastu = 2;
                pspd.isGetBossScoreFinish = new bool[] { false, false };
                pspd.PageId_BossScore = new int[] { 1, 1 };
                pspd.ParlorBossScoreCount = new int[] { 0, 0 };
                pspd.lsParlorBossScoreLog_Last = new List<ParlorShowPanelData.ParlorBossScoreLog>();
                pspd.lsParlorBossScoreLog_Now = new List<ParlorShowPanelData.ParlorBossScoreLog>();
                pspd.FormWebGetParlorBossScoreLogMessage();
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
            }
            else
            {
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }
        }


        public enum ParlorPanel
        {
            TabelGame,
            BossScore,
            Record,
            CheckList,
            IntiveList,
            ParlorGod
        }

        /// <summary>
        /// 打开指定的面板
        /// </summary>
        /// <param name="index"></param>
        public void ShowPointPanel(ParlorPanel panel)
        {
            if (panel == ParlorPanel.ParlorGod)
            {
                GameData.Instance.ParlorShowPanelData.isShowParlorRoundPanel = true;
            }
            else
            {
                GameData.Instance.ParlorShowPanelData.isShowParlorRoundPanel = false;
            }
            for (int i = 0; i < ParlorAllPanel.Length; i++)
            {
                if (ParlorAllPanel[i] == null)
                {
                    continue;
                }

                if (i == (int)panel)
                {
                    ParlorAllPanel[i].SetActive(true);
                }
                else
                {
                    ParlorAllPanel[i].SetActive(false);
                }
            }
        }






        public enum ParlorBtn
        {
            TabelGame,
            BossScore,
            Record,
            CheckList,
            IntivateMem
        }


        /// <summary>
        /// 显示指定按钮的红点信息
        /// </summary>
        /// <param name="status">0表示显示红点  1表示隐藏红点</param>
        /// <param name="temp">指定下标</param>
        public void ShowPointRedPoint(int status, ParlorBtn temp)
        {
            if (status == 0)
            {
                ParlorBtnALL[(int)temp].transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                ParlorBtnALL[(int)temp].transform.GetChild(2).gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 显示已开始房间的红点信息
        /// </summary>
        /// <param name="status"></param>
        public void ShowStartedRedPoint(int status)
        {
            if (status == 1)
            {
                ParlorTableGame.UnStartRedPoint.SetActive(true);
            }
            else
            {
                ParlorTableGame.UnStartRedPoint.SetActive(false);
            }
        }

        /// <summary>
        /// 初始化麻将馆面板的点击事件
        /// </summary>
        public void InitAllBtn_Parlor()
        {
            //打开老板操作按钮       
            for (int i = 0; i < ParlorBtnALL.Length; i++)
            {
                AddLisenter(ParlorBtnALL[i], i);
            }
            InitGameTabel();
        }

        /// <summary>
        /// 初始化显示牌桌信息
        /// </summary>
        public void InitGameTabel()
        {
            for (int i = 0; i < ParlorBtnALL.Length; i++)
            {
                if (i == 0)
                {
                    ParlorBtnALL[i].isOn = true;
                }
                else
                {
                    ParlorBtnALL[i].isOn = false;
                }
            }
        }


        public void AddLisenter(Toggle toggle, int index)
        {
            switch (index)
            {
                case 0:
                    toggle.onValueChanged.AddListener(delegate (bool ison) { BtnParlorTabelGame(ison, index); });
                    break;
                case 1:
                    toggle.onValueChanged.AddListener(delegate (bool ison) { BtnParlorBossScore(ison, index); });
                    break;
                case 2:
                    toggle.onValueChanged.AddListener(delegate (bool ison) { BtnGameRecord(ison, index); });
                    break;
                case 3:
                    toggle.onValueChanged.AddListener(delegate (bool ison) { BtnCheckList(ison, index); });
                    break;
                case 4:
                    toggle.onValueChanged.AddListener(delegate (bool ison) { BtnInviteMember(ison, index); });
                    break;
            }
        }


        //移除所有的监听者
        public void RemoveAllListener()
        {
            for (int i = 0; i < ParlorBtnALL.Length; i++)
            {
                ParlorBtnALL[i].onValueChanged.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 点击成员的搜索按钮
        /// </summary>
        public void BtnMemSearch()
        {
            ParlorBtnALL[5].isOn = true;
        }


        /// <summary>
        /// 查看麻将馆的牌桌记录
        /// </summary>
        public void BtnParlorTabelGame(bool ison, int index)
        {
            if (ison)
            {
                ShowPointRedPoint(1, ParlorBtn.TabelGame);
                //初始化显示未开始界面                
                Toggle[] toggle = ParlorTableGame.TabelToggel.GetComponentsInChildren<Toggle>();
                for (int k = 0; k < toggle.Length; k++)
                {
                    if (k == 0)
                    {
                        toggle[k].isOn = true;
                    }
                    else
                    {
                        toggle[k].isOn = false;
                    }
                }

                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.MyParlorMessageStastu = 1;
                pspd.FromWebPointParlorTabelMessage(5);
                ParlorTableGame.gameObject.SetActive(true);
                ShowPointPanel(ParlorPanel.TabelGame);
                ParlorTableGame.transform.SetAsLastSibling();
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
            }
            else
            {
                ParlorBtnALL[index].transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
            }
        }


        /// <summary>
        /// 选择地区的确定按钮
        /// </summary>
        public void BtnSelectAreaOk(int type)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //if (type == 4)
            //{
            //    if (pspd.iCountyId[1] != pspd.temp_cc[1])
            //    {
            //        pspd.TimeOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            //        pspd.MemberCountOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            //        pspd.ActivityOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            //        pspd.pageId = new int[3] { 1, 1, 1 };
            //    }

            //    if (pspd.temp_cc[0] != 0 && pspd.temp_cc[1] != 0)
            //    {
            //        pspd.iCityId[0] = pspd.temp_cc[0];
            //        pspd.iCountyId[0] = pspd.temp_cc[1];
            //        pspd.iCityId[1] = pspd.temp_cc[0];
            //        pspd.iCountyId[1] = pspd.temp_cc[1];
            //    }
            //}

            //更新申请麻将馆的城市
            if (type == 3)
            {
                string area = MahjongCommonMethod.Instance._dicCityConfig[GameData.Instance.SelectAreaPanelData.iCityId].CITY_NAME +
               MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
                ChoiceArea[0].text = area;
            }
            //更新雀神广场的馆的城市
            else if (type == 4)
            {
                //关闭城市信息
                ParlorGodPanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);

                //string area = MahjongCommonMethod.Instance._dicDisConfig[pspd.iCountyId[1]].COUNTY_NAME;
                //ChoiceArea[1].text = area;
                //在这里重新请求该县的麻将馆信息
                //pspd.FromWebGetParlorMessage(1, 5);
                UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.NoParlorMessage(1);
            }
        }


        /// <summary>
        /// 关闭二次确认申请开馆的请求
        /// </summary>
        public void BtnCloseCreateParlorRemind()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowSecondSure = false;
            SecondSureCreatParlorPanel.SetActive(false);
        }

        /// <summary>
        /// 获取搜索到的麻将馆信息
        /// </summary>
        public void BtnGodRoundSearch(InputField input)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            if (input.text.Length == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您输入的馆号为空，请重新输入!");
                return;
            }

            if (input.text.Length != 5)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您输入的馆号错误，请重新输入!");
                return;
            }
            pspd.FromWebGetSearchParlorMessage(5, input.text);
            pspd.isShowSearchPointParlor = true;
        }


        /// <summary>
        /// 关闭雀神广场的按钮
        /// </summary>
        public void BtnCloseParlorGodRound()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (pspd.isShowSearchPointParlor)
            {
                //ParlorGodPanel.transform.GetChild(1).gameObject.SetActive(true);
                //ParlorGodPanel.transform.GetChild(2).gameObject.SetActive(false);
                pspd.isShowSearchPointParlor = false;
            }
            else
            {
                ParlorGodPanel.gameObject.SetActive(false);
                if (ParlorGodNoviceGuidance != null) ParlorGodNoviceGuidance.SetActive(false);
                if (!pspd.isShowMyParlorMessage)
                {
                    InitData();
                }

                if (pspd.isChangeParlorMessage)
                {
                    pspd.isChangeParlorMessage = false;
                    GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
                    SDKManager.Instance.GetComponent<CameControler>().PostMsg("loading", "正在获取您的麻将馆信息");
                }
            }

            UpdateShow();

            //Debug.LogError("pspd.isShowMyParlorMessage：" + pspd.isShowMyParlorMessage);
        }

        //点击已经未开始游戏的按钮
        public void BtnUnStart(Toggle toggle)
        {
            if (toggle.isOn)
            {
                UIMainView.Instance.ParlorShowPanel.ShowStartedRedPoint(0);
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                //获取当前进入的麻将馆的信息
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                {
                    if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                    {
                        if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iStatus == 1)
                        {
                            UIMgr.GetInstance().GetUIMessageView().Show("麻将馆已被封，无法进行任何操作，详情请联系官方客服！");
                            return;
                        }
                    }
                }
                //关闭已经开始游戏的面板
                ParlorTableGame.ShowPointPanel(1);
                SpwanPorlorTabelMessage(2);
                toggle.transform.GetChild(1).GetComponent<Text>().color = new Color(0.62f, 0.45f, 0.2f, 1f);
            }
            else
            {
                toggle.transform.GetChild(1).GetComponent<Text>().color = new Color(0.25f, 0.5f, 0.5f, 1f);
            }
        }

        //点击已经开始游戏的按钮
        public void BtnStarted(Toggle toggle)
        {
            if (toggle.isOn)
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                //获取当前进入的麻将馆的信息
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                {
                    if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                    {
                        if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iStatus == 1)
                        {
                            UIMgr.GetInstance().GetUIMessageView().Show("麻将馆已被封，无法进行任何操作，详情请联系官方客服！");
                            return;
                        }
                    }
                }
                //关闭已经开始游戏的面板
                ParlorTableGame.ShowPointPanel(2);
                SpwanPorlorTabelMessage(3, 1);
                toggle.transform.GetChild(1).GetComponent<Text>().color = new Color(0.62f, 0.45f, 0.2f, 1f);
            }
            else
            {
                toggle.transform.GetChild(1).GetComponent<Text>().color = new Color(0.25f, 0.5f, 0.5f, 1f);
            }
        }

        /// <summary>
        /// 点击麻将广场的规则说明按钮
        /// </summary>
        public void BtnParlorRulePanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowRulePanel = true;
            UpdateShow();
        }

        /// <summary>
        /// 关闭麻将馆的规则说明
        /// </summary>
        public void BtnCloseParlorRulePanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowRulePanel = false;
            UpdateShow();
        }

        /// <summary>
        /// 点击切换查看不同月份的业绩记录
        /// </summary>
        /// <param name="drop"></param>
        public void BtnChangeMonthStatus(Dropdown drop)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            drop.transform.GetChild(1).localEulerAngles += new Vector3(0, 0, 180);
            //本月
            if (drop.value == 0)
            {
                pspd.Status_BossScore = 1;
            }
            //下一个月
            else
            {
                pspd.Status_BossScore = 2;
            }
            pspd.FormWebGetParlorBossScoreLogMessage();

        }

        /// <summary>
        /// 点击选择月份信息
        /// </summary>
        /// <param name="toggle"></param>
        public void BtnSelectMonth(Toggle toggle)
        {
            RectTransform parent = toggle.GetComponentInParent<RectTransform>();
            parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 120);
            Toggle[] tog = parent.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < tog.Length; i++)
            {
                if (tog[i].isOn)
                {
                    tog[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0.91f, 0.86f, 0.34f, 1);
                }
                else
                {
                    tog[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }

            //if (toggle.isOn)
            //{
            //    toggle.transform.GetChild(0).GetComponent<Image>().color = new Color(0.91f, 0.86f, 0.34f, 1);
            //}
            //else
            //{
            //    toggle.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            //}
        }

        /// <summary>
        /// 点击业绩界面的统计按钮
        /// </summary>
        public void BtnBossScoreStat()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.FromWebGetParlorBossScoreStatData();
            ScoreStatisticsPanel.gameObject.SetActive(true);
        }
        #endregion


        #region Function

        /// <summary>
        /// 对麻将馆名字进行屏蔽字检查
        /// </summary>
        /// <param name="field"></param>
        public string CheckForbidFont(InputField field)
        {
            RealNameApprovePanelData rnapd = GameData.Instance.RealNameApprovePanelData;
            string fornidContent = Filter(field.text);

            Debug.Log ("要检查的内容:" + field.text);

            //如果有屏蔽字  直接给出提示
            if (fornidContent != null)
            {
                StringBuilder str = new StringBuilder();
                str.Append("您的麻将馆名字包含敏感字\"");
                str.Append(fornidContent);
                str.Append("\"，请重新填写");
                //MahjongCommonMethod.Instance.ShowRemindFrame(str.ToString());
                UIMgr.GetInstance().GetUIMessageView().Show(str.ToString());
                return fornidContent;
            }
            return null;
        }

        /// <summary>
        /// 过滤输入的文字
        /// </summary>
        /// <param name="msg">Input输入的文字</param>
        /// <returns></returns>
        public string Filter(string msg)
        {
            RealNameApprovePanelData rnapd = GameData.Instance.RealNameApprovePanelData;
            GameData gd = GameData.Instance;
            for (int i = 0; i < rnapd.ForbiddenNameList.Count; i++)
            {
                if (msg.IndexOf(rnapd.ForbiddenNameList[i]) >= 0)
                {
                    return rnapd.ForbiddenNameList[i];
                }
            }
            return null;
        }


        /// <summary>
        /// 获取已经产生的所有的加入和申请馆的button
        /// </summary>
        public ShowParlorMessage[] DeleteMyParlorMessage()
        {
            Transform trans = Boss_Mem[1].transform;
            ShowParlorMessage[] go = trans.GetComponentsInChildren<ShowParlorMessage>(true);
            return go;
        }

        /// <summary>
        /// 显示我的所有已加入的馆的信息
        /// </summary>
        /// <param name="Info">对应麻将馆的信息</param>
        /// <param name="status">状态1表示产生自己的麻将馆 2表示产生自己的创建按钮 3表示雀神广场的麻将馆 </param>
        /// <param name="trans">父物体</param>
        public void ShowMyParlorMessage(NetMsg.ParlorInfoDef Info, int status, Transform trans, bool isSpecial = false)
        {
            string path = "";
            if (status == 1 || status == 2)
            {
                path = "Lobby/Parlor/SelfParlor";
            }
            else if (status == 3)
            {
                path = "Lobby/Parlor/Parlor";
            }

            GameObject go = Instantiate(Resources.Load<GameObject>(path));
            //if (status == 2)
            //{
            //    go.name = "JoinParlor";
            //}

            go.transform.SetParent(trans);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            if (status < 3)
            {
                go.transform.GetComponent<Toggle>().group = trans.GetComponent<ToggleGroup>();
            }

            //更新自己加入的麻将馆界面
            if (status == 1 || status == 3)
            {
                //更新界面
                if (status == 1)
                {
                    go.transform.GetComponent<ShowParlorMessage>().status = 2;
                    go.transform.GetComponent<ShowParlorMessage>().UpdateParlorBtn(Info);
                }
                else
                {
                    go.transform.GetComponent<ShowParlorMessage>().status = 1;
                    go.transform.GetComponent<ShowParlorMessage>().UpdateShow(Info);
                }

                //显示加入状态
                if (isSpecial && status == 3)
                {
                    go.transform.GetComponent<ShowParlorMessage>().UpdateShowStatus(2);
                    go.transform.GetComponent<ShowParlorMessage>().status = 1;
                }
            }
        }


        /// <summary>
        /// 显示指定某个麻将馆信息
        /// </summary>
        /// <param name="index"></param>
        public void PointShowParlor(int index)
        {
            ShowParlorMessage[] parlor = new ShowParlorMessage[0];
            parlor = Boss_Mem[1].GetComponentsInChildren<ShowParlorMessage>();
            bool isContain = false;
            parlor[parlor.Length - 1].GetComponent<Toggle>().isOn = true;
            for (int i = 0; i < parlor.Length; i++)
            {
                if (parlor[i].InfoDef.iParlorId == index)
                {
                    isContain = true;
                    parlor[i].GetComponent<Toggle>().isOn = true;
                    parlor[i].BtnLeftParlor(parlor[i].GetComponent<Toggle>());
                }
                else
                {
                    parlor[i].GetComponent<Toggle>().isOn = false;
                }
            }

            if (!isContain)
            {
                parlor[0].GetComponent<Toggle>().isOn = true;
            }
        }

        InfinityGridLayoutGroup infinityGridLayoutGroup;


        /// <summary>
        /// 显示指定县的所有的麻将馆的信息
        /// </summary>
        /// <param name="status">0表示不删除原有的   1表示删除之前的</param>        
        /// <param name="type">指定类型</param>        
        public void ShowPointCountyParlor(int status, int type)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Transform parent = ParlorGodPanel.transform.GetChild(0).GetChild(0);
            //ParlorGodPanel.transform.GetChild(1).gameObject.SetActive(true);
            //ParlorGodPanel.transform.GetChild(2).gameObject.SetActive(false);
            //获取当前指定的麻将馆的数量
            ShowParlorMessage[] parlor = parent.GetComponentsInChildren<ShowParlorMessage>(true);

            int count = 0;
            //如果麻将馆的数量不大于8，直接产生
            if (pspd.CountyParlorCount <= 12)
            {
                //如果数量大于4，打开滑动               
                count = pspd.CountyParlorCount;
            }
            else
            {
                count = 12;
                ParlorGodPanel.transform.GetChild(0).GetComponent<ScrollRect>().enabled = true;
            }


            //比较玩家之前产生玩家的预置体数量            
            int Normal_Count = ((count > parlor.Length) ? count : parlor.Length);

            for (int i = 0; i < Normal_Count; i++)
            {
                //说明预置体数量大于要产生的数量
                if (i > count - 1)
                {
                    parlor[i].gameObject.SetActive(false);
                }
                else
                {
                    //产生新的预置体                    
                    if (i > parlor.Length - 1)
                    {
                        //产生对应的麻将馆的预置体
                        if (type == 1)
                        {
                            ShowMyParlorMessage(pspd.TimeOrderParlorMessage[i], 3, parent);
                        }
                        else if (type == 2)
                        {
                            ShowMyParlorMessage(pspd.MemberCountOrderParlorMessage[i], 3, parent);
                        }
                        else if (type == 3)
                        {
                            ShowMyParlorMessage(pspd.ActivityOrderParlorMessage[i], 3, parent);
                        }
                    }
                    //更新原有的预置体
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        NetMsg.ParlorInfoDef info = new NetMsg.ParlorInfoDef();
                        if (type == 1)
                        {
                            info = pspd.TimeOrderParlorMessage[i];
                        }
                        else if (type == 2)
                        {
                            info = pspd.MemberCountOrderParlorMessage[i];
                        }
                        else if (type == 3)
                        {
                            info = pspd.ActivityOrderParlorMessage[i];
                        }
                        parlor[i].transform.GetComponent<ShowParlorMessage>().UpdateShow(info);
                    }
                }

            }
            parent.GetComponent<GridLayoutGroup>().enabled = true;
            parent.GetComponent<ContentSizeFitter>().enabled = true;
            infinityGridLayoutGroup = parent.GetComponent<InfinityGridLayoutGroup>();
            //如果数量过多启用分页功能
            if (pspd.CountyParlorCount > 12)
            {
                infinityGridLayoutGroup.enabled = true;
                //初始化数据列表;                
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(pspd.CountyParlorCount);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateParlorGodMessage;
            }
            else
            {
                //初始化数据列表;                
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.enabled = false;
            }
        }



        /// <summary>
        /// 更新雀神广场的麻将馆的界面信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="trans"></param>
        void UpdateParlorGodMessage(int index, Transform trans)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            ShowParlorMessage parlor = trans.GetComponent<ShowParlorMessage>();
            if (pspd.iNowCheckType == 1)
            {
                if (index > pspd.TimeOrderParlorMessage.Count - 1)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    parlor.UpdateShow(pspd.TimeOrderParlorMessage[index]);
                }
                if (!pspd.isGetAllData_PointCountyParlor[pspd.iNowCheckType - 1] && index == pspd.TimeOrderParlorMessage.Count - 10)
                {
                    pspd.FromWebGetParlorMessage(pspd.iNowCheckType, 5);
                }
            }
            else if (pspd.iNowCheckType == 2)
            {
                if (index > pspd.MemberCountOrderParlorMessage.Count - 1)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    parlor.UpdateShow(pspd.MemberCountOrderParlorMessage[index]);
                }
                if (!pspd.isGetAllData_PointCountyParlor[pspd.iNowCheckType - 1] && index == pspd.MemberCountOrderParlorMessage.Count - 10)
                {
                    pspd.FromWebGetParlorMessage(pspd.iNowCheckType, 5);
                }
            }
            else if (pspd.iNowCheckType == 3)
            {
                if (index > pspd.ActivityOrderParlorMessage.Count - 1)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    parlor.UpdateShow(pspd.ActivityOrderParlorMessage[index]);
                }
                if (!pspd.isGetAllData_PointCountyParlor[pspd.iNowCheckType - 1] && index == pspd.ActivityOrderParlorMessage.Count - 10)
                {
                    pspd.FromWebGetParlorMessage(pspd.iNowCheckType, 5);
                }
            }

            //Debug.LogError("index:" + index + ",页数:" + (pspd.pageId[pspd.iNowCheckType - 1] - 1) + ",count:" + pspd.TimeOrderParlorMessage.Count + ",status:" + pspd.isGetAllData_PointCountyParlor[pspd.iNowCheckType - 1]);

        }


        /// <summary>
        /// 清空信息
        /// </summary>
        /// <param name="parlorId"></param>
        public void ClearPointParlorPrefab(int parlorId)
        {
            ShowParlorMessage[] parlor = Boss_Mem[1].transform.GetComponentsInChildren<ShowParlorMessage>();
            for (int i = 0; i < parlor.Length; i++)
            {
                if (parlor[i].InfoDef.iParlorId == parlorId)
                {
                    Destroy(parlor[i].gameObject);
                }
            }
        }

        /// <summary>
        /// 显示馆主搜索到的玩家的信息
        /// </summary>
        /// <param name="isBelongParlor">是否属于本馆成员</param>
        public void ShowSearchMemberMessage(byte byBelongParlor, NetMsg.UserInfoDef info)
        {
            //首先关闭提示信息
            RemingMessage.SetActive(false);
            MemberMessage.SetActive(true);
            Debug.LogError("GameData.Instance.PlayerNodeDef.iUserId:" + GameData.Instance.PlayerNodeDef.iUserId + ",info.iUserId：" + info.iUserId
                + "，byBelongParlor：" + byBelongParlor);


            //显示馆的信息
            MahjongCommonMethod.Instance.GetPlayerAvatar(MemberMessage.transform.GetChild(0).GetComponent<RawImage>(), info.szHeadimgurl);
            MemberMessage.transform.GetChild(1).GetComponent<Text>().text = info.szNickname;
            float speed = 0;
            if (info.iPlayCardAcc != 0)
            {
                speed = info.iPlayCardTimeAcc / info.iPlayCardAcc;
            }
            MemberMessage.transform.GetChild(2).GetComponent<Text>().text = "出牌速度:" + speed + "秒/次";
            MemberMessage.transform.GetChild(3).GetComponent<Text>().text = "被踢次数:" + info.iKickParlorAcc.ToString();
            MemberMessage.transform.GetChild(4).GetComponent<Text>().text = "退馆次数:" + info.iLeaveParlorAcc.ToString();

            //显示时邀请用户还是踢出用户        
            if (GameData.Instance.PlayerNodeDef.iUserId == info.iUserId)
            {
                BtnInviteAndLevel.SetActive(false);
            }
            else
            {
                BtnInviteAndLevel.SetActive(true);
            }
        }


        /// <summary>
        /// 产生对应的预置体信息
        /// </summary>
        public void SpwanParlorMemberMessage()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (pspd.ParlorAllMemberMessage.Count == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您的麻将馆还没有人加入哦！");
                return;
            }

            Transform parent = ParlorTableGame.Member.transform.GetChild(1).GetChild(0);
            //计算之前的预置体数量
            MemberListMessage[] parlor = parent.GetComponentsInChildren<MemberListMessage>(true);

            int count_0 = pspd.ParlorAllMemberMessage.Count > 6 ? 6 : pspd.ParlorAllMemberMessage.Count;
            int count_1 = parlor.Length > count_0 ? parlor.Length : count_0;
            for (int i = 0; i < count_1; i++)
            {
                MemberListMessage temp = null;
                if (i > parlor.Length - 1)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/MemberMessage"));
                    go.transform.SetParent(parent);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    temp = go.transform.GetComponent<MemberListMessage>();
                }
                else
                {
                    if (i > count_0 - 1)
                    {
                        parlor[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        temp = parlor[i].GetComponent<MemberListMessage>();
                    }
                }

                if (temp != null)
                {
                    temp.UpdateShow_Member(pspd.ParlorAllMemberMessage[i]);
                }
                parent.GetComponent<GridLayoutGroup>().enabled = true;
                parent.GetComponent<ContentSizeFitter>().enabled = true;
                ParlorTableGame.ShowPointPanel(3);

                infinityGridLayoutGroup = parent.GetComponent<InfinityGridLayoutGroup>();
                if (pspd.ParlorAllMemberMessage.Count > 6)
                {
                    //初始化面板显示数据                    
                    infinityGridLayoutGroup.RemoveListener_Rect();
                    infinityGridLayoutGroup.Init();
                    infinityGridLayoutGroup.SetAmount(pspd.MemberNum);
                    infinityGridLayoutGroup.updateChildrenCallback = UpdateMemberMessage;
                }
                else
                {
                    //初始化数据列表;                
                    infinityGridLayoutGroup.RemoveListener_Rect();
                    infinityGridLayoutGroup.Init();
                }
            }

            ////如果是老板显示搜索按钮，否则隐藏
            //if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            //{
            //BtnSearch.gameObject.SetActive(true);
            //}
            //else
            //{
            //    BtnSearch.gameObject.SetActive(false);
            //}
        }

        //更新麻将馆内的成员信息列表
        void UpdateMemberMessage(int index, Transform trans)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            MemberListMessage parlor = trans.GetComponent<MemberListMessage>();
            if (index < pspd.ParlorAllMemberMessage.Count)
            {
                parlor.UpdateShow_Member(pspd.ParlorAllMemberMessage[index]);
            }
            else
            {
                trans.gameObject.SetActive(false);
            }

            //Debug.LogError("index:" + index + ",pspd.ParlorAllMemberMessage[index]:" + pspd.ParlorAllMemberMessage[index].nickname +
            //    ",状态:" + pspd.isGetAllData_ParlorMember + ",pspd.ParlorAllMemberMessage.Count：" + pspd.ParlorAllMemberMessage.Count);
            if (!pspd.isGetAllData_ParlorMember && index == pspd.ParlorAllMemberMessage.Count - 10)
            {
                ParlorTableGame.Member.transform.GetChild(1).GetComponent<ScrollRect>().enabled = false;
                pspd.FromWebGetParlorMember(2);
            }
        }


        //处理审核列表信息
        public void SpwanParlorCheckList()
        {
            ShowPointRedPoint(1, ParlorBtn.CheckList);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (pspd.ParlorCheckList.Count == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("亲，暂无其他玩家申请哦！");
                return;
            }

            //打开审核列表面板
            ShowPointPanel(ParlorPanel.CheckList);
            ParlorCheckList.transform.SetAsLastSibling();

            //赋值父物体
            Transform parent = ParlorCheckList.transform.GetChild(1).GetChild(0);
            Debug.Log("玩家申请数量:" + pspd.ParlorCheckList.Count);
            //如果成员数量大于6
            if (pspd.ParlorCheckList.Count > 7)
            {
                int count = parent.childCount; //保存当前玩家子物体的数量
                if (count < 7)
                {
                    for (int i = 0; i < 7 - count; i++)
                    {
                        GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/CheckUserMessage"));
                        go.transform.SetParent(parent);
                        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                        go.transform.localEulerAngles = Vector3.zero;
                        go.transform.localScale = Vector3.one;
                        go.transform.name = "CheckUserMessage_" + i;
                        go.GetComponent<MemberListMessage>().UpdateShow_Check(pspd.ParlorCheckList[i]);
                    }
                }

                //初始化面板显示数据
                infinityGridLayoutGroup = parent.GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(pspd.ParlorCheckList.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateCheckList;
            }
            else
            {

                //删除之前的预置体
                MemberListMessage[] parlor = parent.GetComponentsInChildren<MemberListMessage>();
                for (int i = 0; i < parlor.Length; i++)
                {
                    Destroy(parlor[i].gameObject);
                }

                //产生预置体
                for (int i = 0; i < pspd.ParlorCheckList.Count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/CheckUserMessage"));
                    go.transform.SetParent(parent);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<MemberListMessage>().UpdateShow_Check(pspd.ParlorCheckList[i]);
                }
            }
        }

        //更新审核列表信息
        void UpdateCheckList(int index, Transform trans)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            MemberListMessage parlor = trans.GetComponent<MemberListMessage>();
            parlor.UpdateShow_Check(pspd.ParlorCheckList[index]);
        }

        /// <summary>
        /// 产生搜索结果的预置体
        /// </summary>
        public void SwpanSearchResult()
        {
            //打开搜索结果面板
            ParlorGodPanel.transform.GetChild(1).gameObject.SetActive(false);
            ParlorGodPanel.transform.GetChild(2).gameObject.SetActive(true);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //获取当前指定的麻将馆的数量
            RectTransform[] parlor = ParlorGodPanel.transform.GetChild(2).GetChild(0).GetComponentsInChildren<RectTransform>();
            if (parlor.Length > 0)
            {
                for (int i = 1; i < parlor.Length; i++)
                {
                    Destroy(parlor[i].gameObject);
                }
            }

            //产生对应的麻将馆的预置体
            for (int i = 0; i < pspd.IdOrNameGetParlorMessageList.Count; i++)
            {
                string path = "Lobby/Parlor/Parlor";
                GameObject go = Instantiate(Resources.Load<GameObject>(path));
                go.transform.SetParent(ParlorGodPanel.transform.GetChild(2).GetChild(0));
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                //更新自己加入的麻将馆界面
                go.transform.GetComponent<ShowParlorMessage>().status = 1;
                NetMsg.ParlorInfoDef InfoDef = new NetMsg.ParlorInfoDef();
                InfoDef.szParlorName = pspd.IdOrNameGetParlorMessageList[i].parlorName;
                InfoDef.iCountyId = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].countyId);
                InfoDef.iMemberNum = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].memberNum);
                InfoDef.iVitality = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].vitality);
                InfoDef.iMonthVitality = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].monthVitality);
                InfoDef.szBossHeadimgurl = pspd.IdOrNameGetParlorMessageList[i].head;
                InfoDef.szBulletin = pspd.IdOrNameGetParlorMessageList[i].bulletin;
                InfoDef.szContact = pspd.IdOrNameGetParlorMessageList[i].contact;
                InfoDef.iBossId = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].bossId);
                InfoDef.iParlorId = System.Convert.ToInt32(pspd.IdOrNameGetParlorMessageList[i].parlorId);
                go.transform.GetComponent<ShowParlorMessage>().UpdateShow(InfoDef);
            }
        }

        int PorlorTabelType = 0;  //类型        
        /// <summary>
        /// 产生对应麻将馆的所有信息
        /// </summary>
        /// <param name="type">2表示未开始游戏的房间 3表示已经开始游戏的房间</param>
        /// <param name="status">0表示默认状态  1表示会删除之前的预置体信息</param>
        public void SpwanPorlorTabelMessage(int type, int status = 0)
        {
            string path = " ";  //预置体路径
            PorlorTabelType = type;
            Transform trans_unstart = ParlorTableGame.UnStart.transform;  //产生的预置体的父物体
            Transform trans_started = ParlorTableGame.Started.transform; ;
            Transform trans_parent = null;
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            int count = 0; //要产生的预置体的数量

            if (type == 2)
            {
                path = "Lobby/Parlor/UnStartRoom";
                trans_unstart.gameObject.SetActive(true);
                trans_started.gameObject.SetActive(false);
                ParlorTableGame.NoRecord_Started.SetActive(false);
                trans_parent = trans_unstart;
                count = (pspd.PointParlorTabelMessage_UnStart.Count > 9 ? 9 : pspd.PointParlorTabelMessage_UnStart.Count);
            }
            else
            {
                path = "Lobby/Parlor/StartedRoom";
                //trans_started.gameObject.SetActive(true);
                trans_parent = trans_started;
                //打开无记录
                if (pspd.PointParlorTabelMessage_Started.Count == 0)
                {
                    ParlorTableGame.NoRecord_Started.SetActive(true);
                    trans_started.gameObject.SetActive(false);
                }
                else
                {
                    ParlorTableGame.NoRecord_Started.SetActive(false);
                    trans_started.gameObject.SetActive(true);
                }
                count = (pspd.PointParlorTabelMessage_Started.Count > 16 ? 16 : pspd.PointParlorTabelMessage_Started.Count);
            }

            ParlorTabelMessage[] temp = trans_parent.GetComponentsInChildren<ParlorTabelMessage>(true);
            int count_ = (count > temp.Length) ? count : temp.Length;

            //Debug.LogError("count:" + count + ",temp:" + temp.Length + ",pspd.iAllTabelNumUnStart:" + pspd.iAllTabelNumUnStart
            //    + ",count_:" + count_);
            trans_parent.GetComponent<GridLayoutGroup>().enabled = true;
            trans_parent.GetComponent<ContentSizeFitter>().enabled = true;

            for (int i = 0; i < count_; i++)
            {
                GameObject go = null;
                if (i > temp.Length - 1)
                {
                    go = Instantiate(Resources.Load<GameObject>(path));
                    go.transform.SetParent(trans_parent);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                }
                else
                {
                    if (temp[i].DTimer > 0)
                    {
                        Destroy(temp[i].gameObject);
                        go = Instantiate(Resources.Load<GameObject>(path));
                        go.transform.SetParent(trans_parent);
                        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                        go.transform.localEulerAngles = Vector3.zero;
                        go.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        temp[i].gameObject.SetActive(false);
                        if (type == 3)
                        {
                            if (i > pspd.PointParlorTabelMessage_Started.Count - 1)
                            {
                                temp[i].gameObject.SetActive(false);
                            }
                            else
                            {
                                temp[i].gameObject.SetActive(true);
                                go = temp[i].gameObject;
                            }
                        }
                        else if (type == 2)
                        {
                            if (i > pspd.PointParlorTabelMessage_UnStart.Count - 1)
                            {
                                temp[i].gameObject.SetActive(false);
                            }
                            else
                            {
                                temp[i].gameObject.SetActive(true);
                                go = temp[i].gameObject;
                            }
                        }
                    }
                }


                if (go != null)
                {
                    if (i == 0)
                    {
                        go.transform.SetAsFirstSibling();
                    }

                    if (type == 2)
                    {
                        if (i < pspd.PointParlorTabelMessage_UnStart.Count)
                        {
                            go.GetComponent<ParlorTabelMessage>().UpdateShow(type, pspd.PointParlorTabelMessage_UnStart[i], null);
                        }
                        else
                        {
                            go.SetActive(false);
                        }

                    }
                    else if (type == 3)
                    {
                        if (i < pspd.PointParlorTabelMessage_Started.Count)
                        {
                            go.GetComponent<ParlorTabelMessage>().UpdateShow(type, null, pspd.PointParlorTabelMessage_Started[i]);
                        }
                        else
                        {
                            go.SetActive(false);
                        }
                    }
                }
            }

            int tempcount = 0;
            int COUNT = 9;
            if (type == 2)
            {
                tempcount = pspd.iAllTabelNumUnStart;
                COUNT = 9;
            }
            else if (type == 3)
            {
                tempcount = pspd.PointParlorTabelMessage_Started.Count;
                COUNT = 16;
            }

            infinityGridLayoutGroup = trans_parent.GetComponent<InfinityGridLayoutGroup>();

            infinityGridLayoutGroup.RemoveListener_Rect();

            //Debug.LogError("tempcount:" + tempcount);


            //如果房间数量过多
            if (tempcount > COUNT)
            {
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(tempcount);
                infinityGridLayoutGroup.updateChildrenCallback = UpdatePorlorTabelMessage;
            }

            //更新玩家信息
            UpdateMemberTabelCount();
        }

        void UpdatePorlorTabelMessage(int index, Transform trans)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            ParlorTabelMessage parlor = trans.GetComponent<ParlorTabelMessage>();
            if (PorlorTabelType == 2)
            {
                //请求下一页的桌的信息                
                if (!pspd.isGetUnStartDataEnd && index == pspd.PointParlorTabelMessage_UnStart.Count - 10)
                {
                    NetMsg.ClientGetParlorTableInfoReq msg = new NetMsg.ClientGetParlorTableInfoReq();
                    msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    msg.iPageNum = pspd.iPageId_TabelGame;
                    msg.iParlorId = pspd.iParlorId;
                    Debug.LogError("=-============================");
                    NetworkMgr.Instance.LobbyServer.SendClientGetParlorTableInfoReq(msg);
                }

                if (index > pspd.PointParlorTabelMessage_UnStart.Count - 1)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    parlor.UpdateShow(PorlorTabelType, pspd.PointParlorTabelMessage_UnStart[index], null);
                }
            }
            else if (PorlorTabelType == 3)
            {
                if (index > pspd.PointParlorTabelMessage_Started.Count - 1)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    parlor.UpdateShow(PorlorTabelType, null, pspd.PointParlorTabelMessage_Started[index]);
                }
            }
        }

        /// <summary>
        /// 更新桌面上指定的预置体
        /// </summary>
        /// <param name="TabelMessage"></param>
        public void UpdateParlorTabelGame_UnStart(NetMsg.TableInfoDef TabelMessage)
        {
            if (TabelMessage == null)
            {
                return;
            }
            Transform trans_unstart = ParlorTableGame.UnStart.transform;
            ParlorTabelMessage[] temp = trans_unstart.GetComponentsInChildren<ParlorTabelMessage>(true);
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].TabelInfo_Un.iRoomNum == TabelMessage.iRoomNum)
                {
                    temp[i].gameObject.SetActive(false);
                    temp[i].UpdateShow(2, TabelMessage, null);
                    break;
                }
            }
        }

        /// <summary>
        /// 更新麻将馆已开始游戏的预置体信息
        /// </summary>
        /// <param name="TabelMessage"></param>
        public void UpdateParlorTabelGame_Started(ParlorShowPanelData.GetPointParlorTabelMessage TabelMessage)
        {
            if (TabelMessage == null)
            {
                return;
            }
            Transform trans_started = ParlorTableGame.Started.transform;
            ParlorTabelMessage[] temp = trans_started.GetComponentsInChildren<ParlorTabelMessage>(true);
            for (int i = 0; i < temp.Length; i++)
            {
                if (Equals(temp[i].TabelInfo_Ed.roomNum, TabelMessage.roomNum))
                {
                    gameObject.SetActive(false);
                    temp[i].UpdateShow(3, null, TabelMessage);
                    break;
                }
            }
        }


        /// <summary>
        /// 产生游戏记录预置体
        /// </summary>
        public void SpwanParlorGameRecord()
        {
            int count = 0; //产生的数量
            Transform trans = null;  //父物体
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            if (pspd.PointParlorGameRecordData.Count > 5)
            {
                count = 5;
            }
            else
            {
                count = pspd.PointParlorGameRecordData.Count;
            }
            ShowPointPanel(ParlorPanel.Record);
            ParlorGameRecord.transform.SetAsLastSibling();
            trans = ParlorGameRecord.transform.GetChild(0).GetChild(0);
            ParlorGameRecord[] parlor = trans.GetComponentsInChildren<ParlorGameRecord>();

            int count_ = (parlor.Length > count ? parlor.Length : count);

            //Debug.LogError("count_:" + count_ + ",pspd.PointParlorGameRecordData.Count:" + pspd.PointParlorGameRecordData.Count);

            for (int i = 0; i < count_; i++)
            {
                if (i > parlor.Length - 1)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/GameRecord"));
                    go.transform.SetParent(trans);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    //更新界面
                    go.transform.GetComponent<ParlorGameRecord>().UpdateShow(pspd.PointParlorGameRecordData[i]);
                }
                else
                {
                    if (i > count - 1)
                    {
                        parlor[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        parlor[i].GetComponent<ParlorGameRecord>().UpdateShow(pspd.PointParlorGameRecordData[i]);
                    }
                }
            }

            trans.GetComponent<GridLayoutGroup>().enabled = true;
            trans.GetComponent<ContentSizeFitter>().enabled = true;
            infinityGridLayoutGroup = trans.GetComponent<InfinityGridLayoutGroup>();
            if (pspd.PointParlorGameRecordData.Count > 5)
            {
                infinityGridLayoutGroup.enabled = true;
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(pspd.PointParlorGameRecordData.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateParlorGameRecord;
            }
        }

        void UpdateParlorGameRecord(int index, Transform tran)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            tran.transform.GetComponent<ParlorGameRecord>().UpdateShow(pspd.PointParlorGameRecordData[index]);
            //Debug.LogError("index:" + index + ",PointParlorGameRecordData:" + pspd.PointParlorGameRecordData[index].openRoomId +
            //    ",isGetAllData_ParlorGameRecord:" + pspd.isGetAllData_ParlorGameRecord);
            if (!pspd.isGetAllData_ParlorGameRecord && index == pspd.PointParlorGameRecordData.Count - 10)
            {
                UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.transform.GetChild(0).
                GetComponent<UnityEngine.UI.ScrollRect>().enabled = false;
                pspd.FromWebPointParlorThreeDayGameNum(6);
            }
            //Debug.LogError("index:" + index + ",PointParlorGameRecordData:" + pspd.PointParlorGameRecordData[index].openRoomId);
        }

        int type = 0;
        /// <summary>
        /// 产生老板业绩积分的预置体
        /// </summary>
        /// <param name="status"></param>
        public void SpwanBossScorePrefab(int status)
        {
            type = status;
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            int count = 0; //要产生的预置体的数量
            Transform parent = ParlorBossScore.transform.GetChild(3).GetChild(0);
            ScoreMessage[] parlor = parent.GetComponentsInChildren<ScoreMessage>(true);
            if (status == 1)
            {
                if (pspd.lsParlorBossScoreLog_Now.Count > 12)
                {
                    count = 12;
                }
                else
                {
                    count = pspd.lsParlorBossScoreLog_Now.Count;
                }
            }
            else
            {
                if (pspd.lsParlorBossScoreLog_Last.Count > 12)
                {
                    count = 12;
                }
                else
                {
                    count = pspd.lsParlorBossScoreLog_Last.Count;
                }
            }

            int count_ = (parlor.Length > count) ? parlor.Length : count;

            for (int i = 0; i < count_; i++)
            {
                ParlorShowPanelData.ParlorBossScoreLog info = new ParlorShowPanelData.ParlorBossScoreLog();
                if (status == 1)
                {
                    if (i < pspd.lsParlorBossScoreLog_Now.Count)
                    {
                        info = pspd.lsParlorBossScoreLog_Now[i];
                    }
                }
                else
                {
                    if (i < pspd.lsParlorBossScoreLog_Last.Count)
                    {
                        info = pspd.lsParlorBossScoreLog_Last[i];
                    }
                }
                if (i > parlor.Length - 1)
                {
                    GameObject go = SpwanPrefabCommon(parent, "Lobby/Parlor/ScoreMessage", null);
                    go.GetComponent<ScoreMessage>().UpdateShow(info);
                }
                else
                {
                    if (i > count - 1)
                    {
                        parlor[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        parlor[i].GetComponent<ScoreMessage>().UpdateShow(info);
                    }
                }
            }

            parent.GetComponent<GridLayoutGroup>().enabled = true;
            parent.GetComponent<ContentSizeFitter>().enabled = true;
            infinityGridLayoutGroup = parent.GetComponent<InfinityGridLayoutGroup>();
            if (pspd.ParlorBossScoreCount[pspd.Status_BossScore - 1] > 12)
            {
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(pspd.ParlorBossScoreCount[pspd.Status_BossScore - 1]);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateParlorScore;
            }

        }


        void UpdateParlorScore(int index, Transform tran)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            if (type == 1)
            {
                tran.GetComponent<ScoreMessage>().UpdateShow(pspd.lsParlorBossScoreLog_Now[index]);
                //Debug.LogError("index:" + index + ",文字信息:" + pspd.lsParlorBossScoreLog_Now[index].roomNum + ",count:" + pspd.lsParlorBossScoreLog_Now.Count);
                if (!pspd.isGetBossScoreFinish[pspd.Status_BossScore - 1] && index == pspd.lsParlorBossScoreLog_Now.Count - 15)
                {
                    pspd.FormWebGetParlorBossScoreLogMessage();
                }
            }
            else
            {
                tran.GetComponent<ScoreMessage>().UpdateShow(pspd.lsParlorBossScoreLog_Last[index]);
                //Debug.LogError("index:" + index + ",文字信息:" + pspd.lsParlorBossScoreLog_Last[index].roomNum);
                if (!pspd.isGetBossScoreFinish[pspd.Status_BossScore - 1] && index == pspd.lsParlorBossScoreLog_Last.Count - 15)
                {
                    pspd.FormWebGetParlorBossScoreLogMessage();
                }
            }


        }

        /// <summary>
        /// 产生预置体的通用方法
        /// </summary>
        /// <param name="parent">父物体</param>
        /// <param name="path">资源加载路径</param>
        /// <param name="status">0为默认状态 其他状态特殊处理</param>
        /// <returns></returns>
        GameObject SpwanPrefabCommon(Transform parent, string path, GameObject prefab, int status = 0)
        {
            GameObject go = null;
            if (path == null)
            {
                go = Instantiate(prefab);
            }
            else
            {
                go = Instantiate(Resources.Load<GameObject>(path));
            }
            go.transform.SetParent(parent);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            return go;
        }

        /// <summary>
        /// 产生所有小红包
        /// </summary>
        public void SpwanSmallRedBag()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Transform parent = ParlorSmallReaBagMessage.transform.GetChild(2).GetChild(0);
            ParlorSmallRedBag[] parlor = parent.GetComponentsInChildren<ParlorSmallRedBag>();
            int count = 0;  //红包数量
            count = (pspd.SmallRedBagMessage.Count > 6) ? 6 : pspd.SmallRedBagMessage.Count;
            int count_ = count > parlor.Length ? count : parlor.Length;

            Debug.LogError("count:" + count_ + ",数组数量:" + pspd.SmallRedBagMessage.Count);
            for (int i = 0; i < count_; i++)
            {
                ParlorSmallRedBag temp = null;
                if (i > parlor.Length - 1)
                {
                    temp = SpwanPrefabCommon(parent, null, parlor[0].gameObject).GetComponent<ParlorSmallRedBag>();
                    //更新界面
                    temp.UpdateShow(pspd.SmallRedBagMessage[i]);
                }
                else
                {
                    if (i > pspd.SmallRedBagMessage.Count - 1)
                    {
                        parlor[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        temp = parlor[i];
                        //更新界面
                        temp.UpdateShow(pspd.SmallRedBagMessage[i]);
                    }
                }
            }
            parent.GetComponent<ContentSizeFitter>().enabled = true;
            parent.GetComponent<GridLayoutGroup>().enabled = true;
            infinityGridLayoutGroup = parent.GetComponent<InfinityGridLayoutGroup>();
            if (pspd.SmallRedBagMessage.Count > 6)
            {
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(pspd.SmallRedBagMessage.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateParlorSamllRedBag;
            }
        }

        void UpdateParlorSamllRedBag(int index, Transform trans)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            if (trans != null)
            {
                trans.GetComponent<ParlorSmallRedBag>().UpdateShow(pspd.SmallRedBagMessage[index]);
            }
        }

        /// <summary>
        /// 产生指定的麻将馆的面板
        /// </summary>
        /// <param name="InfoDef">麻将馆信息</param>
        /// <param name="type">1表示显示申请加馆 2表示取消加馆</param>
        public void SpwanPointParlorPanel(NetMsg.ParlorInfoDef InfoDef, int type)
        {
            //产生对应的面板信息
            GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/ParlorMessage"));
            go.transform.SetParent(UIMainView.Instance.ParlorShowPanel.transform);
            go.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            go.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            //  go.transform.GetComponent<RectTransform>().pivot = Vector2.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            //更新界面显示信息
            go.transform.GetComponent<ParlorMessage>().UpdateShow(InfoDef, type);
        }


        #region 定时执行某个方法
        IEnumerator temp;
        public void TimerDoSomeThing(float time, System.Action<int, int> action)
        {
            if (temp != null)
            {
                //Debug.LogError("停止协成");
                StopCoroutine(temp);
            }
            if (gameObject.activeSelf)
            {
                temp = Timer(time, action);
                StartCoroutine(temp);
            }
        }
        IEnumerator Timer(float time, System.Action<int, int> action)
        {
            yield return new WaitForSeconds(time);
            action(6, 2);
        }
        #endregion



        /// <summary>
        /// 更新麻将馆的申请和加入状态
        /// </summary>
        /// <param name="parlorId"></param>
        /// <param name="status">0不显示  1表示已加入 2表示已申请</param>
        public void UpdateParlorStatus(int parlorId, int status)
        {
            ShowParlorMessage[] parlor = ParlorGodPanel.transform.GetChild(0).GetChild(0).GetComponentsInChildren<ShowParlorMessage>();
            for (int i = 0; i < parlor.Length; i++)
            {
                if (parlorId == parlor[i].InfoDef.iParlorId)
                {
                    parlor[i].UpdateShowStatus(status);
                    break;
                }
            }
        }

        /// <summary>
        /// 删除指定的麻将馆的预置体
        /// </summary>
        /// <param name="parlorId"></param>
        public void DelPointParlorApplyMessage(int parlorId)
        {
            ShowParlorMessage[] parlor = Boss_Mem[1].transform.GetComponentsInChildren<ShowParlorMessage>();
            for (int i = 0; i < parlor.Length; i++)
            {
                if (parlor[i].InfoDef.iParlorId == parlorId)
                {
                    Destroy(parlor[i].gameObject);
                    break;
                }
            }
        }


        IEnumerator Refresh;
        //打开刷新界面
        public void OpenRefresh()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            Refresh = CloseRefresh();
            if (Refresh != null)
            {
                StopCoroutine(Refresh);
            }
            Refresh_God.SetActive(true);
            StartCoroutine(Refresh);
        }

        IEnumerator CloseRefresh()
        {
            yield return new WaitForSeconds(0.5f);
            Refresh_God.SetActive(false);
        }

        #endregion
    }

}
