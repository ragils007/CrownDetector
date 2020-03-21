using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Oracle.ManagedDataAccess.Types;

namespace Msdfa.DB
{
    public class Postgres : IDisposable, IDatabase
    {
        public DatabaseType DatabaseType => DatabaseType.Postgres;

        public class ActiveConnectionsChangedEventArgs : EventArgs
        {
            public bool IsOpened { get; set; }
            public bool IsClosed { get; set; }
            public string Guid { get; set; }
            public string Description { get; set; }
        }

        public static event EventHandler<ActiveConnectionsChangedEventArgs> ActiveConnectionsChanged;
        private static void OnActiveConnectionsChanged(ActiveConnectionsChangedEventArgs e) => ActiveConnectionsChanged?.Invoke(null, e);
        public static int ActiveConnections;

        public IDbConnection Cnn => this.cnn;

        public string Guid { get; set; }
        public string DebugDescription { get; set; }

        public NpgsqlConnection cnn;
        private NpgsqlTransaction trans = null;
        public bool IsDisposed { get; private set; }
        public bool IsInTransaction { get; set; }
        public int transactionLineNo = 0;
        public int TransactionCommitEvery { get; set; }

        public int CommandTimeout { get; set; } = 0;

        public string ClientInfo { get; set; }

        public IDbCommand RunningCommand { get; private set; }

        public string DataSource
        {
            get { return this.cnn.DataSource; }
        }

        private string ConnectionString;

        public ConnectionState State
        {
            get { return this.cnn.State; }
        }

        public string ErrorMsg;

        public Postgres()
        {
        }

        public Postgres(string connectionString, bool connect = true, string debugMessage = null)
        {
            this.ConnectionString = connectionString;
            this.Guid = System.Guid.NewGuid().ToString();

            this.DebugDescription = debugMessage;
            this.Init(connect);
        }

        private void Init(bool connect = true)
        {
            this.cnn = new NpgsqlConnection(this.ConnectionString);
            this.IsInTransaction = false;
            try
            {
                if (connect == true)
                {
                    this.OpenConnection();
                }
            }
            catch (NpgsqlException e)
            {
                this.ErrorMsg = e.Message;
                throw;
            }
        }

        public void BeginTransaction()
        {
            this.trans = this.cnn.BeginTransaction(IsolationLevel.ReadCommitted);
            this.IsInTransaction = true;
        }

        private void CommitAndBeginTransaction()
        {
            if (this.trans != null)
            {
                this.trans.Commit();
                this.BeginTransaction();
            }
            else throw new Exception("CommitAndBegin: Brak aktywnej transakcji!");
        }

        public void CommitTransaction()
        {
            if (this.trans != null)
            {
                this.trans.Commit();
                this.EndTransaction();
            }
            else throw new Exception("Commit: Brak aktywnej transakcji!");
        }

        public void RollbackTransaction()
        {
            if (this.trans != null)
            {
                this.trans.Rollback();
                this.EndTransaction();
            }
            else throw new Exception("Rollback: Brak aktywnej transakcji!");
        }

        public void EndTransaction()
        {
            this.IsInTransaction = false;
            this.trans = null;
        }

        //private string GetStacktraceInfo()
        //{
        //    var st = Environment.StackTrace.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //    st.RemoveRange(0, 2);
        //    var line = st.First(x => !x.Contains("Fetch") && !x.Contains("Oracle") && !x.Contains("Query") && !x.Contains("Execute"));

        //    var tokens = line.Split(new[] { " w ", " in " }, StringSplitOptions.RemoveEmptyEntries);

        //    var debugInfo = "-";

        //    if (tokens.Length > 1)
        //    {
        //        var callMethod = tokens[1];
        //        var method = callMethod.Split('.').Last();

        //        if (tokens.Length > 2)
        //        {
        //            var callLine = tokens[2].Split('\\').Last();
        //            debugInfo = $"{method} {callLine}";
        //        }
        //        else
        //        {
        //            debugInfo = $"{method}";
        //        }
        //    }
        //    return debugInfo;
        //}

