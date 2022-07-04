using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Honey.Core
{
    public static class ColorExpressionParser
    {
        private static readonly Dictionary<string, Color> hardCodedColorCodes;

        static ColorExpressionParser()
        {
            
            hardCodedColorCodes=typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public)
                .Where(item=>item.PropertyType==typeof(Color))
                .ToDictionary(item => item.Name.ToLower(), item =>(Color) item.GetValue(null));
        }
        private static Regex rgbfRegex =
            new Regex(
                @"^[Rr][Gg][Bb][Ff]\((?<r>[10]{0,1}(\.\d*){0,1}),(?<g>[10]{0,1}(\.\d*){0,1}),(?<b>[10]{0,1}(\.\d*){0,1})\)$");

        private static Regex rgbRegex =
            new Regex(@"^[Rr][Gg][Bb]\((?<r>\d{1,3}),(?<g>\d{1,3}),(?<b>\d{1,3})\)$");
        
        public static Color? ParseColorExpression(string colorCode)
        {
            var match = rgbfRegex.Match(colorCode);
            if (match.Success)
            {
                return new Color(float.Parse(match.Groups["r"].Value),
                    float.Parse(match.Groups["g"].Value),
                    float.Parse(match.Groups["b"].Value),
                    1
                );
            }

            match = rgbRegex.Match(colorCode);
            if (match.Success)
            {
                return new Color32(byte.Parse(match.Groups["r"].Value), byte.Parse(match.Groups["g"].Value),
                    byte.Parse(match.Groups["b"].Value), 255);
            }
           
            if (hardCodedColorCodes.TryGetValue(colorCode.ToLower(), out Color res))
                return res;
            return null;
        }
    }
}