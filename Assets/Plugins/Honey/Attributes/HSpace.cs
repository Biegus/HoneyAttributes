#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HSpaceDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;

        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            if (type == AdditionalDrawerCallType.PreBefore)
            {
                var atr = (attribute as HSpaceAttribute)!;
                return EditorGUIUtility.singleLineHeight * atr.Amount;
            }
            else return 0;


        }

        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
        }

    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HSpaceAttribute : HoneyAttribute
    {
        public int Amount { get; }

        public HSpaceAttribute(int amount=1)
        {
            Amount = amount;
        }
    }
#if UNITY_EDITOR
#endif

}