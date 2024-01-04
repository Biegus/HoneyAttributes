#nullable enable
using System;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class HNotNullDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            inp.TempMemory.Push(GUI.backgroundColor);
            if (inp.SerializedProperty.propertyType == SerializedPropertyType.ManagedReference )
            {
                if(inp.SerializedProperty.managedReferenceValue==null)
                    GUI.backgroundColor = new Color32(210, 43, 43, 255);
            }
            else if (inp.SerializedProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                inp.Listener.LogLocalWarning("HNonNull works only for managed/unity references types", inp.Field,
                    attribute);
                return;
            }

            if (inp.SerializedProperty.objectReferenceValue  == null)
            {
                GUI.backgroundColor = new Color32(210, 43, 43, 255);
            }
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            GUI.backgroundColor = (Color)inp.TempMemory.Pop();
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HNotNullAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
#endif

}