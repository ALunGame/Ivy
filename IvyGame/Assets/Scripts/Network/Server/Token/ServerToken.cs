using LiteNetLib;
using Proto;

namespace Game.Network.Server
{
    internal class ServerToken
    {
        public readonly int TokenUid;
        public readonly NetPeer Peer;
        public readonly PlayerInfo Player;

        public ServerToken(int tokenUid, NetPeer peer, PlayerInfo info)
        {
            TokenUid = tokenUid;
            Peer = peer;
            Player = info;
        }
    }
}
