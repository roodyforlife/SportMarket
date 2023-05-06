using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Enums;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class OrderDetailsViewModel
    {
        public List<SelectListItem> Products { get; set; }
        public List<SelectListItem> Statuses { get; set; }
        public OrderedBasket OrderedBasket { get; set; }

        public OrderDetailsViewModel(List<Product> products, OrderedBasket orderedBasket)
        {
            OrderedBasket = orderedBasket;
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem() { Text = "Виберіть товар", Value = "0", Selected = true, Disabled = true });
            selectListItems.AddRange(products.Select(x => new SelectListItem() { Text = x.Title, Value = x.ProductId.ToString() }).ToList());
            Products = selectListItems;
            Statuses =  Enum.GetValues(typeof(StatusBasket)).Cast<StatusBasket>().Where(x => x != StatusBasket.Basket && x != StatusBasket.All)
                .Select(x => new SelectListItem
                {
                    Text = x.GetType()
            .GetMember(x.ToString())
            .FirstOrDefault()
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName(),
                    Value = x.ToString(),
                    Selected = (x.ToString() == orderedBasket.Basket.Status),
                    Disabled = (x == StatusBasket.Ordered)
                }).ToList();
        }
    }
}
