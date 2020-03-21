using Msdfa.Core.DB;
using Msdfa.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Msdfa.Core.DB
{
    public abstract class ConnectionBase : IConnection
    {
        public abstract string ConnectionString { get; }
        public abstract Msdfa.DatabaseType DatabaseType { get; }

        public abstract string Ip { get; }
        public abstract string Sid { get; }
        public abstract long Port { get; }
        public abstract string UserName { get; }
        public abstract string Pass { get; }

        public virtual long MaxPoolSize { get; set; } = 10;
        public virtual long MinPoolSize { get; set; } = 5;

        public virtual string DefaultSchema { get; set; } = null;

        public string DbDescription => $"{this.DatabaseType.ToString()}://{this.UserName}@{this.Ip}:{this.Port}/{this.Sid}";

        public IDatabase Db { get; }

        public ConnectionBase()
        {
            if (this.ConnectionString == null) throw new Exception("ConnectionString is null");
            if (this.DatabaseType == Msdfa.DatabaseType.None) throw new Exception("DatabaseType is set to <None>");

            var man = new DatabaseManager(this.DatabaseType, this.ConnectionString);
            this.Db = man.Db;
        }

        public DataTable Fetch(string sql, int commandTimeout, params object[] args) => this.Db.Fetch(sql, args);
        public IQuery Query(string queryString) => new Query(this.Db, queryString);
        public QueryTyped<T> Query<T>(string queryString) where T : class, new() => new QueryTyped<T>(this.Db, queryString);

        public void TransactionBegin() => this.Db.BeginTransaction();
        public void TransactionCommit() => this.Db.CommitTransaction();
        public void TransactionRollback() => this.Db.RollbackTransaction();

        public void Dispose()
        {
            this.Db.CloseConnection();
        }
    }
}
