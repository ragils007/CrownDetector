using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Msdfa.Core.Tools.Extensions
{
    /// <summary>
    /// Pomocnicza klasa convert wzbogacone o mozliwosc konwersji do typow nullable
    /// null i dbnull jest konwertowany do nulla
    /// </summary>
    public static class ConvertExtensions
    {
        public static long? ToNullableInt64(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return null;

            return Convert.ToInt64(obj);
        }

        public static Int32? ToNullableInt32(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return null;

            return Convert.ToInt32(obj);
        }

        public static DateTime? ToNullableDateTime(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return null;

            return Convert.ToDateTime(obj);
        }

        public static Decimal? ToNullableDecimal(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return null;

            return Convert.ToDecimal(obj);
        }

        public static string ToNullableString(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return null;

            return Convert.ToString(obj);
        }

        public static bool? ToNullableBoolean(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj)) return false;

            return Convert.ToBoolean(obj);
        }

        public static bool IsNullOrDbNull(object obj)
        {
            if (obj == null) return true;

            if (obj.GetType() == typeof(DBNull) && obj == DBNull.Value) return true;

            return false;
        }

    }
}