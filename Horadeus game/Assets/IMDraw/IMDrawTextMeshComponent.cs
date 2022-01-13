using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR


[System.Serializable]
public class IMDrawTextMeshComponent
{
	public Font								m_Font;
	//public Material							m_Material;

	[Range(1, 256)]
	public int								m_FontSize = 64;

	[Range(1, 256)]
	public int								m_MaxTextMeshes = 64;

	public TextAlignment					m_DefaultAlignment = TextAlignment.Center;
	public TextAnchor						m_DefaultAnchor = TextAnchor.MiddleCenter;

	public float							m_MinDistance = 10f;
	public float							m_MaxDistance = 100f;
	public float							m_DistanceScale = 1f;
	public bool								m_DistanceFade = true;
	public bool								m_FontSizeAffectsScale = false;

	private IMDrawCamera					m_IMDrawCamera;

	private Stack<IMDrawTextMesh>			m_TextMeshPool;
	private LinkedList<IMDrawTextMesh>		m_TextMeshList;
	private Vector3							m_CameraPosition;
	private Vector3							m_CameraForward;
	private float							m_MinMaxDistanceDelta;
	private Quaternion						m_LookRotation;
	private bool							m_Perspective;

	public IMDrawCamera IMDrawCamera { get { return m_IMDrawCamera; } }

	public void Init (IMDrawCamera cam)
	{
		//Debug.Log("IMDrawTextMeshComponent.Init");

		m_IMDrawCamera = cam;

		if (m_TextMeshList == null)
			m_TextMeshList = new LinkedList<IMDrawTextMesh>();

		if (m_TextMeshPool == null)
		{
			m_TextMeshPool = new Stack<IMDrawTextMesh>(m_MaxTextMeshes);
		}

		if (m_Font == null)
			m_Font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // If no font is assigned, use the Unity default

		if (m_Font != null)
		{
			Texture tex = m_Font.material.GetTexture(IMDrawSPID._MainTex);

			if (cam.MaterialTextMesh != null)
			{
				cam.MaterialTextMesh.SetTexture(IMDrawSPID._MainTex, tex);
			}
		}
	}

	public void Destroy()
	{
		//Debug.Log("IMDrawTextMeshComponent.Destroy");

		// Destroy all text mesh objects
		GameObject textMeshGameObj;

		while (m_TextMeshList.Count > 0)
		{
			textMeshGameObj = m_TextMeshList.Last.Value.m_RootGameObject;
			m_TextMeshList.RemoveLast();
			UnityEngine.Object.Destroy(textMeshGameObj);
		}

		while (m_TextMeshPool.Count > 0)
		{
			UnityEngine.Object.Destroy(m_TextMeshPool.Pop().m_RootGameObject);
		}
	}

	public void FlushAll()
	{
		while (m_TextMeshList.Count > 0)
		{
			Dispose(m_TextMeshList.Last.Value);
		}
	}

	private IMDrawTextMesh Create()
	{
		IMDrawTextMesh textMesh;

		if (m_TextMeshPool.Count > 0)
		{
			textMesh = m_TextMeshPool.Pop();
			textMesh.m_RootTransform.SetParent(null, false);
		}
		else
		{
			textMesh = IMDrawTextMesh.Create(this);
		}

		m_TextMeshList.AddLast(textMesh.m_ListNode);

		textMesh.SetZTest(IMDrawManager.s_ZTest);

		return textMesh;
	}

	private void Dispose(IMDrawTextMesh textMesh)
	{
		textMesh.Hide();
		m_TextMeshList.Remove(textMesh.m_ListNode);
		m_TextMeshPool.Push(textMesh);
		//Debug.Log("Text mesh disposed");
	}

	public void Tick (float deltaTime)
	{
		if (m_TextMeshList.Count == 0)
			return;

		LinkedListNode<IMDrawTextMesh> node = m_TextMeshList.First, nextNode;

		while (node != null)
		{
			node.Value.m_T -= deltaTime;

			if (node.Value.m_T <= 0f)
			{
				nextNode = node.Next;
				Dispose(node.Value);
				node = nextNode;
			}
			else
			{
				node = node.Next;
			}
		}
	}

	public void Draw ()
	{
		if (m_TextMeshList.Count == 0 || m_IMDrawCamera.MaterialTextMesh == null || m_Font == null)
			return;

		Camera camera = m_IMDrawCamera.Camera;

		Transform cameraTransform = camera.transform;

		m_CameraPosition = cameraTransform.position;
		m_CameraForward = cameraTransform.forward;

		m_Perspective = !camera.orthographic;

		m_LookRotation = Quaternion.LookRotation(m_CameraForward, cameraTransform.up);

		m_MinMaxDistanceDelta = m_MaxDistance - m_MinDistance;

		LinkedListNode<IMDrawTextMesh> node = m_TextMeshList.First;

		while (node != null)
		{
			node.Value.Draw(this);
			node = node.Next;
		}
	}

