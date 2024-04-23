using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Social
{
    [PrimaryKey(nameof(EventId), nameof(CitizenId))]
    [Table("EventParticipants")]
    public class EventParticipant
    {
        public int EventId {  get; set; }// Required foreign key property
        public Event Event { get; set; } = null!; // Required reference navigation to principal
        public string CitizenId { get; set; }// Required foreign key property
        public Citizen Citizen { get; set; } = null!; // Required reference navigation to principal
    }
}
