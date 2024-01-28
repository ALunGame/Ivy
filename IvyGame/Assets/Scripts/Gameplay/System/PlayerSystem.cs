using Game.Network.Client;
using Game.Network.Server;
using Gameplay.Player;
using IAEngine;
using Proto;
using System.Collections.Generic;

namespace Gameplay.System
{
    internal class PlayerSystem : GameplaySystem
    {
        protected override void OnStartGame()
        {
            List<PlayerInfo> playerInfos = GameplayLocate.UserData.Game.PlayerInfos;
            foreach (PlayerInfo info in playerInfos)
            {
                AddGamePlayerInfo addInfo = new AddGamePlayerInfo(info.Uid, info.Id, info.Name, (byte)info.Camp);
                addInfo.PosX = info.Pos != null ? (byte)info.Pos.X : (byte)0;
                addInfo.PosY = info.Pos != null ? (byte)info.Pos.Y : (byte)0;
                addInfo.IsLocalPlayer = info.Uid == NetClientLocate.LocalToken.PlayerUid;
                GameplayLocate.GameIns.AddPlayer(addInfo);
            }
        }

        protected override void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
            if (NetServerLocate.Game == null || NetServerLocate.Game.Room == null)
            {
                return;
            }
            List<ServerPlayer> players = NetServerLocate.Game.Room.GetPlayers();
            if (players.IsLegal())
            {
                foreach (ServerPlayer player in players)
                {
                    if (player != null)
                    {
                        if (player.State == Game.Network.PlayerState.Die)
                        {
                            player.RebornTime -= pDeltaTime;
                            if (player.RebornTime <= 0)
                            {
                                NetServerLocate.Game.RebornPlayer(player.Uid);
                            }
                        }
                    }
                }
            }
        }
    }
}
