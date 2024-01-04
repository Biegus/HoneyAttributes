using System;

namespace Honey
{
    
    public class HoneyAttribute : Attribute
    {
        /// <summary>
        /// Default is 0.
        /// It has the same effect as reordering attributes.
        /// Doesn't affect main drawers.
        /// If one attribute draws in PreBefore and second in Before, order won't make the before one first.
        /// </summary>
        public int Order { get; init; } = 0;
    }
}