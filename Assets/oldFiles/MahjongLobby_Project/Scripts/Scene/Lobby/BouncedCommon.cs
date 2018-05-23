using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class BouncedCommon : MonoBehaviour
    {
        public Image Bg;  //通用提示框的背景图片
        public Text Content;  //通用提示框的显示内容               

        void Start()
        {
            float height = (Screen.height / 2f) * 0.75f;
            //通过字体内容的长度，对图片大小进行缩放
            Bg.rectTransform.sizeDelta = new Vector2(Content.preferredWidth + 100f, 128f);

            //播放弹框动画
            Tweener tweener_0 = Content.transform.parent.DOLocalMoveY(height, 0.3f);
            tweener_0.SetDelay(2f);
            tweener_0.SetEase(Ease.Linear);

            //播放渐隐
            Tweener tweener_1 = transform.transform.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
            tweener_1.SetDelay(2.15f);
            tweener_1.SetEase(Ease.Linear);
            tweener_1.OnComplete(CallBack_2);
        }

        /// <summary>
        /// 第二个动画回调方法
        /// </summary>
        void CallBack_2()
        {
            anhui.MahjongCommonMethod.Instance.isSpwanCommonBounce = false;
            Destroy(gameObject);
        }


    }
}