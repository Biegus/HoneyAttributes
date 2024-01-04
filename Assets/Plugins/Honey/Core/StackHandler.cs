#nullable enable
using System;
using System.Collections.Generic;
using Honey.Core;
using UnityEngine;

namespace Honey.Core
{
    public class StackHandler<T>
    {
        private Action<T> change;
        private Func<T> get;
        private readonly Stack<T> stack=new();

        public StackHandler(Action<T> change, Func<T> get)
        {
            this.change = change?? throw new ArgumentNullException(nameof(change));
            this.get = get ?? throw new ArgumentNullException(nameof(get));
        }

        public void DummyPush()
        {
             Push(get());
        }
        public void Push(T val)
        {
            stack.Push(get());
            change(val);
        }

        public void Pop()
        {
            if (stack.Count == 0)
            {
                throw new InvalidOperationException("empty stack");
            }
            T result=stack.Pop();
            change(result);
        }


    }

    public static class StackGui
    {
        public static readonly StackHandler<Color> Color = new((value) => GUI.color = value, () => GUI.color);

        public static readonly StackHandler<Color> BackgroundColor =
            new((value) => GUI.backgroundColor = value, () => GUI.backgroundColor);

    }
}