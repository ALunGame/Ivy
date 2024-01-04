using Game.Network.SDispatcher;
using LiteNetLib;
using System.Collections.Generic;

namespace Game.Network.Server
{
    /// <summary>
    /// 服务连接中心
    /// </summary>
    internal class ServerTokenCenter
    {
        private int currTokenUid = 0;

        private List<ServerToken> serverTokens = new List<ServerToken>();

        private SDispatcherMapping dispatcherMapping = new SDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        private int GenTokenUid()
        {
            currTokenUid++;
            return currTokenUid;
        }

        public void TokenEnter(NetPeer netPeer, JoinPacket joinPacket)
        {
            TokenLeave(netPeer);
            ServerToken newToken = new ServerToken(GenTokenUid(), netPeer, joinPacket);
            netPeer.Tag = newToken;
            serverTokens.Add(newToken);
        }

        public void TokenLeave(NetPeer netPeer)
        {
            ServerToken token = GetServerToken(netPeer);
            if (token != null)
            {
                serverTokens.Remove(token);
            }
        }

        public ServerToken GetServerToken(NetPeer netPeer)
        {
            return (ServerToken)netPeer.Tag;
        }

        public void OnReceiveMsg(NetPeer peer, NetPacketReader reader)
        {
            if (peer.Tag == null)
                return;

            cachedCommand.Deserialize(reader);

            ushort msgId = cachedCommand.MsgId;
            string protoTypeName = cachedCommand.ProtoTypeName;
            byte[] msgData = cachedCommand.MsgData;
            dispatcherMapping.OnReceiveMsg(msgId,ProtoBufTool.Decode(protoTypeName,msgData));
        }
    }
}
