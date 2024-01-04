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
        BoxGroup,
        Inline,
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowSubEditorAttribute : System.Attribute
    {
        public SubEditorMode Mode { get; }

        /// <summary>
        /// Mode that supports changing: Normal, Box,BoxGroup
        /// </summary>
        public bool Foldout { get; init; } = true;

        public AllowSubEditorAttribute(SubEditorMode mode=SubEditorMode.Normal)
        {
            Mode = mode;
        }
    }


}