using System.Collections.Generic;
using System;

namespace Gameplay
{
    
    public class TbFightDrumsMusicCfg : Dictionary<int, FightDrumsMusicCfg>
    {
        
        public void AddConfig(int key1, FightDrumsMusicCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<FightDrumsMusicCfg> configs)
        {
            foreach (var item in configs)
            {
                FightDrumsMusicCfg config = item;
                AddConfig(config.id, config);
            }
        }

    }

}

