using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuhnevich_vb_lab.Domain.Entities
{
   public class Dish:Entity
    {
        public string Description { get; set; } // описание блюда

        public int CategoryId { get; set; } // описание блюда
        public int Calories { get; set; } // кол. калорий на порцию
        public string Image { get; set; } // имя файла изображения
        public Category Category { get; set; } // Навигационные свойства
    }
}
