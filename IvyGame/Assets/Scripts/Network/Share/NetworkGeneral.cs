using System;

namespace Game.Network
{
    internal static class NetworkGeneral
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

        /// <summary>
        /// 基础速度
        /// </summary>
        public const float BaseSpeed = 35;

        /// <summary>
        /// 编码移动消息数据
        /// </summary>
        /// <param name="moveDir"></param>
        /// <param name="moveDel"></param>
        /// <returns></returns>
        public static int EncodeMoveMsgValue(float moveDel)
        {
            return (int)(moveDel * 100);
        }

        public static float DecodeMoveMsgValue(int moveDel)
        {
            return moveDel / 100.0f;
        }
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
