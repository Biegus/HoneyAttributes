using System;
using System.Collections;
using System.Collections.Generic;

namespace Honey.Helper
{
    public static class DictionaryHelper
    {
        public static TValue GetOrInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,TValue toInsert)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return dictionary[key] = toInsert;
        }
        public static TValue GetOrInsertFunc<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,Func<TValue> toInsert)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return dictionary[key] = toInsert();
        }
        
    }
}