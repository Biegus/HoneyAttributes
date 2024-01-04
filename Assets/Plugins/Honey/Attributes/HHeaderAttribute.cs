#nullable enable
#nullable enable
using System;
using Honey.Objects;
using Honey;
using Honey.Core;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HHeaderDrawer : IHoneyAdditionalDrawer
    {
        private readonly Dictionary<HoneyAttribute,(GUIStyle style, GUIContent content)> styles = new();
        public bool RequestHierarchyQuery => true;

        private void InitStyle(HHeaderAttribute atr,in HoneyDrawerInput inp)
        {
            if (styles.ContainsKey(atr)) return;

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            HoneyValueParser.ParseStyleExpression(atr.StyleExpression, inp.Container.GetType(), ref style);
            styles[atr] = (style, new GUIContent(atr.Header,atr.Tooltip));
        }
        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            HHeaderAttribute atr = attribute.As<HHeaderAttribute>();
            InitStyle(atr,inp);

            (GUIStyle style, GUIContent content) = styles[attribute];
            if (type == AdditionalDrawerCallType.PreBefore)
                return style!.CalcHeight(content,1f);
            else return 0;
        }

        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            HHeaderAttribute atr = attribute.As<HHeaderAttribute>();
            InitStyle(atr,inp);

            rect = EditorGUI.IndentedRect(rect);
            (GUIStyle style, GUIContent content) = styles[attribute];

            GUI.Label(rect, content, style);
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = true)]
    public class HHeaderAttribute : HoneyAttribute
    {
        public string Header { get; }
        public string Tooltip { get; }
        
        public string? StyleExpression { get; }
        public HHeaderAttribute(string header, string tooltip="",string? style=null)
        {
            Header = header;
            Tooltip = tooltip;
            StyleExpression = style;
        }
    }
#if UNITY_EDITOR
#endif

}