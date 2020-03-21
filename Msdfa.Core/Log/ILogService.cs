using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Log
{
    public interface ILogService
    {
        void Info(string msg);
        void Error(string msg);
        void Warn(string msg);
        void Debug(string msg);
    }
}
