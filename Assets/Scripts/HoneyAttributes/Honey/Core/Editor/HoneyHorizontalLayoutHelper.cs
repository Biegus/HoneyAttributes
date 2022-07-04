#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    public static class HoneyHorizontalLayoutHelper
    {
        private static int counter = 0;
        private static (float label, float field) toRestore;
        private static void InternalUp()
        {
            if (counter == 0)
            {

                toRestore = (EditorGUIUtility.labelWidth, EditorGUIUtility.fieldWidth);
            }

            EditorGUIUtility.labelWidth = 60;
            EditorGUIUtility.fieldWidth = 60;

            counter++;
        }

        private static void InternalDown()
        {
            if (counter == 1)
            {
                EditorGUIUtility.labelWidth = toRestore.label;
                EditorGUIUtility.fieldWidth = toRestore.field;
            }

            counter--;

        }

        public static int Counter => counter;
        public static void BeginHorizontal(string style)
        {
            if (style != string.Empty)
                EditorGUILayout.BeginHorizontal(style);
            else
            {
                EditorGUILayout.BeginHorizontal();
            }
            InternalUp();
            
        }

        public static void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
            InternalDown();

        }
    }
}
#endif