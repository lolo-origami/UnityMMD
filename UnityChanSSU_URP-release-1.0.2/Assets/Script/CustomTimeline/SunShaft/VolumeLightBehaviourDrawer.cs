using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VolumeLightBehaviour))]
public class VolumeLightBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var startDensity = property.FindPropertyRelative("StartDensity");
        var endDensity = property.FindPropertyRelative("EndDensity");
        
        var startBrightness = property.FindPropertyRelative("StartBrightness");
        var endBrightness = property.FindPropertyRelative("EndBrightness");
        
        var startRangeFallOff = property.FindPropertyRelative("StartRangeFallOff");
        var endRangeFallOff = property.FindPropertyRelative("EndRangeFallOff");
        
        var startDiffusionIntensity = property.FindPropertyRelative("StartDiffusionIntensity");
        var endDiffusionIntensity = property.FindPropertyRelative("EndDiffusionIntensity");
        
        var transluency = property.FindPropertyRelative("Transluency");
        
        
        EditorGUILayout.PropertyField(startDensity);
        EditorGUILayout.PropertyField(endDensity);
        
        EditorGUILayout.PropertyField(startBrightness);
        EditorGUILayout.PropertyField(endBrightness);

        EditorGUILayout.PropertyField(startRangeFallOff);
        EditorGUILayout.PropertyField(endRangeFallOff);
        
        EditorGUILayout.PropertyField(startDiffusionIntensity);
        EditorGUILayout.PropertyField(endDiffusionIntensity);
        
        EditorGUILayout.PropertyField(transluency);
    }
}