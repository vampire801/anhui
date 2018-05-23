using UnityEngine;
using System.Collections;
using System;
using MahjongLobby_AH;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MahjongLobby_AH.Network.Message;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using XLua;
namespace anhui
{
    [Hotfix]
    [LuaCallCSharp]
    public class MahjongCommonMethod : MonoBehaviour
    {
        public const string assetUrl = @"http://localhost/AssetBundles/";
        public const string fishUrl = "https://by.ibluejoy.com/def.html";

        /// <summary>
        /// shareId：用户ID
        /// type:1朋友圈，2好友，可以为空，为空默认为1
        /// id:当shareId为3（充值红包分享）时表示红包ID、为4（提现红包分享）时表示用户ID，其他情况不需要该值.
        /// </summary>
        public const string shareParam = @"{0}shareParam.x?shareId={1}&type={2}&id={3}";
        public const string communicatUrl = "communication.x?County_id={0}";
        public const string assetUrl_audiosource = "audiosource.lee";
        public const string assetUrl_gameaudio = "gameaudio.lee";
        public const string assetUrl_lobbyaudio = "lobbyaudio";
        #region 实例
        //public Canvas _logCanves;
        //public InputField _logInputField;
        static MahjongCommonMethod instance;
        public static MahjongCommonMethod Instance
        {
            get
            {
                return instance;
            }
        }
        public NetMsg.ClientGetExchangeCoinRes _configcharge = new NetMsg.ClientGetExchangeCoinRes();
        public Dictionary<int, NetMsg.Charge> _dicCharge = new Dictionary<int, NetMsg.Charge>();
        // public NetMsg.Exchange _configExchage = new NetMsg.Exchange();
        public Dictionary<int, NetMsg.Exchange> _dicExchage2 = new Dictionary<int, NetMsg.Exchange>();
        public Dictionary<int, NetMsg.Exchange> _dicExchage3 = new Dictionary<int, NetMsg.Exchange>();
        public List<NetMsg.Exchange> ExChange = new List<NetMsg.Exchange>();  //业绩兑换数据
        public CommonConfig.Json_FAQ _cfgFAQ = new CommonConfig.Json_FAQ();
        public CommonConfig.Json_SpecialTypeCardName _cfgSpecialTypeCardName = new CommonConfig.Json_SpecialTypeCardName();
        public List<int> lsMethodId = new List<int>();  //存储玩法的id    
        public int iCityId; //城市id
        public int iCountyId; //县的id    
        public string Url_Center; //会员中心地址
        [HideInInspector]
        public bool isReadJson;  //是否已经读取过json文件
        HeadImageManager headManager;
        public List<CommonConfig.CityConfig> _cityConfig = new List<CommonConfig.CityConfig>();
        public List<CommonConfig.DistrictConfig> _districtConfig = new List<CommonConfig.DistrictConfig>();
        public List<CommonConfig.MethodConfig> _methodConfig = new List<CommonConfig.MethodConfig>();
        public Dictionary<int, CommonConfig.CityConfig> _dicCityConfig = new Dictionary<int, CommonConfig.CityConfig>();
        public Dictionary<int, CommonConfig.DistrictConfig> _dicDisConfig = new Dictionary<int, CommonConfig.DistrictConfig>();
        public Dictionary<int, CommonConfig.MethodConfig> _dicMethodConfig = new Dictionary<int, CommonConfig.MethodConfig>();
        public bool isVoiceHandler;
        public string MacId = "No_Mac";  //mac地址

        public List<CommonConfig.CardType> _cardType = new List<CommonConfig.CardType>();
        public List<CommonConfig.MethodToCardType> _methodToCardType = new List<CommonConfig.MethodToCardType>();

        //保存每个城市有多少个县
        public Dictionary<int, List<CommonConfig.DistrictConfig>> cityToCounty = new Dictionary<int, List<CommonConfig.DistrictConfig>>();

        /// <summary>
        /// 保存玩法对应的牌型信息
        /// </summary>
        public Dictionary<int, List<CommonConfig.MethodToCardType>> _dicmethodToCardType = new Dictionary<int, List<CommonConfig.MethodToCardType>>();

        public Dictionary<int, CommonConfig.CardType> _cardType_ = new Dictionary<int, CommonConfig.CardType>();
        /// <summary>
        /// 保存每个玩法对应的牌型规则
        /// </summary>
        public Dictionary<int, List<CommonConfig.CardType>> _dicMethodCardType = new Dictionary<int, List<CommonConfig.CardType>>();

        /// <summary>
        /// 保存县对应玩法的名字
        /// </summary>
        public List<CommonConfig.PlayNameConfig> _lsPlayNameConfig = new List<CommonConfig.PlayNameConfig>();
        public Dictionary<int, List<CommonConfig.PlayNameConfig>> _dicPlayNameConfig = new Dictionary<int, List<CommonConfig.PlayNameConfig>>();
        public const string soundSettingDate = "soundSettingDate";
        [HideInInspector]
        public int MusicVolume = 10;
        [HideInInspector]
        public int LastMusicVolume = 100;
        [HideInInspector]
        public int EffectValume = 100;
        [HideInInspector]
        public int LastEffectValume = 100;
        [HideInInspector]
        public int LastVoiceValume = 100;
        [HideInInspector]
        public bool isMusicShut = false;//是否静音
        [HideInInspector]
        public bool isEfectShut = false;
        /// <summary>
        /// 是否开启收听语音
        /// </summary>
        [HideInInspector]
        public bool isOpenVoicePlay = true;
        [HideInInspector]
        public int iFromParlorInGame; //从麻将馆进入游戏的麻将馆编号
        [HideInInspector]
        public int iGameId = 20;  //该游戏对应的游戏编号
        [HideInInspector]
        public string RoomId = "0";  //玩家的房间id
        [HideInInspector]
        public int mySeatid;//玩家位置num
        [HideInInspector]
        public string SeverIp;  //游戏服务器ip 
        [HideInInspector]
        public ushort SeverPort;  //游戏服务器端口    
        [HideInInspector]
        public int PlayerRoomStatus;  //玩家房间状态，0表示没有开房间，1表示已经开过房间
        [HideInInspector]
        public int iTableNum;  //玩家的桌号
        [HideInInspector]
        public int iSeverId;  //玩家桌号对应的服务器编号
        [HideInInspector]
        public byte cOpenRoomMode = 1;  //开放类型，1普通开房，2代开房
        public static bool isGameToLobby;  //是否游戏返回大厅的标志位 
        public static bool isConnectGameSeverFailed;  //连接游戏服务器失败  
        public static bool isLoginOut; //是否是点击注销账号
        public bool isSpwanCommonBounce; //是否已经产生公共弹框
        Transform CommonBoundce;  //公共提示条信息

        [HideInInspector]
        public bool isCreateRoom = true;
        public bool isSitSuccess; //玩家进入游戏是否坐下成功
        public static bool isAuthenSuccessInLobby; //玩家返回大厅，认证回应是否成功
        public bool isLobbySendAuthenReq; //表示从游戏返回游戏到大厅之后，在start发送认证请求失败，再次在成功连接大厅服务器的回应里面再次发送认证请求
        public static int iAutnehType; //保存玩家的登陆方式
        public bool isGameDisConnect;  //是否是游戏内断网的标志位

        //用于第三种登陆方式赋值
        public float fLongitude;  //存储玩家的经纬度
        public float fLatitude;  //存储玩家的经纬度
        public string sPlayerAddress;  //玩家的位置信息
        public static string PlayerIp = "192.168.1.0";
        //玩家的登录ip

        public bool isMoNiQi; //是否是模拟器
        public int iUserid = 0;
        public string accessToken = "26i45KqGqoLbkSbuVmRy-V0x_5IUiwx8JrvTkYASuRK9DqJ6hovlU8PXK71ZzWczYkv2O-kwTUgcIw90tiYF0lxkls_mLwfwkHcVH2-HxMY";

        [HideInInspector]
        public static bool isSpwanLoading;
        //public SceneLoding Loading; //加载场景的面板

        #region 玩家创建房间的选择的参数
        //  [HideInInspector]
        // public int MethodId = 100010;  //玩法的id 
        [HideInInspector]
        public int CreatRoomType;  //创建房间的类型，1表示自己创建，2表示代理代开
        [HideInInspector]
        public int iColorFlag;  //颜色标志
        [HideInInspector]
        public int iCompliment = 0;  //赞的数量
        [HideInInspector]
        public int iDisconnectRate = 0;  //掉线率要求
        [HideInInspector]
        public int iDiscardTime = 0; //出牌速度要求
        [HideInInspector]
        public int iRoomCard = 1;  //开房要消耗的房卡数量

        [HideInInspector]
        public int iPrice = 1;  //付费
        [HideInInspector]
        public int iMultiple = 2; //庄闲倍数，现在默认2
        [HideInInspector]
        public int iBasePoint = 1;  //低分，默认1
        [HideInInspector]
        public int iExposedMode = 0;  //明杠收三家  0表示没选 1表示选中
        [HideInInspector]
        public int iSevenPairs = 0; //七对 
        [HideInInspector]
        public int iLuxurySevenPairs = 0; //豪华七对
        [HideInInspector]
        public int iThirteenIndepend = 0;  //十三不靠
        [HideInInspector]
        public int iFourKong = 0;  //四杠荒庄
        [HideInInspector]
        public int iThirteenOrphans = 0;  //十三幺
        [HideInInspector]
        public int iRaiseMode = 0;  //前抬后和，0表示不用，1表示前抬，2表示后和
        [HideInInspector]
        public int iDraw = 0;  //必须自摸 0表示可接炮  1表示仅可自摸
        [HideInInspector]
        public int iMultiShoot = 0;  //仅当可接炮选择时才会出现一炮多响
        [HideInInspector]
        internal int iPlayingMethod = 0;

        [HideInInspector]
        public delegate void GameLaterDo(); //在这之后到什么条件做什么事
        [HideInInspector]
        public GameLaterDo GameLaterDo_;

        [HideInInspector]
        public NetMsg.ParlorInfoDef[] parlorUserID = new NetMsg.ParlorInfoDef[4];//如果自己是馆主，则把信息存放在数组的第一个位置
                                                                                 //如果自己不是馆主，则会把信息按玩家节点中的馆的信息顺序保存
                                                                                 //设置麻将馆

        /// <summary>
        /// 获取麻将馆
        /// </summary>
        public void SetParlorUserID()
        {
            parlorUserID = GameData.Instance.ParlorShowPanelData.parlorInfoDef;
        }

        /// <summary>
        /// 获取麻将馆馆主ID
        /// </summary>
        /// <param name="QueryParlorID"></param>
        /// <returns></returns>
        public NetMsg.ParlorInfoDef GetParlorUserID(int QueryParlorID)
        {
            NetMsg.ParlorInfoDef QueryID = new NetMsg.ParlorInfoDef();
            for (int i = 0; i < parlorUserID.Length; i++)
            {
                if (QueryParlorID == parlorUserID[i].iBossId)
                {
                    QueryID = parlorUserID[i];
                    break;
                }
            }
            return QueryID;
        }

        #endregion
        public MahjongCommonMethod()
        {

        }
        public MahjongCommonMethod(int methodId, byte[] value)
        {
            iPrice = value[0];
            iExposedMode = value[3];
            iSevenPairs = value[4];
            iLuxurySevenPairs = value[5];

            if (methodId == 100010)
            {
                iThirteenIndepend = value[6];
                iFourKong = value[7];
                iRaiseMode = value[8];
                iDraw = value[9];
                iMultiShoot = value[10];
            }
            else
            {
                iThirteenOrphans = value[6];
                iRaiseMode = value[7];
                iDraw = value[8];
                iMultiShoot = value[9];
            }


        }

        void Awake()
        {
            instance = this;
            //http 加载资源
            //StartCoroutine ( AssetBundleManager.downloadAssetBundle(assetUrl + assetUrl_audiosource, 1));


            headManager = new HeadImageManager();
            MusicVolume = 30;
            EffectValume = 100;
            isMusicShut = false;
            isEfectShut = false;
            DontDestroyOnLoad(gameObject);
            //获取玩家的mac地址
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                MacId = jo.Call<string>("getLocalMacAddressFromIp");
            }
            else
            {
                MacId = "NO_MAC";
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                MacId = "NO_MAC";
            }
        }
        void Start()
        {
            isFinshSceneLoad = true;
            iFromParlorInGame = 0;
            cOpenRoomMode = 1;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            StartCoroutine(GetLocalJson(_cfgFAQ, (a) =>
            {
                _cfgFAQ = a;//读取FAQjson

            }));
            StartCoroutine(GetLocalJson(_cfgSpecialTypeCardName, (a) =>
            {
                _cfgSpecialTypeCardName = a;//读取FAQjson

            }));
        }
        void OnEnable()
        {
            if (PlayerPrefs.HasKey(soundSettingDate))
            {
                deciphering(PlayerPrefs.GetInt(soundSettingDate));
            }
        }
        void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(soundSettingDate, encrypted());
        }
        void OnApplicationPause()
        {
            PlayerPrefs.SetInt(soundSettingDate, encrypted());
        }
        void OnDisable()
        {
            PlayerPrefs.SetInt(soundSettingDate, encrypted());
        }
        //100 000 000 5->101
        //音乐 音效 语音

        int encrypted()
        {
            int aa = 0;
            string str;
            str = isMusicShut ? "1" : "0";
            str += isEfectShut ? "1" : "0";
            str += isOpenVoicePlay ? "1" : "0";
            aa = MusicVolume * 10000000 + EffectValume * 10000 + LastVoiceValume * 10 + Byte.Parse(str);
            return aa;
        }
        void deciphering(int aa)
        {
            MusicVolume = aa / 10000000;
            EffectValume = (aa % 10000000) / 10000;
            LastVoiceValume = ((aa % 10000000) % 10000) / 10;
            int s = aa % 10;
            //Debug.LogError(s);
            isMusicShut = (s & 4) == 4 ? true : false;
            isEfectShut = (s & 2) == 2 ? true : false;
            isOpenVoicePlay = (s & 1) == 1 ? true : false;
        }
        #endregion 实例
        IEnumerator loadAsset()
        {
            AssetBundle a1;
            while (true)
            {
                a1 = AssetBundleManager.getAssetBundle(MahjongCommonMethod.assetUrl + MahjongCommonMethod.assetUrl_audiosource, 1);
                if (a1)
                {
                    break;
                }
                yield return null;
            }
            gameObject.transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().clip = a1.LoadAsset<AudioClip>("renshuoshanxiBest1_01");
        }

        #region 读取配置文件
        public void ReadJson()
        {


            //只用读取一次配置文件
            if (isReadJson)
            {
                return;
            }

            _methodConfig = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/MethodConfig").text).methodConfig;
            for (int i = 0; i < _methodConfig.Count; i++)
            {
                if (SDKManager.Instance.IOSCheckStaus == 1 || SDKManager.Instance.CheckStatus == 1)
                {
                    if (_methodConfig[i].METHOD == 2)
                    {
                        _methodConfig[i].METHOD_NAME = "推倒胡";
                    }
                }
                _dicMethodConfig.Add(_methodConfig[i].METHOD, _methodConfig[i]);
            }

            _cityConfig = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/CityConfig").text).cityConfig;
            for (int i = 0; i < _cityConfig.Count; i++)
            {
                _dicCityConfig.Add(_cityConfig[i].CITY_ID, _cityConfig[i]);
            }

            _districtConfig = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/DistrictConfig").text).districtConfig;


            //Debug.LogError("_districtConfig.Count:" + _districtConfig.Count + ",countyid:" + GameData.Instance.SelectAreaPanelData.iCountyId);
            for (int i = 0; i < _districtConfig.Count; i++)
            {
                if (SDKManager.Instance.IOSCheckStaus == 1 || SDKManager.Instance.CheckStatus == 1)
                {
                    if (_districtConfig[i].COUNTY_ID == 341125)
                    {
                        _districtConfig[i].METHOD = "20015";
                    }
                }
                _dicDisConfig.Add(_districtConfig[i].COUNTY_ID, _districtConfig[i]);
                if (_districtConfig[i].COUNTY_ID == GameData.Instance.SelectAreaPanelData.iCountyId && _districtConfig[i].VALID == 2)
                {
                    string[] id = _districtConfig[i].METHOD.Split('_');
                    for (int k = 0; k < id.Length; k++)
                    {
                        int ID = Convert.ToInt16(id[k]);
                        if (ID != 0)
                        {
                            //  Debug.LogWarning("读取本地配置");
                            lsMethodId.Add(ID);
                        }
                    }
                }
            }
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();

            //读取玩法对应的规则
            _cardType = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/CardTypeConfig").text).cardType;
            for (int i = 0; i < _cardType.Count; i++)
            {
                //Debug.LogWarning ("_________" + _cardType[i].ID);
                _cardType_.Add(_cardType[i].ID, _cardType[i]);
            }

            _methodToCardType = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/MethodToRuleConfig").text).methodToCardType;
            for (int j = 0; j < _methodConfig.Count; j++)
            {
                for (int i = 0; i < _methodToCardType.Count; i++)
                {
                    List<CommonConfig.MethodToCardType> temp_ = new List<CommonConfig.MethodToCardType>();
                    if (_methodToCardType[i].METHOD == _methodConfig[j].METHOD)
                    {
                        temp_.Add(_methodToCardType[i]);
                        if (_dicmethodToCardType.ContainsKey(_methodConfig[j].METHOD))
                        {
                            _dicmethodToCardType[_methodConfig[j].METHOD].Add(_methodToCardType[i]);
                        }
                        else
                        {
                            _dicmethodToCardType.Add(_methodConfig[j].METHOD, temp_);
                        }
                    }

                }

            }



            for (int j = 0; j < _methodConfig.Count; j++)
            {
                for (int i = 0; i < _methodToCardType.Count; i++)
                {
                    if (_methodToCardType[i].METHOD == _methodConfig[j].METHOD)
                    {
                        List<CommonConfig.CardType> temp = new List<CommonConfig.CardType>();
                        //Debug.LogWarning ("++++++" + _methodToCardType[i].RuleId);

                        temp.Add(_cardType_[_methodToCardType[i].RuleId]);

                        //保存值
                        if (_dicMethodCardType.ContainsKey(_methodConfig[j].METHOD))
                        {
                            _dicMethodCardType[_methodConfig[j].METHOD].Add(_cardType_[_methodToCardType[i].RuleId]);
                        }
                        else
                        {
                            _dicMethodCardType.Add(_methodConfig[j].METHOD, temp);
                        }
                    }
                }
            }

            //读取县对应玩法的名字
            _lsPlayNameConfig = JsonBase.DeserializeFromJson<CommonDatas>(Resources.Load<TextAsset>("Json/PlayNameConfig").text).playNameConfig;
            for (int i = 0; i < _lsPlayNameConfig.Count; i++)
            {
                if (SDKManager.Instance.CheckStatus == 1 || SDKManager.Instance.IOSCheckStaus == 1)
                {
                    if (_lsPlayNameConfig[i].COUNTY_ID == 140421)
                    {
                        _lsPlayNameConfig[i].METHOD = 2;
                        _lsPlayNameConfig[i].METHOD_NAME = "推倒胡";
                    }
                }
                List<CommonConfig.PlayNameConfig> temp = new List<CommonConfig.PlayNameConfig>();
                temp.Add(_lsPlayNameConfig[i]);

                if (_dicPlayNameConfig.ContainsKey(_lsPlayNameConfig[i].COUNTY_ID))
                {
                    _dicPlayNameConfig[_lsPlayNameConfig[i].COUNTY_ID].Add(_lsPlayNameConfig[i]);
                }
                else
                {
                    _dicPlayNameConfig.Add(_lsPlayNameConfig[i].COUNTY_ID, temp);
                }

            }

            OnGetCreatRoomNeedMoneyValue();
            UIMainView.Instance.HolidayActivityPanel.GetActivityJson();

            isReadJson = true;
        }

        public class MethodConfig_
        {
            public int id;  //id
            public int mode1;
            public int mode2;
            public int mode3;

            public int num1;
            public int num2;
            public int num3;

            public int price1;
            public int price2;
            public int price3;
        }

        public class MethodConfig__
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<MethodConfig_> data;
        }

        public class MethodConfig___
        {
            public List<MethodConfig__> methodConfig__ = new List<MethodConfig__>();
        }

        void OnGetCreatRoomNeedMoneyValue()
        {
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "playMethod.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "playMethod.x";
            }
            GetPlayerMessageData_IE(url, null, OnGetValue_, "methodConfig__");
        }

        public List<MethodConfig_> methiod = new List<MethodConfig_>();
        void OnGetValue_(string json, int status)
        {
            methiod.Clear();
            MethodConfig___ data = new MethodConfig___();
            data = JsonBase.DeserializeFromJson<MethodConfig___>(json.ToString());
            if (data.methodConfig__[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.methodConfig__[0].status);
                return;
            }
            for (int i = 0; i < data.methodConfig__.Count; i++)
            {
                for (int j = 0; j < data.methodConfig__[i].data.Count; j++)
                {
                    MethodConfig_ content = new MethodConfig_();
                    content.id = data.methodConfig__[i].data[j].id;
                    content.mode1 = data.methodConfig__[i].data[j].mode1;
                    content.mode2 = data.methodConfig__[i].data[j].mode2;
                    content.mode3 = data.methodConfig__[i].data[j].mode3;

                    content.num1 = data.methodConfig__[i].data[j].num1;
                    content.num2 = data.methodConfig__[i].data[j].num2;
                    content.num3 = data.methodConfig__[i].data[j].num3;

                    content.price1 = data.methodConfig__[i].data[j].price1;
                    content.price2 = data.methodConfig__[i].data[j].price2;
                    content.price3 = data.methodConfig__[i].data[j].price3;

                    //Debug.LogError("---------------------------------------");
                    //Debug.LogError("开放设置：" + data.methodConfig__[i].data[j].mode1);
                    //Debug.LogError("圈数：" + content.num1+"," + content.num2 + "," + content.num3);
                    //Debug.LogError("需要钱：" + content.price1 + "," + content.price2 + "," + content.price3);
                    methiod.Add(content);
                }
            }
            //Debug.LogError("开始往内部的复制");
            //Debug.LogError("---------------------------------------" + _dicMethodConfig.Count + "," + methiod.Count);

            foreach (var item in _dicMethodConfig)
            {
                for (int j = 0; j < methiod.Count; j++)
                {
                    if (item.Value.METHOD == methiod[j].id)
                    {
                        item.Value.type = methiod[j].mode1.ToString();
                        item.Value.sum = methiod[j].num1.ToString() + "_" + methiod[j].num2.ToString() + "_" + methiod[j].num3.ToString();
                        item.Value.pay = methiod[j].price1.ToString() + "_" + methiod[j].price2.ToString() + "_" + methiod[j].price3.ToString();
                    }
                }
            }


            //for (int i = 0; i < _dicMethodConfig.Count; i++)
            //{
            //    for (int j = 0; j < methiod.Count; j++)
            //    {
            //        if (_dicMethodConfig[i].METHOD == methiod[j].id)
            //        {
            //            _dicMethodConfig[i].type = methiod[j].mode1.ToString();
            //            _dicMethodConfig[i].sum = methiod[j].num1.ToString() + "_" + methiod[j].num2.ToString() + "_" + methiod[j].num3.ToString();
            //            _dicMethodConfig[i].pay = methiod[j].price1.ToString() + "_" + methiod[j].price2.ToString() + "_" + methiod[j].price3.ToString();
            //        }
            //    }
            //}

        }


        public const string _strjmTime = "hezuoTir";
        public const string _strGGTime = "guanfanggonTir";




        const string strLastDayFristTime = "_strDayFristTime";
        /// <summary>
        /// 判断是否是新的一天
        /// </summary>
        /// <returns></returns>
        public bool IsNewDay()
        {
            bool yesorno = false;
            string timeStr = null;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = Encoding.Default;
            DateTime time = System.DateTime.MinValue;
            try
            {
                string response = client.DownloadString(LobbyContants.WebTime);
                timeStr = response.Substring(2);
                time = startTime.AddMilliseconds(Convert.ToDouble(timeStr));
            }
            catch (Exception)
            {
                throw;
            }
            if (PlayerPrefs.HasKey(strLastDayFristTime) && !String.IsNullOrEmpty(PlayerPrefs.GetString(strLastDayFristTime)))
            {
                DateTime last_time = startTime.AddMilliseconds(Convert.ToDouble(PlayerPrefs.GetString(strLastDayFristTime)));
                if (time.Day > last_time.Day)
                {
                    yesorno = true;
                }
                else if (time.Day == 1 && last_time.DayOfYear == last_time.Day)
                {
                    yesorno = true;
                }
                else
                {
                    yesorno = false;
                }

            }
            else
            {
                yesorno = true;
            }
            Debug.LogWarning("+__保存时间" + timeStr);
            PlayerPrefs.SetString(strLastDayFristTime, timeStr);
            return yesorno;
        }
        #endregion
        #region 游戏通用方法
        public DateTime _DateTime = System.DateTime.MinValue;
        public const string fristMouthTime = "fristMouthTime";//是否有点击
        public bool flagNewTime = false;//是否跟新时间
        public void StartGetTime(Action action)
        {
            StartCoroutine(GetTime(action));
        }
        public IEnumerator GetTime(Action action)
        {
            string timeStr = null;
            WWW www = new WWW(LobbyContants.WebTime);
            yield return www;
            Debug.LogWarning("香港天文时间：" + www.text);
            timeStr = www.text.Substring(2);
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            _DateTime = startTime.AddMilliseconds(Convert.ToDouble(timeStr));
            flagNewTime = true;
            action();
        }

        /// <summary>
        /// 区别每天类型
        /// </summary>
        public enum EVERYDAY : uint
        {
            HEADIMG_UPDATE = 0,//头像更新
        }

        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(10, 10, 100, 40), "ceshi"))
        //    {
        //        Debug.LogError(MahjongCommonMethod.Instance.UnixTimeStampToDateTime(1509672853,0).Day);
        //    }
        //}

        /// <summary>
        /// 产生玩家掉落的道具
        /// </summary>
        /// <param name="pos_born">掉落位置</param>
        /// <param name="pos_target">结束位置</param>
        /// <param name="path">道具对应显示图片的路径</param>
        /// <param name="status">掉落状态，主要是用于判断不同的掉落情况进行不同的处理</param>
        /// <param name="itemNum">掉落道具的数量</param>
        public void SpwanDropItem(Vector3 pos_born, Vector3 pos_target, string name, int status, int itemNum, float radius, Transform parent)
        {
            int num = itemNum / 2;
            if (itemNum <= 0)
            {
                Debug.LogWarning("产生玩家掉落道具失败");
                return;
            }
            if (num > 6)
            {
                num = 6;
            }

            for (int i = 0; i < num; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("CommonPanel/DropItem")) as GameObject;
                go.transform.SetParent(parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                DropItem dropItem = go.GetComponent<DropItem>();
                Vector3 pos = Vector3.zero;
                if (num == 1)
                {
                    dropItem.Item.transform.localPosition = pos_born;
                }
                else
                {
                    Vector2 random = UnityEngine.Random.insideUnitCircle * radius;
                    pos = pos_born + new Vector3(random.x, random.y, 0);
                    dropItem.Item.transform.localPosition = pos;
                }

                dropItem.GetItemMessage(name, itemNum.ToString(), pos_target, pos);
            }
        }

        /// <summary>
        /// 获取指定的图片
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public Sprite GetAppointImage(string path)
        {
            Sprite image = Resources.Load<Sprite>(path);
            return image;
        }
        public Texture _icon;
        internal bool isDelateLoad;
        internal string serverID;
        #region 时间转换




        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public ulong DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
            return Convert.ToUInt64((dateTime - start).TotalSeconds);
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="timestamp">时间戳（秒）</param>
        /// <returns></returns>
        public DateTime UnixTimestampToDateTime(DateTime target, long timestamp)//暂时不用
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return start.AddSeconds(timestamp);
        }
        /// <summary>
        /// time_t转换成显示时间格式
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public DateTime UnixTimeStampToDateTime(double unixTimeStamp, int hour)
        {
            // 定义起始时间
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, hour, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// 获得当天时间第一秒
        /// </summary>
        public static DateTime FirstSecondTime(DateTime time)
        {
            DateTime Oldtime = time;
            DateTime NewTime = new DateTime();
            Oldtime = Oldtime.AddHours(-5);
            NewTime = new DateTime(Oldtime.Year, Oldtime.Month, Oldtime.Day, 0, 0, 0);
            return NewTime;
        }
        /// <summary>
        /// 判断不需要永久保存数据是否需要更新
        /// </summary>
        /// <param name="everyDayEnum">数据类型</param>
        /// <returns></returns>
        public bool IsNextDay(EVERYDAY everyDayEnum)
        {
            bool isNext = false;

            DateTime currentDateTime = DateTime.Now;
            if (PlayerPrefs.HasKey(SDKManager.Instance.szLastday + everyDayEnum))
            {
                ulong lastday = Convert.ToUInt64(PlayerPrefs.GetString(SDKManager.Instance.szLastday + everyDayEnum));
                ulong nowtime = DateTimeToUnixTimestamp(FirstSecondTime(currentDateTime));
                if (lastday < nowtime)
                {
                    PlayerPrefs.SetString(SDKManager.Instance.szLastday + everyDayEnum, DateTimeToUnixTimestamp(currentDateTime).ToString());
                    isNext = true;
                }
                else
                {
                    isNext = false;
                }
            }
            else
            {
                PlayerPrefs.SetString(SDKManager.Instance.szLastday + everyDayEnum, DateTimeToUnixTimestamp(currentDateTime).ToString());
                isNext = true;
            }
            return isNext;
        }
        public void DeletPlayerPrefs(EVERYDAY everyDayEnum)
        {
            if (PlayerPrefs.HasKey(SDKManager.Instance.szLastday + everyDayEnum))
            {
                PlayerPrefs.DeleteKey(SDKManager.Instance.szLastday + everyDayEnum);
            }
        }

        /// <summary>
        /// 现在时间显示时间格式转换成时间戳
        /// </summary>
        /// <returns></returns>
        public int GetCreatetime()
        {
            DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt32((DateTime.Now - DateStart).TotalSeconds);
        }

        #endregion 时间转换


        #region 获取玩家头像
        /// <summary>
        /// 获取玩家头像
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url">网址</param>
        /// <param name="status">1表示会检查本地图片，2表示不会检查本地图片直接从网页加载</param>
        public void GetPlayerAvatar(RawImage raw, string url = "123", Action<bool> action = null)
        {
            if (url == null || url.TrimEnd().Length < 20)
            {
                raw.texture = Resources.Load<Texture>("icon");
                return;
            }

            if (HeadImageManager._peopleHead.ContainsKey(url))
            {
                // Debug.LogError("=========================存在存在=======================");
                raw.texture = HeadImageManager._peopleHead[url];
                // Destroy(t2d);
            }
            else
            {
                //Debug.LogWarning("头像地址:" + url);
                if (url.TrimEnd().Length <= 30 || url == null)
                {
                    raw.texture = Resources.Load<Texture>("icon");
                    return;
                }
                StartCoroutine(LoadUrlTex(raw, url, action));
            }

        }
        public void GetWebImage(RawImage raw, string url, Action<bool> action = null)
        {
            Debug.LogWarning("图片地址:" + url);
            if (HeadImageManager._peopleHead.ContainsKey(url))
            {
                // Debug.LogError("=========================存在存在=======================");
                raw.texture = HeadImageManager._peopleHead[url];
                // Destroy(t2d);
            }
            else
            {
                StartCoroutine(LoadUrlTex(raw, url, action));
            }
        }
        /// <summary>
        /// 去网页下载相应图片显示出来
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Type">0：大厅面板 1玩家信息面板</param>
        /// <returns></returns>
        IEnumerator LoadUrlTex(UnityEngine.UI.RawImage raw, string url, Action<bool> action)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url + "?v=" + DateTime.Now.ToLongTimeString()))
            {
                yield return www.Send();
                if (www.isNetworkError)
                {
                    Debug.LogError("www.error:" + www.error);
                    raw.texture = Resources.Load<Texture>("icon");
                    if (action != null)
                    {
                        action(false);
                    }
                    yield break;
                }
                //while (!www.isDone)
                //{
                //    yield return 0;
                //}
                //yield return www;
                try
                {
                    raw.texture = DownloadHandlerTexture.GetContent(www);
                    HeadImageManager.SaveHeadImage(url, DownloadHandlerTexture.GetContent(www));
                    if (action != null)
                        action(true);
                    yield break;
                }
                catch (ApplicationException a)
                {
                    raw.texture = Resources.Load<Texture>("icon");
                    if (action != null)
                        action(false);
                    throw a;
                }
            }
            //   WWW www = new WWW(url + "?v=" + DateTime.Now.ToLongTimeString());
        }
        #endregion 获取玩家头像



        /// <summary>
        /// 产生大厅的通用提示框
        /// </summary>
        /// <param name="text"></param>
        public void ShowRemindFrame(string text, bool isgame = false)
        {
            if (isSpwanCommonBounce && CommonBoundce != null)
            {
                Destroy(CommonBoundce.gameObject);
            }
            if (text.Length < 1)
            {
                Debug.LogError("提示框内容错误");
                return;
            }
            GameObject go = Instantiate(Resources.Load<GameObject>("CommonPanel/BouncedCommon"));
            //if (isgame)
            //{
            //    go.transform.parent = MahjongGame_AH.GameData.Instance.UiCamera.transform;
            //}
            //else
            //{
            //    go.transform.parent = Camera.main.transform;
            //}

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.GetComponent<BouncedCommon>().Content.text = text;
            go.transform.GetComponent<BouncedCommon>().Bg.transform.localPosition = new Vector3(0, -Screen.height / 4f, 0);
            isSpwanCommonBounce = true;
            CommonBoundce = go.transform;
        }

        public bool isStartInit_Lobby; //是否已经开始初始化
        public bool isStartInit_Game;
        public bool isFinshSceneLoad; //是否完成场景加载
        public static int NetWorkStatus_int; //网络状态 记录上次网络状态，用于区分是否是切换网络  0表示无网  1表示流量 2表示wifi

        /// <summary>
        /// 获取当前网络状态
        /// </summary>
        /// <returns>0表示网络不可用，1表示当前在使用移动数据流量，2表示当前在使用wifi数据</returns>
        public int NetWorkStatus()
        {
            int status = -1;
            //判断如果玩家网络不可用，则弹出提示框
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //Debug.LogError("没有网络");
                SDKManager.Instance.IsDisConnect = true;
                status = 0;
            }

            //判断玩家是否使用移动网络
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                status = 2;
                //Debug.LogError("移动网络");
            }

            //判断玩家是否使用wifi
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                //Debug.LogError("wifi网络");
                status = 3;
            }
            return status;
        }

        public delegate void Deal(int status);

        public void DelayCheckNeteork(Deal dd)
        {
            StartCoroutine(CheckNeteork(dd));
        }

        IEnumerator CheckNeteork(Deal dd)
        {
            yield return new WaitForSeconds(1f);
            dd(NetWorkStatus());
        }

        /// <summary>
        /// 重连服务器
        /// </summary>
        public void SendConnectSever()
        {
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyGateway.Connect();
        }



        /// <summary>
        /// 获取一个物体下的子物体的数量
        /// </summary>
        /// <param name="go"></param>
        public int FindChildNum(GameObject go)
        {
            int num = 0;
            if (go == null)
            {
                return 0;
            }
            num = go.transform.childCount;
            return num;
        }

        /// <summary>
        /// 跳转到大厅初始场景，进行场景初始化
        /// </summary>
        public void InitScene(int type = 1)
        {
            if (isGameToLobby)
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载大厅资源，请稍候...");
            }

            //断开游戏/大厅服务器连接
            if (MahjongLobby_AH.Network.NetworkMgr.Instance)
            {
                MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyGateway.Disconnect();
                MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Disconnect();
            }

            if (MahjongGame_AH.Network.NetworkMgr.Instance)
            {
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Disconnect();
            }

            if (MahjongGame_AH.GameData.Instance)
            {
                MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.isBeginGame = false;
            }

            if (type == 1)
            {
                isLobbySendAuthenReq = true;
            }
            else
            {
                GameData.Instance.WXLoginPanelData.isBtnOk = false;
                GameData.Instance.WXLoginPanelData.isClickLogin = false;
            }

            //重新连接大厅服务器，重新登录
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connect();

            //MahjongLobby_AH.Scene.SceneManager.Instance.LoadScene(MahjongLobby_AH.Scene.ESCENE.MAHJONG_LOBBY_GENERAL);
        }



        public void RetryConnect_Lobby()
        {
            //断开游戏/大厅服务器连接
            if (MahjongLobby_AH.Network.NetworkMgr.Instance)
            {
                Debug.LogError("===============================4");
                MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Disconnect();
            }

            StartCoroutine(DelayConnectLobbySever());
        }

        IEnumerator DelayConnectLobbySever()
        {
            yield return new WaitForSeconds(1f);
            isLobbySendAuthenReq = true;
            GameData.Instance.WXLoginPanelData.isBtnOk = false;
            GameData.Instance.WXLoginPanelData.isClickLogin = false;
            //重新连接大厅服务器，重新登录
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connect();
        }
        /// <summary>
        /// 打印16进制
        /// </summary>
        /// <param name="bytes"></param>
        public void Printbuff(byte[] bytes)
        {
            string s = "";
            s += ("  长度: " + bytes.Length);
            s += "  数据:";
            for (int i = 0; i < bytes.Length; ++i)
            {
                s += (" " + bytes[i].ToString("X2"));
            }
            Debug.Log(s);
        }


        /// <summary>
        /// 复制内容到剪切板
        /// </summary>
        /// <param name="str"></param>
        public void CopyString(string str)
        {
#if UNITY_EDITOR
            TextEditor text = new TextEditor();
            text.text = str;
            text.OnFocus();
            text.Copy();
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
        AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
        //复制文字到剪切板
        jo.Call("copyTextToClipboard",jo, str);
#endif
#if UNITY_IPHONE
        _CopyTextToClipboard(str);
#endif
        }
        [DllImport("__Internal")]
        private static extern void _CopyTextToClipboard(string text);
        /// <summary>
        /// 复制resources中的某个物体
        /// </summary>
        /// <param name="path"></param>
        public GameObject SpwanObjectToName(string path)
        {
            GameObject go = null;
            if (path == null)
            {
                return null;
            }
            go = Instantiate(Resources.Load<GameObject>(path));
            return go;
        }



        public MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioType GetAudioSourceIndex(byte cTitle)
        {
            int index = -1;
            int num = cTitle / 16;
            int num2 = cTitle % 16;
            if (num == 1 || num == 2 || num == 3)
            {
                index = num2 + num * 9 - 10;
            }
            if (num == 4)
            {
                index = 26 + num2;
            }
            if (num == 5)
            {
                index = 30 + num2;
            }
            if (num == 6)
            {

            }
            //Debug.LogError("s____" + index);
            return (MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioType)index;
        }
        public bool isPush = false;
        //获取网页的json数据
        public delegate void CallBack_Url(string str, int status);

        /// <summary>
        /// 获取网页的对应数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="value">网址要传的值</param>
        /// <param name="callBack">获取之后的回调方法</param>
        /// <param name="title">读取json数据时的json文件中的头</param>
        /// <param name="status">状态  4表示使用unity自带的方式读取json数据</param>
        /// <returns></returns>
        public IEnumerator GetPlayerMessageData(string url, Dictionary<string, string> value, CallBack_Url callBack, string title, int status)
        {
            url += "?";
            WWWForm wwf = new WWWForm();
            if (value != null)
            {
                var cachingIter = value.GetEnumerator();
                while (cachingIter.MoveNext())
                {
                    var curUrlPamaName = cachingIter.Current.Key;
                    wwf.AddField(curUrlPamaName, value[curUrlPamaName]);
                    if (!isPush)
                    {
                        url += curUrlPamaName + "=" + value[curUrlPamaName] + "&";
                    }
                }
            }
            using (UnityWebRequest www = isPush ? UnityWebRequest.Post(url, wwf) : UnityWebRequest.Get(url))
            {
                Debug.LogWarning("GetPlayerMessageDataurl" + url);
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    //Debug.LogError(www.url);
                    Debug.LogError("网页获取数据错误:" + www.error + ",:" + www.url);
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    yield break;
                }
                else
                {
                    StringBuilder str = new StringBuilder();

                    if (status > 4)
                    {
                        str.Append(www.downloadHandler.text);
                    }
                    else
                    {
                        str.Append("{\"");
                        str.Append(title);
                        str.Append("\":[");
                        str.Append(www.downloadHandler.text);
                        str.Append("]}");
                    }
                    Debug.Log("str:" + str.ToString());
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                    callBack(str.ToString(), status);
                }
                isPush = false;
            }
        }

        /// <summary>
        /// 获取网页的对应数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="value">网址要传的值</param>
        /// <param name="callBack">获取之后的回调方法</param>
        /// <returns></returns>
        public void GetPlayerMessageData_IE(string url, Dictionary<string, string> value, CallBack_Url callBack, string title, int status = 0)
        {
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "玩命加载中...");
            StartCoroutine(GetPlayerMessageData(url, value, callBack, title, status));
        }
        /// <summary>
        /// 获取streamAssets下的json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerator GetLocalJson<T>(T t, Action<T> action)
        {
            string url = "";

#if UNITY_EDITOR || UNITY_ANDROID
            url = Application.streamingAssetsPath + "/" + t.ToString().Split('+')[1] + ".json";
            //url =  "jar:file://" + Application.dataPath + "!/assets/" + t.ToString().Split('+')[1] + ".json";
#elif UNITY_IPHONE || UNITY_IOS
        url = Application.dataPath + "/Raw/"+t.ToString().Split('+')[1] + ".json";
#endif
            // path = Application.streamingAssetsPath + "/ShortTalk.txt";
            if (url.Contains("://"))
            {
                WWW www = new WWW(url);
                while (!www.isDone)
                {
                    yield return www;
                }
                t = JsonUtility.FromJson<T>(www.text);
            }
            else
            {
                string str = File.ReadAllText(url);
                t = JsonUtility.FromJson<T>(str);
            }
            action(t);
            yield return null;
        }

        /// <summary>
        /// 从网页获取json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="t"></param>
        /// <param name="action">a==t  b==返回字符串</param>
        public void GetUrlJson<T>(string url, T t, Action<T, string> action)
        {
            StartCoroutine(IEGetUrlJson(url, t, action));
            // return t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="t"> t="" 提交数据  t!=""获取数据</param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator IEGetUrlJson<T>(string url, T t, Action<T, string> action)
        {
            //if (isPut )
            //{
            //    action(t,"1");
            //   yield  break ;
            //}
            for (int i = 0; i < 3; i++)
            {
                // WWW www = new WWW(url, Encoding.UTF8.GetBytes(JsonUtility.ToJson(t)));
                WWW www = new WWW(url);
                //Debug.LogError(url);
                while (!www.isDone)
                {
                    yield return null;
                }
                if (www.text.Length == 0)
                {
                    Debug.LogWarning("获取或提交网页出错" + www.error + "www.text:" + www.text.Length);
                    yield return new WaitForSeconds(1.5f);
                    continue;
                }
                else
                {
                    if (t != null)
                    {
                        if (typeof(T).ToString() != "System.String")
                        {
                            //Debug.LogWarning("-----网页接口数据" + www.text);
                            t = JsonUtility.FromJson<T>(www.text);
                        }
                        action(t, www.text);
                    }
                    else
                    {
                        if (action != null)
                        {
                            Debug.LogWarning("IEGetUrlJson_url:" + url);
                            action(t, www.text);
                        }

                    }
                    yield break;
                }
            }
            MahjongCommonMethod.instance.ShowRemindFrame("数据获取出错，请检查网络重试");
        }
        /// <summary>
        /// 产生代开房间的预置体界面
        /// </summary>
        /// <param name="roomInfo"></param>
        public void SpwanInsteadRoomPanel(MahjongLobby_AH.Data.InsteadOpenRoomPanelData.RoomInfo roomInfo)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/RoomMessage"));
            if (go == null)
            {
                return;
            }

            go.transform.SetParent(MahjongLobby_AH.UIMainView.Instance.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            RoomMessagePanel room = go.GetComponent<RoomMessagePanel>();
            //更新该面板的显示数据
            room.roomInfo = roomInfo;
        }

        /// <summary>
        /// 获取经纬度
        /// </summary>
        public void GetPlayerJingWeiNum()
        {
            //获取用户的经纬度数据
            if (Application.isEditor)
            {
                return;
            }
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
            Debug.LogWarning("调用玩家定位方法");
            jo.Call("GetPlayerPos");
#endif
        }

        /// <summary>
        /// 重复登录之后玩家的再次请求玩家认证请求
        /// </summary>
        IEnumerator DelayLogin(float timer)
        {

            yield return new WaitForSeconds(timer);
            if (iAutnehType == 4)
            {
                GameData.Instance.WXLoginPanelData.isClickLogin = false;
                UIMainView.Instance.WXLoginPanel.BtnGusetLogin();
            }
            else
            {
                if (PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType))//自动跳转
                {
                    GameData.Instance.PlayerNodeDef.isSencondTime = 2;
                    Debug.LogError("玩家认证类型:===================3");
                    //判断是否过期              
                    SDKManager.Instance.GetRefreshToken(PlayerPrefs.GetString(SDKManager.Instance.szrefresh_token));
                }
            }
        }

        /// <summary>
        /// 重复登录之后玩家的再次请求玩家认证请求
        /// </summary>
        public void AgainLogin(float timer)
        {
            SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");//加载loading画面---认证回应
            StartCoroutine(DelayLogin(timer));
        }
        #region 打点相关功能
        public enum StateType : int
        {
            CheckVersion = 100001,
            WXLogin,
            LoginSuccess,
            CompeletChoiceMap,
            NerPlayerGift,
            CloseActivity = 100006,
            CreatRoomSucc,
            JoinRoomSucc,
            CompeletOne,
            CompeletTwo,
            CompeletThr = 100011,
            CompeletFiv,
            CompeletTen,
            ClickCharge,
            ClickShareGift,
            ClickJoin = 100016,
            ClickRealName,
            ClickHistory,
            ClickActivity,
            ClickProxyCenter,
            ClickProxy = 100021,
            ClickKeFu,
            ClickShare,
            ClickMessage,
            ClickPlayMethord,
            ClickHead = 100026,
            ClickCreateRoom,
            ClickJoinRoom,
            CompeleGetShareGift







        }

        public class State_Struct
        {
            internal int status;
            internal long userAction;
        }
        public HeadData headData = new HeadData();
        public class HeadData
        {
            public List<State_Struct> ListStateData = new List<State_Struct>();
        }
        string _url = "UserAction.x?uid={0}&id={1}";

        IEnumerator GetState_Joson(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("请求错误" + www.error);
                yield break;
            }
            StringBuilder str = new StringBuilder();
            if (headData.ListStateData.Count > 0)
            {
                headData.ListStateData.RemoveAt(0);
            }
            //Debug.LogWarning((int)StateType.CreatRoomSucc);
            //Debug.LogWarning(www.text);
            str.Append("{\"ListStateData\":["); str.Append(www.text); str.Append("]}");
            headData = JsonBase.DeserializeFromJson<HeadData>(str.ToString());
            if (headData.ListStateData[0].status != 1)
            {
                headData.ListStateData.RemoveAt(0);
            }
        }
        //是否点击过
        public void HasClicked(int id)
        {
            if (instance.iUserid == 0) return;
            string url = string.Format(LobbyContants.MAJONG_PORT_URL + _url, instance.iUserid, id);
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = string.Format(LobbyContants.MAJONG_PORT_URL_T + _url, instance.iUserid, id);
            }
            //Debug.LogError(url);
            bool aa = false;
            if (headData.ListStateData.Count > 0)
            {
                long bb = 1 << (id % 100000 - 1);
                Debug.LogError(headData.ListStateData[0].userAction);
                Debug.LogWarning(bb);
                aa = (headData.ListStateData[0].userAction & bb) == bb ? true : false;
                if (aa) StartCoroutine(GetState_Joson(url));
            }
            else
            {
                StartCoroutine(GetState_Joson(url));
            }
        }
        #endregion 打点相关功能

        #endregion 游戏通用方法


        #region 获取指定麻将馆的红包信息
        public MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBag_[] parlorRedBagInfo = new MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBag_[4]; //保存四个馆的红包信息 按照上边麻将馆的顺序添加
        public int iParlorId;  //进入游戏的麻将馆id
        public bool isGetReaBag; //是否已经领取过麻将馆红包   

        public delegate void GetNowtimerDeleg(int id, int timer);
        public int TimerNow = 0;  //当前时间戳

        public void GetNowTimer(int id, GetNowtimerDeleg action)
        {
            StartCoroutine(GetTimer(id, action));
        }

        IEnumerator GetTimer(int id, GetNowtimerDeleg action)
        {
            WWW www = new WWW(LobbyContants.WebTime);
            yield return www;
            if (www.text.Length >= 15)
            {
                TimerNow = Convert.ToInt32(www.text.Substring(2, 10));
                action(id, TimerNow);
            }
        }

        //拉起安卓设置界面
        public void SettingPanel()
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity"))
            {
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                {
                    jo.Call("SetingPanel");
                }
            }
        }
        #endregion

        #region 网站接口加密

        //string test = "wenda";

        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(10, 10, 100, 40), "123"))
        //    {
        //        Debug.LogError("encode:" + GetUrlEncrypt("https://sxt.ibluejoy.com/m/turn.html", 10001649));
        //    }
        //}

        //加密密钥
        public const string Key_Url = "39a6ed8d6f3ce520d9f85071f0329225";


        /// <summary>
        /// 获取加密后的网址链接
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetUrlEncrypt(string URL, int userid)
        {
            string key = "{uid:" + userid + "}";
            StringBuilder str = new StringBuilder();
            str.Append(URL);
            str.Append("?nt_data=");
            string temp_0 = encode(key, Key_Url);
            //Debug.LogError("加密后：" + temp_0);
            str.Append(temp_0);
            str.Append("&sign=");
            //计算签名
            string temp_1 = sign("nt_data=" + temp_0);
            //Debug.LogError("sign算法:" + temp_1);
            string temp_2 = encode(temp_1, Key_Url);
            //Debug.LogError("加密签名	:" + temp_2);
            str.Append(temp_2);
            //Debug.LogError("最终网址	:" + str.ToString());
            return str.ToString();
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="src">加密前的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的字符串</returns>
        public string encode(string src, string key)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                int n = (0xff & src[i]) + (0xff & key[i % key.Length]);
                sb.Append("@" + n);
            }
            return sb.ToString();
        }

        int[,] shufflePos = { { 1, 13 }, { 5, 17 }, { 7, 23 } };

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public string sign(string src)
        {
            string md5 = GetMD5WithString(src);
            return shuffleSign(md5);
        }


        public string GetMD5WithString(string sDataIn)
        {
            string str = "";
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(sDataIn);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                str += bytes[i].ToString("x2");
            }
            return str;
        }

        /// <summary>
        /// 打乱签名
        /// </summary>
        /// <param name="src">打乱前的签名</param>
        /// <returns>打乱后的签名</returns>
        public string shuffleSign(string src)
        {
            if (src.Length == 0)
            {
                return src;
            }
            char[] bytes = new char[src.Length];

            for (int i = 0; i < src.Length; i++)
            {
                bytes[i] = src[i];
            }

            char temp;
            for (int i = 0; i < 3; i++)
            {
                temp = bytes[shufflePos[i, 0]];
                bytes[shufflePos[i, 0]] = bytes[shufflePos[i, 1]];
                bytes[shufflePos[i, 1]] = temp;
            }

            string str = null;

            for (int i = 0; i < bytes.Length; i++)
            {
                str += bytes[i];
            }
            return str;
        }
        #endregion

        #region 间隔指定时间重连指定服务器
        public static bool isIntivateDisConnct; //是否主动断开游戏服务器
        public const float timer_sever = 3f;


        IEnumerator temp;
        public void StopRetryConnectSever()
        {
            if (temp != null)
            {
                StopCoroutine(temp);
            }
        }


        public void RetryConnectSever(int type_sev)
        {
            temp = DelayConnect(timer_sever, type_sev);
            StartCoroutine(temp);
        }

        /// <summary>
        /// 延迟链接服务器
        /// </summary>
        /// <param name="timer">延迟时间</param>
        /// <param name="type">1表示大厅网关 2大厅服务器 3游戏服务器</param>
        /// <returns></returns>
        IEnumerator DelayConnect(float timer, int type)
        {
            yield return new WaitForSeconds(timer);
            switch (type)
            {
                case 1:
                    MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyGateway.Connect();
                    break;
                case 2:
                    MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connect();
                    break;
                case 3:
                    MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Connect();
                    break;
            }
        }
        #endregion

        #region 根据methodID获取房间的配置写法

        public void GetWeiteInfoForMethodID(StringBuilder discription, int MethodId, uint[] roomMessage_)
        {
            MahjongCommonMethod.instance.ShowParamOfOpenRoom(ref discription, roomMessage_, 0, MahjongCommonMethod.Instance.iPlayingMethod);
            #region OldCode
            // switch (MethodId)
            //{
            //    case 1:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("十三不靠 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("明杠收3家 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("四杠荒庄 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("一炮多响 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        break;
            //    case 2:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("明杠收3家 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("点炮三分 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("一炮多响 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        if (roomMessage_[7] == 0) discription.Append("");
            //        else if (roomMessage_[7] == 1) discription.Append("可连庄 ");

            //        break;
            //    case 3:
            //        if (roomMessage_[0] == 0) discription.Append("10分 ");
            //        else if (roomMessage_[0] == 1) discription.Append("20分 ");
            //        else discription.Append("30分 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("明杠收3家 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        break;
            //    case 4:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        break;
            //    case 5:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("可连庄 ");

            //        break;
            //    case 6:
            //        if (roomMessage_[0] == 0) discription.Append("30分 ");
            //        else if (roomMessage_[0] == 1) discription.Append("60分 ");
            //        else discription.Append("90分 ");
            //        break;

            //    case 7:
            //        if (roomMessage_[0] == 0) discription.Append("8局 ");
            //        else if (roomMessage_[0] == 1) discription.Append("16局 ");
            //        else discription.Append("24局 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("自摸翻倍 ");
            //        break;

            //    case 8:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("十三不靠 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("明杠收3家 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else discription.Append("四杠荒庄 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("可连庄 ");

            //        break;
            //    case 9:

            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("多人支付 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("可连庄 ");
            //        break;
            //    case 10:
            //        if (roomMessage_[0] == 0) discription.Append("8局 ");
            //        else if (roomMessage_[0] == 1) discription.Append("16局 ");
            //        else discription.Append("24局 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("一炮多响 ");
            //        break;
            //    case 11:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("前抬 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("后和 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else discription.Append("可坐庄 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("能跑能下 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("放几出几 ");

            //        if (roomMessage_[7] == 0) discription.Append("");
            //        else if (roomMessage_[7] == 1) discription.Append("可连庄 ");
            //        break;
            //    case 12:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else discription.Append("可连庄 ");

            //        break;
            //    case 13:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else discription.Append("前抬 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else discription.Append("后和 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("翻倍 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("可连庄 ");

            //        break;
            //    case 14:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("十三幺 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("可胡七对 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("可连庄 ");

            //        break;
            //    case 15:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("轮庄 ");
            //        else if (roomMessage_[2] == 1) discription.Append("连庄 ");
            //        break;
            //    case 16:
            //        if (roomMessage_[0] == 0) discription.Append("4局 ");
            //        else if (roomMessage_[0] == 1) discription.Append("8局 ");
            //        else discription.Append("12局 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("杠收三家 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("点炮三分 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("一炮多响 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("前抬 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("后和 ");
            //        break;
            //    case 17:
            //        if (roomMessage_[0] >= 3)
            //        {
            //            if (roomMessage_[0] == 3) discription.Append("4局 ");
            //            else if (roomMessage_[0] == 4) discription.Append("8局 ");
            //            else discription.Append("12局 ");
            //        }
            //        else
            //        {
            //            if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //            else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //            else discription.Append("3圈 ");
            //        }

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else discription.Append("带庄 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("杠收三家 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("点炮三分 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("一炮多响 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("前抬 ");

            //        if (roomMessage_[7] == 0) discription.Append("");
            //        else if (roomMessage_[7] == 1) discription.Append("后和 ");

            //        if (roomMessage_[8] == 0) discription.Append("");
            //        else if (roomMessage_[8] == 1) discription.Append("抢庄 ");

            //        if (roomMessage_[9] == 0) discription.Append("");
            //        else if (roomMessage_[9] == 1) discription.Append("杠随胡走 ");

            //        if (roomMessage_[10] == 0) discription.Append("");
            //        else if (roomMessage_[10] == 1) discription.Append("仅自摸 ");
            //        break;
            //    case 18:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("底分2分 ");
            //        else discription.Append("底分5分 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("七对 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("可吃 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("前台 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("后和 ");
            //        break;
            //    case 19:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("底分2分 ");
            //        else discription.Append("底分5分 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("七对 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("一炮多响 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("前台 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("后和 ");
            //        break;
            //    case 20:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("底分2分 ");
            //        else discription.Append("底分5分 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("七对 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("前台 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("后和 ");
            //        break;
            //    case 21:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("七对 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("前台 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("后和 ");
            //        break;
            //    case 22:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("");
            //        else if (roomMessage_[1] == 1) discription.Append("底分2分 ");
            //        else discription.Append("底分5分 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("带字牌 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("七对 ");

            //        if (roomMessage_[4] == 0) discription.Append("");
            //        else if (roomMessage_[4] == 1) discription.Append("可吃牌 ");

            //        if (roomMessage_[5] == 0) discription.Append("");
            //        else if (roomMessage_[5] == 1) discription.Append("前台 ");

            //        if (roomMessage_[6] == 0) discription.Append("");
            //        else if (roomMessage_[6] == 1) discription.Append("后和 ");
            //        break;
            //    case 23:
            //        if (roomMessage_[0] == 0) discription.Append("1圈 ");
            //        else if (roomMessage_[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomMessage_[1] == 0) discription.Append("不逼金 ");
            //        else if (roomMessage_[1] == 1) discription.Append("逼头金 ");
            //        else discription.Append("逼二金 ");

            //        if (roomMessage_[2] == 0) discription.Append("");
            //        else if (roomMessage_[2] == 1) discription.Append("暗杠可见 ");

            //        if (roomMessage_[3] == 0) discription.Append("");
            //        else if (roomMessage_[3] == 1) discription.Append("可吃牌 ");
            //        break;
            //    default:

            //        break;
            //}
            #endregion

        }

        #endregion

        #region 获取手机号
        public void SendMobileToWeb(int type)
        {
            StringBuilder str = new StringBuilder();
            str.Append(LobbyContants.MAJONG_PORT_URL + "mobile.x?");
            str.Append("uid=");
            if (type == 0)
            {
                str.Append(MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId);
            }
            else
            {
                str.Append(MahjongGame_AH.GameData.Instance.PlayerNodeDef.iUserId);
            }
            str.Append("&type=");
            str.Append(type);
            str.Append("&phone=");
            str.Append(SystemInfo.deviceModel);
            Debug.LogWarning("手机型号信息:" + str.ToString());
            StartCoroutine(str.ToString());
        }

        IEnumerator SendMobile(string url)
        {
            UnityEngine.Networking.UnityWebRequest requst = new UnityEngine.Networking.UnityWebRequest(url);
            requst.timeout = 5;  //超时时间5s
            yield return requst.Send();
            if (requst.isNetworkError)
            {
                Debug.LogError("发送设备信息失败:" + requst.error);
            }
        }

        #endregion

        #region 跳转回放场景
        public void SkipPlayBack_()
        {
            PlayBack_1.PlayBackData.isComePlayBack = true;
            //先关闭预加载的游戏场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", LoadSceneMode.Additive);
            StartCoroutine(DelScene());
            StartCoroutine(DelScene_2());
        }

        IEnumerator DelScene()
        {
            yield return new WaitForSeconds(0.5f);
            Scene temp = UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_GAME_GENERAL.ToString());
            if (temp != null && temp.isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_GAME_GENERAL.ToString());
                yield break;
            }
            else
            {
                StartCoroutine(DelScene());
            }
        }

        IEnumerator DelScene_2()
        {
            yield return new WaitForSeconds(0.5f);
            Scene temp_1 = UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_GAME_MAIN_SCENE.ToString());
            if (temp_1 != null && temp_1.isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_GAME_MAIN_SCENE.ToString());
                yield break;
            }
            else
            {
                StartCoroutine(DelScene_2());
            }
        }
        #endregion

        #region 测试代码   
        public void SendSitRes()
        {
            StartCoroutine(DelaySendSitreq());
        }

        IEnumerator DelaySendSitreq()
        {
            yield return new WaitForSeconds(10f);
            //玩家认证回应消息成功之后，发送玩家入座消息
            MahjongGame_AH.Network.Message.NetMsg.ClientSitReq sit = new MahjongGame_AH.Network.Message.NetMsg.ClientSitReq();
            sit.iRoomNum = System.Convert.ToInt32(MahjongCommonMethod.Instance.RoomId);
            sit.sTableNum = (ushort)MahjongCommonMethod.Instance.iTableNum;
            sit.iUserId = MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId;
            sit.iParlorId = MahjongCommonMethod.Instance.iParlorId;
            MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.SendSitReq(sit);
        }


        //void OnGUI()
        //{
        //    if(GUI.Button(new Rect(10,10,100,40),"load"))
        //    {
        //        MahjongGame_AH.Scene.SceneManager.Instance.PreloadScene(MahjongGame_AH.Scene.ESCENE.MAHJONG_GAME_GENERAL);
        //    }

        //    if (GUI.Button(new Rect(10, 55, 100, 40), "show"))
        //    {
        //        MahjongGame_AH.Scene.SceneManager.Instance.OpenPointScene(MahjongGame_AH.Scene.ESCENE.MAHJONG_GAME_GENERAL);
        //    }
        //}


        #endregion

        public static Color AlphaBlend(Color A, Color B)
        {
            Color C = new Color();
            C.r = (1 - B.a) * A.r + B.a * B.r;
            C.g = (1 - B.a) * A.g + B.a * B.g;
            C.b = (1 - B.a) * A.b + B.a * B.b;
            C.a = 1;
            return C;
        }


        /// <summary>
        /// 如果服务器找不到玩家节点时，重新认证
        /// </summary>
        /// <param name="iError"></param>
        public void SendAuthenReq(int iError)
        {
            if (iError != -1 && iError != -2)
            {
                return;
            }

            GameData gd = GameData.Instance;
            //发送第三种方式认证请求        
            MahjongLobby_AH.Network.Message.NetMsg.ClientAuthenReq msg = new MahjongLobby_AH.Network.Message.NetMsg.ClientAuthenReq();
            msg.wVer = LobbyContants.SeverVersion;
            msg.iAuthenType = 3;
            iAutnehType = 3;
            msg.szToken = accessToken;
            msg.szDui = SystemInfo.deviceUniqueIdentifier;
            msg.szIp = PlayerIp;
            msg.iUserId = iUserid;
            msg.fLatitude = fLatitude;
            msg.fLongitude = fLongitude;
            msg.szAddress = sPlayerAddress;
            msg.iRegistSource = LobbyContants.iChannelVersion;
            msg.szRegistMac = MacId;
            msg.REGISTRATION_ID = SDKManager.Instance.GetRegistID();
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.SendAuthenReq(msg);
        }

        /// <summary>
        /// 按列写入数据到二位数组
        /// </summary>
        /// <param name="_int2int">被写入的数组</param>
        /// <param name="column">第几列</param>
        /// <param name="value">写入的值</param>
        /// <param name="totalRow">总共多少行</param>
        /// <returns></returns>
        public void WriteColumnValue(ref uint[] _int2int, int column, sbyte value, int totalRow)
        {
            for (int i = totalRow - 1; i >= 0; i--)
            {
                if ((value & (0x01 << i)) > 0)//左移
                {
                    _int2int[RowColumn2Index(i, totalRow, column)] = _int2int[RowColumn2Index(i, totalRow, column)] | (0x80000000 >> ((column % 32)));//置一
                }
                else
                {
                    _int2int[RowColumn2Index(i, totalRow, column)] = _int2int[RowColumn2Index(i, totalRow, column)] & ~(0x80000000 >> ((column % 32)));//置零
                }
            }
        }
        /// <summary>
        ///将二维数组按列读取 
        /// </summary>
        /// <param name="_int2int"></param>
        /// <param name="totalRow"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public int ReadColumnValue(uint[] _int2int, int totalRow, int column)
        {
            int num = 0;
            for (int i = totalRow - 1; i >= 0; i--)
            {
                if ((_int2int[RowColumn2Index(i, totalRow, column)] & (0x80000000 >> ((column % 32)))) > 0)
                {
                    num = (num << 1) + 1;
                    // Debug.Log("true" + num);
                }
                else
                {
                    num = num << 1;
                    // Debug.Log("fale:" + num);
                }
            }
            return num;
        }
        int ReadChoicedRule(int column)
        {
            return ReadColumnValue(GameData.Instance.CreatRoomMessagePanelData.roomMessage_, 2, column);
        }
        uint ReadChoicedRule1(int index, int column)
        {
            return ReadInt32toInt4(GameData.Instance.CreatRoomMessagePanelData.roomMessage_[index], column);
        }
        /// <summary>
        /// 根据行列返回数组下标
        /// </summary>
        /// <param name="i"></param>
        /// <param name="Row">总行数</param>
        /// <param name="comlum">对应列</param>
        /// <returns></returns>
        int RowColumn2Index(int i, int Row, int comlum)
        {
            int index = 0;
            index = i + ((comlum) / 32) * Row;
            // Debug.LogWarning("index" + index);
            return index;

        }
        /// <summary>
        /// 计算对应列开始后4位值
        /// </summary>
        /// <param name="num">输入取出的int值</param>
        /// <param name="index">第几列</param>
        /// <returns></returns>
        public uint ReadInt32toInt4(uint num, int index)
        {
            uint reValue = 0;
            reValue = (num & (0xf0000000 >> index)) >> (28 - index);
            return reValue;
        }
        /// <summary>
        /// 赋值对应列开始后4位值
        /// </summary>
        /// <param name="resouce"></param>
        /// <param name="value">赋值</param>
        /// <param name="volumn">第几列</param>
        /// <returns></returns>
        public void WriteInt32toInt4(ref uint resouce, uint value, int volumn)
        {
            int destination = (volumn) / 4;
            resouce = (resouce & ~((uint)0xf << ((7 - destination) * 4))) | (value << ((7 - destination) * 4));
        }
        /// <summary>
        /// 显示玩法
        /// </summary>
        /// <param name="discription"></param>
        /// <param name="parma"></param>
        /// <param name="isOnlyshow">0 不加金币显示 1 加入金币显示</param>
        public void ShowParamOfOpenRoom(ref StringBuilder discription, uint[] parma, byte isOnlyCoin, int iPlayingMethod)
        {
            Debug.LogFormat("{0},{1},{2},{3}", parma[0].ToString("X8"), parma[1].ToString("X8"), parma[2].ToString("X8"), parma[3].ToString("X8"));
            string[] str_0 = _dicMethodConfig[iPlayingMethod].sum.Split('_');
            int rowNum = 2;
            uint prarmPrice = ReadInt32toInt4(parma[1], 16);
            int isAA = ReadColumnValue(parma, rowNum, 39);
            if (iPlayingMethod != 20015)
            {
                if (Int32.Parse(_dicMethodConfig[iPlayingMethod].type) == 1)
                {
                    discription.Append(str_0[prarmPrice] + "圈 ");
                }
                else if (Int32.Parse(_dicMethodConfig[iPlayingMethod].type) == 2)
                {
                    discription.Append(str_0[prarmPrice] + "局 ");
                }
            }
            else//特殊处理既有局又有圈
            {
                switch (prarmPrice)
                {
                    case 0:
                    case 1:
                    case 2:
                        discription.Append(Int32.Parse(str_0[prarmPrice]));
                        discription.Append(ReadColumnValue(parma, rowNum, 4) > 0 ? "局 " : "圈 ");
                        if (isOnlyCoin == 1)
                        {
                            if (isAA <= 1)
                                discription.Append(UIMainView.Instance.CreatRoomMessagePanel.Method_pay[prarmPrice] / 4 + "金币 ");
                            else
                                discription.Append(UIMainView.Instance.CreatRoomMessagePanel.Method_pay[prarmPrice] + "金币 ");
                        }
                        break;
                    case 3:
                    case 4:
                    case 5:
                        discription.Append(Int32.Parse(str_0[prarmPrice - 3]) * 4);
                        discription.Append(ReadColumnValue(parma, 2, 4) > 0 ? "局 " : "圈 ");
                        if (isOnlyCoin == 1)
                        {
                            if (isAA <= 1)
                                discription.Append(UIMainView.Instance.CreatRoomMessagePanel.Method_pay[prarmPrice - 3] / 4 + "金币 ");
                            else
                                discription.Append(UIMainView.Instance.CreatRoomMessagePanel.Method_pay[prarmPrice - 3] + "金币 ");
                        }
                        break;
                    default:
                        break;
                }
            }
            if (ReadColumnValue(parma, rowNum, 39) <= 1)
                discription.Append("四家支付 ");
            else if (ReadColumnValue(parma, rowNum, 39) == 2)
                discription.Append("房主支付 ");
            //discription.Append(ReadColumnValue(parma, rowNum, 5) > 0 ? "十三幺 " : "");       
            //discription.Append(ReadColumnValue(parma, rowNum, 7) > 0 ? "豪华七对 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 8) > 0 ? "十三不靠 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 9) > 0 ? "杠收三家 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 10) > 0 ? "四杠荒庄 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 11) > 0 ? "自摸翻倍 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 13) > 0 ? "可前抬 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 14) > 0 ? "可后和 " : "");
            //discription.Append(ReadColumnValue(parma, rowNum, 20) > 0 ? "接炮一家 " : "");
            switch (iPlayingMethod)
            {
                case 20001:
                    discription.Append(ReadColumnValue(parma, rowNum, 6) > 0 ? "七对 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 40) > 0 ? "抢杠全包 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 37) > 0 ? "荒庄荒杠 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 36) > 0 ? "抢杠无红中 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 28) > 0 ? "红中算码 " : "");
                    if (ReadColumnValue(parma, rowNum, 38) <= 1)
                        discription.Append("原版计分 ");
                    else if (ReadColumnValue(parma, rowNum, 38) == 2)
                        discription.Append("新版小分 ");
                    if (ReadInt32toInt4(parma[0], 16) > 0)
                        discription.Append("扎" + ReadInt32toInt4(parma[0], 16) + "码 ");
                    else
                        discription.Append("无码");
                    break;
                case 20015:
                    discription.Append(ReadColumnValue(parma, rowNum, 30) > 0 ? "缺门 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 31) > 0 ? "坎 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 32) > 0 ? "见面 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 33) > 0 ? "幺九将 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 34) > 0 ? "六连 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 35) > 0 ? "断独幺 " : "");
                    discription.Append(ReadColumnValue(parma, rowNum, 42) > 0 ? "点数 " : "");
                    break;
                case 20016:
                    switch (ReadColumnValue(parma, rowNum, 25))
                    {
                        case 1:
                            discription.Append("1嘴 ");
                            break;
                        case 2:
                            discription.Append("5嘴 ");
                            break;
                        case 3:
                            ;
                            discription.Append("10嘴 ");
                            break;
                        default:
                            break;
                    }
                    if (ReadColumnValue(parma, rowNum, 41) <= 1)
                        discription.Append("平胡 ");
                    else if (ReadColumnValue(parma, rowNum, 41) == 2)
                        discription.Append("大胡 ");
                    discription.Append(ReadColumnValue(parma, rowNum, 12) > 0 ? "一炮多响 " : "");
                    break;
                case 20017:
                    discription.Append(ReadColumnValue(parma, rowNum, 15) > 0 ? "接炮三家 " : "接炮一家 ");
                    discription.Append(ReadColumnValue(parma, rowNum, 45) > 0 ? "两番起胡 " : "一番起胡 ");
                    break;
                case 20018:
                    switch (ReadColumnValue(parma, rowNum, 43))
                    {
                        case 0:
                            discription.Append("不带配子 ");
                            break;
                        case 1:
                            discription.Append("带配子 ");
                            break;
                        case 2:
                            discription.Append("白板配子 ");
                            break;
                        default:
                            break;
                    }
                    switch (ReadColumnValue(parma, rowNum, 44))
                    {
                        case 0:
                            discription.Append("字牌30番 ");
                            break;
                        case 1:
                            discription.Append("字牌30番 ");
                            break;
                        case 2:
                            discription.Append("字牌30番 ");
                            break;
                        default:
                            break;
                    }
                    break;
            }
            //  discription.Append("底分" + ReadInt32toInt4 (parma[0],  21) + "分 ");

            //switch (ReadColumnValue(parma, rowNum, 26))
            //{
            //    case 1:
            //        discription.Append("将码5分 ");
            //        break;
            //    case 2:
            //        discription.Append("将码10分 ");
            //        break;
            //    default:
            //        break;
            //}
            discription.Append(ReadColumnValue(parma, rowNum, 27) > 0 ? "可接炮 " : "");

            //switch (ReadColumnValue(parma, rowNum, 29))
            //{
            //    case 1:
            //        discription.Append("升配 ");
            //        break;
            //    case 2:
            //        discription.Append("原配 ");
            //        break;
            //    case 3:
            //        discription.Append("降配 ");
            //        break;
            //    default:
            //        break;
            //}

            return;
        }
        //获取上次玩家默认的选择玩法
        public int GetPlayerParamRuleId(int RuleId)
        {
            int isOption = 0;
            switch (RuleId)
            {
                case 1001: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                case 1002: isOption = ReadChoicedRule(6) > 0 ? 1 : 0; break;
                case 1003: isOption = ReadChoicedRule(7) > 0 ? 1 : 0; break;
                case 1004: isOption = ReadChoicedRule(8) > 0 ? 1 : 0; break;
                case 1005: isOption = ReadChoicedRule(9) > 0 ? 1 : 0; break;
                case 1006: isOption = ReadChoicedRule(10) > 0 ? 1 : 0; break;
                case 1007: isOption = ReadChoicedRule(11) > 0 ? 1 : 0; break;
                case 2003: isOption = ReadChoicedRule(12) > 0 ? 1 : 0; break;
                // case 2004: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                case 2005: isOption = ReadChoicedRule(13) > 0 ? 1 : 0; break;
                case 2006: isOption = ReadChoicedRule(14) > 0 ? 1 : 0; break;
                case 2007: isOption = ReadChoicedRule(15) == 1 ? 1 : 0; break;
                case 2008: isOption = ReadChoicedRule(15) == 0 ? 1 : 0; break;
                case 2009: isOption = ReadChoicedRule(28) > 0 ? 1 : 0; break;
                case 3001: isOption = ReadChoicedRule(27) > 0 ? 1 : 0; break;
                case 3002: isOption = ReadChoicedRule(26) <= 1 ? 1 : 0; break;
                case 3003: isOption = ReadChoicedRule(26) == 2 ? 1 : 0; break;
                case 3004: isOption = ReadChoicedRule1(0, 16) == 0 ? 1 : 0; break;
                case 3005: isOption = ReadChoicedRule1(0, 16) == 2 ? 1 : 0; break;
                case 3006: isOption = ReadChoicedRule1(0, 16) == 4 ? 1 : 0; break;
                case 3007: isOption = ReadChoicedRule1(0, 16) == 6 ? 1 : 0; break;
                case 3008: isOption = ReadChoicedRule(27) == 1 ? 1 : 0; break;
                case 3009: isOption = ReadChoicedRule(27) == 2 ? 1 : 0; break;
                case 3010: isOption = ReadChoicedRule(27) == 3 ? 1 : 0; break;
                //case 3011: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                //case 3012: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                //case 3013: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                case 3014: isOption = ReadChoicedRule(25) <= 1 ? 1 : 0; break;
                case 3015: isOption = ReadChoicedRule(25) == 2 ? 1 : 0; break;
                case 3016: isOption = ReadChoicedRule(25) == 3 ? 1 : 0; break;
                case 3017: isOption = ReadChoicedRule(41) <= 1 ? 1 : 0; break;
                case 3018: isOption = ReadChoicedRule(41) == 2 ? 1 : 0; break;
                case 3019: isOption = ReadChoicedRule1(0, 20) == 3 ? 1 : 0; break;
                case 3020: isOption = ReadChoicedRule1(0, 20) == 5 ? 1 : 0; break;
                case 3021: isOption = ReadChoicedRule1(0, 20) == 10 ? 1 : 0; break;
                //  case 3022: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                case 3024: isOption = ReadChoicedRule(31) > 0 ? 1 : 0; break;
                case 3025: isOption = ReadChoicedRule(30) > 0 ? 1 : 0; break;
                //  case 3026: isOption = ReadChoicedRule(5) > 0 ? 1 : 0; break;
                case 3027: isOption = ReadChoicedRule(42) > 0 ? 1 : 0; break;
                case 3028: isOption = ReadChoicedRule(32) > 0 ? 1 : 0; break;
                case 3029: isOption = ReadChoicedRule(33) > 0 ? 1 : 0; break;
                case 3030: isOption = ReadChoicedRule(34) > 0 ? 1 : 0; break;
                case 3031: isOption = ReadChoicedRule(35) > 0 ? 1 : 0; break;
                case 3032: isOption = ReadChoicedRule(40) > 0 ? 1 : 0; break;
                case 3033: isOption = ReadChoicedRule(36) > 0 ? 1 : 0; break;
                case 3034: isOption = ReadChoicedRule(37) > 0 ? 1 : 0; break;
                case 3035: isOption = ReadChoicedRule1(0, 16) == 1 ? 1 : 0; break;
                case 3036: isOption = ReadChoicedRule(38) <= 1 ? 1 : 0; break;
                case 3037: isOption = ReadChoicedRule(38) == 2 ? 1 : 0; break;
                case 3038: isOption = ReadChoicedRule(45) == 0 ? 1 : 0; break;
                case 3039: isOption = ReadChoicedRule(45) == 1 ? 1 : 0; break;
                case 3040: isOption = ReadChoicedRule(43) == 0 ? 1 : 0; break;
                case 3041: isOption = ReadChoicedRule(43) == 1 ? 1 : 0; break;
                case 3042: isOption = ReadChoicedRule(43) == 2 ? 1 : 0; break;
                case 3043: isOption = ReadChoicedRule(44) == 0 ? 1 : 0; break;
                case 3044: isOption = ReadChoicedRule(44) == 1 ? 1 : 0; break;
                case 3045: isOption = ReadChoicedRule(44) == 2 ? 1 : 0; break;
                default:
                    break;
            }
            //  Debug.LogError("----"+ RuleId+"__"+isOption );
            return isOption;
        }
        /// <summary>
        /// 规则id
        /// </summary>
        /// <param name="MethodId"></param>
        public bool JudgeIsShow(int MethodId)
        {
            bool isShow = false;
            MahjongGame_AH.Data.PlayerPlayingPanelData pppd = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData;
            switch (MethodId)
            {
                //十三幺
                case 1001:
                    if (pppd.playingMethodConf.byWinSpecialThirteenOrphans == 1)
                    {
                        isShow = true;
                    }
                    break;
                //七对
                case 1002:
                    // Debug.LogError("七对：" + pppd.playingMethodConf.byWinSpecialSevenPairs);
                    if (pppd.playingMethodConf.byWinSpecialSevenPairs == 1)
                    {
                        isShow = true;
                    }
                    break;
                //豪华七对
                case 1003:
                    if (pppd.playingMethodConf.byWinSpecialLuxurySevenPairs == 1)
                    {
                        isShow = true;
                    }
                    break;
                //十三不靠                   
                case 1004:
                    if (pppd.playingMethodConf.byWinSpecialThirteenIndepend == 1)
                    {
                        isShow = true;
                    }
                    break;
                //明杠收三家
                case 1005:
                    if (pppd.playingMethodConf.byFanExtraExposedMode == 2)
                    {
                        isShow = true;
                    }
                    break;
                //四杠荒庄
                case 1006:
                    if (pppd.playingMethodConf.byDrawFourKong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //自摸翻倍
                case 1007:
                    if (pppd.playingMethodConf.byWinPointSelfDrawnMultiple == 2 && pppd.iMethodId == 7)
                    {
                        isShow = true;
                    }
                    break;
                //一炮单响
                case 2002:
                    if (pppd.playingMethodConf.byWinPointMultiShoot == 0)
                    {
                        isShow = true;
                    }
                    break;
                //一炮多响
                case 2003:
                    if (pppd.playingMethodConf.byWinPointMultiShoot == 1)
                    {
                        isShow = true;
                    }
                    break;
                //无需求
                case 2004:
                    //if (pppd.playingMethodConf. == 1)
                    //{
                    //    isShow = true;
                    //}
                    break;
                //可前台
                case 2005:
                //if (pppd.playingMethodConf. == 1)
                //{
                //    isShow = true;
                //}
                //break;
                //可后和
                case 2006:
                //if (pppd.playingMethodConf. == 1)
                //{
                //    isShow = true;
                //}
                //break;
                //接炮收三家
                case 2007:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 1)
                    {
                        isShow = true;
                    }
                    break;
                //接炮收一家
                case 2008:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 0)
                    {
                        isShow = true;
                    }
                    break;
                //红中算码
                case 2009:
                    if (pppd.playingMethodConf.byWinPointRedDargonPoint == 1)
                    {
                        isShow = true;
                    }
                    break;
                //多人付
                case 2010:
                    if (pppd.playingMethodConf.byFanExtraExposedBase == 1)
                    {
                        isShow = true;
                    }
                    break;
                //坐庄
                case 2011:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 1)
                    {
                        isShow = true;
                    }
                    break;
                //带字牌
                case 2018:
                    if (pppd.playingMethodConf.byCreateModeHaveWind > 0 && pppd.playingMethodConf.byCreateModeHaveDragon > 0)
                    {
                        isShow = true;
                    }
                    break;
                //点炮三分
                case 2019:

                    break;
                //轮庄
                case 2020:
                    if (pppd.playingMethodConf.byDealerDraw == 2 || pppd.playingMethodConf.byDealerDealer == 2)
                    {
                        isShow = true;
                    }
                    break;
                //放炮玩法
                case 2021:
                    if (pppd.playingMethodConf.byDealerPlayer == 2)
                    {
                        isShow = true;
                    }
                    break;
                //带庄
                case 2022:
                    if (pppd.playingMethodConf.byWinPointDealerMultiple == 2)
                    {
                        isShow = true;
                    }
                    break;
                //自摸胡
                case 2023:
                    if (pppd.playingMethodConf.byWinLimitSelfDrawn == 1)
                    {
                        isShow = true;
                    }
                    break;
                //杠随胡走
                case 2024:
                    if (pppd.playingMethodConf.byFanExtraMode == 2)
                    {
                        isShow = true;
                    }
                    break;
                //可接炮
                case 3001:
                    if (pppd.playingMethodConf.byWinLimitSelfDrawn == 0)
                    {
                        isShow = true;
                    }
                    break;
                //奖码5分
                case 3002:
                //if (pppd.playingMethodConf.byDiscardChow == 1)
                //{
                //    isShow = true;
                //}
                // break;
                //可吃牌
                case 3003:
                    if (pppd.playingMethodConf.byDiscardChow == 1)
                    {
                        isShow = true;
                    }
                    break;
                //无码
                case 3004:
                    if (pppd.playingMethodConf.byWinPointFanNum == 0)
                    {
                        isShow = true;
                    }
                    break;
                //扎一码
                case 3035:
                    if (pppd.playingMethodConf.byWinPointFanNum == 1)
                    {
                        isShow = true;
                    }
                    break;
                //扎二码
                case 3005:
                    if (pppd.playingMethodConf.byWinPointFanNum == 2)
                    {
                        isShow = true;
                    }
                    break;
                //不逼金
                case 3006:
                    if (pppd.playingMethodConf.byWinPointFanNum == 4)
                    {
                        isShow = true;
                    }
                    break;
                //扎六码
                case 3007:
                    if (pppd.playingMethodConf.byWinPointFanNum == 6)
                    {
                        isShow = true;
                    }
                    break;
                //1嘴
                case 3014:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 1)
                    {
                        isShow = true;
                    }
                    break;
                //5嘴
                case 3015:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 5)
                    {
                        isShow = true;
                    }
                    break;
                //10嘴
                case 3016:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 10)
                    {
                        isShow = true;
                    }
                    break;
                //平胡
                case 3017:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byFanSameColor == 0)
                    {
                        isShow = true;
                    }
                    break;
                //大胡
                case 3018:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byFanSameColor > 0)
                    {
                        isShow = true;
                    }
                    break;
                //坎
                case 3024:
                    if (pppd.playingMethodConf.byWinKong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //缺门
                case 3025:
                    if (pppd.playingMethodConf.byWinLack == 1)
                    {
                        isShow = true;
                    }
                    break;
                //点数
                case 3027:
                    if (pppd.playingMethodConf.byWinPoint == 1)
                    {
                        isShow = true;
                    }
                    break;
                //见面
                case 3028:
                    if (pppd.playingMethodConf.byWinMeet == 1)
                    {
                        isShow = true;
                    }
                    break;
                //幺九将
                case 3029:
                    if (pppd.playingMethodConf.byWinOneNineJong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //六连
                case 3030:
                    if (pppd.playingMethodConf.byWinContinuousSix == 1)
                    {
                        isShow = true;
                    }
                    break;
                //断独幺
                case 3031:
                    if (pppd.playingMethodConf.byWinOrphas == 1)
                    {
                        isShow = true;
                    }
                    break;
                ///抢杠全包
                case 3032:
                    if (pppd.playingMethodConf.byFanExtraExrobbingKongMode == 2)
                    {
                        isShow = true;
                    }
                    break;
                //抢杠无红中
                case 3033:
                    if (pppd.playingMethodConf.byWinLimitRobbingKong == 2)
                    {
                        isShow = true;
                    }
                    break;
                //荒杠荒庄
                case 3034:
                    if (pppd.playingMethodConf.byDrawFourNoKong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //原版计分
                case 3036:
                    if (pppd.playingMethodConf.byWinPointBasePointMode == 0)
                    {
                        isShow = true;
                    }
                    break;
                case 3037:
                    if (pppd.playingMethodConf.byWinPointBasePointMode == 1)
                    {
                        isShow = true;
                    }
                    break;
                case 3038:
                    if (pppd.playingMethodConf.byWinLimitBeginFan == 1)
                    {
                        isShow = true;
                    }
                    break;
                case 3039:
                    if (pppd.playingMethodConf.byWinLimitBeginFan == 2)
                    {
                        isShow = true;
                    }
                    break;
                case 3040:
                    if (pppd.playingMethodConf.byWinWhiteBoardChange == 1)
                    {
                        isShow = true;
                    }
                    break;
                case 3041:
                    if (pppd.playingMethodConf.byWinWhiteBoardChange == 0)
                    {
                        isShow = true;
                    }
                    break;
                case 3042:
                    if (pppd.playingMethodConf.byWinWhiteBoardChange == 2)
                    {
                        isShow = true;
                    }
                    break;
                case 3043:
                    if (pppd.playingMethodConf.byFanHonorTiles == 30)
                    {
                        isShow = true;
                    }
                    break;
                case 3044:
                    if (pppd.playingMethodConf.byFanHonorTiles == 60)
                    {
                        isShow = true;
                    }
                    break;
                case 3045:
                    if (pppd.playingMethodConf.byFanHonorTiles == 100)
                    {
                        isShow = true;
                    }
                    break;
                default:
                    break;
            }

            return isShow;
        }
        public class TaskSystem
        {
            public List<Action> TasksEvents = new List<Action>();

            public void AddEvent(Action ev)
            {
                TasksEvents.Add(ev);
            }
            public void CompeletBeforeTask()
            {
                if (TasksEvents.Count > 0)
                {
                    TasksEvents.RemoveAt(0);
                }
                if (TasksEvents.Count > 0)
                {
                    if (TasksEvents[0] != null)
                    {
                        TasksEvents[0]();
                    }
                    Debug.LogWarning("剩余任务数量：" + TasksEvents.Count);
                }
            }
        }

    }

}