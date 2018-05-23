using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using PlayBack_1;
using System;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewHistroyGradePanel : MonoBehaviour
    {

        public const string MESSAGE_CLOSE = "MainViewHistroyGradePanel.MESSAGE_CLOSE";  //关闭按钮


        public Image Room_Title;  //开房的图片
        public GameObject GradeRect_Room;   //显示玩家开房记录
        public GameObject Round_Title; //具体房间的信息显示
        public GameObject GradeRect_Round; //显示玩家的具体游戏内的每局的游戏记录
        public GameObject NoneGrade;  //玩家没有战绩的记录
        public GameObject PlayBackCode; //输入回放码的地方
        public Text Code_1;  //回放码

        #region 具体的房间信息
        public Text[] NickName;  //四个玩家的昵称
        public Text RoomId;  //房间号
        public Text MethodName;  //玩法名称
        public Text Timer;  //开房时间
        #endregion

        InfinityGridLayoutGroup infinityGridLayoutGroup;


        void Start()
        {
            //if(PlayerPrefs.GetFloat(NewPlayerGuide.Guide.HistroyGrade.ToString())==0)
            //{
            //    NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.HistroyGrade);                
            //}            
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        //面板更新
        public void UpdateShow()
        {
            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
            if (hgpd.isPanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
                if (hgpd.isShowGrade_Room)
                {
                    GradeRect_Room.SetActive(true);
                    Room_Title.gameObject.SetActive(true);
                    SpwanHistoryPanel();
                }
                else
                {
                    if (infinityGridLayoutGroup)
                    {
                        infinityGridLayoutGroup.RemoveListener_Rect();
                    }
                    //Room_Title.gameObject.SetActive(false);
                    GradeRect_Room.SetActive(false);
                }


                if (hgpd.isShowGrade_Round)
                {
                    //处理玩家点击具体游戏列表
                    Room_Title.gameObject.SetActive(false);
                    Round_Title.SetActive(true);
                    GradeRect_Round.SetActive(true);
                    Debug.LogWarning ("已经激活战绩详情面板");
                }
                else
                {
                    Round_Title.SetActive(false);
                    GradeRect_Round.SetActive(false);
                }

                if (!hgpd.isShowGrade_Room && !hgpd.isShowGrade_Round)
                {
                    NoneGrade.SetActive(true);
                    //Room_Title.gameObject.SetActive(false);
                    GradeRect_Round.SetActive(false);
                    Round_Title.SetActive(false);
                    GradeRect_Room.SetActive(false);
                }

            }
            else
            {
                if (infinityGridLayoutGroup)
                {
                    infinityGridLayoutGroup.RemoveListener_Rect();
                }
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 点击面板关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;

            if (hgpd.BtnCloseStatus == 1)
            {
                hgpd.isShowGrade_Round = false;
                hgpd.isShowGrade_Room = true;
                hgpd.isPanelShow = true;
                hgpd.BtnCloseStatus = 2;

                //删除玩家的之前的面板
                HistroyGradeRound[] rounds = UIMainView.Instance.HistroyGradePanel.GradeRect_Round.transform.Find("Content").GetComponentsInChildren<HistroyGradeRound>(true);
                for (int i = 0; i < rounds.Length; i++)
                {
                    Destroy(rounds[i].gameObject);
                }

                GradeRect_Round.transform.Find("Content").GetComponent<InfinityGridLayoutGroup>().Init();

            }
            else if (hgpd.BtnCloseStatus == 2)
            {
                hgpd.isShowGrade_Round = false;
                hgpd.isShowGrade_Room = false;
                hgpd.isPanelShow = false;
            }

            UpdateShow();
        }


        /// <summary>
        /// 产生历史战绩的面板
        /// </summary>
        public void SpwanHistoryPanel()
        {
            //删除玩家的之前的面板
            HistroyGradeRoom[] rooms = GradeRect_Room.transform.Find("Content").GetComponentsInChildren<HistroyGradeRoom>();

            if (rooms.Length > 0)
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    Destroy(rooms[i].gameObject);
                }

                //return;
            }
            GradeRect_Room.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
            GradeRect_Room.transform.Find("Content").GetComponent<ContentSizeFitter>().enabled = true;



            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
            int count = hgpd.GradeMessage.Count;
            if (count <= 0)
            {
                hgpd.isShowGrade_Room = false;
                hgpd.isShowGrade_Round = false;
                UpdateShow();
            }



            //Debug.LogError("hgpd.GradeMessage：" + hgpd.GradeMessage.Count);
            if (count > 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/HistroyGrade/HistroyGrade_Room"));
                    go.transform.SetParent(GradeRect_Room.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "HistroyGrade_Room_" + i;
                }
                //初始化数据列表;
                infinityGridLayoutGroup = GradeRect_Room.transform.Find("Content").GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.Init();
                //Invoke("SetAmount", 0.1f);
                infinityGridLayoutGroup.SetAmount(hgpd.GradeMessage.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/HistroyGrade/HistroyGrade_Room"));
                    go.transform.SetParent(GradeRect_Room.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "HistroyGrade_Room_" + i;
                    HistroyGradeRoom room = go.GetComponent<HistroyGradeRoom>();
                    room.GradeRoom = hgpd.GradeMessage[i];
                    room.index = i;
                    room.UpdateShow();
                }
            }
        }


        void UpdateChildrenCallback(int index, Transform trans)
        {
            HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
            HistroyGradeRoom room = trans.GetComponent<HistroyGradeRoom>();
            room.GradeRoom = hgpd.GradeMessage[index];
            room.index = index;
            room.UpdateShow();
        }


        /// <summary>
        /// 点击查看他人回放
        /// </summary>
        public void BtnLookOther()
        {
            PlayBackCode.SetActive(true);
        }

        /// <summary>
        /// 点击关闭回放输入窗口
        /// </summary>
        public void BtnPlayBackClose()
        {
            PlayBackCode.SetActive(false);
        }


        /// <summary>
        /// 确认回放码的窗口
        /// </summary>
        /// <param name="code"></param>
        public void BtnConfirmPlayBackCode(string str = "")
        {
            if (string.IsNullOrEmpty(str))
            {
                str = Code_1.text;
            }

            if (str.Length != 7)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("请输入正确的回放码");
                return;
            }
            SDKManager.Instance.gameObject.GetComponent<anhui.CameControler>().PostMsg("loading", "玩命加载中...");
            //根据回放码，获取回放数据
            string url = LobbyContants.MAJONG_PORT_URL + "gameData.x";
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "gameData.x";
            }

            //要传的值
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("gameNum", str);

            GameData.Instance.PlayBackData.sPlayBackNum = str;

            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPlayerPlayBackData, "lsPlayBackMessageData", 0);
            PlayBackCode.SetActive(false);
        }

        public class PlayBack_
        {
            public int status;  //状态，0参数错误  1请求成功  9系统错误
            public string dealer;  //庄家座位号
            public string beginTim;  //开始时间
            public string endTim; //结束时间

            public string point1; //座位1的最终分数
            public string point2; //座位2的最终分数
            public string point3; //座位3的最终分数
            public string point4; //座位4的最终分数

            public string nick1;  //座位1的昵称
            public string nick2;  //座位2的昵称
            public string nick3;  //座位3的昵称
            public string nick4;  //座位4的昵称

            public string head1; //座位1的头像
            public string head2; //座位2的头像
            public string head3; //座3的头像
            public string head4; //座4的头像

            public string replay; //回放数据

            public string param;  //开房参数
            public string seatIndex; //座位索引 0--3
            public string roomNum;  //房间号
            public string playing_method;  //玩法编号
            public string roomProgress; //当前局数/总局数
        }


        public class PlayBackMessageData
        {
            public List<PlayBack_> lsPlayBackMessageData = new List<PlayBack_>();
        }

        /// <summary>
        /// 获取玩家的回放数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        void GetPlayerPlayBackData(string json, int status)
        {
            PlayBackData pbd = GameData.Instance.PlayBackData;
            PlayBackMessageData data = new PlayBackMessageData();
            data = JsonBase.DeserializeFromJson<PlayBackMessageData>(json.ToString());
            if (data.lsPlayBackMessageData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.lsPlayBackMessageData[0].status);
                SDKManager.Instance.gameObject.GetComponent<anhui.CameControler>().PostMsg("uloading");
                //弹出错误窗口
                UIMgr.GetInstance().GetUIMessageView().Show("请输入正确的回放码");
                return;
            }
            Debug.LogError("获取回放嘛");
            //将数据保存到list中
            for (int i = 0; i < data.lsPlayBackMessageData.Count; i++)
            {
                //庄家座位号赋值
                pbd.byDealerSeat = Convert.ToInt32(data.lsPlayBackMessageData[i].dealer);

                //昵称赋值
                pbd.sName[0] = data.lsPlayBackMessageData[i].nick1;
                pbd.sName[1] = data.lsPlayBackMessageData[i].nick2;
                pbd.sName[2] = data.lsPlayBackMessageData[i].nick3;
                pbd.sName[3] = data.lsPlayBackMessageData[i].nick4;

                //头像赋值
                pbd.sHeadUrl[0] = data.lsPlayBackMessageData[i].head1;
                pbd.sHeadUrl[1] = data.lsPlayBackMessageData[i].head2;
                pbd.sHeadUrl[2] = data.lsPlayBackMessageData[i].head3;
                pbd.sHeadUrl[3] = data.lsPlayBackMessageData[i].head4;

                //添加房间号，玩法id
                pbd.sRoomId = String.Format("{0:000000}", data.lsPlayBackMessageData[i].roomNum);
                pbd.iMethodId = Convert.ToInt16(data.lsPlayBackMessageData[i].playing_method);
                pbd.iCurrentGameNum = data.lsPlayBackMessageData[i].roomProgress;

                pbd.ShowSeatNum = Convert.ToInt32(data.lsPlayBackMessageData[i].seatIndex) + 1;

                //改变第一视角的座位号
                for (int j = 0; j < pbd.iUserId.Length; j++)
                {
                    if (GameData.Instance.PlayerNodeDef.iUserId == pbd.iUserId[j])
                    {
                        pbd.ShowSeatNum = j + 1;
                    }
                }
                //确定玩家的操作步数
                pbd.iAllDealNum = data.lsPlayBackMessageData[i].replay.Split(',').Length;

                //解析版本号
                pbd.iPlayBackVersion = Convert.ToInt32(data.lsPlayBackMessageData[i].replay.Split('>')[0]);
                if (pbd.iPlayBackVersion < pbd.iPbVersion_Old)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("您的回放数据版本过低，无法查看！");
                    SDKManager.Instance.gameObject.GetComponent<anhui.CameControler>().PostMsg("uloading");
                    return;
                }


                //解析玩法配置回应消息(直接调用的消息结构体，后面有补位“0”)
                string message = data.lsPlayBackMessageData[i].replay.Split('>')[1] + "000000";

                Debug.LogError("配置回应信息:" + message);

                int ioffset = 0;

                //如果是版本2则，读取最新的消息
                if (pbd.iPlayBackVersion == pbd.iPbVersion_New)
                {
                    MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes msg = new MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes();
                    ioffset = msg.parseBytes(pbd.GetByteToString(message, 40), ioffset);
                    pbd.playingMethodConf = msg.playingMethodConf;
                    //解析玩家的初试分数
                    string[] sPoint = data.lsPlayBackMessageData[i].replay.Split('>')[2].Split(',');
                    for (int k = 0; k < 4; k++)
                    {
                        pbd.iPoint[k] = Convert.ToInt16(sPoint[k]);
                        pbd.iPointInit[k] = Convert.ToInt16(sPoint[k]);
                    }
                    //解析回放数据          
                    pbd.sPlayBackMessageData = new string[pbd.iAllDealNum];
                    pbd.sPlayBackMessageData = data.lsPlayBackMessageData[i].replay.Split('>')[3].Split(',');
                }
                //如果是版本1，则读取上个版本的消息
                else if (pbd.iPlayBackVersion == pbd.iPbVersion_Old)
                {
                    MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes_2 msg = new MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes_2();
                    ioffset = msg.parseBytes(pbd.GetByteToString(message, 20), ioffset);
                    pbd.playingMethodConf_2 = msg.playingMethodConf;
                    //解析回放数据          
                    pbd.sPlayBackMessageData = new string[pbd.iAllDealNum];
                    pbd.sPlayBackMessageData = data.lsPlayBackMessageData[i].replay.Split('>')[3].Split(',');
                }

                //解析玩法参数
                string[] str = data.lsPlayBackMessageData[i].param.Split(',');
                for (int j = 0; j < str.Length; j++)
                {
                    pbd.iPlayMethodParam[j] = Convert.ToUInt32(str[j]);
                }

                Debug.LogError("回放数据:" + data.lsPlayBackMessageData[i].replay);

                //开放参数赋值
                if (!data.lsPlayBackMessageData[i].param.Contains(","))
                {
                    //跳转场景
                    anhui.MahjongCommonMethod.Instance.SkipPlayBack_();
                    //pbd.OpenPlayBackScene();
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    //Application.LoadLevelAdditiveAsync();
                    return;
                }

                string[] param = data.lsPlayBackMessageData[i].param.Split(',');
                for (int k = 0; k < param.Length; k++)
                {
                    pbd.iOpenRoomParam[i] = Convert.ToUInt32(param[k]);
                }

                //跳转场景
                anhui.MahjongCommonMethod.Instance.SkipPlayBack_();
                //pbd.OpenPlayBackScene();
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                //Application.LoadLevelAdditiveAsync("GradePlayBack");
            }
        }

    }
}
