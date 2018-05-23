using UnityEngine;
using System.Text;
using System.Collections;
using UnityEngine.UI;
using System;
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ScoreMessage : MonoBehaviour
    {
        public GameObject[] DayOrMessage; //保存天数或者Message信息  1显示信息
        public Text Today; //显示昨天 还是 今天   
        public Text Timer; //时间
        public Image OpenRoomImage; //开房人的标示  
        public Text RoomMessage; //房间信息
        public Text PayOrGet; //支出或者收入
        public Text CurrentScore; //当前积分

        public ParlorShowPanelData.ParlorBossScoreLog info = new ParlorShowPanelData.ParlorBossScoreLog();

        public void UpdateShow(ParlorShowPanelData.ParlorBossScoreLog info)
        {
            if (Convert.ToInt32(info.userId) == 0)
            {
                DayOrMessage[0].SetActive(true);
                DayOrMessage[1].SetActive(false);
                Today.text = info.tim;
            }
            else
            {
                DayOrMessage[0].SetActive(false);
                DayOrMessage[1].SetActive(true);
                Timer.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToInt32(info.tim), 0).ToString("HH:mm");
                if (GameData.Instance.PlayerNodeDef.iUserId == Convert.ToInt32(info.userId))
                {
                    OpenRoomImage.gameObject.SetActive(false);
                }
                else
                {
                    OpenRoomImage.gameObject.SetActive(true);
                }

                StringBuilder str = new StringBuilder();
                if (Convert.ToDouble(info.assetNum) > 0)
                {
                    str.Append("兑换_");
                    switch (info.assetType)
                    {
                        case "1":
                            str.Append("_现金_");
                            str.Append(info.assetNum);
                            str.Append("元");
                            break;
                        case "2":
                            str.Append("_话费_");
                            str.Append(info.assetNum);
                            str.Append("元");
                            break;
                        case "3":
                            str.Append("_流量_");
                            if (Convert.ToInt32(info.assetNum) > 1000)
                            {
                                str.Append((Convert.ToDouble(info.assetNum) / 1000f).ToString("0.0"));
                                str.Append("G");
                            }
                            else
                            {
                                str.Append(info.assetNum);
                                str.Append("M");
                            }
                            break;
                        case "4":
                            str.Append("_储值卡_");
                            str.Append(info.assetNum);
                            str.Append("元");
                            break;
                        case "5":
                            str.Append("_代金券_");
                            str.Append(info.assetNum);
                            str.Append("元");
                            break;
                        case "6":
                            str.Append("_赠币_");
                            str.Append(info.assetNum);
                            break;
                    }

                    PayOrGet.text = "<color=#d81415>-" + Convert.ToDouble(info.score).ToString("0.00") + "</color>";
                }
                else
                {
                    str.Append("房间_");
                    str.Append(Convert.ToInt32(info.roomNum).ToString("d6"));
                    if (Convert.ToInt32(info.coin) > 0)
                    {
                        str.Append("_金币");
                        str.Append(info.coin);
                        str.Append("赠币");
                        str.Append(info.coin3);
                    }
                    else
                    {
                        str.Append("_赠币");
                        str.Append(info.coin3);
                    }

                    PayOrGet.text = "<color=#558E47FF>+" + Convert.ToDouble(info.score).ToString("0.00") + "</color>";
                }
                RoomMessage.text = str.ToString();
                CurrentScore.text = Convert.ToDouble(info.banlance).ToString("0.00");
            }
        }

    }

}
