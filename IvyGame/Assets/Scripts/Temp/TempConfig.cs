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
            {0, Color.white },
            {1, new Color(255, 182, 193) },
            {2, new Color(135, 206, 235) },
            {3, new Color(255, 255, 224) },
            {4, new Color(124, 252, 0) },
            {5, new Color(255, 105, 180) },
            {6, new Color(0, 191, 255) },
            {7, new Color(240, 230, 140) },
            {8, new Color(255, 20, 147) },
            {9, new Color(0, 255, 127) },
            {10, new Color(255, 99, 71) },
            {11, new Color(144, 238, 144) },
            {12, new Color(255, 165, 0) },
            {13, new Color(32, 178, 1703) },
            {14, new Color(255, 69, 0) },
            {15, new Color(173, 255, 47) },
            {16, new Color(255, 215, 0) },
            {17, new Color(100, 149, 237) },
            {18, new Color(255, 218, 185) },
            {19, new Color(255, 240, 245) },
            {20, new Color(240, 255, 240) },
        };

        public static int MaxGamerCnt = 2;

        static TempConfig()
        {
        }
    }
}
