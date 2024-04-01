using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve group members where the current user is a member
            var myGroups = _context.GroupMembers
                .Where(m => m.UserId == currentUserId)
                .ToList();

            var conversationsWithMembers = new Dictionary<string, List<AppIdentityUser>>();
            // Loop through the group members
            foreach (var groupMember in myGroups)
            {
                // Get the conversation ID
                string conversationId = groupMember.ConversationId;

                // Get all members of the conversation
                var members = _context.GroupMembers
                    .Where(m => m.ConversationId == conversationId && m.UserId != currentUserId) // Exclude the current user
                    .Select(m => m.User)
                    .ToList();

                // Add the conversation and its members to the dictionary
                conversationsWithMembers.Add(conversationId, members);
            }
            return View(conversationsWithMembers);
        }
        [Authorize]
        public async Task<IActionResult> RenderChat(string id)
        {
            var sender = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByIdAsync(id);

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
        public IActionResult RenderMessage(string userId, string userName, string message, bool sent)
        {
            MessageViewModel model;
            if (userId == null)
            {
                ClaimsPrincipal currentUser = this.User;
                model = new MessageViewModel { UserId = _userManager.GetUserId(currentUser), UserName = _userManager.GetUserName(currentUser), Text = message };
            }
            else {
                model = new MessageViewModel { UserId = userId, UserName = userName, Text = message };
            }

            if (sent)
            {
                return PartialView("_SentMessage", model);
            }
            else { 
                return PartialView("_ReceivedMessage", model); 
            }
        }
        [Authorize]
        public async Task<IActionResult> Contact(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var organization = await _userManager.FindByIdAsync(id);

            if (currentUser != null && organization != null)
            {
                // Check if there is an existing conversation between the current user and the organization
                var existingConversation = _context.Conversations.FirstOrDefault(c =>
                    c.GroupMembers.Any(m => m.UserId == currentUser.Id) &&
                    c.GroupMembers.Any(m => m.UserId == organization.Id));

                if (existingConversation != null)
                {
                    // Redirect the user to the existing conversation
                    return RedirectToAction("RenderChat", new { id = organization.Id });
                }
                else
                {
                    // Create a new conversation and redirect the user to it
                    var conversation = new Conversation();
                    conversation.GroupMembers.Add(new GroupMember { UserId = currentUser.Id });
                    conversation.GroupMembers.Add(new GroupMember { UserId = organization.Id });
                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("RenderChat", new { id = organization.Id });
                }
            }

            // Handle the case where either the current user or the organization is not found
            return NotFound();
        }
    }
}
