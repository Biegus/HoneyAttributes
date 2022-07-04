#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;

namespace Honey.Editor
{
    public interface IHoneyTempMemory
    {
        void Add((object inst, FieldInfo field) key, object value);
         T TakeAndRemove<T>((object inst, FieldInfo field) key);
         int GetSize();
    }
    
    public class HoneyTempTempMemory : IHoneyTempMemory
    {
        private Dictionary<(object inst, FieldInfo field), object> dict = new();
        public void Add((object inst, FieldInfo field) key, object value)
        {
            dict[key] = value;
        }

        public T TakeAndRemove<T>((object inst, FieldInfo field) key)
        {
            var res= (T)dict[key];
            dict.Remove(key);
            return res;
        }

        public int GetSize()
        {
            return dict.Count;
        }

        public void Clear()
        {
            dict.Clear();
        }
    }
}
#endif