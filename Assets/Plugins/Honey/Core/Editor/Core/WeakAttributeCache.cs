#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Honey.Editor;
using Honey.Helper;
using Honey.Validation;
using UnityEditor;


namespace Honey.Editor
{
    public class WeakAttributeCache<T>
    {
        // conditional weak table prevents memory leak.
        private ConditionalWeakTable<UnityEngine.Object, Dictionary<(string,Attribute?), T>> cache=new();

        private Dictionary<(string,Attribute?),T> GetInner(UnityEngine.Object ob)
        {
            if (ob == null) throw new ArgumentNullException(nameof(ob));
            if (cache.TryGetValue(ob,out Dictionary<(string,Attribute?), T> result))
            {
                return result;
            }
            Dictionary<(string,Attribute?), T> newInner = new();
            cache.Add(ob,newInner);
            return newInner;

        }
        public T GetOrElseInsert(UnityEngine.Object obj,string path,Attribute? attribute,Func<T> fnc)
        {
            return GetInner(obj).GetOrElseInsert((path,attribute), fnc);
        }

        public T GetOrElseInsert(SerializedObject obj, string path,Attribute? attribute, Func<T> fnc)
        {
            return GetOrElseInsert(obj.targetObject, path,attribute, fnc);
        }
        public T GetOrElseInsert(SerializedProperty property,Attribute? attribute,Func<T> fnc)
        {
            return GetOrElseInsert(property.serializedObject, property.propertyPath,attribute,fnc);
        }

        public bool Has(UnityEngine.Object obj, string path, Attribute? attribute )
        {

            return GetInner(obj).TryGetValue((path, attribute), out T _);
        }

        public T GetOr(UnityEngine.Object obj, string path, Attribute? attribute,T def)
        {
            return GetInner(obj).TryGetValue((path, attribute), out T result) ? result : def;
        }

        public T Set(UnityEngine.Object obj, string path, Attribute? attribute, T val)
        {
            return GetInner(obj)[(path, attribute)] = val;
        }

        public T Set(SerializedProperty property,Attribute? attribute, T val)
        {
            return Set(property.serializedObject.targetObject,property.propertyPath,attribute,val);
        }

        public bool TryGetValue(SerializedProperty prop,Attribute? attribute, out T val)
        {
            return GetInner(prop.serializedObject.targetObject).TryGetValue((prop.propertyPath,attribute), out val);
        }

        public T Update(UnityEngine.Object obj, string path,Attribute? attribute, Func<T> def, Func<T, T> useFunc)
        {
            var value = this.GetOrElseInsert(obj, path,attribute, def);
            value = useFunc(value);
            this.Set(obj, path, attribute, value);
            return value;
        }

    }
}
#endif