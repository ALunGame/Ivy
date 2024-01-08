using IAEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    internal class ServerGameAreaGrid
    {
        /// <summary>
        /// 所属阵营
        /// </summary>
        public int Camp {  get; private set; }

        /// <summary>
        /// 在格子里的玩家Uid
        /// </summary>
        public int InGridPlayerUid { get; private set; }

        public ServerGameAreaGrid()
        {
            Camp = 0;
            InGridPlayerUid = 0;
        }
    }

    /// <summary>
    /// 游戏区域
    /// </summary>
    internal class ServerGameArea
    {
        private ServerGameRoom Room;

        /// <summary>
        /// 区域大小
        /// </summary>
        public Vector2Int MaxAreaSize { get; private set; }

        /// <summary>
        /// 区域
        /// </summary>
        public int[,] Area { get; private set; }

        /// <summary>
        /// 玩家占领的格子 playeruid-x-y
        /// </summary>
        private Dictionary<int, Dictionary<int,List<int>>> PlayerCaptureGrid = new Dictionary<int, Dictionary<int, List<int>>>();
        /// <summary>
        /// 玩家占领的格子反向表，x-y-playeruid
        /// </summary>
        private Dictionary<int, Dictionary<int, int>> CapturePlayerGrid = new Dictionary<int, Dictionary<int, int>>();

        public ServerGameArea(ServerGameRoom room, int areaMaxX, int areaMaxY)
        {
            Room = room;

            Area = new int[areaMaxX,areaMaxY];
            MaxAreaSize = new Vector2Int(areaMaxX, areaMaxY);

            for (int x = 0; x < areaMaxX; x++)
            {
                for (int y = 0; y < areaMaxY; y++)
                {
                    Area[x,y] = 0;
                }
            }
        }

        /// <summary>
        /// 获得玩家占领的格子
        /// </summary>
        /// <param name="playerUid"></param>
        /// <returns></returns>
        private Dictionary<int, List<int>> GetPlayerCaptureGrid(int playerUid)
        {
            if (!PlayerCaptureGrid.ContainsKey(playerUid))
                PlayerCaptureGrid.Add(playerUid, new Dictionary<int, List<int>>());
            return PlayerCaptureGrid[playerUid];
        }

        /// <summary>
        /// 通过占领位置获得玩家Id
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        private int GetPlayerUidInCaptureGrid(int posX, int posY)
        {
            if (!CapturePlayerGrid.ContainsKey(posX))
                return -1;
            if (!CapturePlayerGrid[posX].ContainsKey(posY))
                return -1;
            return CapturePlayerGrid[posX][posY];
        }

        /// <summary>
        /// 计算占领的区域，以列为键，行最大最小为键
        /// </summary>
        /// <param name="captureGrids"></param>
        /// <returns></returns>
        private Dictionary<int,Vector2Int> CalcCaptureGridArea(Dictionary<int, List<int>> captureGrids)
        {
            Dictionary<int, Vector2Int> resArea = new Dictionary<int, Vector2Int>();
            foreach (int x in captureGrids.Keys)
            {
                List<int> ylist = captureGrids[x];
                if (ylist.IsLegal())
                {
                    ylist.Sort();
                    resArea.Add(x, new Vector2Int(ylist[0], ylist[ylist.Count - 1]));
                }
            }
            return resArea;
        }

        /// <summary>
        /// 删除占领记录
        /// </summary>
        /// <param name="playerUid"></param>
        private void RemovePlayerCaptureRecord(int playerUid)
        {
            PlayerCaptureGrid.Remove(playerUid);
            List<Vector2Int> removePos = new List<Vector2Int>();

            foreach (int x in CapturePlayerGrid.Keys)
            {
                foreach (int y in CapturePlayerGrid[x].Keys)
                {
                    if (CapturePlayerGrid[x][y] == playerUid)
                    {
                        removePos.Add(new Vector2Int(x, y));
                    }
                }
            }

            for (int i = 0; i < removePos.Count; i++)
            {
                Vector2Int tPos = removePos[i];
                CapturePlayerGrid[tPos.x].Remove(tPos.y);
            }
        }

        /// <summary>
        /// 添加占领记录
        /// </summary>
        /// <param name="playerUid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        private void AddPlayerCaptureRecord(int playerUid, int posX, int posY)
        {
            if (!PlayerCaptureGrid.ContainsKey(playerUid))
                PlayerCaptureGrid.Add(playerUid, new Dictionary<int, List<int>>());

            if (!PlayerCaptureGrid[playerUid].ContainsKey(posX))
                PlayerCaptureGrid[playerUid][posX].Add(posY);

            if (!CapturePlayerGrid.ContainsKey(posX))
                CapturePlayerGrid.Add(posX, new Dictionary<int, int>());

            if (!CapturePlayerGrid[posX].ContainsKey(posY))
                CapturePlayerGrid[posX][posY] = 0;
            CapturePlayerGrid[posX][posY] = playerUid;

        }

        /// <summary>
        /// 占领区域
        /// </summary>
        /// <param name="captureArea"></param>
        /// <returns>返回被占领的玩家</returns>
        private List<int> CaptureGridArea(int playerUid, int playerCamp, Dictionary<int, Vector2Int> captureArea)
        {
            List<int> removePlayerUids = new List<int>();
            foreach (int x in captureArea.Keys)
            {
                int minY = captureArea[x].x;
                int maxY = captureArea[x].y;
                for (int y = minY; y <= maxY; y++)
                {
                    int oldPlayerUid = GetPlayerUidInCaptureGrid(x, y);
                    if (oldPlayerUid != -1)
                    {
                        if (!removePlayerUids.Contains(oldPlayerUid))
                            removePlayerUids.Add(oldPlayerUid);
                    }

                    //改变阵营
                    SetAreaCamp(x, y, playerCamp);
                    //删除记录
                    RemovePlayerCaptureRecord(playerUid);
                }
            }
            return removePlayerUids;
        }

        /// <summary>
        /// 判断点合法
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public bool CheckPointIsLegal(int posX, int posY)
        {
            if (posX < 0 || posX > MaxAreaSize.x - 1)
            {
                return false;
            }

            if (posY < 0 || posY > MaxAreaSize.y - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得区域所属阵营
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public int GetAreaCamp(int posX, int posY)
        {
            if (!CheckPointIsLegal(posX, posY))
            {
                return 0;
            }

            return Area[posX, posY];
        }

        /// <summary>
        /// 设置区域格子阵营
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="camp"></param>
        public void SetAreaCamp(int posX, int posY, int camp)
        {
            if (!CheckPointIsLegal(posX, posY))
            {
                return;
            }

            Area[posX, posY] = camp;
        }

        /// <summary>
        /// 玩家移动
        /// </summary>
        /// <param name="playerUid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns>返回移除的玩家</returns>
        public List<int> OnPlayerMove(int playerUid, int posX, int posY)
        {
            ServerPlayer serverPlayer = Room.GetPlayer(playerUid);
            List<int> removePlayerUids = new List<int>();

            //1，判断是不是自己的领地
            int currCamp = GetAreaCamp(posX, posY);
            //2，是自己的领地，检测合并区域操作
            if (currCamp == serverPlayer.Camp)
            {
                Dictionary<int, List<int>> captureGrids = GetPlayerCaptureGrid(playerUid);
                //3，自己没有出领地，判断他人
                if (captureGrids.Count == 0)
                {
                    //4，判断有人
                    int oldPlayerUid = GetPlayerUidInCaptureGrid(posX, posY);
                    if (oldPlayerUid != -1)
                    {
                        removePlayerUids.Add(oldPlayerUid);
                    }
                    return;
                }

                //5，占领区域
                Dictionary<int, Vector2Int> captureArea = CalcCaptureGridArea(captureGrids);
                removePlayerUids = CaptureGridArea(playerUid, serverPlayer.Camp, captureArea);
            }
            else
            {
                int oldPlayerUid = GetPlayerUidInCaptureGrid(posX, posY);
                //6，判断当前格子是自己，占领区域
                if (oldPlayerUid == playerUid)
                {
                    Dictionary<int, List<int>> captureGrids = GetPlayerCaptureGrid(playerUid);
                    Dictionary<int, Vector2Int> captureArea = CalcCaptureGridArea(captureGrids);
                    removePlayerUids = CaptureGridArea(playerUid, serverPlayer.Camp, captureArea);
                }
                //7，不是自己，就添加记录
                else
                {
                    AddPlayerCaptureRecord(playerUid,posX,posY);
                }
            }

            //删除移除玩家的记录
            for (int i = 0; i < removePlayerUids.Count; i++)
            {
                RemovePlayerCaptureRecord(removePlayerUids[i]);
            }

            return removePlayerUids;
        }
    }
}
