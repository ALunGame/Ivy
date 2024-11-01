using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace IAUI
{
    /// <summary>
    /// UI中心
    /// </summary>
    public class UICenter : MonoBehaviour
    {
        [Header("UI相机")]
        [SerializeField]
        private Camera uiCamera;

        [Header("静态画布")]
        [SerializeField]
        private UICanvas staticCanvas;

        [Header("动态画布")]
        [SerializeField]
        private UICanvas dynamicCanvas;

        public UICanvas StaticCanvas { get => staticCanvas; }
        public UICanvas DynamicCanvas { get => dynamicCanvas; }

        public Camera UICamera { get => uiCamera; }

        private event Action updateFunc;

        private event Action fixedUpdateFunc;

        private void Awake()
        {
        }

        public void Init()
        {
            UILocate.SetUICenter(this);
            UILocate.Init();
        }

        private void Update()
        {
            updateFunc?.Invoke();
        }

        private void FixedUpdate()
        {
            fixedUpdateFunc?.Invoke();
        }

        private void OnDestroy()
        {
            updateFunc = null;
            fixedUpdateFunc = null;
            UILocate.Clear();
        }

        /// <summary>
        /// 获得UI层级的父节点
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="canvasType"></param>
        /// <returns></returns>
        public RectTransform GetUILayerTrans(UILayer layer, UICanvasType canvasType = UICanvasType.Static)
        {
            UICanvas canvas = canvasType == UICanvasType.Static ? StaticCanvas : dynamicCanvas;
            switch (layer)
            {
                case UILayer.Base:
                    return canvas.BaseTrans;
                case UILayer.First:
                    return canvas.FirstTrans;
                case UILayer.Second:
                    return canvas.SecondTrans;
                case UILayer.Three:
                    return canvas.ThreeTrans;
                case UILayer.Top:
                    return canvas.TopTrans;
                default:
                    break;
            }
            return canvas.BaseTrans;
        }

        /// <summary>
        /// 注册Update函数
        /// </summary>
        /// <param name="pUpdateFunc"></param>
        public void RegUpdateFunc(Action pUpdateFunc)
        {
            if (pUpdateFunc == null)
            {
                return;
            }
            updateFunc += pUpdateFunc;
        }

        /// <summary>
        /// 清除Update函数
        /// </summary>
        /// <param name="pUpdateFunc"></param>
        public void RemoveUpdateFunc(Action pUpdateFunc)
        {
            if (pUpdateFunc == null)
            {
                return;
            }
            updateFunc -= pUpdateFunc;
        }

        /// <summary>
        /// 注册Update函数
        /// </summary>
        /// <param name="pUpdateFunc"></param>
        public void RegFixedUpdateFunc(Action pfixedUpdateFunc)
        {
            if (pfixedUpdateFunc == null)
            {
                return;
            }
            fixedUpdateFunc += pfixedUpdateFunc;
        }

        /// <summary>
        /// 清除Update函数
        /// </summary>
        /// <param name="pUpdateFunc"></param>
        public void RemoveFixedUpdateFunc(Action pfixedUpdateFunc)
        {
            if (pfixedUpdateFunc == null)
            {
                return;
            }
            fixedUpdateFunc -= pfixedUpdateFunc;
        }
    }
}