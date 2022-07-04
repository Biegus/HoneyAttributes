#nullable enable
using System;
using System.Collections.Generic;
using Honey;
using Honey.Helper;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Honey.Editor
{
    #if UNITY_EDITOR
    public class EDefVerticalGroupDrawer : IHoneyGroupDrawer
    {
        private Dictionary<object, Dictionary<string, bool>> onOffDict =
            new();

        private Lazy<GUIStyle> boldFoldout = new Lazy<GUIStyle>(() => new GUIStyle("foldout")
        {
            fontStyle = FontStyle.Bold
        });

        public void DrawLayout(string groupPath, string name, HoneyEditorHandler editor, Attribute attribute,
            Group group)
        {
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

            if (atr.Foldout)
            {
                    
                string code = string.Empty;
                if (editor.HoneySerializedObject.GetInstance() is UnityEngine.Object obj1)
                    code = $"_honey_group_foldout_{obj1.GetInstanceID()}_{groupPath}";
                if (!onOffDict.ContainsKey(editor))
                    onOffDict[editor] = new Dictionary<string, bool>();
                if (!onOffDict[editor].ContainsKey(groupPath))
                {
                    if (code!=string.Empty && EditorPrefs.HasKey(code))
                    {
                        onOffDict[editor][groupPath] = EditorPrefs.GetBool(code);
                    }
                    else
                        onOffDict[editor][groupPath] = false;
                }

                onOffDict[editor][groupPath] = EditorGUILayout.Foldout(onOffDict[editor][groupPath], name,
                    style: boldFoldout.Value, toggleOnLabelClick: true);
                EditorGUI.indentLevel += atr.Indentation;
                if (onOffDict[editor][groupPath])
                    editor.DrawGroupContent(@group);
                if (code!=string.Empty)
                {
                    EditorPrefs.SetBool(code,onOffDict[editor][groupPath]);
                }
            }
            else
            {
                if (atr.ShowLabel && !string.IsNullOrEmpty(groupPath))
                    EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
                EditorGUI.indentLevel += atr.Indentation;

                editor.DrawGroupContent(@group);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = old;
                
        }

        public void OnEditorDisable(HoneyEditorHandler editor)
        {
            onOffDict.Remove(editor);
               
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

        public EDefVerticalGroupAttribute(string path, BaseGroupAppearance appearance, bool foldout = false,
            int indentation = 1,
            bool showLabel = true)
            : base(path)
        {
            Appearance = appearance;
            Foldout = foldout;
            Indentation = indentation;
            ShowLabel = showLabel;
        }
    }

}