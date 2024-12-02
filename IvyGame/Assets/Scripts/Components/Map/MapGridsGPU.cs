using Game;
using IAEngine;
using System.Collections.Generic;
using UnityEngine;
using InteropServices = System.Runtime.InteropServices;

namespace Gameplay
{
    /// <summary>
    /// GPU实例化格子
    /// </summary>
    public class MapGridsGPU : MonoBehaviour
    {
        //实例化参数
        struct InstanceParam
        {
            public Color color;
            public Matrix4x4 instanceToObjectMatrix;        //实例化到物方矩阵
        }

        //逻辑数据
        class GridParam
        {
            public int index;                       //索引
            public Vector3 position;                //位置
            public int camp;                        //阵营
            public float scaleY;                    //当前的Y轴缩放

            public int animGridIndex;               //当前网格缩放动画Index
            public float animTime;                  //动画时间
            public float refreshAnimTime;           //动画刷新时间
            public Vector2 animScale;               //动画最大最小缩放
            public bool isThroughing;               //正在被经过
            public bool isLockByOtherGamer;         //被其他不是该阵营的玩家经过锁住
        }

        [Header("地图大小")]
        public Vector2Int MapSize;
        [Header("格子动画时长")]
        public float GridAnimTime = 0.2f;
        [Header("格子网格")]
        public Mesh GridMesh;
        [Header("格子材质")]
        public Material GridMaterial;
        [Header("格子大小")]
        public Vector3 GridSize;
        [Header("格子阵营缩放配置")]
        public List<Vector2> GridCampScales = new List<Vector2>();

        [Header("玩家经过阵营缩放配置")]
        public List<float> GamerThroughCampScales = new List<float>();
        [Header("玩家经过阵营缩放动画区域")]
        public Vector2Int GamerThroughAnimArea = new Vector2Int(4, 4);
        [Header("玩家经过阵营缩放动画时长")]
        public float GamerThroughAnimTime = 0.2f;

        //格子数量
        private int instanceCount;
        //物体的复合变换矩阵Buffer
        private ComputeBuffer allMatricesBuffer;
        private ComputeBuffer argsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        //每个格子的位置和缩放
        private InstanceParam[] shaderArgs;
        private Dictionary<int, Dictionary<int, int>> gridIndexMap = new Dictionary<int, Dictionary<int, int>>();
        private Dictionary<int, GridParam> gridDataDict = new Dictionary<int, GridParam>();
        private HashSet<int> animIndexList = new HashSet<int>();
        //渲染区域
        private Bounds renderBounds = new Bounds();

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            IAToolkit.GizmosHelper.DrawBounds(renderBounds, Color.blue); 
#endif
        }

        private void OnDisable()
        {
            allMatricesBuffer?.Release();
            allMatricesBuffer = null;

            argsBuffer?.Release();
            argsBuffer = null;
        }

        public void CreateMap(Vector2Int pMapSize)
        {
            MapSize = pMapSize;

            animIndexList.Clear();
            gridIndexMap.Clear();
            gridDataDict.Clear();

            instanceCount = MapSize.x * MapSize.y;
            shaderArgs = new InstanceParam[instanceCount];

            renderBounds.min = transform.position;
            renderBounds.max = transform.position + new Vector3(MapSize.x, 0, MapSize.y) + new Vector3(0,5,0);

            Vector3 boundSize = new Vector3(MapSize.x / 2, 0, MapSize.y / 2);
            renderBounds = new Bounds(transform.position + boundSize, new Vector3(MapSize.x, 5, MapSize.y));
            InitMapGrid();
        }

        private void InitMapGrid()
        {
            //复合变换矩阵
            int index = 0;
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    var position = transform.position + new Vector3(x, 0, y);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(GridSize.x, 0.1f, GridSize.z));
                    shaderArgs[index].color = TempConfig.CampColorDict[0];

                    if (!gridIndexMap.ContainsKey(x))
                        gridIndexMap.Add(x, new Dictionary<int, int>());
                    if (!gridIndexMap[x].ContainsKey(y))
                        gridIndexMap[x].Add(y, index);

                    gridDataDict.Add(index, new GridParam()
                    {
                        index = index,
                        camp = 0,
                        position = position,
                    });

