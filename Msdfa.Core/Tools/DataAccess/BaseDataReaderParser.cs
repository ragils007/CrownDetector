using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Entities;
using Msdfa.Core.Tools.Extensions;

namespace Msdfa.Core.Tools.DataAccess
{
    public class BaseDataReaderParser<T>
        where T : new()
    {
        public Dictionary<int, BaseDataReaderColumnInfo> MappedColumns = new Dictionary<int, BaseDataReaderColumnInfo>();

        public void Init(Dictionary<int, BaseDataReaderColumnInfo> mappedColumns)
        {
            this.MappedColumns = mappedColumns;
        }

        public T GetItemFromDataArray(object[] values)
        {
            if (this.MappedColumns.Count == 0) throw new Exception("Nie ustawiono mapowania kolumn!");

            var obj = new T();
            var errors = new List<string>();

            for (var i = 0; i < this.MappedColumns.Count; i++)
            {
                var column = this.MappedColumns[i];
                try
                {
                    object va = null;
                    if (values.Length > i) va = values[i];
                    if (string.IsNullOrEmpty(va?.ToString())) va = column.DefaultValue;
                    if (string.IsNullOrEmpty(va?.ToString()) && !column.IsNullable) throw new Exception($"[{column.PropertyInfo.Name}]: Brak uzupełnionej wartości");

                    try
                    {
                        var asMoney = va as Money;
                        if (asMoney != null)
                        {
                            column.PropertyInfo.SetValue(obj, asMoney);
                            continue;
                        }

                        var asString = va as string;
                        if (asString != null)
                        {
                            if (column.IsHardSpaceAllowed == false) asString = asString.Replace(' ', ' ');   // Zamiana twardych spacji na normalne
                            if (column.IsTextUppercase) asString = asString.ToUpper();                       // Zamiana tekstu na duże litery
                            if (column.IsTrimmed) asString = asString.Trim();

                            if (column.IsNullable == false && string.IsNullOrEmpty(asString))
                            {
                                throw new Exception($"[{column.PropertyInfo.Name}] Kolumna nie może zawierać pustych wartości");
                            }
                        }

                        var val = ValueConverter.Get(asString ?? va, column.PropertyInfo.PropertyType);
                        column.PropertyInfo.SetValue(obj, val);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Błąd konwersji wartości '{va}' na typ '{column.PropertyInfo.PropertyType.Name}':\n{e.Message}");
                    }

                    //if (string.IsNullOrEmpty(values[i]))
                    //{
                    //    if (column.DefaultValue != null)
                    //    {
                    //        var val = ValueConverter.Get(column.DefaultValue, column.PropertyInfo.PropertyType);
                    //        column.PropertyInfo.SetValue(obj, val);
                    //        continue;
                    //    }
                    //    if (column.PropertyInfo.PropertyType.IsValueType || column.IsNullable == false)
                    //    {

                    //    }
                    //}

                    //try
                    //{
                    //    var value = this.GetValue(column.PropertyInfo.PropertyType, values[i]);
                    //    column.PropertyInfo.SetValue(obj, value);
                    //}
                    //catch (Exception e)
                    //{
                    //    throw new Exception(
                    //        $"Błąd konwersji wartości '{values[i]}' na typ: {column.PropertyInfo.PropertyType.Name}");
                    //}
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);
                }
            }


            //for (var i = 0; i < values.Length; i++)
            //{
            //    try
            //    {
            //        var column = this.MappedColumns[i];
            //        try
            //        {
            //            if (string.IsNullOrEmpty(values[i]))
            //            {
            //                if (column.DefaultValue != null)
            //                {
            //                    var val = ValueConverter.Get(column.DefaultValue, column.PropertyInfo.PropertyType);
            //                    column.PropertyInfo.SetValue(obj, val);
            //                    continue;
            //                }
            //                if (column.PropertyInfo.PropertyType.IsValueType || column.IsNullable == false)
            //                {
            //                    throw new Exception("Brak uzupełnionej wartości");
            //                }
            //            }

            //            try
            //            {
            //                var value = this.GetValue(column.PropertyInfo.PropertyType, values[i]);
            //                column.PropertyInfo.SetValue(obj, value);
            //            }
            //            catch (Exception e)
            //            {
            //                throw new Exception($"Błąd konwersji wartości '{values[i]}' na typ: {column.PropertyInfo.PropertyType.Name}");
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            throw new Exception($"<{column.PropertyInfo.Name}> {e.Message}");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        errors.Add(e.Message);
            //    }
            //}

            if (errors.Any()) throw new Exception(string.Join("|", errors));
            return obj;
        }

        /*
        public virtual object GetValue(Type type, string value)
        {
            // Stripping hidden characters
            value = new string(value.Where(c => c >= 32 && c < 128).ToArray()).Trim();

            if (type == typeof(string)) return value;
            if (type == typeof(bool)) return value.TryGetBoolValue();
            if (type == typeof(decimal)) value = value.Replace(".", ",").Replace(" ", "");
            if (type == typeof(int) || type == typeof(long)) value = value.Replace(" ", "");

            var converter = TypeDescriptor.GetConverter(type);

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (converter.CanConvertFrom(typeof(string)))
                {
                    var result = converter.ConvertFrom(value);
                    return result;
                }
            }

            if (type.IsValueType) //return default value
                return Activator.CreateInstance(type);

            return null; //cant create a default value, return null
        }
        */

        /*
         * Do rozważenia, wydajnośc powinna być lepsza niż parsowanie stringów
         */
        protected virtual object GetValue<TValue>(Type retType, TValue value)
        {
            var valType = typeof(TValue);

            // W przypadku oczekiwanego typu string
            if (valType == typeof(string))
            {
                var strValue = new string(((string)(object)value).Where(c => c >= 32 && c < 128).ToArray()).Trim();
                //var strValue = (string) (object) value;

                if (retType == typeof(string)) return strValue;
                if (retType == typeof(bool)) return strValue.TryGetBoolValue();
                if (retType == typeof(DateTime))
                {
                    DateTime date;
                    if (DateTime.TryParseExact(strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return date;
                    return null;
                }

                // W przypadku decimal i float: usuwamy zbędne spacje oraz zamieniamy kropkę i przecinek na separator liczb dziesiętnych systemu
                if (retType == typeof(decimal) || retType == typeof(float)) strValue = strValue
                        .Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        .Replace(" ", "");
                if (retType == typeof(int) || retType == typeof(long)) strValue = strValue.Replace(" ", "");

                var converter = TypeDescriptor.GetConverter(retType);

                if (!string.IsNullOrWhiteSpace(strValue))
                {
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        var result = converter.ConvertFrom(strValue);
                        return result;
                    }
                }

                return retType.IsValueType ? Activator.CreateInstance(retType) : null;
            }

            var val = ValueConverter.Get(value, typeof(TValue));
            return val;
            //if (retType == typeof (TValue)) return value;

            // Stripping hidden characters

            //var converter = TypeDescriptor.GetConverter(type);

            //if (!string.IsNullOrWhiteSpace(value))
            //{
            //    if (converter.CanConvertFrom(typeof(string)))
            //    {
            //        var result = converter.ConvertFrom(value);
            //        return result;
            //    }
            //}

            //if (type.IsValueType) //return default value
            //    return Activator.CreateInstance(type);

            // return null; //cant create a default value, return null
        }
    }
}
