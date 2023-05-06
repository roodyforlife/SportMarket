using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Enums;

namespace SportMarket.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Category> categories, List<int> selectCategories, string title, int? costFrom, int? costTo, IndexSortState sortOrder) : this(categories, selectCategories, title)
        {
            CostFrom = costFrom;
            CostTo = costTo;
            SortState = sortOrder;
        }

        public FilterViewModel(List<Category> categories, List<int> selectCategories, string title)
        {
            Categories = categories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString(), Selected = (selectCategories.Contains(x.CategoryId)) }).ToList();
            SelectedCategories = selectCategories;
            SelectedTitle = title;
        }

        public List<SelectListItem> Categories { get; private set; }
        public List<int> SelectedCategories { get; private set; }
        public string SelectedTitle { get; private set; }
        public int? CostFrom { get; private set; }
        public int? CostTo { get; private set; }
        public IndexSortState SortState { get; private set; }
    }
}
