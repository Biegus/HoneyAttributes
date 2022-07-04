#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Honey.Core;
using Honey.Editor;
using UnityEngine;

namespace Honey.Editor
{
    public static class HoneyTextParser
    {
        public static Regex regRegex = new Regex(@"&[^ ,\n]*?(?=([ ,\n]|$))", RegexOptions.Compiled);
        public static Func< object, string> Parse(string expression, Type containerType, string itself)
        {
            var matches = regRegex.Matches(expression);
            List<Func<object, object>> getters = new(matches.Count);
            foreach (Match match in matches)
            {
                getters.Add( HoneyValueParser.ParseExpression(match.Value[1..], containerType,itself,HoneyValueParseFlags.StringMode));
            }

           
            return (o) =>
            {
                string text = expression;
                int i = 0;
                int dif = 0;
                foreach (Match match in matches)
                {
                    string nwValue = getters[i++](o).ToString();
                    int index = match.Index + dif;
                    text=text.Remove(index, match.Length);
                    text=text.Insert(index, nwValue);
                    dif += (nwValue.Length - match.Length);
                }
                return text;
            };

        }
    }
}
#endif