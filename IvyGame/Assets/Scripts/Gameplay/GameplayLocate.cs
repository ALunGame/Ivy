using Gameplay.UserData;
using UnityEngine;

namespace Gameplay
{
    internal static class GameplayLocate
    {
        /// <summary>
        /// 游戏实例
        /// </summary>
        public static GameInstance GameIns { get; private set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        public static UserDataCenter UserData { get; private set; }

        static GameplayLocate()
        {
            UserData = new UserDataCenter();
            UserData.Init();
        }

        public static void SetGameInstance(GameInstance pIns)
        {
            GameIns = pIns;
        }

        public static void CreateGame(GameModeType modeType, bool isRoomOwner)
        {
            if (GameIns != null )
            {
                Object.Destroy(GameIns.gameObject);
            }

            GameObject gameInsGo = new GameObject("GameInstance");
            gameInsGo.transform.localPosition = new Vector3(0, -500, 0);

            GameInstance newIns = gameInsGo.AddComponent<GameInstance>(); 
            newIns.Init(modeType, isRoomOwner);
        }
    }
}
