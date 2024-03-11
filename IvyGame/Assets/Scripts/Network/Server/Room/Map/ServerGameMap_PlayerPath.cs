using Game.AStar;
using IAEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{

    internal class ServerGameMap_PlayerPath
    {
        private ServerGameMap map;
        private byte camp;

        private ServerRect pathRect;
        private ServerRect containPathRect;
        private ServerPoint[] checkPoints = new ServerPoint[4];


        class PathPoint
        {
            public ServerPoint point;

            public PathPoint lastPoint;

            public PathPoint nextPoint;


            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                if (!(obj is PathPoint))
                    return false;

                PathPoint other = (PathPoint)obj;
                if (this.point == other.point)
                    return true;
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }

        private Dictionary<byte, Dictionary<byte, PathPoint>> pathDict = new Dictionary<byte, Dictionary<byte, PathPoint>>();
        private ServerPoint currPoint;

        public ServerGameMap_PlayerPath(byte pCamp, ServerGameMap pMap)
        {
            camp = pCamp;
            map = pMap;
            for (int i = 0; i < 4; i++)
            {
                checkPoints[i] = new ServerPoint();
            }
            containPathRect = new ServerRect();
        }

        /// <summary>
        /// 添加路径点
        /// </summary>
        public void AddPoint(byte posX, byte posY)
        {
            if (pathRect == null)
                pathRect = map.GetCampRect(camp).Copy();
            pathRect.TryUpdateX(posX);
            pathRect.TryUpdateY(posY);

            UpdateContainPathRect();

            if (!pathDict.ContainsKey(posX))
                pathDict.Add(posX, new Dictionary<byte, PathPoint>());
            if (!pathDict[posX].ContainsKey(posY))
            {
                PathPoint pathPoint = new PathPoint();
                pathPoint.point = new ServerPoint(posX, posY);

                //添加上一个点
                if (currPoint != null)
                {
                    PathPoint lastPoint = pathDict[currPoint.x][currPoint.y];
                    lastPoint.nextPoint = pathPoint;
                    pathPoint.lastPoint = lastPoint;
                }

                pathDict[posX].Add(posY, pathPoint);
                currPoint = pathPoint.point;
            }
        }

        /// <summary>
        /// 添加路径点
        /// </summary>
        public void AddPoint(ServerPoint point)
        {
            AddPoint(point.x, point.y);
        }

        /// <summary>
        /// 删除路径点
        /// </summary>
        public void RemovePoint(ServerPoint removePoint, ServerPoint currPoint)
        {
            pathRect = map.GetCampRect(camp).Copy();
            pathRect.TryUpdateX(currPoint.x);
            pathRect.TryUpdateY(currPoint.y);

            if (pathDict.ContainsKey(removePoint.x) && pathDict[removePoint.x].ContainsKey(removePoint.y))
                pathDict[removePoint.x].Remove(removePoint.y);
        }

        /// <summary>
        /// 检测点在路径内
        /// </summary>
        public bool CheckInPath(byte posX, byte posY)
        {
            return pathDict.ContainsKey(posX) && pathDict[posX].ContainsKey(posY);
        }

        /// <summary>
        /// 检测点在路径内
        /// </summary>
        public bool CheckInPath(ServerPoint point)
        {
            return CheckInPath(point.x, point.y);
        }

        //更新包围框
        private void UpdateContainPathRect()
        {
            ServerPoint leftDownPoint = pathRect.min.Copy();
            if (leftDownPoint.x <= 0)
                leftDownPoint.x = 0;
            else if (leftDownPoint.x >= map.Size.x)
                leftDownPoint.x = map.Size.x;
            else
                leftDownPoint.x -= 1;

            if (leftDownPoint.y <= 0)
                leftDownPoint.y = 0;
            else if (leftDownPoint.y >= map.Size.y)
                leftDownPoint.y = map.Size.y;
            else
                leftDownPoint.y -= 1;

            ServerPoint rightUpPoint = pathRect.max.Copy();
            if (rightUpPoint.x <= 0)
                rightUpPoint.x = 0;
            else if (rightUpPoint.x >= map.Size.x)
                rightUpPoint.x = map.Size.x;
            else
                rightUpPoint.x += 1;

            if (rightUpPoint.y <= 0)
                rightUpPoint.y = 0;
            else if (rightUpPoint.y >= map.Size.y)
                rightUpPoint.y = map.Size.y;
            else
                rightUpPoint.y += 1;

            containPathRect.min = leftDownPoint;
            containPathRect.max = rightUpPoint;

        }

        #region 区域占领

        //占领区域
        public List<ServerPoint> CaptureArea(Action<byte, byte> changeCampCallBack)
        {
            if (pathRect == null)
            {
                return null;
            }
            List<ServerPoint> capturePoints = new List<ServerPoint>();

            //先将路径变为占领区域
            foreach (byte x in pathDict.Keys)
            {
                foreach (byte y in pathDict[x].Keys)
                {
                    capturePoints.Add(new ServerPoint(x, y));
                    changeCampCallBack?.Invoke(x, y);
                    map.ChangePointCamp(x, y, camp);
                }
            }

            for (byte x = pathRect.min.x; x <= pathRect.max.x; x++)
            {
                for (byte y = pathRect.min.y; y <= pathRect.max.y; y++)
                {
                    //不在路径中并且阵营不同
                    if (!CheckInPath(x, y) && map.GetPointCamp(x, y) != camp)
                    {
                        if (CheckPointCanCapture(x, y))
                        {
                            capturePoints.Add(new ServerPoint(x, y));
                            changeCampCallBack?.Invoke(x, y);
                            map.ChangePointCamp(x, y, camp);
                        }
                    }
                }
            }

            pathRect = null;
            currPoint = null;
            return capturePoints;
        }

        //检测点是不是可以占领
        private bool CheckPointCanCapture(byte pointX, byte pointY)
        {
            byte checkCnt = 0;
            //向左检测
            for (byte leftX = pathRect.min.x; leftX <= pointX; leftX++)
            {
                if (map.GetPointCamp(leftX, pointY) == camp)
                {
                    checkCnt++;
                    checkPoints[0] = new ServerPoint(leftX, pointY);
                    break;
                }
            }
            //向右检测
            for (byte rightX = pointX; rightX <= pathRect.max.x; rightX++)
            {
                if (map.GetPointCamp(rightX, pointY) == camp)
                {
                    checkCnt++;
                    checkPoints[1] = new ServerPoint(rightX, pointY);
                    break;
                }
            }

            //向上检测
            for (byte upY = pointY; upY <= pathRect.max.y; upY++)
            {
                if (map.GetPointCamp(pointX, upY) == camp)
                {
                    checkCnt++;
                    checkPoints[2] = new ServerPoint(pointX, upY);
                    break;
                }
            }
            //向下检测
            for (byte downY = pathRect.min.y; downY <= pointY; downY++)
            {
                if (map.GetPointCamp(pointX, downY) == camp)
                {
                    checkCnt++;
                    checkPoints[3] = new ServerPoint(pointX, downY);
                    break;
                }
            }

            //四个方向没全
            if (checkCnt != 4)
            {
                return false;
            }

            //return true;

            ServerPoint pointA = new ServerPoint(pointX, pointY);
            ServerPoint pointB = containPathRect.min.Copy();
            if (pointB.Equals(pathRect.min))
            {
                pointB = containPathRect.max.Copy();
            }

            return !CheckTowPointIsConnect(pointA, pointB);
        }

        //检测俩点相连
        private bool CheckTowPointIsConnect(ServerPoint pointA, ServerPoint pointB)
        {
            PathGrid pathGrid = map.GetPathGrid(camp);
            PathNode startNode = pathGrid.GetNode(pointA.x, pointA.y);
            PathNode targetNode = pathGrid.GetNode(pointB.x, pointB.y);

            List<Vector2Int> pathList = pathGrid.FindPath(startNode, targetNode, CheckPathNodeNeedCheck);
            return pathList.IsLegal();
        }

        private bool CheckPathNodeNeedCheck(int x, int y)
        {
            return containPathRect.CheckContain((byte)x, (byte)y);
        }

        #endregion

        #region 路径点占领

        public List<ServerPoint> RemovePath(ServerPoint lastPoint, ServerPoint currPoint)
        {
            if (!CheckInPath(lastPoint) || !CheckInPath(currPoint))
            {
                return null;
            }

            PathPoint last = pathDict[lastPoint.x][lastPoint.y];
            PathPoint check = pathDict[currPoint.x][currPoint.y];

            last.nextPoint = check;

            List<PathPoint> list = new List<PathPoint>();
            CalcPoint(check, list, check);

            List<ServerPoint> pointList = new List<ServerPoint>();
            foreach (PathPoint pathPoint in list)
            {
                pointList.Add(pathPoint.point);
                pathDict[pathPoint.point.x].Remove(pathPoint.point.y);
            }

            //更新区域
            pathRect = map.GetCampRect(camp).Copy();
            pathRect.TryUpdateX(check.point.x);
            pathRect.TryUpdateY(check.point.y);

            //清理
            check.nextPoint = null;
            this.currPoint = check.point;

            return pointList;
        }

        private void CalcPoint(PathPoint point, List<PathPoint> pointList, PathPoint checkPoint)
        {
            if (point.nextPoint == null)
            {
                return;
            }

            if (point.nextPoint == checkPoint)
            {
                pointList.Add(point);
                return;
            }

            if (point != checkPoint)
            {
                pointList.Add(point);
            }
            CalcPoint(point.nextPoint, pointList, checkPoint);    
        }

        #endregion

        //清空区域
        public void Clear()
        {
            pathDict.Clear();
            pathRect?.Clear();
        }


    }
}
