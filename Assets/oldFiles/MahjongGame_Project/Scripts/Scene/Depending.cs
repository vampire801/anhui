using UnityEngine;
using System.Collections;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class Depending : MonoBehaviour
{
	void Start ()
    {
        //清空gc垃圾
        System.GC.Collect();
        //处理返回大厅
        SceneManager_anhui.Instance.LoadScene(ESCENE.MAHJONG_LOBBY_GENERAL);
    }
	

}
