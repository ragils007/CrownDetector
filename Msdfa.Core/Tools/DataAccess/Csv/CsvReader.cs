using Msdfa.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Msdfa.Tools.DataAccess.Csv
{
    public class CsvReader<T> : BaseDataReader<T> where T : new()
    {
        #region Constructing

        private CsvReader(StreamReader streamReader, bool isFirstRowHeader, string filePath)
            :base(streamReader, filePath, ";", isFirstRowHeader)
        {
        }

        public static CsvReader<T> Create(string filePath, bool isFirstRowHeader)
        {
            var streamReader = new StreamReader(filePath);

            var csvReader = new CsvReader<T>(streamReader, isFirstRowHeader, filePath);

            if (isFirstRowHeader)
                csvReader._columnNames = GetColumnNames(streamReader, ";");

            return csvReader;
        }

        public static CsvReader<List<Object>> CreateLineReader(string filePath, Encoding encoding = null)
        {
            if(encoding == null)
                encoding = TextFileEncodingDetector.DetectTextFileEncoding(filePath);

            var streamReader = new StreamReader(filePath, encoding);

            var csvReader = new CsvReader<List<Object>>(streamReader, false, filePath);

            return csvReader;
        }

        #endregion
    }
}