using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Event
{
    public class BeforeItemSetEventArgs<TItem> : EventArgs
    {
        public TItem Item { get; set; }
        public bool Cancel { get; set; }

        public BeforeItemSetEventArgs(TItem item)
        {
            this.Item = item;
        }
    }
}
