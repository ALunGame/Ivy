namespace Game.Network.Client
{
    internal static class NetClientLocate
    {
        /// <summary>
        /// 网络服务
        /// </summary>
        public static NetClient Net { get; private set; }

        /// <summary>
        /// 日志
        /// </summary>
        public static NetClientLog Log { get; private set; }

        /// <summary>
        /// 逻辑层中心
        /// </summary>
        public static LocalClientToken LocalToken { get; private set; }

        public static void Init(NetClient netServer)
        {
            Net = netServer;
            Log = new NetClientLog();
            LocalToken = new LocalClientToken();
        }

        public static void Clear()
        {
            Net = null;
            Log = null;
            LocalToken = null;
        }
    }
}
