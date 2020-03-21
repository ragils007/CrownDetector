using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    [DebuggerDisplay(@"Kurs: {IsoCode_Z}-{IsoCode_Na}, {Kurs} {IsReversed == true ? ""Rev"" : """"}")]
    public class CurrencyKurs
    {
        public string KursTyp { get; set; }
        public string IsoCode_Z { get; set; }
        public string IsoCode_Na { get; set; }
        public decimal KursZa { get; set; }
        public decimal Kurs { get; set; }
        public decimal KursJedn => this.Kurs / this.KursZa;
        public decimal KursReal => this.IsReversed ? (1 / this.KursJedn) : this.KursJedn;

        public string GetSnapshot() => $"{this.KursTyp}|{this.IsoCode_Z}|{this.IsoCode_Na}|{this.KursZa}|{this.Kurs}";

        /// <summary>
        /// Kurs powstał z kursu przeciwnego. Przykład:
        /// Mamy w systemie kurs PLN -> EUR (= 4.50) 
        /// Poszukujemy kursu EUR -> PLN, którego w systemie nie mamy
        /// Przy założeniu symetrii kursów moglibyśmy zwrócić wartość przeliczoną, czyli (1 / kurs w systemie) - jednak 
        /// takie dzielenie często zwraca bardzo niedokładny wynik (wiele miejsc po przecinku).
        /// Rozwiązaniem jest zwrócenie kursu PLN -> EUR z flagą IsReversed. 
        /// Operacje przeliczenia korzystające z tego kursu zamiast mnożyć przez taki kurs - dzieliłyby
        /// </summary>
        public bool IsReversed { get; set; }

        public CurrencyKurs() { }

        public CurrencyKurs(string isoCodeZ, string isoCodeNa, decimal value, decimal kursZa = 1, string typ = null)
        {
            this.IsoCode_Z = isoCodeZ;
            this.IsoCode_Na = isoCodeNa;
            this.Kurs = value;
            this.KursZa = kursZa;
            this.KursTyp = typ;
        }

        public Money Exchange(decimal value)
        {
            var exchanged = this.IsReversed 
                ? Math.Round(value * this.KursJedn, 2) 
                : Math.Round(value / this.KursJedn, 2);
            return new Money(exchanged, this.IsoCode_Na);
        }

        public static implicit operator decimal(CurrencyKurs item) => item?.KursJedn ?? 0;
    }
}
