using MemoryPack;
using System;

namespace Gameplay.Map
{
    
    /// <summary>
    /// 演员配置
    /// </summary>
    [MemoryPackable]
    public partial class ActorCfg
    {
        
        /// <summary>
        /// 演员Id
        /// </summary>
        public int id;

        /// <summary>
        /// 演员名
        /// </summary>
        public string name;

        /// <summary>
        /// 图片
        /// </summary>
        public string img;

        /// <summary>
        /// 预制体
        /// </summary>
        public string prefab;

    }

}

