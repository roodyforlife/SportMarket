using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Enums
{
    public enum IndexSortState
    {
        [Display(Name = "По назві (A-Z)")]
        TitleAsc,
        [Display(Name = "По назві (Z-A)")]
        TitleDesc,
        [Display(Name = "Спочатку дешевше")]
        CostAsc,
        [Display(Name = "Спочатку дорожчі")]
        CostDesc,
        [Display(Name = "Спочатку старі")]
        DateAsc,
        [Display(Name = "Спочатку новинки")]
        DateDesc,
    }
}
