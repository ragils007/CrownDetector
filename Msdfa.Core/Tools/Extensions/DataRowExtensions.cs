using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DataRowExtensions
    {
        public static long GetLong(this DataRow dr, string colName) => Convert.ToInt64(dr[colName]);
        public static int GetInt(this DataRow dr, string colName) => Convert.ToInt32(dr[colName]);
        public static decimal GetDecimal(this DataRow dr, string colName) => Convert.ToDecimal(dr[colName]);
        public static string GetString(this DataRow dr, string colName) => dr[colName].ToString();
        public static DateTime GetDateTime(this DataRow dr, string colName) => Convert.ToDateTime(dr[colName]);

        public static long? GetLongOrNull(this DataRow dr, string colName) => dr[colName] == DBNull.Value ? null : new long?(Convert.ToInt64(dr[colName]));
        public static int? GetIntOrNull(this DataRow dr, string colName) => dr[colName] == DBNull.Value ? null : new int?(Convert.ToInt32(dr[colName]));
        public static decimal? GetDecimalOrNull(this DataRow dr, string colName) => dr[colName] == DBNull.Value ? null : new decimal?(Convert.ToDecimal(dr[colName]));
        public static DateTime? GetDateTimeOrNull(this DataRow dr, string colName) => dr[colName] == DBNull.Value ? null : new DateTime?(Convert.ToDateTime(dr[colName]));
    }
}
