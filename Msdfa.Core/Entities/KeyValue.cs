using System.ComponentModel.DataAnnotations;
using Msdfa.Core.Tools.Extensions;

namespace Msdfa.Core.Entities
{
    public class KeyValue
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValue() {}

        public KeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString() => this.Value;
    }

    public class KeyValue<TKey, TValue>
    {
        [Key]
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyValue() { }

        public KeyValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}