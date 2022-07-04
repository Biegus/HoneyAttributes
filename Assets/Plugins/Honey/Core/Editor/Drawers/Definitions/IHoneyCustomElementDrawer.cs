using System.Reflection;
using System;
namespace Honey.Editor
{
    public interface IHoneyCustomElementDrawer
    {
        void DrawPropertyLayout(PropertyInfo info,Attribute attributes,object container){}
        void DrawMethodLayout(MethodInfo info, Attribute attribute,object container){}
        void DrawNonSerializedFieldLayout(FieldInfo field, Attribute attribute,object container){}
    }
}