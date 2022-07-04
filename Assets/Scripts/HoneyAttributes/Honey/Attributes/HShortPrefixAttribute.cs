#nullable enable
using Honey;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class HShortPrefixDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            inp.TempMemory.Add((this,inp.Field),EditorGUIUtility.labelWidth);

              
            EditorGUIUtility.labelWidth= Mathf.Max(5,   EditorStyles.label.CalcSize(new GUIContent(inp.SerializedProperty.displayName)).x+EditorGUI.indentLevel* 15);
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            EditorGUIUtility.labelWidth = inp.TempMemory.TakeAndRemove<float>((this, inp.Field));
        }
    }   
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HShortPrefixAttribute : HoneyAttribute
    {
     

     
    }


}