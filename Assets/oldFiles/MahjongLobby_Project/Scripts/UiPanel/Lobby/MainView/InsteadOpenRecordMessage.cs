using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using System;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class InsteadOpenRecordMessage : MonoBehaviour
    {
        public GameObject[] PlayerMessage;  //0表示玩家的信息，1表示房间未开启成功的信息

        public Text RoomId;  //房间的id
        public Text[] Score;  //玩家的分数
        public RawImage[] headImage;  //玩家的头像
        public Text PayRoomNum;  //房卡支付数量
        public Text EndTime; //游戏结束时间
        public Text GameStatus;  //房间状态，分为解散，结束，关闭
        public Text DissloveTime;  //解散时间        
        public Image RoomColorFlag;  //房间的颜色标记  
        public Sprite[] RoomColorImage;  //房间的颜色标记的对应图片 0表示红色，1表示黄色，2表示蓝色，3表示绿色
        public string[] color = new string[2] { "739359FF", "AF3F14FF" };  //二种颜色的值

        

        //保存该条记录的信息
        public InsteadOpenRoomPanelData.InsteadOpenRoomMessage RoomMessage = new InsteadOpenRoomPanelData.InsteadOpenRoomMessage();

        public int index;  //下标

        //更新该条记录的界面w
        public void ShowInsteadRecord()
        {

            if (Convert.ToInt16(RoomMessage.color) == 0)
            {
                RoomId.text = "<color=#95654BFF>"+ RoomMessage.roomNum + "</color>";
            }
            else
            {
                RoomId.text = "<color=#FFFFFFFF>" + RoomMessage.roomNum + "</color>";
            }
            
         
            PayRoomNum.text = RoomMessage.payCard;

            EndTime.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime
                    (Convert.ToDouble(RoomMessage.openTim), 0).ToString("yyyy-MM-dd HH:mm");

            if (Convert.ToInt16(RoomMessage.disType) <=2)
            {
                PlayerMessage[0].SetActive(false);
                PlayerMessage[1].SetActive(true);
                DissloveTime.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime
                    (Convert.ToDouble(RoomMessage.disTim), 0).ToString("yyyy-MM-dd HH:mm");
				GameStatus.text = "<color=#cf410a>已关闭</color>";
            }
            else
            {
                PlayerMessage[1].SetActive(false);
                PlayerMessage[0].SetActive(true);
                if(Convert.ToInt16(RoomMessage.disType) == 3)
                {
					GameStatus.text = "<color=#cf410a>已解散</color>";
                }
                else
                {
					GameStatus.text = "<color=#569424>已结束</color>";
                }
                
                //更新玩家的头像和分数
                for(int i=0;i<4;i++)
                {
                    //分数
                    Score[i].text = RoomMessage.point[i].ToString();
                    //头像
                    anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(headImage[i], RoomMessage.headUrl[i]);
                }

            }

            //更新房间的颜色显示
            RoomColorFlag.sprite = RoomColorImage[Convert.ToInt16(RoomMessage.color)];
        }


    }

}
