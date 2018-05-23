using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class NewPlayerGuide : MonoBehaviour
    {

        #region 单例
        static NewPlayerGuide instance;
        public static NewPlayerGuide Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        #endregion

        public GameObject[] PlayerGuide;  //新手引导的界面

        //引导的枚举
        public enum Guide
        {
            ShareTowX, //分享按钮    
        }

        /// <summary>
        /// 打开某个新手引导
        /// </summary>
        /// <param name="index"></param>
        public void OpenIndexGuide(Guide guide)
        {
            int index = (int)guide;
            PlayerGuide[index].SetActive(true);
        }


        /// <summary>
        ///  隐藏某个新手引导
        /// </summary>
        /// <param name="guide">新手引导对应的枚举</param>
        /// <param name="statsu">0表示把时间保存到注册表，大于0表示把该值保存到注册表</param>
        public void HideIndexGuide(Guide guide,int statsu)
        {
            int index = (int)guide;
            if (!PlayerGuide[index].gameObject.activeInHierarchy)
            {
                return;
            }
            PlayerGuide[index].SetActive(false);
            //隐藏指定新手引导之后，修改指定的引导的对应的注册表的时间
            if(statsu==0)
            {
                float timer = anhui.MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
                PlayerPrefs.SetFloat(guide.ToString(), timer);
            }
            else
            {
                PlayerPrefs.SetInt(guide.ToString(), statsu);
            }
            
        }



        /// <summary>
        /// 固定时间隐藏某个新手引导
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="guide"></param>
        public void SetTimeHideGuide_Ie(float timer, Guide guide, int status)
        {
            StartCoroutine(SetTimeHideGuide(timer, guide, status));
        }

        /// <summary>
        /// 固定时间隐藏某个新手引导
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        IEnumerator SetTimeHideGuide(float timer, Guide guide,int status)
        {
            yield return new WaitForSeconds(timer);
            HideIndexGuide(guide,status);
        }
        /// <summary>
        /// 发送玩家的认证请求
        /// </summary>
        public void SendPlayerAutnenReq(float timer)
        {
            StartCoroutine(DelaySendAuthenReq(timer));
        }

        //延迟发送认证请求
        IEnumerator DelaySendAuthenReq(float timer)
        {
            yield return new WaitForSeconds(timer);
            MahjongGame_AH.Network.Message.NetMsg.ClientAuthenReq msg_au = new MahjongGame_AH.Network.Message.NetMsg.ClientAuthenReq();
            msg_au.wVer = MahjongLobby_AH.LobbyContants.SeverVersion;
            msg_au.iAuthenType = 3;
            msg_au.iUserId = anhui.MahjongCommonMethod.Instance.iUserid;  // MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId;
                                                                    //Debug.LogError("szAccessToken:" +PlayerPrefs.GetString("szaccess_token"));

            msg_au.szToken = anhui.MahjongCommonMethod.Instance.accessToken;
            msg_au.szDui = SystemInfo.deviceUniqueIdentifier;
            msg_au.szIp = anhui.MahjongCommonMethod.PlayerIp;
        //    MahjongLobby_AH.SDKManager.Instance.GetIP(() => { });
        //    Debug.LogWarning("2设备IP：" + msg_au.szIp);

            if (anhui.MahjongCommonMethod.Instance.isMoNiQi)
            {
                msg_au.fLongitude = 0;
                msg_au.fLatitude = 0;
                msg_au.szAddress = "";
            }
            else
            {
                msg_au.fLatitude = anhui.MahjongCommonMethod.Instance.fLatitude;
                msg_au.fLongitude = anhui.MahjongCommonMethod.Instance.fLongitude;
                msg_au.szAddress= anhui.MahjongCommonMethod.Instance.sPlayerAddress;
            }
            msg_au.iRegistSource = MahjongLobby_AH.LobbyContants.iChannelVersion;
            msg_au.szRegistMac = anhui.MahjongCommonMethod.Instance.MacId;
            MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.SendAuthenReq(msg_au);
        }

    }
}

