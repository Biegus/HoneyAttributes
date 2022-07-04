#nullable enable
using System;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Honey.Editor
{
    public class HGetComponentButtonDrawer : IHoneyMainDrawer
    {
        private Dictionary<(FieldInfo field, object container), float> tickers = new();
        private const float REFRESH_TIME = 3f;

        public bool RequestHierarchyQuery => false;

        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title, Action<GUIContent, Rect> body)
        {
            var mono =
                inp.SerializedProperty.serializedObject.targetObject as MonoBehaviour;
                
            if (mono==null)
            {
                string error = "Target should be mono behaviour";
                inp.Listener.LogLocalWarning(error, inp.Field, attribute);
                return;
            }
            var atr = attribute.As<HGetComponentButtonAttribute>();
            Rect buttonRect = rect;
               
            rect.width -= 30f;
                
            buttonRect.width = 30f;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            buttonRect.x += rect.width;
            body(title, rect);
            string code = atr.Conf.HasFlag(ComponentBFlags.AddAutomaticallyIfNotPresent)
                ? "+/↧"
                : "↧";

            void Do(in HoneyDrawerInput input)
            {
                if (!typeof(Component).IsAssignableFrom(input.Field.FieldType))
                {
                    input.Listener.LogGlobalWarning($"Field type is {input.Field.FieldType}, this is not component", input.Field, attribute);
                    return;
                }
                if (atr.Conf.HasFlag(ComponentBFlags.AddAutomaticallyIfNotPresent))
                {
                    if (mono.GetComponent(input.Field.FieldType) == null)
                        mono.gameObject.AddComponent(input.Field.FieldType);
                }
                    
                if (atr.Conf.HasFlag(ComponentBFlags.IncludeChildren))
                    input.SerializedProperty.objectReferenceValue = mono.GetComponentInChildren(input.Field.FieldType);
                else
                    input.SerializedProperty.objectReferenceValue = mono.GetComponent(input.Field.FieldType);
            }
            if (GUI.Button(buttonRect, code))
            {
                Do(inp);
            }

            if (inp.Obj == null)
            {
                if(atr.Conf.HasFlag(ComponentBFlags.Aggressive))
                    if (EditorApplication.timeSinceStartup - tickers.GetValueOrDefault((inp.Field, inp.Container)) >
                        REFRESH_TIME)
                    {
                        tickers[(inp.Field, inp.Container)] = (float)EditorApplication.timeSinceStartup;
                        Do(inp);
                    }
            }
               
        }


        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return 0;
        }

        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return  null;
        }
    }
}
#endif

namespace Honey
{
    [Flags]
    public enum ComponentBFlags
    {
        None=0,
        IncludeChildren=1<<0,
        AddAutomaticallyIfNotPresent=1<<2,
        Aggressive=1<<3,
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class HGetComponentButtonAttribute : HoneyAttribute
    {
      
        public ComponentBFlags Conf { get; }

        public HGetComponentButtonAttribute(ComponentBFlags componentBFlags = ComponentBFlags.None)
        {
            this.Conf = componentBFlags;
        }
    }
#if UNITY_EDITOR
#endif

}