using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Msdfa.Tools
{
   public static class MyAttributes
   {
      public static string GetString(PropertyInfo propertyInfo, string attributeName)
      {
         var data = GetData(propertyInfo, attributeName);
         if (data != null && data.Count == 1 && data.First().GetType() == typeof(string)) return data.First().ToString();
         else return null;
      }
      public static int? GetInt(PropertyInfo propertyInfo, string attributeName)
      {
         var data = GetData(propertyInfo, attributeName);
         if (data != null && data.Count == 1 && data.First().GetType() == typeof(int)) return (int)data.First();
         else return null;
      }
      public static bool? GetBool(PropertyInfo propertyInfo, string attributeName)
      {
         var data = GetData(propertyInfo, attributeName);
         if (data != null && data.Count == 1 && data.First().GetType() == typeof(bool)) return (bool)data.First();
         else return null;
      }
      public static bool HasAttribute(this PropertyInfo propertyInfo, string attributeName)
      {
         var data = GetData(propertyInfo, attributeName);
         if (data != null) return true;
         return false;
      }
      public static bool HasAttribute<TAttrib>(this PropertyInfo propertyInfo)
      {
         return HasAttribute(propertyInfo, typeof(TAttrib).Name);
      }


      public static List<object> GetData(PropertyInfo propertyInfo, string attributeName)
      {
         bool hasAttrib = false;
         var data = new List<object>();

         foreach (var attribData in propertyInfo.GetCustomAttributesData()
            .Where(x => x.Constructor.DeclaringType.Name == attributeName || x.Constructor.DeclaringType.Name == attributeName + "Attribute"))
         {
            hasAttrib = true;

            foreach (var item in attribData.ConstructorArguments)
            {
               data.Add(item.Value);
            }
         }
         if (hasAttrib) return data;
         else return null;
      }

      public static int GetFlags<T>(this PropertyInfo propertyInfo)
      {
         var data = GetData(propertyInfo, typeof (T).Name);
         return data == null ? 0 : data.Cast<int>().Sum();
      }

      public static bool HasFlag<TAttribute, TEnum>(this PropertyInfo property, TEnum flags)
         where TEnum: struct
      {
         var iFlags = Convert.ToInt32(flags);
         var flag = GetFlags<TAttribute>(property);
         return (flag & iFlags) != 0;
      }

      public static List<KeyValuePair<string, TAttribute>> GetAttributes<TAttribute>(PropertyInfo[] properties)
      {
         var data = (from prop in properties
                     from item in prop.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>()
                     select new KeyValuePair<string, TAttribute>(prop.Name, item)).ToList();
         return data;
      }

      /// <summary>
      /// Returns list of attributes of given type T
      /// </summary>
      /// <typeparam name="TAttribute"></typeparam>
      /// <param name="propertyInfo"></param>
      /// <returns></returns>
      public static List<TAttribute> GetAttributeOfTypeList<TAttribute>(PropertyInfo propertyInfo)
      {
         var data = (from item in propertyInfo.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>() select item).ToList();
         if (data.Count > 0) return data;
         else return null;
      }

        public static List<TAttribute> GetAttributeOfTypeList<TAttribute>(MemberExpression mExp)
            where TAttribute : Attribute
        {
            return ((TAttribute[])mExp.Member.GetCustomAttributes(typeof(TAttribute), false)).ToList();
        }


        public static Dictionary<string, object> GetNamedData(PropertyInfo propertyInfo, string attributeName, string namedParameter = null)
      {
         bool hasAttrib = false;
         var data = new Dictionary<string, object>();

         foreach (var attribData in propertyInfo.GetCustomAttributesData()
            .Where(x => x.Constructor.DeclaringType != null && (x.Constructor.DeclaringType.Name == attributeName
                                                                || x.Constructor.DeclaringType.Name == attributeName + "Attribute")))
         {
            hasAttrib = true;

            if (attribData.NamedArguments != null)
               foreach (var item in attribData.NamedArguments)
               {
                  if (namedParameter == null || namedParameter == item.MemberName) data.Add(item.MemberName, item.TypedValue.Value);
               }
         }
         if (hasAttrib) return data;
         else return null;
      }

      // Extension method
      /*
       * Usage: 
       * string name = typeof(MyClass).GetAttributeValue((DomainNameAttribute dna) => dna.Name);
       */
      public static TValue GetAttributeValue<TAttribute, TValue>(
          this Type type,
          Func<TAttribute, TValue> valueSelector)
          where TAttribute : Attribute
      {
         var att = type.GetCustomAttributes(
             typeof(TAttribute), true
         ).FirstOrDefault() as TAttribute;
         if (att != null)
         {
            return valueSelector(att);
         }
         return default(TValue);
      }
   }
}
