using Game.Network.SDispatcher;
using LiteNetLib;

namespace Game.Network.Server
{
    /// <summary>
    /// 服务连接中心
    /// 1，负责消息的派发
    /// 2，连接对象状态改变消息的派发
    /// </summary>
    internal class ServerTokenCenter
    {
        private SDispatcherMapping dispatcherMapping = new SDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        public ServerTokenCenter()
        {
        }

        public void TokenEnter(NetPeer netPeer)
        {
        }

        public void TokenLeave(NetPeer netPeer)
        {
        }

        public void OnReceiveMsg(NetPeer peer, NetPacketReader reader)
        {
            cachedCommand.Deserialize(reader);

            ushort msgId = cachedCommand.MsgId;
            string protoTypeName = cachedCommand.ProtoTypeName;
            byte[] msgData = cachedCommand.MsgData;

            Logger.Server?.LogWarning($"<color=#E0C881>Rec:{msgId}->{protoTypeName}</color>");

            dispatcherMapping.OnReceiveMsg(peer, msgId,ProtoBufTool.Decode(protoTypeName,msgData));
        }
    }
}
