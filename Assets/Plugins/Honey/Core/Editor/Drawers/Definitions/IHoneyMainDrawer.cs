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
    public struct HoneyDrawerInput
    {
        public FieldInfo Field;
        public object? Obj;
        public object Container;
        public SerializedProperty SerializedProperty;
        public IHoneyTempMemory TempMemory;
        public IHoneyErrorListener Listener;
        public bool AllowLayout;
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