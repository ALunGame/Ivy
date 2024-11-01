using Gameplay.GameData;
using Gameplay.GameMap;
using Gameplay.GameMode;

namespace Gameplay
{
    internal static class GameplayGlobal
    {
        /// <summary>
        /// 控制
        /// </summary>
        public static GameplayCtrl Ctrl
        {
            get
            {
                return GameplayCtrl.Instance;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public static GameDataCtrl Data 
        { 
            get 
            { 
                return GameplayCtrl.Instance.GameData;
            } 
        }

        /// <summary>
        /// 地图
        /// </summary>
        public static GameMapCtrl Map
        {
            get
            {
                return GameplayCtrl.Instance.GameMap;
            }
        }

        /// <summary>
        /// 模式
        /// </summary>
        public static GameModeCtrl Mode
        {
            get
            {
                return GameplayCtrl.Instance.GameMode;
            }
        }
    }
}
