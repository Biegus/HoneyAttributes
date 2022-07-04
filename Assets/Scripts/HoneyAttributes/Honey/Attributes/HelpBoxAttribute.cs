#nullable enable
using System;
using Honey;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position)
        {
            HelpBoxAttribute atr = (attribute as HelpBoxAttribute)!;
                
            EditorGUI.HelpBox(position,atr!.Message,MessageType.Info);
        }
    }
}
#endif
namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HelpBoxAttribute: PropertyAttribute
    {
            public string Message { get; }

            public HelpBoxAttribute(string message)
            {
                this.Message = message;
            }
    }
   
}