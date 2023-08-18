#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Honey.Core
{
   public static class HoneyReflectionUtility
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                           BindingFlags.Static;
        /// <summary>
        /// will consider properties, fields, methods
        /// </summary>
        public static Func<object,T>? CreateGeneralGetter<T>(string name, Type type)
        {
            FieldInfo? field = type.GetField(name,Flags);
            if (field != null)
                return CreateFieldGetter<T>(field);
            PropertyInfo? propertyInfo = type.GetProperty(name, Flags);
            if (propertyInfo != null)
                return (Func<object, T>) CastFunc<object,T>(propertyInfo.GetMethod);
            MethodInfo? method = type.GetMethods( Flags).FirstOrDefault(item=>item.GetParameters().Length==0 && item.Name==name);
            if (method != null)
            {
             
                Func<object, object> delg = HoneyReflectionUtility.CastFunc<object, object>(method);
                return (obj) => (T)delg.Invoke(obj);
            }
            return null;
        }
        public static FieldInfo? GetFieldIncludingInheritances(Type type, string name)
        {
            Type? inner = type;
            while (inner != null)
            {
                var f = inner.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f;
                inner = inner.BaseType;
            }
            return null;
        }
        public static Func<object,TOut> CreateFieldGetter<TOut>(FieldInfo field)
        {
            
            string methodName = field.ReflectedType!.FullName + ".get_" + field.Name;
            DynamicMethod setterMethod = new(methodName, typeof(TOut), new[] {typeof(object)}, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Isinst,field.ReflectedType);
             
            gen.Emit(OpCodes.Ldfld, field);
            if (field.FieldType.IsValueType&& !typeof(TOut).IsValueType)
                gen.Emit(OpCodes.Box, field.FieldType);
            gen.Emit(OpCodes.Ret);
            
            return (Func<object, TOut>)setterMethod.CreateDelegate(typeof(Func<object, TOut>));
        }


        public static bool IsInteger(Type type)
        {
            return type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong);
        }

        public static Type GetBaseTypeOfArrayListOrItself(Type type)
        {
            if (!type.IsGenericType)
                return type;
            if(type.GetGenericTypeDefinition()==typeof(List<>) || typeof(Array).IsAssignableFrom(type))
            {
                Type? listInterface = type.GetInterfaces()
                    .FirstOrDefault(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IList<>));
                if (listInterface == null)
                    return type;
                return listInterface.GetGenericArguments()[0];
            }
            else return type;

        }

        private static Func<TN1, TN2> CastFuncInternalTwo<TO1, TO2, TN1, TN2>(Func<TO1, TO2> func)
        {
            return (TN1 a) => (TN2) (object) func((TO1) (object) a!)!;
        }
        private static Func<TN1, TN2,TN3> CastFuncInternalThree<TO1, TO2,TO3, TN1, TN2,TN3>(Func<TO1, TO2,TO3> func)
        {
            return (TN1 a, TN2 b) => (TN3) (object) func((TO1) (object) a!, (TO2) (object) b!)!;
        }
        private static Func<TN1, TN2,TN3,TN4> CastFuncInternalFour<TO1, TO2,TO3,TO4, TN1, TN2,TN3,TN4>(Func<TO1, TO2,TO3,TO4> func)
        {
            return (TN1 a, TN2 b,TN3 c) => (TN4) (object) func((TO1) (object) a!, (TO2) (object) b!,(TO3)(object)c!)!;
        }

        private static MethodInfo CastInternalMethodTwo =
            typeof(HoneyReflectionUtility).GetMethod(nameof(CastFuncInternalTwo),
                BindingFlags.NonPublic | BindingFlags.Static)!;
        private static MethodInfo CastInternalMethodThree =
            typeof(HoneyReflectionUtility).GetMethod(nameof(CastFuncInternalThree),
                BindingFlags.NonPublic | BindingFlags.Static)!;
        private static MethodInfo CastInternalMethodFour =
            typeof(HoneyReflectionUtility).GetMethod(nameof(CastFuncInternalFour),
                BindingFlags.NonPublic | BindingFlags.Static)!;

        
        public static Func<T1, T2> CastFunc<T1,T2>(Delegate @delegate, Type a, Type b)
        {
                return (Func<T1,T2>) CastInternalMethodTwo.MakeGenericMethod(a, b,typeof(T1), typeof(T2))
                .Invoke(null, new object[] {@delegate});
        }
        public static Func<T1, T2> CastFunc<T1,T2>(MethodInfo info)
        {
            (Type a, Type b) = (info.DeclaringType!,  info.ReturnType);
            return CastFunc<T1, T2>(info.CreateDelegate(typeof(Func<,>).MakeGenericType(a, b)), a, b);
        }
        
        public static Func<T1, T2,T3> CastFunc<T1,T2,T3>(Delegate @delegate, Type a, Type b, Type c)
        {
            return (Func<T1,T2,T3>) CastInternalMethodThree.MakeGenericMethod(a, b, c,typeof(T1), typeof(T2),typeof(T3))
                .Invoke(null, new object[] {@delegate});
        }
        public static Func<T1, T2,T3> CastFunc<T1,T2,T3>(MethodInfo info)
        {
            (Type a, Type b, Type c) = (info.DeclaringType!, info.GetParameters()[0].ParameterType, info.ReturnType);
            return CastFunc<T1, T2,T3>(info.CreateDelegate(typeof(Func<,,>).MakeGenericType(a, b,c)), a, b,c);
        }
        public static Func<T1, T2,T3,T4> CastFunc<T1,T2,T3,T4>(Delegate @delegate, Type a, Type b, Type c, Type d)
        {
            return (Func<T1,T2,T3,T4>) CastInternalMethodFour.MakeGenericMethod(a, b, c,d,typeof(T1), typeof(T2),typeof(T3),typeof(T4))
                .Invoke(null, new object[] {@delegate});
        }
      
        public static Func<T1, T2,T3,T4> CastFunc<T1,T2,T3,T4>(MethodInfo info)
        {
            (Type a, Type b, Type c,Type d) = (info.DeclaringType!, info.GetParameters()[0].ParameterType,info.GetParameters()[1].ParameterType, info.ReturnType);
            return CastFunc<T1, T2,T3,T4>(info.CreateDelegate(typeof(Func<,,,>).MakeGenericType(a, b,c,d)), a, b,c,d);
        }
    }
}