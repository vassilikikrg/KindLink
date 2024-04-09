using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Security.Claims;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.ViewModels;
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
        public async Task<IActionResult> CreateOrFetchPrivateChat(string id)
        {
            var senderUser = await _userManager.GetUserAsync(User); // current user, here we suppose he is the sender (as he initiates the conversation)
            var receiverUser = await _userManager.FindByIdAsync(id); // the other user

            if (senderUser != null && receiverUser != null)
            {
                // Check if there is an existing conversation between the 2 users
                var existingConversation = await _chatDataService.FindConversationWithExactMembers(new List<AppIdentityUser>() { senderUser, receiverUser });

                if (existingConversation != null)
                {
                    // Redirect the user to the existing conversation with conversationId at the top
                    return RedirectToAction("Index", new { conversationId = existingConversation.Id });
                }
                else
                {
                    // Create a new conversation and redirect the user to it
                    string conversationId = await _chatDataService.CreateConversationAsync(new List<AppIdentityUser>() { senderUser, receiverUser });
                    return RedirectToAction("Index", new { conversationId });
                }
            }

            // Handle the case where either the current user or the organization is not found
            return NotFound();
        }

        [Authorize(Roles ="Organization")]
        public IActionResult ShowCreateGroupChatForm()
        {   
            var orgId= _userManager.GetUserId(User);
            // Only organizations can create group chats with both citizens and other organizations
            var citizens = _context.Citizens.ToList();
            var organizations = _context.Organizations
                .Where(o => o.Id != orgId)
                .ToList();

            var viewModel = new UserListViewModel
            {
                Citizens = citizens,
                Organizations = organizations
            };

            return View("CreateGroupChatForm",viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> CreateGroupChatAsync(UserListViewModel model)
        {
            // Check if any citizens or organizations are selected
            if (model.SelectedCitizens.Count > 0 || model.SelectedOrganizations.Count > 0)
            {
                // Create a list to store selected users
                List<AppIdentityUser> users = new List<AppIdentityUser>();

                // Retrieve selected citizens
                foreach (var citizenId in model.SelectedCitizens)
                {
                    var citizen = await _context.Citizens.FindAsync(citizenId);
                    if (citizen != null)
                    {
                        users.Add(citizen);
                    }
                }

                // Retrieve selected organizations
                foreach (var orgId in model.SelectedOrganizations)
                {
                    var org = await _context.Organizations.FindAsync(orgId);
                    if (org != null)
                    {
                        users.Add(org);
                    }
                }

                // Add the current user to the list of users
                var currentUser = await _userManager.GetUserAsync(User);
                users.Add(currentUser);
                // Check if there is an existing conversation between the users
                var existingConversation = await _chatDataService.FindConversationWithExactMembers(users);

                if (existingConversation != null)
                {
                    // Redirect the user to the existing conversation with conversationId at the top
                    return RedirectToAction("Index", new { conversationId = existingConversation.Id });
                }
                else
                {
                    // Create a conversation with the selected users
                    string createdConversationId = await _chatDataService.CreateConversationAsync(users);
                    // Redirect to the index action with the conversation ID
                    return RedirectToAction("Index", new { convId = createdConversationId });
                }
            }

            // If no citizens or organizations are selected, redirect to the index action
            return RedirectToAction("Index");
        }


    }
}
