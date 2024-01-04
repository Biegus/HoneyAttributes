#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Honey;
using Honey.Helper;
#if UNITY_EDITOR
using Honey.Core;
using UnityEditor;
#endif
using UnityEngine;

namespace Honey.Editor
{
    #if UNITY_EDITOR
    public class EDefVerticalGroupDrawer : IHoneyGroupDrawer
    {

        private readonly WeakAttributeCache<bool> onOff = new();

        private Lazy<GUIStyle> boldFoldout = new Lazy<GUIStyle>(() => new GUIStyle("foldout")
        {
            fontStyle = FontStyle.Bold
        });

        public void DrawLayout(string groupPath, string name, HoneyEditorHandler editor, Attribute attribute,
            Group group, string innerPath)
        {
            //EditorGUILayout.HelpBox((onOff.AsEnumearble.SelectMany(item=>item.Value.Select(el=>(item.Key,el.Key,el.Value))).ToStringFromCollection("\n")),MessageType.Info);
            var atr = attribute.As<EDefVerticalGroupAttribute>();
            int old = EditorGUI.indentLevel;
            switch (atr.Appearance)
            {
                case BaseGroupAppearance.Box:
                    EditorGUILayout.BeginVertical("box");
                    break;
                case BaseGroupAppearance.BoxGroup:
                    EditorGUILayout.BeginVertical("GroupBox");
                    break;
                case BaseGroupAppearance.Empty:
                    EditorGUILayout.BeginVertical();
                    break;
                default: break;
            }
            string realName = atr.CustomLabel ?? (atr.ShowLabel ? name : string.Empty);

            if (atr.Foldout)
            {
                string key = $"{innerPath}//{groupPath}";
                SerializedObject upper = editor.HoneySerializedObject.GetUpperSerializedObject();

                bool result=onOff.Update(upper.targetObject, key,attribute, () => false,
                    value => EditorGUILayout.Foldout(value, realName, style: boldFoldout.Value,
                        toggleOnLabelClick: true));

                if (result)
                {
                    EditorGUI.indentLevel += atr.Indentation;
                    editor.DrawGroupContent(group);
                }
            }
            else
            {
                if (realName != string.Empty)
                    EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
                EditorGUI.indentLevel += atr.Indentation;

                editor.DrawGroupContent(group);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = old;
                
        }

        public void DisposeEditor(HoneyEditorHandler editor)
        {

        }
    }
    #endif
}

namespace Honey
{
    public enum BaseGroupAppearance
    {
        Empty,
        BoxGroup,
        Box,
    }
    public static class BaseGroupAppearanceExtension
    {
        public static string ToStyleName(this BaseGroupAppearance self)
        {
            if (self == BaseGroupAppearance.Empty) return String.Empty;
            else return self.ToString();
        }
        
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EDefVerticalGroupAttribute : GroupDefinitionAttribute
    {
        public BaseGroupAppearance Appearance { get; }
        public int Indentation { get; }
        public bool Foldout { get; }
        public bool ShowLabel { get; }
        public string? CustomLabel { get; }

        public EDefVerticalGroupAttribute(string path, BaseGroupAppearance appearance, bool foldout = false,
            int indentation = 1,
            bool showLabel = true,string? customLabel=null)
            : base(path)
        {
            Appearance = appearance;
            Foldout = foldout;
            Indentation = indentation;
            ShowLabel = showLabel;
            CustomLabel = customLabel;
        }
    }

}