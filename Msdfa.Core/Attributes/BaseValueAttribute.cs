using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Attributes
{
    public abstract class BaseValueAttribute : Attribute
    {
        public decimal Value { get; private set; }
        public int ValueInt => Convert.ToInt32(this.Value);
        public long ValueLong => Convert.ToInt64(this.Value);

        public BaseValueAttribute(int value)
        {
            this.Value = value;
        }

        public BaseValueAttribute(string value)
        {
            this.Value = decimal.Parse(value);
        }
    }
}
