using Microsoft.AspNetCore.Authorization;
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
        private readonly SignInManager<AppIdentityUser> _signInManager;

        public OrganizationController(ApplicationDbContext context,UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Organization
        public async Task<IActionResult> Index(string searchString,string typeFilter, int? pageNumber)
        {
            if (_context.Organizations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Organizations' is null.");
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
            var loggedInId = _userManager.GetUserId(User);
            if (id == null)
            {
                // If id is null, try to get the ID from the current user
                id = loggedInId;
                if (id == null)
                {
                    // If the ID is still null, return NotFound
                    return NotFound();
                }
            }
            if (id != loggedInId)
            {   // If the logged-in user is not the owner 
                ViewBag.isTheProfileOwner = false;
                if (FollowRelationshipExists(loggedInId, id))
                {
                    ViewBag.isFollowing = true;
                }
                else
                {
                    ViewBag.isFollowing = false;
                }
            }
            else
            {
                ViewBag.isTheProfileOwner = true;
            }

            var organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organization == null)
            {
                return NotFound();
            }
            int followerNumber= await _context.FollowRelationships.Where(f=>f.FollowedId==id).CountAsync();
            ViewBag.followerNumber = followerNumber;

            var posts=await _context.Posts.Where(p=>p.AuthorId==id).ToListAsync();
            var events=await _context.Events.Where(e=>e.OrganizerId==id).ToListAsync();
            ViewData["Posts"] = posts;
            ViewData["Events"] = events;
            return View(organization);
        }

        [Authorize(Roles = "Organization")]
        // GET: Organization/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Retrieve the ID of the currently logged-in user
            var loggedInUserId = _userManager.GetUserId(User);

            // Retrieve the profile from the database
            var profile = await _context.Organizations.FindAsync(id);

            // Check if the profile exists 
            if (profile == null)
            {
                // If the profile doesn't exist return a 404 Not Found
                return NotFound();
            }
            // Check if the logged-in user is the owner
            if (profile.Id != loggedInUserId)
            {   // If the logged-in user is not the owner return an access denied response
                return Forbid();
            }

            // If the logged-in user is the owner, allow them to edit the profile
            return View(profile);
        }

        // POST: Organization/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,OfficialName,PhoneNumber,Website,OrganizationType,Description")] Organization organization)
        {
            if (id != organization.Id)
            {
                return NotFound();
            }

            //// Check if email or password is null
            //if (string.IsNullOrWhiteSpace(organization.UserName))
            //{
            //    ModelState.AddModelError("UserName", "The username is required");
            //}

            //if (string.IsNullOrWhiteSpace(organization.Email))
            //{
            //    ModelState.AddModelError("Email", "The email is required");
            //}

            //// If email or password is null, return to the view with errors
            //if (!ModelState.IsValid)
            //{
            //    return View(organization);
            //}

            // Retrieve the ID of the currently logged-in user
            var loggedInUserId = _userManager.GetUserId(User);

            // Check if the logged-in user is the owner of the profile and can apply changes
            if (organization.Id != loggedInUserId)
            {
                // If the logged-in user is not the owner, return an access denied response
                return Forbid();
            }

            //// Check if the entered username is already in use by another user
            //var existingUserWithSameUsername = await _userManager.FindByNameAsync(organization.UserName);
            //if (existingUserWithSameUsername != null && existingUserWithSameUsername.Id != organization.Id)
            //{
            //    ModelState.AddModelError("UserName", "The username is already in use by another user");
            //}

            //// Check if the entered email is already in use
            //var existingUserWithSameEmail = await _userManager.FindByEmailAsync(organization.Email);
            //if (existingUserWithSameEmail != null && existingUserWithSameEmail.Id != organization.Id)
            //{
            //    ModelState.AddModelError("Email", "The email is already in use");
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the profile from the database
                    var existingOrganization = await _context.Organizations.FindAsync(id);

                    // Check if the profile exists
                    if (existingOrganization == null)
                    {
                        return NotFound();
                    }

                    // Update the existing Organization entity with the new values
                    existingOrganization.UserName = organization.UserName;
                    existingOrganization.Email = organization.Email;
                    existingOrganization.OfficialName = organization.OfficialName;
                    existingOrganization.PhoneNumber = organization.PhoneNumber; // Make sure PhoneNumber is properly bound
                    existingOrganization.Website = organization.Website;
                    existingOrganization.OrganizationType = organization.OrganizationType;
                    existingOrganization.Description = organization.Description;

                    // Update the entity in the database
                    _context.Update(existingOrganization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(organization.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = organization.Id });
            }
            return View(organization);
        }




        // GET: Organization/Delete/5
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Retrieve the ID of the currently logged-in user
            var loggedInUserId = _userManager.GetUserId(User);

            // Check if the logged-in user is the owner of the profile and can delete the account
            if (id != loggedInUserId)
            {
                // If the logged-in user is not the owner, return an access denied response
                return Forbid();
            }

            var rolesForUser = await _userManager.GetRolesAsync(user);
            var logins = await _userManager.GetLoginsAsync(user);
            IdentityResult result = IdentityResult.Success;

            foreach (var login in logins)
            {
                result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                if (result != IdentityResult.Success)
                    break;
            }
            if (rolesForUser.Count > 0)
            {
                foreach (var role in rolesForUser)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        // Handle role removal failure
                        break;
                    }
                }
            }

            var resultDelete = await _userManager.DeleteAsync(user);
            if (resultDelete.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Handle user deletion failure
                return RedirectToAction("Organization", "Delete");
            }
        }



        private bool OrganizationExists(string id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
        private bool FollowRelationshipExists(string followerId, string followedId)
        {
            return _context.FollowRelationships.Any(e => e.FollowerId == followerId && e.FollowedId == followedId);
        }
    }
}