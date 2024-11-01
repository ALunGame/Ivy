using System.Collections.Generic;
using System;

namespace Gameplay.Map
{
    
    public class TbGameLevelCfg : Dictionary<int, GameLevelCfg>
    {
        
        public void AddConfig(int key1, GameLevelCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<GameLevelCfg> configs)
        {
            foreach (var item in configs)
            {
                GameLevelCfg config = item;
                AddConfig(config.id, config);
            }
        }

    }

}

