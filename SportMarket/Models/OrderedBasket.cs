using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class OrderedBasket
    {
        public int Id { get; set; }
        public Basket Basket { get; set; }
        [ForeignKey("Basket")]
        public int BasketId { get; set; }
        [EmailAddress(ErrorMessage = "Некоректний email")]
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Довжина рядка повинна бути від 2 до 15 символів")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Довжина рядка повинна бути від 2 до 15 символів")]
        public string Surname { get; set; }
        [StringLength(15, MinimumLength = 0, ErrorMessage = "Довжина рядка повинна бути до 15 символів")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [Phone(ErrorMessage = "Некоректний телефон")]
        public string Phone { get; set; }
        public Address Address { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public int AddressId { get; set; }
        public bool Bonuses { get; set; }
        public bool BonusesCredited { get; set; }
        public bool BonusesWrittenOff { get; set; }
        public int BonusesNumber { get; set; }
        public bool QuantityWrittenOff { get; set; }
        public DateTime Date { get; set; }
    }
}
