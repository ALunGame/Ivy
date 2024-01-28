using System.Collections.Generic;

namespace Game.Network.Server
{
    /// <summary>
    /// 游戏房间
    /// </summary>
    internal class ServerGameRoom
    {
        private List<ServerPlayer> players;
        private int maxPlayerCnt;
        private ServerGameMap map;

        public ServerGameMap Map {  get { return map; } }

        public void Create(byte mapWidth, byte mapHeight, int maxPlayerCnt)
        {
            map = new ServerGameMap();
            map.Create(mapWidth, mapHeight, this);

            players = new List<ServerPlayer>();
            this.maxPlayerCnt = maxPlayerCnt;
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(ServerPlayer player)
        {
            players.Add(player);
        }

        /// <summary>
        /// 获得玩家
        /// </summary>
        /// <param name="playerUid">玩家Uid</param>
        /// <returns></returns>
        public ServerPlayer GetPlayer(int playerUid)
        {
            foreach (var player in players)
            {
                if (player.Uid == playerUid) 
                    return player;
            }
            return null;
        }

        /// <summary>
        /// 获取玩家
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ServerPlayer GetPlayer(byte x, byte y)
        {
            foreach (var player in players)
            {
                if (player.GridPos.x == x && player.GridPos.y == y)
                    return player;
            }
            return null;
        }

        /// <summary>
        /// 获得所有玩家
        /// </summary>
        /// <returns></returns>
        public List<ServerPlayer> GetPlayers()
        {
            return players;
        }

        /// <summary>
        /// 获得玩家所处位置的区域阵营
        /// </summary>
        /// <param name="playerUid"></param>
        /// <returns></returns>
        public int GetPlayerPosAreaCamp(int playerUid)
        {
            ServerPlayer player = GetPlayer(playerUid);
            return GetCamp(player.GridPos.x, player.GridPos.y);
        }

        /// <summary>
        /// 获得位置所属的阵营
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public int GetCamp(byte posX, byte posY)
        {
            return map.GetPointCamp(posX, posY);
        }

        public bool TestPlayerMove(int playerUid, byte posX, byte posY)
        {
            ServerPlayer player = GetPlayer(playerUid);
            if (player == null) 
                return false;

            if (player.GridPos.Equals(posX,posY))
                return false;

            if (!map.CheckPointIsLegal(posX,posY))
                return false;

            player.SetGridPos(posX, posY);
            map.OnPlayerMove(player);
            return true;
        }
    }
}
