using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    public class IdKeyValue
    {
        public IdKey IdKey { get; set; }
        public string Value { get; set; }

        public IdKeyValue() { }
        public IdKeyValue(long id, string value)
        {
            this.IdKey = id;
            this.Value = value;
        }
        public IdKeyValue(string key, string value)
        {
            this.IdKey = key;
            this.Value = value;
        }

        public override string ToString() { return this.Value; }
    }
}
