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
    }
}
