using MemoryPack;
using System;
using UnityEngine;

namespace Gameplay.Map
{
    
    /// <summary>
    /// 地图配置
    /// </summary>
    [MemoryPackable]
    public partial class MapCfg
    {
        
        /// <summary>
        /// 地图Id
        /// </summary>
        public int id;

        /// <summary>
        /// 地图名
        /// </summary>
        public string name;

        /// <summary>
        /// 预制体
        /// </summary>
        public string prefab;

        /// <summary>
        /// 网格尺寸
        /// </summary>
        public Vector2 gridSize;

    }

}

