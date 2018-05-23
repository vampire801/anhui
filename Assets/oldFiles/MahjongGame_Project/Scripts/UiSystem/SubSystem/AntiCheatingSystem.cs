using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class AntiCheatingSystem 
    {
        //处理面板更新
        public delegate void AntiCheatingUpdateEventHandler();
        public AntiCheatingUpdateEventHandler OnAntiCheatingUpdate;


        public void UpdateShow()
        {
            if (OnAntiCheatingUpdate != null)
            {
                OnAntiCheatingUpdate();
            }
        }

    }

}
