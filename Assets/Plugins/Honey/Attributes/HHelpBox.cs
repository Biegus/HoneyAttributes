#nullable enable
using Honey;
using Honey.Helper;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Honey.Core;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class HHelpBoxDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private readonly WeakAttributeCache<Func<object, string>> funcDict=new();
        private readonly Dictionary<HHelpBoxAttribute, GUIStyle> styleDict=new();
        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            HHelpBoxAttribute atr = attribute.As<HHelpBoxAttribute>();

            if (type != AdditionalDrawerCallType.PreBefore) return 0;
            if (!funcDict.TryGetValue(inp.SerializedProperty, attribute, out var result))
                return 0;

            //if atr.Style!=null and functDict has value at this key, styleDict should too if logic works correctly
            GUIStyle style = atr.Style == null ? EditorStyles.helpBox : styleDict[atr];

            return style.CalcHeight(HoneyEG.TempContent(result(inp.Container)), 1f);
        }

            
        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HHelpBoxAttribute)!;
            Type type = inp.Container.GetType();
            string name = inp.Field.Name;

            Func<Func<object,string>> toInsert = ()=> HoneyTextParser.Parse(atr.TextExpression, type,name);
            string text = funcDict.GetOrElseInsert(inp.SerializedProperty, atr, toInsert).Invoke(inp.Container)?? string.Empty;

            if (atr.Style == null)
                EditorGUI.HelpBox(rect,text ,MessageType.Info);
            else
            {
                GUIStyle style = styleDict.GetOrElseInsert(atr,
                    () =>
                    {
                        GUIStyle style = EditorStyles.helpBox;
                        HoneyValueParser.ParseStyleExpression(atr.Style,type,ref style);
                        return style;
                    });

                GUI.Label(rect,text,style);
            }

        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HHelpBoxAttribute : HoneyAttribute
    {
        public string TextExpression { get; }
        public string? Style { get; set; }

        public HHelpBoxAttribute(string textExpression)
        {
            TextExpression = textExpression;
        }
    }


}