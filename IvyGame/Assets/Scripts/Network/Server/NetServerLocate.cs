namespace Game.Network.Server
{
    internal static class NetServerLocate
    {
        /// <summary>
        /// 网络服务
        /// </summary>
        public static NetServer Net { get; private set;}

        /// <summary>
        /// 日志
        /// </summary>
        public static NetServerLog Log { get; private set; }

        /// <summary>
        /// 逻辑层中心
        /// </summary>
        public static ServerTokenCenter TokenCenter { get; private set; }

        /// <summary>
        /// 游戏控制层
        /// </summary>
        public static ServerGameplayCtrl GameCtrl { get; private set; }

        public static void Init(NetServer netServer)
        {
            Net = netServer;
            Log = new NetServerLog();
            TokenCenter = new ServerTokenCenter();

            GameCtrl = new ServerGameplayCtrl();
            GameCtrl.Init();
        }

        public static void Clear()
        {
            Net = null;
            Log = null;
            TokenCenter = null;

            GameCtrl?.Clear();
            GameCtrl = null;
        }
    }
}
