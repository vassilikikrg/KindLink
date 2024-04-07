using VolunteeringApp.Models.Identity;

namespace VolunteeringApp.Models.ViewModels
{
    public class UserListViewModel
    {
        public List<Citizen> Citizens { get; set; }
        public List<Organization> Organizations { get; set; }
        public List<string> SelectedCitizens { get; set; }
        public List<string> SelectedOrganizations { get; set; }
    }
}
