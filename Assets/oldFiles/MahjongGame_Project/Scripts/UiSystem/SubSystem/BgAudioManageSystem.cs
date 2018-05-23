using UnityEngine;
using MahjongGame_AH;
using System.Collections;
using MahjongGame_AH.Data;
using MahjongGame_AH.GameSystem;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class BgAudioManageSystem : GameSystemBase
    {
        public enum BgmType
        {
            /// <summary>
            /// 停止
            /// </summary>
            BGM_NONE = -1, //停止    
            BGM_1=0,//游戏背景声音     
            HerdingSheeps=1                 
        }

        #region 私有成员变量

        bool debugEnable = false; //是否几率调试信息
        #endregion 私有成员变量

        #region 用户界面要处理的事件
        public delegate void PlayBgmEventHandler(BgmType bgm, bool loop);
        public event PlayBgmEventHandler OnPlayBgm;

        public delegate void UpdateVolumeEventHandler(bool soundOn, int musicVolume);
        public event UpdateVolumeEventHandler OnUpdateVolume;

        #endregion 用户界面要处理的事件

        #region 公共成员方法

        /// <summary>
        /// 播放指定背景音乐
        /// </summary>
        /// <param name="bgm">指定背景音乐</param>
        /// <param name="loop">是否循环</param>
        public void PlayBgm(BgmType bgm, bool loop)
        {
            if (debugEnable)
            {
                Debug.Log("BgAudioManageSystem.PlayBgm");
            }

            if (OnPlayBgm != null)
            {
                OnPlayBgm(bgm, loop);
            }

        }

        /// <summary>
        /// 更新音乐的音量
        /// </summary>
        public void UpdateVolume()
        {

            anhui.MahjongCommonMethod  gd = anhui.MahjongCommonMethod.Instance;
            bool soundOn = gd.isMusicShut ;
            //Debug.LogWarning("执行3"+ soundOn);
            int musicVolume = gd.MusicVolume;
            if (OnUpdateVolume != null)
            {
                OnUpdateVolume(soundOn, musicVolume);
            }
        }

        #endregion 公共成员方法

    }
}

