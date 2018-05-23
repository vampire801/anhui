using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Network.Message;
using System.Text;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System;
using System.Collections.Generic;
using MahjongLobby_AH.Data;
using anhui;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewReservationSeatPanel : MonoBehaviour
    {
        public Text m_TextRoomName;//房间名称
        public Text m_TextRoomID;//房间名称
        public Text m_TextRoomRule;//房间规则

        public GameObject[] m_gFourSeat;//四个玩家的头像界面

        public MainViweImportantMessagePanel m_gImportantMessagePanel;//弹出界面
        public GameObject m_guserinfo;//玩家个人信息界面

        public GameObject BTN;//创建房间的玩家还是老板

        //这个桌的信息
        [HideInInspector]
        public NetMsg.OpenRoomInfo m_openRoomInfo;

        //这个桌上四个玩家的信息
        [HideInInspector]
        public NetMsg.TableUserInfoDef[] m_lUserInfo = new NetMsg.TableUserInfoDef[4];

        MahjongCommonMethod mcm = MahjongCommonMethod.Instance;

        //保留小时时间的
        private string TitleStartHours;
        private bool isYuYueTime = false;
        //保存一张默认的图片
        public RawImage NormalImage;

        //游戏开始文字
        public Text GameStartText;

    /// <summary>
    /// 打开这个界面
    /// </summary>
    /// <param name="roomID">房间的ID号</param>
    public void OnOpen()
    {
        if (m_openRoomInfo == null) return;

            //Debug.Log("游戏玩法:" + m_openRoomInfo.iPlayingMethod);
            //Debug.Log("游戏服务器编号:" + m_openRoomInfo.iServerId);
            //Debug.Log("桌编号:" + m_openRoomInfo.iTableNum);
            //Debug.Log("桌的开房状态，0没使用，1已经预订，2等待开始游戏，3已开始游戏:" + m_openRoomInfo.cOpenRoomStatus);
            //Debug.Log("颜色标志:" + m_openRoomInfo.byColorFlag);
            //Debug.Log("开房时间:" + m_openRoomInfo.iOpenRoomTime + "," + MahjongCommonMethod.Instance.UnixTimeStampToDateTime(m_openRoomInfo.iOpenRoomTime, 0).ToString("HH:mm"));
            //Debug.Log("要求赞的数量:" + m_openRoomInfo.iCompliment);
            //Debug.Log("掉线率要求:" + m_openRoomInfo.iDisconnectRate);
            //Debug.Log("出牌速度要求:" + m_openRoomInfo.iDiscardTime);
            //Debug.Log("开房的参数:" + m_openRoomInfo.caOpenRoomParam[15] + "," + m_openRoomInfo.caOpenRoomParam[14] + "," + m_openRoomInfo.caOpenRoomParam[13]);
            //Debug.Log("房间号:" + m_openRoomInfo.iRoomNum);
            //Debug.Log("桌上用户:" + m_openRoomInfo.iaUserId[0] + "," + m_openRoomInfo.iaUserId[1] + "," + m_openRoomInfo.iaUserId[2] + "," + m_openRoomInfo.iaUserId[3]);
            //Debug.Log("桌上预约用户ID:" + m_openRoomInfo.iBespeakUserId[0] + "," + m_openRoomInfo.iBespeakUserId[1] + "," + m_openRoomInfo.iBespeakUserId[2] + "," + m_openRoomInfo.iBespeakUserId[3]);
            //Debug.Log("桌上四个玩家：" + m_lUserInfo[0] + "," + m_lUserInfo[1] + "," + m_lUserInfo[2] + "," + m_lUserInfo[3]);

            if (m_openRoomInfo.caOpenRoomParam[4] > 0)
            {
                isYuYueTime = true;
            }

            DateTime necttime = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(m_openRoomInfo.iOpenRoomTime, 0).AddMinutes(m_openRoomInfo.caOpenRoomParam[4]));
            int subtime = (int)(MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(necttime) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now));
            //Debug.Log("时间到了：" + subtime+","+ isYuYueTime);
            if (subtime <= 0)
            {
                isYuYueTime = false;
            }
            //Debug.Log("isYuYueTime：" + isYuYueTime);

            gameObject.SetActive(true);
            OnTitleTimeAndName();
            // Debug.LogError(UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iPlayingMethod);
            OnInitRoomRule(UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.iPlayingMethod);
            OnPlayerHeadInfo();

            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)//表示是老板
            {
                BTN.transform.GetChild(0).gameObject.SetActive(false);
                BTN.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                BTN.transform.GetChild(0).gameObject.SetActive(true);
                BTN.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (m_openRoomInfo.cOpenRoomStatus == 3)
            {
                BTN.SetActive(false);
                GameStartText.gameObject.SetActive(true);
            }
            else
            {
                BTN.SetActive(true);
                GameStartText.gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            OnTitleTimeAndName();
        }

        /// <summary>
        /// 显示界面的标题名
        /// </summary>
        void OnTitleTimeAndName()
        {
            if (m_openRoomInfo.caOpenRoomParam[4] > 0 && isYuYueTime)
            {
                TitleStartHours = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(m_openRoomInfo.iOpenRoomTime, 0).AddMinutes(m_openRoomInfo.caOpenRoomParam[4])).ToString("HH:mm");//开始时间
                m_TextRoomName.text = mcm._dicMethodConfig[m_openRoomInfo.iPlayingMethod].METHOD_NAME;
                m_TextRoomID.text = "房间号_" + m_openRoomInfo.iRoomNum.ToString("D6") + "<color=#CC3C3BFF>(" + TitleStartHours + "开始" + ")</color>";
            }
            else
            {
                m_TextRoomName.text = mcm._dicMethodConfig[m_openRoomInfo.iPlayingMethod].METHOD_NAME;
                m_TextRoomID.text = "房间号_" + m_openRoomInfo.iRoomNum.ToString("D6");
            }

        }

        /// <summary>
        /// 房间的规则
        /// </summary>
        void OnInitRoomRule(int iPlayingMethord)
        {
            //房间规则
            StringBuilder RoomRule = new StringBuilder();

            //Debug.Log("===============================0");

            UIMainView.Instance.CreatRoomMessagePanel.UpdateShowMethod(m_openRoomInfo.iPlayingMethod, false);
            // int isAAPay = mcm.ReadColumnValue(m_openRoomInfo.caOpenRoomParam, 2, 39);
            //玩法局或圈的数量
            //玩法数量对应支付的房卡数量

            //if (m_openRoomInfo.PavilionID <= 0)//表示不是老板
            //{
            //    if (isAAPay > 0)//表示四家支付
            //    {
            //        RoomRule.Append("四家支付  ");
            //    }
            //    else
            //    {
            //        RoomRule.Append("房主支付  ");
            //    }
            //}
            #region oldCode
            // int isAAPay = mcm.ReadColumnValue(m_openRoomInfo.caOpenRoomParam, 2, 39);

            //玩法类型
            //string fufei = "";
            //uint priceParam = mcm.ReadInt32toInt4(m_openRoomInfo.caOpenRoomParam[0], 16);
            //if (m_openRoomInfo.iPlayingMethod == 17 && priceParam >= 3)
            //{
            //    if (priceParam == 3)
            //    {
            //        fufei = "4局";
            //        if (isAAPay > 0)//表示四家支付
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[0] / 4 + "金币";
            //        }
            //        else
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[0] + "金币";
            //        }
            //    }
            //    else if (priceParam == 4)
            //    {
            //        fufei = "8局";
            //        if (isAAPay > 0)//表示四家支付
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[1] / 4 + "金币";
            //        }
            //        else
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[1] + "金币";
            //        }
            //    }
            //    else if (priceParam == 5)
            //    {
            //        fufei = "12局";
            //        if (isAAPay > 0)//表示四家支付
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[2] / 4 + "金币";
            //        }
            //        else
            //        {
            //            fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[2] + "金币";
            //        }
            //    }
            //}
            //else
            //{
            //    fufei = UIMainView.Instance.CreatRoomMessagePanel.Method_sum[priceParam].ToString();

            //    if (UIMainView.Instance.CreatRoomMessagePanel.Method_type[0] == 1)
            //    {
            //        fufei += ("圈");
            //    }
            //    else if (UIMainView.Instance.CreatRoomMessagePanel.Method_type[0] == 2)
            //    {
            //        fufei += ("局");
            //    }
            //    else if (UIMainView.Instance.CreatRoomMessagePanel.Method_type[0] == 3)
            //    {
            //        fufei += ("分");
            //    }

            //    if (isAAPay > 0)//表示四家支付
            //    {
            //        fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[priceParam] / 4 + "金币";
            //    }
            //    else
            //    {
            //        fufei += UIMainView.Instance.CreatRoomMessagePanel.Method_pay[priceParam] + "金币";
            //    }
            //}

            //RoomRule.Append(fufei);
            //RoomRule.Append("  ");

            //List<CommonConfig.CardType> PlayTypeRule = new List<CommonConfig.CardType>();
            //if (!mcm._dicMethodCardType.ContainsKey(m_openRoomInfo.iPlayingMethod)) return;
            //PlayTypeRule = mcm._dicMethodCardType[m_openRoomInfo.iPlayingMethod];

            //十三不靠：0（不可胡），1（可胡） 1004
            //明杠收3家：0（被杠者支付），1（三家都支付） 1005
            //四杠荒庄：0（不可以），1（可以） 1006
            //一炮多响：0（一炮单响），1（一炮多响） 2003
            //前抬后和：0（不开启），1（前抬） 2005
            //前抬后和：0（不开启），1（后和） 2006
            //换庄模式: 0 轮庄 1 连庄 2020
            //带字牌: 0 (不带字牌) 1 (带字牌) 2018
            //点炮三分：0 (不开启) 1 (开启) 2019
            //自摸接炮：0（必须自摸），1（一炮单响），2（一炮多响） 2003  只用1和2
            //自摸翻倍：0（不翻倍），1（翻倍）1007
            //支付：0（一人支付），1（多人支付）2009 2010
            //接炮：0（一炮单响），1（一炮多响） 2003
            //坐庄：0不开启，1开启 2011
            //能跑能下 :0不开启，1开启 2012
            //放几出几: 0不开启,1开启(开启放几出几，必须设置【胡 1 2 3 】为必须自摸)  2013
            //翻几倍选择 2 3 4 5 2014 2015 2016 2017
            //十三幺：0（不可胡），1（可胡） 1001
            //七对：0（不可胡），1（可胡） 1002
            #endregion


            mcm.ShowParamOfOpenRoom(ref RoomRule, UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo.caOpenRoomParam, 1, iPlayingMethord);

            m_TextRoomRule.text = RoomRule.ToString();
        }

        string OnFindIndex(List<CommonConfig.CardType> PlayTypeRule, int index)
        {
            string str = "";
            for (int i = 0; i < PlayTypeRule.Count; i++)
            {
                if (PlayTypeRule[i].ID == index)
                {
                    str = PlayTypeRule[i].card_type + "  ";
                    break;
                }
            }
            return str;
        }

        /// <summary>
        /// 显示四个玩家的头像信息
        /// </summary>
        void OnPlayerHeadInfo()
        {
            //先清理一下
            for (int index = 0; index < 4; index++)
            {
                m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = NormalImage.texture;
                m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);//头像的副显示  在房间内不显示 在房间外显示
                m_gFourSeat[index].transform.GetChild(1).gameObject.SetActive(false);//占座按钮
                m_gFourSeat[index].transform.GetChild(2).gameObject.SetActive(false);//占座按钮不可用状态
                m_gFourSeat[index].transform.GetChild(3).gameObject.SetActive(false);//这个座位被占座中
            }

            if (m_openRoomInfo.caOpenRoomParam[4] > 0 && isYuYueTime)//表示预约房
            {
                int[] SeatInfo = new int[4] { 0, 0, 0, 0 };
                for (int index = 0; index < 4; index++)
                {
                    if ((m_openRoomInfo.iaUserId[index] + m_openRoomInfo.iBespeakUserId[index]) != 0)
                    {
                        int seatInfo = 0;
                        Debug.Log("------占座信息" + m_openRoomInfo.iaUserId[index] + "," + m_openRoomInfo.iBespeakUserId[index]);
                        //0什么都没有 1表示没有占座 2表示在座位上但是没有占座 3表示占座了  4表示占座了并且在房间内 5表示占座了不在房间内 
                        if (m_openRoomInfo.iaUserId[index] == 0 && m_openRoomInfo.iBespeakUserId[index] == 0)
                        {
                            Debug.Log(index + "没有任何人占座");
                            seatInfo = 1;
                        }
                        else if (m_openRoomInfo.iaUserId[index] >= 0 && m_openRoomInfo.iBespeakUserId[index] == 0)
                        {
                            Debug.Log(index + "桌上有玩家");
                            seatInfo = 2;
                        }
                        else if (m_openRoomInfo.iaUserId[index] == 0 && m_openRoomInfo.iBespeakUserId[index] >= 0)
                        {
                            Debug.Log(index + "桌上有预约用户");
                            seatInfo = 5;
                        }
                        else if (m_openRoomInfo.iaUserId[index] >= 0 && m_openRoomInfo.iBespeakUserId[index] >= 0)
                        {
                            Debug.Log(index + "桌上有预约用户并且在房间内");
                            seatInfo = 4;
                        }
                        SeatInfo[index] = 1;
                        Debug.Log("头像:" + m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        if (m_lUserInfo[index] != null)
                        {
                            MahjongCommonMethod.Instance.GetPlayerAvatar(m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), m_lUserInfo[index].szHeadimgurl);//加载玩家的头像信息
                        }
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(seatInfo == 5 ? true : false);//头像的副显示  在房间内不显示 在房间外显示
                        m_gFourSeat[index].transform.GetChild(1).gameObject.SetActive(seatInfo == 1 ? true : false);//占座按钮
                        m_gFourSeat[index].transform.GetChild(2).gameObject.SetActive(seatInfo == 2 ? true : false);//占座按钮不可用状态
                        m_gFourSeat[index].transform.GetChild(3).gameObject.SetActive(seatInfo == 4 ? true : false);//这个座位被占座中
                    }
                    else
                    {
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = NormalImage.texture;
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);//头像的副显示  在房间内不显示 在房间外显示
                        m_gFourSeat[index].transform.GetChild(1).gameObject.SetActive(true);//占座按钮
                        m_gFourSeat[index].transform.GetChild(2).gameObject.SetActive(false);//占座按钮不可用状态
                        m_gFourSeat[index].transform.GetChild(3).gameObject.SetActive(false);//这个座位被占座中
                    }
                }

                if ((SeatInfo[0] + SeatInfo[1] + SeatInfo[2] + SeatInfo[3]) == 4)
                {
                    BTN.transform.GetChild(0).gameObject.SetActive(false);
                    BTN.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)//表示是老板
                    {
                        BTN.transform.GetChild(0).gameObject.SetActive(false);
                        BTN.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        BTN.transform.GetChild(0).gameObject.SetActive(true);
                        BTN.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int index = 0; index < 4; index++)
                {
                    //Debug.Log("玩家头像：" + m_lUserInfo[index]);
                    if (m_lUserInfo[index] != null)
                    {
                        MahjongCommonMethod.Instance.GetPlayerAvatar(m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), m_lUserInfo[index].szHeadimgurl);//加载玩家的头像信息
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);//头像的副显示  在房间内不显示 在房间外显示
                        m_gFourSeat[index].transform.GetChild(1).gameObject.SetActive(false);//占座按钮
                        m_gFourSeat[index].transform.GetChild(2).gameObject.SetActive(false);//占座按钮不可用状态
                        m_gFourSeat[index].transform.GetChild(3).gameObject.SetActive(false);//这个座位被占座中
                    }
                    else
                    {
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = NormalImage.texture;
                        m_gFourSeat[index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);//头像的副显示  在房间内不显示 在房间外显示
                        m_gFourSeat[index].transform.GetChild(1).gameObject.SetActive(false);//占座按钮
                        m_gFourSeat[index].transform.GetChild(2).gameObject.SetActive(false);//占座按钮不可用状态
                        m_gFourSeat[index].transform.GetChild(3).gameObject.SetActive(false);//这个座位被占座中
                    }
                }

                if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)//表示是老板
                {
                    BTN.transform.GetChild(0).gameObject.SetActive(false);
                    BTN.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    BTN.transform.GetChild(0).gameObject.SetActive(true);
                    BTN.transform.GetChild(1).gameObject.SetActive(false);
                }

            }
        }

        /// <summary>
        /// 关闭这个界面
        /// </summary>
        public void OnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            gameObject.SetActive(false);
        }

        public void BtnPlayerHeadInfo(int Num)
        {
            if (m_lUserInfo[Num] == null) return;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            GameObject playerpanel = Instantiate(m_guserinfo) as GameObject;
            playerpanel.transform.SetParent(gameObject.transform);
            playerpanel.transform.localPosition = Vector3.zero;
            playerpanel.transform.localRotation = Quaternion.identity;
            playerpanel.transform.localScale = new Vector3(1, 1, 1);

            UserInfo userinfo_ = playerpanel.GetComponent<UserInfo>();
            //缺少填充内容
            userinfo_.GetUserMessageForLobby(m_lUserInfo[Num], 1);
        }

        /// <summary>
        /// 点击头像
        /// </summary>
        /// <param name="seatNum"></param>
        public void BtnPlayerOccupySeat(int seatNum)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            OnOpenImportantPanel(seatNum);
        }

        /// <summary>
        /// 打开MainViweImportantMessagePanel面板
        /// </summary>
        void OnOpenImportantPanel(int seat)
        {
            //Debug.Log ("===========================1");
            m_gImportantMessagePanel.OnOpen(m_openRoomInfo.iOpenRoomTime, m_openRoomInfo.caOpenRoomParam[4], () =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                //发送占座消息
                NetMsg.ClientUserBespeakReqDef msg = new NetMsg.ClientUserBespeakReqDef();
                msg.byFromType = 1;//消息来源，1来自大厅，2来自游戏客户端
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId; // 用户编号
                msg.iSeatNum = (byte)(seat + 1);//座位号
                msg.iRoomNum = m_openRoomInfo.iRoomNum;//房间号
                msg.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
                NetworkMgr.Instance.LobbyServer.SendClientUserBespeakReq(msg);

                //大厅桌面显示房间开始倒计时
                //UIMainView.Instance.LobbyPanel.OnShowYuYueRoom(m_openRoomInfo.caOpenRoomParam[m_openRoomInfo.caOpenRoomParam.Length - 2]*60);
            });
        }

        /// <summary>
        /// 点击占做
        /// </summary>
        /// <param name="seat"></param>
        public void DoZhanZhuo(int seat)
        {
            if (string.IsNullOrEmpty(GameData.Instance.PlayerNodeDef.szHeadimgurl) == false)
                MahjongCommonMethod.Instance.GetPlayerAvatar(m_gFourSeat[seat].transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), GameData.Instance.PlayerNodeDef.szHeadimgurl);//加载玩家的头像信息
            m_gFourSeat[seat].transform.GetChild(1).gameObject.SetActive(false);
            m_gFourSeat[seat].transform.GetChild(2).gameObject.SetActive(false);
            m_gFourSeat[seat].transform.GetChild(3).gameObject.SetActive(true);
        }

        /// <summary>
        /// 老板点击邀请
        /// </summary>
        public void BtnInvitation()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            string url = SDKManager.WXInviteUrl + m_openRoomInfo.iRoomNum.ToString("d6");
            string city;
            try
            {
                city = GameData.Instance.SelectAreaPanelData.listCityMessage[GameData.Instance.PlayerNodeDef.iCityId].cityName;
            }
            catch (Exception)
            {
                city = "双喜";
            }
            StringBuilder title = new StringBuilder();
            if (SDKManager.Instance.CheckStatus == 1)
            {
                title.AppendFormat("{0}麻将->{1}<-房间号：{2} 点击进入房间", city, "推倒胡", m_openRoomInfo.iRoomNum.ToString("d6"));
            }
            else
            {
                title.AppendFormat("{0}麻将->{1}<-房间号：{2} 点击进入房间", city, mcm._dicMethodConfig[m_openRoomInfo.iPlayingMethod].METHOD_NAME, m_openRoomInfo.iRoomNum.ToString("d6"));
            }
            #region 配置

            StringBuilder discription = new StringBuilder();
            Debug.LogWarning("----" + m_openRoomInfo.PavilionID);
            if (m_openRoomInfo.PavilionID > 0)// && GameData.Instance.ParlorShowPanelData.GetNowMahjongPavilionID() == GameData.Instance.PlayerNodeDef.iUserId)
            {
                discription.Append("【老板开房】");
            }

            discription.Append("玩法：");
            CreatRoomMessagePanelData cd = GameData.Instance.CreatRoomMessagePanelData;

            mcm.ShowParamOfOpenRoom(ref discription, cd.roomMessage_, 0, MahjongCommonMethod.Instance.iPlayingMethod);
            //MahjongCommonMethod.Instance.GetWeiteInfoForMethodID(discription, m_openRoomInfo.iPlayingMethod, m_openRoomInfo.caOpenRoomParam);

            #endregion 配置
            Debug.Log("---" + discription);
            Debug.LogWarning("分享预约" + m_openRoomInfo.caOpenRoomParam[4]);
            Debug.LogWarning("分享托管" + m_openRoomInfo.caOpenRoomParam[5]);

            if (m_openRoomInfo.caOpenRoomParam[4] > 0)
            {
                discription.Append("预约 " + m_openRoomInfo.caOpenRoomParam[4] + " 分钟 ");
            }

            if (m_openRoomInfo.caOpenRoomParam[5] > 0)
            {
                discription.Append("托管 " + m_openRoomInfo.caOpenRoomParam[5] + " 分钟");
            }
            SDKManager.Instance.HandleShareWX(url, title.ToString(), discription.ToString(), 0, 0, 0, "");
        }

        /// <summary>
        /// 点击进入房间
        /// </summary>
        public void BtnGoRoom()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            //如果已经有牌局不可以再次进入其他房间
            //if (GameData.Instance.LobbyMainPanelData.isJoinedRoom)
            //{
            //    UIMgr.GetInstance().GetUIMessageView().Show("您已有牌局，请解散房间后加入！");
            //    return;
            //}

            //获取服务器编号
            NetMsg.ClientRoomNumServerTableReq msg = new NetMsg.ClientRoomNumServerTableReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iRoomNum = m_openRoomInfo.iRoomNum;
            msg.iParlorId = MahjongCommonMethod.Instance.iParlorId;
            NetworkMgr.Instance.LobbyServer.SendRoomNumSeverTableReq(msg);
            MahjongCommonMethod.Instance.iFromParlorInGame = msg.iParlorId;
        }
    }

}
