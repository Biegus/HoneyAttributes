#if UNITY_EDITOR
#nullable enable
namespace Honey.Editor
{
    public class HoneyTypeDrawerDefinition
    {
        
        public IHoneyAdditionalDrawer? Additional { get; }
     
        public IHoneyMainDrawer? FirstDrawer { get; }
        public HoneyTypeDrawerDefinition(IHoneyAdditionalDrawer? additional, IHoneyMainDrawer? firstDrawer)
        {
            Additional = additional;
            FirstDrawer = firstDrawer;
        }
        
    }
}
#endif