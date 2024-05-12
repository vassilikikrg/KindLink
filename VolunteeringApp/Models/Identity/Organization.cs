using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Enums;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.Models.Identity
{
    [Table("Organizations")]
    public class Organization : AppIdentityUser
    {
        public string OfficialName { get; set; }

        public OrganizationType OrganizationType { get; set; }

        public string? Website { get; set; }
        public ICollection<Post> Posts { get; } = [];
        public ICollection<Event> Events { get; } = [];
    }
}
