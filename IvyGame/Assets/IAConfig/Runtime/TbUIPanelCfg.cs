using System.Collections.Generic;
using IAUI;
using System;

namespace IAUI
{
    
    public class TbUIPanelCfg : Dictionary<UIPanelDef, UIPanelCfg>
    {
        
        public void AddConfig(UIPanelDef key1, UIPanelCfg config)
        {
            
            if (!this.ContainsKey(key1))
            {
                this.Add(key1, config);
            }
        }

        public void AddConfig(List<UIPanelCfg> configs)
        {
            foreach (var item in configs)
            {
                UIPanelCfg config = item;
                AddConfig(config.id, config);
            }
        }

    }

}