        public DataTable Fetch(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            var myTable = new DataTable("dataTable");

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                RunningCommand = cmd;

                cmd.CommandTimeout = CommandTimeout;
                this.ProcessParameters(cmd, args);

                using (var myDataAdp = new NpgsqlDataAdapter(cmd))
                {
                    using (new NpgsqlCommandBuilder(myDataAdp))
                    {
                        try
                        {
                            myDataAdp.Fill(myTable);
                        }
                        catch (NpgsqlException er)
                        {
                            throw er;
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            RunningCommand = null;
                        }
                    }
                }
            }
            return myTable;
        }

        public async Task<DataTable> FetchAsync(string query, CancellationToken cancellationToken, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            var myTable = new DataTable();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                RunningCommand = cmd;

                cmd.CommandTimeout = CommandTimeout;
                this.ProcessParameters(cmd, args);

                using (var reader = await Task.Run(async () => await cmd.ExecuteReaderAsync(cancellationToken), cancellationToken).ConfigureAwait(false))
                {
                    if (reader.HasRows)
                    {
                        var schemaTable = reader.GetSchemaTable();
                        foreach (DataRow schemarow in schemaTable.Rows)
                        {
                            myTable.Columns.Add(schemarow.ItemArray[0].ToString(), Type.GetType(schemarow.ItemArray[11].ToString()));
                        }

                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            var row = myTable.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }
                            myTable.Rows.Add(row);
                        }
                    }

