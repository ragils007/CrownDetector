using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class NumericExtensions
    {
        public static string StringValueIfGreaterThan0(this int value)
        {
            if (value == 0) return "";
            return value.ToString();
        }

        public static string StringValueIfGreaterThan0(this decimal value)
        {
            if (value == 0) return "";
            return value.ToString();
        }
    }
}
