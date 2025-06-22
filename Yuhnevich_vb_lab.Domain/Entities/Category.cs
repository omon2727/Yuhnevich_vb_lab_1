using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuhnevich_vb_lab.Domain.Entities
{
    public class Category:Entity
    {
      
       
        public string NormalizedName { get; set; }

        public List<Dish> Dishes { get; set; }
    }
}
