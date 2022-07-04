namespace Honey.Objects
{
    public static class UtilityHelper
    {
        public static bool IsAnyWayNull(object obj)
        {
            return obj == null || (obj is UnityEngine.Object unityObj && unityObj == null);
        }
        
    }
}