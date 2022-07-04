#if UNITY_EDITOR
using System;
using Honey.Core;
using Honey.Editor;
using UnityEditor;

namespace  Honey.Editor
{
    public interface IHoneySerializedObject
    {
        Type GetObjType();
        object GetInstance();
        HierarchyQuery GetHierarchyQuery(string path);
        SerializedProperty GetIterator();
        void Update();
        void ApplyModifiedProperties();
        public object GetLowestTarget();
    }
}
#endif