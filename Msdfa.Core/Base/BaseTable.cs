using Msdfa.Core.DB;
using Msdfa.Core.Entities;
using Msdfa.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Msdfa.Core.Base
{
    public class BaseTable
    {
        private readonly IBaseTableGeneric Parent;

        private readonly TypeDetails TypeDetails;
        public readonly string TableName;

        public Dictionary<string, OldNewValue> PropertyValueDict = new Dictionary<string, OldNewValue>();
        public Dictionary<string, OldNewValue> ModifiedPropertyDict => this.PropertyValueDict
            .Where(x => x.Value.OldValue != x.Value.NewValue)
            .ToDictionary(x => x.Key, x => x.Value);

        public bool IsPersisted { get; set; }
        public bool IsIdHandledManually { get; set; }
        public bool IsIdColumnAutoFilled { get; set; } = true;
        public string RowVersionColumnName { get; set; }

        public BaseTable(IBaseTableGeneric parent, string tableName, TypeDetails details)
        {
            this.Parent = parent;
            this.TableName = tableName;
            this.TypeDetails = details;

            this.InitPropertyValueDict();
        }

        private void InitPropertyValueDict()
        {
            this.PropertyValueDict = this.TypeDetails.PropertyInfoDict.Select(x => x.Key).ToDictionary(x => x, x => new OldNewValue());

            foreach (var propItem in this.PropertyValueDict)
            {
                var propType = this.TypeDetails.PropertyInfoDict[propItem.Key].PropertyType;
                if (propType == typeof(string)) continue;
                if (propType.IsArray) continue;

                propItem.Value.OldValue = Activator.CreateInstance(propType);
                propItem.Value.NewValue = propItem.Value.OldValue;
            }
        }

        //public void SetExistingValue(string property, object value)
        //{
        //    if (this.PropertyValueDict.ContainsKey(property) == false) this.PropertyValueDict.Add(property, new OldNewValue());
        //    this.PropertyValueDict[property].OldValue = value;
        //    this.PropertyValueDict[property].NewValue = value;
        //    this.IsPersisted = true;
        //}

        //private void SyncModifiedProperties()
        //{
        //    foreach (var prop in this.TypeDetails.PropertyInfoDict)
        //    {
        //        var newVal = prop.Value.GetValue(this.Parent);
        //        this.PropertyValueDict[prop.Key].NewValue = newVal;
        //    }
        //}

        public void Save(IConnection cnn, bool refreshFromDB = true)
        {
            //this.SyncModifiedProperties();

            if (this.IsPersisted == false) this.Insert(cnn);
            else this.Update(cnn);
        }

        private void Insert(IConnection cnn)
        {
            if (this.TypeDetails.GetIdType() == typeof(long))
            {
                var id = this.TypeDetails.GetId<long>(this.Parent);
                if (id > 0 && this.IsIdHandledManually == false) throw new Exception("Tried to insert row with ID filled.");
            }

            var propList = this.TypeDetails.PropertyInfoWritableDict.Keys.OrderBy(x => x).ToList();
            if (this.IsIdHandledManually == false && this.IsIdColumnAutoFilled) propList.Remove(this.TypeDetails.IdColumnName);

            var insertSql = $"INSERT INTO {this.TableName} ({string.Join(',', propList) }) VALUES ({string.Join(',', propList.Select(x => $":{x}"))})";

            var query = cnn.Query(insertSql);
            foreach (var item in propList) query.Bind(item, this.PropertyValueDict[item].NewValue, this.TypeDetails.PropertyInfoDict[item].PropertyType);
            var newId = query.ExecuteReturning(new Dictionary<string, Type>() { { "id", typeof(long) } });
            this.Parent.SetId(newId["id"]);

            this.PropertyValueDictUpdateToNewValues();

            this.IsPersisted = true;
        }

        private void Update(IConnection cnn)
        {
            var propWritebleList = this.TypeDetails.PropertyInfoWritableDict.Keys.OrderBy(x => x).ToList();

            var propList = this.PropertyValueDict
                .Where(x => propWritebleList.Contains(x.Key))
                .Where(x => x.Value.HasEqualValues() == false)
                .Select(x => x.Key)
                .ToList();

            propList.Remove(this.TypeDetails.IdColumnName);
            if (!propList.Any()) return;

            var setPairs = propList.Select(x => $"{x}=:{x}").ToList();
            var updateSql = $"UPDATE {this.TableName} SET {string.Join(',', setPairs)} WHERE {this.TypeDetails.IdColumnName}=:{this.TypeDetails.IdColumnName}";

            var query = cnn.Query(updateSql);
            foreach (var item in propList) query.Bind(item, this.PropertyValueDict[item].NewValue);
            query.Bind(this.TypeDetails.IdColumnName, this.PropertyValueDict[this.TypeDetails.IdColumnName].NewValue);
            query.ExecuteReturning();

            this.PropertyValueDictUpdateToNewValues();
        }

        private void PropertyValueDictUpdateToNewValues()
        {
            foreach (var item in this.PropertyValueDict)
            {
                item.Value.OldValue = item.Value.NewValue;
            }
        }
    }

    public class BaseTable<T> : IBaseTableGeneric
        where T: class, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

        public BaseTable _base { get; set; }

        public BaseTable()
        {
            this.PropertyChanged += this.BaseTable_PropertyChanged;

            if (!TypeDetails.TypeToDetailsDict.ContainsKey(typeof(T))) this.InitType();
            this._base = new BaseTable(this, typeof(T).Name, TypeDetails.TypeToDetailsDict[typeof(T)]);
        }
        
        private void InitType()
        {
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .ToList();
            var key = props.SingleOrDefault(x => x.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (key == null) throw new Exception($"Type: {typeof(T).FullName}. [Key] attribute not found.");

            var typeDetails = new TypeDetails
            {
                IdColumnName = key.Name,
                PropertyInfoDict = props.ToDictionary(x => x.Name, x => x),
                PropertyInfoWritableDict = props
                    .Where(x => x.CanWrite)
                    .Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                    .ToDictionary(x => x.Name, x => x),
                Type = typeof(T)
            };

            TypeDetails.TypeToDetailsDict.Add(typeof(T), typeDetails);
        }

        protected void BaseTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var value = TypeDetails.TypeToDetailsDict[typeof(T)].PropertyInfoDict[e.PropertyName].GetValue(this);
            this._base.PropertyValueDict[e.PropertyName].NewValue = value;
        }

        public object GetId()
        {
            var td = TypeDetails.TypeToDetailsDict[typeof(T)];
            var ret = td.PropertyInfoDict[td.IdColumnName].GetValue(this);
            return ret;
        }

        public void SetId(object id) 
        {
            var td = TypeDetails.TypeToDetailsDict[typeof(T)];
            td.PropertyInfoDict[td.IdColumnName].SetValue(this, id);
        }

        public void Save(IConnection cnn) => this._base.Save(cnn);

        /// <summary>
        /// Wczytuje zawartość obiektu z bazy danych
        /// </summary>
        public virtual void RefreshFromDB(IConnection cnn)
        {
            var td = TypeDetails.TypeToDetailsDict[typeof(T)];
            var id = td.PropertyInfoDict[td.IdColumnName].GetValue(this);

            var data = cnn.Query($"SELECT * FROM {td.Type.Name} WHERE {td.IdColumnName}=:ID").Bind("ID", id).Fetch();

            //var ret = this._base.

            

            //var id = this.IdValue;
            //var data = this.Base.Factory.GetDataRowBy_Id(id.GetValuesArray());
            //if (data == null)
            //    throw new Exception($"Unable to get object type: [{this.GetType().Name}] id: [{id.KeysString}]");
            //this.SetPropertyValuesFromDataRow(data);

            //if (cnn != null) this.UnsetConnection();
        }
    }
}
