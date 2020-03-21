using Msdfa.Core.Entities;
// using Infragistics.Documents.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.DataAccess
{
    public class BaseDataReaderColumnInfo
    {
        /// <summary>
        /// Rozróżnienie między DefaultValue == null niezdefiniowanym, a ustaionym na null
        /// </summary>
        //public bool IsDefaultValueSet { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public bool IsNullable { get; private set; }
        public object DefaultValue { get; private set; }
        public bool IsHardSpaceAllowed { get; set; }
        public bool IsTextUppercase { get; set; }
        public bool IsTrimmed { get; set; } = true;

        public BaseDataReaderColumnInfo SetIsHardSpaceAllowed(bool value)
        {
            this.IsHardSpaceAllowed = value;
            return this;
        }

        public BaseDataReaderColumnInfo SetDefaultValue(object value)
        {
            this.DefaultValue = value;
            return this;
        }

        public BaseDataReaderColumnInfo SetIsNullable(bool value = true)
        {
            this.IsNullable = value;
            return this;
        }

        public BaseDataReaderColumnInfo SetTextUppercase(bool value = true)
        {
            this.IsTextUppercase = value;
            return this;
        }

        public BaseDataReaderColumnInfo SetTrimmed(bool value = true)
        {
            this.IsTrimmed = value;
            return this;
        }

        /*
        public object GetValueFromCell(WorksheetCell cell)
        {
            if (this.PropertyInfo.PropertyType == typeof(Money))
            {
                if (cell.Value == null) return cell.GetText();

                if (cell.HasCellFormat == false) throw new Exception("Komórka z typem walutowym powinna posiadać formatowanie.");
                var fmt = cell.CellFormat.FormatString;
                var currency = Currency.Empty;

                if (fmt.Contains("zł") || fmt.Contains("PLN")) currency = Currency.PLN;
                else if (fmt.Contains("$$") || fmt.Contains("USD")) currency = Currency.USD;
                else if (fmt.Contains("$€") || fmt.Contains("EUR")) currency = Currency.EUR;
                else if (fmt.Contains("CZK")) currency = Currency.CZK;
                else if (fmt.Contains("GBP")) currency = Currency.GBP;
                else if (fmt.Contains("HUF")) currency = Currency.HUF;

                if (currency == Currency.Empty) throw new Exception($"[{this.PropertyInfo.Name}] Nie udało się określić waluty na podstawie formatu komórki dla właściwości o typie Money!");
                var value = Convert.ToDecimal(cell.Value);
                return value == new Money(value, currency);
            }

            return cell.Value;
        }
        */
    }
}
