#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Honey;
using Honey.Core;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(HoneyRun))]
    public class RunHoneyDrawer : PropertyDrawer
    {
        private static readonly WeakAttributeCache<bool> editedCache = new();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float res = 0;
            HoneyEG.DoSafe(() =>
            {
                res= HoneyHandler.HoneyReflectionCache.Get(fieldInfo).QueryHeight(property, fieldInfo ,label,
                    HoneyErrorListenerStack. GetListener(),false,null);
            },property);

            return res;

        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent unsafeLabel)
        {
            GUIContent label = new GUIContent(unsafeLabel);//unity reuses its labels in a weird way, copy fixes that

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            IHoneyErrorListener listener = HoneyErrorListenerStack.GetListener();
            Stack<object> memory = new Stack<object>();

            HoneyEG.DoSafe(() =>
            {
                HoneyHandler.HoneyReflectionCache.Get(fieldInfo).OnGui(
                    property,
                    fieldInfo,
                    position,
                    memory,
                    label,
                    listener,layoutContext: false, changed: editedCache.GetOrElseInsert(property,null,()=>false));
            },property);

            editedCache.Set(property,null, EditorGUI.EndChangeCheck());
            EditorGUI.EndProperty();
            if (memory.Count > 0)
            {
                listener.LogLocalWarning("memory was not clean", null, null);
            }

            ;
        }
        
    }
}
#endif