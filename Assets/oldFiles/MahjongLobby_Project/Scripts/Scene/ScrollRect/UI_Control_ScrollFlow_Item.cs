using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class UI_Control_ScrollFlow_Item : MonoBehaviour
{
    private UI_Control_ScrollFlow parent;
    [HideInInspector]
    public RectTransform rect;
    public Text img;

    public float v = 0;
    private Vector3 p, s;
    /// <summary>
    /// 缩放值
    /// </summary>
    public float sv;
    // public float index = 0,index_value;
    private Color color;

    //[HideInInspector]
    public int iCountyId; //县城id
    //[HideInInspector]
    public int iCityId; //城市id

    public void Init_Num()
    {
        v = 0;
        p = Vector3.zero;
        s = Vector3.zero;
        sv = 0;
        color = new Color();
    }

    public void Init(UI_Control_ScrollFlow _parent)
    {
        rect = this.GetComponent<RectTransform>();
        img = this.GetComponent<Text>();
        parent = _parent;
        color = img.color;
    }

    public void Drag(float value)
    {    
        v += value;
        p = new Vector3(0, rect.transform.localPosition.y, 0);
        p.y = parent.GetPosition(v);
        rect.localPosition = new Vector3(0f, p.y, 0);

        color.a = parent.GetApa(v);
        img.color = color;
        sv = parent.GetScale(v);
        s.x = sv;
        s.y = sv;
        s.z = 1;
        rect.localScale = s;
    }

}
