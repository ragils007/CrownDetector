using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Msdfa.Core.Tools.MyDataTable;

namespace Msdfa.DB
{
    public class Query : IQuery
    {
        public enum QueryType
        {
            Default,
            Select,
            Insert,
            Update
        }

        public Dictionary<string, object> BindValues { get; set; }
        protected internal Dictionary<string, Type> ReturningColumns;

        public Query(IDatabase db, string queryString = null)
        {
            Db = db;
            Type = QueryType.Default;
            QueryTable = null;
            BindValues = new Dictionary<string, object>();
            PKSequence = null;
            ReturningColumns = new Dictionary<string, Type>();
            QueryString = queryString;
        }

        internal IDatabase Db { get; set; }
        protected string _queryString { get; set; }

        public string QueryString
        {
            get
            {
                if (Type != QueryType.Default) return GetQueryString();
                return _queryString;
            }
            set { _queryString = value; }
        }

        protected string QueryTable { get; set; }
        public QueryType Type { get; set; }

        // W przypadku sekwencji nie obsługiwanych przez trigger, należy do polecenia INSERT dodać pobranie NEXTVAL'a
        // Key: PK column name
        // Value: Sequence name
        protected internal KeyValuePair<string, string>? PKSequence { get; set; }

        private Dictionary<string, object> PKValues { get; set; }

        //public Query(Database db, QueryType type)
        //{
        //   this.Db = db;
        //   this.BindValues = new Dictionary<string, object>();
        //   this.ReturningColumns = new Dictionary<string, Type>();
        //   this.Type = type;
        //}

        public IQuery Bind(string varName, object varValue)
        {
            if (varValue == null)
            {
                BindValues.Add(varName, null);
                return this;
            }

            var valueE = varValue as Enum;
            if (valueE != null)
            {
                var enumType = valueE.GetType();
                var enumDataType = Enum.GetUnderlyingType(enumType);
                if (enumDataType == typeof (long)) varValue = (long) varValue;
                else if (enumDataType == typeof (int)) varValue = (int) varValue;
                else throw new Exception("Enum data type not supported. [" + enumDataType + "]");
            }
            else if (this.Db.DatabaseType == DatabaseType.Oracle && (varValue.GetType() == typeof (bool) || varValue.GetType() == typeof (bool?)))
            {
                varValue = Convert.ToBoolean(varValue) ? 1 : 0;
            }

            BindValues.Add(varName, varValue);
            return this;
        }

        public IQuery SetQueryType(QueryType type, string queryTable = null)
        {
            Type = type;
            QueryTable = queryTable;
            return this;
        }

        public IQuery SetPKValues(Dictionary<string, object> values)
        {
            PKValues = values;
            BindAll(values);
            return this;
        }

        public IQuery SetPKSequence(string columnName, string sequenceName)
        {
            PKSequence = new KeyValuePair<string, string>(columnName, sequenceName);
            return this;
        }

        public IQuery BindIf(bool isTrue, string varName, object varValue)
        {
            if (isTrue) Bind(varName, varValue);
            return this;
        }

        public IQuery BindAll(Dictionary<string, object> values)
        {
            foreach (var item in values)
            {
                Bind(item.Key, item.Value);
            }
            return this;
        }

        /// <summary>
        ///     Binduje listę objektów jako parametry do operatora IN(...)
        /// </summary>
        /// <param name="varName">Nazwa parametru bazowego</param>
        /// <param name="varValues">Lista wartości</param>
        /// <returns>Database</returns>
        public IQuery BindListIn<TDataType>(string varName, List<TDataType> varValues)
        {
            var paramNames = new List<string>();
            for (var i = 0; i < varValues.Count; i++)
            {
                var paramName = varName + i;
                Bind(paramName, varValues[i]);
                paramNames.Add(":" + paramName);
            }

            QueryString = QueryString.Replace(":" + varName, string.Join(", ", paramNames));
            return this;
        }

        public IQuery BindListInIf(bool isTrue, string varName, List<object> varValues)
        {
            if (isTrue) return BindListIn(varName, varValues);
            return this;
        }

        public List<MyDataRow> ToMyDataRowList()
        {
            return this.Fetch().AsEnumerable().Select(x => new MyDataRow(x)).ToList();
        } 

        public DataTable Fetch()
        {
            lock (Db)
            {
                return Db.Fetch(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        public DataTable Fetch(int commandTimeout)
        {
            lock (Db)
            {
                return Db.Fetch(QueryString, commandTimeout, new Dictionary<string, object>(BindValues));
            }
        }

        public async Task<DataTable> FetchAsync(CancellationToken cancellationToken)
        {
            return await Db.FetchAsync(QueryString, cancellationToken, new Dictionary<string, object>(BindValues)).ConfigureAwait(false);
        }

        public List<TDataType> FetchAs<TDataType>(Func<MyDataRow, TDataType> selector)
        {
            return this.Fetch().AsEnumerable().Select(x => new MyDataRow(x)).Select(selector).ToList();
        }

        public DataRow FetchRow()
        {
            lock (Db)
            {
                return Db.FetchRow(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        public object FetchValue()
        {
            lock (Db)
            {
                return Db.FetchValue(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        public T FetchValue<T>()
        {
            lock (Db)
            {
                var ret = Db.FetchValue(QueryString, new Dictionary<string, object>(BindValues));
                var cast = (T)Convert.ChangeType(ret, typeof(T));

                return cast;
            }
        }

        public IDataReader FetchCursor()
        {
            lock (Db)
            {
                return Db.FetchCursor(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        public int Execute()
        {
            lock (Db)
            {
                return Db.Execute(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        /// <summary>
        ///     W przypadku bazy danych ORACLE i nadawania PK przez trigger, należy ręcznie wskazać nazwę kolumny ID, celem
        ///     zwrócenia odpowiedniej wartości.
        /// </summary>
        /// <param name="idColumnName"></param>
        /// <returns></returns>
        public int Insert()
        {
            lock (Db)
            {
                return Db.Insert(QueryString, new Dictionary<string, object>(BindValues));
            }
        }

        public Dictionary<string, object> InsertReturning(Dictionary<string, Type> returningColumns = null)
        {
            ReturningColumns = returningColumns ?? new Dictionary<string, Type>();

            var ret = new Dictionary<string, object>();
            lock (BindValues)
            {
                lock (ReturningColumns)
                {
                    lock (Db)
                    {
                        ret = Db.InsertReturning(this);
                    }
                }
            }
            BindValues.Clear();
            ReturningColumns.Clear();
            return ret;
        }

        public Dictionary<string, object> ExecuteReturning(Dictionary<string, Type> returningColumns = null)
        {
            ReturningColumns = returningColumns ?? new Dictionary<string, Type>();

            var ret = new Dictionary<string, object>();
            lock (BindValues)
            {
                lock (ReturningColumns)
                {
                    lock (Db)
                    {
                        ret = Db.ExecuteReturning(this);
                    }
                }
            }
            BindValues.Clear();
            ReturningColumns.Clear();
            return ret;
        }

        public string GetQueryString()
        {
            var query = "";
            if (Type == QueryType.Insert && QueryTable != null && BindValues.Count > 0)
            {
                query = "INSERT INTO " + QueryTable;

                var bindValuesList = BindValues.Keys.ToList();

                // Jezeli wypełniona jest właściwość PKColumnSequence, sekwencja nie jest obsługiwana przez trigger (ręcznie ją wywołujemy)
                if (PKSequence.HasValue)
                    query += " (" + PKSequence.Value.Key + "," + string.Join(",", bindValuesList) + ") VALUES (" +
                             PKSequence.Value.Value + "," +
                             string.Join(",", from item in bindValuesList select ":" + item) + ")";
                else
                    query += " (" + string.Join(",", bindValuesList) + ") VALUES (" +
                             string.Join(",", from item in bindValuesList select ":" + item) + ")";

                if (ReturningColumns.Count > 0)
                {
                    var returningColumnList = ReturningColumns.Keys.ToList();
                    query += " RETURNING " + string.Join(",", returningColumnList) + " INTO " +
                             string.Join(",", from item in returningColumnList select ":" + item);
                }
            }
            if (Type == QueryType.Update && QueryTable != null && BindValues.Count > 0)
            {
                if (PKValues == null) throw new Exception("PKValues not set. Bulk updating not supported.");

                var bindValuesList = new Dictionary<string, object>(BindValues);

                query = "UPDATE " + QueryTable + " SET ";

                query += string.Join(", ",
                    (from item in
                        bindValuesList.Where(x => PKValues.Keys.Contains(x.Key) == false && x.Key != "ROW_VERSION")
                        select item.Key + "=:" + item.Key).ToList());

                query += " WHERE ROW_VERSION=:ROW_VERSION AND " +
                         string.Join(" AND ", (from item in PKValues select item.Key + "=:" + item.Key).ToList());
                // Jezeli wypełniona jest właściwość PKColumnSequence, sekwencja nie jest obsługiwana przez trigger (ręcznie ją wywołujemy)
                //if (this.PKSequence.HasValue) query += " (" + this.PKSequence.Value.Key + "," + (string.Join(",", bindValuesList) + ") VALUES (" + this.PKSequence.Value.Value + "," + string.Join(",", (from item in bindValuesList select ":" + item))) + ")";
                //else query += " (" + (string.Join(",", bindValuesList) + ") VALUES (" + string.Join(",", (from item in bindValuesList select ":" + item))) + ")";

                if (ReturningColumns.Count > 0)
                {
                    var returningColumnList = ReturningColumns.Keys.ToList();
                    query += " RETURNING " + string.Join(",", returningColumnList) + " INTO " +
                             string.Join(",", from item in returningColumnList select ":" + item);
                }
            }


            return query;
        }

        protected internal string DisplayArgs()
        {
            return BindValuesString(BindValues);
        }
        
        protected internal string DisplayArgs(params object[] args)
        {
            if (args.Length == 0) return "";
            if (args[0].GetType() == typeof (Dictionary<string, object>))
            {
                return BindValuesString((Dictionary<string, object>) args[0]);
            }
            return "['" + string.Join("', '", args) + "'] ";
        }

        protected string BindValuesString(Dictionary<string, object> dict)
        {
            return dict.Count > 0
                ? "[" + string.Join(", ", dict.Select(x => "'" + x.Key + "'->'" + x.Value + "'")) + "] "
                : "";
        }
    }
}