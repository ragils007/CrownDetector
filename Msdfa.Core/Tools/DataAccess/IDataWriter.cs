using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Msdfa.Core.Tools.DataAccess;

namespace Msdfa.Tools.DataAccess
{
    public interface IDataWriter<T> : IDisposable where T : new()
    {
        /// <summary>
        /// Set the full path for the file.
        /// If a previous path existed, the writer will close the stream and open a new one.
        /// </summary>
        /// <param name="fullPath"></param>
        void SetFullPath(string fullPath);

        /// <summary>
        /// Specify which properties should be written to file. If not invoked, all properties will be saved.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        void MapProperties(params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Writes the column header to file.
        /// </summary>
        void WriteColumnHeader();

        /// <summary>
        /// Writes the object to file.
        /// </summary>
        /// <param name="data"></param>
        void WriteLine(T data);

        /// <summary>
        /// Writes the object to file.
        /// </summary>
        /// <param name="data"></param>
        Task WriteLineAsync(T data);

        /// <summary>
        /// Writes the whole List to the file.
        /// </summary>
        /// <param name="data"></param>
        void WriteLines(List<T> data);

        /// <summary>
        /// Writes the whole list to the file.
        /// </summary>
        /// <param name="data"></param>
        Task WriteLinesAsync(List<T> data);

        /// <summary>
        /// Closes the stream.
        /// </summary>
        void Close();
    }
}
