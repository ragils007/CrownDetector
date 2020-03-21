using System;

namespace Msdfa.Core.Attributes
{
   public class FixedValue : Attribute
   {
      public object Value { get; set; }

      public FixedValue(object value)
      {
         var valueE = value as Enum;
         if (valueE != null)
         {
            var enumType = valueE.GetType();
            var enumDataType = Enum.GetUnderlyingType(enumType);

            if (enumDataType == typeof(long)) this.Value = (long)value;
            else if (enumDataType == typeof(int)) this.Value = (int)value;
            else throw new Exception("Enum data type not supported. [" + enumDataType + "]");
         }
         else this.Value = value;
      }
   }
}
