using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.Social;
using VolunteeringApp.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VolunteeringApp.Controllers
{
    [Authorize(Roles ="Organization")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;
        public DashboardController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var loggedInId = _userManager.GetUserId(User);
            var loggedUser= _context.Organizations.Find(loggedInId);
            var totalVolunteers = _context.Participants
                .Where(p => p.Event.OrganizerId == loggedInId)
                .Select(p => p.CitizenId) // Select the UserId of participants
                .Distinct() // Remove duplicate UserIds
                .Count(); // Count the distinct UserIds

            var totalEvents = _context.Events
                .Where(e=>e.OrganizerId==loggedInId)
                .Count();
            var totalPosts = _context.Posts
                .Where(e => e.AuthorId == loggedInId)
                .Count();
            var totalfollowers = _context.FollowRelationships
                .Where(f => f.FollowedId == loggedInId)
                .Count();
            var followers = _context.FollowRelationships
                .Where(f => f.FollowedId == loggedInId)
                .Include(f => f.Follower)
                .Take(5)
                .ToList();

            DashboardViewModel dashboard = new DashboardViewModel()
            {
                Organization = loggedUser,
                TotalVolunteers = totalVolunteers,
                TotalEvents = totalEvents,
                TotalPosts = totalPosts,
                TotalFollowers=totalfollowers,
                Followers=followers
            };
            return View(dashboard);
        }
        [HttpGet]
        [Authorize(Roles = "Organization")]
        public IActionResult GetEventAttendanceData()
        {
            var currentId = _userManager.GetUserId(User);

            // Retrieve events
            var events = _context.Events
                            .Where(e => e.OrganizerId == currentId && e.StartTime<DateTime.UtcNow && e.EndTime<DateTime.UtcNow)
                            .OrderByDescending(e=>e.StartTime)
                            .Take(5)
                            .ToList();

            // Retrieve attendance counts for each event
            var attendanceCounts = events
                             .Select(e => _context.Participants.Count(p => p.EventId == e.Id))
                             .ToList();

            // Combine events and attendance counts using Zip
            var eventAttendance = new
            { 
                labels=events.Select(e=>e.Title).ToList(),
                attendance = attendanceCounts.ToList()
            };

            return Json(eventAttendance);
        }

        [HttpGet]
        [Authorize(Roles = "Organization")]
        public IActionResult GetVolunteerEngagementData()
        {
            var currentId = _userManager.GetUserId(User);
            // Retrieve volunteer engagement data from your data source (e.g., database)
            var events = _context.Events
                .Where(e => e.OrganizerId == currentId && (e.StartTime.Month > DateTime.UtcNow.Month - 6 || e.StartTime.Month <= DateTime.UtcNow.Month))
                .OrderBy(e => e.StartTime.Month)
                .Include(e => e.Participants)
                .ToList();

            var engagementData = events
                .GroupBy(e => e.StartTime.Month)
                .Select(group => new
                {
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key),
                    Sum = group.Sum(e => e.Participants.Count())
                })
                .ToList();

            var labels = engagementData.Select(d => d.MonthName).ToArray();
            var engagementValues = engagementData.Select(d => d.Sum).ToArray();

            var result = new { labels, engagementValues };

            // Return the data as JSON
            return Json(result);
        }



    }
}
