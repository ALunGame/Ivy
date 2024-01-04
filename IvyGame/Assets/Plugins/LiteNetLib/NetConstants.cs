namespace LiteNetLib
{
    /// <summary>
    /// Sending method type
    /// </summary>
    public enum DeliveryMethod : byte
    {
        /// <summary>
        /// 不可靠的。数据包可以被丢弃，可以被复制，可以没有顺序地到达。
        /// </summary>
        Unreliable = 4,

        /// <summary>
        /// 可靠的。包裹不会丢失，不会重复，可以不按顺序到达。
        /// </summary>
        ReliableUnordered = 0,

        /// <summary>
        /// 不可靠的。数据包可以丢弃，不会重复，会有序到达。
        /// </summary>
        Sequenced = 1,

        /// <summary>
        /// 可靠而有序。数据包不会丢失，不会重复，会有序到达。
        /// </summary>
        ReliableOrdered = 2,

        /// <summary>
        /// 唯一可靠的最后一个数据包。数据包可以丢弃(最后一个除外)，不会重复，将按顺序到达。
        /// 不能分片
        /// </summary>
        ReliableSequenced = 3
    }

    /// <summary>
    /// Network constants. Can be tuned from sources for your purposes.
    /// </summary>
    public static class NetConstants
    {
        //can be tuned
        public const int DefaultWindowSize = 64;
        public const int SocketBufferSize = 1024 * 1024; //1mb
        public const int SocketTTL = 255;

        public const int HeaderSize = 1;
        public const int ChanneledHeaderSize = 4;
        public const int FragmentHeaderSize = 6;
        public const int FragmentedHeaderTotalSize = ChanneledHeaderSize + FragmentHeaderSize;
        public const ushort MaxSequence = 32768;
        public const ushort HalfMaxSequence = MaxSequence / 2;

        //protocol
        internal const int ProtocolId = 13;
        internal const int MaxUdpHeaderSize = 68;
        internal const int ChannelTypeCount = 4;

        internal static readonly int[] PossibleMtu =
        {
            576  - MaxUdpHeaderSize, //minimal (RFC 1191)
            1024,                    //most games standard
            1232 - MaxUdpHeaderSize,
            1460 - MaxUdpHeaderSize, //google cloud
            1472 - MaxUdpHeaderSize, //VPN
            1492 - MaxUdpHeaderSize, //Ethernet with LLC and SNAP, PPPoE (RFC 1042)
            1500 - MaxUdpHeaderSize  //Ethernet II (RFC 1191)
        };

        //Max possible single packet size
        public static readonly int MaxPacketSize = PossibleMtu[PossibleMtu.Length - 1];
        public static readonly int MaxUnreliableDataSize = MaxPacketSize - HeaderSize;

        //peer specific
        public const byte MaxConnectionNumber = 4;
    }
}
