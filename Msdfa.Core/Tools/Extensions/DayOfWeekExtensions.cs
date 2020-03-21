using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DayOfWeekExtensions
    {
        public static string DayToString(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "7";
                case DayOfWeek.Monday:
                    return "1";
                case DayOfWeek.Tuesday:
                    return "2";
                case DayOfWeek.Wednesday:
                    return "3";
                case DayOfWeek.Thursday:
                    return "4";
                case DayOfWeek.Friday:
                    return "5";
                case DayOfWeek.Saturday:
                    return "6";
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
            }
        }
        
        public static int DayToInt(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 7;
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
            }
        }
    }
}