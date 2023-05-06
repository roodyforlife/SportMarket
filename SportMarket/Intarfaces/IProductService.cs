using SportMarket.Enums;
using SportMarket.Models;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Intarfaces
{
    public interface IProductService
    {
        public Task<List<Product>> Get();
        public Task AddProduct(ProductViewModel model);
        public Task<Product> Get(int productId);
        public Task AddComment(Comment comment, int productId);
        public Task<List<Comment>> GetCommentsAsync(int productId);
        public AdminProductViewModel GetProductsAsync(List<int> selectCategories, string title, int? quantityFrom, int? quantityTo,
            SortState sortOrder);
        public Task<IndexProductViewModel> GetProductsAsync(List<int> selectCategories, string title, ushort page, int? costFrom, int? costTo,
            IndexSortState sortOrder);
        public Task DeleteProduct(int productId);
        public Task Edit(EditProductViewModel model, int productId);
        public Task<AdminOrderViewModel> GetOrdersAsync(StatusBasket selectedStatus, int orderId, DateTime dateFrom, DateTime dateTo,
            OrderSortState sortOrder = OrderSortState.DateAsc);
        public Task<OrderedBasket> GetOrderAsync(int orderId);
    }
}
