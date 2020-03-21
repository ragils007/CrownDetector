using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
	public static class EnumTools
	{
		public static string GetDescription(this Enum value)
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				FieldInfo field = type.GetField(name);
				if (field != null)
				{
					DescriptionAttribute attr =
						   Attribute.GetCustomAttribute(field,
							 typeof(DescriptionAttribute)) as DescriptionAttribute;
					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public static string GetEnumTextValue(this Enum value)
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);
			if (name == null)
				return null;

			var field = type.GetField(name);
			if (field == null)
				return null;

			var attr = Attribute.GetCustomAttribute(field, typeof(EnumTextValueAttribute)) as EnumTextValueAttribute;
			return attr?.EnumTextValue;
		}

        public static TEnum GetItemByName<TEnum>(string name)
        {
            var ret = (TEnum)Enum.Parse(typeof(TEnum), name);
            return ret;
        }

        public static Dictionary<int, string> GetDictionary_WithName<T>()
        {
            var ret = Enum.GetValues(typeof(T)).Cast<object>()
                .ToDictionary(x => (int)x, x => x.ToString());

            return ret;
        }

        public static Dictionary<int, string> GetDictionary_WithDescription<T>()
        {
            var ret = Enum.GetValues(typeof(T)).Cast<object>()
                .ToDictionary(x => (int)x, x => ((Enum)x).GetDescription());

            return ret;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class EnumTextValueAttribute : Attribute
	{
		public string EnumTextValue { get; set; }
		public EnumTextValueAttribute(string enumTextValue)
		{
			EnumTextValue = enumTextValue;
		}
	}

}
