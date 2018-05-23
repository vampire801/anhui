using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using PlayBack_1;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class HistroyGradeRound : MonoBehaviour
    {
        public Text time_m;  //房间创建时间 ---分
        public Text GradeIndex;  //记录编号从1开始      
        //public Image[] winFlag;  //赢家展示图片
        //public Text[] Score_F;  //玩家负的分数        
        public Text[] Score_Z;  //玩家正的分数        
        //public Text[] nickName;  //玩家昵称

        //public Sprite[] winSprite;  //0表示自摸，1表示点炮
        public GameObject _btnShare;
        //本条记录的数据
        public HistroyGradeRoom.Grade_RoundData RoundData = new HistroyGradeRoom.Grade_RoundData();


        /// <summary>
        /// 对本条记录进行更新
        /// </summary>
        public void UpdateShow(int index)
        {
            //timer.text = RoundData.endTime.ToString("yyyy_MM_dd");            
            time_m.text = RoundData.endTime.ToString("HH:mm:ss");
            GradeIndex.text = index.ToString();

            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
#if UNITY_IOS || UNITY_IPONE

                _btnShare.SetActive(false);
#endif
            }
            else
            {
                _btnShare.SetActive(true);
            }
            //处理头像
            for (int i = 0; i < 4; i++)
            {
                //加载头像
                //MahjongCommonMethod.Instance.GetPlayerAvatar(headImage[i], RoundData.headUrl[i], 2);
                //headImage[i].texture = tex[i+1];
                //玩家分数
                Score_Z[i].gameObject.SetActive(true);
                int mul = 1;
                if (Convert.ToInt32(RoundData.playing_method) == 14)
                {
                    mul = 100;
                }
                Score_Z[i].text = (Convert.ToInt32(RoundData.point[i]) * mul).ToString();

                //玩家昵称
                //nickName[i].text = RoundData.nickName[i];
            }

            ////处理赢家标志
            //for (int i = 0; i < 4; i++)
            //{
            //    //处理自摸
            //    if (i == (Convert.ToInt16(RoundData.selfDrawnSeat) - 1))
            //    {
            //        winFlag[i].gameObject.SetActive(true);
            //        winFlag[i].sprite = winSprite[0];
            //    }
            //    else if (i == (Convert.ToInt16(RoundData.shootSeat) - 1))
            //    {
            //        winFlag[i].gameObject.SetActive(true);
            //        winFlag[i].sprite = winSprite[1];
            //    }
            //    else
            //    {
            //        winFlag[i].gameObject.SetActive(false);
            //    }
            //}

        }

        //点击回放按钮
        public void BtnBackPlay()
        {
            //if (LobbyContants.SeverType != "16")
            //{
            //    MahjongCommonMethod.Instance.ShowRemindFrame("该功能暂未开放");
            //    return;
            //}


            //根据回放码，获取回放数据
            string url = LobbyContants.MAJONG_PORT_URL + url_suffix;
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + url_suffix;
            }

            //要传的值
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("gameNum", RoundData.replayCode);

            Debug.Log ("RoundData.replayCode:" + RoundData.replayCode);
            Debug.Log ("url:" + url);
            GameData.Instance.PlayBackData.sPlayBackNum = RoundData.replayCode;

            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetPlayerPlayBackData, json_data, 0);
        }

        public void BtnShare()
        {
            string url = SDKManager.WXInviteUrl + RoundData.replayCode;

            string PlayMethord = anhui.MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(RoundData.playing_method)].METHOD_NAME;
            string dis = anhui.MahjongCommonMethod.Instance._dicMethodConfig[Convert.ToInt32(RoundData.playing_method)].METHOD_DISCRIPTION;
            string title = string.Format("玩法 ：{0}  回放码 ： {1}", PlayMethord, RoundData.replayCode);
            string[] diss = new string[4];
            string _dis = string.Format("玩家 【{0}】分享了游戏录像，点击即可查看录像。", GameData.Instance.PlayerNodeDef.szNickname);
            SDKManager.Instance.HandleShareWX(url, title, _dis, 0, 0, 0, "");
        }


        #region 获取回放数据
        string url_suffix = "gameData.x";
        string json_data = "lsPlayBackMessageData";  //json的开头的变量名

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

            public string sex1;  //座位1的性别
            public string sex2;  //座位2的性别
            public string sex3;  //座位3的性别
            public string sex4;  //座位4的性别

            public string uid1;  //座位1的玩家编号
            public string uid2;  //座位2的玩家编号
            public string uid3;  //座位3的玩家编号
            public string uid4;  //座位4的玩家编号

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
                UIMgr.GetInstance().GetUIMessageView().Show("您的回放数据版本过低，无法查看！");
                return;
            }

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

                //性别赋值
                pbd.sex[0] = Convert.ToInt32(data.lsPlayBackMessageData[i].sex1);
                pbd.sex[1] = Convert.ToInt32(data.lsPlayBackMessageData[i].sex2);
                pbd.sex[2] = Convert.ToInt32(data.lsPlayBackMessageData[i].sex3);
                pbd.sex[3] = Convert.ToInt32(data.lsPlayBackMessageData[i].sex4);

                //玩家id赋值
                pbd.iUserId[0] = Convert.ToInt32(data.lsPlayBackMessageData[i].uid1);
                pbd.iUserId[1] = Convert.ToInt32(data.lsPlayBackMessageData[i].uid2);
                pbd.iUserId[2] = Convert.ToInt32(data.lsPlayBackMessageData[i].uid3);
                pbd.iUserId[3] = Convert.ToInt32(data.lsPlayBackMessageData[i].uid4);

                //添加房间号，玩法id
                pbd.sRoomId = String.Format("{0:000000}", data.lsPlayBackMessageData[i].roomNum);
                pbd.iMethodId = Convert.ToInt16(data.lsPlayBackMessageData[i].playing_method);
                pbd.iCurrentGameNum = data.lsPlayBackMessageData[i].roomProgress;

              //  Debug.LogError("玩家的第一视角座位号:" + Convert.ToInt32(data.lsPlayBackMessageData[i].seatIndex));

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
                int ioffset = 0;


                //如果是版本2则，读取最新的消息
                if (pbd.iPlayBackVersion == pbd.iPbVersion_New)
                {
                    MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes msg = new MahjongGame_AH.Network.Message.NetMsg.ClientPlayingMethodConfRes();
                    ioffset = msg.parseBytes(pbd.GetByteToString(message, 48), ioffset);
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
                    pbd.iPlayMethodParam[j] = Convert.ToUInt32 (str[j]);
                }

                //Debug.LogError("回放数据:" + data.lsPlayBackMessageData[i].replay);

                //开放参数赋值
                if (!data.lsPlayBackMessageData[i].param.Contains(","))
                {
                    //跳转场景
                    anhui.MahjongCommonMethod.Instance.SkipPlayBack_();
                    //pbd.OpenPlayBackScene();
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    //Application.LoadLevelAdditiveAsync("GradePlayBack");
                    return;
                }

                string[] param = data.lsPlayBackMessageData[i].param.Split(',');
                for (int k = 0; k < param.Length; k++)
                {
                    pbd.iOpenRoomParam[i] = Convert.ToUInt32(param[k]);
                }
            }

            //跳转场景
            anhui.MahjongCommonMethod.Instance.SkipPlayBack_();
            //pbd.OpenPlayBackScene();
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            //Application.LoadLevelAdditiveAsync("GradePlayBack");
        }

        #endregion
    }

}
