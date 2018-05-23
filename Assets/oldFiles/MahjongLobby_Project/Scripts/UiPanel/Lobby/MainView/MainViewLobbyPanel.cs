using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MahjongLobby_AH.Data;
using System;
using MahjongLobby_AH.LobbySystem.SubSystem;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using Spine.Unity;
using System.Text;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewLobbyPanel : MonoBehaviour
    {
        [Serializable]
        public class VoucherPanel
        {
            public GameObject _g;
            public ToggleGroup toggleGroup_VoucherGroup;
            public Image _image;
        }
        [Serializable]
        public class ActiveShow
        {
            public GameObject _gPanel;
            public int ActiveId;
            public Button btnClose;
            public Button btnTurnToActivity;

            /// <summary>
            /// 活动标题
            /// </summary>
            public Text _tTitel;
            /// <summary>
            /// 活动副标题
            /// </summary>
            /// 

            public Text _tTitel2;
            public Text _tContent;
            /// <summary>
            /// 活动图片
            /// </summary>
            public RawImage _raw;
        }
        [Serializable]
        public class LeftBtns
        {
            public Button btnChage;
            public Button btnFish;
            public GameObject _BtnRealName;
        }
        bool FlagWXAuthBtn;
        public LeftBtns Lobby_leftBtn;
        public WalletPanel panel_wallet;
        //子面板0-加群面板 
        public JoinUsPanel panels_joinUs;
        public PanelCharge panel_Charge;
        public ActiveShow activeShow;
        public VoucherPanel panel_voucher;
        /// <summary>
        /// 玩家头像
        /// </summary>
        public RawImage PlayerAvatar;
        /// <summary>
        /// 获赞数量
        /// </summary>
        public Text GetPraise;
        /// <summary>
        /// 房卡数量
        /// </summary>
        public Text RoomCardNum;

        /// <summary>
        /// 玩家的id编号
        /// </summary>
        public Text PlayerID;

        /// <summary>
        /// 用户昵称
        /// </summary>
        public Text Nickname;
        /// <summary>
        /// 公告的字体显示赌博，捕鱼奇兵宣测试测试
        /// </summary>
        public Text Bulltein_Text;

        //公告字体
        public GameObject Bulletein;

        /// <summary>
        /// 节日活动的按钮
        /// </summary>
        public GameObject FestivalActivityBtn;

        //  public GameObject IOSRoomCardChi;//送审IOS
        // public GameObject AndroidCardChi;
        public Sprite NoDailiTexture; //没有代理的显示图片
        /// <summary>
        /// 最后一排的按钮背景
        /// </summary>
        public Image[] BtnBg;

        /// <summary>
        /// 大厅下方所有按钮的图标 1-MemberCenter 2-agency 3-HistroyGrade 4-activity 5customSever
        /// </summary>
        public GameObject[] AllDownBtn;
        /// <summary>
        /// 送审屏蔽0 分享 1 实名 2捕鱼按钮 3首冲
        /// </summary>
        public GameObject[] shareBtn;

        //加我
        public GameObject _BtnJoinUs;
        public Image[] RedPoint;  //0表示头像红点，1表示消息红点，2表示战绩红点，3表示活动红点,4表示代理的红点
                                  //5表示实名注册的红点，6表示分享礼包的红点
                                  /// <summary>
                                  /// 审核隐藏按钮0表示地域的logo  1表示客服
                                  /// </summary>
        public GameObject[] CheckHideBtn;
        public GameObject[] ParlorAndJoinRoom; //麻将馆和加入房间按钮 0雀神广场按钮 1横向加入房间按钮  2纵向加入房间按钮

        /// <summary>
        /// 开启房间的数组，0表示开启房间，1是返回房间
        /// </summary>
        public GameObject[] OpenRoom;

        public Text AreaName;  //县的名称显示
        public GameObject NewPlayerBag;  //玩家的新手礼        
        public GameObject RedPage;//红包
        public Sprite[] DjqBg; //代金券背景
        public bool m_bInGame = false;//是不是站过做的
        int BtnShowCount;  //下方按钮不显示的数量
        [HideInInspector]
        public bool isDengluOnce = false;//是打开app发送信息
        float timer;  //时间
        public GameObject m_HealthyPrompt;//健康忠告

        public const string MESSAGE_REALNAME = "MainViewLobbyPanel.MESSAGE_REALNAME";   //处理实名认证按钮
        public const string MESSAGE_JOINUS = "MainViewLobbyPanel.MESSAGE_JOINUS";//加入群二维码
        public const string MESSAGE_SHARE = "MainViewLobbyPanel.MESSAGE_SHARE";  //处理分享按钮
        public const string MESSAGE_GETGIFTBAG = "MainViewLobbyPanel.MESSAGE_GETGIFTBAG";   //处理点击推广领取礼包按钮
        public const string MESSAGE_PALYINGMETHOD = "MainViewLobbyPanel.MESSAGE_PALYINGMETHOD";  //处理点击玩法
        public const string MESSAGE_PRODUCTAGENCY = "MainViewLobbyPanel.MESSAGE_PRODUCTAGENCY"; //处理点击代理按钮
        public const string MESSAGE_PRODUCTGENE = "MainViewLobbyPanel.MESSAGE_PRODUCTGENE";  //点击会员中心
        public const string MESSAGE_CUSTOMSEVERBTN = "MainViewLobbyPanel.MESSAGE_BUGSUBMITBTN"; //点击客服的按钮  
        public const string MESSAGE_PLAYERMESSAGE = "MainViewLobbyPanel.MESSAGE_PLAYERMESSAGE"; //点击消息按钮
        public const string MESSAGE_PLAYERFestivalActivity = "MainViewLobbyPanel.MESSAGE_PLAYERFestivalActivity"; //点击活动按钮
        public const string MESSAGE_HolidayACTIVITYBTN = "MainViewLobbyPanel.MESSAGE_HolidayACTIVITYBTN"; //点击节日活动按钮
        public const string MESSAGE_CREATROOM = "MainViewLobbyPanel.MESSAGE_CREATROOM"; //点击创建房间按钮
        public const string MESSAGE_RETURNROOM = "MainViewLobbyPanel.MESSAGE_RETURNROOM";  //点击返回房间按钮
        public const string MESSAGE_JOINROOM = "MainViewLobbyPanel.MESSAGE_JOINROOM";  //点击加入房间
        public const string MESSAGE_INSTEADCREATROOM = "MainViewLobbyPanel.MESSAGE_INSTEADCREATROOM";  //点击代开房间
        public const string MESSAGE_HISTROYGRADE = "MainViewLobbyPanel.MESSAGE_HISTROYGRADE";  //点击战绩按钮
        //public const string MESSAGE_MOREBTN = "MainViewLobbyPanel.MESSAGE_MOREBTN";  //点击更多按钮
        public const string MESSAGE_HEADTITLE = "MainViewLobbyPanel.MESSAGE_HEADTITLE";  //点击头像按钮   
        public const string MESSAGE_BUYROOMCARD = "MainViewLobbyPanel.MESSAGE_BUYROOMCARD"; //点击购买房卡按钮   
        public const string MESSAGE_CLOSEBUYROOMCARD = "MainViewLobbyPanel.MESSAGE_CLOSEBUYROOMCARD";  //点击关闭购买房卡界面
        public const string MESSAGE_BUYCARD = "MainViewLobbyPanel.MESSAGE_BUYCARD";//点击购买房卡下单按钮
        public const string MESSAGE_OPENPARLOR = "MainViewLobbyPanel.MESSAGE_CREATPARLOR"; //点击进入麻将馆      
        public const string MESSAGE_REDPAGE = "MainViewLobbyPanel.MESSAGE_REDPAGE";  //红包

        public const string MESSAGE_ClickAlipay = "AliPay";
        public const string MESSAGE_ClickWXpay = "WXPlay";

        #region Sort


        //  #endregion
        #endregion
        IEnumerator GetVoucher(string url, int amount)
        {
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            for (int i = 0; i < 3; i++)
            {
                WWW www = new WWW(url);
                Debug.LogWarning(url);
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    continue;
                }
                else
                {
                    Debug.LogWarning(www.text);
                    lmpd.vc = JsonUtility.FromJson<LobbyMainPanelData.Voucher>(www.text);
                    if (lmpd.vc.status != 1)
                    {
                        MahjongCommonMethod.Instance.ShowRemindFrame("网页数据错误");
                        yield break;
                    }
                    if (lmpd.vc.data.Length <= 0)
                    {
                        panel_voucher._image.gameObject.SetActive(true);
                    }
                    else
                    {
                        panel_voucher._image.gameObject.SetActive(false);

                    }
                    lmpd._dicVoucher.Clear();
                    for (int j = 0; j < lmpd.vc.data.Length; j++)
                    {
                        if (amount > 0)
                        {
                            if (MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) > ulong.Parse(lmpd.vc.data[j].exTime) || int.Parse(lmpd.vc.data[j].state) > 1)
                            { continue; }
                        }
                        LobbyMainPanelData.IVoucherData ivd = new LobbyMainPanelData.IVoucherData();
                        ivd.amount = int.Parse(lmpd.vc.data[j].amount);
                        ivd.exTime = int.Parse(lmpd.vc.data[j].exTime);
                        ivd.getTime = int.Parse(lmpd.vc.data[j].getTime);
                        ivd.limit = int.Parse(lmpd.vc.data[j].limit);
                        ivd.orderId = lmpd.vc.data[j].orderId;
                        ivd.state = int.Parse(lmpd.vc.data[j].state);
                        ivd.useTime = int.Parse(lmpd.vc.data[j].useTime);
                        ivd.voucherId = int.Parse(lmpd.vc.data[j].voucherId);
                        lmpd._dicVoucher.Add(ivd);
                    }
                    int chargeNum = 0;//
                    if (amount > 0)//如果是充值
                    {
                        if (stackprice.Count > 0)
                        {
                            NetMsg.ClientCreateOrderRes req = stackprice.Peek() as NetMsg.ClientCreateOrderRes;
                            if (req != null)
                            {
                                chargeNum = (stackprice.Peek() as NetMsg.ClientCreateOrderRes).iChargeNumber;
                            }
                        }
                    }
                    MahjongCommonMethod.Instance.StartGetTime(() =>
                    {
                        Debug.LogWarning(MahjongCommonMethod.Instance._DateTime);
                    });
                    yield return new WaitUntil(() =>
                    {
                        return MahjongCommonMethod.Instance.flagNewTime;
                    });

                    MahjongCommonMethod.Instance.flagNewTime = false;
                    SystemMgr.Instance.LobbyMainSystem.FindFirst(chargeNum / 100);//找到推介
                    lmpd._dicVoucher.Sort(SystemMgr.Instance.LobbyMainSystem.SortCompare);//排序
                    #region 删除预制体
                    Image[] objdd = panel_voucher.toggleGroup_VoucherGroup.transform.GetComponentsInChildren<Image>(false);
                    for (int dd = 0; dd < objdd.Length; dd++)
                    {
                        Destroy(objdd[dd].gameObject);
                    }
                    #endregion 删除预制体
                    int shutnum = 0;
                    for (int k = 0; k < lmpd._dicVoucher.Count; k++)
                    {
                        Debug.Log("isFirst:" + lmpd._dicVoucher[k].isFirst + " " + lmpd._dicVoucher[k].state + " iCanUse" + lmpd._dicVoucher[k].iCanUse + " " + lmpd._dicVoucher[k].amount + " " + lmpd._dicVoucher[k].exTime);
                        GameObject obj = Instantiate(Resources.Load<GameObject>("Lobby/ElemVoucher"));
                        obj.transform.SetParent(panel_voucher.toggleGroup_VoucherGroup.transform);
                        obj.transform.localPosition = Vector3.zero;
                        VoucherData vd = obj.GetComponent<VoucherData>();
                        vd.Text_amount.text = string.Format("{0:d}", lmpd._dicVoucher[k].amount);//金额
                        vd.Text_amount.color = new Color(1, 1, 1, 1);
                        vd.Sign.color = new Color(1, 1, 1, 1);
                        vd.Text_limit.color = new Color(0.67f, 0.12f, 0.22f, 1);
                        vd.Text_validityTime.color = new Color(1, 0.15f, 0.15f, 1);

                        if (lmpd._dicVoucher[k].limit == 0)
                        {
                            vd.Text_limit.text = "任意金额可用";
                        }
                        else
                        {
                            vd.Text_limit.text = "满" + lmpd._dicVoucher[k].limit + "元可用";
                        }
                        vd.Text_validityTime.text = "有效期：" + MahjongCommonMethod.Instance.UnixTimeStampToDateTime(lmpd._dicVoucher[k].exTime, 0).ToString("yyyy/MM/dd");
                        //此处要判断整理代金券的有效与否

                        Debug.LogError("chargeNum：" + chargeNum * 0.01f + ",lmpd._dicVoucher[k].limit：" + lmpd._dicVoucher[k].limit);

                        if (chargeNum == 0 || chargeNum * 0.01 >= lmpd._dicVoucher[k].limit)
                        {//chargeNum==0 代表钱包     
                            // StartCoroutine(MahjongCommonMethod.Instance.GetTime(() =>
                            //{
                            //Debug.LogError(Time.time);
                            //当前时间小于过期时间
                            //Debug.LogWarning(MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) + "---" + (ulong)lmpd._dicVoucher[k].exTime);
                            if (MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) < (ulong)lmpd._dicVoucher[k].exTime)
                            {
                                shutnum++;
                                vd.GetComponent<CanvasGroup>().alpha = 1f;
                                vd.Guoqi.gameObject.SetActive(false);
                                vd.Bg.sprite = DjqBg[0];
                                Debug.LogWarning("lmpd._dicVoucher[k].state" + lmpd._dicVoucher[k].state);
                                if (lmpd._dicVoucher[k].state == 0)
                                {
                                    //可以点击的代金券
                                    #region 可以点击的代金券
                                    vd.Tog_isChoice.interactable = true;
                                    vd.Tog_isChoice.group = panel_voucher.toggleGroup_VoucherGroup;
                                    vd.Image_recomment.group = panel_voucher.toggleGroup_VoucherGroup;
                                    // vd.Image_recomment.onValueChanged.AddListener((isOn) =>
                                    //{
                                    //    vd.imageText.gameObject.SetActive(isOn);
                                    //});
                                    vd.index_voucher = k;
                                    vd.Tog_isChoice.onValueChanged.AddListener(delegate (bool isOn)
                                    {
                                        int a = vd.index_voucher;
                                        if (isOn)
                                        {
                                            if (amount == 0)//如果是钱包
                                            {
                                                btnCloseVoucherPanel();  //点击代金券关闭面板
                                                OnCloseWallet();
                                                OpenChargePanel();
                                            }
                                            // else
                                            // {
                                            if (stackprice.Count >0)
                                            {
                                                chargeNum = (stackprice.Peek() as NetMsg.ClientCreateOrderRes).iChargeNumber;
                                                //  Debug.LogError("点击代金券"+chargeNum / 100+"________"+ lmpd._dicVoucher[a].amount );
                                                ComputeMoney(2, 0, chargeNum / 100, lmpd._dicVoucher[a].amount);
                                            }
                                            stackVoucher.Clear();
                                            stackVoucher.Push(lmpd._dicVoucher[a]);
                                        }
                                    });
                                    if (lmpd._dicVoucher[k].isFirst > 0)//第一张可用代金券
                                    {
                                        vd.Image_recomment.gameObject.SetActive(true);
                                        vd.Image_recomment.isOn = true;
                                    }
                                    else
                                    {
                                        vd.Image_recomment.gameObject.SetActive(false);
                                    }
                                    #endregion 可以点击的代金券
                                }//以上判断是不是已使用过
                                else
                                {
                                    vd.Text_amount.color = new Color(vd.Text_amount.color.r, vd.Text_amount.color.g, vd.Text_amount.color.b, 0.5f);
                                    vd.GetComponent<CanvasGroup>().alpha = 0.4f;
                                }

                            }//以上判断是不是过期
                            else
                            {
                                vd.Bg.sprite = DjqBg[1];
                                vd.Guoqi.gameObject.SetActive(true);
                                vd.Text_limit.color = new Color(0.6f, 0.6f, 0.6f, 1);
                                vd.Text_validityTime.color = new Color(0.65f, 0.65f, 0.65f, 1);
                                //vd._tNull.SetActive(true);
                                //vd.Text_validityTime.color = new Color(vd.Text_validityTime.color.r, vd.Text_validityTime.color.g, vd.Text_validityTime.color.b, 0.5f);
                            }
                        }//以上判断是金额是不是满足满减 条件
                        else
                        {
                            Debug.LogWarning("============================");
                            //vd._tNull.SetActive(true);
                            //vd.Text_limit.color = new Color(vd.Text_limit.color.r, vd.Text_limit.color.g, vd.Text_limit.color.b, 0.4f);
                            vd.GetComponent<CanvasGroup>().alpha = 0.4f;
                            vd.Tog_isChoice.interactable = false;
                            vd.Guoqi.gameObject.SetActive(false);
                        }
                    }//以上for循环


                    //if (amount > 0)
                    //{
                    //    for (int hh = 0; hh < lmpd._dicVoucher.Count; hh++)
                    //    {
                    //        if (lmpd._dicVoucher[hh].limit > chargeNum)
                    //        {
                    //            shutnum++;
                    //        }
                    //    }
                    //}

                    ComputeMoney(1, shutnum, chargeNum / 100, 0);
                    yield break;
                }
            }
        }

        IEnumerator GetJsonAd(string url, Action action)
        {
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            for (int i = 0; i < 3; i++)
            {
                WWW www = new WWW(url);
                Debug.LogError(url);
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    continue;
                }
                else
                {
                    lmpd.ja = JsonUtility.FromJson<LobbyMainPanelData.Json_Ad>(www.text);
                    action();
                    break;
                }
            }
        }
        public void GetVoucherData(int amount)
        {
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            //获取代金券数据
            string szurl = SDKManager.Instance.IOSCheckStaus == 1 ? string.Format(LobbyContants.MAJONG_PORT_URL_T + lmpd.UrlVoucher, MahjongCommonMethod.Instance.iUserid) : string.Format(LobbyContants.MAJONG_PORT_URL + lmpd.UrlVoucher, MahjongCommonMethod.Instance.iUserid);
            StartCoroutine(GetVoucher(szurl, amount));
        }

        void Start()
        {
            GameData gd;
            gd = GameData.Instance;
            LobbyMainPanelData lmpd = gd.LobbyMainPanelData;
            SystemMgr.Instance.BgmSystem.PlayBgm(LobbySystem.SubSystem.BgAudioManageSystem.BgmType.HaoFengGuang, true);
            //审核状态下，关闭部分功能
            if (SDKManager.Instance.CheckStatus == 1 || SDKManager.Instance.IOSCheckStaus == 1)
            {
                panel_wallet.btnLobbyWallet.gameObject.SetActive(false);
                AllDownBtn[0].SetActive(false);
                AllDownBtn[2].SetActive(false);
                BtnShowCount += 2;
                for (int i = 0; i < CheckHideBtn.Length; i++)
                {
                    CheckHideBtn[i].SetActive(false);
                }
                for (int i = 0; i < shareBtn.Length; i++)
                {
                    shareBtn[i].SetActive(false);
                }
                ParlorAndJoinRoom[0].SetActive(false);
                ParlorAndJoinRoom[1].SetActive(false);
            }
            else
            {
                for (int i = 0; i < CheckHideBtn.Length; i++)
                {
                    CheckHideBtn[i].SetActive(false);
                }
                panel_wallet.btnLobbyWallet.gameObject.SetActive(true);
                for (int i = 0; i < CheckHideBtn.Length; i++)
                {
                    CheckHideBtn[i].SetActive(true);
                }
                ParlorAndJoinRoom[0].SetActive(true);
                ParlorAndJoinRoom[1].SetActive(true);
                //  Lobby_leftBtn.btnFish.onClick.AddListener(() => { Application.OpenURL(MahjongCommonMethod.fishUrl); });
            }
            //初始化最下面一排按钮的框体大小
            InitClickPos(BtnShowCount);
            // PlayerPrefs.DeleteAll();
            if (PlayerPrefs.HasKey(CreatRoomMessagePanelData.SaveRuleParamName))
            {
                CreatRoomMessagePanelData cd = gd.CreatRoomMessagePanelData;
                cd.allRoomMethor.Clear();
                string[] Methords = PlayerPrefs.GetString(CreatRoomMessagePanelData.SaveRuleParamName).Split('|');
                for (int i = 0; i < Methords.Length; i++)
                {
                   // Debug.LogWarning("+++"+Methords[i]);
                    string[] MethordsID = Methords[i].Split(':');
                    string[] rulesID = MethordsID[1].Split('_');
                    uint[] rulei = new uint[rulesID.Length];
                    for (int j = 0; j < rulesID.Length; j++)
                    {
                       // Debug.Log("__"+rulesID[j ]);
                        rulei[j] = Convert.ToUInt32(rulesID[j ],16);
                        //rulei[j] = UInt32.Parse(rulesID[i], System.Globalization.NumberStyles. HexNumber);
                    }
                    cd.allRoomMethor.Add(new CreatRoomMessagePanelData.MethordRuleClass(Int32.Parse(MethordsID[0]), rulei));
                   // Debug.LogErrorFormat("{0}_{1}_{2}_{3}_{4}_{5}_{6}", MethordsID[0],rulei[0].ToString("X8"), rulei[1].ToString("X8"), rulei[2].ToString("X8"), rulei[3].ToString("X8"), rulei[4].ToString("X8"), rulei[5].ToString("X8"));
                }
            }
            #region 添加按钮事件
            #region 加群
            // 0-加群面板/0提交手机号
            panels_joinUs.btn.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                if (panels_joinUs._input.text.Length < 11)
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("您输入的手机号格式不正确");
                }
                else
                {
                    string ss = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
                    MahjongCommonMethod.Instance.GetUrlJson(

                   string.Format(ss + "joinPhone.x?uid={0}&phone={1}", GameData.Instance.PlayerNodeDef.iUserId, panels_joinUs._input.text)
                   , ""
                   , (a, b) =>
                   {
                       Debug.LogWarning(b);
                       int i = int.Parse(b.Substring(b.LastIndexOf(':') + 1, 1));
                       if (i == 1)
                       {
                           MahjongCommonMethod.Instance.ShowRemindFrame("已经提交成功，官方人员会尽快联系您!");
                           BtnCloseJoinUsPanel();
                       }
                       else
                       {
                           if (i == 2)
                           {
                               UIMgr.GetInstance().GetUIMessageView().Show("非常感谢您对我们产品的支持，您今日已提交3次宝贵的建议，若还有宝贵建议请联系客服。", () =>
                               {

                               }, () =>
                               {
                                   Messenger_anhui.Broadcast(MESSAGE_CUSTOMSEVERBTN);
                                   BtnCloseJoinUsPanel();
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
            panels_joinUs.btnLianxiKefu.onClick.AddListener(() =>
           {
               MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickKeFu);
               Messenger_anhui.Broadcast(MESSAGE_CUSTOMSEVERBTN);
               BtnCloseJoinUsPanel();
           });
            //  //添加复制按钮事件
            panels_joinUs.btnCoppyWeChat.onClick.AddListener(() =>
           {
               SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
               string str = panels_joinUs._tWeChatNum.text;
               MahjongCommonMethod.Instance.CopyString(str);
               //处理玩家复制成功之后提示文字
               MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
           });
            panels_joinUs.btnCoppyTel.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                string str = panels_joinUs._Tel.text;
                MahjongCommonMethod.Instance.CopyString(str);
                //处理玩家复制成功之后提示文字
                MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
            });


            //添加滑动事件
            GameData.Instance.LobbyMainPanelData.oriPos = panels_joinUs._g[1].transform.localPosition.x;
            EventTriggerListener.Get(panels_joinUs._g[2]).onUp = SystemMgr.Instance.LobbyMainSystem.OnPointerUp;
            EventTriggerListener.Get(panels_joinUs._g[2]).onDown = SystemMgr.Instance.LobbyMainSystem.OnPointerDown;
            #endregion

            //添加关闭开启面板按钮事件
            #region 钱包
            panel_wallet.btnClose.onClick.AddListener(() =>
            {
                OnCloseWallet();
            });
            panel_wallet.btnLobbyWallet.onClick.AddListener(() =>
            {
                NetMsg.ClientUserDef pd = gd.PlayerNodeDef.userDef;
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet._gPanelWallet.SetActive(true);//打开钱包面板
                for (int i = 0; i < 4; i++)
                {
                    panel_wallet.walletType[i]._tNum.text = pd.da2Asset[i] * 0.01 + "";

                }
                string ss = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
                MahjongCommonMethod.Instance.GetUrlJson(

                    string.Format(ss + gd.LobbyMainPanelData.UrlVoucher, pd.iUserId),
                    gd.LobbyMainPanelData.vc,
                    (vc, str) =>
                    {
                        panel_wallet.walletType[4]._tNum.text = vc.data.Length.ToString();
                    });
            });
            panel_wallet.walletType[0].btn[0].onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet.panelCashToCoin._gPanelCashToCoin.SetActive(true);//打开现金兑换金币面板
                AddExchageEvent(2);//现金兑换
            });

            panel_wallet.panelCashToCoin.btnClose.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet.panelCashToCoin._gPanelCashToCoin.SetActive(false);//关闭现金兑换金币面板
            });
            //打来不同种类的提示
            for (int i = 0; i < 2; i++)
            {
                panel_wallet.walletType[i + 1].btn[0].onClick.AddListener(() =>
                  {
                      SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                      panel_wallet.panelremind.t.text = "请前往微信公众号【" + "<color=#8528ADFF>双喜麻将</color>" + "】兑换";
                      panel_wallet.panelremind._gPanelRemind.SetActive(true);
                  });//打开提示按钮事件绑定
            }
            panel_wallet.walletType[0].btn[1].onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet.panelremind.t.text = "请前往微信公众号【" + "<color=#8528ADFF>双喜麻将</color>" + "】提现";

                panel_wallet.panelremind._gPanelRemind.SetActive(true);
            });//打开提示按钮事件绑定

            panel_wallet.panelremind.btnClose.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

                panel_wallet.panelremind._gPanelRemind.SetActive(false);
            });//关闭提示按钮事件绑定

            panel_wallet.panelremind.btnok.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet.panelremind._gPanelRemind.SetActive(false);
            });//点击知道了按钮事件绑定

            panel_wallet.walletType[4].btn[0].onClick.AddListener(() =>
            {
                Debug.LogWarning("钱包面板");
                btnOpenVoucherPanel(0);
            });//打开储值卡面板

            panel_wallet.walletType[3].btn[0].onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_wallet.panelCashToCoin._gPanelCashToCoin.SetActive(true);
                AddExchageEvent(3);
            });//打开代金券按钮事件绑定
            #endregion 钱包

            #region 购买金币
            //打开金币面板按钮事件绑定
            panel_Charge.btnLobbyBuyCoin.onClick.AddListener(() =>
            {
                OpenChargePanel();
            });
            panel_Charge.btnClose.onClick.AddListener(() =>
           {
               BtnCloseBuyRoomCard();
           });
            panel_Charge.btnOpenVoucher.onClick.AddListener(() =>
           {
               Debug.LogWarning("购买金币");
               btnOpenVoucherPanel(1);
           });

            panel_Charge.btnClose.onClick.AddListener(() =>
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                panel_Charge._gChoiceChannelPanel.SetActive(false);
            });
            panel_Charge.btnChannel[0].onClick.AddListener(() => { btnClickPay(2); });
            panel_Charge.btnChannel[1].onClick.AddListener(() => { btnClickPay(3); });


            #endregion 购买金币

            #endregion 添加按钮事件

