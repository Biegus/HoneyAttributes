#if UNITY_EDITOR
#nullable enable
using System.Reflection;
using System;
using Honey.Objects;
using Honey.Editor;
using Honey.Core;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(InspectorButtonDataAttribute))]
    public class EditorButtonDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var info = (InspectorButtonDataAttribute)attribute;
            return info.Size switch
            {
                InspectorButtonSize.Normal => EditorGUIUtility.singleLineHeight,
                InspectorButtonSize.Medium => EditorGUIUtility.singleLineHeight * 2,
                InspectorButtonSize.Thick => EditorGUIUtility.singleLineHeight * 3,
                _=>EditorGUIUtility.singleLineHeight
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {

            EditorGUI.BeginProperty(position, null, property);
            InspectorButtonDataAttribute info= ((InspectorButtonDataAttribute)attribute);
            var input = property.FindPropertyRelative("input");
            
            EditorMethodButtonHelper.Draw(position, property,input,info.ArgumentDescription??string.Empty,info.Limit,info.Name,info.Method);
         
            EditorGUI.EndProperty();

        }
    }
}
#endif