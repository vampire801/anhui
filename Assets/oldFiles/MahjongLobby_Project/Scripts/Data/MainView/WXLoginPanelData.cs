using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public  class WXLoginPanelData
    {
        public bool isBtnOk;

        public bool isTest;
        public bool isPanelShow;

        public bool isCloseDisConnectPanel;  //关闭网络连接的面板的标志位


        public bool isClickLogin; //是否已经点击过登录按钮

        public bool isAgreUserRule=true;  //同意用户协议的标志,默认同意
        /// <summary>
        /// 登录点击
        /// </summary>
        public int AuthonState;
    }
}
