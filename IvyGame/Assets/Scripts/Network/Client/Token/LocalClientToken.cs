using Game.Network.CDispatcher;
using LiteNetLib;

namespace Game.Network.Client
{
    internal class LocalClientToken : ClientToken
    {
        private CDispatcherMapping dispatcherMapping = new CDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        public void OnReceiveMsg(NetPeer peer, NetPacketReader reader)
        {
            if (peer.Tag == null)
                return;

            cachedCommand.Deserialize(reader);

            ushort msgId = cachedCommand.MsgId;
            string protoTypeName = cachedCommand.ProtoTypeName;
            byte[] msgData = cachedCommand.MsgData;
            dispatcherMapping.OnReceiveMsg(msgId, ProtoBufTool.Decode(protoTypeName, msgData));
        }

    }
}
