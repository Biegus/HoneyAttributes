#if UNITY_EDITOR
#nullable enable
using UnityEngine;
using System.Reflection;
using Honey.Objects;
#if UNITY_EDITOR
using Honey.Editor;
using Honey.Validation;
using UnityEditor;

#endif

namespace Honey.Editor
{
    public static class EditorMethodButtonHelper
    {
        public static void Draw(Rect position,SerializedProperty prop ,SerializedProperty input, string argDesc, InspectorButtonLimits flags,string name, string methodName)
        {
            bool hasArgs = input.type != nameof(NoneType);
            if(hasArgs)
            {
                position.width /= 2;
                EditorGUI.PropertyField(new Rect(position){height=EditorGUIUtility.singleLineHeight}, input,
                    ((!string.IsNullOrEmpty( argDesc))? new GUIContent(argDesc): GUIContent.none));
                position.x += position.width;

            }

            bool bf = GUI.enabled;

            var isActive =(!(input.serializedObject.targetObject is  MonoBehaviour)) || (((MonoBehaviour)input.serializedObject.targetObject)).isActiveAndEnabled;
            if (flags.CheckIfFails(Application.isPlaying,isActive))
            {
                GUI.enabled = false;
            }
            
            if (GUI.Button(position,name))
            {

                object target = HoneyHandler.HoneyReflectionCache.GetHierarchyQuery(prop.propertyPath,
                    input.serializedObject
                        .targetObject.GetType()).RetrieveTwoLast(input.serializedObject.targetObject).container;
             
                MethodInfo? realMethod = target.GetType()
                    .GetMethod(methodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (realMethod == null)
                {
                    Debug.Log($"Method {methodName} does not exist");
                }
                else
                {
                    realMethod.Invoke(target, hasArgs ? new[] { input.GetValue() } : null);
                }
            }
            GUI.enabled = bf;
        }
    }
}
#endif