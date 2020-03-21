using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Msdfa.Core.Tools.Extensions
{
    public static class StringExtension
    {
        public static string SubstringPadded(this string text, int startIndex, int length, bool fillToWidth = true)
        {
            if (text == null) return new string(' ', length);

            if (text.Length >= startIndex)
            {
                if (text.Length >= startIndex + length) return text.Substring(startIndex, length);
                return fillToWidth ? text.Substring(startIndex).PadRight(length) : text.Substring(startIndex);
            }
            return fillToWidth ? new string(' ', length) : "";
        }

        public static string SubstringSafe(this string text, int startIndex, int length)
        {
            if (text == null) return text;

            if (text.Length >= startIndex)
            {
                if(text.Length >= startIndex + length)
                    return text.Substring(startIndex, length);
                else
                    return text.Substring(startIndex);
            }

            return null;
        }

        public static string TrimAt(this string text, int length)
        {
            if (text.Length <= length) return text;
            return text.Substring(0, length);
        }

        public static List<string> SplitByLength(this string text, int width, bool fillToWidth = true)
        {
            var tempText = new List<string>();
            if (text.Length > width)
            {
                do
                {
                    tempText.Add(text.Substring(0, width));
                    text = text.Remove(0, width).TrimStart();
                } while (text.Length > width);
            }
            tempText.Add(fillToWidth ? text.PadRight(width) : text);
            return tempText;
        }

        public static bool TryGetBoolValue(this string txt)
        {
            var yesString = "Tak";
            var noString = "Nie";

            if (txt.Equals(yesString, StringComparison.OrdinalIgnoreCase) || txt == "1" || txt == "T" || txt == "t")
                return true;

            if (txt.Equals(noString, StringComparison.OrdinalIgnoreCase) || txt == "0" || txt == "N" || txt == "n")
                return false;

            throw new FormatException();
        }

        public static string TrimStart(this string text, string prefix)
        {
            return prefix != null && text.StartsWith(prefix)
                ? text.Substring(prefix.Length)
                : text;
        }

        public static string TrimEnd(this string text, string suffix)
        {
            return suffix != null && text.EndsWith(suffix)
                ? text.Substring(0, text.Length - suffix.Length)
                : text;
        }

        public static string EncodeToByte64(string value)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(value));
        }

        public static string DecodeFromByte64(string value)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(value));
        }

        public static List<string> BindValues(List<string> text, Dictionary<string, string> tokens,
            string tokenStart = "#", string tokenEnd = "#")
        {
            var temp = new List<string>();

            foreach (var txt in text)
            {
                temp.Add(BindValues(txt, tokens, tokenStart, tokenEnd));
            }
            return temp;
        }

        public static string BindValues(string text, Dictionary<string, string> tokens, string tokenStart = "#",
            string tokenEnd = "#")
        {
            MatchEvaluator evaluator = match =>
            {
                string token;
                if (tokens.TryGetValue(match.Groups[1].Value, out token)) return token;
                return "";
            };

            return Regex.Replace(text, tokenStart + @"(\S+)" + tokenEnd, evaluator);
        }

        public static string TrimLength(this string text, int maxLength)
        {
            var substr = Math.Min(text.Length, maxLength);
            return text.Substring(0, substr);
        }

        /// <summary>
        /// Replaces the first occruence of 'search' with 'replace'
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string RemoveDiacritics(this string text)
        {
            var map = new Dictionary<char, string>()
            {
                {'ä', "ae"},
                {'ö', "oe"},
                {'ü', "ue"},
                {'Ä', "Ae"},
                {'Ö', "Oe"},
                {'Ü', "Ue"},
                {'ß', "ss"},
                {'&', "und"},
                {'á', "a"},
                {'č', "c"},
                {'ď', "d"},
                {'é', "e"},
                {'ě', "e"},
                {'í', "i"},
                {'ň', "n"},
                {'ó', "o"},
                {'ř', "r"},
                {'š', "s"},
                {'ť', "t"},
                {'ú', "u"},
                {'ů', "u"},
                {'ý', "y"},
                {'ž', "z"},
                {'Á', "A"},
                {'Č', "C"},
                {'Ď', "D"},
                {'É', "E"},
                {'Ě', "E"},
                {'Í', "I"},
                {'Ň', "N"},
                {'Ó', "O"},
                {'Ř', "R"},
                {'Š', "S"},
                {'Ť', "T"},
                {'Ú', "U"},
                {'Ů', "U"},
                {'Ý', "Y"},
                {'Ž', "Z"}
            };

            var res = text.Aggregate(
                new StringBuilder(),
                (sb, c) =>
                {
                    string r;
                    if (map.TryGetValue(c, out r))
                        return sb.Append(r);
                    else
                        return sb.Append(c);
                }).ToString();

            return res;
        }

        /// <summary>
        /// Normalizuje łańcuch tekstowy (usunięcie znaczników końca linii, karetki i białych znaków)
        /// </summary>
        /// <param name="str">Łańcuch tekstowy</param>
        /// <returns>Znormalizowany łańcuch tekstowy</returns>
        public static string CustomNormalize(this string str) => str?.Trim().Replace("\r", "").Replace("\n", "");

        public static string MakeOneLineString(this string str) => str?.Trim().Replace("\r", "").Replace("\n", " ");

        public static string MaxLength(this string str, int maxLength)
            => str?.Substring(0, Math.Min(str.Length, maxLength));

        public static string PascalCaseToSpace(this string str)
            => Regex.Replace(str, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))", "$1 ");

        public static T FromJson<T>(this string json) => JsonToObject<T>(json);
        public static T JsonDecode<T>(this string json) => JsonToObject<T>(json);

        public static T JsonToObject<T>(this string json)
        {
            var item = Serializer.DeserializeFromJson<T>(json);
            return item;
        }

        public static T JsonToAnonymous<T>(this string json, T definition)
        {
            var item = Serializer.DeserializeFromJsonToAnonymous(json, definition);
            return item;
        }
    }
}