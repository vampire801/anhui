using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using XLua;
using anhui;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerMessagePanelData
    {
        //常量
        public string url_suffix = "msg.x";   //获取json数据的后缀    
        public string json_title = "MessageData"; //json数据的标题

        public bool PanelShow;  //面板是否显示的标志位
        public List<PlayerMessage> PlayerMessageData = new List<PlayerMessage>();  //存储玩家的消息

        public float timer;  //上次发消息的时间，主要是用于判断是否有新的消息，用于大厅按钮产生红点        

        public class playerMessageData
        {
            public List<PlayerMessage> MessageData = new List<PlayerMessage>();
        }

        //消息的结构体
        public class PlayerMessage
        {
            public int status;  //请求状态，1成功 5参数错误 9系统错误 其他 失败
            public List<Message> data; //消息的详细信息
        }

        //消息数据
        public class Message
        {
            public string mid;  //消息id
            public string msgType;  //信息类型：1普通信息，2 馆主邀请用户成为馆内成员 3 用户申请加入麻将馆
            public string senderUid;  //发送用户编号，0为系统
            public string senderNick; //发送用户昵称
            public string sendTim;  //发送时间
            public string content;  //消息内容，已经拼接好，直接显示即可
            public string operate;  //接收用户的操作，0表示待操作 1表示同意 2表示拒绝
            public string head; //发送人头像
            public string playCardAcc; //发送人总出牌次数
            public string playCardTimeAcc; //发送人总出牌时间
            public string gameNumAcc; //发送人总局数
            public string disconnectAcc; //发送人掉线局数
            public string compliment; //发送人的赞数
            public string leaveAcc; //发送人退馆次数
            public string kickAcc; //发送人被踢次数
            public string kick; //踢过发送人的麻将馆ID
        }


        /// <summary>
        /// 解析网页获取的json数据
        /// </summary>
        /// <param name="json"></param>
        public void GetMessageData(string json, int status)
        {
            PlayerMessageData.Clear();
            playerMessageData data = new playerMessageData();
            data = JsonBase.DeserializeFromJson<playerMessageData>(json.ToString());
            if (data.MessageData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.MessageData[0].status);
                return;
            }

            //将数据保存到list中
            for (int i = 0; i < data.MessageData.Count; i++)
            {
                PlayerMessageData.Add(data.MessageData[i]);
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.ParlorCheckList = new List<Message>();
                for (int j = 0; j < data.MessageData[i].data.Count; j++)
                {
                    //过滤消息类型，如果玩家消息类型为审核类型，直接过滤添加到馆主的审核列表
                    if (Convert.ToInt32(data.MessageData[i].data[j].msgType) == 3)
                    {
                        pspd.ParlorCheckList.Add(data.MessageData[i].data[j]);
                        UIMainView.Instance.ParlorShowPanel.ShowPointRedPoint(0, MainViewParlorShowPanel.ParlorBtn.CheckList);
                        continue;
                    }

                    //如果玩家收到封馆和解除状态，就会通知馆主消息
                    if (Convert.ToInt32(data.MessageData[i].data[j].msgType) == 4)
                    {
                        Debug.LogError("parlorStatus_feng:" + PlayerPrefs.GetInt("parlorStatus_feng"));
                        if (PlayerPrefs.GetInt("parlorStatus_feng") == 0)
                        {
                            StringBuilder str = new StringBuilder();
                            str.Append("您的麻将馆已被封馆，并将于");
                            int timer_ = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - Convert.ToInt32(data.MessageData[i].data[j].sendTim);
                            str.Append((7 - timer_ / 86400));
                            str.Append("天后自动解散，请尽快联系官方客服解封");
                            UIMgr.GetInstance().GetUIMessageView().Show(str.ToString());
                            PlayerPrefs.SetInt("parlorStatus_feng", (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now));
                            PlayerPrefs.SetInt("parlorStatus_jie", 0);
                            //修改玩家麻将馆的节点信息
                            if (pspd.parlorInfoDef[0] != null)
                            {
                                pspd.parlorInfoDef[0].iStatus = 1;
                            }
                        }



                        continue;
                    }

                    //如果玩家收到封馆和解除状态，就会通知馆主消息
                    if (Convert.ToInt32(data.MessageData[i].data[j].msgType) == 5)
                    {
                        Debug.LogError("parlorStatus_jie:" + PlayerPrefs.GetInt("parlorStatus_jie"));
                        if (PlayerPrefs.GetInt("parlorStatus_jie") == 0)
                        {
                            StringBuilder str = new StringBuilder();
                            str.Append("您的麻将馆已被解封！");
                            UIMgr.GetInstance().GetUIMessageView().Show(str.ToString());
                            PlayerPrefs.SetInt("parlorStatus_jie", (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now));
                            PlayerPrefs.SetInt("parlorStatus_feng", 0);
                            //修改玩家麻将馆的节点信息
                            if (pspd.parlorInfoDef[0] != null)
                            {
                                pspd.parlorInfoDef[0].iStatus = 0;
                            }
                        }
                        continue;
                    }


                    if (Convert.ToInt32(data.MessageData[i].data[j].sendTim) > timer)
                    {
                        //有新的消息，显示提示红点
                        UIMainView.Instance.LobbyPanel.RedPoint[1].gameObject.SetActive(true);
                    }
                }
            }

            if (status == 2)
            {
                UIMainView.Instance.ParlorShowPanel.SpwanParlorCheckList();
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading", "正在读取信息，请稍候...");
                return;
            }

            if (status == 1)
            {
                float nowTime = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
                //如果是新玩家会通知固定消息
                if (PlayerPrefs.GetFloat("RegistTime") == 0 || SDKManager.Instance.CheckStatus == 1)
                {
                    Debug.LogWarning ("不需要添加固定通知消息");
                }
                else if (nowTime - PlayerPrefs.GetFloat("RegistTime") < 604800f)
                {
                    PlayerMessage message = new PlayerMessage();
                    message.status = 1;
                    Message message_0 = new Message();
                    message_0.mid = "0";
                    message_0.msgType = "1";
                    message_0.sendTim = nowTime.ToString();
                    message_0.content = "“姐们，打麻将的诀窍是什么？”\n“多摸，多扛，拼命碰，不放炮。”\n“欧巴，咱们比试比试。”\n“行嘛，欧巴要打一亿飘两亿的。”\n【麻将】是经典国粹棋牌游戏，国人最热爱桌上游泳，据研究中老年人多打麻将还可以健脑活血。有别于其它棋牌游戏，麻将变化多样，考验策略技巧，是令人乐此不疲的博弈游戏。赢了还可以像休闲益智类休闲游戏一样分享战绩和排名到朋友圈、微信好友得瑟得瑟。 \n【双喜麻将】是一款还原山西本土玩法，绿色健康的麻将棋牌游戏，为大家提供轻松愉悦的游戏氛围，游戏内禁止任何形式的赌博行为，一旦发现将永久封停游戏账号。请各位玩家遵守健康游戏和禁赌忠告，轻松游戏，快乐修行是我们的宗旨，祝大家游戏开心！\n◆游戏特色：\n1、山西本土玩法，原汁原味直接上手来一圈；\n2、创建房间，邀请好友，实时对战，尽享自由娱乐体验；\n3、海量金币登陆游戏天天送，操作简单、休闲放松的最佳选择；\n4、时尚简约浓郁中国风，真实还原游戏模式，更接地气；\n5、为你和好友提供同IP报警、防外挂的稳定安全对局环境；/n6、收集更多家乡玩法，快关注官方微信公众号【双喜麻将】联系我们吧，有奖哦！";
                    message_0.operate = "0";
                    PlayerMessageData[0].data.Add(message_0);
                }

                //产生对应的消息面板
                UIMainView.Instance.PlayerMessagePanel.SpwanAgencyMessagePanel();
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading", "正在读取信息，请稍候...");
            }
        }

    }

}
