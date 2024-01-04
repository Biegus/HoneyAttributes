#nullable enable
using System;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class SpritePreviewDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => false;


        private readonly WeakAttributeCache<bool> onOffCache = new();

        private bool GetFoldout(in HoneyDrawerInput inp,bool defaultValue, HoneyAttribute attribute)
        {
                
            return onOffCache.GetOrElseInsert(inp.SerializedProperty,attribute,()=> defaultValue);
        }
        public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
        {

            var atr = (attribute as HSpritePreviewAttribute)!;
            if (type == AdditionalDrawerCallType.After)
            {
                   
                UnityEngine.Object rf = inp.SerializedProperty.objectReferenceValue;
                if (rf is Sprite sprite && sprite.texture != null&&
                    (!atr.Expandable || GetFoldout(inp,atr.DefaultExpandable,atr) ))
                    return EditorGUIUtility.singleLineHeight * atr.Lines;
            }
                  
            return 0;
        }
        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {

            var atr = (attribute as HSpritePreviewAttribute)!;
            if (atr.Expandable)
            {
                int? index =
                    SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(inp.SerializedProperty.propertyPath);
                Rect movedBack = rect;
                movedBack.y -= EditorGUIUtility.singleLineHeight;
                movedBack.height = EditorGUIUtility.singleLineHeight;

                bool result= EditorGUI.Foldout(movedBack, GetFoldout(inp,atr.DefaultExpandable,attribute),"",true);
                onOffCache.Set(inp.SerializedProperty, atr, result);
            }
              
                
            UnityEngine.Object rf = inp.SerializedProperty.objectReferenceValue;
                
            if (rf is Sprite sprite)
            {
                rect.width = rect.height / sprite.texture.height * sprite.texture.width;
                GUI.DrawTexture(rect, sprite.texture);
            }
                  
        }
    }
}
#endif

namespace Honey
{
    /// <summary>
    /// Attributes for Sprite. 
    /// Renders a little preview below it. 
    /// </summary>
    public class HSpritePreviewAttribute : HoneyAttribute
    {
        public int Lines { get; }
        public bool Expandable { get; }
        public bool DefaultExpandable { get; set; }

        public HSpritePreviewAttribute(int lines=4, bool expandable=false)
        {
            Lines = lines;
            Expandable = expandable;
        }
    }
#if UNITY_EDITOR
#endif

}