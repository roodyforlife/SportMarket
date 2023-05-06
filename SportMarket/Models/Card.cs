using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class Card
    {
        public int CardId { get; set; }
        public Basket Basket { get; set; }
        public int BasketId { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
