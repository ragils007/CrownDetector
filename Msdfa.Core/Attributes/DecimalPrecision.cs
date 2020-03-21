using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Attributes
{
    public class DecimalPrecisionAttribute : Attribute
    {
        public int IntegerPlaces { get; set; }
        public int DecimalPlaces { get; set; }

        public DecimalPrecisionAttribute(int integerPlaces, int decimalPlaces)
        {
            IntegerPlaces = integerPlaces;
            DecimalPlaces = decimalPlaces;
        }
    }
}
