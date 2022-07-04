#nullable enable
using System.Linq;
using System.Reflection;
using Honey;
using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using Honey.Core;
using Honey.Editor;
namespace Honey.Editor
{
    public class HDynamicContentDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;
        private Dictionary<HoneySerializedPropertyId, Func<object, string,object?, string>> funcCache = new();

        private static Func<object, string,object?, string> Convert<TContainer>(Func<TContainer, string,object?, string> func)
        {
            return (obj, t,t2) => func((TContainer) obj, t,t2);
        }
           
        public GUIContent GetCustomContent(in HoneyDrawerInput inp, HoneyAttribute attribute, GUIContent content)
        {
            var atr = (attribute as HDynamicContentAttribute)!;
            HoneySerializedPropertyId key = new HoneySerializedPropertyId(inp.Field, inp.Container,
                SerializedPropertyHelper.GetIndexOfSerializedPropertyPath(inp.SerializedProperty.propertyPath));
            if (funcCache.TryGetValue(key, out var result))
            {
                content.text = result.Invoke(inp.Container, content.text,inp.Obj);
            }
            else
            {
                var method = inp.Container.GetType()
                    .GetMethod(atr.MethodName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (method == null)
                {
                    content.text = $"Method {atr.MethodName} was not found";
                    return content;
                }
                else if (!method.GetParameters().Select(item => item.ParameterType).SequenceEqual(new[] {typeof(string), typeof(object)}) || method.ReturnType!=typeof(string) )
                {
                    content.text = "Method definition was wrong";
                    return content;
                }
                Delegate? lowLevelDelg= method.CreateDelegate(typeof(Func<,,,>).MakeGenericType(inp.Container.GetType(),typeof(string),typeof(object),typeof(string)));
                       
                   
                var final=(Func<object,string,object?,string>) (typeof(HDynamicContentDrawer).GetMethod(nameof(Convert), BindingFlags.NonPublic | BindingFlags.Static))!
                    .MakeGenericMethod(inp.Container.GetType()).Invoke(null, new object[] {lowLevelDelg});
                    
                content.text = final.Invoke(inp.Container, content.text,inp.Obj);
                funcCache[key] = final;
            }
                
            return content;
        }
    }
}
#endif

namespace Honey
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class HDynamicContentAttribute : HoneyAttribute
    {
      
        public string MethodName { get; }

        /// <summary>
        /// Method should follow: string NAME(string name, object value)
        /// </summary>
        public HDynamicContentAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
#if UNITY_EDITOR
#endif

}