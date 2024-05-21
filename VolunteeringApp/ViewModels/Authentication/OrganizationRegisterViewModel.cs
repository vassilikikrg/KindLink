using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VolunteeringApp.Models.Enums;

namespace VolunteeringApp.ViewModels.Authentication
{
    public class OrganizationRegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username must contain only letters, numbers, and underscores")]
        [Display(Name = "Username")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please provide the official name of the organization")]
        [StringLength(100, ErrorMessage = "Official name cannot exceed 100 characters")]
        [Display(Name = "Official Name")]
        public string OfficialName { get; set; }

        [Required(ErrorMessage = "Please provide the type of the organization")]
        [Display(Name = "Organization Type")]
        public OrganizationType OrganizationType { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(100, ErrorMessage = "Website URL cannot exceed 100 characters")]
        public string Website { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Short description")]
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }

    }

}
