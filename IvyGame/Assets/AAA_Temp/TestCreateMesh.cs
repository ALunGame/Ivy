using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestCreateMesh : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnEnable()
    {
        //初始化
        Mesh mesh = new Mesh();
        MeshFilter meshfilter = GetComponent<MeshFilter>();
        meshfilter.mesh = mesh;
        //获取顶点坐标和三角序列
        mesh.vertices = GetVertices();
        mesh.triangles = GetTriangles();
    }

    private Vector3[] GetVertices()
    {//赋值顶点坐标
        return new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0),


            new Vector3(0,0,1),
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,1,1),
        };
    }
    private int[] GetTriangles()
    {//赋值三角序列
        return new int[]
        {
            0,1,2,
            0,2,3,
        };
    }
}
