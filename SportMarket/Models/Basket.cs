using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class Basket
    {
        [Key]
        public int BasketId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public User User { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public List<Card> Cards { get; set; }
        public OrderedBasket OrderedBasket { get; set; }
        public int OrderedBasketId { get; set; }
    }
}
