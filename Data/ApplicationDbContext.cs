using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yuhnevich_vb_lab.Data;

namespace Yuhnevich_vb_lab.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Avatar).HasColumnType("BLOB");
                entity.Property(e => e.MimeType).HasColumnType("TEXT");
            });
        }
    }
}