using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class ChangeRoleViewModel
    {
        public User User { get; set; }
        public List<SelectListItem> AllRoles { get; set; }
        public List<string> UserRoles { get; set; }
        public ChangeRoleViewModel(User user, List<string> userRoles, List<IdentityRole> roles)
        {
            User = user;
            AllRoles = roles.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name, Selected = (userRoles.Contains(x.Name)) }).ToList();
        }
    }
}
