using System.Reflection;

namespace Honey.Core
{
    
    public struct HoneySerializedPropertyId
    {
        public object TargetRef;
        public FieldInfo Field;
        public int? Index;
        public override int GetHashCode()
        {
            var code= Field?.GetHashCode() ?? 261581512 ^ Index?.GetHashCode() ?? -41412334;
            code = code * -34143241 + TargetRef?.GetHashCode() ?? 64361561;
            return code;
        }

        public override bool Equals(object obj)
        {
            if (obj is HoneySerializedPropertyId p)
            {
                return TargetRef == p.TargetRef && p.Field == Field && p.Index == Index;
            }

            return false;
        }
        public static bool operator==(HoneySerializedPropertyId a, HoneySerializedPropertyId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HoneySerializedPropertyId a, HoneySerializedPropertyId b)
        {
            return !(a == b);
        }

        public HoneySerializedPropertyId(FieldInfo field, object targetRef, int? index)
        {
            Field = field;
            TargetRef = targetRef;
            Index = index;
        }
    }
}