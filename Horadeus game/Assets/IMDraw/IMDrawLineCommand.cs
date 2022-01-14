using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR


public static class IMDrawLineCount
{
	public const int LINE = 1;
	public const int AXIS = 3;
	public const int GRIDPOINT = 3;
	public const int WIRE_BOX = 12;
	public const int WIRE_PYRAMID = 8;
	public const int WIRE_RHOMBUS = 12;
	public const int WIRE_DISC = 24;
	public const int WIRE_SPHERE = 72;
	public const int WIRE_ELLIPSOID = 72;
	public const int WIRE_CONE = 28; // 2 discs + 4 lines
	public const int WIRE_CAPSULE = 100; // 1 cylinder + 2 discs
	public const int WIRE_CYLINDER = 52; // 2 discs + 4 lines
	public const int WIRE_FRUSTRUM = 12;
}



public class IMDrawLineCommand : IMDrawCommand
{
	private static Color					COLOR_XAXIS = new Color(1.0f, 0.25f, 0.25f, 1.0f);
	private static Color					COLOR_YAXIS = new Color(0.25f, 1.0f, 0.25f, 1.0f);
	private static Color					COLOR_ZAXIS = new Color(0.25f, 0.5f, 1.0f, 1.0f);

	public IMDrawCommandType				m_Type;
	public int								m_Lines; // Lines required for this primitive
	public Vector3							m_P1, m_P2, m_Size;
	public Color							m_C1, m_C2;
	public Quaternion						m_Rotation;
	public IMDrawAxis						m_Axis;

	public Camera							m_Camera; // Used for frustum draw requests
	
	public LinkedListNode<IMDrawLineCommand>	m_ListNode;

	//private Vector3[]						m_VertexArray; // Reference to input vertex array
	//private Color[]							m_ColorArray; // Reference to input color array

	private static float[]					m_Sine;
	private static float[]					m_Cosine;

	private static float					m_TXx, m_TXy, m_TXz, m_TYx, m_TYy, m_TYz, m_TZx, m_TZy, m_TZz;

