using IAEngine;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
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
            pathRect = mapData.GetCampRect(Camp);
            currPoint = new Vector2Int(-1,-1);

            NetServerLocate.NetCom.OnDrawGizmosFunc += ()=>{
                IAToolkit.GizmosHelper.DrawBounds(new Vector3(pathRect.center.x, 0, pathRect.center.y), new Vector3(pathRect.size.x, 1, pathRect.size.y), Color.blue);
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

            RectEx.UpdateRectOnAddPoint(ref pathRect, pPoint);

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

        /// <summary>
        /// 删除路径点
        /// </summary>
        public void RemovePoint(Vector2Int pRemovePoint)
        {
            if (pathDict.ContainsKey(pRemovePoint.x) && pathDict[pRemovePoint.x].ContainsKey(pRemovePoint.y))
                pathDict[pRemovePoint.x].Remove(pRemovePoint.y);

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
            RectEx.UpdateRectOnAddPoint(ref pathRect, currPoint);

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

        private Dictionary<int, Dictionary<int, int>> needCheckCapturePos = new Dictionary<int, Dictionary<int, int>>();
        private Dictionary<int, HashSet<int>> closeCheckPos = new Dictionary<int, HashSet<int>>();
        private ChangeGridCampS2c changeGridMsg = new ChangeGridCampS2c();
        //占领区域
        public void CaptureArea(Action<Vector2Int> changeCampCallBack)
        {
            needCheckCapturePos.Clear();

            //广播消息
            changeGridMsg.Camp = Camp;
            changeGridMsg.gridPosLists.Clear();

            //先将路径变为占领区域
            foreach (int x in pathDict.Keys)
            {
                foreach (int y in pathDict[x].Keys)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    AddChangeGridCampMsg(new NetVector2Int() { X = x, Y = y });
                    mapData.SetPointCamp(pos, Camp);
                    changeCampCallBack?.Invoke(pos);
                }
            }

            //计算区域内的点
            for (int x = pathRect.min.x; x <= pathRect.max.x; x++)
            {
                for (int y = pathRect.min.y; y <= pathRect.max.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    //不在路径中并且阵营不同
                    if (!CheckInPath(pos) && mapData.GetPointCamp(pos) != Camp)
                    {
                        if (!needCheckCapturePos.ContainsKey(pos.x))
                            needCheckCapturePos.Add(pos.x, new Dictionary<int, int>());
                        needCheckCapturePos[pos.x].Add(pos.y, 0);  
                    }
                }
            }

            List<int> xList = needCheckCapturePos.Keys.ToList();
            for (int i = 0; i < xList.Count; i++)
            {
                int x = xList[i];
                List<int> yList = needCheckCapturePos[x].Keys.ToList();
                for (int j = 0; j < yList.Count; j++)
                {
                    int y = yList[j];
                    closeCheckPos.Clear();
                    if (CheckPointCanCapture(new Vector2Int(x, y)))
                    {
                        Vector2Int tPos = new Vector2Int(x, y);
                        AddChangeGridCampMsg(new NetVector2Int() { X = x, Y = y });
                        mapData.SetPointCamp(tPos, Camp);
                        changeCampCallBack?.Invoke(tPos);
                    }
                }
            }

            //更新区域范围
            pathRect = mapData.GetCampRect(Camp);

            AddChangeGridCampMsg(null, true);
        }


        private void AddChangeGridCampMsg(NetVector2Int pPos, bool pForceSendLeft = false)
        {
            if (pPos != null)
            {
                changeGridMsg.gridPosLists.Add(pPos);
            }

            if (changeGridMsg.gridPosLists.Count >= 50 ||pForceSendLeft)
            {
                //广播
                NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.ChangeGridCampS2c, changeGridMsg);
                changeGridMsg.gridPosLists.Clear();
            }
        }

        //检测点是不是可以占领
        private List<Vector2Int> dirPosList = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
        };
        private bool CheckPointCanCapture(Vector2Int pPos)
        {
            if (!needCheckCapturePos.ContainsKey(pPos.x) || !needCheckCapturePos[pPos.x].ContainsKey(pPos.y))
            {
                return false;
            }

            //在边界
            if (mapData.GetPointCamp(pPos.x, pPos.y) == Camp
            || needCheckCapturePos[pPos.x][pPos.y] == 1)
            {
                return true;
            }

            int resCnt = 0;
            for (int i = 0; i < dirPosList.Count; i++)
            {
                Vector2Int tPos = dirPosList[i] + pPos;

                //在边界
                if (mapData.GetPointCamp(tPos.x, tPos.y) == Camp 
                || (closeCheckPos.ContainsKey(tPos.x) && closeCheckPos[tPos.x].Contains(tPos.y)))
                {
                    resCnt++;
                }
                else
                {
                    //获取缓存值
                    int tValue = 0;
                    if (needCheckCapturePos.ContainsKey(tPos.x) && needCheckCapturePos[tPos.x].ContainsKey(tPos.y))
                        tValue = needCheckCapturePos[tPos.x][tPos.y];

                    //已经检测为不封闭
                    if (tValue == -1)
                    {
                        needCheckCapturePos[pPos.x][pPos.y] = -1;
                        return false;
                    }
                    //已经检测为封闭
                    else if (tValue == 1)
                    {
                        resCnt++;
                    }
                    else
                    {
                        //加入忽略
                        if (!closeCheckPos.ContainsKey(pPos.x))
                            closeCheckPos.Add(pPos.x, new HashSet<int>());
                        closeCheckPos[pPos.x].Add(pPos.y);

                        if (CheckPointCanCapture(tPos))
                        {
                            resCnt++;
                        }
                        else
                        {
                            needCheckCapturePos[pPos.x][pPos.y] = -1;
                            return false;
                        }
                    }
                }
            }

            bool success = resCnt == 4;
            needCheckCapturePos[pPos.x][pPos.y] = success ? 1 : -1;
            return success;
        }

        #endregion

        #region Check

        public bool CheckInPath(int pPosX, int pPosY)
        {
            return pathDict.ContainsKey(pPosX) && pathDict[pPosX].ContainsKey(pPosY);
        }

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
