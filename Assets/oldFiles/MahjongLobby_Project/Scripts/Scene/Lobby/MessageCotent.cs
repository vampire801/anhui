using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class MessageCotent : MonoBehaviour
{
    public GameObject content;
    public Text text;
    public ScrollRect rect;

    void Start()
    {
        AdjustLength();
    }

    void AdjustLength()
    {
        float length = text.preferredHeight;        
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length);
        text.transform.localPosition = new Vector3(text.transform.localPosition.x,-length / 2f, 0);
        Debug.LogError("length:" + length);
        if(length>450f)
        {
            rect.vertical = true;            
        }
        else
        {
            rect.vertical = false;
        }
        content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length+20f);
    }

    /// <summary>
    /// 删除自己
    /// </summary>
    public void DesSelf()
    {
        Destroy(gameObject);
    }

}
