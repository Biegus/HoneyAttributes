#nullable enable
using System;
using System.Linq;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Honey.Editor;
using Honey.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceHelperAttribute))]
    public class SerializeReferenceHelperDrawer : PropertyDrawer
    {
        private SearchProvider? provider;


        //not perfect, this dictionary will make "objects" sometimes live a little longer
        //this shouldn't be real problemy, property drawers are discarded relatively often
        private Dictionary<object,(UnityEngine.Object obj, string path)> refs = new();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var atr = (attribute as SerializeReferenceHelperAttribute)!;
            (_, Rect fieldRect) = HoneyEG.CutIntoLabelAndField(position);
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            int? index = SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(property.propertyPath);

            var refKey = (property.serializedObject.targetObject, property.propertyPath);

            if (property.managedReferenceValue != null  )
            {

                if (refs.ContainsKey(property.managedReferenceValue))
                {
                    if (refs[property.managedReferenceValue] != refKey)
                    {
                        if (property.managedReferenceValue is ICloneable cl)
                        {
                            property.managedReferenceValue = cl.Clone();
                        }
                        else
                        {
                            property.managedReferenceValue = Activator.CreateInstance(property.managedReferenceValue.GetType());
                        }
                    }
                }
                else
                {
                    refs.Add(property.managedReferenceValue,refKey);
                }
            }
                
                
            if (EditorGUI.DropdownButton(fieldRect, new GUIContent(property.managedReferenceValue?.GetType().Name??"null"), FocusType.Passive))
            {

                provider ??= ScriptableObject.CreateInstance<SearchProvider>();
                  
                Type type = HoneyReflectionUtility.FlattenCollectionType(fieldInfo.FieldType);
                int i = 0;
                var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(item => item.DefinedTypes)
                    .Where(
                        item => (item.FullName??"").Contains(atr.NamespaceBase)
                                && !item.IsAbstract 
                                && !item.IsInterface 
                                && !item.ContainsGenericParameters
                                && !typeof(UnityEngine.Object).IsAssignableFrom(item)
                                && (atr.AllowAnonymous|| !item.Name.StartsWith("<>"))
                                                                           
                                && (item.IsValueType ||item.DeclaredConstructors.Any(constr=>constr.GetParameters().Length==0))
                                && type.IsAssignableFrom(item) 
                                                                             
                                && (++i) <= 5000);
                types = types.ToArray();

                if (i == 5001)
                {
                    Debug.LogWarning($"There are above  500 types matching. That's too many. Try specifying namespace");
                    return;
                }
                provider.Types = types;
                provider.ShowNamespaces = atr.ShowNamespacesInSearch;
                provider.Action = (t) => HoneyEG.ApplyValueOnProperty(property.serializedObject.targetObject,
                    fieldInfo, property.propertyPath, index, (t!=null)?Activator.CreateInstance(t):null);
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint( Event.current.mousePosition),position.width), provider);
                    
            }
            EditorGUI.PropertyField(position, property, label,true);

        }

        //inhriting from scriptable object is required for some reason
        public class SearchProvider : ScriptableObject, ISearchWindowProvider
        {
            public IEnumerable<Type> Types { get; set; } = Enumerable.Empty<Type>();
            public Action<Type>? Action { get; set; }
            public bool ShowNamespaces { get; set; }
            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                List<SearchTreeEntry> list = new();
                list.Add(new SearchTreeGroupEntry(new GUIContent("Types")));
                SearchTreeEntry nullEntry = new SearchTreeEntry(new GUIContent("null"));
                nullEntry.level = 1;
                list.Add(nullEntry);
                nullEntry.userData = null;
                foreach (var type in Types)
                {
                    string text=(ShowNamespaces )?$"{type.Name} from ({type.Namespace})": type.Name;
                    var entry = new SearchTreeEntry(new GUIContent(text))
                    {
                        level = 1,
                        userData = type,
                    };
                    list.Add(entry );
                }
                return list;
            }

            public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
            {
                Action?.Invoke((searchTreeEntry.userData as Type)!);
                return true;
            }
        }
    }
}
#endif

namespace Honey
{
    //todo: not documented  in readme.md

    /// <summary>
    /// Draws nice dropdown to choose the type for serialize refernce field.
    /// Can only be used with [SerializeReference].
    ///
    /// NOTE: Serialize are reference in unity are generally messy (also slow).
    /// Some problems and solutions are listed below.
    ///
    /// (1) Its extremely recommened for types that will be in this field to implemenet ICloneable.
    /// This will be specially noticeable for List of serialize references
    /// Without ICloneable, moving, or copying element may erase values.
    /// Serialize references cannot be easily copied as normal serialize elements, and default behaviours is just to copy refernce
    /// which is practically never desired option.
    ///
    /// (2) In general avoid too crazy combinations.
    /// List of serialize references such that most of them contain their own serialize reference shuld be the limit.
    /// (in that case definitely implemenet ICloneable for your types)
    ///
    /// (3) With older version of unity it used to be case that removing/renaming classes that were used
    /// sometimes totally corrupted the object.
    /// This is not the case anymore.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]

    public class SerializeReferenceHelperAttribute : PropertyAttribute
    {
            public string NamespaceBase { get; }
            public bool AllowAnonymous { get; set; }
            public bool ShowNamespacesInSearch { get; set; }

            public SerializeReferenceHelperAttribute(string namespaceBase="")
            {
                NamespaceBase = namespaceBase;
              
            }
    }


}