using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class UsersFilterViewModel
    {
        public UsersFilterViewModel(List<IdentityRole> roles, List<string> selectRoles, string title, string email, UsersSortState sortOrder)
        {
            SelectedTitle = title;
            SelectedEmail = email;
            SelectedRoles = selectRoles;
            Roles = roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name, Selected = (selectRoles.Contains(x.Name)) }).ToList();
            SortState = sortOrder;
        }

        public List<SelectListItem> Roles { get; private set; }
        public List<string> SelectedRoles { get; private set; }
        public string SelectedTitle { get; private set; }
        public string SelectedEmail { get; private set; }
        public UsersSortState SortState { get; private set; }
    }
}
