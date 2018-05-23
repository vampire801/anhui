using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongLobby_AH.Data;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorGameRecord : MonoBehaviour
    {
        public Text RoomNum; //房间号
        public Image RoomNumBg; //房间号的背景
        public Sprite[] Bg; //背景 1标示白色背景 2标示红色老板背景
        public Text GameStatus; //1未开始游戏房主主动解散，2未开始游戏时间到了自动解散，3游戏中一局内玩家解散，4游戏中一局后玩家解散,5全部游戏结束解散,6未到预约时间房主解散 7GM强制解散-
        public Text EndTime; //游戏结束时间
        public Text DissolveTime; //解散时间
        public RawImage[] HeadImage; //四个玩家的头像        
        public Image[] OpenRoomSign; //房主的标示  如果是馆主开房不显示
        public Sprite[] OpenRoomSign_Spr; //房间表示图片  1表示创建房间 2表示解散发起人
        public Text[] Score;  //四个玩家的分数
        public Text PayCoinNum; //支付的金币数量
        public GameObject[] PlayerMessage; //1表示开始了游戏显示四个玩家的信息  2表示未成功开启
        //保存该条信息附带的信息
        public ParlorShowPanelData.GetPointParlorGameRecordData info = new ParlorShowPanelData.GetPointParlorGameRecordData();

        public void UpdateShow(ParlorShowPanelData.GetPointParlorGameRecordData Info)
        {
            info = Info;

            int[] temp = new int[] { 0, 0, 0, 0 };  //决定是否显示创建信息

            //显示创建者的信息
            if (Convert.ToInt32(Info.uid) == Convert.ToInt32(Info.uid1))
            {
                temp[0] = 1;
            }

            if (Convert.ToInt32(Info.uid) == Convert.ToInt32(Info.uid2))
            {
                temp[1] = 1;
            }

            if (Convert.ToInt32(Info.uid) == Convert.ToInt32(Info.uid3))
            {
                temp[2] = 1;
            }

            if (Convert.ToInt32(Info.uid) == Convert.ToInt32(Info.uid4))
            {
                temp[3] = 1;
            }

            if (int.Parse(Info.uid) == GameData.Instance.PlayerNodeDef.iUserId)
            {
                RoomNum.text = "<color=#FFFFFFFF>" + Info.roomNum + "</color>";
                RoomNumBg.sprite = Bg[1];
            }
            else
            {
                RoomNum.text = "<color=#b28f73>" + Info.roomNum + "</color>";
                RoomNumBg.sprite = Bg[0];
            }

            //显示分数
            Score[0].text = Info.point1;
            Score[1].text = Info.point2;
            Score[2].text = Info.point3;
            Score[3].text = Info.point4;

            //显示头像
            MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[0], Info.head1);
            MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[1], Info.head2);
            MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[2], Info.head3);
            MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[3], Info.head4);



            //显示时间
            EndTime.text = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(int.Parse(Info.disTim), 0).ToString("yyyy_MM_dd HH:mm");

            if (int.Parse(Info.card) > 0)  //正常结束
            {
                //关闭头像显示
                PlayerMessage[0].SetActive(true);
                PlayerMessage[1].SetActive(false);
                int type = int.Parse(Info.disType);
                if (type != 5)
                {
                    GameStatus.text = "<color=#cf420a>已解散</color>";
                    //显示解散人的标示
                    if (Convert.ToInt32(Info.disUserId) == Convert.ToInt32(Info.uid1))
                    {
                        temp[0] = 2;
                    }

                    if (Convert.ToInt32(Info.disUserId) == Convert.ToInt32(Info.uid2))
                    {
                        temp[1] = 2;
                    }


                    if (Convert.ToInt32(Info.disUserId) == Convert.ToInt32(Info.uid3))
                    {
                        temp[2] = 2;
                    }

                    if (Convert.ToInt32(Info.disUserId) == Convert.ToInt32(Info.uid4))
                    {
                        temp[3] = 2;
                    }
                }
                else
                {
                    GameStatus.text = "<color=#569424FF>已结束</color>";
                    for (int i = 0; i < 4; i++)
                    {
                        OpenRoomSign[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                GameStatus.text = "<color=#cf420a>已关闭</color>";
                //关闭头像显示
                PlayerMessage[0].SetActive(false);
                PlayerMessage[1].SetActive(true);
                //显示结束时间
                //Debug.LogError("解散时间:" + Info.disTim);
                DissolveTime.text = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(int.Parse(Info.disTim), 0).ToString("yyyy_MM_dd HH:mm") + "关闭";
                PayCoinNum.text = "0";
            }

            for (int i = 0; i < 4; i++)
            {
                if (temp[i] > 0)
                {
                    OpenRoomSign[i].gameObject.SetActive(true);
                    OpenRoomSign[i].sprite=OpenRoomSign_Spr[temp[i]-1];
                }
                else
                {
                    OpenRoomSign[i].gameObject.SetActive(false);
                }
            }

            PayCoinNum.text = Info.card;

        }
    }

}
