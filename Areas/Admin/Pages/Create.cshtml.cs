using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Services.CategoryService;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.Areas.Admin.Pages.Dishes
{
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CreateModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = new Dish();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (!categoryResponse.Success || categoryResponse.Data == null || !categoryResponse.Data.Any())
            {
                ModelState.AddModelError(string.Empty, "Не удалось загрузить категории. Попробуйте позже.");
                ViewData["CategoryId"] = new SelectList(new List<Category>(), "Id", "Name");
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data, "Id", "Name");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoryResponse = await _categoryService.GetCategoryListAsync();
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data ?? new List<Category>(), "Id", "Name");
                return Page();
            }

            // Дополнительная проверка
            if (Image != null && !Image.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("Image", "Файл должен быть изображением (например, JPEG или PNG).");
                var categoryResponse = await _categoryService.GetCategoryListAsync();
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data ?? new List<Category>(), "Id", "Name");
                return Page();
            }

            var response = await _productService.CreateProductAsync(Dish, Image);
            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Ошибка при создании блюда. Попробуйте снова.");
                var categoryResponse = await _categoryService.GetCategoryListAsync();
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data ?? new List<Category>(), "Id", "Name");
                return Page();
            }

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                TempData["Warning"] = response.ErrorMessage; // Например, ошибка загрузки изображения
            }

            return RedirectToPage("./Index");
        }
    }
}