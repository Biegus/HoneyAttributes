#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HShortArrayIndexDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;

        public GUIContent GetCustomContent(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent content)
        {
            content.text= content.text.Replace("Element", "");
            return content;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HShortArrayIndexAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}