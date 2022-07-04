#if UNITY_EDITOR
#nullable enable
using System;
namespace  Honey.Editor
{
    public class HoneyRuntimeGroupDefinition
    {
        public Attribute Attribute { get; }
        public IHoneyGroupDrawer Drawer { get; }

        public HoneyRuntimeGroupDefinition( Attribute attribute, IHoneyGroupDrawer drawer)
        {
            Attribute = attribute;
            Drawer = drawer;
        }
    }
}
#endif