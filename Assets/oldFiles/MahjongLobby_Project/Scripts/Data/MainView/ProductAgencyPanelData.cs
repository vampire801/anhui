using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class ProductAgencyPanelData
    {
        public bool PanelShow;  //面板显示的标志位    
        public int index;  //表示要显示的面板的下标  
        public bool IsShowCompany;  //公司信息面板显示的标志位        
        public int iProxyId;
        public string szNickname=null;
        public string szHeadimgurl = "";
        public string wx;  //微信号
        public string szProxyComment;  //代理留言
        public bool isShowSecondUnbind;  //是否关闭二次确认申请解绑面板的标志位
    }

}
