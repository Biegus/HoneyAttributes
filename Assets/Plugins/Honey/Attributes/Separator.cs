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
    [CustomPropertyDrawer(typeof(ColorSeparatorAttribute))]
    public class ColorSeparatorDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var atr = attribute.As<ColorSeparatorAttribute>();
               
               
            EditorGUI.DrawRect(EditorGUI.IndentedRect(position), atr.Color);
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight / 5f;
        }

          
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]

    public class ColorSeparatorAttribute : PropertyAttribute
    {
        public Color Color;

        public ColorSeparatorAttribute(string color)
        {
            Color = ColorExpressionParser.ParseColorExpression(color)?? Color.white;
        }
    }
#if UNITY_EDITOR
#endif

}