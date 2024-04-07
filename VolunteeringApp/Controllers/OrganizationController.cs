using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;

        public OrganizationController(ApplicationDbContext context,UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Organization
        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Organizations == null)
            {
                return Problem("Entity set 'MvcMovieContext.Organizations'  is null.");
            }

            var organizations = from o in _context.Organizations
                                select o;
            // Apply search filter
            if (!String.IsNullOrEmpty(searchString))
            {
                organizations = organizations.Where(s => s.OfficialName!.Contains(searchString) || s.UserName!.Contains(searchString));
            }

            if(User.Identity.IsAuthenticated && User.IsInRole("Organization"))
            {        
                // Exclude the current user from the organizations list
                string currentUserId = _userManager.GetUserId(User);
                organizations = organizations.Where(s=>s.Id!= currentUserId);
            }
            return View(await organizations.ToListAsync());
        }

        // GET: Organization/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationViewModel = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organizationViewModel == null)
            {
                return NotFound();
            }

            return View(organizationViewModel);
        }



        private bool OrganizationExists(string id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}