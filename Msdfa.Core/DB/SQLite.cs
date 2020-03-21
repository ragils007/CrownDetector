using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace Msdfa.DB
{
    /// 
    /// Description of SQLite.
    /// 
    public class SQLite : IDisposable, IDatabase
    {
        public DatabaseType DatabaseType => DatabaseType.SQLite;

        public static event EventHandler ActiveConnectionsChanged;
        private static void OnActiveConnectionsChanged() => ActiveConnectionsChanged?.Invoke(null, EventArgs.Empty);
        public static int ActiveConnections;

        public void CloseConnection()
        {
            if (this.Cnn.State == ConnectionState.Open)
            {
                this.Cnn.Close();
                Interlocked.Decrement(ref ActiveConnections);
                OnActiveConnectionsChanged();
            }
        }

        public void OpenConnection()
        {
            if (this.Cnn.State != ConnectionState.Open)
            {
                this.Cnn.Open();
                Interlocked.Increment(ref ActiveConnections);
                OnActiveConnectionsChanged();
            }
        }


        public IDbConnection Cnn
        {
            get
            {
                return this.cnn;
            }
        }

        public SQLiteConnection cnn;
        public bool IsInTransaction { get; set; }
        public int TransactionCommitEvery { get; set; }
        public bool IsDisposed { get; private set; }

        public int CommandTimeout { get; set; }
        public string ClientInfo { get; set; }

        public ConnectionState State
        {
            get
            {
                return this.cnn.State;
            }
        }

        public SQLite()
        {
            this.Init("database.db3");
        }
        public SQLite(string dbName)
        {
            this.Init(dbName);
        }

        private void Init(string dbName)
        {
            this.cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbName));
            this.IsInTransaction = false;
            try
            {
                this.cnn.Open();
            }
            catch (SQLiteException e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable Fetch(string query, params object[] args)
        {
            DataTable myTable = new DataTable();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
            {
                //int paramNo = 0;
                this.ProcessParameters(cmd, args);
                //foreach (object arg in args)
                //{
                //   SQLiteParameter parameter = new SQLiteParameter("@" + paramNo.ToString(), arg.GetType());
                //   parameter.Value = arg;
                //   cmd.Parameters.Add(parameter);
                //   paramNo++;
                //}
                using (SQLiteDataAdapter myDataAdp = new SQLiteDataAdapter(cmd))
                {
                    using (SQLiteCommandBuilder myCmdBld = new SQLiteCommandBuilder(myDataAdp))
                    {
                        myDataAdp.Fill(myTable);
                    }
                }
            }
            return myTable;
        }

        public DataTable Fetch(string query, int commandTimeout, params object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> FetchAsync(string query, CancellationToken cancellationToken, params object[] args)
        {
            // TODO: token nie wykorzystywany, metoda pisana na szybko
            return await Task.Run(() =>
            {
                DataTable myTable = new DataTable();

                if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
                {
                    //int paramNo = 0;
                    this.ProcessParameters(cmd, args);
                    //foreach (object arg in args)
                    //{
                    //   SQLiteParameter parameter = new SQLiteParameter("@" + paramNo.ToString(), arg.GetType());
                    //   parameter.Value = arg;
                    //   cmd.Parameters.Add(parameter);
                    //   paramNo++;
                    //}
                    using (SQLiteDataAdapter myDataAdp = new SQLiteDataAdapter(cmd))
                    {
                        using (SQLiteCommandBuilder myCmdBld = new SQLiteCommandBuilder(myDataAdp))
                        {
                            myDataAdp.Fill(myTable);
                        }
                    }
                }
                return myTable;
            }, cancellationToken);
        }

        public DataRow FetchRow(string query, params object[] args)
        {
            DataTable myTable = new DataTable();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
            {
                this.ProcessParameters(cmd, args);
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                //   SQLiteParameter parameter = new SQLiteParameter("@" + paramNo.ToString(), arg.GetType());
                //   parameter.Value = arg;
                //   cmd.Parameters.Add(parameter);
                //   paramNo++;
                //}
                using (SQLiteDataAdapter myDataAdp = new SQLiteDataAdapter(cmd))
                {
                    using (SQLiteCommandBuilder myCmdBld = new SQLiteCommandBuilder(myDataAdp))
                    {
                        myDataAdp.Fill(myTable);
                    }
                }
            }
            if (myTable.Rows.Count > 0) return myTable.Rows[0];
            else return null;
        }
        public object FetchValue(string query, params object[] args)
        {
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
            {
                this.ProcessParameters(cmd, args);
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                //   SQLiteParameter parameter = new SQLiteParameter("@" + paramNo.ToString(), arg.GetType());
                //   parameter.Value = arg;
                //   cmd.Parameters.Add(parameter);
                //   paramNo++;
                //}
                return cmd.ExecuteScalar();
            }
        }
        public IDataReader FetchCursor(string query, params object[] args)
        {
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
            {
                this.ProcessParameters(cmd, args);
                return cmd.ExecuteReader();
            }
        }

        public int Insert(string query, params object[] args)
        {
            return this.Execute(query, args);
        }
        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            throw new NotImplementedException();
        }
        public Dictionary<string, object> InsertReturning(Query query)
        {
            throw new NotImplementedException();
        }

        public void Execute(string sql)
        {
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            using (SQLiteCommand com = new SQLiteCommand())
            {
                com.CommandText = sql;
                com.Connection = this.cnn;
                com.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Wersja metody execute, przyjmująca jako argumenty parametry.
        /// Użycie parametrów w zapytaniu: SELECT * FROM foo WHERE id=@0 and col1=@1...
        /// </summary>
        /// <param name="query">Zapytanie SQL</param>
        /// <param name="args">Argumenty</param>
        /// <returns></returns>
        public int Execute(string query, params object[] args)
        {
            if (this.cnn.State.ToString() == "Closed")
                this.cnn.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(query, this.cnn))
            {
                this.ProcessParameters(cmd, args);
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                //   SQLiteParameter parameter = new SQLiteParameter("@" + paramNo.ToString(), arg.GetType());
                //   parameter.Value = arg;
                //   cmd.Parameters.Add(parameter);
                //   paramNo++;
                //}
                return cmd.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedRowId()
        {
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = cnn;
                cmd.CommandText = "SELECT last_insert_rowid();";
                object val = cmd.ExecuteScalar();
                if (val == null) return -1;
                return (int.Parse(val.ToString()));
            }
        }

        private void ProcessParameters(SQLiteCommand cmd, params object[] args)
        {
            if (args.Length > 0)
            {
                /**
				 * Dla argumentów przekazywanych w formie: Dictionary<string, object>
				 */
                if (args[0].GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (KeyValuePair<string, object> dict in (Dictionary<string, object>)args[0])
                    {
                        SQLiteParameter param = new SQLiteParameter(dict.Key, dict.Value.GetType());
                        param.Value = dict.Value;
                        cmd.Parameters.Add(param);
                    }
                }
                /**
				 * Dla argumentów przekazywanych w formie: Dictionary<Enum, object>
				 */
                else if (args[0].GetType() == typeof(Dictionary<object, object>))
                {
                    foreach (KeyValuePair<object, object> dict in (Dictionary<object, object>)args[0])
                    {
                        SQLiteParameter param = new SQLiteParameter(dict.Key.ToString(), dict.Value.GetType());
                        param.Value = dict.Value;
                        cmd.Parameters.Add(param);
                    }
                }
                else
                {
                    int paramNo = 0;
                    foreach (object arg in args)
                    {
                        SQLiteParameter parameter = new SQLiteParameter(paramNo.ToString(), arg.GetType());
                        parameter.Value = arg;
                        cmd.Parameters.Add(parameter);
                        paramNo++;
                    }
                }
            }
        }

        public void BeginTransaction()
        {
            this.Execute("BEGIN");
            this.IsInTransaction = true;
        }
        public void CommitTransaction()
        {
            this.Execute("COMMIT");
            this.IsInTransaction = false;
        }
        public void RollbackTransaction()
        {
            this.Execute("ROLLBACK");
            this.IsInTransaction = false;
        }

        public void Dispose()
        {
            this.CloseConnection();
            this.IsDisposed = true;
            this.cnn.Dispose();
        }

        public void Close() => this.CloseConnection();

        public IDatabase Query(string query)
        {
            throw new Exception("Not implemented!");
        }
        public IDatabase Bind(string varKey, object varVal)
        {
            throw new Exception("Not implemented!");
        }
        public DataTable FetchAll()
        {
            throw new Exception("Not implemented!");
        }
        public DataRow FetchRow()
        {
            throw new Exception("Not implemented!");
        }
        public IDataReader FetchCursor()
        {
            throw new Exception("Not implemented!");
        }
        public int Execute()
        {
            throw new Exception("Not implemented!");
        }

        public void ExecuteProcedure(string procedureName, params object[] args)
        {
            throw new Exception("Not implemented!");
        }

        public T ExecuteFunction<T>(string procedureName, params object[] args)
        {
            throw new Exception("Not implemented!");
        }

        public void LockTable(string tableName)
        {
            throw new Exception("Not implemented.");
        }

        public DateTime GetDBTime()
        {
            throw new NotImplementedException();
        }
    }
}
