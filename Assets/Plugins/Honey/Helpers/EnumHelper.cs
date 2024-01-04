using System;

namespace Honey.Helper
{
    public static class EnumHelper
    {
        public static T If<T>(this T flag,bool cond)
            where T : Enum
        {
            int raw = (flag as IConvertible).ToInt32(null);
            int result = raw & (cond ? ~0 : 0);
            return (T)Enum.ToObject(typeof(T),  result);
        }
    }
}