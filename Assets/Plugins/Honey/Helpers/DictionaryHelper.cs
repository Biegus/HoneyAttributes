#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Honey.Helper
{
    public static class DictionaryHelper
    {
        public static TValue GetOrInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,TValue toInsert)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return dictionary[key] = toInsert;
        }
        public static TValue GetOrElseInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,Func<TValue> toInsert)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            if (toInsert == null)
                throw new ArgumentNullException(nameof(toInsert));

            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return dictionary[key] = toInsert.Invoke();
        }

    }
}