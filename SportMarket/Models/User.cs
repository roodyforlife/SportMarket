using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public bool Male { get; set; }
        public DateTime RegistDate { get; set; } = DateTime.Now;
        public int Bonuses { get; set; }
        public byte[] Image { get; set; }
        public List<Basket> Baskets { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public int TelegramId { get; set; }
    }
}
