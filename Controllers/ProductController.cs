using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.Controllers
{
    [Route("Catalog")] // Задаем базовый маршрут для контроллера
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("")] // Обрабатывает /Catalog
        [Route("{category?}")] // Обрабатывает /Catalog/{category}
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            // Получение списка категорий
            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                Console.WriteLine($"Category error: {categoryResponse.ErrorMessage ?? "Data is null"}");
                return NotFound(categoryResponse.ErrorMessage ?? "Categories not found.");
            }

            // Формирование SelectList для категорий
            var selectList = new SelectList(categoryResponse.Data, "NormalizedName", "Name", category);
            ViewData["Categories"] = selectList;

            ViewData["CurrentCategory"] = category;

            // Получение списка блюд
            var productResponse = await _productService.GetProductListAsync(category, pageNo);
            if (!productResponse.Success)
            {
                return NotFound(productResponse.ErrorMessage);
            }

            return View("~/Views/Home/Index.cshtml", productResponse.Data);
        }
    }
}