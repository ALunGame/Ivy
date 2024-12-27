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
        /// 音乐名
        /// </summary>
        public string name;

        /// <summary>
        /// 音乐资源名
        /// </summary>
        public string res;

        /// <summary>
        /// 每分钟节拍数
        /// </summary>
        public float bpm;

    }

}

