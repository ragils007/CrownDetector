using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msdfa.Core.Base;
using Msdfa.Core.Entities;
using Msdfa.Core.Tools;
using Msdfa.Core.Tools.MyDataTable;
using Msdfa.DB;
using Msdfa.Tools.ExpressionTrees;

namespace Msdfa.Core.DB
{
    public class QueryTyped<TDataType> : Query
        where TDataType : class, new()
    {
        private Dictionary<string, string> PropertyToColumnMappingDict = new Dictionary<string, string>();
        private Dictionary<string, PropertyInfo> _propertyCacheDict;
        private Dictionary<string, dynamic> selectors = new Dictionary<string, dynamic>();
        private HashSet<string> _notMappedProperties = new HashSet<string>();

        public bool IsIgnoreNotMappedPropertiesOn { get; set; }

        public string ResultTableAlias { get; set; }

        public QueryTyped<TDataType> SetIgnoreNotMappedProperties(bool value = true)
        {
            this.IsIgnoreNotMappedPropertiesOn = value;
            return this;
        }

        public QueryTyped(IDatabase db, string queryString = null) : base(db, queryString) { }

        public QueryTyped<TDataType> Bind<TVariableType>(string varName, TVariableType varValue)
        {
            base.Bind(varName, varValue);
            return this;
        }

        public new QueryTyped<TDataType> BindIf(bool isTrue, string varName, object varValue)
        {
            if (isTrue) base.Bind(varName, varValue);
            return this;
        }


        public new QueryTyped<TDataType> BindListIn<TVariableType>(string varName, List<TVariableType> varValues)
        {
            base.BindListIn(varName, varValues);
            return this;
        }

        public QueryTyped<TDataType> BindListInIf<TVariableType>(bool isTrue, string varName, List<TVariableType> varValues)
        {
            if (isTrue) return BindListIn(varName, varValues);
            return this;
        }

        public QueryTyped<TDataType> Map(Expression<Func<TDataType, object>> property, string columnName)
        {
            var propertyName = ExpressionDetail.Create(property).Name;
            if (this.PropertyToColumnMappingDict.ContainsKey(propertyName)) throw new Exception($"Property [{propertyName}] already mapped.");

            this.PropertyToColumnMappingDict.Add(propertyName, columnName);
            return this;
        }

        public QueryTyped<TDataType> Map<TPropertyType>(Expression<Func<TDataType, object>> property, Func<MyDataRow, TPropertyType> selector)
        {
            var propertyName = ExpressionDetail.Create(property).Name;
            this.PropertyToColumnMappingDict.Add(propertyName, null);
            this.selectors.Add(propertyName, selector);
            return this;
        }

        public QueryTyped<TDataType> SetNotMappedProperties(params Expression<Func<TDataType, object>>[] properties)
        {
            foreach (var prop in properties)
            {
                var propName = ExpressionDetail.Create(prop).Name;
                this._notMappedProperties.Add(propName);
            }
            return this;
        }

        //public QueryTyped<TDataType> SetTableAlias(string alias)
        //{
        //    this.ResultTableAlias = alias;
        //    return this;
        //}

        private void CreatePropertyCache()
        {
            if (this._propertyCacheDict == null)
            {
                if (TypeDetails.TypeToDetailsDict.ContainsKey(typeof(TDataType))) this._propertyCacheDict = TypeDetails.TypeToDetailsDict[typeof(TDataType)].PropertyInfoWritableDict;
                else
                {
                    this._propertyCacheDict = typeof(TDataType)
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(x => x.CanWrite)
                        .Where(x => !this._notMappedProperties.Contains(x.Name))
                        .Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                        .ToDictionary(x => x.Name, x => x);
                }
            }
        }

        public TDataType FetchItem()
        {
            this.CreatePropertyCache();

            var row = base.FetchRow();
            if (row == null) return null;

            this.AutoMapProperties(row);

            var item = this.GetItemFromDataRow(row);
            return item;

            //var newObj = new TDataType();
            //foreach (var mapping in this.PropertyToColumnMappingDict)
            //{
            //    var pi = this._propertyCacheDict[mapping.Key];

            //    var value = mapping.Value == null
            //        ? this.selectors[mapping.Key](new MyDataRow(row))
            //        : ValueConverter.Get(row[mapping.Value], pi.PropertyType);

            //    pi.SetValue(newObj, value);
            //}

            //return newObj;
        }

        public List<TDataType> ToList() => this.Fetch();

        public new List<TDataType> Fetch()
        {
            this.CreatePropertyCache();
            //if (this.QueryString.Contains("SELECT [*]"))
            //{
            //    var columns = this._propertyCacheDict.Select(x => this.ResultTableAlias == null ? x.Key : $"{this.ResultTableAlias}.{x.Key}").ToList();
            //    this.QueryString = this.QueryString.Replace("SELECT [*]", $"SELECT {string.Join($"{Environment.NewLine}   , ", columns)}");
            //}

            var data = base.Fetch();
            if (data.Rows.Count == 0) return new List<TDataType>();

            this.AutoMapProperties(data.Rows[0]);

            var temp = new List<TDataType>();
            foreach (var dr in data.AsEnumerable())
            {
                var item = this.GetItemFromDataRow(dr);
                temp.Add(item);
            }
            return temp;
        }

        public async Task<List<TDataType>> FetchAsync()
        {
            this.CreatePropertyCache();

            var ret = await Task.Run(() =>
            {
                var data = base.Fetch();
                if (data.Rows.Count == 0) return new List<TDataType>();

                this.AutoMapProperties(data.Rows[0]);

                var temp = new List<TDataType>();
                foreach (var dr in data.AsEnumerable())
                {
                    var item = this.GetItemFromDataRow(dr);
                    temp.Add(item);
                }
                return temp;
            });
            return ret;
        }

        public async new Task<List<TDataType>> FetchAsync(CancellationToken cancellationToken)
        {
            this.CreatePropertyCache();

            var data = await base.FetchAsync(cancellationToken).ConfigureAwait(false);
            if (data.Rows.Count == 0) return new List<TDataType>();

            this.AutoMapProperties(data.Rows[0]);

            var temp = new List<TDataType>();
            foreach (var dr in data.AsEnumerable())
            {
                var item = this.GetItemFromDataRow(dr);
                temp.Add(item);
            }
            return temp;
        }

        private TDataType GetItemFromDataRow(DataRow dr)
        {
            var newObj = new TDataType();
            var asBaseTable = newObj as BaseTable<TDataType>;

            foreach (var mapping in this.PropertyToColumnMappingDict)
            {
                try
                {
                    var pi = this._propertyCacheDict[mapping.Key];

                    if (this.currencyColumnMappings.ContainsKey(mapping.Key))
                    {
                        var value = Convert.ToDecimal(dr[mapping.Value]);
                        var currency = dr[this.currencyColumnMappings[mapping.Key]].ToString();
                        pi.SetValue(newObj, new Money(value, currency));
                    }
                    else if (this.currencyMappings.ContainsKey(mapping.Key))
                    {
                        var value = Convert.ToDecimal(dr[mapping.Value]);
                        pi.SetValue(newObj, new Money(value, this.currencyMappings[mapping.Key]));
                    }
                    else
                    {
                        var value = mapping.Value == null
                            ? this.selectors[mapping.Key](new MyDataRow(dr))
                            : ValueConverter.Get(dr[mapping.Value], pi.PropertyType);

                        pi.SetValue(newObj, value);
                        //asBaseTable?._base.SetExistingValue(pi.Name, value);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"[{mapping.Key}] Błąd mapowania kolumny: {e.Message}", e);
                }
            }
            return newObj;
        }

        private void AutoMapProperties(DataRow dr)
        {
            var missingProperties = this._propertyCacheDict.Keys
                .Where(x => !this.PropertyToColumnMappingDict.ContainsKey(x))
                .Where(x => !this.selectors.ContainsKey(x));

            // Dictionary of dtColumns in format: <COLUMNNAME, ColumnName>
            var drColumns = dr.Table.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .Select(x => new KeyValuePair<string, string>(x.ToUpper(), x))
                .ToDictionary(x => x.Key, x => x.Value);
            var drColumnsClean = drColumns.ToDictionary(x => x.Key.Replace("_", ""), x => x.Value);

            foreach (var prop in missingProperties)
            {
                var propUpper = prop.ToUpper();
                if (drColumns.ContainsKey(propUpper)) this.PropertyToColumnMappingDict.Add(prop, drColumns[propUpper]);
                else if (drColumnsClean.ContainsKey(propUpper)) this.PropertyToColumnMappingDict.Add(prop, drColumnsClean[propUpper]);
                else if (!this.IsIgnoreNotMappedPropertiesOn)
                {
                    // Próbujemy znaleźć kolumnę do propertiesa bez '_':
                    var propUpperClean = propUpper.Replace("_", "");
                    if (drColumnsClean.ContainsKey(propUpperClean)) this.PropertyToColumnMappingDict.Add(prop, drColumnsClean[propUpperClean]);

                    else throw new Exception($"Unable to automap property: {prop}");
                }

            }
        }

        private Dictionary<string, string> currencyMappings = new Dictionary<string, string>();
        private Dictionary<string, string> currencyColumnMappings = new Dictionary<string, string>();
        public QueryTyped<TDataType> SetCurrencyCode(string currencyCode, params Expression<Func<TDataType, Money>>[] properties)
        {
            foreach (var property in properties)
            {
                var propertyName = ExpressionDetail.Create(property).Name;
                this.currencyMappings.Add(propertyName, currencyCode);
            }
            return this;
        }

        /// <summary>
        /// Ustawienie waluty dla typu Money
        /// </summary>
        /// <param name="currencyColumnName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public QueryTyped<TDataType> SetCurrencyColumn(string currencyColumnName, params Expression<Func<TDataType, Money>>[] properties)
        {
            foreach (var property in properties)
            {
                var propertyName = ExpressionDetail.Create(property).Name;
                this.currencyColumnMappings.Add(propertyName, currencyColumnName);
            }
            return this;
        }
    }
}
