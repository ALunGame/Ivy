using System.Collections.Generic;
using System;

namespace Gameplay.Map
{
    
    public class TbActorCfg : Dictionary<int, ActorCfg>
    {
        
        public void AddConfig(int key1, ActorCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<ActorCfg> configs)
        {
            foreach (var item in configs)
            {
                ActorCfg config = item;
                AddConfig(config.id, config);
            }
        }

    }

}

