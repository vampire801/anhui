using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class SetPanelData
    {
        /// <summary>
        /// 面板是否显示
        /// </summary>
        public bool PanelShow;
        /// <summary>
        /// 画面品质
        /// </summary>
        public int Quality;
        /// <summary>
        /// 音效声音音量
        /// </summary>
        public int SoundVolume = 100;
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public int MusicVolume = 40;
    }

}
