using UnityEngine;
using System.Collections;
using YunvaIM;
using System;
using System.Collections.Generic;
using MahjongGame_AH.Network.Message;
using MahjongGame_AH.Network;
using MahjongGame_AH.Data;
using UnityEngine.UI;
using DG.Tweening;
using gcloud_voice;
using System.IO;
using XLua;
using anhui;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class VoiceManegerMethord : MonoBehaviour
    {
        #region  单例
        static VoiceManegerMethord instance;
        public IGCloudVoice m_voiceengine;  //engine have init int mainscene start function
        public static VoiceManegerMethord Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// 0云娃 1GCloudVoice
        /// </summary>
        public static int VoiceFlag = 0;
        string szCurrentRecordUrlPath;
        // private TimerManager _timerManager;
        public Image _voiceImage;

        /// <summary>
        /// 播放语音状态
        /// </summary>
        public bool isPlayingState;
        public float _fTimeLimit = 10;
        /// <summary>
        /// 语音信息
        /// </summary>
        internal class UsersVoiceInfo
        {
            public int iDeskid;
            public int iUserId;
            public string szRecordUrlPath;
            public string szLocalUrlPath;
            public float fDurTime;
            public byte[] id;
            public UsersVoiceInfo()
            {

            }
            /// <summary>
            /// 玩家语音信息
            /// </summary>

            /// <param name="userid">用户id</param>
            /// <param name="recordUrl">网上下载地址</param>
            /// <param name="localUrl">本地地址</param>
            /// <param name="durtime">持续时长,默认-1表示无此参数</param>
            public UsersVoiceInfo(int userid, string recordUrl, string localUrl, byte[] ID, float durtime = -1)
            {

                iUserId = userid;
                szRecordUrlPath = recordUrl;
                szLocalUrlPath = localUrl;
                id = ID;
                fDurTime = durtime;
            }
        }
        string filePath_1;// 玩家【1】录音地址
        string filePath_2;// 玩家【2】录音地址
        string filePath_3;// 玩家【3】录音地址
        string filePath_4;// 玩家【4】录音地址

        internal List<UsersVoiceInfo> voices = new List<UsersVoiceInfo>();
        //录音模式 0保存本地 1:录音完成上传和识别，结束录音时，会收到上传回调和识别回调；2：录音完成上传，结束录音时，会收到上传回调
        private int num = 2;
        public System.Diagnostics.Stopwatch startwatch;
        /// <summary>
        /// 语音按钮/imageBg/VoiceSprite
        /// </summary>
        public Transform _voiceTrans;
        public GameObject _voiceObj;
        /// <summary>
        /// 是否在录制
        /// </summary>
        bool isRecord;


        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            m_voiceengine = GCloudVoice.GetEngine();
        }
        void Start()
        {
            if (MahjongCommonMethod.Instance.isVoiceHandler)
            {
                return;
            }
            MahjongCommonMethod.Instance.isVoiceHandler = true;
            m_voiceengine.OnApplyMessageKeyComplete += (code) =>
            {
                Debug.Log("OnApplyMessageKeyComplete ret=" + code);
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_MESSAGE_KEY_APPLIED_SUCC)
                {
                    Debug.Log("OnApplyMessageKeyComplete succ");
                }
                else
                {
                    Debug.Log("OnApplyMessageKeyComplete error");
                }
            };
           
            m_voiceengine.OnDownloadRecordFileComplete += (code, url, id) =>
            {
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_DOWNLOAD_RECORD_DONE)
                {
                    int downLoadErr = m_voiceengine.PlayRecordedFile(url);
                    ErroOccur(downLoadErr, null);
                }
                else
                {
                    Debug.Log("OnDownloadRecordFileComplete error" + code);
                }
            };
          
        }
        bool isCancelRecord;
        // Update is called once per frame
        void Update()
        {
            if (isRecord)
            {
                Vector3 v3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
                // Debug.LogError(v3.y - tempPosition_y);
                if (v3.y - tempPosition_y > 0.21f)//取消录音
                {
                    stopDotween();
                    _voiceTrans.gameObject.SetActive(false);
                    _voiceObj.SetActive(true);
                    isCancelRecord = true;
                    //CancelRecord();
                }
                else
                {
                    if (!isLimitTimeEnd)
                    {
                        startDotween();
                        _voiceTrans.gameObject.SetActive(true);
                        _voiceObj.SetActive(false);
                        isCancelRecord = false;
                    }
                }
                return;
            }
            else
            {
                UpdateVoicePlay();
            }
            if (VoiceFlag == 1 && m_voiceengine != null)
            {
                m_voiceengine.Poll();
            }
        }
        void OnDestroy()
        {
            Debug.Log("VoiceManagerMethord.OnDestory");
            if (Directory.Exists(MahjongLobby_AH.SDKManager.DataPath)&& Application.isMobilePlatform)
            {
                Directory.Delete(MahjongLobby_AH.SDKManager.DataPath, true);
            }
        }
        public void UpdateVoicePlay()
        {
            if (!MahjongCommonMethod.Instance.isOpenVoicePlay)//在此开启关闭语音播放
                return;
            if (voices.Count > 0 && !isPlayingState)//播放其他人的
            {
                isPlayingState = true;
                Debug.LogWarning("播放用户:" + voices[0].iUserId);
                StartCoroutine(DealNoReasult(voices[0].fDurTime));
                Chenggong = false;
                PlayVoice("");
            }
        }
        IEnumerator DealNoReasult(float time)
        {
            yield return new WaitForSeconds(time + 0.5f);
            if (!Chenggong)
            {
                Chenggong = true;
                StartCoroutine(CompeletPlay(time));
            }
        }
        bool Chenggong = false;
        public void PlayVoice(string urlLocal)
        {
            AudioListener.volume = 0f;
            //找到对应位置的语音动画播放
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //  Debug.LogWarning("玩家index：" + (pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(voices[0].iUserId)) - 1));
            int index = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(voices[0].iUserId)) - 1;
            UIMainView.Instance.PlayerPlayingPanel._playingHead[index].GetChild(4).GetComponent<Animator>().SetBool("isPlayOthers", true);
            //Debug.LogWarning("动画没问题");
            string ext = DateTime.Now.ToFileTime().ToString();
            if (VoiceFlag == 1)
            {
                string[] pathSplit = voices[0].szRecordUrlPath.Split('/');
                string localFilePath = isExistGvoiceFile(MahjongLobby_AH.SDKManager.DataPath) + pathSplit[pathSplit.Length - 1];
                int downLoadErr = m_voiceengine.DownloadRecordedFile(ByteArrayToHexString(voices[0].id), localFilePath, 6000);
                ShortTalkData std = GameData.Instance.ShortTalkData;
                if (!std._DownLoadFilePath.ContainsKey(voices[0].szRecordUrlPath))
                {
                    std._DownLoadFilePath.Add(voices[0].szRecordUrlPath, localFilePath);
                    Debug.LogWarning("下载地址1：" + voices[0].szRecordUrlPath);
                }
                Debug.LogWarning("下载地址2：" + localFilePath);

                m_voiceengine.OnPlayRecordFilComplete += (code, filePath) =>
                {
                    if (code != IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE)
                    {
                        Debug.Log("OnPlayRecordFilComplete error" + code);
                    }
                    else
                    {
                        Chenggong = true;
                        StartCoroutine(CompeletPlay());
                    }
                };
                ErroOccur(downLoadErr, () => { StartCoroutine(CompeletPlay()); });
            }
            else
            {
                YunVaImSDK.instance.RecordStartPlayRequest(urlLocal, voices[0].szRecordUrlPath, ext, (data2) =>
                {
                    Debug.LogWarning("data2.result:" + data2.result + "Chenggong:" + Chenggong);
                    if (data2.result == 0)
                    {
                        Chenggong = true;
                        StartCoroutine(CompeletPlay());
                    }
                    else
                    {
                        VoiceInit();
                        // StartCoroutine(CompeletPlay());
                        //Debug.LogWarning("播放失败");//关静音
                        Debug.LogWarning(voices[0].szRecordUrlPath);
                        MahjongCommonMethod.Instance.ShowRemindFrame("语音播放失败", true);
                    }
                });
            }
        }
        IEnumerator CompeletPlay(float time = -1)
        {
            yield return new WaitForSeconds(0);
            if (voices.Count > 0)
            {
                PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
                int index_pH = pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(voices[0].iUserId)) - 1;
                Debug.LogWarning("播放完成" + voices[0].iUserId + "index_pH:" + index_pH);

                if (index_pH >= 0 && index_pH <= 3)
                {
                    UIMainView.Instance.PlayerPlayingPanel._playingHead[index_pH].GetChild(4).GetComponent<Animator>().SetBool("isPlayOthers", false);
                }
                isPlayingState = false;//关闭播放状态
                voices.RemoveAt(0);
            }
            AudioListener.volume = 1;
            if (time > 0)
            {
                StopCoroutine(DealNoReasult(time));
            }
        }
        public string isExistGvoiceFile(string datapath)
        {
            string path = datapath;
            if (!Directory.Exists(datapath))
            {
                Directory.CreateDirectory(datapath);
            }
            return path;
        }
        bool iscanClick = true;
        /// <summary>
        /// 语音按钮按下
        /// </summary>
        public void OnVoiceBtnDown()
        {
            if (!iscanClick)
                return;
            Debug.LogWarning(isPlayingState + "  " + !isCompeletUpload + "   " + !isRecord);
            if (isPlayingState || !isCompeletUpload)
            {
                Debug.LogWarning("不能播放按下");
                return;
            }
            isRecord = true;
            filePath_1 =
                //Application.platform==RuntimePlatform.Android ?
                // "file://"+string.Format("{0}/{1}.amr", Application.persistentDataPath, DateTime.Now.ToFileTime() + GameData.Instance.PlayerNodeDef.iUserId):
                string.Format("{0}{1}.amr", isExistGvoiceFile(MahjongLobby_AH.SDKManager.DataPath), DateTime.Now.ToFileTime() + GameData.Instance.PlayerNodeDef.iUserId);

            Debug.Log("FilePath:" + filePath_1);
            AudioListener.volume = 0f;
            // yield return new WaitForSeconds(0.5f);
            int aa = 0;
            if (startwatch == null)
                startwatch = new System.Diagnostics.Stopwatch();
            //  Debug.LogWarning("按钮按下");
            startwatch.Reset();
            startwatch.Start();
            if (VoiceFlag == 1)
            {
                aa = m_voiceengine.StartRecording(filePath_1);
            }
            else
            {
                aa = YunVaImSDK.instance.RecordStartRequest(filePath_1, num);
            }
            if (aa != 0)
            {
                isRecordEnable = false;
                ErroOccur(aa, null);
                MahjongCommonMethod.Instance.ShowRemindFrame("使用录音权限已被禁用", true);
                return;
                // StopCoroutine(QuChuKaishiSheng());
            }
            else
            {
                Invoke("LimitTimeEnd", _fTimeLimit);
                isRecordEnable = true;
            }
            // Debug.LogError("语音"+aa);

            _voiceTrans.gameObject.SetActive(true);
            //播放动画
            startDotween();
            Vector3 v3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            tempPosition_y = v3.y;
            // StartCoroutine(QuChuKaishiSheng());
        }
        //IEnumerator QuChuKaishiSheng()
        //{

        //}
        /// <summary>
        /// 语音按钮释放
        /// </summary>
        public void OnVoiceBtnUp()
        {
            if (!iscanClick)
                return;

            //背景音乐开启
            stopDotween();
            _voiceTrans.gameObject.SetActive(false);
            _voiceObj.SetActive(false);
            //  Debug.LogWarning(isPlayingState + "  " + !isCompeletUpload+"   "+!isRecord );
            if (isPlayingState || !isCompeletUpload || !isRecord)
            {
                Debug.LogWarning("不能松开");
                return;
            }
            isRecord = false;//是否在录制
            AudioListener.volume = 1;
            if (isCancelRecord)
            {
                isCancelRecord = false;
                CancelRecord();
                return;
            }
            if (!isRecordEnable)
            {
                return;
            }
            Debug.LogWarning("按钮松开");
            if (isLimitTimeEnd)
            {
                Debug.LogWarning("由于限制时间到了释放按钮不执行");
                isLimitTimeEnd = false;
                return;
            }
            RecordStop();

        }
        bool isCompeletUpload = true;
        float tempPosition_y;
        bool isRecordEnable = false;
        #region 语音动画
        float number = 0;
        bool flagDoteen = false;
        bool isStart;
        void startDotween()
        {
            if (!isStart)
            {
                isStart = true;
                flagDoteen = true;
                Up();
            }
        }
        void stopDotween()
        {
            isStart = false;
            flagDoteen = false;
        }
        void Up()
        {
            if (flagDoteen)
            {
                Tween tween = DOTween.To(() => number, x => number = x, 1, 0.5f);
                tween.OnUpdate(() => UpdateTween());
                tween.OnComplete(() => Down());
            }

        }
        void Down()
        {
            if (flagDoteen)
            {
                Tween tween = DOTween.To(() => number, x => number = x, 0, 0.5f);
                tween.OnUpdate(() => UpdateTween());
                tween.OnComplete(() => Up());
            }
        }
        void UpdateTween()
        {
            _voiceImage.fillAmount = number;
        }
        #endregion 语音动画

        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(100,100,50,50),"播放"))
        //    {
        //        VoiceManegerMethord.Instance.voices.Add(new VoiceManegerMethord.UsersVoiceInfo(MahjongCommonMethod.Instance .iUserid , "", null));
        //    }
        //}
        /// <summary>
        /// 是否限超制时间
        /// </summary>
        bool isLimitTimeEnd;
        UsersVoiceInfo item = new UsersVoiceInfo();
        Action VoiceBtnUpAction;
        void LimitTimeEnd()
        {
            startwatch.Stop();
            MahjongCommonMethod.Instance.ShowRemindFrame("说话时间不能超过：" + _fTimeLimit + "s");
            Debug.LogError("限制时间到：" + startwatch.ElapsedMilliseconds);
            isLimitTimeEnd = true;
            RecordStop();
        }
        void CancelRecord()
        {
            _voiceObj.SetActive(false);
            _voiceTrans.gameObject.SetActive(false);
            CancelInvoke("LimitTimeEnd");
            startwatch.Stop();
            if (VoiceFlag == 1)
            {
                int err = m_voiceengine.StopRecording();
                if (err != 0)
                {
                    ErroOccur(err, null);
                }
            }
            else
            {
                YunVaImSDK.instance.RecordStopRequest(null);
            }
        }

        delegate void DelOnVoiceErr(int a);
        DelOnVoiceErr HandleUpLoadVoiceErr;
        IEnumerator LockButton()
        {
            iscanClick = false;
            if (VoiceFlag == 1)
            {
                m_voiceengine.StopRecording();
            }
            else
            {
                YunVaImSDK.instance.RecordStopRequest(null);
            }
            yield return new WaitForSeconds(3f);
            iscanClick = true;
        }
        long fVoiceTime;//录音时长
        void RecordStop()
        {
            //停止动画
            CancelInvoke("LimitTimeEnd");

            startwatch.Stop();
            fVoiceTime = startwatch.ElapsedMilliseconds;//录音时长

            if (fVoiceTime <= 500)
            {
                // Debug.LogWarning("录音时间太短");

                MahjongCommonMethod.Instance.ShowRemindFrame("录音时间太短");
                StartCoroutine(LockButton());
                return;
            }
            Debug.LogWarning("按钮松开5");
            // Debug.LogWarning("开始上传了");
            try
            {
                isCompeletUpload = false;
                if (VoiceFlag == 1)
                {
                    //停止录音
                    int err = m_voiceengine.StopRecording();
                    if (err != 0)
                        goto Err1;
                    //上传
                    err = m_voiceengine.UploadRecordedFile(filePath_1, 5500);
                    m_voiceengine.OnUploadReccordFileComplete += (code, path, id) =>
                    {
                        if (IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_UPLOAD_RECORD_DONE == code)
                        {
                            Debug.LogWarning("文件ID：" + id);
                            Debug.Log("返回" + path);
                            Debug.LogWarning("地址：" + filePath_1);
                            byte[] bId = HexStringToByteArray(id);
                            SendVoiceMessage(filePath_1, Convert.ToInt16(fVoiceTime), bId);
                        }
                        else
                        {
                            StartCoroutine(ListWork(fVoiceTime));
                        }
                    };
                    //停止出错
                    Err1: ErroOccur(err, () => { StartCoroutine(ListWork(fVoiceTime)); });
                }
                else
                {
                    YunVaImSDK.instance.RecordStopRequest(
                     (data1) =>
                         {
                             Debug.LogWarning("按钮松开6");
                             Debug.LogWarning(data1.strfilepath);
                             filePath_1 = data1.strfilepath;
                             StartCoroutine(ListWork(fVoiceTime));
                         },
                     (data2) =>//这里处理录音上传，上传录音网址告诉服务器
                       {
                           Debug.LogWarning("上传Result:" + data2.result);
                           if (data2.fileurl.Length <= 25)
                           {
                               Debug.LogWarning("上传语音失败,重新上传");
                               HandleUpLoadVoiceErr += UpLoadVoice;
                               HandleUpLoadVoiceErr(Convert.ToInt16(fVoiceTime));
                               return;
                           }
                           //发送语音请求
                           SendVoiceMessage(data2.fileurl, Convert.ToInt16(fVoiceTime), null);

                       },
                      (data3) =>
                       {
                           Debug.Log("识别返回:" + data3.text);
                       });
                }

            }
            catch (Exception)
            {
                Debug.LogError("上传有异常");
                throw;
            }
            //录音状态改编为非录音状态
        }
        IEnumerator ListWork(long fVoiceTime)
        {
            yield return new WaitForSeconds(1.5f);
            if (!isCompeletUpload)
            {
                isCompeletUpload = true;
                //HandleUpLoadVoiceErr += UpLoadVoice;
                // HandleUpLoadVoiceErr(Convert.ToInt16(fVoiceTime));
            }

        }
        /// <summary>
        /// 发送语音请求
        /// </summary>
        /// <param name="szRecordUrlPath"></param>

        void SendVoiceMessage(string szRecordUrlPath, int time, byte[] id)
        {
            Debug.LogWarning("上传成功开始发送给服务器");
            //录音请求消息
            //上传录音时长
            int iUserId = MahjongCommonMethod.Instance.iUserid;

            //发送网络消息、
            if (VoiceFlag == 0)
            {
                id = new byte[81];
            }
            NetMsg.ClientVoiceReq msg = new NetMsg.ClientVoiceReq(id);
            msg.iUserId = iUserId;
            msg.szUrl = szRecordUrlPath;
            msg.iDuration = time;
            NetworkMgr.Instance.GameServer.SendVoiceReq(msg);
            ChangeBool_isCompeletUpload();

            Debug.Log("发送成功:" + szRecordUrlPath);
        }
        /// <summary>
        /// 上传完成
        /// </summary>
        void ChangeBool_isCompeletUpload()
        {
            Debug.Log("111");
            for (int i = 0; i < 500; i++)
            {
                ; ;
            }
            Debug.Log("222");

            isCompeletUpload = true;
        }
        /// <summary>
        /// 重新上传语音消息
        /// </summary>
        /// <param name="aa"></param>
        public void UpLoadVoice(int aa)
        {
            Debug.LogWarning("____重传_____");
            HandleUpLoadVoiceErr -= UpLoadVoice;
            string fileId = DateTime.Now.ToFileTime().ToString();
            ChangeBool_isCompeletUpload();
            YunVaImSDK.instance.UploadFileRequest(filePath_1, fileId, (data1) =>
            {
                if (data1.result == 0)
                {
                    Debug.Log("重新上传成功:" + data1.fileurl);
                    SendVoiceMessage(data1.fileurl, aa, null);
                }
                else
                {
                    Debug.Log("重新上传失败:" + data1.msg);
                    MahjongCommonMethod.Instance.ShowRemindFrame("上传声音失败,请重新录制 ", true);
                }
            });
        }

        int voiceInitCount = 0;
        /// <summary>
        /// 录音初始化
        /// </summary>
        public void VoiceInit()
        {
            int a = 0;
            if (VoiceFlag == 1)
            {
                voiceInitCount++;
                a = m_voiceengine.SetAppInfo(MahjongLobby_AH.LobbyContants.GvoiceAPP_ID,
                      MahjongLobby_AH.LobbyContants.GvoiceAPP_Key,
                      GameData.Instance.PlayerNodeDef.iUserId.ToString());
                if (a != 0)
                    goto Err3;
                a = m_voiceengine.SetServerInfo("udp://cn.voice.gcloudcs.com:10001");
                if (a != 0)
                    goto Err3;
                a = m_voiceengine.Init();
                if (0 != a)
                    goto Err3;
                ///设置模式
                a = m_voiceengine.SetMode(GCloudVoiceMode.Messages);
                if (0 != a)
                    goto Err3;
                ApplyKey();
                Err3: ErroOccur(a, () => { VoiceInit(); });
            }
            else
            {
                IEVoiceInit();
            }
            if (voiceInitCount >= 4)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("错误：" + a + "请检查你的网络设置");
                voiceInitCount = 0;
            }
        }

        void ApplyKey()
        {
            int err = m_voiceengine.ApplyMessageKey(5500);
            switch ((GCloudVoiceErr)err)
            {
                case GCloudVoiceErr.GCLOUD_VOICE_PARAM_INVALID:
                    Debug.Log("传入的参数不对，比如超时范围5000ms-60000ms。");
                    break;
                case GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT:
                    Debug.LogError("需要执行初始化");
                    break;
                case GCloudVoiceErr.GCLOUD_VOICE_AUTHKEY_ERR:
                    Debug.LogError("请求Key的内部错误，此时需要联系GCloud团队，并提供日志进行定位");
                    MahjongCommonMethod.Instance.ShowRemindFrame("错误：" + err);
                    break;
                default:
                    break;
            }
        }
        void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_IPHONE || UNITY_ANDROID
            Debug.Log("Voice OnApplicationPause: " + pauseStatus);
            if (pauseStatus)
            {
                if (m_voiceengine == null)
                {
                    return;
                }
                m_voiceengine.Pause();

            }
            else
            {
                if (m_voiceengine == null)
                {
                    return;
                }
                int a = m_voiceengine.Resume();
                if (GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT == (GCloudVoiceErr)a)
                {
                    VoiceInit();
                }

            }
#endif
        }


        //“启动”手机时
        void OnApplicationFocus(bool hasFocus)
        {
#if UNITY_IPHONE || UNITY_ANDROID
            if (hasFocus)
            {
                // “启动”手机时，事件

            }

#endif

        }
        void IEVoiceInit()
        {
            // EventListenerManager.AddListener(ProtocolEnum.IM_RECORD_VOLUME_NOTIFY, ImRecordVolume);//录音音量大小回调监听
            int init = YunVaImSDK.instance.YunVa_Init(0, MahjongLobby_AH.LobbyContants.YvWaApp_Id, Application.persistentDataPath, false, false);
            if (init == 0)
            {
                Debug.Log("语音初始化成功");
                //  Loom.RunAsync(() =>
                //  {
                VoiceLogin();
                //  });
            }
            else
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("语音功能初始化失败，请重新进入房间", true);
            }
        }

        public void StopPlayingVoice()
        {
            MahjongCommonMethod mc = MahjongCommonMethod.Instance;//开静音
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (VoiceFlag == 1)
            {
                int err = m_voiceengine.StopPlayFile();
                if (err != 0)
                {
                    ErroOccur(err, null);
                }
            }
            else
            {
                YunVaImSDK.instance.RecordStopPlayRequest();
            }
            UIMainView.Instance.PlayerPlayingPanel._playingHead[pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(voices[0].iUserId)) - 1].GetChild(4).GetComponent<Animator>().SetBool("isPlayOthers", false);
            isPlayingState = false;//关闭播放状态
                                   // mc.isMusicShut = false;
            if (voices.Count > 0)
            {
                voices.RemoveAt(0);
            }
            // SystemMgr.Instance.BgmSystem.UpdateVolume();
        }
        //录音音量大小回调监听
        public void ImRecordVolume(object data)
        {
            ImRecordVolumeNotify RecordVolumeNotify = data as ImRecordVolumeNotify;
            _voiceImage.fillAmount = (RecordVolumeNotify.v_volume + 5) * 0.01f;
        }
        public void VoiceLogin()
        {
            ///
            // return;
            // Debug.LogError("云哇登录serverID " + MahjongCommonMethod.Instance.serverID);
            string ttFormat = "{{\"nickname\":\"{0}\",\"uid\":\"{1}\"}}";
            string nickName = GameData.Instance.PlayerNodeDef.szNickname;
            int id = GameData.Instance.PlayerNodeDef.iUserId;
            if (string.IsNullOrEmpty(nickName))
            {
                nickName = "szNickname";
            }
            if (id <= 0)
            {
                id = GameData.Instance.PlayerNodeDef.iUserId;
            }
            string tt = string.Format(ttFormat, nickName, id);
            Debug.LogWarning("语音：" + tt);
            // return;
            string[] wildcard = new string[2];
            wildcard[0] = "0x001";
            wildcard[1] = "0x002";
            PlayerPlayingPanelData ppd = GameData.Instance.PlayerPlayingPanelData;

            //  Debug.LogWarning("====================================0");
            YunVaImSDK.instance.YunVaOnLogin(tt, MahjongCommonMethod.Instance.serverID, wildcard, 0, (data) =>
            {
                if (data.result == 0)
                {
                    Debug.Log("语音登录成功");
                    ppd.isVoiceLogin = true;
                    //YunVaImSDK.instance.RecordSetInfoReq(true);//开启录音的音量大小回调
                }
                else
                {
                    Debug.LogError("语音登录失败");
                    ppd.isVoiceLogin = false;
                    VoiceInit();
                    MahjongCommonMethod.Instance.ShowRemindFrame("语音登录失败，请重新进入房间");
                }
            });

            // Debug.LogError("====================================1");
        }

        private IEnumerator AutoLogin()
        {
            yield return null;

            while (!GameData.Instance.PlayerPlayingPanelData.isVoiceLogin)
            {
                yield return new WaitForSeconds(3);
                VoiceInit();
            }
            yield break;

        }
        public byte[] HexStringToByteArray(string s)
        {
            if (s.Length == 0)
                Debug.Log("将16进制字符串转换成字节数组时出错，错误信息：被转换的字符串长度为0。");
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        public static string ByteArrayToHexString(byte[] a)
        {
            string str = null;
            for (int i = 0; i < a.Length; i++)
            {
                str += a[i].ToString("x2");
            }
            Debug.Log("16进制转换string" + str);
            return str;
        }
        public void ErroOccur(int err, Action action)
        {
            switch (err)
            {
                case (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC:
                    Debug.Log("成功");
                    break;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT:
                    VoiceInit();
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_MODE_STATE_ERR:
                    Debug.LogError("当前模式不是离线语音模式");
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_PARAM_INVALID:
                    Debug.LogError("传入的参数不对，路径为空");
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_AUTHKEY:
                    Debug.LogError("需要先调用GetAuthKey申请许可");
                    ApplyKey();
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_PATH_ACCESS_ERR:
                    Debug.LogError("提供的路径不合法或者不可写");
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_HTTP_BUSY:
                    Debug.LogError("还在上一次上传或者下载中，需要等待后再尝试");
                    goto cc;
                case (int)GCloudVoiceErr.GCLOUD_VOICE_SPEAKER_ERR:
                    Debug.LogError("打开麦克风失败");
                    goto cc;
                default:
                    Debug.LogWarning("未知错误:" + err);
                    cc: if (action != null)
                        action();
                    break;
            }
        }
    }
}
