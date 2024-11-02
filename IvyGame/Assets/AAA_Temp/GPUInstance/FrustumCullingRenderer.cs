using UnityEngine;
using Random = UnityEngine.Random;

namespace Pamisu
{
    public class FrustumCullingRenderer : MonoBehaviour
    {
        public int instanceCount = 100000;
        public Mesh instanceMesh;
        public Material instanceMaterial;
        public int subMeshIndex = 0;
        public Vector3 objectBoundMin;
        public Vector3 objectBoundMax;
        public ComputeShader cullingComputeShader;

        int cachedInstanceCount = -1;
        int cachedSubMeshIndex = -1;
        int kernel = 0;
        ComputeBuffer allMatricesBuffer;
        ComputeBuffer visibleIDsBuffer;
        ComputeBuffer argsBuffer;
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        Plane[] cameraFrustumPlanes = new Plane[6];
        Vector4[] frustumPlanes = new Vector4[6];

        void Start()
        {
            kernel = cullingComputeShader.FindKernel("CSMain");
        }

        void Update()
        {
            // 更新Buffer
            UpdateBuffers();

            // 方向键改变绘制数量
            if (Input.GetAxisRaw("Horizontal") != 0.0f)
                instanceCount = (int)Mathf.Clamp(instanceCount + Input.GetAxis("Horizontal") * 40000, 1.0f, 5000000.0f);

            // 视锥剔除
            GeometryUtility.CalculateFrustumPlanes(Camera.main, cameraFrustumPlanes);
            for (int i = 0; i < cameraFrustumPlanes.Length; i++)
            {
                var normal = -cameraFrustumPlanes[i].normal;
                frustumPlanes[i] = new Vector4(normal.x, normal.y, normal.z, -cameraFrustumPlanes[i].distance);
            }

            visibleIDsBuffer.SetCounterValue(0);
            cullingComputeShader.SetVectorArray("_FrustumPlanes", frustumPlanes);
            cullingComputeShader.Dispatch(kernel, Mathf.CeilToInt(instanceCount / 640f), 1, 1);
            ComputeBuffer.CopyCount(visibleIDsBuffer, argsBuffer, sizeof(uint));

            // 渲染
            Bounds renderBounds = new Bounds(Vector3.zero, new Vector3(200.0f, 200.0f, 200.0f));
            Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, renderBounds, argsBuffer);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + instanceCount.ToString());
            instanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), instanceCount, 1.0f, 5000000.0f);
        }

        void UpdateBuffers()
        {
            // 不需要更新时返回
            if ((cachedInstanceCount == instanceCount || cachedSubMeshIndex != subMeshIndex)
                && argsBuffer != null)
            {
                return;
            }

            // 规范subMeshIndex
            if (instanceMesh != null)
                subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);

            // 复合变换矩阵
            allMatricesBuffer?.Release();
            allMatricesBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 16);   // float4x4
            Matrix4x4[] trs = new Matrix4x4[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                // 随机位置
                float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
                float distance = Random.Range(8.0f, 90.0f);
                float height = Random.Range(-5.0f, 5.0f);
                float size = Random.Range(0.05f, 1f);
                var position = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
                trs[i] = Matrix4x4.TRS(position, Random.rotationUniform, new Vector3(size, size, size));
            }
            allMatricesBuffer.SetData(trs);
            instanceMaterial.SetBuffer("_AllMatricesBuffer", allMatricesBuffer);

            // 可见实例 Buffer
            visibleIDsBuffer?.Release();
            visibleIDsBuffer = new ComputeBuffer(instanceCount, sizeof(uint), ComputeBufferType.Append);
            instanceMaterial.SetBuffer("_VisibleIDsBuffer", visibleIDsBuffer);

            // Indirect args
            argsBuffer?.Release();
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            if (instanceMesh != null)
            {
                args[0] = instanceMesh.GetIndexCount(subMeshIndex);
                args[1] = (uint)instanceCount;
                args[2] = instanceMesh.GetIndexStart(subMeshIndex);
                args[3] = instanceMesh.GetBaseVertex(subMeshIndex);
            }
            else
            {
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            argsBuffer.SetData(args);

            // ComputeShader
            cullingComputeShader.SetVector("_BoundMin", objectBoundMin);
            cullingComputeShader.SetVector("_BoundMax", objectBoundMax);
            cullingComputeShader.SetBuffer(kernel, "_AllMatricesBuffer", allMatricesBuffer);
            cullingComputeShader.SetBuffer(kernel, "_VisibleIDsBuffer", visibleIDsBuffer);

            cachedInstanceCount = instanceCount;
            cachedSubMeshIndex = subMeshIndex;
        }

        void OnDisable()
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