using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VolunteeringAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(ILogger<StatisticsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("users/total")]
        public async Task<StatisticModel> GetTotalUsersStatistics() {
            var statsValue = 1500;

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
        public async Task<StatisticModel> GetTotalEventsStatistics()
        {
            var statsValue = 100;

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
