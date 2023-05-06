using Microsoft.AspNetCore.Identity;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public int RequiredLength { get; set; } // минимальная длина

        public CustomPasswordValidator(int length)
        {
            RequiredLength = length;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            string pattern = @"[A-Za-z0-9_]+";

            if (password.Length < RequiredLength)
            {
                errors.Add(new IdentityError
                {
                    Description = $"Мінімальна довжина паролю дорівнює {RequiredLength}",
                    Code = "Password"
                });
            }

            if (!Regex.IsMatch(password, pattern))
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль повинен бути з латинських літер, цифр та символу '_'",
                    Code = "Password"
                });
            }
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
