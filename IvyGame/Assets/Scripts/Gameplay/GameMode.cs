
namespace Gameplay
{
    /// <summary>
    /// 游戏模式
    /// </summary>
    internal enum GameModeType : byte
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,

        /// <summary>
        /// 单人
        /// </summary>
        Single,

        /// <summary>
        /// 团队
        /// </summary>
        Team,
    }


    /// <summary>
    /// 游戏模式
    /// 1，包含基础游戏规则
    /// </summary>
    internal class GameMode
    {
        public GameModeType ModeType { get; set; }

        /// <summary>
        /// 最大玩家人数
        /// </summary>
        public int MaxPlayerCnt { get; private set; }

        public GameMode()
        {
            ModeType = GameModeType.Single;
            MaxPlayerCnt = 10;
        }
    }
}
