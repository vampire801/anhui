using UnityEngine;
using System.Collections;
using DG.Tweening;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class MahjongRemind : MonoBehaviour {

    public int Type; //种类
    
    void Start()
    {       
        Move();
    }




    void Move()
    {
        Tweener tweenner = null;
        if (Type == 1)
        {
            tweenner = transform.DOLocalMoveY(transform.localPosition.y + 40f, 0.3f);
        }
        else
        {
            tweenner = transform.DOLocalMoveX(transform.localPosition.x + 25f, 0.3f);
        }

        tweenner.SetLoops(1);
        tweenner.OnComplete(onComplete);
    }

    void onComplete()
    {
        Tweener tweenner = null;
        if (Type == 1)
        {
            tweenner = transform.DOLocalMoveY(transform.localPosition.y - 40f, 0.3f);
        }
        else
        {
            tweenner = transform.DOLocalMoveX(transform.localPosition.x - 25f, 0.3f);
        }

        tweenner.SetDelay(0.1f);
        tweenner.SetLoops(1);
        tweenner.OnComplete(Move);
    }

}
