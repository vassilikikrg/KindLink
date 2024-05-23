using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteeringApp.Data;

namespace VolunteeringAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly ApplicationDbContext _context;

        public StatisticsController(ILogger<StatisticsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("users/total")]
        public  StatisticModel GetTotalUsersStatistics() {
            var statsValue = _context.Users.Count();

            return new StatisticModel
            {
                Title = "Total Users",
                Description = "The total number of registered users.",
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }


        [HttpGet]
        [Route("events/total")]
        public StatisticModel GetTotalEventsStatistics()
        {
            var statsValue = _context.Events.Count();

            return new StatisticModel
            {
                Title = "Total Events",
                Description = "The total number of organized events.",
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
