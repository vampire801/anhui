using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using MahjongGame_AH.Data;
using MahjongGame_AH;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class SpecialTypeScoreNotice : MonoBehaviour
{
    public Font[] font;  //0表示金色字体，1表示银色字体
    Text text;

    void Awake()
    {
        text = transform.GetComponent<Text>();
    }

    /// <summary>
    /// 显示获取的分数
    /// </summary>
    /// <param name="score"></param>    
    public void GetValue(int score)
    {
        if(text==null)
        {
            text= transform.GetComponent<Text>();
        }

        //使用金色字体
        if(score>0)
        {

            text.font = font[0];
            text.text = "+" + score;
        }
        else
        {
            text.font = font[1];
            text.text = score.ToString();
        }

        MoveToUp();
    }


    //向上移动字体，同时渐隐
    void MoveToUp()
    {
        Tweener tweener_0 = transform.DOLocalMoveY(transform.localPosition.y + 55f, 0.6f);
        tweener_0.SetDelay(0.5f);
        Tweener tweener_1 = transform.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
        tweener_1.SetDelay(0.8f);
        tweener_1.OnComplete(Oncompete_0);
    }


    //移动完成之后，删除
    void Oncompete_0()
    {
        //为玩家添加分数
        if(SystemMgr.Instance)
        {
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
        }        
        Destroy(gameObject);
    }
    
}
