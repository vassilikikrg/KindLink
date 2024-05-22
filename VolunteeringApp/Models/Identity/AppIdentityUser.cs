using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Models.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        [ValidateNever]
        public byte[]? Image { get; set; }
        public ICollection<GroupMember> GroupMembers { get; } = new List<GroupMember>();
        public ICollection<Message> Messages { get; } = new List<Message>();
    }
}
