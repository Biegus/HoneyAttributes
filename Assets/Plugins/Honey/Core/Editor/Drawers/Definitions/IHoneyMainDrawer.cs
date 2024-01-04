#if UNITY_EDITOR
#nullable  enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Honey;

using UnityEditor;
using UnityEngine;



namespace Honey.Editor
{
    public readonly struct HoneyDrawerInput
    {
        [Flags]
        public enum Flags
        {
            None,
            AllowLayout,
            Changed,
        }
        public  FieldInfo Field { get; init; }
        public  object? Obj { get; init; }
        public  object Container { get; init; }
        public  SerializedProperty SerializedProperty { get; init; }
        public  Stack<object> TempMemory { get; init; }
        public  IHoneyErrorListener Listener { get; init; }
        public  Flags Flag { get; init; }
        public int Key { get; init; }
        public bool AllowLayout => Flag.HasFlag(Flags.AllowLayout);
        public bool Changed => Flag.HasFlag(Flags.Changed);
    }

   
    public enum HoneyRecursiveMode
    {
        Disabled,
        OnTop,  
        Enable,
        Deep,
        Deepest,
    }
    public interface IHoneyMainDrawer
    {
        bool RequestHierarchyQuery { get; }
         HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Disabled;
        void Draw(in HoneyDrawerInput inp, Rect rect,HoneyAttribute attribute,GUIContent title,Action<GUIContent,Rect> body);

        float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title);
        float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title);
      
    }

  


    
    public interface IHoneyAdditionalDrawer
    {
        /// <summary>
        /// If false on all attributes, "Obj" and "Container" in HoneyDrawerInput will be null.
        /// Retrieving them causes small performance overhead
        /// </summary>
        bool RequestHierarchyQuery { get; }
        HoneyPropertyState GetDesiredState(in HoneyDrawerInput inp, HoneyAttribute attribute)
        {
            return HoneyPropertyState.Normal;
        }
        float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {
            return 0;
        }

        float GetHeightToErase(in HoneyDrawerInput inp, HoneyAttribute attribute)
        {
            return 0;
        }

        void PreBefore(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
        }
        void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            
        }
        void After(in HoneyDrawerInput inp,Rect rect,HoneyAttribute attribute){}
        GUIContent? GetCustomContent(in HoneyDrawerInput inp,HoneyAttribute attribute, GUIContent content)
        {
            return null;
        }

       

        

    }
  
}
#endif