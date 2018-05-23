using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameLauncher : MonoBehaviour
    {


        /// <summary>
        /// 要加载的资源
        /// </summary>
        //  public InstantiateResource[] Resources;

        //   int loadIndex; //资源加载的索引
        //    bool debugEnable = false; //是否记录调试信息

        void Awake()
        {
            Application.targetFrameRate = 30;
            if (!PlayBack_1.PlayBackData.isComePlayBack)
            {
                SceneManager_anhui.Instance.LoadSceneAdditive(ESCENE.MAHJONG_GAME_MAIN_SCENE);
            }
        }

        /// <summary>
        /// 当应用程序退出
        /// </summary>
        void OnApplicationQuit()
        {
            // DEBUG.Final();
        }

    }

}
