using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using MahjongGame_AH.UISystem;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

//需重新配置
namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    /// <summary>
    /// 消息框配置
    /// </summary>
    class MessageBoxConfig
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg;
        /// <summary>
        /// 按钮类型
        /// </summary>
        public UIMessageView.ButtonType BtnType;
        /// <summary>
        /// 确定按钮的委托函数
        /// </summary>
        public UIMessageView.ActionFunc OnOkFunc;
        /// <summary>
        /// 取消按钮的委托函数
        /// </summary>
        public UIMessageView.ActionFunc OnCancelFunc;
        /// <summary>
        /// 倒计时
        /// </summary>
        public int Time;
        /// <summary>
        /// 默认按钮是否是确认
        /// </summary>
        public bool DefaultBtnIsOk;
        /// <summary>
        /// 0默认显示“确认”，1显示玩家退出框
        /// </summary>
        public int OkBtnLab;
        /// <summary>
        /// 0默认显示“取消”，1显示玩家退出框
        /// </summary>
        public int CancelBtnLab;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="str">消息内容</param>
        /// <param name="bt">按钮类型</param>
        /// <param name="ok">确定按钮的委托函数</param>
        /// <param name="cancel">取消按钮的委托函数</param>
        /// <param name="defaultOK">默认按钮是否是确认</param>
        /// <param name="t">倒计时</param>
        public MessageBoxConfig(string title, string msg, UIMessageView.ButtonType bt, UIMessageView.ActionFunc ok,
        UIMessageView.ActionFunc cancel, bool defaultOK, int t, int okBtnLab, int cancelBtnLab)
        {
            Title = title;
            Msg = msg;
            BtnType = bt;
            OnOkFunc = ok;
            OnCancelFunc = cancel;
            DefaultBtnIsOk = defaultOK;
            Time = t;
            OkBtnLab = okBtnLab;
            CancelBtnLab = cancelBtnLab;
        }
    }

    public class UIMessageView : BaseUI
    {
        /// <summary>
        /// 消息内容的标题
        /// </summary>
        public Text LblTitle;
        /// <summary>
        /// 消息内容的标签
        /// </summary>
        public Text LblMsg;
        /// <summary>
        /// 倒计时的标签
        /// </summary>
        public Text LblTimer;
        /// <summary>
        /// 两个按钮时的取消按钮
        /// </summary>
        public GameObject BtnCancel;
        /// <summary>
        /// 两个按钮时的确定按钮
        /// </summary>
        public GameObject BtnOK2;
        /// <summary>
        /// （确认。取消）取消按钮文字
        /// </summary>
        public Text BtnCancelLab;
        /// <summary>
        /// （确认。取消）确认按钮文字
        /// </summary>
        public Text BtnOK2Lab;
        /// <summary>
        /// 一个确认按钮文字
        /// </summary>
        public Text BtnOK1Lab;
        /// <summary>
        /// 一个按钮时的确定按钮
        /// </summary>
        public GameObject BtnOK1;


        /// <summary>
        /// 单例
        /// </summary>
        public static UIMessageView instance;

        //消息的内容
        string MsgStr = "";
        bool msgBoxShow; //消息框是否已经在显示
        string MsgTitle = "";
        string OK2Lab = "";
        string CancelLab = "";
        //按钮类型的定义
        public enum ButtonType
        {
            BT_NULL,
            BT_OK = 1,
            BT_OK_CANCEL = 2,
        }
        /// <summary>
        /// 显示的按钮类型
        /// </summary>
        ButtonType btnType = ButtonType.BT_NULL;
        /// <summary>
        /// 默认按钮是否是确定
        /// </summary>
        bool defaultBtnOK;

        ///委托动作函数类型
        public delegate void ActionFunc();
        ActionFunc okFunc; // 点确定后的处理函数
        ActionFunc cancelFunc; // 点取消后的处理函数
        bool clockEnable = false; // 是否倒计时
        int timeLeft; // 剩余时间有多少
        const int DEFAULT_TIME_LEN = -1; // 倒计时默认时间
        bool CancelIs = false; //是否有取消
        List<MessageBoxConfig> allRequestList = new List<MessageBoxConfig>(); // 请求显示MsgBox的所有请求都会加入到此列表

        Vector3 timerPos1 = new Vector3(66, -50, 0); //计时器位置1
        Vector3 timerPos2 = new Vector3(0, -50, 0); //计时器位置2

        #region 父类的方法

        /// <summary>
        /// 获取UI面板的编号
        /// </summary>
        /// <returns>编号</returns>
        public override ushort GetID()
        {
            return UIConstant.UIID_MAHJONG_GAME_GENERAL_MESSAGE;
        }

        /// <summary>
        /// 视图已经显示，在这可以初始化一些东西
        /// </summary>
        protected override void ViewDidAppear()
        {
            msgBoxShow = true;
        }

        /// <summary>
        /// 视图即将消失，在这可以保存一些东西
        /// </summary>
        protected override void ViewWillDisappear()
        {
            msgBoxShow = false;
        }

        /// <summary>
        /// 关闭自己
        /// </summary>
        protected override void CloseSelf()
        {
            gameObject.SetActive(false);
            base.CloseSelf();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                DEBUG.GuiMessage("UIMessageView是单例，不能多次初始化。", LogType.Error);
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        new void Start()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -200);
            transform.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        #endregion 父类的方法

        #region 显示消息框的方法

        /// <summary>
        /// 只用输入要显示的内容，默认标题显示温馨提示，确定，取消默认状态不显示倒计时,只显示一个确定按钮
        /// </summary>
        /// <param name="msg"></param>
        public void Show(string msg)
        {
            Show(TextConstant.GENERA_TITILE, msg, null, null, ButtonType.BT_OK, true, DEFAULT_TIME_LEN, 0, 0);
        }

        /// <summary>
        /// 显示标题框，输入内容，其他默认，只显示一个确定按钮
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public void Show(string title, string msg)
        {
            Show(title, msg, null, null, ButtonType.BT_OK, true, DEFAULT_TIME_LEN, 0, 0);
        }

        /// <summary>
        /// 输入标题框，显示内容，还有倒计时，只显示一个确定按钮
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="clock"></param>
        public void Show(string title, string msg, int clock)
        {
            Show(title, msg, null, null, ButtonType.BT_OK, true, clock, 0, 0);
        }

        /// <summary>
        /// 显示标题，内容，确认按钮的操作，只显示一个确定按钮
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        public void Show(string title, string msg, ActionFunc ok)
        {
            Show(title, msg, ok, null, ButtonType.BT_OK, true, DEFAULT_TIME_LEN, 0, 0);
        }

        /// <summary>
        /// 只显示内容，确定按钮调用方法，只显示一个确定按钮，不显示倒计时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        public void Show(string msg, ActionFunc ok)
        {
            //Debug.LogError("========================-1");
            Show(TextConstant.GENERA_TITILE, msg, ok, null, ButtonType.BT_OK, true, DEFAULT_TIME_LEN, 0, 0);
        }

        /// <summary>
        /// 显示内容，确认按钮调用方法，自己选择按钮选择方式，不显示倒计时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        /// <param name="buttonType"></param>
        public void Show(string msg, ActionFunc ok, ButtonType buttonType)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, null, buttonType, false, DEFAULT_TIME_LEN, 0, 0);
        }

        /// <summary>
        /// 只显示一个确定按钮，显示内容，倒计时时间
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        /// <param name="clock"></param>
        public void Show(string msg, ActionFunc ok, int clock)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, null, ButtonType.BT_OK, true, clock, 0, 0);
        }

        /// <summary>
        /// 自己输入显示内容，调用确认按钮方法，倒计时时间，确认按钮显示文字
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        /// <param name="clock"></param>
        /// <param name="OkBtnLab"></param>
        public void Show(string msg, ActionFunc ok, int clock, int OkBtnLab)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, null, ButtonType.BT_OK, true, clock, OkBtnLab, 0);
        }

        /// <summary>
        /// 玩家自己输入标题，显示内容，调用显示按钮方法，倒计时时间,只显示一个按钮
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        /// <param name="clock"></param>
        public void Show(string title, string msg, ActionFunc ok, int clock)
        {
            Show(title, msg, ok, null, ButtonType.BT_OK, true, clock, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ok"></param>
        /// <param name="buttonType"></param>
        /// <param name="clock"></param>
        public void Show(string msg, ActionFunc ok, ButtonType buttonType, int clock)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, null, buttonType, false, clock, 0, 0);
        }

        public void Show(string msg, ActionFunc ok, ActionFunc cancel)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, cancel, ButtonType.BT_OK_CANCEL, false, DEFAULT_TIME_LEN, 0, 0);
        }
        public void Show(string msg, ActionFunc ok, ActionFunc cancel, int OkBtnLab, int CancelBtnLab)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, cancel, ButtonType.BT_OK_CANCEL, false, DEFAULT_TIME_LEN, OkBtnLab, CancelBtnLab);
        }
        public void Show(string msg, ActionFunc ok, ActionFunc cancel, bool defaultOK)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, cancel, ButtonType.BT_OK_CANCEL, defaultOK, DEFAULT_TIME_LEN, 0, 0);
        }

        public void Show(string msg, ActionFunc ok, ActionFunc cancel, int clock)
        {
            Show(TextConstant.GENERA_TITILE, msg, ok, cancel, ButtonType.BT_OK_CANCEL, false, clock, 0, 0);
        }

        /// <summary>
        /// 显示通用消息弹框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        /// <param name="ok">确定按钮调用的方法，默认是直接关闭面板</param>
        /// <param name="cancel">取消按钮调用的方法，默认是直接关闭面板</param>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="defaultOK">是否默认显示确定按钮</param>
        /// <param name="clock">倒计时时间，-1表示不显示倒计时</param>
        /// <param name="OkBtnLab">确定按钮显示内容</param>
        /// <param name="CancelBtnLab">取消按钮显示内容</param>
        public void Show(string title, string msg, ActionFunc ok, ActionFunc cancel, ButtonType buttonType, bool defaultOK, int clock, int OkBtnLab, int CancelBtnLab)
        {
            MessageBoxConfig config;

            if (buttonType == ButtonType.BT_OK)
            {
                config = new MessageBoxConfig(title, msg, ButtonType.BT_OK, ok, null, defaultOK, clock, OkBtnLab, CancelBtnLab);
            }
            else
            {
                config = new MessageBoxConfig(title, msg, ButtonType.BT_OK_CANCEL, ok, cancel, defaultOK, clock, OkBtnLab, CancelBtnLab);
            }
            allRequestList.Add(config);
            //Debug.LogError("消息框长度"+allRequestList.Count);
            if (!msgBoxShow)
            {
                _fetchNewRequest();
            }
        }

        #endregion 显示消息框的方法

        #region 私有成员方法

        /// <summary>
        /// 每秒执行一次的定时器
        /// </summary>
        void OnTimer()
        {
            if (clockEnable)
            {
                timeLeft--;
                if (timeLeft > 0)
                {
                    LblTimer.text = timeLeft.ToString();
                }
                else
                {
                    if (btnType == ButtonType.BT_OK_CANCEL && !defaultBtnOK)
                    {
                        BtnCancelOnClick();
                    }
                    else
                    {
                        BtnOkOnClick();
                    }
                }
            }
        }

        protected override void Update()
        {
            if (btnType == ButtonType.BT_OK)
            {
                if (Input.GetKeyDown(KeyCode.Return)
                    || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    BtnOkOnClick();
                }
            }
        }

        /// <summary>
        /// 接受新的请求
        /// </summary>
        void _fetchNewRequest()
        {
            if (allRequestList.Count == 0)
            {
                CancelInvoke("OnTimer");
                CloseSelf();
                return;
            }

            clockEnable = allRequestList[0].Time >= 0;
            timeLeft = allRequestList[0].Time;
            okFunc = allRequestList[0].OnOkFunc;
            cancelFunc = allRequestList[0].OnCancelFunc;
            MsgTitle = allRequestList[0].Title;
            MsgStr = allRequestList[0].Msg;
            btnType = allRequestList[0].BtnType;
            defaultBtnOK = allRequestList[0].DefaultBtnIsOk;


            switch (allRequestList[0].OkBtnLab)
            {
                case 0:
                    OK2Lab = "确 认";
                    break;
                case 1:
                    OK2Lab = "确 定";
                    break;
                case 2:
                    OK2Lab = "同 意";
                    break;
                default:
                    break;
            }
            switch (allRequestList[0].CancelBtnLab)
            {
                case 0:
                    CancelLab = "取 消";
                    CancelIs = false;
                    break;
                case 1:
                    CancelLab = "取 消";
                    CancelIs = false;
                    break;
                case 2:
                    CancelLab = "拒 绝";
                    CancelIs = false;
                    break;
                default:
                    break;
            }
            allRequestList.RemoveAt(0);

            BtnOK2Lab.text = OK2Lab;
            BtnOK1Lab.text = OK2Lab;
            BtnCancelLab.text = CancelLab;
            LblMsg.text = MsgStr;
            LblTitle.text = MsgTitle;
            if (clockEnable)
            {
                LblTimer.text = timeLeft.ToString();
                if (btnType == ButtonType.BT_OK_CANCEL)
                {
                    LblTimer.transform.localPosition = timerPos2;
                }
                else
                {
                    LblTimer.transform.localPosition = timerPos1;
                }
            }
            else
            {
                LblTimer.text = "";
            }
            if (btnType == ButtonType.BT_OK_CANCEL)
            {
                BtnCancel.SetActive(true);
                BtnOK2.SetActive(true);
                BtnOK1.SetActive(false);
            }
            else if (btnType == ButtonType.BT_OK)
            {
                BtnCancel.SetActive(false);
                BtnOK2.SetActive(false);
                BtnOK1.SetActive(true);
            }

            else
            {
                BtnCancel.SetActive(false);
                BtnOK2.SetActive(false);
                BtnOK1.SetActive(false);
            }
            gameObject.SetActive(true);

            InvokeRepeating("OnTimer", 1, 1);
        }

        #endregion 私有成员方法

        #region 按钮事件处理

        /// <summary>
        /// 当点击确定按钮
        /// </summary>
        public void BtnOkOnClick()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            DEBUG.Gui("BtnOk_OnClick");
            if (okFunc != null)
            {
                okFunc();
            }
            _fetchNewRequest();
        }

        /// <summary>
        /// 当点击取消按钮
        /// </summary>
        public void BtnCancelOnClick()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            DEBUG.Gui("OnClick_Cancel");
            if (cancelFunc != null)
            {
                cancelFunc();
            }
            _fetchNewRequest();
        }

        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void BtnCloseOnClick()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            if (btnType == ButtonType.BT_OK_CANCEL)
            {
                Debug.LogError("CancelIs " + CancelIs);
                if (CancelIs)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    BtnCancelOnClick();
                }

            }
            else
            {
                BtnOkOnClick();
            }
        }

        #endregion 按钮事件处理
    }
}