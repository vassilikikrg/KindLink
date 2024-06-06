using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VolunteeringApp.Data;
using VolunteeringApp.Models;
using System;
using System.Linq;

namespace VolunteeringAPI.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public IEnumerable<StatisticModel> GetAllStatistics()
        {
            var statistics = new List<StatisticModel>
            {
                GetTotalUsersStatistics(),
                GetTotalEventsStatistics(),
                GetTotalVolunteersStatistics(),
                GetTotalOrganizationsStatistics(),
                GetAverageParticipantsPerEventStatistics(),GetAverageEventsPerOrganizationStatistics(),
            };

            return statistics;
        }

        [HttpGet]
        [Route("users/total")]
        public StatisticModel GetTotalUsersStatistics()
        {
            var statsValue = _context.Users.Count();

            return new StatisticModel
            {
                Title = "Total Users",
                Description = "The total number of registered users.",
                Type = StatisticModel.StatisticType.Count,
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
                Type = StatisticModel.StatisticType.Count,
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }

        [HttpGet]
        [Route("volunteers/total")]
        public StatisticModel GetTotalVolunteersStatistics()
        {
            var statsValue = _context.Citizens.Count();

            return new StatisticModel
            {
                Title = "Total Volunteers",
                Description = "The total number of registered volunteers.",
                Type = StatisticModel.StatisticType.Count,
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }

        [HttpGet]
        [Route("organizations/total")]
        public StatisticModel GetTotalOrganizationsStatistics()
        {
            var statsValue = _context.Organizations.Count();

            return new StatisticModel
            {
                Title = "Total Organizations",
                Description = "The total number of registered organizations.",
                Type = StatisticModel.StatisticType.Count,
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }

        [HttpGet]
        [Route("events/average-participants")]
        public StatisticModel GetAverageParticipantsPerEventStatistics()
        {
            var totalParticipants = _context.Events.Select(e => e.Participants.Count()).ToList().Sum();
            var totalEvents = _context.Events.Count();
            var averageParticipants = totalEvents == 0 ? 0 : (double)totalParticipants / totalEvents;
            var statsValue=Math.Ceiling(averageParticipants);
            return new StatisticModel
            {
                Title = "Average Participants Per Event",
                Description = "The average number of participants per event.",
                Type = StatisticModel.StatisticType.Average,
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }

        [HttpGet]
        [Route("organizations/events/average")]
        public StatisticModel GetAverageEventsPerOrganizationStatistics()
        {
            var totalOrganizations = _context.Organizations.Count();
            var totalEvents = _context.Events.Count();
            var averageEvents = totalOrganizations == 0 ? 0 : (double)totalEvents / totalOrganizations;
            var statsValue = Math.Ceiling(averageEvents);

            return new StatisticModel
            {
                Title = "Average Events Per Organization",
                Description = "The average number of events per organization.",
                Type = StatisticModel.StatisticType.Average,
                Value = statsValue,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }

    public class StatisticModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public StatisticType Type { get; set; }
        public object Value { get; set; }
        public DateTime GeneratedAt { get; set; }

        public enum StatisticType
        {
            Count,
            Sum,
            Average,
            Percentage,
            List,
            Other
        }
    }
}
