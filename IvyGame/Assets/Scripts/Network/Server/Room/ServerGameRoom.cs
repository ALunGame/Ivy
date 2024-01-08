using System.Collections.Generic;

namespace Game.Network.Server
{
    /// <summary>
    /// 游戏房间
    /// </summary>
    internal class ServerGameRoom
    {
        /// <summary>
        /// 玩家
        /// </summary>
        public List<ServerPlayer> Players { get; private set; }

        /// <summary>
        /// 最大玩家数量
        /// </summary>
        public int MaxPlayerCnt { get; private set; }

        /// <summary>
        /// 游戏区域
        /// </summary>
        public ServerGameArea Area { get; private set; }

        /// <summary>
        /// 获得玩家
        /// </summary>
        /// <param name="playerUid">玩家Uid</param>
        /// <returns></returns>
        public ServerPlayer GetPlayer(int playerUid)
        {
            foreach (var player in Players)
            {
                if (player.Uid == playerUid) 
                    return player;
            }
            return null;
        }

        /// <summary>
        /// 获得玩家所处位置的区域阵营
        /// </summary>
        /// <param name="playerUid"></param>
        /// <returns></returns>
        public int GetPlayerPosAreaCamp(int playerUid)
        {
            ServerPlayer player = GetPlayer(playerUid);
            return GetCampInAreaPos(player.PosX, player.PosY);
        }

        /// <summary>
        /// 获得位置所属的阵营
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public int GetCampInAreaPos(int posX, int posY)
        {
            return Area.GetAreaCamp(posX, posY);
        }
    }
}
