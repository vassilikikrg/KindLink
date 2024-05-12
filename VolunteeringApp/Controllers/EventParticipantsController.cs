using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.Controllers
{
    public class EventParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;


        public EventParticipantsController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EventParticipants
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var @event = _context.Events.Find(id);
            if (id == null || @event == null)
            {
                return NotFound();
            }
            ViewBag.Event=@event;
            var applicationDbContext = _context.Participants
                .Where(e=>e.EventId==id)
                .Include(e => e.Citizen)
                .Include(e => e.Event);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET
        [HttpGet,ActionName("Me")]
        public async Task<IActionResult> GetMyEvents()
        {
            // Get the id of the current user
            string participantId = _userManager.GetUserId(User);

            var applicationDbContext = _context.Participants
                .Where(e => e.CitizenId == participantId)
                .Include(e => e.Event);
            return View("MyEvents",await applicationDbContext.ToListAsync());
        }

        // POST: Events/Join/5
        [Authorize(Roles ="Citizen")]
        [HttpPost, ActionName("Join")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinEvent(int id)
        {

            // Check if the provided id is null or empty, or if a user with that id exists
            if (id == null || _context.Events.Find(id) == null)
            {
                return NotFound();
            }

            // Get the id of the current user
            string participantId = _userManager.GetUserId(User);

            // Check if a follow relationship already exists between the current user and the target user
            if (!hasJoinedEvent(participantId, id))
            {
                // Create a new follow relationship
                EventParticipant participant = new EventParticipant()
                {
                    CitizenId = participantId,
                    EventId = id
                };

                // Add the relationship to the database
                _context.Add(participant);
                await _context.SaveChangesAsync();
            }
            // Redirect to the organization details page
            return RedirectToAction("Details", "Events", new { id = id });
        }
        // POST
        [Authorize(Roles ="Citizen")]
        [HttpPost,ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveEvent(int id)
        {
            // Check if the provided id is null or empty, or if a user with that id exists
            if (id==null || _context.Events.Find(id) == null)
            {
                // If the id is invalid, return 404 Not Found
                return NotFound();
            }
            // Get the id of the current user
            string participantId = _userManager.GetUserId(User);
            // Check if current user is a participant of the event
            if (hasJoinedEvent(participantId, id))
            {
                var eventParticipant = _context.Participants
                    .FirstOrDefault(e => e.CitizenId == participantId && e.EventId == id);
                // Check if the relationship exists
                if (eventParticipant != null)
                {
                    // Remove the participant record from the database
                    _context.Participants.Remove(eventParticipant);
                    await _context.SaveChangesAsync();
                }
            }
            // Redirect to the organization details page
            return RedirectToAction("Details", "Events", new { id = id });
        }
        private bool hasJoinedEvent(string userId, int id)
        {
            return _context.Participants.Any(p => p.EventId == id && p.CitizenId == userId);
        }

        private bool EventParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.EventId == id);
        }
    }
}
