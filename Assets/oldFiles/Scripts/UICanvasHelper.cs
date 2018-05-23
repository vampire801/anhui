//————————————————————————————————————————————
//  UICanvasHelper.cs
//
//  Created by Chiyu Ren on 2016-08-28 00:02
//————————————————————————————————————————————

using UnityEngine;
using UnityEngine.UI;


namespace TooSimpleFramework.Components
{
    /// <summary>
    /// UI画布助手
    /// </summary>
    public class UICanvasHelper : MonoBehaviour
    {
        #region Public Members
        public CanvasScaler UICanvasScaler;
        public Camera UICamera;
        #endregion


        #region Properties
        public static UICanvasHelper Instance { get; private set; }
        #endregion


        #region Private Members
        private float m_fWidthScale = -1;
        private float m_fHeightScale = -1;
        private float m_fMatchValue = -1;
        #endregion


        void Start()
        {
            Instance = this;

            this._SetUIMatch();
            this._SetSizeScale();
        }


        void OnDestroy()
        {
            Instance = null;
        }


        #region Public Methods
        /// <summary>
        /// 世界坐标转换为屏幕坐标
        /// </summary>
        public Vector2 WorldToScreenPoint(Vector3 pWorldPosition, Camera pCamera = null)
        {
            if (pCamera == null)
            {
                pCamera = Camera.main;
            }

#if UNITY_EDITOR // 编辑器模式可能随时要调整画面大小
            this._SetSizeScale();
#endif

            Vector2 ret = pCamera.WorldToScreenPoint(pWorldPosition);
            this._SetPositionScale(ref ret);

            return ret;
        }

        /// <summary>
        /// 屏幕坐标转换为UI坐标
        /// </summary>
        public void ScreenToUIPoint(ref Vector2 pPosition)
        {
#if UNITY_EDITOR // 编辑器模式可能随时要调整画面大小
            this._SetSizeScale();
#endif

            this._SetPositionScale(ref pPosition);
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// 设置分辨率适配比例
        /// </summary>
        private void _SetUIMatch()
        {
            this.UICanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            var scale = Screen.width / (float)(Screen.height);
            if (scale > 1.5f)
            {
                this.m_fMatchValue = 1;
            }
            else if (scale < 1.4f)
            {
                this.m_fMatchValue = 0;
            }
            else
            {
                this.m_fMatchValue = 0.5f;
            }
            this.UICanvasScaler.matchWidthOrHeight = this.m_fMatchValue;
        }

        /// <summary>
        /// 设置尺寸缩放比例
        /// </summary>
        private void _SetSizeScale()
        {
            this.m_fWidthScale = this.UICanvasScaler.referenceResolution.x / Screen.width;
            this.m_fHeightScale = this.UICanvasScaler.referenceResolution.y / Screen.height;
        }

        /// <summary>
        /// 将传入的坐标转换为缩放后的值
        /// </summary>
        private void _SetPositionScale(ref Vector2 pPosition)
        {
            pPosition.x = (pPosition.x - Screen.width * 0.5f) * ((1 - this.m_fMatchValue) * this.m_fWidthScale + this.m_fMatchValue * m_fHeightScale);
            pPosition.y = (pPosition.y - Screen.height * 0.5f) * ((1 - this.m_fMatchValue) * this.m_fWidthScale + this.m_fMatchValue * m_fHeightScale);
        }
        #endregion
    }
}