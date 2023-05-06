using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Назва товару повинна бути від 5 до 200 символів")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(9000, MinimumLength = 10, ErrorMessage = "Опис товару повинен бути від 10 до 9000 символів")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public IFormFile Image { get; set; }
        public byte SaleProcent { get; set; }
        public int BonusNumber { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public int Cost { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public List<int> ProductCategories { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