	static IMDrawLineCommand ()
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
			index++;
		}
	}

	public IMDrawLineCommand()
	{
		m_ListNode = new LinkedListNode<IMDrawLineCommand>(this);
	}

	public IMDrawLineCommand Dispose ()
	{
		m_Camera = null; // Remove reference to camera
		return this;
	}

	public void InitTransform()
	{
		float num = m_Rotation.x * 2.0f;
		float num2 = m_Rotation.y * 2.0f;
		float num3 = m_Rotation.z * 2.0f;

		m_TXx = 1.0f - ((m_Rotation.y * num2) + (m_Rotation.z * num3));
		m_TXy = (m_Rotation.x * num2) + (m_Rotation.w * num3);
		m_TXz = (m_Rotation.x * num3) - (m_Rotation.w * num2);

		m_TYx = (m_Rotation.x * num2) - (m_Rotation.w * num3);
		m_TYy = 1.0f - ((m_Rotation.x * num) + (m_Rotation.z * num3));
		m_TYz = (m_Rotation.y * num3) + (m_Rotation.w * num);

		m_TZx = (m_Rotation.x * num3) + (m_Rotation.w * num2);
		m_TZy = (m_Rotation.y * num3) - (m_Rotation.w * num);
		m_TZz = 1.0f - ((m_Rotation.x * num) + (m_Rotation.y * num2));
	}

	private Vector3 LocalToWorld (ref Vector3 localPos)
	{
		return new Vector3(
			m_P1.x + m_TXx * localPos.x + m_TYx * localPos.y + m_TZx * localPos.z,
			m_P1.y + m_TXy * localPos.x + m_TYy * localPos.y + m_TZy * localPos.z,
			m_P1.z + m_TXz * localPos.x + m_TYz * localPos.y + m_TZz * localPos.z);
	}

	private Vector3 LocalToWorld (Vector3 localPos)
	{
		return new Vector3(
			m_P1.x + m_TXx * localPos.x + m_TYx * localPos.y + m_TZx * localPos.z,
			m_P1.y + m_TXy * localPos.x + m_TYy * localPos.y + m_TZy * localPos.z,
			m_P1.z + m_TXz * localPos.x + m_TYz * localPos.y + m_TZz * localPos.z);
	}

	private Vector3 LocalToWorld (float x, float y, float z)
	{
		return new Vector3(
			m_P1.x + m_TXx * x + m_TYx * y + m_TZx * z,
			m_P1.y + m_TXy * x + m_TYy * y + m_TZy * z,
			m_P1.z + m_TXz * x + m_TYz * y + m_TZz * z);
	}


	private void SizeToDirections(out Vector3 right, out Vector3 up, out Vector3 forward)
	{
		float num = m_Rotation.x * 2.0f;
		float num2 = m_Rotation.y * 2.0f;
		float num3 = m_Rotation.z * 2.0f;

		right.x = m_Size.x * (1.0f - ((m_Rotation.y * num2) + (m_Rotation.z * num3)));
		right.y = m_Size.x * ((m_Rotation.x * num2) + (m_Rotation.w * num3));
		right.z = m_Size.x * ((m_Rotation.x * num3) - (m_Rotation.w * num2));

		up.x = m_Size.y * ((m_Rotation.x * num2) - (m_Rotation.w * num3));
		up.y = m_Size.y * (1.0f - ((m_Rotation.x * num) + (m_Rotation.z * num3)));
		up.z = m_Size.y * ((m_Rotation.y * num3) + (m_Rotation.w * num));

		forward.x = m_Size.z * ((m_Rotation.x * num3) + (m_Rotation.w * num2));
		forward.y = m_Size.z * ((m_Rotation.y * num3) - (m_Rotation.w * num));
		forward.z = m_Size.z * (1.0f - ((m_Rotation.x * num) + (m_Rotation.y * num2)));
	}

	public float GetDistSqrd(ref Vector3 position)
	{
		float dx, dy, dz;

		if (m_Type == IMDrawCommandType.LINE)
		{
			dx = ((m_P1.x + m_P2.x) * 0.5f) - position.x;
			dy = ((m_P1.y + m_P2.y) * 0.5f) - position.y;
			dz = ((m_P1.z + m_P2.z) * 0.5f) - position.z;
		}
		else
		{
			dx = m_P1.x - position.x;
			dy = m_P1.y - position.y;
			dz = m_P1.z - position.z;
		}

		return dx * dx + dy * dy + dz * dz;
	}

	public void InitLine (Vector3 p1, Vector3 p2, ref Color color)
	{
		m_Type = IMDrawCommandType.LINE;
		m_Lines = IMDrawLineCount.LINE;
		m_P1 = p1;
		m_P2 = p2;
		m_C1 = color;
		m_C2 = color;
		m_T = 0f;
    }

	public void InitLine(Vector3 p1, Vector3 p2, ref Color color, float duration)
	{
		m_Type = IMDrawCommandType.LINE;
		m_Lines = IMDrawLineCount.LINE;
		m_P1 = p1;
		m_P2 = p2;
		m_C1 = color;
		m_C2 = color;
		m_T = duration;
	}

	public void InitLine (Vector3 p1, Vector3 p2, ref Color c1, ref Color c2)
	{
		m_Type = IMDrawCommandType.LINE;
		m_Lines = IMDrawLineCount.LINE;
		m_P1 = p1;
		m_P2 = p2;
		m_C1 = c1;
		m_C2 = c2;
		m_T = 0f;
	}

	public void InitLine(Vector3 p1, Vector3 p2, ref Color c1, ref Color c2, float duration)
	{
		m_Type = IMDrawCommandType.LINE;
		m_Lines = IMDrawLineCount.LINE;
		m_P1 = p1;
		m_P2 = p2;
		m_C1 = c1;
		m_C2 = c2;
		m_T = duration;
	}

	public void Draw (IMDrawLineRenderer renderer)
	{
		switch (m_Type)
		{
			case IMDrawCommandType.LINE: DrawLine(renderer); break;
			case IMDrawCommandType.AXIS: DrawAxis(renderer); break;
			case IMDrawCommandType.GRID_SINGLE_COLOR: DrawGridSingleColor(renderer); break;
			//case IMDrawCommandType.GRID_DOUBLE_COLOR: DrawGridDoubleColor(renderer); break;
			case IMDrawCommandType.GRIDPOINT: DrawGridPoint(renderer); break;
			case IMDrawCommandType.WIRE_PYRAMID_ROTATED: DrawPyramid(renderer); break;
			case IMDrawCommandType.WIRE_RHOMBUS_ROTATED: DrawRhombus(renderer); break;
			case IMDrawCommandType.WIRE_BOX: DrawBox(renderer); break;
			case IMDrawCommandType.WIRE_BOX_ROTATED: DrawBoxRotated(renderer); break;
			case IMDrawCommandType.WIRE_DISC_ROTATED: DrawDiscRotated(renderer); break;
			case IMDrawCommandType.WIRE_SPHERE: DrawSphere(renderer); break;
			case IMDrawCommandType.WIRE_SPHERE_ROTATED: DrawSphereRotated(renderer, m_Size.x); break;
			case IMDrawCommandType.WIRE_ELLIPSOID: DrawEllipsoid(renderer); break;
			case IMDrawCommandType.WIRE_ELLIPSOID_ROTATED: DrawEllipsoidRotated(renderer); break;
			case IMDrawCommandType.WIRE_CONE_ROTATED: DrawConeRotated(renderer); break;
			case IMDrawCommandType.WIRE_CAPSULE_ROTATED: DrawCapsuleRotated(renderer); break;
			case IMDrawCommandType.WIRE_CYLINDER_ROTATED: DrawCylinderRotated(renderer); break;
			case IMDrawCommandType.WIRE_FRUSTUM: DrawFrustum(renderer); break;
		}
	}

	// Helper function for setting color with modified alpha
	private static void SetColor(ref Color source, Color target, float alpha)
	{
		source.r = target.r;
		source.g = target.g;
		source.b = target.b;
		source.a = alpha;
	}

	// Helper function for drawing a local space line in world space (assumes m_P1 is origin and transform values have been initialised)
	// Alternative to Line(renderer, Transform(ref p1), Transform(ref p2))
	private void LineTransform(IMDrawLineRenderer renderer, Vector3 p1, Vector3 p2)
	{
		renderer.DrawLine(
			new Vector3(
				m_P1.x + m_TXx * p1.x + m_TYx * p1.y + m_TZx * p1.z,
				m_P1.y + m_TXy * p1.x + m_TYy * p1.y + m_TZy * p1.z,
				m_P1.z + m_TXz * p1.x + m_TYz * p1.y + m_TZz * p1.z),
			new Vector3(
				m_P1.x + m_TXx * p2.x + m_TYx * p2.y + m_TZx * p2.z,
				m_P1.y + m_TXy * p2.x + m_TYy * p2.y + m_TZy * p2.z,
				m_P1.z + m_TXz * p2.x + m_TYz * p2.y + m_TZz * p2.z),
				m_C1);
	}

	private void DrawLine(IMDrawLineRenderer renderer)
	{
		renderer.DrawLine(m_P1, m_P2, m_C1, m_C2);
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = length
	private void DrawAxis(IMDrawLineRenderer renderer)
	{
		COLOR_XAXIS.a = m_C1.a;
		renderer.DrawLine(m_P1, m_P1 + (m_Rotation * new Vector3(m_Size.x, 0f, 0f)), COLOR_XAXIS);

		COLOR_YAXIS.a = m_C1.a;
		renderer.DrawLine(m_P1, m_P1 + (m_Rotation * new Vector3(0f, m_Size.y, 0f)), COLOR_YAXIS);

		COLOR_ZAXIS.a = m_C1.a;
		renderer.DrawLine(m_P1, m_P1 + (m_Rotation * new Vector3(0f, 0f, m_Size.z)), COLOR_ZAXIS);
	}

	private void DrawGridSingleColor(IMDrawLineRenderer renderer)
	{
		InitTransform();

		int cellsX = Convert.ToInt32(m_P2.x);
		int cellsY = Convert.ToInt32(m_P2.y);

		float endX = m_Size.x / 2f;
		float endZ = m_Size.y / 2f;

		float startX = -endX;
		float startZ = -endZ;

		Vector3 p0, p1;
		p0.y = p1.y = 0f;
		float v, vStep;

		v = startX;
		vStep = m_Size.x / m_P2.x;
		p0.z = startZ;
		p1.z = endZ;

		for (int i = 0; i <= cellsX; ++i)
		{
			p0.x = p1.x = v;
			v += vStep;

			renderer.DrawLine(LocalToWorld(ref p0), LocalToWorld(ref p1), m_C1);
		}

		v = startZ;
		vStep = m_Size.y / m_P2.y;
		p0.x = startX;
		p1.x = endX;

		for (int i = 0; i <= cellsY; ++i)
		{
			p0.z = p1.z = v;
			v += vStep;

			renderer.DrawLine(LocalToWorld(ref p0), LocalToWorld(ref p1), m_C1);
		}
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = half extents
	// C1 = colour
	private void DrawGridPoint (IMDrawLineRenderer renderer)
	{
		Vector3 offset;

		// Draw X axis
		offset = m_Rotation * new Vector3(m_Size.x, 0f, 0f);
		renderer.DrawLine(m_P1 - offset, m_P1 + offset, m_C1);

		// Draw Y axis
		offset = m_Rotation * new Vector3(0f, m_Size.y, 0f);
		renderer.DrawLine(m_P1 - offset, m_P1 + offset, m_C1);

		// Draw Z axis
		offset = m_Rotation * new Vector3(0f, 0f, m_Size.z);
		renderer.DrawLine(m_P1 - offset, m_P1 + offset, m_C1);
	}

	// P1 = origin
	// Size.x = base extents
	// Size.y = height
	// C1 = colour
	private void DrawPyramid (IMDrawLineRenderer renderer)
	{
		InitTransform();

		Vector3 p1 = LocalToWorld(new Vector3(m_Size.x, 0f, m_Size.x));
		Vector3 p2 = LocalToWorld(new Vector3(-m_Size.x, 0f, m_Size.x));
		Vector3 p3 = LocalToWorld(new Vector3(-m_Size.x, 0f, -m_Size.x));
		Vector3 p4 = LocalToWorld(new Vector3(m_Size.x, 0f, -m_Size.x));
		Vector3 p5 = LocalToWorld(new Vector3(0f, m_Size.y, 0f));

		// Base
		renderer.DrawLine(p1, p2, m_C1);
		renderer.DrawLine(p2, p3, m_C1);
		renderer.DrawLine(p3, p4, m_C1);
		renderer.DrawLine(p4, p1, m_C1);

		// Length
		renderer.DrawLine(p1, p5, m_C1);
		renderer.DrawLine(p2, p5, m_C1);
		renderer.DrawLine(p3, p5, m_C1);
		renderer.DrawLine(p4, p5, m_C1);

	}

	// P1 = origin
	// Size = half extents
	// C1 = colour
	private void DrawRhombus(IMDrawLineRenderer renderer)
	{
		InitTransform();

		Vector3 p1 = LocalToWorld(new Vector3(m_Size.x, 0f, m_Size.x));
		Vector3 p2 = LocalToWorld(new Vector3(-m_Size.x, 0f, m_Size.x));
		Vector3 p3 = LocalToWorld(new Vector3(-m_Size.x, 0f, -m_Size.x));
		Vector3 p4 = LocalToWorld(new Vector3(m_Size.x, 0f, -m_Size.x));
		Vector3 p5 = LocalToWorld(new Vector3(0f, m_Size.y, 0f));

		// Base
		renderer.DrawLine(p1, p2, m_C1);
		renderer.DrawLine(p2, p3, m_C1);
		renderer.DrawLine(p3, p4, m_C1);
		renderer.DrawLine(p4, p1, m_C1);

		// Up length
		renderer.DrawLine(p1, p5, m_C1);
		renderer.DrawLine(p2, p5, m_C1);
		renderer.DrawLine(p3, p5, m_C1);
		renderer.DrawLine(p4, p5, m_C1);

		// Down length
		p5 = LocalToWorld(new Vector3(0f, -m_Size.y, 0f));
		renderer.DrawLine(p1, p5, m_C1);
		renderer.DrawLine(p2, p5, m_C1);
		renderer.DrawLine(p3, p5, m_C1);
		renderer.DrawLine(p4, p5, m_C1);
	}

	// P1 = origin
	// Size = box half extents
	// C1 = colour
	private void DrawBox(IMDrawLineRenderer renderer)
	{
		Vector3 p1, p2;

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y - m_Size.y; p2.z = m_P1.z - m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x - m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z - m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x - m_Size.x; p2.y = m_P1.y - m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x + m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z - m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x + m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y - m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y + m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z - m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y + m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x - m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z + m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y - m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z + m_Size.z;
		p2.x = m_P1.x - m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x - m_Size.x; p1.y = m_P1.y + m_Size.y; p1.z = m_P1.z + m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x + m_Size.x; p1.y = m_P1.y - m_Size.y; p1.z = m_P1.z + m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);

		p1.x = m_P1.x + m_Size.x; p1.y = m_P1.y + m_Size.y; p1.z = m_P1.z - m_Size.z;
		p2.x = m_P1.x + m_Size.x; p2.y = m_P1.y + m_Size.y; p2.z = m_P1.z + m_Size.z;
		renderer.DrawLine(p1, p2, m_C1);
	}

	// P1 = origin
	// Rotation = rotation
	// Size = box half extents
	// C1 = colour
	private void DrawBoxRotated(IMDrawLineRenderer renderer)
	{
		Vector3 sx, sy, sz;
		SizeToDirections(out sx, out sy, out sz);

		Vector3 max = m_P1 + sx + sy + sz;
		Vector3 min = m_P1 - sx - sy - sz;

		renderer.DrawLine(min, m_P1 + sx - sy - sz, m_C1);
		renderer.DrawLine(min, m_P1 - sx + sy - sz, m_C1);
		renderer.DrawLine(min, m_P1 - sx - sy + sz, m_C1);
		renderer.DrawLine(m_P1 + sx - sy - sz, m_P1 + sx + sy - sz, m_C1);
		renderer.DrawLine(m_P1 + sx - sy - sz, m_P1 + sx - sy + sz, m_C1);
		renderer.DrawLine(m_P1 - sx + sy - sz, m_P1 + sx + sy - sz, m_C1);
		renderer.DrawLine(m_P1 - sx + sy - sz, m_P1 - sx + sy + sz, m_C1);
		renderer.DrawLine(m_P1 + sx - sy + sz, m_P1 - sx - sy + sz, m_C1);
		renderer.DrawLine(m_P1 - sx - sy + sz, m_P1 - sx + sy + sz, m_C1);
		renderer.DrawLine(max, m_P1 - sx + sy + sz, m_C1);
		renderer.DrawLine(m_P1 + sx - sy + sz, max, m_C1);
		renderer.DrawLine(m_P1 + sx + sy - sz, max, m_C1);
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = radius 
	// C1 = colour
	private void DrawDiscRotated(IMDrawLineRenderer renderer)
	{
		InitTransform();

		Vector3 r, s;
		float radius = m_Size.x;
		int index = 0, lastIndex = m_Sine.Length - 1;

		switch (m_Axis)
		{
			case IMDrawAxis.X:
				{
					while (index < lastIndex)
					{
						r.x = 0f;
						r.y = m_Sine[index] * radius;
						r.z = m_Cosine[index] * radius;

						++index;

						s.x = 0f;
						s.y = m_Sine[index] * radius;
						s.z = m_Cosine[index] * radius;

						LineTransform(renderer, r, s);
					}
				}
				break;

			case IMDrawAxis.Y:
				{
					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = 0f;
						r.z = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = 0f;
						s.z = m_Cosine[index] * radius;

						LineTransform(renderer, r, s);
					}
				}
				break;

			case IMDrawAxis.Z:
				{
					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;
						r.z = 0f;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;
						s.z = 0f;

						LineTransform(renderer, r, s);
					}
				}
				break;
		}
	}


	// P1 = origin
	// Size.x = radius
	// C1 = colour
	private void DrawSphere(IMDrawLineRenderer renderer)
	{
		Vector3 p1, p2;
		Vector2 r, s;

		float radius = m_Size.x;

		int index = 0, lastIndex = m_Sine.Length - 1;

		while (index < lastIndex)
		{
			r.x = m_Sine[index] * radius;
			r.y = m_Cosine[index] * radius;

			++index;

			s.x = m_Sine[index] * radius;
			s.y = m_Cosine[index] * radius;

			p1.x = m_P1.x + r.x; p1.y = m_P1.y + r.y; p1.z = m_P1.z;
			p2.x = m_P1.x + s.x; p2.y = m_P1.y + s.y; p2.z = m_P1.z;
			renderer.DrawLine(p1, p2, m_C1);

			p1.x = m_P1.x; p1.y = m_P1.y + r.x; p1.z = m_P1.z + r.y;
			p2.x = m_P1.x; p2.y = m_P1.y + s.x; p2.z = m_P1.z + s.y;
			renderer.DrawLine(p1, p2, m_C1);

			p1.x = m_P1.x + r.x; p1.y = m_P1.y; p1.z = m_P1.z + r.y;
			p2.x = m_P1.x + s.x; p2.y = m_P1.y; p2.z = m_P1.z + s.y;
			renderer.DrawLine(p1, p2, m_C1);
		}
	}


	// P1 = origin
	// Rotation = rotation
	// Size.x = radius
	// C1 = colour
	private void DrawSphereRotated(IMDrawLineRenderer renderer, float radius)
	{
		InitTransform();

		Vector3 p1, p2;
		Vector2 r, s;

		int index = 0, lastIndex = m_Sine.Length - 1;

		while (index < lastIndex)
		{
			r.x = m_Sine[index] * radius;
			r.y = m_Cosine[index] * radius;

			++index;

			s.x = m_Sine[index] * radius;
			s.y = m_Cosine[index] * radius;

			p1.x = r.x; p1.y = r.y; p1.z = 0.0f;
			p2.x = s.x; p2.y = s.y; p2.z = 0.0f;
			LineTransform(renderer, p1, p2);

			p1.x = 0.0f; p1.y = r.x; p1.z = r.y;
			p2.x = 0.0f; p2.y = s.x; p2.z = s.y;
			LineTransform(renderer, p1, p2);

			p1.x = r.x; p1.y = 0.0f; p1.z = r.y;
			p2.x = s.x; p2.y = 0.0f; p2.z = s.y;
			LineTransform(renderer, p1, p2);
		}
	}

	// P1 = origin
	// Size = extents
	// C1 = colour
	private void DrawEllipsoid(IMDrawLineRenderer renderer)
	{
		Vector3 size = m_Size * 0.5f;

		Vector3 p1, p2;
		Vector2 r, s;

		int index = 0, lastIndex = m_Sine.Length - 1;

		while (index < lastIndex)
		{
			r.x = m_Sine[index];
			r.y = m_Cosine[index];

			++index;

			s.x = m_Sine[index];
			s.y = m_Cosine[index];

			p1.x = r.x * size.x; p1.y = r.y * size.y; p1.z = 0.0f;
			p2.x = s.x * size.x; p2.y = s.y * size.y; p2.z = 0.0f;
			renderer.DrawLine(m_P1 + p1, m_P1 + p2, m_C1);

			p1.x = 0.0f; p1.y = r.x * size.y; p1.z = r.y * size.z;
			p2.x = 0.0f; p2.y = s.x * size.y; p2.z = s.y * size.z;
			renderer.DrawLine(m_P1 + p1, m_P1 + p2, m_C1);

			p1.x = r.x * size.x; p1.y = 0.0f; p1.z = r.y * size.z;
			p2.x = s.x * size.x; p2.y = 0.0f; p2.z = s.y * size.z;
			renderer.DrawLine(m_P1 + p1, m_P1 + p2, m_C1);
		}
	}

	// P1 = origin
	// Rotation = rotation
	// Size = extents
	// C1 = colour
	private void DrawEllipsoidRotated (IMDrawLineRenderer renderer)
	{
		InitTransform();

		Vector3 size = m_Size * 0.5f;

		Vector3 p1, p2;
		Vector2 r, s;

		int index = 0, lastIndex = m_Sine.Length - 1;

		while (index < lastIndex)
		{
			r.x = m_Sine[index];
			r.y = m_Cosine[index];

			++index;

			s.x = m_Sine[index];
			s.y = m_Cosine[index];

			p1.x = r.x * size.x; p1.y = r.y * size.y; p1.z = 0.0f;
			p2.x = s.x * size.x; p2.y = s.y * size.y; p2.z = 0.0f;
			renderer.DrawLine(LocalToWorld(ref p1), LocalToWorld(ref p2), m_C1);

			p1.x = 0.0f; p1.y = r.x * size.y; p1.z = r.y * size.z;
			p2.x = 0.0f; p2.y = s.x * size.y; p2.z = s.y * size.z;
			renderer.DrawLine(LocalToWorld(ref p1), LocalToWorld(ref p2), m_C1);

			p1.x = r.x * size.x; p1.y = 0.0f; p1.z = r.y * size.z;
			p2.x = s.x * size.x; p2.y = 0.0f; p2.z = s.y * size.z;
			renderer.DrawLine(LocalToWorld(ref p1), LocalToWorld(ref p2), m_C1);
		}
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = radius
	// Size.y = length
	// C1 = colour
	private void DrawConeRotated(IMDrawLineRenderer renderer)
	{
		InitTransform();

		Vector3 r, s;
		float radius = m_Size.x;
		int index = 0, lastIndex = m_Sine.Length - 1;

		switch (m_Axis)
		{
			case IMDrawAxis.X:
				{
					// Draw base
					while (index < lastIndex)
					{
						r.x = 0f;
						r.y = m_Sine[index] * radius;
						r.z = m_Cosine[index] * radius;

						++index;

						s.x = 0f;
						s.y = m_Sine[index] * radius;
						s.z = m_Cosine[index] * radius;

						LineTransform(renderer, r, s);
					}

					// Draw sides
					Vector3 p = LocalToWorld(m_Size.y, 0f, 0f);
					renderer.DrawLine(p, LocalToWorld(0f, m_Size.x, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, -m_Size.x, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, 0f, m_Size.x), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, 0f, -m_Size.x), m_C1);
				}
				break;

			case IMDrawAxis.Y:
				{
					// Draw base
					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = 0f;
						r.z = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = 0f;
						s.z = m_Cosine[index] * radius;

						LineTransform(renderer, r, s);
					}

					// Draw sides
					Vector3 p = LocalToWorld(0f, m_Size.y, 0f);
					renderer.DrawLine(p, LocalToWorld(m_Size.x, 0f, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(-m_Size.x, 0f, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, 0f, m_Size.x), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, 0f, -m_Size.x), m_C1);
				}
				break;

			case IMDrawAxis.Z:
				{
					// Draw base
					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;
						r.z = 0f;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;
						s.z = 0f;

						LineTransform(renderer, r, s);
					}

					// Draw sides
					Vector3 p = LocalToWorld(0f, 0f, m_Size.y);
					renderer.DrawLine(p, LocalToWorld(m_Size.x, 0f, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(-m_Size.x, 0f, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, m_Size.x, 0f), m_C1);
					renderer.DrawLine(p, LocalToWorld(0f, -m_Size.x, 0f), m_C1);
				}
				break;
		}
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = height
	// Size.y = diameter
	// C1 = colour
	private void DrawCapsuleRotated(IMDrawLineRenderer renderer)
	{
		float diameter = m_Size.y * 2.0f;
		float height = m_Size.x;

		if (height <= diameter) // If the diameter is equal to or less than the height, then we revert to a sphere
		{
			DrawSphereRotated(renderer, m_Size.y);
			return;
		}

		InitTransform();

		float radius = m_Size.y;
		float mid = height - diameter;
		float halfMid = mid * 0.5f;

		Vector3 p1, p2;
		Vector2 r, s;

		int index = 0, lastIndex;

		switch (m_Axis)
		{
			case IMDrawAxis.Z:
				{
					p1.x = radius; p1.y = 0.0f; p1.z = halfMid;
					p2.x = radius; p2.y = 0.0f; p2.z = -halfMid;
					LineTransform(renderer, p1, p2);

					p1.x = -radius; p1.y = 0.0f; p1.z = halfMid;
					p2.x = -radius; p2.y = 0.0f; p2.z = -halfMid;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = radius; p1.z = halfMid;
					p2.x = 0.0f; p2.y = radius; p2.z = -halfMid;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = -radius; p1.z = halfMid;
					p2.x = 0.0f; p2.y = -radius; p2.z = -halfMid;
					LineTransform(renderer, p1, p2);

					lastIndex = m_Sine.Length / 2;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = 0.0f; p1.y = r.x; p1.z = r.y + halfMid;
						p2.x = 0.0f; p2.y = s.x; p2.z = s.y + halfMid;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);

						p1.x = r.x; p1.y = 0.0f; p1.z = r.y + halfMid;
						p2.x = s.x; p2.y = 0.0f; p2.z = s.y + halfMid;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}

					index = 0;
					lastIndex = m_Sine.Length - 1;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = r.x; p1.y = r.y; p1.z = halfMid;
						p2.x = s.x; p2.y = s.y; p2.z = halfMid;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;

			case IMDrawAxis.Y:
				{
					p1.x = radius; p1.y = halfMid; p1.z = 0.0f;
					p2.x = radius; p2.y = -halfMid; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = -radius; p1.y = halfMid; p1.z = 0.0f;
					p2.x = -radius; p2.y = -halfMid; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = halfMid; p1.z = radius;
					p2.x = 0.0f; p2.y = -halfMid; p2.z = radius;
					LineTransform(renderer, p1,  p2);

					p1.x = 0.0f; p1.y = halfMid; p1.z = -radius;
					p2.x = 0.0f; p2.y = -halfMid; p2.z = -radius;
					LineTransform(renderer, p1, p2);

					lastIndex = m_Sine.Length / 2;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = 0.0f; p1.y = r.y + halfMid; p1.z = r.x;
						p2.x = 0.0f; p2.y = s.y + halfMid; p2.z = s.x;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);

						p1.x = r.x; p1.y = r.y + halfMid; p1.z = 0.0f;
						p2.x = s.x; p2.y = s.y + halfMid; p2.z = 0.0f;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}

					index = 0;
					lastIndex = m_Sine.Length - 1;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = r.x; p1.y = halfMid; p1.z = r.y;
						p2.x = s.x; p2.y = halfMid; p2.z = s.y;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;

			case IMDrawAxis.X:
				{
					p1.x = halfMid; p1.y = radius; p1.z = 0.0f;
					p2.x = -halfMid; p2.y = radius; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = halfMid; p1.y = -radius; p1.z = 0.0f;
					p2.x = -halfMid; p2.y = -radius; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = halfMid; p1.y = 0.0f; p1.z = radius;
					p2.x = -halfMid; p2.y = 0.0f; p2.z = radius;
					LineTransform(renderer, p1, p2);

					p1.x = halfMid; p1.y = 0.0f; p1.z = -radius;
					p2.x = -halfMid; p2.y = 0.0f; p2.z = -radius;
					LineTransform(renderer, p1, p2);

					lastIndex = m_Sine.Length / 2;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = r.y + halfMid; p1.y = 0.0f; p1.z = r.x;
						p2.x = s.y + halfMid; p2.y = 0.0f; p2.z = s.x;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);

						p1.x = r.y + halfMid; p1.y = r.x; p1.z = 0.0f;
						p2.x = s.y + halfMid; p2.y = s.x; p2.z = 0.0f;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}

					index = 0;
					lastIndex = m_Sine.Length - 1;

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = halfMid; p1.y = r.x; p1.z = r.y;
						p2.x = halfMid; p2.y = s.x; p2.z = s.y;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;
		}
	}

	// P1 = origin
	// Rotation = rotation
	// Size.x = length
	// Size.y = radius
	// C1 = colour
	private void DrawCylinderRotated(IMDrawLineRenderer renderer)
	{
		InitTransform();

		float length = m_Size.x;
		float halfLength = length * 0.5f;
		float radius = m_Size.y;

		Vector3 p1, p2;
		Vector2 r, s;

		int index = 0, lastIndex = m_Sine.Length - 1;

		switch (m_Axis)
		{
			case IMDrawAxis.Z:
				{
					p1.x = radius; p1.y = 0.0f; p1.z = halfLength;
					p2.x = radius; p2.y = 0.0f; p2.z = -halfLength;
					LineTransform(renderer, p1, p2);

					p1.x = -radius; p1.y = 0.0f; p1.z = halfLength;
					p2.x = -radius; p2.y = 0.0f; p2.z = -halfLength;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = radius; p1.z = halfLength;
					p2.x = 0.0f; p2.y = radius; p2.z = -halfLength;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = -radius; p1.z = halfLength;
					p2.x = 0.0f; p2.y = -radius; p2.z = -halfLength;
					LineTransform(renderer, p1, p2);

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = r.x; p1.y = r.y; p1.z = halfLength;
						p2.x = s.x; p2.y = s.y; p2.z = halfLength;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;

			case IMDrawAxis.Y:
				{
					p1.x = radius; p1.y = halfLength; p1.z = 0.0f;
					p2.x = radius; p2.y = -halfLength; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = -radius; p1.y = halfLength; p1.z = 0.0f;
					p2.x = -radius; p2.y = -halfLength; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = halfLength; p1.z = radius;
					p2.x = 0.0f; p2.y = -halfLength; p2.z = radius;
					LineTransform(renderer, p1, p2);

					p1.x = 0.0f; p1.y = halfLength; p1.z = -radius;
					p2.x = 0.0f; p2.y = -halfLength; p2.z = -radius;
					LineTransform(renderer, p1, p2);

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = r.x; p1.y = halfLength; p1.z = r.y;
						p2.x = s.x; p2.y = halfLength; p2.z = s.y;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;

			case IMDrawAxis.X:
				{
					p1.x = halfLength; p1.y = radius; p1.z = 0.0f;
					p2.x = -halfLength; p2.y = radius; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = halfLength; p1.y = -radius; p1.z = 0.0f;
					p2.x = -halfLength; p2.y = -radius; p2.z = 0.0f;
					LineTransform(renderer, p1, p2);

					p1.x = halfLength; p1.y = 0.0f; p1.z = radius;
					p2.x = -halfLength; p2.y = 0.0f; p2.z = radius;
					LineTransform(renderer, p1, p2);

					p1.x = halfLength; p1.y = 0.0f; p1.z = -radius;
					p2.x = -halfLength; p2.y = 0.0f; p2.z = -radius;
					LineTransform(renderer, p1, p2);

					while (index < lastIndex)
					{
						r.x = m_Sine[index] * radius;
						r.y = m_Cosine[index] * radius;

						++index;

						s.x = m_Sine[index] * radius;
						s.y = m_Cosine[index] * radius;

						p1.x = halfLength; p1.y = r.x; p1.z = r.y;
						p2.x = halfLength; p2.y = s.x; p2.z = s.y;
						renderer.DrawLine(LocalToWorld(p1), LocalToWorld(p2), m_C1);
						renderer.DrawLine(LocalToWorld(-p1), LocalToWorld(-p2), m_C1);
					}
				}
				break;
		}
	}

	// Camera
	private void DrawFrustum(IMDrawLineRenderer renderer)
	{
		if (m_Camera == null) // Ensure the camera reference is still valid, if the camera was deleted between the time the draw was issued and when it is drawn, this would become null
			return;

		Transform camTransform = m_Camera.transform;
		Vector3 pos = camTransform.position;
		Vector3 right = camTransform.right;
		Vector3 up = camTransform.up;
		Vector3 forward = camTransform.forward;
		float nearDist = m_Camera.nearClipPlane;
		float farDist = m_Camera.farClipPlane;
		Vector3 near = pos + forward * nearDist;
		Vector3 far = pos + forward * farDist;
		Rect rect = m_Camera.rect;

		Vector3 xOffset, yOffset, n1, n2, n3, n4, f1, f2, f3, f4;

		if (m_Camera.orthographic)
		{
			float height = m_Camera.orthographicSize;

			float width = height * m_Camera.aspect * rect.width / rect.height;

			yOffset = up * height;
			xOffset = right * width;

			n1 = near - yOffset - xOffset;
			n2 = near - yOffset + xOffset;
			n3 = near + yOffset + xOffset;
			n4 = near + yOffset - xOffset;

			//f1 = far - yOffset - xOffset;
			//f2 = far - yOffset + xOffset;
			//f3 = far + yOffset + xOffset;
			//f4 = far + yOffset - xOffset;
		}
		else // Perspective
		{
			float aspect = m_Camera.aspect * rect.width / rect.height;
			float fov = 2f * (float)Math.Tan(m_Camera.fieldOfView * Mathf.Deg2Rad * 0.5f);

			float nearHeight = fov * nearDist;
			float nearWidth = nearHeight * aspect;

			float farHeight = fov * farDist;
			float farWidth = farHeight * aspect;

			yOffset = up * nearHeight * 0.5f;
			xOffset = right * nearWidth * 0.5f;

			n1 = near - yOffset - xOffset;
			n2 = near - yOffset + xOffset;
			n3 = near + yOffset + xOffset;
			n4 = near + yOffset - xOffset;

			yOffset = up * farHeight * 0.5f;
			xOffset = right * farWidth * 0.5f;

			//f1 = far - yOffset - xOffset;
			//f2 = far - yOffset + xOffset;
			//f3 = far + yOffset + xOffset;
			//f4 = far + yOffset - xOffset;
		}

		f1 = far - yOffset - xOffset;
		f2 = far - yOffset + xOffset;
		f3 = far + yOffset + xOffset;
		f4 = far + yOffset - xOffset;

		// Draw near plane
		renderer.DrawLine(n1, n2, m_C1);
		renderer.DrawLine(n2, n3, m_C1);
		renderer.DrawLine(n3, n4, m_C1);
		renderer.DrawLine(n4, n1, m_C1);

		// Draw far plane
		renderer.DrawLine(f1, f2, m_C1);
		renderer.DrawLine(f2, f3, m_C1);
		renderer.DrawLine(f3, f4, m_C1);
		renderer.DrawLine(f4, f1, m_C1);

		// Draw frustum edges
		renderer.DrawLine(n1, f1, m_C1);
		renderer.DrawLine(n2, f2, m_C1);
		renderer.DrawLine(n3, f3, m_C1);
		renderer.DrawLine(n4, f4, m_C1);
	}
}


//=====================================================================================================================================================================================================

[System.Serializable]
public class IMDrawLineComponent
{
	/// <summary>Hard defined maximum value as a safety against someone putting in a silly number that could cause the application to freeze and run out of memory.</summary>
	public const int							MAX_LINES = 100000;
	private const int							INIT_POOL_CAPACITY = 256;

	private static Stack<IMDrawLineCommand>		s_CommandPool; // Pool for GL draw command objects - note: when this stack grows, it uses Array.resize which will briefly create garbage

	private LinkedList<IMDrawLineCommand>[]		m_CommandListArray; // List for active line draw commands for each ZTest variant

	[SerializeField]
	private int									m_MaxLines = 1000;

	[SerializeField]
	private int									m_RenderLayer = 1; // By default, use transparent layer

	private IMDrawMeshLineRendererSimple		m_MeshRenderer;

	private int									m_CommandCount;
	private int									m_LineCount;
	private bool								m_ExceededBudget;

	static IMDrawLineComponent ()
	{
		s_CommandPool = new Stack<IMDrawLineCommand>(INIT_POOL_CAPACITY);
	}

	public void Init ()
	{
		m_CommandListArray = new LinkedList<IMDrawLineCommand>[9];

		for(int i = 0; i < m_CommandListArray.Length; ++i)
		{
			m_CommandListArray[i] = new LinkedList<IMDrawLineCommand>();
		}

		m_MeshRenderer = new IMDrawMeshLineRendererSimple(m_MaxLines);

		m_CommandCount = 0;
		m_LineCount = 0;
	}

	public void Destroy ()
	{
		FlushAll();

		m_MeshRenderer.Destroy();
	}

	public IMDrawLineCommand CreateLineCommand()
	{
		IMDrawLineCommand command = s_CommandPool.Count > 0 ? s_CommandPool.Pop() : new IMDrawLineCommand();
		m_CommandListArray[(int)IMDrawManager.s_ZTest].AddLast(command.m_ListNode);
		++m_CommandCount;
		return command;
	}

	public void Update (float deltaTime)
	{
		if (m_CommandListArray == null)
			Init();

		m_ExceededBudget = false;

		LinkedList<IMDrawLineCommand> commandList;
		LinkedListNode<IMDrawLineCommand> node, nextNode;

		for (int i = 0; i < m_CommandListArray.Length; ++i)
		{
			if (m_CommandListArray[i].Count == 0)
				continue;

			commandList = m_CommandListArray[i];
			node = commandList.First;

			while (node != null)
			{
				node.Value.m_T -= deltaTime;

				if (node.Value.m_T <= 0f)
				{
					nextNode = node.Next;
					s_CommandPool.Push(node.Value.Dispose());
					commandList.Remove(node);
					node = nextNode;

					--m_CommandCount;
				}
				else
				{
					node = node.Next;
				}
			}
		}
	}

	public void DrawMesh(IMDrawCamera camera)
	{
		if (camera == null)
			return;

		Draw(camera, m_MeshRenderer);
	}

	private void Draw (IMDrawCamera camera, IMDrawLineRenderer renderer)
	{
		//UnityEngine.Profiling.Profiler.BeginSample("*** IMDrawLineComponent.Draw");
		m_LineCount = 0;

		if (!camera.Camera.isActiveAndEnabled)
			return; 

		Material material = camera.MaterialLine;
		LinkedListNode<IMDrawLineCommand> node;

		renderer.SetCamera(camera.Camera);

		if (camera.DistanceCullEnabled)
		{
			Vector3 cameraPos = camera.Camera.transform.position;
			float cullDistSqrd = camera.CullDistanceSqrd;

			for (int i = 0; i < m_CommandListArray.Length; ++i)
			{
				if (m_CommandListArray[i].Count == 0)
					continue;

				material.SetInt(IMDrawSPID._ZTest, i);
				material.SetPass(0);

				renderer.Begin(material, m_RenderLayer);

				node = m_CommandListArray[i].First;

				while (node != null)
				{
					if ((node.Value.m_Lines + m_LineCount) <= m_MaxLines)
					{
						if (node.Value.GetDistSqrd(ref cameraPos) < cullDistSqrd)
						{
							//int prevLineCount = renderer.GetLineCount();

							node.Value.Draw(renderer);
							m_LineCount += node.Value.m_Lines;

							/*
							int linesDrawn = renderer.GetLineCount() - prevLineCount;

							if (linesDrawn != node.Value.m_Lines)
							{
								Debug.LogFormat("Command {0}: Used {1} lines, expected {2}", node.Value.m_Type.ToString(), linesDrawn, node.Value.m_Lines);
							}
							*/
						}
					}
					else
					{
						m_ExceededBudget = true;
					}

					node = node.Next;
				}

				renderer.Finalise();
			}
		}
		else
		{
			for (int i = 0; i < m_CommandListArray.Length; ++i)
			{
				if (m_CommandListArray[i].Count == 0)
					continue;

				material.SetInt(IMDrawSPID._ZTest, i);
				material.SetPass(0);

				renderer.Begin(material, m_RenderLayer);

				node = m_CommandListArray[i].First;

				while (node != null)
				{
					if ((node.Value.m_Lines + m_LineCount) <= m_MaxLines)
					{
						//int prevLineCount = renderer.GetLineCount();

						node.Value.Draw(renderer);
						m_LineCount += node.Value.m_Lines;

						/*
						int linesDrawn = renderer.GetLineCount() - prevLineCount;

						if (linesDrawn != node.Value.m_Lines)
						{
							Debug.LogFormat("Command {0}: Used {1} lines, expected {2}", node.Value.m_Type.ToString(), linesDrawn, node.Value.m_Lines);
						}
						*/
					}
					else
					{
						m_ExceededBudget = true;
					}

					node = node.Next;
				}

				renderer.Finalise();
			}
		}

		//UnityEngine.Profiling.Profiler.EndSample();
	}

	public void FlushAll ()
	{
		if (m_CommandCount == 0)
			return;

		LinkedList<IMDrawLineCommand> commandList;

		for(int i = 0; i < m_CommandListArray.Length; ++i)
		{
			commandList = m_CommandListArray[i];

			while (commandList.Count > 0)
			{
				s_CommandPool.Push(commandList.Last.Value.Dispose());
				commandList.RemoveLast();
			}
		}

		m_CommandCount = 0;
	}

	public int CommandCount { get { return m_CommandCount; } }
	public int LineCount { get { return m_LineCount; } }
	public int MaxLines { get { return m_MaxLines; } }
	public int CommandPoolCount {  get { return s_CommandPool.Count;  } }
	public bool ExceededBudget { get { return m_ExceededBudget; } }

#if UNITY_EDITOR

	// Here we handle reload of scripts (usually the result of recompiling during play)
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReloaded()
	{
		if (Application.isPlaying)
		{
			s_CommandPool = new Stack<IMDrawLineCommand>(INIT_POOL_CAPACITY);
		}
	}

#endif // UNITY_EDITOR
}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(IMDrawLineComponent))]
public class IMDrawLineComponentInspector : IMDrawPropertyDrawer
{
	private static GUIContent LABEL_MAXLINES = new GUIContent("Max lines", "Maximum number of lines that can be rendered. This affects line and wireframe primitives.");
	private static GUIContent LABEL_RENDERLAYER = new GUIContent("Render layer", "The render layer used for line primitives.");

	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		GUILayout.Space(-4f);