#if UNITY_IOS
            //如果是送审 关闭活动
            if (LobbyContants.version_typr == "" || (SDKManager.Instance.CheckStatus == 1 || SDKManager.Instance.IOSCheckStaus == 1))
            {
                FestivalActivityBtn.SetActive(false);
            }   
#endif
            //获取地区信息
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            if (!sapd.IsGetCityMessage)
            {
                if (SDKManager.Instance.IOSCheckStaus == 1)
                {
                    //获取所有城市的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_city
                        , null, sapd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_county
                        , null, sapd.GetCountyMessage, "CountyData");
                    sapd.IsGetCityMessage = true;
                }
                else
                {
                    //获取所有城市的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_city
                        , null, sapd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_county
                        , null, sapd.GetCountyMessage, "CountyData");
                    sapd.IsGetCityMessage = true;
                }
            }
            //获取一下预约时间
            GameData.Instance.LobbyMainPanelData.OnGetAppointmentURL();
            //创建红包所需要的红包类型，参数值待传
            UIMainView.Instance.RedPagePanel.InitRP();

            // Debug.LogWarning("创建红包所需要的红包类型，参数值待传");

            //检测是不是有红包，如果有显示红包的按钮
            StartCoroutine(OnSetRedPageState());
        }
        /// <summary>
        /// 添加兑换事件
        /// </summary>
        /// <param name="type">2 现金兑换 3储值卡兑换</param>
        void AddExchageEvent(int type)
        {
            PlayerNodeDef pd = GameData.Instance.PlayerNodeDef;
            int a = 0;
            if (type == 2)
            {
                panel_wallet.panelCashToCoin._tTypeDis.text = "当前现金:";
                panel_wallet.panelCashToCoin.currentCash.text = pd.userDef.da2Asset[0] * 0.01 + "";
                a = 2001;
                for (int i = 0; i < MahjongCommonMethod.Instance._dicExchage2.Count; i++)
                {
                    if (!panel_wallet.panelCashToCoin._gbox[i].activeInHierarchy)
                    {
                        panel_wallet.panelCashToCoin._gbox[i].SetActive(true);
                    }
                    panel_wallet.panelCashToCoin.tCoin[i].text = MahjongCommonMethod.Instance._dicExchage2[i + a].iCoin + MahjongCommonMethod.Instance._dicExchage2[i + a].iBindCoin + "金币";
                    panel_wallet.panelCashToCoin.tPrice[i].text = MahjongCommonMethod.Instance._dicExchage2[i + a].iAsset + "元";
                    EventTriggerListener.Get(panel_wallet.panelCashToCoin.btn_exchage[i].gameObject, MahjongCommonMethod.Instance._dicExchage2[i + a].iExchangeId.ToString() + "," + type).onClick_Sring = SendExchage;
                }
            }
            else if (type == 3)
            {
                panel_wallet.panelCashToCoin._tTypeDis.text = "当前储值卡:";
                panel_wallet.panelCashToCoin.currentCash.text = pd.userDef.da2Asset[3] * 0.01 + "";
                a = 3001;
                for (int i = 0; i < MahjongCommonMethod.Instance._dicExchage3.Count; i++)
                {
                    if (!panel_wallet.panelCashToCoin._gbox[i].activeInHierarchy)
                    {
                        panel_wallet.panelCashToCoin._gbox[i].SetActive(true);
                    }
                    Debug.LogWarning(i + a);
                    panel_wallet.panelCashToCoin.tCoin[i].text = MahjongCommonMethod.Instance._dicExchage3[i + a].iCoin + MahjongCommonMethod.Instance._dicExchage3[i + a].iBindCoin + "金币";
                    panel_wallet.panelCashToCoin.tPrice[i].text = MahjongCommonMethod.Instance._dicExchage3[i + a].iAsset + "元";
                    EventTriggerListener.Get(panel_wallet.panelCashToCoin.btn_exchage[i].gameObject, MahjongCommonMethod.Instance._dicExchage3[i + a].iExchangeId.ToString() + "," + type).onClick_Sring = SendExchage;
                }
            }
        }

        /// <summary>
        /// 显示或者关闭活动的按钮
        /// </summary>
        /// <param name="state"></param>
        public void DoFestivalActivityShowOrClose(bool state)
        {
            FestivalActivityBtn.SetActive(state);
        }

        /// <summary>
        /// 检测是不是时间内
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="EndTime"></param>
        public void OnTestTimeForScend(int startTime, int EndTime)
        {
            StartCoroutine(OnTestTimeForScend_(startTime, EndTime));
        }
        IEnumerator OnTestTimeForScend_(int startTime, int EndTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f * 60);
                if (startTime > MahjongCommonMethod.Instance.GetCreatetime() || EndTime < MahjongCommonMethod.Instance.GetCreatetime())
                {
                    Debug.LogWarning("开始时间：" + MahjongCommonMethod.Instance.UnixTimeStampToDateTime(startTime, 0).ToString("yyyy年-MM月-dd日 HH:mm"));
                    Debug.LogWarning("现在时间：" + MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0).ToString("yyyy年-MM月-dd日 HH:mm"));
                    Debug.LogWarning("结束时间：" + MahjongCommonMethod.Instance.UnixTimeStampToDateTime(EndTime, 0).ToString("yyyy年-MM月-dd日 HH:mm"));
                    UIMainView.Instance.LobbyPanel.DoFestivalActivityShowOrClose(false);
                    UIMainView.Instance.FestivalActivity.gameObject.SetActive(false);
                }
            }
        }


        void Update()
        {
            //每隔五分钟请求一次，消息、战绩信息
            if (timer < 300f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
                //调用请求方法
                SystemMgr.Instance.LobbyMainSystem.MessageReq(0);  //请求消息
                HistroyGradePanelData hgpd = GameData.Instance.HistroyGradePanelData;
                hgpd.HistroyReq_Web(0);   //请求战绩               
            }
        }

        /// <summary>
        /// 更新面板显示
        /// </summary>
        public void UpdateShow()
        {
            GameData gd_ = GameData.Instance;
            LobbyMainPanelData lmpd = gd_.LobbyMainPanelData;
            if (lmpd.isPanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = true;
                //显示玩家信息的红点
                if (PlayerPrefs.GetInt("userInfoRed" + GameData.Instance.PlayerNodeDef.iUserId) == 1)
                {
                    RedPoint[0].gameObject.SetActive(true);
                }
                else if (PlayerPrefs.GetInt("userInfoRed" + GameData.Instance.PlayerNodeDef.iUserId) == 2)
                {
                    RedPoint[0].gameObject.SetActive(false);
                }
                RedPoint[3].gameObject.SetActive(gd_.HolidayActivityPanelData.IsShowLobbyRed());

                if (GameData.Instance.PlayerNodeDef.userDef.byBossFirstChargeAward == 0 && GameData.Instance.PlayerNodeDef.userDef.byMemberFirstChargeAward == 0
                    && SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                {
                    shareBtn[2].SetActive(true);
                }
                else
                {
                    shareBtn[2].SetActive(false);
                }

                //显示玩家的实名注册的红点
                if (PlayerPrefs.GetFloat(GameData.RedPoint.RealName.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 1)
                {
                    RedPoint[5].gameObject.SetActive(true);
                }
                else
                {
                    RedPoint[5].gameObject.SetActive(false);
                }


                //显示玩家的分享礼包的红点
                if (PlayerPrefs.GetFloat(GameData.RedPoint.ShareBag.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 1)
                {
                    RedPoint[6].gameObject.SetActive(true);
                }
                else
                {
                    RedPoint[6].gameObject.SetActive(false);
                }
                if (SDKManager.Instance.IOSCheckStaus == 1)
                {
                    AllDownBtn[2].SetActive(false);
                    _BtnJoinUs.gameObject.SetActive(false);
                    Lobby_leftBtn.btnChage.gameObject.SetActive(false);
                }
                else
                {
                    EventTriggerListener.Get(Lobby_leftBtn.btnChage.gameObject).onClick = BtnChangeAreaSelect;
                    // Lobby_leftBtn.btnFish.gameObject.SetActive(true);//开启捕鱼按钮

                    AllDownBtn[0].SetActive(true);
                    AllDownBtn[2].SetActive(true);
                    //关闭加我提示
                    if (SDKManager.Instance.CheckStatus == 1)
                    {
                        _BtnJoinUs.SetActive(false);
                    }
                    else
                    {
                        _BtnJoinUs.SetActive(true);
                    }
                    //InstCreatBtn.SetActive(true);
                }

                ////显示本地的qq群
                //if (MahjongCommonMethod.Instance._dicDisConfig.ContainsKey(GameData.Instance.SelectAreaPanelData.iCountyId))
                //{
                //    CountyQQMessae[0].text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME + "QQ交流群";
                //    CountyQQMessae[1].text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].QQ.ToString();
                //}

                //根据选择区域显示要显示的图片   

                if (MahjongCommonMethod.Instance._dicDisConfig.ContainsKey(GameData.Instance.SelectAreaPanelData.iCountyId))
                {
                    if (MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME.Length > 2)
                    {
                        AreaName.GetComponent<Text>().lineSpacing = 0.8f;
                        AreaName.GetComponent<Text>().resizeTextForBestFit = true;
                    }
                    else
                    {
                        AreaName.GetComponent<Text>().lineSpacing = 0.9f;
                        AreaName.GetComponent<Text>().resizeTextForBestFit = false;
                    }
                    AreaName.text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
                }

                if (MahjongCommonMethod.Instance.PlayerRoomStatus == 0)
                {
                    OpenRoom[0].SetActive(true);
                    OpenRoom[1].SetActive(false);
                }
                else
                {
                    OpenRoom[0].SetActive(false);
                    OpenRoom[1].SetActive(true);
                }

                //Debug.LogError("SDKManager.Instance.IOSCheckStaus：" + SDKManager.Instance.IOSCheckStaus);

                //决定是否显示实名认证的按钮
                if (gd_.PlayerNodeDef.byNameAuthen == 0 && SDKManager.Instance.IOSCheckStaus == 0)
                {
                    Lobby_leftBtn._BtnRealName.gameObject.SetActive(true);
                    shareBtn[1].gameObject.SetActive(true);
                }
                else
                {
                    Lobby_leftBtn._BtnRealName.gameObject.SetActive(false);
                    shareBtn[1].gameObject.SetActive(false);
                }
                ////决定是否显示推广礼包按钮
                //if (gd_.PlayerNodeDef.iSpreadGiftTime == 0 && SDKManager.Instance.CheckStatus == 0)
                //{
                //    _BtnProductBag.gameObject.SetActive(true);
                //}
                //else
                //{
                //    _BtnProductBag.gameObject.SetActive(false);
                //}
                ////决定是否显示代开按钮
                //if (SDKManager.Instance.CheckStatus != 1)
                //{
                //    _btnInsteadOpen.gameObject.SetActive(true);
                //}
                //else
                //{
                //    _btnInsteadOpen.gameObject.SetActive(false);
                //}
                //显示购买房卡的按钮
                if (lmpd.isShowBuyRoomCard)
                {
                    //if (SDKManager.Instance.IOSCheckStaus == 1)
                    //{
                    //#if UNITY_IOS || UNITY_IPONE
                    // IOSRoomCardChi.SetActive(true);
                    // AndroidCardChi.SetActive(false);                    
                    //#endif
                    //  }
                    //  else
                    // {
                    // IOSRoomCardChi.SetActive(false);
                    //  AndroidCardChi.SetActive(true);

                    ////为代理昵称头像赋值
                    //if (GameData.Instance.PlayerNodeDef.iProxyId != 0)
                    //{
                    //    if (GameData.Instance.ProductAgencyPanelData.szNickname == null)
                    //    {
                    //        NetMsg.ClientProxyInfoReq msg = new NetMsg.ClientProxyInfoReq();
                    //        msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                    //        msg.iProxyId = GameData.Instance.PlayerNodeDef.iProxyId;
                    //        NetworkMgr.Instance.LobbyServer.SendProxyInfoReq(msg);
                    //    }
                    //    StartCoroutine(DelayShow(0.1f));
                    //}
                    //else
                    //{
                    //    Debug.LogError("===================");
                    //    nickName.text = "您还没有绑定代理";
                    //    headIamge.texture = NoDailiTexture.texture;
                    //}
                    panel_Charge._gChoiceChannelPanel.SetActive(lmpd.isShowChoiceChannelPanel);
                    // }
                    UpdateCoinPanel();
                    panel_Charge._gPanelCharge.SetActive(true);
                }
                else
                {
                    panel_Charge._gPanelCharge.SetActive(false);
                }
                Nickname.text = gd_.PlayerNodeDef.szNickname;
                //显示玩家id
                //Debug.LogError("赋值ID：" + gd_.PlayerNodeDef.iUserId);
                PlayerID.text = "ID:" + gd_.PlayerNodeDef.iUserId.ToString();
                //显示玩家房卡数量
                RoomCardNum.text = (gd_.PlayerNodeDef.iCoin).ToString();
                //显示获赞数量
                GetPraise.text = "<color=#0000ff>赞：</color>" + gd_.PlayerNodeDef.iCompliment.ToString();


                //Debug.LogError("玩家头像:" + GameData.Instance.PlayerNodeDef.szHeadimgurl);

                //获取玩家头像
                if (MahjongCommonMethod.Instance && GameData.Instance.PlayerNodeDef.szHeadimgurl != null)
                {
                    if (GameData.Instance.PlayerNodeDef.szHeadimgurl.TrimEnd().Length < 20)
                    {
                        PlayerAvatar.texture = Resources.Load<Texture>("icon");
                    }
                    else
                    {

                        MahjongCommonMethod.Instance.GetPlayerAvatar(UIMainView.Instance.LobbyPanel.PlayerAvatar, gd_.PlayerNodeDef.szHeadimgurl);
                    }
                }
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        //IEnumerator DelayShow(float timer)
        //{
        //    yield return new WaitForSeconds(timer);
        //    //延迟显示            
        //    nickName.text = GameData.Instance.ProductAgencyPanelData.szNickname;
        //    MahjongCommonMethod.Instance.GetPlayerAvatar(headIamge, GameData.Instance.ProductAgencyPanelData.szHeadimgurl, 2);
        //}
        public void UpdateCoinPanel()
        {
            GameData gd = GameData.Instance;
            LobbyMainPanelData md = gd.LobbyMainPanelData;
            //Debug.LogError("GameData.Instance.PlayerNodeDef.iMyParlorId" + GameData.Instance.PlayerNodeDef.iMyParlorId);
            //Debug.LogError("MemberFirstChargeAward" + GameData.Instance.PlayerNodeDef.userDef.byMemberFirstChargeAward);
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0)
            {
                if (GameData.Instance.PlayerNodeDef.userDef.byBossFirstChargeAward > 0)
                {
                    md.isShowSendCoin = false;
                    UIMainView.Instance.LobbyPanel.panel_Charge._tDoubleTip.text = "";
                    for (int i = 0; i < UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup.Length; i++)
                    {
                        UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup[i]._imgIcon.gameObject.SetActive(false);

                    }
                }
                else
                {
                    md.isShowSendCoin = true;
                    UIMainView.Instance.LobbyPanel.panel_Charge._tDoubleTip.text = "首次充值后，其他充值项不再双倍 ！";
                    for (int i = 0; i < UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup.Length; i++)
                    {
                        UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup[i]._imgIcon.gameObject.SetActive(true);

                    }

                }
            }
            else
            {

                if (GameData.Instance.PlayerNodeDef.userDef.byMemberFirstChargeAward > 0)
                {
                    md.isShowSendCoin = false;
                    UIMainView.Instance.LobbyPanel.panel_Charge._tDoubleTip.text = "";
                    for (int i = 0; i < UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup.Length; i++)
                    {
                        UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup[i]._imgIcon.gameObject.SetActive(false);

                    }

                }
                else
                {
                    md.isShowSendCoin = true;
                    UIMainView.Instance.LobbyPanel.panel_Charge._tDoubleTip.text = "首次充值后，其他充值项不再双倍 ！";
                    for (int i = 0; i < UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup.Length; i++)
                    {
                        UIMainView.Instance.LobbyPanel.panel_Charge.chargeGroup[i]._imgIcon.gameObject.SetActive(true);

                    }
                }
            }

        }
        #region Function 

        /// <summary>
        /// 移动大厅的游戏公告
        /// </summary>
        /// <param name="text">公告内容</param>
        public void MoveBulltein(string text)
        {
            if (GameData.Instance.LobbyMainPanelData.isPlayingBulletin)
            {
                return;
            }

            Bulltein_Text.text = text;
            float speed = 90f;
            float width = Bulltein_Text.preferredWidth + 755f;
            Bulletein.gameObject.SetActive(true);
            GameData.Instance.LobbyMainPanelData.isPlayingBulletin = true;
            //指定text的位置
            Bulltein_Text.transform.localPosition = new Vector3(width / 2f, 0, 0);
            //移动公告
            float x = Bulltein_Text.transform.localPosition.x - (width);

            Tweener tweener = Bulltein_Text.transform.DOLocalMoveX(x, (width / 2f - x) / speed);
            tweener.SetEase(Ease.Linear);
            tweener.OnComplete(MoveBullteinCallback);
        }

        /// <summary>
        /// 公告回调
        /// </summary>
        void MoveBullteinCallback()
        {
            GameData.Instance.LobbyMainPanelData.isPlayingBulletin = false;
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            //移除刚才播放的公告通知
            if (lmpd.lsBulletinNotice.Count > 0)
            {
                lmpd.lsBulletinNotice.RemoveAt(0);
            }

            //播放下一条公告通知
            if (gameObject.activeInHierarchy)
            {
                if (lmpd.lsBulletinNotice.Count > 0)
                {
                    //MoveBulltein(lmpd.lsBulletinNotice[0]);
                    StartCoroutine(DelayShowBulltien(3f));
                }
                else
                {
                    StartCoroutine(DelayCloseBulltein(0f));
                }

            }
        }

        /// <summary>
        /// 延迟显示下条公告
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        IEnumerator DelayShowBulltien(float timer)
        {
            Bulletein.gameObject.SetActive(false);
            yield return new WaitForSeconds(timer);
            LobbyMainPanelData lmpd = GameData.Instance.LobbyMainPanelData;
            MoveBulltein(lmpd.lsBulletinNotice[0]);
        }

        /// <summary>
        /// 延迟关闭公告
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator DelayCloseBulltein(float time)
        {
            yield return new WaitForSeconds(time);
            Bulltein_Text.text = "";
            Bulltein_Text.transform.localPosition = new Vector3(50f, 0f, 0f);
            Bulletein.gameObject.SetActive(false);
        }


        /// <summary>
        /// 重置最下面一排的按钮的背景位置
        /// </summary>
        /// <param name="num"></param>
        void InitClickPos(int num)
        {
            if (num <= 0)
            {
                return;
            }

            Debug.LogWarning("数量：" + BtnShowCount);

            BtnBg[0].rectTransform.localPosition += new Vector3(205f / 4f * BtnShowCount, 0, 0);
            BtnBg[0].rectTransform.SetSizeWithCurrentAnchors(0, BtnBg[0].rectTransform.rect.width - 205f / 2f * BtnShowCount);
            BtnBg[1].rectTransform.localPosition -= new Vector3(205f / 4f * BtnShowCount, 0, 0);
            BtnBg[1].rectTransform.SetSizeWithCurrentAnchors(0, BtnBg[1].rectTransform.rect.width - 205f / 2f * BtnShowCount);
        }

        /// <summary>
        /// 获取新手礼包
        /// </summary>
        public void GetNewplayerBag(int RoomNum)
        {
            GameData.Instance.LobbyMainPanelData.CardNumstatus = 1;
            NewPlayerBag.SetActive(true);
            NewPlayerBag.GetComponent<RawImage>().enabled = true;
        }
        //领取金币按钮
        public void BtnSaveGold()
        {
            GameData.Instance.PlayerNodeDef.iBindCoin += int.Parse(_goldShow.transform.Find("GetMoney").GetComponent<Text>().text);
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            _goldShow.SetActive(false);
        }
        public GameObject _goldShow;
        /// <summary>
        /// 点击领取新手礼包
        /// </summary>
        public void BtnNewPlayerBag(int num)
        {
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.NerPlayerGift);
            NewPlayerBag.GetComponent<RawImage>().enabled = false;
            //隐藏背景动画
            Transform[] trans = NewPlayerBag.GetComponentsInChildren<Transform>();
            for (int i = 1; i < trans.Length; i++)
            {
                trans[i].gameObject.SetActive(false);
            }
            //显示金币掉落
            StartCoroutine(ShowSaveGoldPanel(num));
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/Effect/gold_Particle"));
            ParticleSystem ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(go, 2);
            //产生新手礼包的预置体
            //GameObject go = Instantiate(Resources.Load<GameObject>("Game/Effect/ShowRoomNum"));
            //go.transform.FindChild("ShowRoomNum").GetComponent<Canvas>().worldCamera = Camera.main;
            //go.transform.FindChild("BlackBg").GetComponent<Canvas>().worldCamera = Camera.main;
            //go.transform.SetParent(UIMainView.Instance.transform);
            //go.transform.FindChild("ShowRoomNum/Text").GetComponent<Text>().text = num.ToString();
            //go.transform.localScale = Vector3.one;
            //go.transform.localPosition = Vector3.zero;
            //go.transform.GetComponent<DeleteEffect>().CardNum = num;

            //string Name = "";
            ////确定播放的动画
            //if (num <= 3)
            //{
            //    Name = num.ToString();
            //}
            //else
            //{
            //    Name = "6";
            //}

            //go.transform.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = Name;
            ////延迟一秒删除，数字
            // StartCoroutine(DelayDel(3.1f, go.transform.FindChild("ShowRoomNum/Text").gameObject));

        }
        IEnumerator ShowSaveGoldPanel(int num)
        {
            yield return new WaitForSeconds(1);
            _goldShow.SetActive(true);
            _goldShow.transform.Find("GetMoney").GetComponent<Text>().text = num.ToString();
        }
        public void PlayNewPlayerBag(int num)
        {
            NewPlayerBag.GetComponent<RawImage>().enabled = false;
            //隐藏背景动画
            Transform[] trans = NewPlayerBag.GetComponentsInChildren<Transform>();
            for (int i = 1; i < trans.Length; i++)
            {
                trans[i].gameObject.SetActive(false);
            }

            //产生新手礼包的预置体
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/Effect/ShowRoomNum"));
            go.transform.Find("ShowRoomNum").GetComponent<Canvas>().worldCamera = Camera.main;
            go.transform.Find("BlackBg").GetComponent<Canvas>().worldCamera = Camera.main;
            go.transform.SetParent(UIMainView.Instance.transform);
            go.transform.Find("ShowRoomNum/Text").GetComponent<Text>().text = num.ToString();
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.GetComponent<DeleteEffect>().CardNum = num;
            string Name = "";
            //确定播放的动画
            if (num <= 3)
            {
                Name = num.ToString();
            }
            else
            {
                Name = "6";
            }

            go.transform.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = Name;
            //延迟一秒删除，数字
            StartCoroutine(PlayDelayDel(3.1f, go.transform.Find("ShowRoomNum/Text").gameObject));

        }

        IEnumerator DelayDel(float timer, GameObject go)
        {
            yield return new WaitForSeconds(timer);
            Destroy(go);
        }

        IEnumerator PlayDelayDel(float timer, GameObject go)
        {
            yield return new WaitForSeconds(timer);
            Destroy(go);
            go.transform.parent.GetComponentInChildren<SkeletonAnimation>().state.End += delegate
            {
                if (MahjongCommonMethod.Instance.IsNewDay())
                {
                    GameData.Instance.HolidayActivityPanelData.isPanelShow = true;
                    SystemMgr.Instance.HolidayActivitySystem.UpdateShow();
                }
            };
            //---------------- 每天显示活动面板 ------------------

        }
        public void PlayGold_Particle(int num)
        {
            StartCoroutine(ShowSaveGoldPanel(num));
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/Effect/gold_Particle"));
            ParticleSystem ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(go, 2);
        }
        #endregion Function


        #region 按钮点击

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        public void BtnRefresh()
        {
            //  PlayerPrefs.DeleteAll();
            NetMsg.ClientRefreshUserReq req = new NetMsg.ClientRefreshUserReq();
            req.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在读取信息，请稍候...");
            Network.NetworkMgr.Instance.LobbyServer.SendClientRefreshUserReq(req);
            //  Debug.LogError("刷新刷星");
            //throw (new ApplicationException("appexc刷新刷星"));
        }

        /// <summary>
        /// 点击实名认证按钮
        /// </summary>
        public void BtnRealName()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_REALNAME);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickRealName);
            GameData.Instance.RealNameApprovePanelData.LoadForbiddenName();
            // throw (new Exception("exc认证"));
        }
        /// <summary>
        /// 点击交流群按钮
        /// </summary>
        public void BtnJoinUs()
        {
            //关闭加群的新手引导
            if (PlayerPrefs.GetFloat(NewPlayerGuide.Guide.JoinMe.ToString() + GameData.Instance.PlayerNodeDef.iUserId) == 0)
            {
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.JoinMe);
            }
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickJoin);
            panels_joinUs._g[0].SetActive(true);
            LobbyMainPanelData md = GameData.Instance.LobbyMainPanelData;
            //if (md.ja != null)
            //{
            //    StartCoroutine(GetJsonAd(LobbyContants.MAJONG_PORT_URL + md.UrlAd, () =>
            //    {
            //        for (int i = 0; i < md.ja.data.Length; i++)
            //        {
            //            if (i >= 3)
            //            {
            //                panels_joinUs._g[1].transform.GetChild(i).gameObject.SetActive(true);
            //                panels_joinUs._toglePoint[i].transform.parent.gameObject.SetActive(true);
            //            }
            //            panels_joinUs._tCounty .text = md.ja.title;
            //           // panels_joinUs._childGroup[i]._textHead.text = md.ja.data[i].title;
            //            //panels_joinUs._childGroup[i]._text.text = md.ja.data[i].context;
            //        }
            //    }));
            //}
            //StartCoroutine(DownloadQRcode(panels_joinUs.rawHead));
            CommonConfig.Json_Join jj = new CommonConfig.Json_Join();
            string ss = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
            MahjongCommonMethod.Instance.GetUrlJson(string.Format(ss + MahjongCommonMethod.communicatUrl, GameData.Instance.SelectAreaPanelData.iCountyId), jj, (j, str) =>
            {
                if (j.status == 1)
                {
                    panels_joinUs._tWeChatNum.text = j.data[0].WEIXIN;
                    panels_joinUs._Tel.text = j.data[0].PHONE;
                    panels_joinUs._tNickName.text = j.data[0].NICKNAME;
                    panels_joinUs._tCounty.text = j.data[0].COUNTY_NAME;
                    string imgUrl = j.data[0].IMG_URL;
                    Debug.LogWarning("panels_joinUs.rawHead :" + LobbyContants.ActivitePic + j.data[0].IMG_URL);
                    MahjongCommonMethod.Instance.GetPlayerAvatar(panels_joinUs.rawHead, LobbyContants.ActivitePic + j.data[0].IMG_URL);
                }
                else
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("未找到该地区专员");
                    Debug.LogWarning(j.status);
                }
            });
        }
        public void BtnCloseJoinUsPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            panels_joinUs._input.text = "";
            panels_joinUs._g[0].SetActive(false);
        }
        IEnumerator DownloadQRcode(RawImage raw)
        {
            for (int i = 0; i < 3; i++)
            {
                WWW www = new WWW(string.Format(LobbyContants.QunQRcode, GameData.Instance.PlayerNodeDef.iCityId, UnityEngine.Random.Range(0, 100)));
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogWarning("www.error:" + www.error);
                    raw.texture = Resources.Load<Texture>("QRcode");
                    StopCoroutine("LoadUrlTex");
                    yield return new WaitForSeconds(1.5f);
                    continue;
                }
                else
                {
                    //为图片赋值
                    if (www.texture)
                    {
                        raw.texture = www.texture;
                    }
                    else
                    {
                        Debug.LogWarning("没有图片");
                        yield return new WaitForSeconds(2);
                        continue;
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 点击分享按钮
        /// </summary>
        public void BtnShareWX()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickShare);
            Messenger_anhui.Broadcast(MESSAGE_SHARE);
        }
        /// <summary>
        /// 点击领取礼包按钮
        /// </summary>
        public void BtnGetGiftBag()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickShareGift);
            Messenger_anhui.Broadcast(MESSAGE_GETGIFTBAG);
        }
        /// <summary>
        /// 点击玩法按钮
        /// </summary>
        public void BtnPlayingMethod()
        {

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickPlayMethord);
            GameData.Instance.CreatRoomMessagePanelData.CreatRoomType = 1;
            GameData.Instance.GamePlayingMethodPanelData.CountyId = GameData.Instance.SelectAreaPanelData.iCountyId;
            Messenger_anhui.Broadcast(MESSAGE_PALYINGMETHOD);
        }

        /// <summary>
        /// 点击红包
        /// </summary>
        public void BtnRedPage()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_REDPAGE);
            UIMainView.Instance.RedPagePanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// 点击代理按钮
        /// </summary>
        public void BtnProductAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            PlayerNodeDef gd = GameData.Instance.PlayerNodeDef;
            ////表示玩家是代理
            //if (gd.iIsProxy == 1)
            //{
            //    //表示如果玩家是一级代理，或者是授权代理
            //    if (gd.iDevProxyId == 0 || gd.iProxyId == gd.iUserId)
            //    {
            //        Messenger<int>.Broadcast(MESSAGE_PRODUCTAGENCY, 2);
            //    }
            //    else
            //    {
            //        Messenger<int>.Broadcast(MESSAGE_PRODUCTAGENCY, 1);
            //    }
            //}
            ////玩家不是代理
            //else
            //{
            //    //没有代理
            //    if (gd.iProxyId == 0)
            //    {
            //        Messenger<int>.Broadcast(MESSAGE_PRODUCTAGENCY, 0);
            //    }
            //    //有上级代理
            //    else
            //    {
            //        Messenger<int>.Broadcast(MESSAGE_PRODUCTAGENCY, 1);
            //    }
            //}
        }

        /// <summary>
        /// 点击会员中心按钮
        /// </summary>
        public void BtnProductGene()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickProxyCenter);
            Messenger_anhui<int>.Broadcast(MESSAGE_PRODUCTGENE, 1);
        }

        /// <summary>
        /// 点击客服按钮
        /// </summary>
        public void BtnCustomSever()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickKeFu);
            Messenger_anhui.Broadcast(MESSAGE_CUSTOMSEVERBTN);
        }

        /// <summary>
        /// 点击大厅的消息按钮
        /// </summary>
        public void BtnPlayerMessage()
        {
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在读取信息，请稍候...");
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickMessage);
            Messenger_anhui.Broadcast(MESSAGE_PLAYERMESSAGE);
        }

        /// <summary>
        /// 点击活动按钮
        /// </summary>
        public void BtnFestivalActivity()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickMessage);
            Messenger_anhui.Broadcast(MESSAGE_PLAYERFestivalActivity);
        }

        /// <summary>
        /// 点击节日活动按钮
        /// </summary>
        bool isLoad;
        public void BtnActivity()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Debug.LogWarning("MahjongCommonMethod.isGameToLobby:" + MahjongCommonMethod.isGameToLobby);
            Messenger_anhui.Broadcast(MESSAGE_HolidayACTIVITYBTN);
        }

        /// <summary>
        /// 点击创建房间
        /// </summary>
        public void BtnCreatRoom()
        {
            //if (UIMainView.Instance.LobbyPanel.m_bInGame == false)
            {
                //SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickCreateRoom);
                Messenger_anhui<int>.Broadcast(MESSAGE_CREATROOM, 1);
            }
            //else
            {
                //   UIMgr.GetInstance().GetUIMessageView().Show("您已在预约房等待，此时无法创建房间");
            }
        }

        /// <summary>
        /// 点击返回房间按钮
        /// </summary>
        public void BtnReturnRoom()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_RETURNROOM);
        }

        /// <summary>
        /// 点击加入房间
        /// </summary>
        public void BtnJoinRoom()
        {
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickJoinRoom);
            if (GameData.Instance.LobbyMainPanelData.isJoinedRoom)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您已有牌局，请解散房间后加入！");
                return;
            }
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_JOINROOM);
        }

        /// <summary>
        /// 点击代开房间
        /// </summary>
        public void BtnInsteadCreatRoom()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_INSTEADCREATROOM);
        }

        /// <summary>
        /// 点击战绩按钮
        /// </summary>
        public void BtnHistroyGrade()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickHistory);
            Messenger_anhui.Broadcast(MESSAGE_HISTROYGRADE);
        }
        /// <summary>
        /// 点击头像按钮
        /// </summary>
        public void BtnHeadTitle()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickHead);
            Messenger_anhui.Broadcast(MESSAGE_HEADTITLE);
        }
        public void btnCopyPublicQRcode(Text wx)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.CopyString(wx.text);
            //处理玩家复制成功之后提示文字
            MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }
        /// <summary>
        /// 复制购买房卡的微信公众号
        /// </summary>
        public void btnCopyCompanyWx(Text wx)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.CopyString(wx.text);
            //处理玩家复制成功之后提示文字
            MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }

        public void BtnHealthyPrompt()
        {
            // Debug.Log ("健康提示");
            if (m_HealthyPrompt.transform.localScale.x <= 0.5f)
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                m_HealthyPrompt.transform.DOLocalMove(new Vector3(40, -160, 0), 0.2f);
                m_HealthyPrompt.transform.DOScale(new Vector3(1, 1, 1), 0.2f);
                StartCoroutine(DoBtnHealthyPromptClose());
            }
            //else if (m_HealthyPrompt.transform.localScale.x >= 1.0f)
            //{
            //    SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            //    m_HealthyPrompt.transform.DOLocalMove(Vector3.zero, 0.2f);
            //    m_HealthyPrompt.transform.DOScale(new Vector3(0.5f,0.5f,1), 0.2f);
            //}
        }

        public void CloseHealthyPrompt()
        {
            StartCoroutine(DoBtnHealthyPromptClose());
        }

        IEnumerator DoBtnHealthyPromptClose()
        {
            yield return new WaitForSeconds(4.0f);
            m_HealthyPrompt.transform.DOLocalMove(new Vector3(0, -24.0f, 0), 0.2f);
            m_HealthyPrompt.transform.DOScale(new Vector3(0.5f, 0.5f, 1), 0.2f);
        }

        #region 充值相关

        /// <summary>
        /// 刷新代金券
        /// </summary>
        /// <param name="type">1未点击代金券  2选取过代金券</param>
        /// <param name="num">代金券数量</param>
        /// <param name="a">原价</param>
        /// <param name="b">现价</param>
        public void ComputeMoney(int type, int num, float a, float b)
        {
            if (type == 1)
            {
                panel_Charge._tFri.text = num == 0 ? "无可用代金券" : "代金券 " + num + " 张可用";
                panel_Charge._tSec.text = "价格： <color=#c83a0c>" + a + "</color>  元";
                panel_Charge.Line.gameObject.SetActive(false);
            }
            if (type == 2)
            {

                panel_Charge._tFri.text = "已使用代金券: " + b + " 元";
                panel_Charge.Line.gameObject.SetActive(true);
                panel_Charge._tSec.text = "价格： <color=#c83a0c>" + a + "</color> 元   现价：<color=#c83a0c><size=36>" + (a - b) + "</size></color> 元";
            }

        }

        public void OpenChargePanel()
        {
            LobbyMainPanelData md = GameData.Instance.LobbyMainPanelData;
            //每次点击要刷新字段
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.ClickCharge);
            if (SDKManager.Instance.CheckStatus == 1)
            {
#if UNITY_ANDROID
                if (GameData.Instance.PlayerNodeDef.byUserSource == 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show("微信登陆后，才可以购买金币哦！");
                    return;
                }
                //跳转网页
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                str.Append("https://sxver.ibluejoy.com/m/clientLogin.shtml?");
                str.Append("t=3&uid=");
                str.Append(GameData.Instance.PlayerNodeDef.iUserId);
                str.Append("&token=");
                str.Append(GameData.Instance.PlayerNodeDef.userDef.szAccessToken);
                Application.OpenURL(str.ToString());
#elif UNITY_IOS
                Messenger.Broadcast(MESSAGE_BUYROOMCARD);
#endif

            }
            else
            {
                Messenger_anhui.Broadcast(MESSAGE_BUYROOMCARD);
            }
            int a = GameData.Instance.PlayerNodeDef.iMyParlorId > 0 ? 2000 : 1000;

            for (int i = 0; i < panel_Charge.chargeGroup.Length; i++)
            {
                panel_Charge.chargeGroup[i].index = i + 1 + a;
                panel_Charge.chargeGroup[i]._tCoinNum.text = MahjongCommonMethod.Instance._dicCharge[panel_Charge.chargeGroup[i].index].Coin + "";
                panel_Charge.chargeGroup[i]._tMoney.text = MahjongCommonMethod.Instance._dicCharge[panel_Charge.chargeGroup[i].index].Price * 0.01f + " 元";
                int coin = md.isShowSendCoin ? MahjongCommonMethod.Instance._dicCharge[panel_Charge.chargeGroup[i].index].Coin : MahjongCommonMethod.Instance._dicCharge[panel_Charge.chargeGroup[i].index].Present;
                if (coin > 0)
                {
                    panel_Charge.chargeGroup[i]._tPresent.text = coin + "";
                    panel_Charge.chargeGroup[i]._tPresent.gameObject.SetActive(true);
                }
                else
                {
                    panel_Charge.chargeGroup[i]._tPresent.gameObject.SetActive(false);
                }
            }
            //事件添加-点击充值按钮
            Debug.LogWarning("iMyParlorId" + GameData.Instance.PlayerNodeDef.iMyParlorId);
            if (GameData.Instance.PlayerNodeDef.iMyParlorId > 0&&Application.platform==RuntimePlatform.IPhonePlayer )
            {
                foreach (var item in panel_Charge.chargeGroup)
                {
                    Debug.LogWarning(item.index);
                    //UnityEngine.Object d = new UnityEngine .Object();
                    EventTriggerListener.Get(item.btnBuy.gameObject, (item.index).ToString()).onClick_Sring = BtnOpenChargeFrame;
                }

            }
            else
            {
                foreach (var item in panel_Charge.chargeGroup)
                {
                    Debug.LogWarning(item.index);
                    //UnityEngine.Object d = new UnityEngine .Object();
                    EventTriggerListener.Get(item.btnBuy.gameObject, (item.index).ToString()).onClick_Sring = btnOpenPayChannelPanel;
                }

            }
            panel_Charge._gPanelCharge.SetActive(true);
        }
        void BtnOpenChargeFrame(GameObject obj)
        {
            UIMgr.GetInstance().GetUIMessageView().Show("馆主请前往微信公众号【双喜麻将】充值");
        }
        Stack stackVoucher = new Stack();
        /// <summary>
        /// 打开代金券面板
        /// </summary>
        /// <param name="amount"> 0-钱包</param>
        public void btnOpenVoucherPanel(int amount)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
