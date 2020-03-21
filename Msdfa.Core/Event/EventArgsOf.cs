using Msdfa.Core.Entities;
using System;

namespace Msdfa.Core.Event
{
    public class EventArgsOf<TDataType> : EventArgs
    {
        private readonly PropertyHolder<TDataType> _value;
        public TDataType Value
        {
            get
            {
                try
                {
                    return this._value.Value;
                }
                catch (ArgumentException)
                {
                    throw new Exception($"Event nie został obsłużony.");
                }
            }
            set
            {
                this._value.Value = value;
            }
        }

        public EventArgsOf()
        {
            this._value = new PropertyHolder<TDataType>();
        }
    }
}
