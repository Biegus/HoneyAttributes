#if UNITY_EDITOR
#nullable  enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Honey;
using Honey.Core;
using Honey.Helper;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    public static class HoneyHandler
    {
        public static readonly HoneyReflectionCache HoneyReflectionCache = new HoneyReflectionCache();
    }

    public class HoneyRuntimeCustomMemberAttributeData
    {
        public IHoneyCustomElementDrawer Drawer { get; }
        public Attribute Attribute { get; }
        public HoneyRuntimeCustomMemberAttributeData(IHoneyCustomElementDrawer drawer, Attribute attribute)
        {
            Drawer = drawer;
            Attribute = attribute;
        }
    }
    public class HoneyRuntimeCustomMember
    {
        private readonly HoneyRuntimeCustomMemberAttributeData[] elements;
        public IReadOnlyList<HoneyRuntimeCustomMemberAttributeData> Elements => elements;
        public MemberInfo Member { get; }
        public HoneyRuntimeCustomMember(HoneyRuntimeCustomMemberAttributeData[] elements,
            MemberInfo member)
        {
            this.elements = elements;
            this.Member = member;
        }

        public void OnGUI(object container)
        {
            
            foreach (var element in elements)
            {
                if (Member is MethodInfo method)
                    element.Drawer.DrawMethodLayout( method,element.Attribute,container);
                else if(Member is FieldInfo field)
                    element.Drawer.DrawNonSerializedFieldLayout(field,element.Attribute,container);
                else if(Member is PropertyInfo prop)
                    element.Drawer.DrawPropertyLayout(prop,element.Attribute,container);
            }
        }
    }
    /// <summary>
    /// all methods are cached
    /// </summary>
    public  class HoneyReflectionCache
    {
        private readonly Dictionary<Type, HoneyTypeDrawerDefinition> fieldsTypeReferences = new();
        private readonly Dictionary<Type, IHoneyGroupDrawer> groupsTypeReferences = new();
        private readonly Dictionary<Type, IHoneyCustomElementDrawer> customMemberTypesReferences = new();
        private readonly Dictionary<Type, Dictionary<string,HoneyRuntimeGroupDefinition>> groupsReferences = new();
        
        private readonly Dictionary<FieldInfo, HoneyRuntimeField> fieldReferences = new();
        private readonly Dictionary<(string path, Type baseType), HierarchyQuery> valueReferences = new();
        private readonly Dictionary<MemberInfo, HoneyRuntimeCustomMember?> customMembersReferences = new();
        private readonly Dictionary<Type, IReadOnlyDictionary<string,StyleOverride>> styles = new();


        
        public IReadOnlyDictionary<string,StyleOverride> GetStyleOverrides(Type type)
        {
            return styles.GetOrInsert(type, BuildStyleOverrides(type));
        }

        private IReadOnlyDictionary<string,StyleOverride> BuildStyleOverrides(Type type)
        {
            Dictionary<string, StyleOverride> dict = new();
            foreach (DefineStyleOverrideAttribute attribute in type.GetCustomAttributes<DefineStyleOverrideAttribute>())
            {
                dict[attribute.NameCode] = attribute.Build();
            }

            return dict;
        }

        public void RegisterCustomMember(Type type, IHoneyCustomElementDrawer drawer)
        {
            customMemberTypesReferences[type] = drawer;
        }
        public HoneyRuntimeCustomMember? GetCustomMember(MemberInfo member)
        {
            if (customMembersReferences.TryGetValue(member, out var res))
            {
                return res;
            }

            HoneyRuntimeCustomMember? custom = BuildCustomMember(member);
           
            return customMembersReferences[member] =custom;

        }

        private HoneyRuntimeCustomMember? BuildCustomMember(MemberInfo member)
        {
            var array = member.GetCustomAttributes<Attribute>()
                .Where(item => customMemberTypesReferences.ContainsKey(item.GetType()))
                .Select(attribute =>new HoneyRuntimeCustomMemberAttributeData( customMemberTypesReferences[attribute.GetType()],attribute)).ToArray();
            if (array.Length > 0)
                return new HoneyRuntimeCustomMember(array,member);
            else return null;

        }
        public IReadOnlyDictionary<string, HoneyRuntimeGroupDefinition> GetGroupData(Type type)
        {
          
            if (groupsReferences.TryGetValue(type, out var dict))
            {
                return dict;
            }

             groupsReferences[type] = BuildRuntimeGroupsDict(type);
             return groupsReferences[type];
        }

        private  Dictionary<string, HoneyRuntimeGroupDefinition> BuildRuntimeGroupsDict(Type type)
        {
            Dictionary<string, HoneyRuntimeGroupDefinition> dict = new Dictionary<string, HoneyRuntimeGroupDefinition>();
            foreach (var attribute in  type.GetCustomAttributes<GroupDefinitionAttribute>())
            {
                dict[attribute.Path] = new HoneyRuntimeGroupDefinition(attribute, groupsTypeReferences[attribute.GetType()]);
            }

            return dict;

        }
        
        public void RegisterFieldAttribute(Type type, HoneyTypeDrawerDefinition typeDrawerDefinition)
        {
            fieldsTypeReferences[type] = typeDrawerDefinition;
        }

        public void RegisterGroupAttribute(Type type, IHoneyGroupDrawer drawer)
        {
            groupsTypeReferences[type] = drawer;
        }
        
        public  HierarchyQuery GetHierarchyQuery(string path, Type baseType)
        {
            if (valueReferences.TryGetValue((path, baseType), out HierarchyQuery query))
            {
                return query;
            }
            
            return valueReferences[(path, baseType)] = HierarchyFactory.BuildHierarchyQuery(path, baseType);
            
        }

        public HierarchyQuery GetHierarchyQuery(SerializedProperty property)
        {
           
            return GetHierarchyQuery(property.propertyPath, property.serializedObject.targetObject.GetType());
            
           
            
        }
        public (object? final, object container) ReadObject(SerializedProperty property)
        {
            
            return   HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(property.propertyPath,
                    property.serializedObject.targetObject.GetType())
                .RetrieveTwoLast(property.serializedObject.targetObject);
        }
        
        public HoneyRuntimeField Get(FieldInfo field)
        {
            if (fieldReferences.TryGetValue(field, out HoneyRuntimeField prop))
            {
                return prop;
            }
            return fieldReferences[field] = Build(field);
        }
      
        private HoneyRuntimeField Build(FieldInfo field)
        {
            
            HoneyRuntimeFieldAttributeData[] array =
                (from item in field.GetCustomAttributes()
                where fieldsTypeReferences.ContainsKey(item.GetType())
                let instance = fieldsTypeReferences[item.GetType()]
                select new HoneyRuntimeFieldAttributeData(instance, item.As<HoneyAttribute>())).ToArray();

           
            return new HoneyRuntimeField(array);
        }
    }
}
#endif