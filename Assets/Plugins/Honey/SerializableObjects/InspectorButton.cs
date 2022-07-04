#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace Honey.Objects
{
    [Serializable]
    public class NoneType{}
   
    [Serializable]
    public class InspectorButton<T>
    {
        #if UNITY_EDITOR // no need to hold data in runtime
        
        
        [SerializeField] private T? input = default!;
        public void Reset()
        {
            input = default;
        }
        #endif
    }
}