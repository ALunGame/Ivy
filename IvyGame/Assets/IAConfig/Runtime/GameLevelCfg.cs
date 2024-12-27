using MemoryPack;
using System;

namespace Gameplay.Map
{
    
    /// <summary>
    /// 关卡配置
    /// </summary>
    [MemoryPackable]
    public partial class GameLevelCfg
    {
        
        /// <summary>
        /// 关卡Id
        /// </summary>
        public int id;

        /// <summary>
        /// 关卡名
        /// </summary>
        public string name;

        /// <summary>
        /// 地图Id
        /// </summary>
        public int mapId;

        /// <summary>
        /// 关卡类型
        /// </summary>
        public int type;

        /// <summary>
        /// 关卡时长
        /// </summary>
        public int time;

        /// <summary>
        /// 下一个关卡Id
        /// </summary>
        public int nextLevel;

    }

}

