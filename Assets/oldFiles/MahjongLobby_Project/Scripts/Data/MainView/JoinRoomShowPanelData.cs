using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class JoinRoomShowPanelData 
    {
        public bool PanelShow;  //面板是否显示的标志位
        public string RoomId="";   //玩家输入的房间号
        public List<char> lsRoomId = new List<char>();  //玩家输入的房间号,最长6位       
    }

}
