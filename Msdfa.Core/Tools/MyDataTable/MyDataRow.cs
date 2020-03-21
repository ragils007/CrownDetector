using System;
using System.Collections.Generic;
using System.Data;
using Msdfa.Core.Entities;

namespace Msdfa.Core.Tools.MyDataTable
{
    public class MyDataRow
    {
        public readonly DataRow DataRow;

        public bool GetColumnsAffectedOnly { get; set; }

        public List<string> QueriedColumns { get; set; } = new List<string>();

        public MyDataRow(DataRow dr)
        {
            this.DataRow = dr;
        }

        public void SetValue<TValue>(string columnName, TValue value)
        {
            this.DataRow[columnName] = value;
        }

        public DataRow GetDataRow() => this.DataRow;

        public decimal GetDecimal(string columnName)
        {
            decimal value = 0;
            if (!GetColumnsAffectedOnly && this.DataRow[columnName] != System.DBNull.Value)
                value = Convert.ToDecimal(this.DataRow[columnName]);
            this.AddQueriedColumn(columnName);
            return this.GetColumnsAffectedOnly ? 0 : value;
        }

        public bool GetBool(string columnName)
        {
            this.AddQueriedColumn(columnName);
            return !this.GetColumnsAffectedOnly && Convert.ToInt32(this.DataRow[columnName]) == 1;
        }

        public int GetInt(string columnName)
        {
            this.AddQueriedColumn(columnName);
            return this.GetColumnsAffectedOnly ? 0 : Convert.ToInt32(this.DataRow[columnName]);
        }

        public long GetLong(string columnName)
        {
            this.AddQueriedColumn(columnName);
            return this.GetColumnsAffectedOnly ? 0 : Convert.ToInt64(this.DataRow[columnName]);
        }

        public string GetString(string columnName)
        {
            this.AddQueriedColumn(columnName);
            return this.GetColumnsAffectedOnly ? "" : this.DataRow[columnName].ToString();
        } 

        private void AddQueriedColumn(string columnName)
        {
            if (!this.QueriedColumns.Contains(columnName)) this.QueriedColumns.Add(columnName);
        }

        public DateTime GetDateTime(string columnName)
        {
            this.AddQueriedColumn(columnName);
            return this.GetColumnsAffectedOnly ? default(DateTime) : Convert.ToDateTime(this.DataRow[columnName]);
        }
    }
}
