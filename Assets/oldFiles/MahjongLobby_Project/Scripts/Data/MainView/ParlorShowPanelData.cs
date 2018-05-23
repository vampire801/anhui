using UnityEngine;
using System;
using System.Collections.Generic;
using MahjongLobby_AH.Network.Message;
using XLua;
using anhui;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorShowPanelData
    {

        #region 常量
        public const string SaveLaseParlorId = "SaveLaseParlorId";  //保存玩家上次进入的麻将馆的id
        public const string IsShowParlorRed = "IsShowParlorRed"; //是否已经显示过麻将馆红包，一天只显示一次
        #endregion


        #region url
        public string ApplyBossCert_Url = "http://192.168.1.22:8080/sx_gm/manage/user/sqlbkh.do?user_id=";
        #endregion
        public int RefreshCount = 0; //广场刷新次数
        public int ColdTimer = 10; //麻将馆的冷却时间
        public bool isShowPanel;  //面板是否显示
        public bool isShowRulePanel;  //显示规则说明面板
        public bool isShowParlorRoundPanel;  //显示雀神广场的面板
        //public bool isShowNormalMemberPanel;  //显示普通玩家进入麻将馆的面板
        public bool isShowApplyCreatParlorPanel;  //显示申请开办麻将馆资格的提示面板
        public bool isShowSecondSure;  //显示创建麻将馆的二次确认的面板
        public bool isShowWriteParlorName;  //填写创建麻将馆名字的面板
        //public bool isShowAreaSetting; //区域设置的面板显示
        public bool isShowChangeParlorMessage; //编辑麻将馆的公告和联系方式
        public bool isShowSecondDismissPanel;  //二次确认的面板显示
        public bool isShowSearchMemberPanel;  //搜索成员的面板
        public bool isShowSearchPointParlor;  //是否正在显示搜索到的指定麻将馆        
        public int iParlorId; //记录当前查看的馆的id
        public int iUserid_Seach; //记录馆主搜索的玩家的id
        public int ParlorRedStatus; //馆的当前红包的状态
        public float ParlorRedDownTimer;  //红包的倒计时时间  
        public bool isShowRedDownTimer; //是否开始显示红包倒计时   
        public bool isShowOrderTimePanel; //预约倒计时面板
        public int iUpdateOrderTimer; //从后台返回刷新倒计时面板界面
        public bool isShowEditParlorName; //是否关闭编辑麻将馆名字和地区面板
        public int iStatus_AreaSeeting; //点击进入选择区域的状态  1表示创建麻将馆选择区域  2表示雀神广场选择区域
        public int[] iCountyId = new int[3]; //县城id   0用来保存创建麻将馆的选择 1只用来保存玩家的雀神广场的选择  2保存玩家所在麻将馆的地区
        public int[] iCityId = new int[3];  //城市id
        public int[] temp_cc = new int[2]; //城市的临时id    
        public int MoreBtnStatus = 1; //1表示更多操作没点击状态  2表示点击过更多操作
        public string ForbidContent; //屏蔽字内容
        public bool isIncludeForbidFont; //是否包含屏蔽字        
        public bool isChangeParlorMessage; //是否改变麻将馆的申请信息

        //保存四个玩家的馆的信息
        public NetMsg.ParlorInfoDef[] parlorInfoDef = new NetMsg.ParlorInfoDef[4];   //如果自己是馆主，则把信息存放在数组的第一个位置
                                                                                     //如果自己不是馆主，则会把信息按玩家节点中的馆的信息顺序保存

        public ParlorRedBag_[] parlorRedBagInfo = new ParlorRedBag_[4]; //保存四个馆的红包信息 按照上边麻将馆的顺序添加

        public string szSelfParlorBulletin = "";  //保存玩家自己馆的公告信息
        public string szContact = "";  //保存玩家自己馆的联系方式

        public bool isShowMyParlorMessage;  //显示自己开的馆信息的界面

        public int MyParlorMessageStastu; //老板麻将馆界面信息 1表示在牌局界面  2表示在其他界面
        public int GetParlorMessageType = 1; //1表示正常获取麻将馆信息 2表示定时刷新麻将馆数据 3表示认证请求之后获取麻将馆信息
        //只保存当前开放的城市
        public List<SelectAreaPanelData.CityMessage> listCityMessage = new List<SelectAreaPanelData.CityMessage>();

        //保存城市对应的开放的县城的开启状况int 是市的id
        public Dictionary<int, List<SelectAreaPanelData.CountyMessage>> dicCountyMessage = new Dictionary<int, List<SelectAreaPanelData.CountyMessage>>();

        public string ParlorBulletin; //保存自己麻将馆的公告信息
        public string ParlorContact; //保存自己麻将馆的联系方式
        public bool isChangeBulletin; //是否修改了公告信息
        public bool isChangeContact; //是否修改了联系方式     
        public bool isSpwanUnStartRoom; //是否已经产生过未开始游戏的房间
        public bool isSpwanStartedRoom; //是否已经产生过已经开始游戏的房间
        public int iOpenRoomStatus;  //玩家在麻将馆的开房数量 0表示没有开房有没有预约房 1表示自己开了房间  2表示自己预约了房间
        //页码  (1表示时间的页码，2表示人数的页码 3表示活跃度的页码)
        public int[] pageId = new int[3] { 1, 1, 1 };
        public bool UnNeedSpecialSort; //不需要特殊排序，主要是指将自己的馆按照顺序排序
        //现在查看玩家的排序方式  1按时间排序 2按会员数量排序 3按月活跃度排序
        public int iNowCheckType;
        //保存该县所有的麻将馆的总数
        public int CountyParlorCount;
        //是否已经获取了指定县的所有的麻将馆的信息
        public bool[] isGetAllData_PointCountyParlor = new bool[3] { false, false, false };
        //保存时间排序 人数排序  活跃度排序的列表信息        
        public List<NetMsg.ParlorInfoDef> TimeOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
        public List<NetMsg.ParlorInfoDef> MemberCountOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
        public List<NetMsg.ParlorInfoDef> ActivityOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();

        public int iPageID_Member = 1; //获取指定馆的所有成员信息的页码
        public bool isGetAllData_ParlorMember; //全部获取到指定馆的所有成员信息
        public int MemberNum; //指定馆的成员数量
        //保存馆内的所有的成员信息
        public List<ParlorMemberMessage> ParlorAllMemberMessage = new List<ParlorMemberMessage>();

        //馆主收到的申请列表
        public List<PlayerMessagePanelData.Message> ParlorCheckList = new List<PlayerMessagePanelData.Message>();

        //保存玩家通过id或者馆名搜索到的麻将馆的信息
        public List<PointIdOrNameGetParlorMessage> IdOrNameGetParlorMessageList = new List<PointIdOrNameGetParlorMessage>();

        //保存指定馆的所有桌的信息
        public int iPageId_TabelGame = 0; //未开始游戏的桌的页码信息
        public bool isGetUnStartDataEnd; //获取未开始的桌的信息是否结束    
        public int iAllTabelNumUnStart; //总的未开始房间的数量    
        public List<NetMsg.TableInfoDef> PointParlorTabelMessage_UnStart = new List<NetMsg.TableInfoDef>();
        public List<GetPointParlorTabelMessage> PointParlorTabelMessage_Started = new List<GetPointParlorTabelMessage>();

        //保存该馆内的所有的游戏记录信息
        public int ParlorGameRecordPageId = 1; //读取的当前页码
        public int ParlorGameTotalNum; //总的牌局记录
        public bool isGetAllData_ParlorGameRecord; //全部获取到指定馆的所有的游戏记录信息
        public List<GetPointParlorGameRecordData> PointParlorGameRecordData = new List<GetPointParlorGameRecordData>();

        //保存麻将馆老板的业绩积分日志
        public int Status_BossScore = 1; //1表示本月 2表示上月
        public int[] PageId_BossScore = new int[2] { 1, 1 };  //保存本月和上月的页码 0表示本月的  1表示上月的
        public int[] ParlorBossScoreCount = new int[2]; //老板业绩的总数量本月的
        public int ParlorBossScoreCount_Last; //老板业绩的总数量上个月的
        public bool[] isGetBossScoreFinish = new bool[2]; //获取老板业绩是否获取完毕
        public int BossScoreCount = 0; //记录当前月的数量记录
        public List<ParlorBossScoreLog> lsParlorBossScoreLog_Now = new List<ParlorBossScoreLog>(); //本月业绩 
        public List<ParlorBossScoreLog> lsParlorBossScoreLog_Last = new List<ParlorBossScoreLog>();  //上月业绩
                                                                                                     //保存所有玩家的的信息
        public Dictionary<int, NetMsg.TableUserInfoDef> userInfo_Tabel = new Dictionary<int, NetMsg.TableUserInfoDef>();

        //保存老板的业绩积分统计信息
        public float CoinPayLength = 460; //消耗金币的100%的表现长度
        public float ScoreGetLength = 460f;  //获得积分100%的表现长度
        public float MinLength = 100; //最小长度
        public ParlorBossScoreStat[] ParlorBossScoreStatMessage = new ParlorBossScoreStat[2];

        public NetMsg.ParlorInfoDef InfoDef_PointParlor = new NetMsg.ParlorInfoDef(); //保存临时的麻将馆数据

        public int iOldParlorNum;  //玩家之前的馆的数量                    

        public int ComeInParlorType; //进入麻将馆的状态 2表示进入未开始游戏的界面 3表示进入已经开始游戏的界面
        #region 获取县所有麻将馆的信息
        //麻将馆的信息
        [Serializable]
        public class ParlorMessage
        {
            public string parlorId;  //麻将馆编号
            public string parlorName; //麻将馆名称
            public string bossId;  //馆主id
            public string nickname; //馆主昵称
            public string head; //馆主头像
            public string memberNum; //成员数量
            public string vitality;  //总活跃度
            public string monthVitality; //月活跃度
            public string createTime;  //创建时间
            public string bulletin; //公告
            public string contact; //联系方式
        }

        //网页传输的数据
        [Serializable]
        public class ParlorMessageData
        {
            public int status;  //1成功0参数错误  9系统错误            
            public int num;  //该县所有麻将馆的数量
            public ParlorMessage[] data;    //麻将馆的信息
        }


        /// <summary>
        /// 解析麻将馆的相关数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="status">5表示产生预置体</param>
        public void GetPointCountyParlor(string json, int status = 0)
        {
            CountyParlorCount = 0;
            ParlorMessageData parlor = new ParlorMessageData();
            parlor = JsonUtility.FromJson<ParlorMessageData>(json);

            //获取麻将馆信息失败
            if (parlor.status != 1)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("获取麻将馆信息失败");
                return;
            }

            //更新广场的标题
            UIMainView.Instance.ParlorShowPanel.UpdateMyParlorMessage_Title(null, 2);

            //处理显示该地区没有麻将馆，给出提示
            if (parlor.data.Length == 0)
            {
                UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.NoParlorMessage(1);
                return;
            }
            else
            {
                UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.NoParlorMessage(2);
            }

            if (parlor.num < 50)
            {
                CountyParlorCount = parlor.data.Length;
            }
            else
            {
                CountyParlorCount = parlor.num;
            }

            int isContainSelfParlor = 0;

            //没有馆的信息
            if (parlor.num > 0)
            {
                for (int i = 0; i < parlor.data.Length; i++)
                {
                    bool isContain_self = false;  //自己所在的馆
                    bool isContain_Apply = false; //自己申请的馆
                    NetMsg.ParlorInfoDef info = new NetMsg.ParlorInfoDef();
                    info.iParlorId = Convert.ToInt32(parlor.data[i].parlorId);
                    info.szParlorName = parlor.data[i].parlorName;
                    info.iBossId = Convert.ToInt32(parlor.data[i].bossId);
                    info.szBossNickname = parlor.data[i].nickname;
                    info.szBossHeadimgurl = parlor.data[i].head;
                    info.iMemberNum = Convert.ToInt32(parlor.data[i].memberNum) + 1;
                    info.iVitality = Convert.ToInt32(parlor.data[i].vitality);
                    info.iMonthVitality = Convert.ToInt32(parlor.data[i].monthVitality);
                    info.iCreateTime = Convert.ToInt32(parlor.data[i].createTime);
                    info.iCityId = iCityId[1];
                    info.iCountyId = iCountyId[1];
                    info.szBulletin = parlor.data[i].bulletin;
                    info.szContact = parlor.data[i].contact;

                    if (!UnNeedSpecialSort)
                    {
                        for (int k = 0; k < parlorInfoDef.Length; k++)
                        {
                            if (parlorInfoDef[k] != null && info.iParlorId == parlorInfoDef[k].iParlorId)
                            {
                                isContainSelfParlor += 1;
                                isContain_self = true;
                                break;
                            }
                        }

                        for (int l = 0; l < ApplyParlorId_All.Length; l++)
                        {
                            if (info.iParlorId == ApplyParlorId_All[l])
                            {
                                isContain_Apply = true;
                                break;
                            }
                        }
                    }


                    //如果玩家选择时间排序
                    if (iNowCheckType == 1)
                    {
                        if (isContain_self)
                        {
                            TimeOrderParlorMessage.Insert(0, info);
                        }
                        else
                        {
                            if (isContain_Apply)
                            {
                                TimeOrderParlorMessage.Insert(isContainSelfParlor, info);
                            }
                            else
                            {
                                TimeOrderParlorMessage.Add(info);
                            }
                        }

                    }
                    //如果玩家选择人数排序
                    else if (iNowCheckType == 2)
                    {
                        if (isContain_self)
                        {
                            MemberCountOrderParlorMessage.Insert(0, info);
                        }
                        else
                        {
                            if (isContain_Apply)
                            {
                                TimeOrderParlorMessage.Insert(isContainSelfParlor, info);
                            }
                            else
                            {
                                MemberCountOrderParlorMessage.Add(info);
                            }
                        }
                    }
                    //玩家选择活跃度排序
                    else if (iNowCheckType == 3)
                    {
                        if (isContain_self)
                        {
                            ActivityOrderParlorMessage.Insert(0, info);
                        }
                        else
                        {
                            if (isContain_Apply)
                            {
                                TimeOrderParlorMessage.Insert(isContainSelfParlor, info);
                            }
                            else
                            {
                                ActivityOrderParlorMessage.Add(info);
                            }
                        }
                    }
                }
            }

            if (UnNeedSpecialSort)
            {
                UnNeedSpecialSort = false;
            }


            SystemMgr.Instance.ParlorShowSystem.UpdateShow();

            //产生预置体
            if (status == 5 && pageId[iNowCheckType - 1] == 1)
            {
                UIMainView.Instance.ParlorShowPanel.ShowPointCountyParlor(1, iNowCheckType);
            }
            //保存json数据
            if (parlor.data.Length < 50)
            {
                isGetAllData_PointCountyParlor[iNowCheckType - 1] = true;  //表示已经获取所有的麻将馆的对应数据                
            }
            else
            {
                pageId[iNowCheckType - 1]++;
            }
        }

        /// <summary>
        /// 获取不同排序方式的麻将馆的信息
        /// </summary>
        /// <param name="type">1表示创建时间排序（默认排序方式） 2表示人数排序 3表示月活跃度排序</param>
        /// <param name="status">4默认 5产生预置体 6初始化预置体</param>
        public void FromWebGetParlorMessage(int type, int status)
        {
            RefreshCount++;
            UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.ParlorGod);

            if (type == 1)
            {
                TimeOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            }
            else if (type == 2)
            {
                MemberCountOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            }
            else if (type == 3)
            {
                ActivityOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
            }

            iNowCheckType = type;
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "countyParlor.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "countyParlor.x";
            }

            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("countyId", sapd.iCountyId.ToString());
            value.Add("page", pageId[type - 1].ToString());
            value.Add("orderType", type.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPointCountyParlor, " ", status);
        }
        #endregion

        #region 获取指定麻将馆所有的成员信息
        [Serializable]
        public class ParlorMemberMessage
        {
            public string userId; //成员编号
            public string nickname; //成员昵称
            public string head; //成员头像
            public string playCardAcc; //总出牌数
            public string playCardTimeAcc;  //总出牌时间
            public string monthVitality;  //月活跃度
            public string loginTime;  //最后登录时间
            public string gameNumAcc;  //总局数
            public string disconnectAcc;  //掉线数
            public string compliment; //点赞数
        }
        [Serializable]
        public class ParlorMemberMessageData
        {
            public int status;  //1成功0参数错误  9系统错误            
            public int num;  //该麻将馆所有成员的数量
            public ParlorMemberMessage[] data;    //成员的信息
        }


        /// <summary>
        /// 获取麻将馆成员的数据信息
        /// </summary>       
        public void FromWebGetParlorMember(int type = 1)
        {
            if (type == 1)
            {
                ParlorAllMemberMessage = new List<ParlorMemberMessage>();
            }
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "parlorUser.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "parlorUser.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("parlorId", iParlorId.ToString());
            //value.Add("parlorId", "10210");
            value.Add("page", iPageID_Member.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPointParlorMember, " ", 5);
        }

        /// <summary>
        /// 解析麻将馆成员的数据信息
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetPointParlorMember(string json, int status = 0)
        {
            ParlorMemberMessageData parlor = new ParlorMemberMessageData();
            parlor = JsonUtility.FromJson<ParlorMemberMessageData>(json);
            if (parlor.status != 1)
            {
                ParlorAllMemberMessage = new List<ParlorMemberMessage>();
                return;
            }

            if (parlor.num < 50)
            {
                MemberNum = parlor.data.Length;
            }
            else
            {
                MemberNum = parlor.num;
            }


            if (GameData.Instance.PlayerNodeDef.userDef.iMyParlorId > 0)
            {
                parlorInfoDef[0].iMemberNum = MemberNum;
            }
            else
            {
                for (int i = 0; i < parlorInfoDef.Length; i++)
                {
                    if (parlorInfoDef[i] != null && parlorInfoDef[i].iParlorId == iParlorId)
                    {
                        parlorInfoDef[i].iMemberNum = MemberNum;
                        break;
                    }
                }
            }

            UIMainView.Instance.ParlorShowPanel.UpdateMemberCount(MemberNum);

           // Debug.LogError("成员长度:" + parlor.data.Length);

            for (int i = 0; i < parlor.data.Length; i++)
            {
                if (iPageID_Member == 1 && GameData.Instance.PlayerNodeDef.iMyParlorId == 0 && Convert.ToInt32(parlor.data[i].userId) == GameData.Instance.PlayerNodeDef.iUserId)
                {
                    ParlorAllMemberMessage.Insert(1, parlor.data[i]);
                }
                else
                {
                    ParlorAllMemberMessage.Add(parlor.data[i]);
                }
            }

            UIMainView.Instance.ParlorShowPanel.ParlorTableGame.Member.transform.GetChild(1).
                GetComponent<UnityEngine.UI.ScrollRect>().enabled = true;

            //产生对应的成员的预置体信息
            if (iPageID_Member == 1)
            {
                UIMainView.Instance.ParlorShowPanel.SpwanParlorMemberMessage();
            }

            if (parlor.data.Length >= 50)
            {
                iPageID_Member++;
                isGetAllData_ParlorMember = false;
            }
            else
            {
                isGetAllData_ParlorMember = true;
            }
        }

        #endregion

        #region 通过名字或者馆的id精确对应馆的数据
        [Serializable]
        public class PointIdOrNameGetParlorMessage
        {
            public string parlorId; //馆的id
            public string parlorName;  //馆的名字
            public string countyId;  //对应的县城的id
            public string bossId;  //馆主的id
            public string nickname;  //馆主的昵称
            public string head;  //馆主的头像
            public string memberNum;  //馆的成员数量
            public string vitality;  //总活跃度
            public string monthVitality;  //月活跃度
            public string createTime;  //创建时间
            public string bulletin;  //公告
            public string contact; //联系方式
            public string parlorStatus;  //麻将馆状态  0表示正常  1表示被封状态
        }

        [Serializable]
        public class PointIdOrNameGetParlorMessageData
        {
            public int status; //1成功0参数错误  9系统错误  
            public PointIdOrNameGetParlorMessage[] data;
        }

        /// <summary>
        /// 获取搜索的麻将馆的相关数据
        /// </summary>
        /// <param name="status">5默认是快速加入 6表示产生该馆的预置体</param>
        public void FromWebGetSearchParlorMessage(int status, string serachContent)
        {
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "parlorSearch.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "parlorSearch.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("key", serachContent);
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPointIdOrNameGetParlorMessage, " ", status);
        }


        /// <summary>
        /// 解析麻将馆的相关数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="status">5默认是快速加入 6表示产生该馆的预置体</param>
        public void GetPointIdOrNameGetParlorMessage(string json, int status = 0)
        {
            IdOrNameGetParlorMessageList = new List<PointIdOrNameGetParlorMessage>();
            PointIdOrNameGetParlorMessageData parlor = new PointIdOrNameGetParlorMessageData();
            parlor = JsonUtility.FromJson<PointIdOrNameGetParlorMessageData>(json);

            //获取麻将馆信息失败
            if (parlor.status != 1 || parlor.data.Length == 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("您输入的麻将馆名称或馆号不存在!");
                return;
            }

            //保存搜索到的所有的麻将馆的信息
            for (int i = 0; i < parlor.data.Length; i++)
            {
                parlor.data[i].memberNum = (Convert.ToInt32(parlor.data[i].memberNum) + 1).ToString();
                IdOrNameGetParlorMessageList.Add(parlor.data[i]);
            }

            //显示所有结果提示
            if (parlor.data.Length > 0)
            {
                int parlorId = Convert.ToInt32(parlor.data[0].parlorId);

                if (status == 6)
                {
                    NetMsg.ParlorInfoDef InfoDef = new NetMsg.ParlorInfoDef();
                    InfoDef.iParlorId = Convert.ToInt32(parlor.data[0].parlorId);
                    InfoDef.szParlorName = parlor.data[0].parlorName;
                    InfoDef.iBossId = Convert.ToInt32(parlor.data[0].bossId);
                    InfoDef.szBossNickname = parlor.data[0].nickname;
                    InfoDef.szBossHeadimgurl = parlor.data[0].head;
                    InfoDef.iMemberNum = Convert.ToInt32(parlor.data[0].memberNum);
                    InfoDef.iVitality = Convert.ToInt32(parlor.data[0].vitality);
                    InfoDef.iMonthVitality = Convert.ToInt32(parlor.data[0].monthVitality);
                    InfoDef.iCreateTime = Convert.ToInt32(parlor.data[0].createTime);
                    InfoDef.iCountyId = Convert.ToInt32(parlor.data[0].countyId);
                    InfoDef.iStatus = Convert.ToInt32(parlor.data[0].parlorStatus);
                    InfoDef.szBulletin = parlor.data[0].bulletin;
                    InfoDef.szContact = parlor.data[0].contact;
                    //产生对应的面板信息
                    UIMainView.Instance.ParlorShowPanel.SpwanPointParlorPanel(InfoDef, 1);
                }
                else
                {
                    //表示馆被封
                    if (Convert.ToInt32(parlor.data[0].parlorStatus) == 1)
                    {
                        UIMgr.GetInstance().GetUIMessageView().Show("该馆目前在封馆状态，无法申请!");
                        return;
                    }

                    if (parlorId > 0)
                    {
                        UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.SearchResultDeal(parlorId);
                    }
                }
            }
        }


        #endregion

        #region 获取指定馆的三天内的牌局记录
        [Serializable]
        public class GetPointParlorGameRecord
        {
            public int status;  //状态
            public int num; //数量
            public GetPointParlorGameRecordData[] data; //所有数据
        }

        [Serializable]
        public class GetPointParlorGameRecordData
        {
            public string openRoomId; //开房序号
            public string roomNum; //房间号
            public string openTim; //开房时间            
            public string disType; //房间解散类型：1未开始游戏房主主动解散，2未开始游戏时间到了自动解散，3游戏中一局内玩家解散，4游戏中一局后玩家解散,5全部游戏结束解散,6未到预约时间房主解散,7GM强制解散,8封馆解散,9服务器解散-
            public string disTim; //解散时间-
            public string disUserId; //解散发起人
            public string card; //开房房费
            public string uid; //开房人的id
            public string point1; //座位1的最终分数--
            public string point2; //座位2的最终分数--
            public string point3; //座位3的最终分数--
            public string point4; //座位4的最终分数--
            public string nick1;  //座位1的用户昵称-
            public string nick2;  //座位2的用户昵称-
            public string nick3;  //座位3的用户昵称-
            public string nick4;  //座位4的用户昵称-
            public string head1;  //座位1的用户头像
            public string head2;  //座位2的用户头像
            public string head3;  //座位3的用户头像
            public string head4;  //座位4的用户头像
            public string uid1;  //座位1的用户ID
            public string uid2;  //座位2的用户ID
            public string uid3;  //座位3的用户ID
            public string uid4;  //座位4的用户ID
            public string comp1;  //玩家1的点赞数
            public string comp2;  //玩家2的点赞数
            public string comp3;  //玩家3的点赞数
            public string comp4;  //玩家4的点赞数
            public string gameNum1;  //玩家1的牌局数
            public string gameNum2;  //玩家2的牌局数
            public string gameNum3;  //玩家3的牌局数
            public string gameNum4;  //玩家4的牌局数            
            public string disconnect1;  //玩家1的掉线数
            public string disconnect2;  //玩家2的掉线数
            public string disconnect3;  //玩家3的掉线数
            public string disconnect4;  //玩家4的掉线数
            public string playCard1;  //玩家1的总出牌数
            public string playCard2;  //玩家2的总出牌数
            public string playCard3;  //玩家3的总出牌数
            public string playCard4;  //玩家4的总出牌数
            public string playTim1;  //玩家1的总出牌时间
            public string playTim2;  //玩家2的总出牌时间
            public string playTim3;  //玩家3的总出牌时间
            public string playTim4;  //玩家4的总出牌时间
        }

        /// <summary>
        /// 获取指定馆三天内的所有的牌局信息
        /// </summary>
        /// <param name="status">4默认 5产生预置体 6初始化预置体</param>
        public void FromWebPointParlorThreeDayGameNum(int status)
        {
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载牌局记录");
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "parlorOpenRoom1.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "parlorOpenRoom1.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("parlorId", GameData.Instance.ParlorShowPanelData.iParlorId.ToString());
            //value.Add("parlorId", "10483");
            value.Add("page", GameData.Instance.ParlorShowPanelData.ParlorGameRecordPageId.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPointParlorThreeDayGameNum, " ", status);
        }

        /// <summary>
        /// 解析麻将馆三天内的所有的游戏记录
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetPointParlorThreeDayGameNum(string json, int status)
        {
            GetPointParlorGameRecord parlor = new GetPointParlorGameRecord();
            parlor = JsonUtility.FromJson<GetPointParlorGameRecord>(json);
            //获取数据状态错误
            if (parlor.status != 1 || parlor.num == 0)
            {
                //显示没有游戏数据
                UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.SetActive(true);
                UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.transform.SetAsLastSibling();
                UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.transform.GetChild(1).gameObject.SetActive(true);
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                return;
            }
            else
            {
                UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (status == 5)
            {
                PointParlorGameRecordData = new List<GetPointParlorGameRecordData>();
            }

            if (parlor.num < 50)
            {
                ParlorGameTotalNum = parlor.data.Length;
            }
            else
            {
                ParlorGameTotalNum = parlor.num;
            }

            for (int i = 0; i < parlor.data.Length; i++)
            {
                PointParlorGameRecordData.Add(parlor.data[i]);
            }

            //打开滑动面板
            UIMainView.Instance.ParlorShowPanel.ParlorGameRecord.transform.GetChild(0).
                GetComponent<UnityEngine.UI.ScrollRect>().enabled = true;

            //产生预置体,只有页码为1的时候产生预置体
            if (ParlorGameRecordPageId == 1)
            {
                UIMainView.Instance.ParlorShowPanel.SpwanParlorGameRecord();
            }

            //保存数据，并且产生对应的预置体
           // Debug.Log ("parlor.num：" + parlor.num);

            if (parlor.data.Length >= 50)
            {
                ParlorGameRecordPageId += 1;
                isGetAllData_ParlorGameRecord = false;
            }
            else
            {
                isGetAllData_ParlorGameRecord = true;
            }
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
        }

        #endregion

        #region 获取指定馆的所有已经开始或者预约的桌的信息
        [Serializable]
        public class GetPointParlorTabelMessage
        {
            public string openRoomId; //开房序号
            public string roomNum; //房间号
            public string beginTim; //游戏开始时间
            public string uid; //开房人ID 
            public string uid1; //座位1的用户ID
            public string uid2; //座位2的用户ID
            public string uid3; //座位3的用户ID
            public string uid4; //座位4的用户ID
            public string ruid1; //座位1的预约用户ID
            public string ruid2; //座位2的预约用户ID
            public string ruid3; //座位3的预约用户ID
            public string ruid4; //座位4的预约用户ID
            public string param; //开房参数
            public string rTim; //预约时间
            public string playMethod; //玩法编号
        }

        [Serializable]
        public class GetPointParlorTabelMessageData
        {
            public int status;  //1成功0参数错误  9系统错误
            public int num; //表示已经创建房间，但是未开始游戏处于等待状态
            public GetPointParlorTabelMessage[] data; //信息
        }

        /// <summary>
        /// 获取指定馆的所有已经开始或者预约的桌的信息
        /// </summary>
        /// <param name="status">4默认 5产生预置体 6初始化预置体</param>
        public void FromWebPointParlorTabelMessage(int status)
        {
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "parlorOpenRoom0.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "parlorOpenRoom0.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("parlorId", GameData.Instance.ParlorShowPanelData.iParlorId.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, AnlyGetPointParlorTabelMessage, " ", status);
        }


        /// <summary>
        /// 解析麻将馆的相关数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="status">5表示产生预置体</param>
        public void AnlyGetPointParlorTabelMessage(string json, int status = 0)
        {
            PointParlorTabelMessage_Started = new List<GetPointParlorTabelMessage>();
            GetPointParlorTabelMessageData parlor = new GetPointParlorTabelMessageData();
            parlor = JsonUtility.FromJson<GetPointParlorTabelMessageData>(json);

            //获取麻将馆信息失败
            if (parlor.status != 1)
            {
                Debug.LogError("获取麻将馆桌的信息失败");
            }

            //保存该麻将馆所有未开始桌的信息
            iAllTabelNumUnStart = parlor.num + 1;

            Debug.Log("未开始游戏的房间的数量:" + parlor.num);

            //保存搜索到的所有的麻将馆已经开始游戏的桌的信息
            for (int i = 0; i < parlor.data.Length; i++)
            {
                PointParlorTabelMessage_Started.Add(parlor.data[i]);
            }

            //获取到桌的数量之后，请求桌的详细信息
            if (iPageId_TabelGame == 0)
            {                
                NetMsg.ClientGetParlorTableInfoReq msg = new NetMsg.ClientGetParlorTableInfoReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iParlorId = iParlorId;
                msg.iPageNum = iPageId_TabelGame;
                Network.NetworkMgr.Instance.LobbyServer.SendClientGetParlorTableInfoReq(msg);
            }
        }


        public class OpenRoomInfo
        {
            public int[] iaUserId = new int[4]; // 桌上用户
            public int[] iaBespeakUserId = new int[4]; //预约用户编号
            public int iRoomNum;//房间编号
            public int iParlorId; // 房间所属麻将馆编号
            public byte byOpenRoomStatus; // 桌的开房状态，0没使用，1已经预订，2等待开始游戏，3已开始游戏
            public int iDissolveByte; //解散类型
                                      // 1未开始游戏房主主动解散
                                      // 2未开始游戏时间到了自动解散
                                      // 3一局游戏内解散，没计费
                                      // 4一局游戏后解散，有计费
                                      // 5全部游戏结束解散
                                      // 6未到预约时间房主解散
                                      // 7后台解散
        }

        /// <summary>
        /// 玩家信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateTabelMessage(OpenRoomInfo info)
        {
            int DealStatus = 0;  //处理状态  1表示处理了未开始游戏的状态  2表示已开始游戏的状态
           // Debug.Log ("房间号：" + info.iRoomNum + ",解散类型:" + info.iDissolveByte + ",开桌状态:" + info.byOpenRoomStatus);
            //首先判断结束本本局是否结束
            if (info.iDissolveByte > 0)
            {
                //未开始游戏解散，删除等待状态的房间信息
                if (info.iDissolveByte == 1 || info.iDissolveByte == 2 || info.iDissolveByte == 6 || info.iDissolveByte == 7)
                {
                    for (int i = 0; i < PointParlorTabelMessage_UnStart.Count; i++)
                    {
                        if (info.iRoomNum == Convert.ToInt32(PointParlorTabelMessage_UnStart[i].iRoomNum))
                        {
                            DealStatus = 1;
                            PointParlorTabelMessage_UnStart.RemoveAt(i);
                            iAllTabelNumUnStart--;

                            ////如果是自己的预约房解散，取消显示预约时间信息
                            //for (int k = 0; k < 4; k++)
                            //{
                            //    Debug.LogError("info.iaBespeakUserId[k]：" + info.iaBespeakUserId[k]);
                            //    if (GameData.Instance.PlayerNodeDef.iUserId == info.iaBespeakUserId[k])
                            //    {
                            //        GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = false;
                            //        SystemMgr.Instance.ParlorShowSystem.UpdateShow();
                            //        break;
                            //    }
                            //}

                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < PointParlorTabelMessage_Started.Count; i++)
                    {
                        if (info.iRoomNum == Convert.ToInt32(PointParlorTabelMessage_Started[i].roomNum))
                        {
                            DealStatus = 2;
                            PointParlorTabelMessage_Started.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            else
            {
                //未开始状态或者未开始状态
                if (info.byOpenRoomStatus < 3)
                {
                    bool isNew = true;
                    for (int i = 0; i < PointParlorTabelMessage_UnStart.Count; i++)
                    {
                        if (info.iRoomNum == Convert.ToInt32(PointParlorTabelMessage_UnStart[i].iRoomNum))
                        {
                            PointParlorTabelMessage_UnStart[i].iaUserId = info.iaUserId;
                            PointParlorTabelMessage_UnStart[i].iaBespeakUserId = info.iaBespeakUserId;
                            //更新界面
                            UIMainView.Instance.ParlorShowPanel.UpdateParlorTabelGame_UnStart(PointParlorTabelMessage_UnStart[i]);
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        FromWebPointParlorTabelMessage(5);
                    }
                }
                //从未开始状态转化为已开始状态
                else
                {
                    for (int i = 0; i < PointParlorTabelMessage_UnStart.Count; i++)
                    {
                        if (info.iRoomNum == Convert.ToInt32(PointParlorTabelMessage_UnStart[i].iRoomNum))
                        {
                            GetPointParlorTabelMessage messgae = new GetPointParlorTabelMessage();
                            messgae.uid1 = info.iaUserId[0].ToString();
                            messgae.uid2 = info.iaUserId[1].ToString();
                            messgae.uid3 = info.iaUserId[2].ToString();
                            messgae.uid4 = info.iaUserId[3].ToString();
                            messgae.ruid1 = info.iaBespeakUserId[0].ToString();
                            messgae.ruid1 = info.iaBespeakUserId[1].ToString();
                            messgae.ruid1 = info.iaBespeakUserId[2].ToString();
                            messgae.ruid1 = info.iaBespeakUserId[3].ToString();
                            messgae.openRoomId = "0-" + PointParlorTabelMessage_UnStart[i].iSeverId;
                            messgae.roomNum = info.iRoomNum.ToString();
                            messgae.beginTim = PointParlorTabelMessage_UnStart[i].iOpenRoomTime.ToString();

                            System.Text.StringBuilder str = new System.Text.StringBuilder();
                            for (int k = 0; k < PointParlorTabelMessage_UnStart[i].param.Length ; k++)
                            {
                                str.Append(PointParlorTabelMessage_UnStart[i].param[k]);
                                if (k != PointParlorTabelMessage_UnStart[i].param.Length-1)
                                {
                                    str.Append(",");
                                }
                            }
                            messgae.param = str.ToString();

                            messgae.rTim = PointParlorTabelMessage_UnStart[i].iBespeakTime.ToString();
                            messgae.playMethod = PointParlorTabelMessage_UnStart[i].iPlayMethod.ToString();
                            PointParlorTabelMessage_Started.Insert(0, messgae);
                            PointParlorTabelMessage_UnStart.RemoveAt(i);
                            iAllTabelNumUnStart--;
                            DealStatus = 2;
                            UIMainView.Instance.ParlorShowPanel.ShowStartedRedPoint(1);
                            break;
                        }
                    }
                }
            }

            //更新状态
            if (DealStatus > 0)
            {
                UIMainView.Instance.ParlorShowPanel.SpwanPorlorTabelMessage(2, 0);
            }
            //else if (DealStatus == 2)
            //{
            //    UIMainView.Instance.ParlorShowPanel.SpwanPorlorTabelMessage(3, 0);
            //}

            //更新玩家数据
            UIMainView.Instance.ParlorShowPanel.UpdateMemberTabelCount();
        }
        #endregion

        #region 获取老板业绩积分日志信息
        [Serializable]
        public class ParlorBossScoreLog
        {
            public string userId;  //开房人用户编号
            public string score;  //业绩积分变化量-
            public string banlance;  //业绩积分变化后余额-
            public string roomNum;  //房间号-
            public string coin;  //开房所用金币。
            public string coin3;  // 开房所用赠币。
            public string assetType;  //兑换的资源类型，1现金，2话费，3流量，4储值卡,5代金券,6赠币。
            public string assetNum;  //兑换的资源数量
            public string tim;  //发生的时间
        }

        [Serializable]
        public class ParlorBossScoreLogData
        {
            public int status; //1成功  9系统错误 0没有数据
            public int num; //总数量
            public ParlorBossScoreLog[] data; //日志信息
        }

        /// <summary>
        /// 从网页请求老板业绩积分日志信息
        /// </summary>
        public void FormWebGetParlorBossScoreLogMessage()
        {
            //if (isGetBossScoreFinish[Status_BossScore - 1])
            //{
            //    return;
            //}
            string url = " ";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "bossScoreLog.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL + "bossScoreLog.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            value.Add("bossId", GameData.Instance.PlayerNodeDef.iUserId.ToString());
            //value.Add("bossId", "10001459");
            value.Add("page", PageId_BossScore[Status_BossScore - 1].ToString());
            value.Add("period", (Status_BossScore - 1).ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetParlorBossScoreLogMessage, "12", 5);
        }

        /// <summary>
        /// 解析老板业绩积分日志数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetParlorBossScoreLogMessage(string json, int status)
        {
            ParlorBossScoreLogData parlor = new ParlorBossScoreLogData();
            parlor = JsonUtility.FromJson<ParlorBossScoreLogData>(json);

            lsParlorBossScoreLog_Last = new List<ParlorBossScoreLog>();
            lsParlorBossScoreLog_Now = new List<ParlorBossScoreLog>();

            UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.BossScore);
            UIMainView.Instance.ParlorShowPanel.ParlorBossScore.transform.SetAsLastSibling();
            UIMainView.Instance.ParlorShowPanel.BossScore.text = (GameData.Instance.PlayerNodeDef.userDef.fParlorScore / 100f).ToString("0.00");

            //if (Status_BossScore == 1)
            //{
            //    lsParlorBossScoreLog_Now = new List<ParlorBossScoreLog>();
            //}
            //else
            //{
            //    lsParlorBossScoreLog_Last = new List<ParlorBossScoreLog>();
            //}

            //获取数据状态信息
            if (parlor.status != 1 || parlor.num == 0)
            {
                UIMainView.Instance.ParlorShowPanel.ParlorBossScore.transform.GetChild(3).gameObject.SetActive(false);
                UIMainView.Instance.ParlorShowPanel.ParlorBossScore.transform.GetChild(4).gameObject.SetActive(true);
                UIMainView.Instance.ParlorShowPanel.SpwanBossScorePrefab(Status_BossScore);
                return;
            }
            else
            {
                UIMainView.Instance.ParlorShowPanel.ParlorBossScore.transform.GetChild(3).gameObject.SetActive(true);
                UIMainView.Instance.ParlorShowPanel.ParlorBossScore.transform.GetChild(4).gameObject.SetActive(false);
            }

            int iNowDay = DateTime.Now.Day;  //当前日期
            int Month = DateTime.Now.Month;  //当月时间
            int ExtraCount = 0;  //额外添加数量

            for (int i = 0; i < parlor.data.Length; i++)
            {
                int Day = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToInt32(parlor.data[i].tim), 0).Day;

                if (i == 0)
                {
                    ParlorBossScoreLog log = new ParlorBossScoreLog();
                    log.userId = "0";
                    if (Day == iNowDay)
                    {
                        log.tim = "今天";
                    }
                    else
                    {
                        if (iNowDay - Day == 1)
                        {
                            log.tim = "昨天";
                        }
                        else
                        {
                            log.tim = Month + "月" + Day + "日";
                        }
                    }
                    ExtraCount++;
                    if (Status_BossScore == 1)
                    {
                        lsParlorBossScoreLog_Now.Add(log);
                    }
                    else
                    {
                        lsParlorBossScoreLog_Last.Add(log);
                    }
                }
                else
                {
                    int lastDay = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToInt32(parlor.data[i - 1].tim), 0).Day;
                    ParlorBossScoreLog log = new ParlorBossScoreLog();
                    log.userId = "0";
                    if (lastDay - Day > 0)
                    {
                        if (iNowDay - Day == 0)
                        {
                            log.tim = "今天";
                        }
                        else if (iNowDay - Day == 1)
                        {
                            log.tim = "昨天";
                        }
                        else
                        {
                            log.tim = Month + "月" + Day + "日";
                        }
                        ExtraCount++;
                        if (Status_BossScore == 1)
                        {
                            lsParlorBossScoreLog_Now.Add(log);
                        }
                        else
                        {
                            lsParlorBossScoreLog_Last.Add(log);
                        }
                    }
                }

                //添加详细数据                
                if (Status_BossScore == 1)
                {
                    lsParlorBossScoreLog_Now.Add(parlor.data[i]);
                }
                else
                {
                    lsParlorBossScoreLog_Last.Add(parlor.data[i]);
                }
            }


            //获取总数量
            ParlorBossScoreCount[Status_BossScore - 1] = parlor.num + ExtraCount;


            Debug.Log ("Status_BossScore:" + Status_BossScore + "业绩信息:" + PageId_BossScore[Status_BossScore - 1]);

            //产生对应的预置体
            if (PageId_BossScore[Status_BossScore - 1] == 1)
            {
                Debug.Log ("产生业绩预置体信息");
                UIMainView.Instance.ParlorShowPanel.SpwanBossScorePrefab(Status_BossScore);
            }

            //添加页码
            if (parlor.data.Length >= 50)
            {
                PageId_BossScore[Status_BossScore - 1]++;
            }
            else
            {
                isGetBossScoreFinish[Status_BossScore - 1] = true;
            }

        }
        #endregion

        #region 获取老板业绩积分日志统计信息
        [Serializable]
        public class ParlorBossScoreStat
        {
            public int status; //1成功  9系统错误 0没有数据
            public float bossCoin; //老板消耗的金币
            public float bossCoin3; //老板消耗的赠币-
            public double score1; //老板消耗币得到的积分-
            public float memCoin; //成员消耗的金币-
            public float memCoin3; // 成员消耗的赠币。
            public double score2; // 成员消耗币得的积分。
            public double consume; // 老板提现消耗业绩积分。
        }

        [Serializable]
        public class ParlorBossScoreStatData
        {
            public int status;   //1成功  9系统错误 0没有数据
            public ParlorBossScoreStat[] data;
        }

        /// <summary>
        /// 获取两个月的数据统计信息
        /// </summary>
        public void FromWebGetParlorBossScoreStatData()
        {
            string url = " ";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "bossScoreStat.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL + "bossScoreStat.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            int bossid = 0;

            for (int i = 0; i < 4; i++)
            {
                if (parlorInfoDef[i] != null && parlorInfoDef[i].iParlorId == iParlorId)
                {
                    bossid = parlorInfoDef[i].iBossId;
                    continue;
                }
            }
            value.Add("bossId", bossid.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetParlorBossScoreStatData, "12", 5);
        }


        /// <summary>
        /// 解析老板积分统计数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetParlorBossScoreStatData(string json, int status)
        {
            ParlorBossScoreStatData parlor = new ParlorBossScoreStatData();
            parlor = JsonUtility.FromJson<ParlorBossScoreStatData>(json);
            //如果状态不对，两个月的值直接赋为0
            if (parlor.status != 1)
            {
                ParlorBossScoreStat info = new ParlorBossScoreStat();
                ParlorBossScoreStatMessage[0] = info;
                ParlorBossScoreStatMessage[1] = info;
            }
            else
            {
                for (int i = 0; i < parlor.data.Length; i++)
                {
                    ParlorBossScoreStatMessage[i] = parlor.data[i];
                }
                //ParlorBossScoreStat info = new ParlorBossScoreStat();
                //info.bossCoin = 100;
                //info.bossCoin3 = 200;
                //info.score1 = 600;
                //info.score2 = 200;
                //info.memCoin = 400;
                //info.memCoin3 = 200;
                //info.consume = 200;
                //ParlorBossScoreStatMessage[0] = info;
                //ParlorBossScoreStat info_1 = new ParlorBossScoreStat();
                //info_1.bossCoin = 300;
                //info_1.bossCoin3 = 620;
                //info_1.score1 = 600;
                //info_1.score2 = 300;
                //info_1.memCoin = 600;
                //info_1.memCoin3 = 300;
                //info_1.consume = 600;
                //ParlorBossScoreStatMessage[1] = info_1;
            }

            //更新数据
            UIMainView.Instance.ParlorShowPanel.ScoreStatisticsPanel.UpdateShow(1);
        }

        #endregion

        #region 获取馆内红包信息      

        [Serializable]
        public class ParlorRedBag_
        {
            public int parlorId;
            public int rpId;
            public int beginTim;
            public int endTim;
            public string context;
            public int interval;
            public int state;  //0正常  1结束
        }

        [Serializable]
        public class ParlorRedBagData_
        {
            public int status;
            public ParlorRedBag_[] data;
        }

        /// <summary>
        /// 获取麻将馆的红包信息
        /// </summary>
        public void FromWebGetParlorRedBagData()
        {
            string url = " ";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "userRp17.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL + "userRp17.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetParlorRedBagData, "12", 5);
        }

        public void GetParlorRedBagData(string json, int status)
        {
            ParlorRedBagData_ parlor = new ParlorRedBagData_();
            parlor = JsonUtility.FromJson<ParlorRedBagData_>(json);
            //如果状态不对，两个月的值直接赋为0
            if (parlor.status != 1)
            {
                ParlorRedBag_ info = new ParlorRedBag_();
                parlorRedBagInfo[0] = info;
                parlorRedBagInfo[1] = info;
                parlorRedBagInfo[2] = info;
                parlorRedBagInfo[3] = info;
                MahjongCommonMethod.Instance.parlorRedBagInfo = parlorRedBagInfo;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i < parlor.data.Length)
                    {
                        parlorRedBagInfo[i] = parlor.data[i];
                    }
                    else
                    {
                        ParlorRedBag_ info = new ParlorRedBag_();
                        parlorRedBagInfo[i] = info;
                    }
                }
                MahjongCommonMethod.Instance.parlorRedBagInfo = parlorRedBagInfo;
            }
            //MahjongCommonMethod.Instance.GetNowTimer(iParlorId, SystemMgr.Instance.ParlorShowSystem.UpdateParlorRedBagMessage);

            //SystemMgr.Instance.ParlorShowSystem.UpdateParlorRedBagMessage(iParlorId);
        }
        #endregion

        #region 获取某个麻将馆红包的详细信息
        public List<ParlorRedBagMessage> SmallRedBagMessage = new List<ParlorRedBagMessage>();
        public int SmallRedBagCount; //小红包的总数

        [Serializable]
        public class ParlorRedBagMessage
        {
            public string userId; //玩家id
            public string head; //中奖人头像
            public string nickname; //中奖人昵称
            public string assetType; //兑换的资源类型，1现金，2话费，3流量，4储值卡,5代金券,6赠币。
            public string assetNum; //兑换的资源数量
            public int rank; //排名信息：0无，1最小 2最大
        }

        [Serializable]
        public class ParlorRedBagMessageData
        {
            public int status; //1成功  9系统错误 0没有数据;
            public int srpNum; //红包总数
            public ParlorRedBagMessage[] data;
        }

        public void FromWebGetRedBag17()
        {
            string url = " ";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "rp17.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL + "rp17.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            int rpid = 0;
            for (int i = 0; i < parlorRedBagInfo.Length; i++)
            {
                if (iParlorId == parlorRedBagInfo[i].parlorId)
                {
                    rpid = parlorRedBagInfo[i].rpId;
                    break;
                }
            }
            value.Add("rpId", rpid.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetRedBag17Data, "12", 5);
        }

        /// <summary>
        /// 获取指定麻将馆红包的领取信息
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetRedBag17Data(string json, int status)
        {
            ParlorRedBagMessageData parlor = new ParlorRedBagMessageData();
            parlor = JsonUtility.FromJson<ParlorRedBagMessageData>(json);
            UIMainView.Instance.ParlorShowPanel.ParlorSmallReaBagMessage.SetActive(true);
            SmallRedBagMessage = new List<ParlorRedBagMessage>();
            if (parlor.status != 1 || parlor.data.Length == 0)
            {
                UIMainView.Instance.ParlorShowPanel.ParlorSmallReaBagMessage.transform.GetChild(2).gameObject.SetActive(false);
                UIMainView.Instance.ParlorShowPanel.ParlorSmallReaBagMessage.transform.GetChild(3).gameObject.SetActive(true);
                return;
            }

            SmallRedBagCount = parlor.srpNum;

            for (int i = 0; i < parlor.data.Length; i++)
            {
                if (Convert.ToInt32(parlor.data[i].userId) == GameData.Instance.PlayerNodeDef.iUserId)
                {
                    MahjongCommonMethod.Instance.isGetReaBag = true;
                }
                else
                {
                    MahjongCommonMethod.Instance.isGetReaBag = false;
                }
                SmallRedBagMessage.Add(parlor.data[i]);
            }

            //产生预置体
            UIMainView.Instance.ParlorShowPanel.SpwanSmallRedBag();
        }
        #endregion

        #region 获取某个玩家的已经申请的麻将馆的id
        public int[] ApplyParlorId_All = new int[4];
        public List<NetMsg.ParlorInfoDef> ApplyParlorMessage = new List<NetMsg.ParlorInfoDef>();

        [Serializable]
        public class ParlorMessage_app
        {
            public string parlorId;  //麻将馆编号
            public string parlorName; //麻将馆名称
            public string countyId; //县的id
            public string bossId;  //馆主id
            public string nickname; //馆主昵称
            public string head; //馆主头像
            public string memberNum; //成员数量
            public string vitality;  //总活跃度
            public string monthVitality; //月活跃度
            public string createTime;  //创建时间
            public string bulletin; //公告
            public string contact; //联系方式            
        }


        [Serializable]
        public class ApplyParlorId
        {
            public int status; // 1成功  9系统错误 0无数据        
            public ParlorMessage_app[] data;  //麻将馆的信息
            //public int[] data;
        }

        int refresh = 0;  //我的麻将馆的信息请求类型  1表示正常 2表示刷心请求
        public void FromWebGetApplyParlorId(int status, int type)
        {
            refresh = type;
            string url = " ";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "userApplyParlorId.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL + "userApplyParlorId.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetApplyParlorIdData, "12", status);
        }

        /// <summary>
        /// 获取玩家的申请馆的id编号
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        void GetApplyParlorIdData(string json, int status)
        {
            //Debug.LogError("玩家的申请馆的id编号:" + json);
            ApplyParlorId parlor = new ApplyParlorId();
            parlor = JsonUtility.FromJson<ApplyParlorId>(json);
            ApplyParlorId_All = new int[4];
            ApplyParlorMessage = new List<NetMsg.ParlorInfoDef>();
            //获取申请馆id失败，或者没有申请馆
            if (parlor.status != 1 || parlor.data.Length == 0)
            {
                if (status == 6)
                {
                    SendRefreshParlorMessage(refresh);
                }
                return;
            }

            //保存已申请的馆号和馆的信息
            for (int i = 0; i < parlor.data.Length; i++)
            {
                //ApplyParlorId_All[i] = parlor.data[i];
                ApplyParlorId_All[i] = Convert.ToInt32(parlor.data[i].parlorId);
                NetMsg.ParlorInfoDef info = new NetMsg.ParlorInfoDef();
                info.iParlorId = Convert.ToInt32(parlor.data[i].parlorId);
                info.szParlorName = parlor.data[i].parlorName;
                info.iBossId = Convert.ToInt32(parlor.data[i].bossId);
                info.szBossNickname = parlor.data[i].nickname;
                info.szBossHeadimgurl = parlor.data[i].head;
                info.iMemberNum = Convert.ToInt32(parlor.data[i].memberNum) + 1;
                info.iVitality = Convert.ToInt32(parlor.data[i].vitality);
                info.iMonthVitality = Convert.ToInt32(parlor.data[i].monthVitality);
                info.iCreateTime = Convert.ToInt32(parlor.data[i].createTime);
                info.iCityId = iCityId[1];
                info.iCountyId = Convert.ToInt32(parlor.data[i].countyId);
                info.szBulletin = parlor.data[i].bulletin;
                info.szContact = parlor.data[i].contact;
                info.iStatus = 5;
                ApplyParlorMessage.Add(info);
            }

            if (status == 6)
            {
                SendRefreshParlorMessage(refresh);
            }
        }
        #endregion

        /// <summary>
        /// 这个管是不是馆主创建 是馆主的话返回馆主ID
        /// </summary>
        /// <returns></returns>
        public int GetNowMahjongPavilionID()
        {
            for (int i = 0; i < parlorInfoDef.Length; i++)
            {
                if (iParlorId == Convert.ToInt32(parlorInfoDef[i].iParlorId))
                {
                    Debug.Log ("馆的ID" + Convert.ToInt32(parlorInfoDef[i].iParlorId) + "馆主的ID" + parlorInfoDef[i].iBossId);
                    return Convert.ToInt32(parlorInfoDef[i].iBossId);
                }
            }
            return 0;
        }

        /// <summary>
        /// 刷新玩家拥有的四个麻将馆信息
        /// </summary>
        /// <param name="type">1表示正常请求 2表示刷新请求</param>
        public void SendRefreshParlorMessage(int type)
        {
            GetParlorMessageType = type;
            //请求我的麻将馆的信息
            NetMsg.ClientGetParlorInfoReq msg_parlor = new NetMsg.ClientGetParlorInfoReq();
            msg_parlor.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            Network.NetworkMgr.Instance.LobbyServer.SendGetParlorInfoReq(msg_parlor);
        }


        /// <summary>
        /// 处理麻将馆分享出去拉起app进入麻将馆的处理
        /// </summary>
        /// <param name="parlorId"></param>
        public void HandleLqAppToParlorId(int parlorId)
        {
            SDKManager.Instance.ParlorId = 0;
            int status = 0;  //1表示自己已加入改麻将馆   2表示已经在申请列表中 3表示自己已经满四个馆
            int count_parlor = 0;  //自己已拥有或已加入的麻将馆数量

            if (GameData.Instance.PlayerNodeDef.userDef.iMyParlorId > 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("亲，您已是麻将馆老板，无法加入其他麻将馆", Ok_0);
                return;
            }

            //已加入的麻将馆
            for (int i = 0; i < parlorInfoDef.Length; i++)
            {
                if (parlorInfoDef[i].iParlorId > 0)
                {
                    count_parlor++;
                }
                if (parlorInfoDef[i] != null && parlorInfoDef[i].iParlorId == parlorId)
                {
                    status = 1;
                    Ok_0();
                    return;
                }
            }

            //已申请的麻将馆
            for (int i = 0; i < ApplyParlorMessage.Count; i++)
            {
                if (ApplyParlorMessage[i].iParlorId > 0)
                {
                    count_parlor++;
                }

                if (ApplyParlorMessage[i].iParlorId == parlorId)
                {
                    status = 2;
                    PlayerPrefs.SetInt(SaveLaseParlorId, parlorId);
                    Ok_0();
                    return;
                }
            }

            //如果自己已经加入四个馆
            if (count_parlor >= 4)
            {
                status = 3;
                UIMgr.GetInstance().GetUIMessageView().Show("您不是该馆会员，并且加入的麻将馆已达上限", Ok_0);
                return;
            }

            //提交馆的申请信息
            if (count_parlor < 4 && status == 0)
            {
                //打开指定的馆的界面，通过搜索获取指定麻将馆的信息
                isShowPanel = true;
                SystemMgr.Instance.ParlorShowSystem.UpdateShow();
                FromWebGetSearchParlorMessage(6, parlorId.ToString());
            }
        }

        //进入麻将馆
        void Ok_0()
        {
            FromWebGetApplyParlorId(6, 1);
        }
    }

}

