#nullable enable
using System;
using System.Collections.Generic;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
        
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
               
            EditorGUI.BeginProperty(position, label, property);
            var atr = (attribute as DropdownAttribute)!;
            (var obj,var container) = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property).RetrieveTwoLast(property.serializedObject.targetObject);

            (Rect labelRect, Rect dropdown) = HoneyEG.CutIntoLabelAndField(position);
                
            EditorGUI.LabelField(labelRect,label);

            if (EditorGUI.DropdownButton(dropdown, new GUIContent(obj?.ToString()??"<null>"), FocusType.Passive))
            {
                HoneyEG.BuildGenericMenu(property.propertyPath, (UnityEngine.Object) container, atr.Values,fieldInfo)
                    .DropDown(dropdown);
            }
                
            EditorGUI.EndProperty();
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class DropdownAttribute : PropertyAttribute
    {
        private object[] values;
        public IReadOnlyList<object> Values => values;

        public DropdownAttribute(params object[] values)
        {
            this.values = values;
        }

    }
#if UNITY_EDITOR
#endif

}