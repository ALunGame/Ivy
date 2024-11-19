using Game.AStar;
using IAEngine;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;
using static IAEngine.RectEx;

namespace Game.Network.Server
{
    internal class ServerMapData : BaseServerGameData
    {
        /// <summary>
        /// 地图尺寸
        /// </summary>
        public Vector2Int Size { get; private set; }

        /// <summary>
        /// 阵营点
        /// </summary>
        public int[,] CampPoint { get; private set; }

        //阵营区域
        private Dictionary<int, RectInt> campRectDict = new Dictionary<int, RectInt>();
        //阵营寻路区域
        private Dictionary<int, PathGrid> campPathGridDict = new Dictionary<int, PathGrid>();
        //玩家路径
        private Dictionary<string, ServerMapGamerPathData> gamerPathDict = new Dictionary<string, ServerMapGamerPathData>();

        public void Create(int pWidth, int pHeight)
        {
            Size = new Vector2Int(pWidth, pHeight);

            CampPoint = new int[pWidth, pHeight];
            for (int x = 0; x < pWidth; x++)
            {
                for (int y = 0; y < pHeight; y++)
                {
                    CampPoint[x, y] = 0;
                }
            }

            NetServerLocate.NetCom.OnDrawGizmosFunc += () => {
                //IAToolkit.GizmosHelper.DrawRect(pathRect, Color.black);
                //Gizmos.DrawCube(pathRect.center, new Vector3(pathRect.size.x, 10, pathRect.size.y));

                foreach (int camp in campRectDict.Keys)
                {
                    RectInt rect = campRectDict[camp];
                    IAToolkit.GizmosHelper.DrawBounds(new Vector3(rect.center.x, 0, rect.center.y), new Vector3(rect.size.x, 1, rect.size.y), TempConfig.CampColorDict[camp]);
                }
            };
        }

        public override void OnClear()
        {
            foreach (var item in gamerPathDict.Values)
            {
                item.Clear();
            }
            gamerPathDict.Clear();
        }

        public void AddGamerPathData(ServerGamerData pGamerData)
        {
            if (gamerPathDict.ContainsKey(pGamerData.GamerUid))
            {
                return;
            }
            ServerMapGamerPathData pathData = new ServerMapGamerPathData(this, pGamerData.GamerUid, pGamerData.Camp);
            gamerPathDict.Add(pGamerData.GamerUid, pathData);
        }

        #region Check

