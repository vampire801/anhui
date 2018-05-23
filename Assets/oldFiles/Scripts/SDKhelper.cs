using UnityEngine;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class SDKhelper : MonoBehaviour {

    public GameObject sdkMgr;
    public static bool isContainSdkMgr;

    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject   );
        if (!isContainSdkMgr)
        {
            //加载sdkmgr
            GameObject go = Instantiate(sdkMgr);
            go.name = "SDKMgr";
            go.transform.localPosition = Vector3.zero;
            isContainSdkMgr = true;
        }
    }
    /// <summary>
    /// 邀请回调
    /// </summary>
    /// <param name="str"></param>
    //public void OnGetParamCB(string str)
    //{
    //    Debug.LogError("+++++++++++++++++++++++fristCallBack+++++++++++++++");
    //    SDKManager.Instance.OnGetParamCB(str);
    //    // SendAuthReqTyp2();
    //}

}
