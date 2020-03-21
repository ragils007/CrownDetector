using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Models
{
    public class HomeViewModel
    {
        [Display(Name = "Imię")]
        [Required(ErrorMessage = "Pole wymagane")]
        public string Imie { get; set; }
        [Display(Name = "Nazwisko")]
        [Required(ErrorMessage = "Pole wymagane")]
        public string Nazwisko { get; set; }
        [Display(Name = "Pesel")]
        [Required(ErrorMessage = "Pole wymagane")]
        public string Pesel { get; set; }
        [Display(Name = "Opis")]
        [Required(ErrorMessage = "Pole wymagane")]
        public string Opis { get; set; }
        [Display(Name = "Data")]
        [Required(ErrorMessage = "Pole wymagane")]
        public DateTime Data_po { get; set; }
        [Required(ErrorMessage = "Pole wymagane")]
        public IFormFile AttachedFile { get; set; }
        public string Result { get; set; }
        public string Src { get; set; }
        public decimal Procent { get; set; }
        public string Class { get; set; }
    }
}
