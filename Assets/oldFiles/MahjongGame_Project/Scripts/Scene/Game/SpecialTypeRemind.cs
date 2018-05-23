using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using MahjongGame_AH;
using MahjongGame_AH.Data;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class SpecialTypeRemind : MonoBehaviour
{
    //是否显示结算界面
    public bool isShowWinPanel;    
    public Sprite[] RemindImage;  //0表示底图，1表示抢，2表示荒庄

    /// <summary>
    /// 赋值
    /// </summary>
    public void GetValue(string text,bool status)
    {
        transform.GetComponent<Image>().SetNativeSize();
        isShowWinPanel = status;
        transform.Find("Text").GetComponent<Text>().text = text; ChangeTransformScale();
    }          

    //改变物体的大小
    void ChangeTransformScale()
    {
        Tweener tweener_0 = transform.DOScale(new Vector3(2f, 2f, 2f), 0.3f);
        tweener_0.OnComplete(Oncomplete_0);
    }

    void Oncomplete_0()
    {
        Tweener tweener_1 = transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
        tweener_1.SetDelay(0.2f);
        tweener_1.OnComplete(Oncomplete_1);
    }

    //0完成之后回调
    void Oncomplete_1()
    {
        Tweener tweener_2 = transform.DOScale(Vector3.one * 0.5f, 0.3f);
        tweener_2.SetDelay(1.5f);
        Tweener tweener_3 = transform.GetComponent<CanvasGroup>().DOFade(0f, 0.1f);
        tweener_3.SetDelay(1.7f);
        tweener_3.OnComplete(Oncomplete_3);
    }


    void Oncomplete_3()
    {
        if (isShowWinPanel)
        {
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            //更新面板的显示数据
            grpd.isPanelShow = true;
            grpd.isShowRoundGameResult = true;
            SystemMgr.Instance.GameResultSystem.UpdateShow();
            UIMainView.Instance.GameResultPanel.SpwanGameReult_Round();            
        }
        Destroy(gameObject);
    }
}
