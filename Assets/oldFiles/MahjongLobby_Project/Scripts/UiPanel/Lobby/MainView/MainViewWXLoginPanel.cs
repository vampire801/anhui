using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MahjongLobby_AH.Data;
using System;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System.Runtime.InteropServices;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewWXLoginPanel : MonoBehaviour
    {
        public GameObject NetDisConnect;  //网络异常或者服务器异常的提示框
        public Text version;  //版本号
        public Button btn_LoginWX;//微信登录按钮
        public Button btn_youkeLogin;
        public Transform trans_Login;
        public const string MESSAGE_WXLOGINAUTHBTN = "MainViewWXLoginPanel.MESSAGE_WXLOGINAUTHBTN";//微信登录认证        
        public const string MESSAGE_BTNOK = "MainViewLobbyPanel.MESSAGE_BTNOK";  //当网络异常时，会自动点击确定按钮会自动重连服务器


        void Awake()
        {
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                GameData.Instance.WXLoginPanelData.isPanelShow = true;
            }

            version.text = "版本号：" + LobbyContants.version_v + "." + LobbyContants.iChannelVersion + LobbyContants.version_typr;

#if UNITY_IOS
          if (SDKManager.Instance.iShowGuestLogin == 1 && SDKManager.Instance.IOSCheckStaus == 1)
           // if(true)
            {
                btn_youkeLogin.gameObject.SetActive(true);
                btn_youkeLogin.gameObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(288, 104);
            }
            else
            {                               
                btn_youkeLogin.gameObject.SetActive(false);
            }
#elif UNITY_ANDROID||UNITY_EDITOR
            if (LobbyContants.iShowGuestLogin == 1)
            {
                btn_youkeLogin.gameObject.SetActive(true);

            }
            else
            {
                btn_youkeLogin.gameObject.SetActive(false);
            }
#endif
        }
        [DllImport("__Internal")]
        private static extern bool IsHaveWeChat();
        void Start()
        {

#if UNITY_IPHONE
             if (SDKManager.Instance.IOSCheckStaus==1)
            {
               // bool t = IsHaveWeChat();
                //Debug.LogError("+++++:  " + t);
                btn_LoginWX.gameObject.SetActive(false);
              //  if (!t)
               // {
                    btn_youkeLogin.transform.localPosition = trans_Login.localPosition;
               // }
            }
            else
            {
                btn_LoginWX.gameObject.SetActive(true);
            }                  
#endif

        }

        public void UpdateShow()
        {
            if (GameData.Instance.WXLoginPanelData.isPanelShow)
            {
                gameObject.SetActive(true);
                if (SDKManager.Instance.IsDisConnect)
                {
                    NetDisConnect.SetActive(true);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }


            if (GameData.Instance.WXLoginPanelData.isCloseDisConnectPanel)
            {
                NetDisConnect.SetActive(true);
            }
            else
            {
                NetDisConnect.SetActive(false);
            }
        }


        #region 按钮点击      

        /// <summary>
        /// 游客登陆按钮
        /// </summary>
        public void BtnGusetLogin()
        {
            WXLoginPanelData wlpd = GameData.Instance.WXLoginPanelData;
            if (!wlpd.isAgreUserRule)
            {
                UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.DISAGREEUSERDEAL);
                return;
            }
            if (wlpd.isClickLogin)
            {
                return;
            }
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            NetMsg.ClientAuthenReq msg = new NetMsg.ClientAuthenReq();
            msg.wVer = LobbyContants.SeverVersion;
            Debug.LogWarning("LobbyContants.SeverVersion:" + LobbyContants.SeverVersion
                + ",msg.wVer:" + msg.wVer);
            msg.iAuthenType = 4;
            MahjongCommonMethod.iAutnehType = 4;
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            //msg.szDui = "c7wwfhfjb7dfgfhjd734sgdf34il8edej";
            msg.szIp = MahjongCommonMethod.PlayerIp;
          //  SDKManager.Instance.GetIP(() => {  });
        //    Debug.LogWarning("3设备IP：" + msg.szIp);

            if (MahjongCommonMethod.Instance.isMoNiQi)
            {
                msg.fLongitude = 0;
                msg.fLatitude = 0;
                msg.szAddress = " ";
            }
            else
            {
                msg.fLatitude = MahjongCommonMethod.Instance.fLatitude;
                msg.fLongitude = MahjongCommonMethod.Instance.fLongitude;
                msg.szAddress = MahjongCommonMethod.Instance.sPlayerAddress;
            }
            //Debug.LogError("经纬度:" + msg.fLatitude + "," + msg.fLongitude);
            msg.iRegistSource = LobbyContants.iChannelVersion;
            msg.szRegistMac = MahjongCommonMethod.Instance.MacId;
            msg.szRegistImei = "NOIMEI";
            msg.REGISTRATION_ID = SDKManager.Instance.GetRegistID();

            NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);




            wlpd.isClickLogin = true;
        }


        public void BtnOk()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNOK);
        }
        /// <summary>
        /// 微信登录按钮
        /// </summary>
        public void BtnWXLogin()
        {
            GameData gd = GameData.Instance;
            WXLoginPanelData wxlpd = gd.WXLoginPanelData;
            if (!wxlpd.isAgreUserRule)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("请确认并同意用户协议");
                return;
            }

            if (!wxlpd.isBtnOk)
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui.Broadcast(MESSAGE_WXLOGINAUTHBTN);
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");
                StartCoroutine(OpenLoginButton());
                wxlpd.isBtnOk = true;
            }
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.WXLogin);
           
        }




        IEnumerator delaylogin()
        {
            yield return new WaitForSeconds(1.5f);
            //延迟发送游客登陆
            NetMsg.ClientAuthenReq msg = new NetMsg.ClientAuthenReq();
            msg.wVer = LobbyContants.SeverVersion;
            msg.iAuthenType = 4;
            MahjongCommonMethod.iAutnehType = 4;
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            msg.szIp = MahjongCommonMethod.PlayerIp;
           // SDKManager.Instance.GetIP(() => { });
          //  Debug.LogWarning("4设备IP：" + msg.szIp);

            msg.fLatitude = MahjongCommonMethod.Instance.fLatitude;
            msg.fLongitude = MahjongCommonMethod.Instance.fLongitude;
            msg.szAddress = MahjongCommonMethod.Instance.sPlayerAddress;
            //Debug.LogError("经纬度:" + msg.fLatitude + "," + msg.fLongitude);
            msg.iRegistSource = LobbyContants.iChannelVersion;
            msg.szRegistMac = MahjongCommonMethod.Instance.MacId;
            msg.REGISTRATION_ID = SDKManager.Instance.GetRegistID();
            NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);
        }

        IEnumerator OpenLoginButton()
        {
            yield return new WaitForSeconds(13f);
            Debug.LogError("13秒后调用");
            GameData.Instance.WXLoginPanelData.isBtnOk = false;
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
        }

        /// <summary>
        /// 点击勾选用户协议
        /// </summary>
        public void BtnChioceXieyi(Toggle tog)
        {
            GameData gd = GameData.Instance;
            WXLoginPanelData wxlpd = gd.WXLoginPanelData;
            if (tog.isOn)
            {
                wxlpd.isAgreUserRule = true;
            }
            else
            {
                wxlpd.isAgreUserRule = false;
            }
        }
        #endregion 按钮点击

    }
}
