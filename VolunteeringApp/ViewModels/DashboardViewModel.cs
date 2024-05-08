using VolunteeringApp.Models.Identity;
using VolunteeringApp.Models.Social;

namespace VolunteeringApp.ViewModels
{
    public class DashboardViewModel
    {
        public Organization Organization { get; set; }
        public int TotalVolunteers { get; set; }
        public int TotalEvents { get; set; }
        public int TotalPosts { get; set; }
        public int TotalFollowers { get; set; }
        public List<FollowRelationship> Followers { get; set; }


    }
}
