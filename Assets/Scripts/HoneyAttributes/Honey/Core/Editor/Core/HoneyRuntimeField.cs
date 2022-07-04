#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Honey;
using UnityEditor;
using UnityEngine;

namespace Honey.Editor
{
    public  class HoneyRuntimeField
    {
        private readonly HoneyRuntimeFieldAttributeData[] sortedByMain;
        private readonly HoneyRuntimeFieldAttributeData[] elements;
        public IReadOnlyList<HoneyRuntimeFieldAttributeData> Elements => elements;
        internal HoneyRuntimeField(HoneyRuntimeFieldAttributeData[] elements)
        {
            this.elements = elements;
            var first = this.elements.FirstOrDefault(item => item.Definition.FirstDrawer != null);
            if (first == null)
            {
                sortedByMain = Array.Empty<HoneyRuntimeFieldAttributeData>();
                return;
            }

            if (first.Definition.FirstDrawer is {RecursiveMode: HoneyRecursiveMode.Disabled})
            {
                sortedByMain = new[] {first};
                return;
            }
          
            
            sortedByMain=
               (
                   from element in elements
                   where element.Definition.FirstDrawer!=null && element.Definition.FirstDrawer.RecursiveMode!=HoneyRecursiveMode.Disabled
                   orderby  element.Definition.FirstDrawer.RecursiveMode ascending
                   select element
                    ).ToArray();
            if (sortedByMain.Length >= 2 &&
                sortedByMain[sortedByMain.Length - 2].Definition.FirstDrawer.RecursiveMode ==
                HoneyRecursiveMode.Deepest)
                throw new ArgumentException("There cannot be more than one deepest attribute used at the same time");
    

        }

     
        public float QueryHeight(SerializedProperty property, FieldInfo field,GUIContent title,
            IHoneyErrorListener listener,bool layoutContext,float width, IHoneyTempMemory memory)
        {
            (object obj, object container) = GetTupleIfAsked(property);
   
            if (elements.Length == 0)
            {
                //return default

                return EditorGUI.GetPropertyHeight(property);
            }
            HoneyDrawerInput input;
            input.Field = field;
            input.Obj = obj;
            input.Container = container;
            input.SerializedProperty = property;
            input.TempMemory = memory;
            input.Listener = listener;
            input.AllowLayout = layoutContext;  
            float sum = 0;
            var state = GetState(input);
            foreach (var element in elements)
            {
                sum += element.Definition.Additional?.GetHeight(input, AdditionalDrawerCallType.PreBefore,element.Attribute)??0;
                if (state == HoneyPropertyState.Hidden) 
                    continue;
                
                sum += element.Definition.Additional?.GetHeight(input, AdditionalDrawerCallType.Before,element.Attribute)??0;
                sum += element.Definition.Additional?.GetHeight(input, AdditionalDrawerCallType.After,element.Attribute)??0;
                sum -= element.Definition.Additional?.GetHeightToErase(input, element.Attribute) ?? 0;
            }



            if (state != HoneyPropertyState.Hidden)
                sum += GetMainHeight(input, title);
            return sum;
        }

