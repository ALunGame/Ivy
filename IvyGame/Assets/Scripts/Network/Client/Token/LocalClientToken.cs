using Game.Network.CDispatcher;
using LiteNetLib;
using ProtoBuf;
using System;

namespace Game.Network.Client
{
    /// <summary>
    /// 负责消息的接收和派发
    /// </summary>
    internal class LocalClientToken : ClientToken
    {
        private CDispatcherMapping dispatcherMapping = new CDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        public string PlayerUid {  get; private set; }

        public void SetLocalPlayerUid(string playerUid)
        {
            PlayerUid = playerUid;
        }

        public void OnReceiveMsg(NetPeer peer, NetPacketReader reader)
        {

            cachedCommand.Deserialize(reader);

            ushort msgId = cachedCommand.MsgId;
            string protoTypeName = cachedCommand.ProtoTypeName;
            byte[] msgData = cachedCommand.MsgData;

            Logger.Client?.Log($"<color=#8CBBE0>Rec:{msgId}->{protoTypeName}</color>");

            dispatcherMapping.OnReceiveMsg(msgId, ProtoBufTool.Decode(protoTypeName, msgData));
        }

        public void AddListen<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            dispatcherMapping.AddListen(msgId, msgFunc);
        }

        public void RemoveListen<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            dispatcherMapping.RemoveListen(msgId, msgFunc);
        }
    }
}
