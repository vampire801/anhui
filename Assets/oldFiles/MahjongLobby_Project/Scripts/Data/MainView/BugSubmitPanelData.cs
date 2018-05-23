using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class CustomPanelData
    {
        public const int CustomBtnNumber = 2;
        public bool PanelShow;  //是否显示面板的标志位
        public bool isPanelWtWx;  //显示什么是公众号的标志位
        public bool isShowMethodCollect; //是否显示收集玩法的标志位
        public bool isShowCustomSever; //是否客服面板一个标志位
        internal float downPos;
        internal float oriPos;
        internal int indexCurrent=1;

        //public class BtnState
        //{

        //}
    }

}
