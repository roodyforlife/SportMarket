using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportMarket.Data;
using SportMarket.Enums;
using SportMarket.Intarfaces;
using SportMarket.Models;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportMarket.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IDataService _dataService;
        private readonly ApplicationDbContext _db;
        public UserService(UserManager<User> userManager, IDataService dataService, ApplicationDbContext db)
        {
            _userManager = userManager;
            _dataService = dataService;
            _db = db;
        }

        public async Task<LikeResponse> AddLikeAsync(User user, int productId)
        {
            var response = new LikeResponse();
            if (user is null)
            {
                return response;
            }

            var product = user.Likes.FirstOrDefault(x => x.ProductId == productId && x.User.Id == user.Id);
            response.Success = true;
            if (product is null)
            {
                user.Likes.Add(new Like() { User = user, Product = _db.Products.FirstOrDefault(x => x.ProductId == productId)});
                _db.Users.Update(user);
                response.LikeAdded = true;
            }
            else
            {
                _db.Likes.Remove(user.Likes.FirstOrDefault(x => x.ProductId == productId));
                response.LikeAdded = false;
            }

            await _db.SaveChangesAsync();
            response.Count = user.Likes.Count;
            return response;
        }

        public async Task<BasketResponse> AddToBasket(string userIdentity, int productId, int count)
        {
            var user = _userManager.Users.Include(x => x.Baskets).ThenInclude(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefault(x => x.Email == userIdentity);
            var response = new BasketResponse();
            if (user is null)
            {
                response.Success = false;
                response.Count = 0;
            }
            else
            {
                response.Success = true;
                var activeUserBasket = _db.Baskets.Include(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefault(x => x.User.Id == user.Id && x.Status == StatusBasket.Basket.ToString());
                if (activeUserBasket is null)
                {
                        Basket basket = new Basket()
                        {
                            Status = StatusBasket.Basket.ToString(),
                            UserId = user.Id,
                            Cards = new List<Card>()
                        {
                            new Card() { Count = count, Product = _db.Products.FirstOrDefault(x => x.ProductId == productId) }
                        }
                        };

                        _db.Baskets.Add(basket);
                        await _db.SaveChangesAsync();
                        response.BasketAdded = true;
                        response.Count = 1;
                }
                else
                {
                    if (activeUserBasket.Cards.Where(x => x.Product.ProductId == productId).ToList().Count != 0)
                    {
                        response.BasketAdded = false;
                    }
                    else
                    {
                        activeUserBasket.Cards.Add(new Card() { Count = count, Product = _db.Products.FirstOrDefault(x => x.ProductId == productId) });
                        _db.Baskets.Update(activeUserBasket);
                        await _db.SaveChangesAsync();
                        response.BasketAdded = true;
                        response.Count = activeUserBasket.Cards.Count;
                    }
                }
            }

            return response;
        }

        public async Task AddToBasket(int count, int basketId, int productId)
        {
            var basket = await _db.Baskets.Include(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.BasketId == basketId);
            if (!basket.Cards.Any(x => x.ProductId == productId))
            {
                Card card = new Card();
                card.BasketId = basketId;
                card.ProductId = productId;
                card.Count = count;
                await _db.Cards.AddAsync(card);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Checkout(OrderedBasket model)
        {
            model.Date = DateTime.Now;
            await _db.OrderedBaskets.AddAsync(model);
            Basket basket = await _db.Baskets.Include(x => x.User).FirstOrDefaultAsync(x => x.BasketId == model.BasketId);
            basket.Status = StatusBasket.Ordered.ToString();
            basket.OrderedBasket = model;
            if (model.Bonuses)
            {
                model.BonusesNumber = basket.User.Bonuses;
                basket.User.Bonuses = 0;
                model.BonusesWrittenOff = true;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<int> DeleteFromBasketAsync(int cardId)
        {
            var card = await _db.Cards.FirstOrDefaultAsync(x => x.CardId == cardId);
            var totalCost = _db.Baskets.Include(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefault(x => x.Cards.Any(p => p.CardId == cardId))
                .Cards.Where(x => x.CardId != cardId).Sum(p => p.Count * p.Product.Cost);

            _db.Cards.Remove(card);
            await _db.SaveChangesAsync();
            return totalCost;
        }

        public async Task DeleteLikeAsync(int likeId)
        {
            var like = _db.Likes.FirstOrDefault(x => x.LikeId == likeId);
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLikesAsync(User user)
        {
            _db.Likes.RemoveRange(user.Likes);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(EditViewModel model, User user, IPasswordHasher<User> _passwordHasher)
        {
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;
            user.Male = model.Male;
            if (user.PasswordHash is not null && model.Password is not null)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            }

            user.Image = model.Image != null ? _dataService.ImageToByte(model.Image) : user.Image;
            await _userManager.UpdateAsync(user);
        }

        public async Task EditStatus(StatusBasket selectedStatus, int basketId)
        {
            OrderedBasket orderedBasket = await _db.OrderedBaskets.Include(x => x.Basket).ThenInclude(x => x.User).Include(x => x.Basket)
                .ThenInclude(x => x.Cards).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.BasketId == basketId);
            orderedBasket.Basket.Status = selectedStatus.ToString();

            switch (selectedStatus)
            {
                case StatusBasket.Ordered:
                    if (orderedBasket.Bonuses)
                    {
                        orderedBasket.BonusesNumber = orderedBasket.Basket.User.Bonuses;
                        orderedBasket.Basket.User.Bonuses = 0;
                        orderedBasket.BonusesWrittenOff = true;
                        orderedBasket.BonusesCredited = false;
                    }
                    break;
                case StatusBasket.Processed:
                    if (!orderedBasket.QuantityWrittenOff)
                    {
                        foreach (Card card in orderedBasket.Basket.Cards)
                        {
                            card.Product.Quantity -= card.Count;
                        }

                        orderedBasket.QuantityWrittenOff = true;
                    }

                    break;
                case StatusBasket.Delivered:
                    if (!orderedBasket.BonusesCredited)
                    {
                        orderedBasket.Basket.User.Bonuses += orderedBasket.Basket.Cards.Sum(x => x.Product.BonusNumber * x.Count);
                        orderedBasket.BonusesCredited = true;
                        orderedBasket.BonusesNumber = 0;

                        if (!orderedBasket.QuantityWrittenOff)
                        {
                            foreach (Card card in orderedBasket.Basket.Cards)
                            {
                                card.Product.Quantity -= card.Count;
                            }

                            orderedBasket.QuantityWrittenOff = true;
                        }
                    }

                    break;
                case StatusBasket.Rejected:
                    if (orderedBasket.Bonuses)
                    {
                        orderedBasket.Basket.User.Bonuses = orderedBasket.BonusesNumber;
                        orderedBasket.BonusesWrittenOff = false;
                        orderedBasket.BonusesCredited = false;
                    }

                    if (orderedBasket.QuantityWrittenOff)
                    {
                        foreach (Card card in orderedBasket.Basket.Cards)
                        {
                            card.Product.Quantity += card.Count;
                        }
                    }

                    break;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<User> GetAsync(string userIdentity)
        {
            return _userManager.Users.Include(x => x.Baskets.Where(c => c.Status == StatusBasket.Basket.ToString())).ThenInclude(x => x.Cards)
                .Include(x => x.Likes).ThenInclude(x => x.Product).FirstOrDefault(x => x.Email == userIdentity);
        }

        public async Task<UsersViewModel> GetAsync(List<string> selectRoles, string title, string email,
            UsersSortState sortOrder = UsersSortState.NameAsc)
        {
            var users = _userManager.Users;

            if (!String.IsNullOrEmpty(title))
            {
                users = users.Where(x => x.Name.ToUpper().Contains(title.ToUpper()) || x.Surname.ToUpper().Contains(title.ToUpper()));
            }

            if (!String.IsNullOrEmpty(email))
            {
                users = users.Where(x => x.Email.Contains(email));
            }

            switch (sortOrder)
            {
                case UsersSortState.NameDesc:
                    users = users.OrderByDescending(s => s.Name);
                    break;
                case UsersSortState.BirthdayAsc:
                    users = users.OrderBy(s => s.Birthday);
                    break;
                case UsersSortState.SurnameDesc:
                    users = users.OrderByDescending(s => s.Surname);
                    break;
                case UsersSortState.SurnameAsc:
                    users = users.OrderBy(s => s.Surname);
                    break;
                case UsersSortState.BirthdayDesc:
                    users = users.OrderByDescending(s => s.Birthday);
                    break;
                case UsersSortState.SexAsc:
                    users = users.OrderBy(s => s.Male);
                    break;
                case UsersSortState.SexDesc:
                    users = users.OrderByDescending(s => s.Male);
                    break;
                case UsersSortState.BonusesAsc:
                    users = users.OrderBy(s => s.Bonuses);
                    break;
                case UsersSortState.BonusesDesc:
                    users = users.OrderByDescending(s => s.Bonuses);
                    break;
                case UsersSortState.TwoFactorAsc:
                    users = users.OrderBy(s => s.TwoFactorEnabled);
                    break;
                case UsersSortState.TwoFactorDesc:
                    users = users.OrderByDescending(s => s.TwoFactorEnabled);
                    break;
                case UsersSortState.RegisDateAsc:
                    users = users.OrderBy(s => s.RegistDate);
                    break;
                case UsersSortState.RegistDateDesc:
                    users = users.OrderByDescending(s => s.RegistDate);
                    break;
                case UsersSortState.EmailAsc:
                    users = users.OrderBy(s => s.Email);
                    break;
                case UsersSortState.EmailDesc:
                    users = users.OrderByDescending(s => s.Email);
                    break;
                default:
                    users = users.OrderBy(s => s.Name);
                    break;
            }

            List<UserRole> usersRoles = new List<UserRole>();
            foreach (User user in users.ToList())
            {
                usersRoles.Add(new UserRole(user, _userManager.GetRolesAsync(user)));
            }

            if (selectRoles is not null && selectRoles.Count != 0)
            {
                usersRoles = usersRoles.Where(x => x.Roles.Any(p => selectRoles.Contains(p))).ToList();
            }

            UsersViewModel model = new UsersViewModel()
            {
                Users = usersRoles,
                UsersFilterViewModel = new UsersFilterViewModel(await _db.Roles.ToListAsync(), selectRoles, title, email, sortOrder),
                SortUserViewModel = new SortUserViewModel(sortOrder)
            };

            return model;
        }

        public async Task<Basket> GetBasketAsync(string userIdentity)
        {
            return await _db.Baskets.Include(x => x.Cards).ThenInclude(x => x.Product).Include(x => x.User)
                .FirstOrDefaultAsync(x => x.User.Email == userIdentity && x.Status == StatusBasket.Basket.ToString());
        }

        public User GetInfoFromGoogle(ExternalLoginInfo info)
        {
            string picture = info.Principal.FindFirstValue("picture");
            byte[] data;
            using (WebClient webClient = new WebClient())
            {
                data = webClient.DownloadData(picture);
            }

            User user = new User
            {
                Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                Name = info.Principal.FindFirst(ClaimTypes.GivenName).Value,
                Surname = info.Principal.FindFirst(ClaimTypes.Surname).Value,
                Image  = data
            };

            return user;
        }

        public async Task<List<OrderedBasket>> GetOrdersAsync(string userIdentity)
        {
            return _db.OrderedBaskets.Include(x => x.Address).ThenInclude(x => x.City).ThenInclude(x => x.State)
                .Include(x => x.Basket).ThenInclude(x => x.Cards).ThenInclude(x => x.Product)
                .Include(x => x.Basket).ThenInclude(x => x.User).Where(x => x.Basket.User.Email == userIdentity)
                .OrderByDescending(x => x.Date).ToList();
        }
    }
}
