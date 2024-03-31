using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace VolunteeringApp.Models.Chat
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime SentDatetime { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string ConversationId {  get; set; } // Required foreign key property
        public Conversation Conversation { get; set; } = null!; // Required reference navigation to principal



    }
}
