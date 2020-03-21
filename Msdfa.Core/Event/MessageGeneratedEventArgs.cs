using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Event
{
    public class MessageGeneratedEventArgs : EventArgs
    {
        public List<string> MessageList = new List<string>();
        public string Message => string.Join(Environment.NewLine, this.MessageList);
        
        public MessageGeneratedEventArgs(string message)
        {
            this.MessageList.Add(message);
        }

        public MessageGeneratedEventArgs(List<string> messageList)
        {
            this.MessageList = messageList;
        }
    }
}
