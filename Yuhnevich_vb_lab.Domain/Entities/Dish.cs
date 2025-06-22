using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuhnevich_vb_lab.Domain.Entities
{
   public class Dish:Entity
    {
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public Category? Category { get; set; }
    }
}
