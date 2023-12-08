using Microsoft.AspNetCore.Identity;

namespace VolunteeringApp.Models.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public string? Description { get; set; }
    }
}
