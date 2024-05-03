using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.Services
{
    public class SocialService
    {
        private readonly ApplicationDbContext _context;
        public SocialService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<EventParticipant>> GetJoinedEvents(string id)
        {
            var myEvents = await _context.Participants
                .Where(p => p.CitizenId == id)
                .Include(p => p.Event)
                .OrderByDescending(p => p.Event.StartTime)
                .ToListAsync();
            return myEvents;
        }
        public async Task<IEnumerable<EventParticipant>> GetUpcomingJoinedEvents(string id)
        {
            var myEvents = await _context.Participants
                .Where(p => p.CitizenId == id)
                .Include(p => p.Event)
                .OrderByDescending(p => p.Event.StartTime)
                .ToListAsync();
            return myEvents.FindAll(e => e.Event.StartTime >= DateTime.UtcNow);
        }
        public async Task<IEnumerable<EventParticipant>> GetPastJoinedEvents(string id)
        {
            var myEvents = await _context.Participants
                .Where(p => p.CitizenId == id )
                .Include(p => p.Event)
                .OrderByDescending(p => p.Event.StartTime)
                .ToListAsync();
            return myEvents.FindAll(e => e.Event.StartTime < DateTime.UtcNow);
        }
        public async Task<IEnumerable<FollowRelationship>> GetFollowing(string id)
        {
            var following = await _context.FollowRelationships
                 .Where(f => f.FollowerId == id)
                 .Include(f => f.Followed)
                 .ToListAsync();
            return following;
        }
        public async Task<IEnumerable<FollowRelationship>> GetFollowers(string id)
        {
            var following = await _context.FollowRelationships
                 .Where(f => f.FollowedId == id)
                 .Include(f => f.Follower)
                 .ToListAsync();
            return following;
        }
        public async Task<int> CountFollowers(string id)
        {
            var numFollowers = await _context.FollowRelationships
                 .Where(f => f.FollowedId == id)
                 .CountAsync();
            return numFollowers;
        }
    }
}
