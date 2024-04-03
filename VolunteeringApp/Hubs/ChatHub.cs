using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VolunteeringApp.Data;
using VolunteeringApp.Models;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Services;

namespace VolunteeringApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly ChatDataService _chatDataService;
        public ChatHub(ApplicationDbContext context,ChatDataService chatDataService)
        {
            _context = context;
            _chatDataService = chatDataService;
        }
        public override Task OnConnectedAsync()
        {
            // Retrieve user.
            var user = _context.Users
                .Include(m=>m.GroupMembers)
                .SingleOrDefault(u => u.UserName == Context.User.Identity.Name);
            if (user != null) { 
                // Add to each assigned group.
                foreach (var item in user.GroupMembers)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, item.ConversationId);
                }
            }
            return base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(string conversationId, string message)
        {
            var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier); // get sender's Id
            await _chatDataService.SaveMessageAsync(senderId,conversationId,message);
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", conversationId, senderId, message);
        }

        public async Task JoinRoom(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveRoom(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }


    }
}
