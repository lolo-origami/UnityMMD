using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReceiveShadowBehaviour))]
public class ReceiveShadowBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var toggle = property.FindPropertyRelative("IsOn");
        var toggleOverRideArea = property.FindPropertyRelative("IsOverRideArea");
        var startColorProp = property.FindPropertyRelative("StartShadowAreaValue");
        var endColorProp = property.FindPropertyRelative("EndShadowAreaValue");
        
        EditorGUILayout.PropertyField(toggle);
        EditorGUILayout.PropertyField(toggleOverRideArea);
        EditorGUILayout.PropertyField(startColorProp);
        EditorGUILayout.PropertyField(endColorProp);
    }
}