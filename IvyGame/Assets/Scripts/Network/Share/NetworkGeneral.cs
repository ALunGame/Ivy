using System;

namespace Game.Network
{
    internal class NetworkGeneral
    {
        public static readonly int PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;

        /// <summary>
        /// 服务器端口
        /// </summary>
        public const int ServerPort = 10515;

        /// <summary>
        /// 连接标识
        /// </summary>
        public const string NetConnectKey = "IvyGame";
    }

    public enum PlayerState
    {
        /// <summary>
        /// 存活
        /// </summary>
        Alive,

        /// <summary>
        /// 死亡
        /// </summary>
        Die,
    }
}
