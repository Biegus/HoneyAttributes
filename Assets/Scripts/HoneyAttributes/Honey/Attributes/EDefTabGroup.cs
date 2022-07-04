#nullable enable
using System;
using System.Linq;
using Honey;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Honey.Editor
{   
    public class EDefTabHoneyGroupDrawer : IHoneyGroupDrawer
    {
        private Dictionary<string, int> tabGroupIndexDict = new();
        private Dictionary<object, int> tabElementDict = new();
        public void DrawLayout(string groupPath, string name,HoneyEditorHandler editor, Attribute attribute, Group group)
        {
            var atr = (attribute as EDefTabGroupAttribute)!;
            if (!tabGroupIndexDict.ContainsKey(groupPath))
                tabGroupIndexDict[groupPath] = 0;
            EditorGUILayout.BeginVertical("GroupBox");
            int old = tabGroupIndexDict[groupPath];
            tabGroupIndexDict[groupPath]=GUILayout.Toolbar( old, atr.Groups);
            if (old != tabGroupIndexDict[groupPath])
            {
                GUI.FocusControl(null);
            }
                
            foreach (IGroupElement element in @group.Elements)
            {
                if (element is HoneyEditorHandler.ContentGroupElement data)
                {

                    if (!tabElementDict.ContainsKey(data.Data.Member))
                    {
                        ETabElementAttribute tabElement =
                            data.Data.Member.GetCustomAttribute<ETabElementAttribute>();

                        if (tabElement != null)
                            tabElementDict[data.Data.Member] =
                                Array.FindIndex(atr.Groups, item => item == tabElement.TabName);
                        else tabElementDict[data.Data.Member] = 0;
                    }
                    if(tabGroupIndexDict[groupPath]!=tabElementDict[data.Data.Member])
                        continue;
                        
                       
                    editor.ElementGUI(data.Data);   
                }
                else if (element is Group gr)
                {
                    if (!tabElementDict.ContainsKey(gr.Path))
                    {
                        EAssignGroupToTab tabElement =
                            editor.HoneySerializedObject.GetObjType()
                                .GetCustomAttributes<EAssignGroupToTab>()
                                .Where(item => item.Group == gr.Path)
                                .First();
                            
                        if (tabElement != null)
                            tabElementDict[gr.Path] =
                                Array.FindIndex(atr.Groups, item => item == tabElement.Tab);
                        else tabElementDict[gr.Path] = 0;
                    }
                    if(tabGroupIndexDict[groupPath]!=tabElementDict[gr.Path])
                        continue;
                    editor.DrawGroup(gr);
                }
            }
            EditorGUILayout.EndVertical();
                
        }

        public void OnEditorDisable(HoneyEditorHandler editor)
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