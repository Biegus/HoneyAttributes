#nullable enable
using Honey;
using Honey.Helper;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using Honey.Editor;

namespace Honey.Editor
{
    public class ResetValueDrawer : IHoneyMainDrawer
    {
        public bool RequestHierarchyQuery => false;

        public HoneyRecursiveMode RecursiveMode => HoneyRecursiveMode.Enable;


        private bool IsIncorrectKind(SerializedPropertyType type)
        {
            return type is SerializedPropertyType.Generic or SerializedPropertyType.ObjectReference;
        }
        public void Draw(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute, GUIContent title
            ,Action<GUIContent,Rect> innerBody)
        {
            var atr = attribute.As<HResetValueAttribute>();

            if (IsIncorrectKind(inp.SerializedProperty.propertyType))
            {
                Rect errorRect = new Rect(rect);
                errorRect.height = EditorGUIUtility.singleLineHeight;

                const string ERR_TEXT="ResetValue supports only primitives";
                EditorGUI.HelpBox(errorRect,ERR_TEXT, MessageType.Error);
                inp.Listener.MaybeLogLocalWarning(ERR_TEXT, inp.Field, attribute);

                rect.y += errorRect.height;
                rect.height -= rect.y;

            }
            else if (HoneyEG.SideButton(ref rect, "↺"))
            {
                inp.SerializedProperty.SetValue(atr.Value);
            }

            innerBody(title, rect);

        }

        public float GetDesiredAdditionalHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            if (IsIncorrectKind(inp.SerializedProperty.propertyType))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return 0;
        }

        public float? GetDesiredMinBaseHeight(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent title)
        {
            return null;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HResetValueAttribute : HoneyAttribute
    {
            public object Value { get; }

            public HResetValueAttribute(object value)
            {
                Value = value;
            }
    }


}