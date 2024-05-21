using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Models.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        [ValidateNever]
        public byte[]? Image { get; set; }
        public ICollection<GroupMember> GroupMembers { get; } = new List<GroupMember>();
        public ICollection<Message> Messages { get; } = new List<Message>();
    }
}
