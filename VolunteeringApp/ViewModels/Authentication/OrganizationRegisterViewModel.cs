using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Enums;

namespace VolunteeringApp.ViewModels.Authentication
{
    public class OrganizationRegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Please provide the official name of the organization")]
        [Display(Name="Official Name")]
        public string OfficialName { get; set; }

        [Required(ErrorMessage = "Please provide the type of the organization")]
        [Display(Name = "Organization Type")]
        public OrganizationType OrganizationType { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }
        [Display(Name = "Short description")]
        public string Description { get; set; }
    }
}
