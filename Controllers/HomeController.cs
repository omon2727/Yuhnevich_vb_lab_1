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
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // HomeController.cs
        public async Task<IActionResult> Index(string? categoryNormalizedName, int pageNo = 1)
        {
            Console.WriteLine($"HomeController.Index: Category={categoryNormalizedName}, PageNo={pageNo}");

            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                Console.WriteLine($"Category error: {categoryResponse.ErrorMessage ?? "Data is null"}");
                ViewData["Categories"] = new SelectList(new List<Category>(), "NormalizedName", "Name");
            }
            else
            {
                ViewData["Categories"] = new SelectList(categoryResponse.Data, "NormalizedName", "Name", categoryNormalizedName);
            }
            ViewData["CurrentCategory"] = categoryNormalizedName;

            var productResponse = await _productService.GetProductListAsync(categoryNormalizedName, pageNo);
            Console.WriteLine($"Product response: Success={productResponse.Success}, ErrorMessage={productResponse.ErrorMessage}, ItemsCount={productResponse.Data?.Items?.Count ?? 0}, CurrentPage={productResponse.Data?.CurrentPage ?? 0}, TotalPages={productResponse.Data?.TotalPages ?? 0}");
            if (!productResponse.Success || productResponse.Data == null)
            {
                Console.WriteLine($"Product error: {productResponse.ErrorMessage}");
                return View(new ListModel<Dish> { Items = new List<Dish>() });
            }

            return View(productResponse.Data);
        }
    }
}