using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Enums;

namespace VolunteeringApp.Models
{
    public class Organization : AppIdentityUser
    {
        [Required]
        public string OfficialName { get; set; }

        [Required]
        public OrganizationType OrganizationType { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        //public string Location { get; set; }
    }
}
