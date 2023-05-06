using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Некоректний email")]
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Це обов'язкове поле")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
