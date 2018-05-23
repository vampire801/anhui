using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewAcitveGiftPanel : MonoBehaviour
    {

        public GameObject _gTips;


        public Text _textErr;
        // public const string MESSAGE_ASSIST = "MainViewActivePanel.MESSAGE_ASSIST";

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }


        public void UpdateShow()
        {
            ActiveGiftPanelData agpd = GameData.Instance.ActiveGiftPanelData;
            if (agpd.istipsShow)
            {
                GameData.Instance.isShowQuitPanel = false;
                _gTips.SetActive(true);
                _textErr.text = agpd.sTipsFiled;
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                _gTips.SetActive(false);
            }
        }



    }
}
