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
    public class IndexModel : PageModel
    {
        private readonly Yuhnevich_vb_lab.Data.DataDbContext _context;

        public IndexModel(Yuhnevich_vb_lab.Data.DataDbContext context)
        {
            _context = context;
        }

        public IList<Dish> Dish { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Dish = await _context.Dishes
                .Include(d => d.Category).ToListAsync();
        }
    }
}
