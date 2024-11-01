using LiteNetLib.Utils;

namespace Game.Network
{
    public enum PacketType : byte
    {
        /// <summary>
        /// 客户端发现服务器
        /// </summary>
        Discovery,

        /// <summary>
        /// Proto 数据
        /// </summary>
        Proto,
    }

    /// <summary>
    /// 客户端发现服务器
    /// </summary>
    public class DiscoveryPacket
    {
        public string DiscoveryStr { get; set; }
    }

    public struct NetProtoPacket : INetSerializable
    {
        public ushort MsgId;
        public string ProtoTypeName;
        public byte[] MsgData;

        public void PutMsgId(ushort msgId)
        {
            MsgId = msgId;
        }

        public void PutProtoTypeName(string protoTypeName)
        {
            ProtoTypeName = protoTypeName;
        }

        public void PutMsgData(byte[] msgData)
        {
            MsgData = msgData;
        }

        public void Deserialize(NetDataReader reader)
        {
            MsgId = reader.GetUShort();
            ProtoTypeName = reader.GetString();
            MsgData = reader.GetRemainingBytes();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(MsgId);
            writer.Put(ProtoTypeName);
            writer.Put(MsgData);
        }
    }
}
