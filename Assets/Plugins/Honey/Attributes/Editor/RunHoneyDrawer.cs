#if UNITY_EDITOR
using Honey;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    [CustomPropertyDrawer(typeof(HoneyRun))]
    public class RunHoneyDrawer : PropertyDrawer
    {
        private readonly IHoneyTempMemory tempMemory = new HoneyTempTempMemory();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float res = 0;
            HoneyEG.DoSafe(() =>
            {
                res= HoneyHandler.HoneyReflectionCache.Get(fieldInfo).QueryHeight(property, fieldInfo ,label,
                    HoneyErrorListenerStack. GetListener(),false,1f,tempMemory);
            },property);
            return res;

        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            EditorGUI.BeginProperty(position, label, property);
            IHoneyErrorListener listener = HoneyErrorListenerStack.GetListener();
            
            HoneyEG.DoSafe(() =>
            {
                HoneyHandler.HoneyReflectionCache.Get(fieldInfo).OnGui(
                    property,
                    fieldInfo,
                    position,
                    tempMemory,
                    label,
                    listener, false);
            },property);
            
            EditorGUI.EndProperty();
            ;
        }
        
    }
}
#endif