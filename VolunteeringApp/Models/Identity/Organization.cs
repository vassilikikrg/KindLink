using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Enums;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.Models.Identity
{
    public class Organization : AppIdentityUser
    {
        public string OfficialName { get; set; }

        public OrganizationType OrganizationType { get; set; }

        public string? Website { get; set; }

        //public string Location { get; set; }
        public ICollection<Post> Posts { get; } = [];
    }
}
