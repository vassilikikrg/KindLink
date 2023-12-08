using Microsoft.AspNetCore.Identity;

namespace VolunteeringApp.Models
{
    public class AppIdentityUser : IdentityUser
    {
        public string? Description { get; set; }
    }
}
