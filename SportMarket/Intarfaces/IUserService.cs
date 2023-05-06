using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportMarket.Enums;
using SportMarket.Models;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Intarfaces
{
    public interface IUserService
    {
        public User GetInfoFromGoogle(ExternalLoginInfo info);
        public Task Edit(EditViewModel model, User user, IPasswordHasher<User> _passwordHasher);
        public Task<LikeResponse> AddLikeAsync(User user, int productId);
        public Task DeleteLikeAsync(int likeId);
        public Task DeleteLikesAsync(User user);
        public Task<BasketResponse> AddToBasket(string userIdentity, int productId, int count);
        public Task AddToBasket(int count, int basketId, int productId);
        public Task<Basket> GetBasketAsync(string userIdentity);
        public Task<List<OrderedBasket>> GetOrdersAsync(string userIdentity);
        public Task<int> DeleteFromBasketAsync(int cardId);
        public Task<User> GetAsync(string userIdentity);
        public Task<UsersViewModel> GetAsync(List<string> selectRoles, string title, string email,
            UsersSortState sortOrder = UsersSortState.NameAsc);
        public Task Checkout(OrderedBasket model);
        public Task EditStatus(StatusBasket selectedStatus, int basketId);
    }
}
