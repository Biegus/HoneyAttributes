#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Honey.Core;
using Honey.Editor;

#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace Honey.Editor
{
    public class HoneyValueExpressionGetterCache<T>
    {
        private readonly Dictionary<(string name, FieldInfo field,int? index), Func<object, T>> dict = new();


        #if UNITY_EDITOR
        public Func<object, T> Get(string text, FieldInfo field, SerializedProperty property)
        {
            return Get(text, field, SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(property.propertyPath));
        }
        #endif
        public Func<object, T> Get(string text,FieldInfo field,int? index)
        {
            if (dict.TryGetValue((text,field,index), out var result))
                return result;
            HoneyValueParseFlags flags = HoneyValueParseFlags.None;
            if (HoneyReflectionUtility.IsInteger(typeof(T)))
                flags |= HoneyValueParseFlags.IntegerMode;
            if (typeof(T) == typeof(string))
            {
                flags |= HoneyValueParseFlags.StringMode;
            }
            
            var expr= HoneyValueParser.ParseExpression(text, field.ReflectedType,field.Name, 
                flags);
            return dict[(text, field,index)] =
                (obj) => (T)expr(obj);
        }
    }
    public class GetterCache<T>
    {
        private readonly Dictionary<(string name, MemberInfo field), Func<object, T>> dict = new();

     
        public Func<object, T> Get(string name,MemberInfo identity)
        {
            if (dict.TryGetValue((name,identity), out var result))
                return result;
            var getter= HoneyReflectionUtility.CreateGeneralGetter<T>(name, identity.DeclaringType!);
            if (getter == null)
                throw new ArgumentException("getter was not found");
            return dict[(name,identity)] = getter;
            
        }
    }
    
}
#endif