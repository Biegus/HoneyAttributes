using System;
using UnityEngine;
namespace Honey.Objects
{
    //TODO: not documented
    [Serializable]
    public struct HoneyOptional<T>
    {
        public static HoneyOptional<T> Empty { get; }=  new HoneyOptional<T>();
        
        [SerializeField]
        private bool custom;
        [SerializeField]
        private T value;
        public HoneyOptional(T value)
        {
            this.custom = true;
            this.value = value;
        }
        public bool HasValue() { return custom;}
        public T GetValue(T def=default)
        {
            return !custom ? def : value;
        }

    }
}