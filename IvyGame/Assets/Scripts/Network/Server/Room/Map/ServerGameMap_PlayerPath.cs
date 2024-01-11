using Game.AStar;
using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{

    internal class ServerGameMap_PlayerPath
    {
        private ServerGameMap map;
        private byte camp;

        private Dictionary<byte, HashSet<byte>> pointDict = new Dictionary<byte, HashSet<byte>>();
        private ServerRect pathRect;
        private ServerPoint[] checkPoints = new ServerPoint[4];

        public ServerGameMap_PlayerPath(byte pCamp, ServerGameMap pMap)
        {
            camp = pCamp;
            map = pMap;
            for (int i = 0; i < 4; i++)
            {
                checkPoints[i] = new ServerPoint();
            }
        }

        //占领区域
        public List<ServerPoint> CaptureArea()
        {
            List<ServerPoint> capturePoints = new List<ServerPoint>();

            //先将路径变为占领区域
            foreach (byte x in pointDict.Keys)
            {
                foreach (byte y in pointDict[x])
                {
                    capturePoints.Add(new ServerPoint(x, y));
                    map.ChangePointCamp(x, y, camp);
                }
            }

            for (byte x = pathRect.min.x; x <= pathRect.max.x; x++)
            {
                for (byte y = pathRect.min.y; y <= pathRect.max.y; y++)
                {
                    //不在路径中并且阵营不同
                    if (!CheckInPath(x,y) && map.GetPointCamp(x,y) != camp)
                    {
                        if(CheckPointCanCapture(x,y))
                        {
                            capturePoints.Add(new ServerPoint(x, y));
                            map.ChangePointCamp(x, y, camp);
                        }
                    }
                }
            }

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
                    checkCnt ++;
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

            return CheckCheckPointsIsConnect();
        }

        private bool CheckCheckPointsIsConnect()
        {
            if (!CheckTowPointIsConnect(checkPoints[0], checkPoints[1]))
            {
                return false;
            }

            if (!CheckTowPointIsConnect(checkPoints[1], checkPoints[2]))
            {
                return false;
            }

            if (!CheckTowPointIsConnect(checkPoints[2], checkPoints[3]))
            {
                return false;
            }

            if (!CheckTowPointIsConnect(checkPoints[3], checkPoints[0]))
            {
                return false;
            }

            return true;
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
            return pathRect.CheckContain((byte)x, (byte)y);
        }

        //清空区域
        public void Clear()
        {
            pointDict.Clear();
            pathRect.Clear();
        }

        /// <summary>
        /// 添加路径点
        /// </summary>
        public void AddPoint(byte posX, byte posY)
        {
            pathRect = map.GetCampRect(camp).Copy();
            pathRect.TryUpdateX(posX);
            pathRect.TryUpdateY(posY);

            if (!pointDict.ContainsKey(posX))
                pointDict.Add(posX, new HashSet<byte>());
            if (!pointDict[posX].Contains(posY))
                pointDict[posX].Add(posY);
        }

        /// <summary>
        /// 添加路径点
        /// </summary>
        public void AddPoint(ServerPoint point)
        {
            AddPoint(point.x, point.y);
        }

        /// <summary>
        /// 检测点在路径内
        /// </summary>
        public bool CheckInPath(byte posX, byte posY)
        {
            return pointDict.ContainsKey(posX) && pointDict[posX].Contains(posY);
        }

        /// <summary>
        /// 检测点在路径内
        /// </summary>
        public bool CheckInPath(ServerPoint point)
        {
            return CheckInPath(point.x, point.y);
        }
    }
}
