using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class ParsingHelpers
    {
        public static List<string> GetEmails(string text)
        {
            var regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            var matches = regex.Matches(text);
            var result = new List<string>();
            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }
            return result;
        }

        public static decimal? FindDecimalAmountInString(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            
            text = text.Replace(".", "").Replace(" ", "");//currently '.' and ' ' assumed as thousands seperator...

            var match = Regex.Match(text, @"-?\d+(?:,\d+)?");
            if (!match.Success) return null;

            var formatProvider = new NumberFormatInfo();
            formatProvider.NumberDecimalSeparator = ","; //',' assumed as decimal seperator

            decimal result;
            var found = decimal.TryParse(match.Value, NumberStyles.Any, formatProvider, out result);
            if (found) return result;

            return null;
        }
    }
}
