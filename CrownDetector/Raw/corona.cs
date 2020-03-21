using Msdfa.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Msdfa.Data.Raw
{
    [DebuggerDisplay("id: {id}, {imie} {nazwisko}")]
    public class corona : BaseTable<corona>, INotifyPropertyChanged
    {
        [Key]
        public long id { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string pesel { get; set; }
        public string telefon { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime data_w { get; set; }
        public DateTime data_po { get; set; }
        public string opis { get; set; }
        public string uwagi { get; set; }
        public byte[] foto { get; set; }
        public byte[] foto_marked { get; set; }
    }
}
