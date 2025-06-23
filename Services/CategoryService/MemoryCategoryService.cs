using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;

namespace Yuhnevich_vb_lab.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Стартеры", NormalizedName = "starters", Dishes = new List<Dish>() },
                new Category { Id = 2, Name = "Салаты", NormalizedName = "salads", Dishes = new List<Dish>() },
                new Category { Id = 3, Name = "Основные блюда", NormalizedName = "main-dishes", Dishes = new List<Dish>() },
                new Category { Id = 4, Name = "Десерты", NormalizedName = "desserts", Dishes = new List<Dish>() },
                new Category { Id = 5, Name = "Напитки", NormalizedName = "drinks", Dishes = new List<Dish>() }
            };

            var result = new ResponseData<List<Category>>
            {
                Data = categories,
                Success = true,
                ErrorMessage = null
            };

            return Task.FromResult(result);
        }
    }
}
