using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yuhnevich_vb_lab.Data;
using Yuhnevich_vb_lab.Domain.Entities;

namespace Yuhnevich_vb_lab.Areas.Admin.Pages
{
    public class CreateModel : PageModel
    {
        private readonly Yuhnevich_vb_lab.Data.DataDbContext _context;
        public int Qty { get; set; }


        public CreateModel(Yuhnevich_vb_lab.Data.DataDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Dishes.Add(Dish);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
