using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Msdfa.Core.Base
{
    public class TypeDetails
    {
        public static Dictionary<Type, TypeDetails> TypeToDetailsDict = new Dictionary<Type, TypeDetails>();

        public Type Type { get; set; }
        public string IdColumnName { get; set; }
        public Dictionary<string, PropertyInfo> PropertyInfoDict { get; set; }
        public Dictionary<string, PropertyInfo> PropertyInfoWritableDict { get; set; }

        public Type GetIdType() => this.PropertyInfoDict[this.IdColumnName].PropertyType;
        public object GetId(object o) => this.PropertyInfoDict[this.IdColumnName].GetValue(o);
        public T GetId<T>(object o) => (T)this.GetId(o);
    }
}