	public float GetScreenZ(Transform textMeshTransform)
	{
		Vector3 delta = textMeshTransform.localPosition - m_CameraPosition;
		return m_CameraForward.x * delta.x + m_CameraForward.y * delta.y + m_CameraForward.z * delta.z;
	}

	public void FaceCamera(Transform textMeshTransform)
	{
		textMeshTransform.localRotation = m_LookRotation;
	}

	public float GetAlpha (float screenZ)
	{
		return m_MinMaxDistanceDelta > 0f ?
			1f - (screenZ - m_MinDistance) / m_MinMaxDistanceDelta :
			1f;
	}

	public bool IsDrawDisabled { get { return m_TextMeshList.Count >= m_MaxTextMeshes || m_IMDrawCamera.MaterialTextMesh == null || m_Font == null; } }

	public bool IsCameraPerspective { get { return m_Perspective; } }

	//public Material FontMaterial { get { return m_Material; } }

	#region DRAW COMMAND FUNCTIONS

	public void DrawTextMesh(string text, Color color, Vector3 position, float scaleX, float scaleY, IMDrawTextMeshOption options, float duration = 0f)
	{
		IMDrawTextMesh textMesh = Create();
		textMesh.m_TextMesh.text = text;
		textMesh.m_TextMesh.color = color;
		//textMesh.SetFont(m_Font);
		textMesh.m_TextMesh.fontSize = m_FontSize;
		textMesh.m_TextMesh.alignment = m_DefaultAlignment;
		textMesh.m_TextMesh.anchor = m_DefaultAnchor;
		textMesh.m_RootTransform.localPosition = position;
		textMesh.m_Color = color;
		textMesh.m_Scale = m_FontSizeAffectsScale ? new Vector2(scaleX, scaleY) : new Vector2(scaleX / (float)m_FontSize, scaleY / (float)m_FontSize);
		textMesh.m_Option = options;
		textMesh.m_T = duration;

		if ((options & IMDrawTextMeshOption.DISTANCE_SCALE) == 0)
		{
			textMesh.m_RootTransform.localScale = new Vector3(textMesh.m_Scale.x, textMesh.m_Scale.y, 1f); // If no dynamic scaling is used, set the transform scale right away
		}
	}

	public void DrawTextMesh (string text, Color color, Vector3 position, float scaleX, float scaleY, TextAlignment align, TextAnchor anchor, IMDrawTextMeshOption options, float duration = 0f)
	{
		IMDrawTextMesh textMesh = Create();
		textMesh.m_TextMesh.text = text;
		textMesh.m_TextMesh.color = color;
		//textMesh.SetFont(m_Font);
		textMesh.m_TextMesh.fontSize = m_FontSize;
		textMesh.m_TextMesh.alignment = align;
		textMesh.m_TextMesh.anchor = anchor;
		textMesh.m_RootTransform.localPosition = position;
		textMesh.m_Color = color;
		textMesh.m_Scale = m_FontSizeAffectsScale ? new Vector2(scaleX, scaleY) : new Vector2(scaleX / (float)m_FontSize, scaleY / (float)m_FontSize);
		textMesh.m_Option = options;
		textMesh.m_T = duration;

		if ((options & IMDrawTextMeshOption.DISTANCE_SCALE) == 0)
		{
			textMesh.m_RootTransform.localScale = new Vector3(textMesh.m_Scale.x, textMesh.m_Scale.y, 1f); // If no dynamic scaling is used, set the transform scale right away
		}
	}

	public void DrawTextMesh(string text, Color color, Vector3 position, Quaternion rotation, float scaleX, float scaleY, IMDrawTextMeshOption options, float duration = 0f)
	{
		IMDrawTextMesh textMesh = Create();
		textMesh.m_TextMesh.text = text;
		textMesh.m_TextMesh.color = color;
		//textMesh.SetFont(m_Font);
		textMesh.m_TextMesh.fontSize = m_FontSize;
		textMesh.m_TextMesh.alignment = m_DefaultAlignment;
		textMesh.m_TextMesh.anchor = m_DefaultAnchor;
		textMesh.m_RootTransform.localPosition = position;
		textMesh.m_RootTransform.localRotation = rotation;
		textMesh.m_Color = color;
		textMesh.m_Scale = m_FontSizeAffectsScale ? new Vector2(scaleX, scaleY) : new Vector2(scaleX / (float)m_FontSize, scaleY / (float)m_FontSize);
		textMesh.m_Option = options;
		textMesh.m_T = duration;

		if ((options & IMDrawTextMeshOption.DISTANCE_SCALE) == 0)
		{
			textMesh.m_RootTransform.localScale = new Vector3(textMesh.m_Scale.x, textMesh.m_Scale.y, 1f); // If no dynamic scaling is used, set the transform scale right away
		}
	}

