#if UNITY_EDITOR
using Honey;

namespace  Honey.Editor
{
    public class HoneyRuntimeFieldAttributeData
    {
        public HoneyTypeDrawerDefinition Definition { get; }
        public HoneyAttribute Attribute { get; }
        public HoneyRuntimeFieldAttributeData(HoneyTypeDrawerDefinition definition, HoneyAttribute attribute)
        {
            Definition = definition;
            Attribute = attribute;
        }
    }
}
#endif