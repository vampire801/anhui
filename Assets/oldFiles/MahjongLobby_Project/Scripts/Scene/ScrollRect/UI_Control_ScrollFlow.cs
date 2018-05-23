using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MahjongLobby_AH;
using MahjongLobby_AH.Data;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class UI_Control_ScrollFlow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public delegate void CallBack();
    public delegate void CallBack<T>(T t);
    public delegate void CallBack<T, V>(T t, V v);
    public delegate void CallBack<T, V, U>(T t, V v, U u);
    public RectTransform Rect;
    public List<UI_Control_ScrollFlow_Item> Items;
    public int status;  //1表示城市 2表示县城   
    public GameObject CountyMessage;// 县城的预置体
    /// <summary>
    /// 宽度
    /// </summary>
    public float Width = 720;
    /// <summary>
    /// 大小
    /// </summary>
    public float MaxScale = 1;
    /// <summary>
    /// StartValue开始坐标值，AddValue间隔坐标值，小于vmian 达到最左，大于vmax达到最右
    /// </summary>
    public float StartValue = 0.1f, AddValue = 0.2f, VMin = 0.1f, VMax = 0.9f;
    /// <summary>
    /// 坐标曲线
    /// </summary>
    public AnimationCurve PositionCurve;
    /// <summary>
    /// 大小曲线
    /// </summary>
    public AnimationCurve ScaleCurve;
    /// <summary>
    /// 透明曲线
    /// </summary>
    public AnimationCurve ApaCurve;
    /// <summary>
    /// 计算值
    /// </summary>
    private Vector2 start_point, add_vect;
    /// <summary>
    /// 动画状态
    /// </summary>
    public bool _anim = false;
    /// <summary>
    /// 动画速度
    /// </summary>
    public float _anim_speed = 1f;

    private float v = 0;
    private List<UI_Control_ScrollFlow_Item> GotoFirstItems = new List<UI_Control_ScrollFlow_Item>(), GotoLaserItems = new List<UI_Control_ScrollFlow_Item>();
    public event CallBack<UI_Control_ScrollFlow_Item> MoveEnd;


    public void Init()
    {
        VMax = 0.9f;
        start_point = Vector2.zero;
        add_vect = Vector2.zero;
        _anim = false;
        v = 0;
        GotoFirstItems = new List<UI_Control_ScrollFlow_Item>();
        GotoLaserItems = new List<UI_Control_ScrollFlow_Item>();
    }

    public void Refresh(int status_ = 0)
    {
        ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
        int count = 0;
        if (status == 1)
        {
            Debug.LogError("城市信息:" + pspd.listCityMessage.Count);
            //在这里需要对list按需求排序
            for (int i = 0; i < pspd.listCityMessage.Count; i++)
            {
                SelectAreaPanelData.CityMessage temp = new SelectAreaPanelData.CityMessage();
                if (Convert.ToInt32(pspd.listCityMessage[i].cityId) == pspd.iCityId[1])
                {
                    temp = pspd.listCityMessage[i];
                    pspd.listCityMessage.RemoveAt(i);
                    if (pspd.listCityMessage.Count > 0)
                    {
                        pspd.listCityMessage.Insert(pspd.listCityMessage.Count - 1, temp);
                    }
                    else
                    {
                        pspd.listCityMessage.Add(temp);
                    }

                    break;
                }
            }

            count = pspd.listCityMessage.Count;
        }
        else if (status == 2)
        {
            if (pspd.dicCountyMessage.ContainsKey(pspd.iCityId[1]))
            {
                //在这里需要对list按需求排序
                for (int i = 0; i < pspd.dicCountyMessage[pspd.iCityId[1]].Count; i++)
                {
                    SelectAreaPanelData.CountyMessage temp = new SelectAreaPanelData.CountyMessage();
                    if (Convert.ToInt32(pspd.dicCountyMessage[pspd.iCityId[1]][i].countyId) == pspd.iCityId[1])
                    {
                        temp = pspd.dicCountyMessage[pspd.iCityId[1]][i];
                        pspd.dicCountyMessage[pspd.iCityId[1]].RemoveAt(i);
                        if (pspd.dicCountyMessage.Count > 0)
                        {
                            pspd.dicCountyMessage[pspd.iCityId[1]].Insert(pspd.dicCountyMessage.Count - 1, temp);
                        }
                        else
                        {
                            pspd.dicCountyMessage[pspd.iCityId[1]].Add(temp);
                        }

                        break;
                    }
                }
                count = pspd.dicCountyMessage[pspd.iCityId[1]].Count;
            }
        }

        //处理两个子物体
        if (count == 2)
        {
            StartValue = 0.3f;
            VMin = 0.3f;
            VMax = 0.7f;
        }
        else if (count == 1)
        {
            StartValue = 0.5f;
            pspd.temp_cc[0] = Convert.ToInt32(pspd.listCityMessage[0].cityId);
        }


        if (status_ == 0)
        {
            SpwanAllGameobject(count, status);
        }

        //处理一个子物体
        if (count == 1)
        {
            //关闭滑动功能
            transform.GetChild(0).GetComponent<Text>().raycastTarget = false;
        }


        if (Rect.childCount < 5)
        {
            //VMax = StartValue + 4 * AddValue;
        }
        else
        {
            VMax = StartValue + (Rect.childCount - 1) * AddValue;
        }
        if (MoveEnd != null)
        {
            MoveEnd(Current);
        }
    }


    /// <summary>
    /// 产生对应的物体
    /// </summary>
    /// <param name="count">产生数量</param>
    /// <param name="status">1表示城市 2表示县城</param>
    void SpwanAllGameobject(int count, int status)
    {
        if (count <= 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (status == 1)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/City"));
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.SetParent(Rect.transform);
                UI_Control_ScrollFlow_Item item = go.GetComponent<UI_Control_ScrollFlow_Item>();
                if (item != null)
                {
                    MahjongLobby_AH.Data.ParlorShowPanelData pspd = MahjongLobby_AH.GameData.Instance.ParlorShowPanelData;
                    MahjongLobby_AH.Data.SelectAreaPanelData sapd = MahjongLobby_AH.GameData.Instance.SelectAreaPanelData;
                    //更新该预置体的界面信息
                    item.GetComponent<Text>().text = pspd.listCityMessage[i].cityName;
                    item.iCityId = Convert.ToInt32(pspd.listCityMessage[i].cityId);
                    Items.Add(item);
                    item.Init(this);
                    item.Drag(StartValue + i * AddValue);
                    if (item.v - 0.5 < 0.05f)
                    {
                        Current = Items[i];
                    }
                }
            }
            else
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/County"));
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.SetParent(Rect.transform);
                UI_Control_ScrollFlow_Item item = go.GetComponent<UI_Control_ScrollFlow_Item>();
                if (item != null)
                {
                    MahjongLobby_AH.Data.SelectAreaPanelData sapd = MahjongLobby_AH.GameData.Instance.SelectAreaPanelData;
                    MahjongLobby_AH.Data.ParlorShowPanelData pspd = MahjongLobby_AH.GameData.Instance.ParlorShowPanelData;
                    //更新该预置体的界面信息
                    item.GetComponent<Text>().text = pspd.dicCountyMessage[sapd.iCityId][i].countyName;
                    item.iCountyId = Convert.ToInt32(pspd.dicCountyMessage[sapd.iCityId][i].countyId);
                    Items.Add(item);
                    item.Init(this);
                    item.Drag(StartValue + i * AddValue);
                    if (item.v - 0.5 < 0.05f)
                    {
                        Current = Items[i];
                    }
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_point = eventData.position;
        add_vect = Vector3.zero;
        _anim = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        add_vect = eventData.position - start_point;
        v = eventData.delta.y * 1.00f / Width;
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Drag(v);
        }
        Check(v);
    }


    public void Check(float _v)
    {
        if (_v < 0)
        {//向左运动
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].v < (VMin - AddValue / 2))
                {
                    GotoLaserItems.Add(Items[i]);
                }
            }
            if (GotoLaserItems.Count > 0)
            {
                for (int i = 0; i < GotoLaserItems.Count; i++)
                {
                    GotoLaserItems[i].v = Items[Items.Count - 1].v + AddValue;
                    Items.Remove(GotoLaserItems[i]);
                    Items.Add(GotoLaserItems[i]);
                }
                GotoLaserItems.Clear();
            }
        }
        else if (_v > 0)
        {//向右运动，需要把右边的放到前面来

            for (int i = Items.Count - 1; i > 0; i--)
            {
                if (Items[i].v >= VMax)
                {
                    GotoFirstItems.Add(Items[i]);
                }
            }
            if (GotoFirstItems.Count > 0)
            {
                for (int i = 0; i < GotoFirstItems.Count; i++)
                {
                    GotoFirstItems[i].v = Items[0].v - AddValue;
                    Items.Remove(GotoFirstItems[i]);
                    Items.Insert(0, GotoFirstItems[i]);
                }
                GotoFirstItems.Clear();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float k = 0, v1;
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].v >= VMin)
            {
                v1 = (Items[i].v - VMin) % AddValue;
                //Debug.Log(v1 + "--" + NextAddValue);
                if (add_vect.y >= 0)
                {
                    k = AddValue - v1;
                }
                else
                {
                    k = v1 * -1;
                }
                break;
            }
        }
        add_vect = Vector3.zero;
        AnimToEnd(k);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick:" + eventData.pointerPressRaycast.gameObject);
        if (add_vect.sqrMagnitude <= 1)
        {
            Debug.Log("============OnPointerClickOK============");
            UI_Control_ScrollFlow_Item script = eventData.pointerPressRaycast.gameObject.GetComponent<UI_Control_ScrollFlow_Item>();
            if (script != null)
            {
                float k = script.v;
                k = 0.5f - k;
                AnimToEnd(k);
            }

        }
    }


    public float GetApa(float v)
    {
        return ApaCurve.Evaluate(v);
    }
    public float GetPosition(float v)
    {
        return PositionCurve.Evaluate(v) * Width;
    }
    public float GetScale(float v)
    {
        return ScaleCurve.Evaluate(v) * MaxScale;
    }


    private List<UI_Control_ScrollFlow_Item> SortValues = new List<UI_Control_ScrollFlow_Item>();
    private int index = 0;
    public void LateUpdate()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].v >= 0.1f && Items[i].v <= 0.9f)
            {
                index = 0;
                for (int j = 0; j < SortValues.Count; j++)
                {
                    if (Items[i].sv >= SortValues[j].sv)
                    {
                        index = j + 1;
                    }
                }

                SortValues.Insert(index, Items[i]);
            }
        }

        for (int k = 0; k < SortValues.Count; k++)
        {
            SortValues[k].rect.SetSiblingIndex(k);
        }
        SortValues.Clear();
    }

    public void ToLaster(UI_Control_ScrollFlow_Item item)
    {
        item.v = Items[Items.Count - 1].v + AddValue;
        Items.Remove(item);
        Items.Add(item);
    }


    private float AddV = 0, Vk = 0, CurrentV = 0, Vtotal = 0, VT = 0;
    private float _v1 = 0, _v2 = 0;

    private float start_time = 0, running_time = 0;

    public UI_Control_ScrollFlow_Item Current;



    public void AnimToEnd(float k)
    {
        AddV = k;
        if (AddV > 0) { Vk = 1; }
        else if (AddV < 0) { Vk = -1; }
        else
        {
            return;
        }
        Vtotal = 0;
        _anim = true;
        StartCoroutine(GetPointObject());
    }

    /// <summary>
    /// 获取指定的更新物体
    /// </summary>
    IEnumerator GetPointObject()
    {
        yield return new WaitForSeconds(0.2f);
        ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
        SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;

        if (status == 1)
        {
            int CityCount = transform.GetComponentsInChildren<UI_Control_ScrollFlow_Item>().Length;
            UI_Control_ScrollFlow_Item[] cityItem = new UI_Control_ScrollFlow_Item[CityCount];
            cityItem = transform.GetComponentsInChildren<UI_Control_ScrollFlow_Item>();
            for (int i = 0; i < CityCount; i++)
            {
                float y = cityItem[i].sv;
                if (0.9 < y && y < 1)
                {
                    pspd.temp_cc[0] = cityItem[i].iCityId;
                    pspd.iCityId[pspd.iStatus_AreaSeeting - 1] = cityItem[i].iCityId;

                    Debug.LogError("城市id:" + cityItem[i].iCityId);
                    break;
                }
            }

            //更新玩家对应的县的玩家
            UI_Control_ScrollFlow_Item[] temp = CountyMessage.transform.GetComponentsInChildren<UI_Control_ScrollFlow_Item>();
            int count = 0;
            if (pspd.dicCountyMessage.ContainsKey(sapd.iCityId))
            {
                count = pspd.dicCountyMessage[sapd.iCityId].Count;
            }
            CountyMessage.GetComponent<UI_Control_ScrollFlow>().Items.Clear();
            if (temp.Length > count)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i < count)
                    {
                        //更新该预置体的界面信息
                        temp[i].GetComponent<Text>().text = pspd.dicCountyMessage[sapd.iCityId][i].countyName;
                        temp[i].iCountyId = Convert.ToInt32(pspd.dicCountyMessage[sapd.iCityId][i].countyId);
                        CountyMessage.GetComponent<UI_Control_ScrollFlow>().Items.Add(temp[i]);
                        temp[i].Init_Num();
                        temp[i].Init(this);
                        temp[i].Drag(StartValue + i * AddValue);
                        if (temp[i].v - 0.5 < 0.05f)
                        {
                            CountyMessage.GetComponent<UI_Control_ScrollFlow>().Current = temp[i];
                        }
                    }
                    else
                    {
                        if (temp[i] != null)
                        {
                            Destroy(temp[i].gameObject);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    UI_Control_ScrollFlow_Item item = null;
                    if (i < temp.Length)
                    {
                        item = temp[i].GetComponent<UI_Control_ScrollFlow_Item>();
                        item.Init_Num();
                    }
                    else
                    {
                        GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/County"));
                        go.transform.localScale = Vector3.one;
                        go.transform.localEulerAngles = Vector3.zero;
                        go.transform.SetParent(CountyMessage.GetComponent<UI_Control_ScrollFlow>().Rect.transform);
                        item = go.GetComponent<UI_Control_ScrollFlow_Item>();
                    }

                    //更新该预置体的界面信息
                    item.GetComponent<Text>().text = pspd.dicCountyMessage[sapd.iCityId][i].countyName;
                    item.iCountyId = Convert.ToInt32(pspd.dicCountyMessage[sapd.iCityId][i].countyId);
                    CountyMessage.GetComponent<UI_Control_ScrollFlow>().Items.Add(item);
                    item.Init(this);
                    item.Drag(StartValue + i * AddValue);
                    if (item.v - 0.5 < 0.05f)
                    {
                        CountyMessage.GetComponent<UI_Control_ScrollFlow>().Current = item;
                    }
                }
            }
            CountyMessage.GetComponent<UI_Control_ScrollFlow>().Init();
            CountyMessage.GetComponent<UI_Control_ScrollFlow>().Refresh(1);
        }

        if (status == 2)
        {
            int CountyCount = transform.GetComponentsInChildren<UI_Control_ScrollFlow_Item>().Length;
            UI_Control_ScrollFlow_Item[] countyItem = transform.GetComponentsInChildren<UI_Control_ScrollFlow_Item>();
            for (int i = 0; i < CountyCount; i++)
            {
                float y = countyItem[i].sv;
                if (0.95f < y && y < 1.05f)
                {
                    pspd.temp_cc[1] = countyItem[i].iCountyId;
                    Debug.LogError("县城id:" + countyItem[i].iCountyId + ",名字:" + countyItem[i].GetComponent<Text>().text + ",位置:" + countyItem[i].transform.localPosition.y);
                    break;
                }
            }
        }
    }


    void Update()
    {
        if (_anim)
        {
            CurrentV = Time.deltaTime * _anim_speed * Vk;
            VT = Vtotal + CurrentV;
            if (Vk > 0 && VT >= AddV) { _anim = false; CurrentV = AddV - Vtotal; }
            if (Vk < 0 && VT <= AddV) { _anim = false; CurrentV = AddV - Vtotal; }
            //==============
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Drag(CurrentV);
                if (Items[i].v - 0.5 < 0.05f)
                {
                    Current = Items[i];
                }
            }
            Check(CurrentV);
            Vtotal = VT;


            if (!_anim)
            {
                if (MoveEnd != null) { MoveEnd(Current); }
            }
        }
    }

}
