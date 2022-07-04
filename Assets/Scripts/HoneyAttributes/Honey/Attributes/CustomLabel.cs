#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
namespace Honey.Editor
{
    public class HCustomLabelDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;
        public GUIContent? GetCustomContent(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent content)
        {
            HCustomLabelAttribute atr = (attribute as HCustomLabelAttribute)!;
            return string.IsNullOrEmpty(atr.Label) ? GUIContent.none : new GUIContent(atr.Label);
        }
           
    }
}

#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HCustomLabelAttribute : HoneyAttribute
    {
        public string Label { get; }
        public HCustomLabelAttribute(string label)
        {
            this.Label = label;
        }
    }
#if UNITY_EDITOR
#endif

}