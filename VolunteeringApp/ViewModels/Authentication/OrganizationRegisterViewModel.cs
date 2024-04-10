using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Enums;

namespace VolunteeringApp.ViewModels.Authentication
{
    public class OrganizationRegisterViewModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string OfficialName { get; set; }

        [Required]
        public OrganizationType OrganizationType { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

    }
}
