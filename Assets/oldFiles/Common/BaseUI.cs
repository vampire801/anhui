using UnityEngine;
using XLua;

namespace Common
{
    [Hotfix]
    [LuaCallCSharp]
    /// <summary>
    /// UI基础类
    /// </summary>
    public class BaseUI : MonoBehaviour
    {
        public delegate void UIEventHandler(BaseUI sender);
        public event UIEventHandler OnActive;
        public event UIEventHandler OnDeactive;

        /// <summary>
        /// 获取UI面板的ID
        /// </summary>
        public virtual ushort GetID()
        {
            DEBUG.Gui("Get ID: UIConstant.UIID_WRONG_ID. Overwrite the GetID() function in subclass!");

            return 0x0000;
            //return UIConstant.UIID_WRONG_ID;
        }

        /// <summary>
        /// 视图已经显示，在这可以初始化一些东西
        /// </summary>
        protected virtual void ViewDidAppear()
        {
            //DEBUG.Gui("View did appear: " + UIConstant.UIIdToString(GetID()));
        }

        /// <summary>
        /// 视图即将消失，在这可以保存一些东西
        /// </summary>
        protected virtual void ViewWillDisappear()
        {
            //DEBUG.Gui("View will disappear: " + UIConstant.UIIdToString(GetID()));
        }

        /// <summary>
        /// 关闭自己
        /// </summary>
        protected virtual void CloseSelf()
        {
            //UIMgr.CloseUIPanel(GetID());
        }


        #region 父类的方法

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            //DEBUG.Gui("Base UI Awake: " + UIConstant.UIIdToString(GetID()));
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {            
            //DEBUG.Gui("Base UI Start: " + UIConstant.UIIdToString(GetID()));           
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
        }

        #endregion 父类的方法

        /// <summary>
        /// 通知他人视图将被启用，那些实现OnActive委托函数的
        /// </summary>
        public void FireActiveEvent()
        {
            ViewDidAppear();

            if (OnActive != null)
            {
                OnActive(this);
            }
        }

        /// <summary>
        /// 通知他人视图将被停用，那些实现OnDeactive委托函数的
        /// </summary>
        public void FireDeactiveEvent()
        {
            ViewWillDisappear();

            if (OnDeactive != null)
            {
                OnDeactive(this);
            }
        }
    }
}