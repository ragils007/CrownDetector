using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Msdfa.Core.DB;
using Msdfa.Core.Log;

namespace Msdfa.DB
{
    public class DatabaseManager
    {
        public IDatabase Db { get; set; }
        public Encoding Encoding;
        public DatabaseType Type;

        private bool HelperQuery = true;

        public string ClientInfo { get { return Db.ClientInfo; } set { Db.ClientInfo = value; } }


        public DatabaseManager()
        {
        }

        public DatabaseManager(DatabaseType DbType, string connectionString, bool connect = true, string debugMessage = null, bool helperQuery = false)
        {
            Type = DbType;
            switch (DbType)
            {
                case DatabaseType.MsSql:
                    Db = new MSSql(connectionString);
                    break;
                case DatabaseType.MySql:
                    Db = new Core.DB.MySql(connectionString);
                    break;
                case DatabaseType.Oracle:
                    Db = new Oracle(connectionString, connect, debugMessage);
                    break;
                case DatabaseType.Postgres:
                    this.Db = new Postgres(connectionString, connect, debugMessage);
                    break;
                case DatabaseType.SQLite:
                    Db = new SQLite(connectionString);
                    break;
            }

            HelperQuery = helperQuery;
        }

        public DatabaseManager(IDatabase database)
        {
            Db = database;
        }

        public DatabaseManager(string DbType, string connectionString)
        {
            switch (DbType.ToLower())
            {
                case "mssql":
                    Type = DatabaseType.MsSql;
                    Db = new MSSql(connectionString);
                    break;
                case "mysql":
                    Type = DatabaseType.MySql;
                    Db = new Core.DB.MySql(connectionString);
                    break;
                case "oracle":
                    Type = DatabaseType.Oracle;
                    Db = new Oracle(connectionString);
                    break;
                case "sqlite":
                    Type = DatabaseType.SQLite;
                    Db = new SQLite(connectionString);
                    break;
                default:
                    throw new Exception("Database not supported");
            }
        }

        public ConnectionState State
        {
            get { return Db.State; }
        }

        public bool IsInTransaction
        {
            get { return Db.IsInTransaction; }
        }

