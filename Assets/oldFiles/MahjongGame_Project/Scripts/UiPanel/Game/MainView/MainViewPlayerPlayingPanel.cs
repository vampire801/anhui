using UnityEngine;
using System.Collections;
using MahjongGame_AH.Data;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using DG.Tweening;
using MahjongGame_AH.GameSystem.SubSystem;
using System;
using Spine.Unity;
using System.Runtime.InteropServices;
using DG.Tweening.Core;
using MahjongLobby_AH;
using MahjongGame_AH.Network.Message;
using XLua;
using anhui;
namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewPlayerPlayingPanel : MonoBehaviour
    {
        public GameObject _gHongzhongZhongma;
        //房间id
        public Text[] _RoomId;
        //房间开放时间
        public Text NowTime;
        //游戏名称等待界面
        // public Text GameName;
        //游戏名称游戏界面
        public Text GameName_playing;
        //游戏名称游戏界面
        public Image GameName_playing_bg;
        //中间用于显示场次，玩家出牌东南西北，时间等父物体
        public Transform _TimeRemi;
        //语音按钮
        public GameObject ButtonVoice;
        //胡牌的提示按钮
        public GameObject btn_tingpai;
        //放置胡牌之后手牌的初始位置
        public GameObject Win_MahjongFirstPos;

        //用户昵称
        public Text[] _txUsersNickName = new Text[4];
        //玩家分数分数
        public Text[] _txUsersScor = new Text[4];
        //玩家准备状态
        public Image[] ReadyImage = new Image[4];
        //没有玩家的显示
        public GameObject[] NonePlayer = new GameObject[4];
        //玩家头像的父物体
        public Transform[] _playingHead = new Transform[4];
        //花牌显示信息
        public Image[] Flower = new Image[4];
        public GameObject CanelHostingBtn; //取消托管按钮
        public Image[] PlayerOutRoom = new Image[4];//玩家是在房间内还是在房间外
        //public Text[] PlayerInSeat = new Text[4];//座位是不是被占了
        public GameObject[] PlayerSeat = new GameObject[4];//座位是不是占了
        public GameObject[] PlayerReady = new GameObject[2];//四个玩家到齐，出现准备按钮  第一个是邀请按钮 第二个是准备按钮
        public GameObject PlayerReadyTimeLost;//玩家准备时间倒数开始
        float StartPlayerHereReadyTime = 60;//等待时间60秒
        public GameObject[] xiafen = new GameObject[4];//下分的显示  下两分还是下一分

        //房间规则按钮
        public Transform RuleButton;

        //玩家头像变更位置
        public Transform[] _playingChangeHead = new Transform[4];
        //四个玩家的输赢信息显示
        public Image[] HeadIamge = new Image[4];
        //用来显示玩家输赢信息的详细图片
        public Sprite[] LastRemind;
        //四个玩家的牌的父物体
        public Transform[] _Cards = new Transform[4];
        //特殊牌的提示0吃 1碰 2杠 3胡 4听 5过
        public GameObject[] SpecialTileNotice;
        //特殊牌提示的背景框
        public Image SpecialTileNoticeBg;
        //提示吃碰杠胡的位置
        public GameObject[] specialPos;
        //提示吃碰杠胡的位置
        public GameObject[] specialPos_effect;
        //自摸位置
        public GameObject[] specialPos_zimo;
        //胡牌位置
        public GameObject[] specialPos_hu;
        //吃的特效的位置
        public GameObject[] specialPos_Chi;
        //放炮的特殊的位置
        public GameObject[] specialPos_fangpao;
        //补花的位置
        public GameObject[] specialPos_buhua;
        /// <summary>
        /// 补杠 抢杠胡的位置
        /// </summary>
        public GameObject[] speaiclBuGangPos;
        //及时更新分数的位置
        public GameObject[] specialScorePos;
        //特殊牌的父物体
        public GameObject SpecialTitleParent;
        //听牌的提示
        public GameObject TingShow;
        //吃牌提示
        public GameObject ChiShow;

        //万牌的所有图片
        public Sprite[] _wanNum = new Sprite[9];
        //筒牌的所有图片
        public Sprite[] _tonNum = new Sprite[9];
        //条牌的所有图片
        public Sprite[] _tioNum = new Sprite[9];
        //风牌的所有图片
        public Sprite[] _fen = new Sprite[4];
        //中发白牌的所有图片
        public Sprite[] _jiNum = new Sprite[3];
        //花牌的所有图片
        public Sprite[] _hua = new Sprite[8];
        public Sprite _myMingGangBK;
        public Sprite _myAnGangBK;
        public Sprite _heMingGangBK;
        public Sprite _heAnGangBK;
        //显示总局数和已经打完的局数
        public Text[] FishshedGameNum;
        //游戏面板
        public GameObject playingPanel;
        //等待面板
        public GameObject watingPanel;
        //等待面板的解散按钮，主要是非庄家无需显示
        public GameObject BtnDissoveRoom;
        //返回大厅按钮
        public GameObject BtnBackLobby;
        //解散房间的面板
        public GameObject DissolvePanel;

        /// <summary>
        /// 是否是第一次进入游戏  游戏规则面板
        /// </summary>
        [HideInInspector]
        public bool isGameRulePanelOnGetInto = true;
        //游戏规则面板
        public GameObject GameRulePanel;

        //游戏规则面板只在玩家进入的时候显示五秒
        public GameObject GameRulePanelShowOnce;


        //听牌的父物体
        public GameObject TingStatus;

        public GameObject _gCommon;
        //显示十三幺吃抢牌的初始位置
        public GameObject showVCardPre_InitPos;
        /// <summary>
        /// 玩家手牌初始位置（纵向）
        /// </summary>
        public Transform CurrentDCardPre_InitPos;
        public GameObject _gMaskButton;
        public Transform _btnFlagM;//音乐
        public Transform _btnFlagE;//音效
        public Transform _btnFlagV;//语音
        public GameObject _btnShare;
        public GameObject CanDownRu;  //关于能跑能下的界面
        public GameObject BtnReturnLobby_; //返回大厅按钮           
        public GameObject ZhanzuoPanel;//取消占座的面板
        public GameObject AppointmentRoom;//预约房间特殊显示
        public Image SkipWinImage; //过胡提示的图片

        public uint CreatRoomTime;//创建房间的时间
        public uint LoatTime;//等待需要的时间
        public bool m_isYuYueRoom = false;//是预约房
        public bool m_isLost60Scend = false;//是最后的一分钟
        public bool m_gameStart = false;//游戏开始

        public const string MESSAGE_COPYROOMID = "MainViewPlayerPlayingPanel.MESSAGE_COPYROOMID";  //点击复制房间号码按钮
        public const string MESSAGE_SHAREWX = "MainViewPlayerPlayingPanel.MESSAGE_SHAREWX";  //点击微信分享按钮
        public const string MESSAGE_ROOMRULE = "MainViewPlayerPlayingPanel.MESSAGE_ROOMRILE";  //点击房间规则按钮
        public const string MESSAGE_RETURN = "MainViewPlayerPlayingPanel.MESSAGE_RETURN";  //点击返回大厅按钮
        public const string MESSAGE_DISSOLVEROOM = "MainViewPlayerPlayingPanel.MESSAGE_DISSOLVEROOM";   //点击解散房间按钮
        public const string MESSAGE_BTNVOICE = "MainViewPlayerPlayingPanel.MESSAGE_BTNVOICE";  //点击语音按钮
        public const string MESSAGE_BTNALLSETTINGKINDS = "MainViewPlayerPlayingPanel.MESSAGE_BTNALLSETTING";//设置集合
        public const string MESSAGE_CHOICETHIRTEEN = "MainViewPlayerPlayingPanel.MESSAGE_CHOICETHIRTEEN";//选择成十三幺
        public const string MESSAGE_CANCALTHIRTEEN = "MainViewPlayerPlayingPanel.MESSAGE_CANCALTHIRTEEN";//取消十三幺 
        public const string MESSAGE_MUSICVALUE = "MainViewPlayerPlayingPanel.MESSAGE_MUSICVALUE";//修改音乐音量
        public const string MESSAGE_EFFECVALUE = "MainViewPlayerPlayingPanel.MESSAGE_EFFECVALUE";//设置音效音量
        public const string MESSAGE_VOICEVALUE = "MainViewPlayerPlayingPanel.MESSAGE_VOICEVALUE";//设置语音音量

        public const string MESSAGE_MUSICCLICK = "MainViewPlayerPlayingPanel.MESSAGE_MUSICCLICK";//关闭音乐
        public const string MESSAGE_EFFECCLICK = "MainViewPlayerPlayingPanel.MESSAGE_EFFECCLICK";//关闭音效
        public const string MESSAGE_VOICECLICK = "MainViewPlayerPlayingPanel.MESSAGE_EFFECCLICK";//关闭语音 
        public const string MESSAGE_BTNANTICHEATING = "MainViewPlayerPlayingPanel.MESSAGE_BTNANTICHEATING";  //防作弊按钮      
        public const string MESSAGE_BTNTING_DOWN = "MainViewPlayerPlayingPanel.MESSAGE_BTNTING_DOWN"; //查看听牌的按钮 按下
        public const string MESSAGE_BTNTING_UP = "MainViewPlayerPlayingPanel.MESSAGE_BTNTING_UP"; //查看听牌的按钮 抬起

        public const string MESSAGE_INSEAT = "MainViewPlayerPlayingPanel.MESSAGE_INSEAT";//占座
        public const string MESSAGE_OUTSEAT = "MainViewPlayerPlayingPanel.MESSAGE_OUTSEAT";//取消占座 
        public const string MESSAGE_PLAYRREADY = "MainViewPlayerPlayingPanel.MESSAGE_PLAYRREADY";//玩家准备 

        #region 游戏面板
        public const string MESSAGE_BTNPLAYERAVATOR = "MainViewPlayerPlayingPanel.MESSAGE_BTNPLAYERAVATOR";  //点击玩家头像按钮 
        public const string MESSAGE_BTNSPECIALCARD = "MainViewPlayerPlayingPanel.MESSAGE_BTNSPECIALCARD";  //点击吃碰杠胡按钮
        #endregion

        //当前牌的数量
        public Text tLeftCardCount;

        #region 麻将相关信息
        float intervalTime = 0.1f;  //发牌时间间隔        
        float fMahjongWidth = 85f;  //两个麻将之间的间隔
        float fspecialCardInitPos = -485f;  //摆放吃碰杠胡的初始位置        
        float fSpecialCardInterval = 247f;// 特殊牌型摆放间隔
        float fSpecialCard_Thirteen = 5f;  //摆放十三幺吃抢的初始位置
        float fSpecialCard_Thirteen_Interval = 85f;  //摆放十三幺吃抢的间隔
        int iMahId;  //麻将的唯一id                
        #endregion
        PlayerPlayingPanelData PPPD;
        Text DownTime;   //倒计时显示

        /// <summary>
        /// 吃
        /// </summary>
        struct ChiCard
        {
            public byte value1;
            public byte value2;
            public byte value3;

            public int Num;//第几个是吃的牌
        }

        void OnDestroy()
        {
            //恢复推送
            //JPush.JPushBinding.ResumePush();
            flagTimeWarning = false;
            startflagWarning = false;
            flagDoteen = false;
        }

        void FindChildren()
        {
            HorizontalLayoutGroup[] objs = _Cards[0].GetComponentsInChildren<HorizontalLayoutGroup>();
            PoolAgent[] images = objs[0].GetComponentsInChildren<PoolAgent>();
            Debug.LogWarning(images.Length);
            images[0].GetComponent<Image>().sprite = _wanNum[1];
        }

        void Awake()
        {
            PPPD = GameData.Instance.PlayerPlayingPanelData;
            DownTime = _TimeRemi.Find("TextTime").GetComponent<Text>();
        }
        IEnumerator UpdateTime()
        {
            while (true)
            {
                NowTime.text = System.DateTime.Now.ToString("t");
                yield return new WaitForSeconds(59);
            }
        }
        void Start()
        {
            StartCoroutine(UpdateBattery());
            StartCoroutine(UpdateTime());
            if (MahjongLobby_AH.SDKManager.Instance.CheckStatus == 1)
            {
                ButtonVoice.SetActive(false);
            }
            else
            {
                ButtonVoice.SetActive(true);
            }
            // Debug.Log("SDKManager.Instance.IOSCheckStaus" + SDKManager.Instance.IOSCheckStaus);
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
            {
                _btnShare.SetActive(false);
                //  Debug.LogWarning(_btnShare.activeInHierarchy);

            }
            else
            {
                _btnShare.SetActive(true);
            }

            GameData.Instance.PlayerPlayingPanelData.isChoicTir = 0;
        }

        bool startflagWarning;
        bool flagTimeWarning;

        //关闭音效
        public void CloseWaringMusic()
        {
            flagTimeWarning = false;
            startflagWarning = false;
            flagDoteen = false;
        }


        void Update()
        {
            GameHereReadyTime();
            //更新中间的倒计时
            if (PPPD.DownTime >= PPPD.DownTimer_Limit)
            {
                PPPD.DownTime = PPPD.DownTimer_Limit;
            }
            else
            {
                PPPD.DownTime += Time.deltaTime;
                //加音效啊
                if (GameData.Instance.PlayerPlayingPanelData.isCanHandCard)
                {
                    if (PPPD.DownTime <= 15 && PPPD.DownTime >= 5f)
                    {
                        if (!startflagWarning)
                        {
                            flagTimeWarning = true;
                            flagDoteen = true;
                            startflagWarning = true;
                            startWarning();
                        }
                    }
                    else
                    {
                        if (startflagWarning)
                        {
                            flagTimeWarning = false;
                            startflagWarning = false;
                            flagDoteen = false;
                        }
                    }
                }
                else
                {
                    if (startflagWarning)
                    {
                        flagTimeWarning = false;
                        startflagWarning = false;
                        flagDoteen = false;
                    }
                }
            }


            //用于计算玩家的下分的倒计时 安徽暂时用不到
            //if (isDownTimerToChioce > 0)
            //{
            //    fDownTime -= Time.deltaTime;
            //    if (fDownTime <= 0)
            //    {
            //        //发送消息
            //        BtnChioceDownOrRu(0);
            //    }
            //    CanDownRu.transform.Find("Remind/Text").GetComponent<Text>().text = String.Format("{0:0}", fDownTime);

            //}

            ////开始计算玩家出牌倒计时，如果超过出牌时间
            //if (PPPD.isStartPutDownTime)
            //{
            //    fPutCardDownTim -= Time.deltaTime;
            //    //如果到了出牌时间，还不出牌，则直接是玩家进入托管状态，自动出牌
            //    if (fPutCardDownTim <= 0)
            //    {
            //        PPPD.isStartPutDownTime = false;
            //        //发送玩家进入托管状态消息
            //    }
            //}


            DownTime.text = String.Format("{0:00}", PPPD.DownTime);

            if (PPPD.DownTime < 9.5f || PPPD.DownTime >= 20)
            {
                DownTime.transform.localPosition = new Vector3(-2f, 4f, 0);
            }
            else
            {
                DownTime.transform.localPosition = new Vector3(-6f, 4f, 0);
            }
        }
        public bool flagDoteen;
        void startWarning()
        {
            if (flagDoteen)
            {
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Time_Warning, false, false);
                int count = 0;
                Tween time = DOTween.To(() => count, x => count = x, 1, 1f);
                time.OnComplete(() => startWarning());
            }
        }


        /// <summary>
        /// 关闭所有的手势按钮
        /// </summary>
        public void CloseReadyImage()
        {
            //关闭所有玩家的准备按钮
            for (int i = 0; i < 4; i++)
            {
                ReadyImage[i].gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 更新界面
        /// </summary>
        public void UpdateShow()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //Debug.LogError("isUpdateXinHao:" + isUpdateXinHao);
            if (!isUpdateXinHao)
            {
                //Debug.LogError("进入携程函数:UpdateXinHao");
                StartCoroutine(UpdateXinHao());
            }
            //Debug.LogError("_RoomId:" + _RoomId[0].text + "," + MahjongCommonMethod.Instance.RoomId);
            _RoomId[0].text = MahjongCommonMethod.Instance.RoomId;
            //Debug.LogError("处理游戏面板的显示情况:" + pppd.isPanelShow_Playing + "," + pppd.isWatingPlayerDownOrUp);
            //处理游戏面板的显示情况
            if (pppd.isPanelShow_Playing || pppd.isWatingPlayerDownOrUp)
            {
                //_RoomId[1].text =  MahjongCommonMethod.Instance.RoomId;
                bool isEmpty = false;
                if (MahjongCommonMethod.Instance._dicPlayNameConfig.ContainsKey(MahjongCommonMethod.Instance.iCountyId))
                {
                    //添加游戏名字
                    for (int l = 0; l < MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId].Count; l++)
                    {
                        if (MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId][l].METHOD == pppd.iMethodId)
                        {
                            isEmpty = true;
                            GameName_playing_bg.gameObject.SetActive(true);
                            GameName_playing.text = MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId][l].METHOD_NAME;
                            GameName_playing_bg.rectTransform.sizeDelta = new Vector2(GameName_playing.preferredWidth + 170f, 57f);
                        }
                    }
                }
                //如果玩家进入放入房间，和房主开房的
                if (!isEmpty)
                {
                    if (MahjongCommonMethod.Instance._dicMethodConfig.ContainsKey(pppd.iMethodId))
                    {
                        GameName_playing_bg.gameObject.SetActive(true);
                        GameName_playing.text = MahjongCommonMethod.Instance._dicMethodConfig[pppd.iMethodId].METHOD_NAME;
                        GameName_playing_bg.rectTransform.sizeDelta = new Vector2(GameName_playing.preferredWidth + 170f, 57f);
                    }
                }
                //Debug.LogError("GameName_playing.preferredWidth：" + GameName_playing.preferredWidth);

                playingPanel.SetActive(true);
                watingPanel.SetActive(false);
                //更新牌面上的牌的数量
                if (pppd.LeftCardCount <= 0)
                {
                    pppd.LeftCardCount = 0;
                }
                tLeftCardCount.text = "剩牌 <color=#83eae2>" + pppd.LeftCardCount.ToString() + "</color>";

                if (m_gameStart)
                {
                    //更新玩家的分数
                    for (int i = 0; i < 4; i++)
                    {
                        int index = pppd.GetOtherPlayerShowPos(i + 1) - 1;
                        _txUsersScor[index].text = pppd.sScoreShow[i].ToString();
                    }
                }



                StringBuilder str = new StringBuilder();
                if (pppd.playingMethodConf.byBillingMode == 1)
                {
                    str.Append("第");
                    str.Append(pppd.FinshedQuanNum);
                    str.Append("圈");
                    FishshedGameNum[0].text = str.ToString();
                    str.Append("(");
                    if (pppd.byDealerSeat == 0)
                    {
                        str.Append(1);
                    }
                    else
                    {
                        str.Append(pppd.byDealerSeat);
                    }
                    str.Append("/");
                    str.Append(4);
                    str.Append(")");
                    FishshedGameNum[1].text = str.ToString(3, 5);
                    FishshedGameNum[2].gameObject.SetActive(false);
                }
                else if (pppd.playingMethodConf.byBillingMode == 2)
                {
                    FishshedGameNum[0].text = "局数";
                    FishshedGameNum[1].text = "(" + pppd.FinshedGameNum.ToString() + "/" + pppd.AllGameNum + ")";
                }
                else if (pppd.playingMethodConf.byBillingMode == 3)
                {
                    FishshedGameNum[0].text = "局数";
                    FishshedGameNum[1].text = "第" + pppd.FinshedGameNum.ToString() + "局";
                }
            }
            //处理等待面板的显示情况
            if (pppd.isPanelShow_Wating)
            {
                playingPanel.SetActive(false);
                watingPanel.SetActive(true);
                bool isEmpty = false;
                if (MahjongCommonMethod.Instance._dicPlayNameConfig.ContainsKey(MahjongCommonMethod.Instance.iCountyId))
                {
                    //添加游戏名字
                    for (int l = 0; l < MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId].Count; l++)
                    {
                        if (MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId][l].METHOD == pppd.iMethodId)
                        {
                            isEmpty = true;
                            // GameName.text = MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId][l].METHOD_NAME;
                            GameName_playing_bg.gameObject.SetActive(true);
                            GameName_playing.text = MahjongCommonMethod.Instance._dicPlayNameConfig[MahjongCommonMethod.Instance.iCountyId][l].METHOD_NAME;
                            GameName_playing_bg.rectTransform.sizeDelta = new Vector2(GameName_playing.preferredWidth + 170f, 57f);
                        }
                    }
                }

                //如果玩家进入放入房间，和房主开房的
                if (!isEmpty)
                {
                    if (MahjongCommonMethod.Instance._dicMethodConfig.ContainsKey(pppd.iMethodId))
                    {
                        // GameName.text = MahjongCommonMethod.Instance._dicMethodConfig[pppd.iMethodId].METHOD_NAME;
                        GameName_playing_bg.gameObject.SetActive(true);
                        GameName_playing.text = MahjongCommonMethod.Instance._dicMethodConfig[pppd.iMethodId].METHOD_NAME;
                        GameName_playing_bg.rectTransform.sizeDelta = new Vector2(GameName_playing.preferredWidth + 170f, 57f);
                    }
                }


                if (pppd.iOpenRoomUserId == pppd.iUserId)
                {
                    BtnDissoveRoom.SetActive(true);
                    //BtnBackLobby.SetActive(false);
                }
                else
                {
                    BtnDissoveRoom.SetActive(false);
                }

                //馆外创建的房间
                if (MahjongCommonMethod.Instance.iParlorId == 0)
                {
                    if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId)
                    {
                        BtnBackLobby.SetActive(false);
                    }
                    else
                    {
                        BtnBackLobby.SetActive(true);
                    }
                }
                else//馆内开房
                {
                    //馆主
                    if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId && MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId <= 0 && LoatTime <= 0)
                    {
                        BtnBackLobby.SetActive(false);
                    }
                    else
                    {
                        BtnBackLobby.SetActive(true);
                    }
                }


                //if (LoatTime > 0)
                //{
                //    BtnBackLobby.SetActive(true);
                //}
                //if (pppd.iOpenRoomUserId == pppd.iUserId && LoatTime > 0)
                //{
                //    BtnBackLobby.SetActive(false);
                //}
                //else
                //{
                //    BtnBackLobby.SetActive(true);
                //}
            }
        }

        /// <summary>
        /// 显示玩家信息
        /// </summary>
        public void ShowBtnTing(bool isShow)
        {
            if (isShow)
            {
                btn_tingpai.gameObject.SetActive(true);
            }
            else
            {
                btn_tingpai.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 获取电池电量
        /// </summary>
        /// <returns></returns>
        public Image _imageBattery;
        public void GetBatteryLevel()
        {
            float fBattery = 0.5f;
            // Debug.LogError("---------------开始调用电池电量信息------------------------");
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                    Debug.LogError(int.Parse(CapacityString));
                    fBattery = int.Parse(CapacityString) * 0.01f;
                    //return int.Parse(CapacityString);
                }
                catch
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                    AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                    jo.Call("getBatteryState");
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                fBattery = GetiOSBatteryLevel();
            }
            _imageBattery.fillAmount = fBattery;

        }

        [DllImport("__Internal")]
        private static extern float GetiOSBatteryLevel();
        /// <summary>
        /// 每分钟检测电量
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateBattery()
        {
            while (true)
            {
                GetBatteryLevel();
                yield return new WaitForSeconds(58);
            }
        }
        Ping ping;
        //  float delayTime = 0;
        bool isUpdateXinHao;
        IEnumerator UpdateXinHao()
        {
            isUpdateXinHao = true;
            while (true)
            {
                yield return StartCoroutine(repeatFiveTimes());
            }
        }
        public Image XinHao;
        public Sprite[] XinHaosprit = new Sprite[5];
        public Sprite[] XinHaoWifi = new Sprite[5];
        IEnumerator repeatFiveTimes()
        {
            float delayTime = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ping == null)
                {
                    if (!string.IsNullOrEmpty(Network.NetworkMgr.Instance.GameServer.ServerIP))
                    {
                        //Debug.LogWarning("游戏IP：" + Network.NetworkMgr.Instance.GameServer.ServerIP);
                        ping = new Ping(Network.NetworkMgr.Instance.GameServer.ServerIP);
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();
                        yield return new WaitUntil(() =>
                        {
                            // Debug.Log(sw.ElapsedMilliseconds);
                            if (sw.ElapsedMilliseconds > 2000)
                            {
                                sw.Stop();
                                sw.Reset();
                                return true;
                            }
                            return ping.isDone;
                        });
                        // ping.isDone 
                        delayTime += ping.time;
                        //Debug.LogError(ping.time);
                        ping.DestroyPing();
                        ping = null;

                    }
                }
                yield return new WaitForSeconds(1.5f);
            }
            delayTime = delayTime * 0.5f;
            if (XinHao.gameObject.activeInHierarchy )
            {
                XinHao.transform.GetChild(0).GetComponent<Text>().text = (int )delayTime+"";
            }
            //Debug.LogWarning("delayTime：" + delayTime);
            // Debug.LogWarning("2/wifi 1/Carrier当前连接状态：" + Application.internetReachability);
            //当用户使用wifi
            if (!XinHao.gameObject.activeInHierarchy)
            {
                XinHao.gameObject.SetActive(true);
            }
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                if (delayTime <= 32)
                {
                    XinHao.sprite = XinHaoWifi[4];
                }
                else if (delayTime <= 70)
                {
                    XinHao.sprite = XinHaoWifi[3];
                }
                else if (delayTime <= 120)
                {
                    XinHao.sprite = XinHaoWifi[2];
                }
                else if (delayTime <= 250)
                {
                    XinHao.sprite = XinHaoWifi[1];
                }
                else
                {
                    XinHao.sprite = XinHaoWifi[0];
                }
            }
            //当用户使用移动网络时

            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                if (delayTime <= 32)
                {
                    XinHao.sprite = XinHaosprit[4];
                }
                else if (delayTime <= 70)
                {
                    XinHao.sprite = XinHaosprit[3];
                }
                else if (delayTime <= 120)
                {
                    XinHao.sprite = XinHaosprit[2];
                }
                else if (delayTime <= 250)
                {
                    XinHao.sprite = XinHaosprit[1];
                }
                else
                {
                    XinHao.sprite = XinHaosprit[0];
                }
            }
            // Debug.LogError("delayTime：" + delayTime);

        }

        /// <summary>
        /// 移动头像到相关位置
        /// </summary>       
        public void MoveHeadImage()
        {
            for (int i = 0; i < 4; i++)
            {
                _playingHead[i].transform.GetComponent<RectTransform>().localPosition = _playingChangeHead[i].localPosition;
                //Debug.LogError("pos:" + _playingHead[i].transform.localPosition);
            }
            //PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //Transform trans = _playingHead[0].transform;
            //Tweener tweener_y = trans.DOLocalMoveY(GameConstants.headPos_play[0].y, 0.1f);
            //tweener_y.SetEase(Ease.Linear);
            //tweener_y.OnComplete(() => Oncomplete_(trans, 0));
            //for (int i=0;i<4;i++)
            //{                
            //    Transform trans = _playingHead[i].transform;
            //    Tweener tweener_y = trans.DOLocalMoveY(GameConstants.headPos_play[i].y, 0.1f);
            //    tweener_y.SetEase(Ease.Linear);
            //    tweener_y.OnComplete(() => Oncomplete_(trans,i));
            //}            
        }

        void Oncomplete_(Transform go, int index)
        {
            Debug.LogWarning("移动界面");
            Tweener tweener_x = go.DOLocalMoveX(GameConstants.headPos_play[index].x, 0.1f);
            tweener_x.SetEase(Ease.Linear);
        }
        /// <summary>
        /// 更新面板的吃碰杠胡提示情况
        /// </summary>
        /// <param name="status">表示吃碰杠胡对应的下标</param>
        public void ShowSpecialTileNoticeRemind(int[] status)
        {
            if (status.Length <= 0)
            {
                return;
            }
            //设置父物体的位置，大小
            SpecialTitleParent.transform.localScale = Vector3.one;
            SpecialTitleParent.transform.localPosition = Vector3.zero;
            int count = 0;  //显示数量
            for (int i = 0; i < status.Length; i++)
            {
               // Debug.LogError(status[i]);
                if (status[i] == 1)
                {
                    count++;
                    SpecialTileNotice[i].SetActive(true);
                    //恢复杠的界面显示
                    if (i == 2)
                    {
                        SpecialTileNotice[i].transform.Find("Gang").gameObject.SetActive(false);
                        //隐藏其他的牌
                        for (int k = 0; k < 3; k++)
                        {
                            SpecialTileNotice[i].transform.Find("Gang").GetChild(k).gameObject.SetActive(false);
                        }
                        SpecialTileNotice[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        SpecialTileNotice[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                    _gMaskButton.SetActive(true);
                }
                else
                {
                    SpecialTileNotice[i].SetActive(false);
                    _gMaskButton.SetActive(false);
                }
            }

            if (count == 0)
            {
                SpecialTileNoticeBg.transform.localPosition = new Vector3(280f, 0, 0);
            }
            else
            {
                SpecialTileNoticeBg.gameObject.SetActive(true);
                SpecialTileNoticeBg.transform.localPosition = new Vector3(280f - 70 * (count - 1), 0, 0);
            }
            //更新背景
            SpecialTileNoticeBg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 175 * count);


        }

        public class TempMahjong
        {
            public PoolAgent agent;
            public float x;
        }


        /// <summary>
        /// 移动最后一张牌，形成间隙
        /// </summary>
        public void MoveLastCard()
        {
            PoolAgent[] temp = _Cards[0].GetComponentsInChildren<HorizontalLayoutGroup>(false)[0].GetComponentsInChildren<PoolAgent>();

            List<TempMahjong> mahjong_temp = new List<TempMahjong>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].name.Contains("currentDCardPre"))
                {
                    TempMahjong tempp = new TempMahjong();
                    tempp.agent = temp[i];
                    tempp.x = temp[i].transform.localPosition.x;
                    mahjong_temp.Add(tempp);
                    temp[i].GetComponent<Mahjong>().isDealCard = false;
                }
            }

            mahjong_temp.Sort(PPPD.Compare_Pos);

            mahjong_temp[mahjong_temp.Count - 1].agent.transform.localPosition += new Vector3(20, 0, 0);
            mahjong_temp[mahjong_temp.Count - 1].agent.GetComponent<Mahjong>().isDealCard = true;
            MahjongManger.Instance.PlayerDealHandCrad = mahjong_temp[mahjong_temp.Count - 1].agent.GetComponent<Mahjong>();
        }

        bool isComplete;

        /// <summary>
        /// 更新玩家自己的牌面信息
        /// </summary>
        public void CardUpdateShow_Self()
        {
            if (isComplete)
            {
                return;
            }

            isComplete = true;

            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;

            PoolAgent[] temp = _Cards[0].GetComponentsInChildren<HorizontalLayoutGroup>(false)[0].GetComponentsInChildren<PoolAgent>();

            List<PoolAgent> temp_1 = new List<PoolAgent>();


            List<TempMahjong> mahjong_temp = new List<TempMahjong>();

            int specialCount = 0; //特殊牌型数量
            float length = 0;  // 间隔宽度
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].name.Contains("currentDCardPre"))
                {
                    TempMahjong tempp = new TempMahjong();
                    tempp.agent = temp[i];
                    tempp.x = temp[i].transform.localPosition.x;
                    mahjong_temp.Add(tempp);
                }

                if (temp[i].isActiveAndEnabled)
                {
                    if (temp[i].name.Contains("showVCardPre"))
                    {
                        specialCount++;
                        length = 85f;
                    }

                    if (temp[i].name.Contains("pegaDCardsPre"))
                    {
                        specialCount++;
                        length = 247f;
                    }
                }
            }

            mahjong_temp.Sort(PPPD.Compare_Pos);
            //在这里如何判断第一张牌的位置是否正确，如果不正确修整
            Vector3 initpos = Vector3.zero;

            initpos = new Vector3(-557f, 0, 0) + new Vector3(specialCount * length, 0f, 0f);

            for (int i = 0; i < mahjong_temp.Count; i++)
            {
                temp_1.Add(mahjong_temp[i].agent);
                temp_1[i].transform.localPosition = initpos + new Vector3(i * 85f, temp_1[i].transform.localPosition.y, 0);
            }
            PoolAgent[] Card0currents = temp_1.ToArray();
            MahjongManger.Instance.GetFirstMahjongPos(0);

            //遍历当前所有牌
            for (int i = 0; i < Card0currents.Length; i++)
            {
                //如果是玩家自己当前的显示的牌
                if (string.CompareOrdinal(Card0currents[i].agentName, PlayerPlayingPanelData.currentDCardPre) == 0 ||
                    string.CompareOrdinal(Card0currents[i].agentName, PlayerPlayingPanelData.currentMoCardPre) == 0)
                {
                    if (i > ppd.usersCardsInfo[0].listCurrentCards.Count - 1)
                    {
                        continue;
                    }
                    ChangeCardNum(Card0currents[i].transform.Find("Image").Find("num").GetComponent<Image>(), ppd.usersCardsInfo[0].listCurrentCards[i].cardNum);
                    Card0currents[i].transform.GetComponent<Mahjong>().bMahjongValue = ppd.usersCardsInfo[0].listCurrentCards[i].cardNum;
                    Card0currents[i].transform.GetComponent<Mahjong>().iMahId = ppd.usersCardsInfo[0].listCurrentCards[i].MahId;
                }
            }
            isComplete = false;
        }



        /// <summary>
        /// 牌面相关更新显示，只刷新牌面数字，不更改预置体顺序
        /// </summary>
        internal void CardsUpdateShow()
        {
            Debug.LogWarning("数组长度     " + transform.GetComponents<RectTransform>().Length);
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;
            #region 玩家0 - Card1
            ///Card1/currentGroup中所有预置体 玩家1当前所有牌
            PoolAgent[] temp = _Cards[0].GetComponentsInChildren<HorizontalLayoutGroup>()[0].GetComponentsInChildren<PoolAgent>();

            List<PoolAgent> temp_1 = new List<PoolAgent>();

            List<TempMahjong> mahjong_temp = new List<TempMahjong>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].name.Contains("currentDCardPre"))
                {
                    TempMahjong tempp = new TempMahjong();
                    tempp.agent = temp[i];
                    tempp.x = temp[i].transform.localPosition.x;
                    mahjong_temp.Add(tempp);
                    //temp_1.Add(temp[i]);
                }
            }


            mahjong_temp.Sort(PPPD.Compare_Pos);

            for (int i = 0; i < mahjong_temp.Count; i++)
            {
                temp_1.Add(mahjong_temp[i].agent);
                //在这个位置对牌的位置进行初始化
                temp_1[i].transform.localPosition = MahjongManger.Instance.GetFirstMahjongPos(0) + new Vector3(i * 85f, temp_1[i].transform.localPosition.y, 0);
            }
            PoolAgent[] Card0currents = temp_1.ToArray();

            //遍历当前所有牌
            for (int i = 0; i < Card0currents.Length; i++)
            {
                //如果是玩家自己当前的显示的牌
                if (string.CompareOrdinal(Card0currents[i].agentName, PlayerPlayingPanelData.currentDCardPre) == 0 ||
                    string.CompareOrdinal(Card0currents[i].agentName, PlayerPlayingPanelData.currentMoCardPre) == 0)
                {
                    //Card0currents[i].transform.Find("Image").Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[0].listCurrentCards[i].cardNum);
                    ChangeCardNum(Card0currents[i].transform.Find("Image").Find("num").GetComponent<Image>(), ppd.usersCardsInfo[0].listCurrentCards[i].cardNum);
                    Card0currents[i].transform.GetComponent<Mahjong>().bMahjongValue = ppd.usersCardsInfo[0].listCurrentCards[i].cardNum;
                    Card0currents[i].transform.GetComponent<Mahjong>().iMahId = ppd.usersCardsInfo[0].listCurrentCards[i].MahId;
                }
                //如果是杠碰吃预置体  
                else if (string.CompareOrdinal(Card0currents[i].agentName, PlayerPlayingPanelData.pegaDCardsPre) == 0)
                {
                    switch (ppd.usersCardsInfo[0].listSpecialCards[i].type)
                    {
                        case 1://明杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card0currents[i].transform.GetChild(j);
                                if (j == 3)
                                {
                                    tra.GetComponent<Image>().enabled = true;//设置最上面的麻将牌背景贴图显示
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                }
                                tra.Find("num").GetComponent<Image>().enabled = true; //设置牌面数字显示
                                tra.GetComponent<Image>().sprite = _myMingGangBK;
                                //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[j]);
                                ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[j]);
                            }
                            break;
                        case 2://暗杠
                            Debug.LogWarning("angang**************************************");
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card0currents[i].transform.GetChild(j);
                                if (j == 3)//设置暗杠最上一张牌
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[j]);
                                }
                                else//其他牌翻过去，数字不显示
                                {
                                    tra.GetComponent<Image>().sprite = _myAnGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                            }
                            break;
                        case 3://吃
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card0currents[i].transform.GetChild(j);
                                Debug.LogWarning("《《《 tra 》》》" + tra + "," + tra.name);
                                if (j == 3)
                                {
                                    tra.GetComponent<Image>().enabled = false;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                                else
                                {
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    Debug.LogWarning("《《《 吃 》》》" + ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[i]);
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[i]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[0].listSpecialCards[i].mahValue[i]);
                                }
                            }
                            break;

                        case 4://普通


                            break;
                        default:
                            break;//0k
                    }
                }
            }
            #endregion 玩家0

            #region 玩家1-Card1

            //PoolAgent[] Card1show_d = _Cards[1].FindChild("showGroup_d").GetComponentsInChildren<PoolAgent>();
            //for (int i = 0; i < Card1show_d.Length; i++)
            //{
            //    Card1show_d[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, 90);
            //    Card1show_d[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[1].listShowCards[i].cardNum);
            //}

            //PoolAgent[] Card1show_u = _Cards[1].FindChild("showGroup_u").GetComponentsInChildren<PoolAgent>();
            //for (int i = 0; i < Card1show_u.Length; i++)
            //{
            //    Card1show_u[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, 90);
            //    Card1show_u[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[1].listShowCards[i + 11].cardNum);
            //}

            PoolAgent[] Card1currents = _Cards[1].GetComponentsInChildren<VerticalLayoutGroup>(false)[0].GetComponentsInChildren<PoolAgent>();

            for (int i = 0; i < Card1currents.Length; i++)
            {
                //如果是右边玩家当前牌
                //if (string.CompareOrdinal (Card1currents[i].agentName,PlayerPlayingPanelData.currentHCardPre ) ==0)
                //{
                //    Card1currents[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite=ChangeCardNum( ppd.UsersCardsInfo[1].listCurrentCards[i].cardNum);

                //}
                //如果是右边玩家吃碰杠的牌
                if (string.CompareOrdinal(Card1currents[i].agentName, PlayerPlayingPanelData.pegaHCardsPre) == 0)
                {
                    //switch (ppd.usersCardsInfo[1].listSpecialCards[i].type)
                    switch (-1)
                    {
                        case 1://明杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card1currents[i].transform.GetChild(j);
                                tra.GetComponent<Image>().enabled = true;
                                tra.GetComponent<Image>().sprite = _heMingGangBK;
                                tra.Find("num").GetComponent<Image>().enabled = true;
                                tra.Find("num").localEulerAngles = new Vector3(0, 0, 90);
                                //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                                ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                            }
                            break;
                        case 2://暗杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card1currents[i].transform.GetChild(j);
                                if (j == 3)
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                                }
                                else
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _heAnGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                            }
                            break;
                        case 3://吃
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card1currents[i].transform.GetChild(j);
                                if (j == 3)//设置暗杠最上一张牌
                                {
                                    tra.GetComponent<Image>().enabled = false;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                                else//其他正过来，数字显示
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _heMingGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.Find("num").localEulerAngles = new Vector3(0, 0, 90);
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[1].listSpecialCards[i].mahValue[j]);
                                }
                            }
                            break;

                        case 4://普通

                            break;
                        default:
                            break;
                    }
                    // ChangeCardNum(Card1currents[i].transform.FindChild )
                }
            }
            #endregion 右边玩家-Card1

            #region 上边玩家-Card2
            PoolAgent[] Card2currents = _Cards[2].GetComponentsInChildren<HorizontalLayoutGroup>(false)[0].GetComponentsInChildren<PoolAgent>();
            for (int i = 0; i < Card2currents.Length; i++)
            {
                if (string.CompareOrdinal(Card2currents[i].agentName, PlayerPlayingPanelData.pegaUCardsPre) == 0)
                {
                    //switch (ppd.usersCardsInfo[2].listSpecialCards[i].type)
                    switch (-1)
                    {
                        case 4://明杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card2currents[i].transform.GetChild(j);
                                tra.GetComponent<Image>().enabled = true;
                                tra.GetComponent<Image>().sprite = _myMingGangBK;
                                tra.Find("num").localEulerAngles = new Vector3(0, 0, 180);
                                tra.Find("num").GetComponent<Image>().enabled = true;
                                //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                                ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                            }
                            break;
                        case 0://暗杠

                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card2currents[i].transform.GetChild(j);
                                if (j == 3)
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                                }
                                else
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _heAnGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                            }
                            break;
                        case 3://吃
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card2currents[i].transform.GetChild(j);
                                if (j == 4)
                                {
                                    tra.GetComponent<Image>().enabled = false;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                                else
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.Find("num").localEulerAngles = new Vector3(0, 0, 180);
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[2].listSpecialCards[i].mahValue[j]);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            /////上路玩家打出去的牌           

            //PoolAgent[] Card2show_d = _Cards[2].FindChild("showGroup_d").GetComponentsInChildren<PoolAgent>();
            //PoolAgent[] Card2show_u = _Cards[2].FindChild("showGroup_u").GetComponentsInChildren<PoolAgent>();

            //for (int i = 0; i < Card2show_d.Length; i++)
            //{
            //    Card2show_d[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, 180);
            //    Card2show_d[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[2].listShowCards[i].cardNum);
            //}

            //for (int i = 0; i < Card2show_u.Length; i++)
            //{
            //    Card2show_u[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, 180);
            //    Card2show_u[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[2].listShowCards[i + 11].cardNum);
            //}




            #endregion 上边玩家-Card2           

            #region 左边玩家-Card3
            //Debug.LogError("玩家4============================================");
            /////打出去的牌刷新
            //PoolAgent[] Card3show_d = _Cards[3].FindChild("showGroup_d").GetComponentsInChildren<PoolAgent>();
            //for (int i = 0; i < Card3show_d.Length; i++)
            //{
            //    Card3show_d[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, -90);
            //    Card3show_d[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[3].listShowCards[i].cardNum);
            //}

            //PoolAgent[] Card3show_u = _Cards[3].FindChild("showGroup_u").GetComponentsInChildren<PoolAgent>();
            //for (int i = 0; i < Card3show_u.Length; i++)
            //{
            //    Card3show_u[i].transform.FindChild("Image").FindChild("num").localEulerAngles = new Vector3(0, 0, -90);
            //    Card3show_u[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[3].listShowCards[i + 11].cardNum);
            //}

            PoolAgent[] Card3currents = _Cards[3].GetComponentsInChildren<VerticalLayoutGroup>(false)[0].GetComponentsInChildren<PoolAgent>();
            for (int i = 0; i < Card3currents.Length; i++)
            {
                //如果是右边玩家当前牌
                //if (string.CompareOrdinal (Card1currents[i].agentName,PlayerPlayingPanelData.currentHCardPre ) ==0)
                //{
                //    Card1currents[i].transform.FindChild("Image").FindChild("num").GetComponent<Image>().sprite=ChangeCardNum( ppd.UsersCardsInfo[1].listCurrentCards[i].cardNum);

                //}
                //如果是右边玩家吃碰杠的牌
                if (string.CompareOrdinal(Card3currents[i].agentName, PlayerPlayingPanelData.pegaHCardsPre) == 0)
                {
                    switch (-1)
                    //switch (ppd.usersCardsInfo[3].listSpecialCards[i].type)
                    {
                        case 1://明杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card3currents[i].transform.GetChild(j);
                                tra.GetComponent<Image>().enabled = true;
                                tra.GetComponent<Image>().sprite = _heMingGangBK;
                                tra.Find("num").GetComponent<Image>().enabled = true;
                                tra.Find("num").localEulerAngles = new Vector3(0, 0, -90);
                                //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                                ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                            }
                            break;
                        case 2://暗杠
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card3currents[i].transform.GetChild(j);
                                if (j == 3)
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _myMingGangBK;
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                                }
                                else
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _heAnGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                }
                            }
                            break;
                        case 3://吃
                            for (int j = 0; j < 4; j++)
                            {
                                Transform tra = Card3currents[i].transform.GetChild(j);
                                if (j == 4)//设置暗杠最上一张牌
                                {
                                    tra.GetComponent<Image>().enabled = false;
                                    tra.Find("num").GetComponent<Image>().enabled = false;
                                    tra.GetComponent<Image>().sprite = _heMingGangBK;
                                }
                                else//其他正过来，数字显示
                                {
                                    tra.GetComponent<Image>().enabled = true;
                                    tra.GetComponent<Image>().sprite = _heMingGangBK;
                                    tra.Find("num").GetComponent<Image>().enabled = true;
                                    tra.Find("num").localEulerAngles = new Vector3(0, 0, -90);
                                    //tra.Find("num").GetComponent<Image>().sprite = ChangeCardNum(ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                                    ChangeCardNum(tra.Find("num").GetComponent<Image>(), ppd.usersCardsInfo[3].listSpecialCards[i].mahValue[j]);
                                }
                            }
                            break;

                        case 4://普通

                            break;
                        default:
                            break;
                    }
                    // ChangeCardNum(Card1currents[i].transform.FindChild )
                }
            }
            #endregion 左边玩家-Card3
        }

        /// <summary>
        /// 显示哪个玩家下牌
        /// </summary>
        /// <param name="index">显示的</param>
        /// <param name="uindex">不显示的</param>
        public void ShowWitchPlayerPlay(int index, int uindex)
        {

            _TimeRemi.GetChild(0).GetChild(uindex).gameObject.SetActive(false);
            _TimeRemi.GetChild(0).GetChild(index).gameObject.SetActive(true);

        }


        IEnumerator spwanCard = null;

        public void SpwanSelfPutCard_IE(byte cardValue, float time)
        {
            if (spwanCard != null)
            {
                StopCoroutine(spwanCard);
            }
            spwanCard = SpwanSelfPutCard(cardValue, time);
            StartCoroutine(spwanCard);
        }

        /// <summary>
        /// 产生玩家摸得手牌
        /// </summary>
        /// <param name="cardValue">手牌的花色大小值</param>
        public IEnumerator SpwanSelfPutCard(byte cardValue, float timer)
        {
            yield return new WaitForSeconds(timer);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            byte[] mah = new byte[1] { cardValue };
            if (cardValue !=0)
            {
                SpwanMahjong(mah, 20f);
            }
           

            if (cardValue > 96)
            {
                yield break;
            }

            //如果是玩家已经进入托管状态
            if (pppd.iPlayerHostStatus == 1)
            {
                yield break;
            }

            bool isContainSpecial = false;

            //处理玩家摸到手牌之后，判断玩家的杠
            if (MahjongHelper.Instance.JudgeGang(pppd.GetPlayerAllHandCard(2), pppd.usersCardsInfo[0].listSpecialCards) > 0)
            {
                isContainSpecial = true;
                MahjongHelper.Instance.specialValue_[2] = 1;
                MahjongHelper.Instance.specialValue_[7] = 1;
                pppd.isSendPlayerPass = false;
                ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
            }

            //判断自摸
            if (MahjongHelper.Instance.JudgeWin(pppd.GetPlayerAllHandCard(1)) > 0)
            {
                //判断听牌状态信息
                if (pppd.playingMethodConf.byWinLimitReadHand == 0 || pppd.usersCardsInfo[0].listShowCards.Count == 0 || pppd.isChoiceTing == 2)
                {
                    pppd.isSendPlayerPass = false;
                    isContainSpecial = true;
                    MahjongHelper.Instance.specialValue_[3] = 1;
                    MahjongHelper.Instance.specialValue_[7] = 1;
                    ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);

                    //关闭听牌提示
                    Mahjong[] mah_ = MahjongManger.Instance.GetSelfCard();
                    for (int i = 0; i < mah_.Length; i++)
                    {
                        mah_[i].transform.Find("Ting").gameObject.SetActive(false);
                    }
                }
                else
                {
                    //玩家处于停牌状态，1s之后自动出牌
                    if (pppd.isChoiceTing == 2 && !isContainSpecial)
                    {
                        StartCoroutine(DelayAutoPutCard(1));
                    }

                    ////玩家处于托管状态，2s之后自动出牌
                    //if (pppd.iPlayerHostStatus > 0 && !isContainSpecial)
                    //{
                    //    StartCoroutine(DelayAutoPutCard(2));
                    //}
                }
            }
            else
            {
                //玩家进入听牌状态1s自动出牌
                if (pppd.isChoiceTing == 2 && !isContainSpecial)
                {
                    StartCoroutine(DelayAutoPutCard(1));
                }

                ////玩家进入托管状态2s自动出牌
                //if (pppd.iPlayerHostStatus > 0 && !isContainSpecial)
                //{
                //    StartCoroutine(DelayAutoPutCard(2));
                //}
            }


            if (pppd.isCanHandCard)
            {
                if (pppd.isChoiceTing == 0)
                {
                    //在每次出牌通知前，判断玩家出哪几张牌可以听牌                
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongHelper.Instance.mahjongTing = MahjongHelper.Instance.GetEnableTingCard(2);

                    if (MahjongHelper.Instance.mahjongTing.Count > 0)
                    {
                        //显示所有可以听牌的花色值
                        UIMainView.Instance.PlayerPlayingPanel.UpdateTingCard(MahjongHelper.Instance.Ting.ToArray());
                    }
                    else
                    {
                        //关闭听牌提示
                        Mahjong[] mah_ = MahjongManger.Instance.GetSelfCard();
                        for (int i = 0; i < mah_.Length; i++)
                        {
                            mah_[i].transform.Find("Ting").gameObject.SetActive(false);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 延迟自动出牌
        /// </summary>
        /// <param name="timer"></param>
        public void DelayAutoPutCard_(float timer)
        {
            StartCoroutine(DelayAutoPutCard(timer));
        }

        /// <summary>
        /// 延迟1s之后，自动出牌
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayAutoPutCard(float timer)
        {
            yield return new WaitForSeconds(timer);
            //延迟一秒之后，直接打出玩家的手牌        
            if (MahjongManger.Instance.GetDealCard() != null)
            {
                MahjongManger.Instance.GetDealCard().PutCard(1);
            }
        }

        /// <summary>
        /// 产生玩家的手牌
        /// </summary>
        /// <param name="mahjongValue"></param>
        /// <param name="interval">用于设置摸牌和手牌之间的距离</param>
        public void SpwanMahjong(byte[] mahjongValue, float interval)
        {
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;
            UserCardsInfo[] us = ppd.usersCardsInfo;
            if (us[0] == null)
            {
                GameData.Instance.PlayerPlayingPanelData.usersCardsInfo[0] = new Data.UserCardsInfo();//下边的玩家
            }

            Vector3 initPos = MahjongManger.Instance.GetFirstMahjongPos(0);

            //断线重入确定手牌的第一张位置
            if (ppd.isBreakConnect)
            {
                //处理十三幺
                if (ppd.usersCardsInfo[0].listSpecialCards.Count > 0)
                {
                    if (ppd.usersCardsInfo[0].listSpecialCards[0].type == 5 || ppd.usersCardsInfo[0].listSpecialCards[0].type == 6)
                    {
                        initPos += new Vector3(fSpecialCard_Thirteen_Interval * ppd.iSpecialCardNum, 0, 0);
                    }
                    else
                    {
                        initPos += new Vector3(fSpecialCardInterval * ppd.iSpecialCardNum, 0, 0);
                    }
                }
                ppd.isBreakConnect = false;
            }

            MahjongManger.Instance.initMah();

            //处理从庄家开始发牌，然后一次发牌            
            for (int i = 0; i < mahjongValue.Length; i++)
            {
                ppd.iSpwanCardNum++;
                iMahId++;
                CurrentCard cC = new CurrentCard();
                cC.cardNum = mahjongValue[i];
                cC.MahId = iMahId;
                us[0].listCurrentCards.Add(cC);
                GameObject obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.currentDCardPre);
                obj.transform.SetParent(_Cards[0].GetComponentsInChildren<HorizontalLayoutGroup>()[0].transform);

                obj.transform.localPosition = new Vector3(initPos.x + interval + fMahjongWidth * (ppd.iSpwanCardNum - 1), 0, 0);

                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                obj.transform.GetComponent<Button>().interactable = true;
                //obj.transform.Find("Image/num").GetComponent<Image>().sprite = ChangeCardNum(mahjongValue[i]);
                ChangeCardNum(obj.transform.Find("Image/num").GetComponent<Image>(), mahjongValue[i], i);
                //为玩家的花色赋值
                obj.transform.GetComponent<Mahjong>().bMahjongValue = mahjongValue[i];
                obj.transform.GetComponent<Mahjong>().iMahId = iMahId;
                if (interval > 0)
                {
                    //判断玩家摸得是不是花牌
                    if (mahjongValue[i] > 96)
                    {
                        Debug.LogWarning ("玩家摸到花牌:" + mahjongValue[i]);
                        StartCoroutine(ShowFlower(mahjongValue[i]));
                    }
                    else
                    {
                        Network.NetworkMgr.Instance.GameServer.Unlock();
                        obj.transform.GetComponent<Mahjong>().isDealCard = true;
                        MahjongManger.Instance.PlayerDealHandCrad = obj.transform.GetComponent<Mahjong>();
                        ppd.isCanHandCard = true;
                        ppd.isNeedSendPassMessage = false;
                    }
                }
            }
            //减去玩家牌的数量
          //  ppd.LeftCardCount -= mahjongValue.Length;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
        }


        //延迟摸得花牌的显示
        IEnumerator ShowFlower(byte mahjong)
        {
            yield return new WaitForSeconds(0.6f);
            Network.NetworkMgr.Instance.GameServer.Unlock();
            //删除手牌，请求补花
            NetMsg.ClientAppliqueReq msg_que = new NetMsg.ClientAppliqueReq();
            msg_que.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg_que.byFlowerTileNum = 1;
            msg_que.byaFlowerTile[0] = mahjong;
            Network.NetworkMgr.Instance.GameServer.SendClientAppliqueReq(msg_que);
        }

        /// <summary>
        /// 处理其他玩家的手牌
        /// </summary>
        /// <param name="num">复制的预置体数量</param>
        /// <param name="seatNum">要复制的玩家的座位号1--4</param>
        public void SpwanOtherPlayerCrad(int num, int seatNum)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //根据玩家的座位号确定玩家手牌的父物体
            int index = seatNum - 1;
            Transform trans = null;
            if (index == 0 || index == 2)
            {
                trans = _Cards[index].GetComponentsInChildren<HorizontalLayoutGroup>()[0].transform;
            }
            else
            {
                trans = _Cards[index].GetComponentsInChildren<VerticalLayoutGroup>()[0].transform;
            }

            for (int i = 0; i < num; i++)
            {
                GameObject obj = null;
                if (index == 1 || index == 3)
                {
                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.currentHCardPre);
                }
                else
                {
                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.currentUCardPre);
                }

                obj.transform.SetParent(trans);


                obj.transform.localPosition = new Vector3(obj.transform.position.x, obj.transform.position.y, Vector3.zero.z);
                obj.transform.localScale = new Vector3(1, 1, 1);

                if (index == 3)
                {
                    obj.transform.localEulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    obj.transform.localEulerAngles = Vector3.zero;
                }

                //修改其他人的牌的父物体下的下标
                obj.transform.SetAsLastSibling();
            }
            //减去玩家牌的数量
          //  pppd.LeftCardCount -= num;
            //Debug.LogError("产生别的玩家的牌的数量pppd.LeftCardCount：" + pppd.LeftCardCount);
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
        }

        /// <summary>
        /// 处理玩家顺序发牌
        /// </summary>
        public void PlayerDeal_Ie(byte[] mahjong)
        {
            StartCoroutine(PlayerDeal(mahjong));
        }

        /// <summary>
        /// 处理玩家顺序发牌
        /// </summary>
        public IEnumerator PlayerDeal(byte[] mahjong)
        {
            byte[] four = new byte[4];
            byte[] lastValue = new byte[1];
            lastValue[0] = mahjong[12];
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //获取庄家的对应数组的下标
            int index = pppd.GetOtherPlayerShowPos(1);
            //处理玩家分三次，每次摸四张牌
            for (int j = 0; j < 4; j++)
            {
                if (j < 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        four[i] = mahjong[i + j * 4];
                    }
                }
                //如果玩家不是庄家，则从庄家开始发牌
                if (pppd.bySeatNum != 1)
                {
                    for (int i = index; i < index + 4; i++)
                    {
                        int num = i;
                        if (num > 4)
                        {
                            num -= 4;
                        }
                        //先发庄家。再依次发牌
                        if (num == 1)
                        {
                            if (j < 3)
                            {
                                SpwanMahjong(four, 0);
                            }
                            else
                            {
                                SpwanMahjong(lastValue, 0);
                            }
                        }
                        else
                        {
                            if (j < 3)
                            {
                                SpwanOtherPlayerCrad(4, num);
                            }
                            else
                            {
                                SpwanOtherPlayerCrad(1, num);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 5; i++)
                    {
                        if (i == 1)
                        {
                            if (j < 3)
                            {
                                SpwanMahjong(four, 0);
                            }
                            else
                            {
                                SpwanMahjong(lastValue, 0);
                            }
                        }
                        else
                        {
                            if (j < 3)
                            {
                                SpwanOtherPlayerCrad(4, i);
                            }
                            else
                            {
                                SpwanOtherPlayerCrad(1, i);
                            }
                        }
                        yield return new WaitForSeconds(intervalTime);
                    }
                }
              //  Debug.LogError("处理玩家顺序发牌;" + j);
                if (j == 3)
                {
                    //排序
                    GameData.Instance.PlayerPlayingPanelData.CurrentCradSort(1);
                    if (pppd.bySeatNum == pppd.byDealerSeat)
                    {
                        pppd.isFirstDealCard = false;
                     //   Debug.LogError("pppd.FirstDealCard " + pppd.FirstDealCard);
                        if (pppd.FirstDealCard == 0)
                        {
                            Network.NetworkMgr.Instance.GameServer.Unlock();
                            yield break;
                        }
                        SpwanSelfPutCard_IE(pppd.FirstDealCard, 0.1f);
                    }
                    Network.NetworkMgr.Instance.GameServer.Unlock();
                }
                yield return new WaitForSeconds(intervalTime);
            }
        }

        /// <summary>
        /// 产生桌面上的牌
        /// </summary>
        /// <param name="seatNum">玩家座位号</param>
        /// <param name="value">牌面的值</param>
        public GameObject SpwanTableCrad(int seatNum, byte value)
        {
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;
            //把玩家座位号转换为桌上的值
            int index = ppd.GetOtherPlayerShowPos(seatNum) - 1;
            ppd.usersCardsInfo[index].listShowCards.Add(new Data.ShowCard(value));
            //获取对应的预置体的父物体
            Transform trans = null;

            //if (ppd.usersCardsInfo[index].listShowCards.Count <= 10)
            //{
            //    trans = _Cards[index].FindChild("showGroup_d");
            //}
            //else if (ppd.usersCardsInfo[index].listShowCards.Count <= 20)
            //{
            //    trans = _Cards[index].FindChild("showGroup_u");
            //}
            //else
            //{
            //    trans = _Cards[index].FindChild("showGroup_3");
            //}

            if (_Cards[index].Find("showGroup_d").childCount < 10)
            {
                trans = _Cards[index].Find("showGroup_d");
            }
            else if (_Cards[index].Find("showGroup_u").childCount < 10)
            {
                trans = _Cards[index].Find("showGroup_u");
            }
            else
            {
                trans = _Cards[index].Find("showGroup_3");
            }

            GameObject obj = null;
            if (index == 0 || index == 2)
            {
                obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showVCardPre);
            }
            else
            {
                obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showHCardPre);
            }

            //复制对应的预置体            
            obj.transform.SetParent(trans);

            //对数组第二个做特殊处理
            if (index == 1 || index == 2)
            {
                obj.transform.SetAsFirstSibling();
            }

            obj.transform.localPosition = new Vector3(obj.transform.position.x, obj.transform.position.y, Vector3.zero.z);
            obj.transform.localScale = new Vector3(1, 1, 1);

            obj.transform.localEulerAngles = Vector3.zero;
            if (index == 0)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 0f);
            }
            else if (index == 1)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (index == 2)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 180f);
            }
            else if (index == 3)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, -90f);
            }

            //对于预置体进行图片赋值
            //obj.transform.Find("Image/num").GetComponent<Image>().sprite = ChangeCardNum(value);
            ChangeCardNum(obj.transform.Find("Image/num").GetComponent<Image>(), value, index);

            obj.GetComponent<Mahjong>().bMahjongValue = value;
            obj.GetComponent<Mahjong>().RemindJustPutCard();

            MahjongManger.Instance.LastTableCard = obj;

            return obj;
        }


        public void ShowBigCardInTable(int seatNum, int value)
        {
            if (value <= 0)
            {
                return;
            }
            //产生玩家的桌面大牌
            int index = PPPD.GetOtherPlayerShowPos(seatNum) - 1;  //玩家数组下标  0---3
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/Ma/TabelBigCard"));
            go.transform.SetParent(transform.Find("Common/Cards"));
            go.transform.localPosition = MahjongManger.Instance.TableBigCardPos[index].transform.localPosition;
            go.transform.localScale = Vector3.one * 1.2f;
            go.transform.localEulerAngles = Vector3.zero;
            //go.transform.Find("Image/num").GetComponent<Image>().sprite =
            ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), (byte)value, index);
            go.GetComponent<Mahjong>().bMahjongValue = (byte)value;
            go.GetComponent<Mahjong>().PutCardAnimator(seatNum, value, 2);
        }


        /// <summary>
        /// 删除桌面上的牌
        /// </summary>
        /// <param name="index">玩家下标</param>
        /// <param name="value">要删除的牌值</param>
        public void DeleteTableCard(int index, byte value = 0)
        {
            if (index < 0 || index > 3)
            {
                return;
            }

            Transform trans = null;
            if (_Cards[index].Find("showGroup_3").GetComponentsInChildren<Mahjong>(false).Length > 0)
            {
                trans = _Cards[index].Find("showGroup_3");
            }
            else if (_Cards[index].Find("showGroup_u").GetComponentsInChildren<Mahjong>(false).Length > 0)
            {
                trans = _Cards[index].Find("showGroup_u");
            }
            else
            {
                trans = _Cards[index].Find("showGroup_d");
            }

            if (trans == null)
            {
                return;
            }

            GameObject obj = null;

            Mahjong[] temp = trans.GetComponentsInChildren<Mahjong>(false);
            if (temp.Length > 0)
            {
                if (index == 1 || index == 2)
                {
                    obj = temp[0].gameObject;
                }
                else
                {
                    obj = temp[temp.Length - 1].gameObject;
                }
            }
            else
            {
                Debug.LogError("没找到该张牌，异常处理");
            }

            if (obj == null)
            {
                return;
            }

            PoolManager.Unspawn(obj);

            if (PPPD.usersCardsInfo[index].listShowCards.Count > 0)
            {
                //移除玩家桌面上的牌的信息
                PPPD.usersCardsInfo[index].listShowCards.RemoveAt(PPPD.usersCardsInfo[index].listShowCards.Count - 1);
            }
        }

        /// <summary>
        /// 删除其他玩家的手牌
        /// </summary>
        /// <param name="seatNum"></param>
        public void DelHandCrad(int seatNum)
        {
            Transform parent = null;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int index = pppd.GetOtherPlayerShowPos(seatNum) - 1;
            parent = _Cards[index].transform.Find("currentGroup").transform;
            GameObject mah = MahjongManger.Instance.GetOtherPlayerHandCard(parent, index).gameObject;
            if (mah != null)
            {
                PoolManager.Unspawn(mah);
            }
        }


        /// <summary>
        /// 设置显示庄家 & 摆放方位
        /// </summary>
        ///  <param name="pn">玩家座位号1--4</param>
        /// <param name="seatNum">庄家座位号1--4</param>
        /// <param name="isShowBoss">fals-不更新方位、true更新</param>
        public void SetDealer(int pn, int seatNum, bool isShowBoss = false)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            _TimeRemi.GetChild(0).localEulerAngles = new Vector3(0, 0, -90 * (pn - 1));
            //只显示庄家图标
            for (int i = 0; i < 4; i++)
            {
                if (isShowBoss && i == (seatNum - 1))
                {
                    _playingHead[i].GetChild(5).gameObject.SetActive(true);
                }
                else
                {
                    _playingHead[i].GetChild(5).gameObject.SetActive(false);
                }
            }

        }
        /// <summary>
        /// 显示离线
        /// </summary>
        /// <param name="index">座位号</param>
        /// <param name="isLeave">是否离线</param>
        public void ShowLeavePerson(int index, bool isLeave)
        {
            if (index < 0 || index > 3)
            {
                return;
            }
            _playingHead[index].GetChild(6).gameObject.SetActive(isLeave);
        }


        /// <summary>
        /// 显示玩家托管状态
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isHost"></param>
        public void ShowHosting(int index, bool isHost)
        {
            _playingHead[index].GetChild(13).gameObject.SetActive(isHost);
        }

        /// <summary>
        /// 显示取消托管按钮
        /// </summary>
        public void ShowCanelHosting(bool isShow)
        {
            CanelHostingBtn.SetActive(isShow);
        }

        public Animator[] _headAnim;
        /// <summary>
        /// 显示哪个玩家下牌
        /// </summary>
        /// <param name="index">显示的</param>
        /// <param name="uindex">不显示的</param>
        public void ShowWitchPlayerPlay(int index)
        {
            for (int i = 0; i < 4; i++)
            {
                _TimeRemi.GetChild(0).GetChild(i).gameObject.SetActive(false);
                _headAnim[i].GetComponent<SpriteRenderer>().enabled = false;
            }
            if (index >= 0 & index <= 4)
            {
              //  Debug.LogWarning(GameData.Instance.PlayerPlayingPanelData.GetOtherPlayerShowPos(index + 1) - 1);
                _headAnim[GameData.Instance.PlayerPlayingPanelData.GetOtherPlayerShowPos(index + 1) - 1].GetComponent<SpriteRenderer>().enabled = true;
                _TimeRemi.GetChild(0).GetChild(index).gameObject.SetActive(true);

            }
        }
        /// <summary>
        /// 播放聊天快捷语动画
        /// </summary>
        /// <param name="index"></param>
        public Text[] _textShortTalk = new Text[4];
        public void PlayShortTalk(int index, int id)
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            _textShortTalk[index].text = std.szShortTalk[SDKManager.Instance.isex][id];
            Transform trans = _textShortTalk[index].transform.parent;
            int len = std.szShortTalk[SDKManager.Instance.isex][id].Length;
            Debug.LogWarning(len);
            if (len <= 15)
            {
                trans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 110 + (len - 3) * 20);
                trans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 74);
                trans.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                //_textShortTalk[index].transform.localPosition = Vector3.zero;
                //_textShortTalk[index].transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                //_textShortTalk[index].transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal , 0);

            }
            else
            {
                trans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 277);
                trans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 94);
                //_textShortTalk[index].transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                //_textShortTalk[index].transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                trans.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            }
            Debug.LogWarning(_textShortTalk[index].gameObject.name);
            _textShortTalk[index].gameObject.SetActive(true);
            trans.gameObject.SetActive(true);
            Tweener dt = trans.GetComponent<DOTweenAnimation>().tween as Tweener;
            dt.Restart();
            dt.OnComplete(() =>
        {
            _textShortTalk[index].gameObject.SetActive(false);
        });
        }

        public void SetPanelData(int cLeaveType, int pos, int userid)
        {
            StartCoroutine(SetPanelData_(cLeaveType, pos, userid));
        }

        IEnumerator SetPanelData_(int cLeaveType, int pos, int userid)
        {
            yield return new WaitForSeconds(0.2f);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            switch (cLeaveType)
            {
                case 1://退出程序
                    if (pppd.usersInfo.ContainsKey(pos))
                    {
                        Debug.LogWarning("删除玩家节点信息，更新界面");
                        pppd.usersInfo.Remove(pos);
                        SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                    }
                    break;
                case 2:
                    //删除玩家节点信息，更新界面
                    if (pppd.usersInfo.ContainsKey(pos))
                    {
                        pppd.usersInfo.Remove(pos);
                        SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                    }
                    break;
                case 3://游戏状态下掉线
                    UIMainView.Instance.PlayerPlayingPanel.ShowLeavePerson(pppd.GetOtherPlayerShowPos(pos) - 1, true);
                    //保存玩家的离线信息
                    pppd.DisConnectStatus[pos - 1] = 1;
                    break;
                case 4://游戏状态下离开
                    Debug.LogWarning("玩家游戏状态下离开");
                    break;
                case 5://预约房，预约用户已占座离开游戏
                    {
                        Debug.LogWarning("预约房，预约用户已占座离开游戏");
                        UIMainView.Instance.PlayerPlayingPanel.OnSetYuYueGameTabel(3, pppd.GetOtherPlayerShowPos(pos) - 1);
                        int index = pppd.GetOtherPlayerShowPos(pos) - 1;
                        UIMainView.Instance.PlayerPlayingPanel._txUsersScor[index].text = "占座中";
                        pppd.DisConnectStatus[pos - 1] = 1;
                    }
                    break;
                case 6:
                    {
                        Debug.LogWarning("非馆主开房离开");
                        pppd.DisConnectStatus[pos - 1] = 1;
                        pppd.usersInfo.Remove(pos);
                        SystemMgr.Instance.PlayerPlayingSystem.OnPlayerPlayingHeadUpdate(pos);
                    }
                    break;
                default:
                    break;
            }
        }


        bool hasHeadShow;
        /// <summary>
        /// 处理所有玩家的头像显示问题     
        /// </summary>
        /// <param name="pos">玩家座位号1--4</param>
        /// <param name="num">1-全部更新、2-只更新头像、3-出头像之外更新</param>
        public void HeadUpdateShow(int pos, byte num = 2)
        {
            if (pos < 1 || pos > 5)
            {
                return;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //Debug.LogError("pppd.usersInfo.ContainsKey(1):" + pppd.usersInfo.ContainsKey(1));

            //如果不是代开且玩家座位号为1  则为房主
            if (pppd.usersInfo.ContainsKey(1))
            {
                if (pppd.iOpenRoomUserId == pppd.usersInfo[1].iUserId)
                {
                    UpdataFangZhu(1);
                }
            }


            int index = pppd.GetOtherPlayerShowPos(pos) - 1;
            if (num == 1 || num == 2)
            {
                //处理自己的面板更新

                if (pos == pppd.playerPos)
                {
                    //处理等待面吧头像
                    _playingHead[0].gameObject.SetActive(true);
                    _playingHead[0].GetChild(0).GetChild(0).gameObject.SetActive(true);
                    NonePlayer[0].gameObject.SetActive(false);
                    _txUsersNickName[0].text = pppd.usersInfo[pppd.playerPos].szNickname;
                    //_txUsersScor[0].text = "0";   
                    if (!hasHeadShow)
                    {
                        MahjongCommonMethod.Instance.GetPlayerAvatar(_playingHead[0].GetChild(0).GetChild(0).GetComponent<RawImage>(), MahjongLobby_AH.GameData.Instance.PlayerNodeDef.szHeadimgurl);
                        hasHeadShow = true;
                    }
                    //处理准备按钮
                    //Debug.LogError("seatnum:" + pos + ",ready:" + pppd.usersInfo[pos].cReady);
                    if (pppd.usersInfo[pos].cReady == 1)
                    {
                        ReadyImage[0].gameObject.SetActive(true);
                    }
                    else
                    {
                        ReadyImage[0].gameObject.SetActive(false);
                    }
                }
                else
                {
                    //处理别人的面板更新            
                    if (!pppd.usersInfo.ContainsKey(pos))
                    {
                        _playingHead[index].gameObject.SetActive(false);
                        _playingHead[index].GetChild(0).GetChild(0).gameObject.SetActive(false);
                        NonePlayer[index].gameObject.SetActive(true);
                        ReadyImage[index].gameObject.SetActive(false);
                    }
                    else
                    {
                        _playingHead[index].gameObject.SetActive(true);
                        _playingHead[index].GetChild(0).GetChild(0).gameObject.SetActive(true);
                        NonePlayer[index].gameObject.SetActive(false);
                        _txUsersNickName[index].text = pppd.usersInfo[pos].szNickname;
                        MahjongCommonMethod.Instance.GetPlayerAvatar(_playingHead[index].GetChild(0).GetChild(0).GetComponent<RawImage>(), pppd.usersInfo[pos].szHeadimgurl);
                        //_txUsersScor[index].text = "0";
                        if (m_isYuYueRoom)
                        {
                            _txUsersScor[index].text = "占座中";
                            //PlayerInSeat[index].gameObject.SetActive(true);
                        }
                        if (pppd.usersInfo[pos].cReady == 1)
                        {
                            //Debug.LogError("pos:" + pos);
                            ReadyImage[index].gameObject.SetActive(true);
                        }
                        else
                        {
                            ReadyImage[index].gameObject.SetActive(false);
                        }
                    }
                }
            }

            //更新庄家
            if (num == 1 || num == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    _playingHead[i].GetChild(5).gameObject.SetActive(false);
                }
                _playingHead[index].GetChild(5).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 产生玩家的桌面上的特殊牌型
        /// </summary>
        /// <param name="value">牌的花色值</param>
        /// <param name="seatNum">玩家座位号</param>
        /// <param name="specialTile">特殊牌的状态，1吃 2碰 3杠 4胡 5吃（十三幺） 6抢（十三幺）</param>
        /// <param name="KongStatus">杠的状态,1表示明杠，2表示暗杠，3表示碰杠</param> 
        /// <param name="BreakConnect"></param> 
        public void SpwanSpecialCard(byte[] value, int seatNum, byte specialTile, bool BreakConnect, int bySeatNum, int KongStatus = 0)
        {
            //在处理桌面上的特殊牌型的时候，停止接收消息
            Network.NetworkMgr.Instance.GameServer.Lock();

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (seatNum == pppd.bySeatNum && KongStatus != 3)
            {
                if (pppd.bAnkongStatus == 1)
                {
                    //KongStatus = 2;
                    pppd.bAnkongStatus = 0;
                }
                pppd.iSpecialCardNum++;
            }
            //0---3
            int index = pppd.GetOtherPlayerShowPos(seatNum) - 1;
            //产生对应的特殊牌型

            GameObject obj = null;

            if (index == 3)
            {
                if (specialTile == 5 || specialTile == 6)
                {
                    _Cards[index].Find("currentSpecialGroup").GetComponent<VerticalLayoutGroup>().padding.top = 25;
                }
                else
                {
                    _Cards[index].Find("currentSpecialGroup").GetComponent<VerticalLayoutGroup>().padding.top = -15;
                }
            }

            //处理吃碰杠
            if (specialTile == 1 || specialTile == 2 || specialTile == 3)
            {
                int status = -1;
                if (specialTile == 1 || specialTile == 2)
                {
                    pppd.isSpwanSpecialCard = true;
                    status = 0;
                }
                else
                {
                    status = KongStatus;
                }
                //删除多余的牌型
                if (!BreakConnect)
                {
                    DelSpecialCard(value, seatNum, status);
                }


                if (KongStatus != 3)
                {
                    //Debug.LogError("index:" + index);
                    if (index == 0)
                    {
                        obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaDCardsPre);
                    }
                    else if (index == 1 || index == 3)
                    {
                        obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaHCardsPre);
                    }
                    else
                    {
                        obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaUCardsPre);
                    }
                    if (index == 0)
                    {
                        obj.transform.SetParent(_Cards[index].Find("currentGroup"));
                    }
                    else
                    {
                        obj.transform.SetParent(_Cards[index].Find("currentSpecialGroup"));
                    }

                    //obj.transform.SetSiblingIndex(pppd.iSpecialCardNum - 1);                 
                    obj.transform.GetComponent<Mahjong>().bMahjongValue = value[0];
                    if (index == 0)
                    {
                        obj.transform.localPosition = new Vector3(fspecialCardInitPos + fSpecialCardInterval * (pppd.iSpecialCardNum - 1), 5, 0);
                    }
                    else
                    {
                        //obj.transform.SetSiblingIndex(0);
                        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
                    }
                    obj.transform.localScale = new Vector3(1, 1, 1);

                    obj.transform.localEulerAngles = Vector3.zero;

                    //设置预置体在父物体下的下标位置
                    obj.transform.SetAsLastSibling();
                    //把碰杠的值保存到他的mahjong中
                    obj.GetComponent<Mahjong>().bMahjongValue = value[0];
                    if (bySeatNum != 0 && bySeatNum <= 4)
                    {
                        int aa = bySeatNum - seatNum;
                        int bb = pppd.GetOtherPlayerShowPos(seatNum) - 1;
                        obj.transform.GetComponent<Mahjong>()._imageFinger.gameObject.SetActive(true);
                        if (aa == 2 || aa == -2)
                        {
                            aa = 0;
                        }
                        obj.transform.GetComponent<Mahjong>()._imageFinger.transform.localEulerAngles = new Vector3(0, 0, -aa * 90 + bb * 90);
                    }
                    else
                    {
                        obj.transform.GetComponent<Mahjong>()._imageFinger.gameObject.SetActive(false);
                    }
                }

                int type = specialTile;
                int kongstatus = 0;
                //处理如果玩家碰杠，直接改变原来的预置体变为杠
                if (specialTile == 3)
                {
                    GameObject go = null;
                    kongstatus = KongStatus;
                    if (KongStatus == 3)
                    {
                        //如果是碰杠单独处理
                        Mahjong[] mah = null;
                        if (index == 0)
                        {
                            mah = _Cards[index].transform.Find("currentGroup").GetComponentsInChildren<Mahjong>(true);
                        }
                        else
                        {
                            mah = _Cards[index].transform.Find("currentSpecialGroup").GetComponentsInChildren<Mahjong>(true);
                        }
                        //Mahjong[] mah = _Cards[index].transform.FindChild("currentSpecialGroup").GetComponentsInChildren<Mahjong>(true);
                        for (int l = 0; l < mah.Length; l++)
                        {
                            if (mah[l].name.Contains("pega") && mah[l].bMahjongValue == value[0])
                            {
                                go = mah[l].gameObject;
                                break;
                            }
                        }
                        UpdateSpecialCardImage(value, KongStatus, index, go, specialTile);
                    }
                }
                UpdateSpecialCardImage(value, kongstatus, index, obj, specialTile);

            }
            //处理玩家的十三幺的吃抢功能
            if (specialTile == 5 || specialTile == 6)
            {
                if (index == 0)
                {
                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showVCardPre);
                    obj.transform.SetParent(_Cards[index].Find("currentGroup"));
                    obj.transform.localScale = Vector3.one * 1.6f;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.localPosition = new Vector3(showVCardPre_InitPos.transform.localPosition.x + fSpecialCard_Thirteen_Interval * (pppd.iSpecialCardNum - 1),
                        showVCardPre_InitPos.transform.localPosition.y, 0);
                    obj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    obj.transform.Find("Image/num").transform.localEulerAngles = Vector3.zero;
                    obj.transform.SetAsLastSibling();
                }
                else if (index == 2)
                {
                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showVCardPre);
                    obj.transform.SetParent(_Cards[index].Find("currentSpecialGroup"));
                    obj.transform.GetComponent<LayoutElement>().preferredWidth = 50f;
                    obj.transform.Find("Image").transform.localPosition += new Vector3(10f, 0, 0);
                    obj.transform.SetAsLastSibling();
                    obj.transform.GetComponent<Mahjong>().bMahjongValue = value[0];
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.Find("Image/num").localEulerAngles = new Vector3(0, 0, 180);
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);

                }
                else if (index == 1)
                {
                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showHCardPre);
                    obj.transform.SetParent(_Cards[index].Find("currentSpecialGroup"));
                    obj.transform.SetAsLastSibling();
                    obj.transform.GetComponent<Mahjong>().bMahjongValue = value[0];
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localEulerAngles = new Vector3(0, 0, 90);
                    obj.transform.Find("Image/num").localEulerAngles = new Vector3(0, 0, 90);
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
                }
                else if (index == 3)
                {
                    //如果是吃抢牌 修改第四个玩家的特殊牌的位置


                    obj = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showHCardPre);
                    obj.transform.SetParent(_Cards[index].Find("currentSpecialGroup"));
                    obj.transform.SetAsFirstSibling();
                    obj.transform.GetComponent<Mahjong>().bMahjongValue = value[0];
                    obj.transform.GetComponent<LayoutElement>().preferredWidth = 45f;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localEulerAngles = new Vector3(0, 0, 0);
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
                    obj.transform.Find("Image").transform.localEulerAngles = Vector3.zero;
                    obj.transform.Find("Image").transform.localPosition += new Vector3(0, 25f, 0);
                    obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, -90f);
                }

                //更新牌面的信息
                //obj.transform.Find("Image/num").GetComponent<Image>().sprite = ChangeCardNum(value[0]);
                ChangeCardNum(obj.transform.Find("Image/num").GetComponent<Image>(), value[0], index);
                if (pppd.bySeatNum == seatNum)
                {
                    //平移所有的牌
                    Mahjong[] mah = MahjongManger.Instance.GetSelfCard();
                    for (int i = 0; i < mah.Length; i++)
                    {
                        mah[i].MoveSelf(2, 1);
                    }
                    pppd.isSpwanSpecialCard = true;
                }

            }

            //如果是碰杠直接改变状态不是新增
            if (specialTile == 3 && KongStatus == 3)
            {
                for (int i = 0; i < pppd.usersCardsInfo[index].listSpecialCards.Count; i++)
                {
                    if (pppd.usersCardsInfo[index].listSpecialCards[i].mahValue[0] == value[0])
                    {
                        pppd.usersCardsInfo[index].listSpecialCards[i].type = 3;
                        pppd.usersCardsInfo[index].listSpecialCards[i].mahValue[3] = value[0];
                    }
                }
            }
            else
            {
                byte[] mahValue = new byte[4] { value[0], value[1], value[2], 0 };
                int specialTile_ = 0;
                //添加吃碰杠信息到玩家的信息中
                specialTile_ = specialTile;
                if (specialTile == 3)
                {
                    if (KongStatus == 2)
                    {
                        //Debug.LogError("KongStatus:" + KongStatus);
                        specialTile_ = 7;
                    }
                }
                SpecialCard card = new SpecialCard(specialTile_, mahValue);
                pppd.usersCardsInfo[index].listSpecialCards.Add(card);
            }

            //if (specialTile == 3 && KongStatus == 2 && !pppd.isBreakConnect)
            if ((specialTile == 3 || specialTile == 1) && !pppd.isBreakConnect)
            {
                pppd.CurrentCradSort(2);
            }

            if (!GameData.Instance.PlayerPlayingPanelData.isGangToPeng_Later)
            {
                //Debug.LogError("开始接收消息=====================3");
                //开始接受消息
                Network.NetworkMgr.Instance.GameServer.Unlock();
            }

            //Debug.LogError("=============================0");
        }


        /// <summary>
        /// 改变吃碰杠胡的麻将的显示情况
        /// </summary>
        /// <param name="value">牌的花色值</param>
        /// <param name="KongStatus">杠的状态，0没有杠，1明杠，2暗杠,3碰杠</param>
        /// <param name="index">玩家对应的数组下标</param>
        /// <param name="go">特殊牌的预置体</param>
        void UpdateSpecialCardImage(byte[] value, int KongStatus, int index, GameObject go, byte specialType)
        {
            if (go == null)
            {
                return;
            }

            //获取子物体下的具体物体
            Transform tran_L = go.transform.Find("belowCardL");
            Transform tran_M = go.transform.Find("belowCardM");
            Transform tran_R = go.transform.Find("belowCardR");
            Transform tran_U = go.transform.Find("topCard");
            Transform num_L = tran_L.transform.Find("num");
            Transform num_M = tran_M.transform.Find("num");
            Transform num_R = tran_R.transform.Find("num");
            Transform num_U = tran_U.transform.Find("num");


            if (index == 1)
            {
                num_L.transform.localEulerAngles = new Vector3(0, 0, 90f);
                num_M.transform.localEulerAngles = new Vector3(0, 0, 90f);
                num_R.transform.localEulerAngles = new Vector3(0, 0, 90f);
                num_U.transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (index == 2)
            {
                num_L.transform.localEulerAngles = new Vector3(0, 0, 180);
                num_M.transform.localEulerAngles = new Vector3(0, 0, 180);
                num_R.transform.localEulerAngles = new Vector3(0, 0, 180);
                num_U.transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else if (index == 3)
            {
                num_L.transform.localEulerAngles = new Vector3(0, 0, 270);
                num_M.transform.localEulerAngles = new Vector3(0, 0, 270);
                num_R.transform.localEulerAngles = new Vector3(0, 0, 270);
                num_U.transform.localEulerAngles = new Vector3(0, 0, 270);
            }


            //处理碰
            if (specialType == 2 || specialType == 1)
            {
                //num_L.GetComponent<Image>().sprite = ChangeCardNum(value[0], index);
                //num_M.GetComponent<Image>().sprite = ChangeCardNum(value[2] == 0 ? value[0] : value[2],index);
                //num_R.GetComponent<Image>().sprite = ChangeCardNum(value[1] == 0 ? value[0] : value[1], index);
                ChangeCardNum(num_L.GetComponent<Image>(), value[0], index);
                ChangeCardNum(num_M.GetComponent<Image>(), value[2] == 0 ? value[0] : value[2], index);
                ChangeCardNum(num_R.GetComponent<Image>(), value[1] == 0 ? value[0] : value[1], index);
            }
            else
            {
                if (KongStatus == 1 || KongStatus == 3)
                {
                    //明杠
                    ChangeCardNum(num_L.GetComponent<Image>(), value[0], index);
                    ChangeCardNum(num_M.GetComponent<Image>(), value[0], index);
                    ChangeCardNum(num_R.GetComponent<Image>(), value[0], index);
                    tran_U.GetComponent<Image>().enabled = true;
                    num_U.GetComponent<Image>().enabled = true;
                    ChangeCardNum(num_U.GetComponent<Image>(), value[0], index);
                }
                else
                {
                    tran_U.transform.Find("Image").gameObject.SetActive(false);
                    //处理玩家如果是不让其他玩家看见杠的牌面
                    if (PPPD.iMethodId == 13)
                    {
                        if (index == 0)
                        {
                            tran_U.GetComponent<Image>().enabled = true;
                            tran_U.GetComponent<Image>().sprite = _myMingGangBK;
                            num_U.GetComponent<Image>().enabled = true;
                            ChangeCardNum(num_U.GetComponent<Image>(), value[0], index);
                            tran_L.GetComponent<Image>().enabled = true;
                            tran_L.GetComponent<Image>().sprite = _myAnGangBK;
                            num_L.gameObject.SetActive(false);
                            tran_R.GetComponent<Image>().enabled = true;
                            tran_R.GetComponent<Image>().sprite = _myAnGangBK;
                            num_R.gameObject.SetActive(false);
                        }
                        else
                        {
                            tran_U.GetComponent<Image>().enabled = true;
                            tran_U.GetComponent<Image>().sprite = _myAnGangBK;
                            num_U.GetComponent<Image>().enabled = false;
                            //num_U.GetComponent<Image>().sprite = ChangeCardNum(value[0]);
                            tran_L.GetComponent<Image>().enabled = true;
                            tran_L.GetComponent<Image>().sprite = _myAnGangBK;
                            num_L.gameObject.SetActive(false);
                            tran_R.GetComponent<Image>().enabled = true;
                            tran_R.GetComponent<Image>().sprite = _myAnGangBK;
                            num_R.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        //暗杠
                        if (index == 0 || index == 2)
                        {
                            tran_U.GetComponent<Image>().enabled = true;
                            tran_U.GetComponent<Image>().sprite = _myMingGangBK;
                            num_U.GetComponent<Image>().enabled = true;
                            ChangeCardNum(num_U.GetComponent<Image>(), value[0], index);
                            tran_L.GetComponent<Image>().enabled = true;
                            tran_L.GetComponent<Image>().sprite = _myAnGangBK;
                            num_L.gameObject.SetActive(false);
                            tran_R.GetComponent<Image>().enabled = true;
                            tran_R.GetComponent<Image>().sprite = _myAnGangBK;
                            num_R.gameObject.SetActive(false);
                        }
                        else
                        {
                            tran_U.GetComponent<Image>().enabled = true;
                            tran_U.GetComponent<Image>().sprite = _heMingGangBK;
                            num_U.GetComponent<Image>().enabled = true;
                            ChangeCardNum(num_U.GetComponent<Image>(), value[0], index);
                            tran_L.GetComponent<Image>().enabled = true;
                            tran_L.GetComponent<Image>().sprite = _heAnGangBK;
                            num_L.gameObject.SetActive(false);
                            tran_R.GetComponent<Image>().enabled = true;
                            tran_R.GetComponent<Image>().sprite = _heAnGangBK;
                            num_R.gameObject.SetActive(false);
                        }
                    }
                }
            }

        }
        private IEnumerator PlayMoneyAnim(int fromIndex, int toIndex)
        {
            Tweener[] tw = new Tweener[4];
            GameObject[] gos = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
              //  Debug.LogError(i);
                yield return new WaitForSeconds(0.13f);
                gos[i] = Instantiate(Resources.Load<GameObject>("Game/moveProp")) as GameObject;
                gos[i].GetComponent<Image>().sprite = sprit_props[2];
                gos[i].transform.SetParent(_playingChangeHead[fromIndex].parent);
                gos[i].transform.localPosition = _playingChangeHead[fromIndex].localPosition;
                tw[i] = gos[i].GetComponent<DOTweenAnimation>().tween as Tweener;
                tw[i].ChangeEndValue(_playingChangeHead[toIndex].localPosition);
                tw[i].Restart();
            }
        }
        public Sprite[] sprit_props = new Sprite[4];
        /// <summary>
        /// 播放收到聊天消息道具动画
        /// </summary>
        /// <param name="fromIndex">来自用户</param>
        /// <param name="toIndex"></param>
        /// <param name="byContentId"></param>

        internal void PlayGameTools(int fromIndex, int toIndex, byte byContentId)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/moveProp")) as GameObject;
            go.GetComponent<Image>().sprite = sprit_props[byContentId - 1];
            go.transform.SetParent(_playingChangeHead[fromIndex].parent);
            go.transform.localPosition = _playingChangeHead[fromIndex].localPosition;
            Tweener dt = go.GetComponent<DOTweenAnimation>().tween as Tweener;
            dt.ChangeEndValue(_playingChangeHead[toIndex].localPosition);
            dt.Restart();
           // Debug.LogWarning("byContentId" + byContentId);
            if (byContentId == 1)
            {
                dt.OnComplete(() =>
                {
                   // Debug.LogWarning("ok1");
                    GameObject go1 = Instantiate(Resources.Load<GameObject>("Game/flower"));
                    go1.transform.SetParent(_playingChangeHead[toIndex]);
                    go1.transform.localPosition = Vector3.zero;
                    go1.transform.localScale = new Vector3(110, 110, 1);
                    go1.GetComponent<animationPlay>().PlayAnim(byContentId);
                    // dt.tween.Kill();
                    Destroy(go);
                });
            }
            else if (byContentId == 2)
            {
                dt.OnComplete(() =>
                {
                    GameObject go1 = Instantiate(Resources.Load<GameObject>("Game/hand"));
                    go1.transform.SetParent(_playingChangeHead[toIndex]);
                    go1.transform.localPosition = Vector3.zero;
                    go1.transform.localScale = new Vector3(110, 110, 1);
                    go1.GetComponent<animationPlay>().PlayAnim(byContentId);
                    // dt.tween.Kill();
                    Destroy(go);
                });
            }
            else if (byContentId == 3)
            {
                StartCoroutine(PlayMoneyAnim(fromIndex, toIndex));
                dt.OnComplete(() =>
                {
                    Destroy(go);
                    GameObject go1 = Instantiate(Resources.Load<GameObject>("Game/money"));
                    go1.transform.SetParent(_playingChangeHead[toIndex]);
                    go1.transform.localPosition = Vector3.zero;
                    go1.transform.localScale = new Vector3(110, 110, 1);
                    go1.GetComponent<animationPlay>().PlayAnim(byContentId);
                    // dt.tween.Kill();
                });
            }
            else if (byContentId == 4)
            {
                dt.OnComplete(() =>
                {
                    GameObject go1 = Instantiate(Resources.Load<GameObject>("Game/props"));
                    go1.transform.SetParent(_playingChangeHead[toIndex]);
                    go1.transform.localPosition = Vector3.zero;
                    go1.transform.localScale = new Vector3(110, 110, 1);
                    go1.GetComponent<animationPlay>().PlayAnim(byContentId);
                    // dt.tween.Kill();
                    Destroy(go);
                });

            };
        }




        /// <summary>
        /// 删除对应的玩家的特殊牌型对应的手牌
        /// </summary>
        /// <param name="value">应该删除的手牌花色值</param>
        /// <param name="seatNum">玩家对应的座位号</param>
        /// <param status="0表示碰顺子，1表示明杠，2表示暗杠，3表示碰杠">特殊牌状态</param>
        public void DelSpecialCard(byte[] value, int seatNum, int status)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (value.Length <= 0)
            {
                return;
            }
            if (seatNum < 1 || seatNum > 4)
            {
                return;
            }

            bool isAnKongByHandCard = false;
            //存储要删除牌的数据
            byte[] byteMah = null;

            //保存玩家要删的牌面的下标            
            int cardIndex_a = -1;
            //座位号转换为对应的数组下标0---3
            int index = pppd.GetOtherPlayerShowPos(seatNum) - 1;

            Transform parent = _Cards[index].Find("currentGroup");

            //所有的子物体
            Mahjong[] mahChild = pppd.GetTransfromChild(parent);

            //根据玩家特殊牌状态决定如何删除手牌
            if (seatNum != pppd.bySeatNum)
            {
                int icradCount = 0;
                if (status == 0)
                {
                    icradCount = 2;
                }
                else if (status == 1)
                {
                    icradCount = 3;
                }
                else if (status == 2)
                {
                    icradCount = 4;
                }
                else if (status == 3)
                {
                    icradCount = 1;
                }

                //删除玩家的牌
                if (index == 1 || index == 2)
                {
                    for (int i = 0; i < icradCount; i++)
                    {
                        PoolManager.Unspawn(mahChild[i].gameObject);
                    }
                }
                else
                {
                    for (int i = 0; i < icradCount; i++)
                    {
                        PoolManager.Unspawn(mahChild[mahChild.Length - 1 - i].gameObject);
                    }
                }

            }
            else
            {
                //存储要删除的牌型
                List<byte> listMah = new List<byte>();
                bool isDelHandCard = false;  //碰杠，删除刚摸的手牌                            
                //处理碰
                if (status == 0)
                {
                    listMah.Add(value[0]);
                    listMah.Add(value[1] == 0 ? value[0] : value[1]);
                    isDelHandCard = false;
                }
                //处理明杠
                else if (status == 1)
                {
                    //Debug.LogError("处理明杠的状态");
                    listMah.Add(value[0]);
                    listMah.Add(value[0]);
                    listMah.Add(value[0]);
                    isDelHandCard = false;
                }
                //处理暗杠
                else if (status == 2)
                {
                    //Debug.LogError("处理暗杠的状态");
                    listMah.Add(value[0]);
                    listMah.Add(value[0]);
                    listMah.Add(value[0]);
                    listMah.Add(value[0]);
                    isDelHandCard = false;
                }
                //处理碰杠
                else if (status == 3)
                {
                    Debug.LogWarning("处理碰杠的状态3");
                    listMah.Add(value[0]);
                    isDelHandCard = true;
                }

                byteMah = listMah.ToArray();

                if (isDelHandCard)
                {
                    Mahjong mah_ga = MahjongManger.Instance.GetPointCard(value[0]);
                    //处理碰杠的特殊情况，是摸到的手牌进行碰杠
                    if (mah_ga.isDealCard)
                    {
                        Debug.LogWarning("碰杠的牌是刚摸的手牌");
                        //移除对应的list数据
                        for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
                        {
                            if (mah_ga.bMahjongValue == pppd.usersCardsInfo[0].listCurrentCards[i].cardNum &&
                                mah_ga.iMahId == pppd.usersCardsInfo[0].listCurrentCards[i].MahId)
                            {
                                pppd.usersCardsInfo[0].listCurrentCards.RemoveAt(i);
                                MahjongManger.Instance.insertCardIndex = i - 1;
                                break;
                            }
                        }
                        MahjongManger.Instance.iputCardIndex = pppd.usersCardsInfo[0].listCurrentCards.Count - 1;
                    }
                    else
                    {
                        Debug.LogWarning("碰杠的牌不是刚摸的手牌");
                        pppd.isGangToPeng_Later = true;
                        //移除对应的list数据
                        for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
                        {
                            if (mah_ga.bMahjongValue == pppd.usersCardsInfo[0].listCurrentCards[i].cardNum &&
                                mah_ga.iMahId == pppd.usersCardsInfo[0].listCurrentCards[i].MahId)
                            {
                                pppd.usersCardsInfo[0].listCurrentCards.RemoveAt(i);
                                MahjongManger.Instance.iputCardIndex = i;
                                break;
                            }
                        }

                        //获取上次手牌的位置
                        int deadcardValue = -1;
                        int mahid = -1;
                        if (MahjongManger.Instance.PlayerDealHandCrad != null)
                        {
                            deadcardValue = MahjongManger.Instance.PlayerDealHandCrad.bMahjongValue;
                            mahid = MahjongManger.Instance.PlayerDealHandCrad.iMahId;
                        }
                        for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
                        {
                            if (deadcardValue == pppd.usersCardsInfo[0].listCurrentCards[i].cardNum && mahid == pppd.usersCardsInfo[0].listCurrentCards[i].MahId)
                            {
                                MahjongManger.Instance.insertCardIndex = i;
                                break;
                            }
                        }

                        mah_ga.PutCard(2);
                    }
                    //删除玩家碰杠的牌
                    PoolManager.Unspawn(mah_ga.gameObject);
                }
                else
                {
                    int value_bMahjongValue_byteMah_index = 0;
                    //处理碰、明杠、暗杠，删除对应的手中的牌                    
                    for (int j = 0; j < byteMah.Length; j++)
                    {
                        cardIndex_a = -1;

                        mahChild = pppd.GetTransfromChild(parent);
                        //获取玩家当前的牌
                        for (int i = 0; i < mahChild.Length; i++)
                        {
                            if (value_bMahjongValue_byteMah_index > byteMah.Length)
                            {
                                continue;
                            }

                            if (mahChild[i].bMahjongValue == byteMah[value_bMahjongValue_byteMah_index])
                            {
                                value_bMahjongValue_byteMah_index++;
                                //对摸到手里的牌进行暗杠
                                if (status == 2 && mahChild[i].isDealCard)
                                {
                                    isAnKongByHandCard = true;
                                    pppd.CurrentCradSort(0);
                                }

                                for (int k = 0; k < pppd.usersCardsInfo[0].listCurrentCards.Count; k++)
                                {
                                    if (pppd.usersCardsInfo[0].listCurrentCards[k].cardNum == mahChild[i].bMahjongValue)
                                    {
                                        //获取对应牌的下标
                                        if (cardIndex_a == -1)
                                        {
                                            cardIndex_a = k;
                                        }

                                        //取最小值
                                        if (cardIndex_a > k)
                                        {
                                            cardIndex_a = k;
                                        }
                                    }
                                }

                                pppd.usersCardsInfo[0].listCurrentCards.RemoveAt(cardIndex_a);
                                PoolManager.Unspawn(mahChild[i].gameObject);
                                break;
                            }
                        }
                    }

                    MahjongManger.Instance.insertCardIndex = cardIndex_a;
                    MahjongManger.Instance.iputCardIndex = 0;

                    //对暗杠进行特殊处理
                    if (status == 2 && seatNum == pppd.bySeatNum)
                    {
                        //先把牌移动到右边三个单位
                        Mahjong[] mah_0 = MahjongManger.Instance.GetWillMoveCard();

                        int count = byteMah.Length;

                        if (!isAnKongByHandCard)
                        {
                            count = 4;
                        }
                        else
                        {
                            if (count >= 3)
                            {
                                count = 3;
                            }
                        }

                        //在对右边的牌，左移一个单位         
                        for (int i = 0; i < mah_0.Length; i++)
                        {
                            mah_0[i].transform.localPosition += new Vector3(85f * count, 0f, 0f);
                        }

                        pppd.iSpwanCardNum -= byteMah.Length;
                        //处理玩家刚摸的手牌的位置
                        if (isAnKongByHandCard)
                        {
                            isAnKongByHandCard = false;
                        }

                        //pppd.CurrentCradSort(2);
                        //Debug.LogError("=============================1");
                        return;
                    }

                }

                //只有是玩家自己
                if (seatNum == pppd.bySeatNum)
                {
                    pppd.iSpwanCardNum -= byteMah.Length;
                }

                if (status == 0)
                {
                    //移动对应的牌型
                    Mahjong[] mah = MahjongManger.Instance.GetWillMoveCard();
                    if (isDelHandCard)
                    {
                        for (int i = 0; i < mah.Length; i++)
                        {
                            mah[i].MoveSelf(1, 1);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < mah.Length; i++)
                        {
                            mah[i].MoveSelf(2, byteMah.Length);
                        }
                    }
                }

                //如果玩家在吃碰之后，移动牌到右边一个位置
                Debug.LogWarning("seatNum:" + seatNum + ",pppd.bySeatNum:" + pppd.bySeatNum
                    + ",status:" + status);

                if (seatNum == pppd.bySeatNum && status == 0)
                {
                    Debug.LogWarning("peng");
                    StartCoroutine(DelayMovePos_Pong(1));
                }
            }
        }

        IEnumerator DelayMovePos_Pong(int status)
        {
            yield return new WaitForSeconds(0.15f);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //如果碰杠之后，选择碰，会对牌型重新排序
            if (pppd.isGangToPeng_Sort)
            {
                pppd.isGangToPeng_Sort = false;
                pppd.CurrentCradSort(2);
            }
            pppd.DelayDiscardTileNotice(pppd.DiscardTileNotice);
        }


        /// <summary>
        /// 点击吃碰杠胡 请求消息
        /// </summary>
        /// <param name="index">下标</param>
        public void BtnSpecialCard(int index)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);

            if (index == 3)
            {
                Transform trans = SpecialTileNotice[2].transform;
                //如果有两个杠的牌
                if (pppd.bkongValue_.Count > 1)
                {
                    Transform temp = trans.Find("Gang");
                    trans.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    trans.GetChild(0).gameObject.SetActive(false);
                    //隐藏其他的牌
                    for (int i = 0; i < 3; i++)
                    {
                        temp.GetChild(i).gameObject.SetActive(false);
                    }

                    //显示可以杠的牌
                    for (int i = 0; i < pppd.bkongValue_.Count; i++)
                    {
                        temp.GetChild(i).gameObject.SetActive(true);
                        //更新牌的值
                        ChangeCardNum(temp.GetChild(i).Find("Image").GetComponent<Image>(), pppd.bkongValue_[i].kongValue);
                    }
                    temp.gameObject.SetActive(true);

                    //处理背景位置宽度
                    if (pppd.bkongValue_.Count > 2)
                    {
                        temp.transform.localPosition = new Vector3(-45f, 0, 0);
                        temp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 75 * pppd.bkongValue_.Count);
                    }
                    else
                    {
                        temp.transform.localPosition = new Vector3(-10f, 0, 0);
                        temp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 75 * pppd.bkongValue_.Count);
                    }

                    //改变背景
                    SpecialTileNoticeBg.gameObject.SetActive(false);

                    return;
                }

                trans.Find("Gang").gameObject.SetActive(false);
                //隐藏其他的牌
                for (int i = 0; i < 3; i++)
                {
                    trans.Find("Gang").GetChild(i).gameObject.SetActive(false);
                }
                trans.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                trans.GetChild(0).gameObject.SetActive(true);

                pppd.isGangToPeng_Sort = false;
            }
            if (index == 1)
            {
                pppd.isGangToPeng_Sort = true;
                OnSpwanEatShow(index);
            }
            else
            {
                Messenger_anhui<int>.Broadcast(MESSAGE_BTNSPECIALCARD, index);
            }
        }


        /// <summary>
        /// 点击杠的牌
        /// </summary>
        /// <param name="index"></param>
        public void BtnGangCard(int index)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            byte[] mah = new byte[2];
            mah[0] = pppd.bkongValue_[index].kongValue;
            Network.Message.NetMsg.ClientSpecialTileReq msg = new Network.Message.NetMsg.ClientSpecialTileReq();
            msg.byPongKong = (byte)pppd.bkongValue_[index].kongType;
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.byaTiles = mah;
            msg.bySpecial = 3;
            Network.NetworkMgr.Instance.GameServer.SendSpecialTileReq(msg);
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
        }


        public Animator[] _emotion = new Animator[4];
        public void PlayEmotion(int index, int ID)
        {
            _emotion[index].gameObject.SetActive(true);
            StartCoroutine(DeleatCancel(index, ID));
        }
        IEnumerator DeleatCancel(int index, int ID)
        {
            _emotion[index].SetInteger("Type", ID);
            // Debug.Log("______" + ID);
            yield return new WaitForSeconds(2.1f);
            //  _emotion[index].
            _emotion[index].SetInteger("Type", 0);
            _emotion[index].GetComponent<SpriteRenderer>().sprite = null;
            _emotion[index].gameObject.SetActive(false);
        }
        /// <summary>
        /// 更换牌面数字
        /// </summary>
        /// <param name="spr">要更换的Image组件</param>
        /// <param name="cardNum">要更换成的数字</param>
        public void ChangeCardNum(Image image, byte cardNum, int index = 0)
        {
            MainViewPlayerPlayingPanel pp = UIMainView.Instance.PlayerPlayingPanel;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            Sprite spr = new Sprite();
            switch (cardNum)
            {
                #region 万
                case (byte)CardType.WAN1:
                    spr = pp._wanNum[0];
                    break;
                case (byte)CardType.WAN2:
                    spr = pp._wanNum[1];
                    break;
                case (byte)CardType.WAN3:
                    spr = pp._wanNum[2];
                    break;
                case (byte)CardType.WAN4:
                    spr = pp._wanNum[3];
                    break;
                case (byte)CardType.WAN5:
                    spr = pp._wanNum[4];
                    break;
                case (byte)CardType.WAN6:
                    spr = pp._wanNum[5];
                    break;
                case (byte)CardType.WAN7:
                    spr = pp._wanNum[6];
                    break;
                case (byte)CardType.WAN8:
                    spr = pp._wanNum[7];
                    break;
                case (byte)CardType.WAN9:
                    spr = pp._wanNum[8];
                    break;
                #endregion 万

                #region 条
                case (byte)CardType.TIO1:
                    spr = pp._tioNum[0];
                    break;
                case (byte)CardType.TIO2:
                    spr = pp._tioNum[1];
                    break;
                case (byte)CardType.TIO3:
                    spr = pp._tioNum[2];
                    break;
                case (byte)CardType.TIO4:
                    spr = pp._tioNum[3];
                    break;
                case (byte)CardType.TIO5:
                    spr = pp._tioNum[4];
                    break;
                case (byte)CardType.TIO6:
                    spr = pp._tioNum[5];
                    break;
                case (byte)CardType.TIO7:
                    spr = pp._tioNum[6];
                    break;
                case (byte)CardType.TIO8:
                    spr = pp._tioNum[7];
                    break;
                case (byte)CardType.TIO9:
                    spr = pp._tioNum[8];
                    break;
                #endregion 条

                #region 筒
                case (byte)CardType.TON1:
                    spr = pp._tonNum[0];
                    break;
                case (byte)CardType.TON2:
                    spr = pp._tonNum[1];
                    break;
                case (byte)CardType.TON3:
                    spr = pp._tonNum[2];
                    break;
                case (byte)CardType.TON4:
                    spr = pp._tonNum[3];
                    break;
                case (byte)CardType.TON5:
                    spr = pp._tonNum[4];
                    break;
                case (byte)CardType.TON6:
                    spr = pp._tonNum[5];
                    break;
                case (byte)CardType.TON7:
                    spr = pp._tonNum[6];
                    break;
                case (byte)CardType.TON8:
                    spr = pp._tonNum[7];
                    break;
                case (byte)CardType.TON9:
                    spr = pp._tonNum[8];
                    break;
                #endregion 筒

                #region 东南西北风中发白
                case (byte)CardType.FE:
                    spr = pp._fen[0];
                    break;
                case (byte)CardType.FS:
                    spr = pp._fen[1];
                    break;
                case (byte)CardType.FW:
                    spr = pp._fen[2];
                    break;
                case (byte)CardType.FN:
                    spr = pp._fen[3];
                    break;
                case (byte)CardType.JZ:
                    spr = pp._jiNum[0];
                    break;
                case (byte)CardType.JF:
                    spr = pp._jiNum[1];
                    break;
                case (byte)CardType.JB:
                    spr = pp._jiNum[2];
                    break;
                #endregion 风

                #region 花牌
                case (byte)CardType.HSP:
                    spr = pp._hua[0];
                    break;
                case (byte)CardType.HSU:
                    spr = pp._hua[1];
                    break;
                case (byte)CardType.HAU:
                    spr = pp._hua[2];
                    break;
                case (byte)CardType.HWI:
                    spr = pp._hua[3];
                    break;
                case (byte)CardType.HM:
                    spr = pp._hua[4];
                    break;
                case (byte)CardType.HL:
                    spr = pp._hua[5];
                    break;
                case (byte)CardType.HZ:
                    spr = pp._hua[6];
                    break;
                case (byte)CardType.HJ:
                    spr = pp._hua[7];
                    break;
                #endregion 花牌
                default:
                    break;
            }
            image.sprite = spr;


            //注释报听后是否第一张牌可见
            //if (pppd.playingMethodConf.byDiscardSeeReadHandTile == 0 && cardNum == 255)
            //{
            //    Sprite sprite_back = new Sprite();
            //    if (index == 0 || index == 2)
            //        sprite_back = pp._myAnGangBK;
            //    else
            //        sprite_back = pp._heAnGangBK;
            //    image.transform.parent.GetComponent<Image>().sprite = sprite_back;
            //    image.enabled = false;
            //}
            //else
            //{
            //    //if (image.enabled == false)
            //    //    image.enabled = true;
            //    image.sprite = spr;
            //}
            //return spr;
        }


        #region 点击事件
        [HideInInspector]
        public bool isSeat = true;
        /// <summary>
        /// 点击占座按钮
        /// </summary>
        /// <param name="index"></param>
        public void BtnClickInSeat()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int index = pppd.bySeatNum;

            //if (PlayerSeat[index].transform.GetChild(1).GetComponent<Text>().text.Trim() == "占座" || PlayerSeat[index].transform.GetChild(1).GetComponent<Text>().text.Trim() == "点击占座")
            {
                Debug.LogWarning("点击占座按钮1" + index);
                UIMainView.Instance.ImportantMessagePanel.OnOpen((int)CreatRoomTime, LoatTime,
                    () =>
                    {
                        SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                        Messenger_anhui<int>.Broadcast(MESSAGE_INSEAT, index);
                    });
            }
            //else if (PlayerSeat[index].transform.GetChild(1).GetComponent<Text>().text.Trim() == "取消占座")
            //{

            //}
        }

        public void BtnClickOutSeat()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);

            ZhanzuoPanel.SetActive(true);
        }

        /// <summary>
        /// 占座按钮点击
        /// </summary>
        public void Btn_Zhanzuo_OK()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int index = pppd.bySeatNum;
            Messenger_anhui<int>.Broadcast(MESSAGE_OUTSEAT, index);
            ZhanzuoPanel.SetActive(false);
        }

        /// <summary>
        /// 关闭占座面板
        /// </summary>
        public void Btn_Zhanzuo_Canle()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            ZhanzuoPanel.SetActive(false);
        }

        /// <summary>
        /// 玩家准备按钮
        /// </summary>
        public void BtnPlayReady()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_PLAYRREADY);
        }

        /// <summary>
        /// 处理点击玩家头像按钮
        /// </summary>
        /// <param name="go"></param>
        public void BtnPlayerAvator(GameObject go)
        {
            int index = 1;
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui<int>.Broadcast(MESSAGE_BTNPLAYERAVATOR, index);
        }
        public Slider[] _slider = new Slider[3];
        public void BtnSettingPanelShow()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            _slider[0].value = MahjongCommonMethod.Instance.MusicVolume * 0.01f;
            _slider[1].value = MahjongCommonMethod.Instance.EffectValume * 0.01f;
            _slider[2].value = MahjongCommonMethod.Instance.LastVoiceValume * 0.01f;
        }


        /// <summary>
        /// 点击复制房间号码
        /// </summary>
        public void BtnCopyRoomId()
        {
            //  MahjongLobby_AH.SDKManager.Instance.HandleShareImage(MahjongLobby_AH.LobbyContants.DownLoadQRcode, 0, 1);

            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_COPYROOMID);
        }

        /// <summary>
        /// 点击邀请好友消息
        /// </summary>
        public void BtnShareWx()
        {
            Messenger_anhui.Broadcast(MESSAGE_SHAREWX);
        }

        /// <summary>
        /// 点击房间规则按钮
        /// </summary>
        public void BtnRoomRule()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_ROOMRULE);
        }

        public GameObject _gSettingGroup;
        public GameObject _closeBtn2;
        /// <summary>
        /// 点击系统设置按钮
        /// </summary>
        public void BtnSystemSettingGroupClick(GameObject setBtn)
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            byte state = GameData.Instance.PlayerPlayingPanelData.isChoicTir;
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;
            //  Debug.LogWarning("-------" + ppd.isShowSettingGroup);
            if (ppd.isShowSettingGroup == true)
            {
                setBtn.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).OnComplete(() =>
                {
                    setBtn.transform.localEulerAngles = new Vector3(0, 0, 0);
                });

                _gSettingGroup.transform.GetComponent<DOTweenAnimation>().GetTweens()[1].Restart();
                //   .OnComplete(() =>
                //   {
                //     ppd.isShowSettingGroup = false;
                //     _closeBtn2.SetActive(false);
                //_gSettingGroup.transform.localPosition = new Vector3(205, _gSettingGroup.transform.localPosition.y, 0);
                //  });

            }

            else
            {
                setBtn.transform.DOLocalRotate(new Vector3(0, 0, 270), 0.3f).OnComplete(() =>
                {
                    setBtn.transform.localEulerAngles = new Vector3(0, 0, 270);
                });
                ppd.isShowSettingGroup = true;
                _gSettingGroup.transform.GetComponent<DOTweenAnimation>().GetTweens()[0].Restart();
                //  .OnComplete(() =>
                //  {
                //      _closeBtn2.SetActive(true);
                //_gSettingGroup.transform.localPosition = new Vector3(205, _gSettingGroup.transform.localPosition.y, 0);
                //  });

                DealThirteenState(state);
            }
        }
        public void OnDoTweenComplete0()
        {
            _closeBtn2.SetActive(true);
            // _gSettingGroup.transform.localPosition = new Vector3(932, _gSettingGroup.transform.localPosition.y, 0);
        }
        public void OnDoTweenComplete1()
        {
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;

            ppd.isShowSettingGroup = false;
            _closeBtn2.SetActive(false);
            // _gSettingGroup.transform.localPosition = new Vector3(932, _gSettingGroup.transform.localPosition.y, 0);
        }

        /// <summary>
        /// 处理十三幺状态
        /// </summary>
        /// <param name="state">0-不做牌，1-做牌</param>
        public void DealThirteenState(byte state = 0)
        {

            byte num = GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinSpecialThirteenOrphans;

            RectTransform rect = _gSettingGroup.GetComponent<RectTransform>();
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (num == 0 || pppd.playingMethodConf.byWinSpecialThirteenOrphansCr == 0)//大厅没有选择十三幺
            {
                // rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 254);
                rect.transform.GetChild(3).gameObject.SetActive(false);
                rect.transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (num == 1)//大厅选择十三幺
            {
                // rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 315);
                if (state == 0)
                {
                    rect.transform.GetChild(3).gameObject.SetActive(true);
                    rect.transform.GetChild(4).gameObject.SetActive(false);
                }
                if (state == 1)
                {
                    rect.transform.GetChild(3).gameObject.SetActive(false);
                    rect.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
        }
        /// <summary>
        /// 选择成十三幺
        /// </summary>
        public void BtnChoiceTir()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (pppd.iTirState == 0 || pppd.iTirState == 2)
            {
                if (PPPD.iMethodId != 13)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show(PlayerPlayingPanelData.szThirteenChoiceDis, ThirteenOk, null, true);
                }
                else
                {
                    UIMgr.GetInstance().GetUIMessageView().Show(PlayerPlayingPanelData.szThirteenChoiceDis_LC, ThirteenOk, null, true);
                }

            }
            else if (pppd.iTirState == 1)
            {
                UIMgr.GetInstance().GetUIMessageView().Show(PlayerPlayingPanelData.szThirteenUnableChoice);
            }
        }
        void ThirteenOk()
        {
            Messenger_anhui.Broadcast(MESSAGE_CHOICETHIRTEEN);
        }

        /// <summary>
        /// 取消成十三幺
        /// </summary>
        public void BtnCancalTir()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (pppd.iTirState == 0)
            {
                Messenger_anhui.Broadcast(MESSAGE_CANCALTHIRTEEN);
            }
            else if (pppd.iTirState == 2)
            {
                UIMgr.GetInstance().GetUIMessageView().Show(PlayerPlayingPanelData.szThirteenUnableCancal);
            }
        }
        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void BtnReturnLobby()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_RETURN);
        }

        /// <summary>
        /// 点击解散房间按钮
        /// </summary>
        public void BtnDissolveRoom()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            string str = TextConstant.ROOMONWER_DISSOLVEROOM;
            //是不是预约房
            if (LoatTime > 0)
            {
                TimeSpan time3_1 = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime)).Subtract(MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).Duration();
                if (time3_1.Seconds < 0 || m_gameStart)
                {
                    //弹出二次确认框
                    if (pppd.usersInfo.Count == 1)
                    {
                        str = TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT;
                    }
                    else if (pppd.usersInfo.Count > 1)
                    {
                        str = TextConstant.ROOMONWER_DISSOLVEROOM_;
                    }
                    UIMgr.GetInstance().GetUIMessageView().Show(str, Ok, null);
                }
                else
                {
                    UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT, Ok, UIMessageView.ButtonType.BT_OK_CANCEL);
                }
            }
            else
            {
                //弹出二次确认框
                if (pppd.usersInfo.Count == 1)
                {
                    MahjongCommonMethod.Instance.GameLaterDo_ =
                        () =>
                        {
                            MahjongLobby_AH.UIMgr.GetInstance().GetUIMessageView().Show(MahjongGame_AH.TextConstant.ROOMONWER_DISSOLVEROOM_NOSTART);
                        };
                    Ok();
                }
                else if (pppd.usersInfo.Count > 1)
                {
                    UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_, Ok, null);
                }
            }

            PlayerPrefs.SetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo, "0000");
            PlayerPrefs.SetInt("GameActivie", 0);
        }

        void Ok()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_DISSOLVEROOM);
        }

        /// <summary>
        /// 点击语音按钮
        /// </summary>
        public void BtnVoice()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_BTNVOICE);
        }
        /// <summary>
        /// 修改音乐
        /// </summary>
        public void OnMusicValueChanged(Slider sd)
        {
            Messenger_anhui<float>.Broadcast(MESSAGE_MUSICVALUE, sd.value);
        }
        /// <summary>
        /// 修改音效音量
        /// </summary>
        /// <param name="sd"></param>
        public void OnEffectValueChanged(Slider sd)
        {
            Messenger_anhui<float>.Broadcast(MESSAGE_EFFECVALUE, sd.value);
        }
        /// <summary>
        /// 修改语音音量
        /// </summary>
        /// <param name="sd"></param>
        public void OnVoiceValueChanged(Slider sd)
        {
            Messenger_anhui<float>.Broadcast(MESSAGE_VOICEVALUE, sd.value);
        }
        /// <summary>
        /// 音乐静音
        /// </summary>
        public void BtnIsMusicMute(Slider obj1)
        {

            if (_btnFlagM.GetChild(0).gameObject.activeInHierarchy)
            {//开启
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                obj1.enabled = true;
                _btnFlagM.GetChild(0).gameObject.SetActive(false);
                _btnFlagM.GetChild(1).gameObject.SetActive(true);
                obj1.value = MahjongCommonMethod.Instance.LastMusicVolume * 0.01f;
            }
            else
            {//静音
                _btnFlagM.GetChild(0).gameObject.SetActive(true);
                _btnFlagM.GetChild(1).gameObject.SetActive(false);
                MahjongCommonMethod.Instance.LastMusicVolume = (int)(obj1.value * 100);
                obj1.value = 0;
                obj1.enabled = false;
            }
            Messenger_anhui.Broadcast(MESSAGE_MUSICCLICK);
        }
        /// <summary>
        /// 音效静音
        /// </summary>
        public void BtnIsEffecMute(Slider obj1)
        {
            if (_btnFlagE.GetChild(0).gameObject.activeInHierarchy)
            {//开启
                obj1.enabled = true;
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                _btnFlagE.GetChild(0).gameObject.SetActive(false);
                _btnFlagE.GetChild(1).gameObject.SetActive(true);
                obj1.value = MahjongCommonMethod.Instance.LastEffectValume * 0.01f;
            }//静音
            else
            {
                _btnFlagE.GetChild(0).gameObject.SetActive(true);
                _btnFlagE.GetChild(1).gameObject.SetActive(false);
                MahjongCommonMethod.Instance.LastEffectValume = (int)(obj1.value * 100);
                obj1.value = 0;
                obj1.enabled = false;
            }

            Messenger_anhui.Broadcast(MESSAGE_EFFECCLICK);
        }
        /// <summary>
        /// 语音静音
        /// </summary>
        public void BtnIsVoiceMute(Slider obj1)
        {
            if (_btnFlagV.GetChild(0).gameObject.activeInHierarchy)
            {//开启
                obj1.enabled = true;
                _btnFlagV.GetChild(0).gameObject.SetActive(false);
                _btnFlagV.GetChild(1).gameObject.SetActive(true);
                obj1.value = 1;
                MahjongCommonMethod.Instance.isOpenVoicePlay = true;
            }
            else
            {//静音
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                _btnFlagV.GetChild(0).gameObject.SetActive(true);
                _btnFlagV.GetChild(1).gameObject.SetActive(false);
                MahjongCommonMethod.Instance.LastVoiceValume = (int)(obj1.value * 100);
                obj1.value = 0;
                MahjongCommonMethod.Instance.isOpenVoicePlay = false;
                obj1.enabled = false;
            }
            //Messenger.Broadcast(MESSAGE_VOICECLICK);
        }

        /// <summary>
        /// 播放点击音
        /// </summary>
        public void BtnPlayClickVoice()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
        }
        public void BtnOpenShortTalk()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MainViewShortTalkPanel.MessageOpenChatPenal);
        }

        /// <summary>
        /// 防作弊按钮
        /// </summary>
        public void BtnAntiCheating()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_BTNANTICHEATING);
        }

        /// <summary>
        /// 按下听牌的按钮
        /// </summary>
        public void BtnTingCardDown()
        {
            Messenger_anhui.Broadcast(MESSAGE_BTNTING_DOWN);
        }

        /// <summary>
        /// 抬起听牌的按钮
        /// </summary>
        public void BtnTingCardUp()
        {
            Messenger_anhui.Broadcast(MESSAGE_BTNTING_UP);
        }

        /// <summary>
        /// 取消托管按钮
        /// </summary>
        public void BtnCanelHosting()
        {
            NetMsg.ClientCancleAutoStatusReq msg = new NetMsg.ClientCancleAutoStatusReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iSeatNum = (byte)GameData.Instance.PlayerPlayingPanelData.bySeatNum;
            Network.NetworkMgr.Instance.GameServer.SendClientCancleAutoStatusReq(msg);
        }
        #endregion

        //初始化面板数据
        public void InitPanel()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //清空面板数据
            pppd.isGameEnd = false;
            pppd.FirstDealCard = 0;
            pppd.isPanelShow_Playing = true;
            pppd.isPanelShow_Wating = false;
            MahjongHelper.Instance.iFanThirteenIndepend = 0;
            MahjongHelper.Instance.iFanWin = 0;
            pppd.IsReady = false;
            pppd.isBanker = false;
            pppd.isFirstDealCard = true;
            pppd.iSpwanCardNum = 0;
            pppd.iSpecialCardNum = 0;
            pppd.iTirState = 0;
            pppd.isChoicTir = 0;
            pppd.isGangToPeng_Later = false;
            pppd.isGangToPeng = false;
            pppd.isLastPongOrKong = false;
            pppd.isChoiceTing = 0;
            pppd.isChoiceTing_ALL = new byte[4];
            pppd.iDissolveFlag = 0;
            pppd.PlayerFlowerCount = new int[] { 0, 0, 0, 0 };
            if (pppd.playingMethodConf.byCreateModeHaveFlower > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    UIMainView.Instance.PlayerPlayingPanel.UpdateShowFlowerCount(2, i + 1, pppd.PlayerFlowerCount[i], 1);
                }
            }

            pppd.DismissRoomRes = new NetMsg.ClientDismissRoomRes();
            for (int i = 0; i < 4; i++)
            {
                UIMainView.Instance.PlayerPlayingPanel.TingStatus.transform.GetChild(i).gameObject.SetActive(false);
            }
            pppd.isAlreadyChangeStatus = false;
            pppd.isNeedSendPassMessage = false;
            TingShow.SetActive(false);
            //清空list中的数据
            for (int i = 0; i < 4; i++)
            {
                pppd.usersCardsInfo[i].inm = 0;
                pppd.usersCardsInfo[i].bySeatNum = 0;
                pppd.usersCardsInfo[i].listCurrentCards.Clear();
                pppd.usersCardsInfo[i].listShowCards.Clear();
                pppd.usersCardsInfo[i].listSpecialCards.Clear();
            }
            pppd.isCanHandCard = false;
            pppd.bLastCard = 0;
            //if (pppd.playingMethodConf.byCreateModeHaveFlower > 0)
            //{
            //    pppd.LeftCardCount = 144;
            //}
            //else
            //{
            //    pppd.LeftCardCount = 136;
            //}

            //如果去除字牌玩家手牌是108张            
            //if (pppd.playingMethodConf.byCreateModeHaveWind == 0)
            //{
            //    pppd.LeftCardCount -= 16;
            //}

            ////处理玩家手中的箭牌
            //if (pppd.playingMethodConf.byCreateModeHaveDragon == 0)
            //{
            //    pppd.LeftCardCount -= 12;
            //}

            //清空玩家听牌信息
            MahjongHelper.Instance.TingCount = new List<MahjongHelper.TingMessage>();
            btn_tingpai.gameObject.SetActive(false);

            pppd.byLastCard = 0;
            pppd.iGangNum = 0;
            //删除之前生成的麻将
            for (int i = 0; i < _Cards.Length; i++)
            {
                //删除手牌
                Mahjong[] handCard = _Cards[i].Find("currentGroup").GetComponentsInChildren<Mahjong>(false);
                for (int j = 0; j < handCard.Length; j++)
                {
                    DestroyImmediate(handCard[j].gameObject);
                }

                //删除赢的手牌
                if (i != 0)
                {
                    Mahjong[] winCard = _Cards[i].Find("winCard").GetComponentsInChildren<Mahjong>();
                    for (int j = 0; j < winCard.Length; j++)
                    {
                        Destroy(winCard[j].gameObject);
                    }
                }


                //删除赢得手牌
                if (i != 0)
                {
                    Mahjong[] specialHand = _Cards[i].Find("currentSpecialGroup").GetComponentsInChildren<Mahjong>();
                    for (int j = 0; j < specialHand.Length; j++)
                    {
                        Destroy(specialHand[j].gameObject);
                    }
                }

                Mahjong[] tableCard = _Cards[i].Find("showGroup_d").GetComponentsInChildren<Mahjong>();

                for (int j = 0; j < tableCard.Length; j++)
                {
                    Destroy(tableCard[j].gameObject);
                }

                Mahjong[] tableCard_u = _Cards[i].Find("showGroup_u").GetComponentsInChildren<Mahjong>();

                for (int j = 0; j < tableCard_u.Length; j++)
                {
                    Destroy(tableCard_u[j].gameObject);
                }

                Mahjong[] tableCard_3 = _Cards[i].Find("showGroup_3").GetComponentsInChildren<Mahjong>();

                for (int j = 0; j < tableCard_3.Length; j++)
                {
                    Destroy(tableCard_3[j].gameObject);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                HeadIamge[i].gameObject.SetActive(false);
                _TimeRemi.GetChild(0).GetChild(i).gameObject.SetActive(false);
                pppd.DisConnectStatus[i] = 0;
            }

            //关闭托管按钮
            if (GameData.Instance.PlayerPlayingPanelData.iPlayerHostStatus == 0)
            {
                UIMainView.Instance.PlayerPlayingPanel.ShowCanelHosting(false);
            }

            //清空对象池
            PoolManager.Clear();
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
            UpdateShow();
        }

        /// <summary>
        /// 胡牌之后的动画
        /// </summary>
        public void WinAction(int iseatNum)
        {
            Debug.LogWarning("胡牌之后：");
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            int index = pppd.GetOtherPlayerShowPos(iseatNum + 1) - 1;
            Vector3 initPos = Win_MahjongFirstPos.transform.localPosition;
            //int isinit_2 = 0;  //是否初始化玩家四的位置  碰牌的            
            string name = "";
            int count = 0;
            if (index == 0)
            {
                name = "currentDCardPre";
            }
            else if (index == 1 || index == 3)
            {
                name = "currentHCardPre";
            }
            else if (index == 2)
            {
                name = "currentUCardPre";
            }



            Mahjong[] handCard = _Cards[index].Find("currentGroup").GetComponentsInChildren<Mahjong>();
            //只删除之前的牌的信息
            for (int i = 0; i < handCard.Length; i++)
            {
                if (handCard[i] != null && handCard[i].name.Contains(name))
                {
                    Destroy(handCard[i].gameObject);
                }
            }

            //保存产生的麻将
            List<byte> mahjong_value = new List<byte>();
            for (int i = 0; i < 14; i++)
            {
                if (grpd.bHandleTiles[iseatNum, i] != 0)
                {
                    mahjong_value.Add(grpd.bHandleTiles[iseatNum, i]);
                }
            }
            //排序
            mahjong_value.Sort(iCompareList);
            //添加最后一张牌,不参与排序
            if (grpd.byShootSeat == 0)
            {
                for (int i = 0; i < mahjong_value.Count; i++)
                {
                    if (mahjong_value[i] == grpd.resultType[iseatNum].lastTile.bySuit * 16 + grpd.resultType[iseatNum].lastTile.byValue)
                    {
                        mahjong_value.RemoveAt(i);
                        break;
                    }
                }
                mahjong_value.Add(0);
                mahjong_value.Add((byte)(grpd.resultType[iseatNum].lastTile.bySuit * 16 + grpd.resultType[iseatNum].lastTile.byValue));
            }
            else
            {
                mahjong_value.Add(0);
                mahjong_value.Add((byte)(grpd.resultType[iseatNum].lastTile.bySuit * 16 + grpd.resultType[iseatNum].lastTile.byValue));
            }
            //获取玩家产生第一张牌的位置
            if (index == 0)
            {
                //处理自己的牌
                if (pppd.usersCardsInfo[0].listSpecialCards.Count > 0)
                {
                    if (pppd.usersCardsInfo[0].listSpecialCards[0].type == 5 || pppd.usersCardsInfo[0].listSpecialCards[0].type == 6)
                    {
                        initPos += new Vector3(fSpecialCard_Thirteen_Interval * (pppd.iSpecialCardNum - 1), 0, 0);
                    }
                    else
                    {
                        initPos += new Vector3(fSpecialCardInterval * (pppd.iSpecialCardNum - 1), 0, 0);
                    }
                }
            }

            if (index == 0)
            {
                for (int i = 0; i < mahjong_value.Count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Game/Ma/showVCardPre"));
                    go.transform.SetParent(_Cards[index].Find("currentGroup").transform);
                    go.transform.localScale = Vector3.one * 1.8f;

                    go.transform.localPosition = new Vector3(initPos.x + 76f * count, initPos.y, 0);
                    if (i == mahjong_value.Count - 1)
                    {
                        go.transform.localPosition -= new Vector3(50, 0, 0);
                    }
                    //如果等于0，隐藏
                    if (mahjong_value[i] == 0)
                    {
                        go.transform.Find("Image").gameObject.SetActive(false);
                    }
                    else
                    {
                        ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), mahjong_value[i], index);
                    }
                    count++;
                }
            }
            else if (index == 2)
            {
                for (int i = 0; i < mahjong_value.Count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Game/Ma/showVCardPre"));
                    go.transform.SetParent(_Cards[index].Find("winCard").transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.GetComponent<LayoutElement>().preferredWidth = 38f;
                    if (mahjong_value[i] == 0)
                    {
                        go.transform.Find("Image").gameObject.SetActive(false);
                        go.transform.GetComponent<LayoutElement>().preferredWidth = 10;
                    }
                    else
                    {
                        ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), mahjong_value[i], index);
                        //改变麻将里面的麻将旋转角度
                        go.transform.Find("Image/num").GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, 180f);
                    }
                    go.transform.SetAsFirstSibling();
                    //go.transform.GetChild(0).transform.localPosition += new Vector3(0, -10, 0);
                }

                //修改

            }
            else if (index == 1 || index == 3)
            {
                for (int i = 0; i < mahjong_value.Count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Game/Ma/WinHCard"));
                    go.transform.SetParent(_Cards[index].Find("winCard").transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    if (mahjong_value[i] == 0)
                    {
                        go.transform.Find("Image").gameObject.SetActive(false);
                        if (index == 3)
                        {
                            go.transform.GetComponent<LayoutElement>().preferredHeight = 8;
                        }
                        else
                        {
                            go.transform.GetComponent<LayoutElement>().preferredHeight = 5;
                        }

                    }
                    else
                    {
                        ChangeCardNum(go.transform.Find("Image/Num").GetComponent<Image>(), mahjong_value[i], index);
                        if (index == 1)
                        {
                            go.transform.Find("Image/Num").GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, 90);
                        }
                        else
                        {
                            go.transform.Find("Image/Num").GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, -90);
                        }
                    }

                    if (index == 1)
                    {
                        go.transform.SetAsFirstSibling();
                    }

                    //if (index == 3)
                    //{
                    //    go.transform.GetChild(0).transform.localPosition += new Vector3(-20f, 0, 0);
                    //}
                }
                //处理扎妈
                if (true)
                {
                    Debug.LogWarning("扎妈=============");
                }
                //if (index == 3 && mahjong_value.Count == 9)
                //{
                //    isinit_2 = 1;
                //    _Cards[index].FindChild("currentGroup").transform.localPosition -= new Vector3(0f, 20f, 0f);
                //}

                //if (index == 3 && mahjong_value.Count == 6)
                //{
                //    isinit_2 = 2;
                //    _Cards[index].FindChild("currentGroup").transform.localPosition -= new Vector3(0f, 36f, 0f);
                //}

            }
            StartCoroutine(DelayShowResult(4f));
        }
        //手牌排序规则
        int iCompareList(byte b, byte a)
        {
            int res = 0;
            //获取麻将牌的花色和大小
            int a_0 = a / 16;
            int a_1 = a % 16;
            int b_0 = b / 16;
            int b_1 = b % 16;
            if (a_0 < b_0)
            {
                res = 1;
            }
            else if (a_0 == b_0)
            {
                if (a_1 <= b_1)
                {
                    res = 1;
                }
                else
                {
                    res = -1;
                }
            }
            else
            {
                res = -1;
            }
            return res;
        }

        /// <summary>
        /// 延迟显示结算界面
        /// </summary>
        public void DelayShowResule_(float timer)
        {
            StartCoroutine(DelayShowResult(timer));
        }

        /// <summary>
        /// 延迟显示结算界面
        /// </summary>
        IEnumerator DelayShowResult(float timer)
        {
            yield return new WaitForSeconds(timer);
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            //更新面板的显示数据
            grpd.isPanelShow = true;
            grpd.isShowRoundGameResult = true;
            SystemMgr.Instance.GameResultSystem.UpdateShow();
            UIMainView.Instance.GameResultPanel.SpwanGameReult_Round();
        }


        /// <summary>
        /// 延迟显示特效
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="index"></param>
        /// <param name="type_"></param>
        public void DelaySpwanSpeaiclTypeRemind(float timer, int index, int type_)
        {
            // Debug.Log("对句结束timer： " + timer + "type_" + type_);
            StartCoroutine(DelaySpwanSpeaiclTypeRemind_(timer, index, type_));
        }

        IEnumerator DelaySpwanSpeaiclTypeRemind_(float timer, int index, int type_)
        {

            // Debug.LogWarning("对句结束timer： " + timer + "type_" + type_);
            yield return new WaitForSeconds(timer);
            _gHongzhongZhongma.SetActive(false);
            SpwanSpeaiclTypeRemind(index, type_);
        }

        [HideInInspector]
        public GameObject EffectBuhua;
        /// <summary>
        /// 产生特殊牌型的提示
        /// </summary>
        /// <param name="index">0--3数组下标4表示放在屏幕正中央</param>
        /// <param name="isShowWinPanel"></param>
        /// <param name="type">1吃2碰3杠4胡5十三幺吃6十三幺抢7听8放炮9荒庄10自摸  11开局信息，12对局结束,13前抬  14后和  15放炮 16补花</param>
        public void SpwanSpeaiclTypeRemind(int index, int type_, bool isShowWinPanel = false)
        {
           // Debug.LogError("杠杠杠杠杠杠杠杠杠杠杠杠"+ type_);

            string path = "";
            switch (type_)
            {
                case 1:
                case 5:
                    path = "Game/Effect/Chi";
                    break;
                case 2:
                    path = "Game/Effect/Peng";
                    break;
                case 3:
                    path = "Game/Effect/Gang";
                    break;
                case 4:
                    path = "Game/Effect/Hu";
                    break;
                case 7:
                    path = "Game/Effect/Ting";
                    break;
                case 6:
                    path = "Game/Effect/qiang";
                    break;
                case 9:
                    path = "Game/Effect/huangzhuang";
                    break;
                case 10:
                    path = "Game/Effect/Zimo";
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    path = "Game/Effect/qiantai";
                    break;
                case 15:
                    path = "Game/Effect/fangpao";
                    break;
                case 16:
                    path = "Game/Effect/buhua";
                    break;
                //抢杠胡
                case 17:
                //补杠
                case 18:
                    path = "Game/Effect/RobbingWin";
                    break;
            }

            if (type_ == 16 && EffectBuhua != null)
            {
                DestroyImmediate(EffectBuhua);
            }

            //产生胡牌的图标
            GameObject go = Instantiate(Resources.Load<GameObject>(path));
            go.transform.SetParent(UIMainView.Instance.PlayerPlayingPanel.transform.Find("Common"));

            //如果补花动画特殊处理
            if (type_ == 16)
            {
                EffectBuhua = go;
            }

            if (index == 4)
            {
                //决定播放哪个动画
                if (type_ == 11)
                {
                    go.transform.localPosition = new Vector3(20f, 0f, 0f);
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_mgt";
                }
                else if (type_ == 12)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_hfj";
                }
                else if (type_ == 13)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_fcy";
                }
                else if (type_ == 14)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_kb";
                }
                else
                {
                    go.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                //根据类型不同，取不同位置，自摸胡特殊处理
                if (type_ == 10)
                {
                    go.transform.localPosition = specialPos_zimo[index].transform.localPosition;
                }
                else if (type_ == 4)
                {
                    go.transform.localPosition = specialPos_hu[index].transform.localPosition;
                }
                //荒庄
                else if (type_ == 10)
                {
                    go.transform.localPosition = Vector3.zero;
                }
                //前台后和
                else if (type_ == 14 || type_ == 13)
                {
                    go.transform.localPosition = Vector3.zero;
                }
                //听
                else if (type_ == 7)
                {
                    go.transform.localPosition = specialPos[index].transform.localPosition;
                }
                //吃
                else if (type_ == 5)
                {
                    go.transform.localPosition = specialPos_Chi[index].transform.localPosition;
                }
                //放炮
                else if (type_ == 15)
                {
                    go.transform.localPosition = specialPos_fangpao[index].transform.localPosition;
                }
                else if (type_ == 16)
                {
                    go.transform.localPosition = specialPos_buhua[index].transform.localPosition;
                }
                //抢杠胡
                else if (type_ == 17)
                {
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_hfj";
                    go.transform.localPosition = speaiclBuGangPos[index].transform.localPosition;
                }
                //补杠
                else if (type_ == 18)
                {
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_kb";
                    go.transform.localPosition = speaiclBuGangPos[index].transform.localPosition;
                }
                else
                {
                    go.transform.localPosition = specialPos_effect[index].transform.localPosition;
                }
            }

            if (type_ == 9)
            {
                go.transform.localScale = Vector3.one * 85;
            }
            else
            {
                go.transform.localScale = Vector3.one * 100;
            }

            go.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 显示玩家输赢之后玩家头像的标志
        /// </summary>
        /// <param name="iseatNum">玩家座位号  1----5</param>
        /// <param name="type">输赢类型，0表示点炮，1表示胡牌，2表示自摸</param>
        public void ShowPlayerCardMessage(int iseatNum, int type)
        {
            //Debug.LogError("iseatNum:" + iseatNum + ",type:" + type);
            //数组下标0---3
            int index = PPPD.GetOtherPlayerShowPos(iseatNum) - 1;

            HeadIamge[index].gameObject.SetActive(true);
            //更换输赢的详细图片
            HeadIamge[index].sprite = LastRemind[type];
        }


        /// <summary>
        /// 及时分数更新
        /// </summary>
        /// <param name="index"></param>
        /// <param name="score"></param>
        public void SpwanSpeaiclScoreRemind(int index, int score)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/GameResult/SpecialTypeScoreNotice"));
            go.transform.SetParent(specialScorePos[index].transform);
            go.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            go.transform.GetComponent<CanvasGroup>().alpha = 1;
            SpecialTypeScoreNotice notice = go.GetComponent<SpecialTypeScoreNotice>();
            notice.GetValue(score);
        }

        [DllImport("__Internal")]
        private static extern double _CalcDistanceBetweenCoor(double l1, double l2, double l3, double l4);

        public void BtnPlayerHeadImage(GameObject Go)
        {
            int iseatNum = 0;  //点击头像的玩家座位号
            int Num = System.Convert.ToInt32(Go.name.Split('_')[1]);
            iseatNum = Num - 1 + PPPD.bySeatNum;
            if (iseatNum > 4)
            {
                iseatNum -= 4;
            }
            if (iseatNum < 1 || iseatNum > 4)
            {
                Debug.LogError("玩家座位号错误，请检查");
                return;
            }

            GameObject go = Instantiate(Resources.Load<GameObject>("Game/UserInfoPanel"));
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.GetComponent<Canvas>().overrideSorting = true;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            MahjongLobby_AH.SDKManager.Instance.userInfoLast = go;

            UserInfo info = go.GetComponent<UserInfo>();
            if (PPPD.usersInfo[iseatNum].fLongitude == 0 || PPPD.usersInfo[iseatNum].fLatitude == 0)
            {
                info.sPosMessage = "未获得";
                info.fDistance = 0;
                info.GetUserMessage(PPPD.usersInfo[iseatNum], 2);
                return;
            }
            else
            {
                if (PPPD.bySeatNum == iseatNum)
                {
                    info.sPosMessage = MahjongCommonMethod.Instance.sPlayerAddress;
                }
                else
                {
                    info.sPosMessage = PPPD.usersInfo[iseatNum].szAddress;
                }

#if UNITY_ANDROID
                if (PPPD.bySeatNum == iseatNum)
                {
                    info.fDistance = 0;
                }
                else
                {
                    if (!Application.isEditor)
                    {
                        AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                        AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                        //调用安卓的测距方法
                        info.fDistance = jo.Call<float>("GetDistance", PPPD.usersInfo[PPPD.bySeatNum].fLongitude, PPPD.usersInfo[PPPD.bySeatNum].fLatitude, PPPD.usersInfo[iseatNum].fLongitude, PPPD.usersInfo[iseatNum].fLatitude);
                    }
                    else
                    {
                        info.fDistance = 0;
                    }

                }
#elif UNITY_IPHONE || UNITY_IOS
                if (PPPD.bySeatNum == iseatNum)
                {
                    info.fDistance = 0;
                }
                else
                {                    
                    //调用ios的测距方法
                    info.fDistance = (float)_CalcDistanceBetweenCoor(PPPD.usersInfo[PPPD.bySeatNum].fLongitude, PPPD.usersInfo[PPPD.bySeatNum].fLatitude, PPPD.usersInfo[iseatNum].fLongitude, PPPD.usersInfo[iseatNum].fLatitude);
                }                
#endif
            }
            info.GetUserMessage(PPPD.usersInfo[iseatNum], 2);
        }


        /// <summary>
        /// 更新显示玩家的可以听的牌
        /// </summary>
        /// <param name="mahjong"></param>
        public void UpdateTingCard(byte[] mahjong)
        {
            Debug.LogWarning("更新显示玩家的可以听的牌" + mahjong.Length);
            if (mahjong.Length <= 0)
            {
                return;
            }

            Mahjong[] mah = MahjongManger.Instance.GetSelfCard();//MahjongManger.Instance.GetSelfCard();

            for (int i = 0; i < mah.Length; i++)
            {
                mah[i].transform.Find("Ting").gameObject.SetActive(false);
            }

            for (int i = 0; i < mahjong.Length; i++)
            {
                for (int j = 0; j < mah.Length; j++)
                {
                    if (mah[j].GetComponent<Mahjong>().bMahjongValue == mahjong[i] && MahjongHelper.Instance.mahjongTing.ContainsKey(mahjong[i]) && MahjongHelper.Instance.mahjongTing[mahjong[i]].Length > 0)
                    {
                        // Debug.LogWarning ("可以听的牌：" + mahjong[i]);
                        mah[j].transform.Find("Ting").gameObject.SetActive(true);
                    }
                }
            }

            //如果需要报听的玩法，则显示听牌的信息
            if (PPPD.playingMethodConf.byWinLimitReadHand > 0 && PPPD.isChoiceTing == 0)
            {
                MahjongHelper.Instance.specialValue_[6] = 1;
                MahjongHelper.Instance.specialValue_[7] = 1;
                GameData.Instance.PlayerPlayingPanelData.isSendPlayerPass = false;
                GameData.Instance.PlayerPlayingPanelData.isNeedSendPassMessage = false;
                ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
            }
        }
        void CloseTingShow(GameObject obj)
        {
            TingShow.gameObject.SetActive(false );
           // MahjongManger.Instance.HideTingLogo();
        }
        /// <summary>
        /// 展示显示可胡的牌
        /// </summary>
        /// <param name="mah"></param>
        public void SpwanTingShow(MahjongHelper.TingMessage[] mah1)
        {
            //Debug.LogError("SpwanTingShow" + mah1.Length);
            if (mah1.Length <= 0)
            {
                TingShow.gameObject.SetActive(false);
                return;
            }
            MahjongHelper.Instance.ByteListTing.Clear();
            for (int i = 0; i < mah1.Length ; i++)
            {
                MahjongHelper.Instance.ByteListTing.Add(mah1[i].value);
            }
            MahjongHelper.Instance.ByteListTing.Sort(MahjongHelper.Instance.sortbyteTingCardslist);
            byte[] mah = MahjongHelper.Instance.ByteListTing.ToArray();
            TingShow.gameObject.SetActive(true);
            EventTriggerListener .Get( TingShow.gameObject ).onClick =CloseTingShow ;

            if (TingShow.transform.childCount > 1)
            {
                for (int i = 1; i < TingShow.transform.childCount; i++)
                {
                    Destroy(TingShow.transform.GetChild(i).gameObject);
                }
            }

            for (int i = mah.Length - 1; i >= 0; i--)
            {
                //if (TingShow.transform.GetChild(i))
                //{

                //}
                GameObject ma = Instantiate(Resources.Load<GameObject>("Game/Ting/Mahjong"));
                ma.transform.SetParent(TingShow.transform);
                ma.transform.name = "Mahjong";
                ma.transform.localScale = Vector3.one;
                ma.transform.localPosition = new Vector3(ma.transform.localPosition.x, ma.transform.localPosition.y, 0);
                ma.transform.localEulerAngles = Vector3.zero;
                //更新图片
                ChangeCardNum(ma.transform.Find("Mahjong/Image").GetComponent<Image>(), mah[i]);

                int count = UpdateLeftCardCount(mah[i]);
                Debug.LogWarning("---这种牌还剩余多少张：" + count + "," + mah[i]);
                Transform trans_1 = ma.transform.Find("Mahjong").GetChild(1);
                Transform trans_2 = ma.transform.Find("Mahjong").GetChild(2);

                if (count == 0)
                {
                    //使牌变灰
                    ma.transform.Find("Mahjong").GetComponent<Image>().color = new Color(0.62f, 0.62f, 0.62f, 1);
                    trans_1.gameObject.SetActive(true);
                    trans_2.gameObject.SetActive(false);
                    //更新该牌还有多少张
                    trans_1.GetChild(0).GetComponent<Text>().text = UpdateLeftCardCount(mah[i]).ToString();
                }
                else
                {
                    trans_1.gameObject.SetActive(false);
                    trans_2.gameObject.SetActive(true);
                    //更新该牌还有多少张
                    trans_2.GetChild(0).GetComponent<Text>().text = UpdateLeftCardCount(mah[i]).ToString();
                }
                #region 处理胡牌倍数
                //处理胡牌倍数
                //int iMultiple = mah[i].count;  //倍数，默认为1               

                //if (iMultiple <= 1)
                //{
                //    iMultiple = 1;
                //}

                //更新倍数
                //ma.transform.GetChild(1).GetComponent<Text>().text = iMultiple.ToString();

                //修改番数倍数
                //if (PPPD.iMethodId == 14)
                //{
                //    ma.transform.GetChild(2).GetComponent<Text>().text = "番";
                //}
                //else
                //{
                // ma.transform.GetChild(2).GetComponent<Text>().text = "倍";
                //  }
            }
            #endregion

            int mah_length = mah.Length ;
            if (mah.Length >= 9)
            {
                mah_length = 9;
              
                #region oldcode
                // TingShow.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 420 + 130 * (mah.Length - 3));
                //  TingShow.GetComponent<GridLayoutGroup>().spacing = new Vector2(15f, 0);
                // if (mah.Length > 8)
                // {
                //     TingShow.transform.localScale = Vector3.one * 0.75f;
                // }
                // else
                //  {
                TingShow.transform.localScale = Vector3.one;
                // }
                //  }
                // else
                // {
                //     TingShow.GetComponent<GridLayoutGroup>().spacing = new Vector2(50f, 0);
                //      TingShow.GetComponent<Image>().SetNativeSize();
                #endregion
            }
            //  Debug.LogError("这里执行        ");
            TingShow.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            TingShow.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount ;
            TingShow.GetComponent<GridLayoutGroup>().constraintCount = mah_length;
        }

        /// <summary>
        /// 吃牌提示
        /// </summary>
        public void OnSpwanEatShow(int index)
        {
            Mahjong[] game = ChiShow.GetComponentsInChildren<Mahjong>();
            if (game.Length > 0)
            {
                for (int i = 0; i < game.Length; i++)
                {
                    Destroy(game[i].gameObject);
                }
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            List<ChiCard> chicard = new List<ChiCard>();
            Mahjong[] mah_ = new Mahjong[14];
            int CardType = pppd.bLastCard / 16;
            int CardValue = pppd.bLastCard % 16;
            bool ismahjong = false;

            if (pppd.bLastCard <= 0) return;

            int value_index = 0;
            for (int i = 0; i < MahjongManger.Instance.GetSelfCard().Length; i++)
            {
                if ((MahjongManger.Instance.GetSelfCard()[i].bMahjongValue / 16) == CardType)
                {
                    mah_[value_index] = MahjongManger.Instance.GetSelfCard()[i];
                    value_index++;
                }
            }

            if (mah_.Length < 2) return;//这个花色的牌值小于两张

            ismahjong = false;
            if ((CardValue + 1) <= 9 && (CardValue + 1) > 0)
            {
                for (int i = 0; i < mah_.Length; i++)
                {
                    if (mah_[i] != null && mah_[i].bMahjongValue % 16 == (CardValue + 1))
                    {
                        for (int j = 0; j < mah_.Length; j++)
                        {
                            if (mah_[j] != null && mah_[j].bMahjongValue % 16 == (CardValue + 2))
                            {
                                ChiCard chi = new ChiCard();
                                chi.value1 = (byte)(CardValue + (CardType * 16));
                                chi.value2 = mah_[i].bMahjongValue;
                                chi.value3 = mah_[j].bMahjongValue;
                                chi.Num = 1;
                                chicard.Add(chi);
                                ismahjong = true;
                                break;
                            }
                        }
                        if (ismahjong) break;
                    }
                }
            }

            ismahjong = false;
            if ((CardValue - 1) > 0)
            {
                for (int i = 0; i < mah_.Length; i++)
                {
                    if (mah_[i] != null && mah_[i].bMahjongValue % 16 == (CardValue - 1))
                    {
                        for (int j = 0; j < mah_.Length; j++)
                        {
                            if (mah_[j] != null && mah_[j].bMahjongValue % 16 == (CardValue + 1))
                            {
                                ChiCard chi = new ChiCard();
                                chi.value1 = mah_[i].bMahjongValue;
                                chi.value2 = (byte)(CardValue + (CardType * 16));
                                chi.value3 = mah_[j].bMahjongValue;
                                chi.Num = 2;
                                chicard.Add(chi);
                                ismahjong = true;
                                break;
                            }
                        }
                        if (ismahjong) break;
                    }
                }
            }

            ismahjong = false;
            if ((CardValue - 2) > 0)
            {
                for (int i = 0; i < mah_.Length; i++)
                {
                    if (mah_[i] != null && mah_[i].bMahjongValue % 16 == (CardValue - 2))
                    {
                        for (int j = 0; j < mah_.Length; j++)
                        {
                            if (mah_[j] != null && mah_[j].bMahjongValue % 16 == (CardValue - 1))
                            {
                                ChiCard chi = new ChiCard();
                                chi.value1 = mah_[i].bMahjongValue;
                                chi.value2 = mah_[j].bMahjongValue;
                                chi.value3 = (byte)(CardValue + (CardType * 16));
                                chi.Num = 3;
                                chicard.Add(chi);
                                ismahjong = true;
                                break;
                            }
                        }
                        if (ismahjong) break;
                    }
                }
            }

            if (chicard.Count <= 0)
            {
                return;
            }

            if (chicard.Count > 1)
            {
                ChiShow.SetActive(true);
                GameObject[] obj = new GameObject[3];
                for (int i = 0; i < chicard.Count; i++)
                {
                    obj[i] = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaDCardsPre);
                    obj[i].transform.SetParent(ChiShow.transform);
                    obj[i].transform.localScale = Vector3.one;
                    obj[i].transform.localRotation = Quaternion.identity;
                    obj[i].transform.localPosition = Vector3.one;

                    //更新牌面的信息
                    if (chicard[i].Num == 1)
                        obj[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
                    else if (chicard[i].Num == 2)
                        obj[i].transform.GetChild(1).GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
                    else if (chicard[i].Num == 3)
                        obj[i].transform.GetChild(2).GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);

                    ChangeCardNum(obj[i].transform.GetChild(0).GetChild(0).GetComponent<Image>(), chicard[i].value1, index);

                    ChangeCardNum(obj[i].transform.GetChild(1).GetChild(0).GetComponent<Image>(), chicard[i].value2, index);

                    ChangeCardNum(obj[i].transform.GetChild(2).GetChild(0).GetComponent<Image>(), chicard[i].value3, index);

                    ChiCard chi = new ChiCard(); chi.value1 = chicard[i].value1; chi.value2 = chicard[i].value2; chi.value3 = chicard[i].value3; chi.Num = chicard[i].Num;
                    obj[i].transform.GetComponent<Mahjong>().ClickPanel.SetActive(true);
                    Button btn = obj[i].transform.GetComponent<Mahjong>().ClickPanel.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(delegate () { OnChiClickListen_(chi, index); });
                }
            }
            else
            {
                SendChi(chicard[0], index);
            }
            //处理玩家点击吃碰杠胡之后，直接删除按钮图标
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
        }

        void OnChiClickListen_(ChiCard chi, int index)
        {
            SendChi(chi, index);
            ChiShow.SetActive(false);
        }

        void SendChi(ChiCard chi, int index)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            byte[] mah = new byte[2];
            if (chi.Num == 1)
            {
                mah[0] = chi.value2; mah[1] = chi.value3;
            }
            else if (chi.Num == 2)
            {
                mah[0] = chi.value1; mah[1] = chi.value3;
            }
            else if (chi.Num == 3)
            {
                mah[0] = chi.value1; mah[1] = chi.value2;
            }

            NetMsg.ClientSpecialTileReq msg = new NetMsg.ClientSpecialTileReq();
            msg.byPongKong = (byte)pppd.IsPongKong();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.byaTiles = mah;
            msg.bySpecial = (byte)index;
            Network.NetworkMgr.Instance.GameServer.SendSpecialTileReq(msg);
        }

        /// <summary>
        /// 遍历还有几张某种花色的牌
        /// </summary>
        /// <param name="mahjong"></param>
        /// <returns></returns>
        public int UpdateLeftCardCount(byte mahjong)
        {
            int count = 4;
            //先遍历自己的手牌
            for (int i = 0; i < PPPD.usersCardsInfo[0].listCurrentCards.Count; i++)
            {
                if (PPPD.usersCardsInfo[0].listCurrentCards[i].cardNum == mahjong)
                {
                    count--;
                }
            }

            //遍历桌面上所有的牌
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < PPPD.usersCardsInfo[i].listShowCards.Count; k++)
                {
                    if (PPPD.usersCardsInfo[i].listShowCards[k].cardNum == mahjong)
                    {
                        count--;
                    }
                }
            }

            //遍历所有玩家的特殊牌型
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < PPPD.usersCardsInfo[i].listSpecialCards.Count; k++)
                {
                    for (int index = 0; index < 4; index++)
                    {
                        if (PPPD.usersCardsInfo[i].listSpecialCards[k].mahValue[index] == mahjong && PPPD.usersCardsInfo[i].listSpecialCards[k].mahValue[index] != 0)
                        {
                            //Debug.LogError("value：" + PPPD.usersCardsInfo[i].listSpecialCards[k].mahValue[index] + ",type:" + PPPD.usersCardsInfo[i].listSpecialCards[k].type);
                            //吃
                            if (PPPD.usersCardsInfo[i].listSpecialCards[k].type == 1)
                            {
                                count--;
                            }

                            //吃抢
                            if (PPPD.usersCardsInfo[i].listSpecialCards[k].type == 5 || PPPD.usersCardsInfo[i].listSpecialCards[k].type == 6)
                            {
                                count--;
                            }

                            //碰
                            if (PPPD.usersCardsInfo[i].listSpecialCards[k].type == 2)
                            {
                                count -= 3;
                            }

                            //杠
                            if (PPPD.usersCardsInfo[i].listSpecialCards[k].type == 3 || PPPD.usersCardsInfo[i].listSpecialCards[k].type == 7)
                            {
                                count -= 4;
                            }
                        }
                    }
                }
            }


            if (count <= 0)
            {
                count = 0;
            }

            return count;
        }



        int isDownTimerToChioce = 0;
        float fDownTime = 9f;

        /// <summary>
        /// 显示玩家能跑能下的界面
        /// </summary>
        /// <param name="type"></param>
        /// <param name="PlayID"> 显示那种状态，1是下分跑分  2是下分下一下二下三 </param>
        public void ShowCanDownRu(int type, int PlayID, int time = 0/*目前只有是2的情况下会有时间显示*/)
        {
            //显示界面
            CanDownRu.SetActive(true);
            Debug.LogWarning("显示玩家能跑能下的界面");
            if (PlayID == 1)
            {
                CanDownRu.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                CanDownRu.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                if (type == 1)
                {
                    CanDownRu.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    CanDownRu.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    CanDownRu.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    CanDownRu.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
                }
                Debug.LogWarning("下分跑分");
            }
            else if (PlayID == 2)
            {
                fDownTime = time;
                CanDownRu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                CanDownRu.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                Debug.LogWarning("下一下二下三");
            }

            isDownTimerToChioce = PlayID;
        }


        /// <summary>
        /// 点击选择下分 
        /// </summary>
        /// <param name="type"></param>
        public void BtnChioceDownOrRu(int type)
        {
            if (isDownTimerToChioce == 1)
            {
                Network.Message.NetMsg.ClientCanDownRunReq msg = new Network.Message.NetMsg.ClientCanDownRunReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.byCanDownRu = (byte)type;
                Network.NetworkMgr.Instance.GameServer.SendClientCanDownRuReq(msg);
            }
            else if (isDownTimerToChioce == 2)
            {
                Network.Message.NetMsg.ClientNextPointReqDef msg = new Network.Message.NetMsg.ClientNextPointReqDef();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.byNextPoint = (byte)type;
                //Network.NetworkMgr.Instance.GameServer.SendClientNextPointReqDef(msg);
            }

            isDownTimerToChioce = 0;
            fDownTime = 9f;
            CanDownRu.SetActive(false);
        }


        /// <summary>
        /// 改变玩家手中手牌的状态
        /// </summary>
        /// <param name="status"></param>
        public void ChangeHandCardStatus(int status)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (pppd.isChoiceTing < 1 || (status == 1 && pppd.isAlreadyChangeStatus))
            {
                return;
            }

            if (status == 1)
            {
                pppd.isAlreadyChangeStatus = true;
            }
            //改变牌的状态
            Mahjong[] mah = MahjongManger.Instance.GetSelfCard();

            for (int i = 0; i < mah.Length; i++)
            {
                if (!mah[i].transform.GetChild(3).gameObject.activeSelf)
                {
                    mah[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(0.62f, 0.62f, 0.62f, 1);
                    mah[i].transform.GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
            }
        }

        public GameObject[] Fangzhu;
        /// <summary>
        /// 房主
        /// </summary>
        /// <param name="seat"></param>
        public void UpdataFangZhu(int iseatNum)
        {
            int index = PPPD.GetOtherPlayerShowPos(iseatNum) - 1;

            for (int i = 0; i < 4; i++)
            {
                Fangzhu[i].SetActive(false);
            }
            Fangzhu[index].SetActive(true);
        }

        /// <summary>
        /// 更新玩家的听牌状态
        /// </summary>
        public void UpdatePlayerTingStatus(int index)
        {
            if (index < 0 && index > 3)
            {
                return;
            }
            TingStatus.transform.GetChild(index).gameObject.SetActive(true);
        }

        public void PlayPropAnimation()
        {

        }


        /// <summary>
        /// 四个玩家都在，等待服务器准备消息发过来
        /// </summary>
        /// <param name="PlayerComoIn">是玩家到齐了 false没有到齐</param>
        public void OnPlayerHereReady(bool PlayerComoIn, int ReadyTime, bool isShow = true)
        {
            if (isShow)
            {
                isCloseTime = false;
                PlayerReadyTimeLost.gameObject.SetActive(false);
                isYuYueRoomOnce = true;
                StartPlayerHereReadyTime = ReadyTime;
                if (PlayerComoIn)
                {
                    PlayerReady[0].SetActive(false);
                    PlayerReady[1].SetActive(true);
                    PlayerReadyTimeLost.transform.GetChild(2).gameObject.SetActive(false);
                    if (LoatTime <= 0)
                    {
                        Debug.Log("是预约房" + LoatTime);
                        PlayerReadyTimeLost.gameObject.SetActive(true);
                        //StartCoroutine(OnPlayerHereReady_());
                        isGameHereReadyTime = true;
                    }
                    else
                    {
                        Debug.Log("不是预约房" + LoatTime);
                        PlayerReadyTimeLost.gameObject.SetActive(false);
                    }

                }
                else
                {
                    if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
                    {
                        _btnShare.SetActive(false);
                        //  Debug.LogWarning(_btnShare.activeInHierarchy);

                    }
                    else
                    {
                        _btnShare.SetActive(true);
                    }
                    PlayerReady[1].SetActive(false);
                    //PlayerReadyTimeLost.gameObject.SetActive(false);
                    //StopCoroutine(OnPlayerHereReady_());
                }
            }
            else
            {
                PlayerReady[0].SetActive(false);
                PlayerReady[1].SetActive(false);
                PlayerReadyTimeLost.transform.GetChild(2).gameObject.SetActive(true);
                //PlayerReadyTimeLost.gameObject.SetActive(false);
                //StopCoroutine(OnPlayerHereReady_());
            }
        }
        //是预约房 打开预约房间的标志
        bool isCloseTime = false;

        /// <summary>
        /// 运行切到后台
        /// </summary>
        /// <param name="lostTime"></param>
        public void StartOnPlayerHereReady(int lostTime)
        {
            StartPlayerHereReadyTime = lostTime;
            LostTime = lostTime;
            //StartCoroutine(OnPlayerHereReady_());
        }

        //时间倒数
        IEnumerator OnPlayerHereReady_()
        {
            if (PlayerReadyTimeLost.gameObject.activeSelf == false) yield break;
            while (true)
            {
                if (LoatTime > 0) yield break;
                StartPlayerHereReadyTime--;
                if (StartPlayerHereReadyTime < 0)
                {
                    if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
                    {
                        _btnShare.SetActive(false);
                        //  Debug.LogWarning(_btnShare.activeInHierarchy);

                    }
                    else
                    {
                        _btnShare.SetActive(true);
                    }
                    PlayerReady[1].SetActive(false);
                    PlayerReadyTimeLost.gameObject.SetActive(false);
                    yield break;
                }
                PlayerReadyTimeLost.transform.GetChild(1).GetComponent<Text>().text = StartPlayerHereReadyTime.ToString();
                if (isCloseTime) yield break;
                yield return new WaitForSeconds(1.0f);
            }
        }

        bool isGameHereReadyTime = false;
        float isGameHereReadyTimetimer = 0;
        void GameHereReadyTime()
        {
            if (isGameHereReadyTime)
            {
                if (PlayerReadyTimeLost.gameObject.activeSelf == false) { isGameHereReadyTime = false; return; }
                while (isGameHereReadyTimetimer >= 1)
                {
                    if (LoatTime > 0) { isGameHereReadyTime = false; return; }
                    StartPlayerHereReadyTime--;
                    if (StartPlayerHereReadyTime < 0)
                    {
                        if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
                        {
                            _btnShare.SetActive(false);
                            //  Debug.LogWarning(_btnShare.activeInHierarchy);

                        }
                        else
                        {
                            _btnShare.SetActive(true);
                        }
                        PlayerReady[1].SetActive(false);
                        PlayerReadyTimeLost.gameObject.SetActive(false);
                        isGameHereReadyTime = false;
                        return;
                    }
                    isGameHereReadyTimetimer -= 1;
                    PlayerReadyTimeLost.transform.GetChild(1).GetComponent<Text>().text = StartPlayerHereReadyTime.ToString();
                    if (isCloseTime) break;
                }
                isGameHereReadyTimetimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// 关闭占座中
        /// </summary>
        public void OnCloseZhanZuo()
        {
            m_isYuYueRoom = false;
            m_gameStart = true;
            for (int i = 0; i < _txUsersScor.Length; i++)
            {
                _txUsersScor[i].text = "0";
                //PlayerInSeat[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < PlayerOutRoom.Length; i++)
            {
                PlayerOutRoom[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 进入房间是不是预约了或者在不在放假内的什么的
        /// </summary>
        /// <param name="outroom"></param>
        /// <param name="seat"></param>
        public void OnSetYuYueGameTabel(int outroom, int seat)
        {
            if (seat < 0) return;
            //0没人 1有人 2有人并且占座 3有人占座不在房间内
            switch (outroom)
            {
                case 0:
                    {
                        PlayerOutRoom[seat].gameObject.SetActive(false);
                        _txUsersScor[seat].text = "0";
                    }
                    break;
                case 1:
                    {
                        PlayerOutRoom[seat].gameObject.SetActive(false);
                        _txUsersScor[seat].text = "0";
                    }
                    break;
                case 2:
                    {
                        PlayerOutRoom[seat].gameObject.SetActive(false);
                        _txUsersScor[seat].text = "占座中";
                    }
                    break;
                case 3:
                    {
                        PlayerOutRoom[seat].gameObject.SetActive(true);
                        _txUsersScor[seat].text = "占座中";
                    }
                    break;
            }
        }

        /// <summary>
        /// 占座和取消占座的消息回应
        /// </summary>
        /// <param name="index">是哪一个座位号</param>
        /// <param name="isInSeat">是占座还是取消占座 true占座</param>
        public void OnCLickSeatSuccess(int index, bool isInSeat, bool isPlayerShow = true, bool isShow = true)
        {
            index = 0;
            if (isShow)
            {
                if (isPlayerShow)
                {
                    PlayerSeat[index].SetActive(true);
                }
                else
                {
                    PlayerSeat[index].SetActive(false);
                }

                if (isInSeat)//占座
                {
                    PlayerSeat[index].transform.GetChild(0).gameObject.SetActive(true);
                    PlayerSeat[index].transform.GetChild(2).gameObject.SetActive(true);
                    PlayerSeat[index].transform.GetChild(2).GetComponent<Text>().text = "取消占座";

                    PlayerSeat[index].transform.GetChild(0).gameObject.SetActive(false);
                    PlayerSeat[index].transform.GetChild(1).gameObject.SetActive(true);
                    isSeat = true;
                }
                else//取消占座
                {
                    PlayerSeat[index].transform.GetChild(0).gameObject.SetActive(true);
                    PlayerSeat[index].transform.GetChild(2).gameObject.SetActive(true);
                    PlayerSeat[index].transform.GetChild(2).GetComponent<Text>().text = "占座";

                    PlayerSeat[index].transform.GetChild(0).gameObject.SetActive(true);
                    PlayerSeat[index].transform.GetChild(1).gameObject.SetActive(false);
                    isSeat = false;
                }

                PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
                //说明是创建房间的 并且是预约房 并且不是老板
                if (pppd.iOpenRoomUserId == GameData.Instance.PlayerNodeDef.iUserId && LoatTime > 0 && MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iMyParlorId <= 0)
                {
                    PlayerSeat[index].transform.GetChild(0).gameObject.SetActive(false);
                    PlayerSeat[index].transform.GetChild(1).gameObject.SetActive(false);
                    PlayerSeat[index].transform.GetChild(2).GetComponent<Text>().text = "";
                    _txUsersScor[index].text = "占座中";
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    PlayerSeat[i].SetActive(false);
                }
            }
        }

        [HideInInspector]
        public uint WaitTimeForStartGame = 0;
        /// <summary>
        /// 是预约房间界面上增加时间显示
        /// </summary>
        /// <param name="WaitTime"></param>
        public void OnAppointmentRoom(uint WaitTime)
        {
            WaitTimeForStartGame = WaitTime;
            LoatTime = WaitTime;
            if (WaitTime <= 0 || CreatRoomTime <= 0)
            {
                AppointmentRoom.SetActive(false);
                return;
            }

            ////if (LoatTime > 0)
            //{
            //    BtnBackLobby.SetActive(true);
            //}

            DateTime necttime = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime));
            int subtime = (int)(MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(necttime) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now));

            //ulong lingTime = MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime)) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now);
            //if (lingTime <= 0)
            //{
            //    AppointmentRoom.SetActive(false);
            //    return;
            //}

            isSeat = false;//如果是其他玩家
            PlayerReadyTimeLost.gameObject.SetActive(false);
            isCloseTime = true;
            isYuYueRoomOnce = true;
            yuyuefanglosttime = false;
            AppointmentRoom.SetActive(true);
            string time = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime)).ToString("HH:mm");
            AppointmentRoom.transform.GetChild(1).GetComponent<Text>().text = time.ToString();
            isWaitForAppointmentRoom = true;
            EndWaitForAppointmentRoomTime = LoatTime * 60;

            StartCoroutine(GetNetWorkTimeForInt());
        }

        IEnumerator GetNetWorkTimeForInt()
        {
            WWW www = new WWW(LobbyContants.WebTime);
            yield return www;
            int TimerNow = Convert.ToInt32(www.text.Substring(2, 10));

            DateTime necttime = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime));
            int subtime = (int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(necttime) - TimerNow;

            if (subtime <= 0)
            {
                Debug.LogWarning("这个预约房的时间已经到了");

                isWaitForAppointmentRoom = false;
                AppointmentRoom.SetActive(false);
                LoatTime = 0;
                PlayerSeat[0].transform.GetChild(0).gameObject.SetActive(false);
                PlayerSeat[0].transform.GetChild(1).gameObject.SetActive(false);
                PlayerSeat[0].transform.GetChild(2).gameObject.SetActive(false);
                for (int i = 0; i < _txUsersScor.Length; i++)
                    _txUsersScor[i].text = "0";
                //for (int i = 0; i < PlayerInSeat.Length; i++)
                //    PlayerInSeat[i].gameObject.SetActive(false);
                for (int i = 0; i < PlayerOutRoom.Length; i++)
                    PlayerOutRoom[i].gameObject.SetActive(false);

                AppointmentRoom.SetActive(false);
            }
            else
            {
                StartCoroutine(WaitForAppointmentRoomTime());
            }
        }

        private float StartWaitForAppointmentRoomTime = 0;
        private uint EndWaitForAppointmentRoomTime = 0;
        //是预约房的标志
        private bool isWaitForAppointmentRoom = false;
        bool isYuYueRoomOnce = false;
        bool isOnce = false;
        IEnumerator WaitForAppointmentRoomTime()
        {
            while (true)
            {
                if (isWaitForAppointmentRoom == false || m_gameStart)
                    yield break;

                EndWaitForAppointmentRoomTime--;

                WWW www = new WWW(LobbyContants.WebTime);
                yield return www;
                int TimerNow = Convert.ToInt32(www.text.Substring(2, 10));

                //TimeSpan time_ = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime) - DateTime.Now);
                TimeSpan time3_1 = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(CreatRoomTime, 0).AddMinutes(LoatTime)).Subtract(MahjongCommonMethod.Instance.UnixTimeStampToDateTime(TimerNow, 0)).Duration();
                string Toime = time3_1.Hours.ToString("00") + ":" + time3_1.Minutes.ToString("00") + ":" + time3_1.Seconds.ToString("00");
                AppointmentRoom.transform.GetChild(2).GetComponent<Text>().text = Toime;

                //if((time3_1.Hours <= 0 && time3_1.Minutes <= 0 && time3_1.Seconds <= 60))
                //{
                //    Debug.LogError("最后六十秒");
                //    OnSetYuYueRoomLostOneMin(time3_1.Seconds);
                //}

                if (EndWaitForAppointmentRoomTime <= 0 || (time3_1.Hours <= 0 && time3_1.Minutes <= 0 && time3_1.Seconds <= 1))
                {
                    //是不是老板创建的房间
                    //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER, () => { OnPlayerHereReady(false, 0, false); }, Ok);
                    //UIMgr.GetInstance().GetUIMessageView().Show("服务器正在处理，请稍等！！！");

                    isWaitForAppointmentRoom = false;
                    AppointmentRoom.SetActive(false);
                    LoatTime = 0;
                    PlayerSeat[0].transform.GetChild(0).gameObject.SetActive(false);
                    PlayerSeat[0].transform.GetChild(1).gameObject.SetActive(false);
                    PlayerSeat[0].transform.GetChild(2).gameObject.SetActive(false);
                    for (int i = 0; i < _txUsersScor.Length; i++)
                        _txUsersScor[i].text = "0";
                    //for (int i = 0; i < PlayerInSeat.Length; i++)
                    //    PlayerInSeat[i].gameObject.SetActive(false);
                    for (int i = 0; i < PlayerOutRoom.Length; i++)
                        PlayerOutRoom[i].gameObject.SetActive(false);

                    AppointmentRoom.SetActive(false);
                    yield break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }


        public void OnOpenGameRulePanel()
        {
            StartCoroutine(UIMainView.Instance.playeringRule.OnShowFirstComeInGame());
        }

        /// <summary>
        /// 如果是第二次预约时间倒计时
        /// </summary>
        /// <param name="lostTime"></param>
        public void OnSetYuYueRoomLostOneMin(int lostTime)
        {
            if (lostTime <= 0) return;
            //if (LostTime > 0 && lostTime > 0)
            //{
            //    Debug.LogError("两个时间都不为零");
            //    return;
            //}
            Debug.LogWarning("预约房最后一分钟");
            isWaitForAppointmentRoom = false;

            isCloseTime = false;
            PlayerReadyTimeLost.gameObject.SetActive(true);
            //StopCoroutine(OnPlayerHereReady_());
            isGameHereReadyTime = true;
            isYuYueRoomOnce = false;
            AppointmentRoom.SetActive(false);
            LostTime = lostTime;
            yuyuefanglosttime = true;
            StartCoroutine(OnSetYuYueRoomLostOneMin_());
        }
        public int LostTime = 0;
        public bool yuyuefanglosttime = false;
        public float yuyuefanglosttimeFloat = 0.0f;
        IEnumerator OnSetYuYueRoomLostOneMin_()
        {
            if (PlayerReadyTimeLost.gameObject.activeSelf == false) yield break;
            while (true)
            {
                LostTime--;

                if (m_gameStart) yield break;

                if (LostTime < 0)
                {
                    //UIMgr.GetInstance().GetUIMessageView().Show(TextConstant.ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER, () => { OnPlayerHereReady(false, 0, false); }, Ok);
                    //UIMgr.GetInstance().GetUIMessageView().Show("服务器正在处理，请稍等！！！");
                    PlayerReadyTimeLost.gameObject.SetActive(false);

                    EndWaitForAppointmentRoomTime = 0;
                    StartWaitForAppointmentRoomTime = 0;
                    isWaitForAppointmentRoom = false;
                    AppointmentRoom.SetActive(false);
                    LoatTime = 0;
                    PlayerSeat[0].transform.GetChild(0).gameObject.SetActive(false);
                    PlayerSeat[0].transform.GetChild(1).gameObject.SetActive(false);
                    PlayerSeat[0].transform.GetChild(2).gameObject.SetActive(false);
                    for (int i = 0; i < _txUsersScor.Length; i++)
                        _txUsersScor[i].text = "0";
                    //for (int i = 0; i < PlayerInSeat.Length; i++)
                    //    PlayerInSeat[i].gameObject.SetActive(false);
                    for (int i = 0; i < PlayerOutRoom.Length; i++)
                        PlayerOutRoom[i].gameObject.SetActive(false);

                    PlayerReadyTimeLost.gameObject.SetActive(false);
                    LostTime = 0;
                    yield break;
                }
                PlayerReadyTimeLost.transform.GetChild(1).GetComponent<Text>().text = LostTime.ToString();
                yield return new WaitForSeconds(1.0f);
            }
        }

        public void OnSetClose()
        {
            PlayerSeat[0].transform.GetChild(0).gameObject.SetActive(false);
            PlayerSeat[0].transform.GetChild(1).gameObject.SetActive(false);
            PlayerSeat[0].transform.GetChild(2).gameObject.SetActive(false);
            for (int i = 0; i < _txUsersScor.Length; i++)
                _txUsersScor[i].text = "0";
            for (int i = 0; i < PlayerOutRoom.Length; i++)
                PlayerOutRoom[i].gameObject.SetActive(false);
        }


        /// <summary>
        /// 更新花牌数量
        /// </summary>
        /// <param name="status">0表示不包含花牌 1表示包含花牌 2更新具体某个花牌</param>
        public void UpdateShowFlowerCount(int status, int seatNum, int count, int Add = 0)
        {
            if (status == 0)
            {
                for (int i = 0; i < Flower.Length; i++)
                {
                    Flower[i].gameObject.SetActive(false);
                }
            }
            else
            {
                if (status == 1)
                {
                    for (int i = 0; i < Flower.Length; i++)
                    {
                        Flower[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    int index = PPPD.GetOtherPlayerShowPos(seatNum) - 1;
                    Flower[index].gameObject.SetActive(true);
                    if (Add == 0)
                    {
                        PPPD.PlayerFlowerCount[seatNum - 1] += count;
                    }
                    Flower[index].transform.GetChild(0).GetComponent<Text>().text = "x" + PPPD.PlayerFlowerCount[seatNum - 1];
                }
            }
        }
        public bool DealHongZhongZhongma(byte[] value)
        {
            bool isFanMa = false;
            // Debug.LogError("获得种马"+value.Length  );
            for (int i = 1; i < 7 ; i++)
            {
                Debug.LogWarning("翻码结果;"+value[i].ToString("x2"));
                switch (value[i] >> 4)
                {
                    case 1:
                        _gHongzhongZhongma.transform.GetChild(i-1).GetChild(0).GetComponent<Image>().sprite = _wanNum[(value[i] & 0x0f) - 1];
                        break;
                    case 2:
                        _gHongzhongZhongma.transform.GetChild(i-1).GetChild(0).GetComponent<Image>().sprite = _tonNum[(value[i] & 0x0f) - 1];
                        break;
                    case 3:
                        _gHongzhongZhongma.transform.GetChild(i-1).GetChild(0).GetComponent<Image>().sprite = _tioNum[(value[i] & 0x0f) - 1];
                        break;
                    case 4:
                        _gHongzhongZhongma.transform.GetChild(i-1).GetChild(0).GetComponent<Image>().sprite = _fen[(value[i] & 0x0f) - 1];
                        break;
                    case 5:
                        _gHongzhongZhongma.transform.GetChild(i-1).GetChild(0).GetComponent<Image>().sprite = _jiNum[(value[i] & 0x0f) - 1];
                        break;
                    default:
                        break;
                }
                if (value[i] > 0)
                {
                    isFanMa = true;//显示翻码
                    _gHongzhongZhongma.transform.GetChild(i-1).gameObject.SetActive(true);
                }
                else
                {
                    _gHongzhongZhongma.transform.GetChild(i-1).gameObject.SetActive(false);
                }
            }
            _gHongzhongZhongma.SetActive(isFanMa);
            return isFanMa;

        }
        public void AddIEnumerator()
        {
            Debug.Log (Time.deltaTime + "--  "+MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t);
           GameData.Instance.PlayerPlayingPanelData . DelayToolBtnClick = DelayEnableBtnClickEffect();
           StartCoroutine(GameData.Instance.PlayerPlayingPanelData.DelayToolBtnClick );
        }
        IEnumerator DelayEnableBtnClickEffect()
        {

            while (MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t <= MahjongGame_AH.Data.PlayerPlayingPanelData.GLOBLE_BUTTONTIME)
            {
                MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t += Time.fixedDeltaTime;
                yield return null;
            }

            yield break;
        }
        /// <summary>
        /// 过胡提示
        /// </summary>
        /// <param name="show"></param>
        public void ShowSkipWin(bool show)
        {
            SkipWinImage.gameObject.SetActive(show);
        }
    }
}
