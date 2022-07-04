using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Honey
{
    public enum SubEditorMode
    {
        Normal,
        VectLike,
        Box,
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowSubEditorAttribute : System.Attribute
    {
        public SubEditorMode Mode { get; }

        public AllowSubEditorAttribute(SubEditorMode mode=SubEditorMode.Normal)
        {
            Mode = mode;
        }
    }


}