        private (object obj, object container) GetTupleIfAsked(SerializedProperty property)
        {
            bool anyWantsHierarchyQuery = elements.Any(element => element.Definition.Additional?.RequestHierarchyQuery??false);
            anyWantsHierarchyQuery = anyWantsHierarchyQuery || elements.Any(element => element.Definition.FirstDrawer?.RequestHierarchyQuery??false);
            object obj = null;
            object container = null;
            if (anyWantsHierarchyQuery)
                ( obj,  container) = HoneyHandler.HoneyReflectionCache.ReadObject(property);
            return (obj, container);
        }
        public void OnGui(SerializedProperty property, FieldInfo field, Rect position, IHoneyTempMemory tempMemory,
            GUIContent title, IHoneyErrorListener listener, bool layoutContext)
        {

            (object obj, object container) = GetTupleIfAsked(property);
            if (elements.Length == 0)
            {
                //draw normal gui
                EditorGUI.PropertyField(position, property);
                return;
            }
            Color color = GUI.color;
            HoneyDrawerInput drawerInput;
            drawerInput.Field = field;
            drawerInput.Container = container;
            drawerInput.Obj = obj;
            drawerInput.SerializedProperty = property;
            drawerInput.TempMemory = tempMemory;
            drawerInput.Listener = listener;
            drawerInput.AllowLayout = layoutContext;

            bool anyCustomTitle = false;
            foreach (HoneyRuntimeFieldAttributeData element in elements)
            {
                var custom = element.Definition.Additional?.GetCustomContent(drawerInput, element.Attribute, title);
                if (custom != null)
                {
                    anyCustomTitle = true;
                    title = custom;
                }
                
                position.height= element.Definition.Additional?.GetHeight(drawerInput, AdditionalDrawerCallType.PreBefore,
                    element.Attribute) ?? 0;
                element.Definition.Additional?.PreBefore(drawerInput, position, element.Attribute);
               
                position.y +=position.height ;
            }

            if (anyCustomTitle)
                title = new GUIContent(title);//has to do it, content will reverse otherwise, no need to do in case no custom title is present.

            HoneyPropertyState state = HoneyPropertyState.Normal;
            state = GetState(drawerInput);

            bool oldGUi = GUI.enabled;
            if (state == HoneyPropertyState.Disabled)
            {
                GUI.enabled = false;
            }
            else if (state == HoneyPropertyState.Hidden || state== HoneyPropertyState.Invisible)
            {
                return;
            }
            
            
            foreach (HoneyRuntimeFieldAttributeData element in elements)
            {

                position.height= element.Definition.Additional?.GetHeight(drawerInput, AdditionalDrawerCallType.Before,
                    element.Attribute) ?? 0;
                element.Definition.Additional?.Before(drawerInput, position, element.Attribute);
                position.y += position.height;
            }

            if (sortedByMain.Length==0)
            {
               
               
                position.height = EditorGUI.GetPropertyHeight(drawerInput.SerializedProperty,title);
               HoneyEG.PropertyField(position, drawerInput.SerializedProperty,title);
                position.y += position.height;
            }
               
            else
            {
                void Body(GUIContent content, Rect rect,int index)
                {
                    for (int i = index + 1; i < elements.Length; i++)
                    {
                        if (elements[i].Definition.FirstDrawer != null)
                        {
                            elements[i].Definition.FirstDrawer.Draw(
                                drawerInput,rect,elements[i].Attribute,content, (c,r) =>
                                {
                                    int cp = i;
                                    Body(c, r, cp);
                                    
                                });
                            return;
                        }
                    }
                    HoneyEG.PropertyField(rect, property);
                }
                int firstIndex = -1;
                int i = 0;
              
                foreach (var element in elements)
                {
                    if (element.Definition.FirstDrawer != null)
                    {
                        firstIndex = i;
                        break;
                    }
                    i += 1;
                }

                var first = elements[i].Definition.FirstDrawer;
                position.height =GetMainHeight(drawerInput,title);
                first.Draw(drawerInput,position,elements[i].Attribute,title,
                    (c,r) =>
                    {
                        Body(c,r,firstIndex);
                    });
                position.y += position.height;
            }

     
            foreach (HoneyRuntimeFieldAttributeData element in elements.Reverse())//reverse order for better dispose handling
            {
                position.height= element.Definition.Additional?.GetHeight(drawerInput, AdditionalDrawerCallType.After,
                    element.Attribute) ?? 0;
                element.Definition.Additional?.After(drawerInput,position,element.Attribute);
                position.y += position.height;
            }

            GUI.enabled = oldGUi;
            GUI.color = color;
        }

        private float GetMainHeight(in HoneyDrawerInput drawerInput,GUIContent title)
        {
            float h = 0;
            float hMin = -1;
            foreach (var element in sortedByMain)
            {
                
                h += element.Definition.FirstDrawer?.GetDesiredAdditionalHeight(drawerInput, element.Attribute, title) ?? 0;
                hMin = Math.Max(
                    element.Definition.FirstDrawer?.GetDesiredMinBaseHeight(drawerInput, element.Attribute, title) ??
                    -1f, hMin);
            }

            if (hMin == -1)
                hMin = EditorGUI.GetPropertyHeight(drawerInput.SerializedProperty);
            return h + hMin;
        }

        private HoneyPropertyState GetState(HoneyDrawerInput drawerInput)
        {
            HoneyPropertyState state = HoneyPropertyState.Normal;
            foreach (HoneyRuntimeFieldAttributeData element in elements)
            {
                HoneyPropertyState givenState = element.Definition.Additional?.GetDesiredState(drawerInput, element.Attribute) ??
                                                HoneyPropertyState.Normal;
                state = (HoneyPropertyState) Mathf.Max((int) state, (int) givenState);
                if (state == HoneyPropertyState.Hidden)
                    break;
            }

            return state;
        }
    }
}
#endif