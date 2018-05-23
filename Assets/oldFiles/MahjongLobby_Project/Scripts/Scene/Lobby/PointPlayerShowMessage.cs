using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class PointPlayerShowMessage : MonoBehaviour
    {
        public RawImage HeadIamge;  //玩家头像
        public Text NickName; //玩家昵称
        public Text UserId; //玩家id
        public Text PutCardSpeed;  //出牌速度
        public Text EscapeRate; //逃跑率
        public Text Completite;  //获赞数量
        public GameObject[] Btn;  //0表示审核列表的操作按钮
                                  //1表示成员列表的操作按钮

        public GameObject ParlorMessage_Num; //麻将馆退管次数和被踢次数        

        PlayerMessagePanelData.Message Message_Check_ = new PlayerMessagePanelData.Message();
        int useId; //玩家id

        public void UpdateShow_Member(ParlorShowPanelData.ParlorMemberMessage Message_Member)
        {
            transform.GetComponent<Canvas>().overrideSorting = true;
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadIamge, Message_Member.head);
            NickName.text = Message_Member.nickname;

            ParlorMessage_Num.SetActive(false);
            int BossId = 0;
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //更新老板还是个人
            for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
            {
                if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == pspd.iParlorId)
                {
                    BossId = pspd.parlorInfoDef[i].iBossId;
                    break;
                }
            }

            if (BossId == Convert.ToInt32(Message_Member.userId) || Convert.ToInt32(Message_Member.userId) == GameData.Instance.PlayerNodeDef.iUserId || GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            {
                UserId.text = "ID:" + Message_Member.userId;
            }
            else
            {
                UserId.text = "ID:" + Message_Member.userId.Substring(0, 4) + "****";
            }

            useId = Convert.ToInt32(Message_Member.userId);

            if (Convert.ToInt32(Message_Member.playCardAcc) == 0)
            {
                PutCardSpeed.text = "0秒/平均";
            }
            else
            {
                PutCardSpeed.text = (Convert.ToInt32(Message_Member.playCardTimeAcc) / Convert.ToInt32(Message_Member.playCardAcc)).ToString() + "秒/平均";
            }

            if (Convert.ToInt32(Message_Member.gameNumAcc) == 0)
            {
                EscapeRate.text = "0%";
            }
            else
            {
                EscapeRate.text = (Convert.ToInt32(Message_Member.disconnectAcc) / Convert.ToInt32(Message_Member.gameNumAcc)).ToString() + "%";
            }

            Completite.text = Message_Member.compliment;

            //如果查看人不是老板，不显示踢出按钮
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0 && GameData.Instance.PlayerNodeDef.iUserId != Convert.ToInt32(Message_Member.userId))
            {
                Btn[1].SetActive(true);
                Btn[0].SetActive(false);
            }
            else
            {
                Btn[1].SetActive(false);
                Btn[0].SetActive(false);
            }
        }

        public void UpdateShow_Check(PlayerMessagePanelData.Message Message_Check)
        {
            transform.GetComponent<Canvas>().overrideSorting = true;
            Message_Check_ = Message_Check;
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadIamge, Message_Check.head);
            NickName.text = Message_Check.senderNick;
            UserId.text = "ID:" + Message_Check.senderUid;
            useId = Convert.ToInt32(Message_Check.senderUid);
            if (Convert.ToInt32(Message_Check.playCardAcc) == 0)
            {
                PutCardSpeed.text = "0秒/平均";
            }
            else
            {
                PutCardSpeed.text = (Convert.ToInt32(Message_Check.playCardTimeAcc) / Convert.ToInt32(Message_Check.playCardAcc)).ToString() + "秒/平均";
            }

            if (Convert.ToInt32(Message_Check.gameNumAcc) == 0)
            {
                EscapeRate.text = "0%";
            }
            else
            {
                EscapeRate.text = (Convert.ToInt32(Message_Check.disconnectAcc) / Convert.ToInt32(Message_Check.gameNumAcc)).ToString() + "%";
            }

            Completite.text = Message_Check.compliment;
            if (Convert.ToInt32(Message_Check.operate) != 0)
            {
                Btn[0].SetActive(false);
                Btn[1].SetActive(false);
            }
            else
            {
                Btn[0].SetActive(true);
                Btn[1].SetActive(false);
            }
            ParlorMessage_Num.SetActive(true);
            ParlorMessage_Num.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = Message_Check.leaveAcc + "次";
            ParlorMessage_Num.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = Message_Check.kickAcc + "次";
        }

        //踢出本馆
        public void BtnKickParlor()
        {
            NetMsg.ClientKickParlorReq msg = new NetMsg.ClientKickParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
            msg.iKickUserId = useId;
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.SendKickParlorReq(msg);
            Destroy(gameObject);
        }

        //同意或拒绝加馆申请
        public void BtnAgreeParlor(int index)
        {
            GameData gd = GameData.Instance;
            NetMsg.ClientMessageOperateReq msg = new NetMsg.ClientMessageOperateReq();
            msg.iUserId = gd.PlayerNodeDef.iUserId;
            msg.iMessageId = Convert.ToInt32(Message_Check_.mid);
            msg.cMessageType = (sbyte)Convert.ToInt16(Message_Check_.msgType);
            msg.cOperate = (sbyte)index;
            NetworkMgr.Instance.LobbyServer.SendMessageOperateReq(msg);

            MemberListMessage[] parlor = UIMainView.Instance.ParlorShowPanel.ParlorCheckList.transform.GetChild(1).GetChild(0).GetComponentsInChildren<MemberListMessage>();

            //修改消息状态
            for (int i = 0; i < parlor.Length; i++)
            {
                if (Convert.ToInt32(parlor[i].Message_Check.mid) == Convert.ToInt32(Message_Check_.mid))
                {
                    parlor[i].ChangeMessageShow(index);
                    break;
                }
            }

            Destroy(gameObject);
        }
    }
}