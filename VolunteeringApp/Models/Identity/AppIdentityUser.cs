using Microsoft.AspNetCore.Identity;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Models.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public string? Description { get; set; }
        public ICollection<GroupMember> GroupMembers { get; } = [];
        public ICollection<Message> Messages { get; } = [];
    }
}
