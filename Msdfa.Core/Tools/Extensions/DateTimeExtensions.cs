using Msdfa.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Zwraca listę dat (bez godzin, minut etc.) do podanego czasu.
        /// Czyli dla poniedziałku do środy zwróci (pon, wt, śr)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<DateTime> GetDaysToDate(this DateTime from, DateTime to)
        {
            var totalDays = (int)(to - from).TotalDays;

            var result = new List<DateTime>();
            var firstDate = from.Date;
            result.Add(firstDate);

            for (int i = 0; i < totalDays; i++)
            {
                result.Add(firstDate.AddDays(i + 1));
            }

            return result;
        }

        /// <summary>
        /// Dodaje podane godziny i minuty w formacie hh:mm do podanego DateTime
        /// </summary>
        /// <param name="from"></param>
        /// <param name="hhmm"></param>
        /// <returns></returns>
        public static DateTime AddHoursMins(this DateTime from, string hhmm)
        {
            if (string.IsNullOrWhiteSpace(hhmm)) return from;

            var match = Regex.Match(hhmm, @"\d\d:\d\d");
            if (!match.Success) return from;

            hhmm = match.Value;

            var splitted = hhmm.Split(':');

            var hours = Convert.ToInt32(splitted[0]);
            var mins = Convert.ToInt32(splitted[1]);

            var newDate = from.AddHours(hours);
            newDate = newDate.AddMinutes(mins);

            return newDate;
        }

        public static DateTime RemoveHoursMins(this DateTime from, string hhmm)
        {
            if (string.IsNullOrWhiteSpace(hhmm)) return from;

            var match = Regex.Match(hhmm, @"\d\d:\d\d");
            if (!match.Success) return from;

            hhmm = match.Value;

            var splitted = hhmm.Split(':');

            var hours = Convert.ToInt32(splitted[0]);
            var mins = Convert.ToInt32(splitted[1]);

            var newDate = from.AddHours(-hours);
            newDate = newDate.AddMinutes(-mins);

            return newDate;
        }

        public static DateTime StartOfWeek(this DateTime fromDate)
        {
            int diff = fromDate.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
                diff += 7;
            return fromDate.AddDays(-1 * diff).Date;
        }

        public static DateTime NextStartOfWeek(this DateTime fromDate)
        {
            var startOfWeek = fromDate.StartOfWeek();
            return startOfWeek.AddDays(7).Date;
        }

        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        public static DateTime SubtractBussinessDays(this DateTime date, int days, bool isSaturdayBussinessDay)
        {
            if (days > 0)
                throw new ArgumentException("days cannot be positive", nameof(days));

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Sunday && !isSaturdayBussinessDay)
                days -= 1;

            if (date.DayOfWeek == DayOfWeek.Monday)
            {
                days -= 1;
                if (!isSaturdayBussinessDay) days -= 1;
            }

            var daysInBussinessWeek = isSaturdayBussinessDay ? 6 : 5;
            var wholeBussinessWeeks = days / daysInBussinessWeek * 7;
            var extraDays = days % daysInBussinessWeek;

            //sprawdzic czy nie przekraczamy weekendu
            var currentDayOfWeek = date.DayOfWeek.DayToInt();
            if (currentDayOfWeek + extraDays < 0)
                extraDays -= (7 - daysInBussinessWeek);

            date = date.AddDays(wholeBussinessWeeks);
            date = date.AddDays(extraDays);

            return date;
        }

        public static string GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static bool IsBetweenDates(this DateTime value, DateTime dateFrom, DateTime dateTo) => value >= dateFrom && value <= dateTo;

        public static DateTime FirstOfTheMonth(this DateTime date) => DateTimeTools.GetFirstOfTheMonth(date);
        public static DateTime LastOfTheMonth(this DateTime date) => DateTimeTools.GetLastOfTheMonth(date);

        public static DateTime BeginningOfDay(this DateTime date) => GetSOD(date);
        public static DateTime GetStartOfDay(this DateTime date) => GetSOD(date);
        public static DateTime GetSOD(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        public static DateTime EndOfDay(this DateTime date) => GetEOD(date);
        public static DateTime GetEOD(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        public static string YYYYMM(this DateTime date) => $"{date.ToString("yyyy")}{date.ToString("MM")}";
        public static string YYYY_MM(this DateTime date, string separator = "-") => $"{date.ToString("yyyy")}{separator}{date.ToString("MM")}";
        public static string YYYY_MM_DD(this DateTime date, string separator = "-") => $"{date.ToString("yyyy")}{separator}{date.ToString("MM")}{separator}{date.ToString("dd")}";
        public static string YYYY_MM_DD_HH_MI_SS(this DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
        public static string YYYYMMDDHHMISS(this DateTime date) => date.ToString("yyyyMMddHHmmss");
        public static string YYYYMMDD_HHMISS(this DateTime date) => date.ToString("yyyyMMdd.HHmmss");
    }
}