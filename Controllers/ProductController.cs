using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.Controllers
{
    [Route("Catalog")]
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
        public async Task<IActionResult> Index()
        {
            return await IndexInternal(null, 1);
        }

        [HttpGet("{pageNo:int}")] // Обрабатывает /Catalog/{pageNo}
        public async Task<IActionResult> IndexWithPageNo(int pageNo)
        {
            return await IndexInternal(null, pageNo);
        }

        [HttpGet("{category}/{pageNo:int?}")] // Обрабатывает /Catalog/{category}/{pageNo}
        public async Task<IActionResult> IndexWithCategory(string category, int pageNo = 1)
        {
            return await IndexInternal(category, pageNo);
        }

        private async Task<IActionResult> IndexInternal(string? category, int pageNo)
        {
            Console.WriteLine($"ProductController.IndexInternal: Category={category}, PageNo={pageNo}");

            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                Console.WriteLine($"Category error: {categoryResponse.ErrorMessage ?? "Data is null"}");
                ViewData["Categories"] = new SelectList(new List<Category>(), "NormalizedName", "Name");
            }
            else
            {
                ViewData["Categories"] = new SelectList(categoryResponse.Data, "NormalizedName", "Name", category);
            }
            ViewData["CurrentCategory"] = category;

            var productResponse = await _productService.GetProductListAsync(category, pageNo);
            Console.WriteLine($"Product response: Success={productResponse.Success}, ErrorMessage={productResponse.ErrorMessage}, ItemsCount={productResponse.Data?.Items?.Count ?? 0}, CurrentPage={productResponse.Data?.CurrentPage ?? 0}, TotalPages={productResponse.Data?.TotalPages ?? 0}");
            if (!productResponse.Success || productResponse.Data == null)
            {
                Console.WriteLine($"Product error: {productResponse.ErrorMessage}");
                return View("~/Views/Home/Index.cshtml", new ListModel<Dish> { Items = new List<Dish>() });
            }

            return View("~/Views/Home/Index.cshtml", productResponse.Data);
        }
    }
}