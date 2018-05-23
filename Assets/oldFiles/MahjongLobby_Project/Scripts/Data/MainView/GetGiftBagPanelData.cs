using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class GetGiftSpreadBagPanelData
    {
        public bool PanelShow;   //面板是否显示的标志位
        public string ActivityKey;  //激活码信息
        public bool isShowCode;  //是否显示解释推广码界面
        public string GiftSpreadCode=""; //推广码
        public bool isShared;//是否被推广
        public bool isSavedGift;//是否领取过礼包
        public string szUrlHead;//头像地址
        public string szNickName;//推广员昵称
    }
}

