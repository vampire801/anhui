using System;
using UnityEngine;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MahjongLobby_AH;
using System.Text;
using DG.Tweening;
using XLua;

namespace anhui
{/*
 * 包体命名规则sxmj_渠道编号_版本号；   例如sxmj_1001_1001.apk  //山西版
 * 差异包命名规则sxmj_渠道编号_新版本_from_旧版本；  例如sxmj_1001_1002_from_1001.patch 
*/
    [Hotfix]
    [LuaCallCSharp]
    public class CheckUpdateManager : MonoBehaviour
    {
        public GameObject DisConnectPanel;
        public GameObject EventSystem;  //ugui的事件处理器
        public GameObject CustomerPanel; //客服面板
        public Camera Camera; //摄像机

        #region 公共弹框
        public GameObject Remind; //公共提示弹框 
        public Button LeftBtn; //弹框的左边按钮
        public Button RightBtn; //弹框右边的按钮
        public Button CloseBtn; //关闭按钮
                                //public Text LeftBtn; //左边按钮的文字
                                //public Text RightBtn;  //右边按钮文字
        public Text Content; //面板显示内容
        public float UrlReadTimer; //网页读取时间  如果超过10s，默认超时，给出弹框提示    
        #endregion

        #region 实例
        static CheckUpdateManager instance;
        public static CheckUpdateManager Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
            gameObject.SetActive(true);
            DontDestroyOnLoad(EventSystem);
        }
        #endregion 实例          


        private WWW www;  //加载场景的www
        private string scenePath; //场景的路径    
        float prograss = 0; //进入大厅的进度条

        //==================================================   
        public RawImage LoadingIamge; //加载的背景画面
        public RawImage LoadingWrite;  //视屏的白色背景图
        public Text Loding;  //加载显示的文字
        public Slider slider; //加载进度条
        public Text sliderValueToText;//显示进度条加载了多少
        public GameObject UpdateFrame; //更新显示框 
        public Text UpdateContent; //更新显示内容
        public Text precentUpdate;  //显示更新进度的百分比
        public Text UpdateContent_Down; //进度条下方更新显示内容
        public GameObject image1;
        public GameObject image2;
        List<GameObject> doMove = new List<GameObject>();
        public Text Version; //版本号
        int VersionNew = 0;  //新的版本号
        [HideInInspector]
        public float precent;  //表示更新进度的进度值，范围0-1
        int status_down;  //下载状态，1表示下载差异包，2表示下载最新安装包,0表示测试状态
        int status_down_ok; //点击确定按钮之后的下载状态
        [HideInInspector]
        public int iStatus;  //0表示不需要更新，1表示可关闭更新，2表示必须强更   
        public Dictionary<string, string[]> md5 = new Dictionary<string, string[]>();//用来存储读取的版本的md5码，数组中0存储md5码，1中存储的是该文件的大小，单位k

        public float fFileSize;  //本次更新包的大小

        void Start()
        {
            Version.text = "版本号：" + LobbyContants.version_v + "." + LobbyContants.iChannelVersion + LobbyContants.version_typr;
            Camera.depth = 2;
            //#if UNITY_ANDROID || UNITY_IOS
            //        //播放公司logo视频     
            //        Handheld.PlayFullScreenMovie("iBlueJoy_Logo.mp4", Color.white, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);
            //        //StartCoroutine(DelayShowLoaing());
            //#endif
            DisConnectPanel.SetActive(false);
            SDKManager.Instance.GetIP(() => { Debug.LogWarning("IPI---PIP::" + MahjongCommonMethod.PlayerIp); });

            LoadingIamge.gameObject.SetActive(true);
            LoadingWrite.gameObject.SetActive(false);
            if (Application.platform == RuntimePlatform.Android)
            {
                //获取用户的经纬度数据
                AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                //Debug.LogError("调用玩家定位方法");
                jo.Call("GetPlayerPos");

                //打印mac地址
                string macId = jo.Call<string>("getLocalMacAddressFromIp");
                Debug.Log ("手机的mac地址:" + macId);
            }
            if (NetWorkStatus() <= 0)
            {
                DisConnectPanel.SetActive(true);
                return;
            }
            else
            {
                DisConnectPanel.SetActive(false);
            }

            if (LobbyContants.SeverType != "199")
            {
                //检查版本是否需要更新            
                StartCoroutine(GetUpdateMessage());
                //开始计算访问网页时间，用于判断是否超时
            }
            else
            {
                Loding.text = TextConstant.LOADINGTEXT;
                StartCoroutine(BeginLoader());
            }

            OnInstanceAdvertisementImage();
            //LoadAdvertisementData();
            OnShowGuanggao(true);
            PlayerPrefs.SetString(LobbyContants.SetSeatIDAgo, "0000");
        }

