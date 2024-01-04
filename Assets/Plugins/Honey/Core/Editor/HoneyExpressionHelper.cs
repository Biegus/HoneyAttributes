#if UNITY_EDITOR
#nullable enable
using System;
using System.Reflection;
using Honey.Core;
using Honey.Editor;
using UnityEditor;

namespace Honey.Editor
{
    public class HoneyExpressionHelper
    {
        public static HResult< Func<object,T>,string > DoAndCatch<T>(
            string expr,
            FieldInfo field
        )
        {
            if (expr == null)
                throw new ArgumentNullException(nameof(expr));
            if (field == null)
                throw new ArgumentNullException(nameof(field));


            HoneyValueParseFlags flags = HoneyValueParseFlags.None;
            if (HoneyReflectionUtility.IsInteger(typeof(T)))
                flags |= HoneyValueParseFlags.IntegerMode;
            if (typeof(T) == typeof(string))
            {
                flags |= HoneyValueParseFlags.StringMode;
            }
            try
            {
                    Func<object, object> directFunc =
                        HoneyValueParser.ParseExpression(expr, field.ReflectedType, field.Name, flags);
                    return HResult<Func<object, T>, string>.Value( o => (T) directFunc(o));
            }
            catch(Exception exception)
            {
                return HResult<Func<object, T>, string>.Err(
                    $"Honey expression \"{expr}\" failed with {exception.Message}");
            }
        }
    }
}
#endif