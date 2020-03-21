using Msdfa.Core.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrownDetector.DB
{
    public class ConfigureConnections
    {
        public ConfigureConnections()
        {
            ConnectionContext.Configure<CnnCorona>("corona");
        }
    }
}
