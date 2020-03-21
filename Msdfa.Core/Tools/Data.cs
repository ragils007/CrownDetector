using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Msdfa.Tools
{
	public class Data
	{
		public static void DataTableToCsv(DataTable dt, string fileName, char delimiter = '\t', bool exportHeaders = true)
		{
			StringBuilder sb = new StringBuilder();

			if (exportHeaders == true)
			{
				var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
				sb.AppendLine(string.Join(delimiter.ToString(), columnNames));
			}

			foreach (DataRow row in dt.Rows)
			{
                var fields = new List<object>();
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (col.DataType == typeof(string)) fields.Add($@"=""{row[col]}""");
                    else fields.Add(row[col]);
                            
                }

                sb.AppendLine(string.Join(delimiter.ToString(), fields));
			}

			File.WriteAllText(fileName, sb.ToString());
		}

		/// <summary>
		/// !!! Uwaga !!! Przy decimalach, zamienia je przy odtwarzaniu na string.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="delimiter"></param>
		/// <param name="exportHeaders"></param>
		/// <returns></returns>
		public static DataTable DataTableFromCsv(string fileName, char delimiter = '\t', bool exportHeaders = true)
		{
			DataTable temp = new DataTable();

			using (StreamReader sr = new StreamReader(fileName))
			{
				string[] firstLineColumns = sr.ReadLine().Split(delimiter);

				for (int columnNo = 0; columnNo < firstLineColumns.Length; columnNo++)
				{
					temp.Columns.Add(exportHeaders == true ? firstLineColumns[columnNo].ToString() : "");
				}
			}

			foreach (string line in File.ReadLines(fileName).Skip(exportHeaders == true ? 1 : 0))
			{
				temp.Rows.Add(line.Split(delimiter));
			}
			return temp;	
		}
	}
}
