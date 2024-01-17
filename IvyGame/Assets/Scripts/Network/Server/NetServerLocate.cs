using Gameplay;

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
        /// 游戏
        /// </summary>
        public static ServerGameInstance Game { get; private set; }

        public static void Init(NetServer netServer)
        {
            Net = netServer;
            Log = new NetServerLog();
            TokenCenter = new ServerTokenCenter();
        }

        public static void Clear()
        {
            Net = null;
            Log = null;
            TokenCenter = null;
            Game = null;
        }

        public static void StartGame(int cfgId, GameModeType modeType)
        {
            Game = new ServerGameInstance();
            Game.Create(cfgId,modeType);
        }
    }
}
