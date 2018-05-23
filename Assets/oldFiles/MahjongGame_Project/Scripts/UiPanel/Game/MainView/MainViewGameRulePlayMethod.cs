using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewGameRulePlayMethod : MonoBehaviour
    {
        public Text Name;//这一行的名称
        public Transform Parent;//父物体

        public GameObject GameRuleName;//显示的勾选内容
        public GameObject GameRuleGold;//需要的房卡数量
    }
}
