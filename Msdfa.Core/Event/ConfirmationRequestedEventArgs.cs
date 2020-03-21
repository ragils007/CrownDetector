using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Event
{
    public class ConfirmationRequestedEventArgs
    {
        public List<string> MessageList = new List<string>();
        public string Message => string.Join(Environment.NewLine, this.MessageList);

        /// <summary>
        /// Null value indicates that the event was not handled.
        /// </summary>
        public bool? IsAccepted { get; set; }

        public ConfirmationRequestedEventArgs(string message)
        {
            this.MessageList.Add(message);
        }

        public ConfirmationRequestedEventArgs(List<string> messageList)
        {
            this.MessageList = messageList;
        }
    }
}
