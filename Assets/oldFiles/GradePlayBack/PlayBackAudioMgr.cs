using UnityEngine;
using System.Collections;
using XLua;

namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayBackAudioMgr : MonoBehaviour
    {
        static PlayBackAudioMgr instance;
        public static PlayBackAudioMgr Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public enum AudioType
        {
            /// <summary>
            /// 停止
            /// </summary>
            AUDIO_NONE = -1, //停止

            WAN1 = 0, WAN2 = 1, WAN3 = 2, WAN4 = 3, WAN5 = 4, WAN6 = 5, WAN7 = 6, WAN8 = 7, WAN9 = 8,
            TONG1 = 9, TONG2 = 10, TONG3 = 11, TONG4 = 12, TONG5 = 13, TONG6 = 14, TONG7 = 15, TONG8 = 16, TONG9 = 17,
            TIAO1 = 18, TIAO2 = 19, TIAO3 = 20, TIAO4 = 21, TIAO5 = 22, TIAO6 = 23, TIAO7 = 24, TIAO8 = 25, TIAO9 = 26,
            FenE = 27, FenS = 28, FenW = 29, FenN = 30, ZHONG = 31, FA = 32, BAI = 33,
            Hu = 34, BuHua = 35, Hu_Zimo = 36, Chi = 37, Peng = 38, Gang = 39, Ting = 40, Qiang = 41, DajiaHao, Kuaidianba, Jiaopen,
            Paitaihao, Buhaoyisi, WoHule, Duanxian, Chiyige, Hehe, Xiaci

        }
        public enum AudioMenel
        {
            Audio_None = -1,
            Win = 0,
            Lose = 1,
            Card_Darping = 2,
            Card_Dealing = 3,
            Time_Warning = 4,
            Card_Chage = 5,
            Card_Putout = 6,
            Card_Down_Win = 7,
            Card_Ordering = 8,
            Btn_Click = 9,
            card_sound = 10,
            pick_card = 11,
            join_match = 12,
            leave_game = 13

        }


        public AudioType GetAudioSourceIndex(byte cTitle)
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
            return (AudioType)index;
        }

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
        bool debugEnable = false; //是否记录调试信息
        #endregion 私有成员变量     

        void Update()
        {
            Init();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            OnUpdateVolume(anhui.MahjongCommonMethod.Instance.isEfectShut, (int)anhui.MahjongCommonMethod.Instance.EffectValume);
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
        }

        #region 消息处理函数

        /// <summary>
        /// 处理自动播放音效
        /// </summary>
        /// <param name="audio">播放类型</param>
        /// <param name="index">0-未选择 1-男 2-女</param>   
        public void OnPlayAuto(AudioType audio, int index)
        {
            if (index != 1)
            {
                index = 0;
            }
            if (debugEnable)
            {
                Debug.Log("AudioMage.OnPlayAuto," + audio);
            }
            if (audio == AudioType.AUDIO_NONE)
            {
                return;
            }
            int audioIndex = (int)audio;
            if (AutoSources[index][audioIndex].clip != null)
            {
                for (int i = 0; i < AutoSources.Length; i++)
                {
                    if (!AutoSources[index][audioIndex].isPlaying)
                    {
                        AutoSources[index][audioIndex].loop = false;
                        AutoSources[index][audioIndex].Play();
                        break;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 处理手动播放音效
        /// </summary>
        /// <param name="audio">指定音效</param>
        /// <param name="loop">是否循环播放</param>
        public void OnPlayManual(AudioMenel audio, bool isStop, bool loop)
        {
            if (debugEnable)
            {
                DEBUG.Graphics("AudioManage.OnPlayManual, " + audio + ", " + loop);
            }
            int audioIndex = (int)audio;
            if (isStop)
            {
                Debug.LogWarning("停止播放");
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
        public void OnUpdateVolume(bool soundOn, int soundVolume)
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
                for (int j = 0; j < AutoSources[i]._myAudio.Length; j++)
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