        public bool IsDisposed
        {
            get { return Db.IsDisposed; }
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public void TransactionBegin()
        {
            Logger.Info("[" + Type + ": TransactionBegin()]");
            Db.BeginTransaction();
        }

        public void TransactionCommit()
        {
            Logger.Info("[" + Type + ": TransactionCommit()]");
            Db.CommitTransaction();
        }

        public void TransactionRollback()
        {
            Logger.Info("[" + Type + ": TransactionRollback()]");
            Db.RollbackTransaction();
        }

        public int Execute(string sql, params object[] args)
        {
            Logger.Info("Execute: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = Db.Execute(sql, args);
            Logger.Info("Execute: Done. (" + res + ")");
            return res;
        }

        public void ExecuteProcedure(string procedureName, params object[] args)
        {
            Logger.Info("ExecuteProcedure: " + DisplayArgs(args) + MakeSingleLineString(procedureName));
            Db.ExecuteProcedure(procedureName, args);
            Logger.Info("ExecuteProcedure: Done.");
        }

        public T ExecuteFunction<T>(string functionName, params object[] args)
        {
            T retutnValue;
            Logger.Info("ExecuteFunction: " + DisplayArgs(args) + MakeSingleLineString(functionName));
            retutnValue = Db.ExecuteFunction<T>(functionName, args);
            Logger.Info("ExecuteFunction: Done.");
            return retutnValue;
        }


        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            Logger.Info("ExecuteReturning: " + query.DisplayArgs() + MakeSingleLineString(query.QueryString));
            var res = Db.ExecuteReturning(query);
            Logger.Info("Execute: Done. (" + res + ")");
            return res;
        }

        public Dictionary<string, object> InsertReturning(Query query)
        {
            Logger.Info("Insert: " + query.DisplayArgs() + MakeSingleLineString(query.QueryString));
            var res = Db.InsertReturning(query);
            Logger.Info("Insert: Done. (" + res + ")");
            return res;
        }

        public int Insert(string sql, params object[] args)
        {
            Logger.Info("Insert: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = Db.Insert(sql, args);
            Logger.Info("Insert: Done. (" + res + ")");
            return res;
        }

        public DataTable Fetch(string sql, params object[] args)
        {
            Logger.Info("Fetch: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = Db.Fetch(sql, args);
            Logger.Info("Fetch: Done. (" + res.Rows.Count + " rows)");
            return res;
        }

        public DataTable Fetch(string sql, int commandTimeout, params object[] args)
        {
            Logger.Info($"Fetch:  { DisplayArgs(args)}  { MakeSingleLineString(sql)} commandTimeout = {commandTimeout.ToString()}");
            DisplayHelperQuery(sql, args);
            var res = Db.Fetch(sql, commandTimeout, args);
            Logger.Info("Fetch: Done. (" + res.Rows.Count + " rows)");
            return res;
        }

        public async Task<DataTable> FetchAsync(string sql, CancellationToken cancellationToken, params object[] args)
        {
            Logger.Info("Fetch: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = await Db.FetchAsync(sql, cancellationToken, args).ConfigureAwait(false);
            Logger.Info("Fetch: Done. (" + res.Rows.Count + " rows)");
            return res;
        }


        public DataRow FetchRow(string sql, params object[] args)
        {
            Logger.Info("FetchRow: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = Db.FetchRow(sql, args);
            Logger.Info("FetchRow: Done. " + (res == null ? "(null)" : ""));
            return res;
        }

        public IDataReader FetchCursor(string sql, params object[] args)
        {
            return Db.FetchCursor(sql, args);
        }

        public object FetchValue(string sql, params object[] args)
        {
            Logger.Info("FetchValue: " + DisplayArgs(args) + MakeSingleLineString(sql));
            DisplayHelperQuery(sql, args);
            var res = Db.FetchValue(sql, args);
            Logger.Info("FetchValue: Done. " + (res == null ? "(null)" : ""));
            return res;
        }

        private string MakeSingleLineString(string input)
        {
            return Regex.Replace(input, @"[\s]+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public T FetchValue<T>(string sql, params object[] args)
        {
            return (T)FetchValue(sql, args);
        }

        private string DisplayArgs(params object[] args)
        {
            if (args.Length == 0) return "";
            if (args[0].GetType() == typeof(Dictionary<string, object>))
            {
                return BindValuesString((Dictionary<string, object>)args[0]);
            }
            return "['" + string.Join("', '", args) + "'] ";
        }

        private void DisplayHelperQuery(string sql, params object[] args)
        {
            if (!HelperQuery)
                return;

            var text = "******* DEBUG QUERY START *******\n";

            if (args.Any() && args[0].GetType() == typeof(Dictionary<string, object>))
            {
                var dict = (Dictionary<string, object>)args[0];
                var defines = dict.Select(x => $"define {x.Key.Replace(":", "")} = {x.Value}");
                var definesLines = string.Join("\n", defines);

                text += definesLines + "\n";
                text += sql.Replace(":", "&") + "\n";
            }
            else
            {
                text += "['" + string.Join("', '", args) + "']\n";
                text += sql + "\n";
            }

            text += "******* DEBUG QUERY END *******\n";
            Logger.Debug(text);
        }

        private string BindValuesString(Dictionary<string, object> dict)
        {
            return dict.Count > 0
                ? "[" + string.Join(", ", dict.Select(x => "'" + x.Key + "'->'" + x.Value + "'").ToArray()) + "] "
                : "";
        }

        public IQuery Query(string queryString)
        {
            return new Query(this.Db, queryString);
        }

        public IStoredProcedure StoredProcedure(string procedureName)
        {
            return new StoredProcedure(this.Db, procedureName);
        }

        public QueryTyped<TDataType> Query<TDataType>(string queryString)
            where TDataType : class, new()
        {
            return new QueryTyped<TDataType>(this.Db, queryString);
        }

        public IQuery QueryInsert(Query.QueryType type, string tableName)
        {
            return new Query(this.Db).SetQueryType(type, tableName);
        }

        public DateTime GetDBTime()
        {
            return this.Db.GetDBTime();
        }
    }
}