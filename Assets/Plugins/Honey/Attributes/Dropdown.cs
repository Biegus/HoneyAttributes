#nullable enable
using Honey;
using Honey.Helper;
using UnityEngine;
using System;

#if UNITY_EDITOR

namespace Honey.Editor
{
    public class HDropdownButtonDrawer : IHoneyMainDrawer
    {
        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Enable;
        public bool RequestHierarchyQuery => true;
        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return null;
        }
        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return 0;
        }
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title
            ,Action<GUIContent,Rect> body)
        {

            var atr = attribute.As<HDropdownButtonAttribute>();
                
            string path = inp.SerializedProperty.propertyPath;
               
            if (HoneyEG.SideButton(ref rect,"☰"))
            {
                GUI.FocusControl(null);
                HoneyEG.BuildGenericMenu(path, (UnityEngine.Object) inp.Container, atr.Values,inp.Field)
                    .ShowAsContext();
            }
            body(title, rect);
        }

         
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HDropdownButtonAttribute : HoneyAttribute
    {
        public object[] Values { get; }

        public HDropdownButtonAttribute(params object[] values)
        {
            this.Values = values;
        }
    }


}