using CrownDetector.DB;
using Msdfa.Data;
using Msdfa.Data.Raw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector
{
    public class DBSample
    {
        public void ProcessCorona()
        {
            using (var cc = CC.GetCorona())
            {
                // Pobranie listy rekordów z bazy
                var dataList = cc.Cnn.Query<corona>("SELECT * FROM corona").ToList();

                // Pobranie pojedynczego rekordu
                var dataItem = cc.Cnn.Query<corona>("SELECT * FROM corona WHERE id = :id")
                    .Bind("id", 1)
                    .FetchItem();

                // Dodanie nowego rekordu
                var img = File.ReadAllBytes(@"d:\indeks.jpg");
                var img2 = File.ReadAllBytes(@"d:\csdb.png");

                var newItem = new corona
                {
                    imie = "Jan",
                    nazwisko = "Kowalski",
                    pesel = "123456",
                    opis = "Dobry gość",
                    data_po = new DateTime(1978, 1, 1),
                    foto = img,
                    foto_marked = img2,
                };
                newItem.Save(cc.Cnn);
            }
        }
    }
}
