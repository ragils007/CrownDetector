using System.IO;

namespace Msdfa.Tools.DataAccess
{
    class DataReader<T> : BaseDataReader<T> where T : new()
    {
        #region Constructors and Initialization

        private DataReader(StreamReader streamReader, string filePath, string seperator, bool isFirstRowHeader)
            :base(streamReader, filePath, seperator, isFirstRowHeader)
        {
        }

        public static DataReader<T> Create(string filePath, string seperator, bool isFirstRowHeader)
        {
            var streamReader = new StreamReader(filePath);
            var txtReader = new DataReader<T>(streamReader, filePath, seperator, isFirstRowHeader);

            if (isFirstRowHeader)
            {
                txtReader._columnNames = GetColumnNames(streamReader, seperator);
            }

            return txtReader;
        }
        
        #endregion
    }
}
