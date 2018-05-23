using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewProductAgencyPanel : MonoBehaviour
    {
        public InputField Input;  //输入id的框
        public GameObject[] ShowPanel;  //要显示的面板
        public GameObject CompanyMessage; //公司联系方式面板
        public Text Id;  //上级代理的id
        public Text Name;  //上级代理的昵称
        public RawImage Head;  //上级代理的头像
        public Text AgencyMessage;  //上级代理的留言
        public Text wx;  //微信号
        public GameObject UnbindSecondSure;  //申请解绑的二次确认框

        #region 常量
        public const string MESSAGE_CLOSEBTN = "MainViewProductAgencyPanel.MESSAGE_CLOSEBTN";  //关闭按钮
        public const string MESSAGE_AGENCYBTN = "MainViewProductAgencyPanel.MESSAGE_AGENCYBTN";  //点击绑定按钮
        public const string MESSAGE_CHANGEAGENCYBTN = "MainViewProductAgencyPanel.MESSAGE_COMINGSOONBTN"; //点击成为代理按钮       
        public const string MESSAGE_APPLYUNBIND = "MainViewProductAgencyPanel.MESSAGE_APPLYUNBIND";  //申请解绑按钮
        public const string MESSAGE_SUREUNBIND = "MainViewProductAgencyPanel.MESSAGE_SUREUNBIND";  //确认解除绑定
        public const string MESSAGE_CANELUNBIND = "MainViewProductAgencyPanel.MESSAGE_CANELUNBIND"; //取消解除绑定
        public const string MESSAGE_BTNCLOSECHANGEAGENC = "MainViewProductAgencyPanel.MESSAGE_BTNCLOSECHANGEAGENC"; //点击关闭公司信息的按钮
        public const string MESSAGE_BTNCOPYWXNUMBER = "MainViewProductAgencyPanel.MESSAGE_BTNCOPYWXNUMBER";  //点击复制微信号按钮
        #endregion 常量


        void Start()
        {
            //关闭代理的新手引导
            if(PlayerPrefs.GetFloat(NewPlayerGuide.Guide.JoinAgency.ToString())==0)
            {
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.JoinAgency);
            }
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
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
            ProductAgencyPanelData papd = gd.ProductAgencyPanelData;
            if(papd.PanelShow)
            {
                Input.text = "";
                ShowIndexPanel(papd.index);
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
                if (papd.IsShowCompany)
                {
                    CompanyMessage.SetActive(true);
                    for(int i=0;i<ShowPanel.Length;i++)
                    {
                        ShowPanel[i].SetActive(false);
                    }
                }
                else
                {
                    CompanyMessage.SetActive(false);
                    ShowIndexPanel(papd.index);
                }

                if(papd.isShowSecondUnbind)
                {
                    UnbindSecondSure.SetActive(true);
                }
                else
                {
                    UnbindSecondSure.SetActive(false);
                }
                
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 显示对应的面板
        /// </summary>
        /// <param name="index">面板的数组下标</param>
        void ShowIndexPanel(int index)
        {
            for(int i=0;i< ShowPanel.Length;i++)
            {
                if(i==index)
                {               
                    ShowPanel[i].SetActive(true);
                    if(i==1)
                    {
                        if(GameData.Instance.ProductAgencyPanelData.szNickname==null)
                        {
                            NetMsg.ClientProxyInfoReq msg = new NetMsg.ClientProxyInfoReq();
                            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                            //msg.iProxyId = GameData.Instance.PlayerNodeDef.iProxyId;                                                        
                            //NetworkMgr.Instance.LobbyServer.SendProxyInfoReq(msg);
                        }
                        //Id.text = "ID："+GameData.Instance.PlayerNodeDef.iProxyId.ToString();                        
                        Name.text= GameData.Instance.ProductAgencyPanelData.szNickname;
                        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(Head, GameData.Instance.ProductAgencyPanelData.szHeadimgurl);
                        AgencyMessage.text = GameData.Instance.ProductAgencyPanelData.szProxyComment;
                        wx.text= GameData.Instance.ProductAgencyPanelData.wx;
                    }
                }
                else
                {
                    ShowPanel[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
        }

        /// <summary>
        /// 点击绑定按钮
        /// </summary>
        public void BtnAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (Input.text.Length != 8)
            {
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("代理id输入错误，请检查之后重新输入");
                return;
            }
            Messenger_anhui<string>.Broadcast(MESSAGE_AGENCYBTN,Input.text);
        }




        /// <summary>
        /// 点击成为代理按钮
        /// </summary>
        public void BtnChangeAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CHANGEAGENCYBTN);
        }


        /// <summary>
        /// 点击申请解绑按钮
        /// </summary>
        public void BtnUnBind()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            for (int i=0;i<ShowPanel.Length;i++)
            {
                ShowPanel[i].SetActive(false);
            }
            UnbindSecondSure.SetActive(true);
        }


        /// <summary>
        /// 点击确认申请解绑
        /// </summary>
        public void BtnSureSecondUnbind()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_APPLYUNBIND);
        }

        /// <summary>
        /// 点击取消申请解绑
        /// </summary>
        public void BtnCanelSecondUnbind()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            UnbindSecondSure.SetActive(false);
            UpdateShow();
        }

        /// <summary>
        /// 点击关闭公司信息面板
        /// </summary>
        public void BtnCloseChangeAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSECHANGEAGENC);
        }

        /// <summary>
        /// 点击复制微信号按钮
        /// </summary>
        /// <param name="text"></param>
        public void BtnCopyWxNumber(Text text)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui<Text>.Broadcast(MESSAGE_BTNCOPYWXNUMBER,text);
        }

    }

}
