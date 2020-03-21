using Msdfa.Core.Entities;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Msdfa.Core.Tools
{
    public static class ValueConverter
    {
        public static object Get(object value)
        {
            return value == null
                ? null
                : Get(value, value.GetType());
        }

        public static T GetValue<T>(string value)
        {
            var tempType = typeof(T);
            var type = Nullable.GetUnderlyingType(tempType) ?? tempType;
            var safeValue = (string.IsNullOrWhiteSpace(value)) ? null : Convert.ChangeType(value, type);

            return (T)safeValue;
        }

        public static object Get(object value, Type propertyType)
        {
            if (value == null || value is DBNull) return null;
            if (value.GetType() == propertyType) return value; //uniknąć niepotrzbnego boxowania!!!!!!

            if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                var stringValue = Regex.Replace(value.ToString(), "[^0-9,-]", "");
                return Convert.ToInt64(stringValue);
            }
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) return Convert.ToDateTime(value);

            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                var asString = value as string;
                if (asString != null)
                {
                    if (new[] { "1", "Y", "YES", "T", "TAK", "TRUE" }.Contains(asString.ToUpper())) return true;
                    if (new[] { "0", "N", "NO", "NIE", "FALSE" }.Contains(asString.ToUpper())) return false;
                    throw new Exception($"Unable to parse string { asString } to bool.");
                }
                return Convert.ToBoolean(value);
            }
            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                var stringValue = Regex.Replace(value.ToString(), "[^0-9,.-]", "");
                stringValue = stringValue.Replace(',', Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                stringValue = stringValue.Replace('.', Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));

                // Przechodzimy przez decimal, żeby obsłużyć wartości '0,00'
                var decimalValue = decimal.Parse(stringValue);
                var intValue = Convert.ToInt32(decimalValue);
                return intValue;
            }

            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                || propertyType == typeof(double) || propertyType == typeof(double?)
                || propertyType == typeof(float) || propertyType == typeof(float?))
            {
                if (value.GetType() == typeof(decimal)
                    || value.GetType() == typeof(double)
                    || value.GetType() == typeof(float))
                    return Convert.ToDecimal(value);

                if (value != null &&
                    (value.GetType() == typeof(decimal?)
                    || value.GetType() == typeof(double?)
                    || value.GetType() == typeof(float?)))
                    return Convert.ToDecimal(value);

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ",";

                var stringValue = value.ToString().Replace('.', ',');
                stringValue = Regex.Replace(stringValue, "[^0-9,-]", "");

                return Convert.ToDecimal(stringValue, nfi);
            }

            if (propertyType == typeof(string)) return Convert.ToString(value);

            if (propertyType == typeof(Date)) return new Date((DateTime)value);

            if (propertyType == typeof(IdKey))
            {
                if (value is long) return new IdKey((long)value);
                return new IdKey(value.ToString());
            }

            if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value.ToString());
            }

            return value;
        }

        /// <summary>
        /// Dużo szybsza metoda konwertowania danych niż Get. 
        /// Używana na rzecz klasy BaseTable
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Get2(object value, Type type)
        {
            if (value == null || value is DBNull) return null;
            if (value.GetType() == type) return value; //uniknąć niepotrzbnego boxowania!!!!!!
            type = Nullable.GetUnderlyingType(type) ?? type;
            return Convert.ChangeType(value, type);
        }
    }
}
