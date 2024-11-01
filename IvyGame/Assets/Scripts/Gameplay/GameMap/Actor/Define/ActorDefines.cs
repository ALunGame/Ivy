namespace Gameplay.GameMap.Actor
{
    public enum ActorType
    {
        /// <summary>
        /// 本地玩家
        /// </summary>
        LocalPlayer = 1,

        /// <summary>
        /// 远端玩家
        /// </summary>
        RemotePlayer,

        /// <summary>
        /// 敌人
        /// </summary>
        Enemy,

        /// <summary>
        /// 地图格子
        /// </summary>
        MapGrid,
    }

    public static class ActorDefine
    {
        public const int LocalPlayerActorId = 101;
        public const int RemotePlayerActorId = 102;
        public const int EnemyActorId = 103;
    }
}
