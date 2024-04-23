using System.ComponentModel.DataAnnotations;

namespace VolunteeringApp.ViewModels.Social
{
    public class EventViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Display(Name = "Maximum number of participants")]
        public int? MaxParticipants { get; set; }
        [Required]
        public string Location { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
