#nullable enable
using System;
using System.Linq;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace Honey.Editor
{
    public class EHConstSizeDrawer : IHoneyMainDrawer
    {
        private readonly Dictionary<(FieldInfo field, int? index, object inst), ReorderableList> dict = new();
        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Deepest;
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title, Action<GUIContent, Rect> body)
        {

             
            EHConstSizeAttributeAttribute atr = (attribute as EHConstSizeAttributeAttribute)!;
            if (!inp.AllowLayout)
            {
                inp.Listener.LogLocalWarning("will only work with ehoneyrun",inp.Field,attribute);
                return;
            }

            inp.SerializedProperty.arraySize = atr.Size;
            var prop = inp.SerializedProperty;

              
            var key = (inp.Field, SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(prop.propertyPath),prop.serializedObject.targetObject);
            var l = dict.GetOrInsertFunc(key
                , () =>
                {
                    var list = new ReorderableList(prop.serializedObject, prop)
                    {
                        displayAdd = false,
                        displayRemove = false
                    };
                    GUIContent content = title;
                    list.drawHeaderCallback = (Rect r) =>
                    {
                        EditorGUI.LabelField(r,content);
                    };
                    list.elementHeight = EditorGUIUtility.singleLineHeight;
                  
                    list.drawElementCallback = (Rect innerRect,
                        int index,
                        bool isActive,
                        bool isFocused) =>
                    {
                        HoneyEG.PropertyField(innerRect, prop.GetArrayElementAtIndex(index));
                    };
                       
                    return list;
                        
                });
            try
            {
                   
                l.DoList(rect);
            }
            catch (ArgumentNullException exc) // pretty bad solution, but it doesn't seem to be way to check if  reord list is still "valid"
            {
                if (exc.ParamName != "_unity_self")
                    throw;
                dict.Remove(key);
            }
                

              
             
              
        }

        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return 0;
        }

        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return null;
        }

        public bool RequestHierarchyQuery => false;
           
    }
}
#endif

namespace Honey
{
   
    public class EHConstSizeAttributeAttribute : HoneyAttribute
    {
        public int Size { get; }

        public EHConstSizeAttributeAttribute(int size)
        {
            Size = size;
        }
    }
#if UNITY_EDITOR
#endif

}