using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.Chat
{
    public class ChatRoom
    {
        public ChatRoom()
        {
        }
        public ChatRoom(List<AppIdentityUser> members,string roomName)
        {
            this.members = members;
            this.roomName = roomName;
        }
        public List<AppIdentityUser>  members { get; set; }
        public string roomName { get; set; } = string.Empty;
        
    }
}
