using UnityEngine;
using System.Collections;

public enum GizmoDrawAxis
{
	X,
	Y,
	Z,
}

public static class IMGizmos
{
#if UNITY_EDITOR

	private class GizmoMesh
	{
		private string m_Path;
		private Mesh m_Mesh;

		public GizmoMesh(string meshPath)
		{
			m_Path = meshPath;
			m_Mesh = null;
		}

		public Mesh Mesh
		{
			get
			{
				if (m_Mesh == null)
				{
					m_Mesh = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath(m_Path, typeof(Mesh));

					if (m_Mesh == null)
					{
						Debug.LogError(string.Format("Mesh {0} not found", m_Path));
					}
				}

				return m_Mesh;
			}
		}
	}

	private static readonly Color			COLOR_XAXIS = new Color(1.0f, 0.25f, 0.25f, 1.0f);
	private static readonly Color			COLOR_YAXIS = new Color(0.25f, 1.0f, 0.25f, 1.0f);
	private static readonly Color			COLOR_ZAXIS = new Color(0.25f, 0.5f, 1.0f, 1.0f);

	private static readonly Quaternion		CAPSULE_BOTTOM_CAP_ROTATION = Quaternion.Euler(180f, 0f, 0f);

	private static readonly Vector3			_Vector3Zero = new Vector3(0f, 0f, 0f);
	private static readonly Vector3			_Vector3One = new Vector3(1f, 1f, 1f);

	private static GUIStyle					m_Style;
	private static GUIContent				m_GUIContent;

	private static Matrix4x4				m_PrevMatrix;
	private static Color					m_PrevColor;
	private static Matrix4x4				m_MatrixTemp = new Matrix4x4();

	private static Material					m_GizmoMaterial;
	private static Material					m_GizmoArcMaterial;
	private static int						m_ColorPropID;
	private static int						m_ZTestPropID;
	private static int						m_InnerRadiusPropID;
	private static int						m_DirAnglePropID;
	private static int						m_SectorAnglePropID;
	

	private static IMDrawZTest				m_ZTest = IMDrawZTest.LessEqual;

	private static float[]					m_Sine;
	private static float[]					m_Cosine;

	// Note: if you have moved IMDraw to a different path, you will need to update these strings
	private const string MESH_ASSET_PATH = "Assets/IMDraw/Mesh/";
	private const string IMGIZMOS_MATERIAL_PATH = "Assets/IMDraw/Material/IMGizmosMaterial.mat";
	private const string IMGIZMOS_ARC_MATERIAL_PATH = "Assets/IMDraw/Material/IMGizmosArcMaterial.mat";

	private static GizmoMesh m_MeshQuad = new GizmoMesh(MESH_ASSET_PATH + "IMDrawQuad.obj");
	private static GizmoMesh m_MeshBox = new GizmoMesh(MESH_ASSET_PATH + "IMDrawBox.obj");
	private static GizmoMesh m_MeshPyramid = new GizmoMesh(MESH_ASSET_PATH + "IMDrawPyramid.obj");
	private static GizmoMesh m_MeshRhombus = new GizmoMesh(MESH_ASSET_PATH + "IMDrawRhombus.obj");
	private static GizmoMesh m_MeshDisc = new GizmoMesh(MESH_ASSET_PATH + "IMDrawDisc.obj");
	private static GizmoMesh m_MeshSphere = new GizmoMesh(MESH_ASSET_PATH + "IMDrawSphere.obj");
	private static GizmoMesh m_MeshCone = new GizmoMesh(MESH_ASSET_PATH + "IMDrawCone.obj");
	private static GizmoMesh m_MeshCapsuleCap = new GizmoMesh(MESH_ASSET_PATH + "IMDrawCapsuleCap.obj");
	private static GizmoMesh m_MeshCapsuleBody = new GizmoMesh(MESH_ASSET_PATH + "IMDrawCapsuleBody.obj");
	private static GizmoMesh m_MeshCylinder = new GizmoMesh(MESH_ASSET_PATH + "IMDrawCylinder.obj");

	private static void InitLookupTable ()
	{
		if (m_Sine != null) // Skip if already initialised
			return;

		int step = 15;
		int pcount = (360 / step) + 1;

		int index = 0;

		float anglef;

		m_Sine = new float[pcount];
		m_Cosine = new float[pcount];

		for (int angle = 0; angle <= 360; angle += step)
		{
			anglef = (float)(angle - 90) * Mathf.Deg2Rad;

			m_Sine[index] = Mathf.Sin(anglef);
			m_Cosine[index] = Mathf.Cos(anglef);
			++index;
		}
	}

	private static void InitGUI ()
	{
		if (m_Style == null)
		{
			m_Style = new GUIStyle();
		}

		if (m_GUIContent == null)
		{
			m_GUIContent = new GUIContent();
		}
	}

	private static void SaveGizmosState()
	{
		m_PrevMatrix = Gizmos.matrix;
		m_PrevColor = Gizmos.color;
	}

	private static void RestoreGizmosState()
	{
		Gizmos.matrix = m_PrevMatrix;
		Gizmos.color = m_PrevColor;
	}
#endif // UNITY_EDITOR

	/// <summary>Set current draw mode to use default Z testing (LEqual). Only affects mesh primitives.</summary>
	public static void SetDefaultZTest()
	{
		#if UNITY_EDITOR
		m_ZTest = IMDrawZTest.LessEqual;
		#endif // UNITY_EDITOR
	}

	// <summary>Set/get current draw mode to use specified Z testing. Only affects mesh primitives.</summary>
	public static IMDrawZTest ZTest
	{
		#if UNITY_EDITOR
		set { m_ZTest = value; }
		get { return m_ZTest; }
		#else
		set {  }
		get { return IMDrawZTest.LessEqual; }
		#endif
	}

#if true

	private static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
	{
	#if UNITY_EDITOR
		if (m_GizmoMaterial == null)
		{
			m_GizmoMaterial = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(IMGIZMOS_MATERIAL_PATH, typeof(Material));

			if (m_GizmoMaterial == null)
			{
				Debug.LogError(string.Format("Material {0} not found", IMGIZMOS_MATERIAL_PATH));
			}

			m_ColorPropID = Shader.PropertyToID("_Color");
			m_ZTestPropID = Shader.PropertyToID("_ZTest");
		}

		if (m_GizmoMaterial != null)
		{
			m_GizmoMaterial.SetColor(m_ColorPropID, color);
			m_GizmoMaterial.SetInt(m_ZTestPropID, (int)m_ZTest);
			m_GizmoMaterial.SetPass(0);

			m_MatrixTemp.SetTRS(position, rotation, scale);
			Graphics.DrawMeshNow(mesh, m_MatrixTemp);
		}
	#endif // UNITY_EDITOR
	}
#else
	// Using Gizmos.DrawMesh doesn't give you any control over the shader, also the color comes out incorrect.
	private static void DrawMesh (Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawMesh(mesh, position, rotation, scale);
	}
#endif

	#region LINES

	/// <summary>
	/// Draw a 3D line.
	/// </summary>
	/// <param name="from">Starting point of the line.</param>
	/// <param name="to">Ending point of the line.</param>
	/// <param name="color">Line color.</param>
	public static void Line3D (Vector3 from, Vector3 to, Color color)
	{
#if UNITY_EDITOR
		Color prevColor = Gizmos.color;	
		Gizmos.color = color;
		Gizmos.DrawLine(from, to);
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="ray">Source ray.</param>
	/// <param name="length">Length of line.</param>
	/// <param name="color">Ray color.</param>
	public static void Ray3D (Ray ray, float length, Color color)
	{
#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * length);
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="origin">Origin of ray.</param>
	/// <param name="direction">Direction of ray (assumed to be normalised).</param>
	/// <param name="length">Length of line.</param>
	/// <param name="color">Ray color.</param>
	public static void Ray3D (Vector3 origin, Vector3 direction, float length, Color color)
	{
#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(origin, origin + direction * length);
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="origin">Origin of ray.</param>
	/// <param name="rotation">Ray orientation.</param>
	/// <param name="length">Ray length.</param>
	/// <param name="color">Ray color.</param>
	public static void Ray3D (Vector3 origin, Quaternion rotation, float length, Color color)
	{
#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(0f, 0f, length));
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	#endregion // LINES

	#region QUADS

	private static void DrawWireQuad ()
	{
#if UNITY_EDITOR
		Vector3 p1 = new Vector3(-0.5f, 0f, -0.5f);
		Vector3 p2 = new Vector3(0.5f, 0f, -0.5f);
		Vector3 p3 = new Vector3(0.5f, 0f, 0.5f);
		Vector3 p4 = new Vector3(-0.5f, 0f, 0.5f);

		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D quad.
	/// </summary>
	/// <param name="center">Quad center position.</param>
	/// <param name="rotation">Quad rotation.</param>
	/// <param name="sizeX">Width of quad.</param>
	/// <param name="sizeY">Height of quad.</param>
	/// <param name="axis">Orientation axis of quad.</param>
	/// <param name="color">Quad color.</param>
	public static void WireQuad3D(Vector3 center, Quaternion rotation, float sizeX, float sizeY, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(center, rotation, new Vector3(sizeX, 0f, sizeY));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireQuad();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D quad.
	/// </summary>
	/// <param name="center">Quad center position.</param>
	/// <param name="rotation">Quad rotation.</param>
	/// <param name="sizeX">Width of quad.</param>
	/// <param name="sizeY">Height of quad.</param>
	/// <param name="axis">Orientation axis of quad.</param>
	/// <param name="color">Quad color.</param>
	public static void Quad3D (Vector3 center, Quaternion rotation, float sizeX, float sizeY, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshQuad.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

			DrawMesh(mesh, center, rotation, new Vector3(sizeX, 0f, sizeY), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D quad.
	/// </summary>
	/// <param name="center">Quad center position.</param>
	/// <param name="rotation">Quad rotation.</param>
	/// <param name="sizeX">Width of quad.</param>
	/// <param name="sizeY">Height of quad.</param>
	/// <param name="axis">Orientation axis of quad.</param>
	/// <param name="solidColor">Quad fill color.</param>
	/// <param name="wireframeColor">Quad wireframe color.</param>
	public static void Quad3D(Vector3 center, Quaternion rotation, float sizeX, float sizeY, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR

		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		Mesh mesh = m_MeshQuad.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, new Vector3(sizeX, 0f, sizeY), solidColor);
		}

		m_MatrixTemp.SetTRS(center, rotation, new Vector3(sizeX, 0f, sizeY));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireQuad();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // QUAD

	#region BOXES

	/// <summary>
	/// Draw a wireframe 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="rotation">Box orientation.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	public static void WireBox3D (Vector3 center, Quaternion rotation, Vector3 size, Color color)
	{
#if UNITY_EDITOR

		SaveGizmosState();

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		Gizmos.DrawWireCube(_Vector3Zero, size);

		RestoreGizmosState();

#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="rotation">Box orientation.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	public static void Box3D (Vector3 center, Quaternion rotation, Vector3 size, Color color)
    {
#if UNITY_EDITOR

		Mesh mesh = m_MeshBox.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation,size, color);
		}

		/*
		SaveGizmosState();

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		Gizmos.DrawCube(_Vector3Zero, size);

		//DrawMesh(mesh, center, rotation, size, solidColor);

		RestoreGizmosState();
		*/
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="rotation">Box orientation.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="solidColor">Box fill color.</param>
	/// <param name="wireframeColor">Box wireframe color.</param>
	public static void Box3D (Vector3 center, Quaternion rotation, Vector3 size, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR

		Mesh mesh = m_MeshBox.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, size, solidColor);
		}

		SaveGizmosState();

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		//Gizmos.color = solidColor;
		//Gizmos.DrawCube(_Vector3Zero, size);

		Gizmos.color = wireframeColor;
		Gizmos.DrawWireCube(_Vector3Zero, size);

		RestoreGizmosState();

#endif // UNITY_EDITOR
	}

	#endregion // BOXES

	#region PYRAMIDS

	private static void DrawWirePyramid3D ()
	{
#if UNITY_EDITOR
		Vector3 p0 = new Vector3(0f, 1f, 0f);
		Vector3 p1 = new Vector3(-0.5f, 0f, -0.5f);
		Vector3 p2 = new Vector3(0.5f, 0f, -0.5f);
		Vector3 p3 = new Vector3(0.5f, 0f, 0.5f);
		Vector3 p4 = new Vector3(-0.5f, 0f, 0.5f);

		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);

		Gizmos.DrawLine(p0, p1);
		Gizmos.DrawLine(p0, p2);
		Gizmos.DrawLine(p0, p3);
		Gizmos.DrawLine(p0, p4);
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D pyramid. 
	/// </summary>
	/// <param name="position">Pyramid base position.</param>
	/// <param name="rotation">Pyramid rotation.</param>
	/// <param name="height">Pyramid height.</param>
	/// <param name="width">Pyramid base width.</param>
	/// <param name="axis">Pyramid reference axis.</param>
	/// <param name="color">Pyramid color.</param>
	public static void WirePyramid3D(Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(position, rotation, new Vector3(width, height, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWirePyramid3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D pyramid.
	/// </summary>
	/// <param name="position">Pyramid base position.</param>
	/// <param name="rotation">Pyramid rotation.</param>
	/// <param name="height">Pyramid height.</param>
	/// <param name="width">Pyramid base width.</param>
	/// <param name="axis">Pyramid reference axis.</param>
	/// <param name="color">Pyramid color.</param>
	public static void Pyramid3D(Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshPyramid.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);
			DrawMesh(mesh, position, rotation, new Vector3(width, height, width), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D pyramid.
	/// </summary>
	/// <param name="position">Pyramid base position.</param>
	/// <param name="rotation">Pyramid rotation.</param>
	/// <param name="height">Pyramid height.</param>
	/// <param name="width">Pyramid base width.</param>
	/// <param name="axis">Pyramid reference axis.</param>
	/// <param name="solidColor">Pyramid fill color.</param>
	/// <param name="wireframeColor">Pyramid wireframe color.</param>
	public static void Pyramid3D(Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		Mesh mesh = m_MeshPyramid.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, position, rotation, new Vector3(width, height, width), solidColor);
		}

		m_MatrixTemp.SetTRS(position, rotation, new Vector3(width, height, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWirePyramid3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // PYRAMIDS

	#region RHOMBUS

	private static void DrawWireRhombus3D ()
	{
#if UNITY_EDITOR
		Vector3 pTop = new Vector3(0f, 0.5f, 0f);
		Vector3 pBottom = new Vector3(0f, -0.5f, 0f);

		Vector3 p1 = new Vector3(-0.5f, 0f, -0.5f);
		Vector3 p2 = new Vector3(0.5f, 0f, -0.5f);
		Vector3 p3 = new Vector3(0.5f, 0f, 0.5f);
		Vector3 p4 = new Vector3(-0.5f, 0f, 0.5f);

		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);

		Gizmos.DrawLine(pTop, p1);
		Gizmos.DrawLine(pTop, p2);
		Gizmos.DrawLine(pTop, p3);
		Gizmos.DrawLine(pTop, p4);

		Gizmos.DrawLine(pBottom, p1);
		Gizmos.DrawLine(pBottom, p2);
		Gizmos.DrawLine(pBottom, p3);
		Gizmos.DrawLine(pBottom, p4);
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D rhombus.
	/// </summary>
	/// <param name="center">Rhombus center.</param>
	/// <param name="rotation">Rhombus rotation.</param>
	/// <param name="length">Rhombus length.</param>
	/// <param name="width">Rhombus width.</param>
	/// <param name="axis">Rhombus reference axis.</param>
	/// <param name="color"></param>
	public static void WireRhombus3D(Vector3 center, Quaternion rotation, float length, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(center, rotation, new Vector3(width, length, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireRhombus3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D rhombus.
	/// </summary>
	/// <param name="center">Rhombus center.</param>
	/// <param name="rotation">Rhombus rotation.</param>
	/// <param name="length">Rhombus length.</param>
	/// <param name="width">Rhombus width.</param>
	/// <param name="axis">Rhombus reference axis.</param>
	/// <param name="color">Rhombus color.</param>
	public static void Rhombus3D(Vector3 center, Quaternion rotation, float length, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshRhombus.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);
			DrawMesh(mesh, center, rotation, new Vector3(width, length, width), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D rhombus.
	/// </summary>
	/// <param name="center">Rhombus center.</param>
	/// <param name="rotation">Rhombus rotation.</param>
	/// <param name="length">Rhombus length.</param>
	/// <param name="width">Rhombus width.</param>
	/// <param name="axis">Rhombus reference axis.</param>
	/// <param name="solidColor">Rhombus fill color.</param>
	/// <param name="wireframeColor">Rhombus wireframe color.</param>
	public static void Rhombus3D(Vector3 center, Quaternion rotation, float length, float width, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		// Draw solid shape
		Mesh mesh = m_MeshRhombus.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, new Vector3(width, length, width), solidColor);
		}

		// Draw wire shape
		m_MatrixTemp.SetTRS(center, rotation, new Vector3(width, length, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireRhombus3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // RHOMBUS

	#region ARC

	/// <summary>
	/// Draw a solid filled arc in the Y-axis.
	/// </summary>
	/// <param name="center">Arc center.</param>
	/// <param name="rotation">Arc rotation.</param>
	/// <param name="radius">Arc radius.</param>
	/// <param name="sectorAngle">Arc angle of sector, in degrees.</param>
	/// <param name="color">Arc color.</param>
	public static void Arc3D(Vector3 center, Quaternion rotation, float radius, float sectorAngle, Color color)
	{
#if UNITY_EDITOR
		if (InitArcMaterial() && radius > 0f)
		{
			m_GizmoArcMaterial.SetColor(m_ColorPropID, color);
			m_GizmoArcMaterial.SetInt(m_ZTestPropID, (int)m_ZTest);
			m_GizmoArcMaterial.SetFloat(m_InnerRadiusPropID, 0f);
			m_GizmoArcMaterial.SetFloat(m_DirAnglePropID, 0f);
			m_GizmoArcMaterial.SetFloat(m_SectorAnglePropID, sectorAngle * 0.5f);
			m_GizmoArcMaterial.SetPass(0);

			m_MatrixTemp.SetTRS(center, rotation, new Vector3(radius, 1f, radius));
			Graphics.DrawMeshNow(m_MeshQuad.Mesh, m_MatrixTemp);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a solid filled arc in the Y-axis.
	/// </summary>
	/// <param name="center">Arc center.</param>
	/// <param name="rotation">Arc rotation.</param>
	/// <param name="innerRadius">Arc inner radius.</param>
	/// <param name="outerRadius">Arc outer radius.</param>
	/// <param name="directionAngle">Arc angular direction (clockwise), in degrees.</param>
	/// <param name="sectorAngle">Arc angle of sector, in degrees.</param>
	/// <param name="color">Arc color.</param>
	public static void Arc3D(Vector3 center, Quaternion rotation, float innerRadius, float outerRadius, float directionAngle, float sectorAngle, Color color)
	{
#if UNITY_EDITOR
		if (InitArcMaterial() && outerRadius > 0f && innerRadius < outerRadius)
		{
			m_GizmoArcMaterial.SetColor(m_ColorPropID, color);
			m_GizmoArcMaterial.SetInt(m_ZTestPropID, (int)m_ZTest);
			m_GizmoArcMaterial.SetFloat(m_InnerRadiusPropID, innerRadius / outerRadius);
			m_GizmoArcMaterial.SetFloat(m_DirAnglePropID, directionAngle);
			m_GizmoArcMaterial.SetFloat(m_SectorAnglePropID, sectorAngle * 0.5f);
			m_GizmoArcMaterial.SetPass(0);

			m_MatrixTemp.SetTRS(center, rotation, new Vector3(outerRadius, 1f, outerRadius));
			Graphics.DrawMeshNow(m_MeshQuad.Mesh, m_MatrixTemp);
		}
#endif // UNITY_EDITOR
	}

#if UNITY_EDITOR
	private static bool InitArcMaterial()
	{
		if (m_GizmoArcMaterial == null)
		{
			m_GizmoArcMaterial = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(IMGIZMOS_ARC_MATERIAL_PATH, typeof(Material));

			if (m_GizmoArcMaterial == null)
			{
				Debug.LogError(string.Format("Material {0} not found", IMGIZMOS_ARC_MATERIAL_PATH));
				return false;
			}

			m_ColorPropID = Shader.PropertyToID("_Color");
			m_ZTestPropID = Shader.PropertyToID("_ZTest");
			m_InnerRadiusPropID = Shader.PropertyToID("_InnerRadius");
			m_DirAnglePropID= Shader.PropertyToID("_DirAngle");
			m_SectorAnglePropID = Shader.PropertyToID("_SectorAngle");
		}
		
		return true;
	}
#endif // UNITY_EDITOR

	#endregion // ARC

	#region DISCS

	private static void DrawWireDisc()
	{
#if UNITY_EDITOR
		InitLookupTable();

		int index = 0, lastIndex = m_Sine.Length - 1;

		Vector3 p0, p1;
		p0.y = p1.y = 0f;

		p0.x = m_Sine[0];
		p0.z = m_Cosine[0];

		while (index < lastIndex)
		{
			++index;

			p1.x = m_Sine[index];
			p1.z = m_Cosine[index];

			Gizmos.DrawLine(p0, p1);

			p0 = p1;
		}
#endif // UNITY_EDITOR
	}


	/// <summary>
	/// Draw a wireframe 3D disc.
	/// </summary>
	/// <param name="origin">Disc origin.</param>
	/// <param name="rotation">Disc orientation.</param>
	/// <param name="radius">Disc radius.</param>
	/// <param name="axis">Disc reference axis.</param>
	/// <param name="color">Disc color.</param>
	public static void WireDisc3D (Vector3 origin, Quaternion rotation, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(origin, rotation, new Vector3(radius, 0f, radius));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireDisc();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D disc.
	/// </summary>
	/// <param name="origin">Disc origin.</param>
	/// <param name="rotation">Disc orientation.</param>
	/// <param name="radius">Disc radius.</param>
	/// <param name="axis">Disc reference axis.</param>
	/// <param name="color">Disc color.</param>
	public static void Disc3D(Vector3 origin, Quaternion rotation, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshDisc.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);
			DrawMesh(mesh, origin, rotation, new Vector3(radius, 0f, radius), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D disc.
	/// </summary>
	/// <param name="origin">Disc origin.</param>
	/// <param name="rotation">Disc orientation.</param>
	/// <param name="radius">Disc radius.</param>
	/// <param name="axis">Disc reference axis.</param>
	/// <param name="solidColor">Disc fill color.</param>
	/// <param name="wireframeColor">Disc wireframe color.</param>
	public static void Disc3D(Vector3 origin, Quaternion rotation, float radius, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		Mesh mesh = m_MeshDisc.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, origin, rotation, new Vector3(radius, 0f, radius), solidColor);
		}

		m_MatrixTemp.SetTRS(origin, rotation, new Vector3(radius, 0f, radius));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireDisc();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // DISCS

	#region SPHERES

	/// <summary>
	/// Draw a wireframe 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="rotation">Sphere radius.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	public static void WireSphere3D(Vector3 center, Quaternion rotation, float radius, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		Gizmos.DrawWireSphere(_Vector3Zero, radius);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="rotation">Sphere radius.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	public static void Sphere3D(Vector3 center, Quaternion rotation, float radius, Color color)
	{
#if UNITY_EDITOR
		//SaveGizmosState();
		//m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		//Gizmos.matrix = m_MatrixTemp;
		//Gizmos.DrawSphere(_Vector3Zero, radius);
		//RestoreGizmosState();

		Mesh mesh = m_MeshSphere.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, new Vector3(radius, radius, radius), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="rotation">Sphere radius.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="solidColor">Sphere fill color.</param>
	/// <param name="wireframeColor">Sphere wireframe color.</param>
	public static void Sphere3D(Vector3 center, Quaternion rotation, float radius, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		Mesh mesh = m_MeshSphere.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, new Vector3(radius, radius, radius), solidColor);
		}

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		Gizmos.DrawWireSphere(_Vector3Zero, radius);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // SPHERES

	#region ELLIPSOIDS

	/// <summary>
	/// Draw a wireframe 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="rotation">Ellipsoid rotation.</param>
	/// <param name="extents">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	public static void WireEllipsoid3D(Vector3 center, Quaternion rotation, Vector3 extents, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		m_MatrixTemp.SetTRS(center, rotation, extents);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		Gizmos.DrawWireSphere(_Vector3Zero, 0.5f);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="rotation">Ellipsoid rotation.</param>
	/// <param name="extents">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	public static void Ellipsoid3D(Vector3 center, Quaternion rotation, Vector3 extents, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshSphere.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, extents * 0.5f, color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="rotation">Ellipsoid rotation.</param>
	/// <param name="extents">Ellipsoid size.</param>
	/// <param name="solidColor">Ellipsoid fill color.</param>
	/// <param name="wireframeColor">Ellipsoid wireframe color.</param>
	public static void Ellipsoid3D(Vector3 center, Quaternion rotation, Vector3 extents, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		Mesh mesh = m_MeshSphere.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, extents * 0.5f, solidColor);
		}

		m_MatrixTemp.SetTRS(center, rotation, extents);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		Gizmos.DrawWireSphere(_Vector3Zero, 0.5f);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // ELLIPSOIDS

	#region CONES

	private static void DrawWireCone3D ()
	{
#if UNITY_EDITOR
		Vector3 p0 = new Vector3(0f, 1f, 0f);
		Vector3 p1 = new Vector3(-1f, 0f, 0f);
		Vector3 p2 = new Vector3(1f, 0f, 0f);
		Vector3 p3 = new Vector3(0f, 0f, -1f);
		Vector3 p4 = new Vector3(0f, 0f, 1f);

		Gizmos.DrawLine(p0, p1);
		Gizmos.DrawLine(p0, p2);
		Gizmos.DrawLine(p0, p3);
		Gizmos.DrawLine(p0, p4);
		DrawWireDisc();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D cone.
	/// </summary>
	/// <param name="position">Cone position (origin is located at the base).</param>
	/// <param name="rotation">Cone rotation.</param>
	/// <param name="height">Cone height.</param>
	/// <param name="width">Cone base width.</param>
	/// <param name="axis">Cone reference axis.</param>
	/// <param name="color">Cone color.</param>
	public static void WireCone3D(Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		width *= 0.5f;

		m_MatrixTemp.SetTRS(position, rotation, new Vector3(width, height, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireCone3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D cone.
	/// </summary>
	/// <param name="position">Cone position (origin is located at the base).</param>
	/// <param name="rotation">Cone rotation.</param>
	/// <param name="height">Cone height.</param>
	/// <param name="width">Cone base width.</param>
	/// <param name="axis">Cone reference axis.</param>
	/// <param name="color">Cone color.</param>
	public static void Cone3D (Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshCone.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);
			DrawMesh(mesh, position, rotation, new Vector3(width, height, width), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D cone.
	/// </summary>
	/// <param name="position">Cone position (origin is located at the base).</param>
	/// <param name="rotation">Cone rotation.</param>
	/// <param name="height">Cone height.</param>
	/// <param name="width">Cone base width.</param>
	/// <param name="axis">Cone reference axis.</param>
	/// <param name="solidColor">Cone fill color.</param>
	/// <param name="wireframeColor">Cone wireframe color.</param>
	public static void Cone3D (Vector3 position, Quaternion rotation, float height, float width, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		// Draw solid cone
		Mesh mesh = m_MeshCone.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, position, rotation, new Vector3(width, height, width), solidColor);
		}

		// Draw wire cone
		width *= 0.5f;

		m_MatrixTemp.SetTRS(position, rotation, new Vector3(width, height, width));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireCone3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // CONES

	#region CAPSULES

	private static void DrawWireCapsule3D (float height, float radius)
	{
#if UNITY_EDITOR
		InitLookupTable();

		height = height * 0.5f - radius;

		// Draw sides
		Gizmos.DrawLine(new Vector3(-radius, height, 0f), new Vector3(-radius, -height, 0f));
		Gizmos.DrawLine(new Vector3(radius, height, 0f), new Vector3(radius, -height, 0f));
		Gizmos.DrawLine(new Vector3(0f, height, -radius), new Vector3(0f, -height, -radius));
		Gizmos.DrawLine(new Vector3(0f, height, radius), new Vector3(0f, -height, radius));

		// Draw end discs
		int index = 0, lastIndex = m_Sine.Length - 1;

		Vector3 p0, p1;
		p0.y = p1.y = 0f;

		p0.x = m_Sine[0] * radius;
		p0.z = m_Cosine[0] * radius;

		while (index < lastIndex)
		{
			++index;

			p1.x = m_Sine[index] * radius;
			p1.z = m_Cosine[index] * radius;

			p0.y = p1.y = -height;

			Gizmos.DrawLine(p0, p1);

			p0.y = p1.y = height;

			Gizmos.DrawLine(p0, p1);

			p0 = p1;
		}

		// Draw end caps

		index = 0;
		lastIndex = m_Sine.Length / 2;

		float rx, ry, sx, sy;

		while (index < lastIndex)
		{
			rx = m_Sine[index] * radius;
			ry = m_Cosine[index] * radius;

			++index;

			sx = m_Sine[index] * radius;
			sy = m_Cosine[index] * radius;

			p0.x = 0.0f; p0.y = ry + height; p0.z = rx;
			p1.x = 0.0f; p1.y = sy + height; p1.z = sx;

			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(-p0, -p1);

			p0.x = rx; p0.y = ry + height; p0.z = 0.0f;
			p1.x = sx; p1.y = sy + height; p1.z = 0.0f;

			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(-p0, -p1);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D capsule.
	/// </summary>
	/// <param name="center">Capsule center.</param>
	/// <param name="rotation">Capsule rotation.</param>
	/// <param name="height">Capsule height.</param>
	/// <param name="radius">Capsule radius.</param>
	/// <param name="axis">Capsule reference axis.</param>
	/// <param name="color">Capsule color.</param>
	public static void WireCapsule3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		if (height <= (radius * 2f)) // If the diameter is equal to or less than the height, then we revert to a sphere
		{
			WireSphere3D(center, rotation, radius, color);
			return;
		}

		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireCapsule3D(height, radius);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D capsule.
	/// </summary>
	/// <param name="center">Capsule center.</param>
	/// <param name="rotation">Capsule rotation.</param>
	/// <param name="height">Capsule height.</param>
	/// <param name="radius">Capsule radius.</param>
	/// <param name="axis">Capsule reference axis.</param>
	/// <param name="color">Capsule color.</param>
	public static void Capsule3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		if (height <= (radius * 2f)) // If the diameter is equal to or less than the height, then we revert to a sphere
		{
			Sphere3D(center, rotation, radius, color);
			return;
		}

		Mesh meshBody = m_MeshCapsuleBody.Mesh;
		Mesh meshCap = m_MeshCapsuleCap.Mesh;

		if (meshBody != null && meshCap != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

			float bodyLength = height - (radius * 2f);

			DrawMesh(meshBody, center, rotation, new Vector3(radius, bodyLength, radius), color); // Draw body

			bodyLength *= 0.5f;

			Vector3 up = rotation * new Vector3(0f, bodyLength, 0f);

			DrawMesh(meshCap, center + up, rotation, new Vector3(radius, radius, radius), color); // Draw top cap
			DrawMesh(meshCap, center - up, rotation * CAPSULE_BOTTOM_CAP_ROTATION, new Vector3(radius, radius, radius), color); // Draw bottom cap
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D capsule.
	/// </summary>
	/// <param name="center">Capsule center.</param>
	/// <param name="rotation">Capsule rotation.</param>
	/// <param name="height">Capsule height.</param>
	/// <param name="radius">Capsule radius.</param>
	/// <param name="axis">Capsule reference axis.</param>
	/// <param name="solidColor">Capsule fill color.</param>
	/// <param name="wireframeColor">Capsule wireframe color.</param>
	public static void Capsule3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		if (height <= (radius * 2f)) // If the diameter is equal to or less than the height, then we revert to a sphere
		{
			Sphere3D(center, rotation, radius, solidColor, wireframeColor);
			return;
		}

		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		// Draw solid mesh
		Mesh meshBody = m_MeshCapsuleBody.Mesh;
		Mesh meshCap = m_MeshCapsuleCap.Mesh;

		if (meshBody != null && meshCap != null)
		{
			float bodyLength = height - (radius * 2f);

			DrawMesh(meshBody, center, rotation, new Vector3(radius, bodyLength, radius), solidColor); // Draw body

			bodyLength *= 0.5f;

			Vector3 up = rotation * new Vector3(0f, bodyLength, 0f);

			DrawMesh(meshCap, center + up, rotation, new Vector3(radius, radius, radius), solidColor); // Draw top cap
			DrawMesh(meshCap, center - up, rotation * CAPSULE_BOTTOM_CAP_ROTATION, new Vector3(radius, radius, radius), solidColor); // Draw bottom cap
		}

		// Draw wireframe
		m_MatrixTemp.SetTRS(center, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireCapsule3D(height, radius);

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // CAPSULES

	#region CYLINDERS

	private static void DrawWireCylinder3D()
	{
#if UNITY_EDITOR
		InitLookupTable();

		Gizmos.DrawLine(new Vector3(-1f, 0.5f, 0f), new Vector3(-1f, -0.5f, 0f));
		Gizmos.DrawLine(new Vector3(1f, 0.5f, 0f), new Vector3(1f, -0.5f, 0f));
		Gizmos.DrawLine(new Vector3(0f, 0.5f, -1f), new Vector3(0f, -0.5f, -1f));
		Gizmos.DrawLine(new Vector3(0f, 0.5f, 1f), new Vector3(0f, -0.5f, 1f));

		int index = 0, lastIndex = m_Sine.Length - 1;

		Vector3 p0, p1;
		p0.y = p1.y = 0f;

		p0.x = m_Sine[0];
		p0.z = m_Cosine[0];

		while (index < lastIndex)
		{
			++index;

			p1.x = m_Sine[index];
			p1.z = m_Cosine[index];

			p0.y = p1.y = -0.5f;

			Gizmos.DrawLine(p0, p1);

			p0.y = p1.y = 0.5f;

			Gizmos.DrawLine(p0, p1);

			p0 = p1;
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe 3D cylinder.
	/// </summary>
	/// <param name="center">Cylinder center.</param>
	/// <param name="rotation">Cylinder rotation.</param>
	/// <param name="height">Cylinder height.</param>
	/// <param name="radius">Cylinder radius.</param>
	/// <param name="axis">Cylinder reference axis.</param>
	/// <param name="color">Cylinder color.</param>
	public static void WireCylinder3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(center, rotation, new Vector3(radius, height, radius));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;
		DrawWireCylinder3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D cylinder.
	/// </summary>
	/// <param name="center">Cylinder center.</param>
	/// <param name="rotation">Cylinder rotation.</param>
	/// <param name="height">Cylinder height.</param>
	/// <param name="radius">Cylinder radius.</param>
	/// <param name="axis">Cylinder reference axis.</param>
	/// <param name="color">Cylinder color.</param>
	public static void Cylinder3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		Mesh mesh = m_MeshCylinder.Mesh;

		if (mesh != null)
		{
			IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);
			DrawMesh(mesh, center, rotation, new Vector3(radius, height, radius), color);
		}
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D cylinder.
	/// </summary>
	/// <param name="center">Cylinder center.</param>
	/// <param name="rotation">Cylinder rotation.</param>
	/// <param name="height">Cylinder height.</param>
	/// <param name="radius">Cylinder radius.</param>
	/// <param name="axis">Cylinder reference axis.</param>
	/// <param name="solidColor">Cylinder fill color.</param>
	/// <param name="wireframeColor">Cylinder wireframe color.</param>
	public static void Cylinder3D(Vector3 center, Quaternion rotation, float height, float radius, GizmoDrawAxis axis, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		Mesh mesh = m_MeshCylinder.Mesh;

		if (mesh != null)
		{
			DrawMesh(mesh, center, rotation, new Vector3(radius, height, radius), solidColor);
		}

		m_MatrixTemp.SetTRS(center, rotation, new Vector3(radius, height, radius));
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = wireframeColor;
		DrawWireCylinder3D();

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	#endregion // CYLINDERS

	#region MESH

	/// <summary>
	/// Draw a wireframe mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="color">Mesh color.</param>
	public static void WireMesh (Mesh mesh, Vector3 position, Quaternion rotation, Color color)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		Color prevColor = Gizmos.color;

		Gizmos.color = color;
		Gizmos.DrawWireMesh(mesh, position, rotation);

		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="scale">Mesh scale.</param>
	/// <param name="color">Mesh color.</param>
	public static void WireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		Color prevColor = Gizmos.color;

		Gizmos.color = color;
		Gizmos.DrawWireMesh(mesh, position, rotation, scale);

		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a wireframe mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="tf">Mesh transform.</param>
	/// <param name="color">Mesh color.</param>
	public static void WireMesh(Mesh mesh, Transform tf, Color color)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		Color prevColor = Gizmos.color;

		Gizmos.color = color;
		Gizmos.DrawWireMesh(mesh, tf.position, tf.rotation, tf.lossyScale);

		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="color">Mesh color.</param>
	public static void Mesh (Mesh mesh, Vector3 position, Quaternion rotation, Color color)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		DrawMesh(mesh, position, rotation, _Vector3One, color);
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="scale">Mesh scale.</param>
	/// <param name="color">Mesh color.</param>
	public static void Mesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		DrawMesh(mesh, position, rotation, scale, color);
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="solidColor">Mesh fill color.</param>
	/// <param name="wireframeColor">Mesh wireframe color.</param>
	public static void Mesh(Mesh mesh, Vector3 position, Quaternion rotation, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		DrawMesh(mesh, position, rotation, _Vector3One, solidColor);

		Color prevColor = Gizmos.color;
		Gizmos.color = wireframeColor;
		Gizmos.DrawWireMesh(mesh, position, rotation);
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}


	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Source mesh.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="scale">Mesh scale.</param>
	/// <param name="solidColor">Mesh fill color.</param>
	/// <param name="wireframeColor">Mesh wireframe color.</param>
	public static void Mesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color solidColor, Color wireframeColor)
	{
#if UNITY_EDITOR
		if (mesh == null)
			return;

		DrawMesh(mesh, position, rotation, scale, solidColor);

		Color prevColor = Gizmos.color;
		Gizmos.color = wireframeColor;
		Gizmos.DrawWireMesh(mesh, position, rotation, scale);
		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}

	#endregion // MESH

	#region SPECIAL PRIMITIVES

	/// <summary>
	/// Draw a 3-dimensional axis (X-axis=red, Y-axis=green, Z-axis=blue).
	/// </summary>
	/// <param name="origin">Axis origin.</param>
	/// <param name="rotation">Axis rotation.</param>
	/// <param name="length">Length of axis lines.</param>
	/// <param name="alpha">Axis transparency.</param>
	public static void Axis3D(Vector3 origin, Quaternion rotation, float length, float alpha)
	{
		#if UNITY_EDITOR
		Color prevColor = Gizmos.color;

		Gizmos.color = new Color(COLOR_XAXIS.r, COLOR_XAXIS.g, COLOR_XAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(length, 0f, 0f));

		Gizmos.color = new Color(COLOR_YAXIS.r, COLOR_YAXIS.g, COLOR_YAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(0f, length, 0f));

		Gizmos.color = new Color(COLOR_ZAXIS.r, COLOR_ZAXIS.g, COLOR_ZAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(0f, 0f, length));

		Gizmos.color = prevColor;
		#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3-dimensional axis (X-axis=red, Y-axis=green, Z-axis=blue).
	/// </summary>
	/// <param name="origin">Axis origin.</param>
	/// <param name="rotation">Axis rotation.</param>
	/// <param name="length">Length of each axis line.</param>
	/// <param name="alpha">Axis transparency.</param>
	public static void Axis3D(Vector3 origin, Quaternion rotation, Vector3 length, float alpha)
	{
		#if UNITY_EDITOR
		Color prevColor = Gizmos.color;

		Gizmos.color = new Color(COLOR_XAXIS.r, COLOR_XAXIS.g, COLOR_XAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(length.x, 0f, 0f));

		Gizmos.color = new Color(COLOR_YAXIS.r, COLOR_YAXIS.g, COLOR_YAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(0f, length.y, 0f));

		Gizmos.color = new Color(COLOR_ZAXIS.r, COLOR_ZAXIS.g, COLOR_ZAXIS.b, alpha);
		Gizmos.DrawLine(origin, origin + rotation * new Vector3(0f, 0f, length.z));

		Gizmos.color = prevColor;
		#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3-dimensional axis grid point (X-axis=red, Y-axis=green, Z-axis=blue).
	/// </summary>
	/// <param name="origin">Axis origin.</param>
	/// <param name="rotation">Axis rotation.</param>
	/// <param name="halfLength">Half length of each axis line.</param>
	/// <param name="alpha">Axis transparency.</param>
	public static void AxisGridPoint3D (Vector3 origin, Quaternion rotation, Vector3 halfLength, float alpha)
	{
#if UNITY_EDITOR
		Color prevColor = Gizmos.color;

		Vector3 offset;

		Gizmos.color = new Color(COLOR_XAXIS.r, COLOR_XAXIS.g, COLOR_XAXIS.b, alpha);
		offset = rotation * new Vector3(halfLength.x, 0f, 0f);
		Gizmos.DrawLine(origin - offset, origin + offset);

		Gizmos.color = new Color(COLOR_YAXIS.r, COLOR_YAXIS.g, COLOR_YAXIS.b, alpha);
		offset = rotation * new Vector3(0f, halfLength.y, 0f);
		Gizmos.DrawLine(origin - offset, origin + offset);

		Gizmos.color = new Color(COLOR_ZAXIS.r, COLOR_ZAXIS.g, COLOR_ZAXIS.b, alpha);
		offset = rotation * new Vector3(0f, 0f, halfLength.z);
		Gizmos.DrawLine(origin - offset, origin + offset);

		Gizmos.color = prevColor;
#endif // UNITY_EDITOR
	}


	/// <summary>
	/// Draw a 3D grid point.
	/// </summary>
	/// <param name="origin">Grid point origin.</param>
	/// <param name="rotation">Grid point orientation.</param>
	/// <param name="extents">Grid point extents.</param>
	/// <param name="color">Grid point colour.</param>
	public static void GridPoint3D (Vector3 origin, Quaternion rotation, float extents, Color color)
	{
		#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;

		extents *= 0.5f;

		Vector3 offset;

		offset = rotation * new Vector3(extents, 0f, 0f);
		Gizmos.DrawLine(origin + offset, origin - offset);

		offset = rotation * new Vector3(0f, extents, 0f);
		Gizmos.DrawLine(origin + offset, origin - offset);

		offset = rotation * new Vector3(0f, 0f, extents);
		Gizmos.DrawLine(origin + offset, origin - offset);

		Gizmos.color = prevColor;
		#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D grid point.
	/// </summary>
	/// <param name="origin">Grid point origin.</param>
	/// <param name="rotation">Grid point orientation.</param>
	/// <param name="extents">Grid point XYZ extents.</param>
	/// <param name="color">Grid point colour.</param>
	public static void GridPoint3D (Vector3 origin, Quaternion rotation, Vector3 extents, Color color)
	{
		#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;

		extents *= 0.5f;

		Vector3 offset;

		offset = rotation * new Vector3(extents.x, 0f, 0f);
		Gizmos.DrawLine(origin + offset, origin - offset);

		offset = rotation * new Vector3(0f, extents.y, 0f);
		Gizmos.DrawLine(origin + offset, origin - offset);

		offset = rotation * new Vector3(0f, 0f, extents.z);
		Gizmos.DrawLine(origin + offset, origin - offset);

		Gizmos.color = prevColor;
		#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a 3D grid in a specified plane.
	/// </summary>
	/// <param name="origin">Grid origin.</param>
	/// <param name="rotation">Grid orientation.</param>
	/// <param name="extentX">Grid width.</param>
	/// <param name="extentY">Grid height.</param>
	/// <param name="cellsX">Number of cells along the width.</param>
	/// <param name="cellsY">Number of cells along the height.</param>
	/// <param name="axis">The reference plane for the grid.</param>
	/// <param name="color">Grid color.</param>
	public static void Grid3D (Vector3 origin, Quaternion rotation, float extentX, float extentY, int cellsX, int cellsY, GizmoDrawAxis axis, Color color)
	{
#if UNITY_EDITOR
		if (cellsX < 1 || cellsY < 1 || extentX <= 0f || extentY <= 0f)
			return;

		SaveGizmosState();

		IMDrawRotationHelper.SetRotationAxis(ref rotation, (int)axis);

		m_MatrixTemp.SetTRS(origin, rotation, _Vector3One);
		Gizmos.matrix = m_MatrixTemp;

		Gizmos.color = color;

		float endX = extentX / 2f;
		float endZ = extentY / 2f;

		float startX = -endX;
		float startZ = -endZ;

		Vector3 p0, p1;
		p0.y = p1.y = 0f;

		float v, vStep;

		v = startX;
		vStep = extentX / (float)cellsX;
        p0.z = startZ;
		p1.z = endZ;

		for (int i = 0; i <= cellsX; ++i)
		{
			p0.x = p1.x = v;

			v += vStep;

			Gizmos.DrawLine(p0, p1);
		}

		v = startZ;
		vStep = extentY / (float)cellsY;
		p0.x = startX;
		p1.x = endX;

		for (int i = 0; i <= cellsY; ++i)
		{
			p0.z = p1.z = v;

			v += vStep;

			Gizmos.DrawLine(p0, p1);
		}

		RestoreGizmosState();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw 3D bounds (an axis aligned box).
	/// </summary>
	/// <param name="bounds">Bounds that specifies a position and extents.</param>
	/// <param name="color">Bounds color.</param>
	public static void Bounds (Bounds bounds, Color color)
	{
		#if UNITY_EDITOR
		Color prevColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
		Gizmos.color = prevColor;
		#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw the 3D bounds for a renderer (note: only draws bounds if renderer is visible).
	/// </summary>
	/// <param name="renderer">Renderer whose bounds will be drawn.</param>
	/// <param name="color">Bounds color.</param>
	public static void Bounds (Renderer renderer, Color color)
	{
#if UNITY_EDITOR
		if (renderer != null && renderer.isVisible)
		{
			Color prevColor = Gizmos.color;
			Gizmos.color = color;
			Bounds bounds = renderer.bounds;
			Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
			Gizmos.color = prevColor;
		}
#endif // UNITY_EDITOR
	}

	#endregion // SPECIAL PRIMITIVES

	#region LABELS

	// Note: changed y offset direction since screen Y is flipped before rather than after.
	private static void SetPivot (ref float x, ref float y, Vector2 size, LabelPivot pivot)
	{
#if UNITY_EDITOR
		switch (pivot)
		{
			case LabelPivot.LOWER_CENTER: x -= size.x * 0.5f; y -= size.y; break;
			case LabelPivot.LOWER_LEFT: y -= size.y; break;
			case LabelPivot.LOWER_RIGHT: x -= size.x; y -= size.y; break;

			case LabelPivot.MIDDLE_CENTER: x -= size.x * 0.5f; y -= size.y * 0.5f; break;
			case LabelPivot.MIDDLE_LEFT: y -= size.y * 0.5f; break;
			case LabelPivot.MIDDLE_RIGHT: x -= size.x; y -= size.y * 0.5f; break;

			case LabelPivot.UPPER_CENTER: x -= size.x * 0.5f; break;
			//case TextAnchor.UpperLeft: break;
			case LabelPivot.UPPER_RIGHT: x -= size.x; break;
		}
#endif // UNITY_EDITOR
	}

	private static void SetAlignment (LabelAlignment alignment)
	{
#if UNITY_EDITOR
		switch (alignment)
		{
			case LabelAlignment.LEFT: m_Style.alignment = TextAnchor.UpperLeft; break;
			case LabelAlignment.CENTER: m_Style.alignment = TextAnchor.UpperCenter; break;
			case LabelAlignment.RIGHT: m_Style.alignment = TextAnchor.UpperRight; break;
		}
#endif // UNITY_EDITOR
	}

	private static Camera GetEditorSceneCamera ()
	{
#if UNITY_EDITOR
	#if UNITY_2019_1_OR_NEWER
		return UnityEditor.SceneView.currentDrawingSceneView.camera;
	#else
		return Camera.current;
	#endif
#else
		return null;
#endif // UNITY_EDITOR
	}

	private static float GetPixelsPerPoint ()
	{
#if UNITY_EDITOR && UNITY_5_4_OR_NEWER
		return UnityEditor.EditorGUIUtility.pixelsPerPoint;
#else
		return 1f;
#endif
	}

	// Correctly calculate screen position from editor scene view world position taking into account editor UI scaling
	private static Vector3 WorldToScreenPoint (Camera cam, Vector3 position)
	{
		float pixelsPerPoint = GetPixelsPerPoint();
		Vector3 screenPoint = cam.WorldToViewportPoint(position); // Note: we don't use WorldToSceenPoint because it is broken and doesn't take into account pixels per point
		screenPoint.x *= cam.pixelWidth / pixelsPerPoint;
		screenPoint.y = (cam.pixelHeight / pixelsPerPoint) * (1f - screenPoint.y);
		//screenPoint.y -= 15f; // There seems to be some weird Y offset when using WorldToViewportPoint, so correct it
		return screenPoint;
	}

	// Fix for Unity issue where GUI elements are offset in Y-axis proportional to font size
	private static void DoLabelPositionCorrection (int fontSize, ref float screenY)
	{
		screenY -= (float)fontSize * 0.09f; // This line can be commented out to avoid correction
	}


	/// <summary>
	/// Draw a label in screen space.
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void Label(float x, float y, Color color, LabelPivot pivot, LabelAlignment alignment, string label, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		m_Style.fontSize = fontSize;
		m_Style.normal.textColor = color;
		m_Style.alignment = TextAnchor.UpperLeft;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		if (x < -size.x || y < -size.y)
			return;

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		if (x > cam.pixelWidth || y > cam.pixelHeight)
			return;

		DoLabelPositionCorrection(fontSize, ref y);

		float invPixelsPerPoint = 1f / GetPixelsPerPoint();
		x *= invPixelsPerPoint;
		y *= invPixelsPerPoint;

		SetPivot(ref x, ref y, size, pivot); // Needs to be done after screen position has been scaled

		UnityEditor.Handles.BeginGUI();
		GUI.Label(new Rect(x, y, size.x, size.y), m_GUIContent, m_Style);
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a label (with drop shadow) in screen space.
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void LabelShadowed(float x, float y, Color color, LabelPivot pivot, LabelAlignment alignment, string label, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		m_Style.fontSize = fontSize;
		m_Style.alignment = TextAnchor.UpperLeft;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		if (x < -size.x || y < -size.y)
			return;

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		if (x > cam.pixelWidth || y > cam.pixelHeight)
			return;

		DoLabelPositionCorrection(fontSize, ref y);

		float invPixelsPerPoint = 1f / GetPixelsPerPoint();
		x *= invPixelsPerPoint;
		y *= invPixelsPerPoint;

		SetPivot(ref x, ref y, size, pivot); // Needs to be done after screen position has been scaled

		UnityEditor.Handles.BeginGUI();
		m_Style.normal.textColor = new Color(0f, 0f, 0f, color.a);
		GUI.Label(new Rect(x + 2f, y + 2f, size.x, size.y), m_GUIContent, m_Style); // Shadow label

		m_Style.normal.textColor = color;
		GUI.Label(new Rect(x, y, size.x, size.y), m_GUIContent, m_Style); // Main label
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a label in 3D space.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="maxDist">Max draw distance from viewer.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void Label (Vector3 position, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float maxDist, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		Vector3 screenPos = WorldToScreenPoint(cam, position);

		if (screenPos.z < 0f || screenPos.z > maxDist)
			return;

		m_Style.fontSize = fontSize;
		m_Style.normal.textColor = color;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		DoLabelPositionCorrection(fontSize, ref screenPos.y);

		SetPivot(ref screenPos.x, ref screenPos.y, size, pivot);

		UnityEditor.Handles.BeginGUI();
		GUI.Label(new Rect((float)System.Math.Round(screenPos.x), (float)System.Math.Round(screenPos.y), size.x, size.y), m_GUIContent, m_Style);
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a label (with drop shadow) in 3D space.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="maxDist">Max draw distance from viewer.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void LabelShadowed(Vector3 position, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float maxDist, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		Vector3 screenPos = WorldToScreenPoint(cam, position);

		if (screenPos.z < 0f || screenPos.z > maxDist)
			return;

		m_Style.fontSize = fontSize;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		DoLabelPositionCorrection(fontSize, ref screenPos.y);

		SetPivot(ref screenPos.x, ref screenPos.y, size, pivot);
		screenPos.x = (float)System.Math.Round(screenPos.x);
		screenPos.y = (float)System.Math.Round(screenPos.y);

		UnityEditor.Handles.BeginGUI();
		m_Style.normal.textColor = new Color (0f, 0f, 0f, color.a);
		GUI.Label(new Rect(screenPos.x + 2f, screenPos.y + 2f, size.x, size.y), m_GUIContent, m_Style);

		m_Style.normal.textColor = color;
		GUI.Label(new Rect(screenPos.x, screenPos.y, size.x, size.y), m_GUIContent, m_Style);
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}

	/// <summary>
	/// Draw a label in 3D space.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="offsetX">Screen space X position offset.</param>
	/// <param name="offsetY">Screen space Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="maxDist">Max draw distance from viewer.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void Label(Vector3 position, float offsetX, float offsetY, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float maxDist, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		Vector3 screenPos = WorldToScreenPoint(cam, position);

		if (screenPos.z < 0f || screenPos.z > maxDist)
			return;

		m_Style.fontSize = fontSize;
		m_Style.normal.textColor = color;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		DoLabelPositionCorrection(fontSize, ref screenPos.y);

		SetPivot(ref screenPos.x, ref screenPos.y, size, pivot);

		UnityEditor.Handles.BeginGUI();
		GUI.Label(new Rect((float)System.Math.Round(screenPos.x + offsetX), (float)System.Math.Round(screenPos.y - offsetY), size.x, size.y), m_GUIContent, m_Style);
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}


	/// <summary>
	/// Draw a label (with drop shadow) in 3D space.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="offsetX">Screen space X position offset.</param>
	/// <param name="offsetY">Screen space Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="maxDist">Max draw distance from viewer.</param>
	/// <param name="fontSize">Label font size.</param>
	public static void LabelShadowed (Vector3 position, float offsetX, float offsetY, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float maxDist, int fontSize = 12)
	{
#if UNITY_EDITOR
		InitGUI();

		Camera cam = GetEditorSceneCamera();

		if (cam == null)
			return;

		Vector3 screenPos = WorldToScreenPoint(cam, position);

		if (screenPos.z < 0f || screenPos.z > maxDist)
			return;

		m_Style.fontSize = fontSize;
		m_Style.normal.textColor = color;
		SetAlignment(alignment);

		m_GUIContent.text = label;
		Vector3 size = m_Style.CalcSize(m_GUIContent);

		DoLabelPositionCorrection(fontSize, ref screenPos.y);

		SetPivot(ref screenPos.x, ref screenPos.y, size, pivot);
		screenPos.x = (float)System.Math.Round(screenPos.x + offsetX);
		screenPos.y = (float)System.Math.Round(screenPos.y - offsetY);

		UnityEditor.Handles.BeginGUI();
		m_Style.normal.textColor = new Color(0f, 0f, 0f, color.a);
		GUI.Label(new Rect(screenPos.x + 2f, screenPos.y + 2f, size.x, size.y), m_GUIContent, m_Style);

		m_Style.normal.textColor = color;
		GUI.Label(new Rect(screenPos.x, screenPos.y, size.x, size.y), m_GUIContent, m_Style);
		UnityEditor.Handles.EndGUI();
#endif // UNITY_EDITOR
	}

#endregion // LABELS

#region IMAGES

	/// <summary>
	/// Draw a texture in screen space.
	/// </summary>
	/// <param name="rect">Screen position, width and height.</param>
	/// <param name="color">Texture color.</param>
	/// <param name="texture">Source texture.</param>
	public static void Image (Rect rect, Color color, Texture2D texture)
	{
#if UNITY_EDITOR

		if (texture == null)
			return;

		float invPixelsPerPoint = 1f / GetPixelsPerPoint();
		rect.x *= invPixelsPerPoint;
		rect.y *= invPixelsPerPoint;
		rect.width *= invPixelsPerPoint;
		rect.height *= invPixelsPerPoint;

		UnityEditor.Handles.BeginGUI();
		Color prevColor = GUI.color;
		GUI.color = color;
		GUI.DrawTexture(rect, texture);
		GUI.color = prevColor;
		UnityEditor.Handles.EndGUI();

#endif // UNITY_EDITOR
	}

	/*
	// Note: RotateAroundPivot doesn't appear to work correctly in scene view
	public static void Image2D (Rect rect, Vector2 pivot, float angle, Color color, Texture2D texture)
	{
#if UNITY_EDITOR

		if (texture == null)
			return;

			Camera cam = Camera.current;

			if (cam == null)
				return;

		UnityEditor.Handles.BeginGUI();

		Color prevColour = GUI.color;
		GUI.color = color;

		if (angle != 0f)
		{
			Matrix4x4 matrixBackup = GUI.matrix;

			//pivot.x = rect.x + (rect.width * pivot.x);
			//pivot.y = rect.y - (rect.height * pivot.y);

				pivot.x = cam.pixelWidth / 2f;
				pivot.y = cam.pixelHeight / 2f;

			GUIUtility.RotateAroundPivot(angle, pivot);
			GUI.DrawTexture(rect, texture);
			GUI.matrix = matrixBackup;
		}
		else
		{
			GUI.DrawTexture(rect, texture);
		}
		
		GUI.color = prevColour;
		UnityEditor.Handles.EndGUI();

#endif // UNITY_EDITOR
	}
	*/

#endregion // IMAGES
}
