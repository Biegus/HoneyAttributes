#nullable enable
using System;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class EDefHorizontalGroupDrawer : IHoneyGroupDrawer
    {
        public void DrawLayout(string groupPath, string name, HoneyEditorHandler editor, Attribute attribute, Group group)
        {
            var atr = attribute.As<EDefHorizontalGroupAttribute>();


            if (atr.ShowLabel == HorizontalLabelMode.Above && !string.IsNullOrEmpty(groupPath))
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            HoneyHorizontalLayoutHelper.BeginHorizontal(atr.Appearance.ToStyleName());
            if(atr.ShowLabel == HorizontalLabelMode.AsElement &&!string.IsNullOrEmpty(groupPath))
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            if (atr.EnterIndentSpace)
                GUILayout.Label(string.Empty, GUILayout.MinWidth(0),
                    GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / 2.6f));

            editor.DrawGroupContent(@group);
            HoneyHorizontalLayoutHelper.EndHorizontal();
        }

        public void OnEditorDisable(HoneyEditorHandler editor)
        {
               
        }
    }
}
#endif

namespace Honey
{
    public enum HorizontalLabelMode
    {
        Hide,
        Above,
        AsElement
        
    }
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Struct, AllowMultiple = true)]
    public class EDefHorizontalGroupAttribute : GroupDefinitionAttribute
    {
      

        public BaseGroupAppearance Appearance { get; }
        public bool EnterIndentSpace { get; }
        public HorizontalLabelMode ShowLabel { get; set; }
        public EDefHorizontalGroupAttribute(string path,BaseGroupAppearance appearance=BaseGroupAppearance.Empty, bool enterIndentSpace=false)
        :base(path)
        {
            Appearance = appearance;
            EnterIndentSpace = enterIndentSpace;
        }
    }
#if UNITY_EDITOR
#endif

}