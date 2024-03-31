using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
    {
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            builder.Entity<Citizen>(entity => { entity.ToTable("Citizens"); });
            builder.Entity<Organization>(entity => { entity.ToTable("Organizations"); });
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
