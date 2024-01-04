using LiteNetLib.Utils;

namespace Game.Network
{
    public enum PacketType : byte
    {
        /// <summary>
        /// 加入房间，连接开始
        /// </summary>
        JoinRoom,

        /// <summary>
        /// Proto 数据
        /// </summary>
        Proto,
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    public class JoinPacket
    {
        public string UserName { get; set; }
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
            MsgData = reader.GetRemainingBytes();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(MsgId);
            writer.Put(MsgData);
        }
    }
}
