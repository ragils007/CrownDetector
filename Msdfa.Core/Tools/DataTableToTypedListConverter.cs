using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Tools.DataAccess;

namespace Msdfa.Core.Tools
{
    public class DataTableToTypedListConverter<TDataType> where TDataType : new()
    {
        private DataTable _dt;
        public Dictionary<PropertyInfo, string> MappedProperties = new Dictionary<PropertyInfo, string>();

        public DataTableToTypedListConverter(DataTable dt)
        {
            _dt = dt;
        }

        private void Map(Expression<Func<TDataType, object>> expression, string name)
        {
            throw new NotImplementedException();
        }

        public virtual void AutoMapProperties()
        {
            var properties = GetPropertiesToMap().ToDictionary(x => UnifyName(x.Name), x => x);
            foreach (DataColumn col in _dt.Columns)
            {
                var colName = UnifyName(col.ColumnName);
                if (!properties.ContainsKey(colName))
                    throw new Exception("Unable to map property #" + col.ColumnName + "#");

                var prop = properties[colName];
                MappedProperties.Add(prop, col.ColumnName);
            }
        }

        private List<PropertyInfo> GetPropertiesToMap()
        {
            var properties = typeof(TDataType).GetProperties();
            var result = new List<PropertyInfo>();
            foreach (var prop in properties)
            {
                var notMappedAttr = prop.GetCustomAttribute<NotMappedAttribute>();
                if (notMappedAttr != null) continue;

                result.Add(prop);
            }

            return result;
        }

        public List<TDataType> Convert()
        {
            if (MappedProperties.Count == 0) 
                AutoMapProperties();

            var result = new List<TDataType>();
            foreach (DataRow row in _dt.Rows)
                result.Add(GetItemFromDataRow(row));

            return result;
        }

        private TDataType GetItemFromDataRow(DataRow row)
        {
            var item = new TDataType();
            foreach (var prop in MappedProperties)
            {
                var value = ValueConverter.Get(row[prop.Value], prop.Key.PropertyType);
                prop.Key.SetValue(item, value);
            }

            return item;
        }

        private string UnifyName(string name)
        {
            return name.ToUpper().Replace("_", "").Replace(" ", "");
        }
    }
}
