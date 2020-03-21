using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    [DebuggerDisplay("Old: {OldValue}, New: {NewValue}")]
    public class OldNewValue
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public bool HasEqualValues()
        {
            if ( (this.OldValue == null && this.NewValue == null) ||
                 (this.OldValue?.Equals(this.NewValue) ?? false) )
            {
                return true;
            }

            return false;
        }
    }
}
