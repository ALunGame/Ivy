using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Game.AStar
{
    /// <summary>
    /// 一个寻路实例
    /// 0：目前优化方式
    /// 1：open表用二叉堆实现
    /// 2：close用has表实现
    /// 3：缓存H值不要重复计算
    /// 4：F值计算动态加权
    /// </summary>
    public class Pathfinder
    {
        private PathGrid pathGrid;
        private PathNode startPosition;
        private PathNode endPosition;
        
        private Dictionary<int, Dictionary<int, PathCost>> pathCost = new Dictionary<int, Dictionary<int, PathCost>>();

        //路径点是否需要检测
        private Func<int, int, bool> pathNodeNeedFunc;
        
        public Pathfinder(PathGrid grid, PathNode start, PathNode target, Func<int, int, bool> checkNodeNeedFunc = null)
        {
            pathGrid = grid;
            startPosition = start;
            endPosition = target;
            pathCost = new Dictionary<int, Dictionary<int, PathCost>>();
            pathNodeNeedFunc = checkNodeNeedFunc;
        }
        
        public List<Vector2Int> FindPath()
        {
            if(startPosition == null || endPosition == null)
                return new List<Vector2Int>();
            
            List<Vector2Int> pathList = FindPathActual(startPosition, endPosition);
            
            return pathList;
        }
        
        private List<Vector2Int> FindPathActual(PathNode start, PathNode target)
        {
            List<Vector2Int> foundPath = new List<Vector2Int>();

            //喜闻乐见的俩个表
            Heap<PathCost> openSet = new Heap<PathCost>(pathGrid.Size.x * pathGrid.Size.y);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();

            //将起始点放入
            PathCost startCost = AddPathCost(start);
            openSet.Add(startCost);
            startCost.gCost = 0;

            while (openSet.Count() > 0)
            {
                PathCost currentNodeCost = openSet.RemoveFirst();
                PathNode currentNode = GetNode(currentNodeCost.X, currentNodeCost.Y);
                //放入closedSet
                closedSet.Add(currentNode);

                //当前点就是目标点
                if (currentNode.Equals(target))
                {
                    //找到路径
                    foundPath = RetracePath(start, currentNode, currentNodeCost);
                    break;
                }

                //查找当前检测点周围点的移动花费
                foreach (PathNode neighbour in GetNeighbours(currentNode))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        PathCost neiCost = AddPathCost(neighbour);
                        //移动到邻居的成本
                        float newMovementCostToNeighbour = currentNodeCost.gCost + CalG(currentNode, neighbour);
                        //新的成本小于旧的
                        if (newMovementCostToNeighbour < neiCost.gCost || !openSet.Contains(neiCost))
                        {
                            //成本赋值
                            neiCost.gCost = newMovementCostToNeighbour;
                            if (neiCost.hCost != 0)
                                neiCost.hCost = CalH(neighbour);
                            neiCost.fCost = CalF(neiCost);
                            //添加父节点
                            neiCost.parentNode = currentNode;

                            //添加到openSet
                            if (!openSet.Contains(neiCost))
                            {
                                openSet.Add(neiCost);
                            }
                        }
                    }
                }
            }

            return foundPath;
        }

        private PathCost AddPathCost(PathNode node)
        {
            if (!pathCost.ContainsKey(node.X))
                pathCost.Add(node.X, new Dictionary<int, PathCost>());
            if (!pathCost[node.X].ContainsKey(node.Y))
                pathCost[node.X].Add(node.Y, new PathCost(node.X, node.Y));
            return pathCost[node.X][node.Y];
        }

        //获得路径
        private List<Vector2Int> RetracePath(PathNode startNode, PathNode endNode, PathCost endCost)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode currentNode = endNode;
            PathCost currentCost = endCost;

            while (!currentNode.Equals(startNode))
            {
                path.Add(currentNode.ToVector());
                currentNode = currentCost.parentNode;
                currentCost = AddPathCost(currentNode);
                if (currentNode == null)
                {
                    UnityEngine.Debug.LogError($"寻路出错》》》》{startNode} {endNode}");
                    break;
                }
            }

            path.Reverse();
            return path;
        }

        private List<PathNode> GetNeighbours(PathNode node)
        {
            if (pathGrid.FinderType == FinderType.Eight)
            {
                return GetNeighbours_Eight(node);
            }
            else
            {
                return GetNeighbours_Four(node);
            }
        }
        
        /// <summary>
        /// 八方向
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<PathNode> GetNeighbours_Eight(PathNode node)
        {
            List<PathNode> retList = new List<PathNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    CheckNeighbourNode(node.X + x, node.Y + y, retList);
                }
            }
            return retList;
        }

        /// <summary>
        /// 四方向
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<PathNode> GetNeighbours_Four(PathNode node)
        {
            List<PathNode> retList = new List<PathNode>();

            CheckNeighbourNode(node.X, node.Y + 1, retList);
            CheckNeighbourNode(node.X, node.Y - 1, retList);
            CheckNeighbourNode(node.X - 1, node.Y, retList);
            CheckNeighbourNode(node.X + 1, node.Y, retList);
            
            return retList;
        }

        private void CheckNeighbourNode(int x, int y, List<PathNode> nodeList)
        {
            //外部检测
            if (pathNodeNeedFunc != null)
            {
                if (!pathNodeNeedFunc.Invoke(x,y))
                {
                    return;
                }
            }

            PathNode checkNode = pathGrid.GetNode(x,y);
            
            if (checkNode != null && !checkNode.IsObs)
            {
                nodeList.Add(checkNode);
            }
        }
        

        private PathNode GetNode(int x, int y)
        {
            PathNode n = null;
            lock (pathGrid)
            {
                n = pathGrid.GetNode(x, y);
            }
            return n;
        }

        /// <summary>
        /// 当前目标移动到终点的花费
        /// </summary>
        private float CalH(PathNode pos)
        {
            int cntX = Mathf.Abs(pos.X - endPosition.X);
            int cntY = Mathf.Abs(pos.Y - endPosition.Y);
            if (cntX > cntY)
                return 14 * cntY + 10 * (cntX - cntY);
            else
                return 14 * cntX + 10 * (cntY - cntX);
        }

        /// <summary>
        /// 俩点移动花费
        /// </summary>
        private float CalG(PathNode pos, PathNode targetpos)
        {
            int xMove = targetpos.X - pos.X;
            int yMove = targetpos.Y - pos.Y;
            //对角线
            if (Mathf.Abs(xMove) + Mathf.Abs(yMove) == 2)
            {
                return 14;
            }
            else
            {
                return 10;
            }
        }

        /// <summary>
        /// 节点总花费
        /// </summary>
        private float CalF(PathCost pathCost)
        {
            int weight;
            if (pathCost.hCost > 70)
                weight = 5;
            else if (pathCost.hCost > 50)
                weight = 4;
            else if (pathCost.hCost > 30)
                weight = 3;
            else if (pathCost.hCost > 10)
                weight = 2;
            else
                weight = 1;
            return pathCost.gCost + weight * pathCost.hCost;
        }
    }
}