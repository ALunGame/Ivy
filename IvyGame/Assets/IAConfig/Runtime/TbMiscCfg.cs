using System.Collections.Generic;
using System;

namespace Gameplay.Map
{
    
    public class TbMiscCfg : Dictionary<string, MiscCfg>
    {
        
        public void AddConfig(string key1, MiscCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<MiscCfg> configs)
        {
            foreach (var item in configs)
            {
                MiscCfg config = item;
                AddConfig(config.name, config);
            }
        }

    }

}

