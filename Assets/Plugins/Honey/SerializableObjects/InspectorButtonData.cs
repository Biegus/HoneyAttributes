#nullable enable
using System;
using UnityEngine;

namespace Honey.Objects
{
    public enum InspectorButtonSize
    {
        Normal,
        Medium,
        Thick
    }

    [Flags]
    public enum InspectorButtonLimits
    {
        NoLimits=0,
        NotInPlayMode=1<<0,
        NotInEditorMode=1<<1,
        NotIfObjectIsNotEnabled=1<<2,
        NotIfObjectIsEnabled=1<<3,
    }
    public static class InspectorButtonLimitsHelper{
        public static bool CheckIfFails(this InspectorButtonLimits flags, bool isPlaying, bool isActive)
        {
            return (flags.HasFlag(InspectorButtonLimits.NotInEditorMode) && !isPlaying)
                    || (flags.HasFlag(InspectorButtonLimits.NotInPlayMode) && isPlaying)
                    || (flags.HasFlag(InspectorButtonLimits.NotIfObjectIsEnabled) && isActive)
                    || (flags.HasFlag(InspectorButtonLimits.NotIfObjectIsNotEnabled) && !isActive);
        }
    }
    public class InspectorButtonDataAttribute: PropertyAttribute
    {
        public string Name { get; }
        public string Method { get; }
        public InspectorButtonSize Size { get; }
        public  InspectorButtonLimits Limit { get; set; }
        public string? ArgumentDescription { get; set; } = null;
        public InspectorButtonDataAttribute(string name,  string method, InspectorButtonSize size)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Size = size;
        }
    }
}