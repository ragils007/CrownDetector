using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Attributes
{
    public class MinValueAttribute : BaseValueAttribute
    {
        public MinValueAttribute(int value) : base(value) {}
        public MinValueAttribute(string value) : base(value) { }
    }
}
