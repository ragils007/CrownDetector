using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msdfa.DB;
using MySql.Data.MySqlClient;

namespace Msdfa.Core.DB
{
    public class MySql : IDisposable, IDatabase
    {
        public DatabaseType DatabaseType => DatabaseType.MySql;

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


        public IDbConnection Cnn => cnn;

        public bool IsDisposed { get; private set; }

        public bool IsConnected { get; set; }

        public bool IsInTransaction { get; set; }

        public int TransactionCommitEvery { get; set; }

        private MySqlTransaction trans;

        public ConnectionState State => cnn.State;

        public MySqlConnection cnn;

        public int CommandTimeout { get; set; }
        public string ClientInfo { get; set; }

        public MySql(string connectionString)
        {
            Connect(connectionString);
        }

        public MySql(string dbIp, string dbName, string dbLogin, string dbPassword)
        {
            Connect($"Server={dbIp};Database={dbName};Uid={dbLogin};Pwd={dbPassword};");
        }

        public void BeginTransaction()
        {
            trans = cnn.BeginTransaction(IsolationLevel.ReadCommitted);
            IsInTransaction = true;
        }

        public void CommitTransaction()
        {
            if (trans != null)
            {
                trans.Commit();
                IsInTransaction = false;
            }
        }

        public void RollbackTransaction()
        {
            if (trans != null)
            {
                trans.Rollback();
                IsInTransaction = false;
            }
        }

        private void Connect(string connectionString)
        {
            IsConnected = false;
            IsInTransaction = false;
            cnn = new MySqlConnection(connectionString);

            try
            {
                cnn.Open();
                IsConnected = true;
            }
            catch (MySqlException e)
            {
                IsConnected = false;
                throw new Exception(e.Message);
                //System.Windows.Forms.MessageBox.Show("Error: " + e);
            }
        }

