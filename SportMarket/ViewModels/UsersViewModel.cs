using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class UsersViewModel
    {
        public List<UserRole> Users { get; set; }
        public UsersFilterViewModel UsersFilterViewModel { get; set; }
        public SortUserViewModel SortUserViewModel { get; set; }
    }
}
