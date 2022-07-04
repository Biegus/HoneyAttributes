#nullable enable
using System;
using Honey;
using Honey.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(CurveBoundAttribute))]
    public class CurveBoundDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            if (property.propertyType != SerializedPropertyType.AnimationCurve)
            {
                string error = $"({property.propertyType} {property.displayName}) - type is not animation curve";
                HoneyEG.DrawLocalAttributeError(position,fieldInfo,property.displayName,attribute,error);
                return;
            }
            CurveBoundAttribute atr = (attribute as CurveBoundAttribute)!;
            Rect ranges = atr.Rect;
            property.animationCurveValue = EditorGUI.CurveField(position,label, property.animationCurveValue,atr.CurveColor,ranges);
            EditorGUI.EndProperty();

        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CurveBoundAttribute : PropertyAttribute
    {
     
        public Rect Rect { get; }
        public Color CurveColor { get; }
        
        public CurveBoundAttribute(float xMin,float xMax, float yMin, float yMax, string? colorExpr=null)
        {
             Rect = new Rect(new Vector2(xMin, yMin), new Vector2(xMax-xMin, yMax-yMin));
             if (colorExpr != null)
                 CurveColor = ColorExpressionParser.ParseColorExpression(colorExpr) ?? Color.green;
             else
                 CurveColor = Color.green;
        }
    }
#if UNITY_EDITOR
#endif

}