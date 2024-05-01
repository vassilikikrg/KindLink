using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Social
{
    [Table("Events")]
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string OrganizerId { get; set; } // Required foreign key property
        [ValidateNever]
        public Organization Organizer { get; set; } = null!; // Required reference navigation to principal
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Display(Name ="Maximum number of participants")]
        public int? MaxParticipants { get; set; }
        [Required]
        public string Location {  get; set; }
        [ValidateNever]
        public byte[] Image { get; set; }

        // Navigation property to represent the collection of participants
        public ICollection<EventParticipant> Participants { get; set; }
    }
}
