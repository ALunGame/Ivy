using Game;
using System.Collections.Generic;
using UnityEngine;
using InteropServices = System.Runtime.InteropServices;

namespace Gameplay
{
    public class MapGridGPUInstance : MonoBehaviour
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
            public int index;
            public Vector3 position;
            public int camp;        //阵营
            public float animTime;
        }

        [Header("地图大小")]
        public Vector2Int MapSize;
        [Header("格子动画时长")]
        public float GridAnimTime = 0.5f;
        [Header("格子网格")]
        public Mesh GridMesh;
        [Header("格子材质")]
        public Material GridMaterial;
        [Header("格子最小包围盒")]
        public Vector3 GridBoundMin;
        [Header("格子最大包围盒")]
        public Vector3 GridBoundMax;
        [Header("视锥剔除ComputeShader")]
        public ComputeShader CullingComputeShader;

        public int subMeshIndex = 0;
        private int instanceCount;
        private int cachedInstanceCount = -1;
        private int cachedSubMeshIndex = -1;
        //ComputeShader中内核函数索引
        private int kernel = 0;
        //物体的复合变换矩阵Buffer
        private ComputeBuffer allMatricesBuffer;
        //当前可见物体的instanceID Buffer   
        private ComputeBuffer visibleIDsBuffer;
        private ComputeBuffer argsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        //相机的视锥平面
        private Plane[] cameraFrustumPlanes = new Plane[6];
        //传入ComputeShader的视锥平面  
        private Vector4[] frustumPlanes = new Vector4[6];

        //每个格子的位置和缩放
        private InstanceParam[] shaderArgs;
        private Dictionary<int, Dictionary<int, int>> gridIndexMap = new Dictionary<int, Dictionary<int, int>>();
        private Dictionary<int, GridParam> gridDataDict = new Dictionary<int, GridParam>();
        private HashSet<int> animIndexList = new HashSet<int>();
        private bool needRefresh = false;
        private Bounds renderBounds = new Bounds();

        private void Start()
        {
            kernel = CullingComputeShader.FindKernel("CSMain");
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                CreateMap(new Vector2Int(10,10));
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                ChangeGridCamp(new Vector2Int(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y)), Random.Range(1, 3));
            }

            if (visibleIDsBuffer == null)
            {
                return;
            }

            RefreshBuffer();

            // 动画
            PlayAnim(Time.deltaTime);

            //// 视锥剔除
            //GeometryUtility.CalculateFrustumPlanes(Camera.main, cameraFrustumPlanes);
            //for (int i = 0; i < cameraFrustumPlanes.Length; i++)
            //{
            //    var normal = -cameraFrustumPlanes[i].normal;
            //    frustumPlanes[i] = new Vector4(normal.x, normal.y, normal.z, -cameraFrustumPlanes[i].distance);
            //}

            //visibleIDsBuffer.SetCounterValue(0);
            //CullingComputeShader.SetVectorArray("_FrustumPlanes", frustumPlanes);
            //CullingComputeShader.Dispatch(kernel, Mathf.CeilToInt(instanceCount / 640f), 1, 1);
            //ComputeBuffer.CopyCount(visibleIDsBuffer, argsBuffer, sizeof(uint));

            // 渲染
            Graphics.DrawMeshInstancedIndirect(GridMesh, subMeshIndex, GridMaterial, renderBounds, argsBuffer);
        }

        private void OnDrawGizmosSelected()
        {
            IAToolkit.GizmosHelper.DrawBounds(renderBounds, Color.blue);
        }

        private void OnDisable()
        {
            allMatricesBuffer?.Release();
            allMatricesBuffer = null;

            visibleIDsBuffer?.Release();
            visibleIDsBuffer = null;

            argsBuffer?.Release();
            argsBuffer = null;
        }

        private void PlayAnim(float pTimeDelta)
        {
            if (allMatricesBuffer == null || animIndexList == null)
            {
                return;
            }

            needRefresh = false;
            foreach (int index in animIndexList)
            {
                GridParam gridParam = gridDataDict[index];
                gridParam.animTime += pTimeDelta;
                if (gridParam.animTime >= GridAnimTime)
                {
                    float newScale = Random.Range(1.0f, 3.0f);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(gridParam.position, Quaternion.identity, new Vector3(1, newScale, 1));
                    needRefresh = true;

                    gridParam.animTime = 0;
                }
            }

            if (needRefresh)
            {
                //更新数据
                allMatricesBuffer.SetData(shaderArgs);
                GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
                CullingComputeShader.SetBuffer(kernel, "_AllMatricesBuffer", allMatricesBuffer);
            }
        }

        private void RefreshBuffer()
        {
            //if (!needRefresh)
            //    return;

            //needRefresh = false;
            ////更新数据
            //allMatricesBuffer.SetData(shaderArgs);
            //GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
            //CullingComputeShader.SetBuffer(kernel, "_AllMatricesBuffer", allMatricesBuffer);
        }

        public void CreateMap(Vector2Int pMapSize)
        {
            MapSize = pMapSize;

            animIndexList.Clear();
            gridIndexMap.Clear();
            gridDataDict.Clear();

            instanceCount = MapSize.x * MapSize.y;
            shaderArgs = new InstanceParam[instanceCount];

            //renderBounds.min = transform.position;
            //renderBounds.max = new Vector3(MapSize.x, 3, MapSize.y);
            renderBounds = new Bounds(Vector3.zero, new Vector3(200.0f, 200.0f, 200.0f));

            InitMapGrid();
        }

        private void InitMapGrid()
        {
            // 规范subMeshIndex
            if (GridMesh != null)
                subMeshIndex = Mathf.Clamp(subMeshIndex, 0, GridMesh.subMeshCount - 1);

            //复合变换矩阵
            int index = 0;
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    var position = transform.position + new Vector3(x, 0, y);
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

            Debug.Log($"InitMapGrid:{MapSize}-->{index}");

            //设置数据
            allMatricesBuffer?.Release();
            int stride = InteropServices.Marshal.SizeOf(typeof(InstanceParam));
            allMatricesBuffer = new ComputeBuffer(instanceCount, stride);
            allMatricesBuffer.SetData(shaderArgs);

            //设置到Shader
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);

            // 可见实例 Buffer
            visibleIDsBuffer?.Release();
            visibleIDsBuffer = new ComputeBuffer(instanceCount, sizeof(uint), ComputeBufferType.Append);
            GridMaterial.SetBuffer("_VisibleIDsBuffer", visibleIDsBuffer);

            // Indirect args
            argsBuffer?.Release();
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            if (GridMesh != null)
            {
                args[0] = GridMesh.GetIndexCount(subMeshIndex);
                args[1] = (uint)instanceCount;
                args[2] = GridMesh.GetIndexStart(subMeshIndex);
                args[3] = GridMesh.GetBaseVertex(subMeshIndex);
            }
            else
            {
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            argsBuffer.SetData(args);

            // ComputeShader
            CullingComputeShader.SetVector("_BoundMin", GridBoundMin);
            CullingComputeShader.SetVector("_BoundMax", GridBoundMax);
            CullingComputeShader.SetBuffer(kernel, "_AllMatricesBuffer", allMatricesBuffer);
            CullingComputeShader.SetBuffer(kernel, "_VisibleIDsBuffer", visibleIDsBuffer);
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

            needRefresh = true;
            //更新数据
            allMatricesBuffer.SetData(shaderArgs);
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
            CullingComputeShader.SetBuffer(kernel, "_AllMatricesBuffer", allMatricesBuffer);

            Debug.Log($"ChangeGridCamp:{pGridPos}-->{pCamp}");
        }
    }
}
