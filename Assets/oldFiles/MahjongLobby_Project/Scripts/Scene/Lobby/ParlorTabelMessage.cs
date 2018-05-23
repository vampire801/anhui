using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorTabelMessage : MonoBehaviour
    {
        public GameObject[] ready;  //准备的手势
        public GameObject[] holdSeat;  //占座的手势
        public GameObject DownTimerImage; //倒计时背景图
        public Text Minutes; //分
        public Text Seconds; //秒
        public Text MethodName; //玩法名称
        public Text RoomNum; //房间号
        public Image BossImage; //老板头像
        public GameObject CreatRoom;  //创建房间

        public float DTimer; //倒计时时间
        public int type; //1表示创建房间的预置体 2表示未开始游戏的预置体  3表示已经开始游戏的预置体        

        //保存未开始游戏桌的信息
        public NetMsg.TableInfoDef TabelInfo_Un = new NetMsg.TableInfoDef();

        //保存已经开始游戏桌的信息        
        public ParlorShowPanelData.GetPointParlorTabelMessage TabelInfo_Ed = new ParlorShowPanelData.GetPointParlorTabelMessage();

        //void OnDisable()
        //{
        //    if (OneSecond != null)
        //    {
        //        Debug.LogError("关闭协成");
        //        StopCoroutine(OneSecond);
        //    }
        //}

        //public void StopOneSecond()
        //{
        //    if (OneSecond != null)
        //    {
        //        Debug.LogError("关闭协成");
        //        StopCoroutine(OneSecond);
        //    }
        //}


        /// <summary>
        /// 更新预置体显示界面
        /// </summary>
        public void UpdateShow(int type_, NetMsg.TableInfoDef Info, ParlorShowPanelData.GetPointParlorTabelMessage info_1)
        {
            gameObject.SetActive(true);
            type = type_;
            if (type == 2)
            {
                TabelInfo_Un = Info;
            }
            else if (type == 3)
            {
                TabelInfo_Ed = info_1;
                RoomNum.text = "房间号_" + Convert.ToInt32(info_1.roomNum).ToString("D6");
                return;
            }
            //显示老板的标志
            int bossId = 0;
            string bossImage = " ";
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
            {
                if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                {
                    bossId = pspd.parlorInfoDef[i].iBossId;
                    bossImage = pspd.parlorInfoDef[i].szBossHeadimgurl;
                    break;
                }
            }
            if (BossImage == null)
            {
                BossImage = transform.GetChild(0).GetChild(3).GetComponent<Image>();
            }

            if (bossId == Convert.ToInt32(Info.iOpenRoomId))
            {
                BossImage.gameObject.SetActive(true);
                MahjongCommonMethod.Instance.GetPlayerAvatar(BossImage.transform.GetChild(0).GetComponent<RawImage>(), bossImage);
            }
            else
            {
                BossImage.gameObject.SetActive(false);
            }


            if (type == 3)
            {
                RoomNum.text = "房号_" + Info.iRoomNum.ToString("D6");
                return;
            }


            if (Info == null || (Info.iRoomNum == 0 && Info.iSeverId == 0))
            {
                type = 1;
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                CreatRoom.SetActive(true);
                return;
            }
            else
            {
                type = 2;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(2).gameObject.SetActive(true);
                CreatRoom.SetActive(false);
            }

            int[] userid = new int[4];
            int[] ruserid = new int[4];

            userid = Info.iaUserId;
            ruserid = Info.iaBespeakUserId;

            //显示玩家的状态
            for (int i = 0; i < 4; i++)
            {
                if (userid[i] > 0)
                {
                    //有人但是没占座
                    if (ruserid[i] == 0)
                    {
                        ready[i].SetActive(true);
                        holdSeat[i].SetActive(false);
                    }
                    //有人且占座
                    else
                    {
                        ready[i].SetActive(false);
                        holdSeat[i].SetActive(true);
                    }
                }
                else
                {
                    //无人但是占座
                    if (ruserid[i] > 0)
                    {
                        ready[i].SetActive(false);
                        holdSeat[i].SetActive(true);
                    }
                    else
                    {
                        ready[i].SetActive(false);
                        holdSeat[i].SetActive(false);
                    }
                }
            }

            RoomNum.text = "房间号_" + Info.iRoomNum.ToString("D6");
            if (MethodName != null)
            {
                //显示玩法名称
                MethodName.text = MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(Info.iPlayMethod)].METHOD_NAME;
            }
            else
            {
                transform.GetChild(1).GetComponent<Text>().text = MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(Info.iPlayMethod)].METHOD_NAME;
            }

            if (TabelInfo_Un.iBespeakTime > 0)
            {
                DownTimerImage.SetActive(true);
                //如果预约时间大于0，显示倒计时
                MahjongCommonMethod.Instance.GetNowTimer(0, IeDelayGetTimer);
            }
            else
            {
                DownTimerImage.SetActive(false);
            }
        }


        void IeDelayGetTimer(int id, int timer)
        {
            //显示预约放的时间
            int yyTime = 0;  //游戏开始时间
            yyTime = TabelInfo_Un.iOpenRoomTime + TabelInfo_Un.iBespeakTime * 60;  //游戏开始时间
            if (timer < yyTime)
            {
                DownTimerImage.SetActive(true);
                //显示预约时间
                DateTime timer_ = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(yyTime, 0);
                Minutes.text = timer_.ToString("HH");
                Seconds.text = timer_.ToString("mm");
            }
            else
            {
                DownTimerImage.SetActive(false);
            }
        }

        //IEnumerator OneSecond; //协成

        //void UpdateDownTimer(float timer)
        //{
        //    OneSecond = OneSecondDelay(timer);
        //    //Debug.LogError("开启协成");
        //    StartCoroutine(OneSecond);
        //}

        //IEnumerator OneSecondDelay(float timer)
        //{
        //    yield return new WaitForSeconds(1f);
        //    DTimer--;
        //    if (DTimer <= 0)
        //    {
        //        DTimer = 0;
        //        Minutes.text = "";
        //        //清空这个房间数据
        //        ClearPointTabelGame();
        //        yield break;
        //    }
        //    string tim = "0";
        //    TimeSpan timspan = new TimeSpan(0, 0, (int)DTimer);

        //    tim = timspan.Hours.ToString("00") + ":" + timspan.Minutes.ToString("00") + ":" + timspan.Seconds.ToString("00");

        //    Minutes.text = tim;
        //    UpdateDownTimer(DTimer);
        //}


        void ClearPointTabelGame()
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //老板创房
            if (TabelInfo_Un.iOpenRoomId != pspd.iParlorId)
            {
                TabelInfo_Un.iBespeakTime = 0;
                TabelInfo_Un.iaBespeakUserId = new int[4];
            }

            for (int i = 0; i < pspd.PointParlorTabelMessage_UnStart.Count; i++)
            {
                if (pspd.PointParlorTabelMessage_UnStart[i].iRoomNum == TabelInfo_Un.iRoomNum)
                {
                    if (TabelInfo_Un.iOpenRoomId != pspd.iParlorId)
                    {
                        pspd.PointParlorTabelMessage_UnStart[i].iBespeakTime = 0;
                        pspd.PointParlorTabelMessage_UnStart[i].iaBespeakUserId = new int[4];
                    }
                    else
                    {
                        pspd.PointParlorTabelMessage_UnStart.RemoveAt(i);
                        pspd.iAllTabelNumUnStart--;
                        UIMainView.Instance.ParlorShowPanel.SpwanPorlorTabelMessage(2);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        public void BtnTabel()
        {
            GameData.Instance.ParlorShowPanelData.ComeInParlorType = type;
            //点击创建房间
            if (type == 1)
            {
                GameData.Instance.GamePlayingMethodPanelData.CountyId = GameData.Instance.ParlorShowPanelData.iCountyId[2];
                GameData.Instance.GamePlayingMethodPanelData.Status = 1; //表示从麻将馆创建房间                

                if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
                {
                    Messenger_anhui<int>.Broadcast(MainViewLobbyPanel.MESSAGE_CREATROOM, 2);
                }
                else
                {
                    Messenger_anhui<int>.Broadcast(MainViewLobbyPanel.MESSAGE_CREATROOM, 3);
                }
                MahjongCommonMethod.Instance.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
            }

            //点击未开始游戏的房间
            if (type == 2)
            {
                NetMsg.OpenRoomInfo openRoomInfo = new NetMsg.OpenRoomInfo();
                openRoomInfo.iPlayingMethod = TabelInfo_Un.iPlayMethod;
                openRoomInfo.PavilionID = GameData.Instance.ParlorShowPanelData.GetNowMahjongPavilionID();
                openRoomInfo.iServerId = TabelInfo_Un.iSeverId;
                openRoomInfo.iTableNum = 0;
                if (TabelInfo_Un.iBespeakTime == 0)
                {
                    openRoomInfo.cOpenRoomStatus = 0;
                }
                else if (TabelInfo_Un.iBespeakTime != 0)
                {
                    if (TabelInfo_Un.iBespeakTime > MahjongCommonMethod.Instance.GetCreatetime())
                        openRoomInfo.cOpenRoomStatus = 3;
                    else
                        openRoomInfo.cOpenRoomStatus = 2;
                }
                openRoomInfo.byColorFlag = 0;
                openRoomInfo.iOpenRoomTime = TabelInfo_Un.iOpenRoomTime;
                openRoomInfo.iCompliment = 0;
                openRoomInfo.iDisconnectRate = 0;
                openRoomInfo.iDiscardTime = 0;
                openRoomInfo.iRoomNum = TabelInfo_Un.iRoomNum;
                openRoomInfo.iaUserId = TabelInfo_Un.iaUserId;
                //转成数组
                uint[] caOpenRoomParam = new uint[TabelInfo_Un.param.Length]; // 开房的参数
                for (int i = 0; i < TabelInfo_Un.param.Length; i++)
                {
                    caOpenRoomParam[i] = TabelInfo_Un.param[i];
                }
                openRoomInfo.caOpenRoomParam = caOpenRoomParam;
                openRoomInfo.iBespeakUserId = TabelInfo_Un.iaBespeakUserId;
                UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo = openRoomInfo;
                UIMainView.Instance.ParlorShowPanel.BtnSendClickTable();
            }
            //点击已经开始开始游戏的房间
            if (type == 3)
            {
                NetMsg.OpenRoomInfo openRoomInfo = new NetMsg.OpenRoomInfo();
                openRoomInfo.iPlayingMethod = Convert.ToInt32(TabelInfo_Ed.playMethod);
                openRoomInfo.PavilionID = GameData.Instance.ParlorShowPanelData.GetNowMahjongPavilionID();
                openRoomInfo.iServerId = Convert.ToInt32(TabelInfo_Ed.openRoomId.Split('-')[0]);
                openRoomInfo.iTableNum = 0;
                openRoomInfo.cOpenRoomStatus = 3;
                openRoomInfo.byColorFlag = 0;
                openRoomInfo.iOpenRoomTime = Convert.ToInt32(TabelInfo_Ed.beginTim);
                openRoomInfo.iCompliment = 0;
                openRoomInfo.iDisconnectRate = 0;
                openRoomInfo.iDiscardTime = 0;
                openRoomInfo.iRoomNum = Convert.ToInt32(TabelInfo_Ed.roomNum);
                openRoomInfo.iaUserId = new int[] { Convert.ToInt32(TabelInfo_Ed.uid1), Convert.ToInt32(TabelInfo_Ed.uid2),
                    Convert.ToInt32(TabelInfo_Ed.uid3), Convert.ToInt32(TabelInfo_Ed.uid4)};
                //转成数组
                for (int i = 0; i < openRoomInfo.caOpenRoomParam.Length ; i++)
                {
                    openRoomInfo.caOpenRoomParam[i] = Convert.ToUInt32(TabelInfo_Ed.param.Split(',')[i]);
                }
                openRoomInfo.caOpenRoomParam[4] = 0;
                openRoomInfo.iBespeakUserId = new int[] { Convert.ToInt32(TabelInfo_Ed.ruid1), Convert.ToInt32(TabelInfo_Ed.ruid2),
                Convert.ToInt32(TabelInfo_Ed.ruid3), Convert.ToInt32(TabelInfo_Ed.ruid4)};
                UIMainView.Instance.ReservationSeatPanel.m_openRoomInfo = openRoomInfo;
                UIMainView.Instance.ParlorShowPanel.BtnSendClickTable();
            }
        }

    }

}
