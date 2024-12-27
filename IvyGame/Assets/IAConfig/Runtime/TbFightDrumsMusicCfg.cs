using System.Collections.Generic;
using System;

namespace Gameplay
{
    
    public class TbFightDrumsMusicCfg : Dictionary<string, FightDrumsMusicCfg>
    {
        
        public void AddConfig(string key1, FightDrumsMusicCfg config)
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
                AddConfig(config.name, config);
            }
        }

    }

}

