#nullable enable
using Honey;
using Honey.Helper;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using Honey.Editor;

namespace Honey.Editor
{
    public class ResetValueDrawer : IHoneyMainDrawer
    {
        public bool RequestHierarchyQuery => false;

        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Enable;

        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title
            ,Action<GUIContent,Rect> innerBody)
        {
            var atr = attribute.As<HResetValueAttribute>();
            if (HoneyEG.SideButton(ref rect, "↺"))
            {
                inp.SerializedProperty.SetValue(atr.Value);
            }

            innerBody(title, rect);

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
    public class HResetValueAttribute : HoneyAttribute
    {
            public object Value { get; }

            public HResetValueAttribute(object value)
            {
                Value = value;
            }
    }


}