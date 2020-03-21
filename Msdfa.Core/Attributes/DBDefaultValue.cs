using System;

namespace Msdfa.Core.Attributes
{
   public class DBDefaultValue : Attribute
   {
      public object Value { get; set; }

      public DBDefaultValue(object value)
      {
         if (value is Enum) this.Value = (long)value;
         else this.Value = value;
      }
   }
}
