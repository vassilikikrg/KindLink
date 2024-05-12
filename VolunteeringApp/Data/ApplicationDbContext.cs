using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
    {
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<FollowRelationship> FollowRelationships { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> Participants { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //SeedRoles(builder);
        }
        //private static void SeedRoles(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<IdentityRole>().HasData(
        //        new IdentityRole() { Name = "Organization", ConcurrencyStamp = "1",NormalizedName="ORGANIZATION"},
        //        new IdentityRole() { Name = "Citizen", ConcurrencyStamp = "2", NormalizedName = "CITIZEN" }
        //        );
        //}
    }
}
