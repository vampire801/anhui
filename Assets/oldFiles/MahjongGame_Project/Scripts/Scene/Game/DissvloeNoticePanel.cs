using Common;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using MahjongGame_AH.Data;
using MahjongGame_AH.Network;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class DissvloeNoticePanel : MonoBehaviour
    {
        public Text Title; //显示发起人的信息
        public Text[] PlayerDissolveMessage;  //玩家的同意拒绝的详情
        public RawImage[] HeadImage; //头像显示
        public Text TimerDown; //玩家的倒计时界面
        public Image[] Initiator; //发起人图片        
        public Image[] DisConnect; //离线的显示图片
        public Button Agree;  //同意按钮
        public Button Canel;  //拒绝按钮

        public int iseatNum;   //发起人的座位号     1--4
        [HideInInspector]
        public float timer = 100f;   //面板倒计时，如果玩家点击同意或者拒绝当倒计时到了之后 自动发送同意解散消息

        public string[] color = new string[2] { "208716FF", "7E4B0EFF" };

        void OnEnable()
        {
            timer = 100.0f;
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //关闭界面，同时发送同意解散房间的需求
                SendDissolveMessage(2);
                gameObject.SetActive(false);
            }
            else
            {
                if (TimerDown != null)
                {
                    TimerDown.text = timer.ToString("0");
                }
            }
        }

        /// <summary>
        /// 发送同意解散房间的消息
        /// </summary>
        /// <param name="status">1发起，2表示同意，3表示拒绝</param>
        public void SendDissolveMessage(int status)
        {
            NetMsg.ClientDismissRoomReq msg = new NetMsg.ClientDismissRoomReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.cType = (byte)status;
            NetworkMgr.Instance.GameServer.SendDisMissRoomReq(msg);
            //发送准备请求
            if (status == 3 && GameData.Instance.PlayerPlayingPanelData.iDissolveStatus == 1)
            {
                GameData.Instance.PlayerPlayingPanelData.iDissolveStatus = 0;
                NetMsg.ClientReadyReq ready = new NetMsg.ClientReadyReq();
                ready.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                NetworkMgr.Instance.GameServer.SendReadyReq(ready);
            }
        }


        public void ShowAllPlayerMessage()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            StringBuilder str = new StringBuilder();

            //显示玩家头像
            for (int i = 0; i < 4; i++)
            {
                anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[i], pppd.usersInfo[i + 1].szHeadimgurl);
            }

            str.Append("玩家【");
            str.Append(pppd.usersInfo[iseatNum].szNickname);
            str.Append("】申请解散房间");
            Title.text = str.ToString();

            for (int i = 0; i < 4; i++)
            {
                StringBuilder str_0 = new StringBuilder();

                if (i == iseatNum - 1)
                {
                    PlayerDissolveMessage[i].text = "<color=#" + color[0] + ">已同意</color>";
                    Initiator[i].gameObject.SetActive(true);
                }
                else
                {
                    PlayerDissolveMessage[i].text = "<color=#" + color[1] + ">等待选择</color>";
                    Initiator[i].gameObject.SetActive(false);
                }
            }

            if (iseatNum == pppd.bySeatNum)
            {
                HideBtn();
            }
            else
            {
                Agree.gameObject.SetActive(true);
                Canel.gameObject.SetActive(true);
            }

            for (int i = 0; i < pppd.DisConnectStatus.Length; i++)
            {
                if (pppd.DisConnectStatus[i] == 1)
                {
                    DisConnect[i].gameObject.SetActive(true);
                }
                else
                {
                    DisConnect[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 改变玩家的解散房间的状态
        /// </summary>
        /// <param name="seatNum">1--4玩家座位号</param>
        /// <param name="status">1表示同意，2表示拒绝</param>
        public void UpdateShow(int seatNum, int status)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //更新别人的状态，出去发起人按照其他三个用户的座位号大小按顺序显示状态
            StringBuilder str_1 = new StringBuilder();

            //改变状态
            if (status == 1)
            {
                str_1.Append("<color=#" + color[0] + ">已同意</color>");
            }
            else
            {
                str_1.Append("<color=#" + color[1] + ">等待选择</color>");
            }
            PlayerDissolveMessage[seatNum - 1].text = str_1.ToString();

            if (seatNum == iseatNum)
            {
                Initiator[seatNum - 1].gameObject.SetActive(true);
            }
            else
            {
                Initiator[seatNum - 1].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 隐藏按钮
        /// </summary>
        public void HideBtn()
        {
            Agree.gameObject.SetActive(false);
            Canel.gameObject.SetActive(false);
        }

    }

}
