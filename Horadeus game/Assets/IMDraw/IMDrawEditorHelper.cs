#if UNITY_EDITOR

//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;


// Note: Unity appears to leak any active inspector Editor objects if the game is recompiled whilst playing.
// This results in "The referenced script on this Behaviour is missing!" messages when compilation is finished.
// There is no workaround for this, but otherwise it is fairly harmless.

public abstract class IMDrawEditorBase : Editor
{
	protected static GUIStyle					m_PanelStyle; 
	protected static GUIStyle					m_FoldoutStyle;

	private static GUIStyle						m_HelpboxStyle;
	private static GUIContent					m_HelpBoxMessage;

	private delegate Texture2D EditorGUIUtilityGetHelpIcon (MessageType messageType);
	private static EditorGUIUtilityGetHelpIcon s_GetHelpIcon;

	protected static System.Text.StringBuilder m_SB;

	protected static GUIContent s_GUIContent = new GUIContent();

	static IMDrawEditorBase()
	{
		// Expose hidden internal EditorGUIUtility.GetHelpIcon function using reflection so we can use it for our custom helpbox
		Type type = typeof(EditorGUIUtility);
		System.Reflection.MethodInfo dynMethod = type.GetMethod("GetHelpIcon",
			System.Reflection.BindingFlags.NonPublic |
			System.Reflection.BindingFlags.Static);

		if (dynMethod != null)
		{
			s_GetHelpIcon = (EditorGUIUtilityGetHelpIcon)Delegate.CreateDelegate(typeof(EditorGUIUtilityGetHelpIcon), dynMethod);
		}

		m_HelpBoxMessage = new GUIContent();

		m_SB = new System.Text.StringBuilder(64);
	}

	private void InitPanelStyle ()
	{
		if (m_PanelStyle != null)
			return;

		m_PanelStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
		m_PanelStyle.richText = false;
		m_PanelStyle.fontStyle = FontStyle.Bold;
		m_PanelStyle.normal.textColor = Color.white;
	}

	private void InitFoldoutStyle ()
	{
		if (m_FoldoutStyle != null)
			return;

		m_FoldoutStyle = new GUIStyle(EditorStyles.toolbarButton);
		m_FoldoutStyle.fontSize = 11;
	}

	protected void DrawPanel (string text, int fontSize = 13)
	{
		InitPanelStyle();

		m_PanelStyle.alignment = TextAnchor.UpperLeft;
		m_PanelStyle.fontSize = fontSize;
		EditorGUILayout.LabelField(text, m_PanelStyle);
	}

	protected void DrawPanel(string text, TextAnchor anchor, int fontSize = 13)
	{
		InitPanelStyle();

		m_PanelStyle.alignment = anchor;
		m_PanelStyle.fontSize = fontSize;
		EditorGUILayout.LabelField(text, m_PanelStyle);
	}

	protected bool Foldout (string label, ref bool foldout)
	{
		InitFoldoutStyle();

		if (GUILayout.Button(label, m_FoldoutStyle))
		{
			foldout = !foldout;
		}

		GUI.Label(GUILayoutUtility.GetLastRect(), foldout ? "▼" : "►");

		return foldout;
	}

	protected void Header (string label) 
	{
		InitFoldoutStyle();

		GUILayout.Label(label, m_FoldoutStyle);
	}

	protected void SetExecutionOrder (int order)
	{
		MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);

		if (script != null)
		{
			MonoImporter.SetExecutionOrder(script, order);
			return;
		}

		Debug.LogWarning(string.Format("IMDrawManager: Script for {0} not found!", target.name));
	}

	protected int GetExecutionOrder ()
	{
		MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);

		if (script != null)
		{
			return MonoImporter.GetExecutionOrder(script);
		}

		Debug.LogWarning(string.Format("IMDrawManager: Script for {0} not found!", target.name));

		return 0;
	}

	protected void PropertyField(SerializedProperty property, string label)
	{
		s_GUIContent.text = label;
		s_GUIContent.tooltip = string.Empty;
		EditorGUILayout.PropertyField(property, s_GUIContent);
	}

	protected void PropertyField(string propertyName, string label, string tooltip)
	{
		s_GUIContent.text = label;
		s_GUIContent.tooltip = tooltip;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), s_GUIContent);
	}

	protected bool BoolField(string propertyName, string label, string tooltip)
	{
		s_GUIContent.text = label;
		s_GUIContent.tooltip = tooltip;
		SerializedProperty boolProperty = serializedObject.FindProperty(propertyName);
		EditorGUILayout.PropertyField(boolProperty, s_GUIContent);
		return boolProperty.boolValue;
	}

	protected void HelpBox (string message, MessageType messageType)
	{
		if (m_HelpboxStyle == null)
		{
			m_HelpboxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
			m_HelpboxStyle.fontSize = 11;
			m_HelpboxStyle.normal.textColor = Color.red;
			m_HelpboxStyle.fontStyle = FontStyle.Bold;
		}

		switch(messageType)
		{
			case MessageType.Error: m_HelpboxStyle.normal.textColor = Color.red; break;
			case MessageType.Warning: m_HelpboxStyle.normal.textColor = Color.yellow; break;
			default: m_HelpboxStyle.normal.textColor = GUI.skin.label.normal.textColor; break;
		}

		m_HelpBoxMessage.text = message;
		m_HelpBoxMessage.image = s_GetHelpIcon != null ? s_GetHelpIcon(messageType) : null;

		EditorGUILayout.LabelField(GUIContent.none, m_HelpBoxMessage, m_HelpboxStyle, null);// new GUILayoutOption[0]);
	}

	protected void LogFunction (string functionName)
	{
		Debug.Log(string.Format("#{0} {1}.{2}", GetInstanceID(), GetType().ToString(), functionName));
	}

	protected void Space ()
	{
		GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
	}
}

