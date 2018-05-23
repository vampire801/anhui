using UnityEngine;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewCustomSeverPanel : MonoBehaviour
    {
        [Serializable]
        public class PanelUI_Custom
        {
            /// <summary>
            /// 面板id 0-联系客服 1-bug提交 2-玩法收集 3-关注公众号 4-常见问题
            /// </summary>
            public int iPanelID;
            public Toggle toggle_Tag;
            //面板0-联系客服  1-Bug 2-玩法收集 3-关注公众号 4-常见问题
            public PanelChild childPanel;
        }
        [Serializable]
        public class PanelChild
        {
            public GameObject _gPanel;
            public Text[] t;
            public InputField[] _input;
            public Button btn;
            public Text textContent;
            public FAQ[] faq4;
            public GameObject[] _i3;
        }
        [Serializable]
        public class FAQ
        {
            public Text textHead;
            public Toggle tgl3;
        }
        public int PointIndex; //指定显示默认显示哪个面板
        #region 总面板UI
        public PanelUI_Custom[] panel_Custom;
        #endregion 总面板UI

        #region 子面板UI
        // public Transform []trans_movePos=new Transform [3];//移动位置
        public Button btn_window;
        public GameObject gameObj_ShowGroup;
        //三个点
        public Toggle[] toggle_point = new UnityEngine.UI.Toggle[3];
        #endregion 子面板UI

        //public GameObject[] ShowContent;  //面板要显示的内容
        //public Text[] IntrouceText; //面板介绍的内容0表示正常版，1表示审核版
        //public GameObject[] Btn; //审核要隐藏的按钮
        //public GameObject MethodCollectPanel;  //玩法收集面板
        //public GameObject CustomSeverPanel;  //客服面板
        //public Text _textios;//
        //public GameObject iosNeedUDis;
        public const string MESSAGE_CLOSEBTN = "MainViewBugSubmitPanel.MESSAGE_CLOSEBTN";  //点击关闭按钮
        public const string MESSAGE_WHATWX = "MainViewBugSubmitPanel.MESSAGE_WHATWX"; //点击如何关注公众号
        public const string MESSAGE_BTNMETHODCOLLECT = "MainViewBugSubmitPanel.MESSAGE_BTNMETHODCOLLECT"; //点击玩法收集按钮
        public const string MESSAGE_BTNCUSTOMSEVER = "MainViewBugSubmitPanel.MESSAGE_BTNCUSTOMSEVER"; //点击客服按钮

        void Start()
        {
            for (int i = 0; i < panel_Custom.Length; i++)
            {
                PanelUI_Custom pc = panel_Custom[i];
                pc.iPanelID = i;
                pc.toggle_Tag.onValueChanged.AddListener((bool isOn) =>
                {
                    pc.toggle_Tag.transform.GetChild(0).gameObject.SetActive(isOn);
                    pc.toggle_Tag.transform.GetChild(1).gameObject.SetActive(!isOn);
                    pc.childPanel._gPanel.transform.SetAsLastSibling();
                });
            }
            //Debug.LogError("=====================================0");
            AddLisonerForPanelBtn();
            init();
        }
        public void init()
        {
            Debug.Log ("指定显示某个面板:"+ PointIndex);
            panel_Custom[0].toggle_Tag.isOn = false;
            panel_Custom[PointIndex].toggle_Tag.isOn = true;
        }
        [Serializable]
        public class BugFFF
        {
            public string userid;
            public string text;
            public string phone;
            public BugFFF()
            {

            }
            public BugFFF(string U, string t, string p)
            {
                userid = U;
                text = t;
                phone = p;
            }
        }

        /// <summary>
        /// 为面板按钮添加事件
        /// </summary>
        public void AddLisonerForPanelBtn()
        {
            panel_Custom[0].childPanel.btn.onClick.AddListener(() =>
           {
               panel_Custom[3].toggle_Tag.isOn = true;
           });
            GameData.Instance.CustomPanelData.oriPos = gameObj_ShowGroup.transform.localPosition.x;
            EventTriggerListener.Get(btn_window.gameObject).onDown =SystemMgr.Instance.CustomSystem . OnPointerDown;
            EventTriggerListener.Get(btn_window.gameObject).onUp =SystemMgr.Instance.CustomSystem. OnPointerUp;
            panel_Custom[1].childPanel.btn.onClick.AddListener(() =>
            {
                Regex reg = new Regex("^1[3|4|5|7|8][0-9]{9}$");
                //提交bug
                if (panel_Custom[1].childPanel._input[0].text.Length <= 0 || !reg.IsMatch( panel_Custom[1].childPanel._input[1].text))
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("您输入的手机号或描述有误，请重新输入。");
                }
                else
                {
                    //string ss="http://192.168.1.21:8888/sx_interface/";
                    string ss = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
                    string sss = panel_Custom[1].childPanel._input[0].text;
                    MahjongCommonMethod.Instance.GetUrlJson(
                   string.Format(ss + "Feedback.x?type={0}&user_id={1}&context={2}&phone={3}", 16, GameData.Instance.PlayerNodeDef.iUserId, Uri.EscapeDataString(sss), panel_Custom[1].childPanel._input[1].text),"", 
                   (a, b) =>
                   {
                       Regex regex = new Regex(@"[1]");
                       Debug.LogError(regex.IsMatch(b));
                       if (regex.IsMatch(b))
                       {
                           MahjongCommonMethod.Instance.ShowRemindFrame("成功提交！谢谢参与，我们会尽快处理！");
                           panel_Custom[1].childPanel._input[0].text = "";
                           panel_Custom[1].childPanel._input[1].text = "";
                       }
                       else
                       {
                           regex = new Regex(@"[2]");
                           if (regex.IsMatch(b))
                           {
                               panel_Custom[1].childPanel._input[0].text = "";
                               panel_Custom[1].childPanel._input[1].text = "";
                               UIMgr.GetInstance().GetUIMessageView().Show("非常感谢您对我们产品的支持，您今日已提交3次宝贵的建议，若还有宝贵建议请联系客服。", () => {
                               }, () => {
                                   init();
                               }, 0, 2);
                           }
                           else
                           {
                               MahjongCommonMethod.Instance.ShowRemindFrame("提交失败！请再试一次。。。");
                           }
                       }
                   });
                }
            });
            panel_Custom[2].childPanel.btn.onClick.AddListener(() =>
            {
                Regex reg = new Regex("^1[3|4|5|7|8][0-9]{9}$");
                //_input[1]--Phone  _input[0]--context
                //if (panel_Custom[2].childPanel._input[0].text.Length <= 0 || panel_Custom[2].childPanel._input[1].text.Length < 11)
                if (!reg.IsMatch(panel_Custom[2].childPanel._input[1].text)|| panel_Custom[2].childPanel._input[0].text.Length <= 0)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("您输入的手机号或描述有误，请重新输入。");
                }
                else
                {
                    string sss = panel_Custom[2].childPanel._input[0].text;
                    //提交玩法
                    string surl = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
                    MahjongCommonMethod.Instance.GetUrlJson(
                   string.Format(surl + "Feedback.x?type={0}&user_id={1}&context={2}&phone={3}", 15,
                   GameData.Instance.PlayerNodeDef.iUserId, Uri.EscapeDataString(sss), panel_Custom[2].childPanel._input[1].text)
                  ,
                   ""
                   , (a, b) =>
                   {
                       Regex regex = new Regex(@"[1]");
                       if (regex.IsMatch(b))
                       {
                           MahjongCommonMethod.Instance.ShowRemindFrame("成功提交！谢谢参与，我们会尽快处理！");
                           panel_Custom[2].childPanel._input[0].text = "";
                           panel_Custom[2].childPanel._input[1].text = "";
                       }
                       else
                       {
                           regex = new Regex(@"[2]");
                           if (regex.IsMatch(b))
                           {
                               panel_Custom[2].childPanel._input[0].text = "";
                               panel_Custom[2].childPanel._input[1].text = "";
                               UIMgr.GetInstance().GetUIMessageView().Show("非常感谢您对我们产品的支持，您今日已提交3次宝贵的建议，若还有宝贵建议请联系客服。", () => {
                               }, () => {
                                   init();
                               }, 0, 2);
                           }
                           else
                           {
                               MahjongCommonMethod.Instance.ShowRemindFrame("提交失败！请再试一次。。。");
                           }
                       }
                   });
                }

            });
            //FAQ
            for (int i = 0; i < MahjongCommonMethod.Instance._cfgFAQ.num; i++)
            {
                panel_Custom[4].childPanel.faq4[i].textHead.text = MahjongCommonMethod.Instance._cfgFAQ.data[i].title;
                int mm = i;
                panel_Custom[4].childPanel.faq4[i].tgl3.onValueChanged.AddListener((a) =>
                {
                    ActionByClick(a, mm);
                });
            }
        }
        void ActionByClick(bool a, int mm)
        {
            if (a)
            {
                for (int j = 0; j < panel_Custom[4].childPanel._i3.Length; j++)
                {
                    panel_Custom[4].childPanel._i3[j].SetActive(true);
                }
                panel_Custom[4].childPanel.textContent.text = MahjongCommonMethod.Instance._cfgFAQ.data[mm].content;
                //判断_i[]是不是在所点击的toggle之上
                if (panel_Custom[4].childPanel._i3[0].transform.GetSiblingIndex() < panel_Custom[4].childPanel.faq4[mm].tgl3.transform.GetSiblingIndex())
                {
                    for (int j = panel_Custom[4].childPanel._i3.Length - 1; j >= 0; j--)
                    {
                        panel_Custom[4].childPanel._i3[j].transform.SetSiblingIndex(panel_Custom[4].childPanel.faq4[mm].tgl3.transform.GetSiblingIndex());
                    }
                }
                else
                {
                    for (int j = panel_Custom[4].childPanel._i3.Length - 1; j >= 0; j--)
                    {
                        panel_Custom[4].childPanel._i3[j].transform.SetSiblingIndex(panel_Custom[4].childPanel.faq4[mm].tgl3.transform.GetSiblingIndex() + 1);
                    }
                }
            }
            else
            {
                //判断_i【0】是不是在设置falseToggle之后一个
                if (panel_Custom[4].childPanel._i3[0].transform.GetSiblingIndex() == panel_Custom[4].childPanel.faq4[mm].tgl3.transform.GetSiblingIndex() + 1)
                {
                    for (int j = 0; j < panel_Custom[4].childPanel._i3.Length; j++)
                    {
                        panel_Custom[4].childPanel._i3[j].SetActive(false);
                    }
                }
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
            if (isDraging)
            {
                // gDragDis = v3currentPos.x - v3begignPos.x;
            }
        }
        /// <summary>
        /// 当前图片
        /// </summary>
        int i_currentPic;
        bool isDraging;
        //  Vector3 v3_begignPos;
        Vector3 v3_currentPos;
        float gDragDis;
        /// <summary>
        /// 面板的更新显示
        /// </summary>
        /// 

        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            CustomPanelData bspd = gd.CustomPanelData;
            if (bspd.PanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;

            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        #region Button
        public void BtnOnPanelClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            for (int j = 0; j < panel_Custom[4].childPanel._i3.Length; j++)
            {
                panel_Custom[4].childPanel._i3[j].SetActive(false);
            }
            panel_Custom[PointIndex].toggle_Tag.isOn = true;
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
        }
        /// <summary>
        /// 关闭如何关注微信公众号
        /// </summary>
        public void BtnCloseWhatWx()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CustomPanelData bspd = GameData.Instance.CustomPanelData;
            bspd.isPanelWtWx = false;
            Messenger_anhui.Broadcast(MESSAGE_WHATWX);
        }
        public Text text_detal;
        /// <summary>
        /// 点击复制按钮
        /// </summary>
        public void BtnCopy(UnityEngine.UI.Text text)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.CopyString(text.text);
            //处理玩家复制成功之后提示文字
            MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }
        /// <summary>
        /// 点击玩法收集的按钮
        /// </summary>
        public void BtnMethodCollect()
        {
            Messenger_anhui.Broadcast(MESSAGE_BTNMETHODCOLLECT);
        }

        #endregion Button



    }


}
