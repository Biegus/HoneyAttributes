#nullable enable
using System;
using Honey;
using Honey.Helper;
using Honey.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class ColorDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private static HoneyValueExpressionGetterCache<bool> cache = new HoneyValueExpressionGetterCache<bool>();

        //hacky avoiding dividing by zero
        private Color Push(Color color)
        {
            return  new Color(color.r + 0.01f, color.g + 0.01f, color.b + 0.01f, color.a + 0.01f);
        }
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            HColorAttribute atr = attribute.As<HColorAttribute>();
            if (atr.Color==null)
            {
                inp.Listener.LogLocalWarning("Color is wrongly defined", inp.Field, atr);
                return;
            }
            if (atr.BExpression != string.Empty)
            {

                //error is displayer in after
                if (!(cache.GetAndInvoke(inp.Container, atr.BExpression, inp.Field,inp.SerializedProperty).Map(e => e).UnwrapOr(false)))
                {
                    return;
                }   
            }
            Color color = Push(atr.Color.Value);
            inp.TempMemory.Push(GUI.color);
            Color reverse = new Color(1f /color.r, 1f / color.g, 1f / color.b, 1f / color.a);

            if (atr.DontIncludeLabel)
            {
                EditorStyles.label.normal.textColor = reverse*EditorStyles.label.normal.textColor;
                EditorStyles.label.focused.textColor = reverse*EditorStyles.label.focused.textColor;
                EditorStyles.label.hover.textColor = reverse*EditorStyles.label.hover.textColor;
            }
            GUI.color =color;
        }

      

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            HColorAttribute atr = attribute.As<HColorAttribute>();
            if (atr.Color==null)
            {
                return;
            }
            if (atr.BExpression != string.Empty)
            {
                var maybeboolean= cache.GetAndInvoke(inp.Container, atr.BExpression, inp.Field, inp.SerializedProperty);
                if (maybeboolean.TryError(out var error))
                {
                    inp.Listener.LogLocalWarning($"BExpression failed: {error}",inp.Field,attribute);
                    return;
                }
                var boolean = maybeboolean.Unwrap();
                if (!boolean)
                {
                    return;
                }   
            }

            var data = (Color)inp.TempMemory.Pop();
            GUI.color = data;
            Color color = Push(atr.Color.Value);
            if (atr.DontIncludeLabel)
            {
                EditorStyles.label.normal.textColor *= color;
                EditorStyles.label.focused.textColor *= color;
                EditorStyles.label.hover.textColor *= color;
            }
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HColorAttribute: HoneyAttribute
    {
        public bool DontIncludeLabel { get; set; } = false;
        public Color? Color { get; }
        public string BExpression { get; set; } = string.Empty;
        
        public HColorAttribute(string colorExpression)
        {
            Color= ColorExpressionParser.ParseColorExpression(colorExpression);
        }
    }
}