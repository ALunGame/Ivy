using System;

namespace Game.Network.Client
{
    internal static class NetClientLocate
    {
        /// <summary>
        /// 网络服务
        /// </summary>
        public static NetClient Net { get; private set; }

        /// <summary>
        /// 逻辑层中心
        /// </summary>
        public static LocalClientToken LocalToken { get; private set; }

        private static Action onLogicUpdateCallBack;

        public static void Init(NetClient netServer)
        {
            Net = netServer;
            LocalToken = new LocalClientToken();
            onLogicUpdateCallBack = null;
        }

        /// <summary>
        /// 网络逻辑帧Update
        /// </summary>
        public static void OnNetWorkLogicUpdate()
        {
            onLogicUpdateCallBack?.Invoke();
        }

        /// <summary>
        /// 注册网络帧更新回调
        /// </summary>
        /// <param name="pCallBack"></param>
        public static void RegLogicUpdateCallBack(Action pCallBack)
        {
            onLogicUpdateCallBack += pCallBack;
        }

        /// <summary>
        /// 移除网络帧更新回调
        /// </summary>
        /// <param name="pCallBack"></param>
        public static void RemoveLogicUpdateCallBack(Action pCallBack)
        {
            onLogicUpdateCallBack -= pCallBack;
        }

        public static void Clear()
        {
            Net = null;
            LocalToken = null;
            onLogicUpdateCallBack = null;
        }
    }
}
