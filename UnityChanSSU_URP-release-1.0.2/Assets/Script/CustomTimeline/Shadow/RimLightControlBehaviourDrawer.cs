using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RimLightControlBehaviour))]
public class RimLightControlBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var toggleRim = property.FindPropertyRelative("EnableRim");
        var toggleDarkRim = property.FindPropertyRelative("EnableDarkRim");
        
        var RimSmoothValue = property.FindPropertyRelative("RimSmoothValue");
        var RimPowValue = property.FindPropertyRelative("RimPowValue");
        
        var DarkRimSmoothValue = property.FindPropertyRelative("DarkRimSmoothValue");
        var DarkRimPowValue = property.FindPropertyRelative("DarkRimPowValue");
        
        EditorGUILayout.PropertyField(toggleRim);
        EditorGUILayout.PropertyField(RimSmoothValue);
        EditorGUILayout.PropertyField(RimPowValue);
        
        EditorGUILayout.PropertyField(toggleDarkRim);        
        EditorGUILayout.PropertyField(DarkRimSmoothValue);
        EditorGUILayout.PropertyField(DarkRimPowValue);        
    }
}