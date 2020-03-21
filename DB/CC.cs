using Msdfa.Core.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrownDetector.DB
{
    public class CC
    {
        static CC() => new ConfigureConnections();

        public static ConnectionContext GetCorona(IConnection cnn = null) => new ConnectionContext("corona", cnn);
    }
}
