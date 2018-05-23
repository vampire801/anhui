using UnityEngine;
using System.Collections.Generic;
using System;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class HistroyGradePanelData
    {
        //常量
        public string url_suffix = "userGameList.x";   //获取json数据的后缀    
        public string json_title = "HistoryMessageData"; //json数据的标题

        public bool isPanelShow;  //面板显示的标志位
        public bool isShowGrade_Room;  //显示房间战绩的面板
        public bool isShowGrade_Round;  //显示具体每局战绩的面板

        public int BtnCloseStatus;  //按钮点击状态，1表示关闭具体局的面板，2表示关闭历史战绩面板
        public float timer; //上次查询历史战绩的时间

        #region 玩家开放记录

        public List<HistroyGradeMessage_Self> GradeMessage = new List<HistroyGradeMessage_Self>();  //保存玩家的开房信息

        //保存玩家的开放数据信息 
        public class HistroyGradeMessage_Self
        {
            public string openRoomId;  //开房序号，格式：服务器编号-序号-
            public string playing_method;//玩法id
            public string roomNum; //房间号
            public DateTime openTim;  //开房时间
            public string disTime; //解散时间
            public string disType; //解散类型
            public int[] point = new int[4];  //四个玩家的分数
            public string[] nickName = new string[4];  //四个玩家的昵称
            public string[] headUrl = new string[4]; //玩家头像地址    
            public string[] userid = new string[4];  //玩家的id
            public string[] playerComplient = new string[4];  //四个玩家分别赞的座位号                    
        }


        //玩家房间战绩数据
        public class HistroyGradeMessage
        {
            public string openRoomId;  //开房序号，格式：服务器编号-序号-
            public string playing_method;//玩法ID
            public string roomNum;   //房间号
            public string openTim;  //开房时间

            public string payCard;  //实际消耗房卡数量
            public string disType;  // 房间解散类型：1未开始游戏房主主动解散，2未开始游戏时间到了自动解散，3游戏中玩家解散，4全部游戏结束解散-
            public string disTim;  // 解散时间-

            public string point1;  //玩家分数
            public string point2;
            public string point3;
            public string point4;

            public string nick1;  //玩家昵称
            public string nick2;
            public string nick3;
            public string nick4;

            public string head1; //玩家头像
            public string head2;
            public string head3;
            public string head4;

            public string uid1;  //玩家id
            public string uid2;
            public string uid3;
            public string uid4;

            public string comp1;   //玩家赞的信息
            public string comp2;
            public string comp3;
            public string comp4;
        }


        public class HistroyGrade
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<HistroyGradeMessage> data;  //具体的开房数据            
        }

        public class HistroyGradeMessageData
        {
            public List<HistroyGrade> HistoryMessageData = new List<HistroyGrade>();
        }

        /// <summary>
        /// 解析网页获取的json数据
        /// </summary>
        /// <param name="json"></param>
        public void GetMessageData(string json, int status)
        {
            GradeMessage.Clear();
            HistroyGradeMessageData data = new HistroyGradeMessageData();
            data = JsonBase.DeserializeFromJson<HistroyGradeMessageData>(json.ToString());
            //Debug.LogError("+++++++++++++++战绩+++++++++++"+json.ToString());
            if (data.HistoryMessageData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.HistoryMessageData[0].status);
                return;
            }

            //将数据保存到list中
            for (int i = 0; i < data.HistoryMessageData.Count; i++)
            {
                for (int j = 0; j < data.HistoryMessageData[i].data.Count; j++)
                {
                    if (Convert.ToInt32(data.HistoryMessageData[i].data[j].uid1) == 0)
                    {
                        continue;
                    }
                    HistroyGradeMessage_Self grade = new HistroyGradeMessage_Self();

                    grade.openRoomId = data.HistoryMessageData[i].data[j].openRoomId;
                    grade.playing_method = data.HistoryMessageData[i].data[j].playing_method;
                    grade.roomNum = data.HistoryMessageData[i].data[j].roomNum;
                    grade.disType = data.HistoryMessageData[i].data[j].disType;
                    grade.disTime = data.HistoryMessageData[i].data[j].disTim;
                    grade.openTim = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(data.HistoryMessageData[i].data[j].openTim), 0);
                    int mul = 1;  //分数的倍数
                    if (Convert.ToInt32(grade.playing_method) == 14)
                    {
                        mul = 100;
                    }

                    grade.point[0] = Convert.ToInt32(data.HistoryMessageData[i].data[j].point1) * mul;
                    grade.point[1] = Convert.ToInt32(data.HistoryMessageData[i].data[j].point2) * mul;
                    grade.point[2] = Convert.ToInt32(data.HistoryMessageData[i].data[j].point3) * mul;
                    grade.point[3] = Convert.ToInt32(data.HistoryMessageData[i].data[j].point4) * mul;
                    grade.nickName[0] = data.HistoryMessageData[i].data[j].nick1;
                    grade.nickName[1] = data.HistoryMessageData[i].data[j].nick2;
                    grade.nickName[2] = data.HistoryMessageData[i].data[j].nick3;
                    grade.nickName[3] = data.HistoryMessageData[i].data[j].nick4;
                    grade.headUrl[0] = data.HistoryMessageData[i].data[j].head1;
                    grade.headUrl[1] = data.HistoryMessageData[i].data[j].head2;
                    grade.headUrl[2] = data.HistoryMessageData[i].data[j].head3;
                    grade.headUrl[3] = data.HistoryMessageData[i].data[j].head4;
                    grade.userid[0] = data.HistoryMessageData[i].data[j].uid1;
                    grade.userid[1] = data.HistoryMessageData[i].data[j].uid2;
                    grade.userid[2] = data.HistoryMessageData[i].data[j].uid3;
                    grade.userid[3] = data.HistoryMessageData[i].data[j].uid4;
                    grade.playerComplient[0] = data.HistoryMessageData[i].data[j].comp1;
                    grade.playerComplient[1] = data.HistoryMessageData[i].data[j].comp2;
                    grade.playerComplient[2] = data.HistoryMessageData[i].data[j].comp3;
                    grade.playerComplient[3] = data.HistoryMessageData[i].data[j].comp4;
                    GradeMessage.Add(grade);
                }
            }

            //判断红点
            if (data.HistoryMessageData[0].data.Count > 0 && Convert.ToInt64(data.HistoryMessageData[0].data[data.HistoryMessageData[0].data.Count - 1].disTim) > timer)
            {
                UIMainView.Instance.LobbyPanel.RedPoint[2].gameObject.SetActive(true);
            }

            ////判断新手引导是否需要指引
            //if(PlayerPrefs.GetFloat(NewPlayerGuide.Guide.HistroyGrade.ToString())==0&& data.HistoryMessageData[0].data.Count > 0)
            //{
            //    NewPlayerGuide.Instance.OpenIndexGuide(NewPlayerGuide.Guide.HistroyGrade);
            //    NewPlayerGuide.Instance.SetTimeHideGuide_Ie(5f, NewPlayerGuide.Guide.HistroyGrade);
            //}


            //判断代理的新手引导
            if (PlayerPrefs.GetFloat(NewPlayerGuide.Guide.JoinAgency.ToString()) == 0 && data.HistoryMessageData[0].data.Count > 0)
            {
                NewPlayerGuide.Instance.OpenIndexGuide(NewPlayerGuide.Guide.JoinAgency);
                NewPlayerGuide.Instance.SetTimeHideGuide_Ie(5f, NewPlayerGuide.Guide.JoinAgency);
            }
        }

        /// <summary>
        /// 向网页请求战绩
        /// </summary>
        /// <param name="status"></param>
        public void HistroyReq_Web(int status)
        {
            string str = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL + url_suffix : LobbyContants.MAJONG_PORT_URL_T + url_suffix;
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
            //value.Add("uid", "10001626");
            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(str, value, GetMessageData, json_title, status);
        }

        #endregion 玩家开放记录       
    }

}
