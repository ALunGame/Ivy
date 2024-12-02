using Game;
using IAEngine;
using IAFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

namespace Gameplay
{
    /// <summary>
    /// CPU实例化格子
    /// </summary>
    public class MapGridsCPU : MonoBehaviour
    {
        public static Vector3 GridSize = new Vector3(0.9f, 0.9f, 0.9f);
        public static string GridPoolName = "Map_Grid";
        //逻辑数据
        class GridData
        {
            public GameObject go;                   //格子节点
            public Vector2 position;                //位置
            public int camp;                        //阵营
            public float scaleY;                    //当前的Y轴缩放

            public int animGridIndex;               //当前网格缩放动画Index
            public float animTime;                  //动画时间
            public float refreshAnimTime;           //动画刷新时间
            public Vector2 animScale;               //动画最大最小缩放
            public bool isThroughing;               //正在被经过
            public bool isLockByOtherGamer;         //被其他不是该阵营的玩家经过锁住

            public GridData(GameObject pGo, Vector2Int pPos)
            {
                go = pGo;
                position = pPos;
            }

            //每帧更新
            public void Update(float pTimeDelta)
            {
                if (isLockByOtherGamer)
                {
                    return;
                }
                else if (isThroughing)
                {
                    float newScale = animScale.x;
                    //播放完毕
                    if (scaleY == newScale)
                    {
                        return;
                    }
                    //大于目标缩放才播放
                    if (scaleY > newScale)
                    {
                        animTime += pTimeDelta;
                        if (animTime >= refreshAnimTime)
                        {
                            go.transform.localScale = new Vector3(GridSize.x, newScale, GridSize.z);
                            animTime = 0;
                            scaleY = newScale;
                        }
                    }
                }
                else
                {
                    animTime += pTimeDelta;
                    if (animTime >= refreshAnimTime)
                    {
                        float newScale = scaleY == animScale.x ? animScale.y : animScale.x;
                        go.transform.localScale = new Vector3(GridSize.x, newScale, GridSize.z); animTime = 0;
                        scaleY = newScale;
                    }
                }
            }

            public void SetCamp(int pCamp)
            {
                camp = pCamp;
                if (pCamp != 0)
                {
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetColor("_MainColor", TempConfig.CampColorDict[pCamp]);
                }
            }
        }

        [Header("地图大小")]
        public Vector2Int MapSize;
        [Header("格子动画时长")]
        public float GridAnimTime = 0.2f;
        [Header("格子阵营缩放配置")]
        public List<Vector2> GridCampScales = new List<Vector2>();

        [Header("玩家经过阵营缩放配置")]
        public List<float> GamerThroughCampScales = new List<float>();
        [Header("玩家经过阵营缩放动画区域")]
        public Vector2Int GamerThroughAnimArea = new Vector2Int(4, 4);
        [Header("玩家经过阵营缩放动画时长")]
        public float GamerThroughAnimTime = 0.2f;

        private Dictionary<int, Dictionary<int, GridData>> gridDataDict = new Dictionary<int, Dictionary<int, GridData>>();

        private bool GetGrid(Vector2Int pPos, out GridData pGrid)
        {
            if (!gridDataDict.ContainsKey(pPos.x) || !gridDataDict[pPos.x].ContainsKey(pPos.y))
            {
                pGrid = null;
                return false;
            }

            pGrid = gridDataDict[pPos.x][pPos.y];
            return true;
        }

        private GridData GetGridNullCreate(Vector2Int pPos)
        {
            GridData gridData = null;
            if (!GetGrid(pPos, out gridData))
            {
                if (!CachePool.HasGameObjectPool(GridPoolName))
                {
                    CachePool.CreateGameObjectPool(GridPoolName, () =>
                    {
                        return GameEnv.Asset.CreateGo("MapGrid");
                    });
                }

                GameObject pathPointGo = CachePool.GetGameObject(GridPoolName);
                pathPointGo.transform.SetParent(transform);
                pathPointGo.transform.position = new Vector3(pPos.x, 0.1f, pPos.y);
                pathPointGo.name = pPos.ToString();

                gridData = new GridData(pathPointGo, pPos);
                if (!gridDataDict.ContainsKey(pPos.x))
                    gridDataDict.Add(pPos.x, new Dictionary<int, GridData>());
                gridDataDict[pPos.x].Add(pPos.y, gridData);
            }
            return gridData;    
        }

        public void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            foreach (int x in gridDataDict.Keys)
            {
                foreach (int y in gridDataDict[x].Keys)
                {
                    GridData gridData = gridDataDict[x][y];
                    gridData.Update(pDeltaTime);
                }
            }
        }

