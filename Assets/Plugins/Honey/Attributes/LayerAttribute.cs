#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                HoneyEG.DrawLocalAttributeError(position,fieldInfo,property.displayName,attribute,"Type is not integer");
                return;
            }
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
#endif

namespace Honey

{
    /// <summary>
    /// Attributes for ints.
    /// Lets pick layer from dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LayerAttribute : PropertyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}