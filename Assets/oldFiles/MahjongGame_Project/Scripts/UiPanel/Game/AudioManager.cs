using UnityEngine;
using System.Collections;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class AudioManager : MonoBehaviour
    {

        #region 公共成员变量
        /// <summary>
        /// 要加载的音效资源
        /// </summary>
        //  public AudioSource[] ResAudios;

        public AudioClip[] AudioClips;


        /// <summary>
        /// 自动控制的音源
        /// </summary>
        public MyAudioSource[] AutoSources = new MyAudioSource[2];

        public AudioSource[] ManualSources;

        #endregion 公共成员变量



        #region 私有成员变量
        //  int loaadIndex; //资源加载的索引
        bool debugEnable = false; //是否记录调试信息
        #endregion 私有成员变量

        void Awake()
        {
            //  DontDestroyOnLoad(gameObject);
        }

        void Update()
        {

            if (MahjongGame_AH.SystemMgr.Instance.Inited)
            {
                BgAudioManage.instance.Init();
                Init();
                MahjongGame_AH.SystemMgr.Instance.Inited = false;
            }
        }
        void OnEnable()
        {
            if (SystemMgr.Instance!=null&& SystemMgr.Instance.AudioSystem.OnPlayAuto !=null )
            {
                SystemMgr.Instance.AudioSystem.OnPlayAuto -= OnPlayAuto; //收到通知自动播放音效
                SystemMgr.Instance.AudioSystem.OnPlayManual -= OnPlayManual; //收到通知手动播放音效
                SystemMgr.Instance.AudioSystem.OnUpdateVolume -= OnUpdateVolume;

            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            Debug.LogWarning("音效声音初始化");
            if (debugEnable)
            {
                Debug.Log("AudioMsg.Init");
            }
            OnUpdateVolume(anhui.MahjongCommonMethod.Instance.isEfectShut ,(int )anhui.MahjongCommonMethod .Instance .EffectValume );
            SystemMgr.Instance.AudioSystem.OnPlayAuto += OnPlayAuto; //收到通知自动播放音效
            SystemMgr.Instance.AudioSystem.OnPlayManual += OnPlayManual; //收到通知手动播放音效
            SystemMgr.Instance.AudioSystem.OnUpdateVolume += OnUpdateVolume;
        }
        /// <summary>
        /// 当应用程序退出
        /// </summary>
        public void OnApplicationQuit()
        {
            for (int j = 0; j < AutoSources.Length; j++)
            {
                for (int i = 0; i < AutoSources[j]._myAudio.Length; i++)
                {
                    AutoSources[j][i].Stop();
                }
            }
            for (int i = 0; i < ManualSources.Length; i++)
            {
                ManualSources[i].Stop();
            }
            //for (int i = 0; i < AudioClips.Length; i++)
            //{
            //    AudioClips[i] = null;
            //}
          
        }

        #region 消息处理函数

        /// <summary>
        /// 处理自动播放音效
        /// </summary>
        /// <param name="audio">播放类型</param>
        /// <param name="index">0-未选择 1-男 2-女</param>   
        void OnPlayAuto(AudioSystem.AudioType audio, int index)
        {
            if (index!=1)
            {
                index = 0;
            }
            if (debugEnable)
            {
                Debug.Log("AudioMage.OnPlayAuto," + audio);
            }
            if (audio == AudioSystem.AudioType.AUDIO_NONE)
            {
                return;
            }
            int audioIndex = (int)audio;
            // if (AudioClips[audioIndex] != null)
            if (AutoSources[index][audioIndex].clip != null)
            {
                bool bPlayed = false;
                for (int i = 0; i < AutoSources.Length; i++)
                {
                    //if (audioIndex < 19)
                    //{
                    //    if (!AutoSources[index][i].isPlaying)
                    //    {
                    //        AutoSources[index][i].clip = AudioClips[audioIndex];
                    //        AutoSources[index][i].loop = false;
                    //        AutoSources[index][i].Play();
                    //        bPlayed = true;
                    //        break;
                    //    }
                    //}
                    //else
                    //{
                    if (!AutoSources[index][audioIndex].isPlaying)
                    {
                        // AutoSources[index][audioIndex].clip = AudioClips[audioIndex];
                        AutoSources[index][audioIndex].loop = false;
                        AutoSources[index][audioIndex].Play();
                        bPlayed = true;
                        break;
                    }
                    else
                    {
                        return;
                    }
                    //}
                }

                if (!bPlayed)
                {
                    DEBUG.Audio("AudioType " + audio + " is not played. All AutoSources are used. ", LogType.Warning);
                    for (int i = 0; i < AutoSources.Length; i++)
                    {
                        DEBUG.Audio("AudioType " + AutoSources[index][i].clip.name, LogType.Warning);
                    }
                }
            }
            else
            {
                DEBUG.Audio("AudioType " + audio + " is null", LogType.Warning);
            }

        }

        /// <summary>
        /// 处理手动播放音效
        /// </summary>
        /// <param name="audio">指定音效</param>
        /// <param name="loop">是否循环播放</param>
        void OnPlayManual(AudioSystem.AudioMenel audio, bool isStop, bool loop)
        {
            if (debugEnable)
            {
                DEBUG.Graphics("AudioManage.OnPlayManual, " + audio + ", " + loop);
            }
            int audioIndex = (int)audio;
            if (isStop)
            {
                Debug.LogError("停止播放");
                ManualSources[audioIndex].Stop();
                return;
            }
            if (ManualSources[audioIndex] != null)
            {
                ManualSources[audioIndex].Stop();
                ManualSources[audioIndex].loop = loop;
                ManualSources[audioIndex].Play();
            }
        }

        /// <summary>
        /// 更新音量
        /// </summary>
        /// <param name="soundOn">是否关闭声音</param>
        /// <param name="soundVolume">音量</param>
        void OnUpdateVolume(bool soundOn, int soundVolume)
        {
            SetVolume(soundOn, soundVolume * 0.01f);
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">音量</param>
        void SetVolume(bool soundOn, float volume)
        {
            for (int i = 0; i < AutoSources.Length; i++)
            {
                for (int j = 0; j < AutoSources[i]._myAudio .Length ; j++)
                {
                    AutoSources[i][j].mute = soundOn;
                    AutoSources[i][j].volume = volume;
                }
            }
            for (int i = 0; i < ManualSources.Length; i++)
            {
                ManualSources[i].mute = soundOn;
                ManualSources[i].volume = volume;
            }
            //NGUITools.soundVolume = volume;
        }

        #endregion 消息处理函数
    }
    [System.Serializable]
    public class MyAudioSource
    {
        public AudioSource[] _myAudio;
        public AudioSource this[int index]
        {
            get { return _myAudio[index]; }
        }
        public MyAudioSource()
        {
            this._myAudio = new AudioSource[4];
        }
        public MyAudioSource(int index)
        {
            this._myAudio = new AudioSource[index];
        }
    }
}