                    RunningCommand = null;
                }
            }
            return myTable;
        }

        public DataTable Fetch(string query, int commandTimeout, params object[] args)
        {
            var tempCommandTimeout = CommandTimeout;
            CommandTimeout = commandTimeout;

            var returnValue = Fetch(query, args);
            CommandTimeout = tempCommandTimeout;

            return returnValue;
        }

        public DataRow FetchRow(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            DataTable myTable = new DataTable();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                RunningCommand = cmd;

                cmd.CommandTimeout = CommandTimeout;
                this.ProcessParameters(cmd, args);

                using (var myDataAdp = new NpgsqlDataAdapter(cmd))
                {
                    using (new NpgsqlCommandBuilder(myDataAdp))
                    {
                        myDataAdp.Fill(myTable);
                    }
                }

                RunningCommand = null;
            }
            if (myTable.Rows.Count > 0) return myTable.Rows[0];
            return null;
        }

        public object FetchValue(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            this.SynchronizeSCN();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();
            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                RunningCommand = cmd;

                cmd.CommandTimeout = CommandTimeout;

                if (this.trans != null) cmd.Transaction = this.trans;
                this.ProcessParameters(cmd, args);
                var objRes = cmd.ExecuteScalar();

                RunningCommand = null;
                return objRes;
            }
        }

        public IDataReader FetchCursor(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                RunningCommand = cmd;

                this.ProcessParameters(cmd, args);
                return cmd.ExecuteReader();
            }
        }

        public int Execute(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");
            if (this.cnn.State.ToString() == "Closed")
                this.cnn.Open();

            //this.cnn.ActionName = this.GetStacktraceInfo();

            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;
                RunningCommand = cmd;

                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                this.ProcessParameters(cmd, args);

                // this.cnn.ClientInfo = this.ClientInfo;

                int countRecord = 0;

                countRecord = cmd.ExecuteNonQuery();
                RunningCommand = null;

                return countRecord;
            }
        }

        public void ExecuteProcedure(string procedureName, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            //this.cnn.ActionName = this.GetStacktraceInfo();

            using (var cmd = new NpgsqlCommand(procedureName, this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType.StoredProcedure;
                RunningCommand = cmd;

                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                this.ProcessParameters(cmd, args);

                // this.cnn.ClientInfo = this.ClientInfo;

                cmd.ExecuteNonQuery();

                RunningCommand = null;
            }
        }

        public T ExecuteFunction<T>(string functionName, params object[] args)
        {
            object returnValue = null;

            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            //this.cnn.ActionName = this.GetStacktraceInfo();

            using (var cmd = new NpgsqlCommand(functionName, this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;

                cmd.CommandType = CommandType.StoredProcedure;
                RunningCommand = cmd;
                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                this.ProcessParameters(cmd, args);

                // this.cnn.ClientInfo = this.ClientInfo;


                NpgsqlParameter returnParametr = null;

                if (typeof(T) == typeof(string))
                {
                    returnParametr = new NpgsqlParameter("ret", DbType.String);
                    returnParametr.Size = 256;
                }
                else
                {
                    throw new Exception("Nieoprogramowana sytuacja");
                }

                returnParametr.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Insert(0, returnParametr);

                cmd.ExecuteNonQuery();
                RunningCommand = null;

                if (typeof(T) == typeof(string))
                {
                    returnValue = returnParametr.Value.ToString();
                }

            }

            return (T)returnValue;
        }




        /**
         * Synchronizuje SCN (System Change Number), powinno to wyeliminować przypadek zwracanila niezaktualizowanych
         * danych w przypadku następujących po sobie szybko UPDATE i SELECT

            http://docs.oracle.com/cd/B28359_01/server.111/b28310/ds_txnman010.htm#i1008473:

            Managing Read Consistency

            An important restriction exists in the Oracle Database implementation of distributed read consistency. The problem arises because each system has its own SCN, which you can view as the database internal timestamp. The Oracle Database server uses the SCN to decide which version of data is returned from a query.
            The SCNs in a distributed transaction are synchronized at the end of each remote SQL statement and at the start and end of each transaction. Between two nodes that have heavy traffic and especially distributed updates, the synchronization is frequent. Nevertheless, no practical way exists to keep SCNs in a distributed system absolutely synchronized: a window always exists in which one node may have an SCN that is somewhat in the past with respect to the SCN of another node.
            Because of the SCN gap, you can execute a query that uses a slightly old snapshot, so that the most recent changes to the remote database are not seen. In accordance with read consistency, a query can therefore retrieve consistent, but out-of-date data. Note that all data retrieved by the query will be from the old SCN, so that if a locally executed update transaction updates two tables at a remote node, then data selected from both tables in the next remote access contain data prior to the update.
            One consequence of the SCN gap is that two consecutive SELECT statements can retrieve different data even though no DML has been executed between the two statements. For example, you can issue an update statement and then commit the update on the remote database. When you issue a SELECT statement on a view based on this remote table, the view does not show the update to the row. The next time that you issue the SELECT statement, the update is present.
            You can use the following techniques to ensure that the SCNs of the two machines are synchronized just before a query:
            1. Because SCNs are synchronized at the end of a remote query, precede each remote query with a dummy remote query to the same site, for example, SELECT * FROM DUAL@REMOTE
            2. Because SCNs are synchronized at the start of every remote transaction, commit or roll back the current transaction before issuing the remote query

         */

        private void SynchronizeSCN()
        {
            //27.03.2017 D.T. testowo wyłączyłem
            //this.Execute("SELECT * FROM DUAL");
        }

        /**
         * Specjalna wersja Execute zwracająca ID ostatnio dodanego wiersza.
         * WAŻNE! Kolumna podana jako pierwsza w kolumnach do dodania jest uznawana za ID
         */

        public int Insert(string query, params object[] args)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");

            if (query.StartsWith("INSERT ") == false) throw new Exception("Not an INSERT statement.");
            if (this.cnn.State.ToString() == "Closed") this.cnn.Open();

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            query = string.Format("{0} RETURNING {1} INTO :NEW_ID"
                , query
                ,
                query.Substring(query.IndexOf("(", StringComparison.Ordinal) + 1,
                    query.IndexOf(",", StringComparison.Ordinal) - query.IndexOf("(", StringComparison.Ordinal) - 1));
            using (var cmd = new NpgsqlCommand(query, this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;
                RunningCommand = cmd;

                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                var returnParam = new NpgsqlParameter("NEW_ID", DbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(returnParam);

                this.ProcessParameters(cmd, args);
                cmd.ExecuteNonQuery();
                RunningCommand = null;

                return int.Parse(cmd.Parameters["NEW_ID"].Value.ToString());
            }
        }

        public Dictionary<string, object> InsertReturning(Query query)
        {
            throw new NotImplementedException();
            //if (query.GetQueryString().StartsWith("INSERT ") == false) throw new Exception("Not an INSERT statement.");
            //return this.ExecuteReturning(query);
        }

        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            if (this.IsDisposed) throw new Exception("Connection is disposed. Please open new one.");
            if (this.cnn.State.ToString() == "Closed")
                this.cnn.Open();

            //this.cnn.ActionName = this.GetStacktraceInfo();

            using (var cmd = new NpgsqlCommand($"{query.QueryString} RETURNING *", this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;
                RunningCommand = cmd;

                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                this.ProcessParameters(cmd, query.BindValues);

                foreach (var item in query.ReturningColumns)
                {
                    if (cmd.Parameters.Contains(item.Key)) cmd.Parameters[item.Key].Direction = ParameterDirection.InputOutput;
                    else cmd.Parameters.Add(new NpgsqlParameter(item.Key, GetDbTypeFromObject(item.Value)) { Direction = ParameterDirection.Output });
                }

                cmd.ExecuteScalar();

                RunningCommand = null;

                var ret = new Dictionary<string, object>();
                foreach (NpgsqlParameter par in cmd.Parameters)
                {
                    if (par.Direction == ParameterDirection.Output || par.Direction == ParameterDirection.InputOutput)
                    {
                        ret.Add(par.ParameterName, par.Value);
                    }
                }
                
                return ret;
            }
        }


        /*
        public Dictionary<string, object> ExecuteReturning(Query query)
        {
            var queryString = query.GetQueryString();
            var returnDict = new Dictionary<string, object>();

            if (this.cnn.State.ToString() == "Closed") this.Init();

            //this.cnn.ActionName = this.GetStacktraceInfo();
            this.SynchronizeSCN();

            using (var cmd = new NpgsqlCommand(queryString, this.cnn))
            {
                cmd.CommandTimeout = CommandTimeout;
                RunningCommand = cmd;

                if (this.trans != null)
                {
                    cmd.Transaction = this.trans;
                    if (this.TransactionCommitEvery > 0 && this.transactionLineNo == this.TransactionCommitEvery)
                    {
                        this.CommitAndBeginTransaction();
                        this.transactionLineNo = 0;
                    }
                    this.transactionLineNo++;
                }

                foreach (var item in query.ReturningColumns)
                {
                    var param = new NpgsqlParameter(item.Key, GetDbType(item.Value));
                    if (query.BindValues.ContainsKey(item.Key))
                    {
                        param.Direction = ParameterDirection.InputOutput;
                    }
                    else param.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(param);
                }

                this.ProcessParameters(cmd, query.BindValues);
                var rowsAffected = cmd.ExecuteNonQuery();
                if ((query.Type == Query.QueryType.Insert || query.Type == Query.QueryType.Update) && rowsAffected == 0)
                    throw new NoRowsAffectedException();

                foreach (var item in query.ReturningColumns)
                {
                    returnDict.Add(item.Key, ConvertToCSharpType(cmd.Parameters[item.Key]));
                }

                RunningCommand = null;
                return returnDict;
            }
        }
        */

        /// <summary>
        /// Parametry mogą być przekazywane do metody na dwa sposoby:
        /// 1. "SELECT * FROM FOO WHERE VAR_A = :0 AND VAR_B = :1", val_0, val_1
        ///	 "SELECT * FROM FOO WHERE VAR_A = :x AND VAR_B = :y", val_0, val_1
        ///	 "SELECT * FROM FOO WHERE VAR_A = :x AND VAR_B = :x", val_0, val_1   // to także zadziała
        ///	binding typu wyliczeniowego, parametry przekazywane jako argumenty funkcji, bindowane są
        ///	w kolejności ich podawania, nie zależnie od tego jak nazwiemy parametry w zapytaniu.
        /// 2. "SELECT * FROM FOO WHERE VAR_A = :var_a AND VAR_B = :var_b", new Dictionary<string, object>()
        ///		{
        ///			{ "var_a", val_a },
        ///			{ "var_b", var_b }
        ///		});
        /// </summary>
        /// <param name="cmd">Zapytanie SQL</param>
        /// <param name="args">Parametry</param>
        private void ProcessParameters(NpgsqlCommand cmd, object[] args)
        {
            if (args.Length > 0)
            {
                /**
                 * Dla argumentów przekazywanych w formie: Dictionary<string, object>
                 */
                if (args[0].GetType() == typeof(Dictionary<string, (object, Type)>))
                {
                    // cmd.BindByName = true;

                    foreach (var dict in (Dictionary<string, (object value, Type dataType)>)args[0])
                    {
                        var type = dict.Value.dataType ?? dict.Value.value?.GetType();

                        // 2016-07-22: Denis Trzeciok, obsługa bindowania tablic do bulkowych insertów
                        //      BZ: w przypadku byte[] mamy do czynienia z danymi binarnymi - ignorujemy
                        // if (type?.IsArray == true && type.GetElementType() != typeof(byte)) //D.T. 22.07.2016
                        // {
                        //     var array = (ICollection)dict.Value;
                        //     cmd.ArrayBindCount = array.Count;
                        // }

                        // Na niektórych systemach bindowanie pól typu CLOB powodowało błędy. Trzeba ustawić kierunek InputOutput
                        var pgsqlType = GetDbTypeFromObject(dict.Value.value);
                        // if (pgsqlType == DbType.String && dict.Key.EndsWith("_CLOB")) pgsqlType = DbType.;

                        var param = new NpgsqlParameter(dict.Key, pgsqlType) { Value = dict.Value.value == null ? DBNull.Value : dict.Value.value };
                        // if (pgsqlType == DbType.Clob) param.Direction = ParameterDirection.InputOutput;
                        cmd.Parameters.Add(param);
                    }
                }
                else
                {
                    int paramNo = 0;
                    foreach (object arg in args)
                    {
                        var parameter = new NpgsqlParameter(paramNo.ToString(), GetDbTypeFromObject(arg));
                        parameter.Value = arg;
                        cmd.Parameters.Add(parameter);
                        paramNo++;
                    }
                }
            }
        }

        private void ProcessParameters(NpgsqlCommand cmd, Dictionary<string, (object value, Type dataType)> args)
        {
            // cmd.BindByName = true;
            foreach (var dict in args)
            {
                // Parametry InputOutput są już dodane do cmd
                if (cmd.Parameters.Contains(dict.Key)) (cmd.Parameters[dict.Key]).Value = dict.Value;
                else
                {
                    NpgsqlParameter param;

                    if (dict.Value.dataType == null)
                    {
                        param = new NpgsqlParameter(dict.Key, GetDbTypeFromObject(dict.Value.value)) { Value = dict.Value.value ?? DBNull.Value };
                    }
                    else
                    {
                        param = new NpgsqlParameter(dict.Key, GetDbTypeFromType(dict.Value.dataType)) { Value = dict.Value.value ?? DBNull.Value };
                    }

                    cmd.Parameters.Add(param);
                }
            }
        }

        public static DbType GetDbTypeFromObject(object o)
        {
            var type = o?.GetType();

            if (type?.IsArray ?? false == true)
            {
                var ienum = (IEnumerable)o;
                var enumerator = ienum.GetEnumerator();
                enumerator.Reset();
                enumerator.MoveNext();
                var obj = enumerator.Current;
                var elementType = obj?.GetType();

                if (elementType == typeof(string)) return DbType.String;
                if (elementType == typeof(DateTime)) return DbType.Date;
                if (elementType == typeof(long)) return DbType.Int64;
                if (elementType == typeof(int)) return DbType.Int32;
                if (elementType == typeof(short)) return DbType.Int16;
                //if (elementType == typeof(byte)) return DbType.Byte;
                if (elementType == typeof(byte[])) return DbType.Binary;
                if (elementType == typeof(decimal)) return DbType.Decimal;
                if (elementType == typeof(float)) return DbType.Single;
                if (elementType == typeof(double)) return DbType.Double;
                if (elementType == typeof(bool)) return DbType.Int32;
            }

            if (o is string) return DbType.String;
            if (o is DateTime) return DbType.Date;
            if (o is long) return DbType.Int64;
            if (o is int) return DbType.Int32;
            if (o is short) return DbType.Int16;
            if (o is byte) return DbType.Byte;
            if (o is decimal) return DbType.Decimal;
            if (o is float) return DbType.Single;
            if (o is double) return DbType.Double;
            if (o is byte[]) return DbType.Binary;
            if (o is bool) return DbType.Boolean;

            return DbType.String;
        }

        public static DbType GetDbTypeFromType(Type t)
        {
            if (t == typeof(string)) return DbType.String;
            if (t == typeof(DateTime)) return DbType.Date;
            if (t == typeof(long)) return DbType.Int64;
            if (t == typeof(int)) return DbType.Int32;
            if (t == typeof(short)) return DbType.Int16;
            if (t == typeof(byte)) return DbType.Byte;
            if (t == typeof(decimal)) return DbType.Decimal;
            if (t == typeof(float)) return DbType.Single;
            if (t == typeof(double)) return DbType.Double;
            if (t == typeof(byte[])) return DbType.Binary; 
            if (t == typeof(bool)) return DbType.Boolean;
            return DbType.String;
        }

        private object ConvertToCSharpType(NpgsqlParameter p)
        {
            if (p.DbType == DbType.String)
                return (OracleString)p.Value == null ? null : ((OracleString)p.Value).Value;
            if (p.DbType == DbType.Date) return ((OracleDate)p.Value).Value;
            if (p.DbType == DbType.Int64) return ((OracleDecimal)p.Value).ToInt64();
            if (p.DbType == DbType.Int32)
            {
                if (p.Value is OracleDecimal) return ((OracleDecimal)p.Value).ToInt32();
                return Convert.ToInt32(p.Value);
            }
            if (p.DbType == DbType.Int16) return ((OracleDecimal)p.Value).ToInt16();
            if (p.DbType == DbType.Byte) return ((OracleDecimal)p.Value).ToByte();
            if (p.DbType == DbType.Decimal) return ((OracleDecimal)p.Value).Value;
            if (p.DbType == DbType.Single) return ((OracleDecimal)p.Value).ToSingle();
            if (p.DbType == DbType.Double) return ((OracleDecimal)p.Value).ToDouble();
            if (p.DbType == DbType.Binary) return ((OracleBinary)p.Value).Value;
            return p.Value;
        }

        public void CloseConnection()
        {
            if (this.Cnn.State == ConnectionState.Open)
            {
                this.Cnn.Close();
                Interlocked.Decrement(ref ActiveConnections);
                RunningCommand = null;
                OnActiveConnectionsChanged(new ActiveConnectionsChangedEventArgs { IsClosed = true, Guid = this.Guid });
            }
        }

        public void OpenConnection()
        {
            if (this.Cnn.State != ConnectionState.Open)
            {
                this.Cnn.Open();
                Interlocked.Increment(ref ActiveConnections);
                RunningCommand = null;
                OnActiveConnectionsChanged(new ActiveConnectionsChangedEventArgs { IsOpened = true, Guid = this.Guid, Description = this.DebugDescription });
            }
        }

        public void Dispose()
        {
            this.RunningCommand = null;
            this.CloseConnection();
            this.IsDisposed = true;
            this.cnn.Dispose();
        }

        ~Postgres()
        {
            this.Dispose();
        }

        public void LockTable(string tableName)
        {
            this.Execute("LOCK TABLE " + tableName);
        }

        public DateTime GetDBTime()
        {
            throw new NotImplementedException();
        }
    }
}