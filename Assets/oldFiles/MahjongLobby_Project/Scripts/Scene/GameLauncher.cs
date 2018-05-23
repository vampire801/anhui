using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameLauncher : MonoBehaviour
    {        
        public GameObject sdkMgr;        
        public static bool isContainSdkMgr;  

        void Awake()
        { 
            SceneManager_anhui.Instance.LoadSceneAdditive(ESCENE.MAHJONG_LOBBY_MAIN_SCENE);
        }
       
    }

}
