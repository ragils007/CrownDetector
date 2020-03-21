using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Attributes
{
    public class MaxValueAttribute : BaseValueAttribute
    {
        public MaxValueAttribute(int value) : base(value) {}
        public MaxValueAttribute(string value) : base(value) { }
    }
}
