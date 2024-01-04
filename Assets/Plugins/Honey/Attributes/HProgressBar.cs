#nullable enable
using System;
using Honey;
using Honey.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class HJustProgressBarDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private HoneyValueExpressionGetterCache<float> cache = new();
        private GUIStyle? style;
        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HJustProgressBarAttribute)!;
            var maybeValue = cache.GetAndInvoke(inp.Container, atr.FloatExpression, inp.Field, inp.SerializedProperty);
            var maybeStart = cache.GetAndInvoke(inp.Container, atr.Start, inp.Field, inp.SerializedProperty);
            var maybeEnd = cache.GetAndInvoke(inp.Container, atr.End, inp.Field,inp.SerializedProperty);

            var error = Hr.EitherError(maybeValue, maybeStart, maybeEnd);
            if (error != null)
            {
                inp.Listener.LogLocalWarning(error,inp.Field,attribute);
                return;
            }

            float value = maybeValue.Unwrap();
            float start = maybeStart.Unwrap();
            float end = maybeEnd.Unwrap();

            float percentValue = Mathf.Clamp01( (value - start) / (end - start));

            Color barColor;
            Color labelColor;
            (barColor, labelColor) = HoneyEG.GetColorsFromProgressStyle(atr.Style);


            string name = atr.Name ?? atr.FloatExpression;

            if(!atr.UseUnity)
                HoneyEG.ProgressBar(rect,percentValue,$"<b>{name} </b> ({value:0.00}/{end})",barColor,labelColor );
            else
            {
                EditorGUI.ProgressBar(rect, percentValue, $"{name} ({value:0.00}/{end})");
            }
        }
          

        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {

            var atr = (attribute as HJustProgressBarAttribute)!;
            if (type == AdditionalDrawerCallType.PreBefore)
                return EditorGUIUtility.singleLineHeight*atr.Size;
            else return 0;
        }

           
    }
}
#endif

namespace Honey
{
    public enum ProgressBarStyle
    {
        Red,
        Blue,
    }
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = true)]
    public class HJustProgressBarAttribute : HoneyAttribute
    {
        public string FloatExpression { get; }
        public string Start { get; }
        public string End { get; }
      
  
        public ProgressBarStyle Style { get; }
        public bool UseUnity { get; set; }
        public string? Name { get; set; }
        public int Size { get; set; } = 1;

        public HJustProgressBarAttribute(string floatExpression, string start="0",string end="1", ProgressBarStyle style = ProgressBarStyle.Blue)
        {
            FloatExpression = floatExpression;
            Start = start;
            End = end;
            Style = style;
        }

    }
#if UNITY_EDITOR
#endif

}