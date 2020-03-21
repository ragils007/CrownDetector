using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Msdfa.Core.Entities
{
    public class Date
    {
        public readonly DateTime DateTime;

        public DateTime ValueStart => new DateTime(this.Year, this.Month, this.Day, 0, 0, 0);
        public DateTime ValueEnd => new DateTime(this.Year, this.Month, this.Day, 23, 59, 59);

        public int Year => this.DateTime.Year;
        public int Month => this.DateTime.Month;
        public int Day => this.DateTime.Day;
        public DayOfWeek DayOfTheWeek => this.DateTime.DayOfWeek;
        public int DayOfYear => this.DateTime.DayOfYear;
        public string ValueString => this.YYYY_MM_DD;
        public string YYYY_MM_DD => this.DateTime.ToString("yyyy-MM-dd");

        public bool IsEmpty => this.DateTime.Year == 1 && this.DateTime.Month == 1 && this.DateTime.Day == 1
            && this.DateTime.Hour == 0 && this.DateTime.Minute == 0 && this.DateTime.Second == 0;

        public Date(DateTime date)
        {
            this.DateTime = date.Date;
        }

        public Date(DateTime? date)
        {
            if (date == null)
                this.DateTime = new DateTime();
            else
                this.DateTime = date.Value.Date;
        }

        public static bool operator ==(Date a, Date b)
        {
            if (ReferenceEquals(a, b)) { return true; }                     // If both are null, or both are same instance, return true.
            if (((object)a == null) || ((object)b == null)) return false;   // If one is null, but not both, return false.
            return a.Equals(b);                                             // Return true if the fields match
        }
        public static bool operator !=(Date a, Date b) => !(a == b);
        public static bool operator >(Date a, Date b) => a.DateTime > b.DateTime;
        public static bool operator <(Date a, Date b) => a.DateTime < b.DateTime;
        public static bool operator >=(Date a, Date b) => a.DateTime >= b.DateTime;
        public static bool operator <=(Date a, Date b) => a.DateTime <= b.DateTime;

        public override bool Equals(object obj)
        {
            var asDate = obj as Date;
            if (asDate == null) return false;
            return this.Year == asDate.Year && this.Month == asDate.Month && this.Day == asDate.Day;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + this.Year.GetHashCode();
            hash = (hash * 7) + this.Month.GetHashCode();
            hash = (hash * 7) + this.Day.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return this.DateTime.ToString("yyyy-MM-dd");
        }

        public static Date GetByYYYYMM_FirstDay(string yyyyMM)
        {
            if (yyyyMM.Length != 6) throw new Exception("Oczekiwano ciągu 6 znaków");

            int year = 0, month = 0;

            if (int.TryParse(yyyyMM.Substring(0, 4), out year) == false) throw new Exception("Nie rozpoznany format roku");
            if (int.TryParse(yyyyMM.Substring(4, 2), out month) == false) throw new Exception("Nie rozpoznany format miesiąca");

            return new Date(new DateTime(year, month, 1));
        }

        public static implicit operator DateTime(Date date) => date.DateTime;

        public static explicit operator Date(DateTime dateTime) => new Date(dateTime);

    }
}
