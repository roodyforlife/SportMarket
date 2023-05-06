using Microsoft.AspNetCore.Identity;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class UserRole
    {
        public User User { get; set; }
        public List<string> Roles { get; }

        public UserRole(User user, Task<IList<string>> task)
        {
            User = user;
            if (user is not null)
            {
                Roles = task.Result.ToList();
            }
        }
    }
}
