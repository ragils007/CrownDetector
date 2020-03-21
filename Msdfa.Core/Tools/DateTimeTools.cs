using Msdfa.Core.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class DateTimeTools
    {
        /// <summary>
        /// Zwraca ilość miesięcy o jakie zahacza okres między dwoma datami. 
        /// Miesiąc jest wliczany jeżeli przynajmniej jeden dzień z danego miesiąca znajduje się w analizowanych datach.
        /// Przykłady:
        /// 2010-12-01 i 2010-12-30 => 1
        /// 2010-12-31 i 2011-01-01 => 2
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int DiffMonths(DateTime from, DateTime to)
        {
            var tempFrom = new DateTime(from.Year, from.Month, 1);
            var tempTo = new DateTime(to.Year, to.Month, 1);

            if (tempTo < tempFrom) return 0;
            if (tempTo == tempFrom) return 1;

            var months = 0;
            while (tempFrom.AddMonths(months++) < tempTo) {}
            return months;
        }

        /// <summary>
        /// Jak wyżej, lecz powinno być szybsze
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int DiffMonths2(DateTime from, DateTime to)
        {
            return ((to.Year - from.Year)*12) + to.Month - from.Month;
        }

        /// <summary>
        /// Metoda zwraca liczbę dni między dwoma datami.
        /// Jeżeli data od = data do => wynikiem będzie 0.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int DiffDays(DateTime from, DateTime to)
        {
            var tempFrom = from.Date;
            var tempTo = to.Date;

            if (tempTo < tempFrom) throw new Exception("from > to");
            if (tempTo == tempFrom) return 0;

            var days = 0;
            while (tempFrom.AddDays(days) < tempTo)
            {
                days++;
            }
            return days;
        }

        public static List<DateTime> GetDatesRange(DateTime from, DateTime to)
        {
            var newFrom = from.Date;
            var newTo = to.Date;

            if (newTo < newFrom) throw new Exception("FROM date is after TO date");

            var list = new List<DateTime>();

            for (var dt = newFrom; dt <= newTo; dt = dt.AddDays(1))
            { 
                list.Add(dt);
            }

            return list;
        }

        public static List<DateTime> GetMonthsRange(DateTime from, DateTime to)
        {
            var newFrom = GetFirstOfTheMonth(from);
            var newTo = GetFirstOfTheMonth(to);

            if (newTo < newFrom) throw new Exception("FROM date is after TO date");

            var list = new List<DateTime>();

            for (var dt = newFrom; dt <= newTo; dt = dt.AddMonths(1))
            {
                list.Add(dt);
            }

            return list;
        }

        public static List<string> GetOkresRange(string okresOd, string okresDo)
        {
            var dataOd = GetDateFromString(okresOd, "yyyyMM");
            var dataDo = GetDateFromString(okresDo, "yyyyMM");

            var ret = GetMonthsRange(dataOd, dataDo)
                .Select(x => x.YYYY_MM(""))
                .ToList();

            return ret;
        }

        /// <summary>
        /// Z podanego roku i miesiąca zwraca datę pierwszego dnia tego roku i miesiąca.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>

        public static DateTime GetFrom_YYYYMM(string value)
        {
            if (value.Length == 6 && !value.Contains("-")) value = value.Insert(4, "-");
            return GetFrom_YYYY_MM(value);
        }
        public static DateTime GetFrom_YYYY_MM(string value, char separator = '-')
        {
            var tokens = value.Split('-');
            var rok = Convert.ToInt32(tokens[0]);
            var miesiac = Convert.ToInt32(tokens[1]);
            return new DateTime(rok, miesiac, 1);
        }

        public static DateTime GetFrom_YYYYMMDD(string value)
        {
            if (value.Length != 8) throw new Exception("Oczekiwano ciąg o długości 8 znaków");

            var rok = Convert.ToInt32(value.Substring(0, 4));
            var mies = Convert.ToInt32(value.Substring(4, 2));
            var dzien = Convert.ToInt32(value.Substring(6, 2));

            return new DateTime(rok, mies, dzien);
        }

        public static DateTime GetFirstOfTheMonth(DateTime date) => new DateTime(date.Year, date.Month, 1, 0, 0, 0);
        public static string GetString_YYYY_M(DateTime date) => $"{date.ToString("yyyy")}{date.ToString("%M").PadLeft(2)}";

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime GetLastOfTheMonth(DateTime data)
        {
            var daysInMonth = DateTime.DaysInMonth(data.Year, data.Month);
            return new DateTime(data.Year, data.Month, daysInMonth, 23, 59, 59);
        }

        public static DateTime GetFirstDayOfMonth(DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }

        public static DateTime GetFirstDateOfWeek(int year, int weekOfYear)
        {
            // poniedziałek jako pierwszy dzień tygodnia
            var ci = CultureInfo.CreateSpecificCulture("pl-PL");
            
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            var firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int offset = fdow - date.DayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime GetLastDayOfWeek(DateTime date)
        {
            DateTime ldowDate = GetFirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }

        public static DateTime GetStartOfDay(DateTime data)
        {
            var newDate = new DateTime(data.Year, data.Month, data.Day, 0, 0, 0);
            return newDate;
        }

        public static DateTime GetEndOfDay(DateTime data)
        {
            var newDate = new DateTime(data.Year, data.Month, data.Day, 23, 59, 59);
            return newDate;
        }

        public static DateTime Max(DateTime dt1, DateTime dt2)
        {
            if (dt1 > dt2)
                return dt1;

            return dt2;
        }

        public static DateTime? GetDateNullableFromString(string dateString, string formatString)
        {
            if (string.IsNullOrEmpty(dateString)) return null;

            if (formatString.Length != dateString.Length) throw new Exception("Błędna długość ciągu wejściowego");

            DateTime date;
            if (DateTime.TryParseExact(dateString, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return date;
            return null;
        }

        public static DateTime GetDateFromString(string dateString, string formatString)
        {
            if (formatString.Length != dateString.Length) throw new Exception("Błędna długość ciągu wejściowego");

            DateTime date;
            if (DateTime.TryParseExact(dateString, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return date;
            throw new Exception($"Nie udało się sparsować daty! [{dateString} -> {formatString}]");
        }
    }
}
