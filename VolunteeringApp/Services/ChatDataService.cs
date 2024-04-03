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
    }
}
