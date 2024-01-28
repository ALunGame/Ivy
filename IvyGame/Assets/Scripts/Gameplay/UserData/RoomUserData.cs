using Proto;
using System.Collections.Generic;

namespace Gameplay.UserData
{
    internal class RoomUserData
    {
        /// <summary>
        /// 房主玩家Id
        /// </summary>
        public int RoomMasterUid { get; private set; }

        /// <summary>
        /// 自己的玩家Id
        /// </summary>
        public int SelfPlayerId { get; private set; }

        public GameModeType GameMode {  get; private set; }
        public List<PlayerInfo> PlayerInfos { get; private set; }

        public RoomUserData()
        {
            GameMode = GameModeType.Local;
            PlayerInfos = new List<PlayerInfo>();
        }

        public bool CheckIsRoomMaster(int playerUid = -1)
        {
            if (playerUid == -1)
            {
                return SelfPlayerId == RoomMasterUid;
            }
            return RoomMasterUid == playerUid;
        }

        public void SetRoomMasterUid(int playerUid)
        {
            RoomMasterUid = playerUid;
        }

        public void SetSelfPlayerId(int playerUid)
        {
            SelfPlayerId = playerUid;
        }

        public void ChangeGameMode(GameModeType newMode)
        {
            GameMode = newMode;
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
