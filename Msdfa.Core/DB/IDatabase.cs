using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Msdfa.DB
{
    public interface IDatabase
    {
        ConnectionState State { get; }
        bool IsInTransaction { get; }
        //int TransactionCommitEvery { get; set; }
        //IDbConnection Cnn { get; }
        bool IsDisposed { get; }
        DatabaseType DatabaseType { get; }

        string ClientInfo { get; set; }

        void CloseConnection();
        void OpenConnection();
        DataTable Fetch(string sql, params object[] args);
        Task<DataTable> FetchAsync(string sql, CancellationToken cancellationToken, params object[] args);
        DataRow FetchRow(string sql, params object[] args);
        IDataReader FetchCursor(string sql, params object[] args);
        object FetchValue(string sql, params object[] args);
        int Execute(string sql, params object[] args);
        void ExecuteProcedure(string procedureName, params object[] args);
        T ExecuteFunction<T>(string procedureName, params object[] args);
        int Insert(string sql, params object[] args);
        Dictionary<string, object> InsertReturning(Query query);
        Dictionary<string, object> ExecuteReturning(Query query);
        //Dictionary<string, object> InsertReturning(string sql, Dictionary<string, object> args, Dictionary<string, Type> returningColumns);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void Dispose();
        //void LockTable(string tableName);
        //int CommandTimeout { get; set; }
        DateTime GetDBTime();
    }
}