#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Editor;
using Honey.Core;
using UnityEditor;

namespace Honey.Editor
{
    public class EHPreviewDrawer : IHoneyMainDrawer
    {
        public bool RequestHierarchyQuery => false;

        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Deepest;
        private readonly Dictionary<FieldInfo, bool> dict = new Dictionary<FieldInfo, bool>();
        private readonly Dictionary<HoneySerializedPropertyId, (UnityEditor.Editor editor, UnityEngine.Object rf)> editorCache = new();
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title, Action<GUIContent, Rect> body)
        {
            body(title,rect);
            if(!inp.AllowLayout)
            {
                inp.Listener.LogLocalWarning("Layout context is required. If you are in editor context, use EHoneyRun instead of HoneyRun", inp.Field, attribute);
                  
                return;
            }
            if (inp.SerializedProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                inp.Listener.LogLocalWarning("Field should be UnityEngine.Object", inp.Field, attribute);
                return;
            }
            if ( inp.SerializedProperty.objectReferenceValue==null || inp.SerializedProperty.objectReferenceValue is GameObject)
            {
                return;
            }
            var key = new HoneySerializedPropertyId(inp.Field,
                inp.SerializedProperty.serializedObject.targetObject,
                SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(inp.SerializedProperty
                    .propertyPath));
            Rect mini =rect;
            mini.width /= 3f;
            mini.y += rect.height - EditorGUIUtility.singleLineHeight;
            mini.height = EditorGUIUtility.singleLineHeight;
            if (!dict.ContainsKey(inp.Field))
                dict[inp.Field] = true;
              
            bool on = dict[inp.Field] = EditorGUI.Foldout(mini, dict[inp.Field], string.Empty,true);
                 
            body(title, rect);
            if (@on)
            {
                 
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");
                   
                UnityEditor.Editor editor;
                if (editorCache.TryGetValue(key, out var res)&& res.rf== inp.SerializedProperty.objectReferenceValue && res.editor!=null)
                {
                    editor = res.editor;
                        
                }
                else
                {
                    editorCache[key] = (
                        UnityEditor.Editor.CreateEditor(
                            inp.SerializedProperty.objectReferenceValue),
                        inp.SerializedProperty.objectReferenceValue);
                    editor = editorCache[key].editor;
                }


                    
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
               
        }

        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return 0;
        }

        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return null;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EHPreviewAttribute : HoneyEBasedAttritube
    {

    }
#if UNITY_EDITOR
#endif

}