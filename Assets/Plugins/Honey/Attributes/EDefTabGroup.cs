#nullable enable
using System;
using System.Linq;
using Honey;
using Honey.Helper;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Honey.Core;
using UnityEditor;

namespace Honey.Editor
{   
    public class EDefTabHoneyGroupDrawer : IHoneyGroupDrawer
    {
        private readonly WeakAttributeCache<int> tabGroupIndexDict=new();
        private readonly WeakAttributeCache<Dictionary<object, int>> tabElementDict = new();
        public void DrawLayout(string groupPath, string name, HoneyEditorHandler editor, Attribute attribute,
            Group @group, string innerPath)
        {
            EDefTabGroupAttribute atr = (EDefTabGroupAttribute) attribute;
            string key = $"{innerPath}//{groupPath}";
            SerializedObject? serialized = editor.HoneySerializedObject.GetUpperSerializedObject();
            Dictionary<object, int> specificTabElementDict =
                tabElementDict.GetOrElseInsert(serialized.targetObject, key,atr, () => new());

            EditorGUILayout.BeginVertical("GroupBox");

            int indx=tabGroupIndexDict.GetOrElseInsert(editor.HoneySerializedObject.GetUpperSerializedObject(), key,atr,
                () => 0);

            int nwIndex=GUILayout.Toolbar( indx, atr.Groups);
            if (nwIndex != indx)
                GUI.FocusControl(null);

            tabGroupIndexDict.Set(editor.HoneySerializedObject.GetUpperSerializedObject().targetObject, key,atr, nwIndex);

            foreach (IGroupElement element in @group.Elements)
            {
                if (element is HoneyEditorHandler.ContentGroupElement data)
                {

                    if (!specificTabElementDict.ContainsKey(data.Data.Member))
                    {
                        ETabElementAttribute tabElement =
                            data.Data.Member.GetCustomAttribute<ETabElementAttribute>();

                        if (tabElement != null)
                            specificTabElementDict[data.Data.Member] =
                                Array.FindIndex(atr.Groups, item => item == tabElement.TabName);
                        else specificTabElementDict[data.Data.Member] = 0;
                    }
                    if(nwIndex==specificTabElementDict[data.Data.Member])
                        editor.ElementGUI(data.Data);
                }
                else if (element is Group gr)
                {
                    if (!specificTabElementDict.ContainsKey(gr.Path))
                    {
                        EAssignGroupToTab tabElement =
                            editor.HoneySerializedObject
                                .GetObjType()
                                .GetCustomAttributes<EAssignGroupToTab>()
                                .First(item => item.Group == gr.Path);
                            
                        if (tabElement != null)
                            specificTabElementDict[gr.Path] =
                                Array.FindIndex(atr.Groups, item => item == tabElement.Tab);
                        else specificTabElementDict[gr.Path] = 0;
                    }
                    if(nwIndex==specificTabElementDict[gr.Path])
                        editor.DrawGroup(gr);
                }
            }
            EditorGUILayout.EndVertical();
                
        }

        public void DisposeEditor(HoneyEditorHandler editor)
        {
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EDefTabGroupAttribute : GroupDefinitionAttribute
    {
        public string[] Groups { get; }
        public EDefTabGroupAttribute(string path,params string[] groups) : base(path)
        {
            Groups = groups;
        }
    }
#if UNITY_EDITOR
#endif

}