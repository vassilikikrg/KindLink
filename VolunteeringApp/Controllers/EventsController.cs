using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using VolunteeringApp.Data;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.Social;
using VolunteeringApp.ViewModels.Social;
using Microsoft.AspNetCore.Http;

namespace VolunteeringApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;


        public EventsController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events
                .OrderBy(e=>e.StartTime)
                .Include(e => e.Organizer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
        [Authorize(Roles ="Organization")]
        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Create([Bind("Title,Description,StartTime,EndTime,MaxParticipants,Location,ImageFile")] EventViewModel eventViewModel)
        {
            if (ModelState.IsValid)
            {
                if (eventViewModel.StartTime >= eventViewModel.EndTime)
                {
                    ModelState.AddModelError("StartTime", "The event must start before it ends");
                    return View(eventViewModel);
                }
                var organizerId = _userManager.GetUserId(User); // the organizer is the current logged in user
                Event @event = new Event()
                {
                    OrganizerId = organizerId,
                    Title = eventViewModel.Title,
                    Description = eventViewModel.Description,
                    StartTime = eventViewModel.StartTime,
                    EndTime = eventViewModel.EndTime,
                    MaxParticipants = eventViewModel.MaxParticipants,
                    Location = eventViewModel.Location
                };
                if (eventViewModel.ImageFile != null && eventViewModel.ImageFile.Length > 0)
                {
                    if (IsImage(eventViewModel.ImageFile))
                    {
                        // convert image form file to byte array
                        byte[] imageData = null;
                        using (var formStream = eventViewModel.ImageFile.OpenReadStream())
                        using (var memoryStream = new MemoryStream())
                        {
                            formStream.CopyTo(memoryStream);
                            imageData = memoryStream.ToArray();
                        }
                        @event.Image = imageData;
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Please upload a photo file (accepted jpeg,png,gif)");
                        return View(eventViewModel);
                    }
                }
                await _context.AddAsync(@event); //add to db and save changes
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Check if the current user is authorized to edit this event
            if (@event.OrganizerId != _userManager.GetUserId(User))
            {//if the user is not the organizer,he can't edit the post
                return Forbid();
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrganizerId,Title,Description,StartTime,EndTime,MaxParticipants,Location,Image")] Event @event, IFormFile? ImageFile)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (@event.OrganizerId != _userManager.GetUserId(User))
            {//if the user is not the organizer,he can't edit the post
                return Forbid();
            }

            // Check if the event start time is before the end time
            if (@event.StartTime >= @event.EndTime)
            {
                ModelState.AddModelError("StartTime", "The event must start before it ends");
                return View(@event);
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    // Check if the uploaded file is an image
                    if (!IsImage(ImageFile))
                    {
                        ModelState.AddModelError("ImageFile", "Please upload a valid image file (jpeg, png, gif)");
                        return View(@event);
                    }

                    // Convert the image file to a byte array
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    @event.Image = imageData;
                }

                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }




        // GET: Events/Delete/5
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            if (@event.OrganizerId != _userManager.GetUserId(User))
            { //if the user is not the organizer,he can't delete the post
                return Forbid();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event.OrganizerId != _userManager.GetUserId(User))
            { //if the user is not the organizer,he can't delete the post
                return Forbid();
            }
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }


        // Method to check if a file is an image
        private bool IsImage(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            // Check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

    }
}
