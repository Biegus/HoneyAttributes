#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using Honey.Editor;

namespace Honey.Editor
{
    public class Group : IGroupElement
    {
        public  string Path { get; }
        public  string Name { get; }
        public readonly List<IGroupElement> Elements = new List<IGroupElement>();
        public Group? Father { get; }
        public IHoneyGroupDrawer? Drawer { get; }
        public Attribute? GroupAttribute { get; }
        public Group(string path,string name, Group? father,IHoneyGroupDrawer? drawer, Attribute? attribute)
        {
            Path = path;
            Name = name;
            Father = father;
            Drawer = drawer;
            this.GroupAttribute = attribute;
        }
    }
}
#endif