using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Controllers
{
    
    public class ChatController : Controller
    {
        ApplicationDbContext _context;
        UserManager<AppIdentityUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = this.User;
            var users=_context.Users
            .Where(u => u.Id != _userManager.GetUserId(currentUser)).ToList();
            var conversations = new List<ChatRoom>();
            foreach (var user in users)
            {
                conversations.Add(new ChatRoom(new List<AppIdentityUser>() { user }, "patatakia"));
            };
            return View(conversations);
        }

        [Authorize]
        public async Task<IActionResult> RenderChat(string receiverId)
        {
            var sender = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            // Ensure both the sender and receiver are not null
            if (sender != null && receiver != null)
            {
                var members = new List<AppIdentityUser>() { sender, receiver };
                var model = new ChatRoom { members = members, roomName = "patatakia" };

                return PartialView("_Chatroom", model); 
            }

            // Handle the case where either the current user or the receiver is not found
            return NotFound();
        }

        [Authorize]
        public IActionResult RenderMessage(string userId, string message)
        {
            var model = new Message { UserId = userId, Text = message };
            return PartialView("_Message", model);
        }

    }
}
