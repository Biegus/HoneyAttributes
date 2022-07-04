#nullable enable
using System.Globalization;
using Honey;
using Honey.Core;
using UnityEngine;
using System;

#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(SliderAttribute))]
    public class SliderDrawer : PropertyDrawer
    {
        private HoneyValueExpressionGetterCache<float> floatCache = new();
        private HoneyValueExpressionGetterCache<long> intCache = new();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            var atr = (attribute as SliderAttribute)!;
            var query = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property.propertyPath,
                property.serializedObject.targetObject.GetType());
            var container = query.RetrieveTwoLast(property.serializedObject.targetObject).container;
            float floatLeft = floatCache.Get(atr.Left, fieldInfo,property)(container);
            float floatRight = floatCache.Get(atr.Right, fieldInfo,property)(container);

            long intLeft = (property.propertyType != SerializedPropertyType.Float)
                ? intCache.Get(atr.Left, fieldInfo,property)(container)
                : -1;
            long intRight = (property.propertyType != SerializedPropertyType.Float)
                ? intCache.Get(atr.Right, fieldInfo,property)(container)
                : -1;
               
            Rect labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;
            Rect field = position;
            field.width = position.width - labelRect.width;
            field.x += labelRect.width;
            EditorGUI.LabelField(labelRect,label);
            Action<Rect> drawSlider;
              
            if (property.propertyType == SerializedPropertyType.Float)
            {
                    
                drawSlider=(r)=>property.floatValue=  EditorGUI.Slider(r, GUIContent.none, property.floatValue, floatLeft, floatRight);
            }
            else 
            {
                drawSlider =(r)=>property.intValue= EditorGUI.IntSlider(r,label, property.intValue,(int) intLeft,(int) intRight);
            }

            if (atr.Mode == SliderMode.ProgressBar)
            {
                Rect numberField = field;
                numberField.width = EditorGUIUtility.fieldWidth;
                Rect nonNumberField = field;
                nonNumberField.width = field.width - EditorGUIUtility.fieldWidth-10f;
                nonNumberField.x += numberField.width+10f;
                var colors = HoneyEG.GetColorsFromProgressStyle(ProgressBarStyle.Blue);
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    HoneyEG.ProgressBar(nonNumberField, (property.floatValue - floatLeft) / (floatRight - floatLeft),
                        $"{property.floatValue}", colors.barColor, colors.labelColor);
                }
                else
                {
                    HoneyEG.ProgressBar(nonNumberField, (property.intValue - intLeft) / (float) (intRight - intLeft),
                        $"{property.intValue}", colors.barColor, colors.labelColor);
                }

                EditorGUI.PropertyField(numberField, property, GUIContent.none);
                Rect cut = nonNumberField;
                cut.width += Mathf.Min(EditorGUIUtility.fieldWidth, nonNumberField.width);
                Color old = GUI.color;

                GUI.color = new Color(0, 0, 0, 0);
                drawSlider(cut);
                GUI.color = old;
            }
            else
                drawSlider(field);
            EditorGUI.EndProperty();
               
        }   
    }
}
#endif

namespace Honey
{
    public enum SliderMode
    {
        Normal,
        ProgressBar
    }
    /// <summary>
    /// Works for floats and integer types. For integers it cannot handle values bigger than max int value (2^31-1) due to unity slider  api.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SliderAttribute : PropertyAttribute
    {
        public string Left { get; }
        
        public string Right { get; }
        
        public SliderMode Mode { get;  }

        public SliderAttribute(string left, string right,SliderMode mode= SliderMode.Normal)
        {
            Left = left;
            Right = right;
            Mode = mode;
        }
        public SliderAttribute(float a, float b)
        {
            Left = a.ToString(CultureInfo.InvariantCulture);
            Right = b.ToString(CultureInfo.InvariantCulture);
        }
        public SliderAttribute(int a, int b)
        {
            Left = a.ToString(CultureInfo.InvariantCulture);
            Right = b.ToString(CultureInfo.InvariantCulture);
        }
        
    }
#if UNITY_EDITOR
#endif

}