using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.ProductService;
using Microsoft.AspNetCore.Authorization;

namespace Yuhnevich_vb_lab.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")] // Добавляем политику авторизации
    public class IndexModel : PageModel
    {


        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public IList<Dish> Dish { get; set; } = new List<Dish>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNo = 1)
        {
            var response = await _productService.GetProductListAsync(null, pageNo);
            if (response.Success && response.Data != null)
            {
                Dish = response.Data.Items ?? new List<Dish>();
                CurrentPage = response.Data.CurrentPage;
                TotalPages = response.Data.TotalPages;
            }
            else
            {
                Dish = new List<Dish>();
                CurrentPage = 1;
                TotalPages = 1;
            }

            Console.WriteLine($"IndexModel.OnGetAsync: PageNo={pageNo}, Dish count={Dish.Count}, CurrentPage={CurrentPage}, TotalPages={TotalPages}");
        }
    }
}