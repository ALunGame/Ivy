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

        /// <summary>
        /// UI更新 时间缩放
        /// </summary>
        public float TimeScale { get; set; }
        /// <summary>
        /// UI更新 时间间隔
        /// </summary>
        public float DeltaTime { get; set; }
        /// <summary>
        /// UI更新 总时间
        /// </summary>
        public float TotalTime { get; set; }

        private event Action<float, float> updateFunc;

        private void Awake()
        {
        }

        public void Init()
        {
            TimeScale = 1.0f;
            TotalTime = 0.0f;
            DeltaTime = Time.deltaTime;

            UILocate.SetUICenter(this);
            UILocate.Init();
        }

        private void Update()
        {
            float deltaTime = DeltaTime * TimeScale;
            TotalTime += deltaTime;
            updateFunc?.Invoke(deltaTime, TotalTime);
        }

        private void OnDestroy()
        {
            updateFunc = null;
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
        public void RegUpdateFunc(Action<float, float> pUpdateFunc)
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
        public void RemoveUpdateFunc(Action<float, float> pUpdateFunc)
        {
            if (pUpdateFunc == null)
            {
                return;
            }
            updateFunc -= pUpdateFunc;
        }
    }
}