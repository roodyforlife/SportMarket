using SportMarket.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class SortOrdersViewModel
    {
        public OrderSortState EmailSort { get; private set; }
        public OrderSortState DateSort { get; private set; }
        public OrderSortState StatusSort { get; private set; }
        public OrderSortState Current { get; private set; }

        public SortOrdersViewModel(OrderSortState sortOrder)
        {
            EmailSort = sortOrder == OrderSortState.EmailAsc ? OrderSortState.EmailDesc : OrderSortState.EmailAsc;
            StatusSort = sortOrder == OrderSortState.StatusAsc ? OrderSortState.StatusDesc : OrderSortState.StatusAsc;
            DateSort = sortOrder == OrderSortState.DateAsc ? OrderSortState.DateDesc : OrderSortState.DateAsc;
            Current = sortOrder;
        }
    }
}
