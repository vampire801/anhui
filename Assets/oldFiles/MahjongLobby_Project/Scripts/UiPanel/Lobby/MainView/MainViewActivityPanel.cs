using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewActivityPanel : MonoBehaviour
    {
        public const string MESSAGE_BTNCLOSE = "MainViewActivityPanel.MESSAGE_BTNCLOSE";  //关闭按钮

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 面板的更新显示
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            ActivityPanelData apd = gd.ActivityPanelData;
            if(apd.PanelShow)
            {
                GameData.Instance.isShowQuitPanel = false;
                gameObject.SetActive(true);
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSE);
        }
    }

}
