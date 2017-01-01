using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mesh : MonoBehaviour {
	Mesh Mesh0;
	MeshCollider MeshCollider1;//组成必需collider
	MeshFilter MeshFilter1;//filter
	Vector3[] vertices;//顶点
	Vector3[] velocity;//速度

	GameObject Cube1;

	void Start () {
		
		MeshCollider1 = GetComponent<MeshCollider> ();//获取组件
		MeshFilter1 = GetComponent<MeshFilter> ();
		Mesh0 = MeshFilter1.mesh;

		GameObject Cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);//创建cube0储存信息
		Mesh Mesh1 = Cube0.GetComponent<MeshFilter>().mesh;
		CreateMesh.vertices = Mesh1.vertices;
		CreateMesh.uvs = Mesh1.uv;
		CreateMesh.indices = Mesh1.GetIndices(0);
		CreateMesh.triangles = Mesh1.triangles;
		Destroy(Cube0);//销毁cube0 

		vertices = CreateMesh.Vertices0;
		velocity = new Vector3[vertices.Length];

		for (int i = 0; i<vertices.Length; i++)//速度初始化
		{
			velocity[i] = Vector3.zero;
		}
	}

	void Update()
	{
		
		Shapematching.Instance.ShapeMatchingDynamics(ref vertices,ref velocity);//调用shapematching
		Mesh0.vertices = vertices;//获取顶点速度
		Mesh0.uv = CreateMesh.uvs;
		Mesh0.triangles = CreateMesh.triangles;
		Mesh0.RecalculateNormals();
		MeshCollider1.sharedMesh = Mesh0;
	}
}

struct CreateMesh//函数CubeMesh
{
	public static Vector2[] uvs;
	public static int[] indices;
	public static int[] triangles;
	public static Vector3[] vertices;
	public static Vector3[] Vertices0 = new Vector3[] 
	{
		new Vector3 (8f, -8f, 8f),
		new Vector3 (-8f, -8f, 8f),
		new Vector3 (5f, 5f, 5f),
		new Vector3 (-5f, 5f, 5f),

		new Vector3 (7f, 7f, -7f),
		new Vector3 (-5f, 5f, -5f),
		new Vector3 (5f, -5f, -5f),
		new Vector3 (-6f, -6f, -6f),

		new Vector3 (5f, 5f, 5f),
		new Vector3 (-5f, 5f, 5f),
		new Vector3 (7f, 7f, -7f),
		new Vector3 (-5f, 5f, -5f),

		new Vector3 (5f, -5f, -5f),
		new Vector3 (8f, -8f, 8f),
		new Vector3 (-8f, -8f, 8f),
		new Vector3 (-6f, -6f, -6f),

		new Vector3 (-8f, -8f, 8f),
		new Vector3 (-5f, 5f, 5f),
		new Vector3 (-5f, 5f, -5f),
		new Vector3 (-6f, -6f, -6f),

		new Vector3 (5f, -5f, -5f),
		new Vector3 (7f, 7f, -7f),
		new Vector3 (5f, 5f, 5f),
		new Vector3 (8f, -8f, 8f),
	};
}