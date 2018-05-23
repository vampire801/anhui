using UnityEngine;
using MahjongLobby_AH.Network.Message;
using System.Collections.Generic;
using System;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class InsteadOpenRoomPanelData
    {
        public bool isClickInsteadOpenRoom; //是否点击了代开按钮

        public bool PanelShow;   //面板的是否显示的标志位        
        public bool InsteadRulePanelShow;  //代开规则是否显示的标志位                                           
        public bool isFirstSpwanInsteadRecord;  //是否已经产生过玩家的代开记录
        public bool isNoRecord;  //无代开记录的标志位
        public int iBtnStatus = 1; //代开房间和代开记录按钮的状态，俩个按钮公用一个参数，状态相反

        //开启的房间信息，表示没有开始游戏的信息
        public List<RoomInfo> OpenRoomInfo_UnStart = new List<RoomInfo>();

        //开启的房间信息，表示已经开始游戏的房间信息
        public List<RoomInfo> OpenRoomInfo_Started = new List<RoomInfo>();

        public int iSpwanPrefabNum = 0;  //产生预置体的次数，只会生成一次

        public class RoomInfo
        {
            public int iPlayingMethod; // 游戏玩法
            public int iServerId; // 游戏服务器编号
            public short iTableNum; // 桌编号            
            public byte cOpenRoomStatus; // 桌的开房状态，0没使用，1已经预订，2等待开始游戏，3已开始游戏
            public byte byColorFlag; // 颜色标志
            public int iOpenRoomTime; // 开房时间
            public int iCompliment; // 要求赞的数量
            public int iDisconnectRate; // 掉线率要求
            public int iDiscardTime; // 出牌速度要求            
            public uint[] caOpenRoomParam = new uint[NetMsg.OpenRoomParamRow]; // 开房的参数
            public int roomNum; //房间号
            public int[] iuserId = new int[4]; //玩家id
        }


        /// <summary>
        /// 存储玩家的代开房间信息
        /// </summary>
        /// <param name="msgg"></param>
        public void Value(NetMsg.OpenRoomInfo msgg)
        {
            RoomInfo roominfo = new RoomInfo();
            roominfo.iPlayingMethod = msgg.iPlayingMethod;
            roominfo.iServerId = msgg.iServerId;
            roominfo.iTableNum = msgg.iTableNum;
            roominfo.cOpenRoomStatus = msgg.cOpenRoomStatus;
            roominfo.byColorFlag = msgg.byColorFlag;
            roominfo.iOpenRoomTime = msgg.iOpenRoomTime;
            roominfo.iCompliment = msgg.iCompliment;
            roominfo.iDisconnectRate = msgg.iDisconnectRate;
            roominfo.iDiscardTime = msgg.iDiscardTime;
            roominfo.caOpenRoomParam = msgg.caOpenRoomParam;
            roominfo.roomNum = msgg.iRoomNum;
            roominfo.iuserId = msgg.iaUserId;

            if (msgg.cOpenRoomStatus < 3)
            {
                OpenRoomInfo_UnStart.Add(roominfo);
            }
            else
            {
                OpenRoomInfo_Started.Add(roominfo);
            }
        }


        /// <summary>
        /// 代开房间状态改变的通知消息
        /// </summary>
        public class InsteadOpenStatusNotice
        {
            public short sTableNum;   //卓编号
            public byte byOpenRoomStatus;  //桌的开放状态
            public int[] iaUserId = new int[4];  //桌上用户
        }


        /// <summary>
        /// 处理改变代开房间状态的通知
        /// </summary>
        /// <param name="msg"></param>
        public void ChangePlayerInsteadMessage(InsteadOpenStatusNotice msg)
        {
            //遍历玩家的没开始的房间的状态
            for (int i = 0; i < OpenRoomInfo_UnStart.Count; i++)
            {
                //找到当前的对应的桌号
                if (OpenRoomInfo_UnStart[i].iTableNum == msg.sTableNum)
                {
                    OpenRoomInfo_UnStart[i].iuserId = msg.iaUserId;

                    //如果四个玩家都已经坐下，修改房间状态
                    bool Status = false;
                    for (int j = 0; j < 4; j++)
                    {
                        if (OpenRoomInfo_UnStart[i].iuserId[j] > 0)
                        {
                            Status = true;
                        }
                        else
                        {
                            Status = false;
                        }
                    }

                    if (Status)
                    {
                        //添加信息到 OpenRoomInfo_Started
                        OpenRoomInfo_UnStart[i].cOpenRoomStatus = 3;
                        OpenRoomInfo_Started.Add(OpenRoomInfo_UnStart[i]);

                        //在这里直接生成一个新的已经开始游戏的房间
                        if (PanelShow)
                        {
                            Debug.LogError("msg.sTableNum:" + msg.sTableNum);
                            GetPointInsteadOpenRoomStatus(msg.sTableNum, 1).Del();
                            UIMainView.Instance.InsteadOpenRoomPanel.SpwanAllRoomStatus(null);
                            UIMainView.Instance.InsteadOpenRoomPanel.SpwanAllRoomStatus(OpenRoomInfo_UnStart[i]);
                        }
                        //移除信息从OpenRoomInfo_UnStart
                        OpenRoomInfo_UnStart.RemoveAt(i);
                        return;
                    }
                    else
                    {
                        if (PanelShow)
                        {
                            //更新玩家界面信息，只更新玩家的桌面玩家数量
                            InsteadOpenMessage room = GetPointInsteadOpenRoomStatus(msg.sTableNum, 1);
                            room.ShowRoomPlayerStatus(msg.iaUserId);
                        }
                        else
                        {
                            OpenRoomInfo_UnStart[i].cOpenRoomStatus = msg.byOpenRoomStatus;
                            OpenRoomInfo_UnStart[i].iuserId = msg.iaUserId;
                        }
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// 获取指定的代开房间的按钮
        /// </summary>
        /// <param name="iTableNum"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public InsteadOpenMessage GetPointInsteadOpenRoomStatus(short iTableNum, int status)
        {
            if (iTableNum <= 0)
            {
                return null;
            }

            InsteadOpenMessage go = null;

            //处理未开始游戏的房间
            if (status == 1)
            {
                InsteadOpenMessage[] AllRoomStatus = UIMainView.Instance.InsteadOpenRoomPanel.CurrentRoomStatus.
                    transform.GetComponentsInChildren<InsteadOpenMessage>();

                for (int i = 0; i < AllRoomStatus.Length; i++)
                {
                    if (AllRoomStatus[i].roomInfo == null)
                    {
                        continue;
                    }
                    if (AllRoomStatus[i].roomInfo.iTableNum == iTableNum)
                    {
                        go = AllRoomStatus[i];
                    }
                }
            }
            else
            {
                InsteadOpenMessage[] AllRoomStatus = UIMainView.Instance.InsteadOpenRoomPanel.RacingRoomStatus.
                    transform.GetComponentsInChildren<InsteadOpenMessage>();
                for (int i = 0; i < AllRoomStatus.Length; i++)
                {
                    if (AllRoomStatus[i].roomInfo.iTableNum == iTableNum)
                    {
                        go = AllRoomStatus[i];
                    }
                }
            }

            Debug.LogError("玩家代开信息：" + go.GetComponent<InsteadOpenMessage>().roomInfo.iTableNum);

            return go;
        }

        #region 获取代开记录的数据

        //常量
        public string url_suffix = "openRoomList.x";   //获取json数据的后缀    
        public string json_title = "InsteadRecordMessage"; //json数据的标题

        public List<InsteadOpenRoomMessage> RoomMessage = new List<InsteadOpenRoomMessage>();  //保存玩家的开房信息

        //保存玩家的开放数据信息 
        public class InsteadOpenRoomMessage
        {
            public string openRoomId;  //开房序号，格式：服务器编号-序号-
            public string roomNum; //房间号
            public string openTim;  //开房时间
            public string payCard;  //实际消耗房卡数量
            public string disType;  //房间解散类型：1未开始游戏房主主动解散，2未开始游戏时间到了自动解散，3游戏中玩家解散，4全部游戏结束解散-
            public string disTim;  //解散时间-
            public string[] point = new string[4];  //四个玩家的分数
            public string[] nickName = new string[4];  //四个玩家的昵称
            public string[] headUrl = new string[4]; //玩家头像地址    
            public string[] userid = new string[4];  //玩家的id
            public string[] playerComplient = new string[4];  //四个玩家分别赞的座位号     
            public string color;  //房间的颜色
        }


        //玩家房间战绩数据
        public class InsteadRecordMessage
        {
            public string openRoomId;  //开房序号，格式：服务器编号-序号-
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

            public string color; //房间的颜色标记
        }


        public class InsteadRecord
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<InsteadRecordMessage> data;  //具体的开房数据            
        }

        public class InsteadRecordMessageData
        {
            public List<InsteadRecord> InsteadRecordMessage = new List<InsteadRecord>();
        }

        /// <summary>
        /// 解析网页获取的json数据
        /// </summary>
        /// <param name="json"></param>
        public void GetMessageData(string json, int status = 0)
        {
            RoomMessage.Clear();
            InsteadRecordMessageData data = new InsteadRecordMessageData();
            data = JsonBase.DeserializeFromJson<InsteadRecordMessageData>(json.ToString());
            if (data.InsteadRecordMessage[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.InsteadRecordMessage[0].status);
                return;
            }





            //将数据保存到list中
            for (int i = 0; i < data.InsteadRecordMessage.Count; i++)
            {
                for (int j = 0; j < data.InsteadRecordMessage[i].data.Count; j++)
                {
                    if (Convert.ToInt16(data.InsteadRecordMessage[i].data[j].disType) == 0)
                    {
                        continue;
                    }
                    InsteadOpenRoomMessage grade = new InsteadOpenRoomMessage();
                    grade.openRoomId = data.InsteadRecordMessage[i].data[j].openRoomId;
                    grade.roomNum = data.InsteadRecordMessage[i].data[j].roomNum;
                    grade.openTim = data.InsteadRecordMessage[i].data[j].openTim;
                    grade.payCard = data.InsteadRecordMessage[i].data[j].payCard;
                    grade.disType = data.InsteadRecordMessage[i].data[j].disType;
                    grade.disTim = data.InsteadRecordMessage[i].data[j].disTim;
                    grade.point[0] = (data.InsteadRecordMessage[i].data[j].point1);
                    grade.point[1] = (data.InsteadRecordMessage[i].data[j].point2);
                    grade.point[2] = (data.InsteadRecordMessage[i].data[j].point3);
                    grade.point[3] = (data.InsteadRecordMessage[i].data[j].point4);
                    grade.nickName[0] = data.InsteadRecordMessage[i].data[j].nick1;
                    grade.nickName[1] = data.InsteadRecordMessage[i].data[j].nick2;
                    grade.nickName[2] = data.InsteadRecordMessage[i].data[j].nick3;
                    grade.nickName[3] = data.InsteadRecordMessage[i].data[j].nick4;
                    grade.headUrl[0] = data.InsteadRecordMessage[i].data[j].head1;
                    grade.headUrl[1] = data.InsteadRecordMessage[i].data[j].head2;
                    grade.headUrl[2] = data.InsteadRecordMessage[i].data[j].head3;
                    grade.headUrl[3] = data.InsteadRecordMessage[i].data[j].head4;
                    grade.userid[0] = data.InsteadRecordMessage[i].data[j].uid1;
                    grade.userid[1] = data.InsteadRecordMessage[i].data[j].uid2;
                    grade.userid[2] = data.InsteadRecordMessage[i].data[j].uid3;
                    grade.userid[3] = data.InsteadRecordMessage[i].data[j].uid4;
                    grade.playerComplient[0] = data.InsteadRecordMessage[i].data[j].comp1;
                    grade.playerComplient[1] = data.InsteadRecordMessage[i].data[j].comp2;
                    grade.playerComplient[2] = data.InsteadRecordMessage[i].data[j].comp3;
                    grade.playerComplient[3] = data.InsteadRecordMessage[i].data[j].comp4;
                    grade.color = data.InsteadRecordMessage[i].data[j].color;
                    RoomMessage.Add(grade);
                }
            }

            if (RoomMessage.Count == 0)
            {
                isNoRecord = true;
            }
            else
            {
                isNoRecord = false;
            }
        }

        #endregion

    }

}
