using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum MoveClickType
    {
        Miss,
        Normal,
        Good,
        Perfect,
    }

    public class MoveClickCfg
    {
        public float buffTime;              //添加速度时间
        public float addSpeedRate;          //添加速度倍率

        public MoveClickCfg(float pBuffTime, float pAddSpeedRate)
        {
            buffTime = pBuffTime;
            addSpeedRate = pAddSpeedRate;
        }
    }

    /// <summary>
    /// 临时配置
    /// </summary>
    public static class TempConfig
    {
        /// <summary>
        /// 点击类型配置
        /// </summary>
        public static Dictionary<MoveClickType, MoveClickCfg> MoveClickCfgDict = new Dictionary<MoveClickType, MoveClickCfg>()
        {
            { MoveClickType.Miss, new MoveClickCfg(0, 1) },
            { MoveClickType.Normal, new MoveClickCfg(1, 1.2f) },
            { MoveClickType.Good, new MoveClickCfg(2, 1.5f) },
            { MoveClickType.Perfect, new MoveClickCfg(3, 2f) },
        };

        /// <summary>
        /// 阵营颜色
        /// </summary>
        public static Dictionary<int, Color> CampColorDict = new Dictionary<int, Color>()
        {
            {0,  ColorEx.New(255, 255, 255) },        
            {1,  ColorEx.New(255, 128, 0) },      // 鲜橙色
            {2,  ColorEx.New(255, 255, 0) },      // 柠檬黄
            {3,  ColorEx.New(128, 255, 0) },      // 青柠绿
            {4,  ColorEx.New(0, 191, 255) },      // 湖蓝色
            {5,  ColorEx.New(0, 0, 255) },        // 深蓝色
            {6,  ColorEx.New(128, 0, 255) },      // 紫罗兰色
            {7,  ColorEx.New(255, 191, 204) },    // 粉红色
            {8,  ColorEx.New(64, 255, 64) },      // 鲜绿色
            {9,  ColorEx.New(255, 215, 0) },      // 金黄色
            {10, ColorEx.New(173, 216, 230) },    // 浅蓝色
            {11, ColorEx.New(255, 0, 128) },      // 玫红色
            {12, ColorEx.New(191, 0, 255) },      // 亮紫色
            {13, ColorEx.New(128, 255, 128) },    // 浅绿色
            {14, ColorEx.New(77, 154, 255) },     // 电蓝色
            {15, ColorEx.New(255, 153, 51) },     // 亮橙色
            {16, ColorEx.New(128, 255, 128) },    // 荧光绿
            {17, ColorEx.New(255, 255, 77) },     // 明黄色
            {18, ColorEx.New(222, 48, 100) },     // 樱桃红
            {19, ColorEx.New(191, 128, 255) },    // 浅紫色
            {20, ColorEx.New(255, 120, 227) },    // 荧光粉色
        };


        public static int MaxGamerCnt = 2;

        static TempConfig()
        {
        }
    }
}
