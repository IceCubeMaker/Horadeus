using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[System.Flags]
public enum IMDrawTextMeshOption
{
	FACE_CAMERA			= (1 << 0), // Specify if text mesh is billboarded
	SCREEN_SPACE		= (1 << 1), // Specify if text mesh is positioned in screen space or 3d world space
	DISTANCE_SCALE		= (1 << 2), // Specify if text mesh should scale with distance (to maintain a fixed screen space size)
}

public class IMDrawTextMesh : MonoBehaviour
{
	public GameObject							m_RootGameObject;
	public Transform							m_RootTransform;
	public MeshRenderer							m_MeshRenderer;
	public TextMesh								m_TextMesh;
	public Material								m_Material;
	public Font									m_Font;
	public Color								m_Color;
	public Vector2								m_Scale;
	public IMDrawTextMeshOption					m_Option;
	public LinkedListNode<IMDrawTextMesh>		m_ListNode;
	public float								m_T; // Time remaining
	private bool								m_Visible;

	private void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(m_Material);
	} 

	/// <summary>Used to clean up on script reloading during play.</summary>
	public static void DestroyAllInstances ()
	{
		IMDrawTextMesh[] textMeshes = FindObjectsOfType<IMDrawTextMesh>();

		if (textMeshes != null)
		{
			for(int i = 0; i < textMeshes.Length; ++i)
			{
				UnityEngine.Object.Destroy(textMeshes[i].gameObject);
			}
		}
	}

	public static IMDrawTextMesh Create (IMDrawTextMeshComponent component)
	{
		GameObject gameObj = new GameObject();
		#if UNITY_EDITOR
		gameObj.name = "IMDrawTextMesh";
		gameObj.hideFlags = HideFlags.HideInHierarchy;
		#endif // UNITY_EDITOR
		Transform tf = gameObj.transform;

		MeshRenderer meshRenderer = gameObj.AddComponent<MeshRenderer>();
		TextMesh textMesh = gameObj.AddComponent<TextMesh>();
		IMDrawTextMesh obj = gameObj.AddComponent<IMDrawTextMesh>();

		obj.m_Material = new Material (component.IMDrawCamera.MaterialTextMesh);

		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = obj.m_Material;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

		#if UNITY_5_4_OR_NEWER
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		
		#if UNITY_5_5_OR_NEWER
		meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		#else
		meshRenderer.motionVectors = false;
		#endif // UNITY_5_5_OR_NEWER

		#else
		meshRenderer.useLightProbes = false;
		#endif

		textMesh.font = component.m_Font;
		textMesh.richText = true;

		obj.m_RootGameObject = gameObj;
		obj.m_RootTransform = tf;
		obj.m_MeshRenderer = meshRenderer;
		obj.m_TextMesh = textMesh;
		obj.m_Option = IMDrawTextMeshOption.FACE_CAMERA | IMDrawTextMeshOption.DISTANCE_SCALE;
		obj.m_ListNode = new LinkedListNode<IMDrawTextMesh>(obj);
		obj.m_Visible = true;

		return obj;
	}

	public void SetZTest (IMDrawZTest zTest) // Called after draw command is created
	{
		m_Material.SetInt(IMDrawSPID._ZTest, (int)zTest);
	}

	public void Draw (IMDrawTextMeshComponent component) // Return true if this text mesh has expired
	{
		float screenZ = component.GetScreenZ(m_RootTransform);

		if (screenZ > 0f && screenZ < component.m_MaxDistance)
		{
			if ((m_Option & IMDrawTextMeshOption.DISTANCE_SCALE) != 0)
			{
				if (component.IsCameraPerspective)
				{
					float distanceScale = screenZ * component.m_DistanceScale;
					m_RootTransform.localScale = new Vector3(distanceScale * m_Scale.x, distanceScale * m_Scale.y, 1f);
				}
				else
				{
					m_RootTransform.localScale = new Vector3(m_Scale.x, m_Scale.y, 1f);
				}
			}

			if ((m_Option & IMDrawTextMeshOption.FACE_CAMERA) != 0)
			{
				component.FaceCamera(m_RootTransform);
			}

			if (component.m_DistanceFade)
			{
				m_TextMesh.color = screenZ > component.m_MinDistance ?
					new Color (m_Color.r, m_Color.g, m_Color.b, m_Color.a * component.GetAlpha(screenZ)) :
					m_Color;
			}

			if (!m_Visible)
			{
				m_Visible = true;
				m_RootGameObject.SetActive(true);
			}
		}
		else
		{
			Hide();
		}
	}

	public void Hide ()
	{
		if (m_Visible)
		{
			m_Visible = false;
			m_RootGameObject.SetActive(false);
		}
	}

	/*
	public void SetFont (Font font)
	{
		if (font != m_Font)
		{
			m_Font = font;
			m_TextMesh.font = font;
			//m_MeshRenderer.sharedMaterial = font.material;
			//m_Material.SetTexture()
		}
	}
	*/
}


#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(IMDrawTextMesh))]
public class IMDrawTextMeshEditor : IMDrawEditorBase
{
	public override void OnInspectorGUI()
	{
	}
}

#endif // UNITY_EDITOR
