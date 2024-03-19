using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Controllers
{
    
    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult RenderMessage(string user, string message)
        {
            var model = new Message { UserId = user, Text = message };
            return PartialView("_Message", model);
        }
    }
}
