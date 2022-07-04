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
            inp.TempMemory.Add((inp.Container,inp.Field ),GUI.backgroundColor);
            if (inp.SerializedProperty.propertyType == SerializedPropertyType.ManagedReference )
            {
                if(inp.SerializedProperty.managedReferenceValue==null)
                    GUI.backgroundColor = new Color32(210, 43, 43, 255);
            }
            else if (inp.SerializedProperty.objectReferenceValue  == null)
            {
                GUI.backgroundColor = new Color32(210, 43, 43, 255);
            }
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            GUI.backgroundColor = inp.TempMemory.TakeAndRemove<Color>((inp.Container, inp.Field));
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