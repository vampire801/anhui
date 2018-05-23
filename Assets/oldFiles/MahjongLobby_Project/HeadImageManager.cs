using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class HeadImageManager  {

    //public static Dictionary<string, byte[]> _peopleHead = new Dictionary<string, byte[]>();
    public static Dictionary<string, Texture2D > _peopleHead = new Dictionary<string, Texture2D>();
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tt"></param>
    public static void  SaveHeadImage(string url,Texture2D tt)
    {
        if (!_peopleHead.ContainsKey(url ))
        {
            //_peopleHead.Add(url, tt.EncodeToJPG());
            _peopleHead.Add(url, tt);
        }
    }
    /// <summary>
    /// 获取图片
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    //public Texture2D GetHeadImage(string url)
    //{

    //    Texture2D tt = new Texture2D(90, 90, TextureFormat.ARGB4444, false);
    //    Debug.LogError(_peopleHead.Count);
    //    if (_peopleHead.ContainsKey(url))
    //    {
    //        Debug.LogError("加载自己数组");
    //        tt.LoadImage(_peopleHead[url]);
    //    }
    //    else
    //    {
    //        tt = null;
    //    }
    //    return tt;
    //}
}
