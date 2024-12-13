using IAFramework.Server;

namespace IAFramework
{
    /// <summary>
    /// 游戏全局环境
    /// 1，负责注入游戏运行服务
    /// 2，提供静态获取接口
    /// 3，全局静态类
    /// </summary>
    public static class GameEnv
    {
        /// <summary>
        /// 资源
        /// </summary>
        public static AssetServer Asset { get; private set; }

        /// <summary>
        /// 音乐
        /// </summary>
        public static AudioServer Audio { get; private set; }

        public static void Init()
        {
            Asset = new AssetServer();
            Asset.Init();

            Audio = new AudioServer();
            Audio.Init();
        }
        
        public static void Clear()
        {
            Asset.Clear();
            Audio.Clear();
        }
    }
}