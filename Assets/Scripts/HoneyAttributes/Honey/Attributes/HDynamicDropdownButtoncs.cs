#nullable enable
using System;
using Honey;
using Honey.Core;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections;

namespace Honey.Editor
{
    public class HDynamicDropdownButtonDrawer : IHoneyMainDrawer
    {
        public bool RequestHierarchyQuery => true;
        private HoneyValueExpressionGetterCache<IEnumerable> cache = new HoneyValueExpressionGetterCache<IEnumerable>();
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title, Action<GUIContent, Rect> body)
        {
            var atr = attribute.As<HDynamicDropdownButtonAttribute>();
                
            if (HoneyEG.SideButton(ref rect,"☰"))
            {
                GUI.FocusControl(null);
                (GUIContent[] names, IEnumerable values) = 
                    HoneyNamedElHelper.SplitObjectArray( cache.Get(atr.References, inp.Field,inp.SerializedProperty)(inp.Container));
                    
                HoneyEG.BuildGenericMenu(inp.SerializedProperty.propertyPath, (UnityEngine.Object) inp.Container, values,inp.Field,names)
                    .ShowAsContext();
            }
            body(title, rect);
        }

        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return 0;
        }

        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return null;
        }
    }
}
#endif

namespace Honey
{
    public class HDynamicDropdownButtonAttribute : HoneyAttribute
    {
        public string References { get; }

        public HDynamicDropdownButtonAttribute( string references)
        {
            References = references;
        }
    }
#if UNITY_EDITOR
#endif

}