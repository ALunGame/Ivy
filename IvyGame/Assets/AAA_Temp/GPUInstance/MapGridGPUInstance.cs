using Unity.VisualScripting;
using UnityEngine;

namespace Test
{
    //实例化参数
    public struct InstanceParam
    {
        public UnityEngine.Color color;
        public Matrix4x4 instanceToObjectMatrix;        //实例化到物方矩阵
    }

    public class MapGridGPUInstance : MonoBehaviour
    {
        [Header("地图大小")]
        public Vector2Int MapSize;
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

        private void Start()
        {
            kernel = CullingComputeShader.FindKernel("CSMain");
            instanceCount = MapSize.x * MapSize.y;
            shaderArgs = new InstanceParam[instanceCount];

            int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(InstanceParam));
            allMatricesBuffer = new ComputeBuffer(instanceCount, stride);
        }

        private void Update()
        {
            // 更新Buffer
            UpdateBuffers();

            TestAnim();

            // 视锥剔除
            GeometryUtility.CalculateFrustumPlanes(Camera.main, cameraFrustumPlanes);
            for (int i = 0; i < cameraFrustumPlanes.Length; i++)
            {
                var normal = -cameraFrustumPlanes[i].normal;
                frustumPlanes[i] = new Vector4(normal.x, normal.y, normal.z, -cameraFrustumPlanes[i].distance);
            }

            visibleIDsBuffer.SetCounterValue(0);
            CullingComputeShader.SetVectorArray("_FrustumPlanes", frustumPlanes);
            CullingComputeShader.Dispatch(kernel, Mathf.CeilToInt(instanceCount / 640f), 1, 1);
            ComputeBuffer.CopyCount(visibleIDsBuffer, argsBuffer, sizeof(uint));

            // 渲染
            Bounds renderBounds = new Bounds(Vector3.zero, new Vector3(200.0f, 200.0f, 200.0f));
            Graphics.DrawMeshInstancedIndirect(GridMesh, subMeshIndex, GridMaterial, renderBounds, argsBuffer);
        }

        private void UpdateBuffers()
        {
            // 不需要更新时返回
            if ((cachedInstanceCount == instanceCount) && argsBuffer != null)
            {
                return;
            }

            // 规范subMeshIndex
            if (GridMesh != null)
                subMeshIndex = Mathf.Clamp(subMeshIndex, 0, GridMesh.subMeshCount - 1);

            // 复合变换矩阵
            //allMatricesBuffer?.Release();
            //allMatricesBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 16);   // float4x4
            //Matrix4x4[] trs = new Matrix4x4[instanceCount];
            int index = 0;
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    float size = Random.Range(0.05f, 1f);
                    var position = transform.position + new Vector3(x, 0, y);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, size, 1));
                    shaderArgs[index].color = Random.ColorHSV();
                    index++;
                }
            }
            //for (int i = 0; i < instanceCount; i++)
            //{
            //    // 随机位置
            //    float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            //    float distance = Random.Range(8.0f, 90.0f);
            //    float height = Random.Range(-5.0f, 5.0f);
            //    float size = Random.Range(0.05f, 1f);
            //    var position = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
            //    trs[i] = Matrix4x4.TRS(position, Random.rotationUniform, new Vector3(size, size, size));
            //}
            allMatricesBuffer.SetData(shaderArgs);
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

            cachedInstanceCount = instanceCount;
            cachedSubMeshIndex = subMeshIndex;
        }

        private void TestAnim()
        {
            int index = 0;
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    float size = Random.Range(0.05f, 1f);
                    var position = transform.position + new Vector3(x, 0, y);
                    shaderArgs[index].instanceToObjectMatrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, size, 1));
                    shaderArgs[index].color = Random.ColorHSV();
                    index++;
                }
            }

            allMatricesBuffer.SetData(shaderArgs);
            GridMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);
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
    }
}
