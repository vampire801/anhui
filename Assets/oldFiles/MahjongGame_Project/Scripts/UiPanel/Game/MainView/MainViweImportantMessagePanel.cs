using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using MahjongGame_AH;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;
namespace anhui
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViweImportantMessagePanel : MonoBehaviour
    {
        public Text TimeText;//在多长时间后开始

        [HideInInspector]
        public string StartTime;//还有多长时间
        Action RunFuntion;

        public void BtnClose()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 打开此面板
        /// </summary>
        /// <param name="WaitTime_"></param>
        /// <param name="hanshu"> 执行什么方法</param>
        public void OnOpen(int WaitStartTime_, uint WaitLostTime_, Action hanshu)
        {
            gameObject.SetActive(true);
            StartTime = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(WaitStartTime_, 0).AddMinutes(WaitLostTime_)).ToString("HH:mm");//开始时间//(MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongLobby_AH.UIMainView.Instance.PlayerPlayingPanel.CreatRoomTime, 0).AddMinutes(UIMainView.Instance.PlayerPlayingPanel.WaitTimeForStartGame)).ToString("HH:mm");        
            RunFuntion = hanshu;
            TimeText.text = "游戏将在" + StartTime + "分开始";
        }

        public void BtnYuYue()
        {
            RunFuntion();
            BtnClose();
        }

        public void OnUpDateShowRoomLateTime(int time)
        {
            if (time > 0)
            {
                TimeText.text = StartTime + "分开始";
            }
            else
            {
                BtnClose();
            }
        }
    }
}

