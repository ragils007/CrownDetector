using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    public class ExchangeRates
    {
        private Currency _baseCurrency = Currency.PLN;

        public string Typ { get; set; }
        public DateTime DataKursu { get; set; }
        public List<CurrencyKurs> Kursy { get; set; } = new List<CurrencyKurs>();

        public virtual void SetKurs(string walutaZ, string walutaNa, decimal value, decimal kursZa = 1, string kursTyp = null)
        {
            this.Kursy.RemoveAll(x => x.IsoCode_Z == walutaZ && x.IsoCode_Na == walutaNa && x.KursTyp == kursTyp);
            this.Kursy.Add(new CurrencyKurs(walutaZ, walutaNa, value, kursZa, kursTyp) { IsReversed = false });
            this.Kursy.Add(new CurrencyKurs(walutaNa, walutaZ, value, kursZa, kursTyp) { IsReversed = true });
        }

        public virtual CurrencyKurs GetKurs(string walutaZ, string walutaNa)
        {
            if (walutaZ == walutaNa) return new CurrencyKurs(walutaZ, walutaNa, 1);

            if (!this.Kursy.Any()) throw new Exception($"[{this.DataKursu.Date.ToString("yyyy-MM-dd")}, {walutaZ} => {walutaNa}]: Brak kursu");

            var kurs = this.Kursy.FirstOrDefault(x => x.IsoCode_Z == walutaZ && x.IsoCode_Na == walutaNa);
            if (kurs != null) return kurs;

            var kursZ = this.Kursy.FirstOrDefault(x => x.IsoCode_Z == walutaZ && x.IsoCode_Na == _baseCurrency);
            var kursNa = this.Kursy.FirstOrDefault(x => x.IsoCode_Z == _baseCurrency && x.IsoCode_Na == walutaNa);

            if (kursZ != null && kursNa != null)
            {
                kurs = new CurrencyKurs(walutaZ, walutaNa, kursZ.KursReal * kursNa.KursReal);
                return kurs;
            }

            throw new Exception($"[{this.DataKursu.ToString("yyyy-MM-dd")} {walutaZ} => {walutaNa}]: Brak kursu");
        }
    }
}
