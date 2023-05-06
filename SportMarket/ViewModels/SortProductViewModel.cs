using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class SortProductViewModel
    {
        public SortState TitleSort { get; private set; }
        public SortState CostSort { get; private set; }
        public SortState DescriptionSort { get; private set; }
        public SortState SaleProcentSort { get; private set; }
        public SortState BonusSort { get; private set; }
        public SortState DateSort { get; private set; }
        public SortState QuantitySort { get; private set; }
        public SortState Current { get; private set; }

        public SortProductViewModel(SortState sortOrder)
        {
            TitleSort = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            CostSort = sortOrder == SortState.CostAsc ? SortState.CostDesc : SortState.CostAsc;
            DescriptionSort = sortOrder == SortState.DescriptionAsc ? SortState.DescriptionDesc : SortState.DescriptionAsc;
            SaleProcentSort = sortOrder == SortState.SaleProcentAsc ? SortState.SaleProcentDesc : SortState.SaleProcentAsc;
            BonusSort = sortOrder == SortState.BonusAsc ? SortState.BonusDesc : SortState.BonusAsc;
            DateSort = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            QuantitySort = sortOrder == SortState.QuantityAsc ? SortState.QuantityDesc : SortState.QuantityAsc;
            Current = sortOrder;
        }
    }
}
