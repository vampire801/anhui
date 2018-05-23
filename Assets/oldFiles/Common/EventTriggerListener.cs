using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    internal  string str;
    internal GameObject obj;
    public delegate void VoidDelegate (GameObject  go);
    public VoidDelegate  onClick;
    public VoidDelegate  onClick_Sring;
    public VoidDelegate  onDown;
    public VoidDelegate  onEnter;
    public VoidDelegate  onExit;
    public VoidDelegate  onUp;
    public VoidDelegate  onSelect;
    public VoidDelegate  onUpdateSelect;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">按钮组件</param>
    /// <param name="str">string类型参数</param>
    /// <param name="obj">gameObject类型参数</param>
    /// <returns></returns>
    public static EventTriggerListener Get(GameObject go, string Str = "", GameObject Obj = null)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();

        if (listener == null)
        {
            listener = go.AddComponent<EventTriggerListener>();
            go.GetComponent<EventTriggerListener>().obj = Obj;
            go.GetComponent<EventTriggerListener>().str = Str;
        }
        else
        {
            if (!string.IsNullOrEmpty (Str))
            {
                go.GetComponent<EventTriggerListener>().str = Str;
            }
            if (Obj!=null )
            {
                go.GetComponent<EventTriggerListener>().obj = Obj;
            }
        }
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject  );
        if (onClick_Sring != null) onClick_Sring(gameObject );
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }
}