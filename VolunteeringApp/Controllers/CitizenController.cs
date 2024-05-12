using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Services;

namespace VolunteeringApp.Controllers
{
    public class CitizenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SocialService _socialService;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager; 

        public CitizenController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager,SocialService socialService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _socialService = socialService;
        }

        // GET: Citizen/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var loggedInId= _userManager.GetUserId(User);
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

            var citizen = await _context.Citizens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (citizen == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is the owner 
            ViewBag.isTheProfileOwner = id == loggedInId;

            var following = await _socialService.GetFollowing(id);
            var myPastEvents = await _socialService.GetPastJoinedEvents(id);
            var myUpcomingEvents = await _socialService.GetUpcomingJoinedEvents(id);
            ViewData["Following"] = following;
            ViewData["MyPastEvents"] = myPastEvents;
            ViewData["MyUpcomingEvents"] = myUpcomingEvents;
            return View(citizen);
        }

        [Authorize(Roles ="Citizen")]
        // GET: Citizen/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Retrieve the ID of the currently logged-in user
            var loggedInUserId = _userManager.GetUserId(User);

            // Retrieve the profile from the database
            var profile = await _context.Citizens.FindAsync(id);

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

        // POST: Citizen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Citizen")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Firstname,Lastname,Description")] Citizen citizen)
        {
            if (id != citizen.Id)
            {
                return NotFound();
            }

            // Retrieve the ID of the currently logged-in user
            var loggedInUserId = _userManager.GetUserId(User);

            // Check if the logged-in user is the owner of the profile and can apply changes
            if (citizen.Id != loggedInUserId)
            {
                // If the logged-in user is not the owner, return an access denied response
                return Forbid();
            }

            // Check if the entered username is already in use by another user
            var existingUserWithSameUsername = await _userManager.FindByNameAsync(citizen.UserName);
            if (existingUserWithSameUsername != null && existingUserWithSameUsername.Id != citizen.Id)
            {
                ModelState.AddModelError("UserName", "The username is already in use by another user");
            }

            // Check if the entered email is already in use
            var existingUserWithSameEmail = await _userManager.FindByEmailAsync(citizen.Email);
            if (existingUserWithSameEmail != null && existingUserWithSameEmail.Id != citizen.Id)
            {
                ModelState.AddModelError("Email", "The email is already in use");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the profile from the database
                    var existingCitizen = await _context.Citizens.FindAsync(id);

                    // Check if the profile exists
                    if (existingCitizen == null)
                    {
                        return NotFound();
                    }

                    // Update the existing citizen entity with the new values
                    existingCitizen.UserName = citizen.UserName;
                    existingCitizen.Email = citizen.Email;
                    existingCitizen.Firstname = citizen.Firstname;
                    existingCitizen.Lastname = citizen.Lastname;
                    existingCitizen.Description = citizen.Description;

                    // Update the entity in the database
                    _context.Update(existingCitizen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenExists(citizen.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = citizen.Id });
            }
            return View(citizen);
        }



        // GET: Citizen/Delete/5
        [Authorize(Roles = "Citizen")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (citizen == null)
            {
                return NotFound();
            }

            return View(citizen);
        }

        // POST: Citizen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Authorize(Roles = "Citizen")]
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
                return RedirectToAction("Citizen", "Delete");
            }
        }

        private bool CitizenExists(string id)
        {
            return _context.Citizens.Any(e => e.Id == id);
        }
    }
}
