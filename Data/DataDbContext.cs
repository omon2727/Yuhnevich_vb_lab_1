using Microsoft.EntityFrameworkCore;

using Yuhnevich_vb_lab.Domain.Entities;

namespace Yuhnevich_vb_lab.Data
{
    public class DataDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("");
        }

        public DbSet<Dish> Dishes{get;set;}
        public DbSet<Category> Categories{get;set;}






    }
}