                    index++;
                }
            }

            //设置数据
            allMatricesBuffer?.Release();
            int stride = InteropServices.Marshal.SizeOf(typeof(InstanceParam));
            allMatricesBuffer = new ComputeBuffer(instanceCount, stride);
            allMatricesBuffer.SetData(shaderArgs);

            //设置到Shader
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);

            // Indirect args
            argsBuffer?.Release();
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            if (GridMesh != null)
            {
                args[0] = GridMesh.GetIndexCount(0);
                args[1] = (uint)instanceCount;
                args[2] = GridMesh.GetIndexStart(0);
                args[3] = GridMesh.GetBaseVertex(0);
            }
            else
            {
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            argsBuffer.SetData(args);
        }

        public void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (allMatricesBuffer == null)
            {
                return;
            }

            //动画
            PlayGridAnim(pDeltaTime);

            //渲染
            Graphics.DrawMeshInstancedIndirect(GridMesh, 0, GridMaterial, renderBounds, argsBuffer);
        }

        //播放格子缩放动画
        private void PlayGridAnim(float pTimeDelta)
        {
            bool needRefresh = false;
            foreach (int index in animIndexList)
            {
                GridParam gridParam = gridDataDict[index];
                if (gridParam.isLockByOtherGamer)
                {
                    continue;
                }
                else if (gridParam.isThroughing)
                {
                    float newScale = gridParam.animScale.x;
                    //播放完毕
                    if (gridParam.scaleY == newScale)
                    {
                        continue;
                    }
                    //大于目标缩放才播放
                    if (gridParam.scaleY > newScale)
                    {
                        gridParam.animTime += pTimeDelta;
                        if (gridParam.animTime >= gridParam.refreshAnimTime)
                        {
                            shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, newScale, GridSize.z));
                            gridParam.animTime = 0;
                            gridParam.scaleY = newScale;
                            needRefresh = true;
                        }
                    }
                }
                else
                {
                    gridParam.animTime += pTimeDelta;
                    if (gridParam.animTime >= gridParam.refreshAnimTime)
                    {
                        float newScale = gridParam.scaleY == gridParam.animScale.x ? gridParam.animScale.y : gridParam.animScale.x;
                        shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, newScale, GridSize.z));
                        gridParam.animTime = 0;
                        gridParam.scaleY = newScale;
                        needRefresh = true;
                    }
                }
            }

            if (needRefresh)
            {
                //更新数据
                allMatricesBuffer.SetData(shaderArgs);
                GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
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

            //更新数据
            allMatricesBuffer.SetData(shaderArgs);
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
        }

        private void ChangeGridCamp(Vector2Int pGridPos, int pCamp, int pScaleIndex)
        {
            if (!gridIndexMap.ContainsKey(pGridPos.x) || !gridIndexMap[pGridPos.x].ContainsKey(pGridPos.y))
            {
                return;
            }

            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.camp = pCamp;

            //设置动画数据
            SetBaseCampGridAnimCfg(pGridPos, pScaleIndex);

            //阵营空
            if (pCamp == 0)
            {
                animIndexList.Remove(index);
            }
            else
            {
                animIndexList.Add(index);
            }

            //更新缩放和颜色
            shaderArgs[index].color = TempConfig.CampColorDict[pCamp];
        }

        /// <summary>
        /// 当玩家穿过阵营区域时
        /// </summary>
        public void OnGamerThroughCampArea(int pCamp, Vector2Int pGridPos)
        {
            int tIndex = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam tGridParam = gridDataDict[tIndex];
            if (tGridParam.camp == 0)
                return;

            //自己阵营
            if (tGridParam.camp == pCamp)
            {
                tGridParam.isLockByOtherGamer = false;
            }
            else
            {
                tGridParam.isLockByOtherGamer = true;
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
                    int index = gridIndexMap[item.x][item.y];
                    GridParam gridParam = gridDataDict[index];
                    if (gridParam.isLockByOtherGamer)
                    {
                        SetBaseCampGridAnimCfg(item, gridParam.animGridIndex);
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
                    if (gridIndexMap.ContainsKey(x) && gridIndexMap[x].ContainsKey(y))
                    {
                        int gridIndex = gridIndexMap[x][y];
                        GridParam gridParam = gridDataDict[gridIndex];
                        if (gridParam.camp != 0 && !gridParam.isThroughing && !gridParam.isLockByOtherGamer)
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

            if (needRefresh)
            {
                //更新数据
                allMatricesBuffer.SetData(shaderArgs);
                GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
            }
        }

        //设置基础阵营动画配置
        private void SetBaseCampGridAnimCfg(Vector2Int pGridPos, int pAnimIndex)
        {
            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.animGridIndex = pAnimIndex;
            gridParam.animTime = 0;
            gridParam.refreshAnimTime = GridAnimTime;
            gridParam.animScale = GridCampScales[pAnimIndex];
            gridParam.scaleY = gridParam.animScale.x;
            gridParam.isLockByOtherGamer = false;

            //更新缩放
            shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, gridParam.animScale.x, GridSize.z));
        }

        //设置玩家穿过阵营动画配置
        private void SetThroughCampGridAnimCfg(Vector2Int pGridPos, int pAnimIndex)
        {
            float targetScale = GamerThroughCampScales[pAnimIndex];

            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.animTime = 0;
            gridParam.refreshAnimTime = GamerThroughAnimTime;
            gridParam.animScale = new Vector2(targetScale, targetScale);
            gridParam.isThroughing = true;
        }

        //被其他不是该阵营的玩家经过锁住的动画配置
        private void SetLockByOtherGamerAnimCfg(Vector2Int pGridPos)
        {
            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.animTime = 0;
            gridParam.refreshAnimTime = 0;
            gridParam.animScale = Vector2.zero;
            gridParam.scaleY = gridParam.animScale.x;

            //更新缩放
            shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, gridParam.animScale.x, GridSize.z));
        }
    } 
}
