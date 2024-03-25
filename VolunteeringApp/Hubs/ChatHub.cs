using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using VolunteeringApp.Models;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessageToGroup(string roomName, string message)
        {
            var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier); // get sender's Id
            await Clients.Group(roomName).SendAsync("ReceiveMessage", senderId, message);
        }
        public async Task SendMessageToUser(string receiverId,string message)
        {
            var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier); // get sender's Id
            var senderUserName = Context.User.Identity.Name; // get sender's username
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, senderUserName, message);
        }

        public async Task JoinPrivateChatRoom(ChatRoom chatRoom)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,chatRoom.roomName);
        }

    }
}
