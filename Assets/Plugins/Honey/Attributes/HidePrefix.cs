﻿using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HHidePrefixDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            inp.TempMemory.Push(EditorGUIUtility.labelWidth);

            EditorGUIUtility.labelWidth = 1;
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            EditorGUIUtility.labelWidth = (float)inp.TempMemory.Pop();
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HHidePrefixAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
#endif
}