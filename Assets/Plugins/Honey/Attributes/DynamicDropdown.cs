#nullable enable
using System;
using System.Collections;
using System.Linq;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using Honey.Core;
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(DynamicDropdownAttribute))]
    public class DynamicDropdownDrawer : PropertyDrawer
    {
        private HoneyValueExpressionGetterCache<IEnumerable> cache = new HoneyValueExpressionGetterCache<IEnumerable>();

        private static WeakAttributeCache<(GUIContent content, object? value)> namesCache = new();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var atr = (attribute as DynamicDropdownAttribute)!;
            (var obj,var container) = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property).RetrieveTwoLast(property.serializedObject.targetObject);

            (Rect labelRect, Rect dropdown) = HoneyEG.CutIntoLabelAndField(position);
                
            EditorGUI.LabelField(labelRect,label);
                
            bool splited = false;
            GUIContent[]? names=null;
            var maybeValues = cache.GetAndInvoke(container, atr.Reference, fieldInfo, property);
            if (maybeValues.TryError(out var error))
            {
                HoneyErrorListenerStack.GetListener().LogLocalWarning(error,null,atr);
                return;
            }

            IEnumerable values = maybeValues.Unwrap();
            GUIContent content;

            if (values is IEnumerable<HoneyNamedEl>)
            {
                if (namesCache.TryGetValue(property,atr, out var res)&& Equals(res,obj) )
                {
                    content = res.content;
                }
                else
                {
                    splited = true;
                    (names, values) = HoneyNamedElHelper.SplitObjectArray(values);
                    values = values.Cast<object>().ToArray();
                    var tuple= values.Cast<object>().Select<object,(object,int)?>((item, i) => (item, i)).Where((tuple => tuple!.Value.Item1.Equals(obj))).FirstOrDefault();
                    if (tuple == null)
                    {
                        content = new GUIContent($"? ({(obj?.ToString() ?? "<null>")})");
                    }
                    else
                        content = new GUIContent($"{names[tuple.Value.Item2]}");

                    namesCache.Set(property,atr, (content, obj));
                }
            }
            else
                content = new GUIContent(obj?.ToString() ?? "<null>");


            if (EditorGUI.DropdownButton(dropdown,content, FocusType.Passive))
            {
                if(!splited)
                    (names, values) = HoneyNamedElHelper.SplitObjectArray(values);
                HoneyEG.BuildGenericMenu(property.propertyPath, (UnityEngine.Object) container, values ,fieldInfo,names)
                    .DropDown(dropdown);
            }
                
            EditorGUI.EndProperty();
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicDropdownAttribute : PropertyAttribute
    {
        public string Reference { get; }

        public DynamicDropdownAttribute(string reference)
        {
            Reference = reference;
        }
    }
#if UNITY_EDITOR
#endif

}