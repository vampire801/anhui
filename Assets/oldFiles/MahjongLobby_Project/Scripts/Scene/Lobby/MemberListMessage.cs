using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MemberListMessage : MonoBehaviour
    {
        public RawImage HeadImage;  //头像
        public Text NickName; //昵称
        public Text UserId; //玩家id
        public Text putSpeed; //出牌速度
        public Text MonthVitity; //月活跃度
        public Text OnlineTimer; //显示上限时长
        public Text LevelParlorAcc; //退出馆的数量
        public Text KickParlorAcc; //踢出馆的数量
        public Image Sign; //老板和自己的特殊标志
        public Sprite[] Sign_Tex;  //老板和自己的图片  0表示老板 1表示会员
        public GameObject[] BtnOpeartion; //操作按钮  0表示操作按钮 1表示已同意  2表示已拒绝
        public GameObject KickSign; //被踢出的标示
        //保存成员的信息
        public ParlorShowPanelData.ParlorMemberMessage Message_Member = new ParlorShowPanelData.ParlorMemberMessage();

        //保存馆主的对应的馆的审核列表
        public PlayerMessagePanelData.Message Message_Check = new PlayerMessagePanelData.Message();

        public int Type; //1表示更新审核列表  2表示更新成员列表
        //更新界面
        public void UpdateShow_Member(ParlorShowPanelData.ParlorMemberMessage message)
        {
            Type = 2;
            Message_Member = message;
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage, message.head);
            NickName.text = message.nickname;
            int length = 0;
            if (message.nickname.Length > 6)
            {
                length = 6;
            }
            else
            {
                length = message.nickname.Length;
            }

            if (NickName.preferredWidth > 120)
            {
                NickName.text = message.nickname.Substring(0, length) + "...";
            }

            if (Convert.ToInt32(message.playCardAcc) != 0)
            {
                putSpeed.text = (Convert.ToInt32(message.playCardTimeAcc) / Convert.ToInt32(message.playCardAcc)).ToString() + "s/平均";
            }
            else
            {
                putSpeed.text = "0s/平均";
            }

            MonthVitity.text = message.monthVitality;
            float timer = (int)anhui.MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - Convert.ToInt32(message.loginTime);
            if (timer <= 0)
            {
                timer = 0;
            }
            //如果上次登录时间大于24小时
            float hour = timer / 3600f;
            if (hour > 24)
            {
                OnlineTimer.text = (int)(hour / 24f + 0.5f) + "天前";
            }
            else
            {
                if (hour > 1)
                {
                    OnlineTimer.text = (int)hour + "小时前";
                }
                else
                {
                    OnlineTimer.text = (int)timer / 60 + "分钟前";
                }
            }
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
            UserId.text = message.userId;
            if (BossId > 0)
            {
                if (BossId == Convert.ToInt32(message.userId))
                {
                    Sign.gameObject.SetActive(true);
                    Sign.sprite = Sign_Tex[0];
                }
                else
                {
                    if (Convert.ToInt32(message.userId) == GameData.Instance.PlayerNodeDef.iUserId)
                    {
                        Sign.gameObject.SetActive(true);
                        Sign.sprite = Sign_Tex[1];
                    }
                    else
                    {
                        Sign.gameObject.SetActive(false);
                        if (GameData.Instance.PlayerNodeDef.iMyParlorId == 0)
                        {
                            UserId.text = message.userId.Substring(0, 4) + "****";
                        }
                    }
                }
            }
        }

        //更新审核列表信息
        public void UpdateShow_Check(PlayerMessagePanelData.Message message)
        {
            Type = 1;
            Message_Check = message;
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage, message.head);
            NickName.text = message.senderNick;

            int length = 0;
            if (message.senderNick.Length > 6)
            {
                length = 6;
            }
            else
            {
                length = message.senderNick.Length;
            }

            if (NickName.preferredWidth > 120)
            {
                NickName.text = message.senderNick.Substring(0, length) + "...";
            }

            UserId.text = message.senderUid;
            LevelParlorAcc.text = message.leaveAcc;
            KickParlorAcc.text = message.kickAcc;
            BtnOpeartion[0].gameObject.SetActive(true);
            BtnOpeartion[1].gameObject.SetActive(false);
            BtnOpeartion[2].gameObject.SetActive(false);
            //显示是否被踢过的信息
            string[] kick = message.kick.Split(',');
            KickSign.SetActive(false);
            for (int i = 0; i < kick.Length; i++)
            {
                if (Convert.ToInt32(kick[i]) == GameData.Instance.ParlorShowPanelData.iParlorId)
                {
                    KickSign.SetActive(true);
                    break;
                }
            }
        }

        /// <summary>
        /// 显示操作信息
        /// </summary>
        /// <param name="status"></param>
        public void ShowOPeration(int status)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == status)
                {
                    BtnOpeartion[i].SetActive(true);
                }
                else
                {
                    BtnOpeartion[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// 点击本身的预置体查看详细信息
        /// </summary>
        public void BtnClick()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/Parlor/PointPlayerShowMessage"));
            go.transform.SetParent(UIMainView.Instance.ParlorShowPanel.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            go.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            go.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            go.GetComponent<RectTransform>().anchorMax = Vector2.one;
            if (Type == 1)
            {
                go.GetComponent<PointPlayerShowMessage>().UpdateShow_Check(Message_Check);
            }
            else
            {
                go.GetComponent<PointPlayerShowMessage>().UpdateShow_Member(Message_Member);
            }
        }

        //同意或拒绝加馆申请
        public void BtnAgreeParlor(int index)
        {
            GameData gd = GameData.Instance;
            NetMsg.ClientMessageOperateReq msg = new NetMsg.ClientMessageOperateReq();
            msg.iUserId = gd.PlayerNodeDef.iUserId;
            msg.iMessageId = Convert.ToInt32(Message_Check.mid);
            msg.cMessageType = (sbyte)Convert.ToInt16(Message_Check.msgType);
            msg.cOperate = (sbyte)index;
            NetworkMgr.Instance.LobbyServer.SendMessageOperateReq(msg);
            ChangeMessageShow(index);
        }

        //馆主点击同意或申请后，删除本条
        public void ChangeMessageShow(int index)
        {
            if (index == 1)
            {
                Message_Check.operate = "1";
            }
            else if (index == 2)
            {
                Message_Check.operate = "2";
            }
            //隐藏按钮
            for (int i = 0; i < BtnOpeartion.Length; i++)
            {
                if (i == index)
                {
                    BtnOpeartion[i].SetActive(true);
                }
                else
                {
                    BtnOpeartion[i].SetActive(false);
                }
            }
        }
    }

}
