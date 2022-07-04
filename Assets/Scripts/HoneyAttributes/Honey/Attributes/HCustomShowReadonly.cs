#nullable enable
using System;
using System.Reflection;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Core;
using UnityEditor;

namespace Honey.Editor
{
    public class EShowAsStringDrawer : IHoneyCustomElementDrawer
    {
        public bool RequestHierarchyQuery => true;
        private GetterCache<object> cache = new GetterCache<object>();

        private void DrawLayout(MemberInfo info,object container)
        {
            object value = cache.Get(info.Name, info).Invoke(container);
            EditorGUILayout.BeginHorizontal();
                
            EditorGUILayout.LabelField(info.Name,GUILayout.Width(EditorGUIUtility.labelWidth));
            bool old = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.TextField(value?.ToString()??">null<");
            GUI.enabled = old;
            EditorGUILayout.EndHorizontal();
            ;
        }
        public void DrawPropertyLayout(PropertyInfo info, Attribute attributes, object container)
        {

            DrawLayout(info,container);
        }

        public void DrawMethodLayout(MethodInfo info, Attribute attribute, object container)
        {
            if (info.GetParameters().Length > 0)
            {
                HoneyEG.DrawLocalAttributeError(EditorGUILayout.GetControlRect(),info,info.ReturnType.ToString(),attribute,
                    "Method should be parameterless");
                return;
            }

            if (info.ReturnType == typeof(void))
            {
                HoneyEG.DrawLocalAttributeError(EditorGUILayout.GetControlRect(),info,info.ReturnType.ToString(),attribute,
                    "Method should return something");
                return;
            }
            DrawLayout(info,container);

        }

        public void DrawNonSerializedFieldLayout(FieldInfo field, Attribute attribute, object container)
        {
            DrawLayout(field,container);
        }
    }
}
#endif

namespace Honey
{
    public class EShowAsStringAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}