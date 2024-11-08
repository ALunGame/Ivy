using Game.AStar;
using IAEngine;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    internal class ServerMapGamerPathData : BaseServerGameData
    {
        /// <summary>
        /// 玩家Uid
        /// </summary>
        public string GamerUid { get; private set; }

        /// <summary>
        /// 阵营
        /// </summary>
        public int Camp { get; private set; }

        private ServerMapData mapData;
        private RectInt pathRect;
        private RectInt containPathRect;
        private Vector2Int[] checkPoints = new Vector2Int[4];

        class PathPoint
        {
            public Vector2Int point;

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

        private Dictionary<int, Dictionary<int, PathPoint>> pathDict = new Dictionary<int, Dictionary<int, PathPoint>>();
        private Vector2Int currPoint;

        public ServerMapGamerPathData(ServerMapData pMapData, string pGamerUid, int pCamp)
        {
            mapData = pMapData;
            GamerUid = pGamerUid;
            Camp = pCamp;

            for (int i = 0; i < 4; i++)
            {
                checkPoints[i] = new Vector2Int();
            }
            containPathRect = new RectInt(0,0,0,0);
            pathRect = mapData.GetCampRect(Camp);
            currPoint = new Vector2Int(-1,-1);

            NetServerLocate.Net.OnDrawGizmosFunc += ()=>{
                //IAToolkit.GizmosHelper.DrawRect(pathRect, Color.black);
                Gizmos.DrawCube(pathRect.center, new Vector3(pathRect.size.x, 10, pathRect.size.y));
            };
        }

        public override void OnClear()
        {
            pathDict.Clear();
            currPoint = new Vector2Int(-1, -1);

            //消息
            if (NetServerLocate.Net.NotNull())
            {
                GamerPathChangeS2c msg = new GamerPathChangeS2c();
                msg.gamerUid = GamerUid;
                msg.Operate = 3;
                msg.Pos = new NetVector2();
                NetServerLocate.Net?.Broadcast((ushort)RoomMsgDefine.GamerPathChangeS2c, msg);
            }
        }

        #region 路径点

        /// <summary>
        /// 添加路径点
        /// </summary>
        /// <param name="pPosX"></param>
        /// <param name="pPosY"></param>
        public void AddPoint(Vector2Int pPoint)
        {
            if (pathDict.ContainsKey(pPoint.x) && pathDict[pPoint.x].ContainsKey(pPoint.y))
                return;

            //pathRect = mapData.GetCampRect(Camp);
            Debug.LogError($"AddPoint更新区域00--->{pPoint}:{pathRect}");
            RectEx.UpdateRect(ref pathRect, pPoint);
            Debug.LogError($"AddPoint更新区域11--->{pPoint}:{pathRect}");

            UpdateContainPathRect();

            if (!pathDict.ContainsKey(pPoint.x))
                pathDict.Add(pPoint.x, new Dictionary<int, PathPoint>());
            if (!pathDict[pPoint.x].ContainsKey(pPoint.y))
            {
                PathPoint pathPoint = new PathPoint();
                pathPoint.point = new Vector2Int(pPoint.x, pPoint.y);

                //添加上一个点
                if (CheckInPath(currPoint))
                {
                    PathPoint lastPoint = pathDict[currPoint.x][currPoint.y];
                    lastPoint.nextPoint = pathPoint;

                    pathPoint.lastPoint = lastPoint;
                }

                pathDict[pPoint.x].Add(pPoint.y, pathPoint);
                currPoint = pathPoint.point;
            }

            //消息
            GamerPathChangeS2c msg = new GamerPathChangeS2c();
            msg.gamerUid = GamerUid;
            msg.Operate = 1;
            msg.Pos = pPoint.ToNetVector2();
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GamerPathChangeS2c, msg);
        }

        //更新包围框
        private void UpdateContainPathRect()
        {
            Vector2Int leftDownPoint = pathRect.min;
            if (leftDownPoint.x <= 0)
                leftDownPoint.x = 0;
            else if (leftDownPoint.x >= mapData.Size.x)
                leftDownPoint.x = mapData.Size.x;
            else
                leftDownPoint.x -= 1;

            if (leftDownPoint.y <= 0)
                leftDownPoint.y = 0;
            else if (leftDownPoint.y >= mapData.Size.y)
                leftDownPoint.y = mapData.Size.y;
            else
                leftDownPoint.y -= 1;

            Vector2Int rightUpPoint = pathRect.max;
            if (rightUpPoint.x <= 0)
                rightUpPoint.x = 0;
            else if (rightUpPoint.x >= mapData.Size.x)
                rightUpPoint.x = mapData.Size.x;
            else
                rightUpPoint.x += 1;

            if (rightUpPoint.y <= 0)
                rightUpPoint.y = 0;
            else if (rightUpPoint.y >= mapData.Size.y)
                rightUpPoint.y = mapData.Size.y;
            else
                rightUpPoint.y += 1;

            containPathRect.min = leftDownPoint;
            containPathRect.max = rightUpPoint;
        }

        /// <summary>
        /// 删除路径点
        /// </summary>
        public void RemovePoint(Vector2Int pRemovePoint)
        {
            if (pathDict.ContainsKey(pRemovePoint.x) && pathDict[pRemovePoint.x].ContainsKey(pRemovePoint.y))
                pathDict[pRemovePoint.x].Remove(pRemovePoint.y);

            Debug.LogWarning($"RemovePathPoint成功Server--{pRemovePoint}");
            //消息
            GamerPathChangeS2c msg = new GamerPathChangeS2c();
            msg.gamerUid = GamerUid;
            msg.Operate = 2;
            msg.Pos = pRemovePoint.ToNetVector2();
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GamerPathChangeS2c, msg);
        }

        public List<Vector2Int> RemovePath(Vector2Int lastPoint, Vector2Int currPoint)
        {
            if (!CheckInPath(lastPoint) || !CheckInPath(currPoint))
            {
                return null;
            }

            PathPoint last = pathDict[lastPoint.x][lastPoint.y];
            PathPoint check = pathDict[currPoint.x][currPoint.y];

            List<PathPoint> list = new List<PathPoint>();
            CalcPoint(last, list, check);

            List<Vector2Int> pointList = new List<Vector2Int>();
            foreach (PathPoint pathPoint in list)
            {
                RemovePoint(pathPoint.point);
            }

            //更新区域
            //pathRect = mapData.GetCampRect(Camp);
            Debug.LogError($"RemovePath更新区域00--->{currPoint}:{pathRect}");
            RectEx.UpdateRect(ref pathRect, currPoint);
            Debug.LogError($"RemovePath更新区域11--->{currPoint}:{pathRect}");

            //清理
            check.nextPoint = null;
            this.currPoint = check.point;

            return pointList;
        }

        private void CalcPoint(PathPoint point, List<PathPoint> pointList, PathPoint checkPoint)
        {
            if (point.lastPoint == null)
            {
                return;
            }

            if (point.lastPoint == checkPoint)
            {
                pointList.Add(point);
                return;
            }

            pointList.Add(point);
            CalcPoint(point.lastPoint, pointList, checkPoint);
        }

        #endregion

        #region 区域占领

        //占领区域
        public List<Vector2Int> CaptureArea(Action<Vector2Int> changeCampCallBack)
        {
            List<Vector2Int> capturePoints = new List<Vector2Int>();

            //先将路径变为占领区域
            foreach (int x in pathDict.Keys)
            {
                foreach (int y in pathDict[x].Keys)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    capturePoints.Add(pos);
                    mapData.SetPointCamp(pos, Camp);
                    changeCampCallBack?.Invoke(pos);
                }
            }

            for (int x = pathRect.min.x; x <= pathRect.max.x; x++)
            {
                for (int y = pathRect.min.y; y <= pathRect.max.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Debug.LogError($"CaptureArea:Pos:{pos}--{!CheckInPath(pos)}-->{mapData.GetPointCamp(pos)};{Camp}");
                    //不在路径中并且阵营不同
                    if (!CheckInPath(pos) && mapData.GetPointCamp(pos) != Camp)
                    {
                        if (CheckPointCanCapture(pos))
                        {
                            Vector2Int tPos = new Vector2Int(x, y);
                            capturePoints.Add(tPos);
                            mapData.SetPointCamp(tPos, Camp);
                            changeCampCallBack?.Invoke(tPos);
                        }
                    }
                }
            }

            return capturePoints;
        }

        //检测点是不是可以占领
        private bool CheckPointCanCapture(Vector2Int pPos)
        {
            Debug.LogWarning($"CheckPointCanCapture:Pos:{pPos}");
            byte checkCnt = 0;
            //向左检测
            for (int leftX = pathRect.min.x; leftX <= pPos.x; leftX++)
            {
                if (mapData.GetPointCamp(leftX, pPos.y) == Camp)
                {
                    checkCnt++;
                    checkPoints[0] = new Vector2Int(leftX, pPos.y);
                    break;
                }
            }
            //向右检测
            for (int rightX = pPos.x; rightX <= pathRect.max.x; rightX++)
            {
                if (mapData.GetPointCamp(rightX, pPos.y) == Camp)
                {
                    checkCnt++;
                    checkPoints[1] = new Vector2Int(rightX, pPos.y);
                    break;
                }
            }

            //向上检测
            for (int upY = pPos.y; upY <= pathRect.max.y; upY++)
            {
                if (mapData.GetPointCamp(pPos.x, upY) == Camp)
                {
                    checkCnt++;
                    checkPoints[2] = new Vector2Int(pPos.x, upY);
                    break;
                }
            }
            //向下检测
            for (int downY = pathRect.min.y; downY <= pPos.y; downY++)
            {
                if (mapData.GetPointCamp(pPos.x, downY) == Camp)
                {
                    checkCnt++;
                    checkPoints[3] = new Vector2Int(pPos.x, downY);
                    break;
                }
            }

            //四个方向没全
            if (checkCnt != 4)
            {
                return false;
            }

            //return true;
            Vector2Int pointB = containPathRect.min;
            if (pointB.Equals(pathRect.min))
            {
                pointB = containPathRect.max;
            }

            return !CheckTowPointIsConnect(pPos, pointB);
        }

        //检测俩点相连
        private bool CheckTowPointIsConnect(Vector2Int pointA, Vector2Int pointB)
        {
            PathGrid pathGrid = mapData.GetPathGrid(Camp);
            PathNode startNode = pathGrid.GetNode(pointA.x, pointA.y);
            PathNode targetNode = pathGrid.GetNode(pointB.x, pointB.y);

            List<Vector2Int> pathList = pathGrid.FindPath(startNode, targetNode, CheckPathNodeNeedCheck);
            return pathList.IsLegal();
        }

        private bool CheckPathNodeNeedCheck(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);
            return containPathRect.Contains(pos);
        }

        #endregion

        #region Check

        public bool CheckInPath(Vector2Int pPoint)
        {
            return pathDict.ContainsKey(pPoint.x) && pathDict[pPoint.x].ContainsKey(pPoint.y);
        }

        #endregion

        #region Helper

        private void TrySetRectX(ref RectInt pRect, int pPosX)
        {
            if (pPosX < pRect.min.x)
            {
                pRect.min = new Vector2Int(pPosX, pRect.min.y);
            }
            else if (pPosX > pRect.max.x)
            {
                pRect.max = new Vector2Int(pPosX, pRect.max.y);
            }
        }

        private void TrySetRectY(ref RectInt pRect, int pPosY)
        {
            if (pPosY < pRect.min.y)
            {
                pRect.min = new Vector2Int(pRect.min.x, pPosY);
            }
            else if (pPosY > pRect.max.y)
            {
                pRect.max = new Vector2Int(pRect.max.x, pPosY);
            }
        }


        #endregion
    }
}
