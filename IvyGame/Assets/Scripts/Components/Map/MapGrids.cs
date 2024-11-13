using Game;
using IAEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using InteropServices = System.Runtime.InteropServices;

namespace Gameplay
{
    public class MapGrids : MonoBehaviour
    {
        //网格动画配置
        [Serializable]
        public class GridAnimCfg
        {
            public float minScaleY;
            public float maxScaleY;
            public int weight;
        }

        //实例化参数
        struct InstanceParam
        {
            public Color color;
            public Matrix4x4 instanceToObjectMatrix;        //实例化到物方矩阵
        }

        //逻辑数据
        class GridParam
        {
            public int index;               //索引
            public Vector3 position;        //位置
            public int camp;                //阵营
            public float currScaleY;        //当前的Y轴缩放

            public bool inPassed;           //正在经过
            public float passedScaleY;       //经过缩放

            public int animCfgIndex;        //动画配置Index
            public float animTime;          //动画时间
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
        [Header("格子动画配置")]
        public List<GridAnimCfg> GridAnimCfgs = new List<GridAnimCfg>();

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
            IAToolkit.GizmosHelper.DrawBounds(renderBounds, Color.blue);
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

        public void ChangeGridsCamp(List<Vector2Int> pGridPosList, int pCamp, int pAnimCfgIndex = -1)
        {
            int resAnimCfgIndex = -1;
            float resScaleY = 0.1f;
            if (pCamp == 0) 
            {
                resAnimCfgIndex = -1;
                resScaleY = 0.1f;
            }
            else
            {
                GridAnimCfg resCfg = null;
                if (pAnimCfgIndex != -1)
                {
                    resCfg = GridAnimCfgs[pAnimCfgIndex];
                }
                else
                {
                    List<int> weightIndexs = new List<int>();
                    for (int i = 0; i < GridAnimCfgs.Count; i++)
                    {
                        weightIndexs.Add(GridAnimCfgs[i].weight);
                    }
                    if (!RandomHelper.GetRandomValueByWeight(GridAnimCfgs, weightIndexs, out resAnimCfgIndex, out resCfg))
                    {
                        resAnimCfgIndex = UnityEngine.Random.Range(0, GridAnimCfgs.Count - 1);
                        resCfg = GridAnimCfgs[resAnimCfgIndex];
                    }
                }

                resScaleY = resCfg.minScaleY;
            }

                
            for (int i = 0; i < pGridPosList.Count; i++)
            {
                ChangeGridCamp(pGridPosList[i], pCamp, resAnimCfgIndex, resScaleY);
            }
        }

        private void ChangeGridCamp(Vector2Int pGridPos, int pCamp, int pAnimCfgIndex, float pChangeScaleY)
        {
            if (!gridIndexMap.ContainsKey(pGridPos.x) || !gridIndexMap[pGridPos.x].ContainsKey(pGridPos.y))
            {
                return;
            }

            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.camp = pCamp;
            gridParam.animTime = 0;
            gridParam.animCfgIndex = pAnimCfgIndex;
            gridParam.inPassed = false;

            //阵营空
            if (pCamp == 0)
            {
                animIndexList.Remove(index);

                shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, pChangeScaleY, GridSize.z));
                shaderArgs[index].color = TempConfig.CampColorDict[0];
            }
            else
            {
                animIndexList.Add(index);

                shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, pChangeScaleY, GridSize.z));
                shaderArgs[index].color = TempConfig.CampColorDict[pCamp];
            }

            //更新数据
            allMatricesBuffer.SetData(shaderArgs);
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
        }

        public void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (allMatricesBuffer == null)
            {
                return;
            }

            //动画
            PlayGridAnim(Time.deltaTime);

            //渲染
            Graphics.DrawMeshInstancedIndirect(GridMesh, 0, GridMaterial, renderBounds, argsBuffer);
        }

        //获得需要播放的缩放
        public float GetPlayAnimScaleY(float pCurrScaleY, int pAnimIndex)
        {
            GridAnimCfg animCfg = GridAnimCfgs[pAnimIndex];
            return animCfg.minScaleY == pCurrScaleY ? animCfg.maxScaleY : animCfg.minScaleY;
        }

        //播放格子缩放动画
        private void PlayGridAnim(float pTimeDelta)
        {
            bool needRefresh = false;
            foreach (int index in animIndexList)
            {
                GridParam gridParam = gridDataDict[index];
                gridParam.animTime += pTimeDelta;
                if (gridParam.animTime >= GridAnimTime)
                {
                    float newScale = GetPlayAnimScaleY(gridParam.currScaleY, gridParam.animCfgIndex);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, newScale, GridSize.z));
                    gridParam.animTime = 0;
                    gridParam.currScaleY = newScale;
                    needRefresh = true;
                }
            }

            if (needRefresh)
            {
                //更新数据
                allMatricesBuffer.SetData(shaderArgs);
                GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
            }
        }

        private Dictionary<string, List<int>> gamerPassedGridDict = new Dictionary<string, List<int>>();
        private Vector2Int gamerPassSize = new Vector2Int(4, 4);
        private List<float> gamerPassDisGridSize = new List<float>()
        {
            0.5f,
            1.0f,
            1.5f,
            2.0f,
            3.0f,
        };
        public void OnGamerGridPosChange(string pGamerUid, Vector2Int pGridPos)
        {
            if (gamerPassedGridDict.ContainsKey(pGamerUid))
            {
                List<int> gridIndexList = gamerPassedGridDict[pGamerUid];
                foreach (var index in gridIndexList)
                {
                    GridParam gridParam = gridDataDict[index];
                    gridParam.inPassed = false;
                }
                gridIndexList.Clear();
            }

            int currIndex = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam currGridParam = gridDataDict[currIndex];
            if (currGridParam.camp == 0)
            {
                return;
            }

            bool needRefresh = false;

            Vector2Int leftDownPos = pGridPos - (gamerPassSize / 2);
            for (int x = leftDownPos.x; x <= leftDownPos.x + gamerPassSize.x; x++)
            {
                for (int y = leftDownPos.y; y <= leftDownPos.y + gamerPassSize.y; y++)
                {
                    if (gridIndexMap.ContainsKey(x) && gridIndexMap[x].ContainsKey(y))
                    {
                        int gridIndex = gridIndexMap[x][y];
                        GridParam gridParam = gridDataDict[gridIndex];
                        if (gridParam.camp != 0)
                        {
                            gridParam.inPassed = true;
                            int xDis = Mathf.Abs(pGridPos.x - x);
                            int yDis = Mathf.Abs(pGridPos.y - y);
                            int resDis = xDis > yDis ? xDis : yDis;
                            float scaleY = gamerPassDisGridSize[resDis];
                            gridParam.passedScaleY = scaleY;

                            needRefresh = true;
                            //更新显示
                            shaderArgs[gridIndex].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(GridSize.x, scaleY, GridSize.z));

                            if (!gamerPassedGridDict.ContainsKey(pGamerUid))
                                gamerPassedGridDict.Add(pGamerUid, new List<int>());
                            gamerPassedGridDict[pGamerUid].Add(gridIndex);
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
    } 
}