public abstract class IMDrawPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) // We don't need to specify height so long as we are using EditorGUILayout
	{
		return 0f;
	}

	protected static bool PropertyField (SerializedProperty prop, string propertyName, GUIContent label)
	{
		return EditorGUILayout.PropertyField(prop.FindPropertyRelative(propertyName), label);
	}

	protected static bool LayerPropertyField (SerializedProperty prop, string propertyName, GUIContent label)
	{
		SerializedProperty layerProperty = prop.FindPropertyRelative(propertyName);

		int newValue = EditorGUILayout.LayerField(label, layerProperty.intValue);

		if (newValue != layerProperty.intValue)
		{
			layerProperty.intValue = newValue;
			return true;
		}

		return false;
	}

	protected static bool IntPropertyField(SerializedProperty prop, string propertyName, GUIContent label, int minValue, int maxValue)
	{
		SerializedProperty property = prop.FindPropertyRelative(propertyName);

		if (property == null)
			return false;

		int intValue = property.intValue;
		int newIntValue = EditorGUILayout.IntField(label, intValue);

		newIntValue = Mathf.Clamp(newIntValue, minValue, maxValue);

		if (intValue != newIntValue)
		{
			property.intValue = newIntValue;
			return true;
		}

		return false;
	}

	/*
	protected static bool DelayedIntPropertyField  (SerializedProperty prop, string propertyName, GUIContent label, int minValue, int maxValue)
	{
		SerializedProperty property = prop.FindPropertyRelative(propertyName);

		if (property == null)
			return false;

		int intValue = property.intValue;
		int newIntValue = EditorGUILayout.DelayedIntField(label, intValue);

		newIntValue = Mathf.Clamp(newIntValue, minValue, maxValue);

		if (intValue != newIntValue)
		{
			property.intValue = newIntValue;
			return true;
		}

		return false;
	}
	*/

	protected static bool FloatPropertyField(SerializedProperty prop, string propertyName, GUIContent label, float minValue)
	{
		SerializedProperty floatProperty = prop.FindPropertyRelative(propertyName);

		bool changed = EditorGUILayout.PropertyField(floatProperty, label);

		if (floatProperty.floatValue < minValue)
			floatProperty.floatValue = minValue;

		return changed;
	}

	protected static bool FloatPropertyField (SerializedProperty prop, string propertyName, GUIContent label, float minValue, float maxValue)
	{
		SerializedProperty floatProperty = prop.FindPropertyRelative(propertyName);

		bool changed = EditorGUILayout.PropertyField(floatProperty, label);

		float value = floatProperty.floatValue;

		if (value < minValue)
			floatProperty.floatValue = minValue;
		else if (value > maxValue)
			floatProperty.floatValue = maxValue;

		return changed;
	}

	protected static void ClampMinMax(ref float valueMin, ref float valueMax, float limitMin, float limitMax)
	{
		if (valueMin < limitMin)
			valueMin = limitMin;
		else if (valueMin > limitMax)
			valueMin = limitMax;

		if (valueMax > limitMax)
			valueMax = limitMax;
		else if (valueMax < valueMin)
			valueMax = valueMin;
	}
}

public static class IMDrawEditorUtility
{
	/// <summary>
	/// Find first occurence of an asset by name and type.
	/// </summary>
	public static T FindAsset<T> (string name) where T : UnityEngine.Object
	{
		Type type = typeof(T);

		string [] guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", name, type.Name));

		if (guids.Length > 0)
		{
			return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), type);
		}

		return null;
	}
}

#endif // UNITY_EDITOR