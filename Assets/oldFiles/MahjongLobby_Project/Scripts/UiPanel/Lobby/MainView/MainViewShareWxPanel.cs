using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewShareWxPanel : MonoBehaviour
    {

        public Text InvitationCode;//好友邀请码
        public GameObject NewPlayerGetGoldPanel;//新手发金币界面
        public GameObject QRCode;//二维码界面
        public GameObject LiaoJieXiangQing;//了解详情

        public const string MESSAGE_CLOSEBTN = "MainViewShareWxPanel.Message_CloseBtn";   //关闭微信分享面板按钮
        /// <summary>
        /// 0分享到微信消息按钮1 分享到微信朋友圈按钮
        /// </summary>
        public const string MESSAGE_SHAREWX = "MainViewShareWxPanel.Message_ShareWx";
        public const string MESSAGE_GETMONY = "MainViewShareWxPanel.MESSAGE_GETMONY";
        public const string MESSAGE_NEWPLAYERGOLD = "MainViewShareWxPanel.MESSAGE_NEWPLAYERGOLD";
        public const string MESSAGE_PROMOTIONGOLD = "MainViewShareWxPanel.MESSAGE_PROMOTIONGOLD";
        // public Text CodeKey_0;  //激活码表示         
        // public Text codeKey_1;  //激活码表示
        
        public Button m_iQRCodeButton;//二维码按钮
        public Text TuiGuanNum;//推广人数
                              
        RawImage m_rQRCode;

        void Start()
        {
            //获取玩家的推广码，并保存
            GameData gd = GameData.Instance;
            ShareWxPanelData swpd = gd.ShareWxPanelData;
            swpd.CodeKey = gd.VerifyCode(gd.PlayerNodeDef.iUserId);
            UpdateShow();

            //如果该玩家还没有操作过分享按钮，则请求次数
            if(PlayerPrefs.GetFloat(NewPlayerGuide.Guide.ShareToWx_2.ToString())==0)
            {
                //请求玩家分享次数
                NetMsg.ClientShareReq msg = new NetMsg.ClientShareReq();
                Network.NetworkMgr.Instance.LobbyServer.SendShareReq(msg);                
            }
            //加载加载所需要使用的二维码图片  
            OnLoadLoadQRCode();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新面板显示
        /// </summary>

        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            ShareWxPanelData swpd = gd.ShareWxPanelData;
            if(swpd.PanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;

                //Debug.LogError("面板显示");
                TuiGuanNum.text = GameData.Instance.PlayerNodeDef.userDef.AllRpNum.ToString() + "个";
                // CodeKey_0.text = "您的专属邀请码：" + swpd.CodeKey;
                // codeKey_1.text = "“"+swpd.CodeKey+"”";
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
            //Debug.LogError("---close");
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);

        }

        /// <summary>
        /// 点击分享到微信消息的按钮
        /// </summary>
        public void BtnShareWx()
        {
            Messenger_anhui<string >.Broadcast(MESSAGE_SHAREWX,"0,10");
        }


        /// <summary>
        /// 点击分享到微信朋友圈的按钮
        /// </summary>
        public void BtnShareWxFriends()
        {
            Messenger_anhui<string >.Broadcast(MESSAGE_SHAREWX,"1,10");
        }

        /// <summary>
        /// 填写邀请码领金币
        /// </summary>
        public void BtnGetInvitationCodeMoney()
        {
            Messenger_anhui.Broadcast(MESSAGE_GETMONY);
        }
        /// <summary>
        /// 新手金币
        /// </summary>
        public void BtnGetNewPlayerGold()
        {
            Messenger_anhui.Broadcast(MESSAGE_NEWPLAYERGOLD);
        }
        /// <summary>
        /// 推广金币
        /// </summary>
        public void BtnGetPromotionGold()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_PROMOTIONGOLD);
        }

        public void BtnCloseNewPlayerGetGoldPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            NewPlayerGetGoldPanel.SetActive(false);
        }


        /// <summary>
        /// 了解详情
        /// </summary>
        public void Btn_liaojiexiangqing()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            LiaoJieXiangQing.SetActive(true);
            //string str ="1、凡是有红包标记的分享，分享后即可获得红包！\n2、凡是好友通过您的分享成为新用户，就会送您红包，无上限！\n3、凡是推荐的好友参与游戏，您也有机会拿红包，游戏越多机会越多！\n4、凡是由您分享出去的内容：战绩、邀请、活动…等等都有机会获得红包哦！";
            //UIMgr.GetInstance().GetUIMessageView().Show(str);
        }

        /// <summary>
        /// 查看二维码
        /// </summary>
        public void Btn_chakanerweima()
        {
            QRCode.SetActive(true);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
           // Debug.LogError(QRCode.transform.GetChild(1).GetComponent<RawImage>().name);
            RawImage image = QRCode.transform.GetChild(1).GetComponent<RawImage>();
            image.texture = m_rQRCode.texture;            
        }

        public void BtnCloseQRCode()
        {
            QRCode.SetActive(false);
        }

        public void BtnCloseLiaojeixiangqing()
        {
            LiaoJieXiangQing.SetActive(false);
        }

        #region 加载游戏下载的二维码下载地址

        /// <summary>
        /// 加载二维码
        /// </summary>
        public void OnLoadLoadQRCode()
        {
            m_rQRCode = UIMainView.Instance.ShareWxPanel.QRCode.transform.GetChild(1).GetComponent<RawImage>();
            string str = LobbyContants.MAJONG_PORT_URL + "userShareQr.x?uid=" + GameData.Instance.PlayerNodeDef.iUserId;
            StartCoroutine(LoadQRCode(str));
        }

        IEnumerator LoadQRCode(string path)
        {
            while (true)
            {
                WWW www = new WWW(path);
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError("------二维码异常-----:" + www.error);
                    // raw.texture = Resources.Load<Texture>("icon");
                    yield return new WaitForSeconds(1);
                    continue;
                }

                while (!www.isDone)
                {
                    yield return 0;
                }
                yield return www;

                if (www.text.Length == 0)
                {
                    //  Debug.LogError("------二维码异常-----" + www.text.Length);
                    yield return new WaitForSeconds(1);
                    continue;
                }
                else
                {
                    //为图片赋值
                    if (www.texture)
                    {
                        m_rQRCode.texture = www.texture;

                        RawImage image = m_iQRCodeButton.GetComponent<RawImage>();
                        image.texture = m_rQRCode.texture;
                        break;
                    }
                    else
                    {
                        yield return new WaitForSeconds(1);
                        continue;
                    }
                    //开启截图
                }
            }
        }

        #endregion
    }
}
