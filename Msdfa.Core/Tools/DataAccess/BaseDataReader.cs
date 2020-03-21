using Msdfa.Core.Entities;
using Msdfa.Core.Tools.DataAccess;
using Msdfa.Tools.ExpressionTrees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Msdfa.Tools.DataAccess
{
    public abstract class BaseDataReader<T> : IDataReader<T> where T : new()
    {
        #region Protected Fields

        protected List<string> _columnNames;
        protected string _filePath;
        protected bool _isFirstRowHeader;
        protected Dictionary<int, BaseDataReaderColumnInfo> _mappedProperties;
        protected string _seperator;
        protected StreamReader _streamReader;
        protected BaseDataReaderParser<T> DataParser;

        public List<string> Errors { get; set; } = new List<string>();

        #endregion Protected Fields

        #region Constructors and initialization

        protected BaseDataReader(StreamReader streamReader, string filePath, string seperator, bool isFirstRowHeader)
        {
            _streamReader = streamReader;
            _filePath = filePath;
            _seperator = seperator;
            _isFirstRowHeader = isFirstRowHeader;
            _mappedProperties = new Dictionary<int, BaseDataReaderColumnInfo>();
            DataParser = new BaseDataReaderParser<T>();
            DataParser.Init(_mappedProperties);
        }

        protected static List<string> GetColumnNames(StreamReader streamReader, string seperator)
        {
            var line = streamReader.ReadLine();
            var columnNames = line.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            return columnNames.ToList();
        }

        #endregion Constructors and initialization

        #region IDataReader

        public virtual void AutoMap()
        {
            if (!_isFirstRowHeader)
                throw new Exception("Unable to automap. First row is not header.");

            var properties = typeof(T).GetProperties();
            foreach (var colName in _columnNames)
            {
                var property = properties.SingleOrDefault(x => x.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));

                if (property == null) continue;

                _mappedProperties.Add(_columnNames.IndexOf(colName), new BaseDataReaderColumnInfo { PropertyInfo = property });
            }
        }

        public void Close()
        {
            _streamReader.Close();
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }

        public virtual BaseDataReaderColumnInfo Map(string propertyName)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName);
            var columnInfo = new BaseDataReaderColumnInfo { PropertyInfo = propertyInfo };
            _mappedProperties.Add(_mappedProperties.Count, columnInfo);

            return columnInfo;
        }

        public virtual BaseDataReaderColumnInfo Map(Expression<Func<T, object>> property)
        {
            var propertyName = ExpressionDetail.Create(property).Name;
            return Map(propertyName);
        }

        public virtual IDataReader<T> Map(Expression<Func<T, object>> property, int columnIndex)
        {
            var propertyName = ExpressionDetail.Create(property).Name;
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName);
            _mappedProperties.Add(columnIndex, new BaseDataReaderColumnInfo { PropertyInfo = propertyInfo });

            return this;
        }

        public virtual T Read()
        {
            if (_streamReader.EndOfStream)
                return default(T);

            var line = _streamReader.ReadLine();
            var obj = CreateObjectFromLine(line);

            return obj;
        }

        public virtual List<T> ReadAll(ProgressReport pr = null)
        {
            using (var streamReader = new StreamReader(_filePath))
            {
                if (_isFirstRowHeader) streamReader.ReadLine(); //ignore first line if is header

                var data = streamReader.ReadToEnd();
                var lines = GetDataLines(data);

                var res = new List<T>();

                for (var i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        res.Add(CreateObjectFromLine(lines[i]));
                    }
                    catch (Exception e)
                    {
                        var items = e.Message.Split('|').ToList();
                        items.ForEach(x => Errors.Add($@"[{i + 1}]: {x}"));
                    }
                }
                if (Errors.Any())
                {
                    Errors.Insert(0, "---------=[ Błędy wczytywania pliku ]=---------");
                    Errors.Add("---------=[ Koniec wczytywania pliku ]=---------");
                    Errors.Add("");
                }

                return res;
            }
        }

        public async Task<List<T>> ReadAllAsync()
        {
            var result = await Task.Run(() => ReadAll());
            return result;
        }

        public List<T> ReadAllFromStream(ProgressReport pr = null)
        {
            _streamReader.BaseStream.Position = 0;

            if (_isFirstRowHeader) _streamReader.ReadLine(); //ignore first line if is header

            var data = _streamReader.ReadToEnd();
            var lines = GetDataLines(data);

            var res = new List<T>();

            for (var i = 0; i < lines.Length; i++)
            {
                try
                {
                    res.Add(CreateObjectFromLine(lines[i]));
                }
                catch (Exception e)
                {
                    var items = e.Message.Split('|').ToList();
                    items.ForEach(x => Errors.Add($@"[{i + 1}]: {x}"));
                }
            }
            if (Errors.Any())
            {
                Errors.Insert(0, "---------=[ Błędy wczytywania pliku ]=---------");
                Errors.Add("---------=[ Koniec wczytywania pliku ]=---------");
                Errors.Add(""); 
            }

            return res;
        }

        public virtual List<Object> ReadLine()
        {
            if (_streamReader.EndOfStream)
                return null;

            var line = _streamReader.ReadLine();
            var values = line.Split(new[] { _seperator }, StringSplitOptions.None);

            return values.Select(x => (Object)x).ToList();
        }
        #endregion IDataReader

        #region Methods

        protected virtual T CreateObjectFromLine(string line)
        {
            var values = line.Split(new[] { _seperator }, StringSplitOptions.None);
            var obj = DataParser.GetItemFromDataArray(values);
            return obj;
        }

        protected virtual string[] GetDataLines(string data)
        {
            var lines = data.Split('\n');
            var result = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    result.Add(lines[i].Replace("\r", ""));
            }

            return result.ToArray();
        }

        #endregion Methods
    }
}