using System.Collections.Generic;
using System;
using UnityEngine;

namespace Gameplay.Map
{
    
    public class TbMapCfg : Dictionary<int, MapCfg>
    {
        
        public void AddConfig(int key1, MapCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<MapCfg> configs)
        {
            foreach (var item in configs)
            {
                MapCfg config = item;
                AddConfig(config.id, config);
            }
        }

    }

}

