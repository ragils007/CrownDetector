using Msdfa.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Msdfa.Core.DB
{
    public interface IConnection
    {
        public string Ip { get; }
        public string Sid { get; }
        public long Port { get; }
        public string UserName { get; }
        public string Pass { get; }

        IDatabase Db { get; }
        DatabaseType DatabaseType { get; }

        DataTable Fetch(string sql, int commandTimeout, params object[] args);
        IQuery Query(string queryString);
        QueryTyped<T> Query<T>(string queryString) where T : class, new();

        string DbDescription { get; }

        void Dispose();
    }
}
