
using System;

#nullable enable
namespace Honey.Helper
{
    public static class UtilityHelper
    {
        public static bool IsAnyWayNull(object? obj)
        {
            return obj == null || obj is UnityEngine.Object unityObj && unityObj == null;
        }

        public static T  ToNonNullable<T>(this T? obj,string? message=null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message??"");
            }

            return obj;
        }



    }
}