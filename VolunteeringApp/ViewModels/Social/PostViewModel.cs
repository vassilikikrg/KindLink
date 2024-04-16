using System.ComponentModel.DataAnnotations;

namespace VolunteeringApp.ViewModels.Social
{
    public class PostViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
