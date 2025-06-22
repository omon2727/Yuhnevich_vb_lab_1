using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuhnevich_vb_lab.Domain.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;


    }
}
