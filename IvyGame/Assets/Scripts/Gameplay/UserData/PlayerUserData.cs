using Proto;
using System.Collections.Generic;

namespace Gameplay.UserData
{
    internal class GameUserData
    {
        public List<PlayerInfo> PlayerInfos {  get; private set; }

        public GameUserData() 
        { 
            PlayerInfos = new List<PlayerInfo>();
        }

        public PlayerInfo GetPlayerInfo(int playerUid)
        {
            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                if (PlayerInfos[i].Uid == playerUid)
                {
                    return PlayerInfos[i];
                }
            }
            return null;
        }

        public void RemovePlayerInfo(int playerUid)
        {
            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                if (PlayerInfos[i].Uid == playerUid)
                {
                    PlayerInfos.RemoveAt(i);
                }
            }
        }

        public void UpdatePlayerInfo(PlayerInfo info)
        {
            RemovePlayerInfo(info.Uid);
            PlayerInfos.Add(info);
        }
    }
}
