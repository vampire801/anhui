using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MahjongLobby_AH.Data;
using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System.Collections;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewHolidayActivityPanel : MonoBehaviour
    {
        [Serializable]
        public struct ActivePanel
        {
            public GameObject _gPanel;
            public Button btnClose;
            public CDKPanel cdk;
            /// <summary>
            /// _btn0-分享  1-跳转
            /// </summary>
            public Button[] _btnChil;
            public Image imageRP;
            public ChilePanel[] childPanel;
            public Text _tBtnDis;
            public Text _tContent;
            public Text _tShare;
            /// <summary>
            /// 活动标题
            /// </summary>
            public Text _tTitel;
            /// <summary>
            /// 活动副标题
            /// </summary>
            public Text _tTitel2;
            /// <summary>
            /// 活动图片
            /// </summary>
            public RawImage _raw;
            public GameObject _gPanelChil;
        }
        [Serializable]
        public struct ChilePanel
        {
            public int ID;
            public Image _red;
            public Toggle _toggleChil;//页签按钮
            public Text[] _tTogTitel;
        }
        [Serializable]
        public struct CDKPanel
        {
            public Image ImageRed;
            public Toggle ToggleCDK;
            public GameObject _gCP;
            public Button btnMessage;
            public InputField input;
            public Button btnDuihuan;
            public Button btnCloseMessage;
            public GameObject _gMP;
        }
        public ActivePanel activePanel;
        public const string MESSAGE_CLOSE = "MianViewHolidayActivityPanel.MESSAGE_CLOSE";
        public const string MESSAGE_RECEIVE = "MianViewHolidayActivityPanel.MESSAGE_RECEIVE";
        public const string MESSAGE_CHOICEACTIVITY = "MianViewHolidayActivityPanel.MESSAGE_CHOICEACTIVITY";
        public const string MESSAGE_BTNGETGIFT = "MianViewHolidayActivityPanel.MESSAGE_BTNGETGIFT";
        void Start()
        {
            AddListener();
        }
        /// <summary>
        /// 初始化面板
        /// </summary>
        void Init()
        {
            //子面板开启
            activePanel.childPanel[0]._toggleChil.isOn = true;
        }
        void AddListener()
        {
            activePanel.btnClose.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Init();
                activePanel._gPanel.SetActive(false);
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            });
            activePanel.cdk.ToggleCDK.onValueChanged.AddListener((b) =>
          {
              SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
              activePanel.cdk._gCP.SetActive(b);//打开面板
              GameData.Instance.HolidayActivityPanelData.SetIsRed(-2017, false, activePanel.cdk.ImageRed.gameObject);
              activePanel.cdk.ToggleCDK.transform.GetChild(0).gameObject.SetActive(b);
              activePanel.cdk.ToggleCDK.transform.GetChild(1).gameObject.SetActive(!b);
          });
            activePanel.cdk.btnMessage.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                activePanel.cdk._gMP.SetActive(true);
            });
            activePanel.cdk.btnCloseMessage.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                activePanel.cdk._gMP.SetActive(false);
            });
            activePanel.cdk.btnDuihuan.onClick.AddListener(() =>
          {
              SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
              BtnGetGift();
          });
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
        /// 更新活动面板
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            HolidayActivityPanelData hapd = gd.HolidayActivityPanelData;
            if (hapd.isPanelShow)
            {
                gameObject.SetActive(true);
                updateRedPoint();//刷新红点
                GameData.Instance.isShowQuitPanel = false;
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }
        }
        /// <summary>
        /// 更新对应活动面板
        /// </summary>
        /// <param name="index">面板ID</param>
        ///  <param name="isOnlyUpdateButton">是否只更新按钮状态</param>
        public void UpdateShowById(int index, bool isOnlyUpdateButton)
        {

        }

        #region 按钮方法//活动分享内容 
        void OnShare(GameObject obj)
        {
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            int id = 0;
            id = int.Parse(EventTriggerListener.Get(obj).str);
            Debug.LogError("分享活动ID:" + id);

            foreach (CommonConfig.Data_Activity d in hapd.jsActive.data)
            {
                Debug.LogError("ACTIVITY_ID" + d.ACTIVITY_ID);

                if (d.ACTIVITY_ID == id)
                {

                    int type = 2 - d.SHARE_TYPE;
                    Debug.LogError("分享类型：" + type);
                    Debug.LogWarning("SHARE_FRIEND_TITLE:" + d.SHARE_FRIEND_TITLE + "SHARE_FRIEND_CONTENT:" + d.SHARE_FRIEND_CONTENT + "SHARE_MOMENTS_CONTENT；" + d.SHARE_MOMENTS_CONTENT);
                    string content = type == 0 ? d.SHARE_FRIEND_CONTENT : d.SHARE_MOMENTS_CONTENT;
                    string title = type == 1 ? content : d.SHARE_FRIEND_TITLE;                    
                    SDKManager.Instance.HandleShareWX(SDKManager.WXInviteUrl + 0, title, content, type, 12, id,"");//分享发送分享  朋友圈 活动类 活动id\
                }
            }
        }
        void OnOpenWeb(GameObject obj)
        {
            string url = EventTriggerListener.Get(obj).str;
            Debug.LogError(url);
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
        }
        void updateRedPoint()
        {
            HolidayActivityPanelData hd = GameData.Instance.HolidayActivityPanelData;
            activePanel.cdk.ImageRed.gameObject.SetActive(hd.GetIsRed(-2017));
            if (hd.jsActive.data == null)
            {
                return;
            }
            for (int i = 0; i < hd.jsActive.data.Length; i++)
            {
                //Debug.Log(hd.jsActive.data[i].ACTIVITY_ID + "是否显示" + hd.GetIsRed(hd.jsActive.data[i].ACTIVITY_ID));
                activePanel.childPanel[i]._red.gameObject.SetActive(hd.GetIsRed(hd.jsActive.data[i].ACTIVITY_ID));
            }
        }
        #endregion 按钮方法
        public void GetActivityJson()
        {
            //Debug.LogError("------------");
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            if (hapd.jsActive.data != null && hapd.jsActive.data.Length > 0 && hapd.jsActive.data[0].ACTIVITY_ID > 0)
            {
                DealCallBack();
                return;
            }

            string ssUrl = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
            anhui.MahjongCommonMethod.Instance.GetUrlJson(string.Format(ssUrl + HolidayActivityPanelData.URL, UnityEngine.Random.Range(0, 10)), hapd.jsActive,
                (a, b) =>
                {
                    if (a.status == 1)//成功
                    {
                        hapd.jsActive = a;
                        SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                        //Debug.LogError("-----成功1");
                        DealCallBack();
                        //Debug.LogError("-----成功2");
                    }
                    else
                    {
                        // MahjongCommonMethod.Instance.ShowRemindFrame("没有活动哦！");
                    }
                });
        }

        /// <summary>
        /// 获取活动json回调
        /// </summary>
        /// <param name="a"></param>
        void DealCallBack()
        {
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            #region 
            int num = hapd.jsActive.data.Length;
            if (num > 0)
            {
                activePanel._gPanelChil.SetActive(true);
            }
            for (int i = 0; i < num; i++)
            {
                int index = i;
                if (hapd.jsActive.data[i].SHOW_FLAG > 0)
                {
                    //Debug.Log(a.data[i  ].ACTIVITY_ID );
                    GameData.Instance.HolidayActivityPanelData.stackID.Push(hapd.jsActive.data[i]);
                }
                activePanel.childPanel[i]._toggleChil.gameObject.SetActive(true);
                activePanel.childPanel[i]._toggleChil.onValueChanged.AddListener((bool b) =>
               {
                   #region 如果点击
                   if (b)//点击刷新面板信息
                   {
                       SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                       hapd.SetIsRed(hapd.jsActive.data[index].ACTIVITY_ID, false, activePanel.childPanel[index]._red.gameObject);
                       if (hapd.jsActive.data[index].ACTIVITY_TYPE == 2)//文字
                       {
                           activePanel._raw.gameObject.SetActive(false);//            1 图片消失                         
                           activePanel._tTitel.text = hapd.jsActive.data[index].TITLE2;//         2 标题
                           activePanel._tTitel2.text = hapd.jsActive.data[index].TITLE3;//        3 副标题
                           activePanel._tContent.text = hapd.jsActive.data[index].DESCRIPTION;//  4 描述

                           activePanel._tTitel.gameObject.SetActive(true);
                           activePanel._tTitel2.gameObject.SetActive(true);
                           activePanel._tContent.gameObject.SetActive(true);
                       }
                       else//图片活动
                       {
                           activePanel._raw.gameObject.SetActive(true);
                           activePanel._tTitel.gameObject.SetActive(false);
                           activePanel._tTitel2.gameObject.SetActive(false);
                           activePanel._tContent.gameObject.SetActive(false);
                           anhui.MahjongCommonMethod.Instance.GetWebImage (activePanel._raw, LobbyContants.ActivitePic + hapd.jsActive.data[index].IMG_URL, (f) =>
                           {
                               if (f)//加载成功
                               {
                                   activePanel._raw.gameObject.SetActive(true);
                               }
                               else
                                   Debug.LogError("加载活动图片出错");
                           });
                       }
                       //外链接
                       if (!string.IsNullOrEmpty(hapd.jsActive.data[index].REDIRECT_URL) && hapd.jsActive.data[index].REDIRECT_URL.Length > 5)
                       {
                           //Debug.LogError("REDIRECT_URL  " + a.data[index].REDIRECT_URL);
                           EventTriggerListener.Get(activePanel._btnChil[1].gameObject,
                             anhui.MahjongCommonMethod.Instance.GetUrlEncrypt(hapd.jsActive.data[index].REDIRECT_URL,GameData.Instance.PlayerNodeDef.iUserId)
                               ).onClick = OnOpenWeb;
                           activePanel._tBtnDis.text = hapd.jsActive.data[index].BUTTON_CHAR;
                           activePanel._btnChil[1].gameObject.SetActive(true);
                           activePanel._btnChil[0].transform.localPosition = new Vector3(-153, activePanel._btnChil[0].transform.localPosition.y, activePanel._btnChil[0].transform.localPosition.z);
                       }
                       else
                       {
                           activePanel._btnChil[0].transform.localPosition = new Vector3(0, activePanel._btnChil[0].transform.localPosition.y, activePanel._btnChil[0].transform.localPosition.z);
                           activePanel._btnChil[1].gameObject.SetActive(false);
                       }
                       if (hapd.jsActive.data[index].RP12_FLAG == 1)//有分享红包
                       {
                           // activePanel._btnChil[0].gameObject.SetActive (true);
                           if (PlayerPrefs.HasKey("activePanel.imageRP" + hapd.jsActive.data[index].ACTIVITY_ID))
                           {
                               activePanel.imageRP.gameObject.SetActive(false);
                               activePanel._tShare.text = "分享";
                           }
                           else
                           {
                               activePanel.imageRP.gameObject.SetActive(true);
                               activePanel._tShare.text = "  分享领红包";
                           }
                           EventTriggerListener.Get(activePanel._btnChil[0].gameObject, hapd.jsActive.data[index].ACTIVITY_ID + "").onClick = OnShare;
                       }
                       else//无分享红包
                       {
                           activePanel._tShare.text = "分享";
                           activePanel.imageRP.gameObject.SetActive(false);
                           EventTriggerListener.Get(activePanel._btnChil[0].gameObject, hapd.jsActive.data[index].ACTIVITY_ID + "").onClick = OnShare;
                       }
                   }
                   #endregion 如果点击
                   else
                   {

                   }
                   activePanel.childPanel[index]._toggleChil.transform.GetChild(0).gameObject.SetActive(b);
                   activePanel.childPanel[index]._toggleChil.transform.GetChild(1).gameObject.SetActive(!b);
               });//以上添加点击事件内容
                activePanel.childPanel[i].ID = hapd.jsActive.data[i].ACTIVITY_ID;
                //改变页签文字
                for (int k = 0; k < 2; k++)
                {
                    activePanel.childPanel[i]._tTogTitel[k].text = hapd.jsActive.data[i].TITLE;
                }
            }//以上for循环
            #endregion
            Init();
            Debug.LogWarning(PlayerPrefs.HasKey("FistActivity"));
            updateRedPoint();
            if (PlayerPrefs.HasKey("FistActivity"))
            {
                //   Debug.LogWarning(PlayerPrefs.GetInt("FistActivity")+"++++"+ DateTime.Now.DayOfYear);

                if (PlayerPrefs.GetInt("FistActivity") == DateTime.Now.DayOfYear)
                {
                    return;
                }
                else
                {
                    PlayerPrefs.SetInt("FistActivity", DateTime.Now.DayOfYear);
                }
            }
            else
            {
                PlayerPrefs.SetInt("FistActivity", DateTime.Now.DayOfYear);
            }
            ShowPanel();
            
        }
        Stack stackID = new Stack();
        void ShowPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;
            if (GameData.Instance.HolidayActivityPanelData.stackID.Count > 0)
            {
                CommonConfig.Data_Activity ad = (CommonConfig.Data_Activity)GameData.Instance.HolidayActivityPanelData.stackID.Pop();
                UIMainView.Instance.LobbyPanel.activeShow._gPanel.SetActive(true);
                Debug.Log(ad.ACTIVITY_TYPE);
                if (ad.ACTIVITY_TYPE > 1)//文字
                {
                    //在这里将btn上添加背景图片                    
                    UIMainView.Instance.LobbyPanel.activeShow._raw.gameObject.SetActive(false);
                    UIMainView.Instance.LobbyPanel.activeShow._tTitel.text = ad.TITLE2;
                    UIMainView.Instance.LobbyPanel.activeShow._tTitel2.text = ad.TITLE3;
                    UIMainView.Instance.LobbyPanel.activeShow._tContent.text = ad.DESCRIPTION;
                }
                else
                {
                    UIMainView.Instance.LobbyPanel.activeShow._raw.gameObject.SetActive(true);
                    Debug.Log("图片地址：" + LobbyContants.ActivitePic + ad.IMG_URL);
                    anhui.MahjongCommonMethod.Instance.GetWebImage (UIMainView.Instance.LobbyPanel.activeShow._raw, LobbyContants.ActivitePic + ad.IMG_URL);
                }

                UIMainView.Instance.LobbyPanel.activeShow.btnClose.onClick.RemoveAllListeners();
                UIMainView.Instance.LobbyPanel.activeShow.btnClose.onClick.AddListener(() => { ShowPanel(); });
                EventTriggerListener.Get(UIMainView.Instance.LobbyPanel.activeShow.btnTurnToActivity.gameObject, ad.ACTIVITY_ID + "").onClick = OpenMainViewH;
            }
            else
            {
                UIMainView.Instance.LobbyPanel.activeShow._gPanel.SetActive(false);
            }
        }
        //首页活动跳转到活动面板
        void OpenMainViewH(GameObject obj)
        {
            GameData.Instance.HolidayActivityPanelData.stackID.Clear();
            UIMainView.Instance.LobbyPanel.activeShow._gPanel.SetActive(false);
            int id = int.Parse(EventTriggerListener.Get(UIMainView.Instance.LobbyPanel.activeShow.btnTurnToActivity.gameObject).str);
            //Debug.LogError("----===="+id);
            UIMainView.Instance.LobbyPanel.BtnActivity();
            for (int i = 0; i < activePanel.childPanel.Length; i++)
            {
                if (activePanel.childPanel[i].ID == id)
                {
                    activePanel.childPanel[i]._toggleChil.isOn = true;
                }
            }
        }

        #region 按扭
        /// <summary>
        /// 显示时间
        /// </summary>
        /// <param name="index">时间Text序号</param>
        public void ShowActivityTime(int index)
        {
            HolidayActivityPanelData hapd = GameData.Instance.HolidayActivityPanelData;

            PlayerNodeDef pnd = GameData.Instance.PlayerNodeDef;

        }

        public void BtnGetGift()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (activePanel.cdk.input.text.Length != 10)
            {
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("您输入的激活码码不正确，请重新输入");
                return;
            }
            Messenger_anhui<string>.Broadcast(MESSAGE_BTNGETGIFT, activePanel.cdk.input.text);
        }
        /// <summary>
        /// 领取礼包按钮
        /// </summary>
        /// <param name="index">1-日赠卡 2节日赠卡</param>
        public void OnBtnReceiveGift(int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui<int>.Broadcast(MESSAGE_RECEIVE, index);
        }
        #endregion 按扭
    }



}
