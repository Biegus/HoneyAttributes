#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Honey.Editor
{
    public static class SerializedPropertyHelper
    {
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
        {
            var copy= property.Copy();
           foreach(object item in copy)
            {
                yield return (SerializedProperty)item;
            }
        }
        public static IEnumerable<SerializedProperty> GetArrayElements(this SerializedProperty property)
        {
            for(int x=0;x<property.arraySize;x++)
            {
                yield return property.GetArrayElementAtIndex(x);
            }
        }
        public static object? GetValue(this SerializedProperty prop)
        {
            SerializedProperty valueProp;
            return prop.propertyType switch
            {
                SerializedPropertyType.Generic when (valueProp = prop.FindPropertyRelative("_Value")) != null => valueProp.GetValue(),
                SerializedPropertyType.String => prop.stringValue,
                SerializedPropertyType.Boolean => prop.boolValue,
                SerializedPropertyType.Integer => prop.intValue,
                SerializedPropertyType.Float => prop.floatValue,
                SerializedPropertyType.Vector2 => prop.vector2Value,
                SerializedPropertyType.Vector3 => prop.vector3Value,
                SerializedPropertyType.Color => prop.colorValue,
                SerializedPropertyType.Rect => prop.rectValue,
                SerializedPropertyType.Enum => prop.enumValueIndex,
                SerializedPropertyType.ObjectReference => prop.objectReferenceValue,
                SerializedPropertyType.RectInt => prop.rectIntValue,
                SerializedPropertyType.Vector2Int => prop.vector2IntValue,
                SerializedPropertyType.Vector3Int => prop.vector3IntValue,
                SerializedPropertyType.ManagedReference => prop.managedReferenceValue,
                _ => null
            };
        }
        public static void SetValue(this SerializedProperty prop, object? value)
        {
            SerializedProperty valueProp;
            _= prop.propertyType switch
            {
                SerializedPropertyType.Generic when (valueProp = prop.FindPropertyRelative("_Value")) != null => valueProp.GetValue(),
                SerializedPropertyType.String => prop.stringValue= (string)value!,
                SerializedPropertyType.Boolean => prop.boolValue=(bool)value!,
                SerializedPropertyType.Integer => prop.intValue=(int)value!,
                SerializedPropertyType.Float => prop.floatValue=(float)value!,
                SerializedPropertyType.Vector2 => prop.vector2Value=(Vector2)value!,
                SerializedPropertyType.Vector3 => prop.vector3Value=(Vector3)value!,
                SerializedPropertyType.Color => prop.colorValue=(Color) value!,
                SerializedPropertyType.Rect => prop.rectValue=(Rect)value!,
                SerializedPropertyType.Enum => prop.intValue= (int) value!,
                SerializedPropertyType.ObjectReference => prop.objectReferenceValue=(UnityEngine.Object?) value,
                SerializedPropertyType.RectInt => prop.rectIntValue=(RectInt) value!,
                SerializedPropertyType.Vector2Int => prop.vector2IntValue=(Vector2Int) value!,
                SerializedPropertyType.Vector3Int => prop.vector3IntValue=(Vector3Int) value!,
                SerializedPropertyType.ManagedReference=>prop.managedReferenceValue=value,
                _ => null
            };
        }
       
        private static Regex indexRegex = new Regex(@"\[(?<value>\d+)\]");

        public static int? GetIndexOfSerializedPropertyPath(string propertyPath)
        {
            var match= indexRegex.Match(propertyPath);
            if (match.Success)
                return int.Parse(match.Groups["value"].Value);
            else return null;
        }
       



    

       
 
    }
    
}
#endif