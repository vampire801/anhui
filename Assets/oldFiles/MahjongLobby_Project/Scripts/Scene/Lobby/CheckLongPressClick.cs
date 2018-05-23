using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    //检测一个按钮是否长按，如果长按则在回调方法里，执行操作
    public class CheckLongPressClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        //长按间隔时间，自己设置
        public float interval = 24 * 60 * 60 * 60f;//0.2f;
                                                   //是否已经长按的标志位
        bool isAleadyLongPress;

        public string Content; //显示内容
        public string Content_Not;//不勾选的时候选的内容

        public GameObject Dis;//不可用
        public GameObject Nor;//正常

        [SerializeField]
        UnityEvent m_OnLongpress = new UnityEvent();


        [SerializeField]
        UnityEvent LevelClick = new UnityEvent();

        //是否鼠标按下的标志
        private bool isPointDown = false;

        //鼠标按下的时间
        private float lastInvokeTime;

        //规则id
        public int RuleId;

        GameObject longPress; //长按的预置体


        void Start()
        {
            longPress = transform.Find("Label/LongPress").gameObject;
            int count = Content.Length / 12 + 1;
            //根据字体多少改变框的大小
            longPress.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(380f, 35f * count + 85f);
            longPress.transform.localPosition += new Vector3(0, (count - 1) * 17.5f, 0);
            longPress.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = Content;
        }

        void Update()
        {
            if (isPointDown)
            {
                if (Time.time - lastInvokeTime > interval)
                {
                    //触发点击;
                    isAleadyLongPress = true;
                    m_OnLongpress.Invoke();

                    //如果不做特殊处理，就直接在这里显示提示
                    if (longPress != null)
                    {
                        longPress.SetActive(true);


                    }

                    lastInvokeTime = Time.time;
                }
            }

        }

        //按下鼠标
        public void OnPointerDown(PointerEventData eventData)
        {
            isPointDown = true;
            lastInvokeTime = Time.time;
        }


        //抬起鼠标
        public void OnPointerUp(PointerEventData eventData)
        {
            if (longPress != null)
            {
                longPress.SetActive(false);
            }

            if (isAleadyLongPress)
            {
                LevelClick.Invoke();
            }
            isPointDown = false;
        }

        //离开鼠标
        public void OnPointerExit(PointerEventData eventData)
        {
            isPointDown = false;
        }


    }
}