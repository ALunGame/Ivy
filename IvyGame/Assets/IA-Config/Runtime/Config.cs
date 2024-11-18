
using System;
using System.Collections.Generic;
using IAFramework;
using MemoryPack;
using IAUI;
using Gameplay.Map;
using Gameplay;


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

        private static TbMapCfg _MapCfg = null;
        /// <summary>
        /// 地图配置
        /// </summary>
        public static TbMapCfg MapCfg
        {
            get
            {
                if (_MapCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbMapCfg");
                    List<MapCfg> configs = MemoryPackSerializer.Deserialize<List<MapCfg>>(byteArray);
                    _MapCfg = new TbMapCfg();
                    _MapCfg.AddConfig(configs);
                }
                return _MapCfg;
            }
        }

        private static TbActorCfg _ActorCfg = null;
        /// <summary>
        /// 演员配置
        /// </summary>
        public static TbActorCfg ActorCfg
        {
            get
            {
                if (_ActorCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbActorCfg");
                    List<ActorCfg> configs = MemoryPackSerializer.Deserialize<List<ActorCfg>>(byteArray);
                    _ActorCfg = new TbActorCfg();
                    _ActorCfg.AddConfig(configs);
                }
                return _ActorCfg;
            }
        }

        private static TbGameLevelCfg _GameLevelCfg = null;
        /// <summary>
        /// 关卡配置
        /// </summary>
        public static TbGameLevelCfg GameLevelCfg
        {
            get
            {
                if (_GameLevelCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbGameLevelCfg");
                    List<GameLevelCfg> configs = MemoryPackSerializer.Deserialize<List<GameLevelCfg>>(byteArray);
                    _GameLevelCfg = new TbGameLevelCfg();
                    _GameLevelCfg.AddConfig(configs);
                }
                return _GameLevelCfg;
            }
        }

        private static TbMiscCfg _MiscCfg = null;
        /// <summary>
        /// 杂项配置
        /// </summary>
        public static TbMiscCfg MiscCfg
        {
            get
            {
                if (_MiscCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbMiscCfg");
                    List<MiscCfg> configs = MemoryPackSerializer.Deserialize<List<MiscCfg>>(byteArray);
                    _MiscCfg = new TbMiscCfg();
                    _MiscCfg.AddConfig(configs);
                }
                return _MiscCfg;
            }
        }

        private static TbFightDrumsMusicCfg _FightDrumsMusicCfg = null;
        /// <summary>
        /// 战斗鼓点配置
        /// </summary>
        public static TbFightDrumsMusicCfg FightDrumsMusicCfg
        {
            get
            {
                if (_FightDrumsMusicCfg == null)
                {
                    Byte[] byteArray = GameEnv.Asset.LoadBytes("TbFightDrumsMusicCfg");
                    List<FightDrumsMusicCfg> configs = MemoryPackSerializer.Deserialize<List<FightDrumsMusicCfg>>(byteArray);
                    _FightDrumsMusicCfg = new TbFightDrumsMusicCfg();
                    _FightDrumsMusicCfg.AddConfig(configs);
                }
                return _FightDrumsMusicCfg;
            }
        }


        public static void Preload()
        {

            var tPreLoadMapCfg = Config.MapCfg;

            var tPreLoadActorCfg = Config.ActorCfg;

            var tPreLoadGameLevelCfg = Config.GameLevelCfg;

            var tPreLoadMiscCfg = Config.MiscCfg;

            var tPreLoadFightDrumsMusicCfg = Config.FightDrumsMusicCfg;

        }

        public static void Reload()
        {

            if(_UIPanelCfg!= null)
				_UIPanelCfg.Clear();

            if(_MapCfg!= null)
				_MapCfg.Clear();

            if(_ActorCfg!= null)
				_ActorCfg.Clear();

            if(_GameLevelCfg!= null)
				_GameLevelCfg.Clear();

            if(_MiscCfg!= null)
				_MiscCfg.Clear();

            if(_FightDrumsMusicCfg!= null)
				_FightDrumsMusicCfg.Clear();

        }
    }
}
