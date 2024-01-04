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
        public bool RequestHierarchyQuery => true;

        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Deepest;
        private readonly WeakAttributeCache<bool> foldout=new();
        private readonly WeakAttributeCache<UnityEditor.Editor> editorCache = new();

        private static int recursiveCounter; // to block non trivial recursive
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title, Action<GUIContent, Rect> body)
        {
            body(title,rect);
            if(!inp.AllowLayout)
            {
                inp.Listener.LogLocalWarning("Layout context is required. If you are in editor context, use EHoneyRun instead of HoneyRun", inp.Field, attribute);
                  
                return;
            }
            if (recursiveCounter > 5)
            {
                inp.Listener.LogLocalWarning("Recursive limit exceeded", inp.Field, attribute);
                   return;
            }
            if (inp.SerializedProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                inp.Listener.LogLocalWarning("Field should be UnityEngine.Object. This won't work for lists, since it has to work through EHoneyRun", inp.Field, attribute);
                return;
            }
            if ( inp.SerializedProperty.objectReferenceValue==null || inp.SerializedProperty.objectReferenceValue is GameObject)
            {
                return;
            }
            Rect mini =rect;
            mini.width /= 3f;
            mini.y += rect.height - EditorGUIUtility.singleLineHeight;
            mini.height = EditorGUIUtility.singleLineHeight;


            bool on = foldout.GetOrElseInsert(inp.SerializedProperty,attribute, () => false);

            on = EditorGUI.Foldout(mini, on, string.Empty, true);
            foldout.Set(inp.SerializedProperty.serializedObject.targetObject, inp.SerializedProperty.propertyPath,attribute, on);

                 
            body(title, rect);
            if (!on) return;

            if (inp.SerializedProperty.objectReferenceValue == (UnityEngine.Object) inp.Container)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Directly recursive",MessageType.Info);
                EditorGUI.indentLevel--;
                return;
            }
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");

            UnityEditor.Editor editor;
            if (editorCache.TryGetValue(inp.SerializedProperty,attribute, out var res)&&  res !=null)
            {
                editor = res;

            }
            else
            {
                editor = editorCache.Set(inp.SerializedProperty, attribute,
                    UnityEditor.Editor.CreateEditor(
                        inp.SerializedProperty.objectReferenceValue));

            }



            recursiveCounter++;
            editor.OnInspectorGUI();
            recursiveCounter--;
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

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