using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Honey.Core
{
    public struct HoneyNamedEl
    {
        public string Name { get; }
        public object Value { get; }

        public HoneyNamedEl(object value, string name)
        {
            Name = name;
            Value = value;
        }
        
    }

    public class HoneyNamedElHelper
    {
        public static (GUIContent [] names, IEnumerable values) SplitObjectArray(IEnumerable values)
        {
            GUIContent[] names = null;
            if (values is IEnumerable<HoneyNamedEl> namedElements)
            {
                var honeyNamedEls = namedElements as IReadOnlyList<HoneyNamedEl> ?? namedElements.ToArray();
                names = honeyNamedEls.Select(item => new GUIContent( item.Name)).ToArray();
                values = honeyNamedEls.Select(item => item.Value).ToArray();
            }

            return (names, values);

        }
    }
}