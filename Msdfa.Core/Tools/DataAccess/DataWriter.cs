using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools.DataAccess
{
    public class DataWriter<T> : BaseDataWriter<T> where T : new()
    {
        public DataWriter(StreamWriter streamWriter, string seperator) 
            : base(streamWriter, seperator)
        {
        }

        public static DataWriter<T> Create(string filePath, string seperator)
        {
            var streamWriter = new StreamWriter(filePath);
            var dataWriter = new DataWriter<T>(streamWriter, seperator);
            return dataWriter;
        }
    }
}
