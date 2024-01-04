using System;
using UnityEngine;

#if UNITY_EDITOR
using Honey.Objects;
using  UnityEditor;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDummy))]
    public class SerializableDummyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }
    }

}
#endif
namespace Honey.Objects
{


    // TODO: not documented
    [Serializable]
    public struct SerializableDummy
    {
    }
}