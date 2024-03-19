using Microsoft.AspNetCore.Mvc;
using VolunteeringApp.Models.Chat;

namespace VolunteeringApp.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RenderMessage(string user, string message)
        {
            var model = new Message { UserId = user, Text = message };
            return PartialView("_Message", model);
        }
    }
}
