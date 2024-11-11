using Game;
using System.Collections.Generic;
using UnityEngine;
using InteropServices = System.Runtime.InteropServices;

namespace Gameplay
{
    public class MapGrids : MonoBehaviour
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
            public int index;               //索引
            public Vector3 position;        //位置
            public int camp;                //阵营
            public float animTime;          //动画时间
        }

        [Header("地图大小")]
        public Vector2Int MapSize;
        [Header("格子动画时长")]
        public float GridAnimTime = 0.5f;
        [Header("格子网格")]
        public Mesh GridMesh;
        [Header("格子材质")]
        public Material GridMaterial;
        [Header("格子大小")]
        public Vector3 GridSize;

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

        private void PlayAnim(float pTimeDelta)
        {
            bool needRefresh = false;
            foreach (int index in animIndexList)
            {
                GridParam gridParam = gridDataDict[index];
                gridParam.animTime += pTimeDelta;
                if (gridParam.animTime >= GridAnimTime)
                {
                    float newScale = Random.Range(1.0f, 3.0f);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(1, newScale, 1));
                    gridParam.animTime = 0;
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
                    var position = transform.position + new Vector3(x, 0, y) + GridSize;
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 0.1f, 1));
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

        public void ChangeGridCamp(Vector2Int pGridPos, int pCamp)
        {
            if (!gridIndexMap.ContainsKey(pGridPos.x) || !gridIndexMap[pGridPos.x].ContainsKey(pGridPos.y))
            {
                return;
            }

            int index = gridIndexMap[pGridPos.x][pGridPos.y];
            GridParam gridParam = gridDataDict[index];
            gridParam.camp = pCamp;
            gridParam.animTime = 0;

            //阵营空
            if (pCamp == 0)
            {
                animIndexList.Remove(index);

                shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(1, 0.1f, 1));
                shaderArgs[index].color = TempConfig.CampColorDict[0];
            }
            else
            {
                animIndexList.Add(index);

                shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(1, 1, 1));
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

            // 动画
            PlayAnim(Time.deltaTime);

            // 渲染
            Graphics.DrawMeshInstancedIndirect(GridMesh, 0, GridMaterial, renderBounds, argsBuffer);
        }
    } 
}
