using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Data;
using SportMarket.Intarfaces;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _db;
        public CategoryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
        }

        public void Delete(int categoryId)
        {
            _db.Categories.Remove(_db.Categories.FirstOrDefault(x => x.CategoryId == categoryId));
            _db.SaveChanges();
        }

        public void Edit(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
        }

        public List<SelectListItem> Get()
        {
            return _db.Categories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
        }

        public List<SelectListItem> Get(Product product)
        {
            List<SelectListItem> categories = Get();
            foreach(SelectListItem item in categories)
            {
                item.Selected = (product.ProductCategories.Exists(c => c.CategoryId == Convert.ToInt32(item.Value)));
            }
            return categories;
        }

        public Category Get(int categoryId)
        {
            return _db.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        }
    }
}