        /// <summary>
        /// 检测点合法
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public bool CheckPointIsLegal(int posX, int posY)
        {
            if (posX < 0 || posX >= Size.x)
            {
                return false;
            }

            if (posY < 0 || posY >= Size.y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测点在边界
        /// </summary>
        /// <param name="pPosX">X轴</param>
        /// <returns></returns>
        public bool CheckPosXInBorder(int pPosX)
        {
            return pPosX == 0 || pPosX == Size.x - 1;
        }

        /// <summary>
        /// 检测点在边界
        /// </summary>
        /// <param name="pPosY">Y轴</param>
        /// <returns></returns>
        public bool CheckPosYInBorder(int pPosY)
        {
            return pPosY == 0 || pPosY == Size.y - 1;
        }


        #endregion

        #region Get

        /// <summary>
        /// 获取阵营区域
        /// </summary>
        /// <param name="pCamp"></param>
        /// <returns></returns>
        public RectInt GetCampRect(int pCamp)
        {
            return campRectDict[pCamp];
        }

        /// <summary>
        /// 获得点所处的阵营
        /// </summary>
        /// <param name="pPosX"></param>
        /// <param name="pPosY"></param>
        /// <returns></returns>
        public int GetPointCamp(int pPosX, int pPosY)
        {
            if (!CheckPointIsLegal(pPosX, pPosY))
            {
                return 0;
            }

            return CampPoint[pPosX, pPosY];
        }

        /// <summary>
        /// 获得点所处的阵营
        /// </summary>
        /// <param name="pPosX"></param>
        /// <param name="pPosY"></param>
        /// <returns></returns>
        public int GetPointCamp(Vector2Int pPos)
        {
            return GetPointCamp(pPos.x, pPos.y);
        }

        //获得在路径中的玩家Uid
        public string GetPlayerUidInPathPoint(int pPosX, int pPosY)
        {
            foreach (string gamerUid in gamerPathDict.Keys)
            {
                if (gamerPathDict[gamerUid].CheckInPath(new Vector2Int(pPosX, pPosY)))
                {
                    return gamerUid;
                }
            }
            return "";
        }

        /// <summary>
        /// 随机获得指定阵营的一个点
        /// </summary>
        /// <param name="pCamp"></param>
        /// <returns></returns>
        public bool GetRandomPointInCamp(int pCamp, Vector2Int outPos)
        {
            if (!campRectDict.ContainsKey(pCamp))
                return false;

            RectInt campRect = campRectDict[pCamp];

            for (int x = campRect.min.x; x <= campRect.max.x; x++)
            {
                for (int y = campRect.min.y; y <= campRect.max.y; y++)
                {
                    if (GetPointCamp(x, y) == pCamp)
                    {
                        int random = UnityEngine.Random.Range(0, 2);
                        if (random == 1)
                        {
                            outPos = new Vector2Int(x, y);  
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public ServerMapGamerPathData GetGamerPathData(string pGamerUid)
        {
            if (!gamerPathDict.ContainsKey(pGamerUid))
            {
                return null;
            }
            return gamerPathDict[pGamerUid];
        }

        #endregion

        #region Set

        /// <summary>
        /// 改变点阵营
        /// </summary>
        /// <param name="pPoint"></param>
        /// <param name="pCamp"></param>
        public void SetPointCamp(Vector2Int pPoint, int pCamp)
        {
            if (!CheckPointIsLegal(pPoint.x, pPoint.y))
            {
                return;
            }

            if (CampPoint[pPoint.x, pPoint.y] == pCamp)
            {
                return;
            }

            //旧阵营
            int oldCamp = CampPoint[pPoint.x, pPoint.y];

            //更新区域
            if (!campRectDict.ContainsKey(pCamp))
                campRectDict.Add(pCamp, new RectInt());
            RectInt rect = campRectDict[pCamp];
            RectEx.UpdateRectOnAddPoint(ref rect, pPoint);
            campRectDict[pCamp] = rect;

            //更新数据
            CampPoint[pPoint.x, pPoint.y] = pCamp;

            //更新删除的区域
            if (oldCamp != 0)
            {
                UpdateRectOnRemoveCamp(oldCamp, pPoint);
            }
        }

        //当删除阵营时更新阵营区域
        private void UpdateRectOnRemoveCamp(int pCamp, Vector2Int pPoint) 
        {
            RectInt campRect = campRectDict[pCamp];
            if (RectEx.CheckInBorder(campRect, pPoint, out RectBorder border))
            {
                List<Vector2Int> points = RectEx.GetPointListByBorder(campRect, border);
                for (int i = 0; i < points.Count; i++)
                {
                    //有一个还在边框内，不需要更新
                    if (GetPointCamp(points[i]) == pCamp)
                    {
                        return;
                    }
                }

                //更新边框
                RectEx.RemoveRectBorder(ref campRect, border);
                campRectDict[pCamp] = campRect;
            }
        }

        /// <summary>
        /// 设置区域阵营
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="camp"></param>
        public void SetRectCamp(RectInt pRect, int pCamp)
        {
            NetServerLocate.Log.Log($"改变区域阵营>>{pRect}->{pCamp}");

            //消息
            ChangeGridCampS2c msg = new ChangeGridCampS2c();
            msg.Camp = pCamp;

            for (int x = pRect.min.x; x <= pRect.max.x; x++)
            {
                for (int y = pRect.min.y; y <= pRect.max.y; y++)
                {
                    msg.gridPosLists.Add(new NetVector2Int() { X = x, Y = y });
                    SetPointCamp(new Vector2Int(x, y), pCamp);
                }
            }

            //广播
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.ChangeGridCampS2c, msg);
        }

        /// <summary>
        /// 添加路径点
        /// </summary>
        /// <param name="pGamerUid"></param>
        /// <param name="pGridPos"></param>
        public void AddGamerPathPoint(string pGamerUid, Vector2Int pGridPos)
        {
            if (gamerPathDict.TryGetValue(pGamerUid, out var pathData))
            {
                pathData.AddPoint(pGridPos);
            }
        }

        /// <summary>
        /// 移除路径点
        /// </summary>
        /// <param name="pGamerUid"></param>
        /// <param name="pGridPos"></param>
        public void RemoveGamerPathPoint(string pGamerUid, Vector2Int pGridPos)
        {
            if (gamerPathDict.TryGetValue(pGamerUid, out var pathData))
            {
                pathData.RemovePoint(pGridPos);
            }
        }

        #endregion

        /// <summary>
        /// 坐标转格子
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public Vector2Int PosToGrid(Vector2 pPos)
        {
            return new Vector2Int((int)pPos.x, (int)pPos.y);
        }

        /// <summary>
        /// 随机获取一个指定大小的阵营区域
        /// </summary>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <param name="pCheckCamp"></param>
        /// <returns></returns>
        public bool RandomGetCampRect(int pWidth, int pHeight, int pCheckCamp, out RectInt OutRect)
        {
            Dictionary<int, int> xMap = RandomHelper.GetRandomNumList(0, Size.x - pWidth - 1);
            Dictionary<int, int> yMap = RandomHelper.GetRandomNumList(0, Size.y - pHeight - 1);

            foreach (int key1 in xMap.Keys)
            {
                int x = xMap[key1];
                foreach (var key2 in yMap.Keys)
                {
                    int y = yMap[key2];
                    if (CampPoint[x, y] == pCheckCamp)
                    {
                        if (CheckRectIsLegal(x, y, pWidth, pHeight, pCheckCamp))
                        {
                            OutRect = new RectInt(x, y, pWidth, pHeight);
                            return true;
                        }
                    }
                }
            }

            OutRect = new RectInt(0, 0, 0, 0);
            return false;
        }

        private bool CheckRectIsLegal(int posX, int posY, int width, int height, int pCamp)
        {
            for (int x = posX; x < posX + width; x++)
            {
                for (int y = posY; y < posY + height; y++)
                {
                    if (!CheckPointIsLegal(x, y) || CampPoint[x, y] != pCamp)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public PathGrid GetPathGrid(int pCamp)
        {
            return campPathGridDict[pCamp];
        }

        internal ServerGamerData GetGamer(Vector2Int pPos)
        {
            string gamerUid = GetPlayerUidInPathPoint(pPos.x, pPos.y);
            return NetServerLocate.GameCtrl.GameData.GetGamer(gamerUid);
        }
    }
}
