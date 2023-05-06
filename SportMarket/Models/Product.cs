using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public byte SaleProcent { get; set; }
        public int BonusNumber { get; set; }
        public int Cost { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Quantity { get; set; }
        public List<Comment> Comments { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
