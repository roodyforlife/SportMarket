using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
