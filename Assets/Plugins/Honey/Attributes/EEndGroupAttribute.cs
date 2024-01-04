using System;

namespace Honey
{
    //todo: not documented
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class EEndGroupAttribute : Attribute
    {

    }
}