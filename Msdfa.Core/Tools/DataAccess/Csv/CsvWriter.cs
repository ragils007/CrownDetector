using System.IO;

namespace Msdfa.Tools.DataAccess.Csv
{
    public class CsvWriter<T> : BaseDataWriter<T> where T : new()
    {
        #region Constructors and Iinitialization

        public CsvWriter()
            : base(";")
        {}

        public CsvWriter(StreamWriter streamWriter)
            :base(streamWriter, ";")
        {}
        
        public static CsvWriter<T> Create(string filePath)
        {
            AssertPathExists(filePath);
            var streamWriter = new StreamWriter(filePath);
            var csvWriter = new CsvWriter<T>(streamWriter);
            return csvWriter;
        }

        public static CsvWriter<T> Create()
        {
            return new CsvWriter<T>();
        }

        #endregion
    }
}