		bool isPlaying = Application.isPlaying;

		GUI.enabled = !isPlaying;
		IntPropertyField(prop, "m_MaxLines", LABEL_MAXLINES, 0, IMDrawLineComponent.MAX_LINES);
		GUI.enabled = true;

		LayerPropertyField(prop, "m_RenderLayer", LABEL_RENDERLAYER);
	}
}

#endif // UNITY_EDITOR

//=====================================================================================================================================================================================================

public abstract class IMDrawLineRenderer
{
	public abstract void Destroy();

	public abstract void SetCamera (Camera camera);

	public abstract void Begin (Material material, int renderLayer); // Prepare for line drawing
	public abstract void Finalise(); // Draw all lines

	public abstract void DrawLine(Vector3 p0, Vector3 p1, Color c);
	public abstract void DrawLine(Vector3 p0, Vector3 p1, Color c0, Color c1);

	public virtual int GetLineCount() { return 0; }
}

#if false // Deprecated - no reason to use this anymore
public class IMDrawGLLineRenderer : IMDrawLineRenderer
{
	public override void Destroy() {}

	public override void SetCamera(Camera camera) {}

	public override void Begin (Material material, int renderLayer)
	{
		GL.Begin(GL.LINES);
	}

	public override void Finalise ()
	{
		GL.End();
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c)
	{
		GL.Color(c);
		GL.Vertex(p0);
		GL.Vertex(p1);
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c0, Color c1)
	{
		GL.Color(c0);
		GL.Vertex(p0);
		GL.Color(c1);
		GL.Vertex(p1);
	}
}
#endif

