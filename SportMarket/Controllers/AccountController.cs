using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMarket.Intarfaces;
using SportMarket.Models;
using SportMarket.Services;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly IDataService _dataService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService,
            IDataService dataService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _dataService = dataService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa");
                }

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Email", "Невірний логін та (або) пароль");
                }
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (_userManager.Users.FirstOrDefault(x => x.Email == model.Email) is not null)
            {
                ModelState.AddModelError("Email", "Акаунт с таким email вже існує");
            }

            if (ModelState.IsValid)
            {
                User user = new User {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    LastName = model.LastName,
                    Birthday = model.Birthday,
                    Male = model.Male,
                    Image = _dataService.ImageToByte("defaultAvatar.png")
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }

            var iddentityUser = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(iddentityUser, _userManager.GetRolesAsync(iddentityUser));
            return View(model);
        }

        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return Redirect("../Home/Index");
        }

        public IActionResult GoogleLogin(string ReturnUrl)
        {
            ReturnUrl = "/";
            string redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa");
            }

            if (result.Succeeded)
            {
                return Redirect("../Home/Index");
            }
            else
            {
                User user = _userService.GetInfoFromGoogle(info);
                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "user");
                        await _signInManager.SignInAsync(user, false);
                        return Redirect("../Home/Index");
                    }
                }
                return RedirectToAction("Registration");
            }
        }

        public async Task<IActionResult> LoginWith2fa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginWith2fa(LoginWith2fa model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(authenticatorKey.ToString(), model.Code);
            if (isValid)
            {
                await _signInManager.SignInAsync(user, true);
                return Redirect("../Home/Index");
            }

            ModelState.AddModelError("Code", "Невірний код");
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var _passwordHasher =
                HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                await _userService.Edit(model, user, _passwordHasher);
                return Redirect("../Home/Index");
            }

            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EnableGoogleAuthenticator()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            if (user is not null)
            {
                if (!user.TwoFactorEnabled)
                {
                    var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                    if (authenticatorKey == null)
                    {
                        await _userManager.ResetAuthenticatorKeyAsync(user);
                        authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                    }
                    TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                    var setupInfo = twoFactor.GenerateSetupCode("SportMarket", user.Email, authenticatorKey.ToString(), false, 3);
                    return View(new ConfigureTwoFactorAuthenticatorModel() { SetupCode = setupInfo.ManualEntryKey, BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl });
                }
            }

            return Redirect("../Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> EnableGoogleAuthenticator(ConfigureTwoFactorAuthenticatorModel model)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(authenticatorKey.ToString(), model.InputCode);
            if (isValid)
            {
                user.TwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("InputCode", "Невірний код");
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DisableGoogleAuthenticator()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            user.TwoFactorEnabled = false;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Edit");
        }
    }
}
