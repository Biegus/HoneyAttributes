#nullable enable
using Honey;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

namespace Honey.Editor
{
    public class MinMaxDrawerFactory
    {
        public static HMinMaxDrawer BuildMin()
        {
            return new HMinMaxDrawer(HoneyMathHelper.Max, HoneyMathHelper.Max, Mathf.Max);
        }
        public static HMinMaxDrawer BuildMax()
        {
            return new HMinMaxDrawer(HoneyMathHelper.Min, HoneyMathHelper.Min, Mathf.Min);
        }
    }
    public class HMinMaxDrawer : IHoneyAdditionalDrawer
    {

        public bool RequestHierarchyQuery => false;
        private readonly Func<long, long, long> longDecider;
        private readonly Func<ulong, ulong, ulong> ulongDecider;
        private readonly Func<float, float, float> floatDecider;
        public HMinMaxDrawer(Func<long,long,long> longDecider, Func<ulong,ulong,ulong> ulongDecider, Func<float,float,float> floatDecider)
        {
            this.longDecider = longDecider;
            this.ulongDecider = ulongDecider;
            this.floatDecider = floatDecider;
        }
        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            InternalBaseMinMaxAttribute atr = (attribute as InternalBaseMinMaxAttribute)!;
            switch (inp.SerializedProperty.propertyType)
            {
                case SerializedPropertyType.Integer when inp.Obj?.GetType() == typeof(ulong):
                    inp.SerializedProperty.ulongValue = ulongDecider(atr.UlongValue, inp.SerializedProperty.ulongValue);
                    break;
                case SerializedPropertyType.Integer:
                    inp.SerializedProperty.longValue = longDecider(atr.LongValue, inp.SerializedProperty.longValue);
                    break;
                case SerializedPropertyType.Float:
                    inp.SerializedProperty.floatValue =floatDecider(atr.LongValue, inp.SerializedProperty.floatValue);
                    break;
                default:
                    inp.Listener.LogLocalWarning(
                        $"type {(inp.Obj?.GetType().Name ?? "Unknown type")} is not supported.", inp.Field, atr);
                    break;
            }
        }
    }
}
#endif

namespace Honey
{
    public abstract  class InternalBaseMinMaxAttribute : HoneyAttribute
    {
        public long LongValue { get; } = long.MinValue;
        public ulong UlongValue { get; } = 0;
        public float FloatValue { get; }

        public InternalBaseMinMaxAttribute(long value)
        {
            LongValue = value;
            if (value >= 0)
                UlongValue =(ulong) value;
            FloatValue = value;
        }

        public InternalBaseMinMaxAttribute(ulong value)
        {
            UlongValue = value;
            if (value <= long.MaxValue)
            {
                LongValue = (long) value;
            }

            FloatValue = value;
        }
        public InternalBaseMinMaxAttribute(float value)
        {
            FloatValue = value;
            if (value is >= 0 and <= ulong.MaxValue)
                UlongValue =(ulong) value;
            if (value is >= long.MinValue and <= long.MaxValue)
            {
                LongValue =(long) value;
            }

        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]

    public class HMinAttribute : InternalBaseMinMaxAttribute
    {
        public HMinAttribute(long value) : base(value)
        {
        }

        public HMinAttribute(ulong value) : base(value)
        {
        }

        public HMinAttribute(float value) : base(value)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HMaxAttribute : InternalBaseMinMaxAttribute
    {
        public HMaxAttribute(long value) : base(value)
        {
        }

        public HMaxAttribute(ulong value) : base(value)
        {
        }

        public HMaxAttribute(float value) : base(value)
        {
        }
    }
    
#if UNITY_EDITOR
#endif

}