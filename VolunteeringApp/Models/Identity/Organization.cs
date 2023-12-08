using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Enums;

namespace VolunteeringApp.Models.Identity
{
    public class Organization : AppIdentityUser
    {
        public string OfficialName { get; set; }

        public OrganizationType OrganizationType { get; set; }

        public string? Phone { get; set; }

        public string? Website { get; set; }

        //public string Location { get; set; }
    }
}
