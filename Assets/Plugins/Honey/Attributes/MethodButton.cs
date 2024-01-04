#nullable enable
using System;
using System.Reflection;
using Honey.Objects;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Honey.Editor
{
    public class MethodButtonDrawer : IHoneyCustomElementDrawer
    {
        public void DrawMethodLayout(MethodInfo info, Attribute attribute,object container)
        {
            var atr = (attribute as EMethodButtonAttribute)!;
            bool isEnabled = container is not MonoBehaviour component || component.isActiveAndEnabled;

            bool orgGui = GUI.enabled;
            if (atr.Flags.CheckIfFails(Application.isPlaying, isEnabled))
                GUI.enabled = false;

            if (GUILayout.Button(info.Name,GUILayout.Height(atr.ButtonSize*EditorGUIUtility.singleLineHeight)))
            {
                info.Invoke(container, null);
            }

            GUI.enabled = orgGui;

        }
    }
}
#endif

namespace Honey
{
    /// <summary>
    /// Makes parametereless method clickable.
    /// </summary>

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EMethodButtonAttribute : Attribute
    {
        public InspectorButtonLimits Flags { get; set; }
        public int ButtonSize { get; }

        public EMethodButtonAttribute(int buttonSize=1)
        {
            ButtonSize = buttonSize;
        }
    }

}