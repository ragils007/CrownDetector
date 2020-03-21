using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Tools;

namespace Msdfa.Core.Tools.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the value of nested object.
        /// </summary>
        /// <param name="obj">RootObject from which we get the nested value</param>
        /// <param name="propertyPath">path to the property we want to get (separated by '.') Property1.Property2.Property3</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyPath, bool throwingException = false)
        {
            if (obj == null) throw new NullReferenceException("Obiekt ma wartość <null>");

            object propertyValue = null;
            if (propertyPath.IndexOf(".", StringComparison.Ordinal) < 0)
            {
                var objType = obj.GetType();
                var pi = objType.GetProperty(propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
                if (pi != null)
                {
                    propertyValue = pi.GetValue(obj, null);
                    return propertyValue;
                }
                var fi = objType.GetField(propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
                propertyValue = fi.GetValue(obj);
                return propertyValue;

            }
            var properties = propertyPath.Split('.').ToList();
            var midPropertyValue = obj;
            while (properties.Count > 0)
            {
                var propertyName = properties.First();
                properties.Remove(propertyName);
                propertyValue = midPropertyValue.GetPropertyValue(propertyName);

                if (throwingException && properties.Count > 0 && propertyValue == null)
                {
                    throw new Exception($"GetPropertyValue(): Unable to traverse path. Null value encountered [{propertyPath} => {propertyName}]");
                }

                midPropertyValue = propertyValue;
            }
            return propertyValue;
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName);

            if (prop == null || !prop.CanWrite) throw new Exception("Cant write to property");

            prop.SetValue(obj, ValueConverter.Get(value));
        }

        public static string GetStringValue<TDataType>(this TDataType obj,
            Expression<Func<TDataType, object>> expression)
        {
            if (obj == null) return null;
            var value = expression.Compile()(obj);
            return value?.ToString();
        }

        public static List<object> GetPropertyValues<T>(this List<T> list, string propertyName)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
                throw new Exception("No such property");

            var values = list.Select(x => propertyInfo.GetValue(x)).ToList();
            return values;
        }

        public static string JsonEncode<T>(this T item) => ToJson(item);
        public static string ToJson<T>(this T item)
        {
            var ret = Serializer.SerializeToJson(item);
            return ret;
        }
    }
}