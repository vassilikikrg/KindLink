using System.ComponentModel.DataAnnotations;

namespace VolunteeringApp.Models.Enums
{
    public enum OrganizationType
    {
        [Display(Name = "Children Charity")]
        Children_Charity,
        [Display(Name = "Religious")]
        Religious,
        [Display(Name = "Disaster Response")]
        Disaster_Response,
        [Display(Name = "Environmental")]
        Environmental,
        [Display(Name = "Human Rights")]
        Human_Rights,
        [Display(Name = "Refugee Aid")]
        Refugee_Aid,
        [Display(Name = "Social Welfare")]
        Social_Welfare
    }
}
