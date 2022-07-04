using System;
#if UNITY_EDITOR
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class ETabElementAttribute : Attribute
    {
        public string TabName { get; }
       
        public ETabElementAttribute( string tabName)
        {
            TabName = tabName;
          
        }

    }


}