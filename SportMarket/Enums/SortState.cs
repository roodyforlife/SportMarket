using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public enum SortState
    {
        TitleAsc,
        TitleDesc,
        CostAsc,
        CostDesc,
        DescriptionAsc,
        DescriptionDesc,
        SaleProcentAsc,
        SaleProcentDesc,
        BonusAsc,
        BonusDesc,
        DateAsc,
        DateDesc,
        QuantityAsc,
        QuantityDesc
    }
}
