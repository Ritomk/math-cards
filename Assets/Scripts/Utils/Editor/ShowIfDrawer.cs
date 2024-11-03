using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIfAttribute = attribute as ShowIfAttribute;
        SerializedProperty conditionProperty = GetConditionProperty(property, showIfAttribute.ConditionFieldName);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return 0f;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
    
        ShowIfAttribute showIfAttribute = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = GetConditionProperty(property, showIfAttribute.ConditionFieldName);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }


    private static SerializedProperty GetConditionProperty(SerializedProperty property, string conditionFieldName)
    {
        if (property.depth == 0)
        {
            return property.serializedObject.FindProperty(conditionFieldName);
        }
        else
        {
            string path = property.propertyPath.Replace(property.name, conditionFieldName);
            return property.serializedObject.FindProperty(path);
        }
    }
}