#if UNITY_ANDROID
            panel_voucher._g.SetActive(true);
            if (amount == 1)//充值
            {
                return;
            }
            GetVoucherData(amount);
#else
            MahjongCommonMethod.Instance.ShowRemindFrame("IOS 无法使用");
#endif


        }
        /// <summary>
        /// 关闭代金券面板
        /// </summary>
        public void btnCloseVoucherPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            panel_voucher._g.SetActive(false);
        }

        public Stack stackprice = new Stack();
        /// <summary>
        /// 打开选择充值渠道面板
        /// </summary>
        /// <param name="price">金额数量</param>
        public void btnOpenPayChannelPanel(GameObject iChargeId)
        {

            int num = int.Parse(EventTriggerListener.Get(iChargeId).str);
            Debug.LogWarning(num);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
#if UNITY_ANDROID
            LobbyMainPanelData md = GameData.Instance.LobbyMainPanelData;
            md.isShowChoiceChannelPanel = true;
            //  stackVoucher.Clear();//充值之前清空代金券保存值
            NetMsg.ClientCreateOrderRes res = new NetMsg.ClientCreateOrderRes();
            res.iChargeId = num;
            res.iChargeNumber = (int)(MahjongCommonMethod.Instance._dicCharge[num].Price);
            res.iBoss = GameData.Instance.PlayerNodeDef.iMyParlorId > 0 ? 1 : 0;
            stackprice.Push(res);
            // chargeNum = (stackprice.Peek() as NetMsg.ClientCreateOrderRes).iChargeNumber;
            if (stackVoucher.Count > 0)
            {
                LobbyMainPanelData. IVoucherData ivd= (LobbyMainPanelData.IVoucherData)stackVoucher.Peek  ();
                Debug.LogWarning("点击代金券" + res.iChargeNumber / 100 + "________" + ivd.amount);
                ComputeMoney(2, 0, res.iChargeNumber / 100, ivd.amount);
            }
            else
            {
                GetVoucherData(1);
            }
          
            UpdateShow();
            GetVoucherData(1);
#else
            btnIOSPay(num);
#endif

        }
        internal bool isOnPay;


        public void btnIOSPay(int id)
        {
            if (isOnPay)
            {
                return;
            }
            isOnPay = true;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui<int>.Broadcast(MESSAGE_BUYCARD, id);
            StartCoroutine(PayDelayDel());
        }
        IEnumerator PayDelayDel()
        {
            MahjongCommonMethod.Instance.ShowRemindFrame("正在充值请耐心等待...");
            yield return new WaitForSeconds(30f);
            isOnPay = false;
        }
        /// <summary>
        /// 点击选择充值渠道
        /// </summary>
        /// <param name="model">1苹果充值，2微信充值，3支付宝充值</param>
        public void btnClickPay(int model)
        {
            if (stackprice.Count > 0)
            {
                //Debug.LogError(stackprice.Pop());
                Debug.LogWarning("开始");
                NetMsg.ClientCreateOrderRes res = stackprice.Pop() as NetMsg.ClientCreateOrderRes;
                NetMsg.ClientCreateOrderReq req = new NetMsg.ClientCreateOrderReq();
                req.iUserId = MahjongCommonMethod.Instance.iUserid;
                res.iUserId = MahjongCommonMethod.Instance.iUserid;
                req.iChargeMode = model;
                res.iChargeMode = model;
                req.iChargeId = res.iChargeId;
                Debug.LogWarning("代金券" + stackVoucher.Count);
                if (stackVoucher.Count > 0)
                {
                    LobbyMainPanelData.IVoucherData data = (LobbyMainPanelData.IVoucherData)stackVoucher.Pop();
                    req.iVoucherAmount = data.amount;
                    res.iVoucherAmount = data.amount;
                    req.iUserVoucherId = data.voucherId;
                    res.iUserVoucherId = data.voucherId;
                    req.iChargeNumber = res.iChargeNumber - req.iVoucherAmount * 100;
                    res.iChargeNumber = res.iChargeNumber - req.iVoucherAmount * 100;
                }
                else
                {
                    req.iUserVoucherId = 0;
                    res.iUserVoucherId = 0;
                    req.iVoucherAmount = 0;
                    res.iVoucherAmount = 0;
                    req.iChargeNumber = res.iChargeNumber;
                }
                Debug.LogWarning("req.iVoucherAmount" + req.iVoucherAmount);
                Debug.LogWarning("data.voucherId" + req.iUserVoucherId);
                Debug.Log("PlayerNodeDef.iMyParlorId)" + GameData.Instance.PlayerNodeDef.iMyParlorId);
                Debug.Log("PlayerNodeDef.iMyParlorId.userDef )" + GameData.Instance.PlayerNodeDef.userDef.iMyParlorId);
                req.iBoss = res.iBoss;//GameData.Instance.PlayerNodeDef.iMyParlorId > 0 ? 1 : 0;
                Debug.LogWarning("iChargeId" + req.iChargeId);
                Debug.LogWarning("iChargeNumber" + req.iChargeNumber);
                Debug.LogWarning("iBoss" + req.iBoss);
                GameData gd = GameData.Instance;
                if (!MahjongCommonMethod.Instance._dicCharge.ContainsKey(req.iChargeId))
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("没有该项充值");
                    return;
                }
                NetMsg.Charge payInfo = MahjongCommonMethod.Instance._dicCharge[req.iChargeId];
                req.szOrderInfo = StringForChargeInfo(payInfo.Coin, req.iChargeNumber, model);
                if (string.IsNullOrEmpty(req.szOrderInfo))
                {
                    MahjongCommonMethod.Instance.ShowRemindFrame("创建订单失败");
                    return;
                }
                if (req.szOrderInfo == null)
                {
                    Debug.LogWarning("szOrderInfo为空");
                    MahjongCommonMethod.Instance.ShowRemindFrame("创建订单失败");
                    return;
                }
                //Debug.LogWarning("++++++++"+req.szOrderInfo);
                stackprice.Clear();
                stackprice.Push(res);
                SDKManager.Instance.CreatChargeOderReq(req);
               // NetworkMgr.Instance.LobbyServer.SendCreateOrderReq(req);
                btnCloseChoicePayChannelPanel();
                BtnCloseBuyRoomCard();
            }
        }
        bool Print()
        {
            int a = 3;
            int b = 5;

            return a > b;
        }
        /// <summary>
        /// 订单所需信息的字符串
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        string StringForChargeInfo(int Coin, int Price, int model)
        {

            if (MahjongCommonMethod.Instance.SeverIp.Length == 0)
            {
                MahjongCommonMethod.Instance.SeverIp = "192.168.1.1";
            }
            if (model == 3)//支付宝
            {
                string resoult = "";
                resoult += "subject=" + "金币 " + Coin + "个" + "&"; //标题
                                                                  // if (payInfo.Limit == 2)
                                                                  // {
                                                                  // if (SdkManager.SDkManager.userChargeid.Contains(id))
                                                                  // {
                                                                  //    resoult += "body=" + payInfo.Name + "&";
                                                                  // }
                                                                  //  else
                                                                  // {
                                                                  //     resoult += "body=" + payInfo.Info + "&"; //介绍
                                                                  // }
                                                                  //  }
                                                                  // else
                                                                  // {
                resoult += "body=" + "金币 " + Coin + "个" + "&";
                // resoult += "timeout_express=10m&"  ;//过期时间10分钟
                resoult += "it_b_pay=10m&";
                //  }
                resoult += "total_amount=" + ((float)Price) / 100;//价格
                                                                  // resoult += "product_code=" + payInfo.Price;  //产品码    
                                                                  // resoult += "product_code=QUICK_MSECURITY_PAY";  //产品码
                                                                  // Debug.LogError("        StringForChargeInfo  " + resoult);
                return resoult;
            }
            if (model == 2)//微信
            {
                StringBuilder result = new StringBuilder();
                string time = GetTenMiniteLaterTime();
                //0.商品描述，1.总金额，2.终端IP，3.交易类型
                result.AppendFormat("body={0}&total_fee={1}&spbill_create_ip={2}&trade_type={3}&time_expire={4}", "金币 " + Coin + "个", (float)Price, MahjongCommonMethod.Instance.SeverIp, "APP", time);
                return result.ToString();
            }
            return null;

        }

        private string GetTenMiniteLaterTime()
        {
            string time = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now.AddMinutes(10f)).ToString();
            Debug.LogWarning("DateTime.Now.AddMinutes (10f)" + DateTime.Now.AddMinutes(10f));
            MahjongCommonMethod.Instance.GetTime(() =>
            {
                time = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(MahjongCommonMethod.Instance._DateTime.AddMinutes(10f)).ToString();
            });
            Debug.LogWarning("_DateTime" + time);
            return time;
        }

        //关闭选择渠道面板       
        public void btnCloseChoicePayChannelPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            LobbyMainPanelData md = GameData.Instance.LobbyMainPanelData;
            md.isShowChoiceChannelPanel = false;
            UpdateShow();
            Image[] obj = panel_voucher.toggleGroup_VoucherGroup.transform.GetComponentsInChildren<Image>(false);
            for (int i = 0; i < obj.Length; i++)
            {
                Destroy(obj[i].gameObject);
            }
        }
        //点击关闭购买房卡界面
        public void BtnCloseBuyRoomCard()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBUYROOMCARD);
        }
        #endregion 充值相关

        //点击切换地区
        public void BtnChangeAreaSelect(GameObject obj)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            //打开选择城市面板
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            sapd.iOpenStatus = 5;
            sapd.pos_index = 0;
            sapd.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
        }


        //点击进入麻将馆
        public void BtnOpenParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
        }

        #endregion 按钮点击  

        /// <summary>
        /// 红包是不是有
        /// </summary>
        IEnumerator OnSetRedPageState()
        {
            while (true)
            {
                int num = 0;
                for (int i = 0; i < UIMainView.Instance.RedPagePanel.RedPage.Count; i++)
                {
                    num += UIMainView.Instance.RedPagePanel.RedPage[i].CanUseNum + UIMainView.Instance.RedPagePanel.RedPage[i].ShareNum;
                }

                if (num > 0 && SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                {
                    RedPage.SetActive(true);
                }
                else
                {
                    RedPage.SetActive(false);
                }
                yield return new WaitForSeconds(1.0f * 60);

                //获取红包数量
                Messenger_anhui.Broadcast(MESSAGE_REDPAGE);
            }
        }


        /// <summary>
        /// 发送兑换请求
        /// </summary>
        /// <param name="str"> ExchangeID</param>
        void SendExchage(GameObject str)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string ss = EventTriggerListener.Get(str).str;
            NetMsg.ClientScoreToCoinReq msg = new NetMsg.ClientScoreToCoinReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iExchangeId = int.Parse(ss.Split(',')[0]);
            msg.byExchange = byte.Parse(ss.Split(',')[1]);
            Debug.LogWarning(msg.iUserId + "/" + msg.iExchangeId + "/" + msg.byExchange);
            NetworkMgr.Instance.LobbyServer.SendClientScoreToCoinReq(msg);
        }
        //关闭钱包
        void OnCloseWallet()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            panel_wallet._gPanelWallet.SetActive(false);//关闭钱包面板
        }

        #region 购买金币
        [Serializable]
        public class PanelCharge
        {
            public Button btnLobbyBuyCoin;
            public GameObject _gPanelCharge;
            public Button btnClose;
            /// <summary>
            /// 0-5 6充值模块
            /// </summary>
            public ChargeGroup[] chargeGroup;
            public GameObject _gChoiceChannelPanel;
            public Image Line;
            public Text _tFri;
            public Text _tSec;
            /// <summary>
            /// 0-微信 1支付宝
            /// </summary>
            public Button[] btnChannel;
            public Button btnCloseChannel;
            public Button btnOpenVoucher;
            public Text _tDoubleTip;
            public PanelCharge()
            {

            }
        }
        [Serializable]
        public class ChargeGroup
        {
            public GameObject _g;
            internal int index;
            /// <summary>
            /// 显示金币
            /// </summary>
            public Text _tCoinNum;
            /// <summary>
            /// 赠送金币
            /// </summary>
            public Text _tPresent;
            public Image _imgIcon;
            /// <summary>
            /// 钱
            /// </summary>
            public Text _tMoney;
            public Button btnBuy;
        }
        #endregion 购买金币

        #region WalletPanel
        [System.Serializable]
        public class WalletPanel
        {
            /// <summary>
            /// 钱包面板
            /// </summary>
            public GameObject _gPanelWallet;
            public PanelCashToCoin panelCashToCoin;
            /// <summary>
            /// 大厅钱包按钮
            /// </summary>
            public Button btnLobbyWallet;
            /// <summary>
            /// 0-现金 1-话费 2-流量  3代金券 4储值卡
            /// </summary>
            public WalletType[] walletType;

            public PanelReminder panelremind;
            /// <summary>
            /// 关闭按钮
            /// </summary>
            public Button btnClose;
        }
        [System.Serializable]
        public class PanelReminder
        {
            public GameObject _gPanelRemind;
            public Text t;
            public Button btnClose;
            public Button btnok;
        }
        [System.Serializable]
        public class WalletType
        {
            public Text _tNum;
            /// <summary>
            /// 0兑换 1提现
            /// </summary>
            public Button[] btn;
        }
        [System.Serializable]
        public class PanelCashToCoin
        {
            public GameObject _gPanelCashToCoin;
            public Text _tTypeDis;
            public Text currentCash;
            public Button btnClose;
            public Button[] btn_exchage;
            public Text[] tPrice;
            public Text[] tCoin;
            public GameObject[] _gbox;
        }
        #endregion WalletPanel

        #region JoinUsPanel
        [System.Serializable]
        public class JoinUsPanel
        {
            /// <summary>
            /// 0-panel 1-group 3mask
            /// </summary>
            public GameObject[] _g;
            /// <summary>
            /// 0-总标题
            /// </summary>
            public Button btnCoppyWeChat;
            public Button btnCoppyTel;

            #region 读取后台
            public Text _tCounty;
            public RawImage rawHead;
            public Text _tNickName;
            public Text _tWeChatNum;
            public Text _Tel;
            //public Text _tCounty;
            #endregion 读取后台

            //0-输入手机号
            public InputField _input;
            /// <summary>
            /// 0-提交手机号
            /// </summary>
            public Button btn;
            public Button btnLianxiKefu;
            public Toggle[] _toglePoint;
            internal Vector3 v3_currentPos;
        }
        #endregion JoinUsPanel
    }




}

