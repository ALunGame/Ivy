using UnityEngine;
using UnityEngine.UI;

namespace LCUI
{
    public static class UIUtil
    {
        /// <summary>
        /// 刷新布局
        /// </summary>
        /// <param name="rectTrans"></param>
        public static void RebuildLayout(this RectTransform rectTrans)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
        }
    }
}
