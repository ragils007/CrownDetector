using System.Collections.Generic;
using System.IO;
using Msdfa.Core.Entities;
using Msdfa.Tools.DataAccess;

namespace Msdfa.Core.Tools.DataAccess.Byte
{
    public class ByteReader<T> : BaseDataReader<T> where T : new()
    {
        public ByteReader(StreamReader streamReader, bool isFirstRowHeader)
            : base(streamReader, "", "\t", isFirstRowHeader)
        {
        }

        public override List<T> ReadAll(ProgressReport pr = null) => ReadAllFromStream(pr);
    }
}