/// Note: This is the default line renderer since it appears to perform faster than the GL version.
public class IMDrawMeshLineRendererSimple : IMDrawLineRenderer
{
	private Mesh		m_Mesh;
	private Material	m_Material;
	private int			m_RenderLayer;
	private Camera		m_Camera;

	private Vector3[]	m_Vertex;
	private Color32[]	m_Color32; // Note: we use Color32 instead of Color because its faster and dramatically reduces the size of the mesh data
	private int[]		m_Index;

	private int			m_Line, m_PrevLineCount;

	private static readonly Vector3 _VERTEX_NULL = new Vector3(0f, 0f, 0f);
	private static readonly Color32 _COLOR_NULL = new Color32(0, 0, 0, 0);
	private static readonly Vector3 _BOUNDS_SIZE = new Vector3(100f, 100f, 100f);

	public IMDrawMeshLineRendererSimple (int maxLines)
	{
		m_Mesh = new Mesh();
#if UNITY_EDITOR
		m_Mesh.name = "IMDrawLineMesh";
#endif // UNITY_EDITOR

		InitVertexData(maxLines);
	}

	private void InitVertexData(int maxLines)
	{
		int vertices = maxLines * 2;

		m_Vertex = new Vector3[vertices];
		m_Color32 = new Color32[vertices];
		m_Index = new int [vertices];

		for (int i = 0; i < vertices; ++i)
		{
			m_Index[i] = i;
		}

		m_Line = 0;
		m_PrevLineCount = 0;
	}

