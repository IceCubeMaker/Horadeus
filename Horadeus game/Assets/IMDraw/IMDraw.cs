using UnityEngine;
using System;
using System.Collections;

public static class IMDraw
{
	public const string VERSION = "1.4.3";

	private static readonly Quaternion QUATERNION_IDENTITY = new Quaternion(0f, 0f, 0f, 1f);

	/// <summary>Set a camera as the draw target.</summary>
	/// <param name="camera">Target camera.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void SetTarget (IMDrawCamera camera)
	{
		if (camera != null)
		{
			camera.SetTarget();
		}
	}

	/// <summary>[Deprecated]</summary>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void SetTarget (int cameraIndex)
	{
		Debug.LogWarning("Function IMDraw.SetTarget(int cameraIndex) has been deprecated.");
	}

	/// <summary>[Deprecated]</summary>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void SetDefaultTarget()
	{
		Debug.LogWarning("Function IMDraw.SetDefaultTarget() has been deprecated.");
	}

	/// <summary>[Deprecated]</summary>
	public static void SetDefault (IMDrawCamera camera)
	{
		Debug.LogWarning("Function IMDraw.SetDefault(IMDrawCamera camera) has been deprecated.");
	}

	/// <summary>[Deprecated]</summary>
	public static void SetDefault (int cameraIndex)
	{
		Debug.LogWarning("Function IMDraw.SetDefault(int cameraIndex) has been deprecated.");
	}

	/// <summary>[Deprecated]</summary>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Flush(int cameraIndex)
	{
		Debug.LogWarning("Function IMDraw.Flush(int cameraIndex) has been deprecated.");
	}

	/// <summary>Flush draw commands on the current target camera.</summary>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Flush()
	{
		if (IMDrawManager.Instance != null)
		{
			IMDrawManager.Instance.Flush();
		}
	}

	/// <summary>Flush draw commands for specified camera.</summary>
	/// <param name="camera">Target IMDrawCamera.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Flush(IMDrawCamera camera)
	{
		if (IMDrawManager.Instance != null && camera != null)
		{
			camera.FlushImmediate();
		}
	}

	/// <summary>Flush all draw commands.</summary>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void FlushAll ()
	{
		if (IMDrawManager.Instance != null)
		{
			IMDrawManager.Instance.FlushAll();
		}
	}

	/// <summary>Set current draw mode to use default Z testing (LEqual). Only affects GL primitives, meshes and text meshes.</summary>
	public static void SetDefaultZTest ()
	{
		IMDrawManager.s_ZTest = IMDrawZTest.LessEqual;
	}

	// <summary>Set/get current draw mode to use specified Z testing. Only affects GL primitives, meshes and text meshes.</summary>
	public static IMDrawZTest ZTest
	{
		set
		{
			IMDrawManager.s_ZTest = value;
		}

		get
		{
			return IMDrawManager.s_ZTest;
		}
	}

	#region 3D LINE PRIMITIVES

	/// <summary>
	/// Draw a 3D line.
	/// </summary>
	/// <param name="from">Starting point of the line.</param>
	/// <param name="to">Ending point of the line.</param>
	/// <param name="color">Line color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Line3D (Vector3 from, Vector3 to, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = from;
		command.m_P2 = to;
		command.m_C1 = color;
		command.m_C2 = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D line.
	/// </summary>
	/// <param name="from">Starting point of the line.</param>
	/// <param name="to">Ending point of the line.</param>
	/// <param name="fromColor">Color of line from the starting point.</param>
	/// <param name="toColor">Color of line from the ending point.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Line3D (Vector3 from, Vector3 to, Color fromColor, Color toColor, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = from;
		command.m_P2 = to;
		command.m_C1 = fromColor;
		command.m_C2 = toColor;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="ray">Source ray.</param>
	/// <param name="length">Length of line.</param>
	/// <param name="color">Ray color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ray3D(Ray ray, float length, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = ray.origin;
		command.m_P2 = ray.origin + (ray.direction * length);
		command.m_C1 = color;
		command.m_C2 = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="ray">Source ray.</param>
	/// <param name="length">Length of line.</param>
	/// <param name="fromColor">Color of line from the starting point.</param>
	/// <param name="toColor">Color of line from the ending point.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ray3D(Ray ray, float length, Color fromColor, Color toColor, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = ray.origin;
		command.m_P2 = ray.origin + (ray.direction * length);
		command.m_C1 = fromColor;
		command.m_C2 = toColor;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="origin">Origin of ray.</param>
	/// <param name="direction">Direction of ray (assumed to be normalised).</param>
	/// <param name="length">Length of line.</param>
	/// <param name="color">Ray color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ray3D(Vector3 origin, Vector3 direction, float length, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = origin;
		command.m_P2 = origin + (direction * length);
		command.m_C1 = color;
		command.m_C2 = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D ray.
	/// </summary>
	/// <param name="origin">Origin of ray.</param>
	/// <param name="direction">Direction of ray (assumed to be normalised).</param>
	/// <param name="length">Length of line.</param>
	/// <param name="startColor">Color of line from starting point of the ray.</param>
	/// <param name="endColor">Color of line from ending point of the ray.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ray3D(Vector3 origin, Vector3 direction, float length, Color startColor, Color endColor, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.LINE;
		command.m_Lines = IMDrawLineCount.LINE;
		command.m_P1 = origin;
		command.m_P2 = origin + (direction * length);
		command.m_C1 = startColor;
		command.m_C2 = endColor;
		command.m_T = duration;
	}

	#endregion

	#region 3D QUAD PRIMITIVES

	/// <summary>
	/// Draw a 3D quad.
	/// </summary>
	/// <param name="center">Quad center position.</param>
	/// <param name="rotation">Quad rotation.</param>
	/// <param name="sizeX">Width of quad.</param>
	/// <param name="sizeY">Height of quad.</param>
	/// <param name="axis">Orientation axis of quad.</param>
	/// <param name="color">Quad color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Quad3D (Vector3 center, Quaternion rotation, float sizeX, float sizeY, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.QUAD;
		command.m_Verts = IMDrawVertexCount.MESH_QUAD;
		command.m_Position = center;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Color = color;
		command.m_Size = new Vector3(sizeX, 1f, sizeY);
		command.m_T = duration;
	}

	#endregion

	#region 3D BOX PRIMITIVES

	/// <summary>
	/// Draw an axis aligned wireframe 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireBox3D (Vector3 center, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_BOX;
		command.m_Lines = IMDrawLineCount.WIRE_BOX;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Size = size * 0.5f; // Convert to half extents
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a rotated wireframe 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="rotation">Box orientation.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireBox3D (Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_BOX_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_BOX;
		command.m_P1 = center;
		command.m_Rotation = rotation;
		command.m_C1 = color;
		command.m_Size = size * 0.5f; // Convert to half extents
		command.m_T = duration;
	}

	/// <summary>
	/// Draw an axis aligned solid 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Box3D (Vector3 center, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.BOX;
		command.m_Verts = IMDrawVertexCount.MESH_BOX;
		command.m_Position = center;
		command.m_Rotation = QUATERNION_IDENTITY;
		command.m_Color = color;
		command.m_Size = size;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a rotated solid 3D box.
	/// </summary>
	/// <param name="center">Box center position.</param>
	/// <param name="rotation">Box orientation.</param>
	/// <param name="size">Box extents.</param>
	/// <param name="color">Box color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Box3D (Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.BOX;
		command.m_Verts = IMDrawVertexCount.MESH_BOX;
		command.m_Position = center;
		command.m_Rotation = rotation;
		command.m_Color = color;
		command.m_Size = size;
		command.m_T = duration;
	}

	#endregion

	#region 3D PYRAMID PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D pyramid.
	/// </summary>
	/// <param name="position">Pyramid base position.</param>
	/// <param name="rotation">Pyramid rotation.</param>
	/// <param name="height">Pyramid height.</param>
	/// <param name="width">Pyramid base width.</param>
	/// <param name="axis">Pyramid reference axis.</param>
	/// <param name="color">Pyramid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WirePyramid3D(Vector3 position, Quaternion rotation, float height, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_PYRAMID_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_PYRAMID;
		command.m_P1 = position;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_C1 = color;
		command.m_Size.x = width * 0.5f; // Convert to half width
		command.m_Size.y = height;
		command.m_T = duration;
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
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Pyramid3D(Vector3 position, Quaternion rotation, float height, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.PYRAMID;
		command.m_Verts = IMDrawVertexCount.MESH_PYRAMID;
		command.m_Position = position;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Color = color;
		command.m_Size.x = command.m_Size.z = width; // Convert to half width
		command.m_Size.y = height;
		command.m_T = duration;
	}

	#endregion

	#region 3D RHOMBUS PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D rhombus.
	/// </summary>
	/// <param name="center">Rhombus center.</param>
	/// <param name="rotation">Rhombus rotation.</param>
	/// <param name="length">Rhombus length.</param>
	/// <param name="width">Rhombus width.</param>
	/// <param name="axis">Rhombus reference axis.</param>
	/// <param name="color">Rhombus color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireRhombus3D (Vector3 center, Quaternion rotation, float length, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_RHOMBUS_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_RHOMBUS;
		command.m_P1 = center;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_C1 = color;
		command.m_Size.x = width * 0.5f; // Convert to half extents
		command.m_Size.y = length * 0.5f; // Convert to half extents
		command.m_T = duration;
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
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Rhombus3D(Vector3 center, Quaternion rotation, float length, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.RHOMBUS;
		command.m_Verts = IMDrawVertexCount.MESH_RHOMBUS;
		command.m_Position = center;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Color = color;
		command.m_Size.x = command.m_Size.z = width; // Convert to half width
		command.m_Size.y = length;
		command.m_T = duration;
	}

	#endregion

	#region 3D ARC PRIMITIVES


	/// <summary>
	/// Draw a solid filled arc in the Y-axis.
	/// </summary>
	/// <param name="origin">Arc origin.</param>
	/// <param name="rotation">Arc rotation.</param>
	/// <param name="radius">Arc radius.</param>
	/// <param name="sectorAngle">Arc angle of sector, in degrees.</param>
	/// <param name="color">Arc color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Arc3D (Vector3 origin, Quaternion rotation, float radius, float sectorAngle, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.ARC;
		command.m_Verts = IMDrawVertexCount.MESH_ARC;
		command.m_Position = origin;
		command.m_Rotation = Quaternion.identity;
		command.m_Size = new Vector3(0f, radius, 0f);
		command.m_DirAngle = 0f;
		command.m_SectorAngle = sectorAngle * 0.5f;
		command.m_Color = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid filled arc in the Y-axis.
	/// </summary>
	/// <param name="origin">Arc origin.</param>
	/// <param name="rotation">Arc rotation.</param>
	/// <param name="innerRadius">Arc inner radius.</param>
	/// <param name="outerRadius">Arc outer radius.</param>
	/// <param name="directionAngle">Arc angular direction (clockwise), in degrees.</param>
	/// <param name="sectorAngle">Arc angle of sector, in degrees.</param>
	/// <param name="color">Arc color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Arc3D (Vector3 origin, Quaternion rotation, float innerRadius, float outerRadius, float directionAngle, float sectorAngle, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.ARC;
		command.m_Verts = IMDrawVertexCount.MESH_ARC;
		command.m_Position = origin;
		command.m_Rotation = rotation;
		command.m_Size = new Vector3(innerRadius, outerRadius, 0f);
		command.m_DirAngle = directionAngle;
		command.m_SectorAngle = sectorAngle * 0.5f;
		command.m_Color = color;
		command.m_T = duration;
	}

	#endregion

	#region 3D DISC PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D disc.
	/// </summary>
	/// <param name="origin">Disc origin.</param>
	/// <param name="rotation">Disc orientation.</param>
	/// <param name="radius">Disc radius.</param>
	/// <param name="axis">Orientation axis.</param>
	/// <param name="color">Disc color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireDisc3D (Vector3 origin, Quaternion rotation, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_DISC_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_DISC;
		command.m_P1 = origin;
		command.m_C1 = color;
		command.m_Rotation = rotation;
		command.m_Size.x = radius;
		command.m_Axis = axis;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D disc.
	/// </summary>
	/// <param name="origin">Disc origin.</param>
	/// <param name="rotation">Disc orientation.</param>
	/// <param name="radius">Disc radius.</param>
	/// <param name="axis">Orientation axis.</param>
	/// <param name="color">Disc color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Disc3D (Vector3 origin, Quaternion rotation, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.DISC;
		command.m_Verts = IMDrawVertexCount.MESH_DISC;
		command.m_Position = origin;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Size = new Vector3(radius, 1f, radius);
		command.m_Color = color;
		command.m_T = duration;
	}

	#endregion

	#region 3D SPHERE PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireSphere3D (Vector3 center, float radius, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_SPHERE;
		command.m_Lines = IMDrawLineCount.WIRE_SPHERE;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Size.x = radius;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a wireframe 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="rotation">Sphere rotation.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireSphere3D (Vector3 center, Quaternion rotation, float radius, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_SPHERE_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_SPHERE;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Rotation = rotation;
		command.m_Size.x = radius;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Sphere3D (Vector3 center, float radius, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.SPHERE;
		command.m_Verts = IMDrawVertexCount.MESH_SPHERE;
		command.m_Position = center;
		command.m_Rotation = QUATERNION_IDENTITY;
		command.m_Color = color;
		command.m_Size = new Vector3(radius, radius, radius);
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D sphere.
	/// </summary>
	/// <param name="center">Sphere center.</param>
	/// <param name="rotation">Sphere rotation.</param>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="color">Sphere color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Sphere3D (Vector3 center, Quaternion rotation, float radius, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.SPHERE;
		command.m_Verts = IMDrawVertexCount.MESH_SPHERE;
		command.m_Position = center;
		command.m_Rotation = rotation;
		command.m_Color = color;
		command.m_Size = new Vector3(radius, radius, radius);
		command.m_T = duration;
	}

	#endregion

	#region 3D ELLIPSOID PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="size">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireEllipsoid3D (Vector3 center, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_ELLIPSOID;
		command.m_Lines = IMDrawLineCount.WIRE_SPHERE;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Size = size;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a wireframe 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="rotation">Ellipsoid rotation.</param>
	/// <param name="size">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireEllipsoid3D (Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_ELLIPSOID_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_SPHERE;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Rotation = rotation;
		command.m_Size = size;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="size">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ellipsoid3D (Vector3 center, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.SPHERE;
		command.m_Verts = IMDrawVertexCount.MESH_SPHERE;
		command.m_Position = center;
		command.m_Rotation = QUATERNION_IDENTITY;
		command.m_Color = color;
		command.m_Size = size * 0.5f; // Convert to half extents
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D ellipsoid.
	/// </summary>
	/// <param name="center">Ellipsoid center.</param>
	/// <param name="rotation">Ellipsoid rotation.</param>
	/// <param name="size">Ellipsoid size.</param>
	/// <param name="color">Ellipsoid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Ellipsoid3D (Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.SPHERE;
		command.m_Verts = IMDrawVertexCount.MESH_SPHERE;
		command.m_Position = center;
		command.m_Rotation = rotation;
		command.m_Color = color;
		command.m_Size = size * 0.5f; // Convert to half extents
		command.m_T = duration;
	}

	#endregion

	#region 3D CONE PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D cone.
	/// </summary>
	/// <param name="position">Cone position (origin is located at the base).</param>
	/// <param name="rotation">Cone rotation.</param>
	/// <param name="height">Cone height.</param>
	/// <param name="width">Cone base width.</param>
	/// <param name="axis">Cone reference axis.</param>
	/// <param name="color">Cone color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireCone3D(Vector3 position, Quaternion rotation, float height, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_CONE_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_CONE;
		command.m_P1 = position;
		command.m_Rotation = rotation;
		command.m_C1 = color;
		command.m_Size.x = width * 0.5f; // Convert to half width
		command.m_Size.y = height;
		command.m_Axis = axis;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a wireframe 3D cone.
	/// </summary>
	/// <param name="origin">Cone start position.</param>
	/// <param name="direction">Cone direction (assumes direction is normalized).</param>
	/// <param name="length">Cone length.</param>
	/// <param name="angle">Cone spread angle (in degrees).</param>
	/// <param name="color">Cone color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireCone3D(Vector3 origin, Vector3 direction, float length, float angle, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_CONE_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_CONE;
		command.m_P1 = origin + (direction * length);
		command.m_Rotation = Quaternion.LookRotation(-direction);
		command.m_C1 = color;
		command.m_Size.x = command.m_Size.z = length * (float)Math.Tan(angle * Mathf.Deg2Rad * 0.5f);
		command.m_Size.y = length;
		command.m_Axis = IMDrawAxis.Z;
		command.m_T = duration;
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
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Cone3D(Vector3 position, Quaternion rotation, float height, float width, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CONE;
		command.m_Verts = IMDrawVertexCount.MESH_CONE;
		command.m_Position = position;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Color = color;
		command.m_Size.x = command.m_Size.z = width; // Convert to half width
		command.m_Size.y = height;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D cone.
	/// </summary>
	/// <param name="origin">Cone start position.</param>
	/// <param name="direction">Cone direction (assumes direction is normalized).</param>
	/// <param name="length">Cone length.</param>
	/// <param name="angle">Cone spread angle (in degrees).</param>
	/// <param name="color">Cone color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Cone3D(Vector3 origin, Vector3 direction, float length, float angle, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CONE;
		command.m_Verts = IMDrawVertexCount.MESH_CONE;
		command.m_Position = origin + (direction * length);
		Quaternion rotation = Quaternion.LookRotation(-direction);
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)IMDrawAxis.Z);
		command.m_Color = color;
		command.m_Size.x = command.m_Size.z = length * (float)Math.Tan(angle * Mathf.Deg2Rad * 0.5f) * 2f;
		command.m_Size.y = length;
		command.m_T = duration;
	}

	#endregion

	#region 3D CAPSULE PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D capsule.
	/// </summary>
	/// <param name="center">Capsule center.</param>
	/// <param name="rotation">Capsule rotation.</param>
	/// <param name="height">Capsule height.</param>
	/// <param name="radius">Capsule radius.</param>
	/// <param name="axis">Capsule reference axis.</param>
	/// <param name="color">Capsule color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireCapsule3D (Vector3 center, Quaternion rotation, float height, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_CAPSULE_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_CAPSULE;
		command.m_P1 = center;
		command.m_C1 = color;
		command.m_Rotation = rotation;
		command.m_Size.x = height;
		command.m_Size.y = radius;
		command.m_Axis = axis;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D capsule.
	/// </summary>
	/// <param name="center">Capsule center.</param>
	/// <param name="rotation">Capsule rotation.</param>
	/// <param name="height">Capsule height.</param>
	/// <param name="radius">Capsule radius.</param>
	/// <param name="axis">Capsule reference axis.</param>
	/// <param name="color">Capsule color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Capsule3D (Vector3 center, Quaternion rotation, float height, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		if (height <= (radius * 2f)) // If the diameter is equal to or less than the height, then we have a sphere
		{
			height = radius * 2f;
		}

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CAPSULE;
		command.m_Verts = IMDrawVertexCount.MESH_CAPSULE;
		command.m_Position = center;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Size = new Vector3(radius, height, radius);
		command.m_Color = color;
		command.m_T = duration;
	}

	#endregion

	#region 3D CYLINDER PRIMITIVES

	/// <summary>
	/// Draw a wireframe 3D cylinder.
	/// </summary>
	/// <param name="center">Cylinder center.</param>
	/// <param name="rotation">Cylinder rotation.</param>
	/// <param name="height">Cylinder height.</param>
	/// <param name="radius">Cylinder radius.</param>
	/// <param name="axis">Cylinder reference axis.</param>
	/// <param name="color">Cylinder color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void WireCylinder3D (Vector3 center, Quaternion rotation, float height, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_CYLINDER_ROTATED;
		command.m_Lines = IMDrawLineCount.WIRE_CYLINDER;
		command.m_P1 = center;
		command.m_Rotation = rotation;
		command.m_C1 = color;
		command.m_Size.x = height;
		command.m_Size.y = radius;
		command.m_Axis = axis;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a solid 3D cylinder.
	/// </summary>
	/// <param name="center">Cylinder center.</param>
	/// <param name="rotation">Cylinder rotation.</param>
	/// <param name="height">Cylinder height.</param>
	/// <param name="radius">Cylinder radius.</param>
	/// <param name="axis">Cylinder reference axis.</param>
	/// <param name="color">Cylinder color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Cylinder3D (Vector3 center, Quaternion rotation, float height, float radius, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CYLINDER;
		command.m_Verts = IMDrawVertexCount.MESH_CYLINDER;
		command.m_Position = center;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_Size = new Vector3(radius, height, radius);
		command.m_Color = color;
		command.m_T = duration;
	}

	#endregion

	#region SPECIAL 3D PRIMITIVES

	/// <summary>
	/// Draw a 3-dimensional axis (X-axis=red, Y-axis=green, Z-axis=blue).
	/// </summary>
	/// <param name="origin">Axis origin.</param>
	/// <param name="rotation">Axis rotation.</param>
	/// <param name="length">Length of axis lines.</param>
	/// <param name="alpha">Axis transparency.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Axis3D (Vector3 origin, Quaternion rotation, float length, float alpha, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.AXIS;
		command.m_Lines = IMDrawLineCount.AXIS;
		command.m_P1 = origin;
		command.m_Rotation = rotation;
		command.m_Size = new Vector3(length, length, length);
		command.m_C1.a = alpha;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3-dimensional axis (X-axis=red, Y-axis=green, Z-axis=blue).
	/// </summary>
	/// <param name="origin">Axis origin.</param>
	/// <param name="rotation">Axis rotation.</param>
	/// <param name="length">Extents of axis lines.</param>
	/// <param name="alpha">Axis transparency.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Axis3D(Vector3 origin, Quaternion rotation, Vector3 length, float alpha, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.AXIS;
		command.m_Lines = IMDrawLineCount.AXIS;
		command.m_P1 = origin;
		command.m_Rotation = rotation;
		command.m_Size = length;
		command.m_C1.a = alpha;
		command.m_T = duration;
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
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Grid3D (Vector3 origin, Quaternion rotation, float extentX, float extentY, int cellsX, int cellsY, IMDrawAxis axis, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		if (cellsX < 1 || cellsY < 1) // There must be more than one cell in each axis to draw the grid
			return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.GRID_SINGLE_COLOR;
		command.m_Lines = (cellsX + 1) + (cellsY + 1); ;// ((cellsX + 1) * 2) + ((cellsY + 1) * 2);
		command.m_P1 = origin;
		IMDrawRotationHelper.SetRotationAxis(out command.m_Rotation, ref rotation, (int)axis);
		command.m_C1 = color;
		command.m_P2.x = cellsX;
		command.m_P2.y = cellsY;
		command.m_Size.x = extentX;
		command.m_Size.y = extentY;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D grid in the X/Z plane.
	/// </summary>
	/// <param name="origin">Grid origin.</param>
	/// <param name="rotation">Grid orientation.</param>
	/// <param name="extentX">Grid width.</param>
	/// <param name="extentY">Grid height.</param>
	/// <param name="cellsX">Number of cells along the width.</param>
	/// <param name="cellsY">Number of cells along the height.</param>
	/// <param name="color">Grid color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Grid3D (Vector3 origin, Quaternion rotation, float extentX, float extentY, int cellsX, int cellsY, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		if (cellsX < 1 || cellsY < 1) // There must be more than one cell in each axis to draw the grid
			return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.GRID_SINGLE_COLOR;
		command.m_Lines = (cellsX + 1) + (cellsY + 1); //((cellsX + 1) * 2) + ((cellsY + 1) * 2);
		command.m_P1 = origin;
		command.m_Rotation = rotation;
		command.m_C1 = color;
		command.m_P2.x = cellsX;
		command.m_P2.y = cellsY;
		command.m_Size.x = extentX;
		command.m_Size.y = extentY;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D grid point.
	/// </summary>
	/// <param name="origin">Grid point origin.</param>
	/// <param name="rotation">Grid point orientation.</param>
	/// <param name="extents">Grid point extents.</param>
	/// <param name="color">Grid point colour.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void GridPoint3D(Vector3 origin, Quaternion rotation, float extents, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		extents *= 0.5f; // Convert to half extents

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.GRIDPOINT;
		command.m_Lines = IMDrawLineCount.GRIDPOINT;
		command.m_P1 = origin;
		command.m_Rotation = rotation;
		command.m_Size = new Vector3(extents, extents, extents);
		command.m_C1 = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a 3D grid point.
	/// </summary>
	/// <param name="origin">Grid point origin.</param>
	/// <param name="rotation">Grid point orientation.</param>
	/// <param name="extents">Grid point XYZ extents.</param>
	/// <param name="color">Grid point colour.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void GridPoint3D (Vector3 origin, Quaternion rotation, Vector3 extents, Color color, float duration = 0.0f)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.GRIDPOINT;
		command.m_Lines = IMDrawLineCount.GRIDPOINT;
		command.m_P1 = origin;
		command.m_Rotation = rotation;
		command.m_Size = extents * 0.5f;
		command.m_C1 = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw 3D bounds (an axis aligned box).
	/// </summary>
	/// <param name="bounds">Bounds that specifies a position and extents.</param>
	/// <param name="color">Bounds color.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Bounds (Bounds bounds, Color color)
	{
		WireBox3D(bounds.center, bounds.extents * 2.0f, color);
	}

	/// <summary>
	/// Draw the 3D bounds for a renderer (note: only draws bounds if renderer is visible).
	/// </summary>
	/// <param name="renderer">Renderer whose bounds will be drawn.</param>
	/// <param name="color">Bounds color.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Bounds (Renderer renderer, Color color)
	{
		if (renderer != null && renderer.isVisible)
		{
			Bounds bounds = renderer.bounds;
			WireBox3D(bounds.center, bounds.extents * 2.0f, color);
		}
	}

	/// <summary>
	/// Draw a sphere collider shape.
	/// </summary>
	/// <param name="sphereCollider">Target SphereCollider object.</param>
	/// <param name="color">Sphere color.</param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	/// <param name="solid">Specify if a solid shape should be drawn.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (SphereCollider sphereCollider, Color color, float scaleOffset, bool solid)
	{
		if (sphereCollider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.DrawCollider(sphereCollider, ref color, scaleOffset, solid);
	}

	/// <summary>
	/// Draw a box collider shape.
	/// </summary>
	/// <param name="boxCollider">Target BoxCollider object.</param>
	/// <param name="color">Box color.</param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	/// <param name="solid">Specify if a solid shape should be drawn.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (BoxCollider boxCollider, Color color, float scaleOffset, bool solid)
	{
		if (boxCollider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.DrawCollider(boxCollider, ref color, scaleOffset, solid);
	}

	/// <summary>
	/// Draw a capsule collider shape.
	/// </summary>
	/// <param name="capsuleCollider">CapsuleCollider object.</param>
	/// <param name="color">Capsule color.</param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	/// <param name="solid">Specify if a solid shape should be drawn.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (CapsuleCollider capsuleCollider, Color color, float scaleOffset, bool solid)
	{
		if (capsuleCollider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.DrawCollider(capsuleCollider, ref color, scaleOffset, solid);
	}

	/// <summary>
	/// Draw a wheel collider shape. 
	/// </summary>
	/// <param name="wheelCollider">WheelCollider object.</param>
	/// <param name="color">Wheel collider color.</param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	/// <param name="solid">Specify if a solid shape should be drawn.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (WheelCollider wheelCollider, Color color, float scaleOffset, bool solid)
	{
		if (wheelCollider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.DrawCollider(wheelCollider, ref color, scaleOffset, solid);
	}

	/// <summary>
	/// Draw a mesh collider shape.
	/// </summary>
	/// <param name="meshCollider">MeshCollider object.</param>
	/// <param name="color">Mesh color.</param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (MeshCollider meshCollider, Color color, float scaleOffset)
	{
		if (meshCollider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.DrawCollider(meshCollider, ref color, scaleOffset);
	}

	/// <summary>
	/// Draw a collider shape.
	/// </summary>
	/// <param name="collider">Collider object.</param>
	/// <param name="color"></param>
	/// <param name="scaleOffset">Specify a scale offset (0=no scale change). Useful for situations where the visualisation overlaps geometry.</param>
	/// <param name="solid">Specify if a solid shape should be drawn.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Collider (Collider collider, Color color, float scaleOffset, bool solid)
	{
		if (collider == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		Type type = collider.GetType();

		if (type == typeof(MeshCollider))
		{
			IMDrawManager.TargetCamera.DrawCollider((MeshCollider)collider, ref color, scaleOffset);
			return;
		}
		else if (type == typeof(SphereCollider))
		{
			IMDrawManager.TargetCamera.DrawCollider((SphereCollider)collider, ref color, scaleOffset, solid);
			return;
		}
		else if (type == typeof(BoxCollider))
		{
			IMDrawManager.TargetCamera.DrawCollider((BoxCollider)collider, ref color, scaleOffset, solid);
			return;
		}
		else if (type == typeof(CapsuleCollider))
		{
			IMDrawManager.TargetCamera.DrawCollider((CapsuleCollider)collider, ref color, scaleOffset, solid); ;
			return;
		}
		else if (type == typeof(WheelCollider))
		{
			IMDrawManager.TargetCamera.DrawCollider((WheelCollider)collider, ref color, scaleOffset, solid);
			return;
		}

		// Other types of collider are not currently supported so just draw their bounding box
		Bounds bounds = collider.bounds;
		WireBox3D(bounds.center, bounds.extents * 2.0f, color);
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Mesh object.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="color">Mesh color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Mesh (Mesh mesh, Vector3 position, Quaternion rotation, Color color, float duration = 0.0f)
	{
		if (mesh == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CUSTOM_MESH;
		command.m_Verts = mesh.vertexCount;
		command.m_Mesh = mesh;
		command.m_Position = position;
		command.m_Rotation = rotation;
		command.m_Size.Set(1f, 1f, 1f);
		command.m_Color = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="mesh">Mesh object.</param>
	/// <param name="position">Mesh position.</param>
	/// <param name="rotation">Mesh rotation.</param>
	/// <param name="scale"></param>
	/// <param name="color">Mesh color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Mesh (Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0.0f)
	{
		if (mesh == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawMeshCommand command = IMDrawManager.TargetCamera.CreateMeshCommand();
		command.m_Type = IMDrawCommandType.CUSTOM_MESH;
		command.m_Verts = mesh.vertexCount;
		command.m_Mesh = mesh;
		command.m_Position = position;
		command.m_Rotation = rotation;
		command.m_Size = scale;
		command.m_Color = color;
		command.m_T = duration;
	}

	/// <summary>
	/// Draw the coverage volume of a spotlight.
	/// </summary>
	/// <param name="light">Source spotlight.</param>
	/// <param name="wireFrameColor">Wireframe color.</param>
	/// <param name="solidColour">Solid color.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Spotlight (Light light, Color wireFrameColor, Color solidColour)
	{
		if (IMDrawManager.IsLineMeshDrawDisabled || light == null || light.type != LightType.Spot) return;

		float length = light.range;
		float width = length * (float)Math.Tan(light.spotAngle * Mathf.Deg2Rad * 0.5f);// * 2f;
		Vector3 direction = light.transform.forward;
		Vector3 position = light.transform.position + (direction * length);
		Quaternion rotation = Quaternion.LookRotation(-direction);
		
		if (wireFrameColor.a > 0f)
		{
			IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
			command.m_Type = IMDrawCommandType.WIRE_CONE_ROTATED;
			command.m_Lines = IMDrawLineCount.WIRE_CONE;
			command.m_P1 = position;
			//Quaternion rotation = Quaternion.LookRotation(-direction);
			command.m_Rotation = rotation;
			command.m_Axis = IMDrawAxis.Z;
			command.m_C1 = wireFrameColor;
			command.m_Size.x = width;//length * (float)Math.Tan(light.spotAngle * Mathf.Deg2Rad * 0.5f);// * 2f;
			command.m_Size.y = length;
			command.m_T = 0f;
		}

		if (solidColour.a > 0f)
		{
			IMDrawMeshCommand meshCommand = IMDrawManager.TargetCamera.CreateMeshCommand();
			meshCommand.m_Type = IMDrawCommandType.CONE;
			meshCommand.m_Verts = IMDrawVertexCount.MESH_CONE;
			meshCommand.m_Position = position;
			IMDrawRotationHelper.SetRotationAxis(out meshCommand.m_Rotation, ref rotation, (int)IMDrawAxis.Z);
			meshCommand.m_Color = solidColour;
			meshCommand.m_Size.x = meshCommand.m_Size.z = width * 2f;
			meshCommand.m_Size.y = length;
			meshCommand.m_T = 0f;
		}
	}

	/// <summary>
	/// Draw a wireframe representation a camera's view frustum.
	/// </summary>
	/// <param name="camera">Source camera.</param>
	/// <param name="color">Frustum color.</param>
	public static void Frustum (Camera camera, Color color)
	{
		if (camera == null || IMDrawManager.IsLineMeshDrawDisabled) return;

		IMDrawLineCommand command = IMDrawManager.TargetCamera.CreateLineCommand();
		command.m_Type = IMDrawCommandType.WIRE_FRUSTUM;
		command.m_Lines = IMDrawLineCount.WIRE_FRUSTRUM;
		command.m_Camera = camera;
		command.m_C1 = color;
		command.m_T = 0f;
	}

	#endregion

	#region GUI LABELS

	/// <summary>
	/// Draw a 2D label. [Not VR compatible]
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label (float x, float y, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(x, y, ref color, 0, pivot, alignment, 0, label, duration);
	}

	/// <summary>
	/// Draw a 2D label.
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label(float x, float y, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(x, y, ref color, fontSize, pivot, alignment, 0, label, duration);
	}

	/// <summary>
	/// Draw a 2D label with a drop shadow.
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed(float x, float y, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(x, y, ref color, 0, pivot, alignment, IMDrawGUICommandFlag.SHADOWED, label, duration);
	}

	/// <summary>
	/// Draw a 2D label with a drop shadow.
	/// </summary>
	/// <param name="x">Screen X position.</param>
	/// <param name="y">Screen Y position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed(float x, float y, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(x, y, ref color, fontSize, pivot, alignment, IMDrawGUICommandFlag.SHADOWED, label, duration);
	}

	/// <summary>
	/// Draw a 3D label.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label (Vector3 position, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, ref color, 0, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE, label, duration);
	}

	/// <summary>
	/// Draw a 3D label.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label(Vector3 position, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, ref color, fontSize, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE, label, duration);
	}

	/// <summary>
	/// Draw a 3D label with a drop shadow.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed (Vector3 position, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, ref color, 0, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE | IMDrawGUICommandFlag.SHADOWED, label, duration);
	}

	/// <summary>
	/// Draw a 3D label with a drop shadow.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed(Vector3 position, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, ref color, fontSize, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE | IMDrawGUICommandFlag.SHADOWED, label, duration);
	}

	/// <summary>
	/// Draw a 3D label.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="positionOffsetX">Specifies an additional screen X position offset.</param>
	/// <param name="positionOffsetY">Specifies an additional screen Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label (Vector3 position, float positionOffsetX, float positionOffsetY, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, positionOffsetX, positionOffsetY, ref color, 0, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE, label, duration);
	}

	/// <summary>
	/// Draw a 3D label.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="positionOffsetX">Specifies an additional screen X position offset.</param>
	/// <param name="positionOffsetY">Specifies an additional screen Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void Label (Vector3 position, float positionOffsetX, float positionOffsetY, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, positionOffsetX, positionOffsetY, ref color, fontSize, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE, label, duration);
	}

	/// <summary>
	/// Draw a 3D label with a drop shadow.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="positionOffsetX">Specifies an additional screen X position offset.</param>
	/// <param name="positionOffsetY">Specifies an additional screen Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed(Vector3 position, float positionOffsetX, float positionOffsetY, Color color, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, positionOffsetX, positionOffsetY, ref color, 0, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE | IMDrawGUICommandFlag.SHADOWED, label, duration);
	}

	/// <summary>
	/// Draw a 3D label with a drop shadow.
	/// </summary>
	/// <param name="position">Label 3D position.</param>
	/// <param name="positionOffsetX">Specifies an additional screen X position offset.</param>
	/// <param name="positionOffsetY">Specifies an additional screen Y position offset.</param>
	/// <param name="color">Label color.</param>
	/// <param name="fontSize">Font size.</param>
	/// <param name="pivot">Label rectangle pivot.</param>
	/// <param name="alignment">Label text alignment.</param>
	/// <param name="label">Label text.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	//[System.Diagnostics.Conditional("IMDRAW_ENABLED")]
	public static void LabelShadowed(Vector3 position, float positionOffsetX, float positionOffsetY, Color color, int fontSize, LabelPivot pivot, LabelAlignment alignment, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateLabelCommand().Init(position, positionOffsetX, positionOffsetY, ref color, fontSize, pivot, alignment, IMDrawGUICommandFlag.WORLD_SPACE | IMDrawGUICommandFlag.SHADOWED, label, duration);
	}


	/// <summary>
	/// Calculate the width and height of text for the current camera GUI font style.
	/// </summary>
	/// <param name="text">Text to be measured.</param>
	/// <returns>The width & height of the label.</returns>
	public static Vector2 GetLabelSize (string text)
	{
		if (string.IsNullOrEmpty(text) || IMDrawManager.IsGUIDrawDisabled) return Vector2.zero;
		return IMDrawManager.TargetCamera.GetLabelSize(text);
	}

	#endregion // GUI LABELS

	#region 2D RECTANGLE

	/// <summary>
	/// Draw a 2D rectangle outline.
	/// </summary>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="color">Outline color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void RectangleOutline2D (Rect rect, Color color, float duration = 0.0f)
	{
		if (color.a == 0f || (rect.width * rect.height) <= 0f || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateRect2DCommand().InitOutline(rect, color, duration);
	}

	/// <summary>
	/// Draw a filled 2D rectangle.
	/// </summary>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="color">Fill color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void RectangleFilled2D (Rect rect, Color color, float duration = 0.0f)
	{
		if (color.a == 0f || (rect.width * rect.height) <= 0f || IMDrawManager.IsGUIDrawDisabled) return;
		IMDrawManager.TargetCamera.CreateRect2DCommand().InitFilled(rect, color, duration);
	}

	/// <summary>
	/// Draw a 2D rectangle.
	/// </summary>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="fillColor">Fill color.</param>
	/// <param name="outlineColor">Outline color.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void Rectangle2D (Rect rect, Color fillColor, Color outlineColor, float duration = 0.0f)
	{
		if ((rect.width * rect.height) <= 0f  || IMDrawManager.IsGUIDrawDisabled) return;
		
		if (outlineColor.a > 0f) // Outline
		{
			if (fillColor.a > 0f && rect.width > 2f && rect.height > 2f) // Fill inside of outline
			{
				IMDrawManager.TargetCamera.CreateRect2DCommand().InitFilled(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 1f, rect.height - 1f), fillColor, duration);
			}

			IMDrawManager.TargetCamera.CreateRect2DCommand().InitOutline(rect, outlineColor, duration);
		}
		else if (fillColor.a > 0f) // Fill only
		{
			IMDrawManager.TargetCamera.CreateRect2DCommand().InitFilled(rect, fillColor, duration);
		}
	}

	#endregion // 2D RECTANGLE

	#region 2D IMAGE

	/// <summary>
	/// Draw a 2D image.
	/// </summary>
	/// <param name="texture">Source texture.</param>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="texCoord">UV coordinates.</param>
	/// <param name="color">Image color tint.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void Image2D(Texture2D texture, Rect rect, Color color, float duration = 0.0f)
	{
		if (texture == null || color.a == 0f || (rect.width * rect.height) <= 0f || IMDrawManager.IsGUIDrawDisabled) return;

		IMDrawManager.TargetCamera.CreateTexture2DCommand().Init(texture, color, rect, duration);
	}

	/// <summary>
	/// Draw a 2D image.
	/// </summary>
	/// <param name="texture">Source texture.</param>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="texCoord">UV coordinates.</param>
	/// <param name="color">Image color tint.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void Image2D (Texture2D texture, Rect rect, Rect texCoord, Color color, float duration = 0.0f)
	{
		if (texture == null || color.a == 0f || (rect.width * rect.height) <= 0f || IMDrawManager.IsGUIDrawDisabled) return;

		IMDrawManager.TargetCamera.CreateTexture2DCommand().Init(texture, color, rect, texCoord, duration);
	}

	/// <summary>
	/// Draw a 2D image.
	/// </summary>
	/// <param name="texture">Source texture.</param>
	/// <param name="rect">Screen space position, width and height of the rectangle.</param>
	/// <param name="texCoord">UV coordinates.</param>
	/// <param name="color">Image color tint.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void Image2D(Texture2D texture, Rect rect, ScaleMode scaleMode, float aspect, Color color, float duration = 0.0f)
	{
		if (texture == null || color.a == 0f || (rect.width * rect.height) <= 0f || IMDrawManager.IsGUIDrawDisabled) return;

		IMDrawManager.TargetCamera.CreateTexture2DCommand().Init(texture, color, rect, scaleMode, aspect, duration);
	}

	#endregion // 2D IMAGE

	#region TEXT MESH

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float scale, Color color, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, scale, scale, IMDrawTextMeshOption.FACE_CAMERA, duration);
	}

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="alignment">How lines of text are aligned (Left, Right, Center)</param>
	/// <param name="anchor">The pivot position of the text mesh.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float scale, Color color, TextAlignment alignment, TextAnchor anchor, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label,color, position, scale, scale, alignment, anchor, IMDrawTextMeshOption.FACE_CAMERA, duration);
	}

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="widthScale">Text mesh width scale.</param>
	/// <param name="heightScale">Text mesh height scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float widthScale, float heightScale, Color color, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, widthScale, heightScale, IMDrawTextMeshOption.FACE_CAMERA, duration);
	}

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="widthScale">Text mesh width scale.</param>
	/// <param name="heightScale">Text mesh height scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="alignment">How lines of text are aligned (Left, Right, Center)</param>
	/// <param name="anchor">The pivot position of the text mesh.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float widthScale, float heightScale, Color color, TextAlignment alignment, TextAnchor anchor, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, widthScale, heightScale, alignment, anchor, IMDrawTextMeshOption.FACE_CAMERA, duration);
	}

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="fixedScale">Maintain screen space scale.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float scale, Color color, bool fixedScale, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, scale, scale, IMDrawTextMeshOption.FACE_CAMERA | (fixedScale ? IMDrawTextMeshOption.DISTANCE_SCALE : 0), duration);
	}

	/// <summary>
	/// Draw a billboard text mesh. The text mesh will always face the camera.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="alignment">How lines of text are aligned (Left, Right, Center)</param>
	/// <param name="anchor">The pivot position of the text mesh.</param>
	/// <param name="fixedScale">Maintain screen space scale.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMeshBillboard(Vector3 position, float scale, Color color, TextAlignment alignment, TextAnchor anchor, bool fixedScale, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, scale, scale, alignment, anchor, IMDrawTextMeshOption.FACE_CAMERA | (fixedScale ? IMDrawTextMeshOption.DISTANCE_SCALE : 0), duration);
	}

	/// <summary>
	/// Draw a 3D text mesh.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="rotation">Text mesh rotation.</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMesh(Vector3 position, Quaternion rotation, float scale, Color color, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, rotation, scale, scale, 0, duration);
	}

	/// <summary>
	/// Draw a 3D text mesh.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="rotation">Text mesh rotation.</param>
	/// <param name="scale">Text mesh scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="alignment">How lines of text are aligned (Left, Right, Center)</param>
	/// <param name="anchor">The pivot position of the text mesh.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMesh (Vector3 position, Quaternion rotation, float scale, Color color, TextAlignment alignment, TextAnchor anchor, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, rotation, scale, scale, alignment, anchor, 0, duration);
	}

	/// <summary>
	/// Draw a 3D text mesh.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="rotation">Text mesh rotation.</param>
	/// <param name="widthScale">Text mesh width scale.</param>
	/// <param name="heightScale">Text mesh height scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMesh(Vector3 position, Quaternion rotation, float widthScale, float heightScale, Color color, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, rotation, widthScale, heightScale, 0, duration);
	}

	/// <summary>
	/// Draw a 3D text mesh.
	/// </summary>
	/// <param name="position">Text mesh 3D position.</param>
	/// <param name="rotation">Text mesh rotation.</param>
	/// <param name="widthScale">Text mesh width scale.</param>
	/// <param name="heightScale">Text mesh height scale.</param>
	/// <param name="color">Text mesh color.</param>
	/// <param name="alignment">How lines of text are aligned (Left, Right, Center)</param>
	/// <param name="anchor">The pivot position of the text mesh.</param>
	/// <param name="label">Text for the text mesh.</param>
	/// <param name="duration">Draw duration (in seconds). If this is zero, it will draw for only a single frame.</param>
	public static void TextMesh(Vector3 position, Quaternion rotation, float widthScale, float heightScale, Color color, TextAlignment alignment, TextAnchor anchor, string label, float duration = 0.0f)
	{
		if (string.IsNullOrEmpty(label) || IMDrawManager.IsTextMeshDrawDisabled) return;

		IMDrawManager.TargetCamera.TextMeshComponent.DrawTextMesh(label, color, position, rotation, widthScale, heightScale, alignment, anchor, 0, duration);
	}

	#endregion
}
