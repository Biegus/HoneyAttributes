#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                HoneyEG.DrawLocalAttributeError(position,fieldInfo,property.displayName,attribute,"Type is not string");
                return;
            }
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TagAttribute : PropertyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}