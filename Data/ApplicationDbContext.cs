using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartGrievancePortal.Models;

namespace SmartGrievancePortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Academic" },
                new Category { Id = 2, Name = "Hostel" },
                new Category { Id = 3, Name = "Transport" },
                new Category { Id = 4, Name = "Maintenance" },
                new Category { Id = 5, Name = "IT Support" },
                new Category { Id = 6, Name = "Library" }
            );
        }
    }
}