	public override void Destroy()
	{
		if (m_Mesh != null)
		{
			UnityEngine.Object.DestroyImmediate(m_Mesh);
			m_Mesh = null;
		}
	}

	public override void SetCamera(Camera camera)
	{
		m_Camera = camera;
	}

	public override void Begin(Material material, int renderLayer)
	{
		m_Material = material;
		m_RenderLayer = renderLayer;
		m_Line = 0;
	}

	public override void Finalise()
	{
		if (m_Camera == null)
			return;

		int index;

		// Ensure unused lines are made into degenerate vertices
		for (int lineIndex = m_Line; lineIndex < m_PrevLineCount; ++lineIndex)
		{
			index = lineIndex * 2;

			m_Vertex[index] = _VERTEX_NULL;
			m_Vertex[index + 1] = _VERTEX_NULL;

			m_Color32[index] = _COLOR_NULL;
			m_Color32[index + 1] = _COLOR_NULL;
		}

		m_Mesh.Clear();
		//m_Mesh.MarkDynamic();
		m_Mesh.vertices = m_Vertex;
		m_Mesh.colors32 = m_Color32;
		m_Mesh.bounds = new Bounds(m_Camera.transform.position, _BOUNDS_SIZE); // Force the mesh to always be rendered over other geometry that is rendered with the same priority
		m_Mesh.SetIndices(m_Index, MeshTopology.Lines, 0);

		Graphics.DrawMesh(m_Mesh, Vector3.zero, Quaternion.identity, m_Material, m_RenderLayer, m_Camera, 0);

		m_Material = null;
		m_Camera = null;

		m_PrevLineCount = m_Line;
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c)
	{
		int i = m_Line * 2;
		++m_Line;

		m_Vertex[i] = p0;
		m_Vertex[i + 1] = p1;

		Color32 c32 = c; // Convert Color to Color32
		m_Color32[i] = c32;
		m_Color32[i + 1] = c32;
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c0, Color c1)
	{
		int i = m_Line * 2;
		++m_Line;

		m_Vertex[i] = p0;
		m_Vertex[i + 1] = p1;

		m_Color32[i] = c0;
		m_Color32[i + 1] = c1;
	}

