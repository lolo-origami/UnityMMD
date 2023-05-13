using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharaOnShadowBehaviour))]
public class CharaOnShadowBehaviourDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var toggle = property.FindPropertyRelative("IsShadowOn");

        var fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(fieldRect, toggle);
    }
}