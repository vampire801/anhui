using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class ActiveGiftPanelData
    {
        /// <summary>
        /// 领取成功提示
        /// </summary>
        public bool istipsShow;
        /// <summary>
        /// 提示内容
        /// </summary>
        public string sTipsFiled;
    }
}
