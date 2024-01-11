using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AStar
{
    public enum FinderType
    {
        Eight,
        Four,
    }
    
    public class PathGridConnectInfo
    {
        public int inGridId;
        public Vector2Int inPoint;

        public int outGridId;
        public Vector2Int outPoint;

        public PathGridConnectInfo(int inGridId, Vector2Int inPoint, int outGridId, Vector2Int outPoint)
        {
            this.inGridId = inGridId;
            this.inPoint = inPoint;
            this.outGridId = outGridId;
            this.outPoint = outPoint;
        }

        public PathGridConnectInfo()
        {

        }
    }

    /// <summary>
    /// 寻路网格
    /// </summary>
    public class PathGrid
    {
        public FinderType FinderType {get; set;}

        /// <summary>
        /// 网格大小
        /// </summary>
        public Vector2Int Size { get; private set; }
        
        /// <summary>
        /// 连接信息
        /// </summary>
        public List<PathGridConnectInfo> ConnectInfos = new List<PathGridConnectInfo>();
        
        /// <summary>
        /// 格子信息
        /// </summary>
        private PathNode[,] grid;

        public PathGrid()
        {
            FinderType = FinderType.Four;
        }

        public void Create(int width, int height)
        {
            Size = new Vector2Int(width, height);

            grid = new PathNode[Size.x + 1, Size.y + 1];

            for (int x = 0; x <= Size.x; x++)
            {
                for (int y = 0; y <= Size.y; y++)
                {
                    grid[x, y] = new PathNode(x, y);
                }
            }
        }

        //获得网格点
        public PathNode GetNode(int x,int y)
        {
            return grid[x, y];
        }
       
        public bool CheckPointIsLegal(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x > Size.x || y > Size.y)
                return false;
            return true;
        }
        
        public bool CheckIsObs(Vector2Int pGridPos)
        {
            return CheckIsObs(pGridPos.x, pGridPos.y);
        }

        public bool CheckIsObs(int x, int y)
        {
            if (!CheckPointIsLegal(x, y))
                return true;
            return grid[x, y].IsObs;
        }

        public void SetObs(Vector2Int pGridPos, bool pIsObs)
        {
            SetObs(pGridPos.x, pGridPos.y, pIsObs);
        }

        public void SetObs(int x, int y, bool pIsObs)
        {
            if (!CheckPointIsLegal(x, y))
                return;
            if (grid[x, y] == null)
            {
                Debug.LogError($"SetObs>>>{x}--{y}");
            }
            grid[x, y].SetObs(pIsObs);
        }

        public List<Vector2Int> FindPath(PathNode start, PathNode target, Func<int, int, bool> checkNodeNeedFunc = null)
        {
            Pathfinder pathfinder = new Pathfinder(this, start, target, checkNodeNeedFunc);
            return pathfinder.FindPath();
        }
    }
}