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
        private Dictionary<FieldInfo , Func<object,string>> dict = new();
        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            if (type == AdditionalDrawerCallType.PreBefore)
                return EditorGUIUtility.singleLineHeight * 2;
            else return 0;
        }

            
        public void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HHelpBoxAttribute)!;
            Type type = inp.Container.GetType();


            string name = inp.Field.Name;
            Func<Func<object,string>> toInsert = ()=> HoneyTextParser.Parse(atr.TextExpression, type,name);
            EditorGUI.HelpBox(rect, dict.GetOrInsertFunc(inp.Field ,toInsert).Invoke(inp.Container),MessageType.Info);
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

        public HHelpBoxAttribute(string textExpression)
        {
            TextExpression = textExpression;
        }
    }


}