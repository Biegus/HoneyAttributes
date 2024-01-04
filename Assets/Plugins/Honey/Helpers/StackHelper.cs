#nullable  enable
using System;
using System.Collections.Generic;

namespace Honey.Helper
{
    public static class StackHelper
    {
            public static T PeekOr<T>(this Stack<T> stack,T def)
            {
                if (stack == null) throw new ArgumentNullException();
                return stack.TryPeek(out T result) ? result : def;
            }

            public static T PeekOrElse<T>(this Stack<T> stack, Func<T> func)
            {
                if (stack == null) throw new ArgumentNullException();
                return stack.TryPeek(out T result) ? result : func();

            }
    }
}