using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? categoryNormalizedName, int pageNo = 1)
        {
            // Получение списка категорий
            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                Console.WriteLine($"Category error: {categoryResponse.ErrorMessage ?? "Data is null"}");
                return NotFound(categoryResponse.ErrorMessage ?? "Categories not found.");
            }

            // Отладка: вывести категории в консоль
            Console.WriteLine($"Categories count: {categoryResponse.Data.Count}");
            foreach (var category in categoryResponse.Data)
            {
                Console.WriteLine($"Category: {category.Name}, NormalizedName: {category.NormalizedName}");
            }

            // Формирование SelectList
            var selectList = new SelectList(categoryResponse.Data, "NormalizedName", "Name", categoryNormalizedName);
            ViewData["Categories"] = selectList;

            // Отладка: проверить содержимое SelectList
            Console.WriteLine($"SelectList items count: {selectList.Items.Cast<SelectListItem>().Count()}");

            ViewData["CurrentCategory"] = categoryNormalizedName;

            // Получение списка блюд
            var productResponse = await _productService.GetProductListAsync(categoryNormalizedName, pageNo);
            if (!productResponse.Success)
            {
                return NotFound(productResponse.ErrorMessage);
            }

            return View("~/Views/Home/Index.cshtml", productResponse.Data);
        }
    }
}