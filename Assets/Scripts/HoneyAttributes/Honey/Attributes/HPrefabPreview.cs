using Honey.Objects;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Editor;
using UnityEditor;
#endif

namespace Honey
{
    public class HPrefabPreviewAttribute : HoneyAttribute
    {

    }
#if UNITY_EDITOR
    namespace Editor
    {
        public class HPrefabPreviewDrawer : IHoneyAdditionalDrawer
        {
            private Dictionary<(FieldInfo, object), (Object rf, UnityEditor.Editor)> editorCache = new();
            public bool RequestHierarchyQuery => false;
            public float GetHeight(in HoneyDrawerInput inp, AdditionalDrawerCallType type, HoneyAttribute attribute)
            {
                if (type == AdditionalDrawerCallType.After && inp.SerializedProperty.objectReferenceValue!=null)
                    return EditorGUIUtility.singleLineHeight * 3;
                else return 0;
            }

            public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
            {
                if (inp.SerializedProperty.objectReferenceValue == null)
                    return;
                UnityEngine.Object rf = inp.SerializedProperty.objectReferenceValue;
                (Object oldRef, UnityEditor.Editor editor) tuple= editorCache.GetOrInsertFunc((inp.Field, inp.Container), () => ( rf,UnityEditor.Editor.CreateEditor(rf)));
                if (tuple.oldRef != inp.SerializedProperty.objectReferenceValue)
                {
                    tuple= editorCache[(inp.Field, inp.Container)] = ( rf, UnityEditor.Editor.CreateEditor(rf));
                }
                string path = AssetDatabase.GetAssetPath(rf);
                Texture2D texture= tuple.editor.RenderStaticPreview(path, null, (int)rect.width, (int)rect.height);
                if (texture == null)
                {
                    texture=Texture2D.grayTexture;
                    
                }
                EditorGUI.DrawPreviewTexture(rect,texture);
            }
        }
    }
#endif

}