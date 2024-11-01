using MemoryPack;
using System;

namespace Gameplay
{
    
    /// <summary>
    /// 战斗鼓点配置
    /// </summary>
    [MemoryPackable]
    public partial class FightDrumsMusicCfg
    {
        
        /// <summary>
        /// 音乐Id
        /// </summary>
        public int id;

        /// <summary>
        /// 音乐名
        /// </summary>
        public string name;

        /// <summary>
        /// 音乐资源名
        /// </summary>
        public string res;

        /// <summary>
        /// 鼓点间隔时间
        /// </summary>
        public float drumsTime;

    }

}

