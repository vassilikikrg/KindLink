using Microsoft.AspNetCore.Identity;

namespace VolunteeringApp.Models.Chat
{
    public class ChatRoom
    {
        public List<IdentityUser>  users { get; set; }
        public string roomName { get; set; } = string.Empty;
        
    }
}
