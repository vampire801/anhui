using UnityEngine;
using System.Collections;


public class SceneManagerHelper : MonoBehaviour
{
    #region 实例

    static SceneManagerHelper instance;
    public static SceneManagerHelper GetInstance()
    {
        return instance;
    }
    #endregion 实例


    void Awake()
    {
        instance = this;
    }

    public ESCENE enteringScene = ESCENE.MAHJONG_GAME_GENERAL;
    public ESCENE leavingScene = ESCENE.MAHJONG_GAME_GENERAL;
    public ESCENE currentScene = ESCENE.NULL;
    /// <summary>
    /// 场景管理器
    /// </summary>
    public SceneManager_anhui _SceneManager;


    /// <summary>
    /// 加载关卡
    /// </summary>
    /// <param name='name'>关卡名称</param>
    public void LoadLevel(string name)
    {
        StartCoroutine(Loading(name));
    }



    public void LoadLevelAdditive(string name)
    {
        StartCoroutine(LoadingAdditive(name));
    }


    /// <summary>
    /// 加载
    /// </summary>
    /// <param name='name'>关卡名称</param>
    IEnumerator Loading(string name)
    {
        yield return 1;        
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, UnityEngine.SceneManagement.LoadSceneMode.Single); ;

        while (true)
        {
            if (load.isDone)
            {
                break;
            }
            yield return 1;
        }

        _SceneManager.LoadingFinished();
    }


    float Progress = 0;  //进度条

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name">场景名字</param>
    /// <returns></returns>
    IEnumerator LoadingAdditive(string name)
    {
        yield return 1;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        while (true)
        {
            if (load.isDone)
            {
                break;
            }
            yield return 1;
        }
        _SceneManager.LoadingFinished();
    }
}

