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
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Required field")]
        public string Imie { get; set; }
        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Required field")]
        public string Nazwisko { get; set; }
        [Display(Name = "Pesel")]
        [Required(ErrorMessage = "Required field")]
        public string Pesel { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Required field")]
        public string Opis { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Required field")]
        public DateTime Data_po { get; set; }
        [Required(ErrorMessage = "Required field")]
        public IFormFile AttachedFile { get; set; }
        public string Result { get; set; }
        public string Src { get; set; }
        public decimal Procent { get; set; }
        public string Class { get; set; }
    }
}