	public override int GetLineCount()
	{
		return m_Line;
	}
}


#if false

// TODO:
// - Develop a more advanced version of line renderer which can handle lines of different thickness.
// - Camera.WorldToScreenPoint and Camera.ScreenToWorldPoint are exceptionally slow, create replacements and potentially move the screen space/world space math to the vertex shader.
// - Line renderers to have their own unique inspector parameters.
public class IMDrawMeshLineRenderer : IMDrawLineRenderer
{
	private const float LINE_HALF_WIDTH = 0.5f;

	private Mesh		m_Mesh;

	private Material	m_Material;
	private int			m_RenderLayer;

	private Camera		m_Camera;
	private float		m_CamPosX, m_CamPosY, m_CamPosZ;
	private float		m_CamDirX, m_CamDirY, m_CamDirZ;
	private float		m_CameraPlaneDistance;

	private Vector3[]	m_Vertex;
	private Color32[]	m_Color32; // Note: we use Color32 instead of Color because its faster and dramatically reduces the size of the mesh data
	private int[]		m_Index;

	private int			m_Line, m_PrevLineCount;

	private static readonly Vector3		_VERTEX_NULL = new Vector3 (0f,0f,0f);
	private static readonly Color32		_COLOR_NULL = new Color32 (0,0,0,0);

