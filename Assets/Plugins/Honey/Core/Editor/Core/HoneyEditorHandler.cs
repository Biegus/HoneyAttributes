
#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Honey;
using Honey.Helper;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace  Honey.Editor
{
    
    public interface IGroupElement
    {
        Group? Father { get; }
    }

    [CanEditMultipleObjects]
    public class HoneyEditor : UnityEditor.Editor
    {
      

        private HoneyEditorHandler? master;
        private void OnEnable()
        {
            
            master = new HoneyEditorHandler(new HoneySerializedObjectDummy(serializedObject));
            master.OnEnable();
        }

      
        public override void OnInspectorGUI()
        {

            if (master == null)
            {
                Debug.LogError("master handler was not initialized");
                return;
            }
            master.OnInspectorGUI(base.OnInspectorGUI);


        }
    }
    public class HoneyEditorHandler 
    {

        
        public class ContentGroupElement: IGroupElement
        {
            public IEditorData Data { get; }
            public Group? Father { get; }

            public ContentGroupElement(IEditorData data, Group father)
            {
                Data = data;
                Father = father;
            }
        }

        public interface IEditorData
        {
            public FolderPath? Path { get; }
            public int Index { get;  }
            public MemberInfo Member { get; }
        }

        public class CustomMemberData : IEditorData
        {
            public MemberInfo Member => Drawer.Member;
            public FolderPath? Path { get;  }
            public int Index { get;  }
            public HoneyRuntimeCustomMember Drawer { get;  }

            public CustomMemberData(FolderPath? path, int index, HoneyRuntimeCustomMember drawer)
            {
                Path = path;
                Index = index;
                Drawer = drawer;
            }
        }
     
        public class PropertyData : IEditorData 
        {
            public HoneyRuntimeField? Drawer { get; set; }
            public HoneyEditorHandler? Handler { get; set; }
            public SubEditorMode? SubMode { get; set; }
            public FieldInfo Field { get; set; }
            public SerializedProperty SerializedProperty { get;  }
            public FolderPath? Path { get; set; }
            public int Index { get; set; }
             MemberInfo IEditorData.Member => Field;

             public PropertyData( FieldInfo field, SerializedProperty serializedProperty)
             {
                 
                 Field = field;
                 SerializedProperty = serializedProperty;
             }
        }
        


        public IHoneySerializedObject HoneySerializedObject { get; }
        private IEditorData[]? elements=null;
        private SerializedProperty? scriptProperty=null;
        private bool incorrectState = false;
        private readonly HoneyLogger errors = new HoneyLogger();
        private readonly HoneyLogger warnings = new HoneyLogger();
        private readonly HoneyLogger localWarnings = new HoneyLogger();
        private TimeSpan guiTime;
        private readonly IReadOnlyDictionary<string, HoneyRuntimeGroupDefinition> groupsDefinition;
        private readonly HoneyTempTempMemory tempMemory = new HoneyTempTempMemory();
        private readonly HoneyErrorListener listener;      


        private Group? masterGroup;
     

        public HoneyEditorHandler(IHoneySerializedObject obj)
        {
            HoneySerializedObject = obj;
            listener = new HoneyErrorListener(errors, localWarnings, warnings);
            groupsDefinition = HoneyHandler.HoneyReflectionCache.GetGroupData(this.HoneySerializedObject.GetObjType());
        }
      
        private void InternalEnable()
        {
            bool empty = false;

            SerializedProperty iter = this.HoneySerializedObject.GetIterator();
            string basePath = iter.propertyPath;
            if (!iter.NextVisible(true))
            {
                empty = true;
            }


            if (this.HoneySerializedObject is HoneySerializedObjectDummy)
            {
                scriptProperty = iter.Copy();

                if (!iter.NextVisible(true))
                {
                    empty = true;
                }
            }

            FolderPath? GetPath(MemberInfo member)
            {
                EGroupAttribute group = member.GetCustomAttribute<EGroupAttribute>();
                if (group != null)
                    return new FolderPath(group.Path);
                return null;
            }
            
            List<SerializedProperty> list = new List<SerializedProperty>();
            if(!empty)
                do
                {
                    if(!iter.propertyPath.Contains(basePath))
                        break;
                    list.Add(iter.Copy());
                } while (iter.NextVisible(false));
            int indx = 0;
            var props = list.Select<SerializedProperty, PropertyData?>((property, i) =>
                {
                    FieldInfo? field = HoneySerializedObject.GetObjType().GetField(property.name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null)
                    {
                        LogError($"Field {property.name} not found");
                        return null;
                    }
                    PropertyData data = new(field,property)
                    {
                        Index = i
                    };
                    indx = i;
                   
                    var subAtr = HoneySerializedObject.GetHierarchyQuery(property.propertyPath).FinalType
                        .GetCustomAttribute<AllowSubEditorAttribute>();
                    if (subAtr != null)
                    {
                        data.Handler = new HoneyEditorHandler(new HoneySubSerializedObject(data.SerializedProperty));
                        data.Handler.OnEnable();
                        data.SubMode = subAtr.Mode;
                    }
                   

                    data.Field = field;
                    if (field.GetCustomAttribute<EHoneyRun>() != null)
                    {
                        if (field.GetCustomAttribute<HoneyRun>() != null)
                        {
                            LogGlobalWarning(
                                $"You should not use HoneyRun attribute on pair with EHoneyRun. Found one pair at \"{field.FieldType} {field.Name}\"\n" +
                                $"[Use HoneyRun when you want to apply attributes to elements, use EHoneyRun when you want to apply them to the whole list]");

                            return data; //skip adding drawer
                        }

                        data.Drawer = HoneyHandler.HoneyReflectionCache.Get(field);
                    }
                    data.Path = GetPath(data.Field);
                    return data;

                })
                .Where(item => item != null)
                .Select(item => item!);

            indx++;
            IEnumerable<CustomMemberData> customs =
                (
                from member in  HoneySerializedObject.GetObjType()
                        .GetMembers(BindingFlags.NonPublic| BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                let runtime= HoneyHandler.HoneyReflectionCache.GetCustomMember(member)
                where runtime!=null
                select new CustomMemberData(GetPath(member),indx++,runtime))
                    .ToArray();
                       
                        
            elements= ((IEnumerable<IEditorData>) props).Union(customs)
                .OrderBy(item=>item.Path==null)
                .ThenBy(item =>
                {

                    return item.Path;
                })
                .ThenBy(item =>
                {
                    return item.Member.MetadataToken;
                })
                .ToArray();

            masterGroup = BuildMasterGroupFromSortedProperties();

        }
        private Group BuildMasterGroupFromSortedProperties()
        {

            if (elements == null)
            {
                throw new InvalidOperationException("elements were not set");
                
            }
            var masterModf = groupsDefinition.GetValueOrDefault(string.Empty);
            Group master = new Group(string.Empty,string.Empty,null, masterModf?.Drawer,masterModf?.Attribute);

            Group currentGroup = master;
            IReadOnlyList<string> pathElements = Array.Empty<string>();
            FolderPath dummy = new(string.Empty);
             foreach (IEditorData data in elements)
             {

                 FolderPath curPath = data.Path ?? dummy;
                 IReadOnlyList<string> curPathElements = curPath.Elements;
                    
                    int incorrectOne = EnumerableHelper.GetFirstIncorrectIndex(curPathElements, pathElements);
                    for (int i = pathElements.Count - 1; i >= incorrectOne; i--)
                    {
                        currentGroup = currentGroup.Father ?? throw new InvalidOperationException("No father in object that was expected to have one");
                    }
                    
                    int indx = incorrectOne;
                    string builder = curPathElements.Take(indx).Aggregate(new StringBuilder(),(a,b)=>a.Append($"{b}/" )).ToString();
                    if (builder.Length > 0)
                        builder = builder[0..(builder.Length - 1)];
                    foreach (var el in curPathElements.Skip(indx))
                    {
                        if (builder != string.Empty)
                            builder += $"/{el}";
                        else
                            builder = $"{el}";
                        
                        if (el == string.Empty) continue;


                        HoneyRuntimeGroupDefinition? def;
                        if (groupsDefinition.TryGetValue(builder, out var result))
                        {
                            def = result;
                        }
                        else
                        {
                            def = null;
                        }
                        var gr= new Group(builder, el,father:currentGroup,def?.Drawer,def?.Attribute);
                      
                        currentGroup.Elements.Add(gr);
                        currentGroup = gr;
                    }

                    
                    pathElements = curPathElements;
                
                currentGroup.Elements.Add(new ContentGroupElement(data,currentGroup));
               
               
            }

             return master;
        }
        public void OnEnable()
        {
            try
            {
                InternalEnable();
            }
            catch (Exception exception)
            {
                LogError($"During enabling an error was thrown {exception}");
                Debug.LogException(exception);
            }
        }

        private void LogGlobalWarning(string warning)
        {
            this.warnings.Log(warning);
        }
        private void LogError(string error)
        {
            errors.Log(error);
            
            incorrectState = true;
        }

        public  void OnInspectorGUI(Action baseOnGui)
        {
            
            HoneySerializedObject.Update();
          
            if (incorrectState)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"Serious errors were encountered. Normal editor will be drawn instead \n\n{errors}",MessageType.Error);
                if (GUILayout.Button("Ignore once"))
                {
                    incorrectState = false;
                }
                EditorGUILayout.EndHorizontal();
                baseOnGui();
            }
            else
            {
                OnHoneyInspectorGUI();
            }
            HoneySerializedObject.ApplyModifiedProperties();
        }
        private void OnHoneyInspectorGUI()
        {
            HoneyErrorListenerStack.Push(listener);
            if (tempMemory.GetSize() > 0)
            {
                localWarnings.Log ("Temp memory was not clean");
                tempMemory.Clear();
            }
            GUI.enabled = false;
            if (scriptProperty != null)
                EditorGUILayout.PropertyField(scriptProperty);
            GUI.enabled = true;
         
            if (errors.ToString().Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"Errors:\n\n {errors}",MessageType.Error);
                if (GUILayout.Button("Restore default editor"))
                {
                    incorrectState = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (warnings.ToString().Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"Warnings:\n\n {warnings}",MessageType.Warning);
                if (GUILayout.Button("Clear"))
                {
                    warnings.Clear();
                }
                EditorGUILayout.EndVertical();
            }
            if (localWarnings.ToString().Length > 0)
            {
                EditorGUILayout.HelpBox($"Some property drawers encountered errors:\n\n{localWarnings}",MessageType.Warning);
            }
            localWarnings.Clear();

            if (masterGroup == null)
            {
               LogError("master group was not set");
               return;
            }
            DrawGroup(masterGroup);
            HoneyErrorListenerStack.Pop();


        }

        public void DrawGroup(Group g)
        {
            Action<string, string,HoneyEditorHandler,Attribute?,Group> action;
            if (g.Drawer == null)
                action = (_, _, _,_,gg) => {DrawGroupContent(gg); };
            else
                action = g.Drawer.DrawLayout;
            action(g.Path, g.Name, this, g.GroupAttribute, g);


        }

        public  void  DrawGroupContent(Group g)
        {
            foreach (IGroupElement element in g.Elements)
            {
                if (element is ContentGroupElement data)
                {
                    ElementGUI(data.Data);   
                }
                else if (element is Group other)
                {
                    DrawGroup(other);
                }
            }
        }
        public void ElementGUI(IEditorData data)
        {
            if (data is PropertyData propertyData)
            {
                if (propertyData.Handler != null)
                {
                
                    
                    HandlePreBuildSubEditor(()=>propertyData.Handler.OnInspectorGUI(() => PropertyGUIIgnoreSubEditors(propertyData)),propertyData);
              
                }
                else PropertyGUIIgnoreSubEditors(propertyData);
            }
            else if (data is CustomMemberData memb)
            {
                DoSafe(() =>
                {
                    memb.Drawer.OnGUI(HoneySerializedObject.GetInstance());
                });
                
            }
        }
        private void HandlePreBuildSubEditor(Action body,PropertyData propertyData)
        {
            if (propertyData.SubMode == null)
            {
                Debug.LogError("SubMode was not defined");
                return;
            }
            switch (propertyData.SubMode.Value)
            {
                case SubEditorMode.VectLike:
                    HandleVectLikeSubEditor(body,propertyData);
                    break;
                case SubEditorMode.Normal:
                    HandleNormalSubEditor(body,propertyData);
                    break;
            }
        }

        private void HandleVectLikeSubEditor(Action body, PropertyData propertyData)
        {
            EditorGUILayout.BeginHorizontal();
            float old = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth /= 2.2f;
            EditorGUILayout.LabelField(propertyData.SerializedProperty.displayName,new GUIStyle(EditorStyles.boldLabel)
            {
            
                fixedWidth = EditorGUIUtility.labelWidth/2f,
                stretchWidth = false
            });
            EditorGUIUtility.labelWidth = old;
            
            EditorGUILayout.BeginVertical();
          
            EditorGUI.indentLevel++;
            body();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void HandleNormalSubEditor(Action body, PropertyData data)
        {
            data.SerializedProperty.isExpanded = EditorGUILayout.Foldout(data.SerializedProperty.isExpanded,
                data.SerializedProperty.displayName,true);
            if (!data.SerializedProperty.isExpanded)
                return;
            EditorGUI.indentLevel++;
            body();
            EditorGUI.indentLevel--;
        }

        private void DoSafe(Action action)
        {
            try
            {
                action();
            }
            catch(Exception exception)
            {
                if (HoneyEG.IsGUIExitException(exception))
                    throw;
                LogError(exception.ToString());
                Debug.LogException(exception);
            }
        }
        private void PropertyGUIIgnoreSubEditors(PropertyData propertyData)
        {
            if (propertyData.Drawer == null )
            {
                DoSafe(() =>
                {
                    HoneyEG.PropertyFieldLayout(propertyData.SerializedProperty,null);
                });
              
            }
            else
            {
                var property = propertyData.SerializedProperty;

                GUIContent title = new GUIContent(property.displayName,property.tooltip);
                
              
                DoSafe(() =>
                {
                    float height = propertyData.Drawer.QueryHeight(propertyData.SerializedProperty, propertyData.Field, title,listener,true,1f,tempMemory);
                    Rect rect = EditorGUILayout.GetControlRect(hasLabel: true, height);
                    propertyData.Drawer.OnGui(propertyData.SerializedProperty,propertyData.Field,rect,tempMemory,title,listener,true);

                });
                
            }

        }
        
    }
}
#endif