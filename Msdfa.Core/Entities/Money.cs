using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    [Serializable]
    [DebuggerDisplay("{this.ToString()}")]
    public class Money : IComparable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Currency Currency { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (this.AmountRaw == value) return;
                this.AmountRaw = value;
                this.OnPropertyChanged(nameof(this.AmountRaw));

                var amount = Math.Round(this.AmountRaw, 2, MidpointRounding.AwayFromZero);
                if (this._amount == amount) return;
                this._amount = amount;
                this.OnPropertyChanged();
            }
        }

        //private decimal _amountRaw;
        /// <summary>
        /// AmountRaw is used for calculations only. To compare with other money objects use Amount property.
        /// </summary>
        private decimal AmountRaw { get; set; }
        public decimal GetAmountRaw() => this.AmountRaw;
        //{
        //    get { return this._amountRaw; }
        //    private set
        //    {
        //        if (this._amountRaw == value) return;
        //        this._amountRaw = value;
        //        this.OnPropertyChanged();
        //    }
        //}

        public Money() { }
        public Money(decimal amount, Currency currency) : this(amount, currency?.IsoCode) { }
        public Money(decimal amount, string currencyCode)
        {
            this.Amount = amount;
            this.Currency = Currency.GetByCode(currencyCode);
        }
 
        public Money ConvertTo(Currency currency, decimal kurs)
        {
            return new Money(this.Amount * kurs, currency);
        }

        public Money GetCopy() => new Money(this.Amount, this.Currency);

        public Money Exchange(CurrencyKurs kurs)
        {
            var value = 0m;
            Money ret = this.GetCopy();

            if (this.Currency == kurs.IsoCode_Z)
            {
                //                value = kurs.IsReversed
                //                    ? this.Amount * kurs.Kurs
                //                    : this.Amount / kurs.Kurs;

                // 2018-04-21: Zmiana kolejności walut w parach spowodowało konieczność zmiany działań
                value = kurs.IsReversed
                    ? this.Amount / kurs.Kurs
                    : this.Amount * kurs.Kurs;

                ret = new Money(value, kurs.IsoCode_Na);
            }
            else if (this.Currency == kurs.IsoCode_Na)
            {
                //                value = !kurs.IsReversed
                //                    ? this.Amount * kurs.Kurs
                //                    : this.Amount / kurs.Kurs;

                // 2018-04-21: Zmiana kolejności walut w parach spowodowało konieczność zmiany działań
                value = !kurs.IsReversed
                    ? this.Amount / kurs.Kurs
                    : this.Amount * kurs.Kurs;

                ret = new Money(value, kurs.IsoCode_Z);
            }
            else throw new Exception("Para kursów nie zawiera waluty wymienianej.");

            return ret;
            //return new Money(this.Amount / kurs.KursReal, kurs.IsoCode_Na);
        }

        public Money Exchange(ExchangeRates rates, Currency currencyTo)
        {
            var exchangeRate = rates.GetKurs(this.Currency, currencyTo);
            return this.Exchange(exchangeRate);
        }

        public void SetValue(decimal value) => this.Amount = value;

        public static implicit operator decimal(Money item) => item?.Amount ?? 0;

        public static Money operator +(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (+) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (-) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static bool operator >(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (>) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return a.Amount > b.Amount;
        }
        public static bool operator >=(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (>=) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return a.Amount >= b.Amount;
        }
        public static bool operator <(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (<) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return a.Amount < b.Amount;
        }
        public static bool operator <=(Money a, Money b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (<=) on null Money property");
            if (a.Currency != b.Currency) throw new Exception($"Currency codes don't match [{a.Currency.IsoCode} : {b.Currency.IsoCode}]");
            return a.Amount <= b.Amount;
        }

        public static Money operator /(Money a, decimal b) => new Money(a.Amount / b, a.Currency);
        public static Money operator /(Money a, int b) => new Money(a.Amount / b, a.Currency);

        public static Money operator *(Money a, decimal b) => new Money(a.Amount * b, a.Currency);
        public static Money operator *(Money a, int b) => new Money(a.Amount * b, a.Currency);

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Money)) return false;
            var other = (Money)obj;

            if (this.Currency == null) return this.Amount.Equals(other.Amount);
            return this.Amount.Equals(other.Amount) && this.Currency.Equals(other.Currency);
        }

        public static bool operator ==(Money a, Money b)
        {
            if (ReferenceEquals(a, b)) { return true; }                     // If both are null, or both are same instance, return true.
            if (((object) a == null) || ((object) b == null)) return false; // If one is null, but not both, return false.
            return a.Equals(b);                                             // Return true if the fields match
        }
        public static bool operator !=(Money a, Money b) => !(a == b);

        public override int GetHashCode()
        {
            var key = this.Currency?.IsoCode?.GetHashCode() ?? 0;
            var amount = this.Amount.GetHashCode();
            return key ^ amount;
        }

        public int CompareTo(object obj)
        {
            var asMoney = obj as Money;
            if (asMoney != null) return asMoney == null ? 1 : this.AmountRaw.CompareTo(asMoney.AmountRaw);

            var asDecimal = Convert.ToDecimal(obj);
            return this.AmountRaw.CompareTo(asDecimal);
        }

        public override string ToString()
        {
            return $"{this.Amount.ToString("N2")} {this.Currency?.IsoCode}";
        }

        public string PadLeft(int pad)
        {
            return this.ToString().PadLeft(pad + 4);
        }
    }

    public class MoneyComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            decimal val1 = 0, val2 = 0;

            // Porównanie string <-> string
            var asString = x as string;
            if (asString != null && y is string)
            {
                decimal.TryParse(asString, out val1);
                decimal.TryParse((string)y, out val2);
            }

            // Porównanie Money <-> string
            var asMoney = x as Money;
            if (asMoney != null && y is string)
            {
                val1 = asMoney.Amount;
                decimal.TryParse((string)y, out val2);
            }

            return val1.CompareTo(val2);
        }
    }

    public static class SumExtensions
    {
        public static Money Sum(this IEnumerable<Money> source)
        {
            return source.Aggregate((x, y) => x + y);
        }

        public static Money Sum<T>(this IEnumerable<T> source, Func<T, Money> selector)
        {
            return source.Select(selector).Aggregate((x, y) => x + y);
        }
    }
}
