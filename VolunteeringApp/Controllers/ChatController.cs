using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Services;

namespace VolunteeringApp.Controllers
{
    
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ChatDataService _chatDataService;

        public ChatController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager, ChatDataService chatDataService)
        {
            _context = context;
            _userManager = userManager;
            _chatDataService = chatDataService;
        }

        [Authorize]
        public IActionResult Index(string convId = null)
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
            // Order the conversations with the specified conversation at the top
            if (!string.IsNullOrEmpty(convId) && conversationsWithMembers.ContainsKey(convId))
            {
                var conversationToDisplayAtTop = conversationsWithMembers[convId];
                conversationsWithMembers.Remove(convId);
                conversationsWithMembers.Add(convId, conversationToDisplayAtTop);
            }
            return View(conversationsWithMembers);
        }

        [Authorize]
        public async Task<IActionResult> RenderChat(string id)
        {
            if (id != null)
            {   //Find all the messages of the specified conversation
                var messages = _context.Messages
                    .Where(m => m.ConversationId == id)
                    .Include(m => m.Sender)
                    .ToList();

                var currentUserId = _userManager.GetUserId(User); // Get current user id
                var members = _context.GroupMembers
                    .Where(m => m.ConversationId == id && m.UserId != currentUserId) // Exclude the current user
                    .Select(m => m.User)
                    .ToList();

                // Add values to view bag
                ViewBag.currentUserId= currentUserId;
                ViewBag.conversationId = id;
                ViewBag.usernames = string.Join(", ", members.Select(m => m.UserName));

                return PartialView("_ChatRoom", messages);
            }

            // Handle the case where either the current user or the receiver is not found
            return NotFound();
        }

        [Authorize]
        public IActionResult RenderMessage(string conversationId, string userId, string message)
        {
            var sender= _context.Users.Find(userId);
            Message model= new Message { Text = message, SenderId = userId,Sender=sender, ConversationId = conversationId };
            if (userId == _userManager.GetUserId(User))
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
                    // Redirect the user to the existing conversation with conversationId at the top
                    return RedirectToAction("Index", new { conversationId = existingConversation.Id });
                }
                else
                {
                    // Create a new conversation and redirect the user to it
                    string conversationId = await _chatDataService.CreateConversationAsync(new List<AppIdentityUser>() { currentUser, organization });
                    return RedirectToAction("Index", new { conversationId });
                }
            }

            // Handle the case where either the current user or the organization is not found
            return NotFound();
        }
    }
}
