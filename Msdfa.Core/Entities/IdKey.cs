using System;
using System.Runtime.CompilerServices;
using Msdfa.Core.Tools;
using Newtonsoft.Json;

namespace Msdfa.Core.Entities
{
    [Serializable]
    public class IdKey : IComparable
    {
        public readonly bool IsLong = false;

        public readonly long Id = int.MinValue;
        public readonly string Key;

        public override string ToString() => this.IsLong ? this.Id.ToString() : this.Key;

        public IdKey(long id)
        {
            this.IsLong = true;
            this.Id = id;
        }

        public IdKey(string key)
        {
            if (string.IsNullOrEmpty(key.Trim())) throw new Exception("IdKey: Key cannot be null value");

            this.IsLong = false;
            this.Key = key;
        }

        [JsonConstructor]
        public IdKey(long id, string Key, bool IsLong)
        {
            this.Id = id;
            this.Key = Key;
            this.IsLong = IsLong;

            if (!string.IsNullOrEmpty(this.Key) && this.IsLong) throw new Exception("Type Mismatch error!");
        }


        public static bool operator ==(IdKey a, long b) => a?.Id >= 0 && a.Id == b;
        public static bool operator !=(IdKey a, long b) => !(a == b);

        public static bool operator ==(IdKey a, int b) => a?.Id >= 0 && a.Id == b;
        public static bool operator !=(IdKey a, int b) => !(a == b);

        public static bool operator ==(IdKey a, string b) => a?.Key != null && a.Key == b;
        public static bool operator !=(IdKey a, string b) => !(a == b);

        //
        public override bool Equals(object obj)
        {
            var type = obj?.GetType();

            if (type == typeof(IdKey))
            {
                var other = (IdKey)obj;
                return (this.Id == other.Id && this.Key == other.Key);
            }
            if (type == typeof(long)) return this.Id == (long)obj;
            if (type == typeof(string)) return this.Key == (string)obj;

            return false;
        }

        public static bool operator ==(IdKey a, IdKey b)
        {
            if (ReferenceEquals(a, b)) { return true; }                     // If both are null, or both are same instance, return true.
            if (((object)a == null) || ((object)b == null)) return false;   // If one is null, but not both, return false.
            return a.Equals(b);                                             // Return true if the fields match
        }
        public static bool operator !=(IdKey a, IdKey b) => !(a == b);

        public static implicit operator IdKey(long a) => new IdKey(a);
        public static implicit operator IdKey(string a) => new IdKey(a);

        public static implicit operator long(IdKey idKey) => idKey.Id;
        public static implicit operator string(IdKey idKey) => idKey.Key;

        public override int GetHashCode()
        {
            return this.IsLong 
                ? this.Id.GetHashCode() 
                : this.Key.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var asIdKey = obj as IdKey;
            if (asIdKey == (IdKey) null) return 0;

            return this.IsLong
                ? this.Id.CompareTo(asIdKey.Id)
                : string.Compare(this.Key, asIdKey.Key, StringComparison.Ordinal);
        }
    }
}
