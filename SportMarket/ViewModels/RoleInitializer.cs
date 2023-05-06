using Microsoft.AspNetCore.Identity;
using SportMarket.Models;
using SportMarket.Intarfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportMarket.Services;
using Microsoft.AspNetCore.Hosting;

namespace SportMarket.ViewModels
{
    public class RoleInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public RoleInitializer(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "_Admin123456";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("manager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("manager"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, Name = "Administration", Image = new byte[3] { 1, 2, 3} };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
