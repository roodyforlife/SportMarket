using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class EditViewModel
    {
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Довжина рядка повинна бути від 2 до 15 символів")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Довжина рядка повинна бути від 2 до 15 символів")]
        public string Surname { get; set; }
        [StringLength(15, MinimumLength = 0, ErrorMessage = "Довжина рядка повинна бути до 15 символів")]
        public string LastName { get; set; }

        [RegularExpression(@"[A-Za-z0-9_]+", ErrorMessage = "Пароль повинен складатися з латинських букв, цифр та символа '_'")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Довжина паролю повинна бути від 6 до 20 символів")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public DateTime Birthday { get; set; }
        public bool Male { get; set; }
        public IFormFile Image { get; set; }
        public string ReturnUrl { get; set; }
    }
}
