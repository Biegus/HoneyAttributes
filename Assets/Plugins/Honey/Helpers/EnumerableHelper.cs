#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Honey.Helper
{
    public static class EnumerableHelper
    {
        public  static string ToStringFromCollection<T>(this IEnumerable<T> collection, string separator = ", " )
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            return collection.Aggregate(new StringBuilder(), (b, v) => b.Append($"{v}{separator}")).ToString();
        }

        public static IEnumerable<TIN> WhileTransform<TIN>(TIN first,  Func<TIN,TIN> transf,  Func<TIN,bool> cond)
        {
            if (transf == null) throw new ArgumentNullException(nameof(transf));
            if (cond == null) throw new ArgumentNullException(nameof(cond));
            var element = first;
            while (cond(element))
            {
                yield return element;
                element = transf(element);
            }

        }
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T,int> action)
        {
            if (collection is null) throw new ArgumentNullException();
            if (action is IReadOnlyList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    action(list[i],i);
                }
            }

            int index = 0;
            foreach (var element in collection)
            {
                action(element,index);
                index += 1;
            }
        }

        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection is null) throw new ArgumentNullException();
            if (action is IReadOnlyList<T> list)
            {

                for (int i = 0; i < list.Count; i++)
                {
                    action(list[i]);
                }
            }
            foreach (var element in collection)
            {
                action(element);
            }
        }

        public static string BuildString<T>(this IEnumerable<T> collection, Func<T,int,string>? getter=null, bool skipLast=false)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            getter ??= (v,i) => v.ToString();
            int index = 0;
            var builder = collection.Aggregate(new StringBuilder(), (b, v) => b.Append(getter(v, index++)));
            if (skipLast && builder.Length>0)
                builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
        public static object GetAtIndex(IEnumerable obj,  int index)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            int i = 0;
            foreach (object el in obj)
            {
                if (i == index)
                    return el;
                i += 1;
            }
            return null;
        }

        public static int GetFirstIncorrectIndex<T>(IReadOnlyList<T> a, IReadOnlyList<T> b)

        {
            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            {
                if (!object.Equals(a[i],b[i]))
                    return i;
            }

            return Math.Min(a.Count, b.Count);
        }

    }
}