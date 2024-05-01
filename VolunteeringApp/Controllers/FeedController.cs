using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;

namespace VolunteeringApp.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FeedController(ApplicationDbContext context)
        {
            _context= context;
        }
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                            .OrderByDescending(e => e.CreatedAt)
                            .Take(10)
                            .Include(p => p.Author)
                            .ToListAsync();
            var recentOrgs=await _context.Organizations
                            .OrderByDescending(e => e.CreatedAt)
                            .Take(5)
                            .ToListAsync();
            var featuredEvents = await _context.Events
                            .Where(e => e.StartTime >= DateTime.UtcNow)
                            .OrderBy(e => e.StartTime)
                            .Take(5)
                            .ToListAsync();
            ViewData["RecentOrganizations"] =recentOrgs;
            ViewData["FeaturedEvents"] = featuredEvents;
            return View(posts);
        }

    }
}
