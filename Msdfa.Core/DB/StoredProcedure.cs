using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Msdfa.Core.Tools.MyDataTable;

namespace Msdfa.DB
{
    public class StoredProcedure : IStoredProcedure
    {
        public Dictionary<string, object> BindValues;

        public StoredProcedure(IDatabase db, string procedureName = null)
        {
            Db = db;
            BindValues = new Dictionary<string, object>();
            _ProcedureName = procedureName;
        }

        internal IDatabase Db { get; set; }
        protected string _ProcedureName { get; set; }

        public IStoredProcedure Bind(string varName, object varValue)
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
            else if (varValue.GetType() == typeof (bool)
                     || varValue.GetType() == typeof (bool?))
            {
                varValue = Convert.ToBoolean(varValue) ? 1 : 0;
            }

            BindValues.Add(varName, varValue);
            return this;
        }

        public IStoredProcedure BindAll(Dictionary<string, object> values)
        {
            foreach (var item in values)
            {
                Bind(item.Key, item.Value);
            }
            return this;
        }

        public void Execute()
        {
            lock (Db)
            {
                Db.ExecuteProcedure(_ProcedureName, new Dictionary<string, object>(BindValues));
            }
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