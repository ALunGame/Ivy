using LiteNetLib;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerToken
    {
        public int TokenUid;
        public NetPeer Peer;
        private PlayerInfo Player;

        public ServerToken(int tokenUid, NetPeer peer, JoinPlayerInfo info)
        {
            TokenUid = tokenUid;
            Peer = peer;
            
            Player = new PlayerInfo();
            Player.Uid = tokenUid;
            Player.Name = info.Name;
            Player.Id = info.Id;
            Player.Camp = 0;               
        }

        /// <summary>
        /// 收集最新的玩家数据
        /// </summary>
        public PlayerInfo CollectPlayerInfo()
        {
            if (NetServerLocate.Game == null || NetServerLocate.Game.Room == null)
            {
                return Player;
            }
            else
            {
                ServerPlayer serverPlayer = NetServerLocate.Game.Room.GetPlayer(TokenUid);
                if (serverPlayer != null)
                {
                    Player.Camp = serverPlayer.Camp;
                    Player.Pos = new NetVector2()
                    {
                        X = serverPlayer.GridPos.x,
                        Y = serverPlayer.GridPos.y,
                    };
                }
            }
            return Player;
        }
    }
}
