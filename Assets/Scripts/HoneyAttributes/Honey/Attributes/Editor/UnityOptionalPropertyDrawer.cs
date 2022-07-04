#if UNITY_EDITOR
using Honey.Objects;
using UnityEditor;
using UnityEngine;
namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(HoneyOptional<>))]
    public class UnityOptionalPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
     
            const float BOOL_SIZE = 15;
            
            EditorGUI.BeginProperty(position, label, property);
            var custom = property.FindPropertyRelative("custom");
            var value = property.FindPropertyRelative("value");

            (Rect labelRect, Rect content) = HoneyEG.CutIntoLabelAndField(position);
            
            EditorGUI.LabelField(labelRect,label);

            content.width = BOOL_SIZE;
            
            custom.boolValue= GUI.Toggle(content, custom.boolValue,string.Empty);
            Debug.Log(content.width);
            content.x += content.width;
            content.width = position.width - content.width - labelRect.width;

            if (custom.boolValue)
                EditorGUI.PropertyField(content, value, GUIContent.none);
            else
            {
                EditorGUI.LabelField(content,"null",new GUIStyle( EditorStyles.boldLabel));
            }
           
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif