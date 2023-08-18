#nullable enable
using System;
using System.Reflection;
using Honey;
using Honey.Core;
using Honey.Helper;
#if UNITY_EDITOR



namespace Honey.Editor
{
    public class ShowIfAttributeDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private static HoneyValueExpressionGetterCache<bool> cache = new HoneyValueExpressionGetterCache<bool>();
        public HoneyPropertyState GetDesiredState(in HoneyDrawerInput inp, HoneyAttribute attribute)
        {

            HShowIfAttribute atr = attribute.As<HShowIfAttribute>();
            if (!Check(inp, atr))
                return atr.State;
            else return HoneyPropertyState.Normal;
        }

        public bool Check(in HoneyDrawerInput inp, HShowIfAttribute attribute)
        {
            try
            {
                return cache.Get(attribute.Name, inp.Field, inp.SerializedProperty)(inp.Container);
            }
            catch (Exception exception)
            {
                inp.Listener.LogLocalWarning(exception.Message,inp.Field,attribute);
                return true;
            }
        }


    }
}
#endif
namespace Honey
{
    public enum HoneyPropertyState
    {
        Normal,
        Disabled,
        Hidden,
        Invisible
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HShowIfAttribute : HoneyAttribute
    {
            
       public string Name { get; }
        public HoneyPropertyState State { get;  }
        public HShowIfAttribute(string name,HoneyPropertyState state=HoneyPropertyState.Hidden)
        {
            Name = name;
            this.State = state;
        }
    
    }
  
}