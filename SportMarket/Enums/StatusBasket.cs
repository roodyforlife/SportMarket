using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Enums
{
    public enum StatusBasket
    {
        [Display(Name = "Усі")]
        All,
        [Display(Name = "Кошики")]
        Basket,
        [Display(Name = "Замовлено")]
        Ordered,
        [Display(Name = "Доставлено")]
        Delivered,
        [Display(Name = "Відхилено")]
        Rejected,
        [Display(Name = "Оброблено")]
        Processed
    }
}
