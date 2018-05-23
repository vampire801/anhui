using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using System.Collections.Generic;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using System;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class HistroyGradeRoom : MonoBehaviour
    {
        public const string url_suffix = "roomGameList.x";  //url后缀
        public const string json_data = "RoundMessageData";  //json的开头的变量名

        public Text openRoomTimer;  //开放时间
        public Text roomId; //房间id
        //public RawImage[] headImage;  //玩家头像
        public Text MethodName;  //玩法名字
        public Text[] nickName;  //玩家昵称
        public Text[] Score_Z;  //玩家正分显示             
        public Image[] compliment;  //玩家的赞的显示
        public Sprite[] complimentSprite;  //赞的显示图片

        //public Texture[] tex = new Texture[5];  //玩家的头像显示图片

        public int index = -1; //该条记录的下标      
        public int RoomIndex = -1;  //该条记录在list中的下标
        //存储玩家的本次开放记录
        public HistroyGradePanelData.HistroyGradeMessage_Self GradeRoom = new HistroyGradePanelData.HistroyGradeMessage_Self();

        //存储玩家本次开放记录对应的每局的记录
        public List<Grade_RoundData> Grade_RoundData_ = new List<Grade_RoundData>();
        public GameObject _btnShare;
        InfinityGridLayoutGroup infinityGridLayoutGroup;
        int scoreMax = 0;  //四个玩家的最大分


        public void UpdateShow()
        {
            scoreMax = 0;
            int iseatNum = -1;  //玩家座位号

            //更新开房时间   改为结束时间                     
            if (Convert.ToDouble(GradeRoom.disTime) == 0)
            {
                openRoomTimer.text = GradeRoom.openTim.ToString("yyyy_MM_dd HH:mm");
            }
            else
            {
                openRoomTimer.text = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(GradeRoom.disTime), 0).ToString("yyyy_MM_dd HH:mm");
            }



            //显示房间id
            roomId.text = GradeRoom.roomNum;

            //显示玩法
            MethodName.text = "(" + MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(GradeRoom.playing_method)].METHOD_NAME + ")";



            //显示四个玩家的头像
            for (int i = 0; i < 4; i++)
            {
                if (Convert.ToInt32(GradeRoom.point[i]) > scoreMax)
                {
                    scoreMax = Convert.ToInt32(GradeRoom.point[i]);
                }
                //MahjongCommonMethod.Instance.GetPlayerAvatar(headImage[i], GradeRoom.headUrl[i], 2);                
                nickName[i].text = GradeRoom.nickName[i];
                //根据分数的正负显示玩家的信息情况 
                Score_Z[i].gameObject.SetActive(true);
                if (GradeRoom.point[i] > 0)
                {
                    Score_Z[i].text = "+" + GradeRoom.point[i].ToString();
                }
                else
                {
                    Score_Z[i].text = GradeRoom.point[i].ToString();
                }

                //加载玩家头像                            
                //tex[i+1] = headImage[i].texture;                
            }

            //对最大分改变显示颜色
            for (int i = 0; i < 4; i++)
            {
                if (Convert.ToInt32(GradeRoom.point[i]) == scoreMax)
                {
                    nickName[i].GetComponent<Text>().color = new Color(0.8f, 0.34f, 0, 1);
                }
                else
                {
                    nickName[i].GetComponent<Text>().color = new Color(0.23f, 0.5f, 0.65f, 1);
                }
            }




            //根据玩家的赞的信息，决定玩家是否显示赞
            for (int i = 0; i < 4; i++)
            {
                //if (10009296 == Convert.ToInt32(GradeRoom.userid[i]))
                if (GameData.Instance.PlayerNodeDef.iUserId == Convert.ToInt32(GradeRoom.userid[i]))
                {
                    iseatNum = i;
                }
            }

            int Compliment = Convert.ToInt32(GradeRoom.playerComplient[iseatNum]) - 1;

            if (Compliment >= 0)
            {
                compliment[Compliment].gameObject.SetActive(true);
                compliment[Compliment].sprite = complimentSprite[1];
                for (int i = 0; i < 4; i++)
                {
                    if (i != Compliment)
                    {
                        compliment[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    //不显示自己的点赞按钮
                    if (i != iseatNum)
                    {
                        compliment[i].gameObject.SetActive(true);
                        compliment[i].sprite = complimentSprite[0];
                    }
                    else
                    {
                        compliment[i].gameObject.SetActive(false);
                    }
                }
            }

        }

        /// <summary>
        /// 本条开房记录的点击
        /// </summary>
        public void BtnGradeRoom()
        {
            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
            string url = LobbyContants.MAJONG_PORT_URL + url_suffix;
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
#if UNITY_IOS
                url = LobbyContants.MAJONG_PORT_URL_T + url_suffix;
#endif
            }
            //要传的值
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("openRoomId", GradeRoom.openRoomId);

            int portIndex = GradeRoom.openRoomId.IndexOf('-');
            string strPort = GradeRoom.openRoomId.Substring(0, portIndex);
            MahjongCommonMethod.Instance.SeverPort = (ushort)Convert.ToInt32(strPort);

            //获取该条记录对应的每局的信息
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetGrade_Round, json_data, 0);
            //打开具体的信息显示
            hgpd.isPanelShow = true;
            hgpd.isShowGrade_Round = true;
            hgpd.isShowGrade_Room = false;
            hgpd.BtnCloseStatus = 1;

            //更新显示该局的房间信息
            UIMainView.Instance.HistroyGradePanel.RoomId.text = GradeRoom.roomNum;
            UIMainView.Instance.HistroyGradePanel.Timer.text = GradeRoom.openTim.ToString("yyyy_MM_dd HH:mm");
            UIMainView.Instance.HistroyGradePanel.MethodName.text = "(" + MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(GradeRoom.playing_method)].METHOD_NAME + ")";
            for (int i = 0; i < 4; i++)
            {
                if (GradeRoom.nickName[i].Length > 3)
                {
                    UIMainView.Instance.HistroyGradePanel.NickName[i].text = GradeRoom.nickName[i].Substring(0, 3) + "..";
                }
                else
                {
                    UIMainView.Instance.HistroyGradePanel.NickName[i].text = GradeRoom.nickName[i];
                }

                if (GradeRoom.point[i] == scoreMax)
                {
                    UIMainView.Instance.HistroyGradePanel.NickName[i].color = new Color(0.8f, 0.34f, 0, 1);
                }
                else
                {
                    UIMainView.Instance.HistroyGradePanel.NickName[i].color = new Color(0.23f, 0.5f, 0.65f, 1);
                }
            }

            SystemMgr.Instance.HistroyGradeSystem.UpdateShow();
        }



        public void SpwanHistroyGrade_Round()
        {
           // Debug.LogError("开始产生对应的战绩预置体");

            UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
            UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").GetComponent<ContentSizeFitter>().enabled = true;
            int count = Grade_RoundData_.Count;

            if (count <= 0)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("本条记录没有战绩");
                return;
            }

            if (count > 5)
            {
                HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/HistroyGrade/HistroyGrade_Round"));
                    go.transform.SetParent(UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "HistroyGrade_Round_" + i;
                    UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").
                        GetComponent<InfinityGridLayoutGroup>().children.Add(go.GetComponent<RectTransform>());
                   // Debug.Log ("go.name：" + go.name);
                }

                //初始化数据列表;
                infinityGridLayoutGroup = UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(Grade_RoundData_.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/HistroyGrade/HistroyGrade_Round"));
                    go.transform.SetParent(UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "HistroyGrade_Round_" + i;
                    HistroyGradeRound round = go.GetComponent<HistroyGradeRound>();
                    round.RoundData = Grade_RoundData_[i];
                    round.UpdateShow(i + 1);
                }

                //初始化数据列表;
                infinityGridLayoutGroup = UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(Grade_RoundData_.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
            }
        }


        void UpdateChildrenCallback(int index_, Transform trans)
        {
            HistroyGradeRound round = trans.GetComponent<HistroyGradeRound>();
            round.RoundData = Grade_RoundData_[index_];
            round.UpdateShow(index_ + 1);
        }


        //对本条记录进行点赞
        public void BtnCompliment(int seatNum)
        {
            int selfSeatNum = -1;  //自己的座位号  0----3

            //获取自己的座位号
            for (int i = 0; i < 4; i++)
            {
                if (GameData.Instance.PlayerNodeDef.iUserId == Convert.ToInt32(GradeRoom.userid[i]))
                {
                    selfSeatNum = i;
                }
            }

            NetMsg.ClientComplimentReq msg = new NetMsg.ClientComplimentReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.szOpenRoomId = GradeRoom.openRoomId;
            msg.byComplimentSeat = (byte)seatNum;
            NetworkMgr.Instance.LobbyServer.SendComplimentReq(msg);
            GradeRoom.playerComplient[seatNum - 1] = 1.ToString();

            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;

            //修改该条房间记录的赞的标志
            for (int i = 0; i < 4; i++)
            {
                if (i == selfSeatNum)
                {
                    Debug.LogError("seatNum:" + seatNum);
                    hgpd.GradeMessage[index].playerComplient[i] = seatNum.ToString();
                }
                else
                {
                    Debug.LogError("======================================");
                    hgpd.GradeMessage[index].playerComplient[i] = 0.ToString();
                }
            }

            //处理当前的界面，更新界面
            UpdateComplient(seatNum - 1);
        }


        void UpdateComplient(int index)
        {
            Debug.LogError("index:" + index);
            for (int i = 0; i < 4; i++)
            {
                if (i == index)
                {
                    compliment[i].gameObject.SetActive(true);
                    compliment[i].sprite = complimentSprite[1];
                }
                else
                {
                    compliment[i].gameObject.SetActive(false);
                }
            }
        }

        #region 获取玩家该次开放对应的每局的信息
        //一局分数数据
        public class Grade_Round
        {
            public string gameNum;   //该局的游戏局号
            public string playing_method;
            public string dealer;  //庄家座位号

            public string beginTim;  // 开始时间-
            public string endTim;  // 结束时间-
            public string selfDrawnSeat;  //玩家自摸的座位号
            public string shootSeat;   //玩家的点炮座位号           

            public string point1;  //玩家的分数
            public string point2;
            public string point3;
            public string point4;

            public string nick1;  //玩家的昵称
            public string nick2;
            public string nick3;
            public string nick4;

            public string head1;  //玩家的头像地址
            public string head2;
            public string head3;
            public string head4;

            public string replayCode;  //回放码
        }

        public class Grade_RoundMessage
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<Grade_Round> data;   //玩家战绩的信息
        }

        public class Grade_RoundMessageData
        {
            public List<Grade_RoundMessage> RoundMessageData = new List<Grade_RoundMessage>();
        }



        public class Grade_RoundData
        {
            public string gameNum;   //该局的游戏局号
            public string playing_method;
            public string dealer;  //庄家座位号

            public string[] point = new string[4];  //玩家的分数
            public string[] nickName = new string[4];  //玩家的昵称
            public string[] headUrl = new string[4];  //头像的地址
            public string selfDrawnSeat;  //玩家自摸的座位号
            public string shootSeat;   //玩家的点炮座位号
            public DateTime endTime;  //本局结束时间
            public string replayCode; //回放码
        }


        public void GetGrade_Round(string json, int status)
        {
            Grade_RoundData_.Clear();
            Grade_RoundMessageData data = new Grade_RoundMessageData();
            data = JsonBase.DeserializeFromJson<Grade_RoundMessageData>(json.ToString());
            if (data.RoundMessageData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.RoundMessageData[0].status);
                return;
            }
            //将数据保存到list中
            for (int i = 0; i < data.RoundMessageData.Count; i++)
            {
                for (int j = 0; j < data.RoundMessageData[i].data.Count; j++)
                {
                    if (Convert.ToInt32(GradeRoom.disType) != 5 && String.Equals(GradeRoom.disTime, data.RoundMessageData[i].data[j].endTim))
                    {
                        continue;
                    }
                    Grade_RoundData grade = new Grade_RoundData();
                    grade.gameNum = data.RoundMessageData[i].data[j].gameNum;
                    grade.playing_method = data.RoundMessageData[i].data[j].playing_method;
                    grade.dealer = data.RoundMessageData[i].data[j].dealer;
                    grade.point[0] = data.RoundMessageData[i].data[j].point1;
                    grade.point[1] = data.RoundMessageData[i].data[j].point2;
                    grade.point[2] = data.RoundMessageData[i].data[j].point3;
                    grade.point[3] = data.RoundMessageData[i].data[j].point4;
                    grade.nickName[0] = data.RoundMessageData[i].data[j].nick1;
                    grade.nickName[1] = data.RoundMessageData[i].data[j].nick2;
                    grade.nickName[2] = data.RoundMessageData[i].data[j].nick3;
                    grade.nickName[3] = data.RoundMessageData[i].data[j].nick4;
                    grade.headUrl[0] = data.RoundMessageData[i].data[j].head1;
                    grade.headUrl[1] = data.RoundMessageData[i].data[j].head2;
                    grade.headUrl[2] = data.RoundMessageData[i].data[j].head3;
                    grade.headUrl[3] = data.RoundMessageData[i].data[j].head4;
                    grade.selfDrawnSeat = data.RoundMessageData[i].data[j].selfDrawnSeat;
                    grade.shootSeat = data.RoundMessageData[i].data[j].shootSeat;
                    grade.endTime = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(data.RoundMessageData[i].data[j].endTim), 0);
                    grade.replayCode = String.Format("{0:0000000}", data.RoundMessageData[i].data[j].replayCode);
                    Grade_RoundData_.Add(grade);
                }
            }

           // Debug.LogWarning ("开始产生对应的战绩预置体");
            SpwanHistroyGrade_Round();
        }
        #endregion

        /// <summary>
        /// 战绩分享
        /// </summary>
        //public void BtnShare()
        //{
        //    MahjongCommonMethod.Instance.ShowRemindFrame("该功能暂未开放");
        //    return;
        //    string url = MahjongLobby_AH.SDKManager.WXInviteUrl;
        //    //Debug.LogError(Convert.ToInt32(GradeRoom.playing_method));
        //    string PlayMethord = MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(GradeRoom.playing_method)].METHOD_NAME;
        //    string dis = MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(GradeRoom.playing_method)].METHOD_DISCRIPTION;
        //    //Debug.LogError(PlayMethord+"  "+dis );
        //    string title = string.Format("{0:yyyy/MM/dd} 双喜麻将->{1}<- 房间码：{2}", DateTime.Now, PlayMethord, roomId.text);
        //    string[] diss = new string[4];
        //    for (int i = 0; i < diss.Length; i++)
        //    {
        //        if (GradeRoom.point[i] >= 0)
        //        {
        //            diss[i] = string.Format("【{0}】{1}", nickName[i].text, "+" + GradeRoom.point[i].ToString());
        //        }
        //        else
        //        {
        //            diss[i] = string.Format("【{0}】{1}", nickName[i].text, GradeRoom.point[i].ToString());

        //        }
        //    }
        //    string _dis = string.Format("本局总成绩：{0},{1},{2},{3},  {4}", diss[0], diss[1], diss[2], diss[3], dis);
        //    SDKManager.Instance.HandleShareWX(url, title, _dis, 0);
        //    // UIMainView.Instance.HistroyGradePanel . BtnClose();
        //}
        //void Spwan

    }

}
