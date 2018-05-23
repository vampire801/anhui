using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

[Hotfix]
[LuaCallCSharp]
//处理加载场景的进度条
public class SceneLoding : MonoBehaviour
{

    #region 单例

    static SceneLoding instance;
    public static SceneLoding Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        //获取相机
        transform.GetComponent<Canvas>().worldCamera = Camera.main;
    }
    #endregion


    public const string Content = "正在加载游戏资源，请耐心等待";

    public Text LoadingContent; //加载显示内容
    public Text tipsContent;  //小贴士内容
    public Slider LoadingSlider;  //加载的进度条       
    public Camera cameras;

    float timer = 2f; //切换下一条tips的时间间隔

    float sliderValue;  //加载条的值

    #region 加载tips的内容
    //获取tips文件的路径
    string path = "";


    //tips文档的任意一条内容
    string tips = "庄家：庄家胡则连庄、否则轮庄";




    /// <summary>
    /// 获取读取的tips的内容
    /// </summary>
    /// <returns></returns>
    public string GetTips()
    {
        StartCoroutine(ReadTips(path));
        return tips;
    }

    /// <summary>
    /// 读取文件夹中的tips文件
    /// </summary>
    /// <param name="path_">tips的存放路径，手机和电脑路径不一样，注意</param>
    /// <returns></returns>
    public IEnumerator ReadTips(string path_)
    {
        if (path.Contains("://"))
        {
            WWW www = new WWW(path);

            while (!www.isDone)
            {
                yield return www;
            }

            if (www.error != null)
            {
                Debug.LogError("读取tips文件失败:" + www.error);
            }

            //获取tips中的所有内容
            string[] tips_ = www.text.Split('\n');

            //随机取其中的任意一条
            tips = tips_[Random.Range(0, tips_.Length - 1)];
        }
        else
        {
            string[] tips_ = (System.IO.File.ReadAllText(path)).Split('\n');
            tips = tips_[Random.Range(0, tips_.Length - 1)];
        }
    }
    #endregion

    #region 更新加载界面
    float initTimer = 0;

    //初始显示一条tips
    public void Init(int status)
    {
        transform.GetComponent<Canvas>().worldCamera = cameras;
#if UNITY_EDITOR
        path = "file:///" + Application.dataPath + "/StreamingAssets/tips.txt";
#elif UNITY_IPHONE || UNITY_ANDROID
        path = Application.streamingAssetsPath + "/tips.txt";
#endif
        sliderValue = 0;
        initTimer = 0;
        transform.Find("LodingBg").gameObject.SetActive(true);
        sliderValue = 0f;
        tipsContent.text = GetTips();
        LoadingSlider.value = 0f;
        LoadingContent.text = Content + (sliderValue * 100f).ToString("0") + "%";
        //StartCoroutine(LoadingScene(status));
    }

    void Update()
    {
        initTimer += Time.deltaTime;
        if (initTimer > timer)
        {
            tipsContent.text = GetTips();
            initTimer = 0f;
        }
    }

    /// <summary>
    /// 加载场景更新进度条的值
    /// </summary>
    /// <param name="status">1表示从大厅进入游戏，2表示从游戏进入大厅</param>
    /// <returns></returns>
    IEnumerator LoadingScene(int status)
    {
        while (sliderValue < 1)
        {
            if (sliderValue < 0.7f)
            {
                sliderValue += Random.Range(0.02f, 0.05f);
            }
            else
            {
                if (status == 1)
                {
                    if (anhui.MahjongCommonMethod.Instance.isSitSuccess)
                    {
                        anhui.MahjongCommonMethod.Instance.isSitSuccess = false;
                        sliderValue += Random.Range(0.03f, 0.06f);
                    }
                    else
                    {
                        sliderValue += Random.Range(0.01f, 0.03f);
                    }
                }
                else
                {
                    if (anhui.MahjongCommonMethod.isAuthenSuccessInLobby)
                    {
                        anhui.MahjongCommonMethod.isAuthenSuccessInLobby = false;
                        sliderValue += Random.Range(0.03f, 0.06f);
                    }
                    else
                    {
                        sliderValue += Random.Range(0.01f, 0.03f);
                    }
                }

            }
            SettingSliderValue(sliderValue);
            yield return new WaitForEndOfFrame();
        }

        if (sliderValue >= 1)
        {
            transform.GetComponent<SceneLoding>().enabled = false;
            transform.Find("LodingBg").gameObject.SetActive(false);

        }
    }


    //设置加载进度条的值
    public void SettingSliderValue(float value)
    {
        if (value <= 0)
        {
            value = 0;
        }
        if (value >= 1)
        {
            value = 1;

            LoadingSlider.value = value;
            LoadingContent.text = Content + (value * 100f).ToString("0") + "%";
            transform.GetComponent<SceneLoding>().enabled = false;
            transform.Find("LodingBg").gameObject.SetActive(false);
        }

        LoadingSlider.value = value;
        LoadingContent.text = Content + (value * 100f).ToString("0") + "%";
    }

    /// <summary>
    /// 删除自己
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    #endregion

}
