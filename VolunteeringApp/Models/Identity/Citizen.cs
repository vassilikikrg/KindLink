namespace VolunteeringApp.Models.Identity
{
    public class Citizen : AppIdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
