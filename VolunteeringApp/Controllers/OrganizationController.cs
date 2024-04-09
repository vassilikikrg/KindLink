using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Helpers;
using VolunteeringApp.Models.Enums;
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
        public async Task<IActionResult> Index(string searchString,string typeFilter, int? pageNumber)
        {
            if (_context.Organizations == null)
            {
                return Problem("Entity set 'MvcMovieContext.Organizations'  is null.");
            }

            var organizations = from o in _context.Organizations
                                select o;
            if (searchString != null)
            {
                pageNumber = 1; //If the search string is changed during paging, the page has to be reset to 1
                ViewData["SearchString"] = searchString;
            }
            //else
            //{
            //    searchString = currentFilter; // Use currentFilter parameter to retrieve searchString
            //}

            // Apply search filter
            if (!String.IsNullOrEmpty(searchString))
            {
                organizations = organizations.Where(s => s.OfficialName!.Contains(searchString) || s.UserName!.Contains(searchString));
                ViewBag.isFiltered = true;// Indicate that filtering is applied
            }

            // Apply filter based on organization type
            if (!String.IsNullOrEmpty(typeFilter))
            {        
                // Check if the provided type corresponds to an existing OrganizationType enum value
                bool correspondsToExistingType = Enum.TryParse(typeFilter, out OrganizationType organizationType);
                if (correspondsToExistingType)
                {
                    organizations = organizations.Where(s => s.OrganizationType == organizationType);
                    ViewBag.isFiltered = true;// Indicate that filtering is applied
                    ViewBag.TypeFilter = organizationType;// Indicate that filtering is applied
                }
            }
            if (User.Identity.IsAuthenticated && User.IsInRole("Organization"))
            {        
                // Exclude the current user from the organizations list
                string currentUserId = _userManager.GetUserId(User);
                organizations = organizations.Where(s=>s.Id!= currentUserId);
            }
            // Define the page size for pagination
            int pageSize = 6;
            return View(await PaginatedList<Organization>.CreateAsync(organizations.AsNoTracking(), pageNumber ?? 1, pageSize));
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