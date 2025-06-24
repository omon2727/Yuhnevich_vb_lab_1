using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Yuhnevich_vb_lab.Data;
using Yuhnevich_vb_lab.Domain.Entities;

namespace Yuhnevich_vb_lab.Areas.Admin.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly Yuhnevich_vb_lab.Data.DataDbContext _context;

        public DetailsModel(Yuhnevich_vb_lab.Data.DataDbContext context)
        {
            _context = context;
        }

        public Dish Dish { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.Id == id);

            if (dish is not null)
            {
                Dish = dish;

                return Page();
            }

            return NotFound();
        }
    }
}
