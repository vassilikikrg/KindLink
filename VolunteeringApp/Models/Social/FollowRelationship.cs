using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Social
{
    [PrimaryKey(nameof(FollowerId), nameof(FollowedId))]
    [Table("FollowRelationships")]
    public class FollowRelationship
    {
        public string FollowerId { get; set; } // The user who is following
        public AppIdentityUser Follower { get; set; } // Navigation property to the follower user

        public string FollowedId { get; set; } // The user who is being followed
        public Organization Followed { get; set; } // Navigation property to the followed user
    }
}
