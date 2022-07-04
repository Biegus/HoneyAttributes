using System;

namespace Honey
{
    public class GroupDefinitionAttribute:  Attribute
    {
        public string Path { get; }

        public GroupDefinitionAttribute(string path)
        {
            Path = path;
        }
    }
}