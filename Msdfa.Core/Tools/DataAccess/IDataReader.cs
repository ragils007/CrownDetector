using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Msdfa.Core.Entities;
using Msdfa.Core.Tools.DataAccess;

namespace Msdfa.Tools.DataAccess
{
    public interface IDataReader<T> : IDisposable where T : new()
    {
        List<string> Errors { get; set; } 

        void AutoMap();

        IDataReader<T> Map(Expression<Func<T, object>> property, int columnIndex);

        BaseDataReaderColumnInfo Map(string propertyName);
        BaseDataReaderColumnInfo Map(Expression<Func<T, object>> property);
        //IDataReader<T> MapDefault<TValue>(Expression<Func<T, TValue>> property, TValue defaultValue);

        /// <summary>
        /// Reads the next row.
        /// </summary>
        /// <returns>The next row or NULL if no other row found.</returns>
        T Read();

        /// <summary>
        /// Returns the whole sheet file.
        /// </summary>
        /// <returns></returns>
        List<T> ReadAll(ProgressReport pr = null);

        /// <summary>
        /// Return the whole sheet file.
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ReadAllAsync();

        /// <summary>
        /// Closes stream.
        /// </summary>
        void Close();

        /// <summary>
        /// Returm row as list object
        /// </summary>
        /// <returns></returns>
        List<Object> ReadLine();
    }
}