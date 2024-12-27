using MemoryPack;
using System;

namespace Gameplay.Map
{
    
    /// <summary>
    /// 杂项配置
    /// </summary>
    [MemoryPackable]
    public partial class MiscCfg
    {
        
        /// <summary>
        /// 杂项名
        /// </summary>
        public string name;

        /// <summary>
        /// 值
        /// </summary>
        public string value;

    }

}

