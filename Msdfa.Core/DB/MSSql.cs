using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Msdfa.DB
{
    public class MSSql : IDisposable, IDatabase
    {
        public DatabaseType DatabaseType => DatabaseType.MsSql;

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
                return cnn;
            }
        }

        public ConnectionState State
        {
            get
            {
                return cnn.State;
            }
        }
        public bool IsDisposed { get; private set; }
        public SqlConnection cnn;
        public bool IsInTransaction { get; set; }
        public int TransactionCommitEvery { get; set; }
        private SqlTransaction trans;

        public int CommandTimeout { get; set; }
        public string ClientInfo { get; set; }

        public MSSql(string connectionString)
        {
            Connect(connectionString);
        }
        public MSSql(string dbIp, string dbName, string dbLogin, string dbPassword)
        {
            Connect($"Data Source = {dbIp}; Initial Catalog = {dbName}; User Id = {dbLogin}; Password = {dbPassword};");
        }

        private void Connect(string connectionString)
        {
            cnn = new SqlConnection(connectionString);
            IsInTransaction = false;
            try
            {
                cnn.Open();
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable Fetch(string query, params object[] args)
        {
            DataTable myTable = new DataTable();

            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (SqlCommand cmd = new SqlCommand(query, cnn))
            {
                ProcessParameters(cmd, args);
                using (SqlDataAdapter myDataAdp = new SqlDataAdapter(cmd))
                {
                    using (SqlCommandBuilder myCmdBld = new SqlCommandBuilder(myDataAdp))
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

                if (cnn.State.ToString() == "Closed") cnn.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnn))
                {
                    ProcessParameters(cmd, args);
                    using (SqlDataAdapter myDataAdp = new SqlDataAdapter(cmd))
                    {
                        using (SqlCommandBuilder myCmdBld = new SqlCommandBuilder(myDataAdp))
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

            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (SqlCommand cmd = new SqlCommand(query, cnn))
            {
                ProcessParameters(cmd, args);
                using (SqlDataAdapter myDataAdp = new SqlDataAdapter(cmd))
                {
                    using (SqlCommandBuilder myCmdBld = new SqlCommandBuilder(myDataAdp))
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
            using (SqlCommand cmd = new SqlCommand(query, cnn))
            {
                ProcessParameters(cmd, args);
                return cmd.ExecuteScalar();
            }
        }
        public IDataReader FetchCursor(string query, params object[] args)
        {
            if (cnn.State.ToString() == "Closed") cnn.Open();

            using (SqlCommand cmd = new SqlCommand(query, cnn))
            {
                ProcessParameters(cmd, args);
                return cmd.ExecuteReader();
            }
        }

        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            throw new NotImplementedException();
        }
        public Dictionary<string, object> InsertReturning(Query query)
        {
            throw new NotImplementedException();
        }
        public int Insert(string query, params object[] args)
        {
            return Execute(query, args);
        }

        public int Execute(string query, params object[] args)
        {
            if (cnn.State.ToString() == "Closed") cnn.Open();
            using (SqlCommand cmd = new SqlCommand(query, cnn))
            {
                if (trans != null) cmd.Transaction = trans;
                ProcessParameters(cmd, args);
                return cmd.ExecuteNonQuery();
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

        public void InsertOrUpdate(string sqlCondition, string sqlInsert, string sqlUpdate)
        {
            string query = string.Format(" IF NOT EXISTS ({0}) BEGIN {1} END ELSE BEGIN {2} END ", sqlCondition, sqlInsert, sqlUpdate);
            Execute(query);
        }

        public void BeginTransaction()
        {
            trans = cnn.BeginTransaction();
            IsInTransaction = true;
        }
        public void CommitTransaction()
        {
            if (trans != null)
            {
                trans.Commit();
                trans = null;
                IsInTransaction = false;
            }
        }
        public void RollbackTransaction()
        {
            if (trans != null)
            {
                trans.Rollback();
                trans = null;
                IsInTransaction = false;
            }
        }
        public void DestroyTransaction()
        {
            trans = null;
        }

        private void ProcessParameters(SqlCommand cmd, params object[] args)
        {
            int paramNo = 0;
            cmd.CommandText = cmd.CommandText.Replace(":", "@");
            foreach (object arg in args)
            {
                SqlParameter parameter = new SqlParameter("@" + paramNo.ToString(), arg.GetType());
                parameter.Value = arg;
                cmd.Parameters.Add(parameter);
                paramNo++;
            }
        }

        public void Close() => this.CloseConnection();

        public void Dispose()
        {
            this.CloseConnection();
            IsDisposed = true;
            cnn.Dispose();
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
        //public DataTable Fetch()
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