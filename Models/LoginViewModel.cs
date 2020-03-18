using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace CrownDetector.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Login")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Login { get; set; }

        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Password { get; set; }

    }
}
