using System;
using System.Collections.Generic;
using System.Data;
using Msdfa.Core.Tools.MyDataTable;

namespace Msdfa.DB
{
    public interface IQuery
    {
        string QueryString { get; set; }
        Query.QueryType Type { get; set; }
        Dictionary<string, object> BindValues { get; set; }

        IQuery Bind(string varName, object varValue);
        IQuery BindIf(bool isTrue, string varName, object varValue);
        IQuery BindListIn<TDataType>(string varName, List<TDataType> varValues);
        IQuery BindListInIf(bool isTrue, string varName, List<object> varValues);
        int Execute();
        Dictionary<string, object> ExecuteReturning(Dictionary<string, Type> returningColumns = null);
        DataTable Fetch();
        List<MyDataRow> ToMyDataRowList();
        List<TDataType> FetchAs<TDataType>(Func<MyDataRow, TDataType> selector);
        IDataReader FetchCursor();
        DataRow FetchRow();
        object FetchValue();
        T FetchValue<T>();
        string GetQueryString();
        int Insert();
        Dictionary<string, object> InsertReturning(Dictionary<string, Type> returningColumns = null);
        IQuery SetPKSequence(string columnName, string sequenceName);
        IQuery SetPKValues(Dictionary<string, object> values);
        IQuery SetQueryType(Query.QueryType type, string queryTable = null);
    }
}