        IEnumerator OneSecondDelay()
        {
            yield return new WaitForSeconds(1f);
            UrlReadTimer++;
            if (UrlReadTimer >= 10)
            {
                Camera.depth = 2;
                UrlReadTimer = 0;
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("uloading");
                CommenRemind("客 服", "重 试", "亲，网站服务器连接不上，请稍后重试或联系客服寻求帮助！", () => { CustomerPanel.SetActive(true); },
                    () => { StartCoroutine(GetUpdateMessage()); }, () => { BtnQuitApp(); });
                yield break;
            }
            else
            {
                //如果超过2s未获取
                if (UrlReadTimer == 2)
                {
                    SDKManager.Instance.GetComponent<CameControler>().PostMsg("loading", "正在获取更新信息，请耐心等待！");
                }
                StartCoroutine(OneSecondDelay());
            }
        }

        /// <summary>
        /// 实例化需要播放的图片
        /// </summary>
        void OnInstanceAdvertisementImage()
        {
            GameObject go1 = Instantiate(image1) as GameObject;
            go1.transform.SetParent(gameObj_ShowGroup.transform);
            go1.transform.localPosition = new Vector3(0, 0, 0);
            go1.transform.localRotation = Quaternion.identity;
            go1.transform.localScale = new Vector3(1, 1, 1);
            go1.name = "1";
            go1.SetActive(true);
            doMove.Add(go1);

            GameObject go2 = Instantiate(image2) as GameObject;
            go2.transform.SetParent(gameObj_ShowGroup.transform);
            go2.transform.localPosition = new Vector3(0, 720, 0);
            go2.transform.localRotation = Quaternion.identity;
            go2.transform.localScale = new Vector3(1, 1, 1);
            go2.name = "2";
            go2.SetActive(true);
            doMove.Add(go2);

            //GameObject go3 = Instantiate(image2) as GameObject;
            //go3.transform.SetParent(gameObj_ShowGroup.transform);
            //go3.transform.localPosition = new Vector3(0, -720, 0);
            //go3.transform.localRotation = Quaternion.identity;
            //go3.transform.localScale = new Vector3(1, 1, 1);
            //go3.name = "3";
            //go3.SetActive(true);
            //doMove.Add(go3);
        }

        IEnumerator DelayShowLoaing()
        {
            LoadingIamge.gameObject.SetActive(true);
            LoadingWrite.gameObject.SetActive(false);
            yield return 0;//new WaitForSeconds(0.1f);
        }

        /// <summary>
        /// 网络接口的数据
        /// </summary>
        public class NetWorkDomain
        {
            public int status;   //1成功  0参数错误 2该版本不存在  9系统错误
            public string server_ver;  //服务器版本（消息版本）
            public string ip;  //ip
            public string domian;  //网关地址
            public string ip_ios;  //ios审核的ip地址
            public string domian_ios;  //ios审核的网关地址
            public string port;  //端口
            public string ykFlag;  //游客登陆
            public string examine; // IOS审查状态，0不在审查中，1在审查中
            public string updateFlag;  //是否强更 0否 1是
            public string newVer; //更新后版本
        }


        public class Domain_
        {
            public List<NetWorkDomain> Domain_Data = new List<NetWorkDomain>();
        }

        //保存网关接口的数据
        public NetWorkDomain domain = new NetWorkDomain();


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
                status = 0;
            }

            //判断玩家是否使用移动网络
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                status = 2;
            }

            //判断玩家是否使用wifi
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                status = 3;
            }
            return status;
        }



        /// <summary>
        /// 点击返回登录的按钮
        /// </summary>
        public void BtnDisConnectReturn()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
            jo.Call("doRestart", 50);
#elif UNITY_IOS
        Application.Quit();
