#nullable enable
using Honey;
using Honey.Core;
using UnityEngine;
using System;

#if UNITY_EDITOR
using Honey.Editor;
using Honey.Validation;
using UnityEditor;

namespace Honey.Editor
{
    public class HValidateDrawer : IHoneyAdditionalDrawer
    {
        public bool RequestHierarchyQuery => true;

        private HoneyValueExpressionGetterCache<bool> cache = new();
        public void Before(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {
            var atr = (attribute as HValidateAttribute)!;
            if (HoneyReflectionUtility.FlattenCollectionType(inp.Field.FieldType!) != inp.Field.FieldType)
            {
                inp.Listener.LogLocalWarning("HValidate doesn't support lists/arrays",inp.Field,atr);
                return;
            }

            var maybeRes = cache.Get(atr.BoolExpression, inp.Field, inp.SerializedProperty);
            if (maybeRes.TryError(out var error))
            {
                inp.Listener.LogLocalWarning(error, inp.Field, attribute);
                StackGui.BackgroundColor.DummyPush();
                return;
            }
            var res = maybeRes.Unwrap()(inp.Container);

            if (!res)
            {
                StackGui.BackgroundColor.Push(new Color32(210, 43, 43, 255));
            }
            else StackGui.BackgroundColor.DummyPush();
        }

        public void After(in HoneyDrawerInput inp, Rect rect, HoneyAttribute attribute)
        {

            if (HoneyReflectionUtility.FlattenCollectionType(inp.Field.FieldType!) == inp.Field.FieldType)
                StackGui.BackgroundColor.Pop();
        }
    }
}
#endif

namespace Honey
{

    /// <summary>
    /// If field doesn't met requirments it will be colored red.
    /// </summary>
    /// <example>
    /// `itself>5`
    /// `itself>someOtherVarariable`
    /// `itself!=someOtherVariable`
    /// `FunctionReturningBool(itself)`
    /// </example>
    public class HValidateAttribute : HoneyAttribute
    {
            public string BoolExpression { get; }

            public HValidateAttribute(string boolExpression)
            {
                BoolExpression = boolExpression;
            }
    }

}