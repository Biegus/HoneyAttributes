#nullable enable
using System.Globalization;
using Honey;
using Honey.Core;
using UnityEngine;
using System;
using Honey.Helper;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System.Collections.Generic;
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(SliderAttribute))]
    public class SliderDrawer : PropertyDrawer
    {
        private readonly HoneyValueExpressionGetterCache<float> floatCache = new();
        private readonly HoneyValueExpressionGetterCache<long> intCache = new();

        private IHoneyValueExpressionGetterCache? GetCache(SerializedPropertyType type)
        {
            return type switch
            {
                SerializedPropertyType.Integer => intCache,
                SerializedPropertyType.Float => floatCache,
                _=> null
            };
        }

        private void ApplySliderFor( SerializedProperty prop, Rect rect, GUIContent label, object left,
            object right)
        {
             switch(prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue= EditorGUI.IntSlider(rect, (int) prop.intValue , (int)(long) left, (int)(long) right);
                    return;
                case SerializedPropertyType.Float:
                    prop.floatValue= EditorGUI.Slider(rect, (float) prop.floatValue, (float) left, (float) right);
                    return;

                default: throw new ArgumentException("Not supported type");
            }
        }

        private void DrawProgressBarFor(SerializedProperty property, Rect rect, Color barColor, Color labelColor,
            object left, object right)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    HoneyEG.ProgressBar(rect, (property.floatValue - (float)left) / ((float)right - (float)left),
                        $"{property.floatValue}", barColor, labelColor);
                    return;
                case SerializedPropertyType.Integer:
                    HoneyEG.ProgressBar(rect, (property.intValue - (long)left) / ((float)((long)right - (long)left)),
                        $"{property.intValue}", barColor, labelColor);
                    return;
                default: throw new ArgumentException("Not supported type");
            }
        }

        private void DrawContent(Rect position, SerializedProperty property, GUIContent label)
        {
            SliderAttribute atr = (attribute as SliderAttribute)!;

                        HierarchyQuery query = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property.propertyPath,
                            property.serializedObject.targetObject.GetType());
                        object? container = query.RetrieveTwoLast(property.serializedObject.targetObject).container;

                        Rect labelRect = position;
                        labelRect.width = EditorGUIUtility.labelWidth;
                        Rect field = position;
                        field.width = position.width - labelRect.width;
                        field.x += labelRect.width;

                        EditorGUI.LabelField(labelRect,label);

                        var cache = GetCache(property.propertyType);
                        if (cache == null)
                        {
                            HoneyErrorListenerStack.GetListener().LogError("Slider doesn't support this type", null, attribute);
                            return;
                        }


                        IHoneyErrorListener listener = HoneyErrorListenerStack.GetListener();

                        var maybeLeft = cache.GetAndInvokeRaw(container , atr.Left, fieldInfo, property);
                        var maybeRight = cache.GetAndInvokeRaw(container, atr.Right, fieldInfo, property);
                        {
                            if (maybeLeft.TryError(out string er))
                            {
                                listener.LogLocalWarning(er, null, attribute);
                                return;
                            }
                        }
                        {
                            if (maybeRight.TryError(out string er))
                            {
                                listener.LogLocalWarning(er, null, attribute);
                                return;
                            }
                        }
                        object left = maybeLeft.Unwrap();
                        object right = maybeRight.Unwrap();

                        Action<Rect> drawSlider = (r) => ApplySliderFor(property, r, label, left, right);

                        if (atr.Mode == SliderMode.ProgressBar)
                        {
                            Rect numberField = field;
                            numberField.width = EditorGUIUtility.fieldWidth;
                            Rect nonNumberField = field;
                            nonNumberField.width = field.width - EditorGUIUtility.fieldWidth-10f;
                            nonNumberField.x += numberField.width+10f;

                            var colors = HoneyEG.GetColorsFromProgressStyle(ProgressBarStyle.Blue);

                            DrawProgressBarFor(property, nonNumberField, colors.barColor, colors.labelColor, left, right);
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
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            DrawContent(position, property, label);
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
    /// Honey version of UnityEngine.Range, support expressions.    
    /// Works for floats and integer types. For integers it cannot handle values bigger than max int value (2^31-1) due to unity slider  api.
    /// For integers when using expression, it should return long not int.
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
        public SliderAttribute(float a, float b, SliderMode mode = SliderMode.Normal)
        {
            Left = a.ToString(CultureInfo.InvariantCulture);
            Right = b.ToString(CultureInfo.InvariantCulture);
            Mode = mode;
        }
        public SliderAttribute(int a, int b,SliderMode mode = SliderMode.Normal)
        {
            Left = a.ToString(CultureInfo.InvariantCulture);
            Right = b.ToString(CultureInfo.InvariantCulture);
            Mode = mode;
        }


    }
#if UNITY_EDITOR
#endif

}