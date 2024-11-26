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

        public static Color New(int pR, int pG, int pB, int pA = 255)
        {
            if (pA == 255)
            {
                return new Color(pR / 255.0f, pG / 255.0f, pB / 255.0f);
            }
            else
            {
                return new Color(pR / 255.0f, pG / 255.0f, pB / 255.0f, pA / 255.0f);
            }
        }

        /// <summary>
        /// 阵营颜色
        /// </summary>
        public static Dictionary<int, Color> CampColorDict = new Dictionary<int, Color>()
        {
            {0,  New(255, 255, 255) },
            {1,  New(255, 182, 193) },
            {2,  New(135, 206, 235) },
            {3,  New(255, 255, 224) },
            {4,  New(124, 252, 0) },
            {5,  New(255, 105, 180) },
            {6,  New(0, 191, 255) },
            {7,  New(240, 230, 140) },
            {8,  New(255, 20, 147) },
            {9,  New(0, 255, 127) },
            {10, New(255, 99, 71) },
            {11, New(144, 238, 144) },
            {12, New(255, 165, 0) },
            {13, New(32, 178, 1703) },
            {14, New(255, 69, 0) },
            {15, New(173, 255, 47) },
            {16, New(255, 215, 0) },
            {17, New(100, 149, 237) },
            {18, New(255, 218, 185) },
            {19, New(255, 240, 245) },
            {20, New(240, 255, 240) },
        };

        //public static Dictionary<int, Color> CampColorDict = new Dictionary<int, Color>()
        //{
        //    {0, Color.white },
        //    {1, Color.green },
        //    {2, Color.blue },
        //    {3, Color.red },
        //};

        public static int MaxGamerCnt = 2;

        static TempConfig()
        {
        }
    }
}
