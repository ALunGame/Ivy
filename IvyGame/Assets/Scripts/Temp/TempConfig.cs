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
        public static Dictionary<int, Color> CampColorDict = new Dictionary<int, Color>();

        public static int MaxGamerCnt = 20;

        static TempConfig()
        {
            CampColorDict.Add(0, Color.white);
            for (int i = 1; i <= MaxGamerCnt; i++) 
            {
                float a = RandomHelper.Range(0.0f, 1.0f);
                float g = RandomHelper.Range(0.0f, 1.0f);
                float b = RandomHelper.Range(0.0f, 1.0f);

                CampColorDict.Add(i, new Color(a, g, b, 1));
            }
        }
    }
}
