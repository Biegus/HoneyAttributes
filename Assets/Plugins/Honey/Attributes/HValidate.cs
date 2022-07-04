#nullable enable
using Honey;
using Honey.Core;
using UnityEngine;
using System;

#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HValidateDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;

        private HoneyValueExpressionGetterCache<bool> cache = new();
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HValidateAttribute)!;
            bool res= cache.Get(atr.BoolExpression, inp.Field,inp.SerializedProperty)(inp.Container);
            inp.TempMemory.Add((inp.Container, inp.Field), GUI.backgroundColor);
            if (!res)
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
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HValidateAttribute : HoneyAttribute
    {
            public string BoolExpression { get; }

            public HValidateAttribute(string boolExpression)
            {
                BoolExpression = boolExpression;
            }
    }
#if UNITY_EDITOR
#endif

}