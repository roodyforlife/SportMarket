using Microsoft.AspNetCore.Mvc.Rendering;
using SportMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Intarfaces
{
    public interface ICategoryService
    {
        public List<SelectListItem> Get();
        public List<SelectListItem> Get(Product product);
        public Task Add(Category category);
        public Category Get(int categoryId);
        public void Edit(Category category);
        public void Delete(int categoryId);
    }
}
