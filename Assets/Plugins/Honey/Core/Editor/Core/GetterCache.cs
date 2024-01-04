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
using Honey.Helper;
using Object = UnityEngine.Object;

#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace Honey.Editor
{
    public interface IHoneyValueExpressionGetterCache
    {
        public HResult<Func<object, object>,string> GetRaw(string text, FieldInfo field, SerializedProperty property);
        public HResult<object,string> GetAndInvokeRaw(object container,string text, FieldInfo field, SerializedProperty property);
    }

    public class HoneyValueExpressionGetterCache<T> : IHoneyValueExpressionGetterCache
    {

        private readonly WeakAttributeCache<Dictionary<string, HResult< Func<object, T>,string>>> cache = new();

        /// <summary>
        /// Returns err either for parsing or invoke error.
        /// </summary>
        public HResult<T, string> GetAndInvoke(object container, string expression, FieldInfo field,
            SerializedProperty property)
        {
            var fnc = Get(expression, field, property);
            try
            {
                return fnc.Map(e => e(container));
            }
            catch (Exception exception)
            {
                return HResult<T, string>.Err(
                    $"Error while executing honey expression: \"{expression}\" \n: Err:{exception.Message}");
            }
        }
        public HResult<Func<object, T>, string> Get(string text,FieldInfo field, SerializedProperty property)
        {
            return cache.GetOrElseInsert(property, null, () => new())
                .GetOrElseInsert(text, () => HoneyExpressionHelper.DoAndCatch<T>(text, field));
        }

        public HResult< Func<object, object>,string> GetRaw(string text, FieldInfo field, SerializedProperty property)
        {
            return Get(text, field, property).Map<Func<object, object>>(e => a => e(a).ToNonNullable()!);
        }

        public HResult<object, string> GetAndInvokeRaw(object container,string text, FieldInfo field, SerializedProperty property)
        {
            return GetAndInvoke(container,text, field, property).Map(e => ((object?) e).ToNonNullable());
        }
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
#endif