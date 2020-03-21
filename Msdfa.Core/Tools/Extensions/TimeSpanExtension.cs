using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class TimeSpanExtension
    {
        public static string ToTimeString(this TimeSpan ts)
        {
            var negatiwe = ts < TimeSpan.Zero ? "-" : "";
            var days = ts.Days != 0 ? "d " + ts.Days.ToString() : "";
            var time = $"{Math.Abs(ts.Hours).ToString()}:{Math.Abs(ts.Minutes).ToString().PadLeft(2,'0')}:{Math.Abs(ts.Seconds).ToString().PadLeft(2,'0')}";

            return $"{negatiwe}{days}{time}";
        }

    }
}
