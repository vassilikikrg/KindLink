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
    [Authorize]
    public class FollowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;

        public FollowController(ApplicationDbContext context,UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Follow/GetFollowings
        public async Task<IActionResult> GetFollowings()
        {
            // Get the current user's id
            string currentId = _userManager.GetUserId(User);

            // Retrieve the follow relationships where the current user is the follower
            var applicationDbContext = _context.FollowRelationships
                .Where(f => f.FollowerId == currentId)
                .Include(f => f.Followed);

            // Return the view with the list of followings
            return View("Following", await applicationDbContext.ToListAsync());
        }

        // GET: Follow/GetFollowings
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> GetFollowers()
        {
            // Only organizations can have followers
            // Get the current user's id
            string currentId = _userManager.GetUserId(User);

            // Retrieve the follow relationships where the current user is the followed one
            var applicationDbContext = _context.FollowRelationships
                .Where(f => f.FollowedId == currentId)
                .Include(f => f.Follower);

            // Return the view with the list of followers
            return View("Follower", await applicationDbContext.ToListAsync());
        }


        // POST: Follow/Follow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string id)
        {
            // Check if the provided id is null or empty, or if a user with that id exists
            if (string.IsNullOrWhiteSpace(id) || _context.Users.Find(id) == null)
            {
                return NotFound();
            }

            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            // Get the roles of the current user
            var userRoles = await _userManager.GetRolesAsync(user);
            // Check if the user is assigned the "Citizen" role
            if (userRoles.Contains("Citizen"))
            {
                // If the user is a "Citizen", forbid following
                return Forbid();
            }
            // Get the id of the current user
            string followerId = user.Id;
            // Check if the user is trying to follow themselves
            if (followerId == id)
            {
                // If the user is trying to follow themselves, forbid the action
                return Forbid();
            }

            // Check if a follow relationship already exists between the current user and the target user
            if (FollowRelationshipExists(followerId, id))
            {
                // If a relationship already exists, redirect to the organization details page
                return RedirectToAction("Details", "Organization", new { id = id });
            }
            else
            {
                // Create a new follow relationship
                FollowRelationship relationship = new FollowRelationship()
                {
                    FollowerId = followerId,
                    FollowedId = id
                };

                // Add the relationship to the database
                _context.Add(relationship);
                await _context.SaveChangesAsync();

                // Redirect to the organization details page
                return RedirectToAction("Details", "Organization", new { id = id });
            }
        }


        // POST: Follow/Unfollow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(string id)
        {
            // Check if the provided id is null or empty, or if a user with that id exists
            if (string.IsNullOrWhiteSpace(id) || _context.Users.Find(id) == null)
            {
                // If the id is invalid, return 404 Not Found
                return NotFound();
            }
            // Get the id of the current user
            string followerId = _userManager.GetUserId(User);
            // Check if a follow relationship exists between the current user and the target user
            if (FollowRelationshipExists(followerId, id))
            {
                var followRelationship = _context.FollowRelationships.FirstOrDefault(e => e.FollowerId == followerId && e.FollowedId == id);
                // Check if the relationship exists
                if (followRelationship != null)
                {
                    // Remove the relationship from the database
                    _context.FollowRelationships.Remove(followRelationship);
                    await _context.SaveChangesAsync();
                }
            }
            // Redirect to the organization details page
            return RedirectToAction("Details", "Organization", new { id = id });
        }

        public async Task<IActionResult> RedirectToDetails(string id)
        {
            // Find the user by id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Get the roles of the user
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Citizen"))
            {
                // If the user is a "Citizen", redirect to the Citizen details page
                return RedirectToAction("Details", "Citizen", new { id = id });
            }
            else
            {
                // If the user is not a "Citizen", redirect to the Organization details page
                return RedirectToAction("Details", "Organization", new { id = id });
            }
        }

        private bool FollowRelationshipExists(string followerId,string followedId)
        {
            return _context.FollowRelationships.Any(e => e.FollowerId == followerId && e.FollowedId == followedId);
        }
    }
}