	public IMDrawMeshLineRenderer (int maxLines)
	{
		m_Mesh = new Mesh();
		m_Mesh.name = "IMDrawLineMesh";
		//m_Mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

		InitVertexData(maxLines);
	}

	private void InitVertexData(int maxLines)
	{
		m_Vertex = new Vector3[maxLines * 4];
		m_Color32 = new Color32[maxLines * 4];
		m_Index = new int[maxLines * 6];

		int index = 0, vert;

		for (int i = 0; i < maxLines; ++i)
		{
			vert = i * 4;

			m_Index[index++] = vert;
			m_Index[index++] = vert + 2;
			m_Index[index++] = vert + 1;
			m_Index[index++] = vert + 2;
			m_Index[index++] = vert + 3;
			m_Index[index++] = vert + 1;
		}

		m_Line = 0;
		m_PrevLineCount = 0;

		//Debug.LogFormat("Vertex data = {0} lines", maxLines);
	}

	public override void Destroy()
	{
		UnityEngine.Object.Destroy(m_Mesh);
		m_Mesh = null;
	}

	public override void SetCamera(Camera camera)
	{
		m_Camera = camera;

		if (camera != null)
		{
			Transform cameraTransform = camera.transform;

			Vector3 cameraPos = cameraTransform.position;
			Vector3 cameraDir = cameraTransform.forward;
			m_CameraPlaneDistance = -Vector3.Dot(cameraDir, cameraPos + (cameraDir * camera.nearClipPlane));
			m_CamPosX = cameraPos.x;
			m_CamPosY = cameraPos.y;
			m_CamPosZ = cameraPos.z;
			m_CamDirX = cameraDir.x;
			m_CamDirY = cameraDir.y;
			m_CamDirZ = cameraDir.z;
		}
	}

