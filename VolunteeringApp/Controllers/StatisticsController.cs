using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VolunteeringApp.ViewModels.Statistics;

namespace VolunteeringApp.Controllers
{
    public class StatisticsController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7141/api"); //base address of the API
        private readonly HttpClient _client;


        public StatisticsController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<StatisticsViewModel> model = new List<StatisticsViewModel>();
            HttpResponseMessage response =  _client.GetAsync(_client.BaseAddress+"/Statistics/GetAllStatistics").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<List<StatisticsViewModel>>(data);
            }
            return View(model);
        }
    }
}
