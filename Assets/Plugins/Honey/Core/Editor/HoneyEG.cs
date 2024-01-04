#if UNITY_EDITOR
#nullable  enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Honey;
using UnityEngine;
#if UNITY_EDITOR
using Honey.Editor;
using UnityEditor;
#endif

namespace Honey.Editor
{
   public static class HoneyEG
   {
      private static GUIContent Temp=new();

      /// <summary>
      /// Should only by used when giving direcly to flat unity function that won't call it again.
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static GUIContent TempContent(string text)
      {
         Temp.text = text;
         return Temp;
      }
      public static void PropertyField(Rect rect, SerializedProperty serializedProperty, GUIContent? content = null)
      {
         int bf = EditorGUI.indentLevel;
         if (HoneyHorizontalLayoutHelper.Counter > 0)
            EditorGUI.indentLevel = 0;
         if (serializedProperty.propertyType == SerializedPropertyType.ManagedReference)
         {
            DrawSerializeReference(rect, serializedProperty, content);
         }
         else
            EditorGUI.PropertyField(rect, serializedProperty, content, true);
         EditorGUI.indentLevel = bf;
      }
      public static void PropertyFieldLayout(SerializedProperty serializedProperty, GUIContent? content )
      {
         HoneyEG.PropertyField(EditorGUILayout.GetControlRect(true,EditorGUI.GetPropertyHeight(serializedProperty)), serializedProperty,content);
      }

      public static void DrawSerializeReference(Rect rect, SerializedProperty serializedProperty, GUIContent? content = null)
      {
         EditorGUI.PropertyField(rect, serializedProperty,content, true);
       
      }

      public static bool SideButton(ref Rect rect, string text)
      {
         Rect buttonRect = rect;

         rect.width -= 25f;

         buttonRect.width = 25f;
         buttonRect.height = EditorGUIUtility.singleLineHeight;
         buttonRect.x += rect.width;

         return GUI.Button(buttonRect, text);
      }
      public static GUIStyle? ProgressBarLabelBaseStyle { get; private set; }
      public static GUIStyle? ProgressBarBarBaseStyle { get; private set; }
      public static GUIStyle? ProgressBarBarBackgroundStyle { get; private set; }

      private static void InitProgressBarStyles()
      {
         if (ProgressBarLabelBaseStyle == null)
         {
            ProgressBarLabelBaseStyle = new GUIStyle("label")
            {
               alignment = TextAnchor.MiddleCenter,
               normal = {textColor = Color.white},
               richText = true
            };

            Texture2D texture = new Texture2D(10,10);
            for(int y=0;y<10;y++)
            for (int x = 0; x < 10; x++)
            {
               Color clr = Color.white * Mathf.Lerp(0.8f, 1f, Mathf.Clamp((9 - y) / 2f, 0, 1));
               clr.a = 1;
               texture.SetPixel(x,y,clr);
            }
            texture.Apply();
            ProgressBarBarBaseStyle = new GUIStyle()
            {
               normal = {background = texture}
            };
         
          
            PropertyInfo? internalPropertyBack = typeof(EditorStyles)
               .GetProperty("progressBarBack", BindingFlags.NonPublic | BindingFlags.Static);

            if (internalPropertyBack == null) //rip internal api changed
            {
               //i will use some base version then
               ProgressBarBarBackgroundStyle = new GUIStyle()
               {
                  normal = {background = Texture2D.whiteTexture}
               };
            }
            else ProgressBarBarBackgroundStyle = new GUIStyle((GUIStyle)internalPropertyBack.GetValue(null));

         }
      }
      public static void ProgressBar(Rect rect, float value, string label, Color? barColor=null,Color? labelColor=null, GUIStyle? labelStyle=null,GUIStyle? barStyle=null, GUIStyle? backstyle=null)
      {
         InitProgressBarStyles();
       
         Rect left = rect;
         left.width *= value;
         var old = GUI.color;
       
         var style = labelStyle ?? (ProgressBarLabelBaseStyle);

         barStyle ??= ProgressBarBarBaseStyle;
         backstyle ??= ProgressBarBarBackgroundStyle;
         if (Event.current.type == EventType.Repaint)
         {
            backstyle!.Draw(rect, GUIContent.none, true, false, false, false);
            GUI.color = barColor ?? GUI.color;

            barStyle!.Draw(left, GUIContent.none, false, false, false, false);
            GUI.color = old;

         }

         Rect bt1 = rect;
      
         bt1.x += rect.width * (8f/ 10);
         bt1.width = ( 1 / 10f * rect.width)/1.8f;
         bt1.height /= 1.3f;
         bt1.y += (rect.height- bt1.height) / 2f;
         Rect bt2 = bt1;
         bt2.x += rect.width * (1f / 10);
         
         GUI.color = labelColor ?? GUI.color;
         GUI.Label(rect, label, style);
         GUI.color = old;


      }

