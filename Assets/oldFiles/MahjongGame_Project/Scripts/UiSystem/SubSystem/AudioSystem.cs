using UnityEngine;
using System.Collections;
using MahjongGame_AH;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class AudioSystem : GameSystemBase
    {

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
            Hu = 34, BuHua = 35, Hu_Zimo = 36, Chi = 37, Peng = 38, Gang = 39, Ting = 40, Qiang = 41,DajiaHao, Kuaidianba , Jiaopen,
            Paitaihao, Buhaoyisi, WoHule, Duanxian, Chiyige, Hehe,  Xiaci

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
            Btn_Click=9,
            card_sound=10,
            pick_card=11,
            join_match=12,
            leave_game=13

        }


        #region 用户界面要处理的事件
        /// <summary>
        /// 通知音效自动播放
        /// </summary>
        /// <param name="audio">播放音效类型</param>
        /// <param name="index">默认播放女声0,2，index==1播放男声</param>
        public delegate void PlayAutoEventHandler(AudioType audio, int index);
        public PlayAutoEventHandler OnPlayAuto;
        // public event PlayAutoEventHandler OnPlayAuto;

        /// <summary>
        /// 通知音效手动播放
        /// </summary>
        /// <param name="audio">播放音效类型</param>
        /// <param name="loop">是否循环</param>
        public delegate void PlayManualEventHandler(AudioMenel audio, bool isStop, bool loop);
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
        public void PlayAuto(AudioType audio, int index)
        {
            if (OnPlayAuto != null)
            {
                OnPlayAuto(audio, index);
            }
        }
        /// <summary>
        /// 手动播放音效
        /// </summary>
        /// <param name="audio">指定音效</param>
        /// <param name="loop">是否循环播放</param>
        public void PlayManual(AudioSystem.AudioMenel audio, bool isStop, bool loop)
        {
            if (OnPlayManual != null)
            {
                OnPlayManual(audio, isStop, loop);
            }
        }

        /// <summary>
        /// 更新音效的音量
        /// </summary>
        public void UpdateVolume()
        {
            anhui.MahjongCommonMethod  gd = anhui.MahjongCommonMethod.Instance;
            bool soundOn = gd.isEfectShut   ;
            float  soundVolume = gd.EffectValume;

            if (OnUpdateVolume != null)
            {
                OnUpdateVolume(soundOn, (int )soundVolume);
            }
        }


        #endregion 公共成员方法
    }

}
