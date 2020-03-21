using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Msdfa.Core.Entities
{
    public class IdValue
    {
        [Key]
        public long Id { get; set; }
        public string Value { get; set; }

        public IdValue() { }
        public IdValue(long id, string key)
        {
            this.Id = id;
            this.Value = key;
        }

        public override string ToString() { return this.Value; }

        public static List<IdValue> YesNoList
        {
            get
            {
                var ret = new List<IdValue>();
                ret.Add(new IdValue(0, "Nie"));
                ret.Add(new IdValue(1, "Tak"));
                return ret;
            }
        }
    }

    public class IdValueDescription : IdValue
    {
        public string Description { get; set; }

        public IdValueDescription(long id, string value, string description) : base(id, value)
        {
            this.Description = description;
        }
    }
}