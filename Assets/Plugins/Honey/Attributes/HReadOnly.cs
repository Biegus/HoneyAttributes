#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HReadOnlyDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;

        public HoneyPropertyState GetDesiredState(in HoneyDrawerInput inp, HoneyAttribute attribute)
        {
            return HoneyPropertyState.Disabled;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HReadOnlyAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}