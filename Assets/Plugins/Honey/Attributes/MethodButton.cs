#nullable enable
using System;
using System.Reflection;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Core;
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class MethodButtonDrawer : IHoneyCustomElementDrawer
    {
        public void DrawMethodLayout(MethodInfo info, Attribute attribute,object container)
        {
            var atr = (attribute as EMethodButtonAttribute)!;
            if (GUILayout.Button(info.Name,GUILayout.Height(atr.ButtonSize*EditorGUIUtility.singleLineHeight)))
            {
                   
                info.Invoke(container, null);

            }
        }
    }
}
#endif

namespace Honey
{
   
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EMethodButtonAttribute : Attribute
    {
        public int ButtonSize { get; }

        public EMethodButtonAttribute(int buttonSize=1)
        {
            ButtonSize = buttonSize;
        }
    }
#if UNITY_EDITOR
#endif

}