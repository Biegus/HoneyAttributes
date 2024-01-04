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
            var maybeResult = cache.GetAndInvoke(inp.Container, attribute.Expr, inp.Field, inp.SerializedProperty);
            if (maybeResult.TryError(out string err))
            {
                inp.Listener.LogLocalWarning(err,inp.Field,attribute);
                return true;
            }
            return maybeResult.Unwrap();
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
            
       public string Expr { get; }
        public HoneyPropertyState State { get;  }
        public HShowIfAttribute(string expr,HoneyPropertyState state=HoneyPropertyState.Hidden)
        {
            Expr = expr;
            this.State = state;
        }
    
    }
  
}