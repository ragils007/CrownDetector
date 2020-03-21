using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Models
{
    public class ListViewModel
    {
        public List<ListItem> ListItems { get; set; }
    }

    public class ListItem
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pesel { get; set; }
        public string Opis { get; set; }
        public DateTime Data_po { get; set; }
    }
}
