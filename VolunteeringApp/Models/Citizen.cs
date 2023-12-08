namespace VolunteeringApp.Models
{
    public class Citizen : AppIdentityUser
    {
        public string Firstname {  get; set; }
        public string Lastname { get; set; }
    }
}
