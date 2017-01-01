using UnityEngine;
using System.Collections;

public class Shapematching : MonoBehaviour {
	Matrix4x4 p;
	[Range(0,0.1f)]
	public float hard = 0.02f;//硬度系数
	[Range(0, 1.0f)]
	public float damp =0.90f;//渐衰速度
	public Vector3 force = Vector3.zero;

	private static Shapematching instance;
	public static Shapematching Instance
	{
		get
		{
			if (instance == null)
			{
				Debug.LogError("Shapematching instance does not exist");
			}
			return instance;
		}
	}
	void Awake()
	{
		instance = this;
		p = Matrix4x4.identity;
	}

	public void ShapeMatchingDynamics(ref Vector3[] vertices,ref Vector3[] velocity)
	{
		//计算重心（公式11.6）质量设为1
		Vector3 c0, c;
		Vector3 sum0 = Vector3.zero;
		Vector3 sum = Vector3.zero;
		for(int i = 0; i < vertices.Length; i++)
		{
			sum0 += CreateMesh.vertices[i];
			sum += vertices[i];
		}
		c0 = sum0/vertices.Length;
		c = sum/vertices.Length;

		//计算moment matrix=Apq（11.9）
		Matrix4x4 Apq = Matrix4x4.zero;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector4 Qi = CreateMesh.vertices[i] - c0; //相对位置
			Vector4 Pi = vertices[i] - c;
			Matrix4x4 Qm = new Matrix4x4();//变成矩阵
			Matrix4x4 Pm = new Matrix4x4();
			Pm.SetColumn(0, Pi);//列
			Qm.SetRow(0, Qi);//行
			Apq = MatrixAdd(Apq, Pm*Qm);//调用MatrixAdd
		}
		//计算矩阵 P&D
		Matrix4x4 d = Apq.transpose * Apq;
		Jacobi(ref p, ref d, 12, 0.01f);//调用Jacobi函数
		//计算矩阵 S
		Matrix4x4 s = p * MatrixSqrt(d) * p.transpose;//调用MatrixSqrt
		//计算矩阵 R
		Matrix4x4 r = Apq * s.inverse;//逆矩阵
		//计算goal position
		Vector3[] goalPosition = new Vector3[vertices.Length];
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 Qi = CreateMesh.vertices[i] - c0;
			goalPosition[i] = r.MultiplyVector(Qi) + c;
		}
		//更新顶点和速度
		for (int i = 0; i < vertices.Length; i++)
		{
			velocity[i] += hard / Time.deltaTime * (goalPosition[i] - vertices[i]) + Time.deltaTime * force;
			vertices[i] += Time.deltaTime * velocity[i];
			velocity[i] *= damp;
		}
	}

	//Jacobi正交矩阵对角化
	void Jacobi(ref Matrix4x4 p, ref Matrix4x4 m,int maxNum,float thresh)
	{
		if (!p.isIdentity)
		{
			m = p.transpose*m*p;
		}

		for(int i = 0; i < maxNum; i++)
		{
			Matrix4x4 r = Matrix4x4.identity; 

			if (m.m01 >= m.m02 && m.m01 >= m.m12)
			{
				if (Mathf.Abs(m.m01) < thresh) return;
				float theta = Mathf.Atan2(-2 * m.m01, m.m00 - m.m11)/2;
				r.m00 = Mathf.Cos(theta);
				r.m01 = Mathf.Sin(theta);
				r.m10 = -Mathf.Sin(theta);
				r.m11 = Mathf.Cos(theta);
			}
			else if (m.m02 >= m.m01 && m.m02 >= m.m12)
			{
				if (Mathf.Abs(m.m02) < thresh) return;
				float theta = Mathf.Atan2(-2 * m.m02, m.m00 - m.m22)/2;
				r.m00 = Mathf.Cos(theta);
				r.m02 = Mathf.Sin(theta);
				r.m20 = -Mathf.Sin(theta);
				r.m22 = Mathf.Cos(theta);
			}
			else if (m.m12 >= m.m01 && m.m12 >= m.m02)
			{
				if (Mathf.Abs(m.m12) < thresh) return;
				float theta = Mathf.Atan2(-2 * m.m12, m.m11 - m.m22)/2;
				r.m11 = Mathf.Cos(theta);
				r.m12 = Mathf.Sin(theta);
				r.m21 = -Mathf.Sin(theta);
				r.m22 = Mathf.Cos(theta);
			}

			m = r.transpose * (m * r);
			p = p * r;
		}
	}
	//矩阵加法
	Matrix4x4 MatrixAdd(Matrix4x4 a,Matrix4x4 b)
	{
		Matrix4x4 c = Matrix4x4.zero;
		c.SetColumn(0, a.GetColumn(0) + b.GetColumn(0));
		c.SetColumn(1, a.GetColumn(1) + b.GetColumn(1));
		c.SetColumn(2, a.GetColumn(2) + b.GetColumn(2));
		return c;
	}
	//矩阵乘法
	Matrix4x4 MatrixSqrt(Matrix4x4 m)
	{
		Matrix4x4 t=Matrix4x4.zero;
		t.m00 = Mathf.Sqrt(m.m00);
		t.m11 = Mathf.Sqrt(m.m11);
		t.m22 = Mathf.Sqrt(m.m22);
		t.m33 = 1;
		return t;
	}
}
