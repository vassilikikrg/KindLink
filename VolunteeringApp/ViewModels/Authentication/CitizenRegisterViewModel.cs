using System.ComponentModel.DataAnnotations;

namespace VolunteeringApp.ViewModels.Authentication
{
    public class CitizenRegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username must contain only letters, numbers, and underscores")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [StringLength(500, ErrorMessage = "Maximum length is 500 characters")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
