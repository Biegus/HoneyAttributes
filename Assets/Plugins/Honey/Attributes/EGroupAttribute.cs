#nullable enable
using System;

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class EGroupAttribute : Attribute
    {
        public string Path { get; }

        public EGroupAttribute(string path)
        {
            Path = path;
        }
    }   
}