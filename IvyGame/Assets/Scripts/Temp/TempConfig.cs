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
            {1,  ColorEx.New(255, 182, 193) },
            {2,  ColorEx.New(135, 206, 235) },
            {3,  ColorEx.New(255, 255, 224) },
            {4,  ColorEx.New(124, 252, 0) },
            {5,  ColorEx.New(255, 105, 180) },
            {6,  ColorEx.New(0, 191, 255) },
            {7,  ColorEx.New(240, 230, 140) },
            {8,  ColorEx.New(255, 20, 147) },
            {9,  ColorEx.New(0, 255, 127) },
            {10, ColorEx.New(255, 99, 71) },
            {11, ColorEx.New(144, 238, 144) },
            {12, ColorEx.New(255, 165, 0) },
            {13, ColorEx.New(32, 178, 1703) },
            {14, ColorEx.New(255, 69, 0) },
            {15, ColorEx.New(173, 255, 47) },
            {16, ColorEx.New(255, 215, 0) },
            {17, ColorEx.New(100, 149, 237) },
            {18, ColorEx.New(255, 218, 185) },
            {19, ColorEx.New(255, 240, 245) },
            {20, ColorEx.New(240, 255, 240) },
        };

        public static int MaxGamerCnt = 2;

        static TempConfig()
        {
        }
    }
}