        public DataTable Fetch(string query, params object[] args)
        {
            var myTable = new DataTable();

            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (var cmd = new MySqlCommand(query, cnn))
            {
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                ProcessParameters(cmd, args);
                //MySqlParameter parameter = new MySqlParameter("@" + paramNo.ToString(), arg.GetType());
                //parameter.Value = arg;
                //cmd.Parameters.Add(parameter);
                //paramNo++;
                //}
                using (var myDataAdp = new MySqlDataAdapter(cmd))
                {
                    using (var myCmdBld = new MySqlCommandBuilder(myDataAdp))
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
            var myTable = new DataTable();

            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (var cmd = new MySqlCommand(query, cnn))
            {
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                ProcessParameters(cmd, args);
                //MySqlParameter parameter = new MySqlParameter("@" + paramNo.ToString(), arg.GetType());
                //parameter.Value = arg;
                //cmd.Parameters.Add(parameter);
                //paramNo++;
                //}
                using (var myDataAdp = new MySqlDataAdapter(cmd))
                {
                    using (var myCmdBld = new MySqlCommandBuilder(myDataAdp))
                    {
                        await myDataAdp.FillAsync(myTable, cancellationToken);
                    }
                }
            }
            return myTable;
        }

        public DataRow FetchRow(string query, params object[] args)
        {
            var myTable = new DataTable();

            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (var cmd = new MySqlCommand(query, cnn))
            {
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                ProcessParameters(cmd, args);
                //MySqlParameter parameter = new MySqlParameter("@" + paramNo.ToString(), arg.GetType());
                //parameter.Value = arg;
                //cmd.Parameters.Add(parameter);
                //paramNo++;
                //}
                using (var myDataAdp = new MySqlDataAdapter(cmd))
                {
                    using (new MySqlCommandBuilder(myDataAdp))
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
            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (var cmd = new MySqlCommand(query, cnn))
            {
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                ProcessParameters(cmd, args);
                //MySqlParameter parameter = new MySqlParameter("@" + paramNo.ToString(), arg.GetType());
                //parameter.Value = arg;
                //cmd.Parameters.Add(parameter);
                //paramNo++;
                //}
                return cmd.ExecuteScalar();
            }
        }

        public IDataReader FetchCursor(string query, params object[] args)
        {
            if (cnn.State.ToString() == "Closed") cnn.Open();

            using (var cmd = new MySqlCommand(query, cnn))
            {
                //int paramNo = 0;
                //foreach (object arg in args)
                //{
                ProcessParameters(cmd, args);
                //MySqlParameter parameter = new MySqlParameter("@" + paramNo.ToString(), arg.GetType());
                //parameter.Value = arg;
                //cmd.Parameters.Add(parameter);
                //paramNo++;
                //}
                return cmd.ExecuteReader();
            }
        }

        public int Insert(string query, params object[] args)
        {
            return Execute(query, args);
        }

        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> InsertReturning(Query query)
        {
            throw new NotImplementedException();
        }


        //public void Execute(string sql)
        //{
        //   if (this.cnn.State.ToString() == "Closed")
        //      this.cnn.Open();

        //   using (MySqlCommand com = new MySqlCommand())
        //   {
        //      com.CommandText = sql;
        //      com.Connection = this.cnn;

        //      com.ExecuteNonQuery();
        //   }
        //}

        /// <summary>
        /// Wersja metody execute, przyjmująca jako argumenty parametry.
        /// Użycie parametrów w zapytaniu: SELECT * FROM foo WHERE id=@0 and col1=@1...
        /// </summary>
        /// <param name="query">Zapytanie SQL</param>
        /// <param name="args">Argumenty</param>
        /// <returns></returns>
        public int Execute(string query, params object[] args)
        {
            if (cnn.State.ToString() == "Closed")
                cnn.Open();

            using (var cmd = new MySqlCommand(query, cnn))
            {
                ProcessParameters(cmd, args);
                if (query.StartsWith("INSERT"))
                {
                    cmd.CommandText += "; SELECT LAST_INSERT_ID()";
                }
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void ExecuteProcedure(string procedureName, params object[] args)
        {
            throw new Exception("Not implemented!");
        }

        public T ExecuteFunction<T>(string procedureName, params object[] args)
        {
            throw new Exception("Not implemented!");
        }

        private void ProcessParameters(MySqlCommand cmd, params object[] args)
        {
            if (args.Length > 0)
            {
                /*
				 * Podmieniamy parametry ':' na '@', ale tylko dla parametrów poza stringami
				 */
                var isInString = false;
                var newCommand = new StringBuilder(cmd.CommandText.Length);
                foreach (var t in cmd.CommandText)
                {
                    if (t == '\'') isInString = (!isInString);
                    newCommand.Append((!isInString && t == ':') ? '@' : t);
                }
                cmd.CommandText = newCommand.ToString();

                if (args[0].GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (var dict in (Dictionary<string, object>)args[0])
                    {
                        var param = new MySqlParameter("@" + dict.Key, GetMySqlDbType(args.GetType()), 100);
                        param.Value = dict.Value;
                        cmd.Parameters.Add(param);
                    }
                }
                else if (args[0].GetType() == typeof(Dictionary<object, object>))
                {
                    foreach (var dict in (Dictionary<object, object>)args[0])
                    {
                        var param = new MySqlParameter("@" + dict.Key,
                            GetMySqlDbType(args.GetType()), 100);
                        param.Value = dict.Value;
                        cmd.Parameters.Add(param);
                    }
                }
                else
                {
                    var paramNo = 0;
                    foreach (var arg in args)
                    {
                        var parameter = new MySqlParameter("@" + paramNo,
                            GetMySqlDbType(args.GetType()), 100);
                        parameter.Value = arg;
                        cmd.Parameters.Add(parameter);
                        paramNo++;
                    }
                }
            }
        }

        public void Close() => this.CloseConnection();

        public void Dispose()
        {
            this.CloseConnection();
            IsDisposed = true;
            cnn.Dispose();
        }


        private MySqlDbType GetMySqlDbType(object o)
        {
            if (o is string) return MySqlDbType.VarChar;
            if (o is DateTime) return MySqlDbType.Date;
            if (o is Int64) return MySqlDbType.Int64;
            if (o is Int32) return MySqlDbType.Int32;
            if (o is Int16) return MySqlDbType.Int16;
            if (o is byte) return MySqlDbType.Byte;
            if (o is decimal) return MySqlDbType.Decimal;
            if (o is float) return MySqlDbType.Float;
            if (o is double) return MySqlDbType.Double;
            if (o is byte[]) return MySqlDbType.Blob;
            return MySqlDbType.VarChar;
        }

        public void LockTable(string tableName)
        {
            throw new Exception("Not implemented.");
        }

        public DateTime GetDBTime()
        {
            throw new NotImplementedException();
        }

        //public IDatabase Query(string query)
        //{
        //   throw new Exception("Not implemented!");
        //}
        //public IDatabase Bind(string varKey, object varVal)
        //{
        //   throw new Exception("Not implemented!");
        //}
        //public DataTable FetchAll()
        //{
        //   throw new Exception("Not implemented!");
        //}
        //public DataRow FetchRow()
        //{
        //   throw new Exception("Not implemented!");
        //}
        //public IDataReader FetchCursor()
        //{
        //   throw new Exception("Not implemented!");
        //}
        //public int Execute()
        //{
        //   throw new Exception("Not implemented!");
        //}

    }
}
