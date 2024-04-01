using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Chat
{
    [PrimaryKey(nameof(UserId), nameof(ConversationId))]
    [Table("GroupMembers")]
    public class GroupMember
    {
        public string UserId { get; set; }
        public AppIdentityUser User { get; set; } = null!;
        public string ConversationId { get; set; } // Required foreign key property
        public Conversation Conversation { get; set; } = null!; // Required reference navigation to principal
        public DateTime JoinedDatetime { get; set; }
        public GroupMember()
        {
            JoinedDatetime = DateTime.UtcNow; // Set the JoinedDatetime to the current UTC time when the object is created
        }
    }
}
