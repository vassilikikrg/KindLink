using Microsoft.AspNetCore.SignalR;
using VolunteeringApp.Models;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string roomName, string message)
        {
            var senderUserId = Context.User.Identity.Name; // get sender's Id
            await Clients.Group(roomName).SendAsync("ReceiveMessage", senderUserId, message);
        }

        public async Task JoinPrivateChatRoom(ChatRoom chatRoom)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,chatRoom.roomName);
        }
    }
}
