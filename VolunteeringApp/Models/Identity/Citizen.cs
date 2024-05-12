using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteeringApp.Models.Identity
{
    [Table("Citizens")]
    public class Citizen : AppIdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
