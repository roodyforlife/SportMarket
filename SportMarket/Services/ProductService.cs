using Microsoft.EntityFrameworkCore;
using SportMarket.Data;
using SportMarket.Enums;
using SportMarket.Intarfaces;
using SportMarket.Models;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataService _dataService;
        private readonly ApplicationDbContext _db;
        public ProductService(IDataService dataService, ApplicationDbContext db)
        {
            _dataService = dataService;
            _db = db;
        }

        public async Task AddComment(Comment comment, int productId)
        {
            comment.Product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
        }

        public async Task AddProduct(ProductViewModel model)
        {
            Product product = new Product() {
                Title = model.Title,
                Description = model.Title,
                SaleProcent = model.SaleProcent,
                BonusNumber = model.BonusNumber,
                Cost = model.Cost,
                Quantity = model.Quantity,
                Image = _dataService.ImageToByte(model.Image)
            };

            var productCategories = new List<ProductCategory>();
            foreach(var item in model.ProductCategories)
            {
                productCategories.Add(new ProductCategory() { CategoryId = item });
            }

            product.ProductCategories = productCategories;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            Product product = await Get(productId);
            _db.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(EditProductViewModel model, int productId)
        {
            Product product = await _db.Products.Include(x => x.ProductCategories).ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == productId);
            product.Title = model.Title;
            product.Description = model.Description is not null ? model.Description : product.Description;
            product.SaleProcent = model.SaleProcent;
            product.BonusNumber = model.BonusNumber;
            product.Cost = model.Cost;
            product.Quantity = model.Quantity;
            product.Image = model.Image is not null ? _dataService.ImageToByte(model.Image) : product.Image;
            product.ProductCategories.Clear();
            foreach(int i in model.ProductCategories)
            {
                product.ProductCategories.Add(new ProductCategory() { CategoryId = i });
            }

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Product>> Get()
        {
            return await _db.Products.Include(x => x.Comments).Include(x => x.ProductCategories).ThenInclude(x => x.Category).ToListAsync();
        }

        public async Task<Product> Get(int productId)
        {
            var product = await _db.Products.Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.ProductCategories).ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == productId);
            product.Comments.Reverse();
            return product;
        }

        public async Task<List<Comment>> GetCommentsAsync(int productId)
        {
            var comments = await _db.Comments.Include(x => x.Product).Include(x => x.User).Where(x => x.Product.ProductId == productId).ToListAsync();
            comments.Reverse();
            return comments;
        }

        public async Task<OrderedBasket> GetOrderAsync(int orderId)
        {
            return await _db.OrderedBaskets.Include(x => x.Basket).ThenInclude(x => x.User).Include(x => x.Basket)
                .ThenInclude(x => x.Cards).ThenInclude(x => x.Product).Include(x => x.Address)
                .ThenInclude(x => x.City).ThenInclude(x => x.State).FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task<AdminOrderViewModel> GetOrdersAsync(StatusBasket selectedStatus, int orderId, DateTime dateFrom, DateTime dateTo, 
            OrderSortState sortOrder = OrderSortState.DateAsc)
        {
            IQueryable<OrderedBasket> orders = _db.OrderedBaskets.Include(x => x.Basket).ThenInclude(x => x.Cards).ThenInclude(x => x.Product)
                .Include(x => x.Basket).ThenInclude(x => x.User);
            if (orderId != 0)
            {
                orders = orders.Where(x => x.Id == orderId);
            }

            if (selectedStatus != StatusBasket.All)
            {
                orders = orders.Where(x => x.Basket.Status == selectedStatus.ToString());
            }

            if (dateFrom.Year != 1)
            {
                orders = orders.Where(x => x.Date >= dateFrom);
            }

            orders = orders.Where(x => x.Date <= dateTo);

            switch (sortOrder)
            {
                case OrderSortState.DateDesc:
                    orders = orders.OrderByDescending(s => s.Date);
                    break;
                case OrderSortState.EmailAsc:
                    orders = orders.OrderBy(s => s.Email);
                    break;
                case OrderSortState.EmailDesc:
                    orders = orders.OrderByDescending(s => s.Email);
                    break;
                case OrderSortState.StatusAsc:
                    orders = orders.OrderBy(s => s.Basket.Status);
                    break;
                case OrderSortState.StatusDesc:
                    orders = orders.OrderByDescending(s => s.Basket.Status);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Date);
                    break;
            }

            AdminOrderViewModel viewModel = new AdminOrderViewModel
            {
                OrdersFilterViewModel = new OrdersFilterViewModel(selectedStatus, orderId, dateFrom, dateTo),
                SortOrdersViewModel = new SortOrdersViewModel(sortOrder),
                OrderedBaskets = await orders.ToListAsync()
            };

            return viewModel;
        }

        public AdminProductViewModel GetProductsAsync(List<int> selectCategories, string title, int? quantityFrom, int? quantityTo,
            SortState sortOrder)
        {
            IQueryable<Product> products = _db.Products.Include(x => x.ProductCategories).ThenInclude(x => x.Category);
            if (selectCategories != null && selectCategories.Count != 0)
            {
                products = products.Where(p => p.ProductCategories.Any(x => selectCategories.Contains(x.Category.CategoryId)));
            }

            if (!String.IsNullOrEmpty(title))
            {
                products = products.Where(x => x.Title.ToUpper().Contains(title.ToUpper()));
            }

            if (quantityFrom is not null)
            {
                products = products.Where(x => x.Quantity >= quantityFrom);
            }

            if (quantityTo is not null)
            {
                products = products.Where(x => x.Quantity <= quantityTo);
            }

            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    products = products.OrderByDescending(s => s.Title);
                    break;
                case SortState.CostAsc:
                    products = products.OrderBy(s => s.Cost);
                    break;
                case SortState.CostDesc:
                    products = products.OrderByDescending(s => s.Cost);
                    break;
                case SortState.DescriptionAsc:
                    products = products.OrderBy(s => s.Description);
                    break;
                case SortState.DescriptionDesc:
                    products = products.OrderByDescending(s => s.Description);
                    break;
                case SortState.SaleProcentAsc:
                    products = products.OrderBy(s => s.SaleProcent);
                    break;
                case SortState.SaleProcentDesc:
                    products = products.OrderByDescending(s => s.SaleProcent);
                    break;
                case SortState.BonusAsc:
                    products = products.OrderBy(s => s.BonusNumber);
                    break;
                case SortState.BonusDesc:
                    products = products.OrderByDescending(s => s.BonusNumber);
                    break;
                case SortState.DateAsc:
                    products = products.OrderBy(s => s.Date);
                    break;
                case SortState.DateDesc:
                    products = products.OrderByDescending(s => s.Date);
                    break;
                case SortState.QuantityAsc:
                    products = products.OrderBy(s => s.Quantity);
                    break;
                case SortState.QuantityDesc:
                    products = products.OrderByDescending(s => s.Quantity);
                    break;
                default:
                    products = products.OrderBy(s => s.Title);
                    break;
            }

            AdminProductViewModel viewModel = new AdminProductViewModel
            {
                FilterViewModel = new FilterViewModel(_db.Categories.ToList(), selectCategories, title),
                SortViewModel = new SortProductViewModel(sortOrder),
                Products = products.ToList()
            };

            return viewModel;
        }

        public async Task<IndexProductViewModel> GetProductsAsync(List<int> selectCategories, string title, ushort page, int? costFrom,int? costTo,
            IndexSortState sortOrder)
        {
            int pageSize = 20;
            IQueryable<Product> products = _db.Products.Include(x => x.Comments).Include(x => x.ProductCategories).ThenInclude(x => x.Category);
            if (selectCategories != null && selectCategories.Count != 0)
            {
                products = products.Where(p => p.ProductCategories.Any(x => selectCategories.Contains(x.Category.CategoryId)));
            }

            if (!String.IsNullOrEmpty(title))
            {
                products = products.Where(x => x.Title.ToUpper().Contains(title.ToUpper()));
            }

            if (costFrom is not null)
            {
                products = products.Where(x => x.Cost >= costFrom);
            }

            if (costTo is not null)
            {
                products = products.Where(x => x.Cost <= costTo);
            }

            switch (sortOrder)
            {
                case IndexSortState.TitleDesc:
                    products = products.OrderByDescending(s => s.Title);
                    break;
                case IndexSortState.CostAsc:
                    products = products.OrderBy(s => s.Cost);
                    break;
                case IndexSortState.CostDesc:
                    products = products.OrderByDescending(s => s.Cost);
                    break;
                case IndexSortState.DateAsc:
                    products = products.OrderBy(s => s.Date);
                    break;
                case IndexSortState.DateDesc:
                    products = products.OrderByDescending(s => s.Date);
                    break;
                default:
                    products = products.OrderBy(s => s.Title);
                    break;
            }

            IndexProductViewModel model = new IndexProductViewModel
            {
                PageViewModel = new PageViewModel(await products.CountAsync(), page, pageSize),
                Products = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                FilterViewModel = new FilterViewModel(await _db.Categories.ToListAsync(), selectCategories, title, costFrom, costTo, sortOrder)
            };
            return model;
        }
    }
}
