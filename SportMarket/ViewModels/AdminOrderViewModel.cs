using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class AdminOrderViewModel
    {
        public List<OrderedBasket> OrderedBaskets { get; set; }
        public OrdersFilterViewModel OrdersFilterViewModel { get; set; }
        public SortOrdersViewModel SortOrdersViewModel { get; set; }
    }
}
