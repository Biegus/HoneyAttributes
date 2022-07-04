#nullable enable
using UnityEngine;
namespace Honey.Core
{
    public class StyleOverride
    {
        public Color? Color { get; }
        public FontStyle? Style { get; }
        public int? FontSize { get; }
        public TextAnchor? Alignment { get; }
        public string? BaseStyle { get; }

        public void ApplyOn(ref GUIStyle style)
        {
            if (BaseStyle != null)
            {
                style = new GUIStyle(BaseStyle);
            }
            style.hover.textColor=  style.normal.textColor = Color??style.normal.textColor;
            style.fontStyle = Style??style.fontStyle;
            style.fontSize = FontSize?? style.fontSize;
            style.alignment = Alignment ?? style.alignment;
            
        }

        public StyleOverride(Color? color, FontStyle? style, int? fontSize, TextAnchor alignment, string? baseStyle)
        {
            Color = color;
            Style = style;
            FontSize = fontSize;
            Alignment = alignment;
            BaseStyle = baseStyle;
        }
    }
   
}