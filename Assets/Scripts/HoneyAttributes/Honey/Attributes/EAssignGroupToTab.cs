#nullable enable
using System;

namespace Honey
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EAssignGroupToTab : Attribute
    {
        public string Group { get; }
        public string Tab { get; }
        public EAssignGroupToTab(string @group, string tab)
        {
            Group = @group;
            Tab = tab;
        }

       
    }
}