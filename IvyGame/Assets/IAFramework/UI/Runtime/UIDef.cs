using System.Collections;
using UnityEngine;

namespace LCUI
{
    /// <summary>
    /// 界面显示规则
    /// </summary>
    public enum UIShowRule
    {
        /// <summary>
        /// 覆盖（直接显示不隐藏界面,不加入界面栈）
        /// </summary>
        Overlay_NoNeedBack,

        /// <summary>
        /// 覆盖（直接显示不隐藏界面,加入界面栈）
        /// </summary>
        Overlay,

        /// <summary>
        /// 关闭其他界面，不加入界面栈
        /// </summary>
        HideOther_NoNeedBack,

        /// <summary>
        /// 关闭其他界面，加入界面栈
        /// </summary>
        HideOther,
    }

    /// <summary>
    /// 界面层级
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 基础界面，类似主界面
        /// </summary>
        Base,

        /// <summary>
        /// 从主界面打开
        /// </summary>
        First,

        /// <summary>
        /// 功能界面打开
        /// </summary>
        Second,

        /// <summary>
        /// 扩展
        /// </summary>
        Three,

        /// <summary>
        /// 最上层，一般是弹窗
        /// </summary>
        Top,
    }

    /// <summary>
    /// UI画布类型
    /// </summary>
    public enum UICanvasType
    {
        /// <summary>
        /// 静态界面，没有大量快速移动的UI元素
        /// </summary>
        Static,

        /// <summary>
        /// 动态界面，比如战斗飘字
        /// </summary>
        Dynamic,
    }
}