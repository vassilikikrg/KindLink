using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Social
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id {  get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string AuthorId { get; set; } // Required foreign key property
        [ValidateNever]
        public Organization Author { get; set; } = null!; // Required reference navigation to principal

        public DateTime? CreatedAt { get; set; }
        public Post()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
