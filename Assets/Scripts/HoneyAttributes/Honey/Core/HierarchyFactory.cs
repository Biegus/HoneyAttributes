using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Reflection.Emit;
using Honey.Editor;
using Honey.Helper;
using UnityEngine;
#nullable enable
namespace Honey.Core
{

    public class HierarchyQuery
    {
        private readonly Func<object, object>[] funcs;
        public Type FinalType { get; }
        public HierarchyQuery(Func<object,object>[] list, Type finalType)
        {
            if (list.Length == 0)
            {
                throw new InvalidOperationException("Func list was empty");
            }
            funcs = list;
            FinalType = finalType;
        }

        public object Retrieve(int start, int count, object obj)
        {
            for (int i = start; i < start + count; i++)
            {
                obj = funcs[i](obj);
            }

            return obj;
        }
        public (object? final, object container) RetrieveTwoLast(object obj)
        {
            object? before = null;
            for (int index = 0; index < funcs.Length; index++)
            {
                var func = funcs[index];
                before = obj;
                obj = func(obj);
            }

           
            return (obj,before!); //len(funcs)>0
        }

        public object? RetrieveLast(object obj)
        {
            return RetrieveTwoLast(obj).final;
        }
        
    }
  
    public class HierarchyFactory
    {
        private static readonly Regex indexRegex = new Regex(@"^(?<name>.*?)\[(?<value>\d+)\]");

        public struct HierarchyElement
        {
            public int? Index;
            public FieldInfo Field;
            
        }
        
       
        public static HierarchyQuery BuildHierarchyQuery(string path, Type type)
        {
            List<Func<object, object>> functions = new List<Func<object, object>>();
            Type finalType = type;
            foreach (var element in GetHierarchy(path,type,0))
            {
                Func<object, object> getter;
                getter= HoneyReflectionUtility.CreateFieldGetter<object>(element.Field);
                finalType = element.Field.FieldType;
                if (element.Index == null)
                {
                    functions.Add(getter);
                }
                else
                {

                    finalType = element.Field.FieldType.GetInterfaces().Where(item =>
                            item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .First().GenericTypeArguments[0];
                    functions.Add((v) =>
                    {
                        object value = getter(v);
                        return EnumerableHelper.GetAtIndex(value as IEnumerable, element.Index.Value);
                    });
                }
            }

            return new HierarchyQuery(functions.ToArray(),finalType);
        }
        public static IEnumerable<HierarchyElement> GetHierarchy(string propPath,Type type,int stepsBack=1)
        {
            void ThrowInvalidPath()
            {
                throw new ArgumentException(
                    "Path was invalid, probably property was inside SerializeReference. This is not supported by GetHierarchy");
            }

            propPath= propPath.Replace(".Array.data[", "[");
      
            var elements = propPath.Split('.');
            foreach (var element in elements.Take(elements.Length-stepsBack))
            {
                
                if (element.Contains("["))
                {
                    var match = indexRegex.Match(element);
                    int index = int.Parse(match.Groups["value"].Value);
                    string name = match.Groups["name"].Value;
                    FieldInfo? field = HoneyReflectionUtility.GetFieldIncludingInheritances(type, name);
                    if (field == null)
                    {
                        ThrowInvalidPath();
                        yield break;
                    }
                    yield return new HierarchyElement()
                    {
                        Field = field,
                        Index = index
                    };
                    Type? interfaceType = field.FieldType
                        .GetInterfaces().FirstOrDefault(item => item.IsGenericType&&item.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (interfaceType == null)
                    {
                        throw new ArgumentException("Path marks an array at a point that doesn't have it");
                    }
                    type = interfaceType.GetGenericArguments()[0];

                }
                else
                {
                    FieldInfo? field = HoneyReflectionUtility.GetFieldIncludingInheritances(type, element);
                    if (field == null)
                    {
                        ThrowInvalidPath();
                        yield break;
                    }
                    yield return new HierarchyElement()
                        {Field = field};
                    type = field.FieldType;

                }
            }
        }
        
    

    }
}