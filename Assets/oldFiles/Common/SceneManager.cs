using UnityEngine;
using System.Collections;


//场景枚举
public enum ESCENE
{
    //空场景
    NULL,
    EMPTY,
    //大厅场景
    MAHJONG_LOBBY_GENERAL,
    //大厅主场景
    MAHJONG_LOBBY_MAIN_SCENE,

    //游戏内部的一般场景
    MAHJONG_GAME_GENERAL,
    //游戏的主场景
    MAHJONG_GAME_MAIN_SCENE,

    //捕鱼
    FISH_GAME_SCENE,

    //空场景
    FISH_GAME_SKIP,

    //回放场景
    GradePlayBack
}

public class SceneManager_anhui
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public delegate void EventHandler(SceneManager_anhui sender);
    /// <summary>
    /// 离开场景事件
    /// </summary>
    public event EventHandler OnLeaveScene;
    /// <summary>
    /// 进入场景事件
    /// </summary>
    public event EventHandler OnEnterScene;

    float timer; //计时器


    #region  单例
    static SceneManager_anhui instance;
    public static SceneManager_anhui Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneManager_anhui();
                instance.Init();
            }
            return instance;
        }
    }
    #endregion 单例

    GameObject scenenManagerObject;
    SceneManagerHelper sceneManagerHelper;

    /// <summary>
    /// 初始化
    /// 创建对象，并附加脚本
    /// </summary>
    void Init()
    {
        GameObject go = GameObject.Find("__SceneManager");

        if (go == null)
        {
            scenenManagerObject = new GameObject("__SceneManager");
            GameObject.DontDestroyOnLoad(scenenManagerObject);
            sceneManagerHelper = (SceneManagerHelper)scenenManagerObject.AddComponent(typeof(SceneManagerHelper));
        }
        else
        {
            if (go.GetComponent<SceneManagerHelper>() != null)
            {
                GameObject.Destroy(go.GetComponent<SceneManagerHelper>());
            }
            else
            {
                go.AddComponent<SceneManagerHelper>();
            }
            sceneManagerHelper = go.GetComponent<SceneManagerHelper>();
        }

        sceneManagerHelper._SceneManager = this;
    }

    [SerializeField]
    ESCENE leavingScene = ESCENE.MAHJONG_LOBBY_GENERAL;
    /// <summary>
    /// 离开的场景
    /// </summary>
    public ESCENE LeavingScene
    {
        get
        {
            return leavingScene;
        }
        set
        {
            leavingScene = value;
        }
    }

    [SerializeField]
    ESCENE enteringScene = ESCENE.MAHJONG_LOBBY_GENERAL;
    /// <summary>
    /// 进入的场景
    /// </summary>
    public ESCENE EnteringScene
    {
        get
        {
            return enteringScene;
        }
        set
        {
            enteringScene = value;
        }
    }

    ESCENE currentScene = ESCENE.NULL;
    /// <summary>
    /// 当前的场景
    /// </summary>
    public ESCENE CurrentScene
    {
        get
        {
            return currentScene;
        }
        set
        {
            currentScene = value;
        }
    }


    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name='scene'>场景</param>
    public void LoadScene(ESCENE scene)
    {
        leavingScene = enteringScene;
        enteringScene = scene;

        sceneManagerHelper.leavingScene = leavingScene;
        sceneManagerHelper.enteringScene = enteringScene;
        Leave();
        sceneManagerHelper.LoadLevel(scene.ToString());
    }

    public void LoadSceneSingle(ESCENE scene)
    {
        leavingScene = enteringScene;
        enteringScene = scene;

        sceneManagerHelper.leavingScene = leavingScene;
        sceneManagerHelper.enteringScene = enteringScene;
        Leave();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void LoadSceneAdditive(ESCENE scene)
    {
        leavingScene = enteringScene;
        enteringScene = scene;

        sceneManagerHelper.leavingScene = leavingScene;
        sceneManagerHelper.enteringScene = enteringScene;
        Leave();

        sceneManagerHelper.LoadLevelAdditive(scene.ToString());
    }

    /// <summary>
    /// 当场景加载完毕时调用此函数
    /// </summary>
    public void LoadingFinished()
    {
        Resources.UnloadUnusedAssets();
        Enter();

    }

    /// <summary>
    /// 离开场景
    /// </summary>
    private void Leave()
    {
        CurrentScene = ESCENE.NULL;
        sceneManagerHelper.currentScene = CurrentScene;

        if (OnLeaveScene != null)
        {
            OnLeaveScene(this);
        }

        timer = Time.realtimeSinceStartup;

    }

    /// <summary>
    /// 进入场景
    /// </summary>
    private void Enter()
    {
        if (CurrentScene == EnteringScene)
        {
            return;
        }

        //if (PlayBack_1.PlayBackData.isComePlayBack)
        //{
        //    return;
        //}

        if (OnEnterScene != null)
        {
            OnEnterScene(this);
        }

        CurrentScene = EnteringScene;
        sceneManagerHelper.currentScene = CurrentScene;
        float time = Time.realtimeSinceStartup - timer;
        DEBUG.LogTracer("SceneManager 加载场景: " + CurrentScene.ToString() + " 花费了 [ " + time + " ] 秒");
    }


    #region 新的场景加载方法
    public AsyncOperation PreGamescene; //游戏的预加载场景


    /// <summary>
    /// 预加载游戏场景
    /// </summary>
    /// <param name="scene"></param>
    public void PreloadScene(ESCENE scene)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene.ToString()).name == null)
        {
            PreGamescene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.ToString(),
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
            PreGamescene.allowSceneActivation = false;
        }
    }


    /// <summary>
    /// 打开指定的场景
    /// </summary>
    public void OpenPointScene(ESCENE scene)
    {
        leavingScene = enteringScene;
        enteringScene = scene;

        sceneManagerHelper.leavingScene = leavingScene;
        sceneManagerHelper.enteringScene = enteringScene;
        Leave();


        if (MahjongGame_AH.GameData.Instance && PreGamescene != null)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_GAME_GENERAL.ToString()).name != null)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_GAME_GENERAL.ToString());
            }

            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_GAME_MAIN_SCENE.ToString()).name != null)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_GAME_MAIN_SCENE.ToString());
            }
        }


        if (MahjongLobby_AH.GameData.Instance && PreGamescene != null)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_LOBBY_GENERAL.ToString()).name != null)
            {
                AsyncOperation general = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_LOBBY_GENERAL.ToString());
            }

            //卸载其他场景            
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_LOBBY_MAIN_SCENE.ToString()).name != null)
            {
                AsyncOperation main = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(ESCENE.MAHJONG_LOBBY_MAIN_SCENE.ToString());
            }
        }

        EnterScene(scene);
    }




    public void EnterScene(ESCENE scene)
    {
        if (PreGamescene != null)
        {
            PreGamescene.allowSceneActivation = true;
        }
        else
        {
            LoadSceneSingle(scene);
        }

        if (CurrentScene == EnteringScene)
            return;
        if (OnEnterScene != null)
        {
            OnEnterScene(this);
        }
        CurrentScene = EnteringScene;
        sceneManagerHelper.currentScene = CurrentScene;
    }
    #endregion
}





