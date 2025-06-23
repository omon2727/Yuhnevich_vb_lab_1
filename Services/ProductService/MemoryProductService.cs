using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.Services
{
    public class MemoryProductService : IProductService
    {
        private readonly List<Dish> _dishes;
        private readonly List<Category> _categories;
        private readonly int _pageSize;

        public MemoryProductService(
            [FromServices] IConfiguration config,
            ICategoryService categoryService)
        {
            _categories = categoryService.GetCategoryListAsync().Result.Data;
            _pageSize = config.GetValue<int>("ItemsPerPage", 3);

            // Инициализация _dishes в конструкторе
            _dishes = new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Суп-харчо",
                    Description = "Острый грузинский суп с говядиной и рисом",
                    Price = 250.00m,
                    Image = "/images/soup-kharcho.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("starters"))?.Id ?? 1
                },
                new Dish
                {
                    Id = 2,
                    Name = "Цезарь",
                    Description = "Салат с курицей, пармезаном и крутонами",
                    Price = 350.00m,
                    Image = "/images/caesar-salad.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("salads"))?.Id ?? 2
                },
                new Dish
                {
                    Id = 3,
                    Name = "Стейк рибай",
                    Description = "Сочный стейк из мраморной говядины",
                    Price = 1200.00m,
                    Image = "/images/ribeye-steak.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("main-dishes"))?.Id ?? 3
                },
                new Dish
                {
                    Id = 4,
                    Name = "Чизкейк",
                    Description = "Классический десерт с ягодным соусом",
                    Price = 300.00m,
                    Image = "/images/cheesecake.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("desserts"))?.Id ?? 4
                },
                new Dish
                {
                    Id = 5,
                    Name = "Лимонад",
                    Description = "Освежающий напиток с лимоном и мятой",
                    Price = 150.00m,
                    Image = "/images/lemonade.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("drinks"))?.Id ?? 5
                }
            };
        }

        public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var query = _dishes.AsQueryable();

            if (!string.IsNullOrEmpty(categoryNormalizedName))
            {
                var category = _categories.FirstOrDefault(c => c.NormalizedName.Equals(categoryNormalizedName, StringComparison.OrdinalIgnoreCase));
                if (category != null)
                {
                    query = query.Where(d => d.CategoryId == category.Id);
                }
                else
                {
                    return Task.FromResult(new ResponseData<ListModel<Dish>>
                    {
                        Success = false,
                        ErrorMessage = $"Category '{categoryNormalizedName}' not found.",
                        Data = new ListModel<Dish> { Items = new List<Dish>() }
                    });
                }
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / _pageSize);
            pageNo = Math.Max(1, Math.Min(pageNo, totalPages));

            var items = query
                .Skip((pageNo - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            var model = new ListModel<Dish>
            {
                Items = items ?? new List<Dish>(),
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            var result = new ResponseData<ListModel<Dish>>
            {
                Data = model,
                Success = true,
                ErrorMessage = null
            };

            return Task.FromResult(result);
        }

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}