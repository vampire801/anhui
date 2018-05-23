using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class animationPlay : MonoBehaviour
{
    // Use this for initialization
    /// <summary>
    /// 播放道具动画
    /// </summary>
    /// <param name="num">1-玫瑰 2-拍手 3-现金 4-鸡蛋</param>
    public void PlayAnim(int num)
    {
        // Debug.LogError("---------------"+gameObject.name);
        //gameObject.GetComponent<Animator>().Play("flower", 128);
       // Debug.LogError("over");
        gameObject.GetComponent<Animator>().SetInteger("PropType", num);
        if (num==2)
        {
            StartCoroutine(DelayDistroy());
        }
    }
    IEnumerator DelayDistroy()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetInteger("PropType", 0);
        onDisable();
    }
    public void onDisable()
    {
        Destroy(gameObject);
    }
    public void PlayMove()
    {
        //GameObject go = Instantiate(Resources.Load<GameObject>("Game/moveProp")) as GameObject;
        //go.GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        //go.transform.SetParent(_playingChangeHead[fromIndex].parent);
        //go.transform.localPosition = _playingChangeHead[fromIndex].localPosition;
        //Tweener dt = go.GetComponent<DOTweenAnimation>().tween as Tweener;

        //dt.ChangeEndValue(_playingChangeHead[toIndex].localPosition);
        //dt.Restart();
        //Debug.LogWarning(_playingChangeHead[toIndex].localPosition);
        //dt.OnComplete(() =>
        //{

        //    Destroy(go);
        //});
    }
}
