using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MeshRendererBehaviour))]
public class MeshRendererBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var dropDown = property.FindPropertyRelative("ShadowCastingMode");
        EditorGUILayout.PropertyField(dropDown);
    }
}