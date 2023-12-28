using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace IAUI
{
    /// <summary>
    /// UI面板
    /// </summary>
    public abstract class InternalUIPanel
    {
        #region 界面数据

        public abstract UIModel Model { get; }

        #endregion

        #region 内部字段

        /// <summary>
        /// 界面节点
        /// </summary>
        public Transform transform { get; private set; }

        public RectTransform rectTransform { get; private set; }

        public void SetTransform(Transform trans)
        {
            transform = trans;
            rectTransform = trans.GetComponent<RectTransform>();
        }

        #endregion

        #region 胶水字段

        private List<UIGlue> glues = new List<UIGlue>();

        public IReadOnlyList <UIGlue> Glues { get => glues;}

        #endregion

        #region 生命周期

        public InternalUIPanel()
        {
            foreach (var item in ReflectionHelper.GetFieldInfos(this.GetType()))
            {
                object value = item.GetValue(this);
                if (value != null && value is UIGlue)
                {
                    glues.Add((UIGlue)value);
                }
            }
        }

        /// <summary>
        /// 创建时初始化
        /// </summary>
        public virtual void Awake()
        {
            if (transform != null)
            {
                transform.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            if (transform != null)
            {
                transform.gameObject.SetActive(true);
                transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            if (transform != null)
            {
                transform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Destroy()
        {
            if (transform!=null)
            {
                GameObject.Destroy(transform);
            }
        }

        #endregion
    }
}