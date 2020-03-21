using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools.DataAccess.Txt
{
    public class TxtReader<T> : BaseDataReader<T> where T : new()
    {
        private TxtReader(StreamReader streamReader, bool isFirstRowHeader, string filePath) 
            : base(streamReader, filePath, "\t", isFirstRowHeader)
        {
        }

        public static TxtReader<T> Create(string filePath, bool isFirstRowHeader)
        {
            var streamReader = new StreamReader(filePath);

            var txtReader = new TxtReader<T>(streamReader, isFirstRowHeader, filePath);

            if (isFirstRowHeader)
                txtReader._columnNames = GetColumnNames(streamReader, "\t");

            return txtReader;
        }

        public static TxtReader<List<Object>> CreateLineReader(string filePath)
        {
            var streamReader = new StreamReader(filePath);

            var txtReader = new TxtReader<List<Object>>(streamReader, false, filePath);

            return txtReader;
        }
    }
}