using IAEngine;
using System.Collections.Generic;

namespace IAUI
{
    public class UIModel
    {
        private List<IBindable> bindFields = new List<IBindable>();

        /// <summary>
        /// 绑定字段
        /// </summary>
        public List<IBindable> BindFields { get => bindFields; }


        public UIModel()
        {
            foreach (var item in ReflectionHelper.GetFieldInfos(this.GetType()))
            {
                object value = item.GetValue(this);
                if (value != null && value is IBindable)
                {
                    bindFields.Add((IBindable)value);
                }
            }
        }

        /// <summary>
        /// 刷新绑定
        /// </summary>
        public void RefreshBindable()
        {
            for (int i = 0; i < bindFields.Count; i++)
            {
                bindFields[i].ValueChanged();
            }
        }

        /// <summary>
        /// 清楚绑定事件
        /// </summary>
        public void ClearEvent()
        {
            for (int i = 0; i < bindFields.Count; i++)
            {
                bindFields[i].ClearChangedEvent();
            }
        }
    }
}
