#if UNITY_EDITOR
using System;
using Honey.Core;
using UnityEditor;

namespace  Honey.Editor
{
    public class HoneySerializedObjectDummy : IHoneySerializedObject
    {
        
        public SerializedObject SerializedObject { get; }
        public HoneySerializedObjectDummy(SerializedObject serializedObject)
        {
            SerializedObject = serializedObject;
        }
       
        public Type GetObjType()
        {
            return SerializedObject.targetObject.GetType();
        }

        public object GetInstance()
        {
            return SerializedObject.targetObject;
        }

        public HierarchyQuery GetHierarchyQuery(string path)
        {
            return HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(path,
                SerializedObject.targetObject.GetType());
        }

        public SerializedProperty GetIterator()
        {
            var it = SerializedObject.GetIterator();
            return it;
        }

        public void Update()
        {
            SerializedObject.Update();
        }

        public void ApplyModifiedProperties()
        {
            SerializedObject.ApplyModifiedProperties();
        }

        public object GetLowestTarget()
        {
            return SerializedObject.targetObject;
        }
    }
}
#endif