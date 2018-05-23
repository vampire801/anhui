using UnityEngine;
using MahjongLobby_AH.LobbySystem;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
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
            HAPPY = 0,
            NOISY = 1,
            LEISURE = 2,
            EASY = 3,
            PATYEARCALL=4,
            HaoFengGuang=5


        }
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
            anhui.MahjongCommonMethod gd = anhui.MahjongCommonMethod.Instance;
            if (OnUpdateVolume != null)
            {
                OnUpdateVolume(gd .isMusicShut, gd .MusicVolume);
            }
        }
        #endregion 公共成员方法
    }

}
