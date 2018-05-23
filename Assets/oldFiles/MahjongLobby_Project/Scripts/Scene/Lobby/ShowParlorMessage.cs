using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ShowParlorMessage : MonoBehaviour
    {
        public Text Area; //显示麻将馆的地区
        public Text ParlorName; //麻将馆的名字
        public Text ParlorMemberCount; //麻将馆的成员的数量
        public Text ParlorMonthActivity;  //麻将馆的月活跃度
        public RawImage BossImage; //麻将馆馆主的头像
        public GameObject[] ParlorMessage; //0表示创建麻将馆  1表示麻将馆详细信息
        public Image ParlorStatus; //麻将馆的状态
        public Sprite[] ParlorStatus_Spr; //0表示已加入 1表示已申请
        public NetMsg.ParlorInfoDef InfoDef = new NetMsg.ParlorInfoDef();  //保存这个馆对应的详细信息

        public ParlorShowPanelData.PointIdOrNameGetParlorMessage InfoDef_Search = new ParlorShowPanelData.PointIdOrNameGetParlorMessage();
        public int status;  //1表示点击查看麻将馆的详情，2表示点击进入自己加入的麻将馆

        public Image add;  //加号
        public Sprite[] Add_s;  //加号图片

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="statusq">0不显示  1表示已加入 2表示已申请</param>
        public void UpdateShowStatus(int statusq)
        {
            if (statusq == 0)
            {
                ParlorStatus.gameObject.SetActive(false);
            }
            else
            {
                ParlorStatus.gameObject.SetActive(true);
                ParlorStatus.sprite = ParlorStatus_Spr[statusq - 1];
            }
        }

        /// <summary>
        /// 更新麻将馆按钮的名称
        /// </summary>
        /// <param name="info"></param>
        public void UpdateParlorBtn(NetMsg.ParlorInfoDef info)
        {
            if (info == null)
            {
                NetMsg.ParlorInfoDef temp = new NetMsg.ParlorInfoDef();
                temp.iParlorId = 0;
                InfoDef = temp;
                status = 0;
            }
            else
            {
                add.gameObject.SetActive(false);
                InfoDef = info;
                string name = "";
                if (info.szParlorName != null && info.szParlorName.Length > 5)
                {
                    name = info.szParlorName.Substring(0, 4) + "...";
                }
                else
                {
                    name = info.szParlorName;
                }
                ParlorName.text = name;
                status = 2;
            }
        }

        public int status_parlor = 0;  //0表示无需求  1表示已加入 2表示已申请     
        //更新对应麻将馆的信息
        public void UpdateShow(NetMsg.ParlorInfoDef info)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            ParlorStatus.gameObject.SetActive(false);
            status_parlor = 0;

            if (pspd.isShowParlorRoundPanel)
            {
                for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                {
                    if (pspd.parlorInfoDef[i] != null && info.iParlorId == pspd.parlorInfoDef[i].iParlorId)
                    {
                        status_parlor = 1;
                        break;
                    }
                }

                for (int i = 0; i < pspd.ApplyParlorId_All.Length; i++)
                {
                    if (info.iParlorId == pspd.ApplyParlorId_All[i])
                    {
                        status_parlor = 2;
                        break;
                    }
                }
            }


            if (status_parlor == 0)
            {
                ParlorStatus.gameObject.SetActive(false);
            }
            else
            {
                ParlorStatus.gameObject.SetActive(true);
                ParlorStatus.sprite = ParlorStatus_Spr[status_parlor - 1];
            }


            if (info.iParlorId == 0 && info.iBossId == 1)
            {
                ParlorMessage[0].SetActive(true);
                ParlorMessage[1].SetActive(false);
                return;
            }
            else
            {
                ParlorMessage[1].SetActive(true);
                ParlorMessage[0].SetActive(false);
            }
            InfoDef = info;
            string name = anhui.MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
            if (name.Length < 3)
            {
                Area.fontSize = 30;
                Area.lineSpacing = 1f;
            }
            else
            {
                Area.fontSize = 25;
                Area.lineSpacing = 0.85f;
            }
            Area.text = name;
            ParlorName.text = info.szParlorName;
            ParlorMemberCount.text = (info.iMemberNum).ToString();
            ParlorMonthActivity.text = info.iMonthVitality.ToString();
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(BossImage, info.szBossHeadimgurl);
        }

        /// <summary>
        /// 点击广场中的麻将馆信息
        /// </summary>
        public void BtnParlorGodParlor()
        {
            if (status == 1)
            {
                //发送该馆的申请详情
                NetMsg.ClientCAncelApplyOrJudgeApplyTooReq msg = new NetMsg.ClientCAncelApplyOrJudgeApplyTooReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iParlorId = InfoDef.iParlorId;
                msg.iType = 1;
                Network.NetworkMgr.Instance.LobbyServer.SendClientCAncelApplyOrJudgeApplyTooReq(msg);
                GameData.Instance.ParlorShowPanelData.InfoDef_PointParlor = InfoDef;
            }
        }



        /// <summary>
        /// 点击对应的麻将馆,查看详细信息
        /// </summary>        
        public void BtnLeftParlor(Toggle tog)
        {
            //改变颜色
            if (!tog.isOn)
            {
                transform.GetChild(1).GetComponent<Text>().color = new Color(0.17f, 0.78f, 0.87f, 1);
                if (status == 0)
                {         
                    transform.GetChild(1).GetComponent<Text>().text = "";
                    add.gameObject.SetActive(true);
                    add.sprite = Add_s[0];
                }
                else
                {
                    add.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                transform.GetChild(1).GetComponent<Text>().color = new Color(0.565f, 0.35f, 0.08f, 1);
                if (status == 0)
                {
                    transform.GetChild(1).GetComponent<Text>().text = "";
                    add.gameObject.SetActive(true);
                    add.sprite = Add_s[1];
                }
                else
                {
                    add.gameObject.SetActive(false);
                }

                if (status == 2)
                {
                    //打开麻将馆信息界面
                    UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.TabelGame);
                }
                else if (status == 0)
                {
                    if (GameData.Instance.ParlorShowPanelData.RefreshCount > 0)
                    {
                        UIMainView.Instance.ParlorShowPanel.OpenRefresh();
                    }
                    //在这里重新请求该县的麻将馆信息(暂时关闭麻将广场的界面信息)
                    //GameData.Instance.ParlorShowPanelData.FromWebGetParlorMessage(1, 5);
                    UIMainView.Instance.ParlorShowPanel.ShowPointPanel(MainViewParlorShowPanel.ParlorPanel.ParlorGod);
                    //直接显示策划指定信息
                    UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.NoParlorMessage(1);
                }

                //进入麻将馆查看详情
                if (status == 2)
                {
                    ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                    pspd.iParlorId = InfoDef.iParlorId;
                    pspd.isShowMyParlorMessage = true;
                    UIMainView.Instance.ParlorShowPanel.RemoveAllListener();
                    UIMainView.Instance.ParlorShowPanel.InitAllBtn_Parlor();
                    if (InfoDef.iStatus == 0)
                    {
                        UIMainView.Instance.ParlorShowPanel.BtnParlorTabelGame(true, 0);
                    }
                    SystemMgr.Instance.ParlorShowSystem.UpdateMyParlor(InfoDef);

                    pspd.iCityId[2] = InfoDef.iCityId;
                    pspd.iCountyId[2] = InfoDef.iCountyId;

                    //隐藏老板的操作按钮
                    int count = UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).childCount;
                    for (int i = 1; i < count - 1; i++)
                    {
                        UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
                    }
                    //开启普通成员的操作按钮
                    UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.transform.GetChild(1).GetChild(count - 1).gameObject.SetActive(true);
                    if (FindObjectOfType<ParlorTabelGameMessage>())
                    {
                        FindObjectOfType<ParlorTabelGameMessage>().Member.SetActive(false);
                    }
                    SystemMgr.Instance.ParlorShowSystem.UpdateShow();
                }
            }            
        }


        public void BtnCreatParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MainViewParlorShowPanel.MESSAGE_CREATPARLOR);
        }
    }

}