      public static (Color barColor, Color labelColor) GetColorsFromProgressStyle(ProgressBarStyle style)
      {
         Color barColor;
         Color labelColor;
         switch (style)
         {
            case ProgressBarStyle.Red:
               barColor = new Color32(222, 49, 99, 255);
               labelColor = Color.white;
               break;
            case  ProgressBarStyle.Blue:
               barColor = new Color32(114, 133, 165,255);
               labelColor = Color.white;
               break;
            default:
               barColor = Color.black;
               labelColor = Color.black;
               break;
         }

         return (barColor, labelColor);
      }

      public static void DrawLocalAttributeError(Rect rect,MemberInfo member, string typeName, Attribute attribute,string message)
      {
         EditorGUI.HelpBox(rect,$"[{attribute}]({typeName} {member.Name}) -> {message}",MessageType.Warning);
      }


      public static GenericMenu BuildGenericMenu(string path, UnityEngine.Object container, IEnumerable values, FieldInfo field,
         IReadOnlyList<GUIContent>? names = null)
         => BuildGenericMenuGeneratingValues(path, container, values.Cast<object>().Select<object,Func<object>>(item => () => item), field,
            names);

      public static void ApplyValueOnProperty(UnityEngine.Object container, FieldInfo field,string path, int? index, object? value)
      {
         var serializedObject = new SerializedObject(container);
         var prop = serializedObject.FindProperty(path);
         if (prop.propertyType == SerializedPropertyType.Generic)
         {
            if (index != null)
            {
               IList? list = field.GetValue(container) as IList;
               if (list == null)
               {
                  throw new ArgumentException("Index was not null, but field was not collection");
               }
               list[index.Value] = value;
            }
            else
            {
               field.SetValue(container,value);
            }
                
                 
         }
         else // it's better to use unity system since it will work with a structs
         {
                  
            prop.SetValue(value);
                 
         }

         serializedObject.ApplyModifiedProperties();
      }
      public static GenericMenu BuildGenericMenuGeneratingValues(string path, UnityEngine.Object container, IEnumerable<Func<object>> values,FieldInfo field, IReadOnlyList<GUIContent>? names=null)
      {

         int? index = SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(path);
         
         GenericMenu menu = new();
         int i = 0;
         foreach (Func<object> action in values)
         {
            var element = action.Invoke();
            menu.AddItem(((names==null)? new GUIContent(element?.ToString()??">null<"): names[i++]),false, (value) =>
            {
               ApplyValueOnProperty(container,field,path,index,value);  
            },element);
         }

         return menu;
      }
      public static (Rect label, Rect field)  CutIntoLabelAndField(Rect position)
      {
         Rect labelRect = position;
         Rect fieldRect = position;
         labelRect.width = Mathf.Min(labelRect.width, EditorGUIUtility.labelWidth);
         fieldRect.width -= labelRect.width;
         fieldRect.x += labelRect.width;
         return (labelRect, fieldRect);
      }
      public static void DoSafe(Action action, SerializedProperty property)
      {
         try
         {
            action();
         }
         catch (Exception exception)
         {
            if (IsGUIExitException(exception))
               throw;
            Debug.LogException(exception);
            Debug.LogError($"^ above exception happened during drawing {property.name} ({property.displayName}) in {property.propertyPath}");
         }
      }
      public static bool IsGUIExitException(Exception exception)
      {
         while (exception is TargetInvocationException && exception.InnerException != null)
         {
            exception = exception.InnerException;
         }
         return exception is ExitGUIException;
         
      }
    
     
     
   }
}
#endif