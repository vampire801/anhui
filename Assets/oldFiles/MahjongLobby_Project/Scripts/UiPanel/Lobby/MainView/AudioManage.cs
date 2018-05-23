using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class AudioManage : MonoBehaviour
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
        public AudioSource[] AutoSources;

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
            if (SystemMgr.Instance)
            {
                if (SystemMgr.Instance.Inited)
                {
                    BgAudioManage.instance.Init();
                    Init();
                    SystemMgr.Instance.Inited = false;
                }
            }

        }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            //  Debug.LogWarning("音效声音初始化");
            if (debugEnable)
            {
                Debug.Log("AudioMsg.Init");
            }
            OnUpdateVolume(anhui.MahjongCommonMethod .Instance.isEfectShut , anhui.MahjongCommonMethod.Instance.EffectValume );
            SystemMgr.Instance.AudioSystem.OnPlayAuto += OnPlayAuto; //收到通知自动播放音效
            SystemMgr.Instance.AudioSystem.OnPlayManual += OnPlayManual; //收到通知手动播放音效
            SystemMgr.Instance.AudioSystem.OnUpdateVolume += OnUpdateVolume;
        }
        /// <summary>
        /// 当应用程序退出
        /// </summary>
        public void OnApplicationQuit()
        {
            for (int i = 0; i < AutoSources.Length; i++)
            {
                if (AutoSources[i] != null)
                {
                    AutoSources[i].Stop();
                }
            }
            for (int i = 0; i < ManualSources.Length; i++)
            {
                if (ManualSources[i] != null)
                {
                    ManualSources[i].Stop();

                }
            }
            for (int i = 0; i < AudioClips.Length; i++)
            {
                AudioClips[i] = null;
            }

            SystemMgr.Instance.AudioSystem.OnPlayAuto -= OnPlayAuto; //收到通知自动播放音效
            SystemMgr.Instance.AudioSystem.OnPlayManual -= OnPlayManual; //收到通知手动播放音效
            SystemMgr.Instance.AudioSystem.OnUpdateVolume -= OnUpdateVolume;

        }

        #region 消息处理函数

        /// <summary>
        /// 处理自动播放音效
        /// </summary>
        /// <param name="audio"></param>
        void OnPlayAuto(AudioSystem.AudioType audio)
        {
            if (debugEnable)
            {
                Debug.Log("AudioMage.OnPlayAuto," + audio);
            }
            if (audio == AudioSystem.AudioType.AUDIO_NONE)
            {
                return;
            }
            int audioIndex = (int)audio;
            if (AudioClips[audioIndex] != null)
            {
                bool bPlayed = false;
                for (int i = 0; i < AutoSources.Length; i++)
                {
                    if (!AutoSources[i].isPlaying)
                    {
                        AutoSources[i].clip = AudioClips[audioIndex];
                        AutoSources[i].loop = false;
                        AutoSources[i].Play();
                        bPlayed = true;
                        break;
                    }
                }

                if (!bPlayed)
                {
                    DEBUG.Audio("AudioType " + audio + " is not played. All AutoSources are used. ", LogType.Warning);
                    for (int i = 0; i < AutoSources.Length; i++)
                    {
                        DEBUG.Audio("AudioType " + AutoSources[i].clip.name, LogType.Warning);
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
        /// <param name="index">指定ManualSources索引</param>
        /// <param name="loop">是否循环播放</param>
        void OnPlayManual(AudioSystem.AudioType audio, int index, bool loop)
        {
            if (debugEnable)
            {
                DEBUG.Graphics("AudioManage.OnPlayManual, " + audio + ", " + index + ", " + loop);
            }
            if (audio == AudioSystem.AudioType.AUDIO_NONE)
            {
                ManualSources[index].Stop();
                return;
            }

            int audioIndex = (int)audio;
            if (AudioClips[audioIndex] != null)
            {
                ManualSources[index].Stop();
                ManualSources[index].clip = AudioClips[audioIndex];
                ManualSources[index].loop = loop;
                ManualSources[index].Play();
            }
        }

        /// <summary>
        /// 更新音量
        /// </summary>
        /// <param name="soundOn">是否关闭声音</param>
        /// <param name="soundVolume">音量</param>
        void OnUpdateVolume(bool soundOn, int soundVolume)
        {
            SetVolume(soundOn, soundVolume / 100.0f);
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">音量</param>
        void SetVolume(bool ison, float volume)
        {
            for (int i = 0; i < AutoSources.Length; i++)
            {
                AutoSources[i].mute = ison;
                AutoSources[i].volume = volume;
            }
            for (int i = 0; i < ManualSources.Length; i++)
            {
                ManualSources[i].mute = ison;
                ManualSources[i].volume = volume;
            }

            //NGUITools.soundVolume = volume;

        }

        #endregion 消息处理函数
    }

}
