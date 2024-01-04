using System;

namespace Honey
{
    //todo: not documented
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class EBeginGroupAttribute : Attribute
    {
        public string Path { get; }

        public EBeginGroupAttribute(string path)
        {
            Path = path;
        }
    }
}