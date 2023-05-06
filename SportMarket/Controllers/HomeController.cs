using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportMarket.Data;
using SportMarket.Enums;
using SportMarket.Intarfaces;
using SportMarket.Models;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SportMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<User> _userManager;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _db;
        public HomeController(ICategoryService categoryService, UserManager<User> userManager, IProductService productService,
            IUserService userService, ApplicationDbContext db)
        {
            _categoryService = categoryService;
            _userManager = userManager;
            _productService = productService;
            _userService = userService;
            _db = db;
        }

        public async Task<IActionResult> Index(List<int> selectCategories, string title, int? costFrom, int? costTo, IndexSortState sortOrder = IndexSortState.TitleAsc, ushort page = 1)
        {
            var model = await _productService.GetProductsAsync(selectCategories, title, page, costFrom, costTo, sortOrder);
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(model);
        }

        public async Task<object> AddLike(int productId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            return await _userService.AddLikeAsync(user, productId);
        }

        [Authorize]
        public async Task<IActionResult> Likes()
        {
               var user = _userManager.Users.Include(x => x.Baskets.Where(c => c.Status == StatusBasket.Basket.ToString())).ThenInclude(x => x.Cards)
                .Include(x => x.Likes).ThenInclude(x => x.Product).ThenInclude(x => x.ProductCategories)
                .ThenInclude(x => x.Category).FirstOrDefault(x => x.Email == User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(user.Likes);
        }

        [Authorize]
        public async Task<int> DeleteLike(int likeId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            await _userService.DeleteLikeAsync(likeId);
            return user.Likes.Count - 1;
        }

        [Authorize]
        public async Task DeleteLikes()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            await _userService.DeleteLikesAsync(user);
        }

        public async Task<IActionResult> GetFullProduct(int productId)
        {
            return View(await _productService.Get(productId));
        }

        public async Task<BasketResponse> AddToBasket(int productId, int count)
        {
            return await _userService.AddToBasket(User.Identity.Name, productId, count);
        }

        public async Task<IActionResult> GetBasket()
        {
            var model = await _userService.GetBasketAsync(User.Identity.Name);
            return View(model);
        }

        [Authorize]
        public async Task<int> DeleteFromBasket(int cardId) => await _userService.DeleteFromBasketAsync(cardId);

        public async Task<IActionResult> Product(int productId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(await _productService.Get(productId));
        }

        [Authorize]
        public async Task AddComment(Comment comment, int productId)
        {
            comment.UserId = _userManager.Users.FirstOrDefault(x => x.Email == User.Identity.Name).Id;
            await _productService.AddComment(comment, productId);
        }

        public async Task<IActionResult> LoadComments(int productId)
        {
            return View(await _productService.GetCommentsAsync(productId));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(int basketId)
        {
            if(basketId != 0)
            {
                ViewBag.States = new SelectList(_db.States, "Id", "Name");
                var user = await _userService.GetAsync(User.Identity.Name);
                if (user.Baskets.FirstOrDefault().BasketId != basketId)
                {
                    return Redirect("/Home/Index");
                }

                ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
                return View(new OrderedBasket() { BasketId = basketId, Basket = await _db.Baskets.Include(x => x.User)
                    .Include(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.BasketId == basketId)
                });
            }

            return Redirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderedBasket model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("CheckoutConfirm", model);
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            ViewBag.States = new SelectList(_db.States, "Id", "Name");
            return View(new OrderedBasket() { BasketId = model.BasketId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckoutConfirm(OrderedBasket model)
        {
            if (ModelState.IsValid)
            {
                model.Basket = await _db.Baskets.Include(x => x.User).Include(x => x.Cards)
                    .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.BasketId == model.BasketId);
                model.Address = await _db.Addresses.Include(x => x.City).ThenInclude(x => x.State).FirstOrDefaultAsync(x => x.Id == model.AddressId);
                return View(model);
            }

            return Redirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutConfirm(OrderedBasket model, bool success)
        {
            if (success)
            {
                await _userService.Checkout(model);
            }

            return Redirect("/Home/Index");
        }

        public async Task<IActionResult> GetCities(int id)
        {
            return PartialView(_db.Cities.Where(x => x.StateId == id));
        }

        public async Task<IActionResult> GetAddresses(int id)
        {
            return PartialView(_db.Addresses.Where(x => x.CityId == id));
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(await _userService.GetOrdersAsync(User.Identity.Name));
        }
    }
}
