using Game.AStar;
using IAEngine;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerPoint
    {
        public byte x;
        public byte y;

        /// <summary>
        /// 合法的坐标，为了实现默认为空清空
        /// </summary>
        public bool isLegal = true;

        public ServerPoint()
        {
            x = 0;
            y = 0;
        }

        public ServerPoint(bool pIsLegal)
        {
            isLegal = pIsLegal;
        }

        public ServerPoint(byte pX, byte pY)
        {
            x = pX;
            y = pY;
        }

        public ServerPoint(int pX, int pY)
        {
            x = (byte)pX;
            y = (byte)pY;
        }

        public bool Equals(byte pX, byte pY)
        {
            return x == pX && y == pY;
        }

        public override bool Equals(object obj)
        {
            if (obj is ServerPoint)
            {
                ServerPoint p = (ServerPoint)obj;
                return Equals(p.x, p.y);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"x;{x} y:{y}";
        }

    }

    internal class ServerRect
    {
        public ServerPoint min = new ServerPoint(false);
        public ServerPoint max = new ServerPoint(false);

        public void TryUpdateX(byte x)
        {
            if (!min.isLegal)
            {
                min.x = x;
                max.x = x;
                min.isLegal = true;
                max.isLegal = true;
                return;
            }

            if (x < min.x)
            {
                min.x = x;
            }
            else if (x > max.x)
            {
                max.x = x;
            }
        }

        public void TryUpdateY(byte y)
        {
            if (!min.isLegal)
            {
                min.y = y;
                max.y = y;
                min.isLegal = true;
                max.isLegal = true;
                return;
            }

            if (y < min.y)
            {
                min.y = y;
            }
            else if (y > max.y)
            {
                max.y = y;
            }
        }

        public ServerRect Copy()
        {
            ServerRect rect = new ServerRect();
            rect.min = min;
            rect.max = max;
            return rect;
        }

        public bool CheckContain(byte x, byte y)
        {
            if (x < min.x || x > max.x)
            {
                return false;
            }
            if (y < min.y || y > max.y)
            {
                return false;
            }
            return true;
        }

        public void Clear()
        {
            min.isLegal = true;
            max.isLegal = true ;
        }

    }

    #region Delegate

    /// <summary>
    /// 点阵营改变委托
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="camp"></param>
    internal delegate void OnPointCampChange(byte posX, byte posY, byte camp);

    /// <summary>
    /// 击杀玩家委托
    /// </summary>
    /// <param name="diePlayerUid">死亡玩家Uid</param>
    /// <param name="killPlayerUid">击杀玩家Uid</param>
    internal delegate void OnKillPlayer(int diePlayerUid, int killPlayerUid);

    /// <summary>
    /// 添加玩家路径委托
    /// </summary>
    /// <param name="playerUid"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    internal delegate void OnAddPlayerPathPoint(int playerUid, byte posX, byte posY);

    /// <summary>
    /// 移除玩家路径委托
    /// </summary>
    /// <param name="playerUid"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    internal delegate void OnRemovePlayerPath(int playerUid); 

    #endregion

    internal class ServerGameMap
    {
        private ServerPoint size;

        private byte[,] area;

        private Dictionary<int, ServerGameMap_PlayerPath> playerPathDict = new Dictionary<int, ServerGameMap_PlayerPath>();

        //阵营区域
        private Dictionary<byte, ServerRect> campRectDict = new Dictionary<byte, ServerRect>();
        //阵营寻路区域
        private Dictionary<byte, PathGrid> campPathGridDict = new Dictionary<byte, PathGrid>();

        #region 事件

        public event OnPointCampChange Evt_PointCampChange;
        public event OnKillPlayer Evt_KillPlayer;
        public event OnAddPlayerPathPoint Evt_AddPlayerPathPoint;
        public event OnRemovePlayerPath Evt_RemovePlayerPath;

        #endregion

        //创建
        public void Create(byte width, byte height)
        {
            size = new ServerPoint(width, height);

            area = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    area[x, y] = 0;
                }
            }
        }

        //添加玩家
        public void AddPlayer(ServerPlayer player)
        {
            if (playerPathDict.ContainsKey(player.Uid))
                playerPathDict.Remove(player.Uid);

            ServerGameMap_PlayerPath playerPath = new ServerGameMap_PlayerPath(player.Camp,this);
            playerPathDict.Add(player.Uid, playerPath);

            if (!campPathGridDict.ContainsKey(player.Camp))
            {
                PathGrid pathGrid = new PathGrid();
                pathGrid.FinderType = FinderType.Four;
                campPathGridDict.Add(player.Camp, pathGrid);
                campPathGridDict[player.Camp].Create(size.x, size.y);
            }

            ChangePointCamp(player.Pos.x, player.Pos.y, player.Camp);
        }

        //检测点合法
        public bool CheckPointIsLegal(byte posX, byte posY)
        {
            if (posX < 0 || posX > size.x - 1)
            {
                return false;
            }

            if (posY < 0 || posY > size.y - 1)
            {
                return false;
            }

            return true;
        }

        //获得点属于的阵营
        public byte GetPointCamp(byte posX, byte posY)
        {
            if (!CheckPointIsLegal(posX, posY))
            {
                return 0;
            }

            return area[posX, posY];
        }

        //获得在路径中的玩家Uid
        public int GetPlayerUidInPathPoint(byte posX, byte posY)
        {
            foreach (int playerUid in playerPathDict.Keys)
            {
                if (playerPathDict[playerUid].CheckInPath(posX,posY))
                {
                    return playerUid;
                }
            }
            return -1;
        }

        //改变点的阵营
        public void ChangePointCamp(byte posX, byte posY, byte camp)
        {
            if (!CheckPointIsLegal(posX,posY))
            {
                return;
            }

            if (area[posX,posY] == camp)
            {
                return;
            }

            byte oldCamp = area[posX,posY];

            //区域
            if (!campRectDict.ContainsKey(camp))
                campRectDict.Add(camp, new ServerRect());
            campRectDict[camp].TryUpdateX(posX);
            campRectDict[camp].TryUpdateY(posY);

            //寻路
            if (campPathGridDict.ContainsKey(camp))
                campPathGridDict[camp].SetObs(posX, posY, false);
            if (campPathGridDict.ContainsKey(oldCamp))
                campPathGridDict[oldCamp].SetObs(posX, posY, true);

            area[posX, posY] = camp;

            Evt_PointCampChange?.Invoke(posX, posY, camp);
        }

        //获得阵营区域水平轴区间
        public ServerRect GetCampRect(byte camp)
        {
            return campRectDict[camp];
        }

        //获得阵营寻路区域
        public PathGrid GetPathGrid(byte camp)
        {
            return campPathGridDict[camp];
        }

        #region 事件触发

        public void OnPlayerMove(ServerPlayer player)
        {
            List<int> diePlayerUids = new List<int>();
            ServerGameMap_PlayerPath playerPath = playerPathDict[player.Uid];
            List<ServerPoint> areaPoints = null;

            //判断有人
            int currPlayerUid = GetPlayerUidInPathPoint(player.Pos.x, player.Pos.y);
            if (currPlayerUid != -1 && currPlayerUid != player.Uid)
            {
                diePlayerUids.Add(currPlayerUid);
            }

            //1，判断是不是自己的领地
            byte currCamp = GetPointCamp(player.Pos.x, player.Pos.y);
            //2，是自己的领地，区域占领
            if (currCamp == player.Camp)
            {
                areaPoints = playerPath.CaptureArea();
                //自己路径清理
                playerPathDict[player.Uid].Clear();
                Evt_RemovePlayerPath?.Invoke(player.Uid);
            }
            else
            {
                //连接了，区域占领
                if (currPlayerUid == player.Uid)
                {
                    areaPoints = playerPath.CaptureArea();
                    //自己路径清理
                    playerPathDict[player.Uid].Clear();
                    Evt_RemovePlayerPath?.Invoke(player.Uid);
                }
                else
                {
                    playerPath.AddPoint(player.Pos.x, player.Pos.y);

                    Evt_AddPlayerPathPoint?.Invoke(player.Uid, player.Pos.x, player.Pos.y);
                }
            }

            //收集死亡玩家
            if (areaPoints.IsLegal())
            {
                for (int i = 0; i < areaPoints.Count; i++)
                {
                    ServerPoint point = areaPoints[i];
                    int tPlayerUid = GetPlayerUidInPathPoint(point.x, point.y);
                    if (tPlayerUid != -1 && currPlayerUid != player.Uid)
                    {
                        if (!diePlayerUids.Contains(tPlayerUid))
                        {
                            diePlayerUids.Add(tPlayerUid);
                        }
                    }
                }
            }

            //发送击杀事件
            if (diePlayerUids.IsLegal())
            {
                for (int i = 0; i < diePlayerUids.Count; i++)
                {
                    int tPlayerUid = diePlayerUids[i];
                    playerPathDict[tPlayerUid].Clear();

                    Evt_RemovePlayerPath?.Invoke(tPlayerUid);
                    Evt_KillPlayer?.Invoke(tPlayerUid, player.Uid);
                }
            }
        }

        #endregion

    }
}
