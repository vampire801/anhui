using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameConstants
    {
        public const int messageCount = 20;   //每帧发消息的限制个数

        public static Vector3 vTableShowPos = new Vector3(-180f, 140f, 0);

        //保存玩家打出的麻将在桌面显示的位置
        public static Vector3[] showCardPos = new Vector3[]
        {
            new Vector3(18,-22f,0),
            new Vector3(945f,245f,0),
            new Vector3(0,0,0),
            new Vector3(-945f,245f,0)
        };

        /// <summary>
        /// 等待界面的位置
        /// </summary>
        public static Vector3[] headPos_wait = new Vector3[]
        {
            new Vector3(75f,130f,0),
            new Vector3(-132f,10f,0),
            new Vector3(20f,-100f,0),
            new Vector3(140,5f,0)
        };

        /// <summary>
        /// 游戏界面的位置
        /// </summary>
        public static Vector3[] headPos_play = new Vector3[]
       {
            new Vector3(-480f,200f,0),
            new Vector3(-65f,25f,0),
            new Vector3(-275f,0,0),
            new Vector3(75f,25f,0)
       };
    }
}

