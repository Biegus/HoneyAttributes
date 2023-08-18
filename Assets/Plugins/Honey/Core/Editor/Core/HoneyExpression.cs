#if UNITY_EDITOR
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Honey.Core;
using Honey.Editor;

using UnityEngine;

namespace Honey.Editor
{

 
    public class HoneyValueParser
    {
        
        private static Regex comparsionRegex =
            new Regex(
                @"(^(?<left>^[^=><!]*?)(?<operand>[=><!+\-*\/]{1,2})(?<right>[^=><!\n]*)$)|(^((?<left_single>[^=><!]*?$)))");

        private static Regex argumentsRegex =
            new Regex(
                @"(?<=,|\()(.*?)(?=,|\))");

        private static Regex methodNameRegex
            = new Regex(
                @".*?(?=\()");

        public static Func<object, object> ParseExpression(string text, Type type, string itself, HoneyValueParseFlags flags)
        {
            text = text.Replace("itself", itself);
          
            
            var func= InternalParseExpression(text, type,flags);
            if (flags.HasFlag(HoneyValueParseFlags.StringMode))
                return (o) => func(o).ToString();
            else return func;
        }

        private static ArgumentException GetElementNotFoundException()
        {
             return new ArgumentException($"Honey expression is incorrect: element not found");
        }
        private static Func<object,object> InternalParseExpression(string  text, Type type, HoneyValueParseFlags flags)
        {
            Func<object, object> HandleSimpleSingle(string single, HoneyValueParseFlags innerFlags)
            {
                if (text[0] == '!')
                    return Negate(HandleSimpleSingle(single[1..],innerFlags));
                var getter = HoneyReflectionUtility.CreateGeneralGetter<object>(single, type);
                if (getter != null)
                    return getter;
                if (single.ToLower() == "true")
                {
                    return (_) => true;
                }
                else if (single.ToLower() == "false")
                {
                    return (_) => false;
                }
                if (innerFlags.HasFlag( HoneyValueParseFlags.IntegerMode))
                {
                    if (long.TryParse(single, NumberStyles.Integer, CultureInfo.InvariantCulture,out long parsed ))
                    {
                        return _ => parsed;
                    }
                    throw GetElementNotFoundException();
                }
                else
                {
                    if (float.TryParse(single, NumberStyles.Integer, CultureInfo.InvariantCulture,out float parsed ))
                    {
                        return _ => parsed;
                    }
                    throw GetElementNotFoundException();
                }

            }

            Func<object, object> HandleAdvSingle(string single)
            {
                if (text[0] == '!')
                    return Negate(HandleAdvSingle(single[1..]));
                string methodName = methodNameRegex.Match(single).Value;
                if (single.Contains("(") && !single.Contains("()"))
                {
                    var args = argumentsRegex.Matches(single);
                    MethodInfo methodInfo = type.GetMethods(
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .FirstOrDefault(item=>args.Count==item.GetParameters().Length && item.Name==methodName);
                    if (methodInfo==null)
                    {
                        throw GetElementNotFoundException();
                    }
                    Func<object, object> a=null,b=null;
                    if (args.Count < 1)
                    {
                        return null;
                    }   
                    if (args.Count >= 1)
                    {
                        a = HandleSimpleSingle( args[0].Value,HoneyValueParseFlags.None);
                        
                    }
                    if (args.Count == 1)
                    {
                       
                        Func<object,object,object> delg = HoneyReflectionUtility.CastFunc<object, object, object>(methodInfo);
                        var paramType = methodInfo.GetParameters()[0].ParameterType;
                        Func<object, object> res = (obj) => delg.Invoke(obj,Convert.ChangeType(a(obj),paramType));
                        return res;
                    }
                    else if(args.Count==2)
                    {
                        b = HandleSimpleSingle( args[1].Value,HoneyValueParseFlags.None);
                        var p = methodInfo.GetParameters();
                        Func<object,object,object,object> delg = HoneyReflectionUtility.CastFunc<object, object, object, object>(methodInfo);

                        Func<object, object> res = (obj) => delg.Invoke(obj,
                            Convert.ChangeType(a(obj), methodInfo.GetParameters()[0].ParameterType),
                            Convert.ChangeType(b(obj), methodInfo.GetParameters()[1].ParameterType));
                        return res;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return HandleSimpleSingle(single.Replace("()",""),flags);
                }
            }

            Func<object, object> Negate(Func<object, object> f)
            {
                return (o) => !(bool) f(o);
            }
            if (string.IsNullOrEmpty(text))
                return null;
            if (text[0] == '!')
            {
                return Negate(InternalParseExpression(text[1..], type,flags));
            }
            var cmpr = comparsionRegex.Match(text);
            if (!cmpr.Success)
                return null;
            var leftSingle = cmpr.Groups["left_single"].ToString();
            
            if (leftSingle != string.Empty)
            {
                return HandleAdvSingle(leftSingle);
            }
            else
            {
                Func<object, object> left = HandleAdvSingle(cmpr.Groups["left"].Value);
                Func<object, object> simpleRight = HandleAdvSingle(cmpr.Groups["right"].Value);
                Func<object,object> right = (a) => Convert.ChangeType(simpleRight(a), left(a).GetType());
                Func<object,object> res= cmpr.Groups["operand"].Value switch
                {
                    "=" => (o) => left(o).Equals(right(o)),
                    "==" => (o) => left(o).Equals( right(o)),
                    ">" => (o) => (left(o) as IComparable).CompareTo(right(o)) == 1,
                    "<" => (o) => (left(o) as IComparable).CompareTo(right(o)) == -1,
                    ">=" => (o) => (left(o) as IComparable).CompareTo(right(o)) > 0,
                    "<=" => (o) => (left(o) as IComparable).CompareTo(right(o)) < 0,
                    "!=" => (o) => !left(o).Equals(right(o)),
                  
                   
                    _ => null

                };
                if (res != null)
                    return res;


                Func<Func<double, double,object>, Func<object, object>> doIt = (operation) =>
                    (o) => operation(Convert.ToDouble(left(o)), Convert.ToDouble(right(o)));
                return cmpr.Groups["operand"].Value switch
                {
                    "+" => doIt((a, b) => a + b),
                    "-" => doIt((a, b) => a - b),
                    "/" => doIt((a, b) => a / b),
                    "*" => doIt((a, b) => a * b),
                    _ => null
                };



            }

           
        }

        public static void ParseStyleExpression(string text, Type type,ref GUIStyle style)
        {
            var o= HoneyHandler.HoneyReflectionCache.GetStyleOverrides(type);
            if (string.IsNullOrEmpty(text))
                return;
            if (!o.ContainsKey(text))
            {
                if (text[0] != '_')
                    style = new GUIStyle(text);
            }
            var res = o[text];
            res.ApplyOn(ref style);
        }
        
    }
}
#endif