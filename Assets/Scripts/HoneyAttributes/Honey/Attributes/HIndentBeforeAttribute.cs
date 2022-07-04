#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class HIndentThisDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;
       

        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            HIndentBeforeAttribute atr = (attribute as HIndentBeforeAttribute)!;
            EditorGUI.indentLevel += atr.Value;
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            HIndentBeforeAttribute atr = (attribute as HIndentBeforeAttribute)!;
            EditorGUI.indentLevel -= atr.Value;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HIndentBeforeAttribute : HoneyAttribute
    {
        public int Value { get; }

        public HIndentBeforeAttribute(int value)
        {
            Value = value;
        }
    }
#if UNITY_EDITOR
#endif

}