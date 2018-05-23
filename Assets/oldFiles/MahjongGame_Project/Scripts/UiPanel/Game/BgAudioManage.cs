using UnityEngine;
using System.Collections;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class BgAudioManage : MonoBehaviour
    {
        public static BgAudioManage instance;
        /// <summary>
        /// 要加载的音乐资源
        /// </summary>
        #region 公共成员变量

        public AudioSource[] Bgms;


        #endregion 公共成员变量


        void Awake()
        {
            instance = this;
        }


        public void Init()
        {
           // Debug.LogWarning("背景声音初始化");
          
            OnUpdateVolume(anhui.MahjongCommonMethod.Instance.isMusicShut ,(int)anhui.MahjongCommonMethod.Instance.MusicVolume );
            SystemMgr.Instance.BgmSystem.OnPlayBgm += OnPlayBgm; //收到通知播放背景音乐
            SystemMgr.Instance.BgmSystem.OnUpdateVolume += OnUpdateVolume; //更新音量

        }
        #region 消息处理函数
        /// <summary>
        /// 处理播放背景音乐
        /// </summary>
        /// <param name="bgm">指定背景音乐</param>
        /// <param name="loop">是否循环播放</param>
        void OnPlayBgm(BgAudioManageSystem.BgmType bgm, bool loop)
        {
            if (bgm == BgAudioManageSystem.BgmType.BGM_NONE)
            {
                GetComponent<AudioSource>().Stop();
                return;
            }

            int index = (int)bgm;
            if (Bgms[index] != null)
            {
                GetComponent<AudioSource>().clip = Bgms[index].clip;
                GetComponent<AudioSource>().loop = loop;
                GetComponent<AudioSource>().Play();
            }
        }

        /// <summary>
        /// 更新音量
        /// </summary>
        /// <param name="soundOn">是否开启声音</param>
        /// <param name="musicVolume">音量</param>
        void OnUpdateVolume(bool soundOn, int musicVolume)
        {
            GetComponent<AudioSource>().mute = soundOn;
            GetComponent<AudioSource>().volume = musicVolume *0.01f ;
        }

        #endregion 消息处理函数
    }

}
