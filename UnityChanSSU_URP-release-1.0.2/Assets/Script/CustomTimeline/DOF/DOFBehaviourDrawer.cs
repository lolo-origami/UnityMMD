using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DOFBehaviour))]
public class DOFBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var startColorProp = property.FindPropertyRelative("StartValue");
        var endColorProp = property.FindPropertyRelative("EndValue");
        var overRideProp = property.FindPropertyRelative("Override");

        var fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(fieldRect, startColorProp);

        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, endColorProp);
        
        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, overRideProp);
    }
}