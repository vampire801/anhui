using UnityEngine;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class AudioSystem :GameSystemBase 
    {
        public enum AudioType
        {
            /// <summary>
            /// 停止
            /// </summary>
            AUDIO_NONE = -1,
            VIEW_CLOSE = 0,//点击音

        }


        #region 私有成员变量

        bool debugEnable = false; //是否记录调试信息
        #endregion 私有成员变量

        #region 用户界面要处理的事件
        /// <summary>
        /// 通知音效自动播放
        /// </summary>
        /// <param name="audio">播放音效类型</param>
        public delegate void PlayAutoEventHandler(AudioType audio);
        public event PlayAutoEventHandler OnPlayAuto;

        /// <summary>
        /// 通知音效手动播放
        /// </summary>
        /// <param name="audio">播放音效类型</param>
        /// <param name="index">播放音效索引</param>
        /// <param name="loop">是否循环</param>
        public delegate void PlayManualEventHandler(AudioType audio, int index, bool loop);
        public event PlayManualEventHandler OnPlayManual;

        /// <summary>
        /// 通知更新音效音量
        /// </summary>
        /// <param name="soundOn">是否开启声音</param>
        /// <param name="soundVolume">声音大小</param>
        public delegate void UpdateVolumeEventHandler(bool soundOn, int soundVolume);
        public event UpdateVolumeEventHandler OnUpdateVolume;
        #endregion 用户界面要处理的事件

        #region 公共成员方法

        /// <summary>
        /// 播放指定音效
        /// </summary>
        /// <param name="audio"></param>
        public void PlayAuto(AudioType audio)
        {
            if (debugEnable)
            {
                Debug.LogError("AudioSystem.PlayAudio, " + audio);
            }
            if (OnPlayAuto != null)
            {
                OnPlayAuto(audio);
            }
        }
        /// <summary>
        /// 手动播放音效
        /// </summary>
        /// <param name="audio">指定音效</param>
        /// <param name="index">指定ManualSources索引</param>
        /// <param name="loop">是否循环播放</param>
        public void PlayManual(AudioSystem.AudioType audio, int index, bool loop)
        {
            if (debugEnable)
            {
                Debug.LogError("AudioSystem.PlayerAudio, " + audio);
            }
            if (OnPlayManual != null)
            {
                OnPlayManual(audio, index, loop);
            }
        }

        /// <summary>
        /// 更新音效的音量
        /// </summary>
        public void UpdateVolume()
        {
            anhui.MahjongCommonMethod  gd = anhui.MahjongCommonMethod.Instance;

            bool soundOn = gd.isEfectShut ;
            int soundVolume = gd.EffectValume ;
            if (OnUpdateVolume != null)
            {
                OnUpdateVolume(soundOn, soundVolume);
            }
        }

        #endregion 公共成员方法
    }
}

