using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMarket.Data;
using SportMarket.Enums;
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
    public class AdminPanelController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly IDataService _dataService;
        private readonly ApplicationDbContext _db;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISqlService _sql;

        public AdminPanelController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService,
            IDataService dataService, ApplicationDbContext db, IProductService productService, ICategoryService categoryService, ISqlService sql)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _dataService = dataService;
            _db = db;
            _productService = productService;
            _categoryService = categoryService;
            _sql = sql;
        }

        [Authorize(Roles = "admin, manager")]
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(new ProductViewModel() { Categories = _categoryService.Get() });
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddProduct(model);
                return Redirect("../Home/Index");
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            model.Categories = _categoryService.Get();
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Products(List<int> selectCategories, string title, int? quantityFrom, int? quantityTo,
            SortState sortOrder = SortState.TitleAsc)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(_productService.GetProductsAsync(selectCategories, title, quantityFrom, quantityTo, sortOrder));
        }

        [Authorize(Roles = "admin, manager")]
        public async Task DeleteProduct(int productId)
        {
            await _productService.DeleteProduct(productId);
        }

        [HttpGet]
        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            var model = new EditProductViewModel();
            model.Product = await _productService.Get(productId);
            model.Categories = _categoryService.Get(model.Product);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductViewModel model, int productId)
        {
            if (ModelState.IsValid)
            {
                await _productService.Edit(model, productId);
                return Redirect("../AdminPanel/Products");
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            model.Product = await _productService.Get(productId);
            model.Categories = _categoryService.Get(model.Product);
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Categories()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(_db.Categories.ToList());
        }

        [Authorize(Roles = "admin, manager")]
        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            var dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryName == category.CategoryName);
            if(dbCategory is not null)
            {
                ModelState.AddModelError("CategoryName", "Категорія вже існує");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.Add(category);
                return Redirect("../AdminPanel/Categories");
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        [Authorize(Roles = "admin, manager")]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(_categoryService.Get(categoryId));
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            var dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryName == category.CategoryName);
            if (dbCategory is not null)
            {
                ModelState.AddModelError("CategoryName", "Категорія вже існує");
            }

            if (ModelState.IsValid)
            {
                _categoryService.Edit(category);
                return Redirect("../AdminPanel/Categories");
            }

            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View();
        }

        public void DeleteCategory(int categoryId)
        {
            _categoryService.Delete(categoryId);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users(string title, string email, List<string> selectRoles,
            UsersSortState sortOrder = UsersSortState.NameAsc)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(await _userService.GetAsync(selectRoles, title, email, sortOrder));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = _userManager.GetRolesAsync(user).Result.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel(user, userRoles, await _db.Roles.ToListAsync());
                ViewBag.User = new UserRole(await _userService.GetAsync(User.Identity.Name), _userManager.GetRolesAsync(user));
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, List<string> roles)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return RedirectToAction("Users");
            }

            return NotFound();
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Orders(int orderId, StatusBasket selectedStatus, DateTime dateFrom, DateTime dateTo,
            OrderSortState sortOrder = OrderSortState.DateAsc)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            dateTo = dateTo.Year == 1 ? DateTime.Now : dateTo;
            var model = await _productService.GetOrdersAsync(selectedStatus, orderId, dateFrom, dateTo, sortOrder);
            return View(model);
        }


        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Order(int orderId)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            OrderDetailsViewModel model = new OrderDetailsViewModel(await _productService.Get(), await _productService.GetOrderAsync(orderId));
            return View(model);
        }

        public async Task<IActionResult> AddToBasket(int count, int basketId, int selectedProduct, int orderId)
        {
            await _userService.AddToBasket(count, basketId, selectedProduct);
            return RedirectToAction("Order", new { orderId = orderId});
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> RemoveCard(int cardId, int orderId)
        {
            var basket = await _db.Baskets.Include(x => x.Cards).FirstOrDefaultAsync(x => x.Cards.Any(c => c.CardId == cardId));
            if (basket.Cards.Count > 1)
            {
                await _userService.DeleteFromBasketAsync(cardId);
            }

            return RedirectToAction("Order", new { orderId = orderId });
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> EditStatus(StatusBasket selectedStatus, int orderId, int basketId)
        {
            await _userService.EditStatus(selectedStatus, basketId);
            return RedirectToAction("Orders");
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> RequestUsers()
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            return View(_sql.SendRequest(@"Select TOP 10 Email, Name, Surname, Birthday, (Sum(Count)) as Кількість_замовлень from AspNetUsers
                                            join Baskets on Baskets.UserId = AspNetUsers.Id
                                            join Cards on Cards.BasketId = Baskets.BasketId
                                            Where Baskets.Status = 'Delivered'
                                            Group by Email, Name, Surname, Birthday
                                            Order by Кількість_замовлень DESC"));
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> RequestComments(string title)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            ViewBag.Title = title;
            return View(_sql.SendRequest(@$"Select Title, Surname, Name, Email, Text, Comments.Date, Score from Comments
                                        join Products on Comments.ProductId = Products.ProductId
                                        join AspNetUsers on AspNetUsers.Id = Comments.UserId
                                        Where Products.Title LIKE '%{title}%'"));
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> RequestCategories(string title)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            ViewBag.Title = title;
            return View(_sql.SendRequest(@$"Select Categories.CategoryName, SUM(Count) as Кількість_замовлень from Categories
                                                LEFT join ProductCategories on ProductCategories.CategoryId = Categories.CategoryId
                                                LEFT join Products on Products.ProductId = ProductCategories.ProductId
                                                LEFT join Cards on Cards.ProductId = Products.ProductId
                                                LEFT join Baskets on Cards.BasketId = Baskets.BasketId
												WHERE CategoryName LIKE '%{title}%'
                                                GROUP BY Categories.CategoryName
                                                ORDER BY Кількість_замовлень DESC"));
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> RequestUsersAVGCost(DateTime dateFrom, DateTime dateTo)
        {
            var user = await _userService.GetAsync(User.Identity.Name);
            ViewBag.User = new UserRole(user, _userManager.GetRolesAsync(user));
            ViewBag.DateFrom = dateFrom;
            dateTo = dateTo.Year == 1 ? DateTime.Now : dateTo;
            ViewBag.DateTo = dateTo;

            return View(_sql.SendRequest(@$"Select AspNetUsers.Email, AspNetUsers.Name, AspNetUsers.Surname, SUM(Cost * Count) / SUM(Count) as Средняя_Потраченая_сумма from AspNetUsers
                                                join Baskets on Baskets.UserId = AspNetUsers.Id
                                                join Cards on Cards.BasketId = Baskets.BasketId
                                                join OrderedBaskets on OrderedBaskets.BasketId = Baskets.BasketId
                                                join Products on Cards.ProductId = Products.ProductId
                                                Where OrderedBaskets.Date Between '{dateFrom.ToString("yyyy.MM.dd")}' And '{dateTo.ToString("yyyy.MM.dd")}'
                                                Group by AspNetUsers.Email, AspNetUsers.Name, AspNetUsers.Surname"));
        }

        [Authorize(Roles = "admin, manager")]
        public IActionResult SetSale()
        {
            _sql.SendRequest(@"Update Products
                                SET SaleProcent = 15
                                WHERE DATEDIFF(MONTH, Date, GETDATE()) > 0 AND
                                Products.Quantity > 30
                                AND Products.ProductId in (
                                SELECT Products.ProductId from Products
                                LEFT join Cards on Products.ProductId = Cards.ProductId
                                LEFT join Baskets on Baskets.BasketId = Cards.BasketId
                                LEFT join OrderedBaskets on OrderedBaskets.BasketId = Baskets.BasketId
                                WHERE (DATEDIFF(MONTH, OrderedBaskets.Date, GETDATE())) != 0
                                OR (DATEDIFF(MONTH, OrderedBaskets.Date, GETDATE())) IS NULL)");
            return RedirectToAction("Products");
        }
    }
}
