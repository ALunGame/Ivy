
using System;
using System.Collections.Generic;
using IAFramework;
using MemoryPack;
using IAUI;


namespace IAConfig
{
    public static class Config
    {
        
        private static TbUIPanelCfg _UIPanelCfg = null;
        /// <summary>
        /// 界面配置
        /// </summary>
        public static TbUIPanelCfg UIPanelCfg
        {
            get
            {
                if (_UIPanelCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbUIPanelCfg");
                    List<UIPanelCfg> configs = MemoryPackSerializer.Deserialize<List<UIPanelCfg>>(byteArray);
                    _UIPanelCfg = new TbUIPanelCfg();
                    _UIPanelCfg.AddConfig(configs);
                }
                return _UIPanelCfg;
            }
        }


        public static void Reload()
        {

            if(_UIPanelCfg!= null)
				_UIPanelCfg.Clear();

        }
    }
}
