using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class OrderRoomTime : MonoBehaviour
    {
        public GameObject[] Pick;
        public Text[] DownTimer; //倒计时        
        public float Timer;

        void Start()
        {
            Pick[0].SetActive(true);
            Pick[1].SetActive(false);
        }

        void OnDisable()
        {
            if (Temp != null)
            {
                StopCoroutine(Temp);
            }
        }

        //关闭协成
        public void StopIeNum()
        {
            if (Temp != null)
            {
                StopCoroutine(Temp);
            }
        }


        IEnumerator Temp;  //保存协成信息
        public void UpdateShow(float timer)
        {
            Timer = timer;
            if (timer <= 0)
            {
                if (Temp != null)
                {
                    StopCoroutine(Temp);
                }
            }
            TimeSpan timspan = new TimeSpan(0, 0, (int)timer);
            string tim = ((int)timspan.TotalHours).ToString("00") + ":" + timspan.Minutes.ToString("00") + ":" + timspan.Seconds.ToString("00");
            for (int i = 0; i < 2; i++)
            {
                DownTimer[i].text = tim;
            }
            Temp = OneSecondeDelay();
            StartCoroutine(Temp);
        }

        IEnumerator OneSecondeDelay()
        {
            yield return new WaitForSeconds(1f);
            Timer -= 1;
            if (Timer <= 0)
            {
                Timer = 0;
                //修改房间信息
                anhui.MahjongCommonMethod.Instance.PlayerRoomStatus = 0;
                UIMainView.Instance.LobbyPanel.OpenRoom[0].SetActive(true);
                UIMainView.Instance.LobbyPanel.OpenRoom[1].SetActive(false);
                GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel = false;
                GameData.Instance.LobbyMainPanelData.isJoinedRoom = false;
                gameObject.SetActive(false);
                yield break;
            }
            UpdateShow(Timer);
        }

        /// <summary>
        /// 点击返回房间
        /// </summary>
        public void BtnReturnRoom()
        {
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_RETURNROOM);
        }

        /// <summary>
        /// 点击展开/收起时间展示界面
        /// </summary>
        /// <param name="type">0表示收起 1表示放下</param>
        public void BtnDownOrUp(int type)
        {
            if (type == 1)
            {
                StartCoroutine(FiveSceondDelay());
            }

            Pick[type].SetActive(true);
            Pick[1 - type].SetActive(false);
        }

        IEnumerator FiveSceondDelay()
        {
            yield return new WaitForSeconds(5f);
            BtnDownOrUp(0);
        }
    }

}
