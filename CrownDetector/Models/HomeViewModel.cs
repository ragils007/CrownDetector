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
        public IFormFile AttachedFile { get; set; }
        public string Result { get; set; }
        public decimal Procent { get; set; }
    }
}
