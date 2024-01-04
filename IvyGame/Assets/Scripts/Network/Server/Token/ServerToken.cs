using LiteNetLib;

namespace Game.Network.Server
{
    internal class ServerToken
    {
        public readonly int TokenUid;
        public readonly NetPeer Peer;
        public readonly JoinPacket JoinPacket;

        public ServerToken(int tokenUid, NetPeer peer, JoinPacket joinPacket)
        {
            TokenUid = tokenUid;
            Peer = peer;
            JoinPacket = joinPacket;
        }
    }
}
