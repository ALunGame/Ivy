using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    internal static class GameplayLocate
    {
        /// <summary>
        /// 游戏实例
        /// </summary>
        public static GameInstance GameIns { get; private set; }

        public static void SetGameInstance(GameInstance gameInstance)
        {
            GameIns = gameInstance;
        }
    }
}
