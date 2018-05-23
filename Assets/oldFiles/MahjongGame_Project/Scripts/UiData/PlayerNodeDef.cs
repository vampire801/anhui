using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerNodeDef
    {
        public int iUserId;            //用户编号                
        public string szNickname;      // 微信昵称
        public string szHeadimgurl;   // 微信头像网址       
        public byte byLaiziSuit; //  癞子的花色
        public byte byLaiziValue; // 癞子的牌值
        public PlayerNodeDef()
        {

        }
        public PlayerNodeDef(int userId, string Nickname, string headimaurl)
        {
            iUserId = userId; szNickname = Nickname; szHeadimgurl = headimaurl;          
        }
    }
}

