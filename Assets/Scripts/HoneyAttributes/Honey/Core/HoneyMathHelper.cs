namespace Honey
{
    public class HoneyMathHelper
    {
        public static ulong Max(ulong a, ulong b)
        {
            return (a > b) ? a : b;
        }  
        public static long Max(long a, long b)
        {
            return (a > b) ? a : b;
        }  
        public static ulong Min(ulong a, ulong b)
        {
            return (a < b) ? a : b;
        }  
        public static long Min(long a, long b)
        {
            return (a < b) ? a : b;
        }  
    }
}