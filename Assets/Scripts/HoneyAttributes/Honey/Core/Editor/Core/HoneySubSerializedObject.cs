﻿#if UNITY_EDITOR
using System;
using Honey.Core;
using Honey.Editor;
using UnityEditor;

namespace  Honey.Editor
{
    public class HoneySubSerializedObject : IHoneySerializedObject
    {
        public SerializedProperty Property { get; }

        public HoneySubSerializedObject(SerializedProperty property)
        {
            Property = property;
        }
        public Type GetObjType()
        {
            return GetHierarchyQuery(Property.propertyPath).FinalType;
        }

        public object GetInstance()
        {
            return GetHierarchyQuery(Property.propertyPath).RetrieveLast(Property.serializedObject.targetObject);
        }

        public HierarchyQuery GetHierarchyQuery(string path)
        {
            return HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(path, Property.serializedObject.targetObject.GetType());
        }

        public SerializedProperty GetIterator()
        {
            var it= Property.Copy();
            return it;
        }

        public void Update()
        {
            //
        }

        public void ApplyModifiedProperties()
        {
            //
        }

        public object GetLowestTarget()
        {
            return Property.serializedObject.targetObject;
        }
    }
}
#endif