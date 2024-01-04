#if UNITY_EDITOR
#nullable enable
using System.Reflection;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Honey.Editor
{
    public interface IHoneyErrorListener
    {
         void LogError(string text,MemberInfo? member, Attribute? attribute);
         void LogLocalWarning(string text,MemberInfo? member, Attribute? attribute);
         void LogGlobalWarning(string text,MemberInfo? member, Attribute? attribute);

         void MaybeLogLocalWarning(string text, MemberInfo? member, Attribute? attribute)
         {
         }
    }

    public class DummyListener : IHoneyErrorListener
    {

        public static DummyListener Instance { get; } = new DummyListener();
        public void LogError(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            Debug.LogError(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));
        }

        public void LogLocalWarning(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            Debug.LogWarning(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));

        }

        public void LogGlobalWarning(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            Debug.LogWarning(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));

        }
    }

    public static class HoneyErrorListenerStack
    {
        private static Stack<IHoneyErrorListener> stack = new Stack<IHoneyErrorListener>();

        public static void Push(IHoneyErrorListener element)
        {
            stack.Push(element);
        }

        public static IHoneyErrorListener Peek()
        {
            return stack.Peek();
        }

        public static bool IsEmpty()
        {
            return stack.Count == 0;
        }
        public static void Pop()
        {
            stack.Pop();
        }
        public static IHoneyErrorListener GetListener()
        {
            return
                HoneyErrorListenerStack.IsEmpty()
                    ? DummyListener.Instance
                    : HoneyErrorListenerStack.Peek();
            
        }

    }
    public static class HoneyErrorListenerHelper
    {
        public static string BuildMessage(string text, MemberInfo? memberInfo, Attribute? attribute)
        {
            if (memberInfo == null && attribute == null)
                return text;
            else if (attribute == null && memberInfo!=null)
                return $"{memberInfo.Name}->{text}";
            else if (memberInfo == null && attribute!=null)
                return $"[{attribute.GetType().Name}]->{text}";
            else
                return $"[{attribute!.GetType().Name}] {memberInfo!.DeclaringType!.Name} {memberInfo.Name}->{text}";
        }
    }

    public class HoneyErrorListener : IHoneyErrorListener
    {
        private HoneyLogger ErrorLogger { get; }
        private HoneyLogger LocalWarnLogger { get; }
        private HoneyLogger GlobalWarnLogger { get; }

        public HoneyErrorListener(HoneyLogger errorLogger, HoneyLogger localWarnLogger, HoneyLogger globalWarnLogger)
        {
            ErrorLogger = errorLogger;
            LocalWarnLogger = localWarnLogger;
            GlobalWarnLogger = globalWarnLogger;
        }
        
        public void LogError(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            ErrorLogger.Log(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));
        }

        public void MaybeLogLocalWarning(string text, MemberInfo? member, Attribute? attribute)
        {
             LogLocalWarning(text,member,attribute);
        }

        public void LogLocalWarning(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            LocalWarnLogger.Log(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));
        }

        public void LogGlobalWarning(string text, MemberInfo? memberInfo = null, Attribute? attribute = null)
        {
            GlobalWarnLogger.Log(HoneyErrorListenerHelper.BuildMessage(text,memberInfo,attribute));
        }
    }
}
#endif