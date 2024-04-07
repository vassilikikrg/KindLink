using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;

namespace VolunteeringApp.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organization
        public async Task<IActionResult> Index()
        {

            return View(await _context.Organizations.ToListAsync());
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
