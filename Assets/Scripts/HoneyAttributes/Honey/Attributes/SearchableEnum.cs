#nullable enable
using System.Collections.Generic;
using System.Reflection;
using Honey;
using System;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
            
        private EnumSearchProvider? provider;
        private GUIStyle? dropdownStyle;
            
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SearchableEnumAttribute atr = (attribute as SearchableEnumAttribute)!;
            dropdownStyle ??= new GUIStyle("dropdown");
            provider ??= ScriptableObject.CreateInstance<EnumSearchProvider>();
            UnityEngine.Object baseTarget = property.serializedObject.targetObject;
            var query = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property.propertyPath,
                property.serializedObject.targetObject.GetType());
            object target =query
                .RetrieveTwoLast(property.serializedObject.targetObject).container;
            string path = property.propertyPath;
            Rect bt = position;
            position.width =EditorGUIUtility.labelWidth;
            bt.x += position.width;
            bt.width = bt.width - position.width;
            EditorGUI.LabelField( position,label);
            string enumName = property.enumNames[property.enumValueIndex];
            string content = atr.DontShowNumberValue ? enumName : $"{enumName} ({property.intValue})";

               
            if (EditorGUI.DropdownButton(bt, new GUIContent( content),FocusType.Passive,dropdownStyle))
            {
                if (atr.LimitFunction != null)
                {
                    var method = fieldInfo.DeclaringType!.GetMethod(atr.LimitFunction,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    if (method == null)
                    {
                        HoneyErrorListenerStack.GetListener().LogGlobalWarning($"Limit function {atr.LimitFunction} was not found",fieldInfo,atr);
                        return;
                    }
                    provider.Validate = (e) => (bool) method.Invoke(target, new object[] {e});
                }
                else provider.Validate = (_) => true;
                   
                provider.EnumType = fieldInfo.FieldType;
                provider.OnClick = en =>
                {
                    SerializedObject serialized = new SerializedObject(baseTarget);
                    SerializedProperty prop = serialized.FindProperty(path);
                    prop.intValue = (int) en;
                    serialized.ApplyModifiedProperties();
                    serialized.Dispose();
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider
                );
            }
        }
    }
      
    public class EnumSearchProvider: ScriptableObject, ISearchWindowProvider
    {
        public Type? EnumType{ get; set; }
        public Action<object>? OnClick { get; set; }
        public Func<Enum,bool>? Validate { get; set; }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {

            if (EnumType == null || OnClick == null || Validate == null)
                throw new InvalidOperationException("Properties were not set");
            List<SearchTreeEntry> list = new();
            var master=new SearchTreeGroupEntry(new GUIContent(EnumType.Name));
            list.Add(master);
            int i = -1;
            foreach (var enumName in Enum.GetNames(EnumType))
            {
                i++;
                int value = (int)Enum.GetValues(EnumType).GetValue(i);
                if (!Validate((Enum)Enum.GetValues(EnumType).GetValue(i)))
                {
                    continue;
                }
                    
                var entry = new SearchTreeEntry(new GUIContent( $"{enumName} ({value})"));
                entry.level = 1;
                entry.userData = value;
                list.Add(entry);
            }
                
            return list;
        }
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            OnClick?.Invoke( searchTreeEntry.userData);
            return true;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SearchableEnumAttribute : PropertyAttribute
    {
        public bool DontShowNumberValue { get; set; }
        public string? LimitFunction { get; set; }
    }


}