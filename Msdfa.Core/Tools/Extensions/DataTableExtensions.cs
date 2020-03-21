using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Zwraca tabele zawierajaca niepowtarzajace sie rekordy
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static System.Data.DataTable Distinct(this DataTable table)
        {
            return table.DefaultView.ToTable(true);
        }

        public static System.Data.DataTable Distinct(this DataTable table, params string[] columnNames)
        {
            var newDt = table.Clone();
            var distinctIdList = new HashSet<string>();

            foreach (var dr in table.AsEnumerable())
            {
                var id = string.Join("|", from item in columnNames select dr[item]);
                if (distinctIdList.Contains(id)) continue;

                newDt.Rows.Add(dr.ItemArray);
                distinctIdList.Add(id);
            }
            return newDt;
        }

        /// <summary>
        /// Usuwa z tabeli kolumny ktorych nazwa nie zaczyna sie sie od tableNamePrefix
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableNamePrefix"></param>
        /// <returns></returns>
        public static System.Data.DataTable RemoveColumns(this System.Data.DataTable table, string tableNamePrefix)
        {
            for (int i = table.Columns.Count - 1; i >= 0; i--)
            {
                if (table.Columns[i].ColumnName.StartsWith(tableNamePrefix) == false)
                {
                    table.Columns.Remove(table.Columns[i]);
                }
            }

            return table;
        }

        /// <summary>
        /// Zmienia nazwy column, z nazwy kolumny usuwa prefix = tableNamePrefix
        /// uwaga: sytuacja ze nazwa tabeli zawiera "__" jest niedopuszczalna
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableNamePrefix"></param>
        /// <returns></returns>
        public static System.Data.DataTable ChangeNameColumns(this System.Data.DataTable table, string tableNamePrefix)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.StartsWith(tableNamePrefix) == true)
                {
                    table.Columns[i].ColumnName = table.Columns[i].ColumnName.Replace(tableNamePrefix, "");
                }
            }
            return table;
        }

        public static System.Data.DataTable SetEncoding(this System.Data.DataTable table, Encoding encoding)
        {
            if (encoding != null)
            {
                foreach (DataRow dr in table.Rows)
                {
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        if (dr[dc].GetType() == typeof(string))
                            dr[dc] = encoding.GetString(Encoding.Default.GetBytes(dr[dc].ToString()));
                    }
                }
            }
            return table;
        }

        public static DataRow SetEncoding(this DataRow dr, Encoding encoding)
        {
            if (encoding != null)
            {
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    if (dr[dc].GetType() == typeof(string))
                        dr[dc] = encoding.GetString(Encoding.Default.GetBytes(dr[dc].ToString()));
                }
            }
            return dr;
        }
    }
}