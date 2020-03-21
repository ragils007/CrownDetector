using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class TypeExtension
    {
        public static bool IsInteger(this Type t)
        {
            var intTypes = new[] { typeof(int), typeof(Int16), typeof(Int32), typeof(Int64) };
            return intTypes.Contains(t);
        }

        public static bool IsNumeric(this Type t)
        {
            var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
              typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
              typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return numericTypes.Contains(t);
        }
    }
}
