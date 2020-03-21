using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Tools;

namespace Msdfa.Core.Entities
{
    public class DateYM
    {
        public DateTime Value;

        public int Year => this.Value.Year;
        public int Month => this.Value.Month;

        /// <summary>
        /// Format: yyyyMM, np. 201301
        /// </summary>
        public string YYYYMM => this.Value.ToString("yyyyMM");

        /// <summary>
        /// Format: yyyy-MM, np. 2013-01
        /// </summary>
        public string YYYY_MM => this.Value.ToString("yyyy-MM");

        /// <summary>
        /// Format Keszh: [rok][miesiąc padowany od lewej strony spacją do dwóch znaków], np. "2013 3", "201312"
        /// </summary>
        public string YYYY_M => $"{this.Value.ToString("yyyy")}{this.Value.ToString("%M").PadLeft(2)}";

        public DateYM(int year, int month)
        {
            this.Value = new DateTime(year, month, 1);
        }
        public DateYM(DateTime baseDate): this(baseDate.Year, baseDate.Month) { }

        public DateYM AddYears(int value) => new DateYM(this.Value.AddYears(value));
        public DateYM AddMonths(int value) => new DateYM(this.Value.AddMonths(value));

        public static DateYM Current
        {
            get
            {
                var now = DateTime.Now; 
                return new DateYM(now.Year, now.Month);
            }
        }
        public static DateYM FromString(string value)
        {
            int year = 0, month = 0;

            // Format: yyyyMM
            if (value.Length == 6)
            {
                if (int.TryParse(value.Substring(0, 4), out year) || int.TryParse(value.Substring(4, 2), out month))
                {
                    throw new Exception($"Unable to parse YearMonth from given source [{value}]");
                }
            }
            // format: yyyy-MM
            else if (value.Length == 7)
            {
                if (int.TryParse(value.Substring(0, 4), out year) || int.TryParse(value.Substring(5, 2), out month))
                {
                    throw new Exception($"Unable to parse YearMonth from given source [{value}]");
                }
            }
            else throw new Exception("DateYM: Format not supported. Expected 6 (yyyyMM) or 7 (yyyy-MM) digits.");

            return new DateYM(year, month);
        }

        public static List<DateYM> GetRange(DateYM from, DateYM to)
        {
            var temp = new List<DateYM>();
            var current = from;
            while (current <= to)
            {
                temp.Add(current);
                current = current.AddMonths(1);
            }
            return temp;
        }

        public static bool operator ==(DateYM a, DateYM b)
        {
            if (ReferenceEquals(a, b)) { return true; }                     // If both are null, or both are same instance, return true.
            if (((object)a == null) || ((object)b == null)) return false;   // If one is null, but not both, return false.
            return a.Equals(b);                                             // Return true if the fields match
        }
        public static bool operator !=(DateYM a, DateYM b) => !(a == b);
        public static bool operator >(DateYM a, DateYM b) => a.Value > b.Value;
        public static bool operator <(DateYM a, DateYM b) => a.Value < b.Value;
        public static bool operator >=(DateYM a, DateYM b) => a.Value >= b.Value;
        public static bool operator <=(DateYM a, DateYM b) => a.Value <= b.Value;

        public override bool Equals(object obj)
        {
            var asDate = obj as DateYM;
            if (asDate == null) return false;
            return this.Year == asDate.Year && this.Month == asDate.Month;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + this.Year.GetHashCode();
            hash = (hash * 7) + this.Month.GetHashCode();
            return hash;
        }


    }
}
