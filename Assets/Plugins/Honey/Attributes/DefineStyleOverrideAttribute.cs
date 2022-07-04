#nullable enable
using Honey.Core;
using UnityEngine;
using System;


namespace Honey
{
    
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Struct, AllowMultiple = true)]
    public class DefineStyleOverrideAttribute : Attribute
    {
        
        public FontStyle FontStyle
        {
            get => NFontStyle?? default;
            set => NFontStyle = value;
        }

        public int FontSize
        {
            get => NFontSize??default;
            set => NFontSize = value;
        }

        public string ColorExpression
        {
            get => throw new NotSupportedException(); //attributes needs setters
            set => NColor = ColorExpressionParser.ParseColorExpression(value);

        }

        public TextAnchor Alignment
        {
            get => NAlignment??default;
            set => NAlignment = value;
        }
        public string? BaseStyle { get; set; }
        public FontStyle? NFontStyle { get; private set; }
        public int? NFontSize { get; private set; }
        public Color? NColor { get; private set; }
        public TextAnchor? NAlignment { get; private set; }
        public string NameCode { get; }
        

        public DefineStyleOverrideAttribute(string nameCode)
        {
            NameCode = nameCode;
          
        }

        public StyleOverride Build()
        {
            return new StyleOverride(NColor, NFontStyle, NFontSize,Alignment,BaseStyle);
        }
    }


}