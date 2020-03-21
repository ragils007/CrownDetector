using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class DataTableTools
    {
        public static DataTable CreateDataTable<T>(List<T> items)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            var dt = new DataTable();
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = properties.Select(x => x.GetValue(item));
                dt.Rows.Add(values);
            }

            return dt;
        }
    }
}
