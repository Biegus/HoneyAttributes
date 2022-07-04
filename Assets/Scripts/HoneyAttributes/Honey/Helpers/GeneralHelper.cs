#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;
namespace Honey.Helper
{
    public static class GeneralExtension
    {
        private class ScalarStruct<T> : IEnumerable<T>
        {
            public T Value { get; }
            public ScalarStruct(T value)
            {
                this.Value = value;
            }
            public IEnumerator<T> GetEnumerator()
            {
                yield return Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }


        }
      
        public static IEnumerable<T> Scalar<T>(this T element)
        {
            return new ScalarStruct<T>(element);
        }

        public static string ToDebugString<T>(this T obj, int maxDeepLv = 10)
        {
            return ToDebugString(obj, 0, maxDeepLv);
        }
        private static string ToDebugString<T>(this T obj, uint deep, int maxDeeplv)
        {

            string basic = obj?.ToString() ?? ">null<";
            if (deep > maxDeeplv)
            {
                return basic;
            }
            uint next = deep + 1;
            if ((basic == (obj?.GetType().ToString() ?? string.Empty)))
            {

                StringBuilder ele = new StringBuilder();
                bool first = true;
                string SaveToDebugString(object val)
                {
                    if (System.Object.ReferenceEquals(val, obj))
                    {
                        return "this";
                    }
                    else
                        return ToDebugString(val, next, maxDeeplv);
                }
                void CheckFirst()
                {
                    if (first == false)
                    {
                        ele.Append(", ");

                    }

                    first = false;
                }
                switch (obj)
                {
                    case IDictionary dict:

                        foreach (var key in dict.Keys)
                        {
                            CheckFirst();
                            ele.Append($"{{{SaveToDebugString(key)}}}={{{SaveToDebugString(dict[key])}}}");
                        }
                        return $"{{ {ele} }}";
                    case IEnumerable collection:


                        foreach (var element in collection)
                        {
                            CheckFirst();
                            ele.Append(SaveToDebugString(element));
                        }
                        return $"{{ {ele} }}";
                }
            }
            return basic;
        }
        public static T As<T>(this object obj)
            where T:class
        {
            return obj as T ?? throw new InvalidCastException();
        }
    }
}
