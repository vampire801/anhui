using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using MahjongLobby_AH;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class DropItem : MonoBehaviour
{
    public Image ItemImage;  //道具的图片
    public Text ItemNum;  //道具的数量
    public Sprite[] Sprites;  //道具要使用的图片
    public GameObject Item;  //道具的父物体

    public Vector3 posTarget;  //道具的目标位置

    public Sprite GetSprite(string name)
    {
        Sprite sprite = null;
        if(name.Length<1)
        {
            return null; 
        }
        for(int i=0;i<Sprites.Length;i++)
        {
            if(string.Compare(name,Sprites[i].name)==0)
            {
                sprite = Sprites[i];
            }
        }
        return sprite;
    }

    /// <summary>
    /// 为获取的面板赋值
    /// </summary>
    /// <param name="image">道具的图片</param>
    /// <param name="content">道具的数量</param>
    /// <param name="pos">道具的目标位置</param>
    public void GetItemMessage(string name,string content,Vector3 pos,Vector3 initPos)
    {
        posTarget = pos;
        //如果俩者都不为空，则继续执行
        if(name == null&&content==null)
        {
            Debug.LogError("为玩家获取的道具复制出现错误");
            return;
        }
        ItemImage.sprite = GetSprite(name);
        ItemNum.text = "X"+content;
        ChangeScale();
    }

    /// <summary>
    /// 沿曲线移动玩家
    /// </summary>
    /// <param name="targetPos"></param>
    void Change(Vector3 targetPos)
    {
        Vector3[] point = new Vector3[3];
        point[0] = new Vector3(targetPos.x + (targetPos.x - transform.localPosition.x) / 3f,transform.localPosition.y+(targetPos.y- transform.localPosition.y)/3f, 0);
        point[1] = new Vector3(point[0].x + 2 * (targetPos.x - point[0].x) / 3f,point[0].y+ (targetPos.y - point[0].y) * 0.6f, 0);
        point[2] = targetPos;
        Tweener tweener = transform.DOScale(1f, 0.8f);
        Tween path = transform.DOPath(point, 4f, PathType.Linear);
        path.SetEase(Ease.Linear); 
        path.OnComplete(ChangeScale);
    }


    //改变道具的大小
    void ChangeScale()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        Tweener tweener = Item.transform.DOScale(1.5f, 0.3f);
        tweener.SetDelay(0.3f);
        tweener.OnComplete(Oncomplete_0);
    }

    void Oncomplete_0()
    {
        Tweener tweener = Item.transform.DOScale(0.8f, 0.3f);        
        tweener.OnComplete(Oncomplete_1);
    }

    void Oncomplete_1()
    {
        Tweener tweener = Item.transform.DOScale(1f, 0.3f);
        tweener.OnComplete(Move);
    }

    void Move()
    {
        //道具开始移动缩放播放动画       
        Tweener tween_0 = Item.transform.DOLocalMove(posTarget, 1f);                        
        Tweener tween_1 = Item.transform.DOScale(Vector3.one * 0.2f, 0.5f);
        tween_1.SetDelay(0.5f);
        tween_0.OnComplete(OnComplete);
    }

    void OnComplete()
    {
        //UIMainView.Instance.LobbyPanel.NewPlayerBag.SetActive(false);
        //Destroy(gameObject);        
    }

}
