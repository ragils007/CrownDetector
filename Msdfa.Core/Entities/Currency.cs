using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    [DebuggerDisplay("Currency: {IsoCode}")]
    [Serializable]
    public class Currency : IComparable
    {
        public string IsoCode { get; set; }

        public Currency() { }
        public Currency(string isoCode)
        {
            this.IsoCode = isoCode;
        }

        public override bool Equals(object obj)
        {
            return this.IsoCode == (obj as Currency)?.IsoCode;
        }

        public override int GetHashCode()
        {
            return this.IsoCode.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var asCurrency = obj as Currency;
            return asCurrency == null ? 1 : string.Compare(this.IsoCode, asCurrency.IsoCode, StringComparison.Ordinal);
        }

        public static bool operator ==(Currency a, Currency b) => a?.IsoCode == b?.IsoCode;
        public static bool operator !=(Currency a, Currency b) => !(a == b);

        public override string ToString()
        {
            return this.IsoCode;
        }

        public static Currency Empty => new Currency(null);
        public static Currency CZK => new Currency("CZK");
        public static Currency EUR => new Currency("EUR");
        public static Currency GBP => new Currency("GBP");
        public static Currency HUF => new Currency("HUF");
        public static Currency PLN => new Currency("PLN");
        public static Currency USD => new Currency("USD");

        public static implicit operator string(Currency c) => c.IsoCode;
        public static implicit operator Currency(string isoCode) => new Currency(isoCode);

        public static Currency GetByCode(string isoCode) => new Currency(isoCode);
    }
}
