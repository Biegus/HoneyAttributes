#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(AssetsOnlyAttribute))]
    public class ObjectTypeLimitsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            Type type = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property.propertyPath,
                    property.serializedObject.targetObject.GetType())
                .FinalType;
            property.objectReferenceValue =
                EditorGUI.ObjectField(position,label, property.objectReferenceValue, type,false);
            EditorGUI.EndProperty();
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AssetsOnlyAttribute : PropertyAttribute
    {
        
    }
#if UNITY_EDITOR
#endif

}