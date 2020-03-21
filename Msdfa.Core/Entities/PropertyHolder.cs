using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    /// <summary>
    /// Klasa służy do przechowywania wartości propertiesów.
    /// Od normalnych propertiesów różni się tym, że żeby pobrać z niej wartość - wpierw musi ona zostać ustawiona.
    /// Próba odczytania wartości bez jej ustawienia skutkuje stosownym wyjątkiem.
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    [DebuggerDisplay("{Value}")]
    public class PropertyHolder<TDataType>
    {
        private bool IsSet { get; set; }

        private TDataType _value;
        public TDataType Value
        {
            get
            {
                if (!this.IsSet) throw new ArgumentException($"PropertyHolder<{typeof(TDataType).Name}>: Value is not set");
                return this._value;
            }
            set
            {
                this.IsSet = true;
                this._value = value;
            }
        }

        public void Unset()
        {
            this.IsSet = false;
            this.Value = default(TDataType);
        }

        public static implicit operator TDataType(PropertyHolder<TDataType> item) => item.Value;
        public static implicit operator PropertyHolder<TDataType>(TDataType item) => new PropertyHolder<TDataType>() { Value = item };

        public void Do(Action<TDataType> a)
        {
            a.Invoke(this.Value);
        }

        public void DoIfSet(Action<TDataType> a)
        {
            if (!this.IsSet) return;
            a.Invoke(this.Value);
        }
    }
}