#endif
        }

        //退出当前程序
        public void BtnQuitApp()
        {
            Application.Quit();
        }

        /// <summary>
        /// 断开链接的确定按钮
        /// </summary>
        public void BtnDisConnectOk()
        {
            if (NetWorkStatus() <= 0)
            {
                DisConnectPanel.gameObject.SetActive(true);
                SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");
            }
            else
            {
                DisConnectPanel.gameObject.SetActive(false);
                if (LobbyContants.SeverType != "199")
                {
                    //检查版本是否需要更新
                    StartCoroutine(GetUpdateMessage());
                }
                else
                {
                    Loding.text = TextConstant.LOADINGTEXT;
                    StartCoroutine(BeginLoader());
                }
            }
        }


        IEnumerator Chaoshi;

        /// <summary>
        /// 获取是否强更的信息
        /// </summary>
        IEnumerator GetUpdateMessage()
        {
            Camera.depth = 2;
            Chaoshi = OneSecondDelay();
            StartCoroutine(Chaoshi);
            //显示更新提示信息
            int iUserId = 0;
            if (PlayerPrefs.HasKey("userId"))
            {
                iUserId = PlayerPrefs.GetInt("userId");
            }

            Loding.gameObject.SetActive(true);
            Loding.text = "正在版本更新，请耐心等候";
            string url = LobbyContants.MAJONG_PORT_URL + "getDomain.x?devNum=" + MahjongCommonMethod.Instance.MacId + "&ver=" + LobbyContants.ClientVersion +
                "&channel=" + LobbyContants.iChannelVersion + "&userId=" + iUserId;
            if (SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "getDomain.x?devNum=" + MahjongCommonMethod.Instance.MacId + "&ver=" + LobbyContants.ClientVersion +
                "&channel=" + LobbyContants.iChannelVersion + "&userId=" + iUserId;
            }
            Debug.Log("请求网址;" + url);
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                // Debug.LogWarning(url);
                //使用webclient 请求网页数据
                System.Net.WebClient MyWebClient = new System.Net.WebClient();
                yield return MyWebClient;
                MyWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。
                byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据                 
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                StringBuilder str = new StringBuilder();
                Camera.depth = -1;
                UrlReadTimer = 0;
                if (Chaoshi != null)
                {
                    StopCoroutine(Chaoshi);
                }
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("uloading");
                str.Append("{\"");
                str.Append("Domain_Data");
                str.Append("\":[");
                str.Append(pageHtml);
                str.Append("]}");
                Debug.LogWarning("str:" + str.ToString());
                GetDomainData(str.ToString());
            }

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
            {
                WWW www = new WWW(url);

                yield return www;
                Camera.depth = -1;
                UrlReadTimer = 0;
                if (Chaoshi != null)
                {
                    StopCoroutine(Chaoshi);
                }
                SDKManager.Instance.GetComponent<CameControler>().PostMsg("uloading");

                if (www.error != null)
                {
                    Debug.LogError("网页获取数据错误:" + www.error);
                    CommenRemind("客 服", "重 试", "亲，网站服务器连接不上，请稍后重试或联系客服寻求帮助！", () => { CustomerPanel.SetActive(true); },
                        () => { StartCoroutine(GetUpdateMessage()); }, () => { BtnQuitApp(); });
                    yield break;
                }
                else
                {
                    StringBuilder str = new StringBuilder();
                    str.Append("{\"");
                    str.Append("Domain_Data");
                    str.Append("\":[");
                    str.Append(www.text);
                    str.Append("]}");
                    Debug.Log("str:" + str.ToString());
                    GetDomainData(str.ToString());
                }
            }
            MahjongCommonMethod.Instance.HasClicked((int)MahjongCommonMethod.StateType.CheckVersion);
        }



        void CommenRemind(string leftBtn, string rightBtn, string content, Action left, Action right, Action close)
        {
            if (Chaoshi != null)
            {
                StopCoroutine(Chaoshi);
            }
            LeftBtn.transform.GetChild(0).GetComponent<Text>().text = leftBtn;
            RightBtn.transform.GetChild(0).GetComponent<Text>().text = rightBtn;
            Content.text = content;
            //添加事件
            LeftBtn.onClick.AddListener(delegate () { left(); });
            RightBtn.onClick.AddListener(delegate () { right(); });
            CloseBtn.onClick.AddListener(delegate () { close(); });
            Remind.SetActive(true);
        }

        /// <summary>
        /// 获取大厅网关的数据
        /// </summary>
        void GetDomainData(string json)
        {
            Domain_ data = new Domain_();
            data = JsonBase.DeserializeFromJson<Domain_>(json.ToString());
            if (data.Domain_Data[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.Domain_Data[0].status);
                //ComeLobby();
                //获取更新数据失败，提示玩家版本更新失败，重新登录            
                CommenRemind("客 服", "重 试", "亲，网站服务器连接不上，请稍后重试或联系客服寻求帮助！", () => { CustomerPanel.SetActive(true); },
                   () => { StartCoroutine(GetUpdateMessage()); }, () => { BtnQuitApp(); });
                return;
            }

            domain = data.Domain_Data[0];

            //如果最新版本大于当前版本，显示更新
            if (Convert.ToInt16(domain.updateFlag) == 1)
            {
                //先判断消息版本，如果消息版本不一样，则直接更新到最新版本  
                Debug.LogError(domain.newVer);
                if (Convert.ToInt32(domain.newVer) != MahjongLobby_AH.LobbyContants.ClientVersion)
                {
                    //如果是安卓热更
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        VersionNew = Convert.ToInt32(domain.newVer);
                        Debug.LogError("开始更新进程");
                        iStatus = Convert.ToInt16(domain.updateFlag);
                        //开始下载md5文件，显示更新包体大小
                        StartCoroutine(DownloadMd5());
                        return;

                    }
                    else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        UpdateFrame.SetActive(true);
                        UpdateContent.text = "发现新版本，是否更新？";
                        return;
                    }
                    //如果是ios，提示进入应用商店更新
                }
            }

            LobbyContants.SeverVersion = (short)Convert.ToInt32(domain.server_ver);
            //LobbyContants.SeverVersion = 10002;
            LobbyContants.LOBBY_GATEWAY_IP = domain.ip.ToString();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //为网关地址赋值
                if (Convert.ToInt32(domain.examine) == 1 && LobbyContants.isContorledByBack)
                {
                    SDKManager.Instance.CheckStatus = 1;
                    SDKManager.Instance.IOSCheckStaus = 1;

                    SDKManager.Instance.IsConnectNetWork = 2;
                    if (!string.IsNullOrEmpty(domain.ip_ios))
                    {
                        MahjongLobby_AH.LobbyContants.LOBBY_GATEWAY_IP = domain.ip_ios;
                    }
                    // PlayerPrefs.DeleteAll();
                    //  SDKManager.Instance.gameObject.GetComponent<Consolation.TestConsole>()._isShowWind.gameObject.SetActive(false);
                }
                else
                {
                    //  SDKManager.Instance.gameObject.GetComponent<Consolation.TestConsole>()._isShowWind.gameObject.SetActive(true);            
                }
                SDKManager.Instance.iShowGuestLogin = Convert.ToInt32(domain.ykFlag);
            }
            if (!string.IsNullOrEmpty(domain.port))
            {
                MahjongLobby_AH.LobbyContants.LOBBY_GATEWAY_PORT = int.Parse(domain.port);
            }

            iStatus = 0;



            //Debug.LogError("更新状态：" + iStatus);

            //表示该版本不用更新，直接跳转场景
            if (iStatus == 0)
            {
                UpdateContent_Down.gameObject.SetActive(false);
                ComeLobby();
            }
        }
        //void TurntoUrl()
        //{
        //    Application.OpenURL(SDKManager.WXInviteUrl + "0");

        //}

        /// <summary>
        /// 进入大厅
        /// </summary>
        void ComeLobby()
        {
            Loding.text = TextConstant.LOADINGTEXT;
            StartCoroutine(BeginLoader());
        }


        //开始跳转场景，显示进度条
        IEnumerator BeginLoader()
        {
            slider.gameObject.SetActive(true);
            AsyncOperation op = Application.LoadLevelAdditiveAsync("MAHJONG_LOBBY_GENERAL");
            //op.allowSceneActivation = false;        
            prograss = op.progress;
            transform.GetChild(0).GetComponent<Camera>().depth = 1;
            while (op.progress < 0.9f || LoadAdnNum < 2)
            {
                if (prograss < op.progress)
                {
                    SetSliderValue(prograss);
                    prograss = op.progress;
                }

                if (op.progress >= 0.9f && prograss != 1)
                {
                    prograss = 1;
                    Loding.text = TextConstant.LOADINGFINISH;
                    SetSliderValue(prograss);
                }

                yield return 0;
            }

            if (op.progress >= 0.9f)
            {
                transform.GetChild(0).GetComponent<Camera>().depth = -1;
                transform.GetChild(0).GetChild(0).GetComponent<Canvas>().enabled = false;
                if (op.isDone)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_GAME_GENERAL);
                    sw.Stop();
                    Debug.LogWarning("111预加载游戏场景花费时间:" + sw.ElapsedMilliseconds);
                }
                else
                {
                    StartCoroutine(DelayPreloadScene(0.5f, op));
                }
            }
        }


        IEnumerator DelayPreloadScene(float timer, AsyncOperation op)
        {
            yield return new WaitForSeconds(timer);
            if (op.isDone)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                SceneManager_anhui.Instance.PreloadScene(ESCENE.MAHJONG_GAME_GENERAL);
                sw.Stop();
                Debug.LogWarning("222预加载游戏场景花费时间:" + sw.ElapsedMilliseconds);
            }
            else
            {
                StartCoroutine(DelayPreloadScene(0.5f, op));
            }
        }

        /// <summary>
        /// 改变进度条的进度
        /// </summary>
        /// <param name="value"></param>
        void SetSliderValue(float value)
        {
            slider.value = value;
            sliderValueToText.text = (value * 100).ToString("F0") + "%";
        }

        //删除自己
        public void Destroy_Self()
        {
            Destroy(gameObject);
        }


        /// <summary>
        /// 从服务器下载md5文件
        /// </summary>
        /// <returns></returns>
        /// 
        IEnumerator DownloadMd5()
        {
            Loding.text = "正在检查游戏版本信息,请耐心等待";
            string url = LobbyContants.UpdateDownLoadUrl;   //更新服务器的地址
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "md5_files.txt?v=" + random;
            WWW www = new WWW(listUrl);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError("下载文件失败");
                CommenRemind("客 服", "重 试", "亲，版本更新检查失败，请稍后重试或联系客服寻求帮助！", () => { CustomerPanel.SetActive(true); },
                    () => { StartCoroutine(DownloadMd5()); }, () => { BtnQuitApp(); });
                yield break;
            }


            Debug.LogWarning("www.text:" + www.text);

            //保存所有文件的md5码        
            string[] file = www.text.Split('\n');
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == "")
                {
                    continue;
                }
                string[] keyValue = file[i].Split('|');
                string[] content = { keyValue[1], keyValue[2] };
                string FileName = keyValue[0].Split('.')[0];
                if (!md5.ContainsKey(FileName))
                {
                    md5.Add(FileName, content);
                }
            }

            using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity"))
            {
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                {
                    UpdateFrame.SetActive(true);

                    //如果md5一致
                    if (jo.Call<string>("getFileMD5", 0) == md5["sxmj_" + LobbyContants.iChannelVersion + "_" + LobbyContants.ClientVersion][0])
                    {
                        Loding.text = "检查更新文件完毕，开始更新";
                        //显示更新内容详情
                        float fileSize = (Convert.ToInt32(md5[GetpatchName()][1])) / 1024f / 1024f;
                        fFileSize = fileSize;
                        UpdateContent.text = "更新文件1个，文件大小" + fileSize.ToString("0.00") + "M，是否继续更新？（请在WIFI环境下更新）";
                        status_down = 1;
                    }
                    //如果apk的MD5不一致，直接下载最新的安装包，进行安装
                    else
                    {
                        float fileSize = (Convert.ToInt32(md5[GetNewApkName()][1])) / 1024f / 1024f;
                        fFileSize = fileSize;
                        UpdateContent.text = "更新文件1个，文件大小" + fileSize.ToString("0.00") + "M，是否继续更新？（请在WIFI环境下更新）";
                        status_down = 2;
                    }
                }

            }
        }

        /// <summary>
        /// 更新确定按钮
        /// </summary>
        public void update_btn_ok()
        {
            UpdateFrame.SetActive(false);
#if UNITY_ANDROID
            slider.gameObject.SetActive(true);
            status_down_ok = status_down;
            slider.value = 0;
            Loding.text = "正在下载更新补丁文件，请耐心等候";

            precentUpdate.gameObject.SetActive(true);
            precentUpdate.text = "本次更新（" + fFileSize.ToString("0.00") + "M），已更新0%";
            UpdateContent_Down.gameObject.SetActive(true);
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string fileUrl = null;
            if (status_down == 1)
            {
                fileUrl = LobbyContants.UpdateDownLoadUrl + GetpatchName() + ".patch?v=" + random;
            }
            else if (status_down == 2)
            {
                fileUrl = LobbyContants.UpdateDownLoadUrl + GetNewApkName() + ".apk?v=" + random;
            }
            StartCoroutine(loadFile(fileUrl));
#elif UNITY_IOS
        Application.OpenURL(LobbyContants. IOSDownLoadURL);

#endif

        }



        /// <summary>
        /// 更新取消按钮
        /// </summary>
        public void update_btn_canle()
        {
            UpdateFrame.SetActive(false);
            //不强更
            if (Convert.ToInt16(domain.updateFlag) == 0)
            {
                //加载大厅面板
                ComeLobby();
                //关闭更新面板
            }
        }



        /// <summary>
        /// 点击后面背景
        /// </summary>
        public void BtnClickBg()
        {
            if (status_down_ok > 0)
            {
                return;
            }

            if (iStatus == 1)
            {
                //重新显示更新面板
                UpdateFrame.SetActive(true);
            }
        }

        //检查设置
        public void BtnSetting()
        {
            MahjongCommonMethod.Instance.SettingPanel();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IEnumerator loadFile(string url)
        {
            WWW www = new WWW(url);

            while (!www.isDone)
            {
                precent = www.progress;
                slider.value = precent;
                precentUpdate.text = "本次更新（" + fFileSize.ToString("0.00") + "M），已更新" + (precent * 100f).ToString("0.0") + "%";
                yield return 1;
            }

            if (www.isDone)
            {
                precent = 1;
                slider.value = precent;
                //slider.gameObject.SetActive(false);
                Loding.text = "正在解压游戏资源，请耐心等候。（解压过程不消耗流量）";
                byte[] patch = www.bytes;
                int length = patch.Length;
                string localFile = Application.persistentDataPath + "/";    //本地保存地址  
                if (status_down == 0)
                {
                    //更改下载地址
                    localFile = "C:/Users/Administrator/Desktop/";
                }

                //根据下载状态问题，确定下载文件名
                string name_pak = null;
                if (status_down == 1)
                {
                    name_pak = GetpatchName() + ".patch";
                }
                else if (status_down == 2)
                {
                    name_pak = GetNewApkName() + ".apk";
                }

                //Debug.LogError("name_pak:" + name_pak);

                CreatPatchFile(localFile, name_pak, patch, length);

                //Debug.LogError("0========name_pak:" + name_pak);
                if (status_down == 1)
                {
                    string md5_patch = md5file(Application.persistentDataPath + "/" + name_pak);
                    //如果补丁包的md5码一致
                    if (md5_patch == (md5[GetpatchName()][0]))
                    {
                        Debug.LogError("合并新的安装包");
                        using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity"))
                        {
                            using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                            {
                                //Debug.LogError("老版本：" + LobbyContants.ClientVersion + ",新版本:" + VersionNew);
                                //合并生成新的apk安装包
                                int res = jo.Call<int>("BaspatchApk", LobbyContants.iChannelVersion.ToString(), LobbyContants.ClientVersion.ToString(), VersionNew.ToString());

                                //Debug.LogError("res:" + res);

                                //Debug.LogError("md5码：" + md5[GetNewApkName()][0]);

                                //Debug.LogError("getFileMD5:" + jo.Call<string>("getFileMD5", 1));

                                if (res == 0 && md5[GetNewApkName()][0] == jo.Call<string>("getFileMD5", 1))
                                {
                                    jo.Call("InstallApk", LobbyContants.iChannelVersion.ToString(), LobbyContants.ClientVersion.ToString(),
                                    VersionNew.ToString());
                                }
                                else
                                {
                                    ComeLobby();
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("MD5：读取：" + md5_patch + ",---服务器:" + md5[GetpatchName()][0]);
                    }
                }
                else
                {
                    Debug.LogWarning("0========name_pak:" + name_pak);
                    using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity"))
                    {
                        using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                        {
                            Debug.LogWarning("调用安装新的安装包问题");
                            //安装最新的安装包
                            jo.Call("InstallApk", LobbyContants.iChannelVersion.ToString(), LobbyContants.ClientVersion.ToString(),
                                    VersionNew.ToString());
                        }
                    }
                }

                yield return www;
            }
        }

        /// <summary>
        /// 创建对应的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="length"></param>
        void CreatPatchFile(string path, string name, byte[] info, int length)
        {
            Stream sw = null;
            FileInfo fi = new FileInfo(path + name);
            if (!fi.Exists)
            {
                sw = fi.Create();
            }
            else
            {
                sw = fi.Create();
            }

            sw.Write(info, 0, length);
            sw.Close();
            sw.Dispose();
        }



        /// <summary>
        /// 下载差异包
        /// </summary>
        /// <returns></returns>
        void DownLoadPatch()
        {

            string localfile = Application.persistentDataPath + "/" + GetpatchName() + ".patch";
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string fileurl = "";//AppConst.WebUrl + GetpatchName(NewVersion) + ".patch?v=" + random;        

            //localfile = @"C:\Users\Administrator\Desktop\" + GetpatchName(NewVersion) + ".patch";
            //通知开始下载差异包
            //Messenger<object>.Broadcast(NotiConst.DOWNLOAD_PATCH,"");
            //开始下载差异包
            Download_Patch(fileurl, localfile);
        }


        /// <summary>
        /// 下载补丁包，通过webclient下载
        /// </summary>
        void Download_Patch(string url, string currDownFile)
        {
            WebClient client = new WebClient();
            Debug.LogWarning("开始下载补丁包------1");
            client.DownloadFileAsync(new System.Uri(url), currDownFile);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            //client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Complete);                        
        }






        void Complete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string md5_patch = "";//Util.md5file(Application.persistentDataPath + "/" + GetpatchName(NewVersion) + ".patch");
            Debug.LogWarning("补丁包的地址:" + md5_patch);

            using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.byqb.Quick.UnityPlayerActivity"))
            {
                Debug.LogWarning("------------------------1");
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                {
                    jo.Call("test");
                    //Debug.LogError("------------------------1");
                    ////合并生成新的apk安装包
                    //int res = jo.Call<int>("BaspatchApk", FishLobby.GameConstants.IChannelClientId.ToString(), FishLobby.GameConstants.IChannelClientVersion.ToString(),
                    //    NewVersion);
                }
            }
        }

        /// <summary>
        /// 下载文件的进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive)
            {
                Debug.LogWarning("数据：" + e.BytesReceived + ",总的数量:" + e.TotalBytesToReceive);

                using (AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.byqb.Quick.UnityPlayerActivity"))
                {
                    using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity"))
                    {
                        jo.Call("test");
                    }
                }
            }
        }

        /// <summary>
        /// 获取补丁包的名字
        /// </summary>
        /// <param name="name"></param>
        string GetpatchName()
        {
            string patchName = "sxmj_" + LobbyContants.iChannelVersion + "_" + domain.newVer + "_from_" + LobbyContants.ClientVersion;
            return patchName;
        }

        /// <summary>
        /// 获取合成的最新的安装包的名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetNewApkName()
        {
            string newApkName = "sxmj_" + LobbyContants.iChannelVersion + "_" + domain.newVer;
            return newApkName;
        }


        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            //Debug.LogError("开始将计算文件的MD5码，文件路径：" + file);
            try
            {
                //Debug.LogError("计算文件的MD5码---------1");
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    Debug.LogError("计算MD5失败：" + ex.ToString());
                }
                throw new Exception("md5file() fail, error:" + ex.Message);

            }
        }


        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static int GetFileSize(string file)
        {

            if (!string.IsNullOrEmpty(file))
            {
                if (File.Exists(file))
                {
                    System.IO.FileInfo objFI = new System.IO.FileInfo(file);
                    return (int)objFI.Length;
                }
            }
            return 0;
        }


        /// <summary>
        /// 加载是否需要广告的标志
        /// </summary>
        void LoadAdvertisementData()
        {
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "loading.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "loading.x";
            }
            MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, null, OnGetValue_, "", 5);
        }

        void OnGetValue_(string json, int status)
        {
            int data = Convert.ToInt16(json);
            LoadingIamge.gameObject.SetActive(true);
            if (data == 1)
            {
                OnShowGuanggao(true);
            }
            else if (data == 0)
            {
                OnShowGuanggao(false);
            }
            else
            {
                Debug.LogError("数据错误");
            }
        }
        public class Advertisement_
        {
            public int status;  //1成功  9系统错误 0无数据
            public int active;  //2.是否开启广告 ：0 关闭 1 开启 2 数据错误
        }

        /// <summary>
        /// 关闭这个
        /// </summary>
        public void OnCloseHouKenTral()
        {
            gameObj_ShowGroup.SetActive(false);
            BG.SetActive(true);
            m_isAdvertisement = false;
        }

        void OnShowGuanggao(bool advertisement)
        {
            m_isAdvertisement = advertisement;
            if (m_isAdvertisement)
            {
                gameObj_ShowGroup.SetActive(true);
                BG.SetActive(false);
                isMoving = true;
                //LoadAdnNum = 2;
                timer = 0;
            }
            else
            {
                gameObj_ShowGroup.SetActive(false);
                BG.SetActive(true);
                isMoving = false;
                LoadAdnNum = 2;
            }
        }


        Vector3 v3_currentPos;
        public GameObject gameObj_ShowGroup;//需要移动的父物体
        public GameObject BG;//原本的BG
        bool m_isAdvertisement = false;//开启或者关闭广告
        public void OnPointerUp()
        {
            return;
            if (m_isAdvertisement == false) return;

            v3_currentPos = gameObj_ShowGroup.transform.localPosition;
            if (v3_currentPos.y < 0)        //左
            {
                if (doMove[0].transform.localPosition.y == 0)
                {
                    doMove[1].transform.localPosition = new Vector3(0, 720, 0);
                }
                else
                {
                    doMove[0].transform.localPosition = new Vector3(0, 720, 0);
                }
                if (doMove[0].transform.localPosition.y == 0)
                {
                    doMove[0].transform.DOLocalMoveY(-720, 0.3f);
                    doMove[1].transform.DOLocalMoveY(0, 0.3f);
                }
                else
                {
                    doMove[0].transform.DOLocalMoveY(0, 0.3f);
                    doMove[1].transform.DOLocalMoveY(-720, 0.3f);
                }
            }
            else
            {
                if (doMove[0].transform.localPosition.y == 0)
                {
                    doMove[1].transform.localPosition = new Vector3(0, -720, 0);
                }
                else
                {
                    doMove[0].transform.localPosition = new Vector3(0, -720, 0);
                }
                if (doMove[1].transform.localPosition.y == 0)
                {
                    doMove[1].transform.DOLocalMoveY(720, 0.3f);
                    doMove[0].transform.DOLocalMoveY(0, 0.3f);
                }
                else
                {
                    doMove[1].transform.DOLocalMoveY(0, 0.3f);
                    doMove[0].transform.DOLocalMoveY(720, 0.3f);
                }
            }
        }

        void Update()
        {
            OnAdvertisementMove();
        }

        int LoadAdnNum = 0;
        bool isMoving = false;//控制暂停广告图 使用手指来控制
        float timer = 0;
        void OnAdvertisementMove()
        {
            if (m_isAdvertisement)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isMoving = false;
                    timer = 0;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isMoving = true;
                    timer = 0;
                }

                while (timer >= 2 && isMoving)
                {
                    if (doMove[0].transform.localPosition.y == 0)
                    {
                        doMove[1].transform.localPosition = new Vector3(0, 720, 0);
                    }
                    else
                    {
                        doMove[0].transform.localPosition = new Vector3(0, 720, 0);
                    }
                    if (doMove[0].transform.localPosition.y == 0)
                    {
                        doMove[0].transform.DOLocalMoveY(-720, 0.3f);
                        doMove[1].transform.DOLocalMoveY(0, 0.3f);
                    }
                    else
                    {
                        doMove[0].transform.DOLocalMoveY(0, 0.3f);
                        doMove[1].transform.DOLocalMoveY(-720, 0.3f);
                    }

                    LoadAdnNum++;
                    timer -= 2;
                }
                timer += Time.deltaTime;
            }
        }

    }
}
