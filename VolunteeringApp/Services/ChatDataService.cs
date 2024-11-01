﻿using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Chat;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Services
{
    public class ChatDataService
    {
        private readonly ApplicationDbContext _context;

        public ChatDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveMessageAsync(string senderId, string conversationId, string message)
        {
            var newMessage = new Message
            {
                SenderId = senderId,
                Text = message,
                ConversationId = conversationId,
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
        }

        public async Task<string> CreateConversationAsync(List<AppIdentityUser> groupMembers)
        {
            var conversation = new Conversation();
            foreach (var member in groupMembers)
            {
                conversation.GroupMembers.Add(new GroupMember { UserId = member.Id,User=member });
            }
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            // Return the conversation ID
            return conversation.Id;
        }
        public async Task AddGroupMemberAsync(string userId, string conversationId)
        {
            // Find the conversation and the user
            var conversation = await _context.Conversations.FindAsync(conversationId);
            var user = await _context.Users.FindAsync(userId);

            if (conversation == null || user == null)
            {
                // Either the conversation or the user was not found
                throw new InvalidOperationException("Conversation or user not found.");
            }

            var member = new GroupMember
            {
                ConversationId = conversationId,
                UserId = userId
            };
            await _context.GroupMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveGroupMemberAsync(string userId, string conversationId)
        {
            // Find the conversation and the member
            var conversation = await _context.Conversations.FindAsync(conversationId);
            var member = await _context.GroupMembers.FindAsync(userId, conversationId);

            if (conversation == null || member == null)
            {
                // Either the conversation or the member was not found
                throw new InvalidOperationException("Conversation or member not found.");
            }

            // Remove the member from the conversation
            conversation.GroupMembers.Remove(member);
            _context.GroupMembers.Remove(member);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<Conversation?> FindConversationWithExactMembers(List<AppIdentityUser> users)
        {
            // Retrieve the conversation IDs of all conversations involving the users
            var conversationIds = await _context.GroupMembers
                .Where(gm => users.Select(u => u.Id).Contains(gm.UserId))
                .GroupBy(gm => gm.ConversationId)
                .Where(g => g.Count() == users.Count)
                .Select(g => g.Key)
                .ToListAsync();
            // Check if there is any conversation that includes only the users
            foreach (var conversationId in conversationIds)
            {
                var conversation = await _context.Conversations
                    .Include(c => c.GroupMembers)
                    .FirstOrDefaultAsync(c => c.Id == conversationId);

                if (conversation != null && conversation.GroupMembers.Count == users.Count)
                {
                    var conversationUsers = conversation.GroupMembers.Select(gm => gm.UserId).ToList();
                    if (users.All(u => conversationUsers.Contains(u.Id)))
                    {
                        return conversation;
                    }
                }
            }

            return null;
        }



    }
}
