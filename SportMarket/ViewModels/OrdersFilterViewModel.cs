using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class OrdersFilterViewModel
    {
        public OrdersFilterViewModel(StatusBasket selectedStatus, int selectdId, DateTime dateFrom, DateTime dateTo)
        {
            Statuses = Enum.GetValues(typeof(StatusBasket)).Cast<StatusBasket>().Where(x => x != StatusBasket.Basket)
                .Select(x => new SelectListItem {
                    Text = x.GetType()
            .GetMember(x.ToString())
            .FirstOrDefault()
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName(),
                    Value = x.ToString(),
                    Selected = (x == selectedStatus) }).ToList();
            SelectedStatus = selectedStatus;
            SelectedId = selectdId;
            SelectedDateFrom = dateFrom;
            SelectedDateTo = dateTo;
        }

        public List<SelectListItem> Statuses { get; private set; }
        public StatusBasket SelectedStatus { get; private set; }
        public DateTime SelectedDateFrom { get; private set; }
        public DateTime SelectedDateTo { get; private set; }
        public int SelectedId { get; set; }
    }
}