        public void ChangeGridsCamp(List<Vector2Int> pGridPosList, int pCamp)
        {
            int scaleIndex = 0;
            if (pCamp != 0)
                scaleIndex = RandomHelper.Range(1, GridCampScales.Count - 1);

            for (int i = 0; i < pGridPosList.Count; i++)
            {
                ChangeGridCamp(pGridPosList[i], pCamp, scaleIndex);
            }
        }

        private void ChangeGridCamp(Vector2Int pGridPos, int pCamp, int pScaleIndex)
        {
            if (pCamp == 0)
            {
                if (GetGrid(pGridPos, out GridData gridData))
                {
                    gridData.SetCamp(pCamp);
                    //设置动画数据
                    SetBaseCampGridAnimCfg(pGridPos, 0);
                }
            }
            else
            {
                GridData gridData = GetGridNullCreate(pGridPos);
                gridData.SetCamp(pCamp);
                //设置动画数据
                SetBaseCampGridAnimCfg(pGridPos, pScaleIndex);
            }
        }

        /// <summary>
        /// 当玩家穿过阵营区域时
        /// </summary>
        public void OnGamerThroughCampArea(int pCamp, Vector2Int pGridPos)
        {
            GridData gridData = null;
            if (!GetGrid(pGridPos, out gridData))
            {
                return;
            }

            //自己阵营
            if (gridData.camp == pCamp)
            {
                gridData.isLockByOtherGamer = false;
            }
            else
            {
                gridData.isLockByOtherGamer = true;
                SetLockByOtherGamerAnimCfg(pGridPos);
            }

            //刷新经过动画
            RefreshGamerThroughAreaAnim(pGridPos);
        }

        public void OnGamerPathChange(List<Vector2Int> pPosList, int pOperate)
        {
            //Add
            if (pOperate == 1)
            {

            }
            //Remove Clear
            else
            {
                foreach (var item in pPosList)
                {
                    if (GetGrid(item, out GridData gridData))
                    {
                        if (gridData.isLockByOtherGamer)
                        {
                            SetBaseCampGridAnimCfg(item, gridData.animGridIndex);
                        }
                    }
                }
            }
        }

        private void RefreshGamerThroughAreaAnim(Vector2Int pGridPos)
        {
            bool needRefresh = false;

            Vector2Int leftDownPos = pGridPos - (GamerThroughAnimArea / 2);
            for (int x = leftDownPos.x; x <= leftDownPos.x + GamerThroughAnimArea.x; x++)
            {
                for (int y = leftDownPos.y; y <= leftDownPos.y + GamerThroughAnimArea.y; y++)
                {
                    if (GetGrid(new Vector2Int(x, y), out GridData gridData))
                    {
                        if (gridData.camp != 0 && !gridData.isThroughing && !gridData.isLockByOtherGamer)
                        {
                            int xDis = Mathf.Abs(pGridPos.x - x);
                            int yDis = Mathf.Abs(pGridPos.y - y);
                            int resIndex = xDis > yDis ? xDis : yDis;
                            SetThroughCampGridAnimCfg(pGridPos, resIndex);
                            needRefresh = true;
                        }
                    }
                }
            }
        }

        //设置基础阵营动画配置
        private void SetBaseCampGridAnimCfg(Vector2Int pGridPos, int pAnimIndex)
        {
            if (GetGrid(pGridPos, out GridData gridData))
            {
                gridData.animGridIndex = pAnimIndex;
                gridData.animTime = 0;
                gridData.refreshAnimTime = GridAnimTime;
                gridData.animScale = GridCampScales[pAnimIndex];
                gridData.scaleY = gridData.animScale.x;
                gridData.isLockByOtherGamer = false;
            }
        }

        //设置玩家穿过阵营动画配置
        private void SetThroughCampGridAnimCfg(Vector2Int pGridPos, int pAnimIndex)
        {
            float targetScale = GamerThroughCampScales[pAnimIndex];

            if (GetGrid(pGridPos, out GridData gridData))
            {
                gridData.animTime = 0;
                gridData.refreshAnimTime = GamerThroughAnimTime;
                gridData.animScale = new Vector2(targetScale, targetScale);
                gridData.isThroughing = true;
            }
        }

        //被其他不是该阵营的玩家经过锁住的动画配置
        private void SetLockByOtherGamerAnimCfg(Vector2Int pGridPos)
        {
            if (GetGrid(pGridPos, out GridData gridData))
            {
                gridData.animTime = 0;
                gridData.refreshAnimTime = 0;
                gridData.animScale = Vector2.zero;
                gridData.scaleY = gridData.animScale.x;
            }
        }
    }
}
