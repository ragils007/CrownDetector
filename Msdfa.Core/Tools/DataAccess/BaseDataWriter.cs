using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Tools.DataAccess;
using Msdfa.Tools.ExpressionTrees;

namespace Msdfa.Tools.DataAccess
{
    public abstract class BaseDataWriter<T> : IDataWriter<T> where T : new()
    {
        #region Protected members

        protected StreamWriter _streamWriter;
        protected List<PropertyInfo> _propertiesToWrite;
        protected bool _wasColumnHeaderWritten;
        protected string _seperator;

        #endregion

        #region Constructors and Initializaition

        protected BaseDataWriter(StreamWriter streamWriter, string seperator)
        {
            _streamWriter = streamWriter;
            _seperator = seperator;
        }

        protected BaseDataWriter(string seperator)
        {
            _seperator = seperator;
        }

        #endregion

        #region IDataWriter

        public virtual void SetFullPath(string fullPath)
        {
            _streamWriter?.Dispose();
            AssertPathExists(fullPath);
            _streamWriter = new StreamWriter(fullPath);
        }

        public void MapProperties(params Expression<Func<T, object>>[] propertiesToWrite)
        {
            if (_propertiesToWrite != null)
                throw new Exception("Properties already mapped");

            _propertiesToWrite = new List<PropertyInfo>();
            var t = typeof(T);
            var properties = t.GetProperties();

            foreach (var prop in propertiesToWrite)
            {
                var propName = ExpressionDetail.Create(prop).Name;
                var property = properties.SingleOrDefault(x => x.Name == propName);

                _propertiesToWrite.Add(property);
            }
        }

        public virtual void WriteColumnHeader()
        {
            if (_wasColumnHeaderWritten) return;

            if (_propertiesToWrite == null)
                MapAllProperties();

            var propertyNames = _propertiesToWrite.Select(x => x.Name).ToList();
            var columnHeader = string.Join(_seperator, propertyNames);

            _streamWriter.WriteLine(columnHeader);
            _wasColumnHeaderWritten = true;
        }

        public virtual void WriteLine(T data)
        {
            if (_propertiesToWrite == null)
                MapAllProperties();

            var dataLine = GetDataLine(data, false);
            _streamWriter.WriteLine(dataLine);
        }

        public virtual async Task WriteLineAsync(T data)
        {
            await Task.Run(() => WriteLine(data));
        }

        public virtual void WriteLines(List<T> data)
        {
            if (_propertiesToWrite == null)
                MapAllProperties();

            var dataLines = data.Select(x => GetDataLine(x, true));
            var buffer = string.Join("", dataLines);

            _streamWriter.Write(buffer);
        }

        public virtual async Task WriteLinesAsync(List<T> data)
        {
            await Task.Run(() => WriteLines(data));
        }

        public void Close()
        {
            _streamWriter.Close();
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        #endregion

        #region Misc

        public static void AssertPathExists(string fullPath)
        {
            var path = Path.GetDirectoryName(fullPath);
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists) //create the path if doesnt exist yet
            {
                dirInfo.Create();
            }
        }

        protected virtual string GetDataLine(T obj, bool appendCrLf)
        {
            var result = new List<string>();
            foreach (var propToWrite in _propertiesToWrite)
            {
                var value = propToWrite.GetValue(obj);
                var stringValue = value?.ToString();
                result.Add(stringValue);
            }

            var dataLine = string.Join(_seperator, result);
            if (appendCrLf)
                dataLine += "\r\n";

            return dataLine;
        }

        protected void MapAllProperties()
        {
            if (_propertiesToWrite != null)
                throw new Exception("Properties already mapped");

            var t = typeof(T);
            _propertiesToWrite = t.GetProperties().ToList();
        }

        #endregion
    }
}
