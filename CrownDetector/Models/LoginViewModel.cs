using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace CrownDetector.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Login:")]
        [Required(ErrorMessage = "This field is required")]
        public string Login { get; set; }

        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; }

    }
}
