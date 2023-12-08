using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Models;

namespace VolunteeringApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Organization", ConcurrencyStamp = "1",NormalizedName="ORGANIZATION"},
                new IdentityRole() { Name = "Citizen", ConcurrencyStamp = "2", NormalizedName = "CITIZEN" }
                );
        }
    }
}