	public void DrawTextMesh (string text, Color color, Vector3 position, Quaternion rotation, float scaleX, float scaleY, TextAlignment align, TextAnchor anchor, IMDrawTextMeshOption options, float duration = 0f)
	{
		IMDrawTextMesh textMesh = Create();
		textMesh.m_TextMesh.text = text;
		textMesh.m_TextMesh.color = color;
		//textMesh.SetFont(m_Font);
		textMesh.m_TextMesh.fontSize = m_FontSize;
		textMesh.m_TextMesh.alignment = align;
		textMesh.m_TextMesh.anchor = anchor;
		textMesh.m_RootTransform.localPosition = position;
		textMesh.m_RootTransform.localRotation = rotation;
		textMesh.m_Color = color;
		textMesh.m_Scale = m_FontSizeAffectsScale ? new Vector2(scaleX, scaleY) : new Vector2(scaleX / (float)m_FontSize, scaleY / (float)m_FontSize);
		textMesh.m_Option = options;
		textMesh.m_T = duration;

		if ((options & IMDrawTextMeshOption.DISTANCE_SCALE) == 0)
		{
			textMesh.m_RootTransform.localScale = new Vector3(textMesh.m_Scale.x, textMesh.m_Scale.y, 1f); // If no dynamic scaling is used, set the transform scale right away
		}
	}

	#endregion

	#if UNITY_EDITOR

	public void AppendDebugInfo (System.Text.StringBuilder sb)
	{
		sb.Append("\nText meshes: ");
		sb.Append(m_TextMeshList.Count);
		sb.Append(" / ");
		sb.Append(m_MaxTextMeshes);
		//sb.Append(" [");
		//sb.Append(m_Pool.Count);
		//sb.Append(']');
	}

	#endif // UNITY_EDITOR
}


#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(IMDrawTextMeshComponent))]
public class IMDrawTextMeshComponentInspector : IMDrawPropertyDrawer
{
	private static GUIContent LABEL_FONT = new GUIContent("Font override", "Font used for text meshes. If no font is specified, the default font is used.");
	//private static GUIContent LABEL_FONTMATERIAL = new GUIContent("Font material", "Font material used by text mesh objects.");
	private static GUIContent LABEL_FONTSIZE = new GUIContent("Font size", "Font size.");
	private static GUIContent LABEL_MAXTEXTMESHES = new GUIContent("Max text meshes", "Maximum number of text meshes that can be drawn at the same time.");
	private static GUIContent LABEL_DEFAULTALIGNMENT = new GUIContent("Default alignment", "Text alignment used when alignment is unspecified.");
	private static GUIContent LABEL_DEFAULTANCHOR = new GUIContent("Default anchor", "Text anchor used when anchor is unspecified.");
	private static GUIContent LABEL_MINDISTANCE = new GUIContent("Minimum distance", "Minimum fade distance (if fade over distance is enabled).");
	private static GUIContent LABEL_MAXDISTANCE = new GUIContent("Maximum distance", "Maximum draw distance for text meshes.");
	private static GUIContent LABEL_DISTANCESCALE = new GUIContent("Distance scale", "Scaling applied to text meshes that used fixed scale.");
	private static GUIContent LABEL_DISTANCEFADE = new GUIContent("Fade over distance", "Enable/disable fading of text mesh based on their distance from the camera within the minimum and maximum distance. Beyond the maximum distance, text meshes will not appear.");
	private static GUIContent LABEL_FONTSIZEAFFECTSSCALE = new GUIContent("Font size affects scale", "Specifies if changing the font size affects scale. Useful in situations where you want to improve font texture resolution without making the text larger.");

	public override void OnGUI (Rect rect, SerializedProperty prop, GUIContent label)
	{
		GUILayout.Space(-4f);

		bool isPlaying = Application.isPlaying;

		GUI.enabled = !isPlaying;
		PropertyField(prop, "m_Font", LABEL_FONT);
		GUI.enabled = true;

		PropertyField(prop, "m_FontSize", LABEL_FONTSIZE);
		PropertyField(prop, "m_MaxTextMeshes", LABEL_MAXTEXTMESHES);
		PropertyField(prop, "m_DefaultAlignment", LABEL_DEFAULTALIGNMENT);
		PropertyField(prop, "m_DefaultAnchor", LABEL_DEFAULTANCHOR);

		SerializedProperty minDistanceProperty = prop.FindPropertyRelative("m_MinDistance");
		SerializedProperty maxDistanceProperty = prop.FindPropertyRelative("m_MaxDistance");

		EditorGUILayout.PropertyField(minDistanceProperty, LABEL_MINDISTANCE);
		EditorGUILayout.PropertyField(maxDistanceProperty, LABEL_MAXDISTANCE);

		float minDist = minDistanceProperty.floatValue;
		float maxDist = maxDistanceProperty.floatValue;
		ClampMinMax(ref minDist, ref maxDist, 0f, float.MaxValue);

		minDistanceProperty.floatValue = minDist;
		maxDistanceProperty.floatValue = maxDist;

		FloatPropertyField(prop, "m_DistanceScale", LABEL_DISTANCESCALE , 0.0001f);

		PropertyField(prop, "m_DistanceFade", LABEL_DISTANCEFADE);
		PropertyField(prop, "m_FontSizeAffectsScale", LABEL_FONTSIZEAFFECTSSCALE);
	}
}

#endif // UNITY_EDITOR

