#nullable enable
using System.Collections.Generic;
using System.Linq;
using Honey;
using Honey.Core;
using UnityEngine;
using System;

#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;

namespace Honey.Editor
{
    public class HFormatPreviewDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private HoneyValueExpressionGetterCache<string> cache = new();
        private HoneyValueExpressionGetterCache<string[]> arrayCache = new();
        private static Regex countRegex = new Regex(@"\{(?<val>\d+)\}");
        private static Regex errorRegex = new Regex(@"(\{\d*([^\d\}]|$))|({([^\d]|$))|(([^\d\{]|^)\d*\})");

        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            if (type == AdditionalDrawerCallType.After)
                return EditorGUIUtility.singleLineHeight;
            else return 0;
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HFormatPreviewAttribute)!;
            if (inp.SerializedProperty.propertyType != SerializedPropertyType.String)
            {
                HoneyEG.DrawLocalAttributeError(rect,inp.Field,inp.SerializedProperty.displayName,atr,"Type should be string");
                return;
            }
            string s = inp.SerializedProperty.stringValue;
            var errors = errorRegex.Match(s);
            string final;
            if (errors.Success)
                final = s;
            else
            {
                var matches = countRegex.Matches(s);
                
               
                if (errors.Success)
                {
                    
                }
                int len = 0;
                if (matches.Count > 0)
                {
                    len = matches.Select(item =>
                    {
                        if (int.TryParse(item.Groups["val"].Value, out var res))
                        {
                            return res;
                        }

                        return -1;
                    }).Max()+1;
                }
                

                string[] values = new string[len];
                if (atr.ArrayExpression != null)
                {

                    int i = 0;
                    foreach (var el in arrayCache.Get(atr.ArrayExpression, inp.Field,inp.SerializedProperty)(inp.Container))
                    {
                        if(i>=values.Length)
                            break;
                        values[i++] = el;
                    }
                    
                }
                else
                {
                    for(int i=0;i<Math.Min( len, atr.References.Count);i++)
                    {
                        var value = cache.Get(atr.References[i], inp.Field,inp.SerializedProperty)(inp.Container);
                        values[i] = value;
                    }
                }

                final = string.Format(s, values);
            }
               
            Rect labelRect = rect;
            labelRect.width = EditorGUIUtility.labelWidth;
            rect.x += labelRect.width;
            bool old = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.LabelField(labelRect,"->Preview",EditorStyles.boldLabel);
            EditorGUI.TextArea(rect,final);
            GUI.enabled = old;

        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HFormatPreviewAttribute : HoneyAttribute
    {
        private readonly  string[] references;
        public IReadOnlyList<string> References => references;
        public string? ArrayExpression { get; set; }
        public HFormatPreviewAttribute(params string[] references)
        {
            
            this.references = references;
        }
        
    }


}