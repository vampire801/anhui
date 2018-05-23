using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using XLua;

[Hotfix]
[LuaCallCSharp]
//[csharp]view plain copy print?<span style = "font-size:14px;" >
public class AchieveManage : MonoBehaviour
{
    public ScrollRect scroRect;             //ScrollRect  
    public VerticalLayoutGroup group;       //VerticalLayoutGroup  
    public GameObject prefab_item;          //预设  
    public int maxCount = 200;              //生产 数量    

    List<GameObject> list;
    float scroRect_MinHight;                 //预设高度  
    float scroRect_MaxHight;                 //scrollrect 视图高度  

    void Start()
    {
        //1.计算出 scrollrect 视图范围  
        scroRect_MinHight = prefab_item.GetComponent<LayoutElement>().preferredHeight;  //获取预设高度  
        scroRect_MaxHight = scroRect.transform.position.y + scroRect_MinHight;          //获取scrollrect 视图高度    

        list = new List<GameObject>();
        CreateAchieve();                    //创建成就  
        StartCoroutine(IEShowInit());       //隐藏初始化  
    }

    //创建成就  
    void CreateAchieve()
    {
        for (int i = 0; i < maxCount; i++)
        {
            var go = Instantiate(prefab_item) as GameObject;
            go.SetActive(true);
            go.transform.position = Vector3.zero;
            go.transform.SetParent(group.transform);
            go.name = "item" + i;
            go.transform.Find("msg/txt1").GetComponent<Text>().text = go.name;
            list.Add(go);
        }


    }

    //2.拖动完毕委托事件   
    public void VCValueChanged()
    {
        list.ForEach((p) => { CheckPos(p.transform); });
    }
    //3.检测 在scrollrect 视图内  
    void CheckPos(Transform obj)
    {
        var pos = obj.position;
        var go = obj.gameObject;
        if (pos.y >= -scroRect_MinHight && pos.y <= scroRect_MaxHight)
            ShowItem(go);
        else
            HideItem(go);
    }
    //4.隐藏或显示  
    void ShowItem(GameObject go)
    {
        go.transform.GetComponent<Image>().enabled = true;
        go.transform.Find("sprite").gameObject.SetActive(true);
        go.transform.Find("msg").gameObject.SetActive(true);
        go.transform.Find("but").gameObject.SetActive(true);
    }
    void HideItem(GameObject go)
    {
        go.transform.GetComponent<Image>().enabled = false;
        go.transform.Find("sprite").gameObject.SetActive(false);
        go.transform.Find("msg").gameObject.SetActive(false);
        go.transform.Find("but").gameObject.SetActive(false);
    }

    IEnumerator IEShowInit()
    {
        yield return new WaitForSeconds(0.3f);
        VCValueChanged();
    }
}//</span>  