	public override void Begin (Material material, int renderLayer)
	{
		m_Material = material;
		m_RenderLayer = renderLayer;
		m_Line = 0;
	}

	public override void Finalise ()
	{
		for(int i = m_Line; i < m_PrevLineCount; ++i)
		{
			DrawDegenerate(i);
		}

		m_Mesh.Clear();
		m_Mesh.vertices = m_Vertex;
		m_Mesh.colors32 = m_Color32;
		m_Mesh.triangles = m_Index;
		m_Mesh.RecalculateBounds();

		Graphics.DrawMesh(m_Mesh, Vector3.zero, Quaternion.identity, m_Material, m_RenderLayer, m_Camera, 0);

		m_Material = null;
		m_Camera = null;

		m_PrevLineCount = m_Line;
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c)
	{
		if (IsBehind(ref p0))
		{
			if (IsBehind(ref p1)) // Reject since it is behind the camera
				return;

			p0 = GetNearClipIntercept(p0, p1);
		}
		else if (IsBehind(ref p1))
		{
			p1 = GetNearClipIntercept(p1, p0);
		}

		p0 = m_Camera.WorldToScreenPoint(p0);
		p1 = m_Camera.WorldToScreenPoint(p1);

		float cx = p1.x - p0.x;
		float cy = p1.y - p0.y;
		float k = LINE_HALF_WIDTH / (float)System.Math.Sqrt(cx * cx + cy * cy);

		float offsetX = -cy * k;
		float offsetY = cx * k;

		int i = m_Line * 4;
		++m_Line;

		m_Vertex[i] = m_Camera.ScreenToWorldPoint(new Vector3(p0.x + offsetX, p0.y + offsetY, p0.z)); //p0 + offset);
		m_Vertex[i + 1] = m_Camera.ScreenToWorldPoint(new Vector3(p0.x - offsetX, p0.y - offsetY, p0.z)); // p0 - offset);
		m_Vertex[i + 2] = m_Camera.ScreenToWorldPoint(new Vector3(p1.x + offsetX, p1.y + offsetY, p1.z)); //p1 + offset);
		m_Vertex[i + 3] = m_Camera.ScreenToWorldPoint(new Vector3(p1.x - offsetX, p1.y - offsetY, p1.z)); //p1 - offset);

		Color32 c32 = c; // Convert Color to Color32
		m_Color32[i] = c32;
		m_Color32[i + 1] = c32;
		m_Color32[i + 2] = c32;
		m_Color32[i + 3] = c32;
	}

	public override void DrawLine(Vector3 p0, Vector3 p1, Color c0, Color c1)
	{
		if (IsBehind(ref p0))
		{
			if (IsBehind(ref p1)) // Reject since it is behind the camera
				return;

			p0 = GetNearClipIntercept(p0, p1);
		}
		else if (IsBehind(ref p1))
		{
			p1 = GetNearClipIntercept(p1, p0);
		}

		p0 = m_Camera.WorldToScreenPoint(p0);
		p1 = m_Camera.WorldToScreenPoint(p1);

		float cx = p1.x - p0.x;
		float cy = p1.y - p0.y;
		float k = LINE_HALF_WIDTH / (float)System.Math.Sqrt(cx * cx + cy * cy);

		float offsetX = -cy * k;
		float offsetY = cx * k;

		int i = m_Line * 4;
		++m_Line;

		m_Vertex[i] = m_Camera.ScreenToWorldPoint(new Vector3(p0.x + offsetX, p0.y + offsetY, p0.z)); //p0 + offset);
		m_Vertex[i + 1] = m_Camera.ScreenToWorldPoint(new Vector3(p0.x - offsetX, p0.y - offsetY, p0.z)); // p0 - offset);
		m_Vertex[i + 2] = m_Camera.ScreenToWorldPoint(new Vector3(p1.x + offsetX, p1.y + offsetY, p1.z)); //p1 + offset);
		m_Vertex[i + 3] = m_Camera.ScreenToWorldPoint(new Vector3(p1.x - offsetX, p1.y - offsetY, p1.z)); //p1 - offset);

		Color32 c32 = c0; // Convert Color to Color32
		m_Color32[i] = c32;
		m_Color32[i + 1] = c32;

		c32 = c1; // Convert Color to Color32
		m_Color32[i + 2] = c32;
		m_Color32[i + 3] = c32;
	}

	private void DrawDegenerate (int index)
	{
		index *= 4;

		m_Vertex[index] = _VERTEX_NULL;
		m_Vertex[index + 1] = _VERTEX_NULL;
		m_Vertex[index + 2] = _VERTEX_NULL;
		m_Vertex[index + 3] = _VERTEX_NULL;

		m_Color32[index] = _COLOR_NULL;
		m_Color32[index + 1] = _COLOR_NULL;
		m_Color32[index + 2] = _COLOR_NULL;
		m_Color32[index + 3] = _COLOR_NULL;
	}

	private Vector3 GetNearClipIntercept(Vector3 p0, Vector3 p1)
	{
		Vector3 n = Vector3.Normalize(p1 - p0);
		float vdot = n.x * m_CamDirX + n.y * m_CamDirY + n.z * m_CamDirZ; // Dot product

		if (vdot < -0.00001f || vdot > 0.00001f)
		{
			float ndot = -(p0.x * m_CamDirX + p0.y * m_CamDirY + p0.z * m_CamDirZ) - m_CameraPlaneDistance;
			return p0 + (n * (ndot / vdot));
		}

		return p0;
	}

	private bool IsBehind(ref Vector3 pos)
	{
		return (m_CamDirX * (pos.x - m_CamPosX) + m_CamDirY * (pos.y - m_CamPosY) + m_CamDirZ * (pos.z - m_CamPosZ)) < 0f;
	}

	public override int GetLineCount ()
	{
		return m_Line;
	}
}